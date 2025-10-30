ALTER VIEW [dbo].[sysrovwCurrentEmployeeGroups]
AS
SELECT        dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                         dbo.Employees.Name AS EmployeeName, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, 
                         COUNT(dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee) AS CurrentEmployee, dbo.Employees.AttControlled, dbo.Employees.AccControlled, 
                         dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, dbo.GetFullGroupPathName(dbo.EmployeeGroups.IDGroup) 
                         AS FullGroupName, dbo.EmployeeGroups.IsTransfer
FROM            dbo.EmployeeGroups INNER JOIN
                         dbo.Employees ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID INNER JOIN
                         dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                         dbo.sysrosubvwCurrentEmployeePeriod ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
GROUP BY dbo.Groups.Name, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, dbo.Employees.Name, 
                         dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, dbo.Employees.AttControlled, dbo.Employees.AccControlled, dbo.Employees.JobControlled, 
                         dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, dbo.EmployeeGroups.IsTransfer
HAVING        (dbo.EmployeeGroups.EndDate >= CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102)) AND 
                         (dbo.EmployeeGroups.BeginDate <= CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102))

GO

ALTER VIEW [dbo].[sysrovwFutureEmployeeGroups]
AS
SELECT        dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                         dbo.Employees.Name AS EmployeeName, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, '0' AS CurrentEmployee, dbo.Employees.AttControlled, 
                         dbo.Employees.AccControlled, dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, 
                         dbo.GetFullGroupPathName(dbo.EmployeeGroups.IDGroup) AS FullGroupName, dbo.EmployeeGroups.IsTransfer
FROM            dbo.EmployeeGroups INNER JOIN
                         dbo.Employees ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID INNER JOIN
                         dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                         dbo.sysrosubvwCurrentEmployeePeriod ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
WHERE        (dbo.EmployeeGroups.BeginDate > CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102))

GO

ALTER VIEW [dbo].[sysrovwAllEmployeeGroups]
AS
SELECT        GroupName, sysrovwCurrentEmployeeGroups.Path, sysrovwCurrentEmployeeGroups.SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName, (SELECT CASE LEFT(Path, CHARINDEX('\', Path)) WHEN '' THEN Path ELSE CONVERT(INT, LEFT(Path, CHARINDEX('\', Path) - 1)) END FROM Groups WHERE ID = (dbo.Groups.ID)) AS IDCompany, 
   sysrovwCurrentEmployeeGroups.isTransfer, dbo.GetEmployeeBusinessCenterNameOnDate(sysrovwCurrentEmployeeGroups.IDEmployee,sysrovwCurrentEmployeeGroups.BeginDate) As CostCenter
FROM            dbo.Groups INNER JOIN dbo.sysrovwCurrentEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwCurrentEmployeeGroups.IDGroup
UNION
SELECT        GroupName, sysrovwFutureEmployeeGroups.Path, sysrovwFutureEmployeeGroups.SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, 
   CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName, (SELECT CASE LEFT(Path, CHARINDEX('\', Path)) WHEN '' THEN Path ELSE CONVERT(INT, LEFT(Path, CHARINDEX('\', Path) - 1)) END FROM Groups WHERE ID = (dbo.Groups.ID)) AS IDCompany,
   sysrovwFutureEmployeeGroups.isTransfer, dbo.GetEmployeeBusinessCenterNameOnDate(sysrovwFutureEmployeeGroups.IDEmployee,sysrovwFutureEmployeeGroups.BeginDate)  As CostCenter
FROM            dbo.Groups INNER JOIN dbo.sysrovwFutureEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwFutureEmployeeGroups.IDGroup

GO

UPDATE dbo.sysroParameters SET Data='362' WHERE ID='DBVersion'
GO