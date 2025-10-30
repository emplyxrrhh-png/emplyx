-- Modificamos la vista de empleados en grupos futuros
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
WHERE     (dbo.EmployeeGroups.BeginDate > CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102))

GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='269' WHERE ID='DBVersion'
GO
