UPDATE sysroReaderTemplates SET OHP ='1,0',ValidationMode = 'ServerLocal,Local'WHERE Type like 'mxART%' 
GO

UPDATE sysroReaderTemplates SET type = 'SAIWALL' WHERE type='SaiwallSecure'
GO

UPDATE sysroReaderTemplates SET OHP ='1,0' WHERE Type like 'Saiwall%' 
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='279' WHERE ID='DBVersion'
GO
