IF not exists (select * from [dbo].sysroLiveAdvancedParameters where ParameterName like 'SecurityMode')
INSERT INTO [dbo].[sysroLiveAdvancedParameters]([ParameterName],[Value])
     VALUES ('SecurityMode','1')
GO

ALTER TABLE dbo.sysroLiveTasks ADD
	GUID nvarchar(40) NULL
GO

UPDATE sysroParameters SET Data='493' WHERE ID='DBVersion'
GO

