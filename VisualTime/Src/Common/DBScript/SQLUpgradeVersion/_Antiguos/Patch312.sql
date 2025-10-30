
DROP VIEW sysroScheduleCube1
GO

DROP VIEW sysroScheduleCube2
GO

DROP VIEW sysroScheduleCube3
GO

CREATE VIEW sysroScheduleCube1
AS
SELECT     TOP (100) PERCENT CAST(dbo.DailyAccruals.IDEmployee AS varchar) + '-' + CAST(dbo.DailyAccruals.IDConcept AS varchar) 
                      + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.DailyAccruals.Date, 120), 
                      10) AS KeyView, dbo.DailyAccruals.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.DailyAccruals.IDConcept, 
                      dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name AS EmployeeName, 
                      dbo.Concepts.Name AS ConceptName, dbo.DailyAccruals.Date, SUM(dbo.DailyAccruals.Value) AS Value, COUNT(*) AS Count, MONTH(dbo.DailyAccruals.Date) AS Mes, 
                      YEAR(dbo.DailyAccruals.Date) AS Año, (DATEPART(dw, dbo.DailyAccruals.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, 
                      dbo.DailyAccruals.Date) AS WeekOfYear, DATEPART(dy, dbo.DailyAccruals.Date) AS DayOfYear, dbo.sysroEmployeeGroups.Path, 
                      dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate
FROM         dbo.DailyAccruals INNER JOIN
                      dbo.sysroEmployeeGroups ON dbo.DailyAccruals.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND 
                      dbo.DailyAccruals.Date >= dbo.sysroEmployeeGroups.BeginDate AND dbo.DailyAccruals.Date <= dbo.sysroEmployeeGroups.EndDate INNER JOIN
                      dbo.EmployeeContracts ON dbo.DailyAccruals.IDEmployee = dbo.EmployeeContracts.IDEmployee AND 
                      dbo.DailyAccruals.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyAccruals.Date <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                      dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID LEFT OUTER JOIN
                      dbo.Concepts ON dbo.DailyAccruals.IDConcept = dbo.Concepts.ID
GROUP BY dbo.DailyAccruals.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.DailyAccruals.IDConcept, dbo.Concepts.Name, 
                      dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name, dbo.DailyAccruals.Date, MONTH(dbo.DailyAccruals.Date), 
                      YEAR(dbo.DailyAccruals.Date), (DATEPART(dw, dbo.DailyAccruals.Date) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, dbo.DailyAccruals.Date), DATEPART(dy, 
                      dbo.DailyAccruals.Date), CAST(dbo.DailyAccruals.IDEmployee AS varchar) + '-' + CAST(dbo.DailyAccruals.IDConcept AS varchar) 
                      + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.DailyAccruals.Date, 120), 
                      10), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, 
                      dbo.sysroEmployeeGroups.EndDate
ORDER BY dbo.DailyAccruals.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.DailyAccruals.IDConcept
GO


CREATE VIEW sysroScheduleCube2
AS
SELECT     CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
                      + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10) AS KeyView, 
                      dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, 
                      dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeesShifts.ShiftName, 
                      dbo.sysroEmployeesShifts.CurrentDate AS Date, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours, COUNT(*) AS Count, 
                      MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año, (DATEPART(dw, 
                      dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear, 
                      DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, 
                      dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate
FROM         dbo.sysroEmployeesShifts INNER JOIN
                      dbo.sysroEmployeeGroups ON dbo.sysroEmployeesShifts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND 
                      dbo.sysroEmployeesShifts.CurrentDate >= dbo.sysroEmployeeGroups.BeginDate AND 
                      dbo.sysroEmployeesShifts.CurrentDate <= dbo.sysroEmployeeGroups.EndDate INNER JOIN
                      dbo.EmployeeContracts ON dbo.sysroEmployeesShifts.IDEmployee = dbo.EmployeeContracts.IDEmployee AND 
                      dbo.sysroEmployeesShifts.CurrentDate >= dbo.EmployeeContracts.BeginDate AND 
                      dbo.sysroEmployeesShifts.CurrentDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                      dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
GROUP BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name, 
                      dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate, MONTH(dbo.sysroEmployeesShifts.CurrentDate), 
                      YEAR(dbo.sysroEmployeesShifts.CurrentDate), (DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, 
                      dbo.sysroEmployeesShifts.CurrentDate), DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate), dbo.sysroEmployeeGroups.IDGroup, 
                      dbo.EmployeeContracts.IDContract, CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
                      + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10), dbo.sysroEmployeeGroups.Path, 
                      dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate
GO

CREATE VIEW sysroScheduleCube3
AS
SELECT     CONVERT(varchar(10), dbo.DailySchedule.IDEmployee, 120) + '-' + CAST(dbo.EmployeeGroups.IDGroup AS varchar) 
                      + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120) AS KeyView, dbo.EmployeeGroups.IDGroup, 
                      dbo.EmployeeContracts.IDContract, dbo.DailySchedule.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description AS IncidenceName, 
                      dbo.TimeZones.Name AS ZoneTime, dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS Value, YEAR(dbo.DailySchedule.Date) AS Año, 
                      (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailySchedule.Date) AS WeekOfYear, DATEPART(dy, 
                      dbo.DailySchedule.Date) AS DayOfYear, CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName, 
                      dbo.Employees.Name AS EmployeeName, dbo.Groups.Name AS GroupName, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName, dbo.Groups.Path, 
                      dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee AS CurrentEmployee
FROM         dbo.EmployeeGroups INNER JOIN
                      dbo.Causes INNER JOIN
                      dbo.DailyCauses ON dbo.Causes.ID = dbo.DailyCauses.IDCause INNER JOIN
                      dbo.DailySchedule ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date ON 
                      dbo.EmployeeGroups.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.EmployeeGroups.BeginDate <= dbo.DailySchedule.Date AND 
                      dbo.EmployeeGroups.EndDate >= dbo.DailySchedule.Date INNER JOIN
                      dbo.EmployeeContracts ON dbo.DailySchedule.IDEmployee = dbo.EmployeeContracts.IDEmployee AND 
                      dbo.DailySchedule.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailySchedule.Date <= dbo.EmployeeContracts.EndDate INNER JOIN
                      dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                      dbo.sysrosubvwCurrentEmployeePeriod ON dbo.DailySchedule.IDEmployee = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee LEFT OUTER JOIN
                      dbo.Employees ON dbo.DailySchedule.IDEmployee = dbo.Employees.ID LEFT OUTER JOIN
                      dbo.sysroDailyIncidencesDescription INNER JOIN
                      dbo.DailyIncidences INNER JOIN
                      dbo.TimeZones ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID ON dbo.sysroDailyIncidencesDescription.IDIncidence = dbo.DailyIncidences.IDType ON 
                      dbo.DailyCauses.IDEmployee = dbo.DailyIncidences.IDEmployee AND dbo.DailyCauses.Date = dbo.DailyIncidences.Date AND 
                      dbo.DailyCauses.IDRelatedIncidence = dbo.DailyIncidences.ID LEFT OUTER JOIN
                      dbo.Shifts ON dbo.DailySchedule.IDShiftUsed = dbo.Shifts.ID LEFT OUTER JOIN
                      dbo.Shifts AS Shifts_1 ON dbo.DailySchedule.IDShift1 = Shifts_1.ID
