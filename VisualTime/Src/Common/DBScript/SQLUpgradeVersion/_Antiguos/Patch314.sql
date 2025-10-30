
--NUEVO REGISTRO
INSERT INTO dbo.sysroParameters(ID, Data) VALUES ('LOCATIONS', '')
GO


-- Borramos preventivamente todos los registros de la tabla sysropassport_sessions para evitar entradas antiguas malas
DELETE FROM dbo.sysroPassports_Sessions
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='314' WHERE ID='DBVersion'
GO

