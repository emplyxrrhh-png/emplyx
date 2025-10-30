-- Modifica campos descripcion
ALTER TABLE [dbo].[sysroShiftLayerTypes] ALTER COLUMN [Description] NTEXT
GO



-- Modifica campos IDReader
ALTER TABLE [dbo].[Moves] ALTER COLUMN [InIDReader] TINYINT
GO
ALTER TABLE [dbo].[Moves] ALTER COLUMN [OutIDReader] TINYINT
GO



-- Modifica campos Código por terminal
DROP INDEX [dbo].[Causes].[IX_Causes_ReaderInputCode]
GO
ALTER TABLE [dbo].[Causes] ALTER COLUMN [ReaderInputCode] TINYINT
GO
CREATE INDEX [IX_Causes_ReaderInputCode] ON [dbo].[Causes]([ReaderInputCode]) ON [PRIMARY]
GO



-- Modifica campos IDRelatedIncidence
ALTER TABLE [dbo].[DailyCauses] DROP CONSTRAINT [DF_DailyCauses_IDRelatedIncidence]
GO
ALTER TABLE [dbo].[DailyCauses] DROP CONSTRAINT [PK_DailyCauses]
GO
ALTER TABLE [dbo].[DailyCauses] ALTER COLUMN [IdRelatedIncidence] TINYINT NOT NULL
GO
ALTER TABLE [dbo].[DailyCauses] WITH NOCHECK ADD 
	CONSTRAINT [DF_DailyCauses_IDRelatedIncidence] DEFAULT (0) FOR [IDRelatedIncidence],	
	CONSTRAINT [PK_DailyCauses] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[Date],
		[IDRelatedIncidence],
		[IDCause]
	)  ON [PRIMARY] 

GO



-- Modifica campos TimeZone
ALTER TABLE [dbo].[DailyIncidences] DROP CONSTRAINT [DF_DailyIncidences_IDZone]
GO
ALTER TABLE [dbo].[DailyIncidences] DROP CONSTRAINT [FK_DailyPartials_TimeZones]
GO
ALTER TABLE [dbo].[DailyIncidences] ALTER COLUMN [IDZone] TINYINT NOT NULL
GO

ALTER TABLE [dbo].[sysroShiftTimeZones] DROP CONSTRAINT [FK_sysroShiftTimeZones_TimeZones]
GO
ALTER TABLE [dbo].[sysroShiftTimeZones] DROP CONSTRAINT [PK_sysroShiftTimeZones]
GO
ALTER TABLE [dbo].[sysroShiftTimeZones] ALTER COLUMN [IDZone] TINYINT NOT NULL
GO

ALTER TABLE [dbo].[TimeZones] DROP CONSTRAINT [PK_TimeZones]
GO
ALTER TABLE [dbo].[TimeZones] ALTER COLUMN [ID] TINYINT NOT NULL
GO

ALTER TABLE [dbo].[TimeZones] WITH NOCHECK ADD 
	CONSTRAINT [PK_TimeZones] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY]
GO
ALTER TABLE [dbo].[sysroShiftTimeZones] WITH NOCHECK ADD 
	CONSTRAINT [PK_sysroShiftTimeZones] PRIMARY KEY  NONCLUSTERED 
	(
		[IDShift],
		[IDZone],
		[BeginTime]
	)  ON [PRIMARY] 
GO
ALTER TABLE [dbo].[sysroShiftTimeZones] ADD 
	CONSTRAINT [FK_sysroShiftTimeZones_TimeZones] FOREIGN KEY 
	(
		[IDZone]
	) REFERENCES [dbo].[TimeZones] (
		[ID]
	)
GO

ALTER TABLE [dbo].[DailyIncidences] WITH NOCHECK ADD 
	CONSTRAINT [DF_DailyIncidences_IDZone] DEFAULT (0) FOR [IDZone],
	CONSTRAINT [FK_DailyPartials_TimeZones] FOREIGN KEY 
	(
		[IDZone]
	) REFERENCES [dbo].[TimeZones] (
		[ID]
	)
GO



--Cambia ExpectedConceptHours
ALTER TABLE [dbo].[EmployeeExpectedConceptHours] ALTER COLUMN ExpectedHours NUMERIC(12,6)
GO



--No permite nombres nulos de empleados
ALTER TABLE [dbo].[Employees] ALTER COLUMN [Name] NVARCHAR(50) NOT NULL
GO



-- Cambia campos de tabla Entries
ALTER TABLE [dbo].[Entries] ALTER COLUMN [DateTime] SMALLDATETIME NOT NULL
GO
DROP INDEX [dbo].[Entries].[IDTerminal]
GO
ALTER TABLE [dbo].[Entries] ALTER COLUMN [IDReader] TINYINT NOT NULL
GO



-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='133' WHERE ID='DBVersion'
GO

