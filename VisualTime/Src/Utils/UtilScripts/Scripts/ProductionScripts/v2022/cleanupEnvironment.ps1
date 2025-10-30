$initial_path = (Get-Item .).FullName

Write-Host "Limpiando anteriores builds"
Get-ChildItem ($initial_path + '\..\..\..\..\..\..\Src\') -include bin,obj -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
Write-Host "... Intermediate folders OK"
Get-ChildItem ('C:\Work\VisualTimeMTDeploy') | foreach ($_) { remove-item $_.fullname -Force -Recurse }
Write-Host "... Last releases cleanup OK"
