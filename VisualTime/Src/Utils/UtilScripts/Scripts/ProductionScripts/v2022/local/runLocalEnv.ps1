
$functionsToRun = "all"
$funcCommand = ('c:\Users\' + $Env:UserName + '\AppData\Local\AzureFunctionsTools\Releases\4.107.0\cli_x64\func.exe')

if ( $args.count -eq 1 ){
    $functionsToRun = $args[0]
}

if( $functionsToRun -eq "all"){
    Write-Host ("Iniciando todas las funciones locales instaladas")
    Write-Host ("... func version: " + $funcCommand)
    Write-Host ("... Localpath: c:\work\visualtime\fnProc")
}else{
    Write-Host ("Iniciando función local(Disponibles: fnAnalyticsFunction, fnBackgroundFunction, fnBroadcasterFunction, fnDatalinkFunction, fnMailFunction, fnReportFunction, fnNotificationsFunction, fnPushNotificationsFunction, fnEngineFunction, fnPunchConnectorFunction, fnScheduleFunction, fnpnlinkfunction, fnFTPStorageSyncFunction)")
    Write-Host ("... func version: " + $funcCommand)
    Write-Host ("... function app: " + $functionsToRun)
    Write-Host ("... Localpath: c:\work\visualtime\fnProc")
}


Write-Host "Pulsa enter para continuar o CTRL+C para cancelar"
$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") > $null

if( !(Test-Path -Path 'c:\work\VisualTime\fnProc') ){
    Write-Host "No se ha encontrado la carpeta de funciones local. Ejecuta primero el script deployLocalFunc.ps1"
    exit
}

Write-Host "...Iniciando functionApps"
if(($functionsToRun -eq "all") -or ($functionsToRun -eq "fnAnalyticsFunction")){
    Write-Host "... ... fnanalyticsfunction"
    wt -w 0 nt pwsh -NoExit -c {
        $funcCommand = ('c:\Users\' + $Env:UserName + '\AppData\Local\AzureFunctionsTools\Releases\4.107.0\cli_x64\func.exe')
        $workingPath = 'c:\work\VisualTime\fnProc\fnAnalyticsFunction'
        $host.UI.RawUI.WindowTitle = 'fnAnalyticsFunction'
        Start-Process -FilePath $funcCommand -ArgumentList "host","start","--port","6071" -WorkingDirectory $workingPath -NoNewWindow
    }
    Write-Host "... ... OK"	
}

if(($functionsToRun -eq "all") -or ($functionsToRun -eq "fnBackgroundFunction")){
	Write-Host "... ... fnBackgroundFunction"
    wt -w 0 nt pwsh -NoExit -c {
        $funcCommand = ('c:\Users\' + $Env:UserName + '\AppData\Local\AzureFunctionsTools\Releases\4.107.0\cli_x64\func.exe')
        $workingPath = 'c:\work\VisualTime\fnProc\fnBackgroundFunction'
        $host.UI.RawUI.WindowTitle = 'fnBackgroundFunction'
        Start-Process -FilePath $funcCommand -ArgumentList "host","start","--port","6072" -WorkingDirectory $workingPath -NoNewWindow
    }
    Write-Host "... ... OK"
}

if(($functionsToRun -eq "all") -or ($functionsToRun -eq "fnBroadcasterFunction")){
    Write-Host "... ... fnBroadcasterFunction"
    wt -w 0 nt pwsh -NoExit -c {
        $funcCommand = ('c:\Users\' + $Env:UserName + '\AppData\Local\AzureFunctionsTools\Releases\4.107.0\cli_x64\func.exe')
        $workingPath = 'c:\work\VisualTime\fnProc\fnBroadcasterFunction'
        $host.UI.RawUI.WindowTitle = 'fnBroadcasterFunction'
        Start-Process -FilePath $funcCommand -ArgumentList "host","start","--port","6073" -WorkingDirectory $workingPath -NoNewWindow
    }
    Write-Host "... ... OK"
}

if(($functionsToRun -eq "all") -or ($functionsToRun -eq "fnDatalinkFunction")){
	Write-Host "... ... fnDatalinkFunction"
    wt -w 0 nt pwsh -NoExit -c {
        $funcCommand = ('c:\Users\' + $Env:UserName + '\AppData\Local\AzureFunctionsTools\Releases\4.107.0\cli_x64\func.exe')
        $workingPath = 'c:\work\VisualTime\fnProc\fnDatalinkFunction'
        $host.UI.RawUI.WindowTitle = 'fnDatalinkFunction'
        Start-Process -FilePath $funcCommand -ArgumentList "host","start","--port","6074" -WorkingDirectory $workingPath -NoNewWindow
    }
    Write-Host "... ... OK"
}

if(($functionsToRun -eq "all") -or ($functionsToRun -eq "fnMailFunction")){
	Write-Host "... ... fnMailFunction"
    wt -w 0 nt pwsh -NoExit -c {
        $funcCommand = ('c:\Users\' + $Env:UserName + '\AppData\Local\AzureFunctionsTools\Releases\4.107.0\cli_x64\func.exe')
        $workingPath = 'c:\work\VisualTime\fnProc\fnMailFunction'
        $host.UI.RawUI.WindowTitle = 'fnMailFunction'
        Start-Process -FilePath $funcCommand -ArgumentList "host","start","--port","6075" -WorkingDirectory $workingPath -NoNewWindow
    }
    Write-Host "... ... OK"
}

if(($functionsToRun -eq "all") -or ($functionsToRun -eq "fnReportFunction")){
	Write-Host "... ... fnReportFunction"
    wt -w 0 nt pwsh -NoExit -c {
        $funcCommand = ('c:\Users\' + $Env:UserName + '\AppData\Local\AzureFunctionsTools\Releases\4.107.0\cli_x64\func.exe')
        $workingPath = 'c:\work\VisualTime\fnProc\fnReportFunction'
        $host.UI.RawUI.WindowTitle = 'fnReportFunction'
        Start-Process -FilePath $funcCommand -ArgumentList "host","start","--port","6076" -WorkingDirectory $workingPath -NoNewWindow
    }
    Write-Host "... ... OK"
}

if(($functionsToRun -eq "all") -or ($functionsToRun -eq "fnNotificationsFunction")){
	Write-Host "... ... fnNotificationsFunction"
    wt -w 0 nt pwsh -NoExit -c {
        $funcCommand = ('c:\Users\' + $Env:UserName + '\AppData\Local\AzureFunctionsTools\Releases\4.107.0\cli_x64\func.exe')
        $workingPath = 'c:\work\VisualTime\fnProc\fnNotificationsFunction'
        $host.UI.RawUI.WindowTitle = 'fnNotificationsFunction'
        Start-Process -FilePath $funcCommand -ArgumentList "host","start","--port","6077" -WorkingDirectory $workingPath -NoNewWindow
    }
    Write-Host "... ... OK"
}

if(($functionsToRun -eq "all") -or ($functionsToRun -eq "fnPushNotificationsFunction")){
	Write-Host "... ... fnPushNotificationsFunction"
    wt -w 0 nt pwsh -NoExit -c {
        $funcCommand = ('c:\Users\' + $Env:UserName + '\AppData\Local\AzureFunctionsTools\Releases\4.107.0\cli_x64\func.exe')
        $workingPath = 'c:\work\VisualTime\fnProc\fnPushNotificationsFunction'
        $host.UI.RawUI.WindowTitle = 'fnPushNotificationsFunction'
        Start-Process -FilePath $funcCommand -ArgumentList "host","start","--port","6078" -WorkingDirectory $workingPath -NoNewWindow
    }
    Write-Host "... ... OK"
}

if(($functionsToRun -eq "all") -or ($functionsToRun -eq "fnEngineFunction")){
	Write-Host "... ... fnEngineFunction"
    wt -w 0 nt pwsh -NoExit -c {
        $funcCommand = ('c:\Users\' + $Env:UserName + '\AppData\Local\AzureFunctionsTools\Releases\4.107.0\cli_x64\func.exe')
        $workingPath = 'c:\work\VisualTime\fnProc\fnEngineFunction'
        $host.UI.RawUI.WindowTitle = 'fnEngineFunction'
        Start-Process -FilePath $funcCommand -ArgumentList "host","start","--port","6079" -WorkingDirectory $workingPath -NoNewWindow
    }
    Write-Host "... ... OK"
}

if(($functionsToRun -eq "all") -or ($functionsToRun -eq "fnPunchConnectorFunction")){
	Write-Host "... ... fnPunchConnectorFunction"
    wt -w 0 nt pwsh -NoExit -c {
        $funcCommand = ('c:\Users\' + $Env:UserName + '\AppData\Local\AzureFunctionsTools\Releases\4.107.0\cli_x64\func.exe')
        $workingPath = 'c:\work\VisualTime\fnProc\fnPunchConnectorFunction'
        $host.UI.RawUI.WindowTitle = 'fnPunchConnectorFunction'
        Start-Process -FilePath $funcCommand -ArgumentList "host","start","--port","6080" -WorkingDirectory $workingPath -NoNewWindow
    }
    Write-Host "... ... OK"
}

if(($functionsToRun -eq "all") -or ($functionsToRun -eq "fnpnlinkfunction")){
	Write-Host "... ... fnpnlinkfunction"
    wt -w 0 nt pwsh -NoExit -c {
        $funcCommand = ('c:\Users\' + $Env:UserName + '\AppData\Local\AzureFunctionsTools\Releases\4.107.0\cli_x64\func.exe')
        $workingPath = 'c:\work\VisualTime\fnProc\fnpnlinkfunction'
        $host.UI.RawUI.WindowTitle = 'fnpnlinkfunction'
        Start-Process -FilePath $funcCommand -ArgumentList "host","start","--port","6081" -WorkingDirectory $workingPath -NoNewWindow
    }
    Write-Host "... ... OK"
}

if($functionsToRun -eq "fnFTPStorageSyncFunction"){
	Write-Host "... ... fnFTPStorageSyncFunction"
    wt -w 0 nt pwsh -NoExit -c {
        $funcCommand = ('c:\Users\' + $Env:UserName + '\AppData\Local\AzureFunctionsTools\Releases\4.107.0\cli_x64\func.exe')
        $workingPath = 'c:\work\VisualTime\fnProc\fnFTPStorageSyncFunction'
        $host.UI.RawUI.WindowTitle = 'fnFTPStorageSyncFunction'
        Start-Process -FilePath $funcCommand -ArgumentList "host","start","--port","6082" -WorkingDirectory $workingPath -NoNewWindow
    }
    Write-Host "... ... OK"
}

if(($functionsToRun -eq "all") -or ($functionsToRun -eq "fnScheduleFunction")){
	Write-Host "... ... fnScheduleFunction"
    wt -w 0 nt pwsh -NoExit -c {
        $funcCommand = ('c:\Users\' + $Env:UserName + '\AppData\Local\AzureFunctionsTools\Releases\4.107.0\cli_x64\func.exe')
        $workingPath = 'c:\work\VisualTime\fnProc\fnScheduleFunction'
        $host.UI.RawUI.WindowTitle = 'fnScheduleFunction'
        Start-Process -FilePath $funcCommand -ArgumentList "host","start","--port","6083" -WorkingDirectory $workingPath -NoNewWindow
    }
    Write-Host "... ... OK"
}

































