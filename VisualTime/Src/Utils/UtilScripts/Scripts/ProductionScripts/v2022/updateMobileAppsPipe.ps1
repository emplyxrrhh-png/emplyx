Write-Host "... ... Copiando ficheros Android"
copy ".\..\..\..\..\..\VTPortalWeb\index.html" ".\..\..\..\..\..\VTPortalJS\deploy Android\www\index.html" > $null
copy ".\..\..\..\..\..\VTPortalWeb\index.js" ".\..\..\..\..\..\VTPortalJS\deploy Android\www\index.js" > $null
xcopy ".\..\..\..\..\..\VTPortalWeb\js" ".\..\..\..\..\..\VTPortalJS\deploy Android\www\js" /sy > $null
xcopy ".\..\..\..\..\..\VTPortalWeb\1" ".\..\..\..\..\..\VTPortalJS\deploy Android\www\1" /sy > $null
xcopy ".\..\..\..\..\..\VTPortalWeb\2" ".\..\..\..\..\..\VTPortalJS\deploy Android\www\2" /sy > $null

Write-Host "... ... Borrando ficheros servidor Android"
del ".\..\..\..\..\..\VTPortalJS\deploy Android\www\2\indexv2.aspx" > $null
del ".\..\..\..\..\..\VTPortalJS\deploy Android\www\2\indexv2.aspx.designer.vb" > $null
del ".\..\..\..\..\..\VTPortalJS\deploy Android\www\2\indexv2.aspx.vb" > $null


Write-Host "... ... Copiando ficheros IOS"
copy ".\..\..\..\..\..\VTPortalWeb\index.html" ".\..\..\..\..\..\VTPortalJS\deploy iOS\www\index.html" > $null
copy ".\..\..\..\..\..\VTPortalWeb\index.js" ".\..\..\..\..\..\VTPortalJS\deploy iOS\www\index.js" > $null
xcopy ".\..\..\..\..\..\VTPortalWeb\js" ".\..\..\..\..\..\VTPortalJS\deploy iOS\www\js" /sy > $null
xcopy ".\..\..\..\..\..\VTPortalWeb\1" ".\..\..\..\..\..\VTPortalJS\deploy iOS\www\1" /sy > $null
xcopy ".\..\..\..\..\..\VTPortalWeb\2" ".\..\..\..\..\..\VTPortalJS\deploy iOS\www\2" /sy > $null

Write-Host "... ... Borrando ficheros servidor iOS"
del ".\..\..\..\..\..\VTPortalJS\deploy iOS\www\2\indexv2.aspx" > $null
del ".\..\..\..\..\..\VTPortalJS\deploy iOS\www\2\indexv2.aspx.designer.vb" > $null
del ".\..\..\..\..\..\VTPortalJS\deploy iOS\www\2\indexv2.aspx.vb" > $null
