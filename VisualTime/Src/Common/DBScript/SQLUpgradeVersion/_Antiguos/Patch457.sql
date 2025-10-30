IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.AD.URL')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.AD.URL','')
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='457' WHERE ID='DBVersion'
GO
