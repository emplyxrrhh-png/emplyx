  
$generateLang="true"
$publishSlot=""
$uploadScripts=""
$global:logVerbose="false"


if ( $args.count -eq 4 ){
    $generateLang=$args[0].ToLower()
    $global:logVerbose = $args[1].ToLower()
    $publishSlot=$args[2].ToLower()
    $uploadScripts=$args[3].ToLower()

    if(!($generateLang -eq "false")){ $generateLang ='true'}
    if(!($global:logVerbose -eq "false")){ $global:logVerbose ='true'}
    if(!($uploadScripts -eq "false")){ $uploadScripts ='true'}

} else{
    Write-Output "Atributtes missing: correct usage 'generate_version.ps1 generateLang=true verbose=false slot=idi01 uploadScripts=true'"
    exit
}

$versionNumber = ((Get-Content -Path .\..\..\Version.dat -TotalCount 2)[-1]).Replace('Current=','').Split(' ')[0]

Write-Host ("Se va a generar las funciones para runtime local(" + $versionNumber + ")")
Write-Host ("... Compilación idiomas:" + $generateLang)
Write-Host ("... Subir scripts:" + $uploadScripts)
Write-Host ("... Storage donde subir:" + $publishSlot)
Write-Host ("... Log verbose:" + $global:logVerbose)
Write-Host "Pulsa enter para continuar o CTRL+C para cancelar"
$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") > $null

$initial_path = (Get-Item .).FullName

