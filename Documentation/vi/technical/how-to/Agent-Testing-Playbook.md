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
- Hệ điều hành: Windows/Linux/macOS có thể chạy Python 3.
- Unity Editor đang mở dự án này (TestDaemon tự động kích hoạt trên port 9876).
- Có quyền licence Unity trên runner nếu chạy EditMode/PlayMode trong CI.
- Thư mục repo hiện tại là root dự án (chứa `.sln`).
- Có thư mục `Assets/Tests/` với EditMode và PlayMode tests.

## Biến môi trường được sử dụng
- `UNITY_EDITOR_PATH` — đường dẫn tới `Unity.exe`.
- `PROJECT_PATH` — đường dẫn tới root project (mặc định `.`).
- `LOGS_DIR` — nơi ghi log (mặc định `Logs/`).

## Quick start (local)
1. Mở Unity Editor với dự án này (TestDaemon sẽ tự động chạy ngầm trên port 9876).
2. Chạy pipeline nhanh (Tier1 + Tier2):

```bash
python tools/run_tests.py --scope fast
```

3. Chạy pipeline đầy đủ (bao gồm PlayMode):

```bash
python tools/run_tests.py --scope full
```

## Các bước thực thi (chi tiết) — agent có thể gọi từng bước này

### Tier 1 — Compile check (Unity Editor)
Preconditions: TestDaemon đang chạy.

Steps:
Python client gửi lệnh `regen` tới Daemon để đồng bộ project files và kích hoạt trình biên dịch của Unity Editor. Trình biên dịch của Unity hoạt động rất nhanh và chính xác hơn dotnet CLI bên ngoài.
Nếu có Domain Reload (do có thay đổi code), kết nối sẽ bị ngắt tạm thời. Python client sẽ tự động chờ và ping cho đến khi Daemon online trở lại.

Agent action rules:
- Nếu Daemon trả về kết quả compile lỗi (hoặc không thể recover sau 3 phút), dừng pipeline và thông báo lỗi.

### Tier 2 — EditMode tests (TestDaemon)
Preconditions: TestDaemon đang chạy và compilation thành công.

Steps:
Python client gửi lệnh `edit-tests` tới Daemon. Daemon chạy tests qua `TestRunnerApi` của Unity và trả về kết quả JSON.

Agent action rules:
- Nếu tests fail: lấy danh sách test bị lỗi để rerun hoặc báo cáo.
- Rerun rule: nếu chỉ 1 test fail → rerun duy nhất test đó (hỗ trợ bởi filter); nếu flaky (pass ở lần rerun) → đánh dấu flaky.

### Tier 3 — PlayMode tests (TestDaemon)
Preconditions: tương tự Tier 2, chạy khi có cấu hình đầy đủ.

Steps:
Python client gửi lệnh `play-tests` tới Daemon để chạy các test PlayMode.

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
1. Smoke: `python tools/run_tests.py --scope fast` trả exit code 0 và tạo `Logs/editmode-test-results.xml`.
2. Fault injection: thêm test EditMode failing; agent phải detect, rerun theo rule và report failure với stackframe.
3. Full: chạy `python tools/run_tests.py --scope full` local với Unity licensed; verify `Logs/playmode-test-results.xml`.

## Where to find scripts
- `tools/run_tests.py` — orchestration.
- `tools/agent_runner.py` — (gợi ý) parser + reporter (Python) — optional.

## Onboarding quick-checklist
1. Mở Unity Editor.
2. Chạy `python tools/run_tests.py --scope fast`.
3. Kiểm tra `Logs/` để xem `run_tests.log` và các file kết quả.

---

Xem thêm: Xem [How-to-test](../../how-to/How-to-test.md)