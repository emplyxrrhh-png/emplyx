 ALTER PROCEDURE [dbo].[RemoveInactivePassportPermissions]  	(
     @Version int
 	)
 	AS  
     BEGIN  

	   		DECLARE @pVersion int = @Version; 

 	IF @pVersion > 1
 	BEGIN
	delete from sysroPermissionsOverGroups where PassportID in(
 	select IDParentPassport from sysroPassports where GroupType<> 'U' AND NOT ((StartDate IS NULL OR StartDate <= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) AND (ExpirationDate IS NULL OR ExpirationDate >= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) AND isnull(State,0) = 1)
 	)
 	delete from sysroPermissionsOverFeatures where PassportID in(
 	select IDParentPassport from sysroPassports where GroupType<> 'U' AND NOT ((StartDate IS NULL OR StartDate <= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) AND (ExpirationDate IS NULL OR ExpirationDate >= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) AND isnull(State,0) = 1)
 	)
 	delete from sysroPermissionsOverEmployeesExceptions where PassportID in(
 	select IDParentPassport from sysroPassports where GroupType<> 'U' AND NOT ((StartDate IS NULL OR StartDate <= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) AND (ExpirationDate IS NULL OR ExpirationDate >= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) AND isnull(State,0) = 1)
 	)
 	delete from sysroPermissionsOverRequests where IDParentPassport in(
 	select IDParentPassport from sysroPassports where GroupType<> 'U' AND NOT ((StartDate IS NULL OR StartDate <= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) AND (ExpirationDate IS NULL OR ExpirationDate >= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) AND isnull(State,0) = 1)
 	)
 	END

	delete from sysroPermissionsOverRequests where IDParentPassport in(
	select id from sysroPassports where GroupType = 'U' and (select count(1) from sysroPassports x where x.IDParentPassport=sysroPassports.id and x.GroupType <> 'U') = 0
 	)
 	END
GO

 ALTER PROCEDURE [dbo].[ExecuteRequestPassportPermissionsAction]
 	(
     @IDAction int,
     @IDObject int,@Version int
 	)
 	AS
 	/* Añadimos todos los pasaportes que tienen permisos para una solicitud a la tabla temporal */
    	BEGIN
  		DECLARE @pIDAction int = @IDAction,  
 			@pIDObject int = @IDObject, @pVersion int = @Version; 
    		IF @pIDAction = -2
    		BEGIN
   			exec dbo.sysro_GenerateAllPermissionsOverGroups @pVersion
   			exec dbo.sysro_GenerateAllPermissionsOverFeatures
    			exec dbo.sysro_GenerateAllPermissionsOverEmployeesExceptions
   			exec dbo.GenerateAllRequestPassportPermission
    		END
   		IF @pIDAction = -1 -- Cambio de dia
    		BEGIN
   			exec dbo.GenerateChangeDayRequestPassportPermission
    		END
    	IF @pIDAction = 0 -- Creación passport
    	BEGIN
   		exec dbo.sysro_UpdatePassportPermissionsOverGroups @pIDObject,@pVersion
   		exec dbo.sysro_UpdatePassportPermissionsOverFeatures @pIDObject
   		exec dbo.sysro_UpdatePassportPermissionsOverEmployeesExceptions @pIDObject
 		exec dbo.InsertPassportRequestsPermission @pIDObject
    	END
    	IF @pIDAction = 1 -- Modificación passport
    	BEGIN
   		exec dbo.sysro_UpdatePassportPermissionsOverGroups @pIDObject,@pVersion
   		exec dbo.sysro_UpdatePassportPermissionsOverFeatures @pIDObject
   		exec dbo.sysro_UpdatePassportPermissionsOverEmployeesExceptions @pIDObject
 		exec dbo.AlterPassportRequestsPermission @pIDObject
    	END
    	IF @pIDAction = 2 -- Creación solicitud
    	BEGIN
    		exec dbo.InsertRequestPassportPermission @pIDObject
    	END
    	IF @pIDAction = 3 -- Creación grupo de empleados
    	BEGIN
    		exec dbo.InsertGroupPassportPermission @pIDObject,@pVersion
    	END

 		exec dbo.RemoveInactivePassportPermissions @pVersion
 		
    	END	
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [Alias] = 'sysroBirthdate') AND NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [FieldName] = 'Fecha de nacimiento' )
	INSERT INTO [dbo].[sysroUserFields] ([FieldName],[FieldType],[Used],[AccessLevel] ,[Pos],[Category],[ListValues],[Type],[Description],[AccessValidation],[History],[RequestPermissions],[RequestCriteria],[isSystem],[Alias],[DocumentTemplateId]) 
	VALUES('Fecha de nacimiento',2,1,0,null,'Personal','',0,'',0,0,1,'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>',1,'sysroBirthdate',null)
ELSE
	IF NOT EXISTS (select 1 from EmployeeUserFieldValues where FieldName like '%Fecha nacimiento%')
		UPDATE [dbo].[sysroUserFields] set Alias='sysroBirthdate', Category='Personal', isSystem=1,Used = 1, FieldType = 2 where Fieldname = 'Fecha de nacimiento'
	ELSE
		UPDATE [dbo].[sysroUserFields] set Alias='sysroBirthdate', Category='Personal', isSystem=1,Used = 1 where Fieldname = 'Fecha de nacimiento'
GO

UPDATE dbo.sysrouserfields SET category = 'Laboral' WHERE category = 'Igualdad'
GO


CREATE FUNCTION [dbo].[GetEmployeeAge](
    @idEmployee int
)

returns int
as
begin

declare @iMonthDayDob int
declare @iMonthDayPassedDate int
declare @PassedDate datetime = GETDATE()
declare @empDate datetime



select @empDate = convert(smalldatetime,convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(@idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroBirthdate'),@PassedDate)),120)

select @iMonthDayDob = CAST(datepart (mm,@empDate) * 100 + datepart  (dd,@empDate) AS int) 
select @iMonthDayPassedDate = CAST(datepart (mm,@PassedDate) * 100 + datepart  (dd,@PassedDate) AS int) 

return DateDiff(yy,@empDate, @PassedDate) 
- CASE WHEN @iMonthDayDob <= @iMonthDayPassedDate
  THEN 0 
  ELSE 1
  END

End
GO

CREATE FUNCTION [dbo].[GetPunchesByEmployeeAndDate]   
     (  
      @IDEmployee int,
	  @selectedDate smalldatetime
     )  
    RETURNS nvarchar(max)  
    AS  
     BEGIN  
      DECLARE @punches nvarchar(max)
	  select @punches = convert(nvarchar(max),(select case 
				when ActualType = 1 then convert(nvarchar(max),DateTime,108) + 'E '
				when ActualType = 2 then convert(nvarchar(max),DateTime,108) + 'S '
			end AS [data()]
		from punches where ShiftDate = @selectedDate and IDEmployee = @IDEmployee and ActualType in (1,2)
		order by DateTime asc
		FOR XML PATH('')))

      
     RETURN @punches 
  END
GO

ALTER PROCEDURE [dbo].[Genius_SalaryGap]    
         @initialDate smalldatetime,@endDate smalldatetime,@idpassport int,@employeeFilter nvarchar(max),@userFieldsFilter nvarchar(max) AS    
     DECLARE @employeeIDs Table(idEmployee int)    
     DECLARE @pinitialDate smalldatetime = @initialDate  
     DECLARE @pendDate smalldatetime = @endDate  
        DECLARE @pidpassport int = @idpassport  
        DECLARE @pemployeeFilter nvarchar(max) = @employeeFilter  
        DECLARE @puserFieldsFilter nvarchar(max) = @userFieldsFilter  
        insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate    
 	 select salaryGap.*,
 		CASE WHEN salaryGap.sysroGender = 'Hombre' THEN salaryGap.sysroBaseSalary ELSE NULL END as sysroBaseSalary_man,
 		CASE WHEN salaryGap.sysroGender = 'Mujer' THEN salaryGap.sysroBaseSalary ELSE NULL END as sysroBaseSalary_woman,
		CASE WHEN salaryGap.sysroGender = 'Hombre' THEN salaryGap.sysroEarningsOverTime ELSE NULL END as sysroEarningsOverTime_man,
 		CASE WHEN salaryGap.sysroGender = 'Mujer' THEN salaryGap.sysroEarningsOverTime ELSE NULL END as sysroEarningsOverTime_woman,
		CASE WHEN salaryGap.sysroGender = 'Hombre' THEN salaryGap.sysroExtraSalary ELSE NULL END as sysroExtraSalary_man,
 		CASE WHEN salaryGap.sysroGender = 'Mujer' THEN salaryGap.sysroExtraSalary ELSE NULL END as sysroExtraSalary_woman,
		CASE WHEN salaryGap.sysroGender = 'Hombre' THEN salaryGap.sysroTotalSalary ELSE NULL END as sysroTotalSalary_man,
 		CASE WHEN salaryGap.sysroGender = 'Mujer' THEN salaryGap.sysroTotalSalary ELSE NULL END as sysroTotalSalary_woman,
		CASE WHEN salaryGap.sysroGender = 'Hombre' THEN salaryGap.sysroSalarySupp ELSE NULL END as sysroSalarySupp_man,
 		CASE WHEN salaryGap.sysroGender = 'Mujer' THEN salaryGap.sysroSalarySupp ELSE NULL END as sysroSalarySupp_woman
 	 from(
        SELECT emp.ID As IDEmployee, sreg.IDGroup, ec.IDContract,  
          sreg.GroupName, sreg.FullGroupName, emp.Name AS EmployeeName,     
          sreg.Path, sreg.CurrentEmployee, sreg.BeginDate, sreg.EndDate,    
          ec.BeginDate AS BeginContract, ec.EndDate AS EndContract, @initialDate AS BeginPeriod, @pendDate AS EndPeriod,    
          convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroBaseSalary'),@pendDate)) As sysroBaseSalary,
		  convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroEarningsOverTime'),@pendDate)) As sysroEarningsOverTime,
		  convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroExtraSalary'),@pendDate)) As sysroExtraSalary,
		  convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroTotalSalary'),@pendDate)) As sysroTotalSalary,
		  convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroSalarySupp'),@pendDate)) As sysroSalarySupp,
          convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroProfessionalCategory'),@pendDate)) As sysroProfessionalCategory,    
          convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPosition'),@pendDate)) As sysroPosition,    
          convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroGender'),@pendDate)) As sysroGender,
		  dbo.GetEmployeeAge(sreg.idEmployee) As Age,  
          dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,2,'\') As Nivel2,    
          dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,4,'\') As Nivel4,    
          dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,6,'\') As Nivel6,    
          dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,8,'\') As Nivel8,    
          dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,10,'\') As Nivel10,    
          dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),@pendDate) As UserField1,    
          dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),@pendDate) As UserField2,    
          dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),@pendDate) As UserField3,    
          dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),@pendDate) As UserField4,    
          dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),@pendDate) As UserField5,    
          dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),@pendDate) As UserField6,    
          dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),@pendDate) As UserField7,    
          dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),@pendDate) As UserField8,    
          dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),@pendDate) As UserField9,    
          dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@pendDate) As UserField10    
        FROM   
   dbo.sysroEmployeeGroups as sreg with (nolock)  
    INNER JOIN dbo.Employees emp  with (nolock) on sreg.IDEmployee = emp.ID  
          INNER JOIN dbo.EmployeeContracts ec with (nolock) ON ec.IDEmployee = emp.ID AND @pendDate between ec.BeginDate and ec.EndDate    
        WHERE  dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,1,0,0,@pendDate) > 1  
       AND sreg.IDEmployee in( select idEmployee from @employeeIDs) and CurrentEmployee = 1 and @pendDate between sreg.BeginDate and sreg.EndDate) salaryGap
