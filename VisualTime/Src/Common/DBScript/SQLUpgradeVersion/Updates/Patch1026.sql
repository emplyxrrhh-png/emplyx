ALTER PROCEDURE [dbo].[Report_pagos_por_contrato]
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '0',
@startDate datetime2 = '20210301',
@endDate datetime2 = '20210301',
@groupConcepts nvarchar(max) = '0'
AS
SELECT (select name from sysroReportGroups where id = EmployeeConcepts.IDReportGroup) as NombreGrupo, EmployeeConcepts.FullGroupName, EmployeeConcepts.IdGroup, EmployeeConcepts.EmployeeName, EmployeeConcepts.IDEmployee, EmployeeConcepts.Position, EmployeeConcepts.IDConcept, EmployeeConcepts.ConceptName, EmployeeConcepts.ConceptType,
CASE WHEN ISNULL(AccrualsPrice.Price,'') = '' THEN 0 ELSE CONVERT(Numeric(18,3),AccrualsPrice.Price) END AS Price,
MAX(AccrualsPrice.Date) AS MaxDate, SUM(ISNULL(AccrualsPrice.Value,0)) AS Value, EmployeeConcepts.IDReportGroup,
AccrualsPrice.IDContract, AccrualsPrice.BeginDate, AccrualsPrice.EndDate
FROM
(
SELECT sysrovwAllEmployeeGroups.FullGroupName, sysrovwAllEmployeeGroups.IdGroup, sysrovwAllEmployeeGroups.EmployeeName, sysrovwAllEmployeeGroups.IDEmployee, Concepts.ID AS IDConcept, sysroReportGroupConcepts.Position, Concepts.Name AS ConceptName, Concepts.IDType AS ConceptType, sysroReportGroupConcepts.IDReportGroup
FROM Concepts 
INNER JOIN sysroReportGroupConcepts ON Concepts.ID = sysroReportGroupConcepts.IDConcept 
CROSS JOIN sysrovwAllEmployeeGroups
WHERE
sysrovwAllEmployeeGroups.BeginDate <= cast(getDate() as date) AND 
(sysroReportGroupConcepts.IDReportGroup IN (select * from split(@groupConcepts, ','))) AND
(sysrovwAllEmployeeGroups.IDEmployee IN (select * from split(@IDEmployee, ',')))
) AS EmployeeConcepts
LEFT OUTER JOIN
(
SELECT DailyAccruals.IDEmployee, DailyAccruals.IDConcept, DailyAccruals.Date,
CASE WHEN ISNULL(Concepts.FixedPay,0) <> 0 THEN
convert(nvarchar(100),Concepts.PayValue)
ELSE
dbo.GetValueFromEmployeeUserFieldValues(DailyAccruals.IDEmployee, SUBSTRING(Concepts.UsedField,5,100), DailyAccruals.Date)
END AS Price,
DailyAccruals.Value, EmployeeContracts.IDContract, EmployeeContracts.BeginDate, EmployeeContracts.EndDate
FROM DailyAccruals INNER JOIN Concepts ON DailyAccruals.IDConcept = Concepts.ID
inner join (select * from split(@IDEmployee,',')) tmp on DailyAccruals.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = DailyAccruals.IDEmployee and DailyAccruals.Date between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
INNER JOIN EmployeeContracts ON
DailyAccruals.IDEmployee = EmployeeContracts.IDEmployee AND
DailyAccruals.Date >= EmployeeContracts.BeginDate AND
DailyAccruals.Date <= EmployeeContracts.EndDate
--LEFT JOIN sysrovwAllEmployeeGroups eg ON eg.IDEmployee = DailyAccruals.IDEmployee AND cast(DailyAccruals.Date as date) between eg.BeginDate and eg.EndDate 
WHERE
cast(DailyAccruals.Date as date) BETWEEN cast(@startDate as date) AND cast(@endDate as date)
) AS AccrualsPrice
ON
EmployeeConcepts.IDEmployee = AccrualsPrice.IDEmployee AND EmployeeConcepts.IDConcept = AccrualsPrice.IDConcept
WHERE ISNULL(AccrualsPrice.Value,0) <> 0
GROUP BY EmployeeConcepts.FullGroupName, EmployeeConcepts.IdGroup, EmployeeConcepts.EmployeeName, EmployeeConcepts.IDEmployee, EmployeeConcepts.Position, EmployeeConcepts.IDConcept, EmployeeConcepts.ConceptName, EmployeeConcepts.ConceptType, Price, EmployeeConcepts.IDReportGroup,
AccrualsPrice.IDContract, AccrualsPrice.BeginDate, AccrualsPrice.EndDate
ORDER BY EmployeeConcepts.FullGroupName, EmployeeConcepts.EmployeeName, EmployeeConcepts.Position
RETURN NULL
GO


