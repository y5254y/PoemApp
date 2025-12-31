# reencode-textfiles.ps1
# Re-save common text source and static asset files as UTF-8 without BOM
# Run from repository root in PowerShell (Windows)

$extensions = '\*.razor','\*.cshtml','\*.html','\*.htm','\*.css','\*.js','\*.json','\*.cs','\*.xml','\*.md'
$excludeDirs = @('.git','.vs','bin','obj','node_modules')

Write-Output "Searching files to re-encode..."
$files = Get-ChildItem -Path . -Recurse -File -Include $extensions -ErrorAction SilentlyContinue | Where-Object {
    foreach ($d in $excludeDirs) { if ($_.FullName -like "*\\$d\\*") { return $false } }
    return $true
}

if (-not $files -or $files.Count -eq 0) {
    Write-Output "No matching files found."
    exit 0
}

foreach ($f in $files) {
    try {
        Write-Output "Processing: $($f.FullName)"
        $content = Get-Content -Raw -LiteralPath $f.FullName -ErrorAction Stop
        # write as UTF8 without BOM
        [System.IO.File]::WriteAllText($f.FullName, $content, (New-Object System.Text.UTF8Encoding($false)))
    }
    catch {
        Write-Error "Failed to re-encode $($f.FullName): $($_.Exception.Message)"
    }
}

Write-Output "Done. Consider rebuilding Docker images after this change."
