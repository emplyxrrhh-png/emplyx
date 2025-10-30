IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'AnalyticsBIPersistOnSystem')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('AnalyticsBIPersistOnSystem','366')
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='662' WHERE ID='DBVersion'
GO
