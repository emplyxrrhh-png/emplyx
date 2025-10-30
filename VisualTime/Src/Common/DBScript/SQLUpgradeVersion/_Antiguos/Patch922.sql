CREATE OR ALTER FUNCTION [dbo].[GetHolidayPeriods]    
    (      
     @startDate smalldatetime,    
     @endDate smalldatetime,
	 @idEmployee int
    )    
    RETURNS nvarchar(max)    
    AS    
    BEGIN    

	Declare @retValue as nvarchar(max);

	WITH CTE AS (
		SELECT
        Date,
        isHolidays,
        DATEADD(DAY, -ROW_NUMBER() OVER (ORDER BY Date), Date) AS Grupo
		FROM DailySchedule
		WHERE isHolidays = 1 and IDEmployee = @idEmployee
		  AND Date between @startDate and @endDate
	)
	, Periodos AS (
		SELECT 
			MIN(Date) AS FechaInicio, 
			MAX(Date) AS FechaFin
		FROM CTE
		GROUP BY Grupo
	)
	-- Concatenar los periodos en una sola cadena
	SELECT @retValue = STRING_AGG(
		CASE 
			WHEN FechaInicio = FechaFin THEN CONVERT(VARCHAR, FORMAT(FechaInicio, 'dd-MM')) 
			ELSE CONVERT(VARCHAR, FORMAT(FechaInicio, 'dd-MM')) + ' a ' + CONVERT(VARCHAR, FORMAT(FechaFin, 'dd-MM'))
		END, '; ')
	FROM Periodos;


	RETURN @retValue    
    END
GO
  
CREATE OR ALTER FUNCTION [dbo].[GetEmployeeUserFieldValueById]    
    (      
     @idEmployee int,    
     @UserFieldID nvarchar(50),    
     @Date smalldatetime    
    )    
    RETURNS varchar(4000)    
    AS    
    BEGIN    
   DECLARE @value varchar(4000),  
     @pidEmployee int = @idEmployee,    
     @pFieldName nvarchar(50) = @UserFieldID,    
     @pDate smalldatetime =  @Date    


   SELECT TOP 1 @Value = CONVERT(varchar(4000), [Value])    
       FROM EmployeeUserFieldValues        
       WHERE EmployeeUserFieldValues.IDEmployee = @pidEmployee AND     
          EmployeeUserFieldValues.FieldName = (SELECT FieldName from sysroUserFields where ID = CONVERT(INT, @pFieldName)) AND    
          EmployeeUserFieldValues.Date <= @pDate    
       ORDER BY EmployeeUserFieldValues.Date DESC    
   RETURN @value    
    END
GO

CREATE OR ALTER FUNCTION [dbo].[GetEmployeeUserFieldNameById]    
    (      
     @UserFieldID nvarchar(50)
    )    
    RETURNS nvarchar(max)    
    AS    
    BEGIN    
   DECLARE @value nvarchar(max),  
     @pFieldName nvarchar(50) = @UserFieldID


   SELECT TOP 1 @Value = FieldName
       FROM sysroUserFields        
       WHERE id = @UserFieldID
   RETURN @value    
    END
GO

CREATE OR ALTER FUNCTION [dbo].[GetStartOfYear]    
    ( @currentYear int )    
    RETURNS datetime    
    AS    
    BEGIN    
	DECLARE @monthPeriod INT = 23; -- Ejemplo de mes
	DECLARE @yearPeriod INT = 12; -- Ejemplo de año

	SELECT 
		@monthPeriod = CONVERT(xml, Data).value('(/roCollection/Item[@key=("MonthPeriod")]/text())[1]', 'int'),
		@yearPeriod = CONVERT(xml, Data).value('(/roCollection/Item[@key=("YearPeriod")]/text())[1]', 'int')
		FROM 
			sysroParameters rp WHERE ID='OPTIONS'
		
	return DATEFROMPARTS(@currentYear, @yearPeriod, @monthPeriod)
	END
GO

CREATE OR ALTER PROCEDURE [dbo].[StringToIdsTable]  
	@source nvarchar(max),
	@entityName nvarchar(max),
	@idFieldName nvarchar(max)
AS  
BEGIN  
  
DECLARE @SQLString nvarchar(MAX);  
SET @SQLString = 'SELECT DISTINCT ' + @idFieldName + ' FROM ' + @entityName + ' WHERE ' + @idFieldName + ' IN(' + @source + ')'
  
exec sp_executesql @SQLString  
end 
GO

