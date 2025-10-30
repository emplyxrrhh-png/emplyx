ALTER VIEW [dbo].[sysrovwCurrentEmployeeGroups]
AS
SELECT        dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                          dbo.Employees.Name AS EmployeeName, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, 
                          COUNT(dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee) AS CurrentEmployee, dbo.Employees.AttControlled, dbo.Employees.AccControlled, 
                          dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, convert(nvarchar(300),FullGroupName) as FullGroupName, dbo.EmployeeGroups.IsTransfer
FROM            dbo.EmployeeGroups 
 INNER JOIN dbo.Employees 
ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID 
 INNER JOIN  dbo.Groups
ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID
LEFT OUTER JOIN dbo.sysrosubvwCurrentEmployeePeriod 
 ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
GROUP BY dbo.Groups.Name, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, dbo.Employees.Name, 
                          dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, dbo.Employees.AttControlled, dbo.Employees.AccControlled, dbo.Employees.JobControlled, 
                          dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, dbo.EmployeeGroups.IsTransfer, convert(nvarchar(300),FullGroupName) 
 HAVING        CONVERT(date, getdate()) between dbo.EmployeeGroups.BeginDate and dbo.EmployeeGroups.EndDate
GO

ALTER VIEW [dbo].[sysrovwFutureEmployeeGroups]
AS
SELECT        dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                          dbo.Employees.Name AS EmployeeName, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, '0' AS CurrentEmployee, dbo.Employees.AttControlled, 
                          dbo.Employees.AccControlled, dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, 
                          convert(nvarchar(300),FullGroupName) as FullGroupName, dbo.EmployeeGroups.IsTransfer
FROM            dbo.EmployeeGroups INNER JOIN
                          dbo.Employees ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID INNER JOIN
                          dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                          dbo.sysrosubvwCurrentEmployeePeriod ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
WHERE        dbo.EmployeeGroups.BeginDate > CONVERT(date, getdate())
GO

ALTER VIEW [dbo].[sysrovwAllEmployeeGroups]
AS
SELECT        geg.GroupName, sysrovwCurrentEmployeeGroups.Path, sysrovwCurrentEmployeeGroups.SecurityFlags, geg.IDEmployee, geg.IDGroup, EmployeeName, geg.RealBeginDate as BeginDate, geg.EndDate
, CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled
, convert(nvarchar(300),geg.FullGroupName) as FullGroupName, geg.IDCompany, 
    sysrovwCurrentEmployeeGroups.isTransfer,'ERROR' As CostCenter
FROM            dbo.sysrovwGetEmployeeGroup geg
INNER JOIN dbo.sysrovwCurrentEmployeeGroups
ON geg.IDGroup = dbo.sysrovwCurrentEmployeeGroups.IDGroup and geg.IDEmployee=dbo.sysrovwCurrentEmployeeGroups.IDEmployee
UNION
SELECT        geg.GroupName, sysrovwFutureEmployeeGroups.Path, sysrovwFutureEmployeeGroups.SecurityFlags, geg.IDEmployee, geg.IDGroup, EmployeeName, geg.RealBeginDate, geg.EndDate
, CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled
, convert(nvarchar(300),geg.FullGroupName) as FullGroupName, geg.IDCompany, 
    sysrovwFutureEmployeeGroups.isTransfer,'ERROR' As CostCenter
FROM            dbo.sysrovwGetEmployeeGroup geg
INNER JOIN dbo.sysrovwFutureEmployeeGroups 
 ON geg.IDgroup = dbo.sysrovwFutureEmployeeGroups.IDGroup and geg.IDEmployee=dbo.sysrovwFutureEmployeeGroups.IDEmployee
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='469' WHERE ID='DBVersion'
GO

