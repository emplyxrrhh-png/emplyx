$executionTimer = [system.diagnostics.stopwatch]::startNew()
$initial_path = (Get-Item .).FullName
$initialDate = (Get-Date)

cd ../../../../../../Common/roLngTools/

Write-Host "Generando ficheros de idioma"
./runLanguageTool.ps1 
Write-Host "...OK"

cd $initial_path

$executionTimer.Stop()
Write-Host "Version generated in $($executionTimer.Elapsed.TotalMinutes) minutes"