GO

CREATE PROCEDURE [dbo].[GetPivotAccruals]
  @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max) 
AS
BEGIN
 
 DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter  

 DECLARE @employeeIDs Table(idEmployee int)
 DECLARE @conceptNames NVARCHAR(MAX)
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



  DECLARE @SqlStatement NVARCHAR(MAX)
  SET @SqlStatement = N'
    SELECT 
		accruals.IDEmployee As IDEmployee, eg.IDGroup, ec.IDContract, eg.GroupName, eg.FullGroupName, emp.Name AS EmployeeName, accruals.Date AS Date_ToDateString,
		MONTH(accruals.Date) AS Mes, YEAR(accruals.Date) AS Año, (DATEPART(dw, accruals.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, accruals.Date) AS WeekOfYear, 
		DATEPART(dy, accruals.Date) AS DayOfYear, DATEPART(QUARTER, accruals.Date) AS Quarter, eg.Path, eg.CurrentEmployee, eg.BeginDate, eg.EndDate, ec.BeginDate AS BeginContract, ec.EndDate AS EndContract,
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
		' + @conceptNames + ', dbo.GetPunchesByEmployeeAndDate(emp.ID,accruals.Date) as punches
	from (SELECT * FROM
         (SELECT Concepts.name AS ConceptName, Value, IDEmployee, Date FROM DailyAccruals WITH(NOLOCK) inner join Concepts WITH(NOLOCK) on dailyaccruals.IDConcept = Concepts.ID
		  WHERE Date between ''' + convert(nvarchar(max),@pinitialDate,112) + ''' and ''' + convert(nvarchar(max),@pendDate,112) + ''' 
		  and IDEmployee IN(' + @employeeIDFilter + ') and dbo.WebLogin_GetPermissionOverEmployee(' + CAST(@pidpassport as nvarchar(100)) + ',IDEmployee,2,0,0,Date) > 1
		 ) I
         PIVOT( Sum(Value) FOR ConceptName in ('+ @conceptNames +') ) P) accruals
	INNER JOIN dbo.Employees emp with (nolock) ON emp.ID = accruals.IDEmployee 
	INNER JOIN dbo.sysroEmployeeGroups eg with (nolock) ON eg.IDEmployee = accruals.IDEmployee AND accruals.Date between eg.BeginDate and eg.EndDate  
 	INNER JOIN dbo.EmployeeContracts ec with (nolock) ON ec.IDEmployee = accruals.IDEmployee AND accruals.Date between ec.BeginDate and ec.EndDate  

   ORDER BY accruals.IDEmployee,accruals.Date ASC';
 
  EXEC(@SqlStatement)
 
END
GO

CREATE PROCEDURE [dbo].[GetPivotAccrualsWithZeros]
   @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max) 
AS
BEGIN
 
 DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter  

 DECLARE @employeeIDs Table(idEmployee int)
 DECLARE @conceptNames NVARCHAR(MAX)
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



  DECLARE @SqlStatement NVARCHAR(MAX)
  SET @SqlStatement = N'
	WITH alldays AS (  
           SELECT convert(smalldatetime,''' + convert(nvarchar(max),@pinitialDate,112) + ''',120) AS dt  
           UNION ALL  
           SELECT DATEADD(dd, 1, dt)  
          FROM alldays s  
            WHERE DATEADD(dd, 1, dt) <=  convert(smalldatetime,''' + convert(nvarchar(max),@pendDate,112) + ''',120))  

	SELECT 
		accruals.IDEmployee As IDEmployee, eg.IDGroup, ec.IDContract, eg.GroupName, eg.FullGroupName, emp.Name AS EmployeeName, accruals.Date AS Date_ToDateString,
		MONTH(accruals.Date) AS Mes, YEAR(accruals.Date) AS Año, (DATEPART(dw, accruals.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, accruals.Date) AS WeekOfYear, 
		DATEPART(dy, accruals.Date) AS DayOfYear, DATEPART(QUARTER, accruals.Date) AS Quarter, eg.Path, eg.CurrentEmployee, eg.BeginDate, eg.EndDate, ec.BeginDate AS BeginContract, ec.EndDate AS EndContract,
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
		' + @conceptNames + ', dbo.GetPunchesByEmployeeAndDate(emp.ID,accruals.Date) as punches
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

	ORDER BY accruals.IDEmployee,accruals.Date ASC
	option ( MaxRecursion 0 ) 
	';
 
  EXEC(@SqlStatement)
 
END
GO

ALTER PROCEDURE [dbo].[Genius_Access]
         @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max) AS  
 		DECLARE @employeeIDs Table(idEmployee int)  
 		DECLARE @zonesIDs Table(idZone int)  
 		DECLARE @idParentPassport int  
 		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter 
 		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter, @pinitialDate, @pendDate;  
 		select @idParentPassport = IDParentPassport from sysroPassports where id = @pidpassport  
 		if ((select value from sysroLiveAdvancedParameters where ParameterName = 'AccessGroupsMode') = '1')    
 			insert into @zonesIDs select ID from Zones  
 				where ID in (SELECT IDZone FROM AccessGroupsPermissions with (nolock) where IDAccessGroup in (select IDAccessGroup from sysroPassports_AccessGroup with (nolock) where idpassport = @idParentPassport)) or IDParent is null  
 		else    
 			insert into @zonesIDs select ID from Zones with (nolock)  
 		SELECT     dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee,   
                     dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, CONVERT(nvarchar(8),   
                     dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day, MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Year,   
                     (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName,   
                     dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName, dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL   
                     THEN 0 ELSE 1 END AS IsInvalid, CONVERT(nvarchar(MAX), dbo.Punches.TypeDetails) AS Details, dbo.sysroEmployeeGroups.Path,   
                     dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,  
 					dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,  
 					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
 					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
 					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
 					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
 					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.Punches.ShiftDate) As UserField1,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.Punches.ShiftDate) As UserField2,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.Punches.ShiftDate) As UserField3,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.Punches.ShiftDate) As UserField4,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.Punches.ShiftDate) As UserField5,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.Punches.ShiftDate) As UserField6,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.Punches.ShiftDate) As UserField7,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.Punches.ShiftDate) As UserField8,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.Punches.ShiftDate) As UserField9,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.Punches.ShiftDate) As UserField10,
					dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age  
          FROM         dbo.sysroEmployeeGroups with (nolock)   
              INNER JOIN dbo.Punches with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee   
              INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate   
              LEFT OUTER JOIN dbo.Zones with (nolock) ON dbo.Punches.IDZone = dbo.Zones.ID   
              LEFT OUTER JOIN dbo.Terminals with (nolock) ON dbo.Punches.IDTerminal = dbo.Terminals.ID   
              LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID  
          WHERE     dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeeGroups.IDEmployee,9,0,0,dbo.Punches.ShiftDate) > 1  
 				AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @pinitialDate and @pendDate  
 				AND (dbo.Punches.ShiftDate between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate)  
 				AND dbo.Zones.ID IN (SELECT idZone FROM @zonesIDs)  
          GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID,   
                     dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108),   
                     dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name,   
                     (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name,   
                     CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(nvarchar(MAX), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path,   
                     dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,   
                     dbo.sysroEmployeeGroups.IDGroup,dbo.EmployeeContracts.BeginDate , dbo.EmployeeContracts.EndDate  
          HAVING (dbo.Punches.Type = 5) OR (dbo.Punches.Type = 6) OR (dbo.Punches.Type = 7)
GO

ALTER PROCEDURE [dbo].[Genius_Authorizations]
        @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max) AS  
 		DECLARE @employeeIDs Table(idEmployee int)  
 		DECLARE @autorizationsIDs Table(idAccess int)  
 		DECLARE @zonesIDs Table(idZone int)  
 		DECLARE @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter
 		DECLARE @cDate as Date, @idParentPassport int  
 		
 		select @idParentPassport = IDParentPassport from sysroPassports where id = @pidpassport  
 		if ((select value from sysroLiveAdvancedParameters where ParameterName = 'AccessGroupsMode') = '1')  
 		begin  
 			insert into @autorizationsIDs select IDAccessGroup from sysroPassports_AccessGroup with (nolock) where IDPassport = @idParentPassport  
 			insert into @zonesIDs select ID from Zones  
 				where ID in (SELECT IDZone FROM AccessGroupsPermissions with (nolock) where IDAccessGroup in (select IDAccessGroup from sysroPassports_AccessGroup with (nolock) where idpassport = @idParentPassport)) or IDParent is null  
 		end  
 		else  
 		BEGIN  
 			insert into @autorizationsIDs select id from AccessGroups with (nolock)  
 			insert into @zonesIDs select ID from Zones with (nolock)  
 		END  
 		SET @cDate = GETDATE()  
 		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter, @cDate, @cDate;  
 		SELECT dbo.sysroEmployeeGroups.IDEmployee AS IDEmployee, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.IDGroup AS IDGroup, dbo.sysroEmployeeGroups.FullGroupName As FullGroupName, dbo.sysroEmployeeGroups.Path AS GroupPath,  
 				dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee, dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract AS IDContract, dbo.AccessGroups.Name AS AuthorizationName,   
 				Zones.Name AS ZoneName, zones.IsWorkingZone AS IsWorkingZone, AccessPeriods.Name As AccessPeriodName, 1 AS BelongsToGroup,  
 		dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract,  
 		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
 		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
 		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
 		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
 		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
 		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),getdate()) As UserField1,  
 		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),getdate()) As UserField2,  
 		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),getdate()) As UserField3,  
 		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),getdate()) As UserField4,  
 		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),getdate()) As UserField5,  
 		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),getdate()) As UserField6,  
 		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),getdate()) As UserField7,  
 		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),getdate()) As UserField8,  
 		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),getdate()) As UserField9,  
 		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),getdate()) As UserField10,
		dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age   
 			FROM dbo.sysroEmployeeGroups with (nolock)   
 			INNER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID  
 			INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.Employees.ID = dbo.EmployeeContracts.IDEmployee and getdate() between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate  
 			INNER JOIN dbo.AccessGroups with (nolock) ON dbo.AccessGroups.ID IN  
 							(SELECT        dbo.Employees.IDAccessGroup  
 							UNION  
 							SELECT        IDAuthorization  
 							FROM            dbo.sysrovwAccessAuthorizations with (nolock)  
 							WHERE        (IDEmployee = dbo.Employees.ID))  
 			INNER JOIN dbo.AccessGroupsPermissions with (nolock) ON AccessGroupsPermissions.IDAccessGroup = AccessGroups.ID  
 			INNER JOIN dbo.AccessPeriods with (nolock) ON AccessPeriods.ID = AccessGroupsPermissions.IDAccessPeriod  
 			INNER JOIN dbo.Zones with (nolock) on Zones.ID = AccessGroupsPermissions.IDZone  
 		WHERE dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeeGroups.IDEmployee,9,0,0,getdate()) > 1  
 			AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) AND getdate() between sysroEmployeeGroups.BeginDate and sysroEmployeeGroups.EndDate  
 			AND dbo.AccessGroups.ID IN (SELECT idAccess FROM @autorizationsIDs)  
 			AND dbo.Zones.ID in (select idZone from @zonesIDs)
