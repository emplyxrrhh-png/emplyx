/* Insertamos features de puestos */
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (6000 ,Null ,'Assignments' ,'Puestos' ,'' ,'U' ,'RWA' ,NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (6100 ,6000 ,'Assignments.Definition' ,'Definicion puestos' ,'' ,'U' ,'RWA' ,NULL)
GO

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (1570 ,1 ,'Employees.Assignments' ,'Asignar puestos' ,'' ,'U' ,'RW' ,NULL)
GO


/* Insertamos pantalla de puestos */
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,[Type], [Parameters], RequiredFeatures, SecurityFlags, Priority,AllowedSecurity,RequiredFunctionalities)
VALUES('Portal\ShiftControl\Assignments', 'Assignments', 'Assignments/Assignments.aspx',	'Assignment.png'	,NULL	,NULL	,'Feature\HRScheduling',NULL	,665	,NULL	,'U:Assignments.Definition=Read')
GO

/* Damos permisos a la pantalla de puestos */
INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT sysroPassports.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM sysroFeatures, sysroPassports		
WHERE sysroFeatures.ID IN (6000, 6100, 1570) AND
	  sysroPassports.IDParentPassport IS NULL AND sysroPassports.GroupType = 'U'
GO

CREATE TABLE [dbo].[Assignments](
	[ID] [smallint] NOT NULL,
	[Name] [nvarchar](64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ShortName] [nvarchar](3) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Color] [int] NOT NULL,
	[Description] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Assignments] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY] 
GO

CREATE TABLE [dbo].[EmployeeAssignments](
	[IDEmployee] [int] NOT NULL,
	[IDAssignment] [smallint] NOT NULL,
	[Suitability] [smallint] NOT NULL,
 CONSTRAINT [PK_EmployeeAssignments] PRIMARY KEY CLUSTERED 
(
	[IDEmployee] ASC,
	[IDAssignment] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ShiftAssignments](
	[IDShift] [smallint] NOT NULL,
	[IDAssignment] [smallint] NOT NULL,
	[Coverage] [numeric](10, 2) NOT NULL,
 CONSTRAINT [PK_ShiftAssignments] PRIMARY KEY CLUSTERED 
(
	[IDShift] ASC,
	[IDAssignment] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DailyCoverage](
	[IDGroup] [int] NOT NULL,
	[Date] [smalldatetime] NOT NULL,
	[IDAssignment] [smallint] NOT NULL,
	[ExpectedCoverage] [numeric](10, 2) NOT NULL CONSTRAINT [DF_DailyCoverage_ExpectedCoverage]  DEFAULT ((0)),
	[PlannedCoverage] [numeric](10, 2) NULL CONSTRAINT [DF_DailyCoverage_PlannedCoverage]  DEFAULT ((0)),
	[ActualCoverage] [numeric](10, 2) NULL CONSTRAINT [DF_DailyCoverage_ActualCoveraged]  DEFAULT ((0)),
	[ForcedCoveraged] [bit] NULL CONSTRAINT [DF_DailyCoverage_ForcedCoveraged]  DEFAULT ((0)),
	[PlannedStatus] [tinyint] NOT NULL CONSTRAINT [DF_DailyCoverage_Status]  DEFAULT ((0)),
	[ActualStatus] [tinyint] NOT NULL CONSTRAINT [DF_DailyCoverage_ActualStatus]  DEFAULT ((0)),
	[Real] [bit] NULL CONSTRAINT [DF_DailyCoverage_Real]  DEFAULT ((0)),
 CONSTRAINT [PK_DailyCoverage] PRIMARY KEY CLUSTERED 
(
	[IDGroup] ASC,
	[Date] ASC,
	[IDAssignment] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DailySchedule] ADD 
	[IDAssignment] [smallint] NULL,
	[IsCovered] [bit] NULL,
	[OldIDAssignment] [smallint] NULL,
	[OldIDShift] [smallint] NULL,
	[IDEmployeeCovered] [int] NULL
GO

CREATE TABLE [dbo].[EmployeeStatus](
	[IDEmployee] [int] NOT NULL,
	[LastPunch] [smalldatetime] NULL,
	[IDCause] [smallint] NULL,
	[Type] [char](1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[IsPresent] [bit] NULL CONSTRAINT [DF_EmployeeStatus_IsPresent]  DEFAULT ((0)),
	[ShiftDate] [smalldatetime] NULL,
	[BeginMandatory] [smalldatetime] NULL,
	[Verified] [bit] NULL CONSTRAINT [DF_EmployeeStatus_verified]  DEFAULT ((0)),
 CONSTRAINT [PK_EmployeeStatus] PRIMARY KEY CLUSTERED 
(
	[IDEmployee] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE NONCLUSTERED INDEX [_dta_index_DailySchedule_14_503672842__K17_K2_K9] ON [dbo].[DailySchedule] 
(
	[IDAssignment] ASC,
	[Date] ASC,
	[Status] ASC
) ON [PRIMARY]
GO

ALTER TABLE dbo.Requests ADD
	IDAssignment smallint NULL
GO
ALTER TABLE dbo.Requests ADD CONSTRAINT
	FK_Requests_Assignments FOREIGN KEY
	(
	IDAssignment
	) REFERENCES dbo.Assignments
	(
	ID
	) 	 
	
GO

INSERT INTO dbo.sysroNotificationTypes (ID,Name,Scheduler) VALUES(13, 'Employee Absence on coverage', 1)
GO
INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(901, 13, 'Cubrir empleado ausente','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO

INSERT INTO dbo.sysroNotificationTypes (ID,Name,Scheduler) VALUES(14, 'Under Coverage', 1)
GO
INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(902, 14, 'Cobertura planificada insuficiente','<?xml version="1.0"?><roCollection version="2.0"><Item key="MaxDays" type="8">3</Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO

ALTER TABLE [dbo].[sysroNotificationTasks] ADD
	[NextDateTimeExecuted] [datetime] NULL DEFAULT(GETDATE())
GO
UPDATE [dbo].[sysroNotificationTasks] SET [NextDateTimeExecuted] = getdate() WHERE [NextDateTimeExecuted] IS NULL
GO

UPDATE dbo.DailySchedule SET STATUS = 0 WHERE date >= DATEADD(dd, -1, getdate()) and date < getdate()
GO

INSERT INTO dbo.sysroTasks (ProcessID,CreatorID,Context,DateCreated ,ID ,SystemTask ,ExternalTask)  VALUES
('PROC:\\DETECTOR','SESSION:\\SYSTEM','<?xml version="1.0"?><roCollection version="2.0"><Item key="Status" type="8">PENDING</Item><Item key="Command" type="8">ON_UPDATED_SCHEDULE</Item></roCollection>', getdate(),'TASK:\\999999', 0, 0)
GO





-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='257' WHERE ID='DBVersion'
GO
