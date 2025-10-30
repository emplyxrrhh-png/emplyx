--
-- Tabla DailyJobAccruals
--
ALTER TABLE [dbo].[DailyJobAccruals] DROP CONSTRAINT [FK_DailyJobAccruals_Employees]
GO
ALTER TABLE [dbo].[DailyJobAccruals] DROP CONSTRAINT [FK_DailyJobAccruals_JobIncidences]
GO
ALTER TABLE [dbo].[DailyJobAccruals] DROP CONSTRAINT [FK_DailyJobAccruals_Jobs]
GO
ALTER TABLE [dbo].[DailyJobAccruals] DROP CONSTRAINT [FK_DailyJobAccruals_Machines]
GO
ALTER TABLE [dbo].[DailyJobAccruals] DROP CONSTRAINT [FK_DailyJobAccruals_Teams]
GO
ALTER TABLE [dbo].[DailyJobAccruals] ADD [Pieces1] [numeric] (9, 2) NULL 
GO
ALTER TABLE [dbo].[DailyJobAccruals] ADD [Pieces2] [numeric] (9, 2) NULL 
GO
ALTER TABLE [dbo].[DailyJobAccruals] ADD [Pieces3] [numeric] (9, 2) NULL 
GO
ALTER TABLE [dbo].[DailyJobAccruals] WITH NOCHECK ADD
  CONSTRAINT [DF_DailyJobAccruals_Pieces1] DEFAULT (0) FOR [Pieces1],
  CONSTRAINT [DF_DailyJobAccruals_Pieces2] DEFAULT (0) FOR [Pieces2],
  CONSTRAINT [DF_DailyJobAccruals_Pieces3] DEFAULT (0) FOR [Pieces3],
  CONSTRAINT [DF_DailyJobAccruals_IDTeam] DEFAULT (0) FOR IDTeam,
  CONSTRAINT [DF_DailyJobAccruals_IDMachine] DEFAULT (0) FOR IDMachine
GO
ALTER TABLE [dbo].[DailyJobAccruals] ALTER COLUMN [Pieces1] [numeric] (9, 2) NOT NULL 
GO
ALTER TABLE [dbo].[DailyJobAccruals] ALTER COLUMN [Pieces2] [numeric] (9, 2) NOT NULL 
GO
ALTER TABLE [dbo].[DailyJobAccruals] ALTER COLUMN [Pieces3] [numeric] (9, 2) NOT NULL 
GO
DROP INDEX [dbo].[DailyJobAccruals].[IX_DailyJobAccruals_EmployeeDate]
GO
DROP INDEX [dbo].[DailyJobAccruals].[IX_DailyJobAccruals_TeamDate]
GO
DROP INDEX [dbo].[DailyJobAccruals].[IX_DailyJobAccruals_Job]
GO
DROP INDEX [dbo].[DailyJobAccruals].[IX_DailyJobAccruals_JobIncidence]
GO
ALTER TABLE [dbo].[DailyJobAccruals] DROP CONSTRAINT [PK_DailyJobAccruals]
GO
CREATE INDEX [IX_DailyJobAccruals_EmployeeDate] ON [dbo].[DailyJobAccruals]([IDEmployee], [Date]) ON [PRIMARY]
GO
CREATE INDEX [IX_DailyJobAccruals_TeamDate] ON [dbo].[DailyJobAccruals]([IDTeam], [Date]) ON [PRIMARY]
GO
CREATE INDEX [IX_DailyJobAccruals_Job] ON [dbo].[DailyJobAccruals]([IDJob]) ON [PRIMARY]
GO
CREATE INDEX [IX_DailyJobAccruals_JobIncidence] ON [dbo].[DailyJobAccruals]([IDJob], [IDIncidence]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DailyJobAccruals] WITH NOCHECK ADD  CONSTRAINT [PK_DailyJobAccruals] PRIMARY KEY NONCLUSTERED (IDEmployee, IDJob, IDIncidence, IDTeam, IDMachine, Date) 
GO
--
-- Tabla DailySchedule 
--
DROP INDEX [dbo].[DailySchedule].[DailyScheduleByStatus]
GO
DROP INDEX [dbo].[DailySchedule].[IX_DailySchedule]
GO
ALTER TABLE [dbo].[DailySchedule] DROP CONSTRAINT [PK_DailyPlans]
GO 
ALTER TABLE [dbo].[DailySchedule] DROP CONSTRAINT [DF__DailySche__JobSt__10416098]
GO
ALTER TABLE [dbo].[DailySchedule] WITH NOCHECK ADD
  CONSTRAINT [PK_DailySchedule] PRIMARY KEY NONCLUSTERED (IDEmployee, Date) ,
  CONSTRAINT [DF_DailySchedule_JobStatus] DEFAULT (0) FOR JobStatus
