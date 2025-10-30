CREATE OR ALTER PROCEDURE [dbo].[ObtainEmployeeAndPermissionsFromFilter]  
	@idPassport nvarchar(100) = '1',
	@employeeFilter nvarchar(max),  
	@initialDate datetime2,  
	@endDate datetime2  ,  
	@referenceDate datetime2  
AS  
BEGIN  
	DECLARE @pidPassport nvarchar(100) = @idPassport,
			@pemployeeFilter nvarchar(max) = @employeeFilter,
			@pinitialDate smalldatetime   = @initialDate,
			@pendDate smalldatetime  = @endDate,
			@preferenceDate smalldatetime  = @referenceDate  
  
	DECLARE @SQLString nvarchar(MAX);  


	SET @SQLString = 'WITH alldays AS (  
				SELECT CONVERT(datetime,''' + CONVERT(VARCHAR(10), @initialDate, 112) + ''',112) AS dt  
				UNION ALL  
				SELECT DATEADD(dd, 1, dt)  
				FROM alldays s  
				WHERE DATEADD(dd, 1, dt) <= CONVERT(datetime,''' + CONVERT(VARCHAR(10), @pendDate, 112) + ''',112)) '

	SET @SQLString = @SQLString + ' select IDEmployee, Date, 
				case when (actualcontract = ''0'' and referencefuturecontract <> ''0'' ) then 1 else (case when actualcontract <> ''0'' and actualcontract = referenceContract then 1 else 0 end) end as CurrentPeriod from (
				select emp.IDEmployee AS IDEmployee, alldays.dt AS [Date], 
						ISNULL((select idcontract from employeecontracts ac where ac.idemployee = emp.IDEmployee and CONVERT(datetime,''' + CONVERT(VARCHAR(10), @preferenceDate, 112) + ''',112) between ac.BeginDate and ac.EndDate),''0'') as actualcontract,   
						ISNULL(cc.IDContract,''0'') as referenceContract, 
						ISNULL(fc.IDContract,''0'') as referencefuturecontract 
				from alldays
				inner join ( SELECT SYSROEMPLOYEEGROUPS.IdEmployee, IDGroup,  
									CASE WHEN SYSROEMPLOYEEGROUPS.BeginDate <= EC.BeginDate THEN EC.BeginDate ELSE SYSROEMPLOYEEGROUPS.BeginDate END AS StartDate,  
									CASE WHEN SYSROEMPLOYEEGROUPS.ENDDATE >= EC.ENDDATE THEN EC.ENDDATE ELSE SYSROEMPLOYEEGROUPS.ENDDate END AS EndDate 
								 FROM EmployeeGroups AS SYSROEMPLOYEEGROUPS 
									INNER JOIN EMPLOYEECONTRACTS AS EC ON EC.IDEmployee = SYSROEMPLOYEEGROUPS.IDEMPLOYEE '  
	SET @SQLString = @SQLString + ' WHERE (' + @pemployeeFilter + ') ) emp on alldays.dt between emp.StartDate and emp.EndDate '  
	SET @SQLString = @SQLString + ' INNER JOIN sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = emp.IDEmployee and emp.StartDate between poe.BeginDate and poe.EndDate and poe.IdPassport = ' + @idPassport	  
  

	SET @SQLString = @SQLString + ' left join EmployeeContracts cc on emp.IDEmployee = cc.IDEmployee and alldays.dt between cc.BeginDate and cc.EndDate and  CONVERT(datetime,''' + CONVERT(VARCHAR(10), @preferenceDate, 112) + ''',112) between cc.BeginDate and cc.EndDate
					left join ( select ROW_NUMBER() over (PArtition by idemployee order by idemployee, begindate) as rowNumber, IDEmployee, BeginDate, EndDate, IDContract from employeecontracts where BeginDate >  CONVERT(datetime,''' + CONVERT(VARCHAR(10), @preferenceDate, 112) + ''',112) ) fc on fc.IDEmployee = emp.IDEmployee and rowNumber = 1 and alldays.dt between fc.BeginDate and fc.EndDate
					) empReferencePeriods
					order by empReferencePeriods.IDEmployee,empReferencePeriods.Date
					option (maxrecursion 0, USE HINT(''ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS''))' 

	exec sp_executesql @SQLString  
end  
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_mapadoferias]
	@idPassport nvarchar(100) = '1',
	@employeeIds nvarchar(max) = '0',
	@userFieldIDs nvarchar(max) = '0',
	@holidayIDs nvarchar(max) = '32,33',
	@startDate datetime2 = '20210301'
AS

	DECLARE @pidPassport nvarchar(100) = @idPassport,
	@puserFieldIDs nvarchar(max) = @userFieldIDs,
	@pholidayIDs nvarchar(max) = @holidayIDs,
	@pemployeeIds nvarchar(max) = @employeeIds,
	@pstartDate datetime2 = @startDate,
	@yearStart datetime2 = @startDate,
	@yearEnd datetime2 = @startDate

	IF @pholidayIDs = '' SET @pholidayIDs = '-1'
	IF @pemployeeIds = '' SET @pemployeeIds = '-1 = 1'

	DECLARE @shiftIDs Table(id int)
	INSERT INTO @shiftIDs(id) exec dbo.StringToIdsTable @pholidayIDs, 'Shifts','Id';

	DECLARE @companyValues Table(idGroup int, userFieldValue nvarchar(max))
	insert into @companyValues exec dbo.GetCompanysFieldValue 'USR_IDFiscal';

	DECLARE @accrualIDs Table(id int)
	insert into @accrualIDs
	select distinct id from Concepts where id in (select IDConceptBalance from shifts where id in (select id from @shiftIDs))

	SELECT @yearStart = dbo.GetStartOfYear(YEAR(@startDate))
	SET @yearEnd = DATEADD(DAY, -1, DATEADD(YEAR, 1, @yearStart))

	DECLARE @selEmployeePermissions Table(IdEmployee int, [Date] datetime,currentEmployee int)
	insert into @selEmployeePermissions exec dbo.ObtainEmployeeAndPermissionsFromFilter @pidPassport, @pemployeeIds,@yearStart,@yearEnd,@pstartDate;

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
		ISNULL((select count(IDShift1) from DailySchedule ds 
						inner join @selEmployeePermissions sep on sep.currentEmployee = 1 and ds.IDEmployee = sep.IdEmployee and sep.Date = ds.Date
						where ds.IDEmployee = emp.IDEmployee and ds.IDShift1 in (select id from @shiftIDs) and ds.IsHolidays =1 and ds.Date between (select min(date) from @selEmployeePermissions poe where poe.idemployee = emp.idemployee and currentemployee =1) and (select max(date) from @selEmployeePermissions poe where poe.idemployee = emp.idemployee and currentemployee =1)),0) as TakenHolidays,
		ISNULL((select sum(Value) from DailyAccruals da 
						inner join @selEmployeePermissions sep on sep.currentEmployee = 1 and da.IDEmployee = sep.IdEmployee and sep.Date = da.Date
						where da.IDEmployee = emp.IDEmployee  and da.IDConcept in (select id from @accrualIDs) and da.Date between (select min(date) from @selEmployeePermissions poe where poe.idemployee = emp.idemployee and currentemployee =1) and (select max(date) from @selEmployeePermissions poe where poe.idemployee = emp.idemployee and currentemployee =1) and da.Value >= 0),0) as AddedHolidays, 
		ISNULL((select count(distinct da.Date)
				from DailyAccruals da 
					inner join DailySchedule ds on da.IDEmployee = ds.IDEmployee and da.Date = ds.Date and ds.IsHolidays = 1
					inner join @selEmployeePermissions sep on sep.currentEmployee = 1 and ds.IDEmployee = sep.IdEmployee and sep.Date = ds.Date
				where da.IDEmployee = emp.idemployee and da.IDConcept in (select id from @accrualIDs) and da.Date between @yearStart and @yearEnd and da.Value >= 0),0) As AccrualTunne,
		isnull(dbo.GetHolidayPeriods((select min(date) from @selEmployeePermissions poe where poe.idemployee = emp.idemployee and currentemployee =1),(select max(date) from @selEmployeePermissions poe where poe.idemployee = emp.idemployee and currentemployee =1),emp.IDEmployee, @pholidayIDs),'') AS HolidayPeriods
		from sysrovwCurrentEmployeeGroups emp
			inner join @companyValues cv on cv.idGroup = CONVERT(int, dbo.UFN_SEPARATES_COLUMNS(emp.Path,1,'\'))
		WHERE emp.IDEmployee in (select idemployee from @selEmployeePermissions poe where poe.currentEmployee = 1)
	RETURN NULL
GO


-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='933' WHERE ID='DBVersion'
GO
