CREATE TABLE [dbo].[Punches](								
	[ID] [numeric](16, 0) IDENTITY(1,1) NOT NULL,
	[IDCredential] [numeric](28, 0) NULL DEFAULT (0),							
	[IDEmployee] [int] NULL DEFAULT (0),							
	[Type] [tinyint] NOT NULL,
	[ActualType] [tinyint] NULL,
	[InvalidType] [tinyint] NULL,
	[TypeData] [smallint] NULL,							
	[TypeDetails] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ShiftDate] [smalldatetime] NULL,							
	[DateTime] [datetime] NOT NULL,							
	[IDTerminal] [tinyint] NULL,							
	[IDReader] [tinyint] NULL,							
	[IDZone] [tinyint] NULL,							
	[Location] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,				
	[LocationZone] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,				
	[IP] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsNotReliable] [bit] NULL DEFAULT (0),
	[Action] [tinyint] NULL,							
	[IDPassport] [int] NULL,							
 CONSTRAINT [PK_Punches] PRIMARY KEY NONCLUSTERED 								
(								
	[ID] ASC							
) ON [PRIMARY]								
) ON [PRIMARY]								
GO

CREATE TABLE [dbo].[PunchesCaptures](
	[IDPunch] [numeric](16, 0) NOT NULL,
	[Capture] [image] NULL,
 CONSTRAINT [PK_PunchesCaptures] PRIMARY KEY CLUSTERED 
(
	[IDPunch] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[sysroPunchTypes](
	[ID] [tinyint] NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_sysroPunchTypes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [sysroPunchTypes] values(0, 'Not defined')
GO

INSERT INTO [sysroPunchTypes] values(1, 'In')
GO

INSERT INTO [sysroPunchTypes] values(2, 'Out')
GO

INSERT INTO [sysroPunchTypes] values(3, 'Auto')
GO

INSERT INTO [sysroPunchTypes] values(4, 'Task')
GO

INSERT INTO [sysroPunchTypes] values(5, 'Access Valid')
GO

INSERT INTO [sysroPunchTypes] values(6, 'Access Denied')
GO

INSERT INTO [sysroPunchTypes] values(7, 'Access and Atenndance')
GO

INSERT INTO [sysroPunchTypes] values(8, 'In Denied on geographic zone')
GO

INSERT INTO [sysroPunchTypes] values(9, 'Out Denied on geographic zone')
GO

--CREAMOS LA VISTA DE FICHAJES DE EMPLEADO (EN EL APARTADO DE ACCESOS INDICAMOS LA ZONA)
CREATE view [dbo].[sysrovwEmployeesInOutPunches]
as
SELECT dbo.Punches.IDEmployee, MAX(dbo.Punches.DateTime) AS DateTime, dbo.Punches.IDTerminal AS IDReader, '0' as IDZone, 'IN' AS Status, 'Att' AS MoveType
FROM dbo.Punches 
WHERE (dbo.Punches.ActualType = 1) 
GROUP BY dbo.Punches.IDEmployee, dbo.Punches.IDTerminal
UNION
SELECT dbo.Punches.IDEmployee, MAX(dbo.Punches.DateTime) AS DateTime, dbo.Punches.IDTerminal AS IDReader, '0' as IDZone, 'OUT' AS Status, 'Att' AS MoveType
FROM dbo.Punches 
WHERE (dbo.Punches.ActualType = 2)
GROUP BY dbo.Punches.IDEmployee, dbo.Punches.IDTerminal, dbo.Punches.IDTerminal
UNION
SELECT am.IDEmployee, MAX(am.DateTime) AS DateTime, am.IDTerminal as IDReader, IDZone, CASE zo.IsWorkingZone WHEN 1 THEN 'IN' ELSE 'OUT' END AS Status, 'Acc' AS MoveType
FROM dbo.Punches AS am 
INNER JOIN dbo.Zones AS zo ON am.IDZone = zo.ID 
WHERE (DateTime IS NOT NULL and Type IN(5,7))
GROUP BY am.IDEmployee, am.IDTerminal, IDZone, zo.IsWorkingZone, am.DateTime 
GO

--CREAMOS LA VISTA QUE USARÁ EL INFORME DE EMERGENCIA DE ACCESOS
CREATE view [dbo].[sysrovwCurrentEmployeesAccessStatusPunches]
 as
 SELECT DISTINCT IDEmployee, EmployeeName,
  (SELECT TOP 1 DateTime FROM sysrovwEmployeesInOutPunches si WHERE si.IDEmployee = sc.IDEmployee AND Status='IN' AND MoveType='Acc' ORDER BY DateTime DESC) AS DateTime,
  (SELECT TOP 1 IDReader FROM sysrovwEmployeesInOutPunches si WHERE si.IDEmployee = sc.IDEmployee AND Status='IN' AND MoveType='Acc' ORDER BY DateTime DESC) AS IDReader,
  (SELECT TOP 1 IDZone FROM sysrovwEmployeesInOutPunches si WHERE si.IDEmployee = sc.IDEmployee AND Status='IN' AND MoveType='Acc' ORDER BY DateTime DESC) AS IDZone,  
  (SELECT TOP 1 Status FROM sysrovwEmployeesInOutPunches si WHERE si.IDEmployee = sc.IDEmployee AND Status='IN' AND MoveType='Acc' ORDER BY DateTime DESC) AS Status
FROM dbo.sysrovwCurrentEmployeeGroups sc
GO

CREATE view [dbo].[sysrovwCurrentEmployeesPresenceStatusPunches]
  as
  SELECT DISTINCT IDEmployee, EmployeeName,
	(SELECT TOP 1 DateTime FROM sysrovwEmployeesInOutPunches si WHERE si.IDEmployee = sc.IDEmployee ORDER BY DateTime DESC) AS DateTime,
	(SELECT TOP 1 IDReader FROM sysrovwEmployeesInOutPunches si WHERE si.IDEmployee = sc.IDEmployee ORDER BY DateTime DESC) AS IDReader,
	(SELECT TOP 1 MoveType FROM sysrovwEmployeesInOutPunches si WHERE si.IDEmployee = sc.IDEmployee ORDER BY DateTime DESC) AS MoveType,
	(SELECT TOP 1 IDZone FROM sysrovwEmployeesInOutPunches si WHERE si.IDEmployee = sc.IDEmployee AND MoveType='Acc' ORDER BY DateTime DESC) AS IDZone,
	(SELECT TOP 1 Status FROM sysrovwEmployeesInOutPunches si WHERE si.IDEmployee = sc.IDEmployee ORDER BY DateTime DESC) AS Status
FROM dbo.sysrovwCurrentEmployeeGroups sc
GO

CREATE view [dbo].[sysrovwVisitsInOutPunches] 
AS
SELECT vp.EmpVisitedId AS IDEmployee, MAX(vm.BeginTime) AS DateTime,
(SELECT top 1 sc.IDReader FROM dbo.sysrovwEmployeesInOutPunches sc WHERE vp.EmpVisitedId = sc.IDEmployee ORDER BY sc.DateTime DESC) AS IDReader,
'IN' AS Status, vs.ID AS VisitorID, vs.Name AS Visitor
FROM dbo.VisitPlan AS vp
	INNER JOIN dbo.Visitors AS vs ON vs.ID = vp.VisitorId 
	INNER JOIN dbo.VisitMoves AS vm ON vm.VisitPlanId = vp.ID
WHERE (vm.BeginTime IS NOT NULL)
GROUP BY vp.EmpVisitedId, vs.Name, vp.PlannedById, vs.ID
UNION
SELECT vp.EmpVisitedId AS IDEmployee, MAX(vm.EndTime) AS DateTime,
(SELECT top 1 sc.IDReader FROM dbo.sysrovwEmployeesInOutPunches sc WHERE vp.EmpVisitedId = sc.IDEmployee ORDER BY sc.DateTime DESC) AS IDReader,
'OUT' AS Status, vs.ID AS VisitorID, vs.Name AS Visitor
FROM dbo.VisitPlan AS vp 
	INNER JOIN dbo.Visitors AS vs ON vs.ID = vp.VisitorId 
	INNER JOIN dbo.VisitMoves AS vm ON vm.VisitPlanId = vp.ID
WHERE (vm.EndTime IS NOT NULL)
GROUP BY vp.EmpVisitedId, vs.Name, vp.PlannedById, vs.ID
GO

CREATE view [dbo].[sysrovwIncompletedDays] 
AS
SELECT Employees.Name, Punches.IDEmployee, Punches.ShiftDate AS Date
FROM Employees,  Punches
WHERE Employees.ID = Punches.IDEmployee 
 AND (ActualType IN(1,2)) 
 GROUP BY Punches.ShiftDate, Employees.Name, Punches.IDEmployee
 HAVING FLOOR(count(*)/2.00) - (count(*)/2.00) <> 0
GO


CREATE TABLE [dbo].[Tasks](
	[ID] [int] NOT NULL,
	[IDCenter] [smallint] NOT NULL DEFAULT (0),
	[Name] [nvarchar](75) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ShortName] [nvarchar](4) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, 
	[Description] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Status] [tinyint] NOT NULL DEFAULT (0),
	[Color] [int] NOT NULL DEFAULT (0),
	[Project] [nvarchar](75) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Tag] [nvarchar](75) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Priority] [int] NOT NULL DEFAULT (0),	
	[IDPassport] [int] NOT NULL DEFAULT (0),	
	[ExpectedStartDate] [smalldatetime] NULL,
	[ExpectedEndDate] [smalldatetime] NULL,
	[StartDate] [smalldatetime] NULL,
	[EndDate] [smalldatetime] NULL,
	[InitialTime] [numeric](18, 6) NOT NULL DEFAULT (0),
	[TimeChangedRequirements] [numeric](18, 6) NOT NULL DEFAULT (0),
	[ForecastErrorTime] [numeric](18, 6) NOT NULL DEFAULT (0),
	[NonProductiveTimeIncidence] [numeric](18, 6) NOT NULL DEFAULT (0),
	[EmployeeTime] [numeric](18, 6) NOT NULL DEFAULT (0),
	[TeamTime] [numeric](18, 6) NOT NULL DEFAULT (0),
	[MaterialTime] [numeric](18, 6) NOT NULL DEFAULT (0),
	[OtherTime] [numeric](18, 6) NOT NULL DEFAULT (0),
	[TypeCollaboration] [tinyint] NULL DEFAULT (0),
	[ModeCollaboration] [tinyint] NULL DEFAULT (0),
	[TypeActivation] [tinyint] NULL DEFAULT (0),
	[ActivationTask] [int] NULL DEFAULT (0),
	[ActivationDate] [smalldatetime] NULL,
	[TypeClosing] [tinyint] NULL DEFAULT (0),
	[ClosingDate] [smalldatetime] NULL,
	[TypeAuthorization] [tinyint] NULL DEFAULT (0),
	[TimeRemarks] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Tasks] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[EmployeeTasks](
	[IDEmployee] [int] NOT NULL,
	[IDTask] [int]  NOT NULL,
 CONSTRAINT [PK_EmployeeTasks] PRIMARY KEY CLUSTERED 
(
	[IDEmployee] ASC,
	[IDTask] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[GroupTasks](
	[IDGroup] [int] NOT NULL,
	[IDTask] [int]  NOT NULL,
 CONSTRAINT [PK_GroupTasks] PRIMARY KEY CLUSTERED 
(
	[IDGroup] ASC,
	[IDTask] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[DailyTaskAccruals](
	[IDEmployee] [int] NOT NULL,
	[IDTask] [int] NOT NULL,
	[Date] [smalldatetime] NOT NULL,
	[Value] [numeric](8, 6) NOT NULL,
 CONSTRAINT [PK_DailyTaskAccruals] PRIMARY KEY NONCLUSTERED 
(
	[IDEmployee] ASC,
	[IDTask] ASC,
	[Date] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE dbo.DailySchedule ADD TaskStatus [tinyint] not null DEFAULT(80)
GO

ALTER TABLE dbo.Punches ALTER COLUMN TypeData [int] null
GO

INSERT INTO dbo.Tasks (ID,Name,ShortName,Color) values(0,'Sin tarea','SIN',16777215)
GO

INSERT INTO dbo.sysroFeatures (ID,Alias,Name,Description, Type,PermissionTypes, AppHasPermissionsOverEmployees) 
	VALUES (25,'Tasks', 'Tareas','','U','RWA',1)
GO

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (25100 ,25 ,'Tasks.Definition' ,'Definicion tareas' ,'' ,'U' ,'RWA' ,NULL)
GO

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (25200 ,25 ,'Tasks.Punches' ,'Fichajes' ,'' ,'U' ,'R' ,NULL)
GO

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (25300 ,25 ,'Tasks.Analytics' ,'Analítica de tareas' ,'' ,'U' ,'R' ,NULL)
GO


INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,[Type], [Parameters], RequiredFeatures, SecurityFlags, Priority,AllowedSecurity,RequiredFunctionalities)
VALUES('Portal\Task', 'Task', null ,	'Task.png'	,NULL	,NULL	,'Feature\Productiv',NULL	,1003	,NULL	,'U:Tasks.Definition=Read')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,[Type], [Parameters], RequiredFeatures, SecurityFlags, Priority,AllowedSecurity,RequiredFunctionalities)
VALUES('Portal\Task\Tasks', 'Task', 'Tasks/Tasks.aspx',	'Task.png'	,NULL	,NULL	,'Feature\Productiv',NULL	,670	,NULL	,'U:Tasks.Definition=Read')
GO

INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,[Type], [Parameters], RequiredFeatures, SecurityFlags, Priority,AllowedSecurity,RequiredFunctionalities)
VALUES('Portal\Task\Analytics', 'Analytics', 'Tasks/Analytics.aspx',	'TaskAnalytics.png'	,NULL	,NULL	,'Feature\Productiv',NULL	,680	,NULL	,'U:Tasks.Definition=Read')
GO

CREATE TABLE [dbo].[BusinessCenters](
	[ID] [smallint] NOT NULL,
	[Name] [nvarchar](64) NOT NULL,
	[Description] [ntext] NULL,
 CONSTRAINT [PK_BusinessCenters] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY] 
GO

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (25400 ,25 ,'Tasks.BusinessCenters' ,'Centros de Coste' ,'' ,'U' ,'RWA' ,NULL)
GO


INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,[Type], [Parameters], RequiredFeatures, SecurityFlags, Priority,AllowedSecurity,RequiredFunctionalities)
VALUES('Portal\Task\BusinessCenters', 'BusinessCenters', 'Tasks/BusinessCenters.aspx',	'BusinessCenters.png'	,NULL	,NULL	,'Feature\Productiv',NULL	,690	,NULL	,'U:Tasks.Definition=Read')
GO


INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT sysroPassports.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM sysroFeatures, sysroPassports		
WHERE sysroFeatures.ID IN (25, 25100, 25200, 25300 , 25400) AND
	  sysroPassports.IDParentPassport IS NULL AND sysroPassports.GroupType = 'U'
GO

INSERT INTO sysroPassports_PermissionsOverGroups (IDPassport, IDGroup, IDApplication, Permission)
SELECT sysroPassports.ID, Groups.ID, sysroFeatures.ID, 6
FROM sysroFeatures, Groups, sysroPassports
WHERE sysroFeatures.ID IN (25) AND sysroFeatures.AppHasPermissionsOverEmployees = 1 AND Groups.Path = CONVERT(varchar, Groups.ID) AND
	  sysroPassports.GroupType = 'U'
GO


CREATE TABLE [dbo].[sysroPassports_Centers](
	[IDPassport] [int] NOT NULL,
	[IDCenter] [smallint] NOT NULL,
 CONSTRAINT [PK_sysroPassports_Centers] PRIMARY KEY CLUSTERED 
(
	[IDPassport] ASC,
	[IDCenter] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE VIEW sysroCurrentTasksStatus
AS
SELECT     dbo.Tasks.ID, dbo.BusinessCenters.Name as Center, dbo.BusinessCenters.ID as IDCenter , dbo.Tasks.Name, dbo.Tasks.Status, dbo.Tasks.Color, dbo.Tasks.Project, dbo.Tasks.Tag, dbo.Tasks.Priority, dbo.Tasks.ActivationTask, Tasks_1.Color AS ColorTaskPrev,
			convert(numeric(25, 6), isnull(dbo.Tasks.InitialTime, 0) + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0)
				+ isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0)
					+ isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) AS Duration,
		   (SELECT isnull(convert(numeric(25, 6),SUM(Value)), 0) FROM DailyTaskAccruals WHERE DailyTaskAccruals.IDTask = dbo.Tasks.ID)   AS Worked, 
		   
			CASE WHEN convert(numeric(25, 6), isnull(dbo.Tasks.InitialTime, 0) + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0)
				+ isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0)
					+ isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) > 0 then 
		   
			case when (SELECT isnull(convert(numeric(25, 6),SUM(Value)), 0) FROM DailyTaskAccruals WHERE DailyTaskAccruals.IDTask = dbo.Tasks.ID) * 100 / 
			convert(numeric(25, 6), isnull(dbo.Tasks.InitialTime, 0) + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0)
				+ isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0)
					+ isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0))  > 100.00 then 100 else 
			(SELECT isnull(convert(numeric(25, 6),SUM(Value)), 0) FROM DailyTaskAccruals WHERE DailyTaskAccruals.IDTask = dbo.Tasks.ID) * 100 / 
			convert(numeric(25, 6), isnull(dbo.Tasks.InitialTime, 0) + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0)
				+ isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0)
					+ isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) END
								ELSE 
									0 
								END AS Progress, 
			CASE WHEN convert(numeric(25, 6), isnull(dbo.Tasks.InitialTime, 0) + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0)
				+ isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0)
					+ isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) > 0 then 
		   
			case when (SELECT isnull(convert(numeric(25, 6),SUM(Value)), 0) FROM DailyTaskAccruals WHERE DailyTaskAccruals.IDTask = dbo.Tasks.ID) * 100 / 
			convert(numeric(25, 6), isnull(dbo.Tasks.InitialTime, 0) + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0)
				+ isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0)
					+ isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0))  > 100.00 THEN 1 ELSE 0 END
							ELSE 
								0 
							END AS Exceeded, 

		   dbo.Tasks.IDPassport, 
           dbo.Tasks.ExpectedStartDate, 
		   dbo.Tasks.ExpectedEndDate, Tasks_1.Name AS NameTaskPrev, dbo.Tasks.ShortName, 

		   convert(nvarchar(200), (SELECT COUNT(DISTINCT(LastPunch.IDEMPLOYEE)) FROM (SELECT ID as IDEmployee,(select top 1 Typedata from punches where actualtype = 4 and punches.idemployee = employees.id order by datetime desc) as IDTask from employees) LastPunch , EmployeeStatus WHERE LastPunch.IDTask = Tasks.ID and LastPunch.IDEmployee = EmployeeStatus.IDEmployee and (LastPunch >= dateadd(day,-4,getdate())) )) + '@' + 

		   convert(nvarchar(200), (
				SELECT COUNT(DISTINCT(LastPunch.IDEMPLOYEE)) 
								FROM (SELECT ID as IDEmployee ,(select top 1 DateTime from punches where actualtype = 4 and punches.idemployee = employees.id 
										and punches.TypeData = Tasks.id  order by datetime desc) as PunchDate from employees ) 
										LastPunch , EmployeeStatus WHERE  LastPunch.IDEmployee = EmployeeStatus.IDEmployee and (LastPunch.PunchDate > dateadd(day,-7,getdate()) and PunchDate is not null and LastPunch.IDEmployee not in (SELECT DISTINCT(LastPunch.IDEMPLOYEE) FROM (SELECT ID as IDEmployee,(select top 1 Typedata from punches where actualtype = 4 and punches.idemployee = employees.id order by datetime desc) as IDTask from employees) LastPunch , EmployeeStatus WHERE LastPunch.IDTask = Tasks.ID and LastPunch.IDEmployee = EmployeeStatus.IDEmployee and (LastPunch >= dateadd(day,-4,getdate()))))))  AS Employees




