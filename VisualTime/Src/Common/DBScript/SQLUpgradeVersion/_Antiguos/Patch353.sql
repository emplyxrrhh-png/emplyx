ALTER TABLE dbo.punches ADD
	CreatedOn smalldatetime NOT NULL CONSTRAINT DF_Punches_CreatedOn_GETDATE DEFAULT GETDATE()
GO
UPDATE dbo.punches SET CreatedOn = Datetime WHERE CreatedOn > CONVERT(smalldatetime,DateTime) 
GO

ALTER TABLE dbo.Entries ADD
	CreatedOn smalldatetime NOT NULL CONSTRAINT DF_Entries_CreatedOn_GETDATE DEFAULT GETDATE()
GO
UPDATE dbo.Entries SET CreatedOn = Datetime WHERE CreatedOn > CONVERT(smalldatetime,DateTime) 
GO

ALTER TABLE dbo.InvalidEntries ADD
	CreatedOn smalldatetime NOT NULL CONSTRAINT DF_InvalidEntries_CreatedOn_GETDATE DEFAULT GETDATE()
GO

ALTER VIEW [dbo].[sysroScheduleCube4]
AS
SELECT     dbo.Punches.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee, 
                      dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Punches.ActualType AS PunchDirection, 
                      dbo.Punches.TypeData AS PunchIDTypeData, CASE WHEN dbo.Punches.TypeData <> 0 THEN dbo.Causes.Name ELSE '' END AS PunchTypeData, 
                      dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day, 
                      MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Year, (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) 
                      % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName, 
                      dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END AS IsInvalid, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails) 
                      AS Details, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, 
                      dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, dbo.Punches.IsNotReliable, dbo.Punches.Location AS PunchLocation, 
                      dbo.Punches.LocationZone AS PunchLocationZone, dbo.Punches.TimeZone AS PunchTimeZone, dbo.sysroPassports.Name AS EditorName
FROM         dbo.sysroEmployeeGroups INNER JOIN
                      dbo.Punches ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee AND dbo.Punches.ShiftDate >= dbo.sysroEmployeeGroups.BeginDate AND 
                      dbo.Punches.ShiftDate <= dbo.sysroEmployeeGroups.EndDate INNER JOIN
                      dbo.EmployeeContracts ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND 
                      dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                      dbo.Zones ON dbo.Punches.IDZone = dbo.Zones.ID LEFT OUTER JOIN
                      dbo.Terminals ON dbo.Punches.IDTerminal = dbo.Terminals.ID LEFT OUTER JOIN
                      dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID LEFT OUTER JOIN
                      dbo.sysroPassports ON dbo.Punches.IDPassport = dbo.sysroPassports.ID LEFT OUTER JOIN
                      dbo.Causes ON dbo.Punches.TypeData = dbo.Causes.ID
GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID, 
                      dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108), 
                      dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name, 
                      (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name, 
                      CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path, 
                      dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, 
                      dbo.Punches.IsNotReliable, dbo.Punches.Location, dbo.Punches.LocationZone, dbo.Punches.TimeZone, dbo.sysroPassports.Name, dbo.Punches.ActualType, 
                      dbo.Causes.Name, dbo.Punches.TypeData
HAVING      (dbo.Punches.Type = 1) OR
                      (dbo.Punches.Type = 2) OR
                      (dbo.Punches.Type = 3) OR
                      (dbo.Punches.Type = 7)

GO

UPDATE dbo.Concepts SET ViewInTerminals = 0
GO

ALTER TABLE dbo.AccessGroups ADD
	ShortName nvarchar(3) NULL
GO

UPDATE dbo.AccessGroups SET ShortName = RIGHT('000'+CONVERT(NVARCHAR(3),id),3) WHERE ShortName IS NULL
GO

ALTER TABLE dbo.Causes ADD
	BusinessGroup nvarchar(50) NULL
GO

-- Corrección de jerarquía de Zonas 
IF NOT EXISTS(SELECT * FROM dbo.Zones where Name = 'Mi Empresa')
BEGIN
	INSERT INTO dbo.Zones (ID, Name, IsWorkingZone) SELECT CASE 
		WHEN MAX(ID) IS NULL THEN 1 
		ELSE MAX(ID) + 1 
		END, 'Mi Empresa', 0 from Zones
	UPDATE dbo.Zones SET IDParent = (Select ID From dbo.Zones where Name = 'Mi Empresa')
	WHERE ID <> (Select ID From dbo.Zones where Name = 'Mi Empresa')
END

UPDATE dbo.sysroParameters SET Data='353' WHERE ID='DBVersion'
GO


