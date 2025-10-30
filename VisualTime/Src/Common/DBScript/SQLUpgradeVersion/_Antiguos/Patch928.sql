ALTER PROCEDURE [dbo].[Report_quadrant_by_schedule_shifts]
@GivenDate DATETIME2 = '20241014',
@Shifts NVARCHAR(MAX) = '0',
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '0'
AS
BEGIN    

SET DATEFIRST 1;

DECLARE @xmlData XML;
DECLARE @weekPeriod INT;

SELECT @xmlData = Data
FROM sysroParameters
WHERE [ID] = 'OPTIONS'; 

SET @weekPeriod = @xmlData.value('(/roCollection/Item[@key="WeekPeriod"])[1]', 'INT');

DECLARE @FirstDayOfWeek DATE = DATEADD(DAY, -((DATEPART(WEEKDAY, @GivenDate) - @weekPeriod + 7) % 7), @GivenDate);


SELECT         
	distinct s.Name, s.ID
FROM 
    (SELECT 1 AS Number UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7) AS Numbers
	inner join DailySchedule as ds on ds.Date = DATEADD(DAY, Number - 1, @FirstDayOfWeek) and IDEmployee in (select * from split(@IDEmployee,',')) 
	inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = ds.IDEmployee and ds.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 2 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
	inner join shifts as s on s.id = ds.IDShift1
	INNER JOIN EmployeeContracts AS EC ON EC.IDEmployee = DS.IDEMPLOYEE AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate
	where ds.IDShift1 in (select * from split(@Shifts,',')) 
	order by s.Name
	RETURN NULL
END
GO

ALTER PROCEDURE [dbo].[Report_quadrant_by_schedule_total]
@GivenDate DATETIME2 = '20241014',
    @idPassport NVARCHAR(100) = '1',
    @IDEmployee NVARCHAR(MAX) = '0',
	@Shifts NVARCHAR(MAX) = '0'
AS
BEGIN 

SET DATEFIRST 1;

DECLARE @xmlData XML;
    DECLARE @weekPeriod INT;

    SELECT @xmlData = Data
    FROM sysroParameters
    WHERE [ID] = 'OPTIONS'; 

    SET @weekPeriod = @xmlData.value('(/roCollection/Item[@key="WeekPeriod"])[1]', 'INT');

    DECLARE @FirstDayOfWeek DATE = DATEADD(DAY, -((DATEPART(WEEKDAY, @GivenDate) - @weekPeriod + 7) % 7), @GivenDate);

    -- Crear una tabla temporal para almacenar los datos
CREATE TABLE #TempSchedule (
    Number INT,
    WeekDay DATE
);

-- Insertar los datos en la tabla temporal
INSERT INTO #TempSchedule (Number, WeekDay)
SELECT 
    Number,
    DATEADD(DAY, Number - 1, @FirstDayOfWeek) AS WeekDay
FROM 
    (SELECT 1 AS Number UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7) AS Numbers;

