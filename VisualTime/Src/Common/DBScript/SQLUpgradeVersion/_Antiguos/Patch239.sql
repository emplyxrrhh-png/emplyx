/* ***************************************************************************************************************************** */

-- Nueva estructura de tablas para la extension de Notificaciones
CREATE TABLE [dbo].[sysroNotificationTypes](
	[ID] [smallint] NOT NULL,
	[Name] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Scheduler] [int] NULL,
 CONSTRAINT [PK_sysroNotificationTypes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO sysroNotificationTypes VALUES(1,'Before Begin Contract',NULL, 120)
GO
INSERT INTO sysroNotificationTypes VALUES(2, 'Before Finish Contract',NULL,120)
GO
INSERT INTO sysroNotificationTypes VALUES(3,'Before Programmed Absence',NULL,230)
GO
INSERT INTO sysroNotificationTypes VALUES(4,'Begin Programmed Absence',NULL,230)
GO
INSERT INTO sysroNotificationTypes VALUES(5,'Before Finish Programmed Absence',NULL,230)
GO
INSERT INTO sysroNotificationTypes VALUES(6, 'No Working Days',NULL,70)
GO
INSERT INTO sysroNotificationTypes VALUES(7, 'Cut Programmed Absence',NULL,55)
GO
INSERT INTO sysroNotificationTypes VALUES(8,'Punches with incidences',NULL,5)
GO
INSERT INTO sysroNotificationTypes VALUES(9,'Invalid Access',NULL,5)
GO
INSERT INTO sysroNotificationTypes VALUES(10,'Error Messages',NULL,30)
GO
INSERT INTO sysroNotificationTypes VALUES(11,'Terminal Disconnected',NULL,1)
GO
INSERT INTO sysroNotificationTypes VALUES(12,'Access Dennied Framework Security',NULL,1)
GO

CREATE TABLE [dbo].[Notifications](
	[ID] [int] NOT NULL,
	[IDType] [smallint] NOT NULL,
	[Name] [nvarchar](128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Condition] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Destination] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Schedule] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Activated] [bit] NULL DEFAULT(0),
 CONSTRAINT [PK_Notifications] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[sysroNotificationTasks](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[IDNotification] [int] NOT NULL,
	[Key1Numeric] [int] NULL, 
	[Key2Numeric] [int] NULL, 
	[Key3DateTime] [datetime] NULL, 
	[Key4DateTime] [datetime] NULL, 
	[Parameters] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Executed] [bit] NULL DEFAULT(0),
	
 CONSTRAINT [PK_sysroNotificationTasks] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[sysroGUI] ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES
           ('Portal\Configuration\Notifications'
           ,'Notifications'
           ,'Notifications/Notifications.aspx'
           ,'Notificaciones.png'
           ,NULL
           ,NULL
           ,'Process\Notifier'
           ,NULL
           ,150
           ,'NWR'
           ,'U:Administration.Notifications.Definition=Read')
GO


ALTER TABLE ProgrammedAbsences ADD AutomaticClosed bit default(0)
GO
UPDATE  ProgrammedAbsences SET AutomaticClosed=0 WHERE AutomaticClosed IS NULL
GO

-- Nuevo permiso Notifications en sysroFeatures
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (7500 ,7 ,'Administration.Notifications' ,'Notificaciones' ,'' ,'U' ,'RWA' ,NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (7510 ,7500 ,'Administration.Notifications.Definition' ,'Administración notificaciones' ,'' ,'U' ,'RWA' ,NULL)
GO

-- Nuevo permiso planificación informes en sysroFeatures
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (7600 ,7 ,'Administration.ReportScheduler' ,'Planificador de informes' ,'' ,'U' ,'RWA' ,NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (7610 ,7600 ,'Administration.ReportScheduler.Definition' ,'Administración planificador de informes' ,'' ,'U' ,'RWA' ,NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (7620 ,7600 ,'Administration.ReportScheduler.EmergencyReport' ,'Lanzar informes de emergencia' ,'' ,'U' ,'R' ,NULL)
GO

-- Añadimos nueva columna IDPassport a la tabla ReportsScheduler
ALTER TABLE dbo.ReportsScheduler ADD
	IDPassport int NULL
GO
ALTER TABLE dbo.ReportsScheduler ADD CONSTRAINT
	FK_ReportsScheduler_sysroPassports FOREIGN KEY
	(
	IDPassport
	) REFERENCES dbo.sysroPassports
	(
	ID
	) 	 
	
GO
-- sysroGUI Planificador de informes
INSERT INTO [dbo].[sysroGUI]
           ([IDPath]
           ,[LanguageReference]
           ,[URL]
           ,[IconURL]
           ,[Type]
           ,[Parameters]
           ,[RequiredFeatures]
           ,[SecurityFlags]
           ,[Priority]
           ,[AllowedSecurity]
           ,[RequiredFunctionalities])
     VALUES
           ('Portal\Configuration\ReportScheduler'
           ,'ReportScheduler'
           ,'ReportScheduler/ReportScheduler.aspx'
           ,'ReportScheduler.png'
           ,NULL
           ,NULL
           ,'Process\ReportServer'
           ,NULL
           ,160
           ,'NWR'
           ,'U:Administration.ReportScheduler.Definition=Read')
GO


/* ***************************************************************************************************************************** */
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='239' WHERE ID='DBVersion'
GO
