Set-Location -Path $PSScriptRoot

$apiKey = "53618a5a-35a0-41f8-8fb1-e0dbec37403e"
$xamlPath = "..\AllInOneLauncher\Pages\Primary\Online.xaml"
$backupPath = "$PSScriptRoot\Online.xaml.bak"

Copy-Item -Path $xamlPath -Destination $backupPath -Force

$xamlContent = Get-Content $xamlPath
$xamlContent = $xamlContent -replace 'ReplaceAPIKeyHere', $apiKey
Set-Content $xamlPath $xamlContent