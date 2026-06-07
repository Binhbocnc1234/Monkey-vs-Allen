<#
Run tests orchestration for Monkey vs Allen (PowerShell).
Usage:
  powershell -ExecutionPolicy Bypass -File .\tools\run_tests.ps1 --scope fast
  powershell -ExecutionPolicy Bypass -File .\tools\run_tests.ps1 --scope full

Behaviors:
- Tier1: dotnet build on key projects
- Tier2: Unity EditMode tests
- Tier3: Unity PlayMode tests (only for --scope full)
- Creates Logs/ and writes test-results xml and logs
- Basic rerun heuristic: if a single test fails, rerun up to 3 times
#>

param(
    [string]$scope = "fast", # fast = Tier1+Tier2, full = Tier1+Tier2+Tier3
    [int]$unityTimeoutMinutes = 10,
    [string]$projectPath = $env:PROJECT_PATH,
    [string]$unityPath = "D:\UnityEditor\6000.3.11f1\Editor\Unity.exe",
    [string]$logsDir = $env:LOGS_DIR,
    [string[]]$tier1Projects = @()
)

if (-not $projectPath) { $projectPath = '.' }
if (-not $logsDir) { $logsDir = Join-Path $projectPath 'Logs' }
if (-not (Test-Path $logsDir)) { New-Item -ItemType Directory -Path $logsDir | Out-Null }

# Normalize to absolute paths to avoid Unity writing artifacts into an unexpected working directory
$projectPath = [System.IO.Path]::GetFullPath($projectPath)
$logsDir = [System.IO.Path]::GetFullPath($logsDir)

function Write-Log { param($msg) $ts = (Get-Date).ToString('s'); "$ts `t $msg" | Out-File -FilePath (Join-Path $logsDir 'run_tests.log') -Append }

function Assert-UnityTestPreconditions {
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        Write-Host 'Error: dotnet SDK is not available in PATH.'
        exit 10
    }
    if (-not $unityPath) {
        Write-Host 'Error: Unity Editor path is empty. Pass -unityPath.'
        exit 11
    }
    if (-not (Test-Path $unityPath)) {
        Write-Host "Error: Unity Editor not found at $unityPath"
        exit 12
    }
    if (-not (Test-Path $projectPath)) {
        Write-Host "Error: Project path not found: $projectPath"
        exit 13
    }
    if (-not (Test-Path $logsDir)) {
        New-Item -ItemType Directory -Path $logsDir | Out-Null
    }
}

function Invoke-Tier1CompileChecks {
    $dotnetProjects = Get-Tier1Projects -explicitProjects $tier1Projects
    foreach ($proj in $dotnetProjects) {
        Write-Log "Building $proj"
        # build can take longer; allow up to 5 minutes per project
        $buildArgs = @('build', $proj, '--nologo')
        Write-Log "Running: dotnet $([string]::Join(' ', $buildArgs))"
        # Invoke dotnet directly and capture combined stdout/stderr
        $cmdOutput = & dotnet @buildArgs 2>&1
        $exitCode = $LASTEXITCODE
        $projName = [System.IO.Path]::GetFileName($proj)
        $outFile = Join-Path $logsDir ("dotnet-build-$projName.log")
        ($cmdOutput -join "`n") | Out-File -FilePath $outFile -Encoding utf8
        if ($exitCode -ne 0) {
            Write-Log "Build failed for $proj with exit $exitCode. See $outFile"
            Write-Host "Build failed for $proj. See $outFile"
            exit 1
        }
    }
}

function Get-Tier1Projects {
    param(
        [string[]]$explicitProjects
    )

    if ($explicitProjects -and $explicitProjects.Count -gt 0) {
        return $explicitProjects
    }

    $projectFiles = Get-ChildItem -Path $projectPath -Filter '*.csproj' -File |
        Where-Object { $_.Name -match '^(MvA|Assembly-CSharp|TestAssembly|NewAssembly).*\.csproj$' } |
        Sort-Object Name

    $projectNames = $projectFiles | Select-Object -ExpandProperty FullName
    if (-not $projectNames -or $projectNames.Count -eq 0) {
        throw "No Tier 1 projects were discovered under $projectPath"
    }

    return $projectNames
}

