-- Elimina tablas obsoletas
DROP TABLE [dbo].[doors]
GO

-- Elimina campos obsoletos
ALTER TABLE [dbo].[JobIncidences] DROP COLUMN [ImputeToJob]
GO

ALTER TABLE [dbo].[Moves] DROP CONSTRAINT [DF_Moves_Processed]
GO
ALTER TABLE [dbo].[Moves] DROP COLUMN [Processed]
GO

-- Actualiza campos tablas
ALTER TABLE [dbo].[DailySchedule] ADD [JobStatus] TINYINT NOT NULL DEFAULT (0)
GO

ALTER TABLE [dbo].[Jobs] ADD [StartDate] SMALLDATETIME NULL
GO

ALTER TABLE [dbo].[Jobs] ADD [EndDate] SMALLDATETIME NULL
GO

ALTER TABLE [dbo].[Jobs] DROP COLUMN [PreparationTime] 
GO

ALTER TABLE [dbo].[Jobs] DROP COLUMN [PieceTime]
GO

ALTER TABLE [dbo].[Jobs] ADD [PreparationTime] [numeric](9,6) NOT NULL DEFAULT (0)
GO

ALTER TABLE [dbo].[Jobs] ADD [PieceTime] [numeric](9,6) NOT NULL DEFAULT (0)
GO

ALTER TABLE [dbo].[JobTemplates] DROP COLUMN [PreparationTime] 
GO

ALTER TABLE [dbo].[JobTemplates] DROP COLUMN [PieceTime]
GO

ALTER TABLE [dbo].[JobTemplates] ADD [PreparationTime] [numeric](9,6) NOT NULL DEFAULT (0)
GO

ALTER TABLE [dbo].[JobTemplates] ADD [PieceTime] [numeric](9,6) NOT NULL DEFAULT (0)
GO

-- Cambia campos que no pueden ser null y antes si
UPDATE [dbo].[Shifts] SET ExpectedWorkingHours=0 WHERE ExpectedWorkingHours is NULL
GO
ALTER TABLE [dbo].[Shifts] ALTER COLUMN ExpectedWorkingHours [numeric](9,6) NOT NULL
GO
ALTER TABLE [dbo].[Shifts] WITH NOCHECK ADD CONSTRAINT [DF_Shifts_ExpectedWorkingHours] DEFAULT (0) FOR [ExpectedWorkingHours]
GO

UPDATE [dbo].[Shifts] SET IsTemplate=0 WHERE IsTemplate is NULL
GO
ALTER TABLE [dbo].[Shifts] ALTER COLUMN IsTemplate BIT NOT NULL
GO

UPDATE [dbo].[Shifts] SET Color=0 WHERE Color is NULL
GO
ALTER TABLE [dbo].[Shifts] ALTER COLUMN Color INT NOT NULL
GO
ALTER TABLE [dbo].[Shifts] WITH NOCHECK ADD CONSTRAINT [DF_Shifts_Color] DEFAULT (0) FOR [Color]
GO

UPDATE [dbo].[Shifts] SET StartLimit=CONVERT(DATETIME,'1899/12/30 00:00:00 ',120) WHERE StartLimit is NULL
GO
ALTER TABLE [dbo].[Shifts] ALTER COLUMN StartLimit DATETIME NOT NULL
GO

UPDATE [dbo].[Shifts] SET EndLimit=CONVERT(DATETIME,'1899/12/30 23:59:00 ',120) WHERE EndLimit is NULL
GO
ALTER TABLE [dbo].[Shifts] ALTER COLUMN EndLimit DATETIME NOT NULL
GO

UPDATE [dbo].[Employees] SET Type='A' WHERE Type Is Null
GO
ALTER TABLE [dbo].[Employees] ALTER COLUMN Type CHAR(1) NOT NULL
GO


-- Cambia IDs a autonumericos
ALTER TABLE [dbo].[EmployeeJobMoves] DROP CONSTRAINT [PK_EmployeeJobMoves]
GO
ALTER TABLE [dbo].[EmployeeJobMoves] DROP COLUMN [ID]
GO
ALTER TABLE [dbo].[EmployeeJobMoves] ADD [ID] [numeric](16, 0) IDENTITY (1, 1) NOT NULL 
GO
ALTER TABLE [dbo].[EmployeeJobMoves] WITH NOCHECK ADD CONSTRAINT [PK_EmployeeJobMoves] PRIMARY KEY  NONCLUSTERED ([ID]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TeamJobMoves] DROP CONSTRAINT [PK_TeamJobMoves]
GO
ALTER TABLE [dbo].[TeamJobMoves] DROP COLUMN [ID]
GO
ALTER TABLE [dbo].[TeamJobMoves] ADD [ID] [numeric](16, 0) IDENTITY (1, 1) NOT NULL 
GO
ALTER TABLE [dbo].[TeamJobMoves] WITH NOCHECK ADD CONSTRAINT [PK_TeamJobMoves] PRIMARY KEY  NONCLUSTERED ([ID]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Entries] DROP CONSTRAINT [PK_Entries]
GO
ALTER TABLE [dbo].[Entries] DROP COLUMN [ID]
GO
ALTER TABLE [dbo].[Entries] ADD [ID] [numeric](16, 0) IDENTITY (1, 1) NOT NULL 
GO
ALTER TABLE [dbo].[Entries] WITH NOCHECK ADD CONSTRAINT [PK_Entries] PRIMARY KEY  NONCLUSTERED ([ID]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[InvalidEntries] DROP CONSTRAINT [PK_InvalidEntries]
GO
ALTER TABLE [dbo].[InvalidEntries] DROP COLUMN [ID]
GO
ALTER TABLE [dbo].[InvalidEntries] ADD [ID] [numeric](16, 0) IDENTITY (1, 1) NOT NULL 
GO
ALTER TABLE [dbo].[InvalidEntries] WITH NOCHECK ADD CONSTRAINT [PK_InvalidEntries] PRIMARY KEY  NONCLUSTERED ([ID]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[JobEntries] DROP CONSTRAINT [PK_JobEntries]
GO
ALTER TABLE [dbo].[JobEntries] DROP COLUMN [ID]
GO
ALTER TABLE [dbo].[JobEntries] ADD [ID] [numeric](16, 0) IDENTITY (1, 1) NOT NULL 
GO
ALTER TABLE [dbo].[JobEntries] WITH NOCHECK ADD CONSTRAINT [PK_JobEntries] PRIMARY KEY  NONCLUSTERED ([ID]) ON [PRIMARY]
GO

ALTER TABLE Moves DROP CONSTRAINT [PK_Moves1]
GO
ALTER TABLE Moves DROP COLUMN [ID]
GO
ALTER TABLE Moves ADD [ID] [numeric](16, 0) IDENTITY (1, 1) NOT NULL 
GO
ALTER TABLE [dbo].[Moves] WITH NOCHECK ADD CONSTRAINT [PK_Moves1] PRIMARY KEY  NONCLUSTERED ([ID]) ON [PRIMARY] 
GO


-- Añade horario 0
INSERT INTO [dbo].[Shifts](ID,Name,StartLimit,EndLimit) VALUES(0,'???',CONVERT(DATETIME,'1899/12/30 00:00:00 ',120),CONVERT(DATETIME,'1899/12/30 23:59:00 ',120))
GO

-- Crea indices nuevos
CREATE NONCLUSTERED INDEX [DailyScheduleByStatus] ON [dbo].[dailyschedule]([status], [idemployee], [date])
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='132' WHERE ID='DBVersion'
GO

