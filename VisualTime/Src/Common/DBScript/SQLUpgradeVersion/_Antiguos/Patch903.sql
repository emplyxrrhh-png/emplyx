ALTER PROCEDURE [dbo].[Report_monthly_accruals_by_contract_chart]
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '0',
@IDConcept1 nvarchar(max) = '0',
@IDConcept2 nvarchar(max) = '0',
@IDConcept3 nvarchar(max) = '0',
@IDConcept4 nvarchar(max) = '0',
@IDConcept5 nvarchar(max) = '0',
@IDConcept6 nvarchar(max) = '0',
@IDConcept7 nvarchar(max) = '0',
@IDConcept8 nvarchar(max) = '0',
@IDConcept9 nvarchar(max) = '0',
@IDConcept10 nvarchar(max) = '0',
@IDConcept11 nvarchar(max) = '0',
@IDConcept12 nvarchar(max) = '0',
@IDConcept13 nvarchar(max) = '0',
@IDConcept14 nvarchar(max) = '0',
@IDConcept15 nvarchar(max) = '0',
@monthStart nvarchar(2) = '01',
@yearStart nvarchar(4) = '2021',
@monthEnd nvarchar(2) = '04',
@yearEnd nvarchar(4) = '2021'
AS

SET @monthStart = FORMAT(CAST(@monthStart as int), '00');
SET @yearStart = FORMAT(CAST(@yearStart as int), '0000');
SET @monthEnd = FORMAT(CAST(@monthEnd as int), '00');
SET @yearEnd = FORMAT(CAST(@yearEnd as int), '0000');
DECLARE @StartDate date = DATEFROMPARTS(@yearStart,@monthStart,'01');
DECLARE @EndDate date = EOMONTH(DATEFROMPARTS(@yearEnd,@monthEnd,'01'));

WITH DetailedValues AS (
SELECT cs.ShortName + ' - ' + CS.Name AS ConceptName,
cs.ID AS ConceptID,
ABS(SUM(ds.Value)) AS Value,
SUM(ds.Value) AS RealValue,
IIF(SUM(ds.Value) < 0, 0, SUM(ds.Value)) AS PositiveValue
FROM sysroDailyAccrualsByContract ds
INNER JOIN (SELECT * FROM splitMAX(@IDEmployee, ',')) tmp ON ds.IDEmployee = tmp.Value
INNER JOIN sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK)
ON poe.IDPassport = @idPassport
AND poe.IDEmployee = ds.IDEmployee
AND CAST(ds.Date AS DATE) BETWEEN poe.BeginDate AND poe.EndDate
INNER JOIN Employees emp ON emp.ID = ds.IDEmployee
INNER JOIN EmployeeContracts ec ON ec.IDEmployee = ds.IDEmployee
AND ds.Date BETWEEN ec.BeginDate AND ec.EndDate
INNER JOIN EmployeeGroups eg ON eg.IDEmployee = ds.IDEmployee
AND ds.Date BETWEEN eg.BeginDate AND eg.EndDate
INNER JOIN Groups g ON eg.IDGroup = g.ID
LEFT JOIN Concepts cs ON ds.IDConcept = cs.ID
WHERE CAST(ds.Date AS DATE) BETWEEN CAST(@StartDate AS DATE) AND CAST(@EndDate AS DATE)
AND ds.IDConcept IN (@IDConcept1,@IDConcept2,@IDConcept3,@IDConcept4,@IDConcept5,@IDConcept6,@IDConcept7,@IDConcept8,@IDConcept9,@IDConcept10,@IDConcept11,@IDConcept12,@IDConcept13,@IDConcept14,@IDConcept15)
GROUP BY CS.ShortName, cs.Name, cs.ID
),
TotalValue AS (
SELECT SUM(Value) AS TotalPositiveValue
FROM DetailedValues
)
SELECT TOP(15) ConceptName, PositiveValue
FROM (
SELECT case when dv.RealValue < 0 then dv.ConceptName + ' ' + FORMAT(dv.RealValue, 'N2', 'es-ES') + ' (' + FORMAT(ROUND((dv.Value / tv.TotalPositiveValue) * 100, 2), 'N2', 'es-ES')+ ') %'
else  dv.ConceptName + ' ' + FORMAT(dv.RealValue, 'N2', 'es-ES') + ' '  + FORMAT(ROUND((dv.Value / tv.TotalPositiveValue) * 100, 2), 'N2', 'es-ES')+ ' %' end AS ConceptName,
dv.PositiveValue,
1 AS OrderColumn
FROM DetailedValues dv
CROSS JOIN TotalValue tv
UNION
SELECT SHORTNAME + ' - ' + name + ' 0,0 0,0%' AS ConceptName, 0 AS PositiveValue, 1 AS OrderColumn
FROM CONCEPTS
WHERE ID IN (@IDConcept1,@IDConcept2,@IDConcept3,@IDConcept4,@IDConcept5,@IDConcept6,@IDConcept7,@IDConcept8,@IDConcept9,@IDConcept10,@IDConcept11,@IDConcept12,@IDConcept13,@IDConcept14,@IDConcept15)
AND ID NOT IN (SELECT CONCEPTID FROM DetailedValues)
UNION
SELECT 'TOTAL ' + FORMAT(sum(dv.Value), 'N2', 'es-ES') + ' 100,0%' AS ConceptName, 0 AS PositiveValue, 2 AS OrderColumn
from DetailedValues as dv
) AS CombinedResult
ORDER BY OrderColumn, ConceptName ASC;
RETURN NULL
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='903' WHERE ID='DBVersion'
GO