GO

ALTER PROCEDURE [dbo].[Genius_Concepts]
         @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint AS  
 		DECLARE @employeeIDs Table(idEmployee int)  
 		DECLARE @conceptIDs Table(idConcept int)  
 		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros  
 		DECLARE @intdatefirst int  
 		SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'  
 		SET DateFirst @intdatefirst;  
 		
 		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate  
 		insert into @conceptIDs select * from dbo.SplitToInt(@pconceptsFilter,',');  
 		
 		IF @pincludeZeros = 1   
 		BEGIN  
 			WITH alldays AS (  
 				SELECT @initialDate AS dt  
 				UNION ALL  
 				SELECT DATEADD(dd, 1, dt)  
 				FROM alldays s  
 				WHERE DATEADD(dd, 1, dt) <= @pendDate)  
 			SELECT   
 				reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept,   
 				dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName,   
 				reqReg.conceptName AS ConceptName, reqReg.dt AS Date, sum(isnull(dbo.DailyAccruals.Value,0)) AS Value,COUNT(*) AS Count, MONTH(reqReg.dt) AS Mes,   
 				YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week,   
 				reqReg.dt) AS WeekOfYear, DATEPART(dy, reqReg.dt) AS DayOfYear, DATEPART(QUARTER, reqReg.dt) AS Quarter, dbo.sysroEmployeeGroups.Path,   
 				dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
 				dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @pendDate AS EndPeriod,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),reqReg.dt) As UserField1,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),reqReg.dt) As UserField2,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),reqReg.dt) As UserField3,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),reqReg.dt) As UserField4,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),reqReg.dt) As UserField5,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),reqReg.dt) As UserField6,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),reqReg.dt) As UserField7,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),reqReg.dt) As UserField8,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),reqReg.dt) As UserField9,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),reqReg.dt) As UserField10,
				dbo.GetEmployeeAge(reqReg.idEmployee) As Age     
 			FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,   
 				(select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp,   
 				(select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con  
 				where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1  
 				) reqReg  
 				INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate  
 				INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate  
 				LEFT OUTER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept  
 			GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName,   
 				dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt),   
 				YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy,   
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
 				WHERE DATEADD(dd, 1, dt) <= @pendDate)  
 			SELECT
 				reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept,   
 				dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName,   
 				reqReg.conceptName AS ConceptName, reqReg.dt AS Date, sum(isnull(dbo.DailyAccruals.Value,0)) AS Value, COUNT(*) AS Count, MONTH(reqReg.dt) AS Mes,   
 				YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week,   
 				reqReg.dt) AS WeekOfYear, DATEPART(dy, reqReg.dt) AS DayOfYear, DATEPART(QUARTER, reqReg.dt) AS Quarter, dbo.sysroEmployeeGroups.Path,   
 				dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
 				dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @pendDate AS EndPeriod,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),reqReg.dt) As UserField1,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),reqReg.dt) As UserField2,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),reqReg.dt) As UserField3,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),reqReg.dt) As UserField4,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),reqReg.dt) As UserField5,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),reqReg.dt) As UserField6,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),reqReg.dt) As UserField7,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),reqReg.dt) As UserField8,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),reqReg.dt) As UserField9,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),reqReg.dt) As UserField10,
				dbo.GetEmployeeAge(reqReg.idEmployee) As Age 
 		   FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,   
 				(select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp,   
 				(select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con  
 				where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1  
 				) reqReg  
 				INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate  
 				INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate  
 				INNER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept  
 		   GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName,   
 				dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt),   
 				YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy,   
 				reqReg.dt), CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar)   
 				+ '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120),   
 				10), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
 				dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  
 		   option (maxrecursion 0)  
        END  
GO

ALTER PROCEDURE [dbo].[Genius_CostCenters_ActualStatus]
         @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @costCenterFilter nvarchar(max) AS  
 	 DECLARE @employeeIDs Table(idEmployee int)  
      DECLARE @pinitialDate smalldatetime = getdate(), @pendDate smalldatetime = getdate(), @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pbusinessCenterFilter nvarchar(max) = @costCenterFilter
 	 insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
      DECLARE @businessCenterIDs Table(idBusinessCenter int)  
      insert into @businessCenterIDs select * from dbo.SplitToInt(@pbusinessCenterFilter,',');  
 	SELECT     CONVERT(varchar(10), dbo.sysrovwEmployeeStatus.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract  AS KeyView, dbo.sysroEmployeeGroups.IDGroup,   
                 dbo.EmployeeContracts.IDContract, dbo.sysrovwEmployeeStatus.IDEmployee, dbo.sysrovwEmployeeStatus.CostDatetime, isnull(dbo.BusinessCenters.ID, 0) as IDCenter,   
 				isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,    
 				isnull(dbo.BusinessCenters.Field4, '') AS Field4, isnull(dbo.BusinessCenters.Field5, '') AS Field5,
                 dbo.Employees.Name AS EmployeeName, dbo.Groups.Name AS GroupName, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName, dbo.Groups.Path,   
                 dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,   
                 dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),getdate()) As UserField1,  
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),getdate()) As UserField2,  
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),getdate()) As UserField3,  
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),getdate()) As UserField4,  
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),getdate()) As UserField5,  
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),getdate()) As UserField6,  
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),getdate()) As UserField7,  
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),getdate()) As UserField8,  
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),getdate()) As UserField9,  
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),getdate()) As UserField10,
				dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age  
          FROM   dbo.sysroEmployeeGroups with (nolock)   
                     INNER JOIN dbo.sysrovwEmployeeStatus with (nolock) ON  dbo.sysroEmployeeGroups.IDEmployee = dbo.sysrovwEmployeeStatus.IDEmployee AND GETDATE() between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate   
 					INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.sysrovwEmployeeStatus.IDEmployee = dbo.EmployeeContracts.IDEmployee AND GETDATE() >= dbo.EmployeeContracts.BeginDate AND GETDATE() <= dbo.EmployeeContracts.EndDate   
 					INNER JOIN dbo.Groups with (nolock) ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID   
 					INNER JOIN dbo.Employees with (nolock) ON dbo.sysrovwEmployeeStatus.IDEmployee = dbo.Employees.ID    
 					LEFT OUTER JOIN dbo.BusinessCenters with (nolock) ON ISNULL(dbo.sysrovwEmployeeStatus.CostIDCenter,0) = dbo.BusinessCenters.ID   
         where   dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysrovwEmployeeStatus.IDEmployee,2,0,0,getdate()) > 1 
 				AND isnull(dbo.sysrovwEmployeeStatus.CostIDCenter,0) in (select idBusinessCenter from @businessCenterIDs)   
 				AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs)
GO

