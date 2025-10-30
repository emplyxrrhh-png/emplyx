-- Añadir imagen a las fases
ALTER TABLE dbo.JobTemplates ADD Image image NULL
GO

ALTER TABLE dbo.Jobs ADD Image image NULL
GO


--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='181' WHERE ID='DBVersion'
GO
