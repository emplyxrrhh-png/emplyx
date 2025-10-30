-- Elimina indices obsoletos
ALTER TABLE [dbo].[SpaceZones] DROP CONSTRAINT Zonas_FK00
GO

ALTER TABLE [dbo].[Doors] DROP CONSTRAINT Accesos_FK00
GO

ALTER TABLE [dbo].[Doors] DROP CONSTRAINT Accesos_FK01
GO


-- Elimina tablas obsoletas
if exists (select * from sysobjects where id = object_id(N'[dbo].[SpaceZones]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[SpaceZones]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[Terminals]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Terminals]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[SpaceZonesGroups]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[SpaceZonesGroups]
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[sysroFeatures]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[sysroFeatures]
GO


-- Modifica constraints de DailyIncidences
ALTER TABLE [dbo].[DailyIncidences] DROP CONSTRAINT [DF_DailyIncidences_IDJob]
GO

-- Modifica indice de DailyCauses
ALTER TABLE [dbo].[DailyCauses] DROP CONSTRAINT [DF_DailyCauses_IDJob]
GO
ALTER TABLE [dbo].[DailyCauses] DROP CONSTRAINT [PK_DailyCauses]
GO
ALTER TABLE [dbo].[DailyCauses] WITH NOCHECK ADD 
	CONSTRAINT [PK_DailyCauses] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[Date],
		[IDRelatedIncidence],
		[IDCause]
	)  ON [PRIMARY] 
GO

-- Modifica constraints de DailyAccruals
ALTER TABLE [dbo].[DailyAccruals] DROP CONSTRAINT [DF_DailyAccruals_IDJob]
GO

-- Elimina campos obsoletos
ALTER TABLE [dbo].[DailyIncidences] DROP COLUMN [IDJob]
GO

ALTER TABLE [dbo].[DailyIncidences] DROP COLUMN [PiecesOk]
GO

ALTER TABLE [dbo].[DailyIncidences] DROP COLUMN [PiecesBad]
GO

ALTER TABLE [dbo].[DailyCauses] DROP COLUMN  [IDJob]
GO

ALTER TABLE [dbo].[DailyAccruals] DROP COLUMN [IDJob]
GO


-- Actualiza campos numericos
ALTER TABLE [dbo].[EmployeeContracts] ALTER COLUMN [IDCard] [numeric](28, 0) NULL
GO

ALTER TABLE [dbo].[DailyAccruals] ALTER COLUMN  [Value] [numeric](9, 6) NOT NULL
GO

ALTER TABLE [dbo].[DailyAccruals] ALTER COLUMN  [Value] [numeric](9, 6) NOT NULL
GO

DROP INDEX [dbo].[Entries].[IDTarjeta]
GO
ALTER TABLE [dbo].[Entries] ALTER COLUMN  [IDCard] [numeric](28, 0) NOT NULL
GO
CREATE  INDEX [IX_IDCard] ON [dbo].[Entries]([IDCard]) ON [PRIMARY]
GO



-- Añade nuevos campos
ALTER TABLE [dbo].[EmployeeContracts] ADD [IDAccessGroup] [smallint] NULL
GO

ALTER TABLE [dbo].[Entries] ADD [InvalidRead] [bit] NOT NULL DEFAULT (0)
GO


-- Crea tablas nuevas
CREATE TABLE [dbo].[AccessGroups] (
	[ID] [smallint] NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[Description] [ntext] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[AccessGroupsTimeZones] (
	[IDAccessGroup] [smallint] NOT NULL ,
	[DayOfWeek] [tinyint] NOT NULL ,
	[BeginTime] [smalldatetime] NOT NULL ,
	[EndTime] [smalldatetime] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AccessGroupZones] (
	[IDAccessGroup] [smallint] NOT NULL ,
	[IDZone] [tinyint] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DailyJobAccruals] (
	[IDEmployee] [int] NOT NULL ,
	[IDJob] [nvarchar] (25) NOT NULL ,
	[IDIncidence] [int] NOT NULL ,
	[IDTeam] [int] NOT NULL ,
	[Date] [datetime] NOT NULL ,
	[Pieces] [smallint] NOT NULL ,
	[PiecesType] [tinyint] NOT NULL ,
	[Value] [numeric](8, 6) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[EmployeeJobMoves] (
	[ID] [nvarchar] (13) NOT NULL ,
	[IDEmployee] [int] NOT NULL ,
	[InDateTime] [datetime] NULL ,
	[OutDateTime] [datetime] NULL ,
	[InIDReader] [tinyint] NULL ,
	[OutIDReader] [tinyint] NULL ,
	[IDJob] [nvarchar] (30) NULL ,
	[InIDIncidence] [int] NULL ,
	[OutIDIncidence] [int] NULL ,
	[IDMachine] [smallint] NULL ,
	[InMovesFlag] [bit] NOT NULL ,
	[OutMovesFlag] [bit] NOT NULL ,
	[Processed] [bit] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[EmployeeJobPieces] (
	[IDEmployee] [int] NOT NULL ,
	[DateTime] [datetime] NOT NULL ,
	[PieceType] [tinyint] NOT NULL ,
	[IDReader] [smallint] NOT NULL ,
	[Value] [numeric](9, 2) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[EmployeeTeams] (
	[IDEmployee] [int] NOT NULL ,
	[IDTeam] [int] NOT NULL ,
	[BeginDate] [datetime] NOT NULL ,
	[FinishDate] [datetime] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[JobEntries] (
	[ID] [numeric](13, 0) NOT NULL ,
	[DateTime] [datetime] NOT NULL ,
	[IDCard] [numeric](18, 0) NOT NULL ,
	[IDReader] [nvarchar] (255) NOT NULL ,
	[Type] [char] (1) NOT NULL ,
	[IDIncidence] [int] NOT NULL ,
	[IDJob] [nvarchar] (30) NULL ,
	[IDMachine] [smallint] NOT NULL ,
	[InvalidRead] [bit] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[JobIncidenceCategories] (
	[ID] [int] NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[Description] [ntext] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[JobIncidences] (
	[ID] [int] NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[Description] [ntext] NULL ,
	[ImputeToJob] [bit] NULL ,
	[IDCategory] [int] NULL ,
	[ShortName] [nvarchar] (3) NULL ,
	[ReaderInputCode] [smallint] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[Jobs] (
	[ID] [nvarchar] (30) NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[Description] [ntext] NULL ,
	[IDMachineGroup] [smallint] NULL ,
	[IDMachine] [smallint] NULL ,
	[IDTeam] [int] NULL ,
	[PreparationTime] [datetime] NULL ,
	[UnitPieces] [smallint] NULL ,
	[PieceTime] [datetime] NULL ,
	[PiecesAreShared] [bit] NULL ,
	[IsExclusive] [bit] NULL ,
	[AllowOnlyGrantedTeam] [bit] NULL ,
	[Position] [smallint] NULL ,
	[PunchID] [numeric](18, 0) NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[JobTemplates] (
	[ID] [nvarchar] (30) NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[Description] [ntext] NULL ,
	[IDMachineGroup] [smallint] NULL ,
	[IDTeam] [int] NULL ,
	[PreparationTime] [datetime] NULL ,
	[UnitPieces] [smallint] NULL ,
	[PieceTime] [datetime] NULL ,
	[PiecesAreShared] [bit] NULL ,
	[IsExclusive] [bit] NULL ,
	[AllowOnlyGrantedTeam] [bit] NULL ,
	[Position] [smallint] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[MachineGroups] (
	[ID] [smallint] NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[Description] [ntext] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[Machines] (
	[ID] [smallint] NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[HourCost] [numeric](18, 6) NULL ,
	[Description] [ntext] NULL ,
	[IDGroup] [smallint] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[Orders] (
	[ID] [nvarchar] (30) NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[Description] [ntext] NULL ,
	[Position] [smallint] NULL ,
	[Amount] [int] NULL ,
	[BeginDate] [datetime] NULL ,
	[EndDate] [datetime] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[OrderTemplates] (
	[ID] [nvarchar] (30) NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL ,
	[Description] [ntext] NULL ,
	[Position] [smallint] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[OrderUserDefinedFields] (
	[IDOrder] [nvarchar] (30) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[PiecesEntries] (
	[ID] [numeric](13, 0) NOT NULL ,
	[DateTime] [datetime] NOT NULL ,
	[IDCard] [numeric](18, 0) NOT NULL ,
	[IDReader] [smallint] NOT NULL ,
	[Type] [smallint] NOT NULL ,
	[Value] [numeric](9, 2) NOT NULL ,
	[InvalidRead] [bit] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TeamJobMoves] (
	[ID] [nvarchar] (13) NOT NULL ,
	[IDTeam] [int] NOT NULL ,
	[InDateTime] [datetime] NULL ,
	[OutDateTime] [datetime] NULL ,
	[InIDReader] [tinyint] NULL ,
	[OutIDReader] [tinyint] NULL ,
	[IDJob] [nvarchar] (30) NULL ,
	[InIDIncidence] [int] NULL ,
	[OutIDIncidence] [int] NULL ,
	[IDMachine] [smallint] NULL ,
	[InMovesFlag] [bit] NOT NULL ,
	[OutMovesFlag] [bit] NOT NULL ,
	[Processed] [bit] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TeamJobPieces] (
	[IDTeam] [int] NOT NULL ,
	[DateTime] [datetime] NOT NULL ,
	[PieceType] [tinyint] NOT NULL ,
	[IDReader] [smallint] NOT NULL ,
	[Value] [numeric](9, 2) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Teams] (
	[ID] [int] NOT NULL ,
	[Name] [nvarchar] (50) NULL ,
	[Description] [ntext] NULL ,
	[SecurityFlags] [nvarchar] (50) NULL ,
	[Color] [int] NULL ,
	[IDCard] [numeric](28, 0) NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[Terminals] (
	[ID] [tinyint] NOT NULL ,
	[Description] [ntext] NULL ,
	[InIDZone] [tinyint] NULL ,
	[OutIDZone] [tinyint] NULL ,
	[PictureX] [numeric](8, 3) NULL ,
	[PictureY] [numeric](8, 3) NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[Zones] (
	[ID] [tinyint] NOT NULL ,
	[Name] [nvarchar] (50) NULL ,
	[Description] [ntext] NULL ,
	[IsWorkingZone] [bit] NOT NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



-- Crea constraints nuevos
ALTER TABLE [dbo].[AccessGroups] WITH NOCHECK ADD 
	CONSTRAINT [PK_AccessGroups] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[AccessGroupsTimeZones] WITH NOCHECK ADD 
	CONSTRAINT [PK_AccessGroupsTimeZones] PRIMARY KEY  NONCLUSTERED 
	(
		[IDAccessGroup],
		[DayOfWeek],
		[BeginTime]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[AccessGroupZones] WITH NOCHECK ADD 
	CONSTRAINT [PK_AccessGroupZones] PRIMARY KEY  NONCLUSTERED 
	(
		[IDAccessGroup],
		[IDZone]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[DailyJobAccruals] WITH NOCHECK ADD 
	CONSTRAINT [DF_DailyJobAccruals_Pieces] DEFAULT (0) FOR [Pieces],
	CONSTRAINT [DF_DailyJobAccruals_PiecesType] DEFAULT (0) FOR [PiecesType],
	CONSTRAINT [PK_DailyJobAccruals] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[IDJob],
		[IDIncidence],
		[IDTeam],
		[Date],
		[PiecesType]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[EmployeeJobMoves] WITH NOCHECK ADD 
	CONSTRAINT [DF_EmployeeJobMoves_InFlag] DEFAULT (0) FOR [InMovesFlag],
	CONSTRAINT [DF_EmployeeJobMoves_OutMovesFlag] DEFAULT (0) FOR [OutMovesFlag],
	CONSTRAINT [DF_EmployeeJobMoves_Processed] DEFAULT (0) FOR [Processed],
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
	CONSTRAINT [PK_Jobs] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[JobTemplates] WITH NOCHECK ADD 
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
	CONSTRAINT [PK_Teams] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Terminals] WITH NOCHECK ADD 
	CONSTRAINT [PK_Terminales] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Zones] WITH NOCHECK ADD 
	CONSTRAINT [DF_Zones_IsWorkingZone] DEFAULT (0) FOR [IsWorkingZone],
	CONSTRAINT [Zones_PK] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

 CREATE  UNIQUE  INDEX [IX_JobsPunchID] ON [dbo].[Jobs]([PunchID]) ON [PRIMARY]
GO

 CREATE  INDEX [IDZone] ON [dbo].[Zones]([ID]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AccessGroupsTimeZones] ADD 
	CONSTRAINT [FK_AccessGroupsTimeZones_AccessGroups] FOREIGN KEY 
	(
		[IDAccessGroup]
	) REFERENCES [dbo].[AccessGroups] (
		[ID]
	)
GO

ALTER TABLE [dbo].[AccessGroupZones] ADD 
	CONSTRAINT [FK_AccessGroupZones_AccessGroups] FOREIGN KEY 
	(
		[IDAccessGroup]
	) REFERENCES [dbo].[AccessGroups] (
		[ID]
	),
	CONSTRAINT [FK_AccessGroupZones_Zones] FOREIGN KEY 
	(
		[IDZone]
	) REFERENCES [dbo].[Zones] (
		[ID]
	)
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
	CONSTRAINT [FK_DailyJobAccruals_Teams] FOREIGN KEY 
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
	CONSTRAINT [FK_EmployeeJobMoves_Jobs] FOREIGN KEY 
	(
		[IDJob]
	) REFERENCES [dbo].[Jobs] (
		[ID]
	)
GO

ALTER TABLE [dbo].[EmployeeJobPieces] ADD 
	CONSTRAINT [FK_EmployeeJobPieces_Employees] FOREIGN KEY 
	(
		[IDEmployee]
	) REFERENCES [dbo].[Employees] (
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

ALTER TABLE [dbo].[Jobs] ADD 
	CONSTRAINT [FK_Jobs_MachineGroups] FOREIGN KEY 
	(
		[IDMachineGroup]
	) REFERENCES [dbo].[MachineGroups] (
		[ID]
	),
	CONSTRAINT [FK_Jobs_Machines] FOREIGN KEY 
	(
		[IDMachine]
	) REFERENCES [dbo].[Machines] (
		[ID]
	),
	CONSTRAINT [FK_Jobs_Teams] FOREIGN KEY 
	(
		[IDTeam]
	) REFERENCES [dbo].[Teams] (
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


ALTER TABLE [dbo].[TeamJobMoves] ADD 
	CONSTRAINT [FK_TeamJobMoves_JobIncidences] FOREIGN KEY 
	(
		[InIDIncidence]
	) REFERENCES [dbo].[JobIncidences] (
		[ID]
	),
	CONSTRAINT [FK_TeamJobMoves_Jobs] FOREIGN KEY 
	(
		[IDJob]
	) REFERENCES [dbo].[Jobs] (
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
	CONSTRAINT [FK_TeamJobPieces_Teams] FOREIGN KEY 
	(
		[IDTeam]
	) REFERENCES [dbo].[Teams] (
		[ID]
	)
GO

ALTER TABLE [dbo].[Terminals] ADD 
	CONSTRAINT [FK_Terminals_Zones] FOREIGN KEY 
	(
		[InIDZone]
	) REFERENCES [dbo].[Zones] (
		[ID]
	),
	CONSTRAINT [FK_Terminals_Zones1] FOREIGN KEY 
	(
		[OutIDZone]
	) REFERENCES [dbo].[Zones] (
		[ID]
	)
GO


-- Entra/Modifica datos necesarios
UPDATE sysroGUI SET RequiredFeatures='Process\Comms' WHERE RequiredFeatures='Process\VTComms'
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='130' WHERE ID='DBVersion'
GO

