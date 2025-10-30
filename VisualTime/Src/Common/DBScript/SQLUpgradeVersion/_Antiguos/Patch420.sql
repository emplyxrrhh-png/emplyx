IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.IPRestriction.OnlyPunches')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.IPRestriction.OnlyPunches','0')
GO

UPDATE dbo.sysroParameters SET Data='420' WHERE ID='DBVersion'
GO
