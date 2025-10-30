CREATE   PROCEDURE [dbo].[Report_calendario_anual_usuarios_list_without_mobilities]
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
        ec.EndDate AS EndContract        
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
        ec.EndDate       
    ORDER BY 
        d.IDEmployee, 
        Month, 
        Year
    OPTION (MAXRECURSION 0, USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'))

    RETURN
END
GO

CREATE PROCEDURE [dbo].[Report_calendario_anual_usuarios_resumen_without_mobilities]
@monthStart nvarchar(2) = '1',
@yearStart nvarchar(4) = '2021',
@monthEnd nvarchar(2) = '04',
@yearEnd nvarchar(4) = '2021',
@IDEmployee nvarchar(max) = '777',
@IDPassport nvarchar(100) = '0'
AS
SET @monthStart = FORMAT(CAST(@monthStart as int), '00');
SET @yearStart = FORMAT(CAST(@yearStart as int), '0000');
SET @monthEnd = FORMAT(CAST(@monthEnd as int), '00');
SET @yearEnd = FORMAT(CAST(@yearEnd as int), '0000');
DECLARE @fechaStart date = DATEFROMPARTS(@yearStart,@monthStart,'01');
declare @fechaEnd datetime = EOMONTH(DATEFROMPARTS(@yearEnd,@monthEnd,'01'));
select ISNULL(ds.ShiftName1,s.Name) AS Name, s.ShortName, ISNULL(ds.ShiftColor1,s.Color) AS Color, COUNT(ISNULL(ds.IDShiftUsed,ds.IDShift1)) as Days, SUM(CASE WHEN s.ShiftType = 2 THEN 0 ELSE ISNULL(ds.ExpectedWorkingHours,s.ExpectedWorkingHours) end) as ExpectedWorkingHours, IIF(ISNULL(pa.AbsenceID,0)>0,1,0) as IsAbsence from DailySchedule ds
inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK)
On poe.IDPassport = @IDPassport
And poe.IDEmployee = ds.IDEmployee
And ds.Date between poe.BeginDate And poe.EndDate
inner join shifts s on (IDShiftUsed is not null and IDShiftUsed = s.ID) or (IDShift1 = s.ID)
inner join EmployeeGroups as eg on ds.IDEmployee = eg.IDEmployee and ds.Date between eg.BeginDate and eg.EndDate
and ds.Date between eg.BeginDate and eg.EndDate
left join ProgrammedAbsences as pa
on ds.IDEmployee = pa.IDEmployee
and ds.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
where ds.IDEmployee IN (select * from split(@IDEmployee,','))
and Date between @fechaStart and EOMONTH(@fechaEnd)
GROUP BY ISNULL(ds.ShiftName1,s.Name), s.ShortName, ISNULL(ds.ShiftColor1,s.Color), IIF(ISNULL(pa.AbsenceID,0)>0,1,0) 
option (USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'));
RETURN NULL
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1032' WHERE ID='DBVersion'
GO
