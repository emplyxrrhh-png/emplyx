/* ***************************************************************************************************************************** */
/* Modificación de la vista vista sysroEmployeeShifts porque fuera pasado, presente o futuro siempre comparaba el Shifts.ID con IDShiftUsed de la DailySchedule  */
ALTER VIEW [dbo].[sysroEmployeesShifts]
AS
SELECT dbo.Employees.ID AS IDEmployee, dbo.Employees.Name, dbo.sysroEmployeeGroups.IDGroup, 
	dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.Path, 
	dbo.Shifts.Name AS ShiftName, dbo.Shifts.ExpectedWorkingHours, dbo.DailySchedule.Date AS CurrentDate, 
	dbo.sysroEmployeeGroups.SecurityFlags, dbo.GetFullGroupPathName(dbo.sysroEmployeeGroups.IDGroup) AS FullGroupName
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

/* Modificación de la vista sysrovwCurrentEmployeesAccessStatus, ya que, era incorrecta */
ALTER view [dbo].[sysrovwCurrentEmployeesAccessStatus]
AS
  SELECT DISTINCT IDEmployee, EmployeeName,
   (SELECT TOP 1 DateTime FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee AND MoveType='Acc' ORDER BY DateTime DESC) AS DateTime,
   (SELECT TOP 1 IDReader FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee AND MoveType='Acc' ORDER BY DateTime DESC) AS IDReader,
   (SELECT TOP 1 IDZone FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee AND MoveType='Acc' ORDER BY DateTime DESC) AS IDZone,  
   (SELECT TOP 1 Status FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee AND MoveType='Acc' ORDER BY DateTime DESC) AS Status
FROM dbo.sysrovwCurrentEmployeeGroups sc
GO

-- Añadimos el campo que controla si el empleado se loguea por Active Directory en webTerminal y VisualTime Visitas
ALTER TABLE Employees ADD ActiveDirectory bit DEFAULT(0)
GO
UPDATE Employees SET ActiveDirectory = 0
GO

/* ***************************************************************************************************************************** */
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='238' WHERE ID='DBVersion'
GO
