Set-Location -Path $PSScriptRoot

$apiKey = ""
$xamlPath = "..\AllInOneLauncher\Pages\Primary\Online.xaml"
$backupPath = "$PSScriptRoot\Online.xaml.bak"

Copy-Item -Path $xamlPath -Destination $backupPath -Force

$xamlContent = Get-Content $xamlPath
$xamlContent = $xamlContent -replace 'ReplaceAPIKeyHere', $apiKey
Set-Content $xamlPath $xamlContent
