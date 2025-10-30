--Actualiza referencias GUI
if exists(SELECT * FROM sysroGUI WHERE IDPath='NavBar\FirstTab\Multiview') 
UPDATE sysroGUI SET IDPath='NavBar\FirstTab\Shifts',LanguageReference='Shifts' WHERE IDPath='NavBar\FirstTab\Scheduler'
GO
UPDATE sysroGUI SET IDPath='NavBar\FirstTab\Scheduler',LanguageReference='Scheduler' WHERE IDPath='NavBar\FirstTab\Multiview'
GO

-- Actualiza URLs de GUI
UPDATE sysroGUI SET URL='http://www.visualtime.net' WHERE IDPath='Menu\Help\Support\Downloads'
GO

-- Actualiza iconos de GUI
UPDATE sysroGUI SET IconURL='calendar.ico' WHERE IDPath='NavBar\FirstTab\Scheduler'
GO
UPDATE sysroGUI SET IconURL='men.ico' WHERE IDPath='NavBar\FirstTab\Employees'
GO
UPDATE sysroGUI SET IconURL='reports.ico' WHERE IDPath='NavBar\FirstTab\Reports'
GO
UPDATE sysroGUI SET IconURL='shifts.ico' WHERE IDPath='NavBar\FirstTab\Shifts'
GO


-- Cambios en ExpectedShifts
ALTER TABLE [dbo].[EmployeeExpectedShifts] ADD [tmpValue] SMALLINT
GO
UPDATE [dbo].[EmployeeExpectedShifts] SET tmpValue=ExpectedShift
GO
UPDATE [dbo].[EmployeeExpectedShifts] SET tmpValue=ExpectedShifts
GO
ALTER TABLE [dbo].[EmployeeExpectedShifts] DROP COLUMN [ExpectedShift]
GO
ALTER TABLE [dbo].[EmployeeExpectedShifts] DROP COLUMN [ExpectedShifts]
GO
ALTER TABLE [dbo].[EmployeeExpectedShifts] ADD [ExpectedShifts] SMALLINT NOT NULL DEFAULT (0)
GO
UPDATE [dbo].[EmployeeExpectedShifts] SET ExpectedShifts=tmpvalue
GO
ALTER TABLE [dbo].[EmployeeExpectedShifts] DROP COLUMN [tmpValue]
GO


-- Cambios en ProgrammedAbsences
ALTER TABLE [dbo].[ProgrammedAbsences] ALTER COLUMN Description NTEXT
GO
UPDATE [dbo].[ProgrammedAbsences] SET MaxLastingDays=99 WHERE MaxLastingDays IS NULL
GO
ALTER TABLE [dbo].[ProgrammedAbsences] ALTER COLUMN MaxLastingDays SMALLINT NOT NULL
GO


-- Borra tablas obsoletas
ALTER TABLE [dbo].[DailyIncidences] DROP CONSTRAINT FK_DailyIncidences_sysroDailyIncidencesTypes
GO
ALTER TABLE [dbo].[sysroShiftsLayers] DROP CONSTRAINT FK_sysroShiftsLayers_sysroShiftLayerTypes
GO
DROP TABLE [dbo].[sysroShiftLayerTypes]
GO


-- Cambia sysroShiftsCausesRules
ALTER TABLE [dbo].[sysroShiftsCausesRules] DROP CONSTRAINT [PK_sysroShiftsCausesRules]
GO

ALTER TABLE [dbo].[sysroShiftsCausesRules] ALTER COLUMN ID TINYINT NOT NULL
GO

ALTER TABLE [dbo].[sysroShiftsCausesRules] WITH NOCHECK ADD 
	CONSTRAINT [PK_sysroShiftsCausesRules] PRIMARY KEY  NONCLUSTERED 
	(
		[IDShift],
		[ID]
	)  ON [PRIMARY] 
GO


-- Actualiza tablas de producción a la versión 136
ALTER TABLE [dbo].[JobIncidences] DROP CONSTRAINT FK_JobIncidences_JobIncidenceCategories
GO

ALTER TABLE [dbo].[DailyJobAccruals] DROP CONSTRAINT FK_DailyJobAccruals_JobIncidences
GO

ALTER TABLE [dbo].[DailyJobPieces] DROP CONSTRAINT FK_DailyJobPieces_JobIncidences
GO

