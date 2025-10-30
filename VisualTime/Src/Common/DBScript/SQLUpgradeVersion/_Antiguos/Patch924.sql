CREATE PROCEDURE [dbo].[Report_quadrant_by_schedule_absents]
    @WeekDay1 DATETIME2 = '20241014',
    @WeekDay2 DATETIME2 = '20241015',
    @WeekDay3 DATETIME2 = '20241016',
    @WeekDay4 DATETIME2 = '20241017',
    @WeekDay5 DATETIME2 = '20241018',
    @WeekDay6 DATETIME2 = '20241019',
    @WeekDay7 DATETIME2 = '20241020',
    @IDShift NVARCHAR(100) = '0',
    @idPassport NVARCHAR(100) = '1',
    @IDEmployee NVARCHAR(MAX) = '0'
AS
BEGIN    

-- Crear tablas temporales
CREATE TABLE #t1 (Name NVARCHAR(100), RowNum INT, ShiftName NVARCHAR(100));
CREATE TABLE #t2 (Name NVARCHAR(100), RowNum INT, ShiftName NVARCHAR(100));
CREATE TABLE #t3 (Name NVARCHAR(100), RowNum INT, ShiftName NVARCHAR(100));
CREATE TABLE #t4 (Name NVARCHAR(100), RowNum INT, ShiftName NVARCHAR(100));
CREATE TABLE #t5 (Name NVARCHAR(100), RowNum INT, ShiftName NVARCHAR(100));
CREATE TABLE #t6 (Name NVARCHAR(100), RowNum INT, ShiftName NVARCHAR(100));
CREATE TABLE #t7 (Name NVARCHAR(100), RowNum INT, ShiftName NVARCHAR(100));

-- Insertar datos en las tablas temporales
INSERT INTO #t1
SELECT 
    e.Name,         
    ROW_NUMBER() OVER (ORDER BY e.Name) AS RowNum,
	s.Name as ShitfName
FROM 
    DailySchedule AS ds
    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe 
        ON poe.IdEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN poe.BeginDate AND poe.EndDate 
        AND poe.IdFeature = 2 
        AND poe.FeaturePermission > 1 
        AND poe.IdPassport = @idPassport
    INNER JOIN shifts AS s 
        ON s.id = ds.IDShift1
    INNER JOIN Employees AS e 
        ON e.id = ds.IDEmployee
	INNER JOIN EmployeeContracts AS EC ON EC.IDEmployee = E.ID AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate
		LEFT JOIN ProgrammedAbsences AS PA ON PA.IDEmployee = ds.IDEmployee and  
		(ds.Date between pa.BeginDate and pa.FinishDate OR ds.Date between pa.BeginDate and DATEADD(DD,pa.MaxLastingDays, pa.BeginDate))
		LEFT JOIN ProgrammedCauses AS PC ON PC.IDEmployee = DS.IDEmployee AND DS.Date BETWEEN PC.Date AND PC.FinishDate
		LEFT JOIN ProgrammedHolidays AS PH ON PH.IDEmployee = DS.IDEmployee AND DS.Date = PH.Date
WHERE 
    ds.IDShift1 = @IDShift 
    AND ds.Date = @WeekDay1 
    AND ds.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))
	AND (PA.IDEmployee IS NOT NULL OR DS.IsHolidays = 1 OR s.ExpectedWorkingHours = 0 OR PC.IDEmployee IS NOT NULL OR PH.IDEmployee IS NOT NULL)
order by E.Name;

INSERT INTO #t2
SELECT 
    e.Name,         
    ROW_NUMBER() OVER (ORDER BY e.Name) AS RowNum,
	s.Name as ShitfName
FROM 
    DailySchedule AS ds
    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe 
        ON poe.IdEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN poe.BeginDate AND poe.EndDate 
        AND poe.IdFeature = 2 
        AND poe.FeaturePermission > 1 
        AND poe.IdPassport = @idPassport
    INNER JOIN shifts AS s 
        ON s.id = ds.IDShift1
    INNER JOIN Employees AS e 
        ON e.id = ds.IDEmployee
		INNER JOIN EmployeeContracts AS EC ON EC.IDEmployee = E.ID AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate
		LEFT JOIN ProgrammedAbsences AS PA ON PA.IDEmployee = ds.IDEmployee and  
		(ds.Date between pa.BeginDate and pa.FinishDate OR ds.Date between pa.BeginDate and DATEADD(DD,pa.MaxLastingDays, pa.BeginDate))
		LEFT JOIN ProgrammedCauses AS PC ON PC.IDEmployee = DS.IDEmployee AND DS.Date BETWEEN PC.Date AND PC.FinishDate
		LEFT JOIN ProgrammedHolidays AS PH ON PH.IDEmployee = DS.IDEmployee AND DS.Date = PH.Date
WHERE 
    ds.IDShift1 = @IDShift 
    AND ds.Date = @WeekDay2 
    AND ds.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))
	AND (PA.IDEmployee IS NOT NULL OR DS.IsHolidays = 1 OR s.ExpectedWorkingHours = 0 OR PC.IDEmployee IS NOT NULL OR PH.IDEmployee IS NOT NULL)
	order by E.Name;

INSERT INTO #t3
SELECT 
    e.Name,         
    ROW_NUMBER() OVER (ORDER BY e.Name) AS RowNum,
	s.Name as ShitfName
