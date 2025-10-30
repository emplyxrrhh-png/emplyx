UPDATE dbo.sysrogui SET RequiredFunctionalities = NULL WHERE IDPath = 'Portal\Configuration'
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='338' WHERE ID='DBVersion'
GO


