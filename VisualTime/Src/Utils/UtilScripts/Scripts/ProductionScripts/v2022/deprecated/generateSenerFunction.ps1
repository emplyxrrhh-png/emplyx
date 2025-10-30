  
$generateLang="true"
$generateMT="true"
$publishSlots=""
$uploadScripts=""
$global:logVerbose="false"


if ( $args.count -eq 5 ){
    $generateLang=$args[0].ToLower()
    $generateMT=$args[1].ToLower()
    $publishSlots=$args[2].ToLower()
    $uploadScripts=$args[4].ToLower()
    $global:logVerbose = $args[3].ToLower()

    if(!($generateLang -eq "false")){ $generateLang ='true'}
    if(!($generateMT -eq "false")){ $generateMT ='true'}
    if(!($uploadScripts -eq "false")){ $uploadScripts ='true'}
    if(!($global:logVerbose -eq "false")){ $global:logVerbose ='true'}

} else{
    Write-Output "Atributtes missing: correct usage 'generate_version.ps1 generateLang=true generateMT=true slot=idi0x,idi0x,idi0x verbose=false uploadScripts=true'"
    exit
}


$versionNumber = "6.5.8.0"

Write-Host ("Se va a generar la siguiente release(" + $versionNumber + ")")
Write-Host ("... Compilación idiomas:" + $generateLang)
Write-Host ("... Plataforma zip:" + $generateMT)
Write-Host ("... Publicar en slot:" + $publishSlots)
Write-Host ("... Log verbose:" + $global:logVerbose)
Write-Host ("... Upload scripts:" + $uploadScripts)
Write-Host "Pulsa enter para continuar o CTRL+C para cancelar"
$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") > $null

$initial_path = (Get-Item .).FullName


function compileFunctionMTPlataforma([string] $solution, [string] $folderName){
    Write-Host ("... ... Publishing " + $folderName)
    
    $command = {msbuild $solution -restore /p:Configuration=Multitennant /p:DeployOnBuild=true /p:PublishProfile="VisualTimeMTDeploy.pubxml" /p:WebPublishMethod=FileSystem }
    if ($global:logVerbose -eq "true") {& $command} else {& $command | Out-Null}
    Write-Host "... ... OK"
    
    $sourcePath = ('C:\Work\VisualTimeMTDeploy\' + $folderName)
    $destinationPath = ('c:\temp\latestVersion\plataforma\' + $folderName + '.' + $versionNumber + '.zip')
    
    Write-Host ("... ... Generating zip " + $destinationPath)
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
    $includeBaseDirectory = $false
    [System.IO.Compression.ZipFile]::CreateFromDirectory("$sourcePath","$destinationPath",$compressionLevel,$includeBaseDirectory)
    Write-Host "... ... OK"
}

if( $generateMT -eq "true")
{
    if( Test-Path -Path 'c:\temp\latestVersion' ){
        Remove-Item –path 'c:\temp\latestVersion' –recurse
    }

    if( $generateLang -eq "true")
    {
    
        $tempLangDirectory = $initial_path + '\..\..\..\..\..\..\Src\Utils\roLanguageTools\tempFiles'
        mkdir $tempLangDirectory -Force > $null

        cd ../../../../../../Common/LanguageTools/

        Write-Host "Generando ficheros de idioma"
        ./generate_resources.ps1 > $null
        Write-Host "...OK"

        cd $initial_path
    }

    mkdir c:\temp\latestVersion -Force > $null
    mkdir c:\temp\latestVersion\plataforma -Force > $null
    mkdir c:\temp\latestVersion\saas -Force > $null

    Write-Host "Iniciando publicación Multitenant Zip plataforma"
    $executionTimer = [system.diagnostics.stopwatch]::startNew()

  
    Write-Host "... Generando zips para Multitenant plataforma"

    $currentProj = $initial_path + '\..\..\..\..\..\..\..\SrcTools\Functions\roSenerFunction\roSenerFunction.csproj'
    compileFunctionMTPlataforma $currentProj 'fnSenerFunction' 

   
    cd $initial_path

    Write-Host "...OK"

    $executionTimer.Stop()
    Write-Host "Multitenant Zip plataforma published in $($executionTimer.Elapsed.TotalMinutes) minutes"
}

Set-MpPreference -DisableRealtimeMonitoring $false
Write-Host "Activando Windows defender RealTime monitoring"