FROM 
    DailySchedule AS ds
    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe 
        ON poe.IdEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN poe.BeginDate AND poe.EndDate 
        AND poe.IdFeature = 2 
        AND poe.FeaturePermission > 1 
        AND poe.IdPassport = @idPassport
    INNER JOIN shifts AS s 
        ON s.id = ds.IDShift1
    INNER JOIN Employees AS e 
        ON e.id = ds.IDEmployee
		INNER JOIN EmployeeContracts AS EC ON EC.IDEmployee = E.ID AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate
		LEFT JOIN ProgrammedAbsences AS PA ON PA.IDEmployee = ds.IDEmployee and  
		(ds.Date between pa.BeginDate and pa.FinishDate OR ds.Date between pa.BeginDate and DATEADD(DD,pa.MaxLastingDays, pa.BeginDate))
		LEFT JOIN ProgrammedCauses AS PC ON PC.IDEmployee = DS.IDEmployee AND DS.Date BETWEEN PC.Date AND PC.FinishDate
		LEFT JOIN ProgrammedHolidays AS PH ON PH.IDEmployee = DS.IDEmployee AND DS.Date = PH.Date
WHERE 
    ds.IDShift1 = @IDShift 
    AND ds.Date = @WeekDay3 
    AND ds.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))
	AND (PA.IDEmployee IS NOT NULL OR DS.IsHolidays = 1 OR s.ExpectedWorkingHours = 0 OR PC.IDEmployee IS NOT NULL OR PH.IDEmployee IS NOT NULL)
	order by E.Name;

INSERT INTO #t4
SELECT 
    e.Name,         
    ROW_NUMBER() OVER (ORDER BY e.Name) AS RowNum,
	s.Name as ShitfName
FROM 
    DailySchedule AS ds
    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe 
        ON poe.IdEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN poe.BeginDate AND poe.EndDate 
        AND poe.IdFeature = 2 
        AND poe.FeaturePermission > 1 
        AND poe.IdPassport = @idPassport
    INNER JOIN shifts AS s 
        ON s.id = ds.IDShift1
    INNER JOIN Employees AS e 
        ON e.id = ds.IDEmployee
		INNER JOIN EmployeeContracts AS EC ON EC.IDEmployee = E.ID AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate
		LEFT JOIN ProgrammedAbsences AS PA ON PA.IDEmployee = ds.IDEmployee and  
		(ds.Date between pa.BeginDate and pa.FinishDate OR ds.Date between pa.BeginDate and DATEADD(DD,pa.MaxLastingDays, pa.BeginDate))
		LEFT JOIN ProgrammedCauses AS PC ON PC.IDEmployee = DS.IDEmployee AND DS.Date BETWEEN PC.Date AND PC.FinishDate
		LEFT JOIN ProgrammedHolidays AS PH ON PH.IDEmployee = DS.IDEmployee AND DS.Date = PH.Date
WHERE 
    ds.IDShift1 = @IDShift 
    AND ds.Date = @WeekDay4 
    AND ds.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))
AND (PA.IDEmployee IS NOT NULL OR DS.IsHolidays = 1 OR s.ExpectedWorkingHours = 0 OR PC.IDEmployee IS NOT NULL OR PH.IDEmployee IS NOT NULL)
	order by E.Name;

INSERT INTO #t5
SELECT 
    e.Name,         
    ROW_NUMBER() OVER (ORDER BY e.Name) AS RowNum,
	s.Name as ShitfName
FROM 
    DailySchedule AS ds
    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe 
        ON poe.IdEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN poe.BeginDate AND poe.EndDate 
        AND poe.IdFeature = 2 
        AND poe.FeaturePermission > 1 
        AND poe.IdPassport = @idPassport
    INNER JOIN shifts AS s 
        ON s.id = ds.IDShift1
    INNER JOIN Employees AS e 
        ON e.id = ds.IDEmployee
		INNER JOIN EmployeeContracts AS EC ON EC.IDEmployee = E.ID AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate
		LEFT JOIN ProgrammedAbsences AS PA ON PA.IDEmployee = ds.IDEmployee and  
		(ds.Date between pa.BeginDate and pa.FinishDate OR ds.Date between pa.BeginDate and DATEADD(DD,pa.MaxLastingDays, pa.BeginDate))
		LEFT JOIN ProgrammedCauses AS PC ON PC.IDEmployee = DS.IDEmployee AND DS.Date BETWEEN PC.Date AND PC.FinishDate
		LEFT JOIN ProgrammedHolidays AS PH ON PH.IDEmployee = DS.IDEmployee AND DS.Date = PH.Date
WHERE 
    ds.IDShift1 = @IDShift 
    AND ds.Date = @WeekDay5 
    AND ds.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))
	AND (PA.IDEmployee IS NOT NULL OR DS.IsHolidays = 1 OR s.ExpectedWorkingHours = 0 OR PC.IDEmployee IS NOT NULL OR PH.IDEmployee IS NOT NULL)
	order by E.Name;

INSERT INTO #t6
SELECT 
    e.Name,         
    ROW_NUMBER() OVER (ORDER BY e.Name) AS RowNum,
	s.Name as ShitfName
FROM 
    DailySchedule AS ds
    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe 
        ON poe.IdEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN poe.BeginDate AND poe.EndDate 
        AND poe.IdFeature = 2 
        AND poe.FeaturePermission > 1 
        AND poe.IdPassport = @idPassport
    INNER JOIN shifts AS s 
        ON s.id = ds.IDShift1
    INNER JOIN Employees AS e 
        ON e.id = ds.IDEmployee
		INNER JOIN EmployeeContracts AS EC ON EC.IDEmployee = E.ID AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate
		LEFT JOIN ProgrammedAbsences AS PA ON PA.IDEmployee = ds.IDEmployee and  
		(ds.Date between pa.BeginDate and pa.FinishDate OR ds.Date between pa.BeginDate and DATEADD(DD,pa.MaxLastingDays, pa.BeginDate))
		LEFT JOIN ProgrammedCauses AS PC ON PC.IDEmployee = DS.IDEmployee AND DS.Date BETWEEN PC.Date AND PC.FinishDate
		LEFT JOIN ProgrammedHolidays AS PH ON PH.IDEmployee = DS.IDEmployee AND DS.Date = PH.Date
WHERE 
    ds.IDShift1 = @IDShift 
    AND ds.Date = @WeekDay6 
    AND ds.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))
