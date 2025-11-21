# Stop PoemApp.API by process name
$processes = Get-Process -Name "PoemApp.API" -ErrorAction SilentlyContinue
if ($processes) {
    $processes | ForEach-Object { Write-Host "Stopping process ID: $($_.Id)"; Stop-Process -Id $_.Id -Force }
} else {
    Write-Host "No running PoemApp.API processes found."
}