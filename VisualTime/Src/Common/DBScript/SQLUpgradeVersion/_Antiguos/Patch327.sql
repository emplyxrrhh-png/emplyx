-- Creación de indice para la gestión de solicitudes
CREATE NONCLUSTERED INDEX PX_TmpRequestPassportPermissions
ON [dbo].[TmpRequestPassportPermissions] ([IDRequest])
GO

-- Corrección vista días incompletos
ALTER view [dbo].[sysrovwIncompletedDays] 
AS
SELECT punchesInfo.pInCount, punchesInfo.pOutCount, punchesInfo.Name, punchesInfo.IDEmployee,punchesInfo.Date from(
	 SELECT ISNULL(pIn.pInCount,0) as pInCount, ISNULL(pOut.pOutCount,0) as pOutCount, ISNULL(pIn.Name,pOut.Name) as Name, Isnull(pIn.IDEmployee,pout.IDEmployee) as IDEmployee ,isnull(pIn.Date,pout.date) as Date FROM
		 (SELECT count(*) AS pInCount, Employees.Name, Punches.IDEmployee, Punches.ShiftDate AS Date
		 FROM Employees,  Punches
		 WHERE Employees.ID = Punches.IDEmployee 
		 AND (ActualType=1) 
		 GROUP BY Punches.ShiftDate, Employees.Name, Punches.IDEmployee) pIn
		FULL OUTER JOIN
		 (SELECT count(*)  AS pOutCount, Employees.Name, Punches.IDEmployee, Punches.ShiftDate AS Date
		 FROM Employees,  Punches
		 WHERE Employees.ID = Punches.IDEmployee 
		 AND (ActualType=2) 
		 GROUP BY Punches.ShiftDate, Employees.Name, Punches.IDEmployee) pOut
	 ON pIn.IDEmployee = pOut.IDEmployee AND pIn.Date = pOut.Date	 
	 ) punchesInfo
WHERE punchesInfo.pInCount <> punchesInfo.pOutCount
GO



-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='327' WHERE ID='DBVersion'
GO