AND (PA.IDEmployee IS NOT NULL OR DS.IsHolidays = 1 OR s.ExpectedWorkingHours = 0 OR PC.IDEmployee IS NOT NULL OR PH.IDEmployee IS NOT NULL)
	order by E.Name;

INSERT INTO #t7
SELECT 
    e.Name,         
    ROW_NUMBER() OVER (ORDER BY e.Name) AS RowNum,
	s.Name as ShitfName
FROM 
    DailySchedule AS ds
    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe 
        ON poe.IdEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN poe.BeginDate AND poe.EndDate 
        AND poe.IdFeature = 2 
        AND poe.FeaturePermission > 1 
        AND poe.IdPassport = @idPassport
    INNER JOIN shifts AS s 
        ON s.id = ds.IDShift1
    INNER JOIN Employees AS e 
        ON e.id = ds.IDEmployee
		INNER JOIN EmployeeContracts AS EC ON EC.IDEmployee = E.ID AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate
		LEFT JOIN ProgrammedAbsences AS PA ON PA.IDEmployee = ds.IDEmployee and  
		(ds.Date between pa.BeginDate and pa.FinishDate OR ds.Date between pa.BeginDate and DATEADD(DD,pa.MaxLastingDays, pa.BeginDate))
		LEFT JOIN ProgrammedCauses AS PC ON PC.IDEmployee = DS.IDEmployee AND DS.Date BETWEEN PC.Date AND PC.FinishDate
		LEFT JOIN ProgrammedHolidays AS PH ON PH.IDEmployee = DS.IDEmployee AND DS.Date = PH.Date
WHERE 
    ds.IDShift1 = @IDShift 
    AND ds.Date = @WeekDay7 
    AND ds.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))
AND (PA.IDEmployee IS NOT NULL OR DS.IsHolidays = 1 OR s.ExpectedWorkingHours = 0 OR PC.IDEmployee IS NOT NULL OR PH.IDEmployee IS NOT NULL)
	order by E.Name;

