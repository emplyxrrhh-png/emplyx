--No aceptaba números de tarjeta mayores que un bigint
 ALTER function [dbo].[fn_decToBase] 
 ( 
 @val as decimal(38,0), 
 @base as int
 ) 
 returns varchar(63) 
 as
 Begin
       If (@val<0) OR (@base < 2) OR (@base> 36) OR (@val>9223372036854775807)  Return Null; 
 Declare @answer as varchar(63); 
 Declare @alldigits as varchar(36);
 Declare @valaux as bigint;
 Set @alldigits='0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ'
 Set @answer=''
 Set @valaux = convert(bigint,@val);
 While @valaux>0 
 Begin
 Set @answer=Substring(@alldigits,@valaux % @base + 1,1) + @answer; 
 Set @valaux = @valaux / @base; 
 End
 return @answer;
 End
GO

--Se aceptaba el login de un usuario sin contrato activo
ALTER PROCEDURE [dbo].[WebLogin_Authenticate] 
  	(
  		@method int,
 		@version nvarchar(50),
 		@biometricID int,
  		@credential nvarchar(255),
  		@password nvarchar(4000)
  	)
  AS
  	SELECT TOP 1 p.ID 
		FROM sysroPassports_AuthenticationMethods a
  			LEFT JOIN sysroPassports p ON a.IDPassport = p.ID
			LEFT JOIN sysrovwCurrentEmployeeGroups c on p.IDEmployee = c.IDEmployee
  		WHERE a.Method = @method AND
 			a.Version = @version AND
 			a.BiometricID = @biometricID AND 
  			a.Credential = @credential AND
  			a.Password = @password COLLATE SQL_Latin1_General_CP1_CS_AS AND
  			(a.StartDate IS NULL OR a.StartDate <= GetDate()) AND
  			(a.ExpirationDate IS NULL OR a.ExpirationDate > GetDate()) AND
 			a.Enabled = 1 AND
			(c.CurrentEmployee = 1 or p.IDEmployee is null)
  	
  	RETURN
GO

--Creación de procedures para nuevas analíticas
ALTER TABLE dbo.sysroSchedulerViews ADD
	UserFields nvarchar(4000) NULL
GO

CREATE FUNCTION [dbo].[SplitToInt](
  @String nvarchar (MAX),
  @Delimiter nvarchar (10)
  )
 returns @ValueTable table ([Value] int)
 begin
  declare @NextString nvarchar(MAX)
  declare @Pos int
  declare @NextPos int
  declare @CommaCheck nvarchar(1)
  --Initialize
  set @NextString = ''
  set @CommaCheck = right(@String,1) 
  --Check for trailing Comma, if not exists, INSERT
  --if (@CommaCheck <> @Delimiter )
  set @String = @String + @Delimiter
  --Get position of first Comma
  set @Pos = charindex(@Delimiter,@String)
  set @NextPos = 1
  --Loop while there is still a comma in the String of levels
  while (@pos <>  0)  
  begin
   set @NextString = substring(@String,1,@Pos - 1)
   insert into @ValueTable ( [Value]) Values (convert(int,@NextString))
   set @String = substring(@String,@pos +1,len(@String))
   set @NextPos = @Pos
   set @pos  = charindex(@Delimiter,@String)
  end
  return
 end
GO

CREATE FUNCTION dbo.UFN_SEPARATES_COLUMNS(
 @TEXT      varchar(8000)
,@COLUMN    tinyint
,@SEPARATOR char(1)
)RETURNS varchar(8000)
AS
  BEGIN
       DECLARE @POS_START  int = 1
       DECLARE @POS_END    int = CHARINDEX(@SEPARATOR, @TEXT, @POS_START)

       WHILE (@COLUMN >1 AND @POS_END> 0)
         BEGIN
             SET @POS_START = @POS_END + 1
             SET @POS_END = CHARINDEX(@SEPARATOR, @TEXT, @POS_START)
             SET @COLUMN = @COLUMN - 1
         END 

       IF @COLUMN > 1  SET @POS_START = LEN(@TEXT) + 1
       IF @POS_END = 0 SET @POS_END = LEN(@TEXT) + 1 

       RETURN SUBSTRING (@TEXT, @POS_START, @POS_END - @POS_START)
  END
GO

