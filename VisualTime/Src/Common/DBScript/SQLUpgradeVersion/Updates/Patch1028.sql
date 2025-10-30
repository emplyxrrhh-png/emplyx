-- No borréis esta línea
-- Corrige el número de días en la línea de resumen
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
        g.FullGroupName AS Ruta
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
        g.FullGroupName
    ORDER BY 
        d.IDEmployee, 
        Month, 
        Year
    OPTION (MAXRECURSION 0, USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'))

    RETURN
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_calendario_anual_usuarios]
    @month nvarchar(2) = '06',
    @year nvarchar(4) = '2021',
    @employee nvarchar(max) = '16',
    @IDPassport nvarchar(100) = '0'
AS
    -- Validación de mes y año
    SET @month = CASE 
                    WHEN CAST(@month AS INT) < 1 THEN '01'
                    WHEN CAST(@month AS INT) > 12 THEN '12'
                    ELSE @month
                 END;
                 
    SET @year = CASE 
                   WHEN CAST(@year AS INT) < 1990 THEN '1990'
                   ELSE @year
                END;

    -- Generación de fechas
    DECLARE @StartDate DATE = DATEFROMPARTS(CAST(@year AS INT), CAST(@month AS INT), 1);
    DECLARE @EndDate DATE = EOMONTH(@StartDate);
    SET DATEFIRST 1;
    
    SET DATEFIRST 1;
    
    WITH Dates AS (
        SELECT 
            @StartDate as Fecha, 
            @employee as IDEmployee, 
            datepart(ww, @StartDate) as Week, 
            datepart(dw, @StartDate) as DayWeek, 
            datepart(dd, @StartDate) as Day, 
            datepart(mm, @StartDate) as Month, 
            NULL as NameShift, 
            NULL as Color, 
            NULL as Hours, 
            NULL as Absence, 
            NULL as Pending
        UNION ALL
        SELECT 
            DATEADD(DAY, 1, Fecha), 
            @employee as IDEmployee, 
            datepart(ww, DATEADD(DAY, 1, Fecha)) as Week, 
            datepart(dw, DATEADD(DAY, 1, Fecha)) as DayWeek, 
            datepart(dd, DATEADD(DAY, 1, Fecha)) as Day, 
            datepart(mm, DATEADD(DAY, 1, Fecha)) as Month, 
            NULL as NameShift,
            NULL as Color, 
            NULL as Hours, 
            NULL as Absence, 
            NULL as Pending
        FROM Dates
        WHERE DATEADD(DAY, 1, Fecha) <= @EndDate
    )
    SELECT * FROM (
        -- First query: Records from DailySchedule
        SELECT 
            Date as Fecha, 
            d.IDEmployee, 
            datepart(ww, Date) as Week, 
            datepart(dw, Date) as DayWeek, 
            datepart(dd, Date) as Day, 
            datepart(mm, Date) as Month, 
            shifts.Name as NameShift, 
            ISNULL(d.ShiftColor1, ISNULL(shifts.Color, 16777215)) as Color, 
            CASE 
                WHEN shifts.ShiftType = 2 THEN 0 
                ELSE ISNULL(d.ExpectedWorkingHours, shifts.ExpectedWorkingHours) 
            END AS Hours, 
            pa.AbsenceID as Absence, 
            r.Status as Pending,
			eg.IDGroup
        FROM DailySchedule as d
        INNER JOIN sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK)
            ON poe.IDPassport = @IDPassport
            AND poe.IDEmployee = d.IDEmployee
            AND d.Date BETWEEN poe.BeginDate AND poe.EndDate
        JOIN EmployeeContracts ec 
            ON ec.IDEmployee = d.IDEmployee
		LEFT JOIN EmployeeGroups eg 
			ON eg.IDEmployee = d.IDEmployee 
			AND d.Date BETWEEN eg.BeginDate AND eg.EndDate
        LEFT JOIN Shifts 
            ON (d.IDShift1 = Shifts.ID)
        LEFT JOIN ProgrammedAbsences as pa
            ON d.IDEmployee = pa.IDEmployee
            AND d.Date BETWEEN pa.BeginDate AND ISNULL(FinishDate, DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
        LEFT JOIN Requests as r 
            ON d.IDEmployee = r.IDEmployee
            AND r.Status IN (0, 1) 
            AND r.RequestType = 6 
            AND r.ID IN (
                SELECT IDRequest FROM sysroRequestDays WHERE Date = d.Date
            )
        WHERE d.IDEmployee = @employee 
            AND datepart(MM, Date) = CAST(@month AS INT) 
            AND datepart(YYYY, Date) = CAST(@year AS INT)
            
        UNION ALL
        
        -- Second query: Dates not in DailySchedule but within employment contract period
        SELECT 
            dat.Fecha, 
            dat.IDEmployee, 
            dat.Week, 
            dat.DayWeek, 
            dat.Day, 
            dat.Month, 
            CAST(dat.NameShift as nvarchar(max)) as NameShift, 
            16777215 as Color, 
            dat.Hours, 
            dat.Absence, 
            dat.Pending,
			eg.IDGroup
        FROM Dates dat
        INNER JOIN sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK)
            ON poe.IDPassport = @IDPassport
            AND poe.IDEmployee = dat.IDEmployee
            AND dat.Fecha BETWEEN poe.BeginDate AND poe.EndDate
        JOIN EmployeeContracts ec 
            ON ec.IDEmployee = dat.IDEmployee
		LEFT JOIN EmployeeGroups eg 
			ON eg.IDEmployee = ec.IDEmployee 
			AND dat.Fecha BETWEEN eg.BeginDate AND eg.EndDate
        WHERE dat.Fecha NOT IN (
                SELECT dsc.Date 
                FROM DailySchedule dsc 
                WHERE dsc.IDEmployee = @employee 
                    AND datepart(MM, dsc.Date) = CAST(@month AS INT)
                    AND datepart(YYYY, dsc.Date) = CAST(@year AS INT)
            )
            AND dat.fecha BETWEEN 
                (SELECT TOP 1 begindate FROM employeeContracts sec WHERE sec.IDEmployee = dat.IDEmployee ORDER BY BeginDate DESC) 
                AND 
                (SELECT TOP 1 EndDate FROM employeeContracts sec WHERE sec.IDEmployee = dat.IDEmployee ORDER BY BeginDate DESC)
        
        UNION ALL
        
        -- Third query: Dates not in DailySchedule and not within employment contract period
        SELECT 
            dat.Fecha, 
            dat.IDEmployee, 
            dat.Week, 
            dat.DayWeek, 
            dat.Day, 
            dat.Month, 
            CAST(dat.NameShift as nvarchar(max)) as NameShift, 
            NULL as Color, 
            dat.Hours, 
            dat.Absence, 
            dat.Pending,
			eg.IDGroup
        FROM Dates dat
        INNER JOIN sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK)
            ON poe.IDPassport = @IDPassport
            AND poe.IDEmployee = dat.IDEmployee
            AND dat.Fecha BETWEEN poe.BeginDate AND poe.EndDate
        JOIN EmployeeContracts ec 
            ON ec.IDEmployee = dat.IDEmployee
		LEFT JOIN EmployeeGroups eg 
			ON eg.IDEmployee = ec.IDEmployee
			AND dat.Fecha BETWEEN eg.BeginDate AND eg.EndDate
        WHERE dat.Fecha NOT IN (
                SELECT dsc.Date 
                FROM DailySchedule dsc 
                WHERE dsc.IDEmployee = @employee 
                    AND datepart(MM, dsc.Date) = CAST(@month AS INT)
                    AND datepart(YYYY, dsc.Date) = CAST(@year AS INT)
            )
            AND dat.fecha NOT BETWEEN 
                (SELECT TOP 1 begindate FROM employeeContracts sec WHERE sec.IDEmployee = dat.IDEmployee ORDER BY BeginDate DESC) 
                AND 
                (SELECT TOP 1 EndDate FROM employeeContracts sec WHERE sec.IDEmployee = dat.IDEmployee ORDER BY BeginDate DESC)
    ) t
    GROUP BY fecha, t.IDEmployee, Week, DayWeek, Day, Month, NameShift, Color, Hours, Absence, Pending, t.IDGroup
    ORDER BY IDEmployee, Fecha, Month, Day, DayWeek
    
    OPTION (USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'))
    
    RETURN NULL
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1028' WHERE ID='DBVersion'
GO
