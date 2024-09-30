Set-Location -Path $PSScriptRoot

$xamlPath = "..\AllInOneLauncher\Pages\Primary\Online.xaml"
$backupPath = "$PSScriptRoot\Online.xaml.bak"

if (Test-Path $backupPath) {
    Copy-Item -Path $backupPath -Destination $xamlPath -Force
    Remove-Item -Path $backupPath -Force
    Write-Output "Online.xaml has been successfully restored, and the backup has been deleted."
} else {
    Write-Error "Backup file not found. Failed to restore Online.xaml."
}