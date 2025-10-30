ALTER TABLE [dbo].[Employees] ADD Telecommuting BIT NOT NULL DEFAULT 0;
GO

ALTER TABLE [dbo].[Employees] ADD TelecommutingDays nvarchar(100) NOT NULL DEFAULT '';
GO

ALTER TABLE [dbo].[Zones]
ADD [MapInfo] NVARCHAR(MAX) NULL
GO

ALTER TABLE [dbo].[Zones]
ADD [Area] FLOAT NULL
GO

ALTER TABLE [dbo].[Zones]
ADD [Capacity] INT NULL
GO

ALTER TABLE [dbo].[Zones]
ADD [CapacityVisible] BIT NULL
GO

ALTER TABLE [dbo].[Zones]
ADD [IsEmergencyZone] BIT NULL
GO

ALTER TABLE [dbo].[Zones]
ADD [ZoneNameAsLocation] BIT NULL

ALTER TABLE [dbo].[Zones]
ADD [WorkCenter] NVARCHAR(50) NULL

ALTER TABLE [dbo].[DailySchedule]
ADD [WorkCenter] NVARCHAR(50) NULL

ALTER TABLE [dbo].[DailySchedule]
ADD [Telecommuting] BIT NULL

ALTER TABLE [dbo].[Punches]
ADD [InTelecommute] BIT NOT NULL DEFAULT 0
GO

ALTER TABLE [dbo].[Punches]
ADD [WorkCenter] NVARCHAR(50) NULL
GO

ALTER TABLE [dbo].[Punches]
ADD [TimeSpan] [numeric](18, 0) NULL
GO

DROP FUNCTION IF EXISTS [dbo].[EmployeeWorkCenterBetweenDates]
 GO
 CREATE FUNCTION [dbo].[EmployeeWorkCenterBetweenDates]
   (				
 	@datebeginpar smalldatetime,
 	@dateendpar smalldatetime,
	@employeeidlist nvarchar(max)
   )
   RETURNS @ValueTable table(IDEmployee int, IDContract NVARCHAR(50), TelecommutingExpected BIT, TelecommutePlanned BIT, InTelecommute BIT, RefDate smalldatetime, ContractWorkCenter NVARCHAR(50), 
							 DailyWorkCenter NVARCHAR(50), CalculatedWorkCenter NVARCHAR(50), InAbsence BIT, NoWork BIT) 
   AS
   BEGIN
    declare @iniperiod smalldatetime 
 	declare @endperiod smalldatetime
	DECLARE @employees nvarchar(max)
	DECLARE @employeeIDs Table(idEmployee int)
 	SET @iniperiod = @datebeginpar
 	SET @endperiod = @dateendpar
	SET @employees = @employeeidlist

	IF LEN(@employees) = 0 
		INSERT INTO @ValueTable
		SELECT Employees.Id AS IDEmployee,
			   EmployeeContracts.IDContract,
			   CASE WHEN (Employees.Telecommuting = 1 AND ISNULL(Employees.TelecommutingDays,'') = '') THEN 1 ELSE (CASE WHEN CHARINDEX(CONVERT(VARCHAR,DATEPART(weekday, dt) - 1), TelecommutingDays) > 0 THEN 1 ELSE 0 END) END AS TelecommutingExpected,
			   DailySchedule.Telecommuting AS TelecommutePlanned,
			   ISNULL(PunchesTC.ID,0) InTelecommute,
			   dt AS [RefDate],
			   ISNULL(EmployeeContracts.Enterprise,'') as ContractWorkCenter,
			   ISNULL(DailySchedule.WorkCenter,'') as DailyWorkCenter,
			   CASE WHEN DailySchedule.WorkCenter IS NOT NULL THEN ISNULL(DailySchedule.WorkCenter,'') ELSE ISNULL(EmployeeContracts.Enterprise,'') END AS CalculatedWorkCenter,
			   CASE WHEN ProgrammedAbsences.IDEmployee IS NULL THEN 0 ELSE 1 END AS InAbsence,
			   CASE WHEN Shifts.ID IS NULL OR Shifts.ShiftType = 2 OR Shifts.ExpectedWorkingHours = 0 THEN 1 ELSE 0 END AS NoWork
		FROM   [dbo].[Alldays] (@iniperiod, @endperiod)
			   LEFT JOIN EmployeeContracts WITH(NOLOCK) ON alldays.dt BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate
			   INNER JOIN Employees WITH(NOLOCK) ON Employees.ID = EmployeeContracts.IDEmployee
			   LEFT JOIN DailySchedule WITH(NOLOCK) ON DailySchedule.Date = alldays.dt AND DailySchedule.IDEmployee = EmployeeContracts.IDemployee
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Shiftdate ORDER BY DateTime DESC) AS 'RowNumberTC', * FROM Punches WITH(NOLOCK) WHERE InTelecommute = 1) PunchesTC ON PunchesTC.IDEmployee = EmployeeContracts.IDemployee AND PunchesTC.ShiftDate = dt AND (RowNumberTC = 1 OR RowNumberTC IS NULL)
			   LEFT JOIN ProgrammedAbsences WITH(NOLOCK) ON ProgrammedAbsences.IDEmployee = EmployeeContracts.IDEmployee AND dt BETWEEN ProgrammedAbsences.BeginDate AND isnull(ProgrammedAbsences.FinishDate,DATEADD(day,ProgrammedAbsences.maxlastingdays,ProgrammedAbsences.BeginDate))
			   LEFT JOIN Shifts WITH(NOLOCK) ON Shifts.ID = DailySchedule.IDShift1
	ELSE
   		INSERT INTO @ValueTable
		SELECT Employees.Id AS IDEmployee,
			   EmployeeContracts.IDContract,
			   CASE WHEN (Employees.Telecommuting = 1 AND ISNULL(Employees.TelecommutingDays,'') = '') THEN 1 ELSE (CASE WHEN CHARINDEX(CONVERT(VARCHAR,DATEPART(weekday, dt) - 1), TelecommutingDays) > 0 THEN 1 ELSE 0 END) END AS TelecommutingExpected,
			   DailySchedule.Telecommuting AS TelecommutePlanned,
			   ISNULL(PunchesTC.ID,0) InTelecommute,
			   dt AS [RefDate],
			   ISNULL(EmployeeContracts.Enterprise,'') as ContractWorkCenter,
			   ISNULL(DailySchedule.WorkCenter,'') as DailyWorkCenter,
			   CASE WHEN DailySchedule.WorkCenter IS NOT NULL THEN ISNULL(DailySchedule.WorkCenter,'') ELSE ISNULL(EmployeeContracts.Enterprise,'') END AS CalculatedWorkCenter,
			   CASE WHEN ProgrammedAbsences.IDEmployee IS NULL THEN 0 ELSE 1 END AS InAbsence,
			   CASE WHEN Shifts.ID IS NULL OR Shifts.ShiftType = 2 OR Shifts.ExpectedWorkingHours = 0 THEN 1 ELSE 0 END AS NoWork
		FROM   [dbo].[Alldays] (@iniperiod, @endperiod)
			   LEFT JOIN EmployeeContracts WITH(NOLOCK) ON alldays.dt BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate
			   INNER JOIN Employees WITH(NOLOCK) ON Employees.ID = EmployeeContracts.IDEmployee
			   LEFT JOIN DailySchedule WITH(NOLOCK) ON DailySchedule.Date = alldays.dt AND DailySchedule.IDEmployee = EmployeeContracts.IDemployee
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Shiftdate ORDER BY DateTime DESC) AS 'RowNumberTC', * FROM Punches WITH(NOLOCK) WHERE InTelecommute = 1) PunchesTC ON PunchesTC.IDEmployee = EmployeeContracts.IDemployee AND PunchesTC.ShiftDate = dt AND (RowNumberTC = 1 OR RowNumberTC IS NULL)
			   LEFT JOIN ProgrammedAbsences WITH(NOLOCK) ON ProgrammedAbsences.IDEmployee = EmployeeContracts.IDEmployee AND dt BETWEEN ProgrammedAbsences.BeginDate AND isnull(ProgrammedAbsences.FinishDate,DATEADD(day,ProgrammedAbsences.maxlastingdays,ProgrammedAbsences.BeginDate))
			   LEFT JOIN Shifts WITH(NOLOCK) ON Shifts.ID = DailySchedule.IDShift1
        WHERE EmployeeContracts.IDEmployee IN (SELECT Value AS IDEmployye FROM SplitToInt(@employees,','))
	RETURN
   END
GO

-- Nueva notificación de cambio en estado de teletrabajo
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 75)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,OnlySystem) VALUES(75,'Advice for change in telecommuting',null, 1, 1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notifications] WHERE ID = 1300)
	INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal) VALUES(1300, 75, 'Aviso de cambio en teletrabajo','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',1)
GO

-- Vista de tiempos efectivos
DROP VIEW IF EXISTS [dbo].[sysrovwDailyTelecommutingAccruals]
GO
CREATE VIEW [dbo].[sysrovwDailyTelecommutingAccruals]
 AS
SELECT IdEmployee, ShiftDate Date, SUM(Value)/3600 AS Value
FROM (
	SELECT Avg(CAST(ActualType AS DECIMAL(10,2))) OVER (partition by Punches.IdEmployee, Punches.ShiftDate order by Punches.IdEmployee asc, Punches.DateTime asc ROWS BETWEEN CURRENT ROW AND 1 FOLLOWING) AS 'MediaSiguienteDelDia',
			Avg(CAST(ActualType AS DECIMAL(10,2))) OVER (partition by Punches.IdEmployee, Punches.ShiftDate order by Punches.IdEmployee asc, Punches.DateTime asc ROWS 1 PRECEDING) AS 'MediaAnteriorDelDia',
				SUM(Punches.TimeSpan) OVER (partition by Punches.IdEmployee, Punches.ShiftDate ORDER BY Punches.IdEmployee ASC, Punches.DateTime ASC ROWS BETWEEN CURRENT ROW AND 1 FOLLOWING) AS 'Value',
				Punches.IdEmployee, Employees.Name, Punches.ActualType, Punches.DateTime, Punches.ShiftDate, Punches.InTelecommute
	FROM PUNCHES 
	INNER JOIN Employees ON Employees.Id = Punches.IdEmployee And Punches.ActualType IN (1,2)
	LEFT OUTER JOIN DailySchedule ON DailySchedule.IdEmployee = Employees.Id AND DailySchedule.Date = Punches.ShiftDate
	WHERE Employees.Id = Punches.IdEmployee
) TMP 
where TMP.ActualType = 1 AND InTelecommute = 1--=> Sólo contemplo entradas (y sumo con el siguiente, que deberá ser una salida) ... 
AND TMP.MediaAnteriorDelDia <> 2 --=> O bien es el primer fichaje del día y es una entrada (1), o bien el fichaje anterior tiene el sentido correcto (1,5)
AND TMP.MediaSiguienteDelDia = 1.5 --=> No es el último fichaje del día, y el siguiente tiene el sentido correcto
GROUP BY IDEmployee, ShiftDate
GO

DROP VIEW IF EXISTS [dbo].[sysrovwDailyEfectiveWorkingHours]
GO
CREATE VIEW [dbo].[sysrovwDailyEfectiveWorkingHours]
 AS
