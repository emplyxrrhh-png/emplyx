ALTER TABLE [dbo].[Punches] ADD ManuallyZone bit
GO

ALTER VIEW [dbo].[sysrovwCurrentEmployeesZoneStatus]
  AS
  SELECT p.IDEmployee, em.Name AS EmployeeName, p.DateTime, p.IDTerminal AS IDReader, p.IDZone, p.ActualType, dbo.Terminals.Type AS TerminalType, p.ManuallyZone, p.Location  
  FROM dbo.Punches AS p INNER JOIN
                 (SELECT dbo.Punches.IDEmployee, MAX(dbo.Punches.ID) AS idp
                 FROM    dbo.Punches INNER JOIN
                                  (SELECT IDEmployee, MAX(DateTime) AS dat
                                  FROM    dbo.Punches AS Punches_1
                                  WHERE (IDZone <> 0) AND (NOT (IDZone IS NULL))
                                  GROUP BY IDEmployee) AS maxidat ON dbo.Punches.IDEmployee = maxidat.IDEmployee AND dbo.Punches.DateTime = maxidat.dat
                 GROUP BY dbo.Punches.IDEmployee) AS maxPunch ON p.IDEmployee = maxPunch.IDEmployee AND p.ID = maxPunch.idp LEFT OUTER JOIN
             dbo.Zones AS zo ON p.IDZone = zo.ID LEFT OUTER JOIN
             dbo.Employees AS em ON p.IDEmployee = em.ID LEFT OUTER JOIN
             dbo.Terminals ON dbo.Terminals.ID = p.IDTerminal
WHERE (p.Type <> 6)
GO

ALTER TABLE [dbo].[TMPEmergencyBasic] ADD IDZone int
GO

ALTER TABLE [dbo].[TMPEmergencyBasic] ADD ManuallyZone bit
GO

ALTER TABLE [dbo].[TMPEmergencyBasic] ADD IsGeolocated bit
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO


UPDATE sysroParameters SET Data='558' WHERE ID='DBVersion'
GO