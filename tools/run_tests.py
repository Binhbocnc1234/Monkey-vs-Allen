"""
Run tests orchestration for Monkey vs Allen.

Usage:
  python tools/run_tests.py --scope fast
  python tools/run_tests.py --scope full

Connects to the TestDaemon TCP server running inside Unity Editor (port 9876).
If Unity is not open, start it first — the daemon auto-starts via [InitializeOnLoad].

Tiers:
- Tier1: dotnet build on key projects (runs locally, no Unity needed)
- Tier2: Unity EditMode tests (via daemon)
- Tier3: Unity PlayMode tests (via daemon, --scope full only)
"""

import argparse
import json
import os
import re
import socket
import subprocess
import sys
import time
from datetime import datetime
from pathlib import Path

# ---------------------------------------------------------------------------
# Globals
# ---------------------------------------------------------------------------
DAEMON_HOST = "127.0.0.1"
DAEMON_PORT = 9876

project_path: Path
logs_dir: Path
unity_path: str


def write_log(msg: str) -> None:
    ts = datetime.now().strftime("%Y-%m-%dT%H:%M:%S")
    with open(logs_dir / "run_tests.log", "a", encoding="utf-8") as f:
        f.write(f"{ts}\t{msg}\n")


# ---------------------------------------------------------------------------
# Log cleanup
# ---------------------------------------------------------------------------
def clear_old_logs() -> None:
    old_files = list(logs_dir.glob("*.log")) + list(logs_dir.glob("*.xml"))
    if old_files:
        removed = 0
        for f in old_files:
            try:
                f.unlink(missing_ok=True)
                removed += 1
            except PermissionError:
                pass  # file locked by Unity, skip
        if removed:
            print(f"Cleared {removed} old log/result file(s) from {logs_dir}")


# ---------------------------------------------------------------------------
# Daemon client
# ---------------------------------------------------------------------------
def daemon_send(cmd: str, *, filter: str = "", timeout_minutes: int = 10, exit_on_fail: bool = True, retry_on_reload: bool = True) -> dict:
    """Send a command to the daemon. Returns parsed JSON response. Supports domain reload auto-retry."""
    attempts = 2 if retry_on_reload else 1
    for attempt in range(attempts):
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.settimeout(timeout_minutes * 60)
        try:
            sock.connect((DAEMON_HOST, DAEMON_PORT))
            request = json.dumps({"cmd": cmd, "filter": filter}) + "\n"
            sock.sendall(request.encode("utf-8"))
            data = b""
            while not data.endswith(b"\n"):
                chunk = sock.recv(4096)
                if not chunk:
                    break
                data += chunk
            res_str = data.decode("utf-8")
            if not res_str:
                raise json.JSONDecodeError("Empty response", "", 0)
            return json.loads(res_str)
        except (ConnectionError, socket.timeout, OSError, json.JSONDecodeError) as ex:
            if attempt < attempts - 1:
                # Suspect domain reload
                print(f"\nDomain reload/reconnect detected during '{cmd}'. Waiting for Unity to recover...", end=" ", flush=True)
                write_log(f"Domain reload/reconnect detected during '{cmd}', waiting for daemon to recover")
                # Wait for daemon to recover (up to 3 minutes)
                max_attempts = 180
                recovered = False
                for _ in range(max_attempts):
                    time.sleep(1.0)
                    if daemon_ping():
                        recovered = True
                        break
                if recovered:
                    print("OK (reconnected)")
                    write_log("Daemon recovered, retrying command")
                    continue
                else:
                    print("TIMEOUT")
                    write_log("Daemon failed to recover from domain reload")
            
            # If we reach here, we have no attempts left or recovery timed out
            if exit_on_fail:
                print(f"Error: Cannot communicate with TestDaemon on port 9876 ({ex}).")
                sys.exit(20)
            else:
                raise ex
        finally:
            sock.close()


def daemon_ping() -> bool:
    """Check if daemon is reachable."""
    try:
        result = daemon_send("ping", timeout_minutes=1, exit_on_fail=False, retry_on_reload=False)
        return result.get("status") == "ok"
    except (ConnectionRefusedError, socket.timeout, OSError, json.JSONDecodeError):
        return False



# ---------------------------------------------------------------------------
# Preconditions
# ---------------------------------------------------------------------------
def assert_preconditions() -> None:
    if not daemon_ping():
        print("Error: TestDaemon is not running.")
        print("       User must open Unity Editor with this project — daemon starts automatically.")
        sys.exit(20)
    logs_dir.mkdir(parents=True, exist_ok=True)


