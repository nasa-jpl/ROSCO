# run as administrator
# powershell -ExecutionPolicy Bypass -File clear-icon-cache.ps1
# Script to clear Windows icon cache
# Run this as Administrator after rebuilding

Write-Host "Clearing Windows icon cache..."

# Stop Explorer
Write-Host "Stopping Windows Explorer..."
Stop-Process -Name explorer -Force

# Delete icon cache files
$iconCachePath = "$env:LOCALAPPDATA\IconCache.db"
$iconCacheFolder = "$env:LOCALAPPDATA\Microsoft\Windows\Explorer"

if (Test-Path $iconCachePath) {
    Remove-Item $iconCachePath -Force
    Write-Host "Deleted IconCache.db"
}

if (Test-Path $iconCacheFolder) {
    Get-ChildItem $iconCacheFolder -Filter "*.db" | Remove-Item -Force
    Write-Host "Deleted icon cache files in Explorer folder"
}

# Restart Explorer
Write-Host "Restarting Windows Explorer..."
Start-Process explorer

Write-Host "Icon cache cleared! Please rebuild your application and run it again."
