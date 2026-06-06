<#
Finds outgoing and incoming related-document links by document id.

Usage:
  powershell -ExecutionPolicy Bypass -File .\tools\find_related_docs.ps1 -Id testing
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$Id,

    [string]$Root = ".\Documentation"
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
$records = @()

foreach ($doc in $docs) {
    $frontmatter = Get-MarkdownFrontmatter -Path $doc.FullName
    if (-not $frontmatter) { continue }

    $related = Get-ValueList -Value $frontmatter['related']
    $module = [string]$frontmatter['module']
    $fileId = [System.IO.Path]::GetFileNameWithoutExtension($doc.Name)

    $records += [pscustomobject]@{
        Path = $doc.FullName.Substring($rootPath.Length).TrimStart('\', '/')
        Module = $module
        FileId = $fileId
        Related = $related
    }
}

$outgoing = $records | Where-Object { $_.Module -eq $Id -or $_.FileId -eq $Id } |
    ForEach-Object {
        foreach ($target in $_.Related) {
            [pscustomobject]@{ Direction = 'outgoing'; Source = $_.Path; Target = $target }
        }
    }

$incoming = $records | Where-Object { @($_.Related) -contains $Id } |
    ForEach-Object {
        [pscustomobject]@{ Direction = 'incoming'; Source = $_.Path; Target = $Id }
    }

@($outgoing) + @($incoming) | Sort-Object Direction, Source, Target | Format-Table -AutoSize -Wrap