GO
CREATE  INDEX [IX_DailyScheduleByStatus] ON [dbo].[DailySchedule]([Status], [IDEmployee], [Date]) ON [PRIMARY]
GO
CREATE  INDEX [IX_DailyScheduleByJobStatus] ON [dbo].[DailySchedule]([IDEmployee], [Date], [JobStatus]) ON [PRIMARY]
GO
--
-- Tabla EmployeeExpectedShifts
--
ALTER TABLE [dbo].[EmployeeExpectedShifts] DROP CONSTRAINT [DF__EmployeeE__Expec__7CF981FA]
GO
ALTER TABLE [dbo].[EmployeeExpectedShifts] WITH NOCHECK ADD 
    CONSTRAINT [DF__EmployeeE__Expec__530E3526] DEFAULT (0) FOR ExpectedShifts
GO
--
-- Tabla EmployeeJobMoves
--
ALTER TABLE [dbo].[EmployeeJobMoves] DROP
  CONSTRAINT [FK_EmployeeJobMoves_Employees],
  CONSTRAINT [FK_EmployeeJobMoves_JobIncidences],
  CONSTRAINT [FK_EmployeeJobMoves_Jobs],
  CONSTRAINT [FK_EmployeeJobMoves_Machines],
  CONSTRAINT [PK_EmployeeJobMoves],
  CONSTRAINT [DF_EmployeeJobMoves_InFlag],
  CONSTRAINT [DF_EmployeeJobMoves_OutMovesFlag],
  CONSTRAINT [DF_EmployeeJobMoves_Processed],
  CONSTRAINT [DF_EmployeeJobMoves_IDIncidence]
GO
ALTER TABLE [dbo].[EmployeeJobMoves] ADD 
  [Pieces1] [numeric] (9, 2) NULL,
  [Pieces2] [numeric] (9, 2) NULL, 
  [Pieces3] [numeric] (9, 2) NULL,
  [Value] [numeric] (8, 6) NULL,
  [IsDistributed] [bit] NULL,
  [IsCalculated] [bit]  NULL 
GO
ALTER TABLE [dbo].[EmployeeJobMoves] ALTER COLUMN [IDMachine] [tinyint] NOT NULL
GO
ALTER TABLE [dbo].[EmployeeJobMoves] DROP 
  COLUMN [InIDIncidence],
  COLUMN [OutIDIncidence],
  COLUMN [InMovesFlag],
  COLUMN [OutMovesFlag]
GO    
ALTER TABLE [dbo].[EmployeeJobMoves] WITH NOCHECK ADD
  CONSTRAINT [DF_EmployeeJobMoves_IDMachine] DEFAULT (0) FOR IDMachine,
  CONSTRAINT [DF_EmployeeJobMoves_Processed] DEFAULT (0) FOR Processed,
  CONSTRAINT [DF_EmployeeJobMoves_IDIncidence] DEFAULT (0) FOR IDIncidence,
  CONSTRAINT [DF_EmployeeJobMoves_Pieces1] DEFAULT (0) FOR [Pieces1],
  CONSTRAINT [DF_EmployeeJobMoves_Pieces2] DEFAULT (0) FOR [Pieces2],
  CONSTRAINT [DF_EmployeeJobMoves_Pieces3] DEFAULT (0) FOR [Pieces3],
  CONSTRAINT [DF_EmployeeJobMoves_Value] DEFAULT (1) FOR [Value],
  CONSTRAINT [DF_EmployeeJobMoves_IsDistributed] DEFAULT (0) FOR [IsDistributed],
  CONSTRAINT [DF_EmployeeJobMoves_IsCalculated] DEFAULT (1) FOR [IsCalculated]
