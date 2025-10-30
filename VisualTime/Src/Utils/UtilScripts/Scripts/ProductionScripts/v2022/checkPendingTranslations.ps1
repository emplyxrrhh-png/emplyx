$initial_path = (Get-Item .).FullName

function Test-FileEmpty([string] $file) {
  if ((Test-Path -LiteralPath $file) -and !((Get-Content -LiteralPath $file -Raw) -match '\S')) {return "OK"} else {return "KO"}

}
$fileName = $initial_path + '\..\..\..\..\..\..\Src\Resources\language_missing_tags.txt' 


return Test-FileEmpty $fileName