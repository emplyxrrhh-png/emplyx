CREATE PROCEDURE [dbo].[Report_saldos_con_detalle_chart]
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '0',
@IDConcept nvarchar(max) = '0',
@FechaInicio date = '20210301',
@FechaFin date = '20210301'
AS
WITH DetailedValues AS (
    SELECT cs.ShortName + ' - ' + CS.Name AS ConceptName,
           cs.ID AS ConceptID,
           ABS(SUM(ds.Value)) AS Value,
		   SUM(ds.Value) AS RealValue,		   
           IIF(SUM(ds.Value) < 0, 0, SUM(ds.Value)) AS PositiveValue		 
    FROM DailyAccruals ds
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
    WHERE CAST(ds.Date AS DATE) BETWEEN CAST(@FechaInicio AS DATE) AND CAST(@FechaFin AS DATE)
      AND ds.IDConcept IN (SELECT * FROM split(@IDConcept, ','))	  
    GROUP BY CS.ShortName, cs.Name, cs.ID	
),
TotalValue AS (
    SELECT SUM(Value) AS TotalPositiveValue
    FROM DetailedValues
)
SELECT ConceptName, PositiveValue
FROM (
    SELECT case when dv.RealValue < 0 then dv.ConceptName + ' ' + FORMAT(dv.RealValue, 'N1', 'es-ES') + ' (' + FORMAT(ROUND((dv.Value / tv.TotalPositiveValue) * 100, 2), 'N1', 'es-ES')+ ') %'
	else  dv.ConceptName + ' ' + FORMAT(dv.RealValue, 'N1', 'es-ES') + ' '  + FORMAT(ROUND((dv.Value / tv.TotalPositiveValue) * 100, 2), 'N1', 'es-ES')+ ' %' end AS ConceptName,
           dv.PositiveValue,
           1 AS OrderColumn
    FROM DetailedValues dv
    CROSS JOIN TotalValue tv
    UNION
    SELECT SHORTNAME + ' - ' + name + ' 0,0 0,0%' AS ConceptName, 0 AS PositiveValue, 1 AS OrderColumn
    FROM CONCEPTS 
    WHERE ID IN (SELECT * FROM split(@IDConcept, ',')) 
    AND ID NOT IN (SELECT CONCEPTID FROM DetailedValues)
    UNION
    SELECT 'TOTAL ' + FORMAT(sum(dv.Value), 'N1', 'es-ES') + ' 100,0%' AS ConceptName, 0 AS PositiveValue, 2 AS OrderColumn
	from DetailedValues as dv 
) AS CombinedResult
ORDER BY OrderColumn, ConceptName ASC;
RETURN NULL
GO





-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='851' WHERE ID='DBVersion'

GO