DECLARE @numRows1 smallint = (select isnull(max(rownum),0) from #t1)
DECLARE @numRows2 smallint = (select isnull(max(rownum),0) from #t2)
DECLARE @numRows3 smallint = (select isnull(max(rownum),0) from #t3)
DECLARE @numRows4 smallint = (select isnull(max(rownum),0) from #t4)
DECLARE @numRows5 smallint = (select isnull(max(rownum),0) from #t5)
DECLARE @numRows6 smallint = (select isnull(max(rownum),0) from #t6)
DECLARE @numRows7 smallint = (select isnull(max(rownum),0) from #t7)

print @numrows1
print @numrows2
print @numrows3
print @numrows4
print @numrows5
print @numrows6
print @numrows7

-- Comprobación final para todas las tablas
IF (@numRows1 >= @numRows2 and @numRows1 >= @numRows3 and @numRows1 >= @numRows4 and @numRows1 >= @numRows5 and @numRows1 >= @numRows6 and @numRows1 >= @numRows7)
BEGIN
    SELECT     
        t1.Name AS WeekDay1,     
        t2.Name AS WeekDay2,
        t3.Name AS WeekDay3,
        t4.Name AS WeekDay4,
        t5.Name AS WeekDay5,
        t6.Name AS WeekDay6,
        t7.Name AS WeekDay7, 
		t1.ShiftName
    FROM 
        #t1 t1
        LEFT JOIN #t2 t2 ON t1.RowNum = t2.RowNum
        LEFT JOIN #t3 t3 ON t1.RowNum = t3.RowNum
        LEFT JOIN #t4 t4 ON t1.RowNum = t4.RowNum
        LEFT JOIN #t5 t5 ON t1.RowNum = t5.RowNum
        LEFT JOIN #t6 t6 ON t1.RowNum = t6.RowNum
        LEFT JOIN #t7 t7 ON t1.RowNum = t7.RowNum;
END
ELSE IF (@numRows2 >= @numRows1 and @numRows2 >= @numRows3 and @numRows2 >= @numRows4 and @numRows2 >= @numRows5 and @numRows2 >= @numRows6 and @numRows2 >= @numRows7)
BEGIN
    SELECT     
        t1.Name AS WeekDay1,     
        t2.Name AS WeekDay2,
        t3.Name AS WeekDay3,
        t4.Name AS WeekDay4,
        t5.Name AS WeekDay5,
        t6.Name AS WeekDay6,
        t7.Name AS WeekDay7,
		t2.ShiftName
    FROM 
        #t2 t2
        LEFT JOIN #t1 t1 ON t2.RowNum = t1.RowNum
        LEFT JOIN #t3 t3 ON t2.RowNum = t3.RowNum
        LEFT JOIN #t4 t4 ON t2.RowNum = t4.RowNum
        LEFT JOIN #t5 t5 ON t2.RowNum = t5.RowNum
        LEFT JOIN #t6 t6 ON t2.RowNum = t6.RowNum
        LEFT JOIN #t7 t7 ON t2.RowNum = t7.RowNum;
END
ELSE IF (@numRows3 >= @numRows2 and @numRows3 >= @numRows1 and @numRows3 >= @numRows4 and @numRows3 >= @numRows5 and @numRows3 >= @numRows6 and @numRows3 >= @numRows7)
BEGIN
    SELECT     
        t1.Name AS WeekDay1,     
        t2.Name AS WeekDay2,
        t3.Name AS WeekDay3,
        t4.Name AS WeekDay4,
        t5.Name AS WeekDay5,
        t6.Name AS WeekDay6,
        t7.Name AS WeekDay7,
		t3.ShiftName
    FROM 
        #t3 t3
        LEFT JOIN #t1 t1 ON t3.RowNum = t1.RowNum
        LEFT JOIN #t2 t2 ON t3.RowNum = t2.RowNum
        LEFT JOIN #t4 t4 ON t3.RowNum = t4.RowNum
        LEFT JOIN #t5 t5 ON t3.RowNum = t5.RowNum
        LEFT JOIN #t6 t6 ON t3.RowNum = t6.RowNum
        LEFT JOIN #t7 t7 ON t3.RowNum = t7.RowNum;
END
ELSE IF (@numRows4 >= @numRows2 and @numRows4 >= @numRows3 and @numRows4 >= @numRows1 and @numRows4 >= @numRows5 and @numRows4 >= @numRows6 and @numRows4 >= @numRows7)
BEGIN
    SELECT     
        t1.Name AS WeekDay1,     
        t2.Name AS WeekDay2,
        t3.Name AS WeekDay3,
        t4.Name AS WeekDay4,
        t5.Name AS WeekDay5,
        t6.Name AS WeekDay6,
        t7.Name AS WeekDay7,
		t4.ShiftName
    FROM 
        #t4 t4
        LEFT JOIN #t1 t1 ON t4.RowNum = t1.RowNum
        LEFT JOIN #t2 t2 ON t4.RowNum = t2.RowNum
        LEFT JOIN #t3 t3 ON t4.RowNum = t3.RowNum
        LEFT JOIN #t5 t5 ON t4.RowNum = t5.RowNum
        LEFT JOIN #t6 t6 ON t4.RowNum = t6.RowNum
        LEFT JOIN #t7 t7 ON t4.RowNum = t7.RowNum;
END
ELSE IF (@numRows5 >= @numRows2 and @numRows5 >= @numRows3 and @numRows5 >= @numRows4 and @numRows5 >= @numRows1 and @numRows5 >= @numRows6 and @numRows5 >= @numRows7)
BEGIN
    SELECT     
        t1.Name AS WeekDay1,     
        t2.Name AS WeekDay2,
        t3.Name AS WeekDay3,
        t4.Name AS WeekDay4,
        t5.Name AS WeekDay5,
        t6.Name AS WeekDay6,
        t7.Name AS WeekDay7,
		t5.ShiftName
    FROM 
        #t5 t5
        LEFT JOIN #t1 t1 ON t5.RowNum = t1.RowNum
        LEFT JOIN #t2 t2 ON t5.RowNum = t2.RowNum
        LEFT JOIN #t3 t3 ON t5.RowNum = t3.RowNum
        LEFT JOIN #t4 t4 ON t5.RowNum = t4.RowNum
        LEFT JOIN #t6 t6 ON t5.RowNum = t6.RowNum
        LEFT JOIN #t7 t7 ON t5.RowNum = t7.RowNum;
END
ELSE IF (@numRows6 >= @numRows2 and @numRows6 >= @numRows3 and @numRows6 >= @numRows4 and @numRows6 >= @numRows5 and @numRows6 >= @numRows1 and @numRows6 >= @numRows7)
BEGIN
    SELECT     
        t1.Name AS WeekDay1,     
        t2.Name AS WeekDay2,
        t3.Name AS WeekDay3,
        t4.Name AS WeekDay4,
        t5.Name AS WeekDay5,
        t6.Name AS WeekDay6,
        t7.Name AS WeekDay7,
		t6.ShiftName
    FROM 
        #t6 t6
        LEFT JOIN #t1 t1 ON t6.RowNum = t1.RowNum
        LEFT JOIN #t2 t2 ON t6.RowNum = t2.RowNum
        LEFT JOIN #t3 t3 ON t6.RowNum = t3.RowNum
        LEFT JOIN #t4 t4 ON t6.RowNum = t4.RowNum
        LEFT JOIN #t5 t5 ON t6.RowNum = t5.RowNum
        LEFT JOIN #t7 t7 ON t6.RowNum = t7.RowNum;
END
ELSE IF (@numRows7 >= @numRows2 and @numRows7 >= @numRows3 and @numRows7 >= @numRows4 and @numRows7 >= @numRows5 and @numRows7 >= @numRows6 and @numRows7 >= @numRows1)
BEGIN
    SELECT     
        t1.Name AS WeekDay1,     
        t2.Name AS WeekDay2,
        t3.Name AS WeekDay3,
        t4.Name AS WeekDay4,
        t5.Name AS WeekDay5,
        t6.Name AS WeekDay6,
        t7.Name AS WeekDay7,
		t7.ShiftName
    FROM 
        #t7 t7
        LEFT JOIN #t1 t1 ON t7.RowNum = t1.RowNum
        LEFT JOIN #t2 t2 ON t7.RowNum = t2.RowNum
        LEFT JOIN #t3 t3 ON t7.RowNum = t3.RowNum
        LEFT JOIN #t4 t4 ON t7.RowNum = t4.RowNum
        LEFT JOIN #t5 t5 ON t7.RowNum = t5.RowNum
        LEFT JOIN #t6 t6 ON t7.RowNum = t6.RowNum;
END

-- Eliminar tablas temporales
DROP TABLE #t1;
DROP TABLE #t2;
DROP TABLE #t3;
DROP TABLE #t4;
DROP TABLE #t5;
DROP TABLE #t6;
DROP TABLE #t7;

RETURN NULL

END


GO


CREATE PROCEDURE [dbo].[Report_quadrant_by_schedule_presents]
    @WeekDay1 DATETIME2 = '20241014',
    @WeekDay2 DATETIME2 = '20241015',
    @WeekDay3 DATETIME2 = '20241016',
    @WeekDay4 DATETIME2 = '20241017',
    @WeekDay5 DATETIME2 = '20241018',
    @WeekDay6 DATETIME2 = '20241019',
    @WeekDay7 DATETIME2 = '20241020',
    @IDShift NVARCHAR(100) = '0',
    @idPassport NVARCHAR(100) = '1',
    @IDEmployee NVARCHAR(MAX) = '0'
AS
BEGIN    

-- Crear tablas temporales
CREATE TABLE #t1 (Name NVARCHAR(100), RowNum INT, ShiftName NVARCHAR(100));
CREATE TABLE #t2 (Name NVARCHAR(100), RowNum INT, ShiftName NVARCHAR(100));
CREATE TABLE #t3 (Name NVARCHAR(100), RowNum INT, ShiftName NVARCHAR(100));
CREATE TABLE #t4 (Name NVARCHAR(100), RowNum INT, ShiftName NVARCHAR(100));
CREATE TABLE #t5 (Name NVARCHAR(100), RowNum INT, ShiftName NVARCHAR(100));
CREATE TABLE #t6 (Name NVARCHAR(100), RowNum INT, ShiftName NVARCHAR(100));
CREATE TABLE #t7 (Name NVARCHAR(100), RowNum INT, ShiftName NVARCHAR(100));

-- Insertar datos en las tablas temporales
INSERT INTO #t1
SELECT 
    e.Name,         
    ROW_NUMBER() OVER (ORDER BY e.Name) AS RowNum,
	s.Name as ShitfName
FROM 
    DailySchedule AS ds
    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe 
        ON poe.IdEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN poe.BeginDate AND poe.EndDate 
        AND poe.IdFeature = 2 
        AND poe.FeaturePermission > 1 
        AND poe.IdPassport = @idPassport
    INNER JOIN shifts AS s 
        ON s.id = ds.IDShift1
    INNER JOIN Employees AS e 
        ON e.id = ds.IDEmployee
		INNER JOIN EmployeeContracts AS EC ON EC.IDEmployee = E.ID AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate
		LEFT JOIN ProgrammedAbsences AS PA ON PA.IDEmployee = ds.IDEmployee and  
		(ds.Date between pa.BeginDate and pa.FinishDate OR ds.Date between pa.BeginDate and DATEADD(DD,pa.MaxLastingDays, pa.BeginDate))
		LEFT JOIN ProgrammedCauses AS PC ON PC.IDEmployee = DS.IDEmployee AND DS.Date BETWEEN PC.Date AND PC.FinishDate
		LEFT JOIN ProgrammedHolidays AS PH ON PH.IDEmployee = DS.IDEmployee AND DS.Date = PH.Date
WHERE 
    ds.IDShift1 = @IDShift 
    AND ds.Date = @WeekDay1 
    AND ds.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))
	AND PA.IDEmployee IS NULL
	AND (DS.IsHolidays = 0 or ds.IsHolidays is null)
	AND S.ExpectedWorkingHours > 0
	AND PC.IDEmployee IS NULL
	AND PH.IDEmployee IS NULL
	order by E.Name;

INSERT INTO #t2
SELECT 
    e.Name,         
    ROW_NUMBER() OVER (ORDER BY e.Name) AS RowNum,
	s.Name as ShitfName
FROM 
    DailySchedule AS ds
    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe 
        ON poe.IdEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN poe.BeginDate AND poe.EndDate 
        AND poe.IdFeature = 2 
        AND poe.FeaturePermission > 1 
        AND poe.IdPassport = @idPassport
    INNER JOIN shifts AS s 
        ON s.id = ds.IDShift1
    INNER JOIN Employees AS e 
        ON e.id = ds.IDEmployee
		INNER JOIN EmployeeContracts AS EC ON EC.IDEmployee = E.ID AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate
		LEFT JOIN ProgrammedAbsences AS PA ON PA.IDEmployee = ds.IDEmployee and  
		(ds.Date between pa.BeginDate and pa.FinishDate OR ds.Date between pa.BeginDate and DATEADD(DD,pa.MaxLastingDays, pa.BeginDate))
		LEFT JOIN ProgrammedCauses AS PC ON PC.IDEmployee = DS.IDEmployee AND DS.Date BETWEEN PC.Date AND PC.FinishDate
		LEFT JOIN ProgrammedHolidays AS PH ON PH.IDEmployee = DS.IDEmployee AND DS.Date = PH.Date
WHERE 
    ds.IDShift1 = @IDShift 
    AND ds.Date = @WeekDay2 
    AND ds.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))
	AND PA.IDEmployee IS NULL
	AND (DS.IsHolidays = 0 or ds.IsHolidays is null)
	AND S.ExpectedWorkingHours > 0
	AND PC.IDEmployee IS NULL
	AND PH.IDEmployee IS NULL
	order by E.Name;

INSERT INTO #t3
SELECT 
    e.Name,         
    ROW_NUMBER() OVER (ORDER BY e.Name) AS RowNum,
	s.Name as ShitfName
FROM 
    DailySchedule AS ds
    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe 
        ON poe.IdEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN poe.BeginDate AND poe.EndDate 
        AND poe.IdFeature = 2 
        AND poe.FeaturePermission > 1 
        AND poe.IdPassport = @idPassport
    INNER JOIN shifts AS s 
        ON s.id = ds.IDShift1
    INNER JOIN Employees AS e 
        ON e.id = ds.IDEmployee
		INNER JOIN EmployeeContracts AS EC ON EC.IDEmployee = E.ID AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate
		LEFT JOIN ProgrammedAbsences AS PA ON PA.IDEmployee = ds.IDEmployee and  
		(ds.Date between pa.BeginDate and pa.FinishDate OR ds.Date between pa.BeginDate and DATEADD(DD,pa.MaxLastingDays, pa.BeginDate))
		LEFT JOIN ProgrammedCauses AS PC ON PC.IDEmployee = DS.IDEmployee AND DS.Date BETWEEN PC.Date AND PC.FinishDate
		LEFT JOIN ProgrammedHolidays AS PH ON PH.IDEmployee = DS.IDEmployee AND DS.Date = PH.Date
WHERE 
    ds.IDShift1 = @IDShift 
    AND ds.Date = @WeekDay3 
    AND ds.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))
	AND PA.IDEmployee IS NULL
	AND (DS.IsHolidays = 0 or ds.IsHolidays is null)
	AND S.ExpectedWorkingHours > 0
	AND PC.IDEmployee IS NULL
	AND PH.IDEmployee IS NULL
	order by E.Name;

INSERT INTO #t4
SELECT 
    e.Name,         
    ROW_NUMBER() OVER (ORDER BY e.Name) AS RowNum,
	s.Name as ShitfName
FROM 
    DailySchedule AS ds
    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe 
        ON poe.IdEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN poe.BeginDate AND poe.EndDate 
        AND poe.IdFeature = 2 
        AND poe.FeaturePermission > 1 
        AND poe.IdPassport = @idPassport
    INNER JOIN shifts AS s 
        ON s.id = ds.IDShift1
    INNER JOIN Employees AS e 
        ON e.id = ds.IDEmployee
		INNER JOIN EmployeeContracts AS EC ON EC.IDEmployee = E.ID AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate
		LEFT JOIN ProgrammedAbsences AS PA ON PA.IDEmployee = ds.IDEmployee and  
		(ds.Date between pa.BeginDate and pa.FinishDate OR ds.Date between pa.BeginDate and DATEADD(DD,pa.MaxLastingDays, pa.BeginDate))
		LEFT JOIN ProgrammedCauses AS PC ON PC.IDEmployee = DS.IDEmployee AND DS.Date BETWEEN PC.Date AND PC.FinishDate
		LEFT JOIN ProgrammedHolidays AS PH ON PH.IDEmployee = DS.IDEmployee AND DS.Date = PH.Date
WHERE 
    ds.IDShift1 = @IDShift 
    AND ds.Date = @WeekDay4 
    AND ds.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))
	AND PA.IDEmployee IS NULL
	AND (DS.IsHolidays = 0 or ds.IsHolidays is null)
	AND S.ExpectedWorkingHours > 0
	AND PC.IDEmployee IS NULL
	AND PH.IDEmployee IS NULL
	order by E.Name;