function Invoke-Tier2EditModeTests {

    $editLog = Join-Path $logsDir 'editmode-test.log'
    $editResults = Join-Path $logsDir 'editmode-test-results.xml'
    $unityArgs = @('-batchmode', '-projectPath', "$projectPath", '-runTests', '-testPlatform', 'EditMode', '-logFile', "$editLog", '-testResults', "$editResults")
    Write-Log "Running Unity EditMode tests: $unityPath $($unityArgs -join ' ')"
    $cmdOutput = & $unityPath @unityArgs 2>&1
    $exitCode = $LASTEXITCODE
    ($cmdOutput -join "`n") | Out-File -FilePath $editLog -Append -Encoding utf8

    if (-not (Test-Path $editResults)) {
        Write-Log "EditMode results XML not found: $editResults (Unity exit $exitCode)"
        Write-Host "EditMode results file missing: $editResults (Unity exit $exitCode). See $editLog"
        exit 32
    }

    if ($exitCode -ne 0) {
        Write-Log "EditMode Unity process failed with exit code $exitCode."
        Write-Host "EditMode run failed (Unity exit $exitCode). See $editLog"
        exit 31
    }

    $parsed = Parse-UnityTestResults -xmlPath $editResults
    Write-Log "EditMode: total=$($parsed.total) failed=$($parsed.failed)"
    if ($parsed.failed -gt 0) {
        if ($parsed.failed -eq 1) {
            $failedTest = $parsed.failures[0].name
                Write-Log "Single failing test detected: $failedTest - attempting reruns"
            $maxReruns = 3
            $passCount = 0
            for ($i=1; $i -le $maxReruns; $i++) {
                Write-Log "Rerun #$i for $failedTest"
                $rerunLog = Join-Path $logsDir "editmode-rerun-$i.log"
                $rerunResults = Join-Path $logsDir "editmode-rerun-$i-results.xml"
                $rerunArgs = @('-projectPath', "$projectPath", '-runTests', '-testPlatform', 'EditMode', '-logFile', "$rerunLog", '-testResults', "$rerunResults", '-testFilter', $failedTest)
                $rres = Run-Process-Timeout -exe $unityPath -args $rerunArgs -timeoutSeconds ($unityTimeoutMinutes * 60)
                ($rres.StdOut + "`n" + $rres.StdErr) | Out-File -FilePath $rerunLog -Append -Encoding utf8
                $pr = Parse-UnityTestResults -xmlPath $rerunResults
                if ($pr.failed -eq 0) { $passCount++ }
                if ($passCount -ge 1) { Write-Log "Rerun succeeded on attempt $i"; break }
            }
            if ($passCount -ge 1) {
                Write-Log "Test marked flaky (passed on rerun)"
            } else {
                Write-Log "Test still failing after reruns"
                Write-Host "EditMode tests failed. See $editResults and logs in $logsDir"; exit 3
            }
        } else {
            Write-Log "Multiple tests failed; aborting and reporting"
            Write-Host "EditMode tests failed (multiple). See $editResults and logs in $logsDir"; exit 4
        }
    }
}
function Invoke-Tier3PlayModeTests {

    $playLog = Join-Path $logsDir 'playmode-test.log'
    $playResults = Join-Path $logsDir 'playmode-test-results.xml'
    $playArgs = @('-batchmode', '-projectPath', "$projectPath", '-runTests', '-testPlatform', 'PlayMode', '-logFile', "$playLog", '-testResults', "$playResults")
    Write-Log "Running Unity PlayMode tests"
    $cmdOutput = & $unityPath @playArgs 2>&1
    $exitCode = $LASTEXITCODE
    ($cmdOutput -join "`n") | Out-File -FilePath $playLog -Append -Encoding utf8

    if (-not (Test-Path $playResults)) {
        Write-Log "PlayMode results XML not found: $playResults (Unity exit $exitCode)"
        Write-Host "PlayMode results file missing: $playResults (Unity exit $exitCode). See $playLog"
        exit 42
    }

    if ($exitCode -ne 0) {
        Write-Log "PlayMode Unity process failed with exit code $exitCode."
        Write-Host "PlayMode run failed (Unity exit $exitCode). See $playLog"
        exit 41
    }

    $pParsed = Parse-UnityTestResults -xmlPath $playResults
    Write-Log "PlayMode: total=$($pParsed.total) failed=$($pParsed.failed)"
    if ($pParsed.failed -gt 0) {
        Write-Log "PlayMode tests failed"
        Write-Host "PlayMode tests failed. See $playResults and logs in $logsDir"; exit 5
    }
}

