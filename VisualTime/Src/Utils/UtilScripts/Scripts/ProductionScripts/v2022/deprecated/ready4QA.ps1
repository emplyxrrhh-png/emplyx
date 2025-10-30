  
$generateLang="true"
$generateMT="true"
$publishSlot="dev"
$slotAction="stop"
$global:logVerbose="false"

$versionNumber = ((Get-Content -Path .\..\Version.dat -TotalCount 2)[-1]).Replace('Current=','').Split(' ')[0]

Write-Host ("Se va a generar la siguiente release(" + $versionNumber + ")")
Write-Host ("... Compilación idiomas:" + $generateLang)
Write-Host ("... Plataforma zip:" + $generateMT)
Write-Host ("... Publicar en slot:" + $publishSlot)
Write-Host ("... Log verbose:" + $global:logVerbose)
Write-Host "Pulsa enter para continuar o CTRL+C para cancelar"
$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") > $null

if( Test-Path -Path 'c:\temp\latestVersion' ){
    Remove-Item –path 'c:\temp\latestVersion' –recurse
}

$initial_path = (Get-Item .).FullName

Set-MpPreference -DisableRealtimeMonitoring $true
Write-Host "Desactivando Windows defender RealTime monitoring"

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

if( $publishSlot )
{
    Write-Host ("Iniciando publicación Multitenant en slot:" + $publishSlot) 
    $executionTimer = [system.diagnostics.stopwatch]::startNew()

    $storageName = "rsdevnextstorage"

    Login-AzureRmAccount
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
		Set-AzureStorageBlobContent @UploadFile -Force;
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
		Set-AzureStorageBlobContent @UploadFile -Force;
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
		Set-AzureStorageBlobContent @UploadFile -Force;
    }
    Write-Host "... ... OK"

    az login
    az account set --subscription "Entornos"

    $slotAction="stop"

    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnschedulefunctiondev/slots/$publishSlot


    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnanalyticsfunctiondev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnbackgroundfunctiondev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnbroadcasterfunctiondev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fndatalinkfunctiondev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnmailfunctiondev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnreportfunctiondev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnnotificationsfunctiondev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnpushnotificationsfunctiondev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnenginefunction/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnPunchConnectorFunctiondev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/vtvisitsdev/slots/$publishSlot


    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/vtliveapidev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/vtlivedev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/vtportaldev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/vtterminalsdev/slots/$publishSlot
    
    az webapp deploy --type zip --resource-group mtdev --name fnanalyticsfunctiondev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnAnalyticsFunction.' + $versionNumber + '.zip')
    az webapp deploy --type zip --resource-group mtdev --name fnbackgroundfunctiondev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnBackgroundFunction.' + $versionNumber + '.zip')
    az webapp deploy --type zip --resource-group mtdev --name fnbroadcasterfunctiondev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnBroadcasterFunction.' + $versionNumber + '.zip')
    az webapp deploy --type zip --resource-group mtdev --name fndatalinkfunctiondev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnDatalinkFunction.' + $versionNumber + '.zip')
    az webapp deploy --type zip --resource-group mtdev --name fnmailfunctiondev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnMailFunction.' + $versionNumber + '.zip')
    az webapp deploy --type zip --resource-group mtdev --name fnreportfunctiondev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnReportFunction.' + $versionNumber + '.zip')
    az webapp deploy --type zip --resource-group mtdev --name fnschedulefunctiondev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnScheduleFunction.' + $versionNumber + '.zip')
    az webapp deploy --type zip --resource-group mtdev --name fnnotificationsfunctiondev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnNotificationsFunction.' + $versionNumber + '.zip')
    az webapp deploy --type zip --resource-group mtdev --name fnpushnotificationsfunctiondev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnPushNotificationsFunction.' + $versionNumber + '.zip')
    az webapp deploy --type zip --resource-group mtdev --name fnenginefunction --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnEngineFunction.' + $versionNumber + '.zip')
    az webapp deploy --type zip --resource-group mtdev --name fnPunchConnectorFunctiondev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\fnPunchConnectorFunction.' + $versionNumber + '.zip')

    az webapp deploy --type zip --resource-group mtdev --name vtliveapidev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\VTLiveApi.' + $versionNumber + '.zip')
    az webapp deploy --type zip --resource-group mtdev --name vtlivedev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\VTLive.' + $versionNumber + '.zip')
    az webapp deploy --type zip --resource-group mtdev --name vtportaldev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\VTPortal.' + $versionNumber + '.zip')
    az webapp deploy --type zip --resource-group mtdev --name vtterminalsdev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\VTTerminals.' + $versionNumber + '.zip')
    az webapp deploy --type zip --resource-group mtdev --name vtvisitsdev --slot $publishSlot --src-path ('c:\temp\latestVersion\plataforma\VTVisits.' + $versionNumber + '.zip')

    $slotAction="start"

    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnanalyticsfunctiondev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnbackgroundfunctiondev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnbroadcasterfunctiondev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fndatalinkfunctiondev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnmailfunctiondev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnreportfunctiondev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnnotificationsfunctiondev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnpushnotificationsfunctiondev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnenginefunction/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnPunchConnectorFunctiondev/slots/$publishSlot

    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/vtliveapidev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/vtlivedev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/vtportaldev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/vtterminalsdev/slots/$publishSlot
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/vtterminalsdev/slots/$publishSlot

    
    az resource invoke-action --action $slotAction --ids  /subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/mtdev/providers/Microsoft.Web/sites/fnschedulefunctiondev/slots/$publishSlot

    az logout

    $executionTimer.Stop()
    Write-Host "Multitenant Zip plataforma published in $($executionTimer.Elapsed.TotalMinutes) minutes"
}else{
    Write-Host "No se ha indicado slot para publicar"
}

Set-MpPreference -DisableRealtimeMonitoring $false
Write-Host "Activando Windows defender RealTime monitoring"