# ---------------------------------------------------------------------------
# Tier 0 — Regenerate .csproj
# ---------------------------------------------------------------------------
def regenerate_csproj() -> None:
    print("Regenerating .csproj files...", end=" ", flush=True)
    write_log("Regenerating .csproj via daemon")
    try:
        result = daemon_send("regen", exit_on_fail=False)
        if result.get("status") != "ok":
            print(f"FAILED: {result.get('message', 'unknown')}")
            sys.exit(14)
        print("OK")
        write_log("csproj regeneration complete")
    except (ConnectionError, socket.timeout, OSError, json.JSONDecodeError) as ex:
        print(f"FAILED: {ex}")
        sys.exit(14)


# ---------------------------------------------------------------------------
# Tier 1 — dotnet compile checks (local, no daemon needed)
# ---------------------------------------------------------------------------
# Tier 1 has been removed; compilation check is performed inside Unity Editor via the daemon.


# ---------------------------------------------------------------------------
# Tier 2 — EditMode tests (via daemon)
# ---------------------------------------------------------------------------
def run_tier2() -> None:
    print("Running EditMode tests...", end=" ", flush=True)
    write_log("Running EditMode tests via daemon")
    result = daemon_send("edit-tests")

    total = result.get("total", 0)
    failed = result.get("failed", 0)
    write_log(f"EditMode: total={total} failed={failed}")

    if result.get("status") == "error":
        print(f"ERROR: {result.get('message', 'unknown')}")
        sys.exit(31)

    if failed == 0:
        print(f"OK ({total} passed)")
        return

    # Handle failures
    fail_names = result.get("fail_names", "").split("|")

    if failed == 1:
        print(f"1 failure, attempting reruns...")
        _rerun_single_failure(fail_names[0], "edit-tests")
    else:
        print(f"FAILED ({failed}/{total})\n")
        print("Failure Details:")
        print(result.get("fail_details", ""))
        sys.exit(4)


def _rerun_single_failure(test_name: str, cmd: str) -> None:
    """Rerun a single failing test up to 3 times (flaky detection)."""
    write_log(f"Single failure: {test_name} — attempting reruns")
    for i in range(1, 4):
        write_log(f"Rerun #{i} for {test_name}")
        result = daemon_send(cmd, filter=test_name)
        if result.get("failed", 1) == 0:
            write_log(f"Rerun #{i} passed — marking as flaky")
            print(f"  Passed on rerun #{i} (flaky)")
            return
    write_log("Still failing after reruns")
    print(f"  [X] {test_name} - still failing after 3 reruns")
    print("\nFailure Details:")
    print(result.get("fail_details", ""))
    sys.exit(3)


# ---------------------------------------------------------------------------
# Tier 3 — PlayMode tests (via daemon)
# ---------------------------------------------------------------------------
def run_tier3() -> None:
    print("Running PlayMode tests...", end=" ", flush=True)
    write_log("Running PlayMode tests via daemon")
    result = daemon_send("play-tests")

    total = result.get("total", 0)
    failed = result.get("failed", 0)
    write_log(f"PlayMode: total={total} failed={failed}")

    if result.get("status") == "error":
        print(f"ERROR: {result.get('message', 'unknown')}")
        sys.exit(41)

    if failed > 0:
        print(f"FAILED ({failed}/{total})\n")
        print("Failure Details:")
        print(result.get("fail_details", ""))
        sys.exit(5)

    print(f"OK ({total} passed)")


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------
def main() -> None:
    global project_path, logs_dir, unity_path

    parser = argparse.ArgumentParser(description="Run tests for Monkey vs Allen")
    parser.add_argument("--scope", default="fast", choices=["fast", "full"])
    parser.add_argument("--project-path", default=os.environ.get("PROJECT_PATH", "."))
    parser.add_argument("--unity-path", default=r"D:\UnityEditor\6000.3.11f1\Editor\Unity.exe")
    parser.add_argument("--logs-dir", default=os.environ.get("LOGS_DIR", ""))
    parser.add_argument("--tier1-projects", nargs="*", default=[])
    parser.add_argument("--allow-missing-tier1-projects", action="store_true")
    args = parser.parse_args()

    project_path = Path(args.project_path).resolve()
    logs_dir = Path(args.logs_dir).resolve() if args.logs_dir else project_path / "Logs"
    logs_dir.mkdir(parents=True, exist_ok=True)
    unity_path = args.unity_path

    clear_old_logs()
    write_log(f"Starting run_tests.py scope={args.scope}")

    assert_preconditions()
    regenerate_csproj()

    if args.scope in ("fast", "full"):
        run_tier2()

    if args.scope == "full":
        run_tier3()

    write_log("All requested tests finished successfully")
    print("All requested tests finished successfully")


if __name__ == "__main__":
    main()
