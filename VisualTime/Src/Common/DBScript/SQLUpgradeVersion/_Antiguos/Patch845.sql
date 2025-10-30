IF NOT EXISTS (SELECT 1 FROM ImportGuidesTemplates WHERE ID = 8)
BEGIN
    INSERT [dbo].[ImportGuidesTemplates] ([ID], [IDParentGuide], [Name], [Profile], [Parameters], [PostProcessScript], [PreProcessScript])
	VALUES (8, 20,  N'SAGE Murano', '',  NULL, NULL, NULL)
END 
GO

IF NOT EXISTS (SELECT 1 FROM [sysroFeatures] WHERE ID ='1614')
BEGIN
INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1614,1610,'Employees.DataLink.Imports.SageMurano','Importación de empleados desde SAGE Murano','','U','RWA',NULL,NULL,1)


INSERT INTO [dbo].[sysroGroupFeatures_PermissionsOverFeatures] (IDGroupFeature,IDFeature,Permision) 
(select IDGroupFeature,1614,Permision from [dbo].[sysroGroupFeatures_PermissionsOverFeatures] WHERE [IDFeature] = 1610)


END

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='845' WHERE ID='DBVersion'

GO