GO
ALTER TABLE [dbo].[EmployeeJobMoves] ALTER COLUMN [Pieces1] [numeric] (9, 2) NOT NULL 
GO
ALTER TABLE [dbo].[EmployeeJobMoves] ALTER COLUMN [Pieces2] [numeric] (9, 2) NOT NULL 
GO
ALTER TABLE [dbo].[EmployeeJobMoves] ALTER COLUMN [Pieces3] [numeric] (9, 2) NOT NULL 
GO
ALTER TABLE [dbo].[EmployeeJobMoves] ALTER COLUMN [Value] [numeric] (8, 6) NOT NULL 
GO
ALTER TABLE [dbo].[EmployeeJobMoves] ALTER COLUMN [IsDistributed] [bit] NOT NULL 
GO
ALTER TABLE [dbo].[EmployeeJobMoves] ALTER COLUMN [IsCalculated] [bit] NOT NULL 
GO  
ALTER TABLE [dbo].[EmployeeJobMoves] WITH NOCHECK ADD  CONSTRAINT [PK_EmployeeJobMoves] PRIMARY KEY NONCLUSTERED (ID) 
GO
ALTER TABLE [dbo].[EmployeeJobMoves] ADD CONSTRAINT [FK_EmployeeJobMoves_Employees] FOREIGN KEY (IDEmployee)  REFERENCES [dbo].[Employees] (ID) 
GO
ALTER TABLE [dbo].[EmployeeJobMoves] ADD CONSTRAINT [FK_EmployeeJobMoves_JobIncidences] FOREIGN KEY (IDIncidence)  REFERENCES [dbo].[JobIncidences] (ID) 
GO
ALTER TABLE [dbo].[EmployeeJobMoves] ADD CONSTRAINT [FK_EmployeeJobMoves_Jobs] FOREIGN KEY (IDJob)  REFERENCES [dbo].[Jobs] (ID) 
GO
ALTER TABLE [dbo].[EmployeeJobMoves] ADD CONSTRAINT [FK_EmployeeJobMoves_Machines] FOREIGN KEY (IDMachine)  REFERENCES [dbo].[Machines] (ID) 
GO
--
-- Tabla Entries
-- 
ALTER TABLE [dbo].[Entries] DROP CONSTRAINT [DF__Entries__Invalid__3D491139]
GO
ALTER TABLE [dbo].[Entries] WITH NOCHECK ADD CONSTRAINT [DF_Entries_InvalidRead] DEFAULT (0) FOR InvalidRead
GO
--
-- Tabla Jobs
--
ALTER TABLE [dbo].[Jobs] ADD 
  [Customa] [nvarchar] (50) NULL ,
  [Customb] [nvarchar] (50) NULL ,
  [Customc] [nvarchar] (50) NULL ,
  [Customd] [nvarchar] (50) NULL ,
  [Custome] [numeric] (18, 2) NULL ,
  [Customf] [numeric] (18, 2) NULL ,
  [Customg] [smalldatetime] NULL ,
  [Customh] [smalldatetime] NULL ,
  [Selecteds] [nvarchar] (50) NULL 
GO
ALTER TABLE [dbo].[Jobs] ALTER COLUMN [PiecesAreShared] [bit] NOT NULL
GO
ALTER TABLE [dbo].[Jobs] ALTER COLUMN [IsExclusive] [bit] NOT NULL
GO
ALTER TABLE [dbo].[Jobs] ALTER COLUMN [AllowOnlyGrantedTeam] [bit] NOT NULL
GO
ALTER TABLE [dbo].[Jobs] ADD CONSTRAINT [DF_Jobs_PiecesAreShared] DEFAULT (0) FOR [PiecesAreShared]
GO
ALTER TABLE [dbo].[Jobs] ADD CONSTRAINT [DF_Jobs_IsExclusive] DEFAULT (0) FOR [IsExclusive]
GO
ALTER TABLE [dbo].[Jobs] ADD CONSTRAINT [DF_Jobs_AllowOnlyGrantedTeam] DEFAULT (0) FOR [AllowOnlyGrantedTeam]
GO
ALTER TABLE [dbo].[Jobs] ADD CONSTRAINT [DF_Jobs_Custome] DEFAULT (0) FOR [Custome]
GO
ALTER TABLE [dbo].[Jobs] ADD CONSTRAINT [DF_Jobs_Customf] DEFAULT (0) FOR [Customf]
GO
ALTER TABLE [dbo].[Jobs] ADD CONSTRAINT [FK_Jobs_MachineGroups] FOREIGN KEY (IDMachineGroup)  REFERENCES [dbo].[MachineGroups] (ID) 
GO
ALTER TABLE [dbo].[Jobs] ADD CONSTRAINT [FK_Jobs_Teams] FOREIGN KEY (IDTeam)  REFERENCES [dbo].[Teams] (ID) 
GO
--
-- Tabla JobTemplates
--
ALTER TABLE [dbo].[JobTemplates] ADD 
  [Customa] [nvarchar] (50) NULL ,
  [Customb] [nvarchar] (50) NULL ,
  [Customc] [nvarchar] (50) NULL ,
  [Customd] [nvarchar] (50) NULL ,
  [Custome] [numeric] (18, 2) NULL ,
  [Customf] [numeric] (18, 2) NULL ,
  [Customg] [smalldatetime] NULL ,
  [Customh] [smalldatetime] NULL ,
  [Selecteds] [nvarchar] (50) NULL 
