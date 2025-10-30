-- Elimina tipo de incidencia 'Descanso' (pq no existe)
DELETE FROM sysroDailyIncidencesTypes WHERE ID=1040
GO

-- Elimina horas diurnas (ya existen horas normales)
DELETE FROM TimeZones WHERE ID=21
GO

-- Actualiza Tabla Zones
ALTER TABLE [dbo].[Zones] ADD [X1] [smallint] NULL DEFAULT (-1)
GO
ALTER TABLE [dbo].[Zones] ADD [Y1] [smallint] NULL DEFAULT (-1)
GO
ALTER TABLE [dbo].[Zones] ADD [X2] [smallint] NULL DEFAULT (-1)
GO
ALTER TABLE [dbo].[Zones] ADD [Y2] [smallint] NULL DEFAULT (-1)
GO
ALTER TABLE [dbo].[Zones] ADD [Proportion] [decimal] NULL DEFAULT (-1) 
GO

-- Actualizar Tabla Terminales
ALTER TABLE [dbo].[Terminals] ADD AccessMode TINYINT NOT NULL DEFAULT 0
GO

ALTER TABLE [dbo].[Terminals] ADD SirensMode TINYINT NOT NULL DEFAULT 0
GO

ALTER TABLE [dbo].[Terminals] ADD AccessReaders TINYINT NOT NULL DEFAULT 0
GO

ALTER TABLE [dbo].[Terminals] ADD AccessOutputs TINYINT NOT NULL DEFAULT 0
GO
-- Eliminar clave primaria TerminalSirens	
ALTER TABLE [dbo].[TerminalSirens] DROP CONSTRAINT [PK_TerminalSirens]
GO
-- Crear clave primaria TerminalSirens	
ALTER TABLE [dbo].[TerminalSirens] WITH NOCHECK ADD 
	CONSTRAINT [PK_TerminalSirens] PRIMARY KEY  NONCLUSTERED ([IDTerminal],[ID])  ON [PRIMARY] 
GO
-- Crear Relacion entre Terminal y TerminalSirens
ALTER TABLE [dbo].[TerminalSirens] ADD CONSTRAINT [FK_TerminalSirens_Terminals] FOREIGN KEY (IDTerminal)  REFERENCES [dbo].[Terminals] (ID) 
GO

-- Añadir campo en Employees
ALTER TABLE [dbo].[Employees] ADD [IDAccessGroup] [smallint] NULL 
GO

-- Crear Relacion entre Employees y AccessGroups
ALTER TABLE [dbo].[Employees] ADD 
	CONSTRAINT [FK_Employees_AccessGroups] FOREIGN KEY ([IDAccessGroup]) REFERENCES [dbo].[AccessGroups] ([ID])
GO

-- Actualiza Employee Contracts 
ALTER TABLE [dbo].[EmployeeContracts] DROP COLUMN [IDAccessGroup]
GO
-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='144' WHERE ID='DBVersion'
GO