SELECT IdEmployee, ShiftDate Date, SUM(Value)/3600 AS Value, InTelecommute
FROM (
	SELECT Avg(CAST(ActualType AS DECIMAL(10,2))) OVER (partition by Punches.IdEmployee, Punches.ShiftDate order by Punches.IdEmployee asc, Punches.DateTime asc ROWS BETWEEN CURRENT ROW AND 1 FOLLOWING) AS 'MediaSiguienteDelDia',
			Avg(CAST(ActualType AS DECIMAL(10,2))) OVER (partition by Punches.IdEmployee, Punches.ShiftDate order by Punches.IdEmployee asc, Punches.DateTime asc ROWS 1 PRECEDING) AS 'MediaAnteriorDelDia',
				SUM(Punches.TimeSpan) OVER (partition by Punches.IdEmployee, Punches.ShiftDate ORDER BY Punches.IdEmployee ASC, Punches.DateTime ASC ROWS BETWEEN CURRENT ROW AND 1 FOLLOWING) AS 'Value',
				Punches.IdEmployee, Employees.Name, Punches.ActualType, Punches.DateTime, Punches.ShiftDate, Punches.InTelecommute
	FROM PUNCHES 
	INNER JOIN Employees ON Employees.Id = Punches.IdEmployee And Punches.ActualType IN (1,2)
	LEFT OUTER JOIN DailySchedule ON DailySchedule.IdEmployee = Employees.Id AND DailySchedule.Date = Punches.ShiftDate
	WHERE Employees.Id = Punches.IdEmployee
) TMP 
where TMP.ActualType = 1 --=> Sólo contemplo entradas (y sumo con el siguiente, que deberá ser una salida) ... 
AND TMP.MediaAnteriorDelDia <> 2 --=> O bien es el primer fichaje del día y es una entrada (1), o bien el fichaje anterior tiene el sentido correcto (1,5)
AND TMP.MediaSiguienteDelDia = 1.5 --=> No es el último fichaje del día, y el siguiente tiene el sentido correcto
GROUP BY IDEmployee, ShiftDate, InTelecommute
GO


DROP VIEW IF EXISTS [dbo].[sysrovwEmployeesLastPunchByDate]
GO
CREATE VIEW [dbo].[sysrovwEmployeesLastPunchByDate]
 AS
SELECT IDEmployee, Shiftdate, Datetime, DATEDIFF(minute,Datetime,getdate())/60.0 AS Offset, ActualType, InTelecommute, IDZone, WorkCenter FROM (
SELECT row_number() OVER (PARTITION BY IdEmployee, Shiftdate ORDER BY DateTime DESC) AS 'RowNumber', IDEmployee, Shiftdate, Datetime, InTelecommute, IDZone, WorkCenter, ActualType FROM Punches WITH(NOLOCK) WHERE IDEmployee > 0 
) AUX
WHERE AUX.RowNumber = 1
GO

UPDATE sysroLiveAdvancedParameters SET Value = 16 WHERE ParameterName='VTPortalApiVersion'
GO

ALTER VIEW [dbo].[sysrovwDashboardEmployeeStatus]
    AS
               SELECT svEmployees.IdEmployee, 
               svEmployees.EmployeeName,
               Shifts.Name ShiftName,
               tmpLastPunch.Type as LastPunchType,
               tmpAttendance.InTelecommute as InTelecommute,
               tmpLastPunch.ShiftDate as LastPunchDate,
               tmpLastPunch.DateTime as LastPunchDateTime,
               tmpLastPunch.TypeData as LastPunchTypeData,
               Employees.Image EmployeeImage,
               tmpAttendance.ShiftDate LastAttendanceDate, 
               tmpAttendance.DateTime as LastAttendancePunchDatetime,
               ISNULL(Causes.Name, '') as LastAttendanceCause,
               CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN 'Out-Calculated' ELSE CASE tmpAttendance.ActualType WHEN 1 THEN 'In' ELSE 'Out' END END as AttendanceStatus, 
               tmpTasks.DateTime as LastTaskPunchDatetime, 
		       ISNULL(Tasks.Project + ' : ' +  Tasks.Name,'') as LastTaskName,  
               tmpcosts.DateTime as LastCostPunchDatetime, 
               CASE WHEN BusinessCentersPunch.Name IS NOT NULL THEN BusinessCentersPunch.Name ELSE (CASE WHEN BusinessCentersShifts.Name IS NOT NULL THEN BusinessCentersShifts.Name ELSE BusinessCentersGroups.Name END) END AS CostCenterName,
               CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN '' ELSE tmpLastPunch.LocationZone END As LocationName,
               CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN '' ELSE Zones.Name END As ZoneName,
               CASE Shifts.ShiftType WHEN 2 THEN 1 ELSE 0 END AS InHolidays,
               CASE WHEN ProgrammedAbsences.IDCause > 0 THEN 1 ELSE 0 END AS InAbsence,
               CASE WHEN ProgrammedAbsences.IDCause > 0 THEN Causes1.Name ELSE '' END AS InAbsenceCause,
               CASE WHEN tmpHourAbsences.IDCause > 0 THEN 1 ELSE 0 END AS InHourAbsence,
               CASE WHEN tmpHourAbsences.IDCause > 0 THEN Causes3.Name ELSE '' END AS InHourAbsenceCause,
               CASE WHEN tmpHoursHolidays.IDEmployee IS NOT NULL THEN 1 ELSE 0 END AS InHoursHolidays,
               CASE WHEN Requests.IDCause > 0 THEN 1 ELSE 0 END AS AbsenceRequested,
               CASE WHEN Requests.IDCause > 0 THEN Causes2.Name ELSE '' END AS AbsenceRequestCause,
               CASE WHEN tmpHourAbsenceRequest.IDCause > 0 THEN 1 ELSE 0 END AS HoursAbsenceRequested,
               CASE WHEN tmpHourAbsenceRequest.IDCause > 0 THEN Causes2.Name ELSE '' END AS HoursAbsenceRequestCause
               FROM sysrovwcurrentemployeegroups svEmployees WITH (NOLOCK)
               INNER JOIN Employees WITH (NOLOCK) ON Employees.Id = svEmployees.IDEmployee
			   LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY Id ORDER BY LEN(Path) DESC, LEN(PathGroup) DESC) AS 'RowNumberG', * FROM (SELECT Groups.Id IdGroup, Groups.Path PathGroup, Groups.IDCenter IdCenterGroup, GAux.Id, GAux.Path FROM Groups 
																																INNER JOIN Groups as GAux ON GAux.Path = Groups.Path OR (GAux.Path LIKE CONCAT(Groups.Path ,'\%')) 
																																WHERE Groups.IDCenter IS NOT NULL) GrouCentersAux) GrouCenters ON GrouCenters.ID = svEmployees.IDGroup
               LEFT JOIN DailySchedule WITH (NOLOCK) ON DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) = DailySchedule.Date AND svEmployees.IdEmployee = DailySchedule.IDEmployee
               LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberPunch', * FROM Punches WITH (NOLOCK) WHERE DATEDIFF(day,ShiftDate, GETDATE()) < 30  ) tmpLastPunch ON svEmployees.IDEmployee = tmpLastPunch.IdEmployee
               LEFT JOIN Zones ON tmpLastPunch.IDZone = ZOnes.ID
               LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberAtt', * FROM Punches WITH (NOLOCK) WHERE (ActualType = 1 or ActualType = 2) AND DATEDIFF(day,ShiftDate, GETDATE()) < 30 ) tmpAttendance ON svEmployees.IDEmployee = tmpAttendance.IdEmployee
               LEFT JOIN Causes ON tmpAttendance.TypeData = Causes.ID AND Causes.ID >0
               LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberTsk', * FROM Punches WITH (NOLOCK) WHERE ActualType = 4 AND DATEDIFF(day,ShiftDate, GETDATE()) < 30) tmpTasks ON svEmployees.IDEmployee = tmpTasks.IdEmployee
               LEFT JOIN Tasks ON Tasks.Id = tmpTasks.TypeData
               LEFT JOIN sysroLiveAdvancedParameters ON (ParameterName = 'LastPunchCenterEnabled' and convert(nvarchar,Value) = 1) 
               LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberCost', * FROM Punches WITH (NOLOCK) WHERE ActualType = 13 AND DATEDIFF(day,ShiftDate, GETDATE()) < 30) tmpCosts ON svEmployees.IDEmployee = tmpCosts.IdEmployee AND (sysroLiveAdvancedParameters.Value = 1 OR tmpCosts.ShiftDate = DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))
               LEFT JOIN BusinessCenters BusinessCentersPunch WITH (NOLOCK) ON tmpCosts.TypeData = BusinessCentersPunch.ID            
               LEFT JOIN Shifts WITH (NOLOCK) ON Shifts.Id = DailySchedule.IDShift1
               LEFT JOIN BusinessCenters BusinessCentersShifts WITH (NOLOCK) ON Shifts.IDCenter = BusinessCentersShifts.Id
			   LEFT JOIN BusinessCenters BusinessCentersGroups WITH (NOLOCK) ON GrouCenters.IdCenterGroup = BusinessCentersGroups.ID
               LEFT JOIN ProgrammedAbsences WITH (NOLOCK) ON ProgrammedAbsences.IDEmployee = svEmployees.IdEmployee AND  DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) BETWEEN ProgrammedAbsences.BeginDate AND ISNULL(ProgrammedAbsences.FinishDate,DATEADD(day,ProgrammedAbsences.MaxLastingDays-1,ProgrammedAbsences.BeginDate))
               LEFT JOIN Causes as Causes1 WITH (NOLOCK) ON Causes1.ID = ProgrammedAbsences.IDCause
               LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Date ORDER BY idemployee ASC, Date DESC) AS 'RowNumberHourHol', * FROM ProgrammedHolidays WITH (NOLOCK)) tmpHoursHolidays ON tmpHoursHolidays.Date = DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) and tmpHoursHolidays.IDEmployee = svEmployees.IdEmployee
               LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Date ORDER BY idemployee ASC, Date DESC) AS 'RowNumberHourAbsence', * FROM ProgrammedCauses WITH (NOLOCK)) tmpHourAbsences ON  tmpHourAbsences.IDEmployee = svEmployees.IdEmployee AND DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) BETWEEN tmpHourAbsences.Date AND tmpHourAbsences.FinishDate
               LEFT JOIN Causes as Causes3 WITH (NOLOCK) ON Causes3.ID = tmpHourAbsences.IDCause
               LEFT JOIN Requests WITH (NOLOCK) ON Requests.IDEmployee = svEmployees.IdEmployee AND Requests.RequestType = 7 AND DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) BETWEEN Requests.Date1 AND Requests.Date2 AND Requests.Status IN (0,1)
               LEFT JOIN Causes as Causes2 WITH (NOLOCK) ON Causes2.ID = Requests.IDCause
               LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Date1, Date2 ORDER BY idemployee ASC, RequestDate DESC) AS 'RowHourAbsenceRequest', * FROM Requests WITH (NOLOCK) WHERE RequestType = 9 AND Status IN (0,1)) tmpHourAbsenceRequest ON tmpHourAbsenceRequest.IDEmployee = svEmployees.IdEmployee  AND DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) BETWEEN tmpHourAbsenceRequest.Date1 AND tmpHourAbsenceRequest.Date2 
               LEFT JOIN Causes as Causes4 WITH (NOLOCK) ON Causes4.ID = tmpHourAbsenceRequest.IDCause
               WHERE CurrentEmployee = 1
               AND (RowNumberAtt = 1 OR RowNumberAtt IS NULL)
               AND (RowNumberPunch = 1 OR RowNumberPunch IS NULL)
               AND (RowNumberTsk = 1 OR RowNumberTsk IS NULL)
               AND (RowNumberCost = 1 OR RowNumberCost IS NULL)
               AND (RowNumberHourHol = 1 OR RowNumberHourHol IS NULL)
			   AND (RowNumberHourAbsence = 1 OR RowNumberHourAbsence IS NULL)
               AND (RowHourAbsenceRequest = 1 OR RowHourAbsenceRequest IS NULL)
			   AND (RowNumberG = 1 OR RowNumberG IS NULL)
