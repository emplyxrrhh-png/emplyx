  
$functionsName="start"
$publishSlots=""
$alwaysOn="true"


if ( $args.count -eq 3 ){
    $functionsName=$args[0].ToLower()
    $publishSlots=$args[1].ToLower()
    $alwaysOn=$args[2].ToLower()

    if(!($alwaysOn -eq "false")){ $alwaysOn ='true'}
} else{
    Write-Output "Atributtes missing: correct usage 'setAlwaysOnStatus.ps1 functionName=fnxxxxxxxxxxxxxxxxxx slots=idi0x alwaysOn=true"
    exit
}


Write-Host ("Se van a cambiar el estado del always on")
Write-Host ("... Slots afectados:" + $publishSlots)
Write-Host ("... Funciones afectadas:" + $functionsName)
Write-Host ("... Nuevo estado always on:" + $alwaysOn)
Write-Host "Pulsa enter para continuar o CTRL+C para cancelar"
$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") > $null



if( $publishSlots )
{
    $executionTimer = [system.diagnostics.stopwatch]::startNew()

    az login --tenant 95b5bebd-9caa-4973-a257-8c0a8bc34c04 | out-null
    az account set --subscription "Entornos" | out-null

    $functionsNameArray = $functionsName.Split(",")
	Foreach ($functionName in $functionsNameArray)
    {
	    $publishSlotsArray = $publishSlots.Split(",")
	    Foreach ($publishSlot in $publishSlotsArray)
        {
            Write-Host ("... Estableciendo always on de la función:" + $functionName + "(" + $publishSlot + ") al valor:" + $alwaysOn)
            az functionapp config set --always-on $alwaysOn --resource-group mtdev --name $functionName --slot $publishSlot| out-null
            Write-Host ("... ... OK")

	    }
        az functionapp stop --name $functionName --resource-group mtdev
    }
    az logout

    $executionTimer.Stop()
    Write-Host "Operación realizada con éxito en $($executionTimer.Elapsed.TotalMinutes) minutos"
}else{
    Write-Host "No se ha indicado slot para publicar"
}

Set-MpPreference -DisableRealtimeMonitoring $false
Write-Host "Activando Windows defender RealTime monitoring"