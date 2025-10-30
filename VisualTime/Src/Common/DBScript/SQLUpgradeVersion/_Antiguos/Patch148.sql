-- Crea tabla Temporal para listados con Leyenda
DROP TABLE [dbo].[TMPCONCEPT]
GO
CREATE TABLE [dbo].[TMPCONCEPT] (
	[ConceptName] [nvarchar] (64) NULL ,
	[ShortName] [nvarchar] (3) NULL
) ON [PRIMARY]
GO
--
-- Actualiza sysroGUI para que no aparezca la pestaña de producción si no esta instalado
--
UPDATE [dbo].[sysroGUI] SET [RequiredFeatures]='Forms\Teams' WHERE [IDPath]='NavBar\Job'
GO
--
-- Crea tabla de correspondencia de tarjetas
-- Tabla CardAliases 
--
CREATE TABLE [dbo].[CardAliases] (
	[IDCard] [numeric](28, 0) NOT NULL ,
	[RealValue] [nvarchar] (50) NOT NULL 
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CardAliases] WITH NOCHECK ADD 
	CONSTRAINT [PK_CardAliases] PRIMARY KEY  NONCLUSTERED 
	(
		[RealValue]
	)  ON [PRIMARY] ,
	CONSTRAINT [IX_CardAliases_IDCard] UNIQUE  NONCLUSTERED 
	(
		[IDCard]
	)  ON [PRIMARY] 
GO

--
-- Tabla Entries
--
ALTER TABLE [dbo].[Entries] ADD [Rdr] [tinyint] NULL 
GO
ALTER TABLE [dbo].[Entries] ADD CONSTRAINT [DF_Entries_Rdr] DEFAULT (0) FOR [Rdr]
GO
--
-- Tabla Moves
--
ALTER TABLE [dbo].[Moves] ADD [InIDZone] [tinyint] NULL ,[OutIDZone] [tinyint] NULL 
GO
--
-- Tabla Terminals
--
-- Remove foreign key constraints
ALTER TABLE [dbo].[Terminals] DROP CONSTRAINT [FK_Terminals_Zones]
GO
ALTER TABLE [dbo].[Terminals] DROP CONSTRAINT [FK_Terminals_Zones1]
GO
ALTER TABLE [dbo].[TerminalSirens] DROP CONSTRAINT [FK_TerminalSirens_Terminals]
GO
ALTER TABLE [dbo].[Terminals]
DROP
  CONSTRAINT [DF_Terminals_Reader1Mode],
  CONSTRAINT [DF_Terminals_Reader2Mode],
  CONSTRAINT [DF_Terminals_Reader1Output],
  CONSTRAINT [DF_Terminals_Reader2Output],
  CONSTRAINT [DF_Terminals_SirensOutput]
