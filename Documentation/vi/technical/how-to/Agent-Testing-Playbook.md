---
type: technical

audience: [developer, agent]
status: active
language: vi
description: Vietnamese companion guide for automated agent execution across compile, EditMode, and PlayMode test workflows.
related:
  - how_to_documentation
---

# Agent Testing Playbook

Mục tiêu: hướng dẫn cụ thể, có thể sao chép-thực thi, để một agent (hoặc developer) vận hành pipeline kiểm thử tự động cho dự án — bao gồm Tier 1 (compile), Tier 2 (EditMode) và Tier 3 (PlayMode). Tài liệu này tuân thủ quy ước `How-to-documentation.md` (Preconditions → Steps → Expected → Edge cases → Verification).

## Tiền đề
- Hệ điều hành: Windows (local); CI có thể là Linux hoặc Windows.
- `dotnet` SDK đã cài và có trong `PATH`.
- Unity Editor đã cài; biết đường dẫn `UNITY_EDITOR_PATH` (ví dụ `C:\Program Files\Unity\Hub\Editor\6000.3.11f1\Editor\Unity.exe`).
- Có quyền licence Unity trên runner nếu chạy EditMode/PlayMode trong CI.
- Thư mục repo hiện tại là root dự án (chứa `.sln`).
- Có thư mục `Assets/Tests/` với EditMode và PlayMode tests.

## Biến môi trường được sử dụng
- `UNITY_EDITOR_PATH` — đường dẫn tới `Unity.exe`.
- `PROJECT_PATH` — đường dẫn tới root project (mặc định `.`).
- `LOGS_DIR` — nơi ghi log (mặc định `Logs/`).

## Quick start (local)
1. Thiết lập biến môi trường (PowerShell):

```powershell
$env:UNITY_EDITOR_PATH = 'C:\Program Files\Unity\Hub\Editor\6000.3.11f1\Editor\Unity.exe'
$env:PROJECT_PATH = '.'
$env:LOGS_DIR = 'Logs'
```

2. Chạy pipeline nhanh (Tier1 + Tier2):

```powershell
# Lệnh ví dụ; script orchestration sẽ được tạo: tools/run_tests.ps1 --scope fast
powershell -ExecutionPolicy Bypass -File .\tools\run_tests.ps1 --scope fast
```

3. Chạy pipeline đầy đủ (bao gồm PlayMode):

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\run_tests.ps1 --scope full
```

## Các bước thực thi (chi tiết) — agent có thể gọi từng bước này

### Tier 1 — Compile check (dotnet)
Preconditions: `dotnet` sẵn sàng.

Steps:

```powershell
# Build assemblies; stop on first failure
dotnet build MvA.Core.csproj
dotnet build MvA.Entities.csproj
dotnet build MvA.Effect.csproj
# (tùy chọn) build các project khác
```

Agent action rules:
- Nếu bất kỳ `dotnet build` trả lỗi, dừng pipeline, thu `stdout/stderr`, tóm tắt lỗi compile, report cho dev.

### Tier 2 — EditMode tests (Unity CLI)
Preconditions: `UNITY_EDITOR_PATH` tồn tại và có license.

Steps:

```powershell
& "$env:UNITY_EDITOR_PATH" -projectPath "$env:PROJECT_PATH" -runTests -testPlatform EditMode -logFile "$env:LOGS_DIR/editmode-test.log" -testResults "$env:LOGS_DIR/editmode-test-results.xml"
```

Agent action rules:
- Nếu tests fail: parse `editmode-test-results.xml` (JUnit/Unity format) để lấy test names và first stack trace.
- Rerun rule: nếu chỉ 1 test fail → rerun that single test (Unity supports `-testFilter` or run filtered test runner); nếu nhiều tests fail trong cùng assembly → rerun assembly; nếu flaky (pass on rerun) → mark flaky and attempt 2 more runs before marking unstable.

### Tier 3 — PlayMode tests (Unity CLI, headless)
Preconditions: same as Tier 2, có thể lâu hơn.

Steps:

```powershell
& "$env:UNITY_EDITOR_PATH" -projectPath "$env:PROJECT_PATH" -runTests -testPlatform PlayMode -logFile "$env:LOGS_DIR/playmode-test.log" -testResults "$env:LOGS_DIR/playmode-test-results.xml"
```

Agent action rules:
- Chạy khi Tier2 pass hoặc theo cấu hình (pre-merge/full).
- Thu artifacts log và junit xml; nếu test fail, lưu snapshot scene (nếu config cho phép) và ghi thời điểm frame + component state nếu logger hỗ trợ.

## Phân tích log & báo cáo ngắn gọn (Agent responsibilities)
- Bước 1: lấy `test-results.xml` và tách test failures: test name, class, message, stacktrace.
- Bước 2: map stacktrace tới files trong repo (line numbers) và produce a short report gồm: failure summary (1-2 câu), top stackframe (file + line), suggested fix hints (compile error vs assertion vs null ref).
- Bước 3: attach artifacts to PR comment hoặc upload to CI artifacts: `editmode-test.log`, `playmode-test.log`, `*.xml`.

## Heuristics rerun / flaky handling
- Single-test fail: rerun that test up to 3 times. If 2/3 passes → mark flaky and record metadata.
- Multiple unrelated fails: attempt assembly-level rerun once; if still failing → stop and report.
- Compilation errors: do NOT run Unity tests; stop early.

## Debugging helpers (non-intrusive)
- Log checkpoints: instrument runtime code để emit structured markers (`[TESTCHK] <id> <json-metadata>`) để agent correlate logs với behavior.
- Breakpoint-equivalent: agent tạo file flag `Logs/agent_debug.json` để runtime code bật verbose logging theo nhu cầu (developer phải thêm check vào code). An toàn hơn so với attach debugger tự động.
- Attach debugger (manual): hướng dẫn dev mở VS/VSCode và attach tới Unity Editor process. Agent chỉ produce instructions và stackframe cần inspect.

## CI integration (Guidelines)
- PR pre-check (fast): run Tier1 (dotnet builds) on every push.
- PR gate (full): run Tier1 + Tier2 on PR; optionally run Tier3 on nightly hoặc trước merge vào `main` nếu license/runner có sẵn.
- Artifacts: junit xml, raw logs, short human summary trong PR check.

## Edge cases & recovery
- Unity license missing on CI: fail fast với message rõ ràng "Unity license missing — see CI docs".
- Test runner hangs: kill Unity process sau timeout (mặc định 10 phút), collect partial logs, mark as `timed_out`.
- Corrupted Library build: recovery step — delete `Library/ScriptAssemblies` và reimport; agent có thể retry 1 lần với recovery này nếu lỗi lặp lại.

## Verification
1. Smoke: `tools/run_tests.ps1 --scope fast` trả exit code 0 và tạo `Logs/editmode-test-results.xml`.
2. Fault injection: thêm test EditMode failing; agent phải detect, rerun theo rule và report failure với stackframe.
3. Full: chạy `tools/run_tests.ps1 --scope full` local với Unity licensed; verify `Logs/playmode-test-results.xml`.

## Where to find scripts
- `tools/run_tests.ps1` — orchestration (sẽ scaffold theo yêu cầu).
- `tools/agent_runner.py` — (gợi ý) parser + reporter (Python) — optional.

## Onboarding quick-checklist
1. Thiết lập `UNITY_EDITOR_PATH`.
2. Chạy `powershell -File tools/run_tests.ps1 --scope fast`.
3. Kiểm tra `Logs/` để xem `*-test-results.xml` và `*-test.log`.

---

Xem thêm: Xem [How-to-test](../../how-to/How-to-test.md)