IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =2325)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2325,2320,'Calendar.Punches.Requests.ExternalWorkWeekResume','Resumen semanal de trabajo externo','','U','RWA',NULL,15,2)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2325, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2320
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =20150)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(20150,20100,'Punches.Requests.ExternalWorkWeekResume','Resumen semanal de trabajo externo','','E','RW',NULL,NULL,NULL)
END
GO

EXEC sysro_GenerateAllPermissionsOverGroupsByEmployeeFeatureID 2
GO

INSERT INTO sysroRequestType (IdType,Type) VALUES(15,'ExternalWorkWeekResume')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'Notifications.AllowAssignShift')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('Notifications.AllowAssignShift','0')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 51)
	INSERT INTO [dbo].[sysroNotificationTypes] ([ID],[Name],[Description],[Scheduler],[Feature],[FeatureType],[OnlySystem])
     VALUES (51,'Assign Shift',NULL,360,'Calendar.Scheduler','U',0)
GO


ALTER TABLE dbo.sysroRequestDays ADD
	ActualType tinyint NULL,
	IDCause int NULL ,
	Comments nvarchar(max) NULL
GO


UPDATE dbo.sysroParameters SET Data='415' WHERE ID='DBVersion'
GO
