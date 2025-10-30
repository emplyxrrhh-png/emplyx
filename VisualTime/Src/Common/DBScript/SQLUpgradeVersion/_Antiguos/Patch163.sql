-- Añade la información para saber si una justificación se introdujo manual o automáticamente
ALTER TABLE [DailyCauses] ADD [Manual] BIT DEFAULT (0)
GO

-- Por defecto, las justificaciones que ya habia se consideran como manuales (por compatibilidad).
UPDATE [DailyCauses] SET Manual=1
GO


-- Añade campo para indicar el tipo de consulta por defecto en un acumulado
ALTER TABLE [Concepts] ADD [DefaultQuery] NVARCHAR(1) DEFAULT 'M'
GO
UPDATE [Concepts] SET DefaultQuery='M'
GO

--Modificamos el Valor de los acumulados y de las justificaciones para que el valor pueda ser mayor
ALTER TABLE [DailyAccruals] ALTER COLUMN  [Value] [numeric](19, 6) not null 
GO
ALTER TABLE [DailyCauses] ALTER COLUMN  [Value] [numeric](19, 6) not null 
GO


--Añade nuevo tipo de capas horarias rigidas flotantes
INSERT INTO sysroShiftsLayersTypes(ID,Name) VALUES(1050,'roLTFloatingMandatory')
GO

--Borramos las tablas que se iban a utilizar en el simulador
DROP TABLE SIMDailyAccruals
GO
DROP TABLE SIMDailyCauses
GO
DROP TABLE SIMDailyIncidences
GO
DROP TABLE SIMMoves
GO


--Añadimos tas tablas de arrastres y contadores
CREATE TABLE [EmployeeBargains] (
	[IDEmployee] [int] NOT NULL ,
	[IDConcept] [smallint] NOT NULL ,
	[TYPE] [NVARCHAR] (1) NOT NULL ,
	[MaxValue] [numeric](19, 6) NOT NULL  ,
) ON [PRIMARY]
GO

ALTER TABLE [EmployeeBargains] WITH NOCHECK ADD 
	CONSTRAINT [PK_EmployeeBargains] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[IDConcept],
		[Type]
	)  ON [PRIMARY] 
GO


CREATE TABLE [EmployeeAnnualLimits] (
	[IDEmployee] [int] NOT NULL ,
	[IDConcept] [smallint] NOT NULL ,
	[MaxAnnual] [numeric](19, 6) NOT NULL  ,
) ON [PRIMARY]
GO

ALTER TABLE [EmployeeAnnualLimits] WITH NOCHECK ADD 
	CONSTRAINT [PK_EmployeeAnnualLimits] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[IDConcept]
	)  ON [PRIMARY] 
GO



--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='163' WHERE ID='DBVersion'
GO
