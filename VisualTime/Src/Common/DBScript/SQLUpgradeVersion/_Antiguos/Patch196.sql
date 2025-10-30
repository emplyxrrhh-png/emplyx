-- Modificación del tipo del campo Definition de nvarchar a text
ALTER TABLE dbo.sysroReportProfile ALTER COLUMN Definition text
GO

-- Aumentamos el tamaño del campo Path para poder tener mayor profuncidad en el árbol de departamentos
ALTER TABLE dbo.groups ALTER COLUMN Path nvarchar(32)
GO

--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='196' WHERE ID='DBVersion'
GO


