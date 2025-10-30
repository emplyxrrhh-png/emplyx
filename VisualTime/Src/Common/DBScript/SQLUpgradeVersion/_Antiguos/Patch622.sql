INSERT INTO [dbo].[sysroRequestType]
           ([IdType]
           ,[Type]
           ,[IDCategory])
     VALUES
           (17,'DailyRecord',6)
GO

-- Permiso declaración dependiendo de solicitudes para seguridad

DELETE [dbo].[sysroFeatures] WHERE [ID] = 2301
GO

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[EmployeeFeatureID],[Edition],[AliasID]) VALUES (2595,2500,'Calendar.DailyRecord','Declaración de jornada','','U','RWA',null, 2, null, 17)
GO

INSERT INTO [dbo].[sysroGroupFeatures_PermissionsOverFeatures] (IDGroupFeature,IDFeature,Permision) select IDGroupFeature,2595,Permision from [dbo].[sysroGroupFeatures_PermissionsOverFeatures] WHERE [IDFeature] = 2
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2595, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2
GO

EXEC dbo.sysro_GenerateAllPermissionsOverFeatures
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='622' WHERE ID='DBVersion'
GO
