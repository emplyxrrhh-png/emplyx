-- Crea tabla Temporal para listados con Leyenda
CREATE TABLE [dbo].[TMPCONCEPT] (
	[ConceptName] [nvarchar] NULL ,
	[ShortName] [nvarchar] NULL
) ON [PRIMARY]
GO
--
-- Tabla Terminales
--
-- Eliminar referencias
ALTER TABLE [dbo].[Terminals] DROP CONSTRAINT [FK_Terminals_Zones]
GO
ALTER TABLE [dbo].[Terminals] DROP CONSTRAINT [FK_Terminals_Zones1]
GO
ALTER TABLE [dbo].[TerminalSirens] DROP CONSTRAINT [FK_TerminalSirens_Terminals]
GO
DROP TABLE [dbo].[Terminals]
GO
-- Crear tabla
CREATE TABLE [dbo].[Terminals]
    (
  [ID] [tinyint] NOT NULL ,
  [Description] [nvarchar] (50) NULL ,
  [Reader1IDZone] [tinyint] NULL ,
  [Reader2IDZone] [tinyint] NULL ,
  [PictureX] [numeric] (8, 3) NULL ,
  [PictureY] [numeric] (8, 3) NULL ,
  [Type] [nvarchar] (50) NULL ,
  [Behavior] [nvarchar] (50) NULL ,
  [Location] [nvarchar] (50) NULL ,
  [LastUpdate] [smalldatetime] NULL ,
  [LastStatus] [nvarchar] (50) NULL ,
  [Reader1Mode] [nvarchar] (20) NULL ,
  [Reader2Mode] [nvarchar] (20) NULL ,
  [Reader1Output] [tinyint] NULL ,
  [Reader2Output] [tinyint] NULL ,
  [SirensOutput] [tinyint] NULL 
)
GO
-- Añadir valores defecto
ALTER TABLE [dbo].[Terminals] WITH NOCHECK
ADD
 CONSTRAINT [DF_Terminals_Reader1Mode] DEFAULT ('') FOR Reader1Mode,
 CONSTRAINT [DF_Terminals_Reader2Mode] DEFAULT ('') FOR Reader2Mode,
 CONSTRAINT [DF_Terminals_Reader1Output] DEFAULT (0) FOR Reader1Output,
 CONSTRAINT [DF_Terminals_Reader2Output] DEFAULT (0) FOR Reader2Output,
 CONSTRAINT [DF_Terminals_SirensOutput] DEFAULT (0) FOR SirensOutput
GO
-- Añadir Clave Principal
ALTER TABLE [dbo].[Terminals] WITH NOCHECK ADD  CONSTRAINT [PK_Terminales] PRIMARY KEY NONCLUSTERED (ID) 
GO
-- Añadir Claves Externas
ALTER TABLE [dbo].[Terminals] ADD CONSTRAINT [FK_Terminals_Zones] FOREIGN KEY (Reader1IDZone)  REFERENCES [dbo].[Zones] (ID) 
GO
ALTER TABLE [dbo].[Terminals] ADD CONSTRAINT [FK_Terminals_Zones1] FOREIGN KEY (Reader2IDZone)  REFERENCES [dbo].[Zones] (ID) 
GO
ALTER TABLE [dbo].[TerminalSirens] ADD CONSTRAINT [FK_TerminalSirens_Terminals] FOREIGN KEY (IDTerminal)  REFERENCES [dbo].[Terminals] (ID) 
GO

-- Insertamos los valores de los tipos de piezas de prouccion

DELETE FROM  [dbo].[sysroPieceTypes]
GO

INSERT INTO sysroPieceTypes(ID, Name, IsConsideredValid, CountOnPerformance) 
	VALUES (0,'Ninguna',0,0)
GO

INSERT INTO sysroPieceTypes(ID, Name, IsConsideredValid, CountOnPerformance) 
	VALUES (1,'Buenas',1,1)
GO

INSERT INTO sysroPieceTypes(ID, Name, IsConsideredValid, CountOnPerformance) 
	VALUES (2,'Malas',0,0)
GO

INSERT INTO sysroPieceTypes(ID,Name,IsConsideredValid,CountOnPerformance) VALUES 
	(3,'Desechadas',0,0)
GO


DELETE FROM  sysroProductionFields
GO


-- Insertamos los campos de produccion
INSERT INTO sysroProductionFields(ID,Order_Job,Name,TypeField,DefaultValue,AutoLauncher) 
	VALUES ('a',0,'Fase1',0,'',0)
