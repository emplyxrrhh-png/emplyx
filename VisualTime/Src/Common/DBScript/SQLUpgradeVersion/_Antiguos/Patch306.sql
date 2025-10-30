
-- =========================================
ALTER VIEW [dbo].[sysroScheduleCube1]
AS
SELECT     TOP (100) PERCENT CAST(dbo.DailyAccruals.IDEmployee AS varchar) + '-' + CAST(dbo.DailyAccruals.IDConcept AS varchar) 
                      + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.DailyAccruals.Date, 120), 
                      10) AS KeyView, dbo.DailyAccruals.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.DailyAccruals.IDConcept, 
                      dbo.sysroEmployeeGroups.GroupName, dbo.Employees.Name AS EmployeeName, dbo.Concepts.Name AS ConceptName, dbo.DailyAccruals.Date, 
                      SUM(dbo.DailyAccruals.Value) AS Value, COUNT(*) AS Count, MONTH(dbo.DailyAccruals.Date) AS Mes, YEAR(dbo.DailyAccruals.Date) AS Año, (DATEPART(dw, 
                      dbo.DailyAccruals.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailyAccruals.Date) AS WeekOfYear, DATEPART(dy, 
                      dbo.DailyAccruals.Date) AS DayOfYear
FROM         dbo.DailyAccruals INNER JOIN
                      dbo.sysroEmployeeGroups ON dbo.DailyAccruals.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND 
                      dbo.DailyAccruals.Date >= dbo.sysroEmployeeGroups.BeginDate AND dbo.DailyAccruals.Date <= dbo.sysroEmployeeGroups.EndDate INNER JOIN
                      dbo.EmployeeContracts ON dbo.DailyAccruals.IDEmployee = dbo.EmployeeContracts.IDEmployee AND 
                      dbo.DailyAccruals.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyAccruals.Date <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                      dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID LEFT OUTER JOIN
                      dbo.Concepts ON dbo.DailyAccruals.IDConcept = dbo.Concepts.ID
GROUP BY dbo.DailyAccruals.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.DailyAccruals.IDConcept, dbo.Concepts.Name, 
                      dbo.sysroEmployeeGroups.GroupName, dbo.Employees.Name, dbo.DailyAccruals.Date, MONTH(dbo.DailyAccruals.Date), YEAR(dbo.DailyAccruals.Date), 
                      (DATEPART(dw, dbo.DailyAccruals.Date) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, dbo.DailyAccruals.Date), DATEPART(dy, dbo.DailyAccruals.Date), 
                      CAST(dbo.DailyAccruals.IDEmployee AS varchar) + '-' + CAST(dbo.DailyAccruals.IDConcept AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
                      + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.DailyAccruals.Date, 120), 10)
ORDER BY dbo.DailyAccruals.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.DailyAccruals.IDConcept
GO

-- =========================================
ALTER VIEW [dbo].[sysroScheduleCube2]
AS
SELECT     CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
                      + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10) AS KeyView, 
                      dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, 
                      dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate AS Date, 
                      SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours, COUNT(*) AS Count, MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, 
                      YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año, (DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 
                      DATEPART(wk, dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear, DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear
FROM         dbo.sysroEmployeesShifts INNER JOIN
                      dbo.sysroEmployeeGroups ON dbo.sysroEmployeesShifts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND 
                      dbo.sysroEmployeesShifts.CurrentDate >= dbo.sysroEmployeeGroups.BeginDate AND 
                      dbo.sysroEmployeesShifts.CurrentDate <= dbo.sysroEmployeeGroups.EndDate INNER JOIN
                      dbo.EmployeeContracts ON dbo.sysroEmployeesShifts.IDEmployee = dbo.EmployeeContracts.IDEmployee AND 
                      dbo.sysroEmployeesShifts.CurrentDate >= dbo.EmployeeContracts.BeginDate AND 
                      dbo.sysroEmployeesShifts.CurrentDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                      dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
GROUP BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.GroupName, dbo.Employees.Name, dbo.sysroEmployeesShifts.ShiftName, 
                      dbo.sysroEmployeesShifts.CurrentDate, MONTH(dbo.sysroEmployeesShifts.CurrentDate), YEAR(dbo.sysroEmployeesShifts.CurrentDate), (DATEPART(dw, 
                      dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, dbo.sysroEmployeesShifts.CurrentDate), DATEPART(dy, 
                      dbo.sysroEmployeesShifts.CurrentDate), dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, 
                      CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
                      + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10)
GO

-- =========================================
ALTER VIEW [dbo].[sysroScheduleCube3]
AS
SELECT     CONVERT(varchar(10), dbo.DailySchedule.IDEmployee, 120) + '-' + CAST(dbo.EmployeeGroups.IDGroup AS varchar) 
                      + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120) AS KeyView, dbo.EmployeeGroups.IDGroup, 
                      dbo.EmployeeContracts.IDContract, dbo.DailySchedule.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description AS IncidenceName, 
                      dbo.TimeZones.Name AS ZoneTime, dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS Value, YEAR(dbo.DailySchedule.Date) AS Año, 
                      (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailySchedule.Date) AS WeekOfYear, DATEPART(dy, 
                      dbo.DailySchedule.Date) AS DayOfYear, CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName, 
                      dbo.Employees.Name AS EmployeeName, dbo.Groups.Name AS GroupName
FROM         dbo.EmployeeGroups INNER JOIN
                      dbo.Causes INNER JOIN
                      dbo.DailyCauses ON dbo.Causes.ID = dbo.DailyCauses.IDCause INNER JOIN
                      dbo.DailySchedule ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date ON 
                      dbo.EmployeeGroups.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.EmployeeGroups.BeginDate <= dbo.DailySchedule.Date AND 
                      dbo.EmployeeGroups.EndDate >= dbo.DailySchedule.Date INNER JOIN
                      dbo.EmployeeContracts ON dbo.DailySchedule.IDEmployee = dbo.EmployeeContracts.IDEmployee AND 
                      dbo.DailySchedule.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailySchedule.Date <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                      dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
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
                      THEN Shifts.Name ELSE Shifts_1.Name END, dbo.Employees.Name, dbo.Groups.Name, DATEPART(wk, dbo.DailySchedule.Date), DATEPART(dy, 
                      dbo.DailySchedule.Date), (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.EmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract
GO


-- =========================================
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (24200 ,24 ,'DiningRoom.Punches' ,'Fichajes' ,'' ,'U' ,'R' ,NULL)
GO

-- =========================================
INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT sysroPassports.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE 
WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM sysroFeatures, sysroPassports		
WHERE sysroFeatures.ID IN (24200) AND sysroPassports.IDParentPassport IS NULL AND sysroPassports.GroupType = 'U'
GO




-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='306' WHERE ID='DBVersion'
GO