GO

 ALTER VIEW [dbo].[sysrovwEmployeeStatus]
    AS
  	SELECT svEmployees.IdEmployee, 
  	Employees.Image as EmployeeImage,
  	Employees.Name as EmployeeName,
   	tmpAttendance.ShiftDate AttShiftDate, 
   	tmpAttendance.DateTime as AttDatetime, 
	tmpAttendance.InTelecommute,
   	CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN 'Out-Calculated' ELSE CASE tmpAttendance.ActualType WHEN 1 THEN 'In' ELSE 'Out' END END as AttStatus, 
   	tmpTasks.DateTime as TskDatetime, 
      Tasks.Project + ' : ' +  Tasks.Name as TskName,  
  	CASE WHEN BusinessCenters.Name IS NOT NULL THEN tmpcosts.DateTime ELSE (CASE WHEN BusinessCentersShifts.Name IS NOT NULL THEN DailySchedule.Date ELSE CASE WHEN GrouCenters.IdCenterGroup IS NOT NULL THEN svEmployees.BeginDate END END) END AS CostDatetime ,
   	CASE WHEN BusinessCenters.Name IS NOT NULL THEN BusinessCenters.Name ELSE (CASE WHEN BusinessCentersShifts.Name IS NOT NULL THEN BusinessCentersShifts.Name ELSE BusinessCentersGroups.Name END) END AS CostCenterName ,
  	CASE WHEN BusinessCenters.ID IS NOT NULL THEN BusinessCenters.ID ELSE (CASE WHEN BusinessCentersShifts.ID IS NOT NULL THEN BusinessCentersShifts.ID ELSE GrouCenters.IdCenterGroup END) END AS CostIDCenter
   	FROM sysrovwcurrentemployeegroups svEmployees
  	inner join Employees ON Employees.ID = svEmployees.IDEmployee
	LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY Id ORDER BY LEN(Path) DESC, LEN(PathGroup) DESC) AS 'RowNumberG', * FROM (SELECT Groups.Id IdGroup, Groups.Path PathGroup, Groups.IDCenter IdCenterGroup, GAux.Id, GAux.Path FROM Groups 
																													INNER JOIN Groups as GAux ON GAux.Path = Groups.Path OR (GAux.Path LIKE CONCAT(Groups.Path ,'\%')) 
																													WHERE Groups.IDCenter IS NOT NULL) GrouCentersAux) GrouCenters ON GrouCenters.ID = svEmployees.IDGroup
   	LEFT JOIN DailySchedule WITH (NOLOCK) ON DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) = DailySchedule.Date AND svEmployees.IdEmployee = DailySchedule.IDEmployee
	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberAtt', * FROM Punches WHERE (ActualType = 1 OR ActualType = 2) AND DATEDIFF(day,ShiftDate, GETDATE()) < 120) tmpAttendance ON svEmployees.IDEmployee = tmpAttendance.IdEmployee
   	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberTsk', * FROM Punches WHERE ActualType = 4 AND DATEDIFF(day,ShiftDate, GETDATE()) < 120) tmpTasks ON svEmployees.IDEmployee = tmpTasks.IdEmployee
   	left join Tasks ON Tasks.Id = tmpTasks.TypeData
  	LEFT JOIN sysroLiveAdvancedParameters ON (ParameterName = 'LastPunchCenterEnabled' and convert(nvarchar,Value) = 1) 
   	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberCost', * FROM Punches WHERE ActualType = 13) tmpCosts ON svEmployees.IDEmployee = tmpCosts.IdEmployee AND (sysroLiveAdvancedParameters.Value = 1 OR tmpCosts.ShiftDate = DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))
   	left join BusinessCenters ON tmpCosts.TypeData = BusinessCenters.ID
	left join BusinessCenters BusinessCentersGroups WITH (NOLOCK) ON GrouCenters.IdCenterGroup = BusinessCentersGroups.ID
  	LEFT JOIN Shifts WITH (NOLOCK) ON Shifts.Id = DailySchedule.IDShift1
  	LEFT JOIN BusinessCenters BusinessCentersShifts WITH (NOLOCK) ON Shifts.IDCenter = BusinessCentersShifts.Id
   	WHERE CurrentEmployee = 1
   	and (RowNumberAtt = 1 or RowNumberAtt is null)
   	and (RowNumberTsk = 1 or RowNumberTsk is null)
   	and (RowNumberCost = 1 or RowNumberCost is null)
	AND (RowNumberG = 1 OR RowNumberG IS NULL)
GO

--Estudio Genius Tiempo efectivo
DELETE [dbo].[GeniusViews] WHERE [Name] = 'effectiveHours' and [IdPassport] = 0
GO
INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature], [RequieredFeature]) 
VALUES (0, N'effectiveHours', N'', N'S', 1, CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'', N'0', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'{"slice":{"rows":[{"uniqueName":"WorkCenter"},{"uniqueName":"Mes"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Office_ToHours","aggregation":"sum"},{"uniqueName":"Telecommute_ToHours","aggregation":"sum"},{"uniqueName":"Fórmula #1","formula":"sum(\"Telecommute_ToHours\") / (sum(\"Office_ToHours\") + sum(\"Telecommute_ToHours\") )","format":"-5c5v3gxgfjf00"}],"expands":{"expandAll":true},"flatOrder":["WorkCenter","Date_ToDateString","Office_ToHours","Telecommute_ToHours"]},"options":{"grid":{"type":"classic","showTotals":"off"},"chart":{"type":"stacked_column","activeMeasure":{"uniqueName":"Fórmula #1","aggregation":"none"}}},"formats":[{"name":"-5c5v3gxgfjf00","decimalPlaces":2,"isPercent":true}],"creationDate":"2021-10-06T11:12:36.972Z"}', N'', N'', N'', N'{"IncludeZeros":true,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}', N'Genius_EfectiveWork(@initialDate,@endDate,@idpassport,@employeeFilter,@userFieldsFilter)', N'Calendar', N'Calendar.Analytics')
GO

-- TimeSpan a fichajes ya existentes para timepo efectivo en oficina
UPDATE [dbo].[Punches] SET TimeSpan = CASE WHEN ActualType = 1 THEN datediff(second, datetime, convert(smalldatetime, '2021-01-01 00:00')) WHEN ActualType = 2 THEN datediff(second, convert(smalldatetime, '2021-01-01 00:00'), datetime) END
WHERE ShiftDate >= '20210101' 
GO

--Genius: Horas efectivas
DROP PROCEDURE [dbo].[Genius_EfectiveWork]
GO
CREATE PROCEDURE [dbo].[Genius_EfectiveWork] @initialDate smalldatetime, @endDate smalldatetime, @idpassport int,  @userFieldsFilter nvarchar(max), @employeeFilter nvarchar(max) AS  

		
  		DECLARE @employeeIDs Table(idEmployee int)
		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @puserFieldsFilter nvarchar(max) = @userFieldsFilter
		DECLARE @pemployeeFilter nvarchar(max) = @employeeFilter
		DECLARE @intdatefirst int  
  		SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'  
  		SET DateFirst @intdatefirst;  
  		
		INSERT INTO @employeeIDs EXEC dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
		
		SELECT	IDEmployee,
				EmployeeName,
				GroupName, 
				FullGroupName,
				WorkCenter,
				TelecommutingExpected,
				IDContract,
				BeginContract_ToDateString,
				EndContract_ToDateString,
				Age, 
				RegDate AS Date_ToDateString,
				Mes,
				Año,
				DayOfWeek,
				WeekOfYear,
				DayOfYear,
				Quarter,
				Nivel1, 
				Nivel2, 
				Nivel3, 
				Nivel4, 
				Nivel5, 
				Nivel6,
				Nivel7, 
				Nivel8, 
				Nivel9, 
				Nivel10, 
				UserField1, 
				UserField2, 
				UserField3, 
				UserField4, 
				UserField5, 
				UserField6, 
				UserField7, 
				UserField8, 
				UserField9, 
				UserField10, 
				Office as Office_ToHours, 
				ISNULL(Telecommute,0) AS Telecommute_ToHours 
		FROM
		(
			SELECT Employees.Id AS IdEmployee,
				dt AS RegDate, 
				MONTH(dt) AS Mes,   
  				YEAR(dt) AS Año, 
				(DATEPART(dw, dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 
				DATEPART(iso_week, dt) AS WeekOfYear, 
				DATEPART(dy, dt) AS DayOfYear, 
				DATEPART(QUARTER, dt) AS Quarter, 
				Employees.Name as EmployeeName, 
				dbo.sysroEmployeeGroups.GroupName, 
				dbo.sysroEmployeeGroups.FullGroupName, 
				EmployeeContracts.Enterprise As WorkCenter,
				ISNULL(DailySchedule.Telecommuting,CASE WHEN (Employees.Telecommuting = 1 AND ISNULL(Employees.TelecommutingDays,'') = '') THEN 1 ELSE (CASE WHEN CHARINDEX(CONVERT(VARCHAR,DATEPART(weekday, dt) - 1), TelecommutingDays) > 0 THEN 1 ELSE 0 END) END) AS TelecommutingExpected,
				EmployeeContracts.IDContract AS IDContract,
				EmployeeContracts.BeginDate AS BeginContract_ToDateString,
				EmployeeContracts.EndDate AS EndContract_ToDateString,
				ISNULL(sysrovwDailyEfectiveWorkingHours.Value,0.0) As Value, 
				CASE WHEN sysrovwDailyEfectiveWorkingHours.InTelecommute = 1 THEN 'Telecommute' ELSE 'Office' END AS TimeType,
			  	dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,
				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,
				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,
				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,
				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,
				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dt) As UserField1,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dt) As UserField2,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dt) As UserField3,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dt) As UserField4,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dt) As UserField5,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dt) As UserField6,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dt) As UserField7,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dt) As UserField8,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dt) As UserField9,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dt) As UserField10,
 				dbo.GetEmployeeAge(Employees.Id) As Age    
			FROM [dbo].[Alldays] (@pinitialDate, @pendDate)
			LEFT JOIN EmployeeContracts with (nolock) ON dt BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate
			LEFT JOIN sysroEmployeeGroups with (nolock) ON dt BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate AND EmployeeContracts.IDEmployee = sysroEmployeeGroups.IDEmployee
			LEFT JOIN Employees with (nolock) ON Employees.ID = sysroEmployeeGroups.IDEmployee
			LEFT JOIN DailySchedule with (nolock) ON DailySchedule.IDEmployee =  Employees.ID AND DailySchedule.Date = dt
			LEFT JOIN sysrovwDailyEfectiveWorkingHours with (nolock) ON sysrovwDailyEfectiveWorkingHours.Date = dt AND sysrovwDailyEfectiveWorkingHours.IdEmployee = Employees.ID
			WHERE sysroEmployeeGroups.IdEmployee IN (SELECT idemployee FROM @employeeIDs)
			AND dbo.WebLogin_GetPermissionOverEmployee(@idpassport,Employees.Id,2,0,0,dt) > 1 
		) TMP
		PIVOT (AVG(Value) for TimeType in (Office,Telecommute)) AS Accrual
		ORDER BY IdEmployee ASC, RegDate ASC
GO

ALTER FUNCTION [dbo].[GetAllEmployeeUserFieldValue]
  (			
  	@FieldName nvarchar(50),
  	@Date smalldatetime
  )
  RETURNS @ValueTable table(idEmployee int, [value] varchar(4000), [Date] smalldatetime) 
  AS
  BEGIN
  	INSERT INTO @ValueTable
  	SELECT Employees.ID, 
  			(SELECT TOP 1 CONVERT(varchar(4000), [Value])
  			 FROM EmployeeUserFieldValues WITH (NOLOCK)
  			 WHERE EmployeeUserFieldValues.IDEmployee = Employees.ID AND 
  			  	   EmployeeUserFieldValues.FieldName = @FieldName AND
  				   EmployeeUserFieldValues.Date <= @Date
  			 ORDER BY EmployeeUserFieldValues.Date DESC),
  			ISNULL((SELECT TOP 1 [Date]
  				    FROM EmployeeUserFieldValues WITH (NOLOCK)
  			        WHERE EmployeeUserFieldValues.IDEmployee = Employees.ID AND 
  			  	          EmployeeUserFieldValues.FieldName = @FieldName AND
  				          EmployeeUserFieldValues.Date <= @Date
  			        ORDER BY EmployeeUserFieldValues.Date DESC), CONVERT(smalldatetime, '1900/01/01', 120))
  	FROM Employees WITH (NOLOCK), sysroUserFields WITH (NOLOCK)
  	WHERE sysroUserFields.Type = 0 AND sysroUserFields.Used = 1 AND
  		  sysroUserFields.FieldName = @FieldName
  	RETURN
  END
GO

delete from sysroPassports_AuthenticationMethods where Method = 3 and Credential =''
GO