ALTER PROCEDURE [dbo].[Genius_CostCenters_Detail]
         @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @costCenterFilter nvarchar(max)   AS  
 		DECLARE @employeeIDs Table(idEmployee int)  
 		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pbusinessCenterFilter nvarchar(max) = @costCenterFilter
 		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
 		DECLARE @businessCenterIDs Table(idBusinessCenter int)  
 		insert into @businessCenterIDs select * from dbo.SplitToInt(@pbusinessCenterFilter,',');  
 		SELECT     CONVERT(varchar(10), dbo.DailyCauses.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailyCauses.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup,   
                     dbo.EmployeeContracts.IDContract, dbo.DailyCauses.IDEmployee, dbo.DailyCauses.Date,  
                     dbo.Causes.Name AS CauseName, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor, isnull(dbo.BusinessCenters.ID, 0) as IDCenter,   
 					isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,    
 					isnull(dbo.BusinessCenters.Field4, '') AS Field4, isnull(dbo.BusinessCenters.Field5, '') AS Field5, dbo.DailyCauses.Value AS Value, YEAR(dbo.DailyCauses.Date) AS Año,   
                     (DATEPART(dw, dbo.DailyCauses.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, dbo.DailyCauses.Date) AS WeekOfYear, DATEPART(dy, dbo.DailyCauses.Date) AS DayOfYear, dbo.DailyCauses.DefaultCenter, dbo.DailyCauses.ManualCenter,   
                     dbo.Employees.Name AS EmployeeName, dbo.Groups.Name AS GroupName, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName, dbo.Groups.Path,   
                     dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee, 0 AS CostCenterTotalCost,   
 					dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroCostHour'),DailyCauses.date ) as Cost,   
 					dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPriceHour'), DailyCauses.date) as PVP,  
 					dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,  
 					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
 					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
 					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
 					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
 					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
 					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.DailyCauses.Date) As UserField1,  
 					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.DailyCauses.Date) As UserField2,  
 					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.DailyCauses.Date) As UserField3,  
 					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.DailyCauses.Date) As UserField4,  
 					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.DailyCauses.Date) As UserField5,  
 					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.DailyCauses.Date) As UserField6,  
 					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.DailyCauses.Date) As UserField7,  
 					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.DailyCauses.Date) As UserField8,  
 					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.DailyCauses.Date) As UserField9,  
 					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.DailyCauses.Date) As UserField10,
					 dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age  
          FROM		 dbo.sysroEmployeeGroups with (nolock)   
                         INNER JOIN dbo.Causes with (nolock)   
                         INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause  ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.DailyCauses.Date between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate   
 						INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyCauses.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyCauses.Date <= dbo.EmployeeContracts.EndDate   
 						INNER JOIN dbo.Groups with (nolock) ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID   
 						LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.Employees.ID    
 						LEFT OUTER JOIN dbo.BusinessCenters with (nolock) ON ISNULL(dbo.DailyCauses.IDCenter,0) = dbo.BusinessCenters.ID   
           where dbo.dailycauses.date between @pinitialDate and @pendDate 
 					AND isnull(dbo.DailyCauses.IDCenter,0) in (select idBusinessCenter from @businessCenterIDs)   
 					AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs)
GO

