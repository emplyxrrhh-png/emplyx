$initial_path = (Get-Item .).FullName
Add-Type -Path ($initial_path + '\ChilkatDotNet47.dll')

    $vtpackages = ($initial_path + '\..\..\..\..\..\..\Common\VTPackages')
    $vtupdate = ($initial_path + '\..\..\..\..\..\..\Common\VTUpdate')


    $charset = New-Object Chilkat.Charset
    $charset.FromCharset = "utf-8"
    $charset.ToCharset = "ANSI"

    #$charset.ConvertFile('C:\Work\VTLive\Src\Common\DBScript\SQLUpgradeVersion\_Antiguos\Patch123.sql','c:\temp\patch123.sql') > $null



    Write-Host "... ... Copiando Database Legacy Updates"
    $copyFolder = $initial_path + '\..\..\..\..\..\..\Src\Common\DBScript\SQLUpgradeVersion\_Antiguos\*'
    Get-ChildItem -Path $copyFolder -Include *.sql | ForEach-Object {
        $dest = ($vtupdate + '\Core\Updates\')

        $destFileName = ((Get-Item $dest).FullName + $_.Name)


        $charset.ConvertFile($_.FullName,$destFileName) > $null
        #$charset.ConvertFile($_.FullName,(Get-Item $dest).FullName) > $null
    }
    Write-Host "... ... OK"

    