INSERT INTO #t5
SELECT 
    e.Name,         
    ROW_NUMBER() OVER (ORDER BY e.Name) AS RowNum,
	s.Name as ShitfName
FROM 
    DailySchedule AS ds
    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe 
        ON poe.IdEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN poe.BeginDate AND poe.EndDate 
        AND poe.IdFeature = 2 
        AND poe.FeaturePermission > 1 
        AND poe.IdPassport = @idPassport
    INNER JOIN shifts AS s 
        ON s.id = ds.IDShift1
    INNER JOIN Employees AS e 
        ON e.id = ds.IDEmployee
		INNER JOIN EmployeeContracts AS EC ON EC.IDEmployee = E.ID AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate
		LEFT JOIN ProgrammedAbsences AS PA ON PA.IDEmployee = ds.IDEmployee and  
		(ds.Date between pa.BeginDate and pa.FinishDate OR ds.Date between pa.BeginDate and DATEADD(DD,pa.MaxLastingDays, pa.BeginDate))
		LEFT JOIN ProgrammedCauses AS PC ON PC.IDEmployee = DS.IDEmployee AND DS.Date BETWEEN PC.Date AND PC.FinishDate
		LEFT JOIN ProgrammedHolidays AS PH ON PH.IDEmployee = DS.IDEmployee AND DS.Date = PH.Date