CREATE FUNCTION [dbo].[GetEmployeeUserFieldValueMin]
  (		
  	@idEmployee int,
  	@FieldName nvarchar(50),
  	@Date smalldatetime
  )
  RETURNS varchar(4000)
  AS
  BEGIN
	DECLARE @value varchar(4000)
  	
	SELECT TOP 1 @Value = CONVERT(varchar(4000), [Value])
  			FROM EmployeeUserFieldValues				
  			WHERE EmployeeUserFieldValues.IDEmployee = @idEmployee AND 
  				  EmployeeUserFieldValues.FieldName = @FieldName AND
  				  EmployeeUserFieldValues.Date <= @Date
  			ORDER BY EmployeeUserFieldValues.Date DESC
  	
	RETURN @value
  END
GO

CREATE PROCEDURE dbo.ObtainEmployeeIDsFromFilter
	@employeeFilter nvarchar(max)
 AS 
begin
	DECLARE @SQLString nvarchar(MAX);
	 SET @SQLString = 'SELECT DISTINCT IDEmployee FROM sysroEmployeeGroups WHERE ' + @employeeFilter
  exec sp_executesql @SQLString
end
GO

CREATE PROCEDURE [dbo].[Analytics_Incidences]
	@initialDate smalldatetime,
	@endDate smalldatetime,
	@idpassport int,
	@employeeFilter nvarchar(max),
	@userFieldsFilter nvarchar(max)
 AS
 
 DECLARE @employeeIDs Table(idEmployee int)
 insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter;

 SELECT     CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
                       + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup, 
                       dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description AS IncidenceName, 
                       dbo.TimeZones.Name AS ZoneTime, dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS Value, YEAR(dbo.DailySchedule.Date) AS Año, 
                       (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailySchedule.Date) AS WeekOfYear, DATEPART(dy, 
                       dbo.DailySchedule.Date) AS DayOfYear, CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName, 
                       dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.FullGroupName AS FullGroupName, dbo.sysroEmployeeGroups.Path, 
                       dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,
						dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),@endDate) As UserField1,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),@endDate) As UserField2,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),@endDate) As UserField3,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),@endDate) As UserField4,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),@endDate) As UserField5,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),@endDate) As UserField6,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),@endDate) As UserField7,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),@endDate) As UserField8,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),@endDate) As UserField9,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),@endDate) As UserField10

 FROM         dbo.sysroEmployeeGroups 
					INNER JOIN dbo.Causes 
					INNER JOIN dbo.DailyCauses ON dbo.Causes.ID = dbo.DailyCauses.IDCause 
					INNER JOIN dbo.DailySchedule ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailySchedule.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate
					INNER JOIN dbo.EmployeeContracts ON dbo.DailySchedule.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailySchedule.Date between dbo.EmployeeContracts.BeginDate AND dbo.EmployeeContracts.EndDate 
					LEFT OUTER JOIN dbo.sysroDailyIncidencesDescription 
					INNER JOIN dbo.DailyIncidences 
					INNER JOIN dbo.TimeZones ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID ON dbo.sysroDailyIncidencesDescription.IDIncidence = dbo.DailyIncidences.IDType ON dbo.DailyCauses.IDEmployee = dbo.DailyIncidences.IDEmployee AND dbo.DailyCauses.Date = dbo.DailyIncidences.Date AND dbo.DailyCauses.IDRelatedIncidence = dbo.DailyIncidences.ID 
					LEFT OUTER JOIN dbo.Shifts ON dbo.DailySchedule.IDShiftUsed = dbo.Shifts.ID 
					LEFT OUTER JOIN dbo.Shifts AS Shifts_1 ON dbo.DailySchedule.IDShift1 = Shifts_1.ID
					LEFT OUTER JOIN dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
