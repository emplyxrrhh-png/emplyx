-- Actualiza versión de api a ADFS
UPDATE dbo.sysroLiveAdvancedParameters SET Value = 7 WHERE ParameterName='VTPortalApiVersion'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'LivePortal.RedirectToVTPortal')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('LivePortal.RedirectToVTPortal','false')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'SupervisorPortal.RedirectToVTPortal')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('SupervisorPortal.RedirectToVTPortal','false')
GO
-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='456' WHERE ID='DBVersion'
GO
