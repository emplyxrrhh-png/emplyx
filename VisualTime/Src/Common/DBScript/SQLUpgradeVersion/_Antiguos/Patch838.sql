-- No borréis esta línea

IF NOT EXISTS (SELECT 1 FROM ImportGuides WHERE ID ='27')
BEGIN
    INSERT [dbo].[ImportGuides] ([ID], [Name], [Template], [Mode], [Type], [FormatFilePath], [SourceFilePath], [Separator], [CopySource], [LastLog], [RequieredFunctionalities], [FeatureAliasID], [Destination], [Parameters], [Enabled], [Version], [Concept], [Active], [IDDefaultTemplate], [PreProcessScript], [Edition]) 
VALUES (27, N'Supervisors', 0, 1, 1, N'', N'', N'', 1, N'', N'Supervisors.DataLink.Imports.Supervisor', N'Supervisors', 0, NULL, 1, 2, N'Supervisors', NULL, NULL, NULL, NULL)
END 
GO

IF NOT EXISTS (SELECT 1 FROM ImportGuidesTemplates WHERE ID = 7)
BEGIN
    INSERT [dbo].[ImportGuidesTemplates] ([ID], [IDParentGuide], [Name], [Profile], [Parameters], [PostProcessScript], [PreProcessScript])
	VALUES (7, 27,  N'Básica', N'tmplImportsSupervisors.xlsx',  NULL, NULL, NULL)
END 
GO

IF DB_NAME() = 'euro4233'
BEGIN
	IF NOT EXISTS (SELECT 1 FROM [sysroFeatures] WHERE ID ='7800')
INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(7800,7,'Administration.DataLink.Imports','Importaciones','','U','RWA',NULL,NULL,1)


IF NOT EXISTS (SELECT 1 FROM [sysroFeatures] WHERE ID ='7810')
BEGIN
INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(7810,7800,'Supervisors.DataLink.Imports.Supervisor','Importación de supervisores','','U','RWA',NULL,NULL,1)


INSERT INTO [dbo].[sysroGroupFeatures_PermissionsOverFeatures] (IDGroupFeature,IDFeature,Permision) 
(select IDGroupFeature,7810,Permision from [dbo].[sysroGroupFeatures_PermissionsOverFeatures] WHERE [IDFeature] = 7)


INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] 
SELECT [IDPassport], 7810, Permission  FROM [dbo].[sysroPassports_PermissionsOverFeatures] 
WHERE [IDFeature] = 7
END
END


-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='838' WHERE ID='DBVersion'
GO
