--
-- Actualiza referencias GUI
--
if not exists(SELECT * FROM sysroGUI WHERE IDPath='NavBar\FirstTab\AccessGroups') 
INSERT INTO sysroGUI (IDPath, LanguageReference, URL, IconURL, RequiredFeatures, SecurityFlags, Priority, AllowedSecurity) VALUES ('NavBar\FirstTab\AccessGroups','AccessGroups','roFormAccessGroups.vbd','AccessGroups.ico','Forms\Access','3111111111',200,'NRW')
--
-- Relaciones
--
ALTER TABLE [dbo].[AccessGroupsTimeZones] DROP CONSTRAINT [FK_AccessGroupsTimeZones_AccessGroups]
GO
ALTER TABLE [dbo].[AccessGroupZones] DROP CONSTRAINT [FK_AccessGroupZones_AccessGroups]
GO
ALTER TABLE [dbo].[AccessGroupZones] DROP CONSTRAINT [FK_AccessGroupZones_Zones]
GO
ALTER TABLE [dbo].[TerminalSirens] DROP CONSTRAINT [PK_TerminalSirens]
GO
ALTER TABLE [dbo].[TerminalSirens] DROP CONSTRAINT [FK_TerminalSirens_Terminals]
GO
ALTER TABLE [dbo].[Terminals] DROP CONSTRAINT [FK_Terminals_Zones]
GO
ALTER TABLE [dbo].[Terminals] DROP CONSTRAINT [FK_Terminals_Zones1]
GO
ALTER TABLE [dbo].[Employees] DROP CONSTRAINT [FK_Employees_AccessGroups]
GO
--
-- Tabla AccessGroups
--
ALTER TABLE [dbo].[AccessGroups] DROP CONSTRAINT [PK_AccessGroups]
GO
DROP TABLE [dbo].[AccessGroups]
GO
CREATE TABLE [dbo].[AccessGroups]
    ([ID] [smallint] NOT NULL ,
  	[Name] [nvarchar] (50) NOT NULL ,
  	[Description] [ntext] NULL )
GO
ALTER TABLE [dbo].[AccessGroups] WITH NOCHECK ADD CONSTRAINT [PK_AccessGroups] PRIMARY KEY NONCLUSTERED (ID) 
GO
ALTER TABLE [dbo].[Employees] ADD CONSTRAINT [FK_Employees_AccessGroups] FOREIGN KEY ([IDAccessGroup]) REFERENCES [dbo].[AccessGroups] ([ID])
GO
--
-- Tabla AccessGroupsHollyday
--
CREATE TABLE [dbo].[AccessGroupsHollyday]
    ([IDAccessGroup] [smallint] NOT NULL ,
  [DayOfYear] [datetime] NOT NULL ,
  [BeginTime] [datetime] NOT NULL ,
  [EndTime] [datetime] NOT NULL )
GO
ALTER TABLE [dbo].[AccessGroupsHollyday] ADD CONSTRAINT [FK_AccessGroupsHollyday_AccessGroups] FOREIGN KEY (IDAccessGroup)  REFERENCES [dbo].[AccessGroups] (ID) 
GO
ALTER TABLE [dbo].[AccessGroupsHollyday] WITH NOCHECK ADD CONSTRAINT [PK_AccessGroupsHollyday] PRIMARY KEY NONCLUSTERED (IDAccessGroup, DayOfYear) 
GO
--
-- Tabla AccessGroupsTimeZones
--
ALTER TABLE [dbo].[AccessGroupsTimeZones] DROP CONSTRAINT [PK_AccessGroupsTimeZones]
GO
DROP TABLE [dbo].[AccessGroupsTimeZones]
GO
CREATE TABLE [dbo].[AccessGroupsTimeZones]
    ([IDAccessGroup] [smallint] NOT NULL ,
  [DayOfWeek] [tinyint] NOT NULL ,
  [BeginTime] [datetime] NOT NULL ,
  [EndTime] [datetime] NOT NULL )
