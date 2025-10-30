update dbo.sysroReaderTemplates set Direction = NULL where type in ('mxV', 'mx9')
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='477' WHERE ID='DBVersion'
GO