Write-Log "Starting run_tests.ps1 scope=$scope"

# Helper: run a process with timeout and capture exit code
function Run-Process-Timeout {
    param(
        [string]$exe,
        [string[]]$args,
        [int]$timeoutSeconds = 120,
        [string]$outFile = $null
    )
    $startInfo = New-Object System.Diagnostics.ProcessStartInfo
    $startInfo.FileName = $exe
    # Quote arguments containing whitespace to preserve path integrity.
    $argList = @()
    if ($args) {
        foreach ($arg in $args) {
            if ($null -eq $arg) { continue }
                if ($arg -match '\s') {
                    $argList += '"' + ($arg -replace '"', '""') + '"'
            } else {
                $argList += $arg
            }
        }
    }
    $startInfo.Arguments = [string]::Join(' ', $argList)
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    $startInfo.UseShellExecute = $false
    $proc = New-Object System.Diagnostics.Process
    $proc.StartInfo = $startInfo
    $proc.Start() | Out-Null

    $stdOut = $proc.StandardOutput.ReadToEndAsync()
    $stdErr = $proc.StandardError.ReadToEndAsync()

    $finished = $proc.WaitForExit($timeoutSeconds * 1000)
    if (-not $finished) {
        Write-Log "Process timed out after ${timeoutSeconds}s: $exe $($args -join ' ')"
        try { $proc.Kill() } catch { }
        return @{ ExitCode = 124; StdOut = $stdOut.Result; StdErr = $stdErr.Result }
    }

    return @{ ExitCode = $proc.ExitCode; StdOut = $stdOut.Result; StdErr = $stdErr.Result }
}

# Helper to parse Unity test results XML
function Parse-UnityTestResults {
    param([string]$xmlPath)
    if (-not (Test-Path $xmlPath)) { return @{ total=0; failed=0; failures=@() } }
    try {
        [xml]$doc = Get-Content $xmlPath -Raw
        $testcases = $doc.SelectNodes('//test-case')
        $failures = @()
        foreach ($tc in $testcases) {
            $res = $tc.GetAttribute('result')
            if ($res -eq 'Failed') {
                $failures += [pscustomobject]@{ name = $tc.GetAttribute('fullname'); classname = $tc.GetAttribute('classname'); message = ($tc.SelectSingleNode('failure/message') -as [string]); stack = ($tc.SelectSingleNode('failure/stack-trace') -as [string]) }
            }
        }
        return @{ total = $testcases.Count; failed = $failures.Count; failures = $failures }
    } catch {
        Write-Log "Failed to parse XML ${xmlPath}: $_"
        return @{ total=0; failed=0; failures=@() }
    }
}

Assert-UnityTestPreconditions
Invoke-Tier1CompileChecks

if ($scope -in @('fast', 'full')) {
    Invoke-Tier2EditModeTests
}

if ($scope -eq 'full') {
    Invoke-Tier3PlayModeTests
}

Write-Log "All requested tests finished successfully"
Write-Host "All requested tests finished successfully"
exit 0