GO
ALTER TABLE [dbo].[JobTemplates] ADD CONSTRAINT [DF_JobTemplates_PiecesAreShared] DEFAULT (0) FOR [PiecesAreShared]
GO
ALTER TABLE [dbo].[JobTemplates] ADD CONSTRAINT [DF_JobTemplates_IsExclusive] DEFAULT (0) FOR [IsExclusive]
GO
ALTER TABLE [dbo].[JobTemplates] ADD CONSTRAINT [DF_JobTemplates_AllowOnlyGrantedTeam] DEFAULT (0) FOR [AllowOnlyGrantedTeam]
GO
ALTER TABLE [dbo].[JobTemplates] ALTER COLUMN [PiecesAreShared] [bit] NOT NULL
GO
ALTER TABLE [dbo].[JobTemplates] ALTER COLUMN [IsExclusive] [bit] NOT NULL
GO
ALTER TABLE [dbo].[JobTemplates] ALTER COLUMN [AllowOnlyGrantedTeam] [bit] NOT NULL
GO
--
-- Tabla JobEntries
--
ALTER TABLE [dbo].[JobEntries] DROP CONSTRAINT [PK_JobEntries]
GO
ALTER TABLE [dbo].[JobEntries] DROP CONSTRAINT [FK_JobEntries_sysroEntryTypes]
GO
ALTER TABLE [dbo].[JobEntries] DROP 
  CONSTRAINT [DF_JobEntries_IDIncidence],
  CONSTRAINT [DF_JobEntries_IDMachine],
  CONSTRAINT [DF_JobEntries_InvalidRead]
GO
ALTER TABLE [dbo].[JobEntries] ADD 
  [IDJobOrIncidence] [numeric] (18, 0) NULL,
  [Pieces1] [numeric] (9, 2) NULL, 
  [Pieces2] [numeric] (9, 2) NULL,
  [Pieces3] [numeric] (9, 2) NULL
GO
ALTER TABLE [dbo].[JobEntries] DROP 
  COLUMN [IDJob], 
  COLUMN [IDIncidence]
GO
ALTER TABLE [dbo].[JobEntries] WITH NOCHECK ADD
  CONSTRAINT [DF_JobEntries_IDMachine] DEFAULT (0) FOR IDMachine,
  CONSTRAINT [DF_JobEntries_InvalidRead] DEFAULT (0) FOR InvalidRead,
  CONSTRAINT [DF_JobEntries_Pieces1] DEFAULT (0) FOR [Pieces1],
  CONSTRAINT [DF_JobEntries_Pieces11] DEFAULT (0) FOR [Pieces2],
  CONSTRAINT [DF_JobEntries_Pieces12] DEFAULT (0) FOR [Pieces3]
GO
ALTER TABLE [dbo].[JobEntries] ALTER COLUMN [Pieces1] [numeric] (9, 2) NOT NULL 
GO
ALTER TABLE [dbo].[JobEntries] ALTER COLUMN [Pieces2] [numeric] (9, 2) NOT NULL 
GO
ALTER TABLE [dbo].[JobEntries] ALTER COLUMN [Pieces3] [numeric] (9, 2) NOT NULL 
GO
ALTER TABLE [dbo].[JobEntries] WITH NOCHECK ADD  CONSTRAINT [PK_JobEntries] PRIMARY KEY NONCLUSTERED (ID) 
GO
ALTER TABLE [dbo].[JobEntries] ADD CONSTRAINT [FK_JobEntries_sysroEntryTypes] FOREIGN KEY (Type)  REFERENCES [dbo].[sysroEntryTypes] (Type) 
GO
--
-- Tabla TeamJobMoves
--
ALTER TABLE [dbo].[TeamJobMoves] DROP 
   CONSTRAINT [DF_TeamJobMoves_FlagInMoves],
   CONSTRAINT [DF_TeamJobMoves_FlagOutMoves]