update GeniusViews set Name = 'UsersCredentials' where Name = 'users' and IdPassport = 0 and ds= 'U'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[GeniusViews] WHERE [Name] = 'usersContract' and [IdPassport] = 0)
    INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature], [RequieredFeature]) 
	VALUES (0, N'usersContract', N'', N'U', 1, CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'', N'0', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'{"slice":{"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"UserField4"},{"uniqueName":"IDContract"},{"uniqueName":"UserField2"},{"uniqueName":"UserField1"},{"uniqueName":"UserField3"},{"uniqueName":"Age"}],"columns":[{"uniqueName":"[Measures]"}],"expands":{"expandAll":true},"flatOrder":["GroupName","EmployeeName","UserField4","IDContract","UserField2","UserField1","UserField3","Age"]},"options":{"grid":{"type":"flat","showTotals":"off"}},"creationDate":"2021-10-05T14:20:19.691Z"}', N'', N'Categoría profesional,Género,Grupo de cotización,NIF', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":""}', N'Genius_Users(@initialDate,@endDate,@idpassport,@employeeFilter,@userFieldsFilter,@requestTypesFilter)', N'Employees', N'Employees')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[GeniusViews] WHERE [Name] = 'usersWorkCenter' and [IdPassport] = 0)
    INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature], [RequieredFeature]) 
	VALUES (0, N'usersWorkCenter', N'', N'U', 1, CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'', N'0', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'{"slice":{"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"UserField4"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField5"},{"uniqueName":"UserField1"},{"uniqueName":"Age"}],"columns":[{"uniqueName":"[Measures]"}],"expands":{"expandAll":true},"flatOrder":["GroupName","EmployeeName","UserField7","UserField6","UserField4","UserField8","UserField1","UserField5","Age"]},"options":{"grid":{"type":"flat","showTotals":"off"}},"creationDate":"2021-09-06T15:22:19.285Z"}', N'', N'Categoría profesional,Fecha de nacimiento,Fecha Nacimiento,Género,Grupo de cotización,Nacionalidad,NIF,Puesto', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":""}', N'Genius_Users(@initialDate,@endDate,@idpassport,@employeeFilter,@userFieldsFilter,@requestTypesFilter)', N'Employees', N'Employees')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[GeniusViews] WHERE [Name] = 'GapBase' and [IdPassport] = 0)
    INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature], [RequieredFeature]) 
	VALUES (0, N'GapBase', N'', N'E', 1, CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'', N'0', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"}],"rows":[{"uniqueName":"FullGroupName"},{"uniqueName":"sysroProfessionalCategory"},{"uniqueName":"sysroPosition"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"sysroBaseSalary_woman","aggregation":"average","format":"-mfiki0xeapt00"},{"uniqueName":"sysroBaseSalary_man","aggregation":"average","format":"-mfiki0xeapt00"},{"uniqueName":"Total media de Salario base anual","formula":"average(\"sysroBaseSalary\")","format":"-mfiki0xeapt00"},{"uniqueName":"Brecha","formula":"if(average(\"sysroBaseSalary_man\"),\nif(average(\"sysroBaseSalary_man\") > average(\"sysroBaseSalary_woman\")\n,1 - (average(\"sysroBaseSalary_woman\") / average(\"sysroBaseSalary_man\"))\n,1 - (average(\"sysroBaseSalary_man\") / average(\"sysroBaseSalary_woman\"))\n),average(\"sysroBaseSalary_man\"))","format":"-183c8w5x04au00"},{"uniqueName":"sysroBaseSalary","aggregation":"average","active":false,"format":"-i23amfjcdcc00"},{"uniqueName":"Media","formula":"average(\"sysroBaseSalary_woman\")","active":false}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"off"},"chart":{"type":"bar_h","multipleMeasures":true,"activeMeasure":[{"uniqueName":"sysroBaseSalary_man","aggregation":"average"},{"uniqueName":"sysroBaseSalary_woman","aggregation":"average"}]}},"conditions":[{"formula":"#value >= 0.25","measure":"Brecha","format":{"backgroundColor":"#FFFFFF","color":"#000000","fontFamily":"Arial","fontSize":"12px"}},{"formula":"#value < 0.25","measure":"Brecha","format":{"backgroundColor":"#FFFFFF","color":"#8BC34A","fontFamily":"Arial","fontSize":"12px"}},{"formula":"#value >= 12000","measure":"sysroBaseSalary","format":{"backgroundColor":"#DCEDC8","color":"#000000","fontFamily":"Arial","fontSize":"12px"}}],"formats":[{"name":"-183c8w5x04au00","decimalPlaces":2,"isPercent":true},{"name":"-i23amfjcdcc00","thousandsSeparator":",","decimalPlaces":2,"currencySymbol":"€"},{"name":"-mfiki0xeapt00","thousandsSeparator":",","decimalPlaces":0,"currencySymbol":"€","positiveCurrencyFormat":"1$","nullValue":"-","textAlign":"center"}],"creationDate":"2021-10-05T12:18:39.668Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":""}', N'Genius_SalaryGap(@initialDate,@endDate,@idPassport,@employeeFilter,@userFieldsFilter)', N'Employees', N'Employees.UserFields.Information.High')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[GeniusViews] WHERE [Name] = 'GapTotal' and [IdPassport] = 0)
    INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature], [RequieredFeature]) 
	VALUES (0, N'GapTotal', N'', N'E', 1, CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'', N'0', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"FullGroupName"}],"rows":[{"uniqueName":"sysroProfessionalCategory"},{"uniqueName":"sysroPosition"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Total media de Salario total anual mujer","formula":"average(\"sysroTotalSalary_woman\")","format":"-v46gvp0p3ny00"},{"uniqueName":"Total media de Salario total anual hombre","formula":"average(\"sysroTotalSalary_man\")","format":"-v46gvp0p3ny00"},{"uniqueName":"Total media de Salario total anual","formula":"average(\"sysroTotalSalary\")","format":"-v46gvp0p3ny00"},{"uniqueName":"Brecha","formula":"if(average(\"sysroTotalSalary_man\"),\nif(average(\"sysroTotalSalary_man\") > average(\"sysroTotalSalary_woman\")\n,1 - (average(\"sysroTotalSalary_woman\") / average(\"sysroTotalSalary_man\"))\n,1 - (average(\"sysroTotalSalary_man\") / average(\"sysroTotalSalary_woman\"))\n),average(\"sysroTotalSalary_man\"))","format":"-t8pp8bmim9x00"},{"uniqueName":"sysroBaseSalary","aggregation":"average","active":false,"format":"-i23amfjcdcc00"},{"uniqueName":"sysroBaseSalary_man","aggregation":"average","active":false,"format":"-1a1aol6nxd6e00"},{"uniqueName":"sysroBaseSalary_woman","aggregation":"average","active":false,"format":"-yrxyemai4pn00"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"}},"conditions":[{"formula":"#value >= 0.25","measure":"Brecha","format":{"backgroundColor":"#FFFFFF","color":"#F44336","fontFamily":"Arial","fontSize":"12px"}},{"formula":"#value < 0.25","format":{"backgroundColor":"#FFFFFF","color":"#8BC34A","fontFamily":"Arial","fontSize":"12px"}}],"formats":[{"name":"-1a1aol6nxd6e00","thousandsSeparator":",","decimalPlaces":2,"currencySymbol":"€"},{"name":"-yrxyemai4pn00","thousandsSeparator":",","decimalPlaces":2,"currencySymbol":"€"},{"name":"-i23amfjcdcc00","thousandsSeparator":",","decimalPlaces":2,"currencySymbol":"€"},{"name":"-v46gvp0p3ny00","thousandsSeparator":",","decimalPlaces":0,"currencySymbol":"€","positiveCurrencyFormat":"1$","nullValue":"-","textAlign":"center"},{"name":"-t8pp8bmim9x00","decimalPlaces":2,"textAlign":"center","isPercent":true}],"creationDate":"2021-10-05T12:18:20.805Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":""}', N'Genius_SalaryGap(@initialDate,@endDate,@idPassport,@employeeFilter,@userFieldsFilter)', N'Employees', N'Employees.UserFields.Information.High')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[GeniusViews] WHERE [Name] = 'GapPerceptions' and [IdPassport] = 0)
    INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature], [RequieredFeature]) 
	VALUES (0, N'GapPerceptions', N'', N'E', 1, CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'', N'0', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"}],"rows":[{"uniqueName":"FullGroupName"},{"uniqueName":"sysroProfessionalCategory"},{"uniqueName":"sysroPosition"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Total de Complementos salariales mujer","formula":"average(\"sysroSalarySupp_woman\")","format":"-18f39kcywmdj00"},{"uniqueName":"Total de Complementos salariales hombre","formula":"average(\"sysroSalarySupp_man\")","format":"-18f39kcywmdj00"},{"uniqueName":"Total de Complementos salariales total","formula":"average(\"sysroSalarySupp\")","format":"-18f39kcywmdj00"},{"uniqueName":"Brecha","formula":"if(average(\"sysroSalarySupp_man\"),\nif(average(\"sysroSalarySupp_man\") > average(\"sysroSalarySupp_woman\")\n,1 - (average(\"sysroSalarySupp_woman\") / average(\"sysroSalarySupp_man\"))\n,1 - (average(\"sysroSalarySupp_man\") / average(\"sysroSalarySupp_woman\"))\n),average(\"sysroSalarySupp_man\"))","format":"-1bq2q8by3d5k00"},{"uniqueName":"sysroBaseSalary","aggregation":"average","active":false,"format":"-i23amfjcdcc00"},{"uniqueName":"sysroBaseSalary_man","aggregation":"average","active":false,"format":"-1a1aol6nxd6e00"},{"uniqueName":"sysroBaseSalary_woman","aggregation":"average","active":false,"format":"-yrxyemai4pn00"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"}},"conditions":[{"formula":"#value >= 0.25","measure":"Brecha","format":{"backgroundColor":"#FFFFFF","color":"#000000","fontFamily":"Arial","fontSize":"12px"}},{"formula":"#value < 0.25","measure":"Brecha","format":{"backgroundColor":"#FFFFFF","color":"#8BC34A","fontFamily":"Arial","fontSize":"12px"}}],"formats":[{"name":"-1a1aol6nxd6e00","thousandsSeparator":",","decimalPlaces":2,"currencySymbol":"€"},{"name":"-yrxyemai4pn00","thousandsSeparator":",","decimalPlaces":2,"currencySymbol":"€"},{"name":"-i23amfjcdcc00","thousandsSeparator":",","decimalPlaces":2,"currencySymbol":"€"},{"name":"-18f39kcywmdj00","thousandsSeparator":".","decimalPlaces":2,"currencySymbol":"€","positiveCurrencyFormat":"1$","negativeCurrencyFormat":"-1$","nullValue":"-","textAlign":"center"},{"name":"-1bq2q8by3d5k00","decimalPlaces":2,"textAlign":"center","isPercent":true}],"creationDate":"2021-10-07T09:00:56.701Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":""}', N'Genius_SalaryGap(@initialDate,@endDate,@idPassport,@employeeFilter,@userFieldsFilter)', N'Employees', N'Employees.UserFields.Information.High')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[GeniusViews] WHERE [Name] = 'GapExtraSalary' and [IdPassport] = 0)
    INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature], [RequieredFeature]) 
	VALUES (0, N'GapExtraSalary', N'', N'E', 1, CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'', N'0', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"}],"rows":[{"uniqueName":"FullGroupName"},{"uniqueName":"sysroProfessionalCategory"},{"uniqueName":"sysroPosition"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Total media Percepciones extrasalariales mujer","formula":"average(\"sysroExtraSalary_woman\")","format":"-t6d65mf924r00"},{"uniqueName":"Total media Percepciones extrasalariales hombre","formula":"average(\"sysroExtraSalary_man\")","format":"-t6d65mf924r00"},{"uniqueName":"Total media Percepciones extrasalariales","formula":"average(\"sysroExtraSalary\")","format":"-t6d65mf924r00"},{"uniqueName":"Brecha","formula":"if(average(\"sysroExtraSalary_man\"),\nif(average(\"sysroExtraSalary_man\") > average(\"sysroExtraSalary_woman\")\n,1 - (average(\"sysroExtraSalary_woman\") / average(\"sysroExtraSalary_man\"))\n,1 - (average(\"sysroExtraSalary_man\") / average(\"sysroExtraSalary_woman\"))\n),average(\"sysroExtraSalary_man\"))","format":"-wn5pkgb7jey00"},{"uniqueName":"sysroBaseSalary","aggregation":"average","active":false,"format":"-i23amfjcdcc00"},{"uniqueName":"sysroBaseSalary_man","aggregation":"average","active":false,"format":"-1a1aol6nxd6e00"},{"uniqueName":"sysroBaseSalary_woman","aggregation":"average","active":false,"format":"-yrxyemai4pn00"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"}},"conditions":[{"formula":"#value <= 0.25","measure":"Brecha","format":{"backgroundColor":"#FFFFFF","color":"#F44336","fontFamily":"Arial","fontSize":"12px"}},{"formula":"#value < 0.25","measure":"Brecha","format":{"backgroundColor":"#FFFFFF","color":"#8BC34A","fontFamily":"Arial","fontSize":"12px"}}],"formats":[{"name":"-1a1aol6nxd6e00","thousandsSeparator":",","decimalPlaces":2,"currencySymbol":"€"},{"name":"-yrxyemai4pn00","thousandsSeparator":",","decimalPlaces":2,"currencySymbol":"€"},{"name":"-i23amfjcdcc00","thousandsSeparator":",","decimalPlaces":2,"currencySymbol":"€"},{"name":"-t6d65mf924r00","thousandsSeparator":",","decimalPlaces":0,"currencySymbol":"€","positiveCurrencyFormat":"1$","nullValue":"-","textAlign":"center"},{"name":"-wn5pkgb7jey00","decimalPlaces":2,"textAlign":"center","isPercent":true}],"creationDate":"2021-10-05T12:20:33.106Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":""}', N'Genius_SalaryGap(@initialDate,@endDate,@idPassport,@employeeFilter,@userFieldsFilter)', N'Employees', N'Employees.UserFields.Information.High')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[GeniusViews] WHERE [Name] = 'GapComplementary' and [IdPassport] = 0)
    INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature], [RequieredFeature]) 
	VALUES (0, N'GapComplementary', N'', N'E', 1, CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'', N'0', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"}],"rows":[{"uniqueName":"FullGroupName"},{"uniqueName":"sysroProfessionalCategory"},{"uniqueName":"sysroPosition"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Percepciones por extraordinarias y complementarias mujer","formula":"average(\"sysroEarningsOverTime_woman\")","format":"-t6d65mf924r00"},{"uniqueName":"Percepciones por extraordinarias y complementarias hombre","formula":"average(\"sysroEarningsOverTime_man\")","format":"-t6d65mf924r00"},{"uniqueName":"Percepciones por extraordinarias y complementarias","formula":"average(\"sysroEarningsOverTime\")","format":"-t6d65mf924r00"},{"uniqueName":"Brecha","formula":"if(average(\"sysroEarningsOverTime_man\"),\nif(average(\"sysroEarningsOverTime_man\") > average(\"sysroEarningsOverTime_woman\")\n,1 - (average(\"sysroEarningsOverTime_woman\") / average(\"sysroEarningsOverTime_man\"))\n,1 - (average(\"sysroEarningsOverTime_man\") / average(\"sysroEarningsOverTime_woman\"))\n),average(\"sysroEarningsOverTime_man\"))","format":"-wn5pkgb7jey00"},{"uniqueName":"sysroBaseSalary","aggregation":"average","active":false,"format":"-i23amfjcdcc00"},{"uniqueName":"sysroBaseSalary_man","aggregation":"average","active":false,"format":"-1a1aol6nxd6e00"},{"uniqueName":"sysroBaseSalary_woman","aggregation":"average","active":false,"format":"-yrxyemai4pn00"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"}},"conditions":[{"formula":"#value <= 0.25","measure":"Brecha","format":{"backgroundColor":"#FFFFFF","color":"#F44336","fontFamily":"Arial","fontSize":"12px"}},{"formula":"#value < 0.25","measure":"Brecha","format":{"backgroundColor":"#FFFFFF","color":"#8BC34A","fontFamily":"Arial","fontSize":"12px"}}],"formats":[{"name":"-1a1aol6nxd6e00","thousandsSeparator":",","decimalPlaces":2,"currencySymbol":"€"},{"name":"-yrxyemai4pn00","thousandsSeparator":",","decimalPlaces":2,"currencySymbol":"€"},{"name":"-i23amfjcdcc00","thousandsSeparator":",","decimalPlaces":2,"currencySymbol":"€"},{"name":"-t6d65mf924r00","thousandsSeparator":",","decimalPlaces":0,"currencySymbol":"€","positiveCurrencyFormat":"1$","nullValue":"-","textAlign":"center"},{"name":"-wn5pkgb7jey00","decimalPlaces":2,"textAlign":"center","isPercent":true}],"creationDate":"2021-10-07T09:14:43.919Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":""}', N'Genius_SalaryGap(@initialDate,@endDate,@idPassport,@employeeFilter,@userFieldsFilter)', N'Employees', N'Employees.UserFields.Information.High')
GO

delete from dbo.GeniusViews where Name = 'SalaryBaseGap' and IdPassport = 0
GO

ALTER PROCEDURE [dbo].[Genius_Concepts]
         @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint AS  
 		DECLARE @employeeIDs Table(idEmployee int)  
 		DECLARE @conceptIDs Table(idConcept int)  
 		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros  
 		DECLARE @intdatefirst int  
 		SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'  
 		SET DateFirst @intdatefirst;  
 		
 		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate  
 		insert into @conceptIDs select * from dbo.SplitToInt(@pconceptsFilter,',');  
 		
 		IF @pincludeZeros = 1   
 		BEGIN  
 			WITH alldays AS (  
 				SELECT @initialDate AS dt  
 				UNION ALL  
 				SELECT DATEADD(dd, 1, dt)  
 				FROM alldays s  
 				WHERE DATEADD(dd, 1, dt) <= @pendDate)  
 			SELECT   
 				reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept,   
 				dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName,   
 				reqReg.conceptName AS ConceptName, reqReg.dt AS Date, reqReg.dt AS Date_ToDateString, sum(isnull(dbo.DailyAccruals.Value,0)) AS Value,COUNT(*) AS Count, MONTH(reqReg.dt) AS Mes,   
 				YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week,   
 				reqReg.dt) AS WeekOfYear, DATEPART(dy, reqReg.dt) AS DayOfYear, DATEPART(QUARTER, reqReg.dt) AS Quarter, dbo.sysroEmployeeGroups.Path,   
 				dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
 				dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @pendDate AS EndPeriod,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),reqReg.dt) As UserField1,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),reqReg.dt) As UserField2,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),reqReg.dt) As UserField3,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),reqReg.dt) As UserField4,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),reqReg.dt) As UserField5,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),reqReg.dt) As UserField6,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),reqReg.dt) As UserField7,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),reqReg.dt) As UserField8,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),reqReg.dt) As UserField9,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),reqReg.dt) As UserField10,
				dbo.GetEmployeeAge(reqReg.idEmployee) As Age     
 			FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,   
 				(select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp,   
 				(select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con  
 				where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1  
 				) reqReg  
 				INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate  
 				INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate  
 				LEFT OUTER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept  
 			GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName,   
 				dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt),   
 				YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy,   
 				reqReg.dt), CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar)   
 				+ '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120),   
 				10), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
 				dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  
 			option (maxrecursion 0)  
 		END  
 		ELSE  
 		BEGIN  
 			WITH alldays AS (  
 				SELECT @initialDate AS dt  
 				UNION ALL  
 				SELECT DATEADD(dd, 1, dt)  
 				FROM alldays s  
 				WHERE DATEADD(dd, 1, dt) <= @pendDate)  
 			SELECT
 				reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept,   
 				dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName,   
 				reqReg.conceptName AS ConceptName, reqReg.dt AS Date, reqReg.dt AS Date_ToDateString, sum(isnull(dbo.DailyAccruals.Value,0)) AS Value, COUNT(*) AS Count, MONTH(reqReg.dt) AS Mes,   
 				YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week,   
 				reqReg.dt) AS WeekOfYear, DATEPART(dy, reqReg.dt) AS DayOfYear, DATEPART(QUARTER, reqReg.dt) AS Quarter, dbo.sysroEmployeeGroups.Path,   
 				dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
 				dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @pendDate AS EndPeriod,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),reqReg.dt) As UserField1,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),reqReg.dt) As UserField2,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),reqReg.dt) As UserField3,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),reqReg.dt) As UserField4,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),reqReg.dt) As UserField5,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),reqReg.dt) As UserField6,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),reqReg.dt) As UserField7,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),reqReg.dt) As UserField8,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),reqReg.dt) As UserField9,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),reqReg.dt) As UserField10,
				dbo.GetEmployeeAge(reqReg.idEmployee) As Age 
 		   FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,   
 				(select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp,   
 				(select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con  
 				where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1  
 				) reqReg  
 				INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate  
 				INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate  
 				INNER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept  
 		   GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName,   
 				dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt),   
 				YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy,   
 				reqReg.dt), CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar)   
 				+ '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120),   
 				10), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
 				dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  
 		   option (maxrecursion 0)  
        END  
