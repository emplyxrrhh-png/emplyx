-- Elimina tablas de la version beta 163 que deben rehacerse
DROP TABLE EmployeeBargains
GO
DROP TABLE EmployeeAnnualLimits
GO

--
-- Crea tabla de arrastres mensuales/anuales
--
CREATE TABLE [EmployeeConceptCarryOvers] (
	[IDEmployee] [int] NOT NULL,
	[IDConcept] [smallint] NOT NULL,
	[Period] [NVARCHAR] (1) NOT NULL,
	[MaxCarry] [numeric](19, 6) NOT NULL,
	[ExcessCarryIDCause] [smallint] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [EmployeeConceptCarryOvers] WITH NOCHECK ADD 
	CONSTRAINT [PK_EmployeeConceptCarryOvers] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[IDConcept],
		[Period]
	)  ON [PRIMARY] 
GO


-- Crea relación con la tabla Employees
ALTER TABLE [dbo].[EmployeeConceptCarryOvers] ADD CONSTRAINT [FK_EmployeeConceptCarryOvers_Employees] FOREIGN KEY (IDEmployee)  REFERENCES [dbo].[Employees] (ID) 
GO

-- Crea relación con la tabla Concepts
ALTER TABLE [dbo].[EmployeeConceptCarryOvers] ADD CONSTRAINT [FK_EmployeeConceptCarryOvers_Concepts] FOREIGN KEY (IDConcept)  REFERENCES [dbo].[Concepts] (ID)
GO

-- Crea relación con la tabla Causes
ALTER TABLE [dbo].[EmployeeConceptCarryOvers] ADD CONSTRAINT [FK_EmployeeConceptCarryOvers_Causes] FOREIGN KEY (ExcessCarryIDCause)  REFERENCES [dbo].[Causes] (ID)
GO


--
-- Crea tabla de limites anuales
--
CREATE TABLE [EmployeeConceptAnnualLimits] (
	[IDEmployee] [int] NOT NULL,
	[IDConcept] [smallint] NOT NULL,
	[StartupValue] [numeric](19, 6),
	[MaxValue] [numeric](19, 6),
	[MinValue] [numeric](19, 6),
) ON [PRIMARY]
GO
ALTER TABLE [EmployeeConceptAnnualLimits] WITH NOCHECK ADD 
	CONSTRAINT [PK_EmployeeConceptAnnualLimits] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[IDConcept]
	)  ON [PRIMARY] 
GO

-- Crea indice secundario para la tabla, para busquedas de valores iniciales por empleado
CREATE  INDEX [IX_EmployeeConceptAnnualLimits_StartupValue] ON [dbo].[EmployeeConceptAnnualLimits]([IDEmployee], [StartupValue]) ON [PRIMARY]
GO

-- Crea relación con la tabla Employees
ALTER TABLE [dbo].[EmployeeConceptAnnualLimits] ADD CONSTRAINT [FK_EmployeeConceptAnnualLimits_Employees] FOREIGN KEY (IDEmployee)  REFERENCES [dbo].[Employees] (ID) 
GO

-- Crea relación con la tabla Concepts
ALTER TABLE [dbo].[EmployeeConceptAnnualLimits] ADD CONSTRAINT [FK_EmployeeConceptAnnualLimits_Concepts] FOREIGN KEY (IDConcept)  REFERENCES [dbo].[Concepts] (ID)
GO


--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='164' WHERE ID='DBVersion'
GO
