--
-- Actualiza Shifts para VT Express
--

-- Añade campo Tipo Horario
ALTER TABLE [dbo].[Shifts] ADD [TypeShift] [nvarchar] (50) NULL 
GO


-- Borra la tabla Exports
DROP TABLE [dbo].[Exports]
GO

-- Borra la tabla ExportsFormats
DROP TABLE [dbo].[ExportsFormats]
GO

-- Borra la tabla ExportsFormats
DROP TABLE [dbo].[ExportsPeriods]
GO

-- Borra el campo Codigo_Empresa de la ficha del empleado.
ALTER TABLE [dbo].[Employees] DROP COLUMN [USR_Codigo_Empresa]
GO

--
-- Actualiza Tabla terminales para saber si el terminal tiene
-- funcionamiento de presencia o producción por defecto (TCP/IP).
--
ALTER TABLE [dbo].[Terminals] ADD [DefaultMode] [nvarchar] (4) NULL

--Actualiza campos de incidencias de produccion
ALTER TABLE [dbo].[JobIncidences] ADD [ViewInPays] [bit] NULL
GO

ALTER TABLE [dbo].[JobIncidences] ADD [FixedPay] [bit] NULL
GO

ALTER TABLE [dbo].[JobIncidences] ADD [PayValue] [numeric](18, 3) NULL
GO

ALTER TABLE [dbo].[JobIncidences] ADD [UsedField] [nvarchar] (50) NULL
GO

ALTER TABLE [dbo].[JobIncidences] ADD [RoundingBy] [numeric](18, 3) NULL
GO

--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='166' WHERE ID='DBVersion'
GO

