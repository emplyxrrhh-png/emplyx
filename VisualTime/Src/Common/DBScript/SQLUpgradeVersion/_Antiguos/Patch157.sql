--
-- Actualizamos tablas de acumulados
--

ALTER TABLE [dbo].[Concepts] ADD [ViewInPays] [bit] NULL
GO

ALTER TABLE [dbo].[Concepts] ADD [FixedPay] [bit] NULL
GO

ALTER TABLE [dbo].[Concepts] ADD [PayValue] [numeric](18, 3) NULL
GO

ALTER TABLE [dbo].[Concepts] ADD [UsedField] [nvarchar] (50) NULL
GO

ALTER TABLE [dbo].[Concepts] ADD [RoundingBy] [numeric](18, 3) NULL
GO

ALTER TABLE [dbo].[Concepts] ADD [Export] [numeric](18, 2) NULL
GO

ALTER TABLE [dbo].[Concepts] WITH NOCHECK ADD 
	CONSTRAINT [DF_Concepts_Export] DEFAULT (0) FOR [Export] 
GO


-- Actualiazmos tabla de incidencias de produccion
ALTER TABLE [dbo].[JobIncidences] ADD [CountOnPerformance] [bit] NULL
GO


UPDATE [dbo].[JobIncidences] SET  CountOnPerformance = 1 WHERE ID = 0
GO


-- Elimina tabla de mensajes y crea una nueva
DROP TABLE Messages
GO
CREATE TABLE [EmployeeTerminalMessages] (
	[IDEmployee] [int] NOT NULL ,
	[Message] [nvarchar] (64) NOT NULL ,
	[Schedule] [nvarchar] (128) NOT NULL ,
	[LastTimeShown] [smalldatetime] NULL ,
	CONSTRAINT [PK_EmployeeTerminalMessages] PRIMARY KEY  CLUSTERED 
	(
		[IDEmployee],
		[Message]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO



-- Crea tablas nuevas
CREATE TABLE [dbo].[EmployeeMachines] (
	[IDMachine] [smallint] NOT NULL ,
	[IDEmployee] [int] NOT NULL ,
	[Cost] [numeric](18, 3) NULL
) ON [PRIMARY] 
GO

-- Crea constraints nuevos
ALTER TABLE [dbo].[EmployeeMachines] WITH NOCHECK ADD 
	CONSTRAINT [PK_EmployeeMachines] PRIMARY KEY  NONCLUSTERED 
	(	[IDMachine],
		[IDEmployee]
	)  ON [PRIMARY] 

GO

ALTER TABLE [dbo].[EmployeeMachines] WITH NOCHECK ADD 
	CONSTRAINT [DF_EmployeeMachines_Cost] DEFAULT (0) FOR [Cost]
GO


--Creamos las tablas para el simulador de horas extras
CREATE TABLE [dbo].[SIMDailyAccruals] (
	[IDEmployee] [int] NOT NULL ,
	[Date] [smalldatetime] NOT NULL ,
	[IDConcept] [smallint] NOT NULL ,
	[Value] [numeric](9, 6) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SIMDailyCauses] (
	[IDEmployee] [int] NOT NULL ,
	[Date] [smalldatetime] NOT NULL ,
	[IDRelatedIncidence] [tinyint] NOT NULL ,
	[IDCause] [smallint] NOT NULL ,
	[Value] [numeric](8, 6) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SIMDailyIncidences] (
	[ID] [smallint] NOT NULL ,
	[IDEmployee] [int] NOT NULL ,
	[Date] [smalldatetime] NOT NULL ,
	[IDType] [smallint] NOT NULL ,
	[IDZone] [tinyint] NOT NULL ,
	[Value] [numeric](8, 6) NOT NULL ,
	[BeginTime] [smalldatetime] NOT NULL ,
	[EndTime] [smalldatetime] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SIMMoves] (
	[IDEmployee] [int] NOT NULL ,
	[InDateTime] [smalldatetime] NULL ,
	[OutDateTime] [smalldatetime] NULL ,
	[InIDReader] [tinyint] NULL ,
	[OutIDReader] [tinyint] NULL ,
	[InIDCause] [smallint] NULL ,
	[OutIDCause] [smallint] NULL ,
	[ShiftDate] [smalldatetime] NULL ,
	[ID] [numeric](16, 0) NOT NULL ,
	[InIDZone] [tinyint] NULL ,
	[OutIDZone] [tinyint] NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SIMDailyAccruals] WITH NOCHECK ADD 
	CONSTRAINT [PK_SIMDailyTotals] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[Date],
		[IDConcept]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[SIMDailyCauses] WITH NOCHECK ADD 
	CONSTRAINT [DF_SIMDailyCauses_IDRelatedIncidence] DEFAULT (0) FOR [IDRelatedIncidence],
	CONSTRAINT [PK_SIMDailyCauses] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[Date],
		[IDRelatedIncidence],
		[IDCause]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[SIMDailyIncidences] WITH NOCHECK ADD 
	CONSTRAINT [DF_SIMDailyIncidences_IDZone] DEFAULT (0) FOR [IDZone],
	CONSTRAINT [DF_SIMDailyPartials_Value] DEFAULT (0) FOR [Value],
	CONSTRAINT [PK_SIMDailyIncidences] PRIMARY KEY  NONCLUSTERED 
	(
		[ID],
		[IDEmployee],
		[Date]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[SIMMoves] WITH NOCHECK ADD 
	CONSTRAINT [PK_SIMMoves] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

 CREATE  INDEX [IX_SIMDailyAccruals] ON [dbo].[SIMDailyAccruals]([IDEmployee], [Date]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_SIMDailyCauses] ON [dbo].[SIMDailyCauses]([IDEmployee], [Date]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_SIMDailyIncidences] ON [dbo].[SIMDailyIncidences]([IDEmployee], [Date], [IDType]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_SIMDailyIncidences_1] ON [dbo].[SIMDailyIncidences]([IDEmployee], [Date]) ON [PRIMARY]
GO

-- Actualiazmos tabla de Fases de produccion
ALTER TABLE [dbo].[Jobs] ADD [EmployeeCost] [numeric](18, 3) NULL
GO
ALTER TABLE [dbo].[Jobs] WITH NOCHECK ADD 
	CONSTRAINT [DF_JobsEmployee_Cost] DEFAULT (0) FOR [EmployeeCost]
GO



-- Insertamos opcion de Enlace a Access2000
INSERT INTO sysroGUI (IDPath, LanguageReference, URL, IconURL, RequiredFeatures, SecurityFlags, Priority, AllowedSecurity) VALUES ('NavBar\FirstTab\Link','Linker','file://$(VTSYSTEMPATH)\VTMDBLink.exe','Computer.ico','Feature\MDBLink','3111111111',775,'NRW')
GO
--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='157' WHERE ID='DBVersion'
GO

