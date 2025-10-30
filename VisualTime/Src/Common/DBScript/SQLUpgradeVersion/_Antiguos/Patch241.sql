/* Modificación de la vista de visitas para el informe de emergencia */
ALTER view [dbo].[sysrovwVisitsInOutMoves] 
AS
SELECT vp.EmpVisitedId AS IDEmployee, MAX(vm.BeginTime) AS DateTime,
(SELECT top 1 sc.IDReader FROM dbo.sysrovwEmployeesInOutMoves sc WHERE vp.EmpVisitedId = sc.IDEmployee ORDER BY sc.DateTime DESC) AS IDReader,
'IN' AS Status, vs.ID AS VisitorID, vs.Name AS Visitor
FROM dbo.VisitPlan AS vp
	INNER JOIN dbo.Visitors AS vs ON vs.ID = vp.VisitorId 
	INNER JOIN dbo.VisitMoves AS vm ON vm.VisitPlanId = vp.ID
WHERE (vm.BeginTime IS NOT NULL)
GROUP BY vp.EmpVisitedId, vs.Name, vp.PlannedById, vs.ID
UNION
SELECT vp.EmpVisitedId AS IDEmployee, MAX(vm.EndTime) AS DateTime,
(SELECT top 1 sc.IDReader FROM dbo.sysrovwEmployeesInOutMoves sc WHERE vp.EmpVisitedId = sc.IDEmployee ORDER BY sc.DateTime DESC) AS IDReader,
'OUT' AS Status, vs.ID AS VisitorID, vs.Name AS Visitor
FROM dbo.VisitPlan AS vp 
	INNER JOIN dbo.Visitors AS vs ON vs.ID = vp.VisitorId 
	INNER JOIN dbo.VisitMoves AS vm ON vm.VisitPlanId = vp.ID
WHERE (vm.EndTime IS NOT NULL)
GROUP BY vp.EmpVisitedId, vs.Name, vp.PlannedById, vs.ID
GO

/* Soporte para 99 grupos de usuarios */
ALTER TABLE Groups ALTER COLUMN SecurityFlags nvarchar(99) NULL
GO
ALTER TABLE sysrogui ALTER COLUMN SecurityFlags nvarchar(99) NULL
GO
update groups set securityflags = left(securityflags + '111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111',99)
GO
update sysrogui set securityflags = left(securityflags + '111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111',99)
GO

DROP VIEW [dbo].[sysroEmployeeGroups]
GO

CREATE VIEW [dbo].[sysroEmployeeGroups]
AS
 SELECT     dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, 
                       dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName,
					   (SELECT CASE LEFT(Path,CHARINDEX('\',Path)) WHEN '' THEN Path ELSE CONVERT(INT,LEFT(Path,CHARINDEX('\',Path)-1)) END FROM Groups WHERE ID = (dbo.Groups.ID)) AS IDCompany
 FROM         dbo.Groups INNER JOIN
                       dbo.EmployeeGroups ON dbo.Groups.ID = dbo.EmployeeGroups.IDGroup
GO

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


ALTER VIEW [dbo].[sysrovwCurrentEmployeeGroups]
AS
SELECT     dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                      dbo.Employees.Name AS EmployeeName, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, 
                      COUNT(dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee) AS CurrentEmployee, dbo.Employees.AttControlled, dbo.Employees.AccControlled, 
                      dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, 
                      dbo.GetFullGroupPathName(dbo.EmployeeGroups.IDGroup) AS FullGroupName
FROM         dbo.EmployeeGroups INNER JOIN
                      dbo.Employees ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID INNER JOIN
                      dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                      dbo.sysrosubvwCurrentEmployeePeriod ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
GROUP BY dbo.Groups.Name, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                      dbo.Employees.Name, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, dbo.Employees.AttControlled, dbo.Employees.AccControlled, 
                      dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled
HAVING      (dbo.EmployeeGroups.EndDate >= CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102)) AND 
                      (dbo.EmployeeGroups.BeginDate <= CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102))

GO

ALTER VIEW [dbo].[sysrovwPastEmployeeGroups]
AS
SELECT     dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                      dbo.Employees.Name AS EmployeeName, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, 
                      COUNT(dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee) AS CurrentEmployee, dbo.Employees.AttControlled, dbo.Employees.AccControlled, 
                      dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, 
                      dbo.GetFullGroupPathName(dbo.EmployeeGroups.IDGroup) AS FullGroupName
FROM         dbo.EmployeeGroups INNER JOIN
                      dbo.Employees ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID INNER JOIN
                      dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                      dbo.sysrosubvwCurrentEmployeePeriod ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
GROUP BY dbo.Groups.Name, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                      dbo.Employees.Name, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, dbo.Employees.AttControlled, dbo.Employees.AccControlled, 
                      dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled
HAVING      (dbo.EmployeeGroups.EndDate < CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102))

