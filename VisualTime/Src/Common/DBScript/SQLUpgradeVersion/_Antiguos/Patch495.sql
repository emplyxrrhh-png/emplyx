
IF not exists (select * from [dbo].[sysroLiveAdvancedParameters] where ParameterName = 'VTPortal.DefaultVersion')
	insert into [dbo].[sysroLiveAdvancedParameters] values ('VTPortal.DefaultVersion', 'default')
GO

UPDATE sysroParameters SET Data='495' WHERE ID='DBVersion'
GO