GO
ALTER TABLE [dbo].[AccessGroupsTimeZones] ADD CONSTRAINT [FK_AccessGroupsTimeZones_AccessGroups] FOREIGN KEY (IDAccessGroup)  REFERENCES [dbo].[AccessGroups] (ID) 
GO
ALTER TABLE [dbo].[AccessGroupsTimeZones] WITH NOCHECK ADD CONSTRAINT [PK_AccessGroupsTimeZones] PRIMARY KEY NONCLUSTERED (IDAccessGroup, DayOfWeek, BeginTime) 
GO
--
-- Tabla AccessGroupZones
--
ALTER TABLE [dbo].[AccessGroupZones] DROP CONSTRAINT [PK_AccessGroupZones]
GO
DROP TABLE [dbo].[AccessGroupZones]
GO
CREATE TABLE [dbo].[AccessGroupZones]
    ([IDAccessGroup] [smallint] NOT NULL ,
  [IDZone] [tinyint] NOT NULL )
GO
ALTER TABLE [dbo].[AccessGroupZones] ADD CONSTRAINT [FK_AccessGroupZones_AccessGroups] FOREIGN KEY (IDAccessGroup)  REFERENCES [dbo].[AccessGroups] (ID) 
GO
ALTER TABLE [dbo].[AccessGroupZones] WITH NOCHECK ADD CONSTRAINT [PK_AccessGroupZones] PRIMARY KEY NONCLUSTERED (IDAccessGroup, IDZone) 
GO
--
-- Tabla AccessMoves
--
CREATE TABLE [dbo].[AccessMoves]
    ([ID] [int] NOT NULL IDENTITY (1, 1) ,
  [IDEmployee] [int] NOT NULL ,
  [DateTime] [datetime] NULL ,
  [IDReader] [tinyint] NULL ,
  [Type] [char] (1) NULL ,
  [IDZone] [tinyint] NULL )
GO
ALTER TABLE [dbo].[AccessMoves] ADD CONSTRAINT [DF_AccessMoves_DateTime] DEFAULT (getdate()) FOR [DateTime]
GO
ALTER TABLE [dbo].[AccessMoves] ADD CONSTRAINT [FK_AccessMoves_Employees] FOREIGN KEY (IDEmployee)  REFERENCES [dbo].[Employees] (ID) 
GO
ALTER TABLE [dbo].[AccessMoves] WITH NOCHECK ADD CONSTRAINT [PK_AccessMoves] PRIMARY KEY NONCLUSTERED (ID) 
GO
--
-- Tabla Zones
--
ALTER TABLE [dbo].[Zones] DROP CONSTRAINT [Zones_PK]
GO
DROP TABLE [dbo].[Zones]
GO
CREATE TABLE [dbo].[Zones]
    ([ID] [tinyint] NOT NULL ,
  [Name] [nvarchar] (50) NULL ,
  [Description] [ntext] NULL ,
  [IsWorkingZone] [bit] NOT NULL ,
  [X1] [smallint] NULL ,
  [X2] [smallint] NULL ,
  [Y1] [smallint] NULL ,
  [Y2] [smallint] NULL ,
  [Proportion] [decimal] (18, 0) NULL ,
  [IDZoneGroup] [tinyint] NULL )
