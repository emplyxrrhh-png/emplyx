-- Elimina tablas 
DROP TABLE EmployeeExpectedConceptHours
GO

--
-- Actualiza DailyAccruals
--

-- Elimina indices y restricciones antiguas
ALTER TABLE [dbo].[DailyAccruals] DROP CONSTRAINT [PK_DailyTotals]
GO
DROP INDEX [dbo].[DailyAccruals].[IX_DailyAccruals]
GO

-- Añade campo para arrastres
ALTER TABLE [dbo].[DailyAccruals] ADD [CarryOver] [bit] NOT NULL DEFAULT (0)
GO

-- Crea nuevos indices/restricciones
ALTER TABLE [dbo].[DailyAccruals] WITH NOCHECK ADD 
	CONSTRAINT [PK_DailyTotals] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[Date],
		[IDConcept],
		[CarryOver]
	)  ON [PRIMARY] 
GO
CREATE INDEX [IX_DailyAccruals] ON [dbo].[DailyAccruals]([IDEmployee], [Date]) ON [PRIMARY]
GO


--
-- Crea tabla de arrastres pendientes
--
CREATE TABLE [sysroPendingCarryOvers] (
	[IDEmployee] [int] NOT NULL,
	[Year] [smallint] NOT NULL,
	[Month] [smallint] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [sysroPendingCarryOvers] WITH NOCHECK ADD 
	CONSTRAINT [PK_sysroPendingCarryOvers] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[Year],
		[Month]
	)  ON [PRIMARY] 
GO



--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='165' WHERE ID='DBVersion'
GO