FROM       dbo.Tasks LEFT OUTER JOIN
           dbo.Tasks AS Tasks_1 ON dbo.Tasks.ActivationTask = Tasks_1.ID INNER JOIN dbo.BusinessCenters ON dbo.Tasks.IDCenter = dbo.BusinessCenters.ID 
GO


CREATE VIEW dbo.sysroTasksCube
AS
SELECT dbo.sysroEmployeeGroups.IDGroup, sysroEmployeeGroups.FullGroupName as GroupName, dbo.Employees.ID as IDEmployee, dbo.Employees.Name as EmployeeName, datepart(year,dbo.DailyTaskAccruals.Date) as Date_Year, datepart(month,dbo.DailyTaskAccruals.Date) as Date_Month, datepart(day,dbo.DailyTaskAccruals.Date) as Date_Day ,  dbo.DailyTaskAccruals.Date,  dbo.BusinessCenters.ID as IDCenter,  dbo.BusinessCenters.Name as CenterName, dbo.Tasks.ID as IDTask, dbo.Tasks.Name as TaskName, isnull(convert(numeric(25, 6),dbo.DailyTaskAccruals.Value), 0)  as Value, isnull(dbo.Tasks.Project, '') as Project, isnull(dbo.Tasks.Tag, '') as Tag, Tasks.IDPassport, case when Tasks.Status = 0 then 'Activa' ELSE CASE WHEN Tasks.Status = 1 THEN 'Finalizada' ELSE 'Cancelada' END END as Estado
	FROM dbo.Employees INNER JOIN dbo.DailyTaskAccruals on dbo.DailyTaskAccruals.IDEmployee = dbo.Employees.ID INNER JOIN dbo.Tasks on dbo.Tasks.ID = dbo.DailyTaskAccruals.IDTask INNER JOIN dbo.sysroEmployeeGroups on dbo.sysroEmployeeGroups.IDEmployee= dbo.DailyTaskAccruals.IDEmployee INNER JOIN dbo.BusinessCenters ON dbo.Tasks.IDCenter = dbo.BusinessCenters.ID 
		WHERE dbo.DailyTaskAccruals.Date Between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate 
