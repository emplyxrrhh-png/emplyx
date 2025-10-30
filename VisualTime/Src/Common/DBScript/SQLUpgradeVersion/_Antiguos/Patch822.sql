
ALTER TABLE [dbo].[DailySchedule] ADD TimestampEngine smalldatetime NULL
GO	

update [dbo].[DailySchedule] set TimestampEngine = '19000101'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'Process.Engine.DeadTaskRetry')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('Process.Engine.DeadTaskRetry','15')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'Process.Engine.DeadTaskCount')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('Process.Engine.DeadTaskCount','100')
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='822' WHERE ID='DBVersion'
GO
