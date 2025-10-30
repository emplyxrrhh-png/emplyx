$initial_path = (Get-Item .).FullName
$result = "OK"

if( $result -eq "OK"){
    # Ruta al archivo
    $rutaArchivo = $initial_path + '\..\..\..\..\..\..\Src\VTPortalJS\bundleconfig.json' 

    # Lee el contenido del archivo
    $contenido = Get-Content -Path $rutaArchivo
    if ($contenido -contains ('      "enabled": false,')) {
        $result = "KO"
    } else {
        $result = "OK"
    }
}

if( $result -eq "OK"){
    # Ruta al archivo
    $rutaArchivo = $initial_path + '\..\..\..\..\..\..\Src\VTVisitsJS\bundleconfig.json' 

    # Lee el contenido del archivo
    $contenido = Get-Content -Path $rutaArchivo
    if ($contenido -contains ('      "enabled": false,')) {
        $result = "KO"
    } else {
        $result = "OK"
    }
}

if( $result -eq "OK"){
    # Ruta al archivo
    $rutaArchivo = $initial_path + '\..\..\..\..\..\..\Src\VTLive40\bundleconfig.json' 

    # Lee el contenido del archivo
    $contenido = Get-Content -Path $rutaArchivo
    if ($contenido -contains ('      "enabled": false,')) {
        $result = "KO"
    } else {
        $result = "OK"
    }
}
return $result