GO

ALTER TABLE [dbo].[TeamJobMoves] DROP 
  COLUMN [InIDIncidence],
  COLUMN [OutIDIncidence],
  COLUMN [InMovesFlag],
  COLUMN [OutMovesFlag]
GO
ALTER TABLE [dbo].[TeamJobMoves] ADD
  [Pieces1] [numeric] (9, 2) NULL,
  [Pieces2] [numeric] (9, 2) NULL,
  [Pieces3] [numeric] (9, 2) NULL,
  [Value] [numeric] (8, 6) NULL ,
  [IsDistributed] [bit] NULL,
  [IsCalculated] [bit] NULL 
GO
ALTER TABLE [dbo].[TeamJobMoves] ADD 
  CONSTRAINT [DF_TeamJobMoves_IDIncidence] DEFAULT (0) FOR [IDIncidence],
  CONSTRAINT [DF_TeamJobMoves_Pieces1] DEFAULT (0) FOR [Pieces1],
  CONSTRAINT [DF_TeamJobMoves_Pieces2] DEFAULT (0) FOR [Pieces2],
  CONSTRAINT [DF_TeamJobMoves_Pieces3] DEFAULT (0) FOR [Pieces3],
  CONSTRAINT [DF_TeamJobMoves_Value] DEFAULT (1) FOR [Value],
  CONSTRAINT [DF_TeamJobMoves_IDMachine] DEFAULT (0) FOR [IDMachine],
  CONSTRAINT [DF_TeamJobMoves_IsDistributed] DEFAULT (0) FOR [IsDistributed],
  CONSTRAINT [DF_TeamJobMoves_IsCalculated] DEFAULT (1) FOR [IsCalculated]
GO    
ALTER TABLE [dbo].[TeamJobMoves] ALTER COLUMN [IDMachine] [tinyint] NOT NULL 
GO
ALTER TABLE [dbo].[TeamJobMoves] ALTER COLUMN [Pieces1] [numeric] (9, 2) NOT NULL 
GO
ALTER TABLE [dbo].[TeamJobMoves] ALTER COLUMN [Pieces2] [numeric] (9, 2) NOT NULL 
GO
ALTER TABLE [dbo].[TeamJobMoves] ALTER COLUMN [Pieces3] [numeric] (9, 2) NOT NULL 
GO
ALTER TABLE [dbo].[TeamJobMoves] ALTER COLUMN [Value] [numeric] (8, 6) NOT NULL 
GO
ALTER TABLE [dbo].[TeamJobMoves] ALTER COLUMN [IsDistributed] [bit] NOT NULL 
GO
ALTER TABLE [dbo].[TeamJobMoves] ALTER COLUMN [IsCalculated] [bit] NOT NULL 
GO
--
-- Tabla Moves
--
ALTER TABLE [dbo].[Moves] DROP CONSTRAINT [PK_Moves1]
GO
ALTER TABLE [dbo].[Moves] WITH NOCHECK ADD CONSTRAINT [PK_Moves] PRIMARY KEY NONCLUSTERED (ID) 
GO
--
-- Tabla Orders
-- 
ALTER TABLE [dbo].[Orders] ADD
  [PredictableEndDate] [smalldatetime] NULL ,
  [Customa] [nvarchar] (50) NULL ,
  [Customb] [nvarchar] (50) NULL ,
  [Customc] [nvarchar] (50) NULL ,
  [Customd] [nvarchar] (50) NULL ,
  [Custome] [numeric] (18, 2) NULL ,
  [Customf] [numeric] (18, 2) NULL ,
  [Customg] [smalldatetime] NULL ,
  [Customh] [smalldatetime] NULL ,
  [Selecteds] [nvarchar] (50) NULL 
GO
ALTER TABLE [dbo].[Orders] WITH NOCHECK ADD
  CONSTRAINT [DF_Orders_Custome] DEFAULT (0) FOR Custome,
  CONSTRAINT [DF_Orders_Customf] DEFAULT (0) FOR Customf
GO
--
-- Tabla OrderTemplates
--
ALTER TABLE [dbo].[OrderTemplates] ADD
  [Customa] [nvarchar] (50) NULL ,
  [Customb] [nvarchar] (50) NULL ,
  [Customc] [nvarchar] (50) NULL ,
  [Customd] [nvarchar] (50) NULL ,
  [Custome] [numeric] (18, 2) NULL ,
  [Customf] [numeric] (18, 2) NULL ,
  [Customg] [smalldatetime] NULL ,
  [Customh] [smalldatetime] NULL ,
  [Selecteds] [nvarchar] (50) NULL 
