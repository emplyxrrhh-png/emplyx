update sysroGUI_Actions
set AppearsOnPopup = 1
where CssClass = 'btnTbPrint2' and IDGUIPath = 'Portal\Company\Groups'

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='562' WHERE ID='DBVersion'
GO