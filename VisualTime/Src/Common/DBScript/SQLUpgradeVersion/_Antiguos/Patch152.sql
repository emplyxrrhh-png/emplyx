--
-- Elimita AccessEntries
--
DROP TABLE [dbo].[AccessEntries]
GO  
--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='152' WHERE ID='DBVersion'
GO