WHERE	dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.DailyIncidences.IDEmployee,2,0,0,dbo.DailyIncidences.date) > 1
			AND dbo.sysroEmployeeGroups.IDEmployee in(select idEmployee from @employeeIDs) and dbo.DailyIncidences.Date between @initialDate and @endDate
 GROUP BY dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name, 
                       YEAR(dbo.DailySchedule.Date), CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
                       + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120), CASE WHEN DailySchedule.Date <= GETDATE() 
                       THEN Shifts.Name ELSE Shifts_1.Name END, dbo.Employees.Name, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, DATEPART(wk, 
                       dbo.DailySchedule.Date), DATEPART(dy, dbo.DailySchedule.Date), (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1, 
                       dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, 
                       dbo.sysroEmployeeGroups.CurrentEmployee, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate
GO

CREATE PROCEDURE  [dbo].[Analytics_Schedule]
	@initialDate smalldatetime,
	@endDate smalldatetime,
	@idpassport int,
	@employeeFilter nvarchar(max),
	@userFieldsFilter nvarchar(max)
 AS

 DECLARE @employeeIDs Table(idEmployee int)
 insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter;

	 SELECT     CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
						   + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10) AS KeyView, 
						   dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, 
						   dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeesShifts.ShiftName, 
						   dbo.sysroEmployeesShifts.CurrentDate AS Date, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours, COUNT(*) AS Count, 
						   MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año, (DATEPART(dw, 
						   dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear, 
						   DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, 
						   dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
							dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,
							dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
							dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
							dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
							dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
							dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
							dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),@endDate) As UserField1,
							dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),@endDate) As UserField2,
							dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),@endDate) As UserField3,
							dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),@endDate) As UserField4,
							dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),@endDate) As UserField5,
							dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),@endDate) As UserField6,
							dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),@endDate) As UserField7,
							dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),@endDate) As UserField8,
							dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),@endDate) As UserField9,
							dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),@endDate) As UserField10
	 FROM         dbo.sysroEmployeesShifts 
						   INNER JOIN dbo.sysroEmployeeGroups ON dbo.sysroEmployeesShifts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.sysroEmployeeGroups.EndDate 
						   INNER JOIN dbo.EmployeeContracts ON dbo.sysroEmployeesShifts.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.EmployeeContracts.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.EmployeeContracts.EndDate 
						   LEFT OUTER JOIN dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
	 WHERE	dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.sysroEmployeesShifts.IDEmployee,2,0,0,dbo.sysroEmployeesShifts.CurrentDate) > 1
			AND dbo.sysroEmployeesShifts.IDEmployee in( select idEmployee from @employeeIDs) and dbo.sysroEmployeesShifts.CurrentDate between @initialDate and @endDate
	 GROUP BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name, 
						   dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate, MONTH(dbo.sysroEmployeesShifts.CurrentDate), 
						   YEAR(dbo.sysroEmployeesShifts.CurrentDate), (DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, 
						   dbo.sysroEmployeesShifts.CurrentDate), DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate), dbo.sysroEmployeeGroups.IDGroup, 
						   dbo.EmployeeContracts.IDContract, CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
						   + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10), dbo.sysroEmployeeGroups.Path, 
						   dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
							dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate
GO

