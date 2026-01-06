# reencode-textfiles.ps1
# Re-save common text source and static asset files as UTF-8 without BOM
# Run from repository root in PowerShell (Windows)

# Use glob patterns without leading backslash so Get-ChildItem -Include works correctly
$extensions = '*.razor','*.cshtml','*.html','*.htm','*.css','*.js','*.json','*.cs','*.xml','*.md'
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
        $bytes = [System.IO.File]::ReadAllBytes($f.FullName)
        $text = $null

        # Detect BOMs first
        if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
            # UTF-8 BOM
            $encoding = [System.Text.Encoding]::UTF8
            $text = $encoding.GetString($bytes)
        }
        elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFE -and $bytes[1] -eq 0xFF) {
            # UTF-16 BE
            $encoding = [System.Text.Encoding]::BigEndianUnicode
            $text = $encoding.GetString($bytes)
        }
        elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFF -and $bytes[1] -eq 0xFE) {
            # UTF-16 LE
            $encoding = [System.Text.Encoding]::Unicode
            $text = $encoding.GetString($bytes)
        }
        else {
            # No BOM: try strict UTF-8 first (throws on invalid bytes), otherwise fall back to common Windows encoding (GBK/CP936)
            try {
                $utf8Strict = New-Object System.Text.UTF8Encoding($false, $true)
                $text = $utf8Strict.GetString($bytes)
            }
            catch {
                try {
                    $gbk = [System.Text.Encoding]::GetEncoding(936)
                    $text = $gbk.GetString($bytes)
                }
                catch {
                    # As a last resort, try default encoding
                    $text = [System.Text.Encoding]::Default.GetString($bytes)
                }
            }
        }

        if ($null -eq $text) {
            throw "Failed to decode file $($f.FullName) with detected encodings"
        }

        # write as UTF8 without BOM
        [System.IO.File]::WriteAllText($f.FullName, $text, (New-Object System.Text.UTF8Encoding($false)))
    }
    catch {
        Write-Error "Failed to re-encode $($f.FullName): $($_.Exception.Message)"
    }
}

Write-Output "Done. Consider rebuilding Docker images after this change."
