  
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


$versionNumber = ((Get-Content -Path .\..\Version.dat -TotalCount 2)[-1]).Replace('Current=','').Split(' ')[0]

Write-Host ("Se va a generar la siguiente release(" + $versionNumber + ")")
Write-Host ("... Compilación idiomas:" + $generateLang)
Write-Host ("... Plataforma zip:" + $generateMT)
Write-Host ("... Publicar en slot:" + $publishSlots)
Write-Host ("... Log verbose:" + $global:logVerbose)
Write-Host ("... Upload scripts:" + $uploadScripts)
Write-Host "Pulsa enter para continuar o CTRL+C para cancelar"
$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") > $null

$initial_path = (Get-Item .).FullName

Set-MpPreference -DisableRealtimeMonitoring $true
Write-Host "Desactivando Windows defender RealTime monitoring"



function compileWebSiteMTPlataforma([string] $solution, [string] $folderName){
    Write-Host ("... ... Publishing " + $folderName)

    $command = {msbuild $solution /p:Configuration=Multitennant /p:PublishProfile="VisualTimeMTDeploy.pubxml" /p:DeployOnBuild=true }
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

    Write-Host "... Limpiando anteriores builds"
    $solutionpath= $initial_path + '\..\..\..\..\..\..\Src\VisualTime Full MT.sln'
    msbuild.exe $solutionpath /p:configuration=Debug /t:clean  | Out-Null
    Write-Host "... ... Debug OK"
    msbuild.exe $solutionpath /p:configuration=Multitennant /t:clean  | Out-Null
    Write-Host "... ... Multitennant OK"
    msbuild.exe $solutionpath /p:configuration=Release /t:clean   | Out-Null
    Write-Host "... ... Release OK"
    Get-ChildItem ($initial_path + '\..\..\..\..\..\..\Src\') -include bin,obj -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
    Write-Host "... ... Intermediate folders OK"
    if( Test-Path -Path 'C:\Work\VisualTimeMTDeploy' ){
        Remove-Item –path 'C:\Work\VisualTimeMTDeploy' –recurse
    }
    Write-Host "... ... Deploy folder OK"

    
    Write-Host "... Generando zips para Multitenant plataforma"
    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\VTLiveApi\VTLiveApi.vbproj'
    compileWebSiteMTPlataforma $currentProj 'VTLiveApi' 

    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\VTLive40\VTLive40.vbproj'
    compileWebSiteMTPlataforma $currentProj 'VTLive' 

    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\VTPortalWeb\VTPortalWeb.vbproj'
    compileWebSiteMTPlataforma $currentProj 'VTPortal' 

    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\VTTerminalsPushServer\TerminalsPushServer.vbproj'
    compileWebSiteMTPlataforma $currentProj 'VTTerminals' 

    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\LiveVisits\Visits.vbproj'
    compileWebSiteMTPlataforma $currentProj 'VTVisits' 

    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\Functions\roAnalyticsFunctions\roAnalyticsFunctions.csproj'
    compileFunctionMTPlataforma $currentProj 'fnAnalyticsFunction' 

    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\Functions\roBackgroundFunctions\roBackgroundFunctions.csproj'
    compileFunctionMTPlataforma $currentProj 'fnBackgroundFunction' 

    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\Functions\roBroadcasterFunction\roBroadcasterFunction.csproj'
    compileFunctionMTPlataforma $currentProj 'fnBroadcasterFunction' 

    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\Functions\roDatalinkFunctions\roDatalinkFunctions.csproj'
    compileFunctionMTPlataforma $currentProj 'fnDatalinkFunction' 

    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\Functions\roMailFunction\roMailFunction.csproj'
    compileFunctionMTPlataforma $currentProj 'fnMailFunction' 

    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\Functions\roReportFunctions\roReportFunctions.csproj'
    compileFunctionMTPlataforma $currentProj 'fnReportFunction' 

    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\Functions\roScheduleFunctions\roScheduleFunctions.csproj'
    compileFunctionMTPlataforma $currentProj 'fnScheduleFunction' 

    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\Functions\roNotificationsFunction\roNotificationsFunction.csproj'
    compileFunctionMTPlataforma $currentProj 'fnNotificationsFunction'

    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\Functions\roPushNotificationsFunction\roPushNotificationsFunction.csproj'
    compileFunctionMTPlataforma $currentProj 'fnPushNotificationsFunction'

    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\Functions\roEngineFunctions\roEngineFunctions.csproj'
    compileFunctionMTPlataforma $currentProj 'fnEngineFunction'

    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\Functions\roPunchConnectorFunctions\roPunchConnectorFunctions.csproj'
    compileFunctionMTPlataforma $currentProj 'fnPunchConnectorFunction'

    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\Functions\roSCFunction\roSCFunction.csproj'
    compileFunctionMTPlataforma $currentProj 'fnpnlinkfunction'

    $currentProj = $initial_path + '\..\..\..\..\..\..\Src\Functions\roFTPStorageSynchronizationFunction\roFTPStorageSynchronizationFunction.csproj'
    compileFunctionMTPlataforma $currentProj 'fnFTPStorageSyncFunction'


    Write-Host "... ... Copiando Database Legacy Updates"
    mkdir ('c:\temp\latestVersion\Scripts\') -Force > $null
    $copyFolder = $initial_path + '\..\..\..\..\..\..\Src\Common\DBScript\SQLUpgradeVersion\_Antiguos\'
    Copy-Item $copyFolder\*.sql ('c:\temp\latestVersion\Scripts\') -Recurse -Force
    Write-Host "... ... OK"

    Write-Host "... ... Copiando Database Updates"
    $copyFolder = $initial_path + '\..\..\..\..\..\..\Src\Common\DBScript\SQLUpgradeVersion\Updates\'
    Copy-Item $copyFolder\*.sql ('c:\temp\latestVersion\Scripts\') -Recurse -Force
    Copy-Item $copyFolder\*.vtu ('c:\temp\latestVersion\Scripts\') -Recurse -Force
    Write-Host "... ... OK"

    Write-Host "... ... Copiando Informes DX"
    mkdir ('c:\temp\latestVersion\Informes\') -Force > $null
    $copyFolder = $initial_path + '\..\..\..\..\..\..\Src\Common\DBScript\SQLUpgradeVersion\Informes DX\'
    Copy-Item $copyFolder\*.sql ('c:\temp\latestVersion\Informes\') -Recurse -Force
    Copy-Item $copyFolder\*.vtu ('c:\temp\latestVersion\Informes\') -Recurse -Force
    Write-Host "... ... OK"

    Write-Host "... ... Copiando plantillas Import-Export"
    mkdir ('c:\temp\latestVersion\templates\') -Force > $null
    Copy-Item ($initial_path + '\..\Templates\*.xlsx') ('c:\temp\latestVersion\templates\') -Recurse -Force
    Write-Host "... ... OK"

    cd $initial_path

    Write-Host "...OK"

    Write-Host "... Limpiando directorios temporales"
    Remove-Item –path 'C:\Work\VisualTimeMTDeploy' –recurse
    Write-Host "... ... OK"


    $executionTimer.Stop()
    Write-Host "Multitenant Zip plataforma published in $($executionTimer.Elapsed.TotalMinutes) minutes"
}

if( $publishSlots )
{

    $publishSlotsArray = $publishSlots.Split(",")

    
    Login-AzureRmAccount -TenantId "95b5bebd-9caa-4973-a257-8c0a8bc34c04" -SubscriptionId "2575c2c8-2863-425e-ba9f-91da47dc2ee6"  | out-null
    az login --tenant 95b5bebd-9caa-4973-a257-8c0a8bc34c04 | out-null
    az account set --subscription "Entornos"  | out-null

    $executionTimer = [system.diagnostics.stopwatch]::startNew()
    Foreach ($publishSlot in $publishSlotsArray)
    {
        Write-Host ("Iniciando publicación Multitenant en slot:" + $publishSlot) 

        if($uploadScripts -eq "true")
        {
            $storageName = ("romt" + $publishSlot + "storage")
	        $Keys = Get-AzureRmStorageAccountKey -ResourceGroupName mtdev -Name $storageName
	        $StorageContext = New-AzureStorageContext -StorageAccountName $storageName -StorageAccountKey $Keys.Item(0).value;

            Write-Host ("... Copiando ficheros update a storage:" + $storageName)
            Get-ChildItem -Path ('c:\temp\latestVersion\Scripts\') -Recurse -File | ForEach-Object {
		        $UploadFile = @{
			        Context = $StorageContext;
			        Container = 'upgrade';
                    Blob = ("updates/" + $_.Name);
			        File = $_.FullName;
		        }
		        Set-AzureStorageBlobContent @UploadFile -Force | out-null;
            }
            Write-Host "... ... OK"

	        Write-Host ("... Copiando ficheros report a storage:" + $storageName)
            Get-ChildItem -Path ('c:\temp\latestVersion\Informes\') -Recurse -File | ForEach-Object {
		        $UploadFile = @{
			        Context = $StorageContext;
			        Container = 'upgrade';
                    Blob = ("reports/" + $_.Name);
			        File = $_.FullName;
		        }
		        Set-AzureStorageBlobContent @UploadFile -Force | out-null;
            }
            Write-Host "... ... OK"

            Write-Host ("... Copiando export templates a storage:" + $storageName)
            Get-ChildItem -Path ('c:\temp\latestVersion\templates\') -Recurse -File | ForEach-Object {
		        $UploadFile = @{
			        Context = $StorageContext;
			        Container = 'datalink';
                    Blob = ("common_templates/" + $_.Name);
			        File = $_.FullName;
		        }
		        Set-AzureStorageBlobContent @UploadFile -Force | out-null;
            }
            Write-Host "... ... OK"
        }

        
        Write-Host "... ... Publishing appservices"
        Write-Host "... ... ... fnanalyticsfunctiondevv2"
        az webapp deploy --type zip --resource-group mtdev --name fnanalyticsfunctiondevv2 --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnAnalyticsFunction.' + $versionNumber + '.zip') | out-null
        Write-Host "... ... ... fnbackgroundfunctiondevv2"
        az webapp deploy --type zip --resource-group mtdev --name fnbackgroundfunctiondevv2 --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnBackgroundFunction.' + $versionNumber + '.zip') | out-null
        Write-Host "... ... ... fnbroadcasterfunctiondevv2"
        az webapp deploy --type zip --resource-group mtdev --name fnbroadcasterfunctiondevv2 --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnBroadcasterFunction.' + $versionNumber + '.zip') | out-null
        Write-Host "... ... ... fndatalinkfunctiondevv2"
        az webapp deploy --type zip --resource-group mtdev --name fndatalinkfunctiondevv2 --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnDatalinkFunction.' + $versionNumber + '.zip') | out-null
        Write-Host "... ... ... fnmailfunctiondevv2"
        az webapp deploy --type zip --resource-group mtdev --name fnmailfunctiondevv2 --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnMailFunction.' + $versionNumber + '.zip') | out-null
        Write-Host "... ... ... fnreportfunctiondevv2"
        az webapp deploy --type zip --resource-group mtdev --name fnreportfunctiondevv2 --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnReportFunction.' + $versionNumber + '.zip') | out-null
        Write-Host "... ... ... fnschedulefunctiondevv2"
        az webapp deploy --type zip --resource-group mtdev --name fnschedulefunctiondevv2 --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnScheduleFunction.' + $versionNumber + '.zip') | out-null
        Write-Host "... ... ... fnnotificationsfunctiondevv2"
        az webapp deploy --type zip --resource-group mtdev --name fnnotificationsfunctiondevv2 --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnNotificationsFunction.' + $versionNumber + '.zip') | out-null
        Write-Host "... ... ... fnpushnotificationsfunctiondevv2"
        az webapp deploy --type zip --resource-group mtdev --name fnpushfunctiondevv2 --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnPushNotificationsFunction.' + $versionNumber + '.zip') | out-null
        Write-Host "... ... ... fnenginefunctiondevv2"
        az webapp deploy --type zip --resource-group mtdev --name fnenginefunctiondevv2 --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnEngineFunction.' + $versionNumber + '.zip') | out-null
        Write-Host "... ... ... fnPunchConnectorFunctiondevv2"
        az webapp deploy --type zip --resource-group mtdev --name fnconnectorFunctiondevv2 --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnPunchConnectorFunction.' + $versionNumber + '.zip') | out-null
        Write-Host "... ... ... fnpnlinkfunctiondevv2"
        az webapp deploy --type zip --resource-group mtdev --name fnpnlinkfunctiondevv2 --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnpnlinkfunction.' + $versionNumber + '.zip') | out-null
        Write-Host "... ... ... fnFTPStorageSyncFunctiondevv2"
        az webapp deploy --type zip --resource-group mtdev --name fnFTPSyncFunctiondevv2 --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnFTPStorageSyncFunction.' + $versionNumber + '.zip') | out-null
        
        Write-Host "... ... ... vtlivedev"
        az webapp deploy --type zip --resource-group mtdev --name vtlivedev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\VTLive.' + $versionNumber + '.zip') | out-null
        Write-Host "... ... ... vtliveapidev"        
        az webapp deploy --type zip --resource-group mtdev --name vtliveapidev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\VTLiveApi.' + $versionNumber + '.zip') | out-null
        Write-Host "... ... ... vtportaldev"
        az webapp deploy --type zip --resource-group mtdev --name vtportaldev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\VTPortal.' + $versionNumber + '.zip') | out-null
        Write-Host "... ... ... vtterminalsdev"
        az webapp deploy --type zip --resource-group mtdev --name vtterminalsdev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\VTTerminals.' + $versionNumber + '.zip')  | out-null
        Write-Host "... ... ... vtvisitsdev"
        az webapp deploy --type zip --resource-group mtdev --name vtvisitsdev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\VTVisits.' + $versionNumber + '.zip') | out-null
        
        Write-Host "... ... OK"
    }
    
    az logout
    $executionTimer.Stop()

    Write-Host "Multitenant Zip plataforma published in $($executionTimer.Elapsed.TotalMinutes) minutes"
}else{
    Write-Host "No se ha indicado slot para publicar"
}

Set-MpPreference -DisableRealtimeMonitoring $false
Write-Host "Activando Windows defender RealTime monitoring"