GO
DROP TABLE [dbo].[Terminals]
GO
CREATE TABLE [dbo].[Terminals]
    (
  [ID] [tinyint] NOT NULL ,
  [Description] [nvarchar] (50) NULL ,
  [Type] [nvarchar] (50) NULL ,
  [Behavior] [nvarchar] (50) NULL ,
  [Location] [nvarchar] (50) NULL ,
  [LastUpdate] [smalldatetime] NULL ,
  [LastStatus] [nvarchar] (50) NULL ,
  [SupportedModes] [nvarchar] (40) NULL ,
  [SupportedOutputs] [nvarchar] (40) NULL ,
  [SupportedSirens] [nvarchar] (20) NULL ,
  [SirensOutput] [tinyint] NULL 
)
GO
ALTER TABLE [dbo].[Terminals] WITH NOCHECK ADD CONSTRAINT [DF_Terminals_SirensOutput] DEFAULT (0) FOR SirensOutput
GO
ALTER TABLE [dbo].[Terminals] WITH NOCHECK ADD CONSTRAINT [PK_Terminales] PRIMARY KEY NONCLUSTERED (ID) 
GO
-- Add foreign key constraints
ALTER TABLE [dbo].[TerminalSirens] ADD CONSTRAINT [FK_TerminalSirens_Terminals] FOREIGN KEY (IDTerminal)  REFERENCES [dbo].[Terminals] (ID) 
GO
--
-- Tabla Terminal Readers
--
CREATE TABLE [dbo].[TerminalReaders]
    (
  [IDTerminal] [tinyint] NOT NULL ,
  [ID] [tinyint] NOT NULL ,
  [Description] [nvarchar] (50) NULL ,
  [IDZone] [tinyint] NULL ,
  [OutIDZone] [tinyint] NULL ,
  [PictureX] [numeric] (8, 3) NULL ,
  [PictureY] [numeric] (8, 3) NULL ,
  [Mode] [nvarchar] (20) NULL ,
  [Output] [tinyint] NULL 
)
GO
ALTER TABLE [dbo].[TerminalReaders] WITH NOCHECK ADD  CONSTRAINT [PK_TerminalReaders] PRIMARY KEY NONCLUSTERED (IDTerminal, ID) 
GO
-- Add foreign key constraints
ALTER TABLE [dbo].[TerminalReaders] ADD CONSTRAINT [FK_TerminalReaders_Terminals] FOREIGN KEY (IDTerminal)  REFERENCES [dbo].[Terminals] (ID) 
GO
ALTER TABLE [dbo].[TerminalReaders] ADD CONSTRAINT [FK_TerminalReaders_Zones] FOREIGN KEY (IDZone)  REFERENCES [dbo].[Zones] (ID) 
GO
--
-- Tabla Access Entries
--
CREATE TABLE [dbo].[AccessEntries]
    (
  [DateTime] [smalldatetime] NOT NULL ,
  [IDCard] [numeric] (28, 0) NOT NULL ,
  [IDReader] [tinyint] NOT NULL ,
  [Rdr] [tinyint] NOT NULL ,
  [Type] [char] (1) NOT NULL ,
  [InvalidRead] [bit] NOT NULL ,
  [ID] [numeric] (16, 0) NOT NULL IDENTITY (1, 1) 
)
GO
ALTER TABLE [dbo].[AccessEntries] ADD CONSTRAINT [DF_AccessEntries_InvalidRead] DEFAULT (0) FOR [InvalidRead]
GO
ALTER TABLE [dbo].[AccessEntries] WITH NOCHECK ADD CONSTRAINT [PK_AccessEntries] PRIMARY KEY NONCLUSTERED (ID) 
GO
--
-- Obsoletas
--
-- Tabla DailyJobPieces
--
ALTER TABLE [dbo].[DailyJobPieces] DROP CONSTRAINT [DF_DailyJobPieces_PiecesType] 
GO
ALTER TABLE [dbo].[DailyJobPieces] DROP CONSTRAINT [FK_DailyJobPieces_Employees] 
GO
ALTER TABLE [dbo].[DailyJobPieces] DROP CONSTRAINT [FK_DailyJobPieces_JobIncidences] 
GO
ALTER TABLE [dbo].[DailyJobPieces] DROP CONSTRAINT [FK_DailyJobPieces_Jobs] 
GO
ALTER TABLE [dbo].[DailyJobPieces] DROP CONSTRAINT [FK_DailyJobPieces_Machines] 
GO
ALTER TABLE [dbo].[DailyJobPieces] DROP CONSTRAINT [FK_DailyJobPieces_sysroPieceTypes]
GO
ALTER TABLE [dbo].[DailyJobPieces] DROP CONSTRAINT [FK_DailyJobPieces_Teams] 
GO
ALTER TABLE [dbo].[DailyJobPieces] DROP CONSTRAINT [PK_DailyJobPieces] 
GO
DROP INDEX [dbo].[DailyJobPieces].[IX_DailyJobPieces_EmployeeDate]
GO
DROP INDEX [dbo].[DailyJobPieces].[IX_DailyJobPieces_TeamDate]
GO
DROP INDEX [dbo].[DailyJobPieces].[IX_DailyJobPieces_Job]
GO
DROP TABLE [dbo].[DailyJobPieces]
GO    
--
-- Tabla EmployeeJobPieces
--
ALTER TABLE [dbo].[EmployeeJobPieces] DROP CONSTRAINT [DF_EmployeeJobPieces_PieceType]
GO
ALTER TABLE [dbo].[EmployeeJobPieces] DROP CONSTRAINT [FK_EmployeeJobPieces_Employees]
GO
ALTER TABLE [dbo].[EmployeeJobPieces] DROP CONSTRAINT [FK_EmployeeJobPieces_sysroPieceTypes] 
GO
ALTER TABLE [dbo].[EmployeeJobPieces] DROP CONSTRAINT [PK_EmployeeJobPieces] 
GO
DROP TABLE [dbo].[EmployeeJobPieces]
GO
--
-- Tabla EmployeeJobPieces
--
ALTER TABLE [dbo].[modShiftMode] DROP CONSTRAINT [PK__modShiftMode__2F2FFC0C] 
GO
DROP TABLE [dbo].[modShiftMode]
GO
--
-- Tabla EmployeeJobPieces
--
ALTER TABLE [dbo].[TeamJobPieces] DROP CONSTRAINT [DF_TeamJobPieces_PieceType] 
GO
ALTER TABLE [dbo].[TeamJobPieces] DROP CONSTRAINT [FK_TeamJobPieces_sysroPieceTypes] 
GO
ALTER TABLE [dbo].[TeamJobPieces] DROP CONSTRAINT [FK_TeamJobPieces_Teams] 
GO
ALTER TABLE [dbo].[TeamJobPieces] DROP CONSTRAINT [PK_TeamJobPieces] 
GO
DROP TABLE [dbo].[TeamJobPieces]
GO
-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='148' WHERE ID='DBVersion'
GO
