$directoryPath = 'c:\temp'

# Verificar si el directorio ya existe
if (-not (Test-Path $directoryPath)) {
    # Crear el directorio si no existe
    New-Item -ItemType Directory -Path $directoryPath
}


function uploadToVoltBuild([string] $filePath, [string] $appPlatform){
    Write-Host ("... ... ... Autenticando en volt.build ")   
    $uri = "https://api.volt.build/v1/authenticate"
    $body = @{
        grant_type = "client_credentials"
        client_id = "dd550cf0-0533-4d78-ac0e-d254c9499aed"
        client_secret = "MwkY2qLi0gIgRc38WrfFJnWBSY9MhoKw"
    }

    $response = Invoke-RestMethod -Uri $uri -Method Post -Body $body

      

    $uri = "https://api.volt.build/v1/app"
    $headerAuth = ('authorization: Bearer ' + $response.access_token)
    $headers = @{
        'authorization' = ('Bearer ' + $response.access_token)
    }

    $zipPlatform = ("platform=" + $appPlatform)
    $zipApp = ("app=@" + $filePath + ";type=application/zip")
    
    Write-Host ("... ... ... Uploading zip file: " + $zipPlatform + "/" + $zipApp)  

    $response = C:\WINDOWS\system32\curl.exe --request POST --url $uri --header $headerAuth --form $zipPlatform --form $zipApp

    Write-Host ("... ... ... Uploading zip result: " + $response)

    if($response -eq "Accepted"){
		 $uri = "https://api.volt.build/v1/app"
        $runningStates = "Queueing","Queuing","Processing"
        Do
        {

          Start-Sleep -Seconds 10
          $response = Invoke-RestMethod -Uri $uri -Headers $headers

          Write-Host ("... ... ... Volt.build status: " + $response.status) 

        } While ($runningStates -contains $response.status )
    
        Write-Host ("... ... ... Upload process finished with status: " + $response.status)

        if($response.status -eq "Completed"){
            Remove-Item $filePath
            if($appPlatform -eq "android"){
                Invoke-WebRequest $response.links.app -OutFile $filePath.Replace(".zip",".apk")
            }
            else
            {
                Invoke-WebRequest $response.links.app -OutFile $filePath.Replace(".zip",".ipa")
            }
        }

	}else{
        Write-Host ("... ... ... Error: zip file not accepted")
    }
}

$rutaArchivo = ".\..\..\..\..\..\..\VTPortalJS\deploy Android\config.xml"
$xml = [xml](Get-Content $rutaArchivo)
$version = $xml.widget.version.Replace(".","")
$versionCode = $xml.widget.versionCode

Add-Type -AssemblyName System.IO.Compression.FileSystem
$compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
$includeBaseDirectory = $false

$filepath = Get-Item -Path ".\..\..\..\..\..\..\VTPortalJS\deploy Android\config.xml"
$sourcePath = [System.IO.Path]::GetDirectoryName($filepath.FullName)
$destinationPath = ('c:\temp\vtportal.android.c' + $versionCode + '.v' + $version + '.zip')

Write-Host ("... ... Generando zip Android en " + $destinationPath)
[System.IO.Compression.ZipFile]::CreateFromDirectory("$sourcePath","$destinationPath",$compressionLevel,$includeBaseDirectory)

uploadToVoltBuild $destinationPath "android"
