ALTER PROCEDURE [dbo].[GetPivotAccrualsWithZeros]
    @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max) 
 AS
 BEGIN
  DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter  
  DECLARE @employeeIDs Table(idEmployee int)
  DECLARE @conceptNames NVARCHAR(MAX)
  DECLARE @conceptNamesToHours NVARCHAR(MAX)
  DECLARE @conceptNamesWithZero NVARCHAR(MAX)
  DECLARE @conceptNamesWithZeroToHours NVARCHAR(MAX)
  DECLARE @employeeIDFilter NVARCHAR(MAX)
  DECLARE @intdatefirst int
  SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'  
  SET DateFirst @intdatefirst;  
  insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate
  select @employeeIDFilter = substring(tmp.empIds,2, len(tmp.empIds) -1) from 
 	(select convert(nvarchar(max), (select ',' + convert(nvarchar(max),idemployee) as [data()] from @employeeIDs FOR XML PATH(''))) as empIds) tmp
  select @conceptNames = substring(tmp.conceptNames,2, LEN(tmp.conceptNames) -1) from 
 	(select convert(nvarchar(max), (SELECT ',[' + ST1.name +']' AS [data()]
                 FROM dbo.concepts ST1 WITH(NOLOCK)
 				where st1.ID in(select Value from dbo.SplitToInt(@pconceptsFilter,','))
                 FOR XML PATH(''))) as conceptNames) tmp

  select @conceptNamesWithZero = substring(tmp.conceptNames,2, LEN(tmp.conceptNames) -1) from 
 	(select convert(nvarchar(max), (SELECT ',ISNULL([' + ST1.name +'],0) AS [' + ST1.name +']' AS [data()]
                 FROM dbo.concepts ST1 WITH(NOLOCK)
 				where st1.ID in(select Value from dbo.SplitToInt(@pconceptsFilter,','))
                 FOR XML PATH(''))) as conceptNames) tmp
				 

   select @conceptNamesToHours = substring(tmp.conceptNames,2, LEN(tmp.conceptNames) -1) from 
 	(select convert(nvarchar(max), (SELECT ',[' + ST1.name +'] as [' + st1.name + '(HH:MM)_ToHours]' AS [data()]
                 FROM dbo.concepts ST1 WITH(NOLOCK)
 				where st1.ID in(select Value from dbo.SplitToInt(@pconceptsFilter,','))
                 FOR XML PATH(''))) as conceptNames) tmp

	select @conceptNamesWithZeroToHours = substring(tmp.conceptNames,2, LEN(tmp.conceptNames) -1) from 
 	(select convert(nvarchar(max), (SELECT ',ISNULL([' + ST1.name +'],0) as [' + st1.name + '(HH:MM)_ToHours]' AS [data()]
                 FROM dbo.concepts ST1 WITH(NOLOCK)
 				where st1.ID in(select Value from dbo.SplitToInt(@pconceptsFilter,','))
                 FOR XML PATH(''))) as conceptNames) tmp
				 
   DECLARE @SqlStatement NVARCHAR(MAX)
   SET @SqlStatement = N'
 	WITH alldays AS (  
            SELECT convert(smalldatetime,''' + convert(nvarchar(max),@pinitialDate,112) + ''',120) AS dt  
            UNION ALL  
            SELECT DATEADD(dd, 1, dt)  
           FROM alldays s  
             WHERE DATEADD(dd, 1, dt) <=  convert(smalldatetime,''' + convert(nvarchar(max),@pendDate,112) + ''',120))  
 	SELECT 
 		accruals.IDEmployee As IDEmployee, eg.IDGroup, ec.IDContract, eg.GroupName, eg.FullGroupName, emp.Name AS EmployeeName, accruals.Date AS Date_ToDateString, accruals.Date AS Date,
 		MONTH(accruals.Date) AS Mes, YEAR(accruals.Date) AS Año, (DATEPART(dw, accruals.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, accruals.Date) AS WeekOfYear, 
 		DATEPART(dy, accruals.Date) AS DayOfYear, DATEPART(QUARTER, accruals.Date) AS Quarter, eg.Path, eg.CurrentEmployee, eg.BeginDate, eg.BeginDate AS BeginDate_ToDateString, eg.EndDate, eg.EndDate AS EndDate_ToDateString, ec.BeginDate AS BeginContract, ec.BeginDate AS BeginContract_ToDateString, ec.EndDate AS EndContract, ec.EndDate AS EndContract_ToDateString,
 		dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,1,''\'') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,2,''\'') As Nivel2,  
  		dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,3,''\'') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,4,''\'') As Nivel4,  
  		dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,5,''\'') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,6,''\'') As Nivel6,  
  		dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,7,''\'') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,8,''\'') As Nivel8,  
  		dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,9,''\'') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,10,''\'') As Nivel10,
 		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',1,'',''),accruals.Date) As UserField1,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',2,'',''),accruals.Date) As UserField2,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',3,'',''),accruals.Date) As UserField3,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',4,'',''),accruals.Date) As UserField4,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',5,'',''),accruals.Date) As UserField5,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',6,'',''),accruals.Date) As UserField6,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',7,'',''),accruals.Date) As UserField7,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',8,'',''),accruals.Date) As UserField8,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',9,'',''),accruals.Date) As UserField9,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',10,'',''),accruals.Date) As UserField10,
 		dbo.GetEmployeeAge(emp.ID) As Age,
 		' + @conceptNamesWithZero + ',' + @conceptNamesWithZeroToHours + ', dbo.GetPunchesByEmployeeAndDate(emp.ID,accruals.Date) as punches,
		dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,
		(select count(*) from punches where IDEmployee = emp.ID and ShiftDate = accruals.Date and ActualType = 1) as InPunches,
		(select count(*) from punches where IDEmployee = emp.ID and ShiftDate = accruals.Date and ActualType = 2) as OutPunches
 	FROM(
     select alldays.dt as Date, employees.ID as IDEmployee, '+ @conceptNames +' from alldays
 	cross join Employees
 	left outer join (SELECT *
       FROM
          (SELECT Concepts.name AS ConceptName, Value, IDEmployee, Date from DailyAccruals WITH(NOLOCK) inner join Concepts WITH(NOLOCK) on dailyaccruals.IDConcept = Concepts.ID
 		  WHERE Date between ''' + convert(nvarchar(max),@pinitialDate,112) + ''' and convert(smalldatetime,''' + convert(nvarchar(max),@pendDate,112) + ''',120) 
 		 ) I
          PIVOT( Sum(Value) FOR ConceptName in ('+ @conceptNames +') ) P) tmp
 		 on alldays.dt = tmp.Date and tmp.IDEmployee = employees.ID
 	 WHERE employees.ID IN(' + @employeeIDFilter + ') and dbo.WebLogin_GetPermissionOverEmployee(' + CAST(@pidpassport as nvarchar(100)) + ',employees.ID,2,0,0,alldays.dt) > 1
 	) accruals
 	INNER JOIN dbo.Employees emp with (nolock) ON emp.ID = accruals.IDEmployee 
 	INNER JOIN dbo.sysroEmployeeGroups eg with (nolock) ON eg.IDEmployee = accruals.IDEmployee AND accruals.Date between eg.BeginDate and eg.EndDate  
  	INNER JOIN dbo.EmployeeContracts ec with (nolock) ON ec.IDEmployee = accruals.IDEmployee AND accruals.Date between ec.BeginDate and ec.EndDate  
	LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID
 	ORDER BY accruals.IDEmployee,accruals.Date ASC
 	option ( MaxRecursion 0 ) 
 	';
   EXEC(@SqlStatement)
 END
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO


UPDATE sysroParameters SET Data='555' WHERE ID='DBVersion'
GO