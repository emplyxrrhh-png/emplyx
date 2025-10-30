/* Tabla Cameras */
CREATE TABLE [dbo].[Cameras](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] text NULL,
	[Model] [nvarchar](50) NULL,
	[Url] text NOT NULL, 
 CONSTRAINT [PK_Cameras] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

/* Añadimos IDCamera en TerminalReaders */
ALTER TABLE dbo.TerminalReaders ADD
	IDCamera int NULL
GO

/* Añadimos FK TerminalReaders */
ALTER TABLE [dbo].[TerminalReaders]  WITH CHECK ADD  CONSTRAINT [FK_TerminalReaders_Cameras] FOREIGN KEY([IDCamera])
REFERENCES [dbo].[Cameras] ([ID])
GO
ALTER TABLE [dbo].[TerminalReaders] CHECK CONSTRAINT [FK_TerminalReaders_Cameras]
GO

/* Añadimos IDCamera en Zones */
ALTER TABLE dbo.Zones ADD
	IDCamera int NULL
GO

/* Añadimos FK Zones */
ALTER TABLE [dbo].[Zones]  WITH CHECK ADD  CONSTRAINT [FK_Zones_Cameras] FOREIGN KEY([IDCamera])
REFERENCES [dbo].[Cameras] ([ID])
GO
ALTER TABLE [dbo].[Zones] CHECK CONSTRAINT [FK_Zones_Cameras]
GO

-- sysroFeatures
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (7400,7,'Administration.Cameras','Administración cámaras','','U','RWA',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (7410,7400,'Administration.Cameras.Definition','Definición','','U','RWA',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (7420,7400,'Administration.Cameras.Visualization','Visualización','','U','R',NULL)
GO

/* Damos permisos a la pantallas convenios */
INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT sysroPassports.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM sysroFeatures, sysroPassports		
WHERE sysroFeatures.ID IN (7400, 7410, 7420) AND
	  sysroPassports.IDParentPassport IS NULL AND sysroPassports.GroupType = 'U'
GO

/* Entrada a SysroGUI */
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Configuration\Cameras','Cameras','Cameras/Cameras.aspx','Cameras.png',140,'NWR','U:Administration.Cameras.Definition=Read','Version\Live')
GO

/* Tabla ZonePlanes */
CREATE TABLE [dbo].[ZonePlanes](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[PlaneImage] [image] NULL,
 CONSTRAINT [PK_ZonePlanes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/* Zones (add IDZonePlane) */
ALTER TABLE [dbo].[Zones]
ADD IDPlane [int] NULL
GO

ALTER TABLE [dbo].[Zones]  WITH CHECK ADD  CONSTRAINT [FK_Zones_ZonePlanes] FOREIGN KEY([IDPlane])
REFERENCES [dbo].[ZonePlanes] ([ID])
GO
ALTER TABLE [dbo].[Zones] CHECK CONSTRAINT [FK_Zones_ZonePlanes]
GO

/* ***************************************************************************************************************************** */

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='234' WHERE ID='DBVersion'
GO
