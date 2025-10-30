-- En mx8 se puede definir apertura de relé independientemente del modo
update sysroReaderTemplates set Output = '1,0' where TYPE = 'mx8' and Output = '0'
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='343' WHERE ID='DBVersion'
GO


