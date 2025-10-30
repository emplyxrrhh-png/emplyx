INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (20002,20,'Punches.DailyRecord','Declaración de jornada','','E','W',NULL)
GO

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[EmployeeFeatureID],[Edition]) VALUES (2301,2,'Calendar.DailyRecord','Declaración de jornada','','U','RWA',null, 2, null)
GO

INSERT INTO [dbo].[sysroGroupFeatures_PermissionsOverFeatures] (IDGroupFeature,IDFeature,Permision) select IDGroupFeature,2301,Permision from [dbo].[sysroGroupFeatures_PermissionsOverFeatures] WHERE [IDFeature] = 2
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2301, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2
GO

EXEC dbo.sysro_GenerateAllPermissionsOverFeatures

ALTER TABLE [dbo].[Punches] ADD IDRequest INT
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='615' WHERE ID='DBVersion'
GO