ALTER PROCEDURE [dbo].[Report_pagos_por_contrato_total_grupo]
@idPassport nvarchar(100) = '1',
@Employees nvarchar(max) = '0',
@startDate datetime2 = '20210301',
@endDate datetime2 = '20210301',
@IDReportGroup smallint = 0,
@IDGroup smallint = 0
AS
SELECT EmployeeConcepts.FullGroupName, EmployeeConcepts.Position, EmployeeConcepts.IDConcept, EmployeeConcepts.ConceptName,
SUM(
CASE WHEN ISNULL(AccrualsPrice.Price,'') = '' THEN 0 ELSE CONVERT(Numeric(18,3),AccrualsPrice.Price) END * ISNULL(AccrualsPrice.Value,0)
) AS ImporteGrupo
FROM
(
SELECT dbo.GetFullGroupPathName(sysrovwAllEmployeeGroups.IDGroup) AS FullGroupName, sysrovwAllEmployeeGroups.EmployeeName, sysrovwAllEmployeeGroups.IDEmployee, Concepts.ID AS IDConcept, sysroReportGroupConcepts.Position, Concepts.Name AS ConceptName
FROM Concepts INNER JOIN sysroReportGroupConcepts ON Concepts.ID = sysroReportGroupConcepts.IDConcept CROSS JOIN sysrovwAllEmployeeGroups
WHERE (sysroReportGroupConcepts.IDReportGroup = @IDReportGroup) AND
sysrovwAllEmployeeGroups.IDEmployee IN (select * from split(@Employees, ',')) AND
(sysrovwAllEmployeeGroups.IDGroup=@IDGroup)
) AS EmployeeConcepts
LEFT OUTER JOIN
(
SELECT sysrovwAllEmployeeGroups.IDGroup, DailyAccruals.IDEmployee, DailyAccruals.IDConcept,
CASE WHEN ISNULL(Concepts.FixedPay,0) <> 0 THEN convert(nvarchar(100),Concepts.PayValue)
ELSE dbo.GetValueFromEmployeeUserFieldValues(DailyAccruals.IDEmployee, SUBSTRING(Concepts.UsedField,5,100), DailyAccruals.Date) END AS Price,
DailyAccruals.Value
FROM DailyAccruals INNER JOIN Concepts ON DailyAccruals.IDConcept = Concepts.ID
inner join (select * from split(@Employees,',')) tmp on DailyAccruals.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = DailyAccruals.IDEmployee and DailyAccruals.Date between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
INNER JOIN sysrovwAllEmployeeGroups ON DailyAccruals.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee AND sysrovwAllEmployeeGroups.BeginDate <= cast(getDate() as date) 
WHERE
cast(DailyAccruals.Date as date) BETWEEN cast(@StartDate as date) AND cast(@EndDate as date)
) AS AccrualsPrice
ON
EmployeeConcepts.IDEmployee = AccrualsPrice.IDEmployee AND EmployeeConcepts.IDConcept = AccrualsPrice.IDConcept
WHERE ISNULL(AccrualsPrice.Value,0) <> 0
GROUP BY EmployeeConcepts.FullGroupName, EmployeeConcepts.Position, EmployeeConcepts.IDConcept, EmployeeConcepts.ConceptName
ORDER BY EmployeeConcepts.FullGroupName, EmployeeConcepts.Position
RETURN NULL
GO

ALTER PROCEDURE [dbo].[Report_pagos_por_contrato_total_report]
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '0',
@startDate datetime2 = '20210301',
@endDate datetime2 = '20210301',
@grupoSaldos nvarchar(max) = '0'
AS
SELECT EmployeeConcepts.Position, EmployeeConcepts.IDConcept, EmployeeConcepts.ConceptName,
SUM(
CASE WHEN ISNULL(AccrualsPrice.Price,'') = '' THEN 0 ELSE CONVERT(Numeric(18,3),AccrualsPrice.Price) END * ISNULL(AccrualsPrice.Value,0)
) AS ImporteGrupo
FROM
(
SELECT sysrovwAllEmployeeGroups.IDEmployee, Concepts.ID AS IDConcept, sysroReportGroupConcepts.Position, Concepts.Name AS ConceptName
FROM Concepts INNER JOIN sysroReportGroupConcepts ON Concepts.ID = sysroReportGroupConcepts.IDConcept CROSS JOIN sysrovwAllEmployeeGroups
WHERE (sysroReportGroupConcepts.IDReportGroup IN (select * from split(@grupoSaldos, ','))) AND
sysrovwAllEmployeeGroups.IDEmployee IN (select * from split(@IDEmployee, ','))
) AS EmployeeConcepts
LEFT OUTER JOIN
(
SELECT sysrovwAllEmployeeGroups.IDGroup, DailyAccruals.IDEmployee, DailyAccruals.IDConcept,
CASE WHEN ISNULL(Concepts.FixedPay,0) <> 0 THEN convert(nvarchar(100),Concepts.PayValue)
ELSE dbo.GetValueFromEmployeeUserFieldValues(DailyAccruals.IDEmployee, SUBSTRING(Concepts.UsedField,5,100), DailyAccruals.Date) END AS Price,
DailyAccruals.Value
FROM DailyAccruals INNER JOIN Concepts ON DailyAccruals.IDConcept = Concepts.ID
inner join (select * from split(@IDEmployee,',')) tmp on DailyAccruals.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = DailyAccruals.IDEmployee and DailyAccruals.Date between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
INNER JOIN sysrovwAllEmployeeGroups ON DailyAccruals.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee and sysrovwAllEmployeeGroups.BeginDate <= cast(getDate() as date) 
WHERE
DailyAccruals.Date BETWEEN @startdate AND @enddate
) AS AccrualsPrice
ON EmployeeConcepts.IDEmployee = AccrualsPrice.IDEmployee AND EmployeeConcepts.IDConcept = AccrualsPrice.IDConcept
WHERE ISNULL(AccrualsPrice.Value,0) <> 0
GROUP BY EmployeeConcepts.Position, EmployeeConcepts.IDConcept, EmployeeConcepts.ConceptName
ORDER BY  EmployeeConcepts.Position
RETURN NULL
GO


-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1026' WHERE ID='DBVersion'
GO
