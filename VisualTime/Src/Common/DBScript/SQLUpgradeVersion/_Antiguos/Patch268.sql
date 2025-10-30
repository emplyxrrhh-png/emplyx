-- Insertamos los permisos para pedir solicitudes de horas de ausencia
INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT sysroPassports.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM sysroFeatures, sysroPassports		
WHERE sysroFeatures.ID IN (21150) AND
	  sysroPassports.IDParentPassport IS NULL AND sysroPassports.GroupType = 'U'
GO

--Integracion de Visitas en VisualTime Live
INSERT INTO [sysroFeatures] ([ID],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (23,'Visits','Visitas','','E','W',0)
GO

INSERT INTO [sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (23010,23,'Visits.Definition','Definir tipos de visitas','','E','W',0)
GO

INSERT INTO [sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (23020,23,'Visits.Program','Programar visitas','','E','W',0)
GO

INSERT INTO [sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (23030,23,'Visits.Receive','Recibir visitas','','E','W',0)
GO

INSERT INTO [sysroPassports_PermissionsOverFeatures] ([IDPassport],[IDFeature],[Permission])
VALUES (3,23,9)
GO

INSERT INTO [sysroPassports_PermissionsOverFeatures] ([IDPassport],[IDFeature],[Permission])
VALUES (3,23010,9)
GO

INSERT INTO [sysroPassports_PermissionsOverFeatures] ([IDPassport],[IDFeature],[Permission])
VALUES (3,23020,9)
GO

INSERT INTO [sysroPassports_PermissionsOverFeatures] ([IDPassport],[IDFeature],[Permission])
VALUES (3,23030,9)
GO

-- Nueva funcionalidad de Comedor en VisualTime
CREATE TABLE [DiningRoomTurns](
	[ID] [smallint] NOT NULL,
	[IDDiningRoom] [smallint] NULL,
	[Name] [nvarchar](128) NULL,
	[EmployeeSelection] [text] NULL,
	[BeginTime] [smalldatetime] NULL,
	[EndTime] [smalldatetime] NULL,
	[DaysOfWeek] [nvarchar](7) NULL)
GO

ALTER TABLE [DiningRoomTurns] WITH NOCHECK ADD CONSTRAINT [PK_DiningRoomTurns] PRIMARY KEY NONCLUSTERED (ID) 
GO

INSERT INTO [sysroFeatures] ([ID],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (24,'DiningRoom','Comedores','','U','RWA',0)
GO

INSERT INTO [sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (24100,24,'DiningRoom.Turns','Definición de Turnos para acceder al comedor','','U','RWA',0)
GO

INSERT INTO [sysroPassports_PermissionsOverFeatures] ([IDPassport],[IDFeature],[Permission])
VALUES (3,24,9)
GO

INSERT INTO [sysroPassports_PermissionsOverFeatures] ([IDPassport],[IDFeature],[Permission])
VALUES (3,24100,9)
GO

INSERT INTO [sysroGUI]
([IDPath],[LanguageReference],[IconURL],[RequiredFeatures],[Priority],[RequiredFunctionalities])
VALUES ('Portal\DiningRoom','DiningRoom','DiningRoom.png','Forms\DiningRoom',1010,'U:DiningRoom=Read')
GO

INSERT INTO [sysroGUI]
([IDPath],[LanguageReference],[URL],[IconURL],[RequiredFeatures],[Priority],[RequiredFunctionalities])
VALUES ('Portal\DiningRoom\DiningRoomXXX','DiningRoom','DiningRoom/DiningRoom.aspx','DiningRoom.png','Forms\DiningRoom',200,'U:DiningRoom.Turns=Read')
GO

UPDATE sysroGUI SET Priority = 2005 wHERE (IDPath = N'Portal\Security')
GO

UPDATE sysroGUI SET Priority = 2006 wHERE (IDPath = N'Portal\Configuration')
GO

INSERT INTO [sysroReaderTemplates]
([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput])
VALUES ('mx7',1,14,'DIN',1,'Interactive','Server','1,0','0','0','0','0')
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='268' WHERE ID='DBVersion'
GO
