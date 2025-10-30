IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.Consents.RequieredDate')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.Consents.RequieredDate','20180604')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.Consents.ShowAlerts')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.Consents.ShowAlerts','false')
GO

UPDATE dbo.sysroParameters SET Data='441' WHERE ID='DBVersion'
GO

