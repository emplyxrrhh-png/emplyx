CREATE OR ALTER PROCEDURE [dbo].[Report_calendario_anual_usuarios_list]
    @idPassport    NVARCHAR(MAX) = '1024',
    @monthStart    NVARCHAR(2) = '01',
    @yearStart     NVARCHAR(4) = '2021',
    @monthEnd      NVARCHAR(2) = '04',
    @yearEnd       NVARCHAR(4) = '2021',
    @employees     NVARCHAR(MAX) = '1,2'
AS
BEGIN
    -- Normalización de parámetros
    SET @monthStart = FORMAT(CAST(@monthStart AS INT), '00');
    SET @yearStart  = FORMAT(CAST(@yearStart  AS INT), '0000');
    SET @monthEnd   = FORMAT(CAST(@monthEnd   AS INT), '00');
    SET @yearEnd    = FORMAT(CAST(@yearEnd    AS INT), '0000');

    DECLARE @StartDate DATE = DATEFROMPARTS(CAST(@yearStart AS INT), CAST(@monthStart AS INT), 1);
    DECLARE @EndDate   DATE = EOMONTH(DATEFROMPARTS(CAST(@yearEnd AS INT), CAST(@monthEnd AS INT), 1));

    -- Generador de fechas
    WITH Dates AS (
        SELECT 
            @StartDate AS Fecha
        UNION ALL
        SELECT DATEADD(DAY, 1, Fecha)
        FROM Dates
        WHERE DATEADD(DAY, 1, Fecha) <= @EndDate
    )

    -- Consulta principal
    SELECT 
		d.IDEmployee,
        COUNT(
            CASE 
                WHEN ISNULL(d.IDShiftUsed, d.IDShift1) IS NOT NULL THEN 1
                ELSE NULL
            END
        ) AS CountDays,
        DATEPART(MM, d.Date) AS Month,
        DATEPART(YYYY, d.Date) AS Year,
        DATEPART(DD, EOMONTH(d.Date)) AS LastDayMonth,
        SUM(CASE 
                WHEN s.ShiftType = 2 THEN 0 
                ELSE ISNULL(d.ExpectedWorkingHours, s.ExpectedWorkingHours) 
            END) AS Hours,
        e.Name AS EmployeeName,
        ec.IDContract AS Contract,
        ec.BeginDate AS BeginContract,
        ec.EndDate AS EndContract,
        g.Name AS GroupName,
		g.Id AS IdGroup,
        g.FullGroupName AS Ruta,
		eg.BeginDate As StartGroupDate
    FROM Dates dt
    LEFT JOIN (
        SELECT ds.*
        FROM DailySchedule ds
        INNER JOIN split(@employees, ',') empFilter ON ds.IDEmployee = empFilter.Value
        INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe 
            ON poe.IdEmployee = ds.IDEmployee
            AND ds.Date BETWEEN poe.BeginDate AND poe.EndDate
            AND poe.IdFeature = 2
            AND poe.FeaturePermission > 1
            AND poe.IdPassport = @idPassport
    ) AS d ON dt.Fecha = d.Date
    LEFT JOIN Shifts s 
        ON (d.IDShiftUsed IS NOT NULL AND d.IDShiftUsed = s.ID) 
        OR (d.IDShift1 = s.ID)
    LEFT JOIN Employees e ON e.ID = d.IDEmployee
    LEFT JOIN EmployeeContracts ec 
        ON ec.IDEmployee = e.ID 
        AND dt.Fecha BETWEEN ec.BeginDate AND ec.EndDate
    LEFT JOIN EmployeeGroups eg 
        ON eg.IDEmployee = e.ID 
        AND dt.Fecha BETWEEN eg.BeginDate AND eg.EndDate
    LEFT JOIN Groups g ON eg.IDGroup = g.ID
    WHERE 
        d.Date BETWEEN @StartDate AND @EndDate
    GROUP BY 
        d.IDEmployee,
        DATEPART(MM, d.Date),
        DATEPART(YYYY, d.Date),
        DATEPART(DD, EOMONTH(d.Date)),
        e.Name,
        ec.IDContract,
        ec.BeginDate,
        ec.EndDate,
        g.ID,
        g.Name,
        g.FullGroupName,
		eg.BeginDate
    ORDER BY 
        d.IDEmployee, 
        Month, 
        Year
    OPTION (MAXRECURSION 0, USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'))

    RETURN
END
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1055' WHERE ID='DBVersion'
GO
