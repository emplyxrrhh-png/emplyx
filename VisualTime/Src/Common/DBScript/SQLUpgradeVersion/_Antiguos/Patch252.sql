-- Nueva funcionalidad de Flexibilidad horaria y Horarios alternativos para el Live
ALTER TABLE [dbo].[Shifts] ADD IsFloating Bit null default(0), 
StartFloating Datetime null
GO
UPDATE [dbo].[Shifts] Set IsFloating = 0 WHERE  IsFloating is NULL
GO
ALTER TABLE [dbo].[sysroShiftTimeZones] ADD IsBlocked Bit null default(1)
GO
UPDATE [dbo].[sysroShiftTimeZones] Set IsBlocked = 1 WHERE  IsBlocked is NULL
GO
ALTER TABLE [dbo].[DailySchedule] ADD StartShiftUsed datetime null
GO
ALTER TABLE [dbo].[DailySchedule] ADD StartShift1 datetime null, StartShift2 datetime null, StartShift3 datetime null, StartShift4 datetime null
GO

ALTER TABLE dbo.Requests ADD
	StartShift datetime NULL
GO

ALTER VIEW [dbo].[sysroEmployeesShifts]
AS
SELECT dbo.Employees.ID AS IDEmployee, dbo.Employees.Name, dbo.sysroEmployeeGroups.IDGroup, 
	dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.Path, 
	dbo.Shifts.Name AS ShiftName, dbo.Shifts.ExpectedWorkingHours, dbo.DailySchedule.Date AS CurrentDate, 
	dbo.sysroEmployeeGroups.SecurityFlags, dbo.GetFullGroupPathName(dbo.sysroEmployeeGroups.IDGroup) AS FullGroupName, dbo.Shifts.IsFloating, dbo.Shifts.StartFloating,  CASE WHEN dbo.DailySchedule.Date <= GETDATE() 
                      THEN StartShiftUsed ELSE StartShift1 END AS StartFloatingOnDay
 
FROM dbo.DailySchedule 
	INNER JOIN dbo.Employees ON dbo.DailySchedule.IDEmployee = dbo.Employees.ID 
	INNER JOIN dbo.EmployeeContracts ON dbo.Employees.ID = dbo.EmployeeContracts.IDEmployee 
		AND dbo.DailySchedule.Date >= dbo.EmployeeContracts.BeginDate 
		AND dbo.DailySchedule.Date <= dbo.EmployeeContracts.EndDate 
	INNER JOIN dbo.sysroEmployeeGroups ON dbo.Employees.ID = dbo.sysroEmployeeGroups.IDEmployee 
		AND dbo.DailySchedule.Date >= dbo.sysroEmployeeGroups.BeginDate 
		AND dbo.DailySchedule.Date <= dbo.sysroEmployeeGroups.EndDate 
	LEFT OUTER JOIN dbo.Shifts ON dbo.Shifts.ID = (SELECT CASE WHEN Date<=GETDATE() THEN IDShiftUsed ELSE IDShift1 END FROM DailySchedule DS WHERE DS.Date = DailySchedule.Date AND DS.IDEmployee = DailySchedule.IDEmployee)
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='252' WHERE ID='DBVersion'
GO