ALTER PROCEDURE [dbo].[Genius_Incidences]
          @initialDate smalldatetime,    
          @endDate smalldatetime,    
          @idpassport int,    
          @employeeFilter nvarchar(max),    
          @userFieldsFilter nvarchar(max)    
          AS    
         DECLARE @employeeIDs Table(idEmployee int)        
       DECLARE @pinitialDate smalldatetime = @initialDate,        
          @pendDate smalldatetime = @endDate,        
          @pidpassport int = @idpassport,        
          @pemployeeFilter nvarchar(max) = @employeeFilter,        
          @puserFieldsFilter nvarchar(max) = @userFieldsFilter      
    DECLARE @intdatefirst int  
    SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'  
    SET DateFirst @intdatefirst;  
    insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;       
          SELECT     CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)     
                                + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup,     
                                dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description AS IncidenceName,     
                                dbo.TimeZones.Name AS ZoneTime, dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS Value, MONTH(dbo.DailySchedule.Date) AS Mes, YEAR(dbo.DailySchedule.Date) AS Año,     
                                (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, dbo.DailySchedule.Date) AS WeekOfYear, DATEPART(dy, dbo.DailySchedule.Date) AS DayOfYear, 
                                DATEPART(QUARTER, dbo.DailySchedule.Date) as Quarter, CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName,     
                                dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.FullGroupName AS FullGroupName, dbo.sysroEmployeeGroups.Path,     
                                dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,    
               dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod, dbo.DailySchedule.IDAssignment,    
            case when dbo.DailySchedule.IDAssignment > 0 then (select Name FROM dbo.Assignments WHERE ID=dbo.DailySchedule.IDAssignment ) else '' END As AssignmentName,    
                               (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition)) as IDProductiveUnit,    
                               (SELECT Name from ProductiveUnits where id in (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition))) as ProductiveUnitName,    
               dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,    
               dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,    
               dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,    
               dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,    
               dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,    
               dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.DailySchedule.Date) As UserField1,    
               dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.DailySchedule.Date) As UserField2,    
               dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.DailySchedule.Date) As UserField3,    
               dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.DailySchedule.Date) As UserField4,    
               dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.DailySchedule.Date) As UserField5,    
               dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.DailySchedule.Date) As UserField6,    
               dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.DailySchedule.Date) As UserField7,    
               dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.DailySchedule.Date) As UserField8,    
               dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.DailySchedule.Date) As UserField9,    
               dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.DailySchedule.Date) As UserField10,
			   dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age, convert(nvarchar(max),dbo.sysroRemarks.Text) as Remark
          FROM         dbo.sysroEmployeeGroups with (nolock)    
              INNER JOIN dbo.Causes with (nolock)    
              INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause     
              INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailySchedule.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate    
              INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailySchedule.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailySchedule.Date between dbo.EmployeeContracts.BeginDate AND dbo.EmployeeContracts.EndDate     
              LEFT OUTER JOIN dbo.sysroDailyIncidencesDescription with (nolock)    
              INNER JOIN dbo.DailyIncidences with (nolock)    
              INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID ON dbo.sysroDailyIncidencesDescription.IDIncidence = dbo.DailyIncidences.IDType ON dbo.DailyCauses.IDEmployee = dbo.DailyIncidences.IDEmployee AND dbo.DailyCauses.Date = dbo.DailyIncidences.Date AND dbo.DailyCauses.IDRelatedIncidence = dbo.DailyIncidences.ID AND dbo.DailyIncidences.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate    
              LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.DailySchedule.IDShiftUsed = dbo.Shifts.ID     
              LEFT OUTER JOIN dbo.Shifts AS Shifts_1 with (nolock) ON dbo.DailySchedule.IDShift1 = Shifts_1.ID    
              LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID    
           LEFT OUTER JOIN dbo.sysroRemarks on dbo.sysroRemarks.ID = dbo.DailySchedule.Remarks    
         WHERE dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.DailyCauses.IDEmployee,2,0,0,dbo.DailyCauses.date) > 1    
            AND dbo.sysroEmployeeGroups.IDEmployee in(select idEmployee from @employeeIDs)     
           and dbo.dailyCauses.Date between @pinitialDate and @pendDate    
          GROUP BY dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name,     
                                YEAR(dbo.DailySchedule.Date), CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)     
                    + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120), CASE WHEN DailySchedule.Date <= GETDATE()     
                                THEN Shifts.Name ELSE Shifts_1.Name END, dbo.Employees.Name, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, DATEPART(iso_week,     
                                dbo.DailySchedule.Date), DATEPART(dy, dbo.DailySchedule.Date), (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1,     
                                dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,     
                                dbo.sysroEmployeeGroups.CurrentEmployee, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate, dbo.DailySchedule.IDAssignment, dbo.DailySchedule.IDDailyBudgetPosition, convert(nvarchar(max),dbo.sysroRemarks.Text)
GO

ALTER PROCEDURE [dbo].[Genius_Punches]
         @initialDate smalldatetime,  
         @endDate smalldatetime,  
         @idpassport int,  
         @employeeFilter nvarchar(max),  
         @userFieldsFilter nvarchar(max)  
         AS  
         DECLARE @employeeIDs Table(idEmployee int) 
    	 DECLARE @pinitialDate smalldatetime = @initialDate,  
         @pendDate smalldatetime = @endDate,  
         @pidpassport int = @idpassport,  
         @pemployeeFilter nvarchar(max) = @employeeFilter,  
         @puserFieldsFilter nvarchar(max) = @userFieldsFilter  
         insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
         SELECT     dbo.Punches.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee,   
   					dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Punches.ActualType AS PunchDirection,   
   					dbo.Punches.TypeData AS PunchIDTypeData, CASE WHEN dbo.Punches.TypeData <> 0 THEN dbo.Causes.Name ELSE '' END AS PunchTypeData,   
   					dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day,   
   					MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Año, (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1)   
   					% 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName,   
   					dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END AS IsInvalid, CONVERT(nvarchar(MAX), dbo.Punches.TypeDetails)   
   					AS Details, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate,   
   					dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, dbo.Punches.IsNotReliable, dbo.Punches.Location AS PunchLocation,   
   					dbo.Punches.LocationZone AS PunchLocationZone, dbo.Punches.FullAddress , dbo.Punches.TimeZone AS PunchTimeZone, dbo.sysroPassports.Name AS EditorName,  
   					dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,  
   					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
   					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
   					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
   					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
   					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
   					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.Punches.ShiftDate) As UserField1,  
   					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.Punches.ShiftDate) As UserField2,  
   					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.Punches.ShiftDate) As UserField3,  
   					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.Punches.ShiftDate) As UserField4,  
   					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.Punches.ShiftDate) As UserField5,  
   					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.Punches.ShiftDate) As UserField6,  
   					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.Punches.ShiftDate) As UserField7,  
   					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.Punches.ShiftDate) As UserField8,  
   					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.Punches.ShiftDate) As UserField9,  
   					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.Punches.ShiftDate) As UserField10,
					dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age
         FROM         dbo.sysroEmployeeGroups with (nolock) INNER JOIN  
   					dbo.Punches with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee AND dbo.Punches.ShiftDate >= dbo.sysroEmployeeGroups.BeginDate AND   
   					dbo.Punches.ShiftDate <= dbo.sysroEmployeeGroups.EndDate INNER JOIN  
   					dbo.EmployeeContracts with (nolock) ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND   
   					dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN  
   					dbo.Zones with (nolock) ON dbo.Punches.IDZone = dbo.Zones.ID LEFT OUTER JOIN  
   					dbo.Terminals with (nolock) ON dbo.Punches.IDTerminal = dbo.Terminals.ID LEFT OUTER JOIN  
   					dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID LEFT OUTER JOIN  
   					dbo.sysroPassports with (nolock) ON dbo.Punches.IDPassport = dbo.sysroPassports.ID LEFT OUTER JOIN  
   					dbo.Causes with (nolock) ON dbo.Punches.TypeData = dbo.Causes.ID  
         WHERE dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeeGroups.IDEmployee,2,0,0,dbo.Punches.ShiftDate) > 1  
   					AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @pinitialDate and @pendDate  
         GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID,   
   					dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108),   
   					dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name,   
   					(DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name,   
   					CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(nvarchar(MAX), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path,   
   					dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,   
   					dbo.Punches.IsNotReliable, dbo.Punches.Location, dbo.Punches.LocationZone, dbo.Punches.FullAddress, dbo.Punches.TimeZone, dbo.sysroPassports.Name, dbo.Punches.ActualType,   
   					dbo.Causes.Name, dbo.Punches.TypeData, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  
         HAVING (dbo.Punches.Type = 1) OR  
   					(dbo.Punches.Type = 2) OR  
   					(dbo.Punches.Type = 3) OR  
   					(dbo.Punches.Type = 7 AND (dbo.Punches.ActualType = 1 OR dbo.Punches.ActualType = 2))
GO

ALTER PROCEDURE [dbo].[Genius_Requests_CostCenters]
       	@initialDate smalldatetime,@endDate smalldatetime,@idpassport int,@employeeFilter nvarchar(max),@userFieldsFilter nvarchar(max) AS
 		DECLARE @employeeIDs Table(idEmployee int)
  		DECLARE @SecurityMode int
  		SELECT @SecurityMode = convert(numeric(3),isnull(value,'1')) from sysroLiveAdvancedParameters where ParameterName = 'SecurityMode'
 		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter,@initialDate,@endDate;
 		
 		SELECT     dbo.requests.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee, 
                              dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Requests.RequestDate as 'Date', 
                              dbo.Requests.Status AS RequestsStatus, 
   						   	Case 
   							WHEN Requests.Status IN(2,3) THEN (SELECT top 1 isnull(sysropassports.Name,'') from RequestsApprovals inner join sysroPassports on RequestsApprovals.IDPassport = sysroPassports.ID  where IDRequest = Requests.ID AND Status in(2,3) )
   							ELSE '---'
   							END as 'ApproveRefusePassport',
   							Case 
   								WHEN Requests.Status IN(0,1) Then (select dbo.GetRequestNextLevelPassports(Requests.ID,@SecurityMode))
   								ELSE '---'
   							END as 'PendingPassport',
   							dbo.Requests.RequestType,
   							isnull(CONVERT(NVARCHAR(4000), dbo.Requests.Date1, 120),'') as 'PunchDate',
   							ISNULL((SELECT Name from dbo.BusinessCenters WHERE ID = dbo.Requests.IDCenter),'') as 'NameCenter',
   							CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')) as CommentsRequest,
                              CONVERT(nvarchar(8), dbo.Requests.RequestDate, 108) AS Time, DAY(dbo.Requests.RequestDate) AS Day, 
                              MONTH(dbo.Requests.RequestDate) AS Month, YEAR(dbo.Requests.RequestDate) AS Año, (DATEPART(dw, dbo.Requests.RequestDate) + @@DATEFIRST - 1 - 1) 
                              % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, 
                              dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, 
       					   dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,
       						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
       						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
       						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
       						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
       						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
       						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),dbo.Requests.RequestDate) As UserField1,
       						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),dbo.Requests.RequestDate) As UserField2,
       						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),dbo.Requests.RequestDate) As UserField3,
       						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),dbo.Requests.RequestDate) As UserField4,
       						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),dbo.Requests.RequestDate) As UserField5,
       						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),dbo.Requests.RequestDate) As UserField6,
       						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),dbo.Requests.RequestDate) As UserField7,
       						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),dbo.Requests.RequestDate) As UserField8,
       						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),dbo.Requests.RequestDate) As UserField9,
       						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),dbo.Requests.RequestDate) As UserField10,
							dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age
   									
        FROM         dbo.sysroEmployeeGroups with (nolock) INNER JOIN
                              dbo.requests with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.requests.IDEmployee AND dbo.requests.RequestDate >= dbo.sysroEmployeeGroups.BeginDate AND 
                              dbo.requests.RequestDate <= dbo.sysroEmployeeGroups.EndDate INNER JOIN
                              dbo.EmployeeContracts with (nolock) ON dbo.requests.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.requests.RequestDate >= dbo.EmployeeContracts.BeginDate AND 
                              dbo.requests.RequestDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                              dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID 
        WHERE	dbo.GetRequestPassportPermission(@idpassport, Requests.ID) > 1
       			AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.requests.RequestDate between @initialDate and @endDate
   				
        GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.requests.RequestDate), YEAR(dbo.requests.RequestDate), dbo.requests.ID, dbo.requests.IDCenter ,
                              dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.requests.RequestDate), dbo.requests.RequestDate, CONVERT(nvarchar(8), dbo.requests.RequestDate, 108), 
                              dbo.requests.RequestDate,  dbo.EmployeeContracts.IDContract, dbo.Employees.Name, 
                              (DATEPART(dw, dbo.requests.RequestDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.requests.RequestType, dbo.requests.Status, dbo.requests.Date1, CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')), dbo.sysroEmployeeGroups.Path, 
                              dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, 
   							dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate
        HAVING      (dbo.Requests.RequestType IN(12))
GO

ALTER PROCEDURE [dbo].[Genius_Requests_Schedule]
         @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @requestTypesFilter nvarchar(max) AS        
 		
 		DECLARE @employeeIDs Table(idEmployee int)        
 		DECLARE @requestTypeFilter Table(idRequest int)        
  		DECLARE @SecurityMode int
  		
 		SELECT @SecurityMode = convert(numeric(3),isnull(value,'1')) from sysroLiveAdvancedParameters where ParameterName = 'SecurityMode'
         insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter,@initialDate,@endDate;        
 		insert into @requestTypeFilter select * from dbo.SplitToInt(@requestTypesFilter,',');         
 		SELECT dbo.requests.ID AS KeyView, isnull(case when aux.NumeroDias > 0 then aux.NumeroDias else DATEDIFF(day,dbo.Requests.Date1, dbo.Requests.Date2) + 1 end,0) as NumeroDias, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee,         
 				dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Requests.RequestDate as 'Date',         
 				dbo.Requests.Status AS RequestsStatus,         
 				Case WHEN Requests.Status IN(2,3) THEN (SELECT top 1 isnull(sysropassports.Name,'') from RequestsApprovals inner join sysroPassports on RequestsApprovals.IDPassport = sysroPassports.ID  where IDRequest = Requests.ID AND Status in(2,3) ) ELSE '---' END as 'ApproveRefusePassport',        
 				Case WHEN Requests.Status IN(0,1) Then (select dbo.GetRequestNextLevelPassports(Requests.ID,@SecurityMode)) ELSE '---' END as 'PendingPassport',        
 				dbo.Requests.RequestType, isnull(CONVERT(NVARCHAR(4000), dbo.Requests.Date1, 120),'') as 'BeginDateRequest', isnull(CONVERT(NVARCHAR(4000), dbo.Requests.FromTime, 8),'') as 'BeginHourRequest', 
 				isnull(CONVERT(NVARCHAR(4000), isnull(dbo.Requests.Date2,dbo.Requests.Date1), 120),'') as 'EndDateRequest',        
 				isnull(CONVERT(NVARCHAR(4000), dbo.Requests.ToTime, 8),'') as 'EndHourRequest',        
 				ISNULL((SELECT Name from dbo.Causes WHERE ID = dbo.Requests.IDCause),'') as 'CauseNameRequest',        
 				ISNULL((SELECT Name from dbo.Shifts WHERE ID = dbo.Requests.IDShift),'') as 'ShiftNameRequest',        
 				CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')) as CommentsRequest,        
 				CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldName, '')) as FieldNameRequest, 
 				CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldValue, '')) as FieldValueRequest, 
                 CONVERT(nvarchar(8), dbo.Requests.RequestDate, 108) AS Time, DAY(dbo.Requests.RequestDate) AS Day,         
                 MONTH(dbo.Requests.RequestDate) AS Month, YEAR(dbo.Requests.RequestDate) AS Año, (DATEPART(dw, dbo.Requests.RequestDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 
 				1 AS Value, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate,         
                 dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,         
 				dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,        
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,      
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,        
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,      
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,        
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,      
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,        
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,      
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,        
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,      
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,        
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),dbo.Requests.RequestDate) As UserField1,        
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),dbo.Requests.RequestDate) As UserField2,        
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),dbo.Requests.RequestDate) As UserField3,        
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),dbo.Requests.RequestDate) As UserField4,        
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),dbo.Requests.RequestDate) As UserField5,        
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),dbo.Requests.RequestDate) As UserField6,        
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),dbo.Requests.RequestDate) As UserField7,        
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),dbo.Requests.RequestDate) As UserField8,        
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),dbo.Requests.RequestDate) As UserField9,        
 				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),dbo.Requests.RequestDate) As UserField10,
				dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age        
         FROM dbo.sysroEmployeeGroups with (nolock) 
 				INNER JOIN dbo.requests with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.requests.IDEmployee AND dbo.requests.RequestDate >= dbo.sysroEmployeeGroups.BeginDate AND dbo.requests.RequestDate <= dbo.sysroEmployeeGroups.EndDate 
 				INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.requests.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.requests.RequestDate >= dbo.EmployeeContracts.BeginDate AND dbo.requests.RequestDate <= dbo.EmployeeContracts.EndDate 
 				LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
  				left join (SELECT IDRequest, COUNT(*) NumeroDias FROM sysroRequestDays GROUP BY IDRequest) aux on aux.IDRequest = requests.id
         WHERE dbo.GetRequestPassportPermission(@idpassport, Requests.ID) > 1        
 				AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.requests.RequestDate between @initialDate and @endDate        
 				AND dbo.Requests.RequestType in (select idRequest from @requestTypeFilter)      
 		GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.requests.RequestDate), YEAR(dbo.requests.RequestDate), dbo.requests.ID,         
 					dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.requests.RequestDate), dbo.requests.RequestDate, CONVERT(nvarchar(8), dbo.requests.RequestDate, 108),         
 					dbo.requests.RequestDate, dbo.EmployeeContracts.IDContract,
 					dbo.Employees.Name, (DATEPART(dw, dbo.requests.RequestDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.requests.RequestType, dbo.requests.Status, dbo.requests.Date1, dbo.requests.FromTime, dbo.requests.Date2, dbo.requests.ToTime, dbo.requests.IDCause,CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldName, '')),CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldValue, '')), dbo.requests.IDShift,CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')), 
 					dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate , aux.NumeroDias
         HAVING  (dbo.Requests.RequestType IN(1,2,3,4,5,6,7,8,9,11,14,15))