GO


ALTER VIEW [dbo].[sysrovwEmployeesInAllGroups]
  AS
SELECT GroupName, sysrovwPastEmployeeGroups.Path, sysrovwPastEmployeeGroups.SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, CurrentEmployee, AttControlled, AccControlled, 
 	JobControlled, ExtControlled, RiskControlled, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName,
 		(SELECT CASE LEFT(Path,CHARINDEX('\',Path)) WHEN '' THEN Path ELSE CONVERT(INT,LEFT(Path,CHARINDEX('\',Path)-1)) END FROM Groups WHERE ID = (dbo.Groups.ID)) AS IDCompany
  FROM dbo.Groups        
 	inner JOIN dbo.sysrovwPastEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwPastEmployeeGroups.IDGroup
UNION
SELECT GroupName, sysrovwCurrentEmployeeGroups.Path, sysrovwCurrentEmployeeGroups.SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, CurrentEmployee, AttControlled, AccControlled, 
 	JobControlled, ExtControlled, RiskControlled, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName,
 		(SELECT CASE LEFT(Path,CHARINDEX('\',Path)) WHEN '' THEN Path ELSE CONVERT(INT,LEFT(Path,CHARINDEX('\',Path)-1)) END FROM Groups WHERE ID = (dbo.Groups.ID)) AS IDCompany
  FROM dbo.Groups        
 	inner JOIN dbo.sysrovwCurrentEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwCurrentEmployeeGroups.IDGroup
  UNION
  SELECT GroupName, sysrovwFutureEmployeeGroups.Path, sysrovwFutureEmployeeGroups.SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, CurrentEmployee, AttControlled, AccControlled, 
     JobControlled, ExtControlled, RiskControlled, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName,(SELECT CASE LEFT(Path,CHARINDEX('\',Path)) WHEN '' THEN Path ELSE CONVERT(INT,LEFT(Path,CHARINDEX('\',Path)-1)) END FROM Groups WHERE ID = (dbo.Groups.ID)) AS IDCompany
  FROM dbo.Groups
 	INNER JOIN dbo.sysrovwFutureEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwFutureEmployeeGroups.IDGroup

GO

ALTER VIEW [dbo].[sysrovwFutureEmployeeGroups]
AS
SELECT     dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                      dbo.Employees.Name AS EmployeeName, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, '0' AS CurrentEmployee, 
                      dbo.Employees.AttControlled, dbo.Employees.AccControlled, dbo.Employees.JobControlled, dbo.Employees.ExtControlled, 
                      dbo.Employees.RiskControlled, dbo.GetFullGroupPathName(dbo.EmployeeGroups.IDGroup) AS FullGroupName
FROM         dbo.EmployeeGroups INNER JOIN
                      dbo.Employees ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID INNER JOIN
                      dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                      dbo.sysrosubvwCurrentEmployeePeriod ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
WHERE     (dbo.EmployeeGroups.BeginDate > CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102)) AND (dbo.Employees.ID NOT IN
                          (SELECT     IDEmployee
                            FROM          dbo.sysrovwCurrentEmployeeGroups))

GO



ALTER VIEW [dbo].[sysrovwAllEmployeeGroups]
 AS
 SELECT GroupName, sysrovwCurrentEmployeeGroups.Path, sysrovwCurrentEmployeeGroups.SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, CurrentEmployee, AttControlled, AccControlled, 
	JobControlled, ExtControlled, RiskControlled, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName,
		(SELECT CASE LEFT(Path,CHARINDEX('\',Path)) WHEN '' THEN Path ELSE CONVERT(INT,LEFT(Path,CHARINDEX('\',Path)-1)) END FROM Groups WHERE ID = (dbo.Groups.ID)) AS IDCompany
 FROM dbo.Groups        
	inner JOIN dbo.sysrovwCurrentEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwCurrentEmployeeGroups.IDGroup
 UNION
 SELECT GroupName, sysrovwFutureEmployeeGroups.Path, sysrovwFutureEmployeeGroups.SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, CurrentEmployee, AttControlled, AccControlled, 
    JobControlled, ExtControlled, RiskControlled, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName,(SELECT CASE LEFT(Path,CHARINDEX('\',Path)) WHEN '' THEN Path ELSE CONVERT(INT,LEFT(Path,CHARINDEX('\',Path)-1)) END FROM Groups WHERE ID = (dbo.Groups.ID)) AS IDCompany
 FROM dbo.Groups
	INNER JOIN dbo.sysrovwFutureEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwFutureEmployeeGroups.IDGroup
GO

ALTER TABLE sysroUserTasks ALTER COLUMN SecurityFlags nvarchar(99) not null
GO



/* ***************************************************************************************************************************** */
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='241' WHERE ID='DBVersion'
GO
