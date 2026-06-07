using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Unity.CodeEditor;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

/// <summary>
/// TCP daemon that keeps Unity Editor available for fast test execution.
/// Auto-starts via [InitializeOnLoad]. Python client connects on port 9876.
/// Also available via menu: Tools > Test Daemon > Start / Stop.
/// </summary>
[InitializeOnLoad]
public static class TestDaemon
{
    const int Port = 9876;
    static TcpListener s_listener;
    static Thread s_thread;
    static volatile bool s_running;
    static PendingCommand s_active; // only one command at a time
    static int s_regenFrames;
    static PendingCommand s_activeRegen;
    static bool s_compilationStarted;

    static readonly ConcurrentQueue<PendingCommand> s_queue = new();

    // ── Auto-start ──────────────────────────────────────────────
    static TestDaemon()
    {
        CompilationPipeline.compilationStarted += OnCompilationStarted;
        Start();
    }

    static void OnCompilationStarted(object obj)
    {
        s_compilationStarted = true;
    }

    [MenuItem("Tools/Test Daemon/Start")]
    public static void Start()
    {
        if (s_running) {
            Debug.Log("[TestDaemon] already running");
            return;
        }
        s_running = true;

        s_thread = new Thread(ListenLoop) { IsBackground = true, Name = "TestDaemon" };
        s_thread.Start();

        EditorApplication.update += Pump;
        Debug.Log($"[TestDaemon] Listening on port {Port}");
    }

    [MenuItem("Tools/Test Daemon/Stop")]
    public static void Stop()
    {
        if (!s_running) return;
        s_running = false;
        s_listener?.Stop();
        EditorApplication.update -= Pump;
        Debug.Log("[TestDaemon] Stopped");
    }

    // ── TCP listener (background thread) ────────────────────────

    static void ListenLoop()
    {
        s_listener = new TcpListener(IPAddress.Loopback, Port);
        try
        {
            s_listener.Start();
        }
        catch (SocketException ex)
        {
            Debug.LogWarning($"[TestDaemon] Port {Port} unavailable: {ex.Message}");
            s_running = false;
            return;
        }

        try
        {
            while (s_running)
            {
                if (!s_listener.Pending()) { Thread.Sleep(50); continue; }
                var client = s_listener.AcceptTcpClient();
                try { HandleClient(client); }
                catch (Exception ex) { Debug.LogError($"[TestDaemon] {ex.Message}"); }
                finally { client.Close(); }
            }
        }
        catch (SocketException) { /* listener stopped */ }
        finally { try { s_listener.Stop(); } catch { /* ignore */ } }
    }

    static void HandleClient(TcpClient client)
    {
        client.ReceiveTimeout = 5000;
        client.SendTimeout = 10000;
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        using var writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

        var line = reader.ReadLine();
        if (string.IsNullOrEmpty(line)) return;

        var req = JsonUtility.FromJson<Req>(line);
        Debug.Log($"[TestDaemon] ← {req.cmd}");

        // Fast-path commands (no main thread needed)
        if (req.cmd == "ping")  { Reply(writer, "ok"); return; }
        if (req.cmd == "shutdown") { Reply(writer, "ok"); Stop(); return; }

        // Queue to main thread and wait
        var pending = new PendingCommand
        {
            type = req.cmd,
            filter = req.filter,
            signal = new ManualResetEventSlim(false),
        };
        s_queue.Enqueue(pending);

        // Wait up to 10 minutes (tests can be slow)
        if (!pending.signal.Wait(TimeSpan.FromMinutes(10)))
        {
            Reply(writer, "error", message: "timeout");
            return;
        }
        writer.WriteLine(JsonUtility.ToJson(pending.response));
    }

    static void Reply(StreamWriter w, string status, string message = null)
    {
        w.WriteLine(JsonUtility.ToJson(new Res { status = status, message = message ?? "" }));
    }

    // ── Main thread pump (via EditorApplication.update) ─────────

