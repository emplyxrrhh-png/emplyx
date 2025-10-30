-- No borréis esta línea
insert into sysroFeatures(ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees,AliasID,EmployeeFeatureID,Edition)
	values (1800,1,'Employees.Surveys','Encuestas','','U','RW',NULL,NULL,1,NULL)
GO

insert into sysroGroupFeatures_PermissionsOverFeatures(IDGroupFeature,IDFeature,Permision)(select IDGroupFeature, 1800, (CASE Permision WHEN 9 THEN 6 WHEN 3 THEN 0 ELSE Permision END) from sysroGroupFeatures_PermissionsOverFeatures where IDFeature = 1)
GO

insert into sysroPassports_PermissionsOverFeatures(IDPassport,IDFeature,Permission)(select IDPassport, 1800, (CASE Permission WHEN 9 THEN 6 WHEN 3 THEN 0 ELSE Permission END) from sysroPassports_PermissionsOverFeatures where IDFeature = 1)
GO

insert into sysroPermissionsOverFeatures(PassportID,FeatureID,Permission)(select PassportID,1800,(CASE Permission WHEN 9 THEN 6 WHEN 3 THEN 0 ELSE Permission END) from sysroPermissionsOverFeatures where FeatureID = 1)
GO

update sysroGUI set RequiredFunctionalities = 'U:Employees.Surveys=Write' where IDPath = 'Portal\Users\Surveys'
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='678' WHERE ID='DBVersion'
GO
