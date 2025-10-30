
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
(DS.IDShift1 IN (SELECT * FROM dbo.SplitString(@Shifts, ',')) OR ds.IDShiftBase IN (SELECT * FROM dbo.SplitString(@Shifts, ',')))
AND DS.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))
RETURN NULL
END
GO


ALTER PROCEDURE [dbo].[Report_quadrant_by_schedule_presence]
@WeekDay1 DATETIME2 = '20241218',
@WeekDay2 DATETIME2 = '20241219',
@WeekDay3 DATETIME2 = '20241220',
@WeekDay4 DATETIME2 = '20241221',
@WeekDay5 DATETIME2 = '20241222',
@WeekDay6 DATETIME2 = '20241223',
@WeekDay7 DATETIME2 = '20241224',
@IDShift NVARCHAR(100) = '38',
@idPassport NVARCHAR(100) = '1',
@IDEmployee NVARCHAR(MAX) = N'1'
AS
BEGIN

-- Empleados que tienen planificado el horario al menos un día del periodo
SELECT DISTINCT CASE WHEN DS1.DATE IS NOT NULL THEN E.NAME ELSE NULL END AS WEEKDAY1, CASE WHEN DS2.DATE IS NOT NULL THEN E.NAME ELSE NULL END AS WEEKDAY2,
CASE WHEN DS3.DATE IS NOT NULL THEN E.NAME ELSE NULL END AS WEEKDAY3, CASE WHEN DS4.DATE IS NOT NULL THEN E.NAME ELSE NULL END AS WEEKDAY4,
CASE WHEN DS5.DATE IS NOT NULL THEN E.NAME ELSE NULL END AS WEEKDAY5, CASE WHEN DS6.DATE IS NOT NULL THEN E.NAME ELSE NULL END AS WEEKDAY6,
CASE WHEN DS7.DATE IS NOT NULL THEN E.NAME ELSE NULL END AS WEEKDAY7, S.Name AS ShiftName,
CASE WHEN PA1.IDEmployee IS NULL
AND (DS1.IsHolidays = 0 or DS1.IsHolidays is null)
AND (s.ShiftType <> 2)
AND (S.ExpectedWorkingHours > 0 OR DS1.Date IS NULL)
AND (PH1.AllDay IS NULL OR PH1.AllDay = 0 Or PC1.AbsenceID <> NULL)
THEN NULL ELSE 1 END AS ISABSENT1,
CASE WHEN PA2.IDEmployee IS NULL
AND (DS2.IsHolidays = 0 or DS2.IsHolidays is null)
AND (s.ShiftType <> 2)
AND (S.ExpectedWorkingHours > 0 OR DS2.Date IS NULL)
AND (PH2.AllDay IS NULL OR PH2.AllDay = 0 Or PC2.AbsenceID <> NULL)
THEN NULL ELSE 1 END AS ISABSENT2,
CASE WHEN PA3.IDEmployee IS NULL
AND (DS3.IsHolidays = 0 or DS3.IsHolidays is null)
AND (s.ShiftType <> 2)
AND (S.ExpectedWorkingHours > 0 OR DS3.Date IS NULL)
AND (PH3.AllDay IS NULL OR PH3.AllDay = 0 Or PC3.AbsenceID <> NULL)
THEN NULL ELSE 1 END AS ISABSENT3,
CASE WHEN PA4.IDEmployee IS NULL
AND (DS4.IsHolidays = 0 or DS4.IsHolidays is null)
AND (s.ShiftType <> 2)
AND (S.ExpectedWorkingHours > 0 OR DS4.Date IS NULL)
AND (PH4.AllDay IS NULL OR PH4.AllDay = 0 Or PC4.AbsenceID <> NULL)
THEN NULL ELSE 1 END AS ISABSENT4,
CASE WHEN PA5.IDEmployee IS NULL
AND (DS5.IsHolidays = 0 or DS5.IsHolidays is null)
AND (s.ShiftType <> 2)
AND (S.ExpectedWorkingHours > 0 OR DS5.Date IS NULL)
AND (PH5.AllDay IS NULL OR PH5.AllDay = 0 Or PC5.AbsenceID <> NULL)
THEN NULL ELSE 1 END AS ISABSENT5,
CASE WHEN PA6.IDEmployee IS NULL
AND (DS6.IsHolidays = 0 or DS6.IsHolidays is null)
AND (s.ShiftType <> 2)
AND (S.ExpectedWorkingHours > 0 OR DS6.Date IS NULL)
AND (PH6.AllDay IS NULL OR PH6.AllDay = 0 Or PC6.AbsenceID <> NULL)
THEN NULL ELSE 1 END AS ISABSENT6,
CASE WHEN PA7.IDEmployee IS NULL
AND (DS7.IsHolidays = 0 or DS7.IsHolidays is null)
AND (s.ShiftType <> 2)
AND (S.ExpectedWorkingHours > 0 OR DS7.Date IS NULL)
AND (PH7.AllDay IS NULL OR PH7.AllDay = 0 Or PC7.AbsenceID <> NULL)
THEN NULL ELSE 1 END AS ISABSENT7,
CASE WHEN DS1.IsHolidays = 1
THEN 1 ELSE NULL END AS ISHOLIDAYS1,
CASE WHEN DS2.IsHolidays = 1
THEN 1 ELSE NULL END AS ISHOLIDAYS2,
CASE WHEN DS3.IsHolidays = 1
THEN 1 ELSE NULL END AS ISHOLIDAYS3,
CASE WHEN DS4.IsHolidays = 1
THEN 1 ELSE NULL END AS ISHOLIDAYS4,
CASE WHEN DS5.IsHolidays = 1
THEN 1 ELSE NULL END AS ISHOLIDAYS5,
CASE WHEN DS6.IsHolidays = 1
THEN 1 ELSE NULL END AS ISHOLIDAYS6,
CASE WHEN DS7.IsHolidays = 1
THEN 1 ELSE NULL END AS ISHOLIDAYS7,
CASE WHEN PH1.AllDay = 0 Or PC1.AbsenceID IS NOT NULL THEN 1 ELSE NULL END AS ISABSENTHOURS1,
CASE WHEN PH2.AllDay = 0 Or PC2.AbsenceID IS NOT NULL THEN 2 ELSE NULL END AS ISABSENTHOURS2,
CASE WHEN PH3.AllDay = 0 Or PC3.AbsenceID IS NOT NULL THEN 3 ELSE NULL END AS ISABSENTHOURS3,
CASE WHEN PH4.AllDay = 0 Or PC4.AbsenceID IS NOT NULL THEN 4 ELSE NULL END AS ISABSENTHOURS4,
CASE WHEN PH5.AllDay = 0 Or PC5.AbsenceID IS NOT NULL THEN 5 ELSE NULL END AS ISABSENTHOURS5,
CASE WHEN PH6.AllDay = 0 Or PC6.AbsenceID IS NOT NULL THEN 6 ELSE NULL END AS ISABSENTHOURS6,
CASE WHEN PH7.AllDay = 0 Or PC7.AbsenceID IS NOT NULL THEN 7 ELSE NULL END AS ISABSENTHOURS7,
E.Name,
CASE WHEN S.ShiftType <> 2 THEN 1 ELSE NULL END AS ISWORKINGSHIFT
FROM DailySchedule AS DS
INNER JOIN EMPLOYEES AS E ON E.ID = DS.IDEmployee
INNER JOIN SHIFTS AS S2 ON S2.ID = @IDShift AND S2.ShiftType <> 2
INNER JOIN SHIFTS AS S ON (S.ID = DS.IDShift1 and ds.IDShiftBase is null) or (S.ID = DS.IDShiftBase and ds.IDShiftBase is not null)
INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe
ON poe.IdEmployee = ds.IDEmployee
AND ds.Date BETWEEN poe.BeginDate AND poe.EndDate
AND poe.IdFeature = 2
AND poe.FeaturePermission > 1
AND poe.IdPassport = @idPassport
INNER JOIN EmployeeContracts AS EC ON EC.IDEmployee = E.ID AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate
LEFT JOIN DailySchedule AS DS1 ON DS1.IDEmployee = DS.IDEmployee AND DS1.Date = @WeekDay1 AND (DS1.IDShift1 = @IDShift or DS1.IDShiftBase = @IDShift)
LEFT JOIN DailySchedule AS DS2 ON DS2.IDEmployee = DS.IDEmployee AND DS2.Date = @WeekDay2 AND (DS2.IDShift1 = @IDShift  or DS2.IDShiftBase = @IDShift)
LEFT JOIN DailySchedule AS DS3 ON DS3.IDEmployee = DS.IDEmployee AND DS3.Date = @WeekDay3 AND (DS3.IDShift1 = @IDShift  or DS3.IDShiftBase = @IDShift)
LEFT JOIN DailySchedule AS DS4 ON DS4.IDEmployee = DS.IDEmployee AND DS4.Date = @WeekDay4 AND (DS4.IDShift1 = @IDShift or DS4.IDShiftBase = @IDShift)
LEFT JOIN DailySchedule AS DS5 ON DS5.IDEmployee = DS.IDEmployee AND DS5.Date = @WeekDay5 AND (DS5.IDShift1 = @IDShift or DS5.IDShiftBase = @IDShift)
LEFT JOIN DailySchedule AS DS6 ON DS6.IDEmployee = DS.IDEmployee AND DS6.Date = @WeekDay6 AND (DS6.IDShift1 = @IDShift or DS6.IDShiftBase = @IDShift)
LEFT JOIN DailySchedule AS DS7 ON DS7.IDEmployee = DS.IDEmployee AND DS7.Date = @WeekDay7 AND (DS7.IDShift1 = @IDShift or DS7.IDShiftBase = @IDShift)
LEFT JOIN ProgrammedAbsences AS PA1 ON PA1.IDEmployee = ds.IDEmployee and
(ds1.Date between pa1.BeginDate and pa1.FinishDate OR ds1.Date between pa1.BeginDate and DATEADD(DD,pa1.MaxLastingDays, pa1.BeginDate))
LEFT JOIN ProgrammedAbsences AS PA2 ON PA2.IDEmployee = ds.IDEmployee and
(ds2.Date between pa2.BeginDate and pa2.FinishDate OR ds2.Date between pa2.BeginDate and DATEADD(DD,pa2.MaxLastingDays, pa2.BeginDate))
LEFT JOIN ProgrammedAbsences AS PA3 ON PA3.IDEmployee = ds.IDEmployee and
(ds3.Date between pa3.BeginDate and pa3.FinishDate OR ds3.Date between pa3.BeginDate and DATEADD(DD,pa3.MaxLastingDays, pa3.BeginDate))
LEFT JOIN ProgrammedAbsences AS PA4 ON PA4.IDEmployee = ds.IDEmployee and
(ds4.Date between pa4.BeginDate and pa4.FinishDate OR ds4.Date between pa4.BeginDate and DATEADD(DD,pa4.MaxLastingDays, pa4.BeginDate))
LEFT JOIN ProgrammedAbsences AS PA5 ON PA5.IDEmployee = ds.IDEmployee and
(ds5.Date between pa5.BeginDate and pa5.FinishDate OR ds5.Date between pa5.BeginDate and DATEADD(DD,pa5.MaxLastingDays, pa5.BeginDate))
LEFT JOIN ProgrammedAbsences AS PA6 ON PA6.IDEmployee = ds.IDEmployee and
(ds6.Date between pa6.BeginDate and pa6.FinishDate OR ds6.Date between pa6.BeginDate and DATEADD(DD,pa6.MaxLastingDays, pa6.BeginDate))
LEFT JOIN ProgrammedAbsences AS PA7 ON PA7.IDEmployee = ds.IDEmployee and
(ds7.Date between pa7.BeginDate and pa7.FinishDate OR ds7.Date between pa7.BeginDate and DATEADD(DD,pa7.MaxLastingDays, pa7.BeginDate))
LEFT JOIN ProgrammedHolidays AS PH1 ON PH1.Date = DS1.Date AND PH1.IDEmployee = DS1.IDEmployee
LEFT JOIN ProgrammedHolidays AS PH2 ON PH2.Date = DS2.Date AND PH2.IDEmployee = DS2.IDEmployee
LEFT JOIN ProgrammedHolidays AS PH3 ON PH3.Date = DS3.Date AND PH3.IDEmployee = DS3.IDEmployee
LEFT JOIN ProgrammedHolidays AS PH4 ON PH4.Date = DS4.Date AND PH4.IDEmployee = DS4.IDEmployee
LEFT JOIN ProgrammedHolidays AS PH5 ON PH5.Date = DS5.Date AND PH5.IDEmployee = DS5.IDEmployee
LEFT JOIN ProgrammedHolidays AS PH6 ON PH6.Date = DS6.Date AND PH6.IDEmployee = DS6.IDEmployee
LEFT JOIN ProgrammedHolidays AS PH7 ON PH7.Date = DS7.Date AND PH7.IDEmployee = DS7.IDEmployee
LEFT JOIN ProgrammedCauses AS PC1 ON DS1.Date BETWEEN PC1.DATE AND PC1.FINISHDATE AND PC1.IDEmployee = DS1.IDEmployee
LEFT JOIN ProgrammedCauses AS PC2 ON DS2.Date BETWEEN PC2.DATE AND PC2.FINISHDATE AND PC2.IDEmployee = DS2.IDEmployee
LEFT JOIN ProgrammedCauses AS PC3 ON DS3.Date BETWEEN PC3.DATE AND PC3.FINISHDATE AND PC3.IDEmployee = DS3.IDEmployee
LEFT JOIN ProgrammedCauses AS PC4 ON DS4.Date BETWEEN PC4.DATE AND PC4.FINISHDATE AND PC4.IDEmployee = DS4.IDEmployee
LEFT JOIN ProgrammedCauses AS PC5 ON DS5.Date BETWEEN PC5.DATE AND PC5.FINISHDATE AND PC5.IDEmployee = DS5.IDEmployee
LEFT JOIN ProgrammedCauses AS PC6 ON DS6.Date BETWEEN PC6.DATE AND PC6.FINISHDATE AND PC6.IDEmployee = DS6.IDEmployee
LEFT JOIN ProgrammedCauses AS PC7 ON DS7.Date BETWEEN PC7.DATE AND PC7.FINISHDATE AND PC7.IDEmployee = DS7.IDEmployee
WHERE DS.Date between @WeekDay1 and @WeekDay7
AND (DS.IDShift1 = @IDShift OR DS.IDShiftBase = @IDShift)
AND ds.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))
AND S.ShiftType <> 2
ORDER BY E.Name
END
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='974' WHERE ID='DBVersion'
GO
