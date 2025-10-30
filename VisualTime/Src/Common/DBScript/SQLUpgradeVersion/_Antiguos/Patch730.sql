UPDATE sysroGUI
	SET RequiredFunctionalities = 'U:Disabled.Definition=Read' --U:Bots.Definition=Read
WHERE IDPath = 'Portal\Bots' 
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='730' WHERE ID='DBVersion'
GO