GROUP BY dbo.DailySchedule.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name, 
                      YEAR(dbo.DailySchedule.Date), CONVERT(varchar(10), dbo.DailySchedule.IDEmployee, 120) + '-' + CAST(dbo.EmployeeGroups.IDGroup AS varchar) 
                      + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120), CASE WHEN DailySchedule.Date <= GETDATE() 
                      THEN Shifts.Name ELSE Shifts_1.Name END, dbo.Employees.Name, dbo.Groups.Name, dbo.GetFullGroupPathName(dbo.Groups.ID), DATEPART(wk, 
                      dbo.DailySchedule.Date), DATEPART(dy, dbo.DailySchedule.Date), (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1, 
                      dbo.EmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.Groups.Path, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, 
                      dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
GO


DROP VIEW sysroAccessCube
GO

CREATE VIEW sysroAccessCube
AS
SELECT     dbo.Punches.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee, 
                      dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, CONVERT(nvarchar(8), 
                      dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day, MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Year, 
                      (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, 
                      dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName, dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL 
                      THEN 0 ELSE 1 END AS IsInvalid, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails) AS Details, dbo.sysroEmployeeGroups.Path, 
                      dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup
FROM         dbo.sysroEmployeeGroups INNER JOIN
                      dbo.Punches ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee INNER JOIN
                      dbo.EmployeeContracts ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND 
                      dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                      dbo.Zones ON dbo.Punches.IDZone = dbo.Zones.ID LEFT OUTER JOIN
                      dbo.Terminals ON dbo.Punches.IDTerminal = dbo.Terminals.ID LEFT OUTER JOIN
                      dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID, 
                      dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108), 
                      dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name, 
                      (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name, 
                      CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path, 
                      dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, 
                      dbo.sysroEmployeeGroups.IDGroup
HAVING (dbo.Punches.Type = 5) OR (dbo.Punches.Type = 6) OR (dbo.Punches.Type = 7)
GO


DROP VIEW sysroAccessPlates
GO

CREATE VIEW sysroAccessPlates
AS
SELECT     dbo.Punches.ID, dbo.Punches.IDEmployee, dbo.Punches.DateTime AS DatePunche, CONVERT(VARCHAR(8), dbo.Punches.DateTime, 108) AS TimePunche, 
                      CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails) AS TypeDetails, dbo.Employees.Name AS NameEmployee, dbo.Zones.Name AS NameZone, 
                      dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.IDGroup, 
                      dbo.sysroEmployeeGroups.EndDate
FROM         dbo.Punches INNER JOIN
                      dbo.TerminalReaders ON dbo.Punches.IDTerminal = dbo.TerminalReaders.IDTerminal AND dbo.Punches.IDReader = dbo.TerminalReaders.ID INNER JOIN
                      dbo.sysroEmployeeGroups ON dbo.Punches.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND 
                      dbo.Punches.DateTime >= dbo.sysroEmployeeGroups.BeginDate AND dbo.Punches.DateTime <= dbo.sysroEmployeeGroups.EndDate LEFT OUTER JOIN
                      dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID LEFT OUTER JOIN
                      dbo.Zones ON dbo.Punches.IDZone = dbo.Zones.ID
WHERE     (dbo.TerminalReaders.Type = N'MAT') AND (NOT (dbo.Punches.TypeDetails IS NULL)) AND (dbo.Punches.Type = 5 OR dbo.Punches.Type = 6 OR dbo.Punches.Type = 7)
GO


drop view sysrovwAbsences
GO

CREATE VIEW sysrovwAbsences
AS
SELECT row_number() OVER(order by x.BeginDate) AS ID, x.* ,emp.Name as EmployeeName, cause.Name as CauseName, grp.IDGroup, grp.FullGroupName,
        grp.Path, grp.CurrentEmployee, grp.BeginDate AS BeginDateMobility, grp.EndDate AS EndDateMobility
 FROM (
 (SELECT 	NULL AS IDRelatedObject,
 			IDCause AS IDCause, 
 			IDEmployee AS IDEmployee, 
 			BeginDate AS BeginDate, 
 			ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) AS FinishDate, 
 			NULL AS BeginTime, 
 			NULL AS EndTime, 
 			CONVERT(NVARCHAR(4000),Description) AS Description ,
 			(SELECT CASE WHEN (select COUNT(*) from AbsenceTracking at where TrackDay < GETDATE() and DeliveryDate is null and at.IDCause = pa.IDCause and at.IDEmployee = pa.IDEmployee and at.Date = pa.BeginDate) > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered, 
 			(SELECT CASE WHEN GETDATE() between BeginDate and DATEADD(DAY,1,(ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )))) THEN '1' WHEN ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) < GETDATE()  THEN '2'  ELSE '0' END) AS Status,
 			'ProgrammedAbsence' AS AbsenceType 
  FROM dbo.ProgrammedAbsences pa)
 union
 (SELECT     ID AS IDRelatedObject,
 			IDCause AS IDCause, 
 			IDEmployee AS IDEmployee, 
 			Date AS BeginDate, 
 			ISNULL(FinishDate,Date) AS FinishDate, 
 			BeginTime AS BeginTime, 
 			EndTime AS EndTime, 
 			CONVERT(NVARCHAR(4000),Description) AS Description, 
 			(SELECT CASE WHEN (select COUNT(*) from AbsenceTracking at where TrackDay < GETDATE() and DeliveryDate is null and at.IDCause = pc.IDCause and at.IDEmployee = pc.IDEmployee and at.Date = pc.Date and at.IDAbsence = pc.ID) > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered, 
 			(SELECT CASE WHEN GETDATE() between Date and DATEADD(DAY,1,(ISNULL(FinishDate, Date))) THEN '1' WHEN  DATEADD(DAY,1,ISNULL(FinishDate,Date)) < GETDATE()  THEN '2'  ELSE '0' END) AS Status,
 			'ProgrammedCause' AS AbsenceType 
  FROM dbo.ProgrammedCauses pc)
 union
 (SELECT     ID AS IDRelatedObject,
 			IDCause AS IDCause, 
 			IDEmployee AS IDEmployee, 
 			Date1 AS BeginDate, 
 			ISNULL(Date2,Date1) AS FinishDate, 
 			FromTime AS BeginTime, 
 			ToTime AS EndTime, 
 			CONVERT(NVARCHAR(4000),Comments) AS Description, 
 			1 AS DocumentsDelivered,
 			(SELECT CASE WHEN Status = 0 THEN '-1' ELSE '-2' END) AS Status,
 			(SELECT CASE WHEN RequestType = 7 THEN 'RequestAbsence' ELSE 'RequestCause' END) AS AbsenceType 
  FROM dbo.Requests where RequestType in(7,9) and Status in(0,1))
  ) x
  inner join dbo.Employees as emp on x.IDEmployee = emp.ID
  inner join dbo.Causes as cause on x.IDCause = cause.ID
  inner join dbo.sysrovwCurrentEmployeeGroups as grp on x.IDEmployee = grp.IDEmployee
