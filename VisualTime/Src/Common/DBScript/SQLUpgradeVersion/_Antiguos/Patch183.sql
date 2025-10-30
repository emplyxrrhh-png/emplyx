-- ********************    TABLAS PARA VTTAREAS   ****************************************************************************

-- *********** TABLA DE TAREAS ********************

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_EmployeeTasks_Tasks]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[EmployeeTasks] DROP CONSTRAINT FK_EmployeeTasks_Tasks
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_TasksMoves_Tasks]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[TasksMoves] DROP CONSTRAINT FK_TasksMoves_Tasks
GO


CREATE TABLE [dbo].[Tasks] (
	[ID] [int] NOT NULL ,
	[Name] [nchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Description] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[PlanDate] [smalldatetime] NULL ,
	[StartDate] [smalldatetime] NOT NULL ,
	[DueDate] [smalldatetime] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Tasks] WITH NOCHECK ADD 
	CONSTRAINT [PK_Tasks] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

-- *** Insertamos la tarea Inactividad, que estará por defecto ***
INSERT INTO Tasks values (0,'Inactividad','No se está realizando ninguna tarea productiva',NULL,'01/01/2005',NULL)



-- *********** TABLA DE FICHAJES DE TAREAS ********************




CREATE TABLE [dbo].[TasksMoves] (
	[IDEmployee] [int] NOT NULL ,
	[IDTask] [int] NOT NULL ,
	[BeginTime] [smalldatetime] NOT NULL ,
	[EndTime] [smalldatetime] NULL ,
	[ID] [int] IDENTITY (1, 1) NOT NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TasksMoves] WITH NOCHECK ADD 
	CONSTRAINT [PK_TasksMoves] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[TasksMoves] ADD 
	CONSTRAINT [FK_TasksMoves_Employees] FOREIGN KEY 
	(
		[IDEmployee]
	) REFERENCES [dbo].[Employees] (
		[ID]
	),
	CONSTRAINT [FK_TasksMoves_Tasks] FOREIGN KEY 
	(
		[IDTask]
	) REFERENCES [dbo].[Tasks] (
		[ID]
	)
GO



-- *********** TABLA DE TAREAS POR USUARIO ********************


CREATE TABLE [dbo].[EmployeeTasks] (
	[IDTask] [int] NOT NULL ,
	[IDEmployee] [int] NOT NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmployeeTasks] ADD 
	CONSTRAINT [FK_EmployeeTasks_Employees] FOREIGN KEY 
	(
		[IDEmployee]
	) REFERENCES [dbo].[Employees] (
		[ID]
	),
	CONSTRAINT [FK_EmployeeTasks_Tasks] FOREIGN KEY 
	(
		[IDTask]
	) REFERENCES [dbo].[Tasks] (
		[ID]
	)
GO

-- *** Asignamos la tarea inactividad a todos los empleados ***

INSERT INTO EmployeeTasks SELECT 0, ID FROM Employees



-- *********** TABLA DE PERFILES ********************