function compileFunctionMTPlataforma([string] $solution, [string] $folderName){
    Write-Host ("... ... Cleaning old folder " + $folderName)
    $oldFolder = 'c:\work\VisualTime\fnProc\' + $folderName
    if( Test-Path -Path $oldFolder ){
        Remove-Item –path $oldFolder –recurse
    }
    Write-Host "... ... OK"

    Write-Host ("... ... Publishing " + $folderName)
    
    $command = {msbuild $solution -restore /p:Configuration=Multitennant /p:DeployOnBuild=true /p:PublishProfile="VisualTimeMTDeploy.pubxml" /p:WebPublishMethod=FileSystem }
    if ($global:logVerbose -eq "true") {& $command} else {& $command | Out-Null}
    Write-Host "... ... OK"
    
    $sourcePath = ('C:\Work\VisualTimeMTDeploy\' + $folderName)
    $destinationPath = ('c:\work\VisualTime\fnProc')
    
    mkdir $destinationPath -Force > $null

    Write-Host ("... ... Installing function on directory: c:\work\VisualTime\fnProc\" + $folderName)
    Move-Item $sourcePath -Destination $destinationPath
    
    $sourceConfig = ('c:\work\VisualTime\fnProc\local.settings.json' )
    $destConfig = ('c:\work\VisualTime\fnProc\' + $folderName + '\local.settings.json')

    Copy-Item -Path $sourceConfig -Destination $destConfig


    Write-Host "... ... OK"
}

if( Test-Path -Path 'c:\temp\latestVersion' ){
    Remove-Item –path 'c:\temp\latestVersion' –recurse
}

if( $generateLang -eq "true")
{
    
    $tempLangDirectory = $initial_path + '\..\..\..\..\..\..\..\Src\Utils\roLanguageTools\tempFiles'
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

Write-Host "Iniciando publicación local para funciones"
$executionTimer = [system.diagnostics.stopwatch]::startNew()

Write-Host "... Limpiando anteriores builds"
$solutionpath= $initial_path + '\..\..\..\..\..\..\..\Src\VisualTime Full MT.sln'
msbuild.exe $solutionpath /p:configuration=Debug /t:clean  | Out-Null
Write-Host "... ... Debug OK"
msbuild.exe $solutionpath /p:configuration=Multitennant /t:clean  | Out-Null
Write-Host "... ... Multitennant OK"
msbuild.exe $solutionpath /p:configuration=Release /t:clean   | Out-Null
Write-Host "... ... Release OK"
Get-ChildItem ($initial_path + '\..\..\..\..\..\..\..\Src\') -include bin,obj -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
Write-Host "... ... Intermediate folders OK"
if( Test-Path -Path 'C:\Work\VisualTimeMTDeploy' ){
    Remove-Item –path 'C:\Work\VisualTimeMTDeploy' –recurse
}
Write-Host "... ... Deploy folder OK"

    
$currentProj = $initial_path + '\..\..\..\..\..\..\..\Src\Functions\roAnalyticsFunctions\roAnalyticsFunctions.csproj'
compileFunctionMTPlataforma $currentProj 'fnAnalyticsFunction' 

$currentProj = $initial_path + '\..\..\..\..\..\..\..\Src\Functions\roBackgroundFunctions\roBackgroundFunctions.csproj'
compileFunctionMTPlataforma $currentProj 'fnBackgroundFunction' 

$currentProj = $initial_path + '\..\..\..\..\..\..\..\Src\Functions\roBroadcasterFunction\roBroadcasterFunction.csproj'
compileFunctionMTPlataforma $currentProj 'fnBroadcasterFunction' 

$currentProj = $initial_path + '\..\..\..\..\..\..\..\Src\Functions\roDatalinkFunctions\roDatalinkFunctions.csproj'
compileFunctionMTPlataforma $currentProj 'fnDatalinkFunction' 

$currentProj = $initial_path + '\..\..\..\..\..\..\..\Src\Functions\roMailFunction\roMailFunction.csproj'
compileFunctionMTPlataforma $currentProj 'fnMailFunction' 

$currentProj = $initial_path + '\..\..\..\..\..\..\..\Src\Functions\roReportFunctions\roReportFunctions.csproj'
compileFunctionMTPlataforma $currentProj 'fnReportFunction' 

$currentProj = $initial_path + '\..\..\..\..\..\..\..\Src\Functions\roScheduleFunctions\roScheduleFunctions.csproj'
compileFunctionMTPlataforma $currentProj 'fnScheduleFunction' 

$currentProj = $initial_path + '\..\..\..\..\..\..\..\Src\Functions\roNotificationsFunction\roNotificationsFunction.csproj'
compileFunctionMTPlataforma $currentProj 'fnNotificationsFunction'

$currentProj = $initial_path + '\..\..\..\..\..\..\..\Src\Functions\roPushNotificationsFunction\roPushNotificationsFunction.csproj'
compileFunctionMTPlataforma $currentProj 'fnPushNotificationsFunction'

$currentProj = $initial_path + '\..\..\..\..\..\..\..\Src\Functions\roEngineFunctions\roEngineFunctions.csproj'
compileFunctionMTPlataforma $currentProj 'fnEngineFunction'

$currentProj = $initial_path + '\..\..\..\..\..\..\..\Src\Functions\roPunchConnectorFunctions\roPunchConnectorFunctions.csproj'
compileFunctionMTPlataforma $currentProj 'fnPunchConnectorFunction'

$currentProj = $initial_path + '\..\..\..\..\..\..\..\Src\Functions\roSCFunction\roSCFunction.csproj'
compileFunctionMTPlataforma $currentProj 'fnpnlinkfunction'

$currentProj = $initial_path + '\..\..\..\..\..\..\..\Src\Functions\roFTPStorageSynchronizationFunction\roFTPStorageSynchronizationFunction.csproj'
compileFunctionMTPlataforma $currentProj 'fnFTPStorageSyncFunction'

Write-Host "... ... Copiando Database Updates"
mkdir ('c:\temp\latestVersion\Scripts\') -Force > $null
$copyFolder = $initial_path + '\..\..\..\..\..\..\..\Src\Common\DBScript\SQLUpgradeVersion\Updates\'
Copy-Item $copyFolder\*.sql ('c:\temp\latestVersion\Scripts\') -Recurse -Force
Copy-Item $copyFolder\*.vtu ('c:\temp\latestVersion\Scripts\') -Recurse -Force
Write-Host "... ... OK"

Write-Host "... ... Copiando Informes DX"
mkdir ('c:\temp\latestVersion\Informes\') -Force > $null
$copyFolder = $initial_path + '\..\..\..\..\..\..\..\Src\Common\DBScript\SQLUpgradeVersion\Informes DX\'
Copy-Item $copyFolder\*.sql ('c:\temp\latestVersion\Informes\') -Recurse -Force
Copy-Item $copyFolder\*.vtu ('c:\temp\latestVersion\Informes\') -Recurse -Force
Write-Host "... ... OK"

Write-Host "... ... Copiando plantillas Import-Export"
mkdir ('c:\temp\latestVersion\templates\') -Force > $null
Copy-Item ($initial_path + '\..\..\Templates\*.xlsx') ('c:\temp\latestVersion\templates\') -Recurse -Force
Copy-Item ($initial_path + '\..\..\Templates\*.pdf') ('c:\temp\latestVersion\templates\') -Recurse -Force
Write-Host "... ... OK"

cd $initial_path

Write-Host "...OK"

Write-Host "... Limpiando directorios temporales"
Remove-Item –path 'C:\Work\VisualTimeMTDeploy' –recurse
Write-Host "... ... OK"


if( $publishSlot )
{
    if($uploadScripts -eq "true")
    {
        Login-AzureRmAccount -TenantId "604e5547-f49c-4481-8476-c14f22fd79cb" -SubscriptionId "1040671c-f7e4-4d0f-a97b-6d0760799333"  | out-null

        $storageName = ("idistorage" + $publishSlot)
	    $Keys = Get-AzureRmStorageAccountKey -ResourceGroupName MTIdiResources -Name $storageName
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
}else{
    Write-Host "No se ha indicado slot para publicar"
}

$executionTimer.Stop()
Write-Host "Publicación local para funciones finalizada en $($executionTimer.Elapsed.TotalMinutes) minutes"
