IF NOT EXISTS (SELECT 1 FROM [dbo].sysroFeatures WHERE ID = 1850)
	insert into sysroFeatures values
		(1850,1,'Employees.Channels','Canales de comunicación','','U','RWA',NULL,NULL,1,NULL),
		(1900,1,'Employees.Complaints','Canal de denuncias','','U','A',NULL,NULL,1,NULL)
GO


INSERT INTO [dbo].[sysroGroupFeatures_PermissionsOverFeatures] (IDGroupFeature,IDFeature,Permision) select IDGroupFeature,1850,0 from [dbo].[sysroGroupFeatures_PermissionsOverFeatures] WHERE [IDFeature] = 2
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1850, 0  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2
GO

INSERT INTO [dbo].[sysroGroupFeatures_PermissionsOverFeatures] (IDGroupFeature,IDFeature,Permision) select IDGroupFeature,1900,0 from [dbo].[sysroGroupFeatures_PermissionsOverFeatures] WHERE [IDFeature] = 2
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1900, 0  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2
GO


update [dbo].[sysroGroupFeatures_PermissionsOverFeatures] set Permision = 9 where IDGroupFeature in (SELECT ID from sysroGroupFeatures where Name = '@@ROBOTICS@@System')
GO

update [dbo].[sysroPassports_PermissionsOverFeatures] set Permission = 9 where IDPassport in (SELECT IDParentPassport from sysroPassports where IDGroupFeature = (select id from sysroGroupFeatures where Name = '@@ROBOTICS@@System'))
GO

EXEC dbo.sysro_GenerateAllPermissionsOverFeatures
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='709' WHERE ID='DBVersion'
GO