CREATE OR ALTER PROCEDURE [dbo].[GetCompanysFieldValue]    
(
	@UserFieldID nvarchar(50)
)
AS    
    BEGIN  
	DECLARE @sql nvarchar(max),
			@pUserFieldID nvarchar(50) = @UserFieldID,
			@columnExists INT;

	-- Verificar si la columna existe
	SELECT @columnExists = COUNT(*)
	FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'Groups' AND COLUMN_NAME = @pUserFieldID;

	-- Construir la consulta dinámica para obtener el valor de la columna
	IF @columnExists > 0
	BEGIN
		SET @sql = N'SELECT ID As idGroup, ISNULL(' + @pUserFieldID + ','''') AS userFieldValue FROM Groups';
	END
	ELSE
	BEGIN
		SET @sql = N'SELECT ID As idGroup, '''' AS userFieldValue FROM Groups' ;
	END

	-- Ejecutar la consulta dinámica y almacenar el resultado en la variable
	EXEC sp_executesql @sql
	END
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_mapadoferias]
	@idPassport nvarchar(100) = '1',
	@userFieldIDs nvarchar(max) = '0',
	@holidayIDs nvarchar(max) = '32,33',
	@startDate datetime2 = '20210301'
AS

	DECLARE @pidPassport nvarchar(100) = @idPassport,
	@puserFieldIDs nvarchar(max) = @userFieldIDs,
	@pholidayIDs nvarchar(max) = @holidayIDs,
	@pstartDate datetime2 = @startDate,
	@yearStart datetime2 = @startDate,
	@yearEnd datetime2 = @startDate


	DECLARE @shiftIDs Table(id int)
	INSERT INTO @shiftIDs(id) exec dbo.StringToIdsTable @pholidayIDs, 'Shifts','Id';

	DECLARE @companyValues Table(idGroup int, userFieldValue nvarchar(max))
	insert into @companyValues exec dbo.GetCompanysFieldValue 'USR_IDFiscal';

	DECLARE @accrualIDs Table(id int)
	insert into @accrualIDs
	select distinct id from Concepts where id in (select IDConceptBalance from shifts where id in (select id from @shiftIDs))

	SELECT @yearStart = dbo.GetStartOfYear(YEAR(@startDate))
	SET @yearEnd = DATEADD(DAY, -1, DATEADD(YEAR, 1, @yearStart))

	select distinct 
		TRIM(dbo.UFN_SEPARATES_COLUMNS(emp.FullGroupName,1,'\')) AS CompanyName, 
		TRIM(cv.userFieldValue) As CompanyInfo,
		emp.IDEmployee As EmployeeID,
		emp.EmployeeName As EmployeeName,
		TRIM(emp.GroupName) As GroupName, 
		isNULL(dbo.GetEmployeeUserFieldValueMin(emp.IDEmployee,(Select Value from sysroLiveAdvancedParameters where ParameterName = 'ImportPrimaryKeyUserField'),@pstartDate),'') As EmployeeImport,
		ISNULL(dbo.GetEmployeeUserFieldValueById(emp.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldIDs,1,','),@pstartDate),'') As UserField1,
		dbo.GetEmployeeUserFieldNameById(dbo.UFN_SEPARATES_COLUMNS(@puserFieldIDs,1,',')) As UserField1Name,
		ISNULL(dbo.GetEmployeeUserFieldValueById(emp.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldIDs,2,','),@pstartDate),'') As UserField2,
		dbo.GetEmployeeUserFieldNameById(dbo.UFN_SEPARATES_COLUMNS(@puserFieldIDs,2,',')) As UserField2Name,
		ISNULL(dbo.GetEmployeeUserFieldValueById(emp.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldIDs,3,','),@pstartDate),'') As UserField3,
		dbo.GetEmployeeUserFieldNameById(dbo.UFN_SEPARATES_COLUMNS(@puserFieldIDs,3,',')) As UserField3Name,
		ISNULL((select count(IDShift1) from DailySchedule ds where ds.IDEmployee = emp.IDEmployee and ds.IDShift1 in (select id from @shiftIDs) and ds.IsHolidays =1 and ds.Date between @yearStart and @yearEnd),0) as TakenHolidays,
		ISNULL((select sum(Value) from DailyAccruals da where da.IDEmployee = emp.IDEmployee  and da.IDConcept in (select id from @accrualIDs) and da.Date between @yearStart and @yearEnd and da.Value >= 0),0) as AddedHolidays, 
		ISNULL((select count(da.IDEmployee)
				from DailyAccruals da 
					inner join DailySchedule ds on da.IDEmployee = ds.IDEmployee and da.Date = ds.Date and ds.IsHolidays = 1
				where da.IDEmployee = emp.idemployee and da.IDConcept in (select id from @accrualIDs) and da.Date between @yearStart and @yearEnd and da.Value >= 0),0) As AccrualTunne,
		isnull(dbo.GetHolidayPeriods(@yearStart,@yearEnd,emp.IDEmployee),'') AS HolidayPeriods
		from sysrovwCurrentEmployeeGroups emp
			inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = emp.IDEmployee and @pstartDate between poe.BeginDate and poe.EndDate and poe.IdPassport = @pidPassport
			inner join @companyValues cv on cv.idGroup = CONVERT(int, dbo.UFN_SEPARATES_COLUMNS(emp.Path,1,'\'))

	RETURN NULL
GO

ALTER TABLE ReportLayouts
ADD Visible smallint NULL default 0
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='922' WHERE ID='DBVersion'
GO
