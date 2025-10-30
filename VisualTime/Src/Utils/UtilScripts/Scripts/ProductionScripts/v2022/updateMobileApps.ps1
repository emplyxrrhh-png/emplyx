$actualVersion=""
$nextVersion=""
$actualCodeVersion=""
$nextCodeVersion=""

if ( $args.count -eq 4 ){
    $actualVersion= $args[0].ToString().ToLower()
    $nextVersion = $args[1].ToString().ToLower()
    $actualCodeVersion=$args[2].ToString().ToLower()
    $nextCodeVersion=$args[3].ToString().ToLower()

} else{
    Write-Output "Atributtes missing: correct usage 'updateMobileApps.ps1 actualVersion=3.33.2 nextVersion=3.33.3 actualCodeVersion=462 nextCodeVersion=463'"
    exit
}

Write-Host ("Se van a actualizar los ficheros de la aplicación mobil")
Write-Host ("... Versión actual:" + $actualVersion)
Write-Host ("... Nueva versión:" + $nextVersion)
Write-Host ("... Code version:" + $actualCodeVersion)
Write-Host ("... Nueva code version:" + $nextCodeVersion)
Write-Host "Pulsa enter para continuar o CTRL+C para cancelar"
$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") > $null


Write-Host "... ... Copiando ficheros Android"
copy ".\..\..\..\..\..\VTPortalWeb\index.html" ".\..\..\..\..\..\VTPortalJS\deploy Android\www\index.html" > $null
copy ".\..\..\..\..\..\VTPortalWeb\index.js" ".\..\..\..\..\..\VTPortalJS\deploy Android\www\index.js" > $null
xcopy ".\..\..\..\..\..\VTPortalWeb\js" ".\..\..\..\..\..\VTPortalJS\deploy Android\www\js" /sy > $null
xcopy ".\..\..\..\..\..\VTPortalWeb\1" ".\..\..\..\..\..\VTPortalJS\deploy Android\www\1" /sy > $null
xcopy ".\..\..\..\..\..\VTPortalWeb\2" ".\..\..\..\..\..\VTPortalJS\deploy Android\www\2" /sy > $null

del ".\..\..\..\..\..\VTPortalJS\deploy Android\www\2\indexv2.aspx" > $null
del ".\..\..\..\..\..\VTPortalJS\deploy Android\www\2\indexv2.aspx.designer.vb" > $null
del ".\..\..\..\..\..\VTPortalJS\deploy Android\www\2\indexv2.aspx.vb" > $null


Write-Host "... ... Copiando ficheros IOS"
copy ".\..\..\..\..\..\VTPortalWeb\index.html" ".\..\..\..\..\..\VTPortalJS\deploy iOS\www\index.html" > $null
copy ".\..\..\..\..\..\VTPortalWeb\index.js" ".\..\..\..\..\..\VTPortalJS\deploy iOS\www\index.js" > $null
xcopy ".\..\..\..\..\..\VTPortalWeb\js" ".\..\..\..\..\..\VTPortalJS\deploy iOS\www\js" /sy > $null
xcopy ".\..\..\..\..\..\VTPortalWeb\1" ".\..\..\..\..\..\VTPortalJS\deploy iOS\www\1" /sy > $null
xcopy ".\..\..\..\..\..\VTPortalWeb\2" ".\..\..\..\..\..\VTPortalJS\deploy iOS\www\2" /sy > $null

del ".\..\..\..\..\..\VTPortalJS\deploy iOS\www\2\indexv2.aspx" > $null
del ".\..\..\..\..\..\VTPortalJS\deploy iOS\www\2\indexv2.aspx.designer.vb" > $null
del ".\..\..\..\..\..\VTPortalJS\deploy iOS\www\2\indexv2.aspx.vb" > $null

Write-Host "... ... Actualizando config.xml Android"
# Ruta al archivo
$rutaArchivo = ".\..\..\..\..\..\VTPortalJS\deploy Android\config.xml"

# Lee el contenido del archivo
$contenido = Get-Content -Path $rutaArchivo

# Busca la línea que contiene la versión y actualízala
$contenido = $contenido -replace ('version="' + $actualVersion + '"'), ('version="' + $nextVersion + '"')
$contenido = $contenido -replace "<string>$actualVersion</string>", "<string>$nextVersion</string>"
        
$contenido = $contenido -replace ('ios-CFBundleVersion="' + $actualCodeVersion + '"'), ('ios-CFBundleVersion="' + $nextCodeVersion + '"')
$contenido = $contenido -replace ('versionCode="' + $actualCodeVersion + '"'), ('versionCode="' + $nextCodeVersion + '"')

# Guarda el contenido actualizado en el archivo
$contenido | Set-Content -Path $rutaArchivo



Write-Host "... ... Actualizando config.xml iOS"
$rutaArchivo = ".\..\..\..\..\..\VTPortalJS\deploy iOS\config.xml"

# Lee el contenido del archivo
$contenido = Get-Content -Path $rutaArchivo

# Busca la línea que contiene la versión y actualízala
$contenido = $contenido -replace ('version="' + $actualVersion + '"'), ('version="' + $nextVersion + '"')
$contenido = $contenido -replace "<string>$actualVersion</string>", "<string>$nextVersion</string>"
        
$contenido = $contenido -replace ('ios-CFBundleVersion="' + $actualCodeVersion + '"'), ('ios-CFBundleVersion="' + $nextCodeVersion + '"')
$contenido = $contenido -replace ('versionCode="' + $actualCodeVersion + '"'), ('versionCode="' + $nextCodeVersion + '"')

# Guarda el contenido actualizado en el archivo
$contenido | Set-Content -Path $rutaArchivo