CREATE PROCEDURE [dbo].[Analytics_Concepts]
 	@initialDate smalldatetime,
	@endDate smalldatetime,
	@idpassport int,
	@employeeFilter nvarchar(max),
	@conceptsFilter nvarchar(max),
	@userFieldsFilter nvarchar(max),
	@includeZeros tinyint
 AS
 
 DECLARE @employeeIDs Table(idEmployee int)
 DECLARE @conceptIDs Table(idConcept int)

 insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter
 insert into @conceptIDs select * from dbo.SplitToInt(@conceptsFilter,',');
 
 IF @includeZeros = 1 
	 BEGIN
		 WITH alldays AS (
		  SELECT @initialDate AS dt
		  UNION ALL
		  SELECT DATEADD(dd, 1, dt)
			FROM alldays s
		   WHERE DATEADD(dd, 1, dt) <= @endDate)

	SELECT	CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120), 10) AS KeyView, 
			reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept, 
			dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName, 
			reqReg.conceptName AS ConceptName, reqReg.dt AS Date, sum(isnull(dbo.DailyAccruals.Value,0)) AS Value,COUNT(*) AS Count, MONTH(reqReg.dt) AS Mes, 
			YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, 
			reqReg.dt) AS WeekOfYear, DATEPART(dy, reqReg.dt) AS DayOfYear, dbo.sysroEmployeeGroups.Path, 
			dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
			dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,
			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),@endDate) As UserField1,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),@endDate) As UserField2,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),@endDate) As UserField3,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),@endDate) As UserField4,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),@endDate) As UserField5,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),@endDate) As UserField6,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),@endDate) As UserField7,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),@endDate) As UserField8,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),@endDate) As UserField9,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),@endDate) As UserField10
	FROM	(select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays, 
				(select ID, Name from Employees inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp, 
				(select ID, Name from Concepts inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con
				where dbo.WebLogin_GetPermissionOverEmployee(@idpassport,emp.ID,2,0,0,alldays.dt) > 1
			) reqReg
			INNER JOIN dbo.sysroEmployeeGroups ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate
			INNER JOIN dbo.EmployeeContracts ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
			LEFT OUTER JOIN dbo.DailyAccruals on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept

	GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName, 
			dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt), 
			YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, reqReg.dt), DATEPART(dy, 
			reqReg.dt), CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar) 
			+ '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120), 
			10), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
			dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate
	option (maxrecursion 0)
 END
 ELSE
	 BEGIN
		WITH alldays AS (
		  SELECT @initialDate AS dt
		  UNION ALL
		  SELECT DATEADD(dd, 1, dt)
			FROM alldays s
		   WHERE DATEADD(dd, 1, dt) <= @endDate)

	SELECT	CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120), 10) AS KeyView, 
			reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept, 
			dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName, 
			reqReg.conceptName AS ConceptName, reqReg.dt AS Date, sum(isnull(dbo.DailyAccruals.Value,0)) AS Value, COUNT(*) AS Count, MONTH(reqReg.dt) AS Mes, 
			YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, 
			reqReg.dt) AS WeekOfYear, DATEPART(dy, reqReg.dt) AS DayOfYear, dbo.sysroEmployeeGroups.Path, 
			dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
			dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,
			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),@endDate) As UserField1,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),@endDate) As UserField2,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),@endDate) As UserField3,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),@endDate) As UserField4,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),@endDate) As UserField5,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),@endDate) As UserField6,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),@endDate) As UserField7,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),@endDate) As UserField8,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),@endDate) As UserField9,
			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),@endDate) As UserField10
	FROM	(select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays, 
				(select ID, Name from Employees inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp, 
				(select ID, Name from Concepts inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con
				where dbo.WebLogin_GetPermissionOverEmployee(@idpassport,emp.ID,2,0,0,alldays.dt) > 1
			) reqReg
			INNER JOIN dbo.sysroEmployeeGroups ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate
			INNER JOIN dbo.EmployeeContracts ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
			INNER JOIN dbo.DailyAccruals on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept

	GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName, 
			dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt), 
			YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, reqReg.dt), DATEPART(dy, 
			reqReg.dt), CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar) 
			+ '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120), 
			10), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
			dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate
	option (maxrecursion 0)

 END

GO