GO

ALTER PROCEDURE [dbo].[Genius_Requests_Tasks]
       	@initialDate smalldatetime,@endDate smalldatetime,@idpassport int,@employeeFilter nvarchar(max),@userFieldsFilter nvarchar(max) AS
 		DECLARE @employeeIDs Table(idEmployee int)
  		DECLARE @SecurityMode int
  		
 		SELECT @SecurityMode = convert(numeric(3),isnull(value,'1')) from sysroLiveAdvancedParameters where ParameterName = 'SecurityMode'
 		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter,@initialDate,@endDate;
 		
 		SELECT  dbo.requests.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee, 
                 dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Requests.RequestDate as 'Date', 
                 dbo.Requests.Status AS RequestsStatus, 
   				Case 
   				WHEN Requests.Status IN(2,3) THEN (SELECT top 1 isnull(sysropassports.Name,'') from RequestsApprovals inner join sysroPassports on RequestsApprovals.IDPassport = sysroPassports.ID  where IDRequest = Requests.ID AND Status in(2,3) )
   				ELSE '---'
   				END as 'ApproveRefusePassport',
   				Case 
   					WHEN Requests.Status IN(0,1) Then (select dbo.GetRequestNextLevelPassports(Requests.ID,@SecurityMode))
   					ELSE '---'
   				END as 'PendingPassport',
   				dbo.Requests.RequestType,
   				isnull(CONVERT(NVARCHAR(4000), dbo.Requests.Date1, 120),'') as 'DateTask1',
   				isnull(CONVERT(NVARCHAR(4000), dbo.Requests.Date2, 120),'') as 'DateTask2',
   				ISNULL((SELECT Name from dbo.Tasks WHERE ID = dbo.Requests.IDTask1),'') as 'NameTask1',
   				ISNULL((SELECT Name from dbo.Tasks WHERE ID = dbo.Requests.IDTask2),'') as 'NameTask2',
   				isnull(dbo.Requests.CompletedTask,0) as 'CompletedTask',
   				isnull(dbo.Requests.Field1,'') as 'TaskField1',
   				isnull(dbo.Requests.Field2,'') as 'TaskField2',
   				isnull(dbo.Requests.Field3,'') as 'TaskField3',
   				isnull(dbo.Requests.Field4,0) as 'TaskField4',
   				isnull(dbo.Requests.Field5,0) as 'TaskField5',
   				isnull(dbo.Requests.Field6,0) as 'TaskField6',
   				CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')) as CommentsRequest,
                 CONVERT(nvarchar(8), dbo.Requests.RequestDate, 108) AS Time, DAY(dbo.Requests.RequestDate) AS Day, 
                 MONTH(dbo.Requests.RequestDate) AS Month, YEAR(dbo.Requests.RequestDate) AS Año, (DATEPART(dw, dbo.Requests.RequestDate) + @@DATEFIRST - 1 - 1)  % 7 + 1 AS DayOfWeek, 1 AS Value, 
 				dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, 
       			dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,
       			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
       			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
       			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
       			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
       			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
       			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),dbo.Requests.RequestDate) As UserField1,
       			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),dbo.Requests.RequestDate) As UserField2,
       			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),dbo.Requests.RequestDate) As UserField3,
       			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),dbo.Requests.RequestDate) As UserField4,
       			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),dbo.Requests.RequestDate) As UserField5,
       			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),dbo.Requests.RequestDate) As UserField6,
       			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),dbo.Requests.RequestDate) As UserField7,
       			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),dbo.Requests.RequestDate) As UserField8,
       			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),dbo.Requests.RequestDate) As UserField9,
       			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),dbo.Requests.RequestDate) As UserField10,
				dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age
   									
        FROM         dbo.sysroEmployeeGroups with (nolock) INNER JOIN
                              dbo.requests with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.requests.IDEmployee AND dbo.requests.RequestDate >= dbo.sysroEmployeeGroups.BeginDate AND 
                              dbo.requests.RequestDate <= dbo.sysroEmployeeGroups.EndDate INNER JOIN
                              dbo.EmployeeContracts with (nolock) ON dbo.requests.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.requests.RequestDate >= dbo.EmployeeContracts.BeginDate AND 
                              dbo.requests.RequestDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                              dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID 
        WHERE	dbo.GetRequestPassportPermission(@idpassport, Requests.ID) > 1
       			AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.requests.RequestDate between @initialDate and @endDate
   				
        GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.requests.RequestDate), YEAR(dbo.requests.RequestDate), dbo.requests.ID, dbo.requests.IDTask1 , dbo.requests.IDTask2,dbo.requests.CompletedTask,dbo.requests.Field1,dbo.requests.Field2,dbo.requests.Field3,dbo.requests.Field4,dbo.requests.Field5,dbo.requests.Field6,
                              dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.requests.RequestDate), dbo.requests.RequestDate, CONVERT(nvarchar(8), dbo.requests.RequestDate, 108), 
                              dbo.requests.RequestDate,  dbo.EmployeeContracts.IDContract, dbo.Employees.Name, 
                              (DATEPART(dw, dbo.requests.RequestDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.requests.RequestType, dbo.requests.Status, dbo.requests.Date1, dbo.requests.Date2,  CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')), dbo.sysroEmployeeGroups.Path, 
                              dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, 
   							dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate
        HAVING      (dbo.Requests.RequestType IN(10))
GO

ALTER PROCEDURE  [dbo].[Genius_Schedule]
        	@initialDate smalldatetime,
        	@endDate smalldatetime,
        	@idpassport int,
        	@employeeFilter nvarchar(max),
        	@userFieldsFilter nvarchar(max)
         AS
         DECLARE @employeeIDs Table(idEmployee int)
     	 DECLARE @pinitialDate smalldatetime = @initialDate,  
          @pendDate smalldatetime = @endDate,  
          @pidpassport int = @idpassport,  
          @pemployeeFilter nvarchar(max) = @employeeFilter,  
          @puserFieldsFilter nvarchar(max) = @userFieldsFilter 
  		
  		DECLARE @intdatefirst int
  		SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'
  		SET DateFirst @intdatefirst;
          insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;
          SELECT     CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
     		+ '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10) AS KeyView, 
     		dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, 
     		dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeesShifts.ShiftName, 
     		dbo.sysroEmployeesShifts.CurrentDate AS Date, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours,SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHoursAbs) as HoursAbs, COUNT(*) AS Count, 
     		MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año, (DATEPART(dw, 
     		dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear, 
     		DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear, DATEPART(QUARTER, dbo.sysroEmployeesShifts.CurrentDate) as Quarter, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, 
     		dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.endDate,
     		dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.endDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,
     		dbo.sysroEmployeesShifts.IDAssignment,
     		convert(nvarchar(max),dbo.sysroEmployeesShifts.Remark) as Remark,
     		dbo.sysroEmployeesShifts.AssignmentName,
     		dbo.sysroEmployeesShifts.IDProductiveUnit,
     		dbo.sysroEmployeesShifts.ProductiveUnitName, 
   		    dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
     		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
     		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
     		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
     		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
     		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField1,
     		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField2,
     		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField3,
     		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField4,
     		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField5,
     		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField6,
     		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField7,
     		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField8,
     		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField9,
     		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField10,
			dbo.GetEmployeeAge(sysroEmployeesShifts.idEmployee) As Age, (SELECT LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = sysroEmployeesShifts.IDEmployee) as LockDate
         FROM dbo.sysroEmployeesShifts with (nolock) 
     		INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.sysroEmployeeGroups.endDate 
     		INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.EmployeeContracts.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.EmployeeContracts.endDate 
     		LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
     	WHERE  dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeesShifts.IDEmployee,2,0,0,dbo.sysroEmployeesShifts.CurrentDate) > 1
     		AND dbo.sysroEmployeesShifts.IDEmployee in( select idEmployee from @employeeIDs) and dbo.sysroEmployeesShifts.CurrentDate between @pinitialDate and @pendDate
     	GROUP BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name, 
     		dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate, MONTH(dbo.sysroEmployeesShifts.CurrentDate), 
     		YEAR(dbo.sysroEmployeesShifts.CurrentDate), (DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, 
     		dbo.sysroEmployeesShifts.CurrentDate), DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate), dbo.sysroEmployeeGroups.IDGroup, 
     		dbo.EmployeeContracts.IDContract, CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
     		+ '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10), dbo.sysroEmployeeGroups.Path, 
     		dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.endDate,
     		dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.endDate,dbo.sysroEmployeesShifts.IDAssignment,dbo.sysroEmployeesShifts.AssignmentName,dbo.sysroEmployeesShifts.IDProductiveUnit,dbo.sysroEmployeesShifts.ProductiveUnitName, convert(nvarchar(max),dbo.sysroEmployeesShifts.Remark)
GO

ALTER PROCEDURE [dbo].[Genius_Tasks]
       @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @userFieldsFilter nvarchar(max) AS  
   		
 		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @puserFieldsFilter nvarchar(max) = @userFieldsFilter  
 		SELECT  dbo.sysroEmployeeGroups.IDGroup, dbo.sysroEmployeeGroups.FullGroupName AS GroupName, dbo.Employees.ID AS IDEmployee,   
 					dbo.Employees.Name AS EmployeeName, DATEPART(year, dbo.DailyTaskAccruals.Date) AS Date_Year, DATEPART(month, dbo.DailyTaskAccruals.Date) AS Date_Month, 
 					DATEPART(day, dbo.DailyTaskAccruals.Date) AS Date_Day, dbo.DailyTaskAccruals.Date, dbo.BusinessCenters.ID AS IDCenter, dbo.BusinessCenters.Name AS CenterName, 
 					dbo.Tasks.ID AS IDTask, dbo.Tasks.Name AS TaskName, dbo.Tasks.InitialTime, ISNULL(dbo.Tasks.Field1, '') AS Field1_Task, ISNULL(dbo.Tasks.Field2, '') AS Field2_Task, ISNULL(dbo.Tasks.Field3, '') AS Field3_Task, 
 					ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field4), 0) AS Field4_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field5), 0) AS Field5_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field6), 0) AS Field6_Task,
 					ISNULL(dbo.DailyTaskAccruals.Field1, '') AS Field1_Total, ISNULL(dbo.DailyTaskAccruals.Field2, '') AS Field2_Total, ISNULL(dbo.DailyTaskAccruals.Field3, '') AS Field3_Total, 
 					ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field4), 0) AS Field4_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field5), 0) AS Field5_Total, 
 					ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field6), 0) AS Field6_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Value), 0) AS Value, 
 					ISNULL(dbo.Tasks.Project, '') AS Project, ISNULL(dbo.Tasks.Tag, '') AS Tag, dbo.Tasks.IDPassport, ISNULL(dbo.Tasks.TimeChangedRequirements, 0) AS TimeChangedRequirements, 
 					ISNULL(dbo.Tasks.ForecastErrorTime, 0) AS ForecastErrorTime, ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) AS NonProductiveTimeIncidence, ISNULL(dbo.Tasks.EmployeeTime, 0) AS EmployeeTime,   
 					ISNULL(dbo.Tasks.TeamTime, 0) AS TeamTime, ISNULL(dbo.Tasks.MaterialTime, 0) AS MaterialTime, ISNULL(dbo.Tasks.OtherTime, 0) AS OtherTime,
 					ISNULL(dbo.Tasks.InitialTime, 0) + ISNULL(dbo.Tasks.TimeChangedRequirements, 0) + ISNULL(dbo.Tasks.ForecastErrorTime, 0) + ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) + ISNULL(dbo.Tasks.EmployeeTime, 0) + ISNULL(dbo.Tasks.TeamTime, 0) + ISNULL(dbo.Tasks.MaterialTime, 0) + ISNULL(dbo.Tasks.OtherTime, 0) AS Duration,   
                     CASE WHEN Tasks.Status = 0 THEN 'Activa' ELSE CASE WHEN Tasks.Status = 1 THEN 'Finalizada' ELSE CASE WHEN Tasks.Status = 2 THEN 'Cancelada' ELSE 'Pendiente' END END END AS Estado, 
 					dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,  
 					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
 					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
 					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
 					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
 					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.DailyTaskAccruals.Date) As UserField1,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.DailyTaskAccruals.Date) As UserField2,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.DailyTaskAccruals.Date) As UserField3,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.DailyTaskAccruals.Date) As UserField4,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.DailyTaskAccruals.Date) As UserField5,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.DailyTaskAccruals.Date) As UserField6,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.DailyTaskAccruals.Date) As UserField7,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.DailyTaskAccruals.Date) As UserField8,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.DailyTaskAccruals.Date) As UserField9,  
 					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.DailyTaskAccruals.Date) As UserField10,
			        dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age  
 		FROM    dbo.sysroEmployeeGroups with (nolock)    
                     INNER JOIN dbo.DailyTaskAccruals with (nolock) ON dbo.DailyTaskAccruals.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.DailyTaskAccruals.Date BETWEEN dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate
                     INNER JOIN dbo.Tasks with (nolock) ON dbo.Tasks.ID = dbo.DailyTaskAccruals.IDTask   
                     INNER JOIN dbo.BusinessCenters with (nolock) ON dbo.Tasks.IDCenter = dbo.BusinessCenters.ID   
                     INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyTaskAccruals.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyTaskAccruals.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyTaskAccruals.Date <= dbo.EmployeeContracts.EndDate  
 					INNER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID  
        WHERE    (dbo.businesscenters.id is null OR dbo.BusinessCenters.ID IN (SELECT DISTINCT IDCenter FROM sysroPassports_Centers with (nolock) WHERE IDPassport=(SELECT isnull(IdparentPassport,0) from sysroPassports where ID = @pidpassport)) )  
 					AND dbo.DailyTaskAccruals.Date between @pinitialDate and @pendDate
