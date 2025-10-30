--******* MODELO DE DATOS PARA LA APLICACIÓN VISUALTIME VISITAS ********************


CREATE TABLE [VisitTypes] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[ImageURL] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	CONSTRAINT [PK_VisitTypes] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO

CREATE TABLE [Visitors] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[NIF] [nvarchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Company] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Position] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Comments] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Type] [int] NOT NULL ,
	[LastVisitDate] [smalldatetime] NULL ,
	[Created] [smalldatetime] NOT NULL ,
	CONSTRAINT [PK_Visitors] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] ,
	CONSTRAINT [FK_Visitors_VisitTypes] FOREIGN KEY 
	(
		[Type]
	) REFERENCES [VisitTypes] (
		[ID]
	)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [VisitPlan] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[VisitorAlias] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[VisitorId] [int] NULL ,
	[EmpVisitedId] [int] NOT NULL ,
	[Date] [smalldatetime] NOT NULL ,
	[Comments] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[Status] [int] NULL ,
	[PlannedById] [int] NOT NULL ,
	[PlannedDate] [smalldatetime] NOT NULL ,
	[Type] [int] NULL ,
	[Ticket] [int] NULL ,
	CONSTRAINT [PK_VisitPlan] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] ,
	CONSTRAINT [FK_VisitPlan_Employees] FOREIGN KEY 
	(
		[EmpVisitedId]
	) REFERENCES [Employees] (
		[ID]
	),
	CONSTRAINT [FK_VisitPlan_Employees1] FOREIGN KEY 
	(
		[PlannedById]
	) REFERENCES [Employees] (
		[ID]
	),
	CONSTRAINT [FK_VisitPlan_Visitors] FOREIGN KEY 
	(
		[VisitorId]
	) REFERENCES [Visitors] (
		[ID]
	),
	CONSTRAINT [FK_VisitPlan_VisitTypes] FOREIGN KEY 
	(
		[Type]
	) REFERENCES [VisitTypes] (
		[ID]
	)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [VisitMoves] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[VisitPlanId] [int] NOT NULL ,
	[BeginTime] [smalldatetime] NULL ,
	[EndTime] [smalldatetime] NULL ,
	CONSTRAINT [PK_VisitMoves] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] ,
	CONSTRAINT [FK_VisitMoves_VisitPlan1] FOREIGN KEY 
	(
		[VisitPlanId]
	) REFERENCES [VisitPlan] (
		[ID]
	)
) ON [PRIMARY]
GO


-- Nuevos campos para tabla de empleados
alter table Employees add AllowVisits bit default(1)
GO
alter table Employees add AllowVisitsPlan bit default(1)
GO
alter table Employees add AllowVisitsAdmin bit default(0)
GO
alter table Employees add AllowVisitsControl bit default(1)
GO
Update employees set AllowVisits = 1, AllowVisitsPlan = 1, AllowVisitsAdmin = 0, AllowVisitsControl = 1
GO


-- *************************VISTA   SYSROVISITPLAN  **************************************

DROP VIEW sysroVisitPlan
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/****** Object:  View dbo.sysroVisitPlan    Script Date: 05/02/2006 21:17:44 ******/
CREATE VIEW dbo.sysroVisitPlan
AS
SELECT     dbo.Visitors.Name AS VisitorName, dbo.Visitors.NIF AS VisitorNIF, dbo.Employees.Name AS EmployeeVisitedName, dbo.VisitPlan.*, 
                      dbo.VisitMoves.BeginTime AS BeginTime, dbo.VisitMoves.EndTime AS EndTime
FROM         dbo.Employees INNER JOIN
                      dbo.VisitPlan ON dbo.Employees.ID = dbo.VisitPlan.EmpVisitedId LEFT OUTER JOIN
                      dbo.VisitMoves ON dbo.VisitPlan.ID = dbo.VisitMoves.VisitPlanId LEFT OUTER JOIN
                      dbo.Visitors ON dbo.VisitPlan.VisitorId = dbo.Visitors.ID

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- ************************ Modificaciones en auditoria para auditar visitas *************************
Alter Table Audit Add UserType tinyint Default(0)
GO

INSERT INTO sysroAuditScreens (ID,Name) Values (21,'VisitPlan')
GO
INSERT INTO sysroAuditScreens (ID,Name) Values (22,'VisitsResume')
GO
INSERT INTO sysroAuditScreens (ID,Name) Values (23,'Visitors')
GO
INSERT INTO sysroAuditScreens (ID,Name) Values (24,'VisitTypes')
GO

INSERT INTO sysroAuditElements (ID,Name) Values (28,'Visit')
GO
INSERT INTO sysroAuditElements (ID,Name) Values (29,'Visitor')
GO
INSERT INTO sysroAuditElements (ID,Name) Values (30,'VisitType')
GO
INSERT INTO sysroAuditElements (ID,Name) Values (31,'VisitMove_Begin')
GO
INSERT INTO sysroAuditElements (ID,Name) Values (32,'VisitMove_End')
GO
INSERT INTO sysroAuditElements (ID,Name) Values (33,'VisitMove_Cancel')
GO

--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='185' WHERE ID='DBVersion'
GO
