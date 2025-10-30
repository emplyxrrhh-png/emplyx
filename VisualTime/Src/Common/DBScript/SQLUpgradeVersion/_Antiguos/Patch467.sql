INSERT INTO [dbo].[sysroLiveAdvancedParameters]([ParameterName],[Value])
	 VALUES ('VTLive.Notifications.RulesNotifyFrom',substring(convert(nvarchar,dateadd(day,-1,getdate()),120),1,10))
 GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='467' WHERE ID='DBVersion'
GO