GO
INSERT INTO sysroProductionFields(ID,Order_Job,Name,TypeField,DefaultValue,AutoLauncher)
  VALUES ('b',0,'Fase2',0,'',0)
GO
INSERT INTO sysroProductionFields(ID,Order_Job,Name,TypeField,DefaultValue,AutoLauncher)
  VALUES ('c',0,'Fase3',0,'',0)
GO
INSERT INTO sysroProductionFields(ID,Order_Job,Name,TypeField,DefaultValue,AutoLauncher)
  VALUES ('d',0,'Fase4',0,'',0)
GO

INSERT INTO sysroProductionFields(ID,Order_Job,Name,TypeField,DefaultValue,AutoLauncher)
  VALUES ('e',0,'Fase5',1,'',0)
GO

INSERT INTO sysroProductionFields(ID,Order_Job,Name,TypeField,DefaultValue,AutoLauncher)
  VALUES ('f',0,'Fase6',1,'',0)
GO

INSERT INTO sysroProductionFields(ID,Order_Job,Name,TypeField,DefaultValue,AutoLauncher)
  VALUES ('g',0,'Fase7',2,'',0)
GO

INSERT INTO sysroProductionFields(ID,Order_Job,Name,TypeField,DefaultValue,AutoLauncher)
  VALUES ('h',0,'Fase8',2,'',0)
GO

INSERT INTO sysroProductionFields(ID,Order_Job,Name,TypeField,DefaultValue,AutoLauncher)
  VALUES ('a',1,'Orden1',0,'',0)
GO
INSERT INTO sysroProductionFields(ID,Order_Job,Name,TypeField,DefaultValue,AutoLauncher)
  VALUES ('b',1,'Orden2',0,'',0)
GO
INSERT INTO sysroProductionFields(ID,Order_Job,Name,TypeField,DefaultValue,AutoLauncher)
  VALUES ('c',1,'Orden3',0,'',0)
GO
INSERT INTO sysroProductionFields(ID,Order_Job,Name,TypeField,DefaultValue,AutoLauncher)
  VALUES ('d',1,'Orden4',0,'',0)
GO

INSERT INTO sysroProductionFields(ID,Order_Job,Name,TypeField,DefaultValue,AutoLauncher)
  VALUES ('e',1,'Orden5',1,'',0)
GO

INSERT INTO sysroProductionFields(ID,Order_Job,Name,TypeField,DefaultValue,AutoLauncher)
  VALUES ('f',1,'Orden6',1,'',0)
GO

INSERT INTO sysroProductionFields(ID,Order_Job,Name,TypeField,DefaultValue,AutoLauncher)
  VALUES ('g',1,'Orden7',2,'',0)
GO

INSERT INTO sysroProductionFields(ID,Order_Job,Name,TypeField,DefaultValue,AutoLauncher)
  VALUES ('h',1,'Orden8',2,'',0)
GO

---
--- Actualiza roSysUI
---

