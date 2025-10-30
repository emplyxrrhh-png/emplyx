--Actualización de la vista de fichajes impares
 ALTER view [dbo].[sysrovwIncompletedDays] 
 AS
 SELECT punchesInfo.pInCount, punchesInfo.pOutCount, punchesInfo.Name, punchesInfo.IDEmployee,punchesInfo.Date from(
 SELECT pIn.pInCount, pOut.pOutCount, pIn.Name, pIn.IDEmployee,pIn.Date FROM
 (SELECT count(*) AS pInCount, Employees.Name, Punches.IDEmployee, Punches.ShiftDate AS Date
 FROM Employees,  Punches
 WHERE Employees.ID = Punches.IDEmployee 
  AND (ActualType=1) 
  GROUP BY Punches.ShiftDate, Employees.Name, Punches.IDEmployee) pIn,
  (SELECT count(*)  AS pOutCount, Employees.Name, Punches.IDEmployee, Punches.ShiftDate AS Date
 FROM Employees,  Punches
 WHERE Employees.ID = Punches.IDEmployee 
  AND (ActualType=2) 
  GROUP BY Punches.ShiftDate, Employees.Name, Punches.IDEmployee) pOut
  WHERE pIn.IDEmployee = pOut.IDEmployee and pIn.Date = pOut.Date)punchesInfo
  WHERE punchesInfo.pInCount <> punchesInfo.pOutCount
GO

-- Actualización de la vista sysrovwCurrentEmployeesZoneStatus
DROP VIEW [dbo].[sysrovwCurrentEmployeesZoneStatus]
GO

CREATE VIEW [dbo].[sysrovwCurrentEmployeesZoneStatus]
AS
SELECT        p.IDEmployee, em.Name AS EmployeeName, p.DateTime, p.IDTerminal AS IDReader, p.IDZone
FROM            dbo.Punches AS p INNER JOIN
                             (SELECT        dbo.Punches.IDEmployee, MAX(dbo.Punches.ID) AS idp
                               FROM            dbo.Punches INNER JOIN
                                                             (SELECT        IDEmployee, MAX(DateTime) AS dat
                                                               FROM            dbo.Punches AS Punches_1
                                                               WHERE        (IDZone <> 0) AND (NOT (IDZone IS NULL))
                                                               GROUP BY IDEmployee) AS maxidat ON dbo.Punches.IDEmployee = maxidat.IDEmployee AND dbo.Punches.DateTime = maxidat.dat
                               GROUP BY dbo.Punches.IDEmployee) AS maxPunch ON p.IDEmployee = maxPunch.IDEmployee AND p.ID = maxPunch.idp LEFT OUTER JOIN
                         dbo.Zones AS zo ON p.IDZone = zo.ID LEFT OUTER JOIN
                         dbo.Employees AS em ON p.IDEmployee = em.ID
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='324' WHERE ID='DBVersion'
GO

