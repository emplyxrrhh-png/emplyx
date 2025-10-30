$initial_path = (Get-Item .).FullName
 
function Get-FileEncoding($Path) {
    $bytes = [byte[]](Get-Content $Path -Encoding byte -ReadCount 4 -TotalCount 4)

    if(!$bytes) { return 'utf8' }

    switch -regex ('{0:x2}{1:x2}{2:x2}{3:x2}' -f $bytes[0],$bytes[1],$bytes[2],$bytes[3]) {
        '^efbbbf'   { return 'utf8' }
        '^2b2f76'   { return 'utf7' }
        '^fffe'     { return 'unicode' }
        '^feff'     { return 'bigendianunicode' }
        '^0000feff' { return 'utf32' }
        default     { return 'ascii' }
    }
}

$scriptsPath = $initial_path + '\..\..\..\..\..\..\Src\Common\DBScript\SQLUpgradeVersion\Updates\'    
Write-Host ("... Validando formato de los ficheros de update")
$result = "OK"
Get-ChildItem -Path ($scriptsPath) -Recurse -File | ForEach-Object {
    $fileEncoding = Get-FileEncoding($_.FullName)
    
    if( $fileEncoding -ne "utf8" ){
        $result = "KO"
    }
    
    Write-Host ("Fichero:" + $_.Name + "(" + $fileEncoding + ")")    
}

return $result
