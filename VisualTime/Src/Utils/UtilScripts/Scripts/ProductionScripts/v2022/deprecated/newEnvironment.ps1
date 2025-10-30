  
$functionsName="fnanalyticsfunctiondevv2,fnbackgroundfunctiondevv2,fnbroadcasterfunctiondevv2,fnconnectorfunctiondevv2,fndatalinkfunctiondevv2,fnenginefunctiondevv2,fnftpsyncfunctiondevv2,fnmailfunctiondevv2,fnnotificationsfunctiondevv2,fnpnlinkfunctiondevv2,fnpushfunctiondevv2,fnreportfunctiondevv2,fnschedulefunctiondevv2"
$appsName="vtliveapidev,vtlivedev,vtportaldev,vtterminalsdev,vtvisitsdev"
$slotName=""
$netVersion="1"
$action="create"


if ( $args.count -eq 3 ){
    $slotName=$args[0].ToLower()
    $action=$args[1].ToLower()
    $netVersion=$args[2].ToString()

} else{
    Write-Output "Atributtes missing: correct usage 'newEnvironment.ps1 slotname=idi0x action=create/delete netversion=1/4"
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

    az login --tenant 95b5bebd-9caa-4973-a257-8c0a8bc34c04 | out-null
    az account set --subscription "Entornos" | out-null

    $functionsNameArray = $functionsName.Split(",")
    $appsNameArray = $appsName.Split(",")

    $storageKey = "romtrcstorage"
    $appInisghtsName = "romtGlobalStatus"
    $appInisghtsKey = "97412ebd-cc69-4a7a-8a65-2b4eeb973c96"
    $appInsightsSlotKey = ''
    $serviceBusSlotKey= ''
    $cosmosKey = "mongodb://rscosmosdev:uIalSOw2BNUNGvuDtnyM7vkq4wLD9ZMT0fhh7fE9XXkJUCcKQlXeioOyiYiFlngJ6hEjqJzQCE95DOsGjTmpPQ==@rscosmosdev.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&retrywrites=false&maxIdleTimeMS=120000&appName=@rscosmosdev@"
        
    switch ($slotName) {
        "dev" {
            $appInsightsSlotKey = '82de3e6f-8c09-4d31-93d4-df18b925635f'
            $storageSlotKey = 'DefaultEndpointsProtocol=https;AccountName=rsdevnextstorage;AccountKey=VU/BAWgVGNSG5RSaIRyIp1MYqXCR2UEsyZpdV7UkWsByrsmBiFKwBe8e7yjO/O0xLaW9QZ60MFPnRVWZLRC5BQ==;EndpointSuffix=core.windows.net'
            $serviceBusSlotKey = 'Endpoint=sb://romtdevdev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=b3KU8yFwIgIYpDynvDlVV6+rnYjCvMldLbENANpoYm8='
        }
        "idi01" {
            $appInsightsSlotKey = 'b509d5c1-a7c7-46d2-9fac-2d97abe7b17a'
            $storageSlotKey = 'DefaultEndpointsProtocol=https;AccountName=romtidi01storage;AccountKey=cLo2/NmrjRGOraKJ6m6V4jMM7LHYwqHweRHeZwGuObeTc5VfqXKR6RcT8XPCY7c55bpeIISZeEu5z8qUVTJnMQ==;EndpointSuffix=core.windows.net'
            $serviceBusSlotKey = 'Endpoint=sb://romtdevidi01.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=rZZrpTREWTRB4q4mmB8j4N4zxFHSGhQeu57N5Pw8wx8='
        }
        "idi02" {
            $appInsightsSlotKey = '958522d1-6d69-4341-bf88-3eb33751b8ec'
            $storageSlotKey = 'DefaultEndpointsProtocol=https;AccountName=romtidi02storage;AccountKey=Szk5j9bo3tAMltm7gK17YzQNmIMt2jrUT9jzE9lkRpOpiINRAfQygLDLjm70LF1bxSS2gr705rMEjo3FNEuvBA==;EndpointSuffix=core.windows.net'
            $serviceBusSlotKey = 'Endpoint=sb://romtdevidi02.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=TsFD4TlRNfJRZoQcumlwjx6JFsQmZ4EmFYT4iZ2bvcc='
        }
        "idi03" {
            $appInsightsSlotKey = '7a6a81e7-ac5f-423b-b232-bc80b4baec32'
            $storageSlotKey = 'DefaultEndpointsProtocol=https;AccountName=romtidi03storage;AccountKey=EoeoUfQbTmyu6OvN4t15pZqUcw/3FTyDx1yuo3duD6wjnfJMVXDtF2oxpPSvvYe9WiAAJVj/5lj2jhp84JHm7g==;EndpointSuffix=core.windows.net'
            $serviceBusSlotKey = 'Endpoint=sb://romtdevidi03.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=6V1mj2ReW01uEjVvLrUH0CbJ6pYP7nXDO/JQqGFAelo='
        }
        "idi04" {
            $appInsightsSlotKey = '06934aca-69f3-48cd-ae85-5f0d3cb05e71'
            $storageSlotKey = 'DefaultEndpointsProtocol=https;AccountName=romtidi04storage;AccountKey=sPv4Lx8AGYm6Xy6KDUQUlbZ5IK/h0CB3l/NKGit2KjT6kJaDBnklapWbTQRf5o1fDwUec2NZXQzsKDxatfvM7A==;EndpointSuffix=core.windows.net'
            $serviceBusSlotKey = 'Endpoint=sb://romtdevidi04.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=BGSIZ5eNUop3cNquKtbVggw5szXDCjalBwBx5pHPzPQ='
        }
        "idi05" {
            $appInsightsSlotKey = '16db7316-4205-407f-ab34-8650e74b7c7a'
            $storageSlotKey = 'DefaultEndpointsProtocol=https;AccountName=romtidi05storage;AccountKey=jZ/3PajpDepRHHUP/KHljpBlXElH4wiirLVwhUKm3Bctg58TkMsdaigPMYE+YlZUXtJHm5bGnEUuiCy0Kr8rZA==;EndpointSuffix=core.windows.net'
            $serviceBusSlotKey = 'Endpoint=sb://romtdevidi05.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=/lVCompJBRMB1p8J4Ufu9Wn/nJQeMzLfARlVWdS9g/8='
        }
        "idi06" {
            $appInsightsSlotKey = '0e774009-1114-4d07-9b71-41e237b4d294'
            $storageSlotKey = 'DefaultEndpointsProtocol=https;AccountName=romtidi06storage;AccountKey=z/MFbl5dU967ncCY7r7ThgTrs1UWhXWPgw+fyOj3Ppb9VNunu/qyyQqZ5gHgZfKBpMljvMFfyiDInpl5oVr8Sw==;EndpointSuffix=core.windows.net'
            $serviceBusSlotKey = 'Endpoint=sb://romtdevidi06.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=CHBP412EEJT7Lyq/ZkM5jLUi5mUCRW4aeMxVXdpPlTk='
        }
        "idi07" {
            $appInsightsSlotKey = '8fc4921c-5489-48bf-a3ea-0daa4cc6eb24'
            $storageSlotKey = 'DefaultEndpointsProtocol=https;AccountName=romtidi07storage;AccountKey=2jdXKH/jnd8uEsRExyTh9ygpxhtvEc3OqvvoM582t7LH27Uz6+6na0Ulbw0Xf4f7JmO5OYAd2VTROrXK9VbNLA==;EndpointSuffix=core.windows.net'
            $serviceBusSlotKey = 'Endpoint=sb://romtdevidi07.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=MCyI+EfRjibNpbgJvNME/IdaJo3mTSVVIcDgrzHBcx0='
        }
        default {
            $appInsightsSlotKey = ''
            $storageSlotKey = ''
            $serviceBusSlotKey = ''
        }
    }

	#Write-Host ("Creando función:" + $functionName) 
	#az functionapp create -g mtdev  -p "ASP-IDI" -n $functionName -s $storageKey --functions-version 4 --os-type Windows --runtime dotnet --runtime-version 6 --app-insights $appInisghtsName --app-insights-key $appInisghtsKey| out-null
    #az functionapp vnet-integration add --name $functionName --resource-group mtdev --vnet '/subscriptions/2575c2c8-2863-425e-ba9f-91da47dc2ee6/resourceGroups/Entornos/providers/Microsoft.Network/virtualNetworks/entornos-vnet' --subnet 'AppFuncionIDI'| out-null
	#Write-Host ("... OK") 
        
	#Write-Host ("Configurando función:" + $functionName) 
    #az functionapp config appsettings set --settings FUNCTIONS_EXTENSION_VERSION=~4 -g mtdev  -n $functionName| out-null
    #az functionapp config appsettings set --settings FUNCTIONS_WORKER_RUNTIME=dotnet-isolated -g mtdev  -n $functionName| out-null
    #az functionapp config set --net-framework-version v6.0 --always-on true --use-32bit-worker-process false --resource-group mtdev --name $functionName| out-null
    #az functionapp config appsettings set --name fnanalyticsfunctiondev --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" | out-null
	#Write-Host ("... OK") 

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
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "DebugMode=true" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "VTLive.MultitenantService=true" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "WEBSITE_TIME_ZONE=Romance Standard Time" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings '"CosmosDB.ConnectionString='$cosmosKey'"' --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "CosmosDB.DBName=$slotName" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "ApplicationInsights.Key=$appInsightsSlotKey" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "AzureWebJobsServiceBus=$serviceBusSlotKey" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "AzureWebJobsDashboard=$storageSlotKey" --slot $slotName | out-null
            az functionapp config appsettings set --name $functionName --resource-group mtdev --slot-settings "AzureWebJobsStorage=$storageSlotKey" --slot $slotName | out-null
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
            az webapp config appsettings set --name $appName --resource-group mtdev --slot-settings "DebugMode=true" --slot $slotName | out-null
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