CREATE TABLE [Profiles] (
	[ID] [int] NOT NULL ,
	[Name] [char] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	CONSTRAINT [PK_Profiles] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO

-- *********** TABLA DE PERFILES POR EMPLEADO ********************

CREATE TABLE [EmployeeProfiles] (
	[IdEmployee] [int] NOT NULL ,
	[Profile] [int] NOT NULL ,
	CONSTRAINT [PK_EmployeeProfiles] PRIMARY KEY  CLUSTERED 
	(
		[IdEmployee],
		[Profile]
	)  ON [PRIMARY] ,
	CONSTRAINT [FK_EmployeeProfiles_Employees] FOREIGN KEY 
	(
		[IdEmployee]
	) REFERENCES [Employees] (
		[ID]
	),
	CONSTRAINT [FK_EmployeeProfiles_Profiles] FOREIGN KEY 
	(
		[Profile]
	) REFERENCES [Profiles] (
		[ID]
	)
) ON [PRIMARY]
GO

-- *********** Insertar nuevos tipos de solicitud (webTerminal) **************


if not exists(SELECT * FROM sysroRequestType where IdType = 6)
Insert Into sysroRequestType (IdType, Type) VALUES (6,'Aus.Prolongada')
GO

if not exists(SELECT * FROM sysroRequestType where IdType = 7)
Insert Into sysroRequestType (IdType, Type) VALUES (7,'Prev.Incidencia')
GO

-- ********************************  INCIDENCIAS PROGRAMADAS **********************

CREATE TABLE [ProgrammedCauses] (
	[IDCause] [smallint] NOT NULL ,
	[IDEmployee] [int] NOT NULL ,
	[Date] [smalldatetime] NOT NULL ,
	[Duration] [numeric](8, 6) NOT NULL ,
	[Description] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	CONSTRAINT [PK_ProgrammedCauses] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[Date]
	)  ON [PRIMARY] ,
	CONSTRAINT [FK_ProgrammedCauses_Causes] FOREIGN KEY 
	(
		[IDCause]
	) REFERENCES [Causes] (
		[ID]
	),
	CONSTRAINT [FK_ProgrammedCauses_Employees] FOREIGN KEY 
	(
		[IDEmployee]
	) REFERENCES [Employees] (
		[ID]
	)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



-- *******************************  ACTUALIZACIÓN TABLA TERMINALES *********************

ALTER TABLE [dbo].[Terminals] ADD [LastAction] [smalldatetime] NULL 
Go

ALTER TABLE [dbo].[Terminals] ADD [Other] [varchar] (150) NULL
Go

-- ************** ACTUALIZACIÓN Límites y valores iniciales *************

--Eliminamos las constraints
ALTER TABLE [dbo].[EmployeeConceptAnnualLimits] DROP CONSTRAINT [PK_EmployeeConceptAnnualLimits]
Go
ALTER TABLE [dbo].[EmployeeConceptAnnualLimits] DROP CONSTRAINT [FK_EmployeeConceptAnnualLimits_Concepts]
Go
ALTER TABLE [dbo].[EmployeeConceptAnnualLimits] DROP CONSTRAINT [FK_EmployeeConceptAnnualLimits_Employees]
Go

-- Nueva PK para el año

ALTER TABLE [dbo].[EmployeeConceptAnnualLimits] ADD [IDYear] [int]  NULL 
Go
UPDATE EmployeeConceptAnnualLimits SET idyear=2007
Go
ALTER TABLE [dbo].[EmployeeConceptAnnualLimits] ALTER COLUMN [IDYear] [int] NOT NULL
Go

--Crear de nuevo las constraints con la nueva PK incluida

ALTER TABLE [dbo].[EmployeeConceptAnnualLimits] ADD CONSTRAINT [PK_EmployeeConceptAnnualLimits] PRIMARY KEY  CLUSTERED 
	(
		[IDEmployee],
		[IDConcept],
		[IDYear]
	)  ON [PRIMARY]

Go
ALTER TABLE [dbo].[EmployeeConceptAnnualLimits] ADD CONSTRAINT [FK_EmployeeConceptAnnualLimits_Concepts] FOREIGN KEY 
	(
		[IDConcept]
	) REFERENCES [Concepts] (
		[ID]
	)
Go
ALTER TABLE [dbo].[EmployeeConceptAnnualLimits] ADD CONSTRAINT [FK_EmployeeConceptAnnualLimits_Employees] FOREIGN KEY 
	(
		[IDEmployee]
	) REFERENCES [Employees] (
		[ID]
	)
Go

-- Nuevo campo que guarda la fecha 
ALTER TABLE [dbo].[EmployeeConceptAnnualLimits] ADD [LastUpdated] [smalldatetime] NULL 
Go



-- Nuevo campo para tabla de terminales
alter table terminals add RigidMode tinyint default(0)
GO


--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='183' WHERE ID='DBVersion'
GO