GO

CREATE PROCEDURE [dbo].[ObtainFeaturesFromFilter]    
     @requestTypes nvarchar(max)
     AS     
    begin  
		declare @prequestTypes nvarchar(max) = @requestTypes  
		DECLARE @SQLString nvarchar(MAX);    
        SET @SQLString = 'select Alias,EmployeeFeatureId, r.Type from sysroFeatures f inner join sysroRequestType r on f.AliasID = r.IdType and r.IdType IN (' + @requestTypes + ')'

		exec sp_executesql @SQLString    
    end
GO


 ALTER PROCEDURE  [dbo].[Genius_Users]
         @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @requestTypesFilter nvarchar(max) AS      

 		DECLARE @employeeIDs Table(idEmployee int)      
 		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter       
		DECLARE @employeeIDFilter NVARCHAR(MAX)
		DECLARE @featureNames Table(feature nvarchar(MAX), featureid int, fieldName nvarchar(max))
		DECLARE @featureFields NVARCHAR(max)

		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate
		select @employeeIDFilter = substring(tmp.empIds,2, len(tmp.empIds) -1) from
			(select convert(nvarchar(max), (select ',' + convert(nvarchar(max),idemployee) as [data()] from @employeeIDs FOR XML PATH(''))) as empIds) tmp

		IF @requestTypesFilter = ''
			SET @requestTypesFilter = '-1'
			
		insert into @featureNames exec dbo.ObtainFeaturesFromFilter @requestTypesFilter
		select @featureFields = isnull(tmp.empIds,'') from
			(select convert(nvarchar(max), (select ',dbo.GetFeatureNextLevelPassportsByEmployee(emp.id,''' + feature  +''',' + convert(nvarchar(max),featureid) + ') AS [sup' + fieldName + ']' as [data()] from @featureNames FOR XML PATH(''))) as empIds) tmp
		
	DECLARE @SqlStatement NVARCHAR(MAX)
	SET @SqlStatement = N'
 		select * from (  
 			select CAST(emp.ID AS varchar) + ''-'' + CAST(eg.IDGroup AS varchar) + ''-'' + ec.IDContract AS KeyView,
 			emp.Id as IDEmployee, emp.Name as EmployeeName, spam.method as Method, spam.credential as Credential, spam.version as Version,     
 			CASE WHEN len(CONVERT(VARCHAR(MAX), CONVERT(VARBINARY(MAX),spam.BiometricData))) > 0 THEN ''X'' ELSE '''' END AS BiometricData,    
 			spam.Enabled as Enabled, spam.TimeStamp as TimeStamp, t.Description as Terminal, spam.BiometricAlgorithm as BiometricAlgorithm,     
 			g.name as GroupName, g.FullGroupName, ec.IDContract, ec.BeginDate as BeginContract, ec.EndDate as EndContract, eg.BeginDate as BeginDate, eg.EndDate as EndDate,  
 			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,1,''\'') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,2,''\'') As Nivel2,      
 			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,3,''\'') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,4,''\'') As Nivel4,      
 			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,5,''\'') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,6,''\'') As Nivel6,      
 			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,7,''\'') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,8,''\'') As Nivel8,      
 			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,9,''\'') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,10,''\'') As Nivel10,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',1,'',''),GetDate()) As UserField1,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',2,'',''),GetDate()) As UserField2,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',3,'',''),GetDate()) As UserField3,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',4,'',''),GetDate()) As UserField4,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',5,'',''),GetDate()) As UserField5,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',6,'',''),GetDate()) As UserField6,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',7,'',''),GetDate()) As UserField7,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',8,'',''),GetDate()) As UserField8,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',9,'',''),GetDate()) As UserField9,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',10,'',''),GetDate()) As UserField10,
			dbo.GetEmployeeAge(emp.Id) As Age' + @featureFields + '    
         FROM employees emp    
 			 inner join sysroPassports sp on sp.IDEmployee = emp.id    
 			 inner join sysroPassports_AuthenticationMethods spam on spam.IDPassport = sp.ID    
 			 inner join EmployeeContracts ec on emp.id = ec.IDEmployee    
 			 inner join sysroEmployeeGroups eg on eg.IDEmployee = emp.id    
 			 inner join Groups g on g.id = eg.IDGroup    
 			 left join Terminals t on t.id = spam.BiometricTerminalId    
 		WHERE dbo.WebLogin_GetPermissionOverEmployee(' + CAST(@pidpassport as nvarchar(100)) + ',emp.Id,2,0,0,GetDate()) > 1      
 			AND emp.Id in(' + @employeeIDFilter + ') and convert(smalldatetime,''' + convert(nvarchar(max),@pinitialDate,112) + ''',120) >= ec.BeginDate and convert(smalldatetime,''' + convert(nvarchar(max),@pendDate,112) + ''',120) <= ec.EndDate  and Version != ''RXA200''  ) as query';
 
	EXEC(@SqlStatement)
GO

update geniusViews set DSFunction = 'Genius_Users(@initialDate,@endDate,@idpassport,@employeeFilter,@userFieldsFilter,@requestTypesFilter)',
	CustomFields = '{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":""}' where DS = 'U'
GO