DECLARE @WEEKDAY1 AS DATETIME = (SELECT 
    MAX(CASE WHEN Number = 1 THEN WeekDay END) 
FROM #TempSchedule)

DECLARE @WEEKDAY2 AS DATETIME = (SELECT 
    MAX(CASE WHEN Number = 2 THEN WeekDay END) 
FROM #TempSchedule)

DECLARE @WEEKDAY3 AS DATETIME = (SELECT 
    MAX(CASE WHEN Number = 3 THEN WeekDay END) 
FROM #TempSchedule)

DECLARE @WEEKDAY4 AS DATETIME = (SELECT 
    MAX(CASE WHEN Number = 4 THEN WeekDay END) 
FROM #TempSchedule)

DECLARE @WEEKDAY5 AS DATETIME = (SELECT 
    MAX(CASE WHEN Number = 5 THEN WeekDay END) 
FROM #TempSchedule)

DECLARE @WEEKDAY6 AS DATETIME = (SELECT 
    MAX(CASE WHEN Number = 6 THEN WeekDay END) 
FROM #TempSchedule)

DECLARE @WEEKDAY7 AS DATETIME = (SELECT 
    MAX(CASE WHEN Number = 7 THEN WeekDay END) 
FROM #TempSchedule)

DROP TABLE #TempSchedule

SELECT             
    SUM(CASE WHEN DS.Date = @WeekDay1 THEN 1 ELSE 0 END) AS WEEKDAY1,
    SUM(CASE WHEN DS.Date = @WeekDay2 THEN 1 ELSE 0 END) AS WEEKDAY2,
    SUM(CASE WHEN DS.Date = @WeekDay3 THEN 1 ELSE 0 END) AS WEEKDAY3,
    SUM(CASE WHEN DS.Date = @WeekDay4 THEN 1 ELSE 0 END) AS WEEKDAY4,
    SUM(CASE WHEN DS.Date = @WeekDay5 THEN 1 ELSE 0 END) AS WEEKDAY5,
    SUM(CASE WHEN DS.Date = @WeekDay6 THEN 1 ELSE 0 END) AS WEEKDAY6,
    SUM(CASE WHEN DS.Date = @WeekDay7 THEN 1 ELSE 0 END) AS WEEKDAY7
FROM 
    DailySchedule DS
    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe 
        ON poe.IdEmployee = DS.IDEmployee 
        AND DS.Date BETWEEN poe.BeginDate AND poe.EndDate 
        AND poe.IdFeature = 2 
        AND poe.FeaturePermission > 1 
        AND poe.IdPassport = @idPassport
    INNER JOIN Employees E 
        ON E.ID = DS.IDEmployee
		INNER JOIN EmployeeContracts AS EC ON EC.IDEmployee = E.ID AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate
WHERE 
    DS.IDShift1 IN (SELECT * FROM dbo.SplitString(@Shifts, ','))
    AND DS.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))

	RETURN NULL
END
GO

ALTER PROCEDURE [dbo].[Report_quadrant_by_schedule_weekDays]
@GivenDate DATETIME2 = '20241014'
AS
BEGIN
	SET DATEFIRST 1;
    DECLARE @xmlData XML;
    DECLARE @weekPeriod INT;

    SELECT @xmlData = Data
    FROM sysroParameters
    WHERE [ID] = 'OPTIONS'; 

    SET @weekPeriod = @xmlData.value('(/roCollection/Item[@key="WeekPeriod"])[1]', 'INT');

    DECLARE @FirstDayOfWeek DATE = DATEADD(DAY, -((DATEPART(WEEKDAY, @GivenDate) - @weekPeriod + 7) % 7), @GivenDate);

    -- Crear una tabla temporal para almacenar los datos
CREATE TABLE #TempSchedule (
    Number INT,
    WeekDay DATE
);

-- Insertar los datos en la tabla temporal
INSERT INTO #TempSchedule (Number, WeekDay)
SELECT 
    Number,
    DATEADD(DAY, Number - 1, @FirstDayOfWeek) AS WeekDay
FROM 
    (SELECT 1 AS Number UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7) AS Numbers;

-- Pivotar los datos
SELECT 
    MAX(CASE WHEN Number = 1 THEN WeekDay END) AS WeekDay1,
    MAX(CASE WHEN Number = 2 THEN WeekDay END) AS WeekDay2,
    MAX(CASE WHEN Number = 3 THEN WeekDay END) AS WeekDay3,
    MAX(CASE WHEN Number = 4 THEN WeekDay END) AS WeekDay4,
    MAX(CASE WHEN Number = 5 THEN WeekDay END) AS WeekDay5,
    MAX(CASE WHEN Number = 6 THEN WeekDay END) AS WeekDay6,
    MAX(CASE WHEN Number = 7 THEN WeekDay END) AS WeekDay7	
FROM #TempSchedule

-- Eliminar la tabla temporal
DROP TABLE #TempSchedule;


	RETURN NULL
END
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='928' WHERE ID='DBVersion'
GO
