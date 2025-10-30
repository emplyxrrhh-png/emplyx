IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTPortal.ZoneRestrictedByIP')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTPortal.ZoneRestrictedByIP','0')
GO


-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='828' WHERE ID='DBVersion'
GO
