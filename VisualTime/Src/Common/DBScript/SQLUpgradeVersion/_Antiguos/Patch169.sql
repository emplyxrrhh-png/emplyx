--
-- Agregamos un campo nuevo a la tabla de mensajes
--

ALTER TABLE [dbo].[EmployeeTerminalMessages] ADD [ForAllEmployees] [bit] NOT NULL DEFAULT (0)
GO

--
-- Actualizamos tablas para el control de accesos
--

CREATE TABLE [dbo].[AccessPeriods] (
	[ID] [int] NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AccessGroupsPermissions] (
	[IDAccessGroup] [int] NOT NULL ,
	[IDZone] [int] NOT NULL ,
	[IDAccessPeriod] [int] NOT NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AccessPeriodHolidays] (
	[IDAccessPeriod] [int] NOT NULL ,
	[Day] [int] NOT NULL ,
	[Month] [int] NOT NULL ,
	[BeginTime] [DateTime] NOT NULL ,
	[EndTime] [DateTime] NOT NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AccessPeriodDaily] (
	[IDAccessPeriod] [int] NOT NULL ,
	[DayofWeek] [int] NOT NULL ,
	[BeginTime] [DateTime] NOT NULL ,
	[EndTime] [DateTime] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AccessPeriods] WITH NOCHECK ADD 
	CONSTRAINT [PK_AccessPeriods] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[AccessGroupsPermissions] WITH NOCHECK ADD 
	CONSTRAINT [PK_AccessGroupsPermissions] PRIMARY KEY  NONCLUSTERED 
	(
		[IDAccessGroup],
		[IDZone],
		[IDAccessPeriod]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[AccessPeriodDaily] WITH NOCHECK ADD 
	CONSTRAINT [PK_AccessPeriodDaily] PRIMARY KEY  NONCLUSTERED 
	(
		[IDAccessPeriod],
		[DayOfWeek],
		[BeginTime]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[AccessPeriodHolidays] WITH NOCHECK ADD 
	CONSTRAINT [PK_AccessPeriodHolidays] PRIMARY KEY  NONCLUSTERED 
	(
		[IDAccessPeriod],
		[Day],
		[Month],
		[BeginTime]
	)  ON [PRIMARY] 
GO

-- Añade tab de accesos
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity,RequiredFeatures) 
	VALUES ('NavBar\Access','Access','','','3111111111',1010,'NR','Forms\Access')
GO

-- Añade pantalla de grupos en el tab de accesos
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity,RequiredFeatures) 
	VALUES ('NavBar\Access\AccessGroupsPeriods','AccessGroupsPeriods','roFormAcGroupPds.vbd','AccessGroups.ico','3111111111',200,'NRW','Forms\Access')
GO

-- Añade pantalla de períodos en el tab de accesos
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity,RequiredFeatures) 
	VALUES ('NavBar\Access\AccessPeriods','AccessPeriods','roFormAccessPeriods.vbd','Clock.ico','3111111111',200,'NRW','Forms\Access')
GO

-- Añade pantalla de zonas en el tab de accesos
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity,RequiredFeatures) 
	VALUES ('NavBar\Access\AccessZones','AccessZones','roFormAccessZones.vbd','Options2.ico','3111111111',200,'NRW','Forms\Access')
GO

-- Añade pantalla de estado en el tab de accesos
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity,RequiredFeatures) 
	VALUES ('NavBar\Access\AccessStatus','AccessStatus','roFormAccessStatus.vbd','Teacher.ico','3111111111',200,'NRW','Forms\Access')
GO

-- Borra el registro de accesos que había anteriormente.
DELETE FROM sysroGUI where IDPath = 'NavBar\FirstTab\AccessGroups'

--
-- Actualiza Concepts 
--
ALTER TABLE [dbo].[Concepts] ADD [ViewInTerms232] [bit] NULL 
GO
ALTER TABLE [dbo].[Concepts] ADD CONSTRAINT [DF_Concepts_ViewInTerms232] DEFAULT (0) FOR [ViewInTerms232]
GO

--
-- Actualiza la tabla TerminalsReaders
--
ALTER TABLE [dbo].[TerminalReaders] ADD [Duration] [tinyint] NULL 
GO

ALTER TABLE [dbo].[TerminalReaders] ADD CONSTRAINT [DF_TerminalReaders_Duration] DEFAULT (0) FOR [Duration]
GO

ALTER TABLE [dbo].[TerminalReaders] ADD [RequestPin] [tinyint] NULL 
GO

ALTER TABLE [dbo].[TerminalReaders] ADD CONSTRAINT [DF_TerminalReaders_RequestPin] DEFAULT (0) FOR [RequestPin]
GO

ALTER TABLE [dbo].[TerminalReaders] ADD [AccessKey] [tinyint] NULL
GO

ALTER TABLE [dbo].[TerminalReaders] ADD CONSTRAINT [DF_TerminalReaders_AccessKey] DEFAULT (0) FOR [AccessKey]
GO

ALTER TABLE [dbo].[TerminalReaders] DROP COLUMN [OutIDZone]
GO


--
-- Actualiza la tabla Terminals
--

ALTER TABLE [dbo].[Terminals] DROP COLUMN [DefaultMode]
GO

--
-- Actualiza la tabla Employees
--

ALTER TABLE [dbo].[Employees] ADD [Pin] [nvarchar] (4) NULL
GO

ALTER TABLE [dbo].[Employees] ADD CONSTRAINT [DF_Employees_Pin] DEFAULT ('') FOR [Pin]
GO

--
-- Actualiza tablas de movimientos de produccion para poder enlazar con ERPs
--
ALTER TABLE [dbo].[EmployeeJobMoves] ADD [IsExported] [bit] NULL 
GO

ALTER TABLE [dbo].[EmployeeJobMoves] ADD CONSTRAINT [DF_EmployeeJobMoves_ISExported] DEFAULT (0) FOR [ISExported]
GO

ALTER TABLE [dbo].[TeamJobMoves] ADD [IsExported] [bit] NULL 
GO

ALTER TABLE [dbo].[TeamJobMoves] ADD CONSTRAINT [DF_TeamJobMoves_ISExported] DEFAULT (0) FOR [ISExported]
GO


--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='169' WHERE ID='DBVersion'
GO
