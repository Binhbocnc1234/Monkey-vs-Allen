---
type: technical
module: agent_testing_playbook

audience: [developer, agent]
status: deprecated
language: en
description: Companion guide for automated agent execution; translates the canonical testing strategy into a PowerShell-driven workflow for compile, EditMode, and PlayMode runs.
related:
  - testing
  - how-to-run-test-manually
---

<!-- ---
type: technical
module: agent_testing_playbook

audience: developer, agent
description: Companion guide for automated agent execution; translates the canonical testing strategy into a PowerShell-driven workflow for compile, EditMode, and PlayMode runs.
related:
  - testing
  - how_to_documentation
  - software-development-process
---

# Agent Testing Playbook

This document turns the canonical testing strategy into an execution guide for agents and developers. It focuses on how to run the pipeline, how to collect results, and how to react to failures.

## Preconditions
- Windows local environment or a CI runner that can execute PowerShell.
- `dotnet` SDK installed and available in `PATH`.
- Unity Editor installed and the `UNITY_EDITOR_PATH` environment variable set.
- Valid Unity license for any runner that executes EditMode or PlayMode tests.
- The repository root contains the Unity solution and the `Assets/Tests/` directory.

## Environment variables
- `UNITY_EDITOR_PATH`: path to `Unity.exe`.
- `PROJECT_PATH`: root of the Unity project. Default: `.`.
- `LOGS_DIR`: output directory for logs and XML results. Default: `Logs/`.

## Preflight before Unity tests
Run these checks before Tier 2 or Tier 3:
- Confirm `UNITY_EDITOR_PATH` is set and points to an existing `Unity.exe`.
- Confirm `PROJECT_PATH` exists.
- Confirm `LOGS_DIR` exists or can be created.
- Confirm the Unity license is available on the runner.

In `tools/run_tests.ps1`, this maps to the Unity preflight function before `Invoke-Tier2EditModeTests` and `Invoke-Tier3PlayModeTests`.

## Workflow
1. Run Tier 1 compile checks with `Invoke-Tier1CompileChecks`.
2. Run Unity preflight checks with `Assert-UnityTestPreconditions` before Tier 2 or Tier 3.
3. Run Tier 2 EditMode tests with `Invoke-Tier2EditModeTests`.
4. Run Tier 3 PlayMode tests with `Invoke-Tier3PlayModeTests` when a full validation is required.
5. Collect logs and XML results as artifacts.
6. Stop at the first blocking failure and report the smallest useful summary.

## Tier 1: Compile checks

In `tools/run_tests.ps1`, this is implemented by `Invoke-Tier1CompileChecks`.

By default, the runner discovers project-owned assemblies under the repo root and builds the matching `.csproj` files. If the agent already knows the affected surface, it can pass a narrower `tier1Projects` list to the runner.

Typical scope:
- Project-owned `.csproj` files relevant to the changed code
- A narrower explicit list when the agent already knows the impacted assemblies

Expected outcome:
- Build succeeds with exit code `0`.
- The agent can proceed to Unity tests.

Failure handling:
- Stop immediately on compile errors.
- Summarize the failing project and the first useful error.

## Tier 2: EditMode tests

Before this tier runs, `Assert-UnityTestPreconditions` verifies the required Unity path, project path, logs directory, and license availability.

In `tools/run_tests.ps1`, this is implemented by `Invoke-Tier2EditModeTests`.

Expected outcome:
- The EditMode suite completes successfully.
- The run produces a log file and a test result XML file.

Failure handling:
- Parse the XML result and identify the failing test names.
- If only one test fails, treat it as a rerun candidate.
- If several unrelated tests fail, stop and report the failing assembly or group.

## Tier 3: PlayMode tests

Before this tier runs, `Assert-UnityTestPreconditions` verifies the same required conditions as Tier 2.

In `tools/run_tests.ps1`, this is implemented by `Invoke-Tier3PlayModeTests`.

Expected outcome:
- The PlayMode suite completes successfully.
- The run produces a log file and a test result XML file.

Failure handling:
- Report runtime failures with the top stack frame and the affected test.
- Treat long-running or hanging executions as timeout failures.

## Rerun policy
- Rerun a single failing test up to three times if the failure looks flaky.
- If the test passes on a rerun, mark the result as flaky and keep the evidence.
- If compile errors remain, do not continue to Unity test execution.

## Reporting
- Keep the report short and actionable.
- Include the tier, the failing test or project, the top stack frame, and the artifact paths.
- Prefer a single root-cause hypothesis over a long list of guesses.

## Verification
1. `tools/run_tests.ps1 --scope fast` returns exit code `0`.
2. The logs directory contains the expected EditMode XML and log files.
3. A full run also produces PlayMode results when requested.

## Cross-reference
See [How-to-test](How-to-test.md) for the testing strategy, tier definitions, and when to use each tier. -->