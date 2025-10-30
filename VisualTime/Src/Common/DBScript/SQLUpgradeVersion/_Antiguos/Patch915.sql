CREATE PROCEDURE [dbo].[Report_saldos_dia_a_dia_chart]
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '1282',
@IDConcept nvarchar(max) = '17,12',
@FechaInicio date = '20240901',
@FechaFin date = '20240930'
AS
WITH DetailedValues AS (
    SELECT 
        cs.ShortName + ' - ' + cs.Name AS ConceptName,
        cs.ID AS ConceptID,
        ABS(SUM(ds.Value)) AS Value,
        SUM(ds.Value) AS RealValue,
        IIF(SUM(ds.Value) < 0, 0, SUM(ds.Value)) AS PositiveValue
    FROM 
        DailyAccruals ds
    INNER JOIN 
        (SELECT * FROM splitMAX(@IDEmployee, ',')) tmp ON ds.IDEmployee = tmp.Value
    INNER JOIN 
        sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK)
            ON poe.IDPassport = @idPassport
            AND poe.IDEmployee = ds.IDEmployee
            AND CAST(ds.Date AS DATE) BETWEEN poe.BeginDate AND poe.EndDate
    INNER JOIN 
        Employees emp ON emp.ID = ds.IDEmployee
    INNER JOIN 
        EmployeeContracts ec ON ec.IDEmployee = ds.IDEmployee
            AND ds.Date BETWEEN ec.BeginDate AND ec.EndDate
    INNER JOIN 
        EmployeeGroups eg ON eg.IDEmployee = ds.IDEmployee
            AND ds.Date BETWEEN eg.BeginDate AND eg.EndDate
    INNER JOIN 
        Groups g ON eg.IDGroup = g.ID
    LEFT JOIN 
        Concepts cs ON ds.IDConcept = cs.ID
    WHERE 
        CAST(ds.Date AS DATE) BETWEEN CAST(@FechaInicio AS DATE) AND CAST(@FechaFin AS DATE)
        AND EXISTS (
    SELECT 1 
    FROM sysroReportGroupConcepts 
    WHERE IDConcept = ds.IDConcept
    AND IDReportGroup IN (SELECT * FROM split(@IDConcept, ','))
)
    GROUP BY 
        cs.ShortName, cs.Name, cs.ID
),
TotalValue AS (
    SELECT 
        SUM(Value) AS TotalPositiveValue
    FROM 
        DetailedValues
)
SELECT 
    ConceptName, PositiveValue
FROM (
    SELECT 
        CASE 
            WHEN dv.RealValue < 0 THEN dv.ConceptName + ' ' + FORMAT(dv.RealValue, 'N1', 'es-ES') + ' (' + FORMAT(ROUND((dv.Value / tv.TotalPositiveValue) * 100, 2), 'N1', 'es-ES') + ') %'
            ELSE dv.ConceptName + ' ' + FORMAT(dv.RealValue, 'N1', 'es-ES') + ' '  + FORMAT(ROUND((dv.Value / tv.TotalPositiveValue) * 100, 2), 'N1', 'es-ES') + ' %' 
        END AS ConceptName,
        dv.PositiveValue,
        1 AS OrderColumn
    FROM 
        DetailedValues dv
    CROSS JOIN 
        TotalValue tv
    UNION
    SELECT 
        SHORTNAME + ' - ' + name + ' 0,0 0,0%' AS ConceptName, 0 AS PositiveValue, 1 AS OrderColumn
    FROM 
        Concepts
WHERE 
    EXISTS (
        SELECT 1 
        FROM sysroReportGroupConcepts 
        WHERE IDConcept = ID 
          AND IDReportGroup IN (SELECT * FROM split(@IDConcept, ','))
    )
    AND NOT EXISTS (
        SELECT 1 
        FROM DetailedValues dv
        JOIN sysroReportGroupConcepts src ON src.IDConcept = dv.ConceptID
        WHERE src.IDReportGroup IN (SELECT * FROM split(@IDConcept, ','))
    )
    UNION
    SELECT 
        'TOTAL ' + FORMAT(SUM(dv.Value), 'N1', 'es-ES') + ' 100,0%' AS ConceptName, 0 AS PositiveValue, 2 AS OrderColumn
    FROM 
        DetailedValues AS dv
) AS CombinedResult
ORDER BY 
    OrderColumn, ConceptName ASC;
RETURN NULL
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='915' WHERE ID='DBVersion'
GO