DELETE FROM [dbo].[sysroGUI]
GO
IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[dbo].[prcTmp_SDC_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[prcTmp_SDC_Insert]
GO
CREATE PROCEDURE prcTmp_SDC_Insert(@P1 AS nvarchar (200), @P2 AS nvarchar (64), @P3 AS nvarchar (200), @P4 AS nvarchar (200), @P5 AS nvarchar (64), @P6 AS nvarchar (200), @P7 AS nvarchar (200), @P8 AS nvarchar (50), @P9 AS int, @P10 AS nvarchar (25)) AS
INSERT INTO [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity]) VALUES (@P1, @P2, @P3, @P4, @P5, @P6, @P7, @P8, @P9, @P10)
GO
EXEC prcTmp_SDC_Insert 'Menu', 'MainMenu', NULL, NULL, NULL, NULL, NULL, '3111111111', 500, 'N'
GO
EXEC prcTmp_SDC_Insert 'Menu\Action', 'Action', NULL, NULL, 'CONTAINER', 'ACTION', NULL, '3111111111', 580, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\Edit', 'Edit', NULL, NULL, 'CONTAINER', 'EDIT', NULL, '3111111111', 530, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\File', 'File', NULL, NULL, 'CONTAINER', 'FILE', NULL, '3111111111', 510, 'N'
GO
EXEC prcTmp_SDC_Insert 'Menu\File\Close', 'Close', 'fn:\\ExitProgram', NULL, NULL, NULL, NULL, '3111111111', 520, 'N'
GO
EXEC prcTmp_SDC_Insert 'Menu\Help', 'Help', NULL, NULL, 'CONTAINER', 'HELP', NULL, '3111111111', 620, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\Help\About', 'About', 'fn:\\ShowAbout', NULL, NULL, NULL, NULL, '3111111111', 690, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\Help\Agent', 'Agent', NULL, NULL, 'CHECKBOX', 'AGENT', NULL, '3111111111', 722, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\Help\Contents', 'Contents', 'fn:\\Help_ShowContents', NULL, NULL, NULL, NULL, '3111111111', 630, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\Help\HelpS1', '^SEPARATOR', NULL, NULL, NULL, NULL, NULL, '3111111111', 660, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\Help\HelpS2', '^SEPARATOR', NULL, NULL, NULL, NULL, NULL, '3111111111', 680, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\Help\HelpS3', '^SEPARATOR', NULL, NULL, NULL, NULL, NULL, '3111111111', 700, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\Help\S4', '^SEPARATOR', NULL, NULL, NULL, NULL, NULL, '3111111111', 721, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\Help\Search', 'Search', 'fn:\\Help_ShowSearch', NULL, NULL, NULL, NULL, '3111111111', 650, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\Help\Support', 'Support', NULL, NULL, NULL, NULL, NULL, '3111111111', 710, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\Help\Support\Downloads', 'Downloads', 'http://www.visualtime.net', NULL, NULL, NULL, NULL, '3111111111', 720, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\Help\TOC', 'TOC', 'fn:\\Help_ShowIndex', NULL, NULL, NULL, NULL, '3111111111', 640, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\Help\Tutorial', 'Tutorial', 'fn:\\Help_Tutorial', NULL, NULL, NULL, NULL, '3111111111', 670, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\Tools', 'Tools', NULL, NULL, 'CONTAINER', 'TOOLS', NULL, '3111111111', 590, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\Tools\ChangePassword', 'ChangePassword', 'fn:\\ShowPassword', NULL, NULL, NULL, NULL, '3111111111', 585, 'NW'
GO
EXEC prcTmp_SDC_Insert 'Menu\Tools\CommsOptions', 'CommsOptions', 'file://$(VTSYSTEMPATH)\roComms.exe', NULL, NULL, NULL, 'Process\Comms', '3111111111', 610, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\Tools\Options', 'Options', 'roFormOptions.vbd', NULL, NULL, NULL, NULL, '3111111111', 600, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\Tools\S5', '^SEPARATOR', NULL, NULL, NULL, NULL, NULL, '3111111111', 587, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\View', 'View', NULL, NULL, 'CONTAINER', 'VIEW', NULL, '3111111111', 540, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\View\Bar', 'Bar', NULL, NULL, 'CHECKBOX', 'NAVBAR', NULL, '3111111111', 550, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\View\Status', 'Status', NULL, NULL, 'CHECKBOX', 'STATUSBAR', NULL, '3111111111', 560, 'NR'
GO
EXEC prcTmp_SDC_Insert 'Menu\View\Tasks', 'Tasks', NULL, NULL, 'CHECKBOX', 'USERTASKS', NULL, '3111111111', 570, 'NR'
GO
EXEC prcTmp_SDC_Insert 'NavBar', 'NavBar', 'fn://ShowStartupForm', NULL, NULL, NULL, NULL, '3111111111', 400, 'N'
GO
EXEC prcTmp_SDC_Insert 'NavBar\Config', 'Config', NULL, NULL, NULL, NULL, NULL, '3111111111', 900, 'NR'
GO
EXEC prcTmp_SDC_Insert 'NavBar\Config\Options', 'Options', 'roFormOptions.vbd', 'Options.ico', NULL, NULL, NULL, '3111111111', 900, 'NW'
GO
EXEC prcTmp_SDC_Insert 'NavBar\Config\Terminals', 'Terminals', 'roFormTerminals.vbd', 'ReaderOnly_32_256.ico', NULL, NULL, NULL, '3111111111', 900, 'NWR'
GO
EXEC prcTmp_SDC_Insert 'NavBar\Config\Users', 'Users', 'roFormUsers.vbd', 'UserRights.ico', NULL, NULL, 'Forms\Users', '3111111111', 1000, 'N'
GO
EXEC prcTmp_SDC_Insert 'NavBar\FirstTab', 'Shortcuts', NULL, NULL, NULL, NULL, NULL, '3111111111', 500, 'NR'
GO
EXEC prcTmp_SDC_Insert 'NavBar\FirstTab\AccessGroups', 'AccessGroups', 'roFormAccessGroups.vbd', 'AccessGroups.ico', NULL, NULL, 'Forms\Access', '3111111111', 200, 'NRW'
GO
EXEC prcTmp_SDC_Insert 'NavBar\FirstTab\Concepts', 'Concepts', 'roFormConcepts.vbd', 'Causes.ico', NULL, NULL, 'Forms\Causes', '3111111111', 790, 'NWR'
GO
EXEC prcTmp_SDC_Insert 'NavBar\FirstTab\Employees', 'Employees', 'roFormEmployees.vbd', 'men.ico', NULL, NULL, 'Forms\Employees', '3111111111', 750, 'NWR'
GO
EXEC prcTmp_SDC_Insert 'NavBar\FirstTab\Reports', 'Reports', 'roFormReports.vbd', 'reports.ico', NULL, NULL, 'Forms\Reports', '3111111111', 770, 'NW'
GO
EXEC prcTmp_SDC_Insert 'NavBar\FirstTab\Scheduler', 'Scheduler', 'roFormScheduler.VBD', 'calendar.ico', NULL, NULL, 'Forms\Calendar', '3111111111', 760, 'NWR'
GO
EXEC prcTmp_SDC_Insert 'NavBar\FirstTab\Shifts', 'Shifts', 'roFormShifts.vbd', 'shifts.ico', NULL, NULL, 'Forms\Shifts', '3111111111', 780, 'NWR'
GO
EXEC prcTmp_SDC_Insert 'NavBar\Job', 'JobTab', NULL, NULL, NULL, NULL, NULL, '3111111111', 600, 'NR'
GO
EXEC prcTmp_SDC_Insert 'NavBar\Job\JobIncidences', 'JobIncidences', 'roFormJobIncidences.vbd', 'JobIncidences.ico', NULL, NULL, 'Forms\JobIncidences', '3111111111', 820, 'NWR'
GO
EXEC prcTmp_SDC_Insert 'NavBar\Job\JobStatus', 'JobStatus', 'roFormStatusJobs.vbd', 'OrderStatus.ico', NULL, NULL, 'Forms\JobStatus', '3111111111', 999, 'NWR'
GO
EXEC prcTmp_SDC_Insert 'NavBar\Job\JobTemplates', 'JobTemplates', 'roFormJobs.vbd', 'Order.ico', NULL, NULL, 'Forms\JobTemplates', '3111111111', 795, 'NWR'
GO
EXEC prcTmp_SDC_Insert 'NavBar\Job\Machines', 'Machines', 'roFormMachines.vbd', 'Machine.ico', NULL, NULL, 'Forms\Machines', '3111111111', 790, 'NWR'
GO
EXEC prcTmp_SDC_Insert 'NavBar\Job\MachineStatus', 'MachineStatus', 'roFormStatusMachine.vbd', 'MachineStatus.ico', NULL, NULL, 'Forms\MachineStatus', '3111111111', 1000, 'NWR'
GO
EXEC prcTmp_SDC_Insert 'NavBar\Job\Teams', 'Teams', 'roFormTeams.vbd', 'Team.ico', NULL, NULL, 'Forms\Teams', '3111111111', 810, 'NWR'
GO
DROP PROCEDURE prcTmp_SDC_Insert
GO

