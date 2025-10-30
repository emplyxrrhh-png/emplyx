
insert into sysroFeatures values
	(36,NULL,'Bots','Bots','','U','A',NULL,NULL,NULL,NULL),
	(36000,36,'Bots.Definition','Definición de Bots','','U','A',NULL,NULL,NULL,NULL)


INSERT INTO [dbo].[sysroGroupFeatures_PermissionsOverFeatures] (IDGroupFeature,IDFeature,Permision) 
(select IDGroupFeature,36,0 from [dbo].[sysroGroupFeatures_PermissionsOverFeatures] WHERE [IDFeature] = 2)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] 
SELECT [IDPassport], 36, 0  FROM [dbo].[sysroPassports_PermissionsOverFeatures] 
WHERE [IDFeature] = 2

INSERT INTO [dbo].[sysroGroupFeatures_PermissionsOverFeatures] (IDGroupFeature,IDFeature,Permision) 
select IDGroupFeature,36000,0 from [dbo].[sysroGroupFeatures_PermissionsOverFeatures] WHERE [IDFeature] = 2
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] 
SELECT [IDPassport], 36000, 0  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2 

update [dbo].[sysroGroupFeatures_PermissionsOverFeatures] set Permision = 9 where IDGroupFeature in (SELECT ID from sysroGroupFeatures where Name = '@@ROBOTICS@@System')
GO

update [dbo].[sysroPassports_PermissionsOverFeatures] set Permission = 9 where IDPassport in (SELECT ID from sysroPassports where IDGroupFeature = (select id from sysroGroupFeatures where Name = '@@ROBOTICS@@System'))
GO

EXEC dbo.sysro_GenerateAllPermissionsOverFeatures
GO
-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='723' WHERE ID='DBVersion'
GO