CREATE PROCEDURE [dbo].[Analytics_Punches]
	@initialDate smalldatetime,
	@endDate smalldatetime,
	@idpassport int,
	@employeeFilter nvarchar(max),
	@userFieldsFilter nvarchar(max)
 AS
 
 DECLARE @employeeIDs Table(idEmployee int)
 insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter;

 SELECT     dbo.Punches.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee, 
                       dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Punches.ActualType AS PunchDirection, 
                       dbo.Punches.TypeData AS PunchIDTypeData, CASE WHEN dbo.Punches.TypeData <> 0 THEN dbo.Causes.Name ELSE '' END AS PunchTypeData, 
                       dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day, 
                       MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Año, (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) 
                       % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName, 
                       dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END AS IsInvalid, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails) 
                       AS Details, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, 
                       dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, dbo.Punches.IsNotReliable, dbo.Punches.Location AS PunchLocation, 
                       dbo.Punches.LocationZone AS PunchLocationZone, dbo.Punches.TimeZone AS PunchTimeZone, dbo.sysroPassports.Name AS EditorName,
					   dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),@endDate) As UserField1,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),@endDate) As UserField2,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),@endDate) As UserField3,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),@endDate) As UserField4,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),@endDate) As UserField5,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),@endDate) As UserField6,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),@endDate) As UserField7,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),@endDate) As UserField8,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),@endDate) As UserField9,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),@endDate) As UserField10
 FROM         dbo.sysroEmployeeGroups INNER JOIN
                       dbo.Punches ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee AND dbo.Punches.ShiftDate >= dbo.sysroEmployeeGroups.BeginDate AND 
                       dbo.Punches.ShiftDate <= dbo.sysroEmployeeGroups.EndDate INNER JOIN
                       dbo.EmployeeContracts ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND 
                       dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                       dbo.Zones ON dbo.Punches.IDZone = dbo.Zones.ID LEFT OUTER JOIN
                       dbo.Terminals ON dbo.Punches.IDTerminal = dbo.Terminals.ID LEFT OUTER JOIN
                       dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID LEFT OUTER JOIN
                       dbo.sysroPassports ON dbo.Punches.IDPassport = dbo.sysroPassports.ID LEFT OUTER JOIN
                       dbo.Causes ON dbo.Punches.TypeData = dbo.Causes.ID
 WHERE	dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.sysroEmployeeGroups.IDEmployee,2,0,0,dbo.Punches.ShiftDate) > 1
			AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @initialDate and @endDate
 GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID, 
                       dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108), 
                       dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name, 
                       (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name, 
                       CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path, 
                       dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, 
                       dbo.Punches.IsNotReliable, dbo.Punches.Location, dbo.Punches.LocationZone, dbo.Punches.TimeZone, dbo.sysroPassports.Name, dbo.Punches.ActualType, 
                       dbo.Causes.Name, dbo.Punches.TypeData, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate
 HAVING      (dbo.Punches.Type = 1) OR
                       (dbo.Punches.Type = 2) OR
                       (dbo.Punches.Type = 3) OR
                       (dbo.Punches.Type = 7)
GO

CREATE PROCEDURE [dbo].[Analytics_Tasks]
	@initialDate smalldatetime,
	@endDate smalldatetime,
	@idpassport int,
	@userFieldsFilter nvarchar(max)
 AS

  SELECT     dbo.sysroEmployeeGroups.IDGroup, dbo.sysroEmployeeGroups.FullGroupName AS GroupName, dbo.Employees.ID AS IDEmployee, 
                        dbo.Employees.Name AS EmployeeName, DATEPART(year, dbo.DailyTaskAccruals.Date) AS Date_Year, DATEPART(month, dbo.DailyTaskAccruals.Date) 
                        AS Date_Month, DATEPART(day, dbo.DailyTaskAccruals.Date) AS Date_Day, dbo.DailyTaskAccruals.Date, dbo.BusinessCenters.ID AS IDCenter, 
                        dbo.BusinessCenters.Name AS CenterName, dbo.Tasks.ID AS IDTask, dbo.Tasks.Name AS TaskName, dbo.Tasks.InitialTime, ISNULL(dbo.Tasks.Field1, '') 
                        AS Field1_Task, ISNULL(dbo.Tasks.Field2, '') AS Field2_Task, ISNULL(dbo.Tasks.Field3, '') AS Field3_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field4), 0) 
                        AS Field4_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field5), 0) AS Field5_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field6), 0) AS Field6_Task, 
                        ISNULL(dbo.DailyTaskAccruals.Field1, '') AS Field1_Total, ISNULL(dbo.DailyTaskAccruals.Field2, '') AS Field2_Total, ISNULL(dbo.DailyTaskAccruals.Field3, '') 
                        AS Field3_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field4), 0) AS Field4_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field5), 
                        0) AS Field5_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field6), 0) AS Field6_Total, ISNULL(CONVERT(numeric(25, 6), 
                        dbo.DailyTaskAccruals.Value), 0) AS Value, ISNULL(dbo.Tasks.Project, '') AS Project, ISNULL(dbo.Tasks.Tag, '') AS Tag, dbo.Tasks.IDPassport, 
                        ISNULL(dbo.Tasks.TimeChangedRequirements, 0) AS TimeChangedRequirements, ISNULL(dbo.Tasks.ForecastErrorTime, 0) AS ForecastErrorTime, 
                        ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) AS NonProductiveTimeIncidence, ISNULL(dbo.Tasks.EmployeeTime, 0) AS EmployeeTime, 
                        ISNULL(dbo.Tasks.TeamTime, 0) AS TeamTime, ISNULL(dbo.Tasks.MaterialTime, 0) AS MaterialTime, ISNULL(dbo.Tasks.OtherTime, 0) AS OtherTime, 
                        ISNULL(dbo.Tasks.InitialTime, 0) + ISNULL(dbo.Tasks.TimeChangedRequirements, 0) + ISNULL(dbo.Tasks.ForecastErrorTime, 0) 
                        + ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) + ISNULL(dbo.Tasks.EmployeeTime, 0) + ISNULL(dbo.Tasks.TeamTime, 0) + ISNULL(dbo.Tasks.MaterialTime, 0) 
                        + ISNULL(dbo.Tasks.OtherTime, 0) AS Duration, 
                        CASE WHEN Tasks.Status = 0 THEN 'Activa' ELSE CASE WHEN Tasks.Status = 1 THEN 'Finalizada' ELSE CASE WHEN Tasks.Status = 2 THEN 'Cancelada' ELSE 'Pendiente'
                         END END END AS Estado, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
					    dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),@endDate) As UserField1,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),@endDate) As UserField2,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),@endDate) As UserField3,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),@endDate) As UserField4,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),@endDate) As UserField5,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),@endDate) As UserField6,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),@endDate) As UserField7,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),@endDate) As UserField8,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),@endDate) As UserField9,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),@endDate) As UserField10
  FROM         dbo.sysroEmployeeGroups 
                        INNER JOIN dbo.DailyTaskAccruals ON dbo.DailyTaskAccruals.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.DailyTaskAccruals.Date BETWEEN dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate 
                        INNER JOIN dbo.Tasks ON dbo.Tasks.ID = dbo.DailyTaskAccruals.IDTask 
                        INNER JOIN dbo.BusinessCenters ON dbo.Tasks.IDCenter = dbo.BusinessCenters.ID 
                        INNER JOIN dbo.EmployeeContracts ON dbo.DailyTaskAccruals.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyTaskAccruals.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyTaskAccruals.Date <= dbo.EmployeeContracts.EndDate
						INNER JOIN dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
  WHERE     (dbo.businesscenters.id is null OR dbo.BusinessCenters.ID IN (SELECT DISTINCT IDCenter FROM sysroPassports_Centers WHERE IDPassport=@idpassport) )
			AND dbo.DailyTaskAccruals.Date between @initialDate and @endDate
