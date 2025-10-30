-- Creamos un campo en la tabla Employees para indicar que el empleado puede realizar consultas de movimientos en el terminal

ALTER TABLE [dbo].[Employees] ADD [AllowQueryMovesOnTerminal] [bit] NULL  DEFAULT (1) 
GO

-- Por compatibilidad, ponemos el permiso de consulta de movimientos al valor del permiso general de consultas en terminal
update Employees set AllowQueryMovesOnTerminal = AllowQueriesOnTerminal
GO


-- Se crea nuevo modo en los terminales ANTIPASSBACK
alter table Terminals add AllowAntiPassBack bit null default(0)
GO

UPDATE Terminals Set AllowAntiPassBack=0  WHERE AllowAntiPassBack IS NULL
GO

-- Permitimos que en la pantalla de Opciones, los usuarios de grupos diferentes a 'Admin' también puedan escribir.
update sysroGUI set AllowedSecurity = 'NWR' where IDPath = 'NavBar\Config\Options'
GO

-- Nuevos elementos de auditoría
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(37,'Report'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(38,'ReportProfile'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(39,'ReportServerTask'	,NULL)

-- Nueva funcionalidad servidor de informes
CREATE TABLE [dbo].[ReportsScheduler](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Report] [nvarchar](128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Profile] [int] NOT NULL,
	[Format] [smallint] NOT NULL,
	[Scheduler] [nvarchar](128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Destination] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[EmergencyReport] [bit] NULL,
	[LastDateTimeExecuted] [smalldatetime] NULL,
	[LastDateTimeUpdated] [smalldatetime] NULL,
	[NextDateTimeExecuted] [smalldatetime] NULL,
	[ExecuteTask] [bit] NULL CONSTRAINT [DF_ReportsScheduler_ExecuteTask]  DEFAULT ((0)),
	[IDUser] [tinyint] NULL CONSTRAINT [DF_ReportsScheduler_IDUser]  DEFAULT ((0)),
 CONSTRAINT [PK_ReportsScheduler] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)ON [PRIMARY]
) ON [PRIMARY] 
GO

UPDATE SYSROGUI SET AllowedSecurity='NWR' WHERE IDPath = 'NavBar\FirstTab\ReportsCR'
GO

-- Se crea un campo para indicar la Zona horaria del terminal en caso que sea diferente a la del servidor
ALTER TABLE TERMINALS ADD IsDifferentZoneTime  bit null default(0)
GO
update TERMINALS Set IsDifferentZoneTime  = 0 where IsDifferentZoneTime  is null
GO
ALTER TABLE TERMINALS ADD ZoneTime int null 
GO
UPDATE sysroParameters SET Data='198' WHERE ID='DBVersion'
GO