--
-- Actualiza sysroUserGroups
--

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[dbo].[prcTmp_SDC_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[prcTmp_SDC_Insert]
GO
CREATE PROCEDURE prcTmp_SDC_Insert(@P1 AS char (1), @P2 AS nvarchar (50)) AS
INSERT INTO [dbo].[sysroUserGroups] ([ID], [Name]) VALUES (@P1, @P2)
GO
EXEC prcTmp_SDC_Insert '0', 'System'
GO
EXEC prcTmp_SDC_Insert '1', 'Admin'
GO
EXEC prcTmp_SDC_Insert '2', 'Users'
GO
DROP PROCEDURE prcTmp_SDC_Insert
GO

--
-- Actualiza sysroUserGroups
--

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[dbo].[prcTmp_SDC_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[prcTmp_SDC_Insert]
GO
CREATE PROCEDURE prcTmp_SDC_Insert(@P1 AS nvarchar (25), @P2 AS nvarchar (50), @P3 AS char (1)) AS
INSERT INTO [dbo].[sysroUsers] ([Login], [Password], [IDSecurityGroup]) VALUES (@P1, @P2, @P3)
GO
EXEC prcTmp_SDC_Insert 'Admin', '', '1'
GO
DROP PROCEDURE prcTmp_SDC_Insert
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='147' WHERE ID='DBVersion'
GO
