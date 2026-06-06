<#
Validates required YAML frontmatter fields for markdown documents.

Usage:
  powershell -ExecutionPolicy Bypass -File .\tools\validate_docs_frontmatter.ps1
  powershell -ExecutionPolicy Bypass -File .\tools\validate_docs_frontmatter.ps1 -Root .\Documentation\en
#>

param(
    [string]$Root = ".\Documentation"
)

$requiredFields = @('type', 'audience', 'status', 'description', 'related')
$validTypes = @('gameplay', 'technical')
$validAudiences = @('player', 'developer', 'agent')
$validStatuses = @('draft', 'active', 'deprecated')

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
        return @{ Error = 'missing frontmatter block' }
    }

    $endIndex = -1
    for ($i = 1; $i -lt $lines.Count; $i++) {
        if ($lines[$i].Trim() -eq '---') {
            $endIndex = $i
            break
        }
    }

    if ($endIndex -lt 0) {
        return @{ Error = 'unterminated frontmatter block' }
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

function Get-ValueList {
    param($Value)

    if ($null -eq $Value) { return @() }
    if ($Value -is [array]) { return @($Value) }
    if (($Value -is [string]) -and $Value.Contains(',')) {
        return @($Value.Split(',') | ForEach-Object { $_.Trim() })
    }
    return @($Value)
}

if (-not (Test-Path -LiteralPath $Root)) {
    Write-Error "Documentation root not found: $Root"
    exit 1
}

$rootPath = [System.IO.Path]::GetFullPath($Root)
$docs = Get-ChildItem -LiteralPath $rootPath -Recurse -Filter '*.md' -File
$issues = @()

foreach ($doc in $docs) {
    $frontmatter = Get-MarkdownFrontmatter -Path $doc.FullName
    $relativePath = $doc.FullName.Substring($rootPath.Length).TrimStart('\', '/')

    if ($frontmatter.Contains('Error')) {
        $issues += [pscustomobject]@{ Path = $relativePath; Field = 'frontmatter'; Issue = $frontmatter['Error'] }
        continue
    }

    foreach ($field in $requiredFields) {
        if (-not $frontmatter.Contains($field) -or $null -eq $frontmatter[$field] -or [string]::IsNullOrWhiteSpace([string]$frontmatter[$field])) {
            $issues += [pscustomobject]@{ Path = $relativePath; Field = $field; Issue = 'missing required field' }
        }
    }

    if ($frontmatter.Contains('type') -and ($validTypes -notcontains $frontmatter['type'])) {
        $issues += [pscustomobject]@{ Path = $relativePath; Field = 'type'; Issue = "invalid value '$($frontmatter['type'])'" }
    }

    foreach ($audience in (Get-ValueList -Value $frontmatter['audience'])) {
        if ($validAudiences -notcontains $audience) {
            $issues += [pscustomobject]@{ Path = $relativePath; Field = 'audience'; Issue = "invalid value '$audience'" }
        }
    }

    if ($frontmatter.Contains('status') -and ($validStatuses -notcontains $frontmatter['status'])) {
        $issues += [pscustomobject]@{ Path = $relativePath; Field = 'status'; Issue = "invalid value '$($frontmatter['status'])'" }
    }
}

if ($issues.Count -gt 0) {
    $issues | Sort-Object Path, Field | Format-Table -AutoSize -Wrap
    exit 2
}

Write-Host "All markdown frontmatter is valid under $rootPath"
exit 0
