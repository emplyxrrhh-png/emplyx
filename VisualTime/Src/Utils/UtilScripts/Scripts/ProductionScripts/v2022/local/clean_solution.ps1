$initial_path = (Get-Item .).FullName

Write-Host "Limpiando anteriores builds"
$solutionpath= $initial_path + '\..\..\..\..\..\..\..\Src\VisualTime Full Solution.sln'
msbuild.exe $solutionpath /p:configuration=Debug /t:clean > $null
Write-Host "... Debug OK"
msbuild.exe $solutionpath /p:configuration=Multitennant /t:clean > $null
Write-Host "... Multitennant OK"
msbuild.exe $solutionpath /p:configuration=Release /t:clean > $null 
Write-Host "... Release OK"
Get-ChildItem ($initial_path + '\..\..\..\..\..\..\..\Src\') -include bin,obj -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
Write-Host "... Intermediate folders OK"
Get-ChildItem ($initial_path + '\..\..\..\..\..\..\..\Src\CommonBin\') | foreach ($_) { remove-item $_.fullname -Force -Recurse }
Write-Host "... CommonBin OK"
Get-ChildItem ($initial_path + '\..\..\..\..\..\..\..\Src\CommonProcs\') | foreach ($_) { remove-item $_.fullname -Force -Recurse }
Write-Host "... CommonProcs OK"
Get-ChildItem ($initial_path + '\..\..\..\..\..\..\..\Src\packages\') | foreach ($_) { remove-item $_.fullname -Force -Recurse }
Write-Host "... Nuget packages OK"