WHERE 
    ds.IDShift1 = @IDShift 
    AND ds.Date = @WeekDay5 
    AND ds.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))
	AND PA.IDEmployee IS NULL
	AND (DS.IsHolidays = 0 or ds.IsHolidays is null)
	AND S.ExpectedWorkingHours > 0
	AND PC.IDEmployee IS NULL
	AND PH.IDEmployee IS NULL
	order by E.Name;

INSERT INTO #t6
SELECT 
    e.Name,         
    ROW_NUMBER() OVER (ORDER BY e.Name) AS RowNum,
	s.Name as ShitfName
FROM 
    DailySchedule AS ds
    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe 
        ON poe.IdEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN poe.BeginDate AND poe.EndDate 
        AND poe.IdFeature = 2 
        AND poe.FeaturePermission > 1 
        AND poe.IdPassport = @idPassport
    INNER JOIN shifts AS s 
        ON s.id = ds.IDShift1
    INNER JOIN Employees AS e 
        ON e.id = ds.IDEmployee
		INNER JOIN EmployeeContracts AS EC ON EC.IDEmployee = E.ID AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate
		LEFT JOIN ProgrammedAbsences AS PA ON PA.IDEmployee = ds.IDEmployee and  
		(ds.Date between pa.BeginDate and pa.FinishDate OR ds.Date between pa.BeginDate and DATEADD(DD,pa.MaxLastingDays, pa.BeginDate))
		LEFT JOIN ProgrammedCauses AS PC ON PC.IDEmployee = DS.IDEmployee AND DS.Date BETWEEN PC.Date AND PC.FinishDate
		LEFT JOIN ProgrammedHolidays AS PH ON PH.IDEmployee = DS.IDEmployee AND DS.Date = PH.Date
WHERE 
    ds.IDShift1 = @IDShift 
    AND ds.Date = @WeekDay6 
    AND ds.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))
	AND PA.IDEmployee IS NULL
	AND (DS.IsHolidays = 0 or ds.IsHolidays is null)
	AND S.ExpectedWorkingHours > 0
	AND PC.IDEmployee IS NULL
	AND PH.IDEmployee IS NULL
	order by E.Name;

INSERT INTO #t7
SELECT 
    e.Name,         
    ROW_NUMBER() OVER (ORDER BY e.Name) AS RowNum,
	s.Name as ShitfName
