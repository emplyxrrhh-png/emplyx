IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.Datalink.ChangeContractBeginDate')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.Datalink.ChangeContractBeginDate','True')
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='640' WHERE ID='DBVersion'
GO