GO

update dbo.GeniusViews set CubeLayout=N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"ConceptName"}],"measures":[{"uniqueName":"Hours_ToHours","formula":"sum(\"Value\")","caption":"Hours_ToHours"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"columns"}},"creationDate":"2021-10-07T09:43:29.791Z"}' where Name='conceptsByEmployeeDate' and IdPassport=0
GO

  
 CREATE FUNCTION [dbo].[GetDirectSupervisorNameByRequest]  
  (   
      @idEmployee int,  
      @featureAlias nvarchar(100),  
      @employeeFeatureId int,  
   @RequestType int  
  )  
  RETURNS nvarchar(1000)  
  AS  
  BEGIN  
  DECLARE @RetNames nvarchar(1000);  
  SET @RetNames = ''  
  DECLARE @tmpTable Table(PassportID int,LevelOfAuthority int ,Name nvarchar(50))  
   -- si es seguridad v3  
   begin  
   insert into @tmpTable   
   SELECT sysroPassports.id AS PassportID, dbo.GetPassportLevelOfAuthority(sysroPassports.ID,@RequestType,0,3) AS LevelOfAuthority, sysroPassports.Name  
         FROM sysroPassports    
         WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null AND   
           dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@employeeFeatureID,2,1,getdate()) > 3 AND   
        dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID, @featureAlias,'U',2) > 3 AND Description not like '@@ROBOTICS@@%'  
   SET @RetNames = (SELECT  CONVERT(nvarchar(4000),Name ) + ','  AS [text()] from @tmpTable tt where  tt.LevelOfAuthority = (SELECT max(LevelOfAuthority) from @tmpTable where LevelOfAuthority <> 15) For XML PATH (''))  
   IF @RetNames <> ''  
    BEGIN  
         SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)  
    END  
   end  
  RETURN @RetNames  
  END
  GO
  
ALTER PROCEDURE [dbo].[ObtainFeaturesFromFilter]      
     @requestTypes nvarchar(max)  
     AS       
    begin    
	  declare @prequestTypes nvarchar(max) = @requestTypes    
	  DECLARE @SQLString nvarchar(MAX);      
			SET @SQLString = 'select Alias,EmployeeFeatureId, r.Type, r.IdType from sysroFeatures f inner join sysroRequestType r on f.AliasID = r.IdType and r.IdType IN (' + @requestTypes + ')'  
  
	  exec sp_executesql @SQLString      
	end  
GO

