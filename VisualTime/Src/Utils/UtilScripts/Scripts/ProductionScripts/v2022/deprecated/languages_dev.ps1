$executionTimer = [system.diagnostics.stopwatch]::startNew()
$initial_path = (Get-Item .).FullName
$initialDate = (Get-Date)

Set-MpPreference -DisableRealtimeMonitoring $true
Write-Host "Desactivando Windows defender RealTime monitoring"

cd ../../../../../../Common/LanguageTools/

Write-Host "Generando ficheros de idioma"
./generate_resources.ps1 
Write-Host "...OK"

Set-MpPreference -DisableRealtimeMonitoring $false
Write-Host "Activando Windows defender RealTime monitoring"

cd $initial_path

$executionTimer.Stop()
Write-Host "Version generated in $($executionTimer.Elapsed.TotalMinutes) minutes"
