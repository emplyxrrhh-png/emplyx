  
$slotAction="start"
$publishSlots=""
$functionsName="empty"


if ( $args.count -eq 3 ){
    $slotAction=$args[0].ToLower()
    $publishSlots=$args[1].ToLower()
    $functionsName=$args[2].ToLower()
} else{
    Write-Output "Atributtes missing: correct usage 'slotOperation.ps1 action=start/stop/restart/upgrade/delete/globalInsightsOn/globalInsightsOff slotname=idi0x' functionsName=empty"
    exit
}


if( $publishSlots )
{
    $executionTimer = [system.diagnostics.stopwatch]::startNew()

    az login --tenant 95b5bebd-9caa-4973-a257-8c0a8bc34c04 | out-null
    az account set --subscription "Entornos" | out-null


	$publishSlotsArray = $publishSlots.Split(",")
	Foreach ($publishSlot in $publishSlotsArray)
    {
		
		Write-Host ("Realizando operación(" + $slotAction + ") en slot:" + $publishSlot) 

		if ( $slotAction -eq "start" -or $slotAction -eq "stop" -or $slotAction -eq "restart" ){
			Write-Host ("... fnanalyticsfunctiondev") 
			az functionapp $slotAction --name fnanalyticsfunctiondevv2 --resource-group mtdev --slot $publishSlot
			Write-Host ("... fnbackgroundfunctiondev")
			az functionapp $slotAction --name fnbackgroundfunctiondevv2 --resource-group mtdev --slot $publishSlot
			Write-Host ("... fnbroadcasterfunctiondev")
			az functionapp $slotAction --name fnbroadcasterfunctiondevv2 --resource-group mtdev --slot $publishSlot
			Write-Host ("... fndatalinkfunctiondev")
			az functionapp $slotAction --name fndatalinkfunctiondevv2 --resource-group mtdev --slot $publishSlot
			Write-Host ("... fnmailfunctiondev")
			az functionapp $slotAction --name fnmailfunctiondevv2 --resource-group mtdev --slot $publishSlot
			Write-Host ("... fnreportfunctiondev")
			az functionapp $slotAction --name fnreportfunctiondevv2 --resource-group mtdev --slot $publishSlot
			Write-Host ("... fnnotificationsfunctiondev")
			az functionapp $slotAction --name fnnotificationsfunctiondevv2 --resource-group mtdev --slot $publishSlot
			Write-Host ("... fnpushnotificationsfunctiondev")
			az functionapp $slotAction --name fnpushfunctiondevv2 --resource-group mtdev --slot $publishSlot
			Write-Host ("... fnenginefunction")
			az functionapp $slotAction --name fnenginefunctiondevv2 --resource-group mtdev --slot $publishSlot
			Write-Host ("... fnPunchConnectorFunctiondev")
			az functionapp $slotAction --name fnconnectorFunctiondevv2 --resource-group mtdev --slot $publishSlot
			Write-Host ("... fnpnlinkfunction")
			az functionapp $slotAction --name fnpnlinkfunctiondevv2 --resource-group mtdev --slot $publishSlot
			#Write-Host ("... roftpstoragesyncfunction")
			#az functionapp start --name fnFTPSyncFunctiondevv2 --resource-group mtdev --slot $publishSlot
		
			Write-Host ("... vtliveapidev")
			az webapp $slotAction --name vtliveapidev --resource-group mtdev --slot $publishSlot
			Write-Host ("... vtlivedev")
			az webapp $slotAction --name vtlivedev --resource-group mtdev --slot $publishSlot
			Write-Host ("... vtportaldev")
			az webapp $slotAction --name vtportaldev --resource-group mtdev --slot $publishSlot
			Write-Host ("... vtterminalsdev")
			az webapp $slotAction --name vtterminalsdev --resource-group mtdev --slot $publishSlot
			Write-Host ("... vtvisitsdev")
			az webapp $slotAction --name vtvisitsdev --resource-group mtdev --slot $publishSlot
		
			Write-Host ("... fnschedulefunctiondev")
			az functionapp $slotAction --name fnschedulefunctiondevv2 --resource-group mtdev --slot $publishSlot
		}
		elseif( $slotAction -eq "upgrade"){
        
			Write-Host ("... setting FUNCTIONS_EXTENSION_VERSION fnschedulefunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_EXTENSION_VERSION=~4 -g mtdev  -n fnschedulefunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_EXTENSION_VERSION fnanalyticsfunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_EXTENSION_VERSION=~4 -g mtdev  -n fnanalyticsfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_EXTENSION_VERSION fnbackgroundfunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_EXTENSION_VERSION=~4 -g mtdev  -n fnbackgroundfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_EXTENSION_VERSION fndatalinkfunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_EXTENSION_VERSION=~4 -g mtdev  -n fndatalinkfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_EXTENSION_VERSION fnbroadcasterfunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_EXTENSION_VERSION=~4 -g mtdev  -n fnbroadcasterfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_EXTENSION_VERSION fnmailfunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_EXTENSION_VERSION=~4 -g mtdev  -n fnmailfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_EXTENSION_VERSION fnreportfunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_EXTENSION_VERSION=~4 -g mtdev  -n fnreportfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_EXTENSION_VERSION fnnotificationsfunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_EXTENSION_VERSION=~4 -g mtdev  -n fnnotificationsfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_EXTENSION_VERSION fnpushnotificationsfunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_EXTENSION_VERSION=~4 -g mtdev  -n fnpushfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_EXTENSION_VERSION fnenginefunction") 
			az functionapp config appsettings set --settings FUNCTIONS_EXTENSION_VERSION=~4 -g mtdev  -n fnenginefunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_EXTENSION_VERSION fnPunchConnectorFunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_EXTENSION_VERSION=~4 -g mtdev  -n fnconnectorFunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_EXTENSION_VERSION fnpnlinkfunction") 
			az functionapp config appsettings set --settings FUNCTIONS_EXTENSION_VERSION=~4 -g mtdev  -n fnpnlinkfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_EXTENSION_VERSION roftpstoragesyncfunction") 
			az functionapp config appsettings set --settings FUNCTIONS_EXTENSION_VERSION=~4 -g mtdev  -n fnFTPSyncFunctiondevv2 --slot $publishSlot | out-null
		
			Write-Host ("... setting FUNCTIONS_WORKER_RUNTIME fnschedulefunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_WORKER_RUNTIME=dotnet-isolated -g mtdev  -n fnschedulefunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_WORKER_RUNTIME fnanalyticsfunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_WORKER_RUNTIME=dotnet-isolated -g mtdev  -n fnanalyticsfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_WORKER_RUNTIME fnbackgroundfunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_WORKER_RUNTIME=dotnet-isolated -g mtdev  -n fnbackgroundfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_WORKER_RUNTIME fndatalinkfunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_WORKER_RUNTIME=dotnet-isolated -g mtdev  -n fndatalinkfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_WORKER_RUNTIME fnbroadcasterfunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_WORKER_RUNTIME=dotnet-isolated -g mtdev  -n fnbroadcasterfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_WORKER_RUNTIME fnmailfunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_WORKER_RUNTIME=dotnet-isolated -g mtdev  -n fnmailfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_WORKER_RUNTIME fnreportfunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_WORKER_RUNTIME=dotnet-isolated -g mtdev  -n fnreportfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_WORKER_RUNTIME fnnotificationsfunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_WORKER_RUNTIME=dotnet-isolated -g mtdev  -n fnnotificationsfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_WORKER_RUNTIME fnpushnotificationsfunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_WORKER_RUNTIME=dotnet-isolated -g mtdev  -n fnpushfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_WORKER_RUNTIME fnenginefunction") 
			az functionapp config appsettings set --settings FUNCTIONS_WORKER_RUNTIME=dotnet-isolated -g mtdev  -n fnenginefunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_WORKER_RUNTIME fnPunchConnectorFunctiondev") 
			az functionapp config appsettings set --settings FUNCTIONS_WORKER_RUNTIME=dotnet-isolated -g mtdev  -n fnconnectorFunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_WORKER_RUNTIME fnpnlinkfunction") 
			az functionapp config appsettings set --settings FUNCTIONS_WORKER_RUNTIME=dotnet-isolated -g mtdev  -n fnpnlinkfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting FUNCTIONS_WORKER_RUNTIME roftpstoragesyncfunction") 
			az functionapp config appsettings set --settings FUNCTIONS_WORKER_RUNTIME=dotnet-isolated -g mtdev  -n fnFTPSyncFunctiondevv2 --slot $publishSlot | out-null

			
			Write-Host ("... setting net-framework-version v6.0 fnschedulefunctiondev") 
			az functionapp config set --net-framework-version v6.0 --resource-group mtdev --name fnschedulefunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting net-framework-version v6.0 fnanalyticsfunctiondev") 
			az functionapp config set --net-framework-version v6.0 --resource-group mtdev --name fnanalyticsfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting net-framework-version v6.0 fnbackgroundfunctiondev") 
			az functionapp config set --net-framework-version v6.0 --resource-group mtdev --name fnbackgroundfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting net-framework-version v6.0 fndatalinkfunctiondev") 
			az functionapp config set --net-framework-version v6.0 --resource-group mtdev --name fndatalinkfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting net-framework-version v6.0 fnbroadcasterfunctiondev") 
			az functionapp config set --net-framework-version v6.0 --resource-group mtdev --name fnbroadcasterfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting net-framework-version v6.0 fnmailfunctiondev") 
			az functionapp config set --net-framework-version v6.0 --resource-group mtdev --name fnmailfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting net-framework-version v6.0 fnreportfunctiondev") 
			az functionapp config set --net-framework-version v6.0 --resource-group mtdev --name fnreportfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting net-framework-version v6.0 fnnotificationsfunctiondev") 
			az functionapp config set --net-framework-version v6.0 --resource-group mtdev --name fnnotificationsfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting net-framework-version v6.0 fnpushnotificationsfunctiondev") 
			az functionapp config set --net-framework-version v6.0 --resource-group mtdev --name fnpushfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting net-framework-version v6.0 fnenginefunction") 
			az functionapp config set --net-framework-version v6.0 --resource-group mtdev --name fnenginefunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting net-framework-version v6.0 fnPunchConnectorFunctiondev") 
			az functionapp config set --net-framework-version v6.0 --resource-group mtdev --name fnconnectorFunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting net-framework-version v6.0 fnpnlinkfunction") 
			az functionapp config set --net-framework-version v6.0 --resource-group mtdev --name fnpnlinkfunctiondevv2 --slot $publishSlot | out-null
			Write-Host ("... setting net-framework-version v6.0 roftpstoragesyncfunction") 
			az functionapp config set --net-framework-version v6.0 --resource-group mtdev --name fnFTPSyncFunctiondevv2 --slot $publishSlot | out-null
		}
		elseif( $slotAction -eq "globalInsightsOn"){
			$insightsConnectionString = "InstrumentationKey=97412ebd-cc69-4a7a-8a65-2b4eeb973c96;IngestionEndpoint=https://northeurope-2.in.applicationinsights.azure.com/;LiveEndpoint=https://northeurope.livediagnostics.monitor.azure.com/"
			
			Write-Host ("... fnanalyticsfunctiondev") 
			az functionapp config appsettings set --name fnanalyticsfunctiondevv2 --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az functionapp config appsettings set --name fnanalyticsfunctiondevv2 --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
			Write-Host ("... fnbackgroundfunctiondev")
			az functionapp config appsettings set --name fnbackgroundfunctiondevv2 --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az functionapp config appsettings set --name fnbackgroundfunctiondevv2 --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
			Write-Host ("... fnbroadcasterfunctiondev")
			az functionapp config appsettings set --name fnbroadcasterfunctiondevv2 --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az functionapp config appsettings set --name fnbroadcasterfunctiondevv2 --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
			Write-Host ("... fndatalinkfunctiondev")
			az functionapp config appsettings set --name fndatalinkfunctiondevv2 --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az functionapp config appsettings set --name fndatalinkfunctiondevv2 --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
			Write-Host ("... fnmailfunctiondev")
			az functionapp config appsettings set --name fnmailfunctiondevv2 --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az functionapp config appsettings set --name fnmailfunctiondevv2 --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
			Write-Host ("... fnreportfunctiondev")
			az functionapp config appsettings set --name fnreportfunctiondevv2 --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az functionapp config appsettings set --name fnreportfunctiondevv2 --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
			Write-Host ("... fnnotificationsfunctiondev")
			az functionapp config appsettings set --name fnnotificationsfunctiondevv2 --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az functionapp config appsettings set --name fnnotificationsfunctiondevv2 --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
			Write-Host ("... fnpushnotificationsfunctiondev")
			az functionapp config appsettings set --name fnpushfunctiondevv2 --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az functionapp config appsettings set --name fnpushfunctiondevv2 --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
			Write-Host ("... fnenginefunction")
			az functionapp config appsettings set --name fnenginefunctiondevv2 --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az functionapp config appsettings set --name fnenginefunctiondevv2 --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
			Write-Host ("... fnPunchConnectorFunctiondev")
			az functionapp config appsettings set --name fnconnectorFunctiondevv2 --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az functionapp config appsettings set --name fnconnectorFunctiondevv2 --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
			Write-Host ("... fnpnlinkfunction")
			az functionapp config appsettings set --name fnpnlinkfunctiondevv2 --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az functionapp config appsettings set --name fnpnlinkfunctiondevv2 --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
			Write-Host ("... roftpstoragesyncfunction")
			az functionapp config appsettings set --name fnFTPSyncFunctiondevv2 --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az functionapp config appsettings set --name fnFTPSyncFunctiondevv2 --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
		
			Write-Host ("... vtliveapidev")
			az webapp config appsettings set --name vtliveapidev --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az webapp config appsettings set --name vtliveapidev --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
			Write-Host ("... vtlivedev")
			az webapp config appsettings set --name vtlivedev --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az webapp config appsettings set --name vtlivedev --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
			Write-Host ("... vtportaldev")
			az webapp config appsettings set --name vtportaldev --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az webapp config appsettings set --name vtportaldev --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
			Write-Host ("... vtterminalsdev")
			az webapp config appsettings set --name vtterminalsdev --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az webapp config appsettings set --name vtterminalsdev --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
			Write-Host ("... vtvisitsdev")
			az webapp config appsettings set --name vtvisitsdev --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az webapp config appsettings set --name vtvisitsdev --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
		
			Write-Host ("... fnschedulefunctiondev")
			az functionapp config appsettings set --name fnschedulefunctiondevv2 --resource-group mtdev --settings "APPINSIGHTS_INSTRUMENTATIONKEY=97412ebd-cc69-4a7a-8a65-2b4eeb973c96" --slot $publishSlot | out-null
			az functionapp config appsettings set --name fnschedulefunctiondevv2 --resource-group mtdev --settings "APPLICATIONINSIGHTS_CONNECTION_STRING=$insightsConnectionString" --slot $publishSlot | out-null
			
		}
		elseif( $slotAction -eq "globalInsightsOff"){
			
			Write-Host ("... fnanalyticsfunctiondev") 
			az functionapp config appsettings delete --name fnanalyticsfunctiondevv2 --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az functionapp config appsettings delete --name fnanalyticsfunctiondevv2 --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
			Write-Host ("... fnbackgroundfunctiondev")
			az functionapp config appsettings delete --name fnbackgroundfunctiondevv2 --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az functionapp config appsettings delete --name fnbackgroundfunctiondevv2 --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
			Write-Host ("... fnbroadcasterfunctiondev")
			az functionapp config appsettings delete --name fnbroadcasterfunctiondevv2 --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az functionapp config appsettings delete --name fnbroadcasterfunctiondevv2 --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
			Write-Host ("... fndatalinkfunctiondev")
			az functionapp config appsettings delete --name fndatalinkfunctiondevv2 --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az functionapp config appsettings delete --name fndatalinkfunctiondevv2 --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
			Write-Host ("... fnmailfunctiondev")
			az functionapp config appsettings delete --name fnmailfunctiondevv2 --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az functionapp config appsettings delete --name fnmailfunctiondevv2 --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
			Write-Host ("... fnreportfunctiondev")
			az functionapp config appsettings delete --name fnreportfunctiondevv2 --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az functionapp config appsettings delete --name fnreportfunctiondevv2 --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
			Write-Host ("... fnnotificationsfunctiondev")
			az functionapp config appsettings delete --name fnnotificationsfunctiondevv2 --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az functionapp config appsettings delete --name fnnotificationsfunctiondevv2 --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
			Write-Host ("... fnpushnotificationsfunctiondev")
			az functionapp config appsettings delete --name fnpushfunctiondevv2 --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az functionapp config appsettings delete --name fnpushfunctiondevv2 --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
			Write-Host ("... fnenginefunction")
			az functionapp config appsettings delete --name fnenginefunctiondevv2 --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az functionapp config appsettings delete --name fnenginefunctiondevv2 --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
			Write-Host ("... fnPunchConnectorFunctiondev")
			az functionapp config appsettings delete --name fnconnectorFunctiondevv2 --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az functionapp config appsettings delete --name fnconnectorFunctiondevv2 --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
			Write-Host ("... fnpnlinkfunction")
			az functionapp config appsettings delete --name fnpnlinkfunctiondevv2 --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az functionapp config appsettings delete --name fnpnlinkfunctiondevv2 --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
			Write-Host ("... roftpstoragesyncfunction")
			az functionapp config appsettings delete --name fnFTPSyncFunctiondevv2 --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az functionapp config appsettings delete --name fnFTPSyncFunctiondevv2 --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
		
			Write-Host ("... vtliveapidev")
			az webapp config appsettings delete --name vtliveapidev --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az webapp config appsettings delete --name vtliveapidev --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
			Write-Host ("... vtlivedev")
			az webapp config appsettings delete --name vtlivedev --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az webapp config appsettings delete --name vtlivedev --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
			Write-Host ("... vtportaldev")
			az webapp config appsettings delete --name vtportaldev --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az webapp config appsettings delete --name vtportaldev --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
			Write-Host ("... vtterminalsdev")
			az webapp config appsettings delete --name vtterminalsdev --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az webapp config appsettings delete --name vtterminalsdev --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
			Write-Host ("... vtvisitsdev")
			az webapp config appsettings delete --name vtvisitsdev --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az webapp config appsettings delete --name vtvisitsdev --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
		
			Write-Host ("... fnschedulefunctiondev")
			az functionapp config appsettings delete --name fnschedulefunctiondev --resource-group mtdev --setting-names "APPINSIGHTS_INSTRUMENTATIONKEY" --slot $publishSlot | out-null
			az functionapp config appsettings delete --name fnschedulefunctiondev --resource-group mtdev --setting-names "APPLICATIONINSIGHTS_CONNECTION_STRING" --slot $publishSlot | out-null
			
		}elseif( $slotAction -eq "delete"){

			$functionsNameArray = $functionsName.Split(",")
			Foreach ($functionName in $functionsNameArray)
			{
				Write-Host ("... Eliminando el function/slot:" + $functionName + "(" + $publishSlot + ")")
				az functionapp deployment slot delete --slot $publishSlot --name $functionName --resource-group mtdev 
				Write-Host ("... ... OK")
			}


			
		}
	}
    az logout

    $executionTimer.Stop()
    Write-Host "Operación realizada con éxito en $($executionTimer.Elapsed.TotalMinutes) minutos"
}else{
    Write-Host "No se ha indicado slot para publicar"
}

Set-MpPreference -DisableRealtimeMonitoring $false
Write-Host "Activando Windows defender RealTime monitoring"