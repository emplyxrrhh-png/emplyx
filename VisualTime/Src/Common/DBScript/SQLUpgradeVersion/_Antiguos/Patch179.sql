-- PANTALLA DE EMPLEADOS LOGIN AUTENTIFICACION DE WINDOWS
ALTER TABLE dbo.Employees ADD WebWindowsSession nvarchar(50) NULL
GO

ALTER TABLE dbo.DailyCauses ADD CauseUser tinyint,CauseUserType tinyint
GO

ALTER TABLE dbo.moves ADD InIDReaderType tinyint,OutIDReaderType tinyint
GO

ALTER TABLE dbo.sysroUsers ADD IDUser tinyint NOT NULL IDENTITY (100, 1)
GO


if exists (select * from dbo.sysobjects where name  = 'FK_Usuarios_GruposSeguridad')
alter table sysroUsers drop constraint FK_Usuarios_GruposSeguridad
GO
if exists (select * from dbo.sysobjects where name  = 'PK_GruposSeguridad')
alter table sysroUserGroups drop constraint PK_GruposSeguridad
GO
alter table sysroUserGroups alter column ID char(2) not null
GO
alter table sysroUsers alter column IDSecurityGroup char(2) not null
GO
alter table sysroUserGroups add SecurityLevel smallint null
GO
UPDATE sysroUserGroups SET SecurityLevel=0
GO
UPDATE Groups SET SecurityFlags=SecurityFlags + '111111111111111111111111111111'
GO
UPDATE SysroGUI SET SecurityFlags=SecurityFlags + '111111111111111111111111111111'
GO


UPDATE  employees set image=null where len(substring(image,1,10))<10 
GO

ALTER TABLE dbo.Shifts ADD WebVisible bit NOT NULL CONSTRAINT DF_Shifts_webVisible DEFAULT 0,webLaboral bit NULL
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_WtRequest_sysroRequestStatus]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[WtRequest] DROP CONSTRAINT FK_WtRequest_sysroRequestStatus
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[FK_WtRequest_sysroRequestType]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[WtRequest] DROP CONSTRAINT FK_WtRequest_sysroRequestType
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WtRequest]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[WtRequest]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sysroRequestLevel]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[sysroRequestLevel]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sysroRequestStatus]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[sysroRequestStatus]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sysroRequestType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[sysroRequestType]
GO

CREATE TABLE [dbo].[WtRequest] ([IdRequest] [int] IDENTITY (1, 1) NOT NULL ,
[IdEmployee] [int] NOT NULL ,
[RequestType] [tinyint] NOT NULL ,
[RequestDate] [datetime] NOT NULL ,
[Description] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
[Status] [tinyint] NOT NULL ,
[StatusLevel] [smallint] NULL ,
[StatusDateTime] [datetime] NULL ,
[StatusUserId] [int] NULL ,
[DateBegin] [datetime] NOT NULL ,
[DateEnd] [datetime] NULL ,
[ShiftId] [smallint] NULL ,
[Cause] [smallint] NULL ,
[InAction] [bit] NULL ,
[Hours] [tinyint] NULL ,
[IdEmployee2] [int] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[sysroRequestLevel] (
[IdLevel] [smallint] NOT NULL ,
[LevelDesc] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[sysroRequestStatus] (
[IdStatus] [tinyint] NOT NULL ,
[Name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[sysroRequestType] (
[IdType] [tinyint] NOT NULL ,
[Type] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[WtRequest] WITH NOCHECK ADD 
CONSTRAINT [PK_WtRequest] PRIMARY KEY  CLUSTERED 
(
[IdRequest]
) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[sysroRequestLevel] WITH NOCHECK ADD 
CONSTRAINT [PK_sysroRequestLevel] PRIMARY KEY  CLUSTERED 
(
[IdLevel]
)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[sysroRequestStatus] WITH NOCHECK ADD 
CONSTRAINT [PK_sysroRequestStatus] PRIMARY KEY  CLUSTERED 
(
[IdStatus]
)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[sysroRequestType] WITH NOCHECK ADD 
CONSTRAINT [PK_sysroRequestType] PRIMARY KEY  CLUSTERED 
(
[IdType]
) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[WtRequest] WITH NOCHECK ADD 
CONSTRAINT [DF_WtRequest_RequestDate] DEFAULT (getdate()) FOR [RequestDate],
CONSTRAINT [DF_WtRequest_Status] DEFAULT (0) FOR [Status]
GO

ALTER TABLE [dbo].[WtRequest] ADD 
CONSTRAINT [FK_WtRequest_sysroRequestStatus] FOREIGN KEY 
(
[Status]
) REFERENCES [dbo].[sysroRequestStatus] (
[IdStatus]
),
CONSTRAINT [FK_WtRequest_sysroRequestType] FOREIGN KEY 
(
[RequestType]
	) REFERENCES [dbo].[sysroRequestType] (
		[IdType]
	)
GO



INSERT INTO sysroRequestLevel ( IdLevel , LevelDesc ) VALUES ( 1 , 'Validador final (Recursos humanos)' )
GO
INSERT INTO sysroRequestLevel ( IdLevel , LevelDesc ) VALUES ( 2 , 'Validador intermediario (Jefe de departamento)' )
GO
INSERT INTO sysroRequestLevel ( IdLevel , LevelDesc ) VALUES ( 3 , 'Validador inicial (Supervisor directo)' )
GO

INSERT INTO sysroRequestStatus ( IdStatus , Name ) VALUES ( 0 , 'Pendiente' )
go
INSERT INTO sysroRequestStatus ( IdStatus , Name ) VALUES ( 1 , 'Aceptado' )
go
INSERT INTO sysroRequestStatus ( IdStatus , Name ) VALUES ( 2 , 'Denegado' )
go

INSERT INTO sysroRequestType ( IdType , Type ) VALUES ( 1 , 'Registro' )
GO
INSERT INTO sysroRequestType ( IdType , Type ) VALUES ( 2 , 'Planificación' )
GO
INSERT INTO sysroRequestType ( IdType , Type ) VALUES ( 3 , 'Registros' )
GO
INSERT INTO sysroRequestType ( IdType , Type ) VALUES ( 4 , 'Horas' )
GO


ALTER TABLE WtRequest ALTER COLUMN  HOURS [numeric](19, 4)
GO

INSERT INTO sysroRequestType (idtype, type) VALUES (5 , 'Intercambio')
GO


--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='179' WHERE ID='DBVersion'
GO