GO

CREATE PROCEDURE [dbo].[Analytics_CostCenters]
	@initialDate smalldatetime,
	@endDate smalldatetime,
	@idpassport int,
	@userFieldsFilter nvarchar(max)
 AS

  SELECT     CONVERT(varchar(10), dbo.DailyCauses.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
                        + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailyCauses.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup, 
                        dbo.EmployeeContracts.IDContract, dbo.DailyCauses.IDEmployee, dbo.DailyCauses.Date,
                        dbo.Causes.Name AS CauseName, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor, isnull(dbo.BusinessCenters.ID, 0) as IDCenter, 
						isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,  
						isnull(dbo.BusinessCenters.Field4, '') AS Field4, dbo.DailyCauses.Value AS Value, YEAR(dbo.DailyCauses.Date) AS Año, 
                        (DATEPART(dw, dbo.DailyCauses.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailyCauses.Date) AS WeekOfYear, DATEPART(dy, 
                        dbo.DailyCauses.Date) AS DayOfYear, dbo.DailyCauses.DefaultCenter, dbo.DailyCauses.ManualCenter , 
                        dbo.Employees.Name AS EmployeeName, dbo.Groups.Name AS GroupName, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName, dbo.Groups.Path, 
                        dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee, 
						dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroCostHour'),DailyCauses.date ) as Cost, 
						dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPriceHour'), DailyCauses.date) as PVP,
					    dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),@endDate) As UserField1,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),@endDate) As UserField2,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),@endDate) As UserField3,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),@endDate) As UserField4,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),@endDate) As UserField5,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),@endDate) As UserField6,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),@endDate) As UserField7,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),@endDate) As UserField8,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),@endDate) As UserField9,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),@endDate) As UserField10
  FROM         dbo.sysroEmployeeGroups 
                        INNER JOIN dbo.Causes 
                        INNER JOIN dbo.DailyCauses ON dbo.Causes.ID = dbo.DailyCauses.IDCause  ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.DailyCauses.Date between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate 
						INNER JOIN dbo.EmployeeContracts ON dbo.DailyCauses.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyCauses.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyCauses.Date <= dbo.EmployeeContracts.EndDate 
						INNER JOIN dbo.Groups ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID 
						LEFT OUTER JOIN dbo.Employees ON dbo.DailyCauses.IDEmployee = dbo.Employees.ID  
						LEFT OUTER JOIN dbo.BusinessCenters ON dbo.DailyCauses.IDCenter = dbo.BusinessCenters.ID 
   where     (dbo.businesscenters.id is null or dbo.businesscenters.id in (select distinct idcenter from sysropassports_centers where idpassport=@idpassport) )
			 and dbo.dailycauses.date between @initialdate and @enddate
GO

