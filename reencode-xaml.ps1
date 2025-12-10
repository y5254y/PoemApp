# reencode-xaml.ps1
# Re-save all .xaml files as UTF-8 without BOM
# Run from repository root in PowerShell

$files = Get-ChildItem -Path . -Recurse -Filter '*.xaml' -File -ErrorAction SilentlyContinue
if (-not $files) {
    Write-Output "No .xaml files found."
    exit 0
}

foreach ($f in $files) {
    try {
        Write-Output "Processing: $($f.FullName)"
        $text = Get-Content -Raw -LiteralPath $f.FullName -ErrorAction Stop
        # write as UTF8 without BOM
        [System.IO.File]::WriteAllText($f.FullName, $text, (New-Object System.Text.UTF8Encoding($false)))
        Write-Output "Re-encoded: $($f.FullName)"
    }
    catch {
        Write-Error "Failed: $($f.FullName) => $($_.Exception.Message)"
    }
}

Write-Output "Done."