GO

-- Nuevos iconos en portal del empleado para la consulta de CDP
INSERT INTO [dbo].[sysroGUI] ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('LivePortal\CDP','LivePortal.CDP','','CDP.png',NULL,NULL,NULL,NULL,1210,NULL,NULL)
GO

INSERT INTO [dbo].[sysroGUI] ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('LivePortal\CDP\CDPMoves','LivePortal.CDPMoves','CDP/CDPMoves.aspx','Punches.png',NULL,'ProductivEmployee','Forms\JobMoveView',NULL,1310,NULL,'E:TaskPunches.Query=Read')
GO

-- Modificación vista para informe de emergencia
ALTER view [dbo].[sysrovwCurrentEmployeesPresenceStatusPunches]
   as
   select p.idemployee as IDEmployee, em.Name as EmployeeName, datetime as DateTime, IDTerminal as IDReader, case when ActualType in (1,2) then 'Att' else 'Acc' end AS MoveType,ISNULL(p.idzone,0) as IDZone,
		 case when ActualType =1 then 'IN' when actualtype=2 then 'OUT' else CASE zo.IsWorkingZone WHEN 1 THEN 'IN' ELSE 'OUT' END  end as Status
		 
		from Punches p
		inner join (
					select Punches.IDEmployee, max(ID) as idp
					from Punches inner join (select idemployee, MAX(datetime) dat
											from Punches
											where ActualType in (1,2) or type = 5 
											group by IDEmployee ) maxidat
					on punches.IDEmployee=maxidat.IDEmployee and punches.DateTime = maxidat.dat 
					where ActualType in (1,2) or type = 5 
					group by punches.IDEmployee ) maxPunch
		on p.IDEmployee=maxPunch.IDEmployee and p.ID=maxPunch.idp 
		left outer join Zones AS zo ON p.IDZone = zo.ID
		left outer join Employees As em ON p.IDEmployee = em.ID
 
GO

-- Tabla de biometría de caras para rxF
CREATE TABLE [dbo].[EmployeeBiometricFaceDataZK](
	[IDEmployee] [int] NOT NULL,
	[Data] [image] NULL,
	[TimeStamp] [smalldatetime] NULL,
	[IDTerminal] [int] NULL,
 CONSTRAINT [PK_EmployeeBiometricFaceDataZK] PRIMARY KEY CLUSTERED 
(
	[IDEmployee] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='312' WHERE ID='DBVersion'
GO

