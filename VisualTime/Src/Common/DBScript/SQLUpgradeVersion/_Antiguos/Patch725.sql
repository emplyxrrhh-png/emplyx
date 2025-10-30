UPDATE sysroGUI
	SET RequiredFeatures = NULL
WHERE IDPath = 'Portal\Bots'
-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='725' WHERE ID='DBVersion'
GO
