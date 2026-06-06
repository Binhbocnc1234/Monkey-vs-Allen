<#
Queries markdown documents by YAML frontmatter fields.

Usage:
  powershell -ExecutionPolicy Bypass -File .\tools\query_docs_frontmatter.ps1 -Audience agent -Status active
  powershell -ExecutionPolicy Bypass -File .\tools\query_docs_frontmatter.ps1 -Type technical -DescriptionContains testing
#>

param(
    [string]$Root = ".\Documentation",
    [string]$Type,
    [string]$Audience,
    [string]$Status,
    [string]$Module,
    [string]$DescriptionContains
)

function Convert-FrontmatterValue {
    param([string]$Value)

    $trimmed = $Value.Trim()
    if ($trimmed.StartsWith('[') -and $trimmed.EndsWith(']')) {
        $inner = $trimmed.Substring(1, $trimmed.Length - 2)
        if ([string]::IsNullOrWhiteSpace($inner)) { return @() }
        return @($inner.Split(',') | ForEach-Object { $_.Trim().Trim('"').Trim("'") })
    }

    return $trimmed.Trim('"').Trim("'")
}

function Get-MarkdownFrontmatter {
    param([string]$Path)

    $lines = Get-Content -LiteralPath $Path
    if ($lines.Count -lt 2 -or $lines[0].Trim() -ne '---') {
        return $null
    }

    $endIndex = -1
    for ($i = 1; $i -lt $lines.Count; $i++) {
        if ($lines[$i].Trim() -eq '---') {
            $endIndex = $i
            break
        }
    }

    if ($endIndex -lt 0) {
        return $null
    }

    $data = [ordered]@{}
    $currentKey = $null

    for ($i = 1; $i -lt $endIndex; $i++) {
        $line = $lines[$i]

        if ($line -match '^\s*-\s+(.+?)\s*$' -and $currentKey) {
            if (-not ($data[$currentKey] -is [array])) {
                $data[$currentKey] = @()
            }
            $data[$currentKey] = @($data[$currentKey]) + $matches[1].Trim().Trim('"').Trim("'")
            continue
        }

        if ($line -match '^([A-Za-z_][A-Za-z0-9_-]*)\s*:\s*(.*)$') {
            $currentKey = $matches[1]
            $value = $matches[2]
            if ([string]::IsNullOrWhiteSpace($value)) {
                $data[$currentKey] = @()
            } else {
                $data[$currentKey] = Convert-FrontmatterValue -Value $value
            }
        }
    }

    return $data
}

function Test-FieldMatch {
    param($Value, [string]$Expected)

    if (-not $Expected) { return $true }
    if ($null -eq $Value) { return $false }

    if ($Value -is [array]) {
        return @($Value) -contains $Expected
    }

    if (($Value -is [string]) -and $Value.Contains(',')) {
        return @($Value.Split(',') | ForEach-Object { $_.Trim() }) -contains $Expected
    }

    return $Value -eq $Expected
}

if (-not (Test-Path -LiteralPath $Root)) {
    Write-Error "Documentation root not found: $Root"
    exit 1
}

$rootPath = [System.IO.Path]::GetFullPath($Root)
$docs = Get-ChildItem -LiteralPath $rootPath -Recurse -Filter '*.md' -File
$results = @()

foreach ($doc in $docs) {
    $frontmatter = Get-MarkdownFrontmatter -Path $doc.FullName
    if (-not $frontmatter) { continue }

    if (-not (Test-FieldMatch -Value $frontmatter['type'] -Expected $Type)) { continue }
    if (-not (Test-FieldMatch -Value $frontmatter['audience'] -Expected $Audience)) { continue }
    if (-not (Test-FieldMatch -Value $frontmatter['status'] -Expected $Status)) { continue }
    if (-not (Test-FieldMatch -Value $frontmatter['module'] -Expected $Module)) { continue }

    $description = [string]$frontmatter['description']
    if ($DescriptionContains -and $description.IndexOf($DescriptionContains, [System.StringComparison]::OrdinalIgnoreCase) -lt 0) {
        continue
    }

    $results += [pscustomobject]@{
        Path = $doc.FullName.Substring($rootPath.Length).TrimStart('\', '/')
        Type = $frontmatter['type']
        Audience = ($frontmatter['audience'] -join ', ')
        Status = $frontmatter['status']
        Module = $frontmatter['module']
        Description = $description
        Related = ($frontmatter['related'] -join ', ')
    }
}

$results | Sort-Object Path | Format-Table -AutoSize -Wrap
