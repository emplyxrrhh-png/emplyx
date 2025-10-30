function configFile([string] $path)
{
	$pathNew=$path+'.new'
	$pathOld=$path+'.old'

    $iisInfo = Get-ItemProperty HKLM:\SOFTWARE\Microsoft\InetStp\
    [int]$iisversion = "$($iisInfo.MajorVersion)"

	if ((Test-Path $path)) {
		if ((Test-Path $pathNew)){ rm $pathNew }
		$content = Get-Content $path
		foreach ($line in $content)
		{
			if ($line -like '*<!--<endpoint*'){
				$line=$line -replace '<!--', ''
				$line=$line -replace '-->', ''
			}
			if ($line -like '*<!-- SaaS*'){
				$line=$line -replace '<!-- SaaS ', ''
			}
			if ($line -like '*<!--SaaS*'){
				$line=$line -replace '<!--SaaS ', ''
			}
			if ($line -like '*<!-- SaaS*'){
				$line=$line -replace '<!-- SaaS', ''
			}
			if ($line -like '*<!--SaaS*'){
				$line=$line -replace '<!--SaaS', ''
			}
			if ($line -like '* /SaaS -->'){
				$line=$line -replace ' /SaaS -->', ''
			}
			if ($line -like '* /SaaS-->'){
				$line=$line -replace ' /SaaS-->', ''
			}
			if ($line -like '* SaaS-->'){
				$line=$line -replace ' SaaS-->', ''
			}
			if ($line -like '*<!-- Standard -->*'){
				$line=$line -replace 'Standard -->', ''
			}
			if ($line -like '*<!-- /Standard -->*'){
				$line=$line -replace '<!-- /Standard', ''
			            }
            if ($iisversion -lt 10 ){
                if ($line -like '*removeServerHeader="true"*'){
				    $line=$line -replace ' removeServerHeader="true"', ''
			    }

            }
			#Write-Host $line
			Add-Content $pathNew $line
		}
		cp $path $pathOld
		cp $pathNew $path
		rm $pathNew
	}


}



Write-Host "Configuramos web.config de los sites"
configFile("C:\inetpub\wwwroot\LivePortal\web.config")
configFile("C:\inetpub\wwwroot\SupervisorPortal\web.config")
configFile("C:\inetpub\wwwroot\Visits\web.config")
configFile("C:\inetpub\wwwroot\VTLiveApi\web.config")
configFile("C:\inetpub\wwwroot\VTLive\web.config")
configFile("C:\inetpub\wwwroot\VTPortal\web.config")