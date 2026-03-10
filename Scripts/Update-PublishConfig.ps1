# File: Scripts/Update-PublishConfig.ps1
# Purpose:
#   - Sync <ModVersion Value="..."/> in PublishConfiguration.xml to csproj <Version>.
#   - Enforce consistent line endings (CRLF or LF) to prevent VS "MIXED" + popup.
#   - Optional: left-align inner text of <LongDescription> and <ChangeLog> only.
#   - Also update any mod.json found recursively under repo root:
#       "version": "x.y.z" -> match csproj <Version>
#     If mod.json is not found, no changes and no failure.

param(
  # Full path to PublishConfiguration.xml
  [Parameter(Mandatory = $true)][string]$Path,

  # Version string from csproj <Version>
  [Parameter(Mandatory = $true)][string]$Version,

  # Enforced line ending style for PublishConfiguration.xml == PICK ONE ==
  [ValidateSet('crlf','lf')][string]$Eol = 'crlf',

  # Optional flag: if present, strip leading spaces/tabs *inside* LongDescription + ChangeLog blocks only
  [switch]$LeftAlignBlocks
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not (Test-Path -LiteralPath $Path)) {
  throw "PublishConfiguration.xml not found: $Path"
}

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
$original  = [System.IO.File]::ReadAllText($Path, $utf8NoBom)

# Strip Unicode BOM character if present (prevents EF BB BF from being written back out)
if ($original.Length -gt 0 -and $original[0] -eq [char]0xFEFF) {
  $original = $original.Substring(1)
}

if ([string]::IsNullOrWhiteSpace($original)) {
  throw "Refusing to update because file is empty: $Path (restore it first)"
}

function Normalize-Eol([string]$s, [string]$eolKind) {
  $s = $s.Replace("`r`n", "`n").Replace("`r", "`n")
  $s = $s.Replace([char]0x85,  "`n")   # NEL
  $s = $s.Replace([char]0x2028, "`n")  # LS
  $s = $s.Replace([char]0x2029, "`n")  # PS

  if ($eolKind -eq 'crlf') {
    $s = $s.Replace("`n", "`r`n")
  }
  return $s
}

function LeftAlignInnerBlock([string]$s, [string]$tagName) {
  $opts = [System.Text.RegularExpressions.RegexOptions]::IgnoreCase -bor
          [System.Text.RegularExpressions.RegexOptions]::Singleline

  $rx = [System.Text.RegularExpressions.Regex]::new(
    "(<$tagName\b[^>]*>)(.*?)(</$tagName>)",
    $opts
  )

  if (-not $rx.IsMatch($s)) { return $s }

  return $rx.Replace($s, { param($m)
    $openTag   = $m.Groups[1].Value
    $innerText = $m.Groups[2].Value
    $closeTag  = $m.Groups[3].Value

    $inner2 = [System.Text.RegularExpressions.Regex]::Replace($innerText, '(?m)^[\t ]+', '')

    $openTag + $inner2 + $closeTag
  }, 1)
}

function Update-ModJsonVersions([string]$rootDir, [string]$versionValue) {
  $files =
    Get-ChildItem -LiteralPath $rootDir -Recurse -Force -File -Filter 'mod.json' -ErrorAction SilentlyContinue |
    Where-Object {
      $p = $_.FullName
      ($p -notmatch '\\node_modules\\') -and
      ($p -notmatch '\\bin\\') -and
      ($p -notmatch '\\obj\\') -and
      ($p -notmatch '\\dist\\') -and
      ($p -notmatch '\\build\\') -and
      ($p -notmatch '\\out\\')
    }

  if (-not $files -or $files.Count -eq 0) {
    Write-Host "No mod.json found under repo root (skip)."
    return 0
  }

  $rx = [System.Text.RegularExpressions.Regex]::new(
    '(?m)(?<prefix>"version"\s*:\s*")(?<val>[^"]*)(?<suffix>")',
    [System.Text.RegularExpressions.RegexOptions]::IgnoreCase
  )

  $changedCount = 0
  foreach ($f in $files) {
    $json = [System.IO.File]::ReadAllText($f.FullName, $utf8NoBom)

    if ($json.Length -gt 0 -and $json[0] -eq [char]0xFEFF) {
      $json = $json.Substring(1)
    }

    if (-not $rx.IsMatch($json)) {
      continue
    }

    $json2 = $rx.Replace($json, { param($m)
      $m.Groups['prefix'].Value + $versionValue + $m.Groups['suffix'].Value
    }, 1)

    # Repo policy: JSON uses LF
    $json2 = Normalize-Eol $json2 'lf'

    if ($json2 -eq $json) {
      continue
    }

    $tmp = ($f.FullName + ".tmp")
    [System.IO.File]::WriteAllText($tmp, $json2, $utf8NoBom)
    Move-Item -Force -LiteralPath $tmp -Destination $f.FullName

    $rel = $f.FullName.Substring($rootDir.Length).TrimStart('\','/')
    Write-Host ("mod.json updated: {0}" -f $rel)
    $changedCount++
  }

  if ($changedCount -eq 0) {
    Write-Host "mod.json found but no version changes needed."
  }

  return $changedCount
}

# Normalize existing file first (removes MIXED now)
$text = Normalize-Eol $original $Eol

if ($LeftAlignBlocks) {
  $text = LeftAlignInnerBlock $text 'LongDescription'
  $text = LeftAlignInnerBlock $text 'ChangeLog'
}

$rxMod = [System.Text.RegularExpressions.Regex]::new(
  '(?m)^(?<prefix>[\t ]*<ModVersion\b[^>]*\bValue=")[^"]*(?<suffix>")',
  [System.Text.RegularExpressions.RegexOptions]::IgnoreCase
)

if ($rxMod.IsMatch($text)) {
  $text = $rxMod.Replace($text, { param($m)
    $m.Groups['prefix'].Value + $Version + $m.Groups['suffix'].Value
  }, 1)
} else {
  throw "Could not find <ModVersion ... Value=""..."" ...> in: $Path"
}

# Final EOL normalization AFTER edits
$text = Normalize-Eol $text $Eol

if ($Eol -eq 'crlf') {
  if ($text -match "(?<!`r)`n") { throw "Internal error: bare LF found (would create MIXED): $Path" }
  if ($text -match "`r(?!`n)")  { throw "Internal error: bare CR found (would create MIXED): $Path" }
}

$publishChanged = ($text -ne $original)

if ($publishChanged) {
  $bak = "$Path.bak"
  [System.IO.File]::WriteAllText($bak, $original, $utf8NoBom)

  $tmp = "$Path.tmp"
  [System.IO.File]::WriteAllText($tmp, $text, $utf8NoBom)
  Move-Item -Force -LiteralPath $tmp -Destination $Path

  Write-Host ("ModVersion updated to [{0}] in: {1} (EOL={2}, LeftAlignBlocks={3}). BACKUP: {4}" -f `
    $Version, (Split-Path -Leaf $Path), $Eol, $LeftAlignBlocks.IsPresent, (Split-Path -Leaf $bak))
} else {
  Write-Host ("No PublishConfiguration.xml change needed (ModVersion already [{0}] and formatting already clean)." -f $Version)
}

# Repo root = parent of Scripts/
$repoRoot = Split-Path -Parent $PSScriptRoot
$modJsonChanged = Update-ModJsonVersions $repoRoot $Version

Write-Host ("Done. PublishConfigChanged={0}, ModJsonUpdated={1}." -f $publishChanged, $modJsonChanged)
exit 0
