  
$functionsName="fnAnalyticsIDI,fnBackgroundIdi,fnBroadcasterIdi,fnConnectorIdi,fnDatalinkIdi,fnEngineIdi,fnFTPsyncIdi,fnMailidi,fnNotificationsIdi,fnPNLinkIdi,fnPushIdi,fnReportIdi,fnScheduleIdi"
$appsName="vtliveapiidi,vtliveidi,vtportalidi,vtterminalsidi,vtvisitsidi"
$slotName=""
$netVersion="4"
$action="create"


if ( $args.count -eq 2 ){
    $slotName=$args[0].ToLower()
    $action=$args[1].ToLower()

} else{
    Write-Output "Atributtes missing: correct usage 'newEnvironment.ps1 slotname=idi0x action=create/delete"
    exit
}


Write-Host ("Se van a crear el siguiente entorno:")
Write-Host ("... Nomber del slot:" + $slotName)
Write-Host ("... Acción:" + $action)
Write-Host ("... Versions runtime:" + $netVersion)
Write-Host "Pulsa enter para continuar o CTRL+C para cancelar"
$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") > $null



if( !($slotName -eq "") )
{
    $executionTimer = [system.diagnostics.stopwatch]::startNew()

    az login --tenant 604e5547-f49c-4481-8476-c14f22fd79cb | out-null
    az account set --subscription "3128-DEV-VisualTime" | out-null

    $functionsNameArray = $functionsName.Split(",")
    $appsNameArray = $appsName.Split(",")

    $storageKey = "romtrcstorage"
    $appInisghtsName = "idiGlobalStatus"
    $appInisghtsKey = "780b34e0-365f-46ba-a0c5-f04eef55b463"
    $appInsightsSlotKey = ''
    $serviceBusSlotKey= ''
    $cosmosKey = "mongodb://idistagedb:yrk7jh2vi2SGLZxTplQZDmhM7VTrzDD6r5toa5sgNYGxrKIxoXPiQPh7HvMe7xDrawV9FDWhI2cwACDb4Z1M0w%3D%3D@idistagedb.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&retrywrites=false&maxIdleTimeMS=120000&appName=@idistagedb@"
        
    switch ($slotName) {
        "stage" {
            $appInsightsSlotKey = '6874716f-a2aa-4a11-8ef2-3a79165a5850'
            $storageSlotKey = 'DefaultEndpointsProtocol=https;AccountName=idistagestorage;AccountKey=RiRxqy+f+OS8O9IiMB41FCt62qa414XQ9cOBLv3kFdEDRagpo5MjBT6RoaRInRGZocRj+GwONyGp+AStcyWQ/A==;EndpointSuffix=core.windows.net'
            $serviceBusSlotKey = 'Endpoint=sb://idistagesb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=kMCHX3udHkpzUWXFZlQXYD6w2zzs+TtEN+ASbGUmibk='
        }
        default {
            $appInsightsSlotKey = ''
            $storageSlotKey = ''
            $serviceBusSlotKey = ''
        }
    }
    Write-Host ("... Creando entorno:" + $slotName) 

	Foreach ($functionName in $functionsNameArray)
    {
        
        if($action -eq "create"){
		    Write-Host ("... ... Creando función:" + $functionName) 
            if($netVersion -eq "4"){
		        az functionapp deployment slot create --name $functionName --resource-group mtdev --slot $slotName | out-null
            } else{
		        az functionapp deployment slot create --name $functionName --resource-group mtdev --slot $slotName | out-null
			    az functionapp config appsettings set --settings FUNCTIONS_EXTENSION_VERSION=~1 -g mtdev -n $functionName --slot $slotName | out-null
			    az functionapp config appsettings set --settings FUNCTIONS_WORKER_RUNTIME=dotnet -g mtdev -n $functionName --slot $slotName | out-null
                az functionapp config set --net-framework-version v4.0 --resource-group mtdev --name $functionName --slot $slotName | out-null
            }
            Write-Host ("... ... ... OK") 

            Write-Host ("... ... Configurando función:" + $slotName) 
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "ApiService.ConnectionString=https://haapiidi-stage.azurewebsites.net@vtserviceapi@bd1b83fd0cfd8b8bbe27903aa0cbf9a476332e4919f50c24f41cb0f33634f81c580315dba974928cd12a7cf0f3de76d499014e3b7d06201ada895b94ba3d5b81" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "DebugMode=false" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "VTLive.MultitenantService=true" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "WEBSITE_TIME_ZONE=Romance Standard Time" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings '"CosmosDB.ConnectionString='$cosmosKey'"' --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "CosmosDB.DBName=$slotName" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "ApplicationInsights.Key=$appInsightsSlotKey" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "AzureWebJobsServiceBus=$serviceBusSlotKey" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "AzureWebJobsDashboard=$storageSlotKey" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "AzureWebJobsStorage=$storageSlotKey" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "URL.VTLive=https://vtliveidi-stage.azurewebsites.net/" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "URL.VTPortal=https://vtportalidi-stage.azurewebsites.net/" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "KeyVaultUrl=https://pnintegration.vault.azure.net/" --slot $slotName | out-null
            Write-Host ("... ... ... OK") 

            az functionapp stop --name $functionName --resource-group mtdev --slot $slotName
        }elseif( $action -eq "delete"){
            Write-Host ("... ... Eliminando función:" + $functionName) 
            az functionapp deployment slot delete --name $functionName --resource-group mtdev --slot $slotName | out-null
            Write-Host ("... ... ... OK") 
        }
	}


    Foreach ($appName in $appsNameArray)
    {
        
        if($action -eq "create"){
		    Write-Host ("... ... Creando WebApp:" + $appName) 
            az webapp deployment slot create --name $appName --resource-group mtdev --slot $slotName | out-null
            Write-Host ("... ... ... OK") 

            Write-Host ("... ... Configurando WebApp:" + $slotName) 
            az webapp config appsettings set --name $appName --resource-group mtdev --slot-settings "ApiService.ConnectionString=https://haapiidi-stage.azurewebsites.net@vtserviceapi@bd1b83fd0cfd8b8bbe27903aa0cbf9a476332e4919f50c24f41cb0f33634f81c580315dba974928cd12a7cf0f3de76d499014e3b7d06201ada895b94ba3d5b81" --slot $slotName | out-null
            az webapp config appsettings set --name $appName --resource-group mtdev --slot-settings "DebugMode=false" --slot $slotName | out-null
            az webapp config appsettings set --name $appName --resource-group mtdev --slot-settings "IISDistributedEnabled=true" --slot $slotName | out-null
            az webapp config appsettings set --name $appName --resource-group mtdev --slot-settings '"CosmosDB.ConnectionString='$cosmosKey'"' --slot $slotName | out-null
            az webapp config appsettings set --name $appName --resource-group mtdev --slot-settings "CosmosDB.DBName=$slotName" --slot $slotName | out-null
            az webapp config appsettings set --name $appName --resource-group mtdev --slot-settings "ApplicationInsights.Key=$appInsightsSlotKey" --slot $slotName | out-null
            az webapp config appsettings set --name $appName --resource-group mtdev --slot-settings "WEBSITE_TIME_ZONE=Romance Standard Time" --slot $slotName | out-null
            Write-Host ("... ... ... OK") 

            az webapp stop --name $appName --resource-group mtdev --slot $slotName
        }elseif( $action -eq "delete"){
            Write-Host ("... ... Eliminando WebApp:" + $appName) 
            az webapp deployment slot delete --name $appName --resource-group mtdev --slot $slotName | out-null
            Write-Host ("... ... ... OK") 
        }
	}
    
    

    Write-Host ("... ... OK") 
    az logout

    $executionTimer.Stop()
    Write-Host "Operación realizada con éxito en $($executionTimer.Elapsed.TotalMinutes) minutos"
}else{
    Write-Host "No se ha indicado slot para publicar"
}

Set-MpPreference -DisableRealtimeMonitoring $false
Write-Host "Activando Windows defender RealTime monitoring"