ALTER PROCEDURE  [dbo].[Genius_Users]
         @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @requestTypesFilter nvarchar(max) AS      

 		DECLARE @employeeIDs Table(idEmployee int)      
 		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter       
		DECLARE @employeeIDFilter NVARCHAR(MAX)
		DECLARE @featureNames Table(feature nvarchar(MAX), featureid int, fieldName nvarchar(max), requestTypeId int)
		DECLARE @featureFields NVARCHAR(max)
		DECLARE @securityVersion int = 1

		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate
		select @employeeIDFilter = substring(tmp.empIds,2, len(tmp.empIds) -1) from
			(select convert(nvarchar(max), (select ',' + convert(nvarchar(max),idemployee) as [data()] from @employeeIDs FOR XML PATH(''))) as empIds) tmp

		IF @requestTypesFilter = ''
			SET @requestTypesFilter = '-1'

		IF EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'SecurityMode')
			select @securityVersion = convert(int,value) from [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'SecurityMode'
			
		
		
		insert into @featureNames exec dbo.ObtainFeaturesFromFilter @requestTypesFilter
		if @securityVersion < 3
		begin
			select @featureFields = isnull(tmp.empIds,'') from
				(select convert(nvarchar(max), (select ',dbo.GetFeatureNextLevelPassportsByEmployee(emp.id,''' + feature  +''',' + convert(nvarchar(max),featureid) + ') AS [sup' + fieldName + ']' as [data()] from @featureNames FOR XML PATH(''))) as empIds) tmp
		end
		else
		begin
			select @featureFields = isnull(tmp.empIds,'') from
			(select convert(nvarchar(max), (select ',dbo.GetDirectSupervisorNameByRequest(emp.id,''' + feature  +''',' + convert(nvarchar(max),featureid) + ',' + convert(nvarchar(max),requestTypeId) + ') AS [sup' + fieldName + ']' as [data()] from @featureNames FOR XML PATH(''))) as empIds) tmp
		end
		
		
	DECLARE @SqlStatement NVARCHAR(MAX)
	SET @SqlStatement = N'
 		select * from (  
 			select CAST(emp.ID AS varchar) + ''-'' + CAST(eg.IDGroup AS varchar) + ''-'' + ec.IDContract AS KeyView,
 			emp.Id as IDEmployee, emp.Name as EmployeeName, spam.method as Method, spam.credential as Credential, spam.version as Version,     
 			CASE WHEN len(CONVERT(VARCHAR(MAX), CONVERT(VARBINARY(MAX),spam.BiometricData))) > 0 THEN ''X'' ELSE '''' END AS BiometricData,    
 			spam.Enabled as Enabled, spam.TimeStamp as TimeStamp, t.Description as Terminal, spam.BiometricAlgorithm as BiometricAlgorithm,     
 			g.name as GroupName, g.FullGroupName, ec.IDContract, ec.BeginDate as BeginContract, ec.EndDate as EndContract, eg.BeginDate as BeginDate, eg.EndDate as EndDate,  
 			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,1,''\'') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,2,''\'') As Nivel2,      
 			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,3,''\'') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,4,''\'') As Nivel4,      
 			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,5,''\'') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,6,''\'') As Nivel6,      
 			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,7,''\'') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,8,''\'') As Nivel8,      
 			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,9,''\'') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,10,''\'') As Nivel10,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',1,'',''),GetDate()) As UserField1,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',2,'',''),GetDate()) As UserField2,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',3,'',''),GetDate()) As UserField3,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',4,'',''),GetDate()) As UserField4,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',5,'',''),GetDate()) As UserField5,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',6,'',''),GetDate()) As UserField6,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',7,'',''),GetDate()) As UserField7,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',8,'',''),GetDate()) As UserField8,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',9,'',''),GetDate()) As UserField9,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',10,'',''),GetDate()) As UserField10,
			dbo.GetEmployeeAge(emp.Id) As Age' + @featureFields + '    
         FROM employees emp    
 			 inner join sysroPassports sp on sp.IDEmployee = emp.id    
 			 inner join sysroPassports_AuthenticationMethods spam on spam.IDPassport = sp.ID    
 			 inner join EmployeeContracts ec on emp.id = ec.IDEmployee    
 			 inner join sysroEmployeeGroups eg on eg.IDEmployee = emp.id    
 			 inner join Groups g on g.id = eg.IDGroup    
 			 left join Terminals t on t.id = spam.BiometricTerminalId    
 		WHERE dbo.WebLogin_GetPermissionOverEmployee(' + CAST(@pidpassport as nvarchar(100)) + ',emp.Id,2,0,0,GetDate()) > 1      
 			AND emp.Id in(' + @employeeIDFilter + ') and convert(smalldatetime,''' + convert(nvarchar(max),@pinitialDate,112) + ''',120) >= ec.BeginDate and convert(smalldatetime,''' + convert(nvarchar(max),@pendDate,112) + ''',120) <= ec.EndDate  and Version != ''RXA200''  ) as query';
 
	EXEC(@SqlStatement)
GO


-- Vista para informe de emergencia
  ALTER VIEW [dbo].[sysrovwCurrentEmployeesZoneStatus]
  AS
	SELECT p.IDEmployee, em.Name AS EmployeeName, p.DateTime, p.IDTerminal AS IDReader, p.IDZone, p.ActualType ,Terminals.Type AS TerminalType
	FROM dbo.Punches AS p 
	INNER JOIN (SELECT dbo.Punches.IDEmployee, MAX(dbo.Punches.ID) AS idp
					FROM dbo.Punches INNER JOIN
						(SELECT IDEmployee, MAX(DateTime) AS dat
						FROM dbo.Punches AS Punches_1
						WHERE (IDZone <> 0) AND (NOT (IDZone IS NULL))
						GROUP BY IDEmployee) AS maxidat ON dbo.Punches.IDEmployee = maxidat.IDEmployee AND dbo.Punches.DateTime = maxidat.dat
					GROUP BY dbo.Punches.IDEmployee) AS maxPunch ON p.IDEmployee = maxPunch.IDEmployee AND p.ID = maxPunch.idp 
	LEFT OUTER JOIN dbo.Zones AS zo ON p.IDZone = zo.ID 
	LEFT OUTER JOIN dbo.Employees AS em ON p.IDEmployee = em.ID
	LEFT OUTER JOIN dbo.Terminals ON dbo.Terminals.ID = p.IDTerminal
	WHERE p.Type<> 6
GO


-- Pantalla de zonas para SaaS y MT
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI] WHERE [URL] = '/Zones')
    INSERT INTO [dbo].sysroGUI(IDPath,LanguageReference,URL,IconURL,Type,Parameters,RequiredFeatures,SecurityFlags,Priority,AllowedSecurity,RequiredFunctionalities)
         VALUES ('Portal\GeneralManagement\Zones','Gui.Zones','/Zones','AccessZones.png',NULL,'SaaSEnabled',NULL,NULL,102,NULL,'U:Access.Zones=Read')
GO

-- Pantalla de zonas antigua sólo en OP
UPDATE [dbo].sysroGUI SET Parameters = 'OnPremise' WHERE IDPath = 'Portal\GeneralManagement\AccessZones'
GO

ALTER TABLE ProgrammedAbsences ADD RequestId int NULL
go
ALTER TABLE ProgrammedCauses ADD RequestId int NULL
go
ALTER TABLE ProgrammedOvertimes ADD RequestId int NULL
go


IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 76)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,OnlySystem) VALUES(76,'Absence canceled by user',null, 1, 0)
GO

INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (76,1,'Body',1,'EmployeeName','RequestCanceled')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (76,1,'Subject',0,'','RequestCanceled')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (76,1,'Body',2,'RequestType','RequestCanceled')
GO

ALTER PROCEDURE [dbo].[GenerateAllRequestPassportPermission]  
     AS  
     BEGIN  
   /* Rellenamos una tabla  con todos los passaportes de tipo grupo y cada una de las peticiones sobre las que tiene permisos */  
             DELETE FROM sysroPermissionsOverRequests
            
             INSERT INTO sysroPermissionsOverRequests
            select pof.PassportID as passportid, r.ID as IDRequest, pog.EmployeeGroupID from Requests r
            inner join sysroFeatures srf on srf.AliasID = r.RequestType
            inner join sysrovwCurrentEmployeeGroups ceg on ceg.IDEmployee = r.IDEmployee
            inner join sysroPermissionsOverFeatures pof on pof.FeatureID = srf.ID and pof.Permission > 3
            inner join sysroPermissionsOverGroups pog on pof.PassportID = pog.PassportID and pog.EmployeeGroupID = ceg.IDGroup and pog.Permission > 3 and pog.EmployeeFeatureID = srf.EmployeeFeatureID
            left join sysroPermissionsOverEmployeesExceptions poe on poe.PassportID = pof.PassportID and poe.EmployeeID = r.IDEmployee and poe.EmployeeFeatureID = srf.EmployeeFeatureID
            where isnull(poe.Permission,6) > 3
     END
GO


ALTER PROCEDURE [dbo].[InsertPassportRequestsPermission]    
     (    
     @IDParentPassport int    
     )    
     AS    
     /* Al crear un nuevo pasaporte debemos añadir todos los permisos que tiene este sobre las peticiones */    
     BEGIN    
             DECLARE @pIDParentPassport int = @IDParentPassport  
             DELETE FROM sysroPermissionsOverRequests where IDParentPassport = @pIDParentPassport
            INSERT INTO sysroPermissionsOverRequests
                   select pof.PassportID as passportid, r.ID as IDRequest, pog.EmployeeGroupID from Requests r
                   inner join sysroFeatures srf on srf.AliasID = r.RequestType
                   inner join sysrovwCurrentEmployeeGroups ceg on ceg.IDEmployee = r.IDEmployee
                   inner join sysroPermissionsOverFeatures pof on pof.FeatureID = srf.ID and pof.Permission > 3
                   inner join sysroPermissionsOverGroups pog on pof.PassportID = pog.PassportID and pog.EmployeeGroupID = ceg.IDGroup and pog.Permission > 3 and pog.EmployeeFeatureID = srf.EmployeeFeatureID
                   left join sysroPermissionsOverEmployeesExceptions poe on poe.PassportID = pof.PassportID and poe.EmployeeID = r.IDEmployee and poe.EmployeeFeatureID = srf.EmployeeFeatureID
                   where pof.PassportID = @pIDParentPassport and isnull(poe.Permission,6) > 3  
       END
GO


ALTER PROCEDURE [dbo].[AlterPassportRequestsPermission]    
     (    
     @IDParentPassport int    
     )    
     AS    
       /* Al modificar un pasaporte debemos actualizar todos los permisos que tiene este sobre las peticiones */    
     BEGIN    
             DECLARE @renewPassports table(IDPassport int)
            DECLARE @pIDParentPassport int = @IDParentPassport    
             /* Obtenemos todos los pasaportes de tipo grupo que pueden supervisar y que estan dentro del arbol de grupos del pasaporte a actualizar */    
             INSERT INTO @renewPassports(IDPassport)  
                    SELECT ID from sysroPassports where GroupType = 'U' and ID NOT IN(SELECT IDPassport FROM dbo.GetAllPassportParentsEmployeeType()) and (ID in(SELECT ID FROM dbo.GetPassportChilds(@pIDParentPassport)) or ID = @pIDParentPassport)    
             DELETE FROM sysroPermissionsOverRequests where IDParentPassport in (SELECT IDPassport from @renewPassports)
            INSERT INTO sysroPermissionsOverRequests
                   select pof.PassportID as passportid, r.ID as IDRequest, pog.EmployeeGroupID from Requests r
                   inner join sysroFeatures srf on srf.AliasID = r.RequestType
                   inner join sysrovwCurrentEmployeeGroups ceg on ceg.IDEmployee = r.IDEmployee
                   inner join sysroPermissionsOverFeatures pof on pof.FeatureID = srf.ID and pof.Permission > 3
                   inner join sysroPermissionsOverGroups pog on pof.PassportID = pog.PassportID and pog.EmployeeGroupID = ceg.IDGroup and pog.Permission > 3 and pog.EmployeeFeatureID = srf.EmployeeFeatureID
                   left join sysroPermissionsOverEmployeesExceptions poe on poe.PassportID = pof.PassportID and poe.EmployeeID = r.IDEmployee and poe.EmployeeFeatureID = srf.EmployeeFeatureID
                   where pof.PassportID in (SELECT IDPassport from @renewPassports) and isnull(poe.Permission,6) > 3  
     END
GO


