  ALTER VIEW [dbo].[sysrovwEmployeesAccessAuthorizationsOnTerminals]
  AS
 	SELECT dbo.Employees.ID as EmployeeID, TerminalReaders.IDTerminal, TerminalReaders.id IDReader
 	FROM Employees
 	INNER JOIN EmployeeContracts On EmployeeContracts.IDEmployee = Employees.ID
 	And  dbo.EmployeeContracts.BeginDate < GETDATE() And DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())) <= dbo.EmployeeContracts.EndDate
 	LEFT OUTER JOIN sysrovwAccessAuthorizations On sysrovwAccessAuthorizations.IDEmployee = Employees.ID
 	INNER JOIN (Select DISTINCT IDAccessGroup, IDZone  from AccessGroupsPermissions, sysrovwAccessAuthorizations where AccessGroupsPermissions.IDAccessGroup = sysrovwAccessAuthorizations.IDAuthorization) AGP On sysrovwAccessAuthorizations.IDAuthorization=AGP.IDAccessGroup 
 	INNER JOIN TerminalReaders On TerminalReaders.IDZone=AGP.IDZone
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='454' WHERE ID='DBVersion'
GO