CREATE PROCEDURE [dbo].[Analytics_Authorizations]
	@idpassport int,
	@employeeFilter nvarchar(max),
	@userFieldsFilter nvarchar(max)
 AS
  DECLARE @employeeIDs Table(idEmployee int)
  insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter;

 SELECT dbo.sysroEmployeeGroups.IDEmployee AS IDEmployee, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.IDGroup AS IDGroup, dbo.sysroEmployeeGroups.FullGroupName As FullGroupName, dbo.sysroEmployeeGroups.Path AS GroupPath, 
        dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee, dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract AS IDContract, dbo.AccessGroups.Name AS AuthorizationName, 
        Zones.Name AS ZoneName, zones.IsWorkingZone AS IsWorkingZone, AccessPeriods.Name As AccessPeriodName, 1 AS BelongsToGroup,
		dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract,
		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),getdate()) As UserField1,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),getdate()) As UserField2,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),getdate()) As UserField3,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),getdate()) As UserField4,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),getdate()) As UserField5,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),getdate()) As UserField6,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),getdate()) As UserField7,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),getdate()) As UserField8,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),getdate()) As UserField9,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),getdate()) As UserField10
     FROM dbo.sysroEmployeeGroups 
      INNER JOIN dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
      INNER JOIN dbo.EmployeeContracts ON dbo.Employees.ID = dbo.EmployeeContracts.IDEmployee and getdate() between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
      INNER JOIN dbo.AccessGroups ON dbo.AccessGroups.ID IN
                     (SELECT        dbo.Employees.IDAccessGroup
                     UNION
                     SELECT        IDAuthorization
                     FROM            dbo.EmployeeAccessAuthorization
                     WHERE        (IDEmployee = dbo.Employees.ID))
      INNER JOIN dbo.AccessGroupsPermissions ON AccessGroupsPermissions.IDAccessGroup = AccessGroups.ID
      INNER JOIN dbo.AccessPeriods ON AccessPeriods.ID = AccessGroupsPermissions.IDAccessPeriod
      INNER JOIN dbo.Zones on Zones.ID = AccessGroupsPermissions.IDZone
	WHERE dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.sysroEmployeeGroups.IDEmployee,2,0,0,getdate()) > 1
			AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) AND getdate() between sysroEmployeeGroups.BeginDate and sysroEmployeeGroups.EndDate
GO

CREATE PROCEDURE [dbo].[Analytics_Access]
	@initialDate smalldatetime,
	@endDate smalldatetime,
	@idpassport int,
	@employeeFilter nvarchar(max),
	@userFieldsFilter nvarchar(max)
 AS
  DECLARE @employeeIDs Table(idEmployee int)
  insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter;

  SELECT     dbo.Punches.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee, 
                        dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, CONVERT(nvarchar(8), 
                        dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day, MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Year, 
                        (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, 
                        dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName, dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL 
                        THEN 0 ELSE 1 END AS IsInvalid, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails) AS Details, dbo.sysroEmployeeGroups.Path, 
                        dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,
					    dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),@endDate) As UserField1,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),@endDate) As UserField2,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),@endDate) As UserField3,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),@endDate) As UserField4,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),@endDate) As UserField5,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),@endDate) As UserField6,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),@endDate) As UserField7,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),@endDate) As UserField8,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),@endDate) As UserField9,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),@endDate) As UserField10
  FROM         dbo.sysroEmployeeGroups 
						INNER JOIN dbo.Punches ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee 
						INNER JOIN dbo.EmployeeContracts ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate 
						LEFT OUTER JOIN dbo.Zones ON dbo.Punches.IDZone = dbo.Zones.ID 
						LEFT OUTER JOIN dbo.Terminals ON dbo.Punches.IDTerminal = dbo.Terminals.ID 
						LEFT OUTER JOIN dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
  WHERE     dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.sysroEmployeeGroups.IDEmployee,2,0,0,dbo.Punches.ShiftDate) > 1
			AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @initialDate and @endDate
  GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID, 
                        dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108), 
                        dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name, 
                        (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name, 
                        CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path, 
                        dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, 
                        dbo.sysroEmployeeGroups.IDGroup,dbo.EmployeeContracts.BeginDate , dbo.EmployeeContracts.EndDate
  HAVING (dbo.Punches.Type = 5) OR (dbo.Punches.Type = 6) OR (dbo.Punches.Type = 7)
GO

CREATE PROCEDURE [dbo].[Analytics_AccessPlates]
	@initialDate smalldatetime,
	@endDate smalldatetime,
	@idpassport int,
	@employeeFilter nvarchar(max),
	@userFieldsFilter nvarchar(max)
 AS
  DECLARE @employeeIDs Table(idEmployee int)
  insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter;

  SELECT     dbo.Punches.ID, dbo.Punches.IDEmployee, dbo.Punches.DateTime AS DatePunche, CONVERT(VARCHAR(8), dbo.Punches.DateTime, 108) AS TimePunche, 
                        CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails) AS TypeDetails, dbo.Employees.Name AS NameEmployee, dbo.Zones.Name AS NameZone, 
                        dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.IDGroup, 
                        dbo.sysroEmployeeGroups.EndDate,
					    dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),@endDate) As UserField1,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),@endDate) As UserField2,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),@endDate) As UserField3,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),@endDate) As UserField4,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),@endDate) As UserField5,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),@endDate) As UserField6,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),@endDate) As UserField7,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),@endDate) As UserField8,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),@endDate) As UserField9,
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),@endDate) As UserField10
  FROM         dbo.Punches 
						INNER JOIN dbo.TerminalReaders ON dbo.Punches.IDTerminal = dbo.TerminalReaders.IDTerminal AND dbo.Punches.IDReader = dbo.TerminalReaders.ID 
						INNER JOIN dbo.sysroEmployeeGroups ON dbo.Punches.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.Punches.DateTime >= dbo.sysroEmployeeGroups.BeginDate AND dbo.Punches.DateTime <= dbo.sysroEmployeeGroups.EndDate 
						INNER JOIN dbo.EmployeeContracts ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate 
						LEFT OUTER JOIN dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID 
						LEFT OUTER JOIN dbo.Zones ON dbo.Punches.IDZone = dbo.Zones.ID
  WHERE     (dbo.TerminalReaders.Type = N'MAT') AND (NOT (dbo.Punches.TypeDetails IS NULL)) AND (dbo.Punches.Type = 5 OR dbo.Punches.Type = 6 OR dbo.Punches.Type = 7)
			AND dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.sysroEmployeeGroups.IDEmployee,2,0,0,dbo.Punches.ShiftDate) > 1
			AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @initialDate and @endDate