GO
ALTER TABLE [dbo].[Zones] ADD CONSTRAINT [DF_Zones_IDZone] DEFAULT (0) FOR [ID]
GO
ALTER TABLE [dbo].[Zones] ADD CONSTRAINT [DF_Zones_Name] DEFAULT ('') FOR [Name]
GO
ALTER TABLE [dbo].[Zones] ADD CONSTRAINT [DF_Zones_Description] DEFAULT ('') FOR [Description]
GO
ALTER TABLE [dbo].[Zones] ADD CONSTRAINT [DF_Zones_IsWorkingZone] DEFAULT (0) FOR [IsWorkingZone]
GO
ALTER TABLE [dbo].[Zones] ADD CONSTRAINT [DF_Zones_X1] DEFAULT ((-1)) FOR [X1]
GO
ALTER TABLE [dbo].[Zones] ADD CONSTRAINT [DF_Zones_X2] DEFAULT ((-1)) FOR [X2]
GO
ALTER TABLE [dbo].[Zones] ADD CONSTRAINT [DF_Zones_Y1] DEFAULT ((-1)) FOR [Y1]
GO
ALTER TABLE [dbo].[Zones] ADD CONSTRAINT [DF_Zones_Y2] DEFAULT ((-1)) FOR [Y2]
GO
ALTER TABLE [dbo].[Zones] ADD CONSTRAINT [DF_Zones_Proportion] DEFAULT (1) FOR [Proportion]
GO
ALTER TABLE [dbo].[Zones] WITH NOCHECK ADD CONSTRAINT [PK_Zonas] PRIMARY KEY NONCLUSTERED (ID) 
GO
--
-- Tabla ZoneGroups
--
CREATE TABLE [dbo].[ZoneGroups]
    ([ID] [tinyint] NOT NULL ,
  [Name] [nvarchar] (50) NULL ,
  [Description] [ntext] NULL ,
  [IDImage] [int] NULL )
GO
ALTER TABLE [dbo].[ZoneGroups] ADD CONSTRAINT [DF_ZoneGroups_Name] DEFAULT ('') FOR [Name]
GO
ALTER TABLE [dbo].[ZoneGroups] ADD CONSTRAINT [DF_ZoneGroups_Description] DEFAULT ('') FOR [Description]
GO
ALTER TABLE [dbo].[ZoneGroups] WITH NOCHECK ADD CONSTRAINT [PK_ZoneGroups] PRIMARY KEY NONCLUSTERED (ID) 
GO
--
-- Tabla Terminales
--
DROP TABLE [dbo].[Terminals]
GO
CREATE TABLE [dbo].[Terminals]
    ([ID] [tinyint] NOT NULL ,
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
  [SupportedModes] [nvarchar] (128) NULL ,
  [Mode] [nvarchar] (50) NULL )
GO
ALTER TABLE [dbo].[Terminals] ADD CONSTRAINT [FK_Terminals_Zones] FOREIGN KEY (Reader1IDZone)  REFERENCES [dbo].[Zones] (ID) 
GO
ALTER TABLE [dbo].[Terminals] ADD CONSTRAINT [FK_Terminals_Zones1] FOREIGN KEY (Reader2IDZone)  REFERENCES [dbo].[Zones] (ID) 
GO
ALTER TABLE [dbo].[Terminals] WITH NOCHECK ADD CONSTRAINT [PK_Terminales] PRIMARY KEY NONCLUSTERED (ID) 
GO
--
-- Tabla TerminalSirens
--
DROP TABLE [dbo].[TerminalSirens]
GO
CREATE TABLE [dbo].[TerminalSirens]
    ([IDTerminal] [tinyint] NOT NULL ,
  [ID] [tinyint] NOT NULL ,
  [Weekday] [tinyint] NOT NULL ,
  [Hour] [datetime] NOT NULL ,
  [Duration] [tinyint] NOT NULL )
GO
ALTER TABLE [dbo].[TerminalSirens] ADD CONSTRAINT [FK_TerminalSirens_Terminals] FOREIGN KEY (IDTerminal)  REFERENCES [dbo].[Terminals] (ID) 
GO
ALTER TABLE [dbo].[TerminalSirens] WITH NOCHECK ADD CONSTRAINT [PK_TerminalSirens] PRIMARY KEY NONCLUSTERED (IDTerminal, ID) 
GO

-- Relaciones Finales
ALTER TABLE [dbo].[AccessGroupZones] ADD CONSTRAINT [FK_AccessGroupZones_Zones] FOREIGN KEY (IDZone)  REFERENCES [dbo].[Zones] (ID) 
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='145' WHERE ID='DBVersion'
GO