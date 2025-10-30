IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'Process.DeadTaskRetry')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('Process.DeadTaskRetry','30')
GO
-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='720' WHERE ID='DBVersion'
GO
