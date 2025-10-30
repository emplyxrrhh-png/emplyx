
-- Añadimos la columna del turno para poder realizar los informes de turnos de comedor.
ALTER TABLE [AccessMoves] ADD [IDTurn] [smallint] NOT NULL CONSTRAINT [DF_AccessMoves_Turns]  DEFAULT ((0))
GO

-- Corregir nombre incorrecto del registro
UPDATE sysroGUI SET IDPath = 'Portal\DiningRoom\DiningRoomTurn' WHERE IDPath = 'Portal\DiningRoom\DiningRoomXXX'
GO

-- FUNCIONALIDAD DE PERMISOS POR CATEGORIAS
INSERT INTO [sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (2011,2000,'Calendar.Scheduler.Cat1','Categoría de Permisos 1','','U','RW',0)
GO
INSERT INTO [sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (2012,2000,'Calendar.Scheduler.Cat2','Categoría de Permisos 2','','U','RW',0)
GO
INSERT INTO [sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (2013,2000,'Calendar.Scheduler.Cat3','Categoría de Permisos 3','','U','RW',0)
GO
INSERT INTO [sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (2014,2000,'Calendar.Scheduler.Cat4','Categoría de Permisos 4','','U','RW',0)
GO
INSERT INTO [sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (2015,2000,'Calendar.Scheduler.Cat5','Categoría de Permisos 5','','U','RW',0)
GO
INSERT INTO [sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (2016,2000,'Calendar.Scheduler.Cat6','Categoría de Permisos 6','','U','RW',0)
GO
INSERT INTO [sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (2017,2000,'Calendar.Scheduler.Cat7','Categoría de Permisos 7','','U','RW',0)
GO
INSERT INTO [sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (2018,2000,'Calendar.Scheduler.Cat8','Categoría de Permisos 8','','U','RW',0)
GO
INSERT INTO [sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (2019,2000,'Calendar.Scheduler.Cat9','Categoría de Permisos 9','','U','RW',0)
GO
INSERT INTO [sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (2020,2000,'Calendar.Scheduler.Cat10','Categoría de Permisos 10','','U','RW',0)
GO
INSERT INTO [sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (2021,2000,'Calendar.Scheduler.Cat11','Categoría de Permisos 11','','U','RW',0)
GO
INSERT INTO [sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (2022,2000,'Calendar.Scheduler.Cat12','Categoría de Permisos 12','','U','RW',0)
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT 3 AS IDPassport, 2011 AS IDFeature, sysroPassports_PermissionsOverFeatures.Permission
FROM sysroPassports_PermissionsOverFeatures
WHERE sysroPassports_PermissionsOverFeatures.IDPassport = 3 AND sysroPassports_PermissionsOverFeatures.IDFeature = 2000
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT 3 AS IDPassport, 2012 AS IDFeature, sysroPassports_PermissionsOverFeatures.Permission
FROM sysroPassports_PermissionsOverFeatures
WHERE sysroPassports_PermissionsOverFeatures.IDPassport = 3 AND sysroPassports_PermissionsOverFeatures.IDFeature = 2000
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT 3 AS IDPassport, 2013 AS IDFeature, sysroPassports_PermissionsOverFeatures.Permission
FROM sysroPassports_PermissionsOverFeatures
WHERE sysroPassports_PermissionsOverFeatures.IDPassport = 3 AND sysroPassports_PermissionsOverFeatures.IDFeature = 2000
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT 3 AS IDPassport, 2014 AS IDFeature, sysroPassports_PermissionsOverFeatures.Permission
FROM sysroPassports_PermissionsOverFeatures
WHERE sysroPassports_PermissionsOverFeatures.IDPassport = 3 AND sysroPassports_PermissionsOverFeatures.IDFeature = 2000
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT 3 AS IDPassport, 2015 AS IDFeature, sysroPassports_PermissionsOverFeatures.Permission
FROM sysroPassports_PermissionsOverFeatures
WHERE sysroPassports_PermissionsOverFeatures.IDPassport = 3 AND sysroPassports_PermissionsOverFeatures.IDFeature = 2000
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT 3 AS IDPassport, 2016 AS IDFeature, sysroPassports_PermissionsOverFeatures.Permission
FROM sysroPassports_PermissionsOverFeatures
WHERE sysroPassports_PermissionsOverFeatures.IDPassport = 3 AND sysroPassports_PermissionsOverFeatures.IDFeature = 2000
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT 3 AS IDPassport, 2017 AS IDFeature, sysroPassports_PermissionsOverFeatures.Permission
FROM sysroPassports_PermissionsOverFeatures
WHERE sysroPassports_PermissionsOverFeatures.IDPassport = 3 AND sysroPassports_PermissionsOverFeatures.IDFeature = 2000
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT 3 AS IDPassport, 2018 AS IDFeature, sysroPassports_PermissionsOverFeatures.Permission
FROM sysroPassports_PermissionsOverFeatures
WHERE sysroPassports_PermissionsOverFeatures.IDPassport = 3 AND sysroPassports_PermissionsOverFeatures.IDFeature = 2000
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT 3 AS IDPassport, 2019 AS IDFeature, sysroPassports_PermissionsOverFeatures.Permission
FROM sysroPassports_PermissionsOverFeatures
WHERE sysroPassports_PermissionsOverFeatures.IDPassport = 3 AND sysroPassports_PermissionsOverFeatures.IDFeature = 2000
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT 3 AS IDPassport, 2020 AS IDFeature, sysroPassports_PermissionsOverFeatures.Permission
FROM sysroPassports_PermissionsOverFeatures
WHERE sysroPassports_PermissionsOverFeatures.IDPassport = 3 AND sysroPassports_PermissionsOverFeatures.IDFeature = 2000
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT 3 AS IDPassport, 2021 AS IDFeature, sysroPassports_PermissionsOverFeatures.Permission
FROM sysroPassports_PermissionsOverFeatures
WHERE sysroPassports_PermissionsOverFeatures.IDPassport = 3 AND sysroPassports_PermissionsOverFeatures.IDFeature = 2000
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT 3 AS IDPassport, 2022 AS IDFeature, sysroPassports_PermissionsOverFeatures.Permission
FROM sysroPassports_PermissionsOverFeatures
WHERE sysroPassports_PermissionsOverFeatures.IDPassport = 3 AND sysroPassports_PermissionsOverFeatures.IDFeature = 2000
GO

ALTER TABLE [Groups] ADD [DescriptionGroup] [nvarchar](100) default ''
GO

ALTER TABLE [ShiftGroups] ADD [IdCategory] smallint default 0 
GO

UPDATE ShiftGroups SET IdCategory=0
GO

INSERT INTO [sysroNotificationTypes] ([ID],[Name],[Scheduler])
VALUES (15,'Employee Not Arrived or Late',5)
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='270' WHERE ID='DBVersion'
GO