GO

ALTER FUNCTION [dbo].[GetSupervisedEmployeesByPassport]
   (
   	@idPassport int,
   	@featureAlias nvarchar(50)
   )
   RETURNS @result table (ID int PRIMARY KEY)
   AS
   	BEGIN
   		DECLARE @EmployeeID int
   		DECLARE @SupervisorLevel int
   		DECLARE @featureEmployeeID int
   		DECLARE @featurePermission int
   		DECLARE @parentPassport int
    		
  		DECLARE @GroupType nvarchar(50)
   	
  		SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @idPassport
    		
    		if @GroupType = 'U'
    		begin
    			SET @parentPassport = @idPassport
    		end
    		else
    		begin
    			SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @idPassport
    		end
   		
   		SELECT @SupervisorLevel = (SELECT dbo.GetPassportLevelOfAuthority(@idPassport))
   		SELECT @featureEmployeeID = (SELECT EmployeeFeatureID from sysroFeatures where Alias = @featureAlias and Type='U')
   		SELECT @featurePermission = (dbo.WebLogin_GetPermissionOverFeature(@idPassport, @featureAlias,'U',2))
   		
   		IF @featurePermission > 3
   			BEGIN
   				INSERT INTO @result
  					select IDEmployee from (
  						select IDEmployee from sysrovwCurrentEmployeeGroups where IDGroup in (
  							SELECT EmployeeGroupID from (select EmployeeGroupID, MAX(LevelOfAuthority) as MaxLevel, PassportID from [dbo].[sysroPermissionsOverGroups]
   												where Permission > 3 and  EmployeeFeatureID  = @featureEmployeeID  group by EmployeeGroupID,PassportID) allPerm
  							WHERE allPerm.PassportID = @parentPassport and allPerm.MaxLevel = @SupervisorLevel)) allEmployees
  						WHERE IDEmployee not in (select EmployeeID from sysroPermissionsOverEmployeesExceptions where PassportID = @parentPassport and EmployeeFeatureID = @featureEmployeeID and Permission <= 3)
   			END
   		RETURN
   	END
GO

--Modo accesos 1 sentido Presencia en 2 con Portal del empleado
INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,92,'ACCTAEIP',1,'Blind','E,S','LocalServer,ServerLocal,Server,Local','0','1,0','0','1,0','1,0',NULL,1,NULL,'')
GO

UPDATE dbo.sysroParameters SET Data='379' WHERE ID='DBVersion'
GO
