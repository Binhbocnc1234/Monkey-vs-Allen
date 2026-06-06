<#
Lists H1 and H2 sections in a markdown file.

Usage:
  powershell -ExecutionPolicy Bypass -File .\tools\get_markdown_headers.ps1 -Path .\Documentation\en\technical\how-to\How-to-documentation.md

Output:
  Level  Header                  Range
  -----  ------                  -----
  H1     Introduction            [11,23]
  H2     YAML Frontmatter        [81,130]
#>

param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Path
)

if (-not (Test-Path -LiteralPath $Path)) {
    Write-Error "Markdown file not found: $Path"
    exit 1
}

$resolvedPath = [System.IO.Path]::GetFullPath($Path)
$lines = Get-Content -LiteralPath $resolvedPath
$sections = @()
$inFence = $false

for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]

    if ($line -match '^\s*(```|~~~)') {
        $inFence = -not $inFence
        continue
    }

    if ($inFence) {
        continue
    }

    if ($line -match '^(#{1,2})\s+(.+?)\s*$') {
        $level = $matches[1].Length
        $header = $matches[2] -replace '\s+#+\s*$', ''

        $sections += [pscustomobject]@{
            Level = "H$level"
            Header = $header
            Start = $i + 1
            End = $null
        }
    }
}

for ($i = 0; $i -lt $sections.Count; $i++) {
    if ($i -lt $sections.Count - 1) {
        $sections[$i].End = $sections[$i + 1].Start - 1
    } else {
        $sections[$i].End = $lines.Count
    }
}

$sections |
    Select-Object Level, Header, @{ Name = 'Range'; Expression = { "[$($_.Start),$($_.End)]" } } |
    Format-Table -AutoSize