CREATE PROCEDURE [dbo].[Genius_ConceptsAndPunches]
         @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint AS  
 
 		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros  
 		
 		IF @pincludeZeros = 1   
 		BEGIN  
 			 exec GetPivotAccrualsWithZeros @pinitialDate,@pendDate,@idpassport,@pemployeeFilter,@pconceptsFilter,@puserFieldsFilter
 		END  
 		ELSE  
 		BEGIN  
 			exec GetPivotAccruals @pinitialDate,@pendDate,@idpassport,@pemployeeFilter,@pconceptsFilter,@puserFieldsFilter
        END  
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[GeniusViews] WHERE [Name] = 'conceptsColumns' and [IdPassport] = 0)
    INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature], [RequieredFeature]) 
	VALUES (0, N'conceptsColumns', N'', N'S', 1, CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'', N'0', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"},{"uniqueName":"punches"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"IDEmployee","aggregation":"count"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-06-28T07:53:55.468Z"}', N'', N'', N'', N'{"IncludeZeros":true,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}', N'Genius_ConceptsAndPunches(@initialDate,@endDate,@idpassport,@employeeFilter,@conceptsFilter,@userFieldsFilter,@includeZeros)', N'Calendar', N'Calendar.Analytics')
GO

IF EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'RoboticsFeedUrl' and Value= 'https://www.robotics.es/blog/feed/')
	update sysroLiveAdvancedParameters set Value='https://www.visualtime.net/blog/feed/newfeed/' where ParameterName = 'RoboticsFeedUrl'
GO
insert into sysroGUI values ('Portal\Surveys', 'Gui.SurveysGeneral', '/Surveys', 'Surveys96.png', null, null, null, null, 1401, null, 'U:Employees=Read')
GO

DROP TABLE IF EXISTS [dbo].[SurveyEmployeeStatus]
GO
DROP TABLE IF EXISTS [dbo].[SurveyEmployees]
GO
DROP TABLE IF EXISTS [dbo].[SurveyGroups]
GO
DROP TABLE IF EXISTS [dbo].[SurveyEmployeeResponses]
GO
DROP TABLE IF EXISTS [dbo].[Surveys]
GO

CREATE TABLE [dbo].[Surveys](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Title] [NVARCHAR](100) NOT NULL,
    [IdCreatedBy] [int] NOT NULL,
    [CreatedOn] [smalldatetime] NOT NULL,
    [ModifiedOn] [smalldatetime] NOT NULL,
    [SentOn] [smalldatetime] NOT NULL,
    [Content] [nvarchar](max) NOT NULL,
    [Mandatory] [bit] NOT NULL,
    [ResponseMaxPercentage] [int] NULL,
    [ExpirationDate] [smalldatetime] NOT NULL,
    [Status] [smallint] NOT NULL
PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Surveys]  WITH CHECK ADD FOREIGN KEY([IdCreatedBy])
REFERENCES [dbo].[sysroPassports] ([ID])
GO


CREATE TABLE [dbo].[SurveyEmployees](
    [IdSurvey] [int] NOT NULL,
    [IdEmployee] [int] NOT NULL,
 CONSTRAINT [PK_SurveyEmployees] PRIMARY KEY CLUSTERED 
(
    [IdSurvey] ASC,
    [IdEmployee] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SurveyEmployees]  WITH CHECK ADD FOREIGN KEY([IdSurvey])
REFERENCES [dbo].[Surveys] ([Id])
GO

ALTER TABLE [dbo].[SurveyEmployees]  WITH CHECK ADD FOREIGN KEY([IdEmployee])
REFERENCES [dbo].[Employees] ([ID])
GO

CREATE TABLE [dbo].[SurveyGroups](
    [IdSurvey] [int] NOT NULL,
    [IdGroup] [int] NOT NULL,
CONSTRAINT [PK_SurveyGroups] PRIMARY KEY CLUSTERED 
(
    [IdSurvey] ASC,
    [IdGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SurveyGroups]  WITH CHECK ADD FOREIGN KEY([IdSurvey])
REFERENCES [dbo].[Surveys] ([Id])
GO

ALTER TABLE [dbo].[SurveyGroups]  WITH CHECK ADD FOREIGN KEY([IdGroup])
REFERENCES [dbo].[Groups] ([ID])
GO

CREATE TABLE [dbo].[SurveyEmployeeResponses](
    [IdSurvey] [int] NOT NULL,
    [IdEmployee] [int] NOT NULL,
    [SurveyResponse] [nvarchar](max) NOT NULL,
    [ResponseTimeStamp] [smalldatetime] NULL,
     CONSTRAINT [PK_SurveysEmployeeStatus] PRIMARY KEY CLUSTERED 
(
    [IdSurvey] ASC,
    [IdEmployee] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SurveyEmployeeResponses]  WITH CHECK ADD FOREIGN KEY([IdSurvey])
REFERENCES [dbo].[Surveys] ([Id])
GO

ALTER TABLE [dbo].[SurveyEmployeeResponses]  WITH CHECK ADD FOREIGN KEY([IdEmployee])
REFERENCES [dbo].[Employees] ([ID])
GO

DROP VIEW IF EXISTS [dbo].[sysrovwSurveysEmployees]
GO
CREATE VIEW [dbo].[sysrovwSurveysEmployees]    
 AS    
    SELECT IdSurvey, SurveyEmployees.IdEmployee FROM SurveyEmployees
 	LEFT JOIN Surveys ON Surveys.Id = SurveyEmployees.IdSurvey
    UNION    
    SELECT IdSurvey, sysrovwCurrentEmployeeGroups.IdEmployee FROM SurveyGroups
    LEFT JOIN Surveys ON Surveys.Id = SurveyGroups.IdSurvey    
    INNER JOIN Groups ON SurveyGroups.idgroup = Groups.id
    INNER JOIN sysrovwCurrentEmployeeGroups ON (sysrovwCurrentEmployeeGroups.Path = Groups.Path OR sysrovwCurrentEmployeeGroups.Path LIKE Groups.Path + '\%')
    INNER JOIN sysroPassports ON Surveys.IdCreatedBy = sysroPassports.Id 
           and (dbo.WebLogin_GetPermissionOverGroup(sysroPassports.Id, sysrovwCurrentEmployeeGroups.IdGroup, 1, 0) >=  3)  
 		  and dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.Id, sysrovwCurrentEmployeeGroups.IdEmployee,1,0,1,GetDate())>= 3
    WHERE sysrovwCurrentEmployeeGroups.CurrentEmployee = 1  
GO

DROP VIEW IF EXISTS [dbo].[sysrovwSurveyStatistics] 
GO
 CREATE VIEW [dbo].[sysrovwSurveyStatistics]
 AS
 	SELECT Surveys.Id as IdSurvey, 
 		   Surveys.Title, 
 		   sysrovwSurveysEmployees.IdEmployee, 
 		   Employees.Name, 
		   sysrovwCurrentEmployeeGroups.GroupName,
 		   CASE WHEN ResponseTimeStamp IS NULL THEN 0 ELSE 1 END as SurveyStatus, 
 		   SurveyEmployeeResponses.ResponseTimeStamp, 
 		   Surveys.Mandatory as RequiresResponse,
 		   SurveyEmployeeResponses.SurveyResponse
 	FROM sysrovwSurveysEmployees
 	INNER JOIN Employees ON Employees.ID = sysrovwSurveysEmployees.IdEmployee
	INNER JOIN sysrovwCurrentEmployeeGroups ON sysrovwCurrentEmployeeGroups.IDEmployee = sysrovwSurveysEmployees.IdEmployee
 	LEFT JOIN Surveys ON sysrovwSurveysEmployees.IdSurvey = Surveys.Id
 	LEFT JOIN SurveyEmployeeResponses  ON SurveyEmployeeResponses.IdSurvey = sysrovwSurveysEmployees.IdSurvey AND sysrovwSurveysEmployees.IdEmployee = SurveyEmployeeResponses.IdEmployee
GO

UPDATE sysroLiveAdvancedParameters SET Value = 15 WHERE ParameterName='VTPortalApiVersion'
GO

ALTER TABLE [dbo].[ProgrammedAbsences] ADD Timestamp smalldatetime NULL
GO

ALTER TABLE [dbo].[ProgrammedCauses] ADD Timestamp smalldatetime NULL
GO

CREATE TABLE [dbo].[DeletedProgrammedAbsences](
	[IDEmployee] [int] NOT NULL,
	[AbsenceID] [int] NOT NULL,
	[TimeStamp] [smalldatetime] NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DeletedProgrammedCauses](
	[IDEmployee] [int] NOT NULL,
	[AbsenceID] [int] NOT NULL,
	[TimeStamp] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Surveys] ADD [Anonymous] [bit] NOT NULL DEFAULT 0
GO

ALTER TABLE [dbo].[Surveys] ADD [SurveyMode] [bit] NOT NULL DEFAULT 0
GO


ALTER TABLE [dbo].[TMPHOLIDAYSCONTROLByContract]  ADD DaysPendingApproval [numeric](16, 2) NULL default(0)
GO

INSERT INTO [dbo].[sysroLiveAdvancedParameters] (ParameterName, Value) VALUES ('VTWebAPIVersion','1.0.0') 
GO
ALTER TABLE [dbo].[sysroNotificationTasks] ADD GUID nvarchar(max) NULL
GO

CREATE TABLE [dbo].[TMPSupervisedEmployees](
	[IDEmployee] [numeric](18, 0) NOT NULL,
	[IDReportTask] [numeric](16, 0) NOT NULL default(0)
 CONSTRAINT [PK_TMPSupervisedEmployees] PRIMARY KEY NONCLUSTERED 
(
	[IDReportTask] ASC,
	[IDEmployee] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SurveyTemplates](
    [Id] [int] IDENTITY(1,1) NOT NULL,
	[DefaultTitle] [NVARCHAR](100) NOT NULL,
    [Key] [NVARCHAR](100) NOT NULL,
    [Content] [nvarchar](max) NOT NULL
PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

alter table dbo.terminals alter column punchstamp nvarchar(50)
GO

UPDATE sysroParameters SET Data='517' WHERE ID='DBVersion'
GO


 