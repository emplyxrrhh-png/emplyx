-- Eliminamos tablas obsoletas de la aplicación de Tareas
DROP TABLE [dbo].[EmployeeTasks] 
GO
DROP TABLE [dbo].[TasksMoves] 
GO
DROP TABLE [dbo].[Tasks]
GO

-- Se crea un boton personalizado en los terminales RT
alter table Terminals add AllowCustomButton bit null default(0)
GO
UPDATE Terminals Set AllowCustomButton=0  WHERE AllowCustomButton IS NULL
GO
alter table dbo.Terminals add CustomLabel nvarchar(6) null 
GO
alter table dbo.Terminals add CustomOutPut tinyint null  default(0)
GO
alter table dbo.Terminals add CustomDuration tinyint null  default(0)
GO
alter table dbo.Terminals add CustomField nvarchar(50) null  default(0)
GO
alter table dbo.Terminals add CustomFieldValue nvarchar(50) null  default(0)
GO

-- Añadimos un campo nuevo a la base de datos de terminales para el MX6
ALTER TABLE dbo.Terminals ADD ConfData text NULL
GO

-- Se crea un campo para indicar los periodos del modo Consulta del terminal MX6
ALTER TABLE dbo.Employees ADD TerminalQueryPeriod varchar(100) NULL
GO

-- Se crean tablas para capturar las  imagenes del MX6
CREATE TABLE dbo.MovesCaptures
	(
	IDMove numeric(16, 0) NOT NULL,
	InCapture image NULL,
	OutCapture image NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.MovesCaptures ADD CONSTRAINT
	PK_MovesCaptures PRIMARY KEY CLUSTERED 
	(
	IDMove
	) ON [PRIMARY]

GO
ALTER TABLE dbo.MovesCaptures ADD CONSTRAINT
	FK_MovesCaptures_Moves FOREIGN KEY
	(
	IDMove
	) REFERENCES dbo.Moves
	(
	ID
	)
GO

CREATE TABLE dbo.EntriesCaptures
	(
	IDEntry numeric(16, 0) NOT NULL,
	Capture image NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.EntriesCaptures ADD CONSTRAINT
	PK_EntriesCaptures PRIMARY KEY CLUSTERED 
	(
	IDEntry
	) ON [PRIMARY]

GO




UPDATE sysroParameters SET Data='201' WHERE ID='DBVersion'
GO


