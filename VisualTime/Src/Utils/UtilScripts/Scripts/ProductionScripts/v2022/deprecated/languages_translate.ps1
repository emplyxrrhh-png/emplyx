$executionTimer = [system.diagnostics.stopwatch]::startNew()
$initial_path = (Get-Item .).FullName
$initialDate = (Get-Date)

Set-MpPreference -DisableRealtimeMonitoring $true
Write-Host "Desactivando Windows defender RealTime monitoring"

cd ../../../../../../Common/LanguageTools/

Write-Host "Generando ficheros de idioma"
./generate_resources_translate.ps1 

$zipFile = "C:\temp\translate.zip"

if (Test-Path $zipFile) {
	Remove-Item -Path $zipFile -Force
}
Add-Type -AssemblyName System.IO.Compression.FileSystem
$compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
$includeBaseDirectory = $false
[System.IO.Compression.ZipFile]::CreateFromDirectory("C:\temp\translate",$zipFile,$compressionLevel,$includeBaseDirectory)

Write-Host "...OK"

Set-MpPreference -DisableRealtimeMonitoring $false
Write-Host "Activando Windows defender RealTime monitoring"

cd $initial_path

$executionTimer.Stop()
Write-Host "Version generated in $($executionTimer.Elapsed.TotalMinutes) minutes"