ALTER PROCEDURE [dbo].[InsertRequestPassportPermission]    
     (@IDRequest int) AS /* Añadimos todos los pasaportes que tienen permisos para una solicitud a la tabla temporal */    
     BEGIN    
             DECLARE @pIDRequest int = @IDRequest
            DELETE FROM sysroPermissionsOverRequests where IDRequest = @pIDRequest
            
             INSERT INTO sysroPermissionsOverRequests
            select pof.PassportID as passportid, r.ID as IDRequest, pog.EmployeeGroupID from Requests r
            inner join sysroFeatures srf on srf.AliasID = r.RequestType
            inner join sysrovwCurrentEmployeeGroups ceg on ceg.IDEmployee = r.IDEmployee
            inner join sysroPermissionsOverFeatures pof on pof.FeatureID = srf.ID and pof.Permission > 3
            inner join sysroPermissionsOverGroups pog on pof.PassportID = pog.PassportID and pog.EmployeeGroupID = ceg.IDGroup and pog.Permission > 3 and pog.EmployeeFeatureID = srf.EmployeeFeatureID
            left join sysroPermissionsOverEmployeesExceptions poe on poe.PassportID = pof.PassportID and poe.EmployeeID = r.IDEmployee and poe.EmployeeFeatureID = srf.EmployeeFeatureID
            where r.ID = @pIDRequest and isnull(poe.Permission,6) > 3
      END
GO



ALTER PROCEDURE [dbo].[GenerateChangeDayRequestPassportPermission]  
     AS  
     BEGIN  
             /* Rellenamos una tabla  con todos los passaportes de tipo grupo y cada una de las peticiones sobre las que tiene permisos */  
             DECLARE @idRequest int  
             DECLARE @renewRequests table(IDRequest int)  
             INSERT INTO @renewRequests(IDRequest)  
             select Requests.Id from Requests  
                    left outer join sysroPermissionsOverRequests on sysroPermissionsOverRequests.IDRequest = Requests.ID  
                    left outer join sysrovwAllEmployeeGroups on Requests.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee   
                    where (sysroPermissionsOverRequests.IDRequest is null) or (sysroPermissionsOverRequests.EmployeeGroupID <> sysrovwAllEmployeeGroups.IDGroup)  
             DELETE FROM sysroPermissionsOverRequests where IDRequest in (SELECT IDRequest from @renewRequests)  
             INSERT INTO sysroPermissionsOverRequests
            select pof.PassportID as passportid, r.ID as IDRequest, pog.EmployeeGroupID from Requests r
            inner join sysroFeatures srf on srf.AliasID = r.RequestType
            inner join sysrovwCurrentEmployeeGroups ceg on ceg.IDEmployee = r.IDEmployee
            inner join sysroPermissionsOverFeatures pof on pof.FeatureID = srf.ID and pof.Permission > 3
            inner join sysroPermissionsOverGroups pog on pof.PassportID = pog.PassportID and pog.EmployeeGroupID = ceg.IDGroup and pog.Permission > 3 and pog.EmployeeFeatureID = srf.EmployeeFeatureID
            left join sysroPermissionsOverEmployeesExceptions poe on poe.PassportID = pof.PassportID and poe.EmployeeID = r.IDEmployee and poe.EmployeeFeatureID = srf.EmployeeFeatureID
            where r.ID in (SELECT IDRequest from @renewRequests) and isnull(poe.Permission,6) > 3
      END
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] alter column  GroupName nvarchar(max)
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] alter column  EmployeeName nvarchar(max)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 77)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,OnlySystem, Feature, FeatureType, IDCategory) VALUES(77,'Overtime breach',NULL, 60, 0, 'Calendar.Punches','U',6)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 78)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,OnlySystem, Feature, FeatureType, IDCategory) VALUES(78,'Hour absence breach',NULL, 60, 0, 'Calendar.Punches','U',6)
GO

DELETE FROM [dbo].[sysroGUI]
GO

INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL)
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Home', N'Home', N'Main.aspx', N'Robotics.png', NULL, NULL, NULL, NULL, 1000, NULL, NULL)
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\CompanyVisible', N'Gui.Company', N'Employees/Company.aspx', N'Company.png', NULL, NULL, N'Forms\Employees', NULL, 1100, NULL, N'U:Employees=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Company', N'Company', N'Employees/Company.aspx', N'Company.png', NULL, NULL, N'Forms\Employees', NULL, 1100, NULL, N'U:Employees=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Company\Groups', N'Gui.Groups', N'Employees/Groups.aspx', N'Groups.png', NULL, NULL, N'Forms\Employees', NULL, 1101, NULL, N'U:Employees=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Company\SecurityFunctions', N'Gui.SecurityFunctions', N'SecurityChart/SecurityFunctions.aspx', N'SecurityFunctions.png', NULL, N'SecurityV2;SecurityV3', N'Forms\Passports', NULL, 1102, NULL, N'U:Administration.Security=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Company\Passports', N'GUI.Passports', N'Security/Passports.aspx', N'Supervisors.png', NULL, N'SecurityV1', N'Forms\Passports', NULL, 1103, NULL, N'U:Administration.Security=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Company\supervisors', N'Gui.Supervisors', N'SecurityChart/Supervisors.aspx', N'Supervisors.png', NULL, N'SecurityV2', N'Forms\Passports', NULL, 1104, NULL, N'U:Administration.Security=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Company\AdvSupervisors', N'Gui.Supervisors', N'Security/Supervisors.aspx', N'Supervisors.png', NULL, N'SecurityV3', N'Forms\Passports', NULL, 1105, NULL, N'U:Administration.Security=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Company\SecurityChart', N'Gui.SecurityChart', N'SecurityChart/SecurityChart.aspx', N'SecurityChart.png', NULL, N'SecurityV2', N'Forms\Passports', NULL, 1105, NULL, N'U:Administration.Security=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Users', N'GUI.General.Employees', NULL, N'Users.png', NULL, NULL, NULL, NULL, 1200, NULL, N'U:Employees=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Users\Employees', N'RRHH', N'Employees/Employees.aspx', N'Employees.png', NULL, NULL, N'Forms\Employees', NULL, 1201, N'NWR', N'U:Employees=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Users\OnBoarding', N'Gui.OnBoarding', N'/OnBoarding', N'OnBoarding96.png', NULL, N'SaaSEnabled', NULL, NULL, 1202, NULL, N'U:Employees=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Users\Requests', N'Supervisor.Requests', N'Requests/Requests.aspx', N'Requests.png', NULL, NULL, NULL, NULL, 1203, NULL, N'U:Employees.UserFields.Requests=Read OR U:Calendar.Punches.Requests=Read OR U:Calendar.Requests=Read OR U:Tasks.Requests.Forgotten=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Users\Surveys', N'Gui.SurveysGeneral', N'/Surveys', N'Surveys96.png', NULL, NULL, NULL, NULL, 1204, NULL, N'U:Employees=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Access', N'Access', NULL, N'Access.png', NULL, NULL, N'Forms\Access', NULL, 1900, NULL, N'U:Access=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Access\AccessStatus', N'GUI.AccessStatus', N'Access/AccessStatus.aspx', N'AccessStatus.png', NULL, NULL, N'Forms\Access', NULL, 1901, NULL, N'U:Access.Zones=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Access\Analytics', N'AccessAnalytics', N'Access/Analytics.aspx', N'AccessAnalytics.png', NULL, NULL, N'Forms\Access', NULL, 1902, NULL, N'U:Access.Analytics=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Access\Events', N'Events', N'Access/Events.aspx', N'Events.png', NULL, NULL, N'Feature\Events', NULL, 1903, NULL, N'U:Events.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Reports', N'Reports', NULL, N'ReportScheduler.png', NULL, NULL, NULL, NULL, 1600, NULL, NULL)
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Reports\Genius', N'Genius', N'/Genius', N'Genius.png', NULL, N'SaaSEnabled', NULL, NULL, 1601, NULL, N'U:Calendar=Read OR U:Tasks.Definition=Read OR U:Access.Analytics=Read OR U:BusinessCenters.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Reports\AdvReport', N'AdvReport', N'/Report', N'ReportScheduler.png', NULL, N'', N'', NULL, 1602, N'NWR', N'U:Reports=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Reports\ReportScheduler', N'ReportScheduler', N'ReportScheduler/ReportScheduler.aspx', N'RRHHSchedule.png', NULL, N'MonoTenant', N'Process\ReportServer', NULL, 1603, N'NWR', N'U:Administration.ReportScheduler.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Reports\AnalyticScheduler', N'AnalyticScheduler', N'ReportScheduler/ReportScheduler.aspx', N'RRHHSchedule.png', NULL, N'MultiTenant', N'', NULL, 1603, N'NWR', N'U:Administration.ReportScheduler.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Reports\TasksQueue', N'TasksQueue', N'Alerts/TasksQueue.aspx', N'TasksQueue.png', NULL, NULL, NULL, NULL, 1604, NULL, N'U:Administration.TasksQueue=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\DocumentaryManagement', N'DocumentaryManagement', N'/DocumentaryManagement', N'Documents.png', NULL, N'SaaSEnabled', NULL, NULL, 1400, NULL, N'U:Documents.DocumentsDefinition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\ShiftControl', N'GUI.ShiftControl', NULL, N'Presencia.ico', NULL, NULL, NULL, NULL, 1300, N'NWR', NULL)
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\ShiftControl\Scheduler', N'GUI.Scheduler', N'Scheduler/Scheduler.aspx', N'Scheduler.png', NULL, N'CalendarV1', N'Forms\Calendar', NULL, 1301, N'NWR', N'U:Calendar=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\ShiftControl\Calendar', N'GUI.Scheduler', N'Scheduler/Calendar.aspx', N'Calendar.png', NULL, N'CalendarV2', N'Forms\Calendar', NULL, 1301, N'NWR', N'U:Calendar=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\ShiftControl\Analytics', N'GUI.CalendarAnalytics', N'Scheduler/AnalyticsScheduler.aspx', N'CalendarAnalytics.png', NULL, NULL, N'Forms\Calendar', NULL, 1302, NULL, N'U:Calendar=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Reports\DataLink', N'DataLink', N'DataLink/DataLink.aspx', N'DataLink.png', NULL, N'MonoTenant', N'Forms\Datalink;!Feature\DatalinkBusiness', NULL, 1605, NULL, N'U:Employees.DataLink=Read OR U:Calendar.DataLink=Read OR U:Tasks.DataLink=Read OR U:BusinessCenters.DataLink=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Reports\DataLinkBusiness', N'DataLinkBusiness', N'DataLink/DataLinkBusiness.aspx', N'ImportExport.png', NULL, N'', N'Forms\Datalink', NULL, 1605, NULL, N'U:Employees.DataLink=Read OR U:Calendar.DataLink=Read OR U:Tasks.DataLink=Read OR U:BusinessCenters.DataLink=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\ShiftControl\AbsencesStatus', N'GUI.Absences', N'Absences/AbsencesStatus.aspx', N'AbsencesStatus.png', NULL, NULL, N'Feature\Absences', NULL, 1303, N'NWR', N'U:Calendar=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\ShiftControl\Budget', N'Gui.Budget', N'AIScheduler/Budget.aspx', N'Budget.png', NULL, N'SecurityV2', N'Feature\HRScheduling', NULL, 1304, NULL, N'U:Budgets.Schedule=Read OR U:Budgets.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Communiques', N'Communiques', N'/Communique', N'Communique.png', NULL, N'SaaSEnabled', N'', NULL, 1500, N'NWR', N'U:Employees.Communiques=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\CostControl', N'CostControl', NULL, N'BusinessCenters.png', NULL, NULL, NULL, NULL, 1700, NULL, N'U:Tasks.Definition=Read OR U:BusinessCenters.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\CostControl\BusinessCenters', N'BusinessCenters', N'Tasks/BusinessCenters.aspx', N'BusinessCenters.png', NULL, NULL, N'!Feature\ONE', NULL, 1701, NULL, N'U:Tasks.Definition=Read OR U:BusinessCenters.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\CostControl\Analytics', N'CostControlAnalytics', N'Scheduler/AnalyticsCostControl.aspx', N'CostAnalytics.png', NULL, NULL, N'Feature\CostControl', NULL, 1702, NULL, N'U:BusinessCenters.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\CostControl\ReviewBusinessCenters', N'ReviewBusinessCenters', N'Tasks/ReviewBusinessCenters.aspx', N'AssignCenters.png', NULL, NULL, N'Feature\CostControl', NULL, 1703, NULL, N'U:Calendar.JustifyIncidences=Read AND U:BusinessCenters.Punches=Write')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Task', N'ProductiV', NULL, N'Task.png', NULL, NULL, N'Feature\Productiv', NULL, 1800, NULL, N'U:Tasks.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Task\Tasks', N'Task.Status', N'Tasks/Tasks.aspx', N'Task.png', NULL, NULL, N'Feature\Productiv', NULL, 1801, NULL, N'U:Tasks.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Task\Analytics', N'TaskAnalytics', N'Tasks/Analytics.aspx', N'TaskAnalytics.png', NULL, NULL, N'Feature\Productiv', NULL, 1802, NULL, N'U:Tasks.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Task\TaskTemplates', N'TaskTemplates', N'TaskTemplates/TaskTemplates.aspx', N'TaskTemplates.png', NULL, NULL, N'Feature\Productiv', NULL, 1803, NULL, N'U:Tasks.TemplatesDefinition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Alerts', N'Alerts', N'Alerts/Alerts.aspx', N'Alerts.png', NULL, NULL, NULL, NULL, 2000, NULL, N'U:Administration.Alerts=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Security', N'Security', NULL, N'Security.png', NULL, NULL, NULL, NULL, 2100, NULL, NULL)
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Security\Aministration', N'GUI.SaaSAdministration', N'Security/SaaSAdmin.aspx', N'AdminSaaS.png', NULL, N'RoboticsEmployee', N'Forms\Passports', NULL, 2101, NULL, N'U:Administration.Security=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Security\Audit', N'Audit', N'Audit/Audit.aspx', N'Audit.png', NULL, NULL, N'Forms\Audit', NULL, 2102, N'NWR', N'U:Administration.Audit=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Security\Diagnostics', N'GUI.Diagnostics', N'Diagnostics/Status.aspx', N'AdminSaaS.png', NULL, N'RoboticsEmployee', N'Forms\Passports', NULL, 2103, NULL, N'U:Administration.Security=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Security\License', N'GUI.License', N'Security/License.aspx', N'License.png', NULL, NULL, N'Forms\Passports', NULL, 2104, NULL, N'U:Administration.Security=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Security\LockDB', N'GUI.LockDB', N'Security/LockDB.aspx', N'LockDB.png', NULL, NULL, N'Forms\Passports', NULL, 2105, NULL, N'U:Administration.Security=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Security\SDK', N'GUI.SDK', N'SDK/ManagePunches.aspx', N'SDK.png', NULL, N'admin', N'Feature\SDK', NULL, 2106, NULL, N'U:Administration.Security=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Configuration', N'Configuration', NULL, N'Administracion.ico', NULL, NULL, NULL, NULL, 2200, N'NWR', NULL)
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Configuration\UserFields', N'UserFields', N'Options/FieldsManager.aspx', N'UserFields.png', NULL, NULL, N'Forms\Options', NULL, 2201, N'NWR', N'U:Administration.Options.General=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Configuration\Routes', N'Routes', N'Options/RoutesManager.aspx', N'Routes.png', NULL, N'MonoTenant', N'Forms\Options;!Feature\ONE', NULL, 2202, N'NWR', N'U:Administration.Options.General=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Configuration\Options', N'ConfigurationOptions', N'Options/ConfigurationOptions.aspx', N'Options.png', NULL, NULL, N'Forms\Options', NULL, 2203, N'NWR', N'U:Administration.Options.General=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Configuration\SecurityOptions', N'SecurityOptions', N'Security/SecurityOptions.aspx', N'SecurityOptions.png', NULL, NULL, N'Forms\Passports', NULL, 2204, NULL, N'U:Administration.SecurityOptions=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Configuration\EmergencyReport', N'EmergencyReport', N'Options/EmergencyReport.aspx', N'EmergencyPrint.png', NULL, NULL, N'Forms\Options;!Feature\ONE', NULL, 2205, NULL, N'U:Administration.Options.General=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Configuration\SSO', N'SSO', N'/SSO', N'SSO.png', NULL, N'MonoTenant', NULL, NULL, 2206, NULL, N'U:Administration.SecurityOptions=Write')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Configuration\Zones', N'Gui.Zones', N'/Zones', N'AccessZones.png', NULL, N'SaaSEnabled', NULL, NULL, 2207, NULL, N'U:Employees=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Configuration\AccessZones', N'GUI.AccessZones', N'Access/AccessZones.aspx', N'AccessZones.png', NULL, N'OnPremise', N'!Feature\ONE', NULL, 2207, NULL, N'U:Access.Zones=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Configuration\Cameras', N'Cameras', N'Cameras/Cameras.aspx', N'Cameras.png', NULL, NULL, N'Version\Live;!Feature\ONE', NULL, 2208, N'NWR', N'U:Administration.Cameras.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Configuration\Documents', N'Documents', N'Documents/DocumentTemplate.aspx', N'Documents.png', NULL, N'MonoTenant', N'Feature\Absences', NULL, 2209, NULL, N'U:Documents.DocumentsDefinition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Configuration\Notifications', N'Notifications', N'Notifications/Notifications.aspx', N'Notificaciones.png', NULL, NULL, N'Process\Notifier', NULL, 2210, N'NWR', N'U:Administration.Notifications.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\Configuration\Terminal', N'Terminals', N'Terminals/Terminals.aspx', N'TerminalesRT.png', NULL, NULL, N'Forms\Terminals', NULL, 2212, N'NWR', N'U:Terminals=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\ShiftManagement', N'Gui.ShiftManagement', NULL, N'ShiftManagement.png', NULL, NULL, NULL, NULL, 2300, NULL, NULL)
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\ShiftManagement\LabAgree', N'LabAgree', N'LabAgree/LabAgree.aspx', N'LabAgree.png', NULL, NULL, N'Feature\ConcertRules', NULL, 2301, NULL, N'U:LabAgree.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\ShiftManagement\ShiftsPro', N'GUI.Shifts', N'Shifts/Shifts.aspx', N'Shifts.png', NULL, NULL, N'Forms\Shifts', NULL, 2302, N'NWR', N'U:Shifts.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'LivePortal', NULL, N'', NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL)
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'LivePortal\MyRequests', N'LivePortal.MyRequests', N'Requests/Requests.aspx', N'MyRequests.png', NULL, NULL, NULL, NULL, 110, NULL, N'E:Punches.Requests=Read OR E:Planification.Requests=Read OR E:UserFields.Requests=Read OR E:TaskPunches.Query=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'LivePortal\MyUserFields', N'LivePortal.MyUserFields', N'UserFields/UserFields.aspx', N'MyUserFields.png', NULL, NULL, NULL, NULL, 310, NULL, N'E:UserFields.Query=Read OR E:UserFields.Requests=Write')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'LivePortal\TimeControl', N'LivePortal.TimeControl', N'', N'TimeControl.png', NULL, NULL, NULL, NULL, 410, NULL, NULL)
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'LivePortal\TimeControl\MyAccruals', N'LivePortal.MyAccruals', N'Querys/AccrualsQuery.aspx', N'Querys.png', NULL, NULL, NULL, NULL, 610, NULL, N'E:Totals.Query=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'LivePortal\TimeControl\MyCalendar', N'LivePortal.MyCalendar', N'Calendar/Calendar.aspx', N'Calendar.png', NULL, NULL, NULL, NULL, 510, NULL, N'E:Planification.Query=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'LivePortal\TimeControl\MyPunches', N'LivePortal.MyPunches', N'Punches/Punches.aspx', N'Punches.png', NULL, NULL, NULL, NULL, 710, NULL, N'E:Punches.Query=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\ShiftManagement\Causes', N'GUI.Causes', N'Causes/Causes.aspx', N'Causes.png', NULL, NULL, N'Forms\Causes', NULL, 2303, N'NWR', N'U:Causes.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\ShiftManagement\Accruals', N'GUI.Accruals', N'Concepts/Concepts.aspx', N'Concepts.png', NULL, NULL, N'Forms\Concepts', NULL, 2304, N'NWR', N'U:Concepts.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\ShiftManagement\KPI', N'KPI', N'Indicators/Indicators.aspx', N'Indicators.png', NULL, NULL, N'Feature\KPIs', NULL, 2305, NULL, N'U:KPI.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\ShiftManagement\ProductiveUnit', N'Gui.ProductiveUnit', N'AIScheduler/ProductiveUnit.aspx', N'ProductiveUnit.png', NULL, N'SecurityV2', N'Feature\HRScheduling', NULL, 2306, NULL, N'U:ProductiveUnit.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\GeneralManagement\Zones', N'Gui.Zones', N'/Zones', N'AccessZones.png', NULL, N'SaaSEnabled', NULL, NULL, 102, NULL, N'U:Access.Zones=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\ShiftManagement\Assignments', N'Assignments', N'Assignments/Assignments.aspx', N'Assignment.png', NULL, NULL, N'Feature\HRScheduling', NULL, 2307, NULL, N'U:Assignments.Definition=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\AccessManagement', N'Gui.AccessManagement', NULL, N'AccessManagement.png', NULL, NULL, NULL, NULL, 2400, NULL, N'U:Access=Read OR U:Activities.Definition=Read OR U:DiningRoom.Turns=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\AccessManagement\AccessGroups', N'GUI.AccessGroups', N'Access/AccessGroups.aspx', N'AccessGroups.png', NULL, NULL, N'Forms\Access', NULL, 2401, NULL, N'U:Access.Groups=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\AccessManagement\AccessPeriods', N'GUI.AccessPeriods', N'Access/AccessPeriods.aspx', N'AccessPeriods.png', NULL, NULL, N'Forms\Access', NULL, 2402, NULL, N'U:Access.Periods=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\AccessManagement\DiningRoomTurn', N'DiningRoom', N'DiningRoom/DiningRoom.aspx', N'DiningRoom.png', NULL, NULL, N'Forms\DiningRoom', NULL, 2403, NULL, N'U:DiningRoom.Turns=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'LivePortal\Tasks', N'LivePortal.Tasks', N'', N'Task.png', NULL, NULL, NULL, NULL, 810, NULL, NULL)
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'LivePortal\Tasks\MyTask', N'LivePortal.MyTask', N'Tasks/CurrentTask.aspx', N'CurrentTask.png', NULL, N'ProductivEmployee', N'Feature\Productiv', NULL, 910, NULL, NULL)
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'LivePortal\Tasks\Accruals', N'LivePortal.TaskAccruals', N'Tasks/TaskAccrualsQuery.aspx', N'Querys.png', NULL, N'ProductivEmployee', N'Feature\Productiv', NULL, 1010, NULL, N'E:Tasktotals.Query=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'LivePortal\Tasks\Punches', N'LivePortal.TaskPunches', N'Tasks/TaskPunches.aspx', N'Punches.png', NULL, N'ProductivEmployee', N'Feature\Productiv', NULL, 1110, NULL, N'E:TaskPunches.Query=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'LivePortal\CDP', N'LivePortal.CDP', N'', N'CDP.png', NULL, NULL, NULL, NULL, 1210, NULL, NULL)
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'LivePortal\CDP\CDPMoves', N'LivePortal.CDPMoves', N'CDP/CDPMoves.aspx', N'Punches.png', NULL, N'ProductivEmployee', N'Forms\JobMoveView', NULL, 1310, NULL, N'E:TaskPunches.Query=Read')
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'LivePortal\CostControl', N'LivePortal.CostControl', N'', N'CostControl.png', NULL, NULL, NULL, NULL, 1410, NULL, NULL)
GO
INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'LivePortal\CostControl\CCMoves', N'LivePortal.CCMoves', N'CostControl/CCMoves.aspx', N'Punches.png', NULL, NULL, N'Feature\CostControl', NULL, 1510, NULL, N'E:Punches.Query=Read')
GO

UPDATE sysroParameters SET Data='521' WHERE ID='DBVersion'
GO


 