GO
ALTER TABLE [dbo].[OrderTemplates] WITH NOCHECK ADD
  CONSTRAINT [DF_OrderTemplates_Custome] DEFAULT (0) FOR Custome,
  CONSTRAINT [DF_OrderTemplates_Customf] DEFAULT (0) FOR Customf
GO
--
-- Tabla PiecesEntries
--
ALTER TABLE [dbo].[PiecesEntries] DROP CONSTRAINT [FK_PiecesEntries_sysroPieceTypes] 
GO
--
-- Tabla sysroImages
--
CREATE TABLE [dbo].[sysroImages]
    ( [ID] [int] NOT NULL ,
      [Image] [image] NULL )
GO
ALTER TABLE [dbo].[sysroImages] WITH NOCHECK ADD CONSTRAINT [PK_sysroImages] PRIMARY KEY NONCLUSTERED (ID) 
GO
--
-- Tabla sysroPieceTypes
--
ALTER TABLE [dbo].[sysroPieceTypes] DROP 
  CONSTRAINT [DF_sysroPieceTypes_ConsiderValid],
  CONSTRAINT [DF_sysroPieceTypes_CountOnPerformance]
GO
--
-- Tabla sysroDailyIncidencesTypes
--
ALTER TABLE [dbo].[sysroDailyIncidencesTypes] DROP 
   CONSTRAINT [DF_sysroDailyPartialsTypes_Active],
   CONSTRAINT [DF_sysroDailyPartialsTypes_TypePresence] 
GO
ALTER TABLE [dbo].[sysroDailyIncidencesTypes] ADD CONSTRAINT [DF_sysroDailyPartialsTypes_Active] DEFAULT (1) FOR [Stored]
GO
ALTER TABLE [dbo].[sysroDailyIncidencesTypes] ADD CONSTRAINT [DF_sysroDailyPartialsTypes_TypePresence] DEFAULT (1) FOR [WorkingTime]
GO
ALTER TABLE [dbo].[sysroDailyIncidencesTypes] ALTER COLUMN [Description] [nvarchar] (128) NULL 
GO
ALTER TABLE [dbo].[sysroDailyIncidencesTypes] DROP CONSTRAINT [PK_sysroDailyPartialsTypes] 
GO
--
-- Tabla sysroProductionFields
--
CREATE TABLE [dbo].[sysroProductionFields]
    ( [ID] [char] (10) NOT NULL ,
  [Order_Job] [tinyint] NOT NULL ,
  [Name] [nvarchar] (50) NOT NULL ,
  [TypeField] [tinyint] NOT NULL ,
  [DefaultValue] [nvarchar] (50) NULL ,
  [AutoLauncher] [bit] NOT NULL )
GO
ALTER TABLE [dbo].[sysroProductionFields] ADD CONSTRAINT [DF_sysroProductionFields_AutoLauncher] DEFAULT (0) FOR [AutoLauncher]
GO
ALTER TABLE [dbo].[sysroProductionFields] WITH NOCHECK ADD CONSTRAINT [PK_tmp] PRIMARY KEY NONCLUSTERED (ID, Order_Job) 
GO
--
-- Relaciones Finales
--
ALTER TABLE [dbo].[DailyJobAccruals] ADD CONSTRAINT [FK_DailyJobAccruals_Employees] FOREIGN KEY (IDEmployee)  REFERENCES [dbo].[Employees] (ID) 
GO
ALTER TABLE [dbo].[DailyJobAccruals] ADD CONSTRAINT [FK_DailyJobAccruals_JobIncidences] FOREIGN KEY (IDIncidence)  REFERENCES [dbo].[JobIncidences] (ID) 
GO
ALTER TABLE [dbo].[DailyJobAccruals] ADD CONSTRAINT [FK_DailyJobAccruals_Jobs] FOREIGN KEY (IDJob)  REFERENCES [dbo].[Jobs] (ID) 
GO
ALTER TABLE [dbo].[DailyJobAccruals] ADD CONSTRAINT [FK_DailyJobAccruals_Machines] FOREIGN KEY (IDMachine)  REFERENCES [dbo].[Machines] (ID) 
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='146' WHERE ID='DBVersion'
GO

