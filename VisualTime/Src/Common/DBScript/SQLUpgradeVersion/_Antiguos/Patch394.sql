ALTER VIEW [dbo].[sysroEmployeesShifts]
 AS
 SELECT dbo.Employees.ID AS IDEmployee, dbo.Employees.Name, dbo.sysroEmployeeGroups.IDGroup, 
 	dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.Path, 
 	dbo.Shifts.ID AS IDShift , dbo.Shifts.Name AS ShiftName, (case when isnull(dbo.DailySchedule.IsHolidays,0) = 1 then 0 else isnull(dbo.DailySchedule.ExpectedWorkingHours, dbo.Shifts.ExpectedWorkingHours)  end) as ExpectedWorkingHours, dbo.DailySchedule.Date AS CurrentDate, 
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

UPDATE dbo.sysroParameters SET Data='394' WHERE ID='DBVersion'
GO