FROM 
    DailySchedule AS ds
    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe 
        ON poe.IdEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN poe.BeginDate AND poe.EndDate 
        AND poe.IdFeature = 2 
        AND poe.FeaturePermission > 1 
        AND poe.IdPassport = @idPassport
    INNER JOIN shifts AS s 
        ON s.id = ds.IDShift1
    INNER JOIN Employees AS e 
        ON e.id = ds.IDEmployee
		INNER JOIN EmployeeContracts AS EC ON EC.IDEmployee = E.ID AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate
		LEFT JOIN ProgrammedAbsences AS PA ON PA.IDEmployee = ds.IDEmployee and  
		(ds.Date between pa.BeginDate and pa.FinishDate OR ds.Date between pa.BeginDate and DATEADD(DD,pa.MaxLastingDays, pa.BeginDate))
		LEFT JOIN ProgrammedCauses AS PC ON PC.IDEmployee = DS.IDEmployee AND DS.Date BETWEEN PC.Date AND PC.FinishDate
		LEFT JOIN ProgrammedHolidays AS PH ON PH.IDEmployee = DS.IDEmployee AND DS.Date = PH.Date
WHERE 
    ds.IDShift1 = @IDShift 
    AND ds.Date = @WeekDay7 
    AND ds.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))
	AND PA.IDEmployee IS NULL
	AND (DS.IsHolidays = 0 or ds.IsHolidays is null)
	AND S.ExpectedWorkingHours > 0
	AND PC.IDEmployee IS NULL
	AND PH.IDEmployee IS NULL
	order by E.Name;

