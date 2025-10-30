ALTER PROCEDURE [dbo].[Genius_CostCenters_Detail]
           @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @costCenterFilter nvarchar(max), @causesFilter nvarchar(max), @includeZeros as bit   AS  
   		DECLARE @employeeIDs Table(idEmployee int)  
   		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pbusinessCenterFilter nvarchar(max) = @costCenterFilter, @pcausesFilter nvarchar(max) = @causesFilter
		DECLARE @causesIDs Table(idCause int) 
   		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
		if (@causesFilter is NULL OR @causesFilter = '')
	begin
		insert into @causesIDs select ID from dbo.Causes
	end
	else
	begin
		insert into @causesIDs select * from dbo.SplitToInt(@pcausesFilter,',');   
	end
   		DECLARE @businessCenterIDs Table(idBusinessCenter int)  
   		insert into @businessCenterIDs select * from dbo.SplitToInt(@pbusinessCenterFilter,',');  
		IF @includeZeros IS NULL OR @includeZeros = 0
		BEGIN
   		SELECT     CONVERT(varchar(10), dbo.DailyCauses.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailyCauses.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup,   
                       dbo.EmployeeContracts.IDContract, dbo.DailyCauses.IDEmployee, dbo.DailyCauses.Date, dbo.DailyCauses.Date AS Date_ToDateString,  
                       dbo.Causes.Name AS CauseName, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor, isnull(dbo.BusinessCenters.ID, 0) as IDCenter,   
   					isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,    
   					isnull(dbo.BusinessCenters.Field4, '') AS Field4, isnull(dbo.BusinessCenters.Field5, '') AS Field5, IIF(dbo.DailyCauses.Value IS NULL,0,dbo.DailyCauses.Value) AS Value, YEAR(dbo.DailyCauses.Date) AS Año,   
                       (DATEPART(dw, dbo.DailyCauses.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 
					   CASE WHEN DATEPART(iso_week, dbo.DailyCauses.Date) >= 52 AND MONTH(dbo.DailyCauses.Date) = 1 THEN (YEAR(dbo.DailyCauses.Date)-1) * 100 + DATEPART(iso_week, dbo.DailyCauses.Date)
				ELSE (YEAR(dbo.DailyCauses.Date)) * 100 + DATEPART(iso_week, dbo.DailyCauses.Date) END AS WeekOfYear, 
					   DATEPART(dy, dbo.DailyCauses.Date) AS DayOfYear, dbo.DailyCauses.DefaultCenter, dbo.DailyCauses.ManualCenter,   
                       dbo.Employees.Name AS EmployeeName, dbo.Groups.Name AS GroupName, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName, dbo.Groups.Path,   
                       dbo.sysroEmployeeGroups.BeginDate,dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString,dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee, 0 AS CostCenterTotalCost,   
   					dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroCostHour'),DailyCauses.date ) as Cost,   
   					dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPriceHour'), DailyCauses.date) as PVP,  
   					dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod,  @pendDate AS EndPeriod_ToDateString,
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
  					 dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age  ,
 					 dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,					 
					 dbo.Causes.Export as IncidenceEquivalence, Causes.ID AS CauseID
            FROM		 dbo.sysroEmployeeGroups with (nolock)   
                           INNER JOIN dbo.Causes with (nolock)   
                           INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause  ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.DailyCauses.Date between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate   AND DBO.DailyCauses.IDCause IN (SELECT * FROM @causesIDs)     
   						INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyCauses.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyCauses.Date <= dbo.EmployeeContracts.EndDate   
   						INNER JOIN dbo.Groups with (nolock) ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID   
   						INNER JOIN dbo.Employees with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.Employees.ID    
   						LEFT OUTER JOIN dbo.BusinessCenters with (nolock) ON ISNULL(dbo.DailyCauses.IDCenter,0) = dbo.BusinessCenters.ID   
 						LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
             where dbo.dailycauses.date between @pinitialDate and @pendDate 
   					AND isnull(dbo.DailyCauses.IDCenter,0) in (select idBusinessCenter from @businessCenterIDs)   
   					AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs)
					AND dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.DailyCauses.IDEmployee,2,0,0,dbo.DailyCauses.date) > 1   
		END
		ELSE
		BEGIN
		WITH alldays AS ( 
   SELECT @initialDate AS dt 
   UNION ALL 
   SELECT DATEADD(dd, 1, dt) 
   FROM alldays s 
   WHERE DATEADD(dd, 1, dt) <= @pendDate)

			SELECT     CONVERT(varchar(10), reqReg.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), reqReg.dt, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup,   
                       dbo.EmployeeContracts.IDContract, reqReg.IDEmployee, reqReg.dt, reqReg.dt AS Date_ToDateString,  
                       dbo.Causes.Name AS CauseName, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor, isnull(dbo.BusinessCenters.ID, 0) as IDCenter,   
   					isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,    
   					isnull(dbo.BusinessCenters.Field4, '') AS Field4, isnull(dbo.BusinessCenters.Field5, '') AS Field5, IIF(dbo.DailyCauses.Value IS NULL,0,dbo.DailyCauses.Value) AS Value, YEAR(dbo.DailyCauses.Date) AS Año,   
                       (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 
					   CASE WHEN DATEPART(iso_week, reqReg.dt) >= 52 AND MONTH(reqReg.dt) = 1 THEN (YEAR(reqReg.dt)-1) * 100 + DATEPART(iso_week, dbo.DailyCauses.Date)
				ELSE (YEAR(reqReg.dt)) * 100 + DATEPART(iso_week, reqReg.dt) END AS WeekOfYear, 
					   DATEPART(dy, reqReg.dt) AS DayOfYear, dbo.DailyCauses.DefaultCenter, dbo.DailyCauses.ManualCenter,   
                       dbo.Employees.Name AS EmployeeName, dbo.Groups.Name AS GroupName, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName, dbo.Groups.Path,   
                       dbo.sysroEmployeeGroups.BeginDate,dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString,dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee, 0 AS CostCenterTotalCost,   
   					dbo.GetValueFromEmployeeUserFieldValues(reqReg.idEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroCostHour'),reqReg.dt ) as Cost,   
   					dbo.GetValueFromEmployeeUserFieldValues(reqReg.idEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPriceHour'), reqReg.dt) as PVP,  
   					dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod,  @pendDate AS EndPeriod_ToDateString,
   					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
   					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
   					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
   					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
   					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
   					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),reqReg.dt) As UserField1,  
   					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),reqReg.dt) As UserField2,  
   					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),reqReg.dt) As UserField3,  
   					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),reqReg.dt) As UserField4,  
   					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),reqReg.dt) As UserField5,  
   					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),reqReg.dt) As UserField6,  
   					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),reqReg.dt) As UserField7,  
   					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),reqReg.dt) As UserField8,  
   					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),reqReg.dt) As UserField9,  
   					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),reqReg.dt) As UserField10,
  					 dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age  ,
 					 dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,					 
					 dbo.Causes.Export as IncidenceEquivalence, Causes.ID AS CauseID
            FROM 
			(select alldays.dt,emp.ID as idEmployee, emp.Name as employeeName, CAU.idCause AS IdCause, bc.ID AS IdBusinessCenter from alldays,  
   (select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp   ,
   (select IdCause from @causesIDs) cau,
   (select idBusinessCenter AS ID from @businessCenterIDs) bc
   where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1
   and (select count(*) from dbo.DailyCauses where IDEmployee = emp.ID and IDCenter = bc.ID and date between @initialDate and @pendDate) > 0
   ) reqReg 
   INNER JOIN dbo.sysroEmployeeGroups ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate 			
                         LEFT JOIN dbo.Causes ON dbo.Causes.ID = reqReg.IdCause
						   LEFT JOIN dbo.DailyCauses ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.DailyCauses.Date = reqReg.dt and dbo.DailyCauses.IDCause = reqReg.IdCause and dbo.DailyCauses.IDCenter = IdBusinessCenter
   						INNER JOIN dbo.EmployeeContracts  ON reqReg.IDEmployee = dbo.EmployeeContracts.IDEmployee AND reqReg.dt >= dbo.EmployeeContracts.BeginDate AND reqReg.dt <= dbo.EmployeeContracts.EndDate   
   						INNER JOIN dbo.Groups  ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID   
   						LEFT OUTER JOIN dbo.Employees  ON reqReg.idEmployee = dbo.Employees.ID    
   						LEFT JOIN dbo.BusinessCenters  ON DBO.BusinessCenters.ID = IdBusinessCenter
 						LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
             where reqReg.dt between @pinitialDate and @pendDate 
   					AND isnull(IdBusinessCenter,0) in (select idBusinessCenter from @businessCenterIDs)   
   					AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs)					
		END

GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='687' WHERE ID='DBVersion'
GO
