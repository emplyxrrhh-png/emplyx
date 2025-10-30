-- No borréis esta línea
IF NOT EXISTS (SELECT 1 FROM [sysroFeatures] WHERE ID ='7800')
BEGIN
INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(7800,7,'Administration.ExportGuidesTemplates','Configuración de las plantillas de exportación','','U','A',NULL,NULL,NULL)


INSERT INTO [dbo].[sysroGroupFeatures_PermissionsOverFeatures] (IDGroupFeature,IDFeature,Permision) 
(select IDGroupFeature,7800,Permision from [dbo].[sysroGroupFeatures_PermissionsOverFeatures] WHERE [IDFeature] = 7)


END

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='966' WHERE ID='DBVersion'
GO
