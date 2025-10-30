-- Elimina tablas obsoletas
	
DROP TABLE AccessGroupsHollyday
GO


-- Ordena el menú de accesos

UPDATE sysroGUI SET Priority = 100 where IDPath='NavBar\Access\AccessStatus'
GO

UPDATE sysroGUI SET Priority = 200 where IDPath='NavBar\Access\AccessGroupsPeriods'
GO

UPDATE sysroGUI SET Priority = 300 where IDPath='NavBar\Access\AccessPeriods'
GO

UPDATE sysroGUI SET Priority = 400 where IDPath='NavBar\Access\AccessZones'
GO


-- Elimina opciones del menú de ayuda

DELETE From sysroGUI Where IDPath = 'Menu\Help\Agent'
GO

DELETE From sysroGUI Where IDPath = 'Menu\Help\Contents'
GO

DELETE From sysroGUI Where IDPath = 'Menu\Help\Tutorial'
GO

DELETE From sysroGUI Where IDPath = 'Menu\Help\Search'
GO

DELETE From sysroGUI Where IDPath = 'Menu\Help\TOC'
GO

DELETE From sysroGUI Where IDPath = 'Menu\Help\HelpS2'
GO

DELETE From sysroGUI Where IDPath = 'Menu\Help\HelpS3'
GO

DELETE From sysroGUI Where IDPath = 'Menu\Help\S4'
GO

UPDATE sysroGUI SET Priority = 700 Where IDPath = 'Menu\Help\HelpS1'
GO

--
-- Actualizamos la tabla employees para que tenga el campo imagen
--

ALTER TABLE [dbo].[Employees] ADD [Image] [Image] NULL
GO


--
-- Actualizamos la tabla de contratos de empleados para que tenga el campo empresa
--

ALTER TABLE [dbo].[EmployeeContracts] ADD [Enterprise] [nvarchar] (10) NULL
GO

-- Modificamos la tabla de Jobs
ALTER TABLE Jobs ADD ManualEndDate BIT NULL DEFAULT 0
GO
UPDATE JOBS SET ManualEndDate=0
GO


--
-- Creamos la tabla de costes de empleados
--
CREATE TABLE [dbo].[EmployeeJobCost] (
	[ID] [int] NOT NULL ,
	[IsFixed] [bit] NULL DEFAULT 0,
	[Value] [decimal] (18,3) NULL DEFAULT 0,
	[UsedField] [nvarchar] (50) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmployeeJobCost] WITH NOCHECK ADD 
	CONSTRAINT [PK_EmployeeJobCost] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO
INSERT INTO EmployeeJobCost VALUES(1,1,0,NULL)
GO

--
-- Creamos la tabla de costes de incidencias
--
CREATE TABLE [dbo].[EmployeeJobIncidencesCost] (
	[IDJobIncidence] [tinyint] NOT NULL ,
	[IsFixed] [bit] NULL DEFAULT 0,
	[Value] [decimal] (18,3) NULL DEFAULT 0,
	[UsedField] [nvarchar] (50) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmployeeJobIncidencesCost] WITH NOCHECK ADD 
	CONSTRAINT [PK_EmployeeJobIncidencesCost] PRIMARY KEY  NONCLUSTERED 
	(
		[IDJobIncidence]
	)  ON [PRIMARY] 
GO



--
-- Creamos tabla de acumulados de máquina
--
CREATE TABLE [dbo].[DailyMachineAccruals] (
	[IDMachine] [tinyint] NOT NULL ,
	[IDJob] [numeric] (18,0) NOT NULL,
	[Date] [smalldatetime] NOT NULL,
	[Value] [decimal] (9,6) NULL DEFAULT 0
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DailyMachineAccruals] WITH NOCHECK ADD 
	CONSTRAINT [PK_DailyMachineAccruals] PRIMARY KEY  NONCLUSTERED 
	(
		[IDMachine],
		[IDJob],
		[Date]

	)  ON [PRIMARY] 
GO



--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='172' WHERE ID='DBVersion'
GO