GO

ALTER TABLE sysroPassports add CubeLayout [TEXT] NULL
GO

ALTER PROCEDURE [dbo].[Visits_Employee_IsIn]
   	@employeeId int
   AS
   	DECLARE @lastAccessDate smalldatetime;
   	DECLARE @lastPunchIn smalldatetime;
   	DECLARE @lastPunchOut smalldatetime;
  	DECLARE @isWorkingZone bit;
 	DECLARE @isLivePunches bit;
 	if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Punches]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
 		BEGIN
 			set @isLivePunches = 1
 		END
 	ELSE
 		BEGIN
 			set @isLivePunches = 0
 		END
     if (@isLivePunches = 0)
 		begin
 			 -- ULTIMO FICHAJE DE ACCESOS
 			Select Top 1 @lastAccessDate = DateTime, @isWorkingZone = IsWorkingZone  From AccessMoves,Zones, sysrosubvwCurrentEmployeePeriod Where AccessMoves.IDZone = Zones.ID And AccessMoves.IDEmployee = str(@employeeId) And AccessMoves.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee  Order by DateTime desc
 			 -- ULTIMO FICHAJE DE ENTRADA DE PRESENCIA
 			Select Top 1 @lastPunchIn = InDateTime From Moves, sysrosubvwCurrentEmployeePeriod Where Moves.IDEmployee = str(@employeeId)  AND Moves.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee Order by InDateTime desc
 			 -- ULTIMO FICHAJE DE SALIDA DE PRESENCIA
 			Select Top 1 @lastPunchOut = OutDateTime From Moves, sysrosubvwCurrentEmployeePeriod Where Moves.IdEmployee = str(@employeeId)  AND Moves.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee Order by OutDateTime desc
 		end
 	else
 		BEGIN
 			 -- ULTIMO FICHAJE DE ACCESOS
 			Select Top 1 @lastAccessDate = DateTime, @isWorkingZone = IsWorkingZone  From Punches,Zones, sysrosubvwCurrentEmployeePeriod Where Punches.IDZone = Zones.ID And Punches.IDEmployee = str(@employeeId) And Punches.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee AND Type = 5 Order by DateTime desc
 			 -- ULTIMO FICHAJE DE ENTRADA DE PRESENCIA
 			Select Top 1 @lastPunchIn = DateTime From Punches, sysrosubvwCurrentEmployeePeriod Where Punches.IDEmployee = str(@employeeId)  AND Punches.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee AND Type = 1 Order by DateTime desc
 			 -- ULTIMO FICHAJE DE SALIDA DE PRESENCIA
 			Select Top 1 @lastPunchOut = DateTime From Punches, sysrosubvwCurrentEmployeePeriod Where Punches.IdEmployee = str(@employeeId)  AND Punches.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee AND Type = 2 Order by DateTime desc
 		END
 			
  	
	IF ISDATE(@lastAccessDate) <> 1 and ISDATE(@lastPunchIn) <> 1 and ISDATE(@lastPunchIn) <> 1
  		-- Si no hay ningún fichaje de ningún tipo, estoy ausente
  		BEGIN
  			RETURN 0
  		END
  	ELSE
  		IF ISDATE(@lastAccessDate) <> 1 set @lastAccessDate = convert(smalldatetime,'1900-01-01 00:00',120)
  		IF ISDATE(@lastPunchIn) <> 1 set @lastPunchIn = convert(smalldatetime,'1900-01-01 00:00',120)
  		IF ISDATE(@lastPunchOut) <> 1 set @lastPunchOut = convert(smalldatetime,'1900-01-01 00:00',120)
  		-- CALCULOS
   		IF @lastPunchIn >= @lastPunchOut and @lastPunchIn >= @lastAccessDate
   			-- Si lo último es una entrada de presencia ... estoy presente
  			BEGIN
  				RETURN 1
  			END
   		ELSE
  			BEGIN
  				IF @lastPunchOut >= @lastPunchIn and @lastPunchOut >= @lastAccessDate 
  					-- Si lo último es una salida de presencia ... estoy ausente
  					BEGIN
  						RETURN 0
  					END
  				ELSE
  					BEGIN
   						IF @lastAccessDate >= @lastPunchOut and @lastAccessDate >= @lastPunchIn and @isWorkingZone = 1 
  							-- Si lo último es un fichaje de accesos y es a una zona de trabajo ... estoy presente
  							BEGIN
  								RETURN 1
  							END
   						ELSE 
  							BEGIN
  								-- Si lo último es un fichaje de accesos y es a una zona de NO trabajo ... estoy ausente
  								RETURN 0
  							END
  					END
  			END

GO

-- Campo en tabla Punches para marcar fichjes exportados
ALTER TABLE dbo.Punches ADD Exported [bit] not null DEFAULT(0)
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='285' WHERE ID='DBVersion'
GO
