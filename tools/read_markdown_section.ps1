<#
Reads a markdown section by line range.

Usage:
  powershell -ExecutionPolicy Bypass -File .\tools\read_markdown_section.ps1 -Path .\Documentation\en\technical\how-to\How-to-documentation.md -Range "[81,126]"
#>

param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Path,

    [Parameter(Mandatory = $true, Position = 1)]
    [string]$Range
)

if (-not (Test-Path -LiteralPath $Path)) {
    Write-Error "Markdown file not found: $Path"
    exit 1
}

if ($Range -notmatch '^\s*\[\s*(\d+)\s*,\s*(\d+)\s*\]\s*$') {
    Write-Error "Range must use the format [a,b], such as [81,126]."
    exit 2
}

$start = [int]$matches[1]
$end = [int]$matches[2]

if ($start -lt 1 -or $end -lt $start) {
    Write-Error "Invalid range: $Range"
    exit 3
}

$resolvedPath = [System.IO.Path]::GetFullPath($Path)
$lines = Get-Content -LiteralPath $resolvedPath

if ($start -gt $lines.Count) {
    Write-Error "Range start $start is beyond file length $($lines.Count)."
    exit 4
}

if ($end -gt $lines.Count) {
    $end = $lines.Count
}

for ($i = $start; $i -le $end; $i++) {
    $lines[$i - 1]
}
