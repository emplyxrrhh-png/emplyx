ALTER TABLE dbo.sysroSchedulerViews ADD
	GraphOptions nvarchar(4000) NULL
GO
 
INSERT INTO dbo.sysroLiveAdvancedParameters(ParameterName,Value) values ('AnalyticsGraphsEnabled','0')
GO

UPDATE dbo.sysroParameters SET Data='403' WHERE ID='DBVersion'
GO