    static void Pump()
    {
        if (s_activeRegen != null)
        {
            s_regenFrames++;
            if (EditorApplication.isCompiling)
            {
                s_compilationStarted = true;
                return;
            }

            if (s_compilationStarted)
            {
                // Compilation has finished (isCompiling is false now)
            }
            else
            {
                // Wait up to 30 frames to see if compilation starts
                if (s_regenFrames < 30)
                {
                    return;
                }
            }

            var rcmd = s_activeRegen;
            s_activeRegen = null;
            s_active = null;
            s_compilationStarted = false;

            if (EditorUtility.scriptCompilationFailed)
            {
                rcmd.response = new Res { status = "error", message = "Compilation failed in Unity Editor" };
                Debug.LogError("[TestDaemon] → regen failed: Compilation errors found.");
            }
            else
            {
                rcmd.response = new Res { status = "ok" };
                Debug.Log("[TestDaemon] → regen ok");
            }
            rcmd.signal.Set();
            return;
        }

        if (s_active != null) return;            // test or regen in progress
        if (!s_queue.TryDequeue(out var cmd)) return;

        switch (cmd.type)
        {
            case "regen":   ExecRegen(cmd); break;
            case "edit-tests": ExecTests(TestMode.EditMode, cmd); break;
            case "play-tests": ExecTests(TestMode.PlayMode, cmd); break;
            default:
                cmd.response = new Res { status = "error", message = $"unknown cmd: {cmd.type}" };
                cmd.signal.Set();
                break;
        }
    }

    static void ExecRegen(PendingCommand cmd)
    {
        try
        {
            CodeEditor.CurrentEditor.SyncAll();
            s_compilationStarted = false;
            s_regenFrames = 0;
            s_activeRegen = cmd;
            s_active = cmd; // block other commands
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
        catch (Exception ex)
        {
            cmd.response = new Res { status = "error", message = ex.Message };
            cmd.signal.Set();
        }
    }

    static void ExecTests(TestMode mode, PendingCommand cmd)
    {
        s_active = cmd;
        var api = ScriptableObject.CreateInstance<TestRunnerApi>();
        var cb = new Callbacks(cmd, api);
        api.RegisterCallbacks(cb);

        var filter = new Filter { testMode = mode };
        if (!string.IsNullOrEmpty(cmd.filter))
            filter.testNames = new[] { cmd.filter };

        api.Execute(new ExecutionSettings(filter));
        // Callbacks.RunFinished will signal cmd when done
    }

    // ── Test callbacks ──────────────────────────────────────────

    class Callbacks : ICallbacks
    {
        readonly PendingCommand _cmd;
        readonly TestRunnerApi _api;
        int _total, _failed;
        string _failNames = "";
        string _failDetails = "";

        public Callbacks(PendingCommand cmd, TestRunnerApi api)
        { _cmd = cmd; _api = api; }

        public void RunStarted(ITestAdaptor testsToRun) { }
        public void TestStarted(ITestAdaptor test) { }

        public void TestFinished(ITestResultAdaptor r)
        {
            if (r.HasChildren) return;
            _total++;
            if (r.TestStatus == TestStatus.Failed)
            {
                _failed++;
                _failNames += (string.IsNullOrEmpty(_failNames) ? "" : "|") + r.FullName;
                _failDetails += $"Test: {r.FullName}\nError: {r.Message}\nStacktrace:\n{r.StackTrace}\n---\n";
                Debug.LogWarning($"[TestDaemon] Test failed: {r.FullName}\nMessage: {r.Message}\nStacktrace: {r.StackTrace}");
            }
        }

        public void RunFinished(ITestResultAdaptor result)
        {
            _api.UnregisterCallbacks(this);
            _cmd.response = new Res
            {
                status = _failed > 0 ? "fail" : "ok",
                total = _total,
                failed = _failed,
                fail_names = _failNames,
                fail_details = _failDetails,
            };
            Debug.Log($"[TestDaemon] → tests {_cmd.response.status}: {_total} total, {_failed} failed");
            s_active = null;
            _cmd.signal.Set();
        }
    }

    // ── Data ────────────────────────────────────────────────────

    [Serializable] struct Req  { public string cmd; public string filter; }

    [Serializable]
    struct Res
    {
        public string status;
        public string message;
        public int total;
        public int failed;
        public string fail_names; // pipe-separated: "Test.A|Test.B"
        public string fail_details;
    }

    class PendingCommand
    {
        public string type;
        public string filter;
        public ManualResetEventSlim signal;
        public Res response;
    }
}