DECLARE @numRows1 smallint = (select isnull(max(rownum),0) from #t1)
DECLARE @numRows2 smallint = (select isnull(max(rownum),0) from #t2)
DECLARE @numRows3 smallint = (select isnull(max(rownum),0) from #t3)
DECLARE @numRows4 smallint = (select isnull(max(rownum),0) from #t4)
DECLARE @numRows5 smallint = (select isnull(max(rownum),0) from #t5)
DECLARE @numRows6 smallint = (select isnull(max(rownum),0) from #t6)
DECLARE @numRows7 smallint = (select isnull(max(rownum),0) from #t7)

-- Comprobación final para todas las tablas
IF (@numRows1 >= @numRows2 and @numRows1 >= @numRows3 and @numRows1 >= @numRows4 and @numRows1 >= @numRows5 and @numRows1 >= @numRows6 and @numRows1 >= @numRows7)
BEGIN
    SELECT     
        t1.Name AS WeekDay1,     
        t2.Name AS WeekDay2,
        t3.Name AS WeekDay3,
        t4.Name AS WeekDay4,
        t5.Name AS WeekDay5,
        t6.Name AS WeekDay6,
        t7.Name AS WeekDay7, 
		t1.ShiftName
    FROM 
        #t1 t1
        LEFT JOIN #t2 t2 ON t1.RowNum = t2.RowNum
        LEFT JOIN #t3 t3 ON t1.RowNum = t3.RowNum
        LEFT JOIN #t4 t4 ON t1.RowNum = t4.RowNum
        LEFT JOIN #t5 t5 ON t1.RowNum = t5.RowNum
        LEFT JOIN #t6 t6 ON t1.RowNum = t6.RowNum
        LEFT JOIN #t7 t7 ON t1.RowNum = t7.RowNum;
END
ELSE IF (@numRows2 >= @numRows1 and @numRows2 >= @numRows3 and @numRows2 >= @numRows4 and @numRows2 >= @numRows5 and @numRows2 >= @numRows6 and @numRows2 >= @numRows7)
BEGIN
    SELECT     
        t1.Name AS WeekDay1,     
        t2.Name AS WeekDay2,
        t3.Name AS WeekDay3,
        t4.Name AS WeekDay4,
        t5.Name AS WeekDay5,
        t6.Name AS WeekDay6,
        t7.Name AS WeekDay7,
		t2.ShiftName
    FROM 
        #t2 t2
        LEFT JOIN #t1 t1 ON t2.RowNum = t1.RowNum
        LEFT JOIN #t3 t3 ON t2.RowNum = t3.RowNum
        LEFT JOIN #t4 t4 ON t2.RowNum = t4.RowNum
        LEFT JOIN #t5 t5 ON t2.RowNum = t5.RowNum
        LEFT JOIN #t6 t6 ON t2.RowNum = t6.RowNum
        LEFT JOIN #t7 t7 ON t2.RowNum = t7.RowNum;
END
ELSE IF (@numRows3 >= @numRows2 and @numRows3 >= @numRows1 and @numRows3 >= @numRows4 and @numRows3 >= @numRows5 and @numRows3 >= @numRows6 and @numRows3 >= @numRows7)
BEGIN
    SELECT     
        t1.Name AS WeekDay1,     
        t2.Name AS WeekDay2,
        t3.Name AS WeekDay3,
        t4.Name AS WeekDay4,
        t5.Name AS WeekDay5,
        t6.Name AS WeekDay6,
        t7.Name AS WeekDay7,
		t3.ShiftName
    FROM 
        #t3 t3
        LEFT JOIN #t1 t1 ON t3.RowNum = t1.RowNum
        LEFT JOIN #t2 t2 ON t3.RowNum = t2.RowNum
        LEFT JOIN #t4 t4 ON t3.RowNum = t4.RowNum
        LEFT JOIN #t5 t5 ON t3.RowNum = t5.RowNum
        LEFT JOIN #t6 t6 ON t3.RowNum = t6.RowNum
        LEFT JOIN #t7 t7 ON t3.RowNum = t7.RowNum;
END
ELSE IF (@numRows4 >= @numRows2 and @numRows4 >= @numRows3 and @numRows4 >= @numRows1 and @numRows4 >= @numRows5 and @numRows4 >= @numRows6 and @numRows4 >= @numRows7)
BEGIN
    SELECT     
        t1.Name AS WeekDay1,     
        t2.Name AS WeekDay2,
        t3.Name AS WeekDay3,
        t4.Name AS WeekDay4,
        t5.Name AS WeekDay5,
        t6.Name AS WeekDay6,
        t7.Name AS WeekDay7,
		t4.ShiftName
    FROM 
        #t4 t4
        LEFT JOIN #t1 t1 ON t4.RowNum = t1.RowNum
        LEFT JOIN #t2 t2 ON t4.RowNum = t2.RowNum
        LEFT JOIN #t3 t3 ON t4.RowNum = t3.RowNum
        LEFT JOIN #t5 t5 ON t4.RowNum = t5.RowNum
        LEFT JOIN #t6 t6 ON t4.RowNum = t6.RowNum
        LEFT JOIN #t7 t7 ON t4.RowNum = t7.RowNum;
END
ELSE IF (@numRows5 >= @numRows2 and @numRows5 >= @numRows3 and @numRows5 >= @numRows4 and @numRows5 >= @numRows1 and @numRows5 >= @numRows6 and @numRows5 >= @numRows7)
BEGIN
    SELECT     
        t1.Name AS WeekDay1,     
        t2.Name AS WeekDay2,
        t3.Name AS WeekDay3,
        t4.Name AS WeekDay4,
        t5.Name AS WeekDay5,
        t6.Name AS WeekDay6,
        t7.Name AS WeekDay7,
		t5.ShiftName
    FROM 
        #t5 t5
        LEFT JOIN #t1 t1 ON t5.RowNum = t1.RowNum
        LEFT JOIN #t2 t2 ON t5.RowNum = t2.RowNum
        LEFT JOIN #t3 t3 ON t5.RowNum = t3.RowNum
        LEFT JOIN #t4 t4 ON t5.RowNum = t4.RowNum
        LEFT JOIN #t6 t6 ON t5.RowNum = t6.RowNum
        LEFT JOIN #t7 t7 ON t5.RowNum = t7.RowNum;
END
ELSE IF (@numRows6 >= @numRows2 and @numRows6 >= @numRows3 and @numRows6 >= @numRows4 and @numRows6 >= @numRows5 and @numRows6 >= @numRows1 and @numRows6 >= @numRows7)
BEGIN
    SELECT     
        t1.Name AS WeekDay1,     
        t2.Name AS WeekDay2,
        t3.Name AS WeekDay3,
        t4.Name AS WeekDay4,
        t5.Name AS WeekDay5,
        t6.Name AS WeekDay6,
        t7.Name AS WeekDay7,
		t6.ShiftName
    FROM 
        #t6 t6
        LEFT JOIN #t1 t1 ON t6.RowNum = t1.RowNum
        LEFT JOIN #t2 t2 ON t6.RowNum = t2.RowNum
        LEFT JOIN #t3 t3 ON t6.RowNum = t3.RowNum
        LEFT JOIN #t4 t4 ON t6.RowNum = t4.RowNum
        LEFT JOIN #t5 t5 ON t6.RowNum = t5.RowNum
        LEFT JOIN #t7 t7 ON t6.RowNum = t7.RowNum;
END
ELSE IF (@numRows7 >= @numRows2 and @numRows7 >= @numRows3 and @numRows7 >= @numRows4 and @numRows7 >= @numRows5 and @numRows7 >= @numRows6 and @numRows7 >= @numRows1)
BEGIN
    SELECT     
        t1.Name AS WeekDay1,     
        t2.Name AS WeekDay2,
        t3.Name AS WeekDay3,
        t4.Name AS WeekDay4,
        t5.Name AS WeekDay5,
        t6.Name AS WeekDay6,
        t7.Name AS WeekDay7,
		t7.ShiftName
    FROM 
        #t7 t7
        LEFT JOIN #t1 t1 ON t7.RowNum = t1.RowNum
        LEFT JOIN #t2 t2 ON t7.RowNum = t2.RowNum
        LEFT JOIN #t3 t3 ON t7.RowNum = t3.RowNum
        LEFT JOIN #t4 t4 ON t7.RowNum = t4.RowNum
        LEFT JOIN #t5 t5 ON t7.RowNum = t5.RowNum
        LEFT JOIN #t6 t6 ON t7.RowNum = t6.RowNum;
END

-- Eliminar tablas temporales
DROP TABLE #t1;
DROP TABLE #t2;
DROP TABLE #t3;
DROP TABLE #t4;
DROP TABLE #t5;
DROP TABLE #t6;
DROP TABLE #t7;

RETURN NULL

END


GO

CREATE PROCEDURE [dbo].[Report_quadrant_by_schedule_shifts]
@GivenDate DATETIME2 = '20241014',
@Shifts NVARCHAR(MAX) = '0',
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '0'
AS
BEGIN    

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


CREATE PROCEDURE [dbo].[Report_quadrant_by_schedule_total]
@GivenDate DATETIME2 = '20241014',
    @idPassport NVARCHAR(100) = '1',
    @IDEmployee NVARCHAR(MAX) = '0',
	@Shifts NVARCHAR(MAX) = '0'
AS
BEGIN 

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

CREATE PROCEDURE [dbo].[Report_quadrant_by_schedule_total_by_shifts]
@WeekDay1 DATETIME2 = '20241014',
@WeekDay2 DATETIME2 = '20241015',
    @WeekDay3 DATETIME2 = '20241016',
    @WeekDay4 DATETIME2 = '20241017',
    @WeekDay5 DATETIME2 = '20241018',
    @WeekDay6 DATETIME2 = '20241019',
    @WeekDay7 DATETIME2 = '20241020',
    @IDShift NVARCHAR(100) = '0',
    @idPassport NVARCHAR(100) = '1',
    @IDEmployee NVARCHAR(MAX) = '0'
AS
BEGIN    

SELECT         
    'Total ' + (SELECT NAME FROM SHIFTS WHERE ID= @IDShift) as 'TotalShiftName',
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
    DS.IDShift1 = @IDShift 
    AND DS.IDEmployee IN (SELECT * FROM dbo.SplitString(@IDEmployee, ','))
GROUP BY 
    DS.IDShift1;
	RETURN NULL
END
GO


CREATE PROCEDURE [dbo].[Report_quadrant_by_schedule_weekDays]
@GivenDate DATETIME2 = '20241014'
AS
BEGIN
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

UPDATE sysroParameters SET Data='924' WHERE ID='DBVersion'
GO
