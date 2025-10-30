IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.Notification.DisableUserField')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.Notification.DisableUserField','')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.Notification.DisabledNotificationsList')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.Notification.DisabledNotificationsList','')
GO

ALTER TABLE dbo.Terminals
  ALTER COLUMN SerialNumber nvarchar(50)
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='448' WHERE ID='DBVersion'
GO