ALTER TABLE [dbo].[EmployeeJobMoves] DROP CONSTRAINT FK_EmployeeJobMoves_JobIncidences
GO

ALTER TABLE [dbo].[TeamJobMoves] DROP CONSTRAINT FK_TeamJobMoves_JobIncidences
GO

ALTER TABLE [dbo].[DailyJobAccruals] DROP CONSTRAINT FK_DailyJobAccruals_Jobs
GO

ALTER TABLE [dbo].[DailyJobPieces] DROP CONSTRAINT FK_DailyJobPieces_Jobs
GO

ALTER TABLE [dbo].[EmployeeJobMoves] DROP CONSTRAINT FK_EmployeeJobMoves_Jobs
GO

ALTER TABLE [dbo].[TeamJobMoves] DROP CONSTRAINT FK_TeamJobMoves_Jobs
GO

ALTER TABLE [dbo].[JobTemplates] DROP CONSTRAINT FK_JobTemplates_MachineGroups
GO

ALTER TABLE [dbo].[Machines] DROP CONSTRAINT FK_Machines_MachineGroups
GO

ALTER TABLE [dbo].[DailyJobAccruals] DROP CONSTRAINT FK_DailyJobAccruals_Machines
GO

ALTER TABLE [dbo].[DailyJobPieces] DROP CONSTRAINT FK_DailyJobPieces_Machines
GO

ALTER TABLE [dbo].[EmployeeJobMoves] DROP CONSTRAINT FK_EmployeeJobMoves_Machines
GO

ALTER TABLE [dbo].[TeamJobMoves] DROP CONSTRAINT FK_TeamJobMoves_Machines
GO

ALTER TABLE [dbo].[OrderUserDefinedFields] DROP CONSTRAINT FK_OrderUserDefinedFields_Orders
GO

ALTER TABLE [dbo].[DailyJobPieces] DROP CONSTRAINT FK_DailyJobPieces_sysroPieceTypes
GO

ALTER TABLE [dbo].[EmployeeJobPieces] DROP CONSTRAINT FK_EmployeeJobPieces_sysroPieceTypes
GO

ALTER TABLE [dbo].[PiecesEntries] DROP CONSTRAINT FK_PiecesEntries_sysroPieceTypes
GO

ALTER TABLE [dbo].[TeamJobPieces] DROP CONSTRAINT FK_TeamJobPieces_sysroPieceTypes
GO

ALTER TABLE [dbo].[DailyJobAccruals] DROP CONSTRAINT FK_DailyJobAccruals_Teams
GO

ALTER TABLE [dbo].[DailyJobPieces] DROP CONSTRAINT FK_DailyJobPieces_Teams
GO

ALTER TABLE [dbo].[EmployeeTeams] DROP CONSTRAINT FK_EmployeeTeams_Teams
GO

ALTER TABLE [dbo].[JobTemplates] DROP CONSTRAINT FK_JobTemplates_Teams
GO

ALTER TABLE [dbo].[TeamJobMoves] DROP CONSTRAINT FK_TeamJobMoves_Teams
GO

ALTER TABLE [dbo].[TeamJobPieces] DROP CONSTRAINT FK_TeamJobPieces_Teams
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[DailyJobAccruals]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[DailyJobAccruals]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[DailyJobPieces]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[DailyJobPieces]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[EmployeeJobMoves]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[EmployeeJobMoves]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[EmployeeJobPieces]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[EmployeeJobPieces]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[EmployeeTeams]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[EmployeeTeams]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[JobEntries]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[JobEntries]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[JobIncidenceCategories]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[JobIncidenceCategories]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[JobIncidences]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[JobIncidences]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[Jobs]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Jobs]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[JobTemplates]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[JobTemplates]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[MachineGroups]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[MachineGroups]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[Machines]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Machines]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[Orders]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Orders]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[OrderTemplates]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[OrderTemplates]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[OrderUserDefinedFields]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[OrderUserDefinedFields]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[PiecesEntries]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[PiecesEntries]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[sysroPieceTypes]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[sysroPieceTypes]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[TeamJobMoves]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[TeamJobMoves]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[TeamJobPieces]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[TeamJobPieces]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[Teams]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Teams]
GO

CREATE TABLE [dbo].[DailyJobAccruals] (
	[IDEmployee] [int] NOT NULL ,
	[IDJob] [numeric](18, 0) NOT NULL ,
	[IDIncidence] [tinyint] NOT NULL ,
	[IDTeam] [tinyint] NOT NULL ,
	[IDMachine] [tinyint] NOT NULL ,
	[Date] [smalldatetime] NOT NULL ,
	[Value] [numeric](8, 6) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DailyJobPieces] (
	[IDEmployee] [int] NOT NULL ,
	[IDJob] [numeric](18, 0) NOT NULL ,
	[IDIncidence] [tinyint] NOT NULL ,
	[IDTeam] [tinyint] NOT NULL ,
	[Date] [smalldatetime] NOT NULL ,
	[IDMachine] [tinyint] NOT NULL ,
	[PiecesType] [tinyint] NOT NULL ,
	[Value] [numeric](9, 2) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[EmployeeJobMoves] (
	[IDEmployee] [int] NOT NULL ,
	[InDateTime] [smalldatetime] NULL ,
	[OutDateTime] [smalldatetime] NULL ,
	[InIDReader] [tinyint] NULL ,
	[OutIDReader] [tinyint] NULL ,
	[IDJob] [numeric](18, 0) NULL ,
	[InIDIncidence] [tinyint] NULL ,
	[OutIDIncidence] [tinyint] NULL ,
	[IDMachine] [tinyint] NULL ,
	[InMovesFlag] [bit] NOT NULL ,
	[OutMovesFlag] [bit] NOT NULL ,
	[Processed] [bit] NOT NULL ,
	[IDIncidence] [tinyint] NOT NULL ,
	[ID] [numeric](16, 0) IDENTITY (1, 1) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[EmployeeJobPieces] (
	[IDEmployee] [int] NOT NULL ,
	[DateTime] [smalldatetime] NOT NULL ,
	[PieceType] [tinyint] NOT NULL ,
	[IDReader] [tinyint] NOT NULL ,
	[Value] [numeric](9, 2) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[EmployeeTeams] (
	[IDEmployee] [int] NOT NULL ,
	[IDTeam] [tinyint] NOT NULL ,
	[BeginDate] [smalldatetime] NOT NULL ,
	[FinishDate] [smalldatetime] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[JobEntries] (
	[DateTime] [smalldatetime] NOT NULL ,
	[IDCard] [numeric](28, 0) NOT NULL ,
	[IDReader] [tinyint] NOT NULL ,
	[Type] [char] (1) NOT NULL ,
	[IDIncidence] [tinyint] NOT NULL ,
	[IDJob] [numeric](18, 0) NULL ,
	[IDMachine] [tinyint] NOT NULL ,
	[InvalidRead] [bit] NOT NULL ,
	[ID] [numeric](16, 0) IDENTITY (1, 1) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[JobIncidenceCategories] (
	[ID] [tinyint] NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[Description] [ntext] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[JobIncidences] (
	[ID] [tinyint] NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[Description] [ntext] NULL ,
	[IDCategory] [tinyint] NULL ,
	[ShortName] [nvarchar] (3) NULL ,
	[ReaderInputCode] [tinyint] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[Jobs] (
	[ID] [numeric](18, 0) IDENTITY (1, 1) NOT NULL ,
	[Path] [nvarchar] (50) NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[Description] [ntext] NULL ,
	[IDMachineGroup] [tinyint] NULL ,
	[IDMachine] [tinyint] NULL ,
	[IDTeam] [tinyint] NULL ,
	[UnitPieces] [smallint] NULL ,
	[PiecesAreShared] [bit] NULL ,
	[IsExclusive] [bit] NULL ,
	[AllowOnlyGrantedTeam] [bit] NULL ,
	[Position] [smallint] NULL ,
	[PunchID] [numeric](18, 0) NOT NULL ,
	[StartDate] [smalldatetime] NULL ,
	[EndDate] [smalldatetime] NULL ,
	[PreparationTime] [numeric](9, 6) NOT NULL ,
	[PieceTime] [numeric](9, 6) NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[JobTemplates] (
	[ID] [numeric](18, 0) IDENTITY (1, 1) NOT NULL ,
	[Path] [nvarchar] (50) NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[Description] [ntext] NULL ,
	[IDMachineGroup] [tinyint] NULL ,
	[IDTeam] [tinyint] NULL ,
	[UnitPieces] [smallint] NOT NULL ,
	[PiecesAreShared] [bit] NULL ,
	[IsExclusive] [bit] NULL ,
	[AllowOnlyGrantedTeam] [bit] NULL ,
	[Position] [smallint] NULL ,
	[PreparationTime] [numeric](9, 6) NOT NULL ,
	[PieceTime] [numeric](9, 6) NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[MachineGroups] (
	[ID] [tinyint] NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[Description] [ntext] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[Machines] (
	[ID] [tinyint] NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[HourCost] [numeric](18, 6) NULL ,
	[Description] [ntext] NULL ,
	[IDGroup] [tinyint] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[Orders] (
	[ID] [nvarchar] (30) NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[Description] [ntext] NULL ,
	[Position] [smallint] NULL ,
	[Amount] [int] NULL ,
	[BeginDate] [smalldatetime] NULL ,
	[EndDate] [smalldatetime] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[OrderTemplates] (
	[ID] [nvarchar] (30) NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[Description] [ntext] NULL ,
	[Position] [smallint] NULL ,
	[UnitPieces] [smallint] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[OrderUserDefinedFields] (
	[IDOrder] [nvarchar] (30) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[PiecesEntries] (
	[ID] [numeric](13, 0) NOT NULL ,
	[DateTime] [smalldatetime] NOT NULL ,
	[IDCard] [numeric](28, 0) NOT NULL ,
	[IDReader] [tinyint] NOT NULL ,
	[Type] [tinyint] NOT NULL ,
	[Value] [numeric](9, 2) NOT NULL ,
	[InvalidRead] [bit] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[sysroPieceTypes] (
	[ID] [tinyint] NOT NULL ,
	[Name] [nvarchar] (16) NOT NULL ,
	[IsConsideredValid] [bit] NOT NULL ,
	[CountOnPerformance] [bit] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TeamJobMoves] (
	[IDTeam] [tinyint] NOT NULL ,
	[InDateTime] [smalldatetime] NULL ,
	[OutDateTime] [smalldatetime] NULL ,
	[InIDReader] [tinyint] NULL ,
	[OutIDReader] [tinyint] NULL ,
	[IDJob] [numeric](18, 0) NULL ,
	[InIDIncidence] [tinyint] NULL ,
	[OutIDIncidence] [tinyint] NULL ,
	[IDMachine] [tinyint] NULL ,
	[InMovesFlag] [bit] NOT NULL ,
	[OutMovesFlag] [bit] NOT NULL ,
	[Processed] [bit] NOT NULL ,
	[IDIncidence] [tinyint] NOT NULL ,
	[ID] [numeric](16, 0) IDENTITY (1, 1) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TeamJobPieces] (
	[IDTeam] [tinyint] NOT NULL ,
	[DateTime] [smalldatetime] NOT NULL ,
	[PieceType] [tinyint] NOT NULL ,
	[IDReader] [tinyint] NOT NULL ,
	[Value] [numeric](9, 2) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Teams] (
	[ID] [tinyint] NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[Description] [ntext] NULL ,
	[SecurityFlags] [nvarchar] (50) NULL ,
	[Color] [int] NOT NULL ,
	[IDCard] [numeric](28, 0) NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[DailyJobAccruals] WITH NOCHECK ADD 
	CONSTRAINT [PK_DailyJobAccruals] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[IDJob],
		[IDIncidence],
		[IDTeam],
		[IDMachine],
		[Date]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[DailyJobPieces] WITH NOCHECK ADD 
	CONSTRAINT [DF_DailyJobPieces_PiecesType] DEFAULT (0) FOR [PiecesType],
	CONSTRAINT [PK_DailyJobPieces] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[IDJob],
		[IDIncidence],
		[IDTeam],
		[Date],
		[IDMachine],
		[PiecesType]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[EmployeeJobMoves] WITH NOCHECK ADD 
	CONSTRAINT [DF_EmployeeJobMoves_InFlag] DEFAULT (0) FOR [InMovesFlag],
	CONSTRAINT [DF_EmployeeJobMoves_OutMovesFlag] DEFAULT (0) FOR [OutMovesFlag],
	CONSTRAINT [DF_EmployeeJobMoves_Processed] DEFAULT (0) FOR [Processed],
	CONSTRAINT [DF_EmployeeJobMoves_IDIncidence] DEFAULT (0) FOR [IDIncidence],
	CONSTRAINT [PK_EmployeeJobMoves] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[EmployeeJobPieces] WITH NOCHECK ADD 
	CONSTRAINT [DF_EmployeeJobPieces_PieceType] DEFAULT (0) FOR [PieceType],
	CONSTRAINT [PK_EmployeeJobPieces] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[DateTime],
		[PieceType]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[EmployeeTeams] WITH NOCHECK ADD 
	CONSTRAINT [PK_EmployeeTeams] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[IDTeam],
		[BeginDate]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[JobEntries] WITH NOCHECK ADD 
	CONSTRAINT [DF_JobEntries_InvalidRead] DEFAULT (0) FOR [InvalidRead],
	CONSTRAINT [PK_JobEntries] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[JobIncidenceCategories] WITH NOCHECK ADD 
	CONSTRAINT [PK_JobIncidenceCategories] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[JobIncidences] WITH NOCHECK ADD 
	CONSTRAINT [PK_JobIncidences] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Jobs] WITH NOCHECK ADD 
	CONSTRAINT [DF__Jobs__Preparatio__2FDA0782] DEFAULT (0) FOR [PreparationTime],
	CONSTRAINT [DF__Jobs__PieceTime__30CE2BBB] DEFAULT (0) FOR [PieceTime],
	CONSTRAINT [PK_Jobs] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[JobTemplates] WITH NOCHECK ADD 
	CONSTRAINT [DF_JobTemplates_PiecesPerUnit] DEFAULT (0) FOR [UnitPieces],
	CONSTRAINT [DF__JobTempla__Prepa__31C24FF4] DEFAULT (0) FOR [PreparationTime],
	CONSTRAINT [DF__JobTempla__Piece__32B6742D] DEFAULT (0) FOR [PieceTime],
	CONSTRAINT [PK_JobTemplates] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[MachineGroups] WITH NOCHECK ADD 
	CONSTRAINT [PK_MachineGroups] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Machines] WITH NOCHECK ADD 
	CONSTRAINT [PK_Machines] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Orders] WITH NOCHECK ADD 
	CONSTRAINT [PK_Orders] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[OrderTemplates] WITH NOCHECK ADD 
	CONSTRAINT [DF_OrderTemplates_PiecesPerUnit] DEFAULT (0) FOR [UnitPieces],
	CONSTRAINT [PK_OrderTemplates] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[OrderUserDefinedFields] WITH NOCHECK ADD 
	CONSTRAINT [PK_OrderUserDefinedFields] PRIMARY KEY  NONCLUSTERED 
	(
		[IDOrder]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[PiecesEntries] WITH NOCHECK ADD 
	CONSTRAINT [DF_Pieces_Type_1] DEFAULT (0) FOR [Type],
	CONSTRAINT [DF_PiecesEntries_InvalidRead] DEFAULT (0) FOR [InvalidRead],
	CONSTRAINT [PK_Pieces] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[sysroPieceTypes] WITH NOCHECK ADD 
	CONSTRAINT [DF_sysroPieceTypes_ConsiderValid] DEFAULT (1) FOR [IsConsideredValid],
	CONSTRAINT [DF_sysroPieceTypes_CountOnPerformance] DEFAULT (1) FOR [CountOnPerformance],
	CONSTRAINT [PK_sysroPieceTypes] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[TeamJobMoves] WITH NOCHECK ADD 
	CONSTRAINT [DF_TeamJobMoves_FlagInMoves] DEFAULT (0) FOR [InMovesFlag],
	CONSTRAINT [DF_TeamJobMoves_FlagOutMoves] DEFAULT (0) FOR [OutMovesFlag],
	CONSTRAINT [DF_TeamJobMoves_Processed] DEFAULT (0) FOR [Processed],
	CONSTRAINT [PK_TeamJobMoves] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[TeamJobPieces] WITH NOCHECK ADD 
	CONSTRAINT [DF_TeamJobPieces_PieceType] DEFAULT (0) FOR [PieceType],
	CONSTRAINT [PK_TeamJobPieces] PRIMARY KEY  NONCLUSTERED 
	(
		[IDTeam],
		[DateTime],
		[PieceType]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Teams] WITH NOCHECK ADD 
	CONSTRAINT [DF_Teams_Color] DEFAULT (0) FOR [Color],
	CONSTRAINT [PK_Teams] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

 CREATE  INDEX [IX_DailyJobAccruals_EmployeeDate] ON [dbo].[DailyJobAccruals]([IDEmployee], [Date]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_DailyJobAccruals_TeamDate] ON [dbo].[DailyJobAccruals]([IDTeam], [Date]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_DailyJobAccruals_Job] ON [dbo].[DailyJobAccruals]([IDJob]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_DailyJobAccruals_JobIncidence] ON [dbo].[DailyJobAccruals]([IDJob], [IDIncidence]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_DailyJobPieces_EmployeeDate] ON [dbo].[DailyJobPieces]([IDEmployee], [Date]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_DailyJobPieces_TeamDate] ON [dbo].[DailyJobPieces]([IDTeam], [Date]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_DailyJobPieces_Job] ON [dbo].[DailyJobPieces]([IDJob]) ON [PRIMARY]
GO

 CREATE  UNIQUE  INDEX [IX_JobsPunchID] ON [dbo].[Jobs]([PunchID]) ON [PRIMARY]
GO

 CREATE  UNIQUE  INDEX [IX_JobsPath] ON [dbo].[Jobs]([Path]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DailyJobAccruals] ADD 
	CONSTRAINT [FK_DailyJobAccruals_Employees] FOREIGN KEY 
	(
		[IDEmployee]
	) REFERENCES [dbo].[Employees] (
		[ID]
	),
	CONSTRAINT [FK_DailyJobAccruals_JobIncidences] FOREIGN KEY 
	(
		[IDIncidence]
	) REFERENCES [dbo].[JobIncidences] (
		[ID]
	),
	CONSTRAINT [FK_DailyJobAccruals_Jobs] FOREIGN KEY 
	(
		[IDJob]
	) REFERENCES [dbo].[Jobs] (
		[ID]
	),
	CONSTRAINT [FK_DailyJobAccruals_Machines] FOREIGN KEY 
	(
		[IDMachine]
	) REFERENCES [dbo].[Machines] (
		[ID]
	),
	CONSTRAINT [FK_DailyJobAccruals_Teams] FOREIGN KEY 
	(
		[IDTeam]
	) REFERENCES [dbo].[Teams] (
		[ID]
	)
GO

ALTER TABLE [dbo].[DailyJobPieces] ADD 
	CONSTRAINT [FK_DailyJobPieces_Employees] FOREIGN KEY 
	(
		[IDEmployee]
	) REFERENCES [dbo].[Employees] (
		[ID]
	),
	CONSTRAINT [FK_DailyJobPieces_JobIncidences] FOREIGN KEY 
	(
		[IDIncidence]
	) REFERENCES [dbo].[JobIncidences] (
		[ID]
	),
	CONSTRAINT [FK_DailyJobPieces_Jobs] FOREIGN KEY 
	(
		[IDJob]
	) REFERENCES [dbo].[Jobs] (
		[ID]
	),
	CONSTRAINT [FK_DailyJobPieces_Machines] FOREIGN KEY 
	(
		[IDMachine]
	) REFERENCES [dbo].[Machines] (
		[ID]
	),
	CONSTRAINT [FK_DailyJobPieces_sysroPieceTypes] FOREIGN KEY 
	(
		[PiecesType]
	) REFERENCES [dbo].[sysroPieceTypes] (
		[ID]
	),
	CONSTRAINT [FK_DailyJobPieces_Teams] FOREIGN KEY 
	(
		[IDTeam]
	) REFERENCES [dbo].[Teams] (
		[ID]
	)
GO

ALTER TABLE [dbo].[EmployeeJobMoves] ADD 
	CONSTRAINT [FK_EmployeeJobMoves_Employees] FOREIGN KEY 
	(
		[IDEmployee]
	) REFERENCES [dbo].[Employees] (
		[ID]
	),
	CONSTRAINT [FK_EmployeeJobMoves_JobIncidences] FOREIGN KEY 
	(
		[IDIncidence]
	) REFERENCES [dbo].[JobIncidences] (
		[ID]
	),
	CONSTRAINT [FK_EmployeeJobMoves_Jobs] FOREIGN KEY 
	(
		[IDJob]
	) REFERENCES [dbo].[Jobs] (
		[ID]
	),
	CONSTRAINT [FK_EmployeeJobMoves_Machines] FOREIGN KEY 
	(
		[IDMachine]
	) REFERENCES [dbo].[Machines] (
		[ID]
	)
GO

ALTER TABLE [dbo].[EmployeeJobPieces] ADD 
	CONSTRAINT [FK_EmployeeJobPieces_Employees] FOREIGN KEY 
	(
		[IDEmployee]
	) REFERENCES [dbo].[Employees] (
		[ID]
	),
	CONSTRAINT [FK_EmployeeJobPieces_sysroPieceTypes] FOREIGN KEY 
	(
		[PieceType]
	) REFERENCES [dbo].[sysroPieceTypes] (
		[ID]
	)
GO

ALTER TABLE [dbo].[EmployeeTeams] ADD 
	CONSTRAINT [FK_EmployeeTeams_Employees] FOREIGN KEY 
	(
		[IDEmployee]
	) REFERENCES [dbo].[Employees] (
		[ID]
	),
	CONSTRAINT [FK_EmployeeTeams_Teams] FOREIGN KEY 
	(
		[IDTeam]
	) REFERENCES [dbo].[Teams] (
		[ID]
	)
GO

ALTER TABLE [dbo].[JobEntries] ADD 
	CONSTRAINT [FK_JobEntries_sysroEntryTypes] FOREIGN KEY 
	(
		[Type]
	) REFERENCES [dbo].[sysroEntryTypes] (
		[Type]
	)
GO

ALTER TABLE [dbo].[JobIncidences] ADD 
	CONSTRAINT [FK_JobIncidences_JobIncidenceCategories] FOREIGN KEY 
	(
		[IDCategory]
	) REFERENCES [dbo].[JobIncidenceCategories] (
		[ID]
	)
GO

ALTER TABLE [dbo].[JobTemplates] ADD 
	CONSTRAINT [FK_JobTemplates_MachineGroups] FOREIGN KEY 
	(
		[IDMachineGroup]
	) REFERENCES [dbo].[MachineGroups] (
		[ID]
	),
	CONSTRAINT [FK_JobTemplates_Teams] FOREIGN KEY 
	(
		[IDTeam]
	) REFERENCES [dbo].[Teams] (
		[ID]
	)
GO

ALTER TABLE [dbo].[Machines] ADD 
	CONSTRAINT [FK_Machines_MachineGroups] FOREIGN KEY 
	(
		[IDGroup]
	) REFERENCES [dbo].[MachineGroups] (
		[ID]
	)
GO

ALTER TABLE [dbo].[OrderUserDefinedFields] ADD 
	CONSTRAINT [FK_OrderUserDefinedFields_Orders] FOREIGN KEY 
	(
		[IDOrder]
	) REFERENCES [dbo].[Orders] (
		[ID]
	)
GO

ALTER TABLE [dbo].[PiecesEntries] ADD 
	CONSTRAINT [FK_PiecesEntries_sysroPieceTypes] FOREIGN KEY 
	(
		[Type]
	) REFERENCES [dbo].[sysroPieceTypes] (
		[ID]
	)
GO

ALTER TABLE [dbo].[TeamJobMoves] ADD 
	CONSTRAINT [FK_TeamJobMoves_JobIncidences] FOREIGN KEY 
	(
		[IDIncidence]
	) REFERENCES [dbo].[JobIncidences] (
		[ID]
	),
	CONSTRAINT [FK_TeamJobMoves_Jobs] FOREIGN KEY 
	(
		[IDJob]
	) REFERENCES [dbo].[Jobs] (
		[ID]
	),
	CONSTRAINT [FK_TeamJobMoves_Machines] FOREIGN KEY 
	(
		[IDMachine]
	) REFERENCES [dbo].[Machines] (
		[ID]
	),
	CONSTRAINT [FK_TeamJobMoves_Teams] FOREIGN KEY 
	(
		[IDTeam]
	) REFERENCES [dbo].[Teams] (
		[ID]
	)
GO

ALTER TABLE [dbo].[TeamJobPieces] ADD 
	CONSTRAINT [FK_TeamJobPieces_sysroPieceTypes] FOREIGN KEY 
	(
		[PieceType]
	) REFERENCES [dbo].[sysroPieceTypes] (
		[ID]
	),
	CONSTRAINT [FK_TeamJobPieces_Teams] FOREIGN KEY 
	(
		[IDTeam]
	) REFERENCES [dbo].[Teams] (
		[ID]
	)
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='136' WHERE ID='DBVersion'
GO
