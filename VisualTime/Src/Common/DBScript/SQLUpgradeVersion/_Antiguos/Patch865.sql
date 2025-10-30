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
  				reqReg.conceptName AS ConceptName, reqReg.dt AS Date, reqReg.dt AS Date_ToDateString, 
				CASE WHEN dbo.Concepts.DefaultQuery <> 'L' THEN sum(isnull(dbo.DailyAccruals.Value,0))
				ELSE sum(dbo.DailyAccruals.Value) END
				 AS Value, 
				CASE WHEN dbo.Concepts.DefaultQuery <> 'L' THEN sum(isnull(dbo.DailyAccruals.Value,0))
				ELSE sum(dbo.DailyAccruals.Value) END
				AS ValueHHMM_ToHours,
				COUNT(*) AS Count, MONTH(reqReg.dt) AS Mes,   
  				YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 
				CASE WHEN DATEPART(iso_week, reqReg.dt) >= 52 AND MONTH(reqReg.dt) = 1 THEN (YEAR(reqReg.dt)-1) * 100 + DATEPART(iso_week, reqReg.dt)
				ELSE (YEAR(reqReg.dt)) * 100 + DATEPART(iso_week, reqReg.dt) END AS WeekOfYear, DATEPART(QUARTER, reqReg.dt) AS Quarter, dbo.sysroEmployeeGroups.Path,   
  				dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString,
  				dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
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
 				dbo.GetEmployeeAge(reqReg.idEmployee) As Age     ,
 				dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,
				dbo.Concepts.Export as AccrualEquivalence,
                sum(isnull(dbo.DailyAccruals.PositiveValue,0)) AS PositiveValue, sum(isnull(dbo.DailyAccruals.PositiveValue,0)) AS PositiveValueHHMM_ToHours, ExpiredDate, ExpiredDate as ExpiredDate_ToDateString,
				dbo.DailyAccruals.BeginPeriod as BeginAccrualPeriod_ToDateString, dbo.DailyAccruals.EndPeriod as EndAccrualPeriod_ToDateString, dbo.DailyAccruals.BeginPeriod as BeginAccrualPeriod, dbo.DailyAccruals.EndPeriod as EndAccrualPeriod,
				dbo.DailyAccruals.ExpiredDate as ConceptExpirationDate, dbo.DailyAccruals.StartEnjoymentDate as StartEnjoymentDate, dbo.DailyAccruals.ExpiredDate as ConceptExpirationDate_ToDateString, dbo.DailyAccruals.StartEnjoymentDate as StartEnjoymentDate_ToDateString
  			FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName 
				from alldays,   
  				(select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee ) emp,   
  				(select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con    				
				where (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and alldays.dt between BeginDate and EndDate and idpassport = @idpassport and idemployee = emp.ID and FeaturePermission > 0) > 0
  				) reqReg  				
				INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate  
  				INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate  
  				LEFT OUTER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept  
 				LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = reqReg.idEmployee
				LEFT OUTER JOIN dbo.Concepts with (nolock) ON dbo.Concepts.ID = reqReg.IDConcept				
  			GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName,   
  				dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt),   
  				YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy,   
  				reqReg.dt), CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar)   
  				+ '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120),   
  				10), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
  				dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate, dbo.Concepts.Export, PositiveValue, ExpiredDate,
				dbo.DailyAccruals.BeginPeriod, dbo.DailyAccruals.EndPeriod, dbo.DailyAccruals.StartEnjoymentDate,
				dbo.Concepts.DefaultQuery
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
  				reqReg.conceptName AS ConceptName, reqReg.dt AS Date, reqReg.dt AS Date_ToDateString, sum(isnull(dbo.DailyAccruals.Value,0)) AS Value, COUNT(*) AS Count, MONTH(reqReg.dt) AS Mes,   
  				YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week,   
  				reqReg.dt) AS WeekOfYear, DATEPART(dy, reqReg.dt) AS DayOfYear, DATEPART(QUARTER, reqReg.dt) AS Quarter, dbo.sysroEmployeeGroups.Path,   
  				dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate,  dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString,
  				dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod,  @pendDate AS EndPeriod_ToDateString,
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
 				dbo.GetEmployeeAge(reqReg.idEmployee) As Age,
 				dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,
				dbo.Concepts.Export as AccrualEquivalence,
                sum(isnull(dbo.DailyAccruals.PositiveValue,0)) AS PositiveValue, sum(isnull(dbo.DailyAccruals.PositiveValue,0)) AS PositiveValueHHMM_ToHours, ExpiredDate, ExpiredDate as ExpiredDate_ToDateString,
				dbo.DailyAccruals.BeginPeriod as BeginAccrualPeriod_ToDateString, dbo.DailyAccruals.EndPeriod as EndAccrualPeriod_ToDateString, 				dbo.DailyAccruals.BeginPeriod as BeginAccrualPeriod, dbo.DailyAccruals.EndPeriod as EndAccrualPeriod,
				dbo.DailyAccruals.ExpiredDate as ConceptExpirationDate, dbo.DailyAccruals.StartEnjoymentDate as StartEnjoymentDate, dbo.DailyAccruals.ExpiredDate as ConceptExpirationDate_ToDateString, dbo.DailyAccruals.StartEnjoymentDate as StartEnjoymentDate_ToDateString
  		   FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,   
  				(select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp,   
  				(select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con    				
				where (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and alldays.dt between BeginDate and EndDate and idpassport = @idpassport and idemployee = emp.ID and FeaturePermission > 0) > 0
  				) reqReg  					
  				INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate  
  				INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate  
  				INNER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept  				
 				LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = reqReg.idEmployee
				LEFT OUTER JOIN dbo.Concepts with (nolock) ON dbo.Concepts.ID = dbo.DailyAccruals.IDConcept				
  		   GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName,   
  				dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt),   
  				YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy,   
  				reqReg.dt), CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar)   
  				+ '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120),   
  				10), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
  				dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate, dbo.Concepts.Export, PositiveValue, ExpiredDate,
				dbo.DailyAccruals.BeginPeriod, dbo.DailyAccruals.EndPeriod, dbo.DailyAccruals.StartEnjoymentDate
  		   option (maxrecursion 0)  
         END  
GO

ALTER PROCEDURE [dbo].[Genius_Concepts_Hours_Receipt]
@initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint AS
DECLARE @employeeIDs Table(idEmployee int)
DECLARE @conceptIDs Table(idConcept int)
DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros
DECLARE @intdatefirst int
SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'
SET DateFirst @intdatefirst;

insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate
insert into @conceptIDs select * from dbo.SplitToInt(@pconceptsFilter,',');

WITH alldays AS (
SELECT @initialDate AS dt
UNION ALL
SELECT DATEADD(dd, 1, dt)
FROM alldays s
WHERE DATEADD(dd, 1, dt) <= @pendDate)

SELECT reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept,
dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName,
reqReg.conceptName AS ConceptName,
reqReg.dt AS Date, reqReg.dt AS Date_ToDateString,
sum(isnull(AccrualsPrice.Value,0)) AS Value, sum(isnull(AccrualsPrice.Value,0)) AS ValueHHMM_ToHours,COUNT(*) AS Count,
MONTH(reqReg.dt) AS Mes,
YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek,
CASE WHEN DATEPART(iso_week, reqReg.dt) >= 52 AND MONTH(reqReg.dt) = 1 THEN (YEAR(reqReg.dt)-1) * 100 + DATEPART(iso_week, reqReg.dt)
ELSE (YEAR(reqReg.dt)) * 100 + DATEPART(iso_week, reqReg.dt) END AS WeekOfYear, DATEPART(QUARTER, reqReg.dt) AS Quarter, dbo.sysroEmployeeGroups.Path,
dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString,
dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
CASE WHEN ISNULL(AccrualsPrice.Price,'') = '' THEN 0 ELSE CONVERT(Numeric(18,3),AccrualsPrice.Price) END AS Price,
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
AccrualsPrice.BeginPeriod as BeginAccrualPeriod_ToDateString, AccrualsPrice.EndPeriod as EndAccrualPeriod_ToDateString, AccrualsPrice.BeginPeriod as BeginAccrualPeriod, AccrualsPrice.EndPeriod as EndAccrualPeriod,
AccrualsPrice.ExpiredDate as ConceptExpirationDate, AccrualsPrice.StartEnjoymentDate as StartEnjoymentDate, AccrualsPrice.ExpiredDate as ConceptExpirationDate_ToDateString, AccrualsPrice.StartEnjoymentDate as StartEnjoymentDate_ToDateString
FROM
(select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,
(select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp,
(select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con
where (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and alldays.dt between BeginDate and EndDate and idpassport = @idpassport and idemployee = emp.ID and FeaturePermission > 0) > 0
) reqReg
INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate
INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
inner JOIN
(
SELECT DailyAccruals.IDEmployee, DailyAccruals.IDConcept, DailyAccruals.Date,
CASE WHEN ISNULL(Concepts.FixedPay,0) <> 0 THEN
convert(nvarchar(100),Concepts.PayValue)
ELSE
dbo.GetValueFromEmployeeUserFieldValues(DailyAccruals.IDEmployee, SUBSTRING(Concepts.UsedField,5,100), DailyAccruals.Date)
END AS Price,
DailyAccruals.Value, EmployeeContracts.IDContract, EmployeeContracts.BeginDate, EmployeeContracts.EndDate,
DBO.DailyAccruals.BeginPeriod, dbo.DailyAccruals.EndPeriod, DBO.DailyAccruals.BeginPeriod AS BeginPeriod_ToDateString, dbo.DailyAccruals.EndPeriod AS eNDPeriod_ToDateString,
dbo.DailyAccruals.ExpiredDate, dbo.DailyAccruals.StartEnjoymentDate 
FROM DailyAccruals INNER JOIN Concepts ON DailyAccruals.IDConcept = Concepts.ID
INNER JOIN EmployeeContracts ON
DailyAccruals.IDEmployee = EmployeeContracts.IDEmployee AND
DailyAccruals.Date >= EmployeeContracts.BeginDate AND
DailyAccruals.Date <= EmployeeContracts.EndDate
) AS AccrualsPrice
ON
reqReg.IDEmployee = AccrualsPrice.IDEmployee AND reqReg.IDConcept = AccrualsPrice.IDConcept AND AccrualsPrice.Date = reqReg.dt
WHERE ISNULL(AccrualsPrice.Value,0) <> 0 AND ISNULL(AccrualsPrice.Price,'') <> ''
GROUP BY Price,
AccrualsPrice.IDContract, AccrualsPrice.BeginDate, AccrualsPrice.EndDate, reqReg.idEmployee, reqReg.dt
, reqReg.idConcept,
reqReg.employeeName, reqReg.conceptName, AccrualsPrice.IDEmployee, AccrualsPrice.IDConcept, --AccrualsPrice.Date,
dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName,
dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate,
dbo.sysroEmployeeGroups.EndDate, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate, AccrualsPrice.BeginPeriod, AccrualsPrice.EndPeriod,
AccrualsPrice.ExpiredDate, AccrualsPrice.StartEnjoymentDate
ORDER BY sysroEmployeeGroups.FullGroupName, EmployeeName
option (maxrecursion 0)

GO

ALTER PROCEDURE [dbo].[Genius_ConceptsAndSchedule]
@initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint AS
DECLARE @employeeIDs Table(idEmployee int)
DECLARE @conceptIDs Table(idConcept int)
DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros
DECLARE @intdatefirst int
SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'
SET DateFirst @intdatefirst;
insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate
insert into @conceptIDs select * from dbo.SplitToInt(@pconceptsFilter,',');
WITH alldays AS (
SELECT @initialDate AS dt
UNION ALL
SELECT DATEADD(dd, 1, dt)
FROM alldays s
WHERE DATEADD(dd, 1, dt) <= @pendDate)
SELECT
reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept,
dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName,
reqReg.conceptName AS ConceptName, reqReg.dt AS Date, reqReg.dt AS Date_ToDateString, isnull(dbo.DailyAccruals.Value,0) AS Value, isnull(dbo.DailyAccruals.Value,0) AS ValueHHMM_ToHours,COUNT(*) AS Count,
MONTH(reqReg.dt) AS Mes, YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek,
CASE WHEN DATEPART(iso_week, reqReg.dt) >= 52 AND MONTH(reqReg.dt) = 1 THEN (YEAR(reqReg.dt)-1) * 100 + DATEPART(iso_week, reqReg.dt)
ELSE (YEAR(reqReg.dt)) * 100 + DATEPART(iso_week, reqReg.dt) END AS WeekOfYear,
DATEPART(dy, reqReg.dt) AS DayOfYear, DATEPART(QUARTER, reqReg.dt) AS Quarter,
dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString,
dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
Shifts.Name AS ShiftName,
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
dbo.GetEmployeeAge(reqReg.idEmployee) As Age     ,
dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString ,
dbo.Concepts.Export as AccrualEquivalence,
dbo.Shifts.Export as ScheduleEquivalence,
sum(isnull(dbo.DailyAccruals.PositiveValue,0)) AS PositiveValue, sum(isnull(dbo.DailyAccruals.PositiveValue,0)) AS PositiveValueHHMM_ToHours, ExpiredDate, ExpiredDate as ExpiredDate_ToDateString,
dbo.DailyAccruals.BeginPeriod as BeginAccrualPeriod_ToDateString, dbo.DailyAccruals.EndPeriod as EndAccrualPeriod_ToDateString, dbo.DailyAccruals.BeginPeriod as BeginAccrualPeriod, dbo.DailyAccruals.EndPeriod as EndAccrualPeriod,
dbo.DailyAccruals.ExpiredDate as ConceptExpirationDate, dbo.DailyAccruals.StartEnjoymentDate as StartEnjoymentDate, dbo.DailyAccruals.ExpiredDate as ConceptExpirationDate_ToDateString, dbo.DailyAccruals.StartEnjoymentDate as StartEnjoymentDate_ToDateString
FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,
(select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp,
(select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con
where (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and alldays.dt between BeginDate and EndDate and idpassport = @pidpassport and idemployee = emp.ID and FeaturePermission > 0) > 0
) reqReg
INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailySchedule.IDEmployee = reqReg.idEmployee and dbo.DailySchedule.Date = reqReg.dt
INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate
INNER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee
INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
INNER JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(dbo.DailySchedule.IDShiftUsed,dbo.DailySchedule.IDShift1)
LEFT OUTER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept
LEFT OUTER JOIN dbo.Concepts with (nolock) ON dbo.Concepts.ID = dbo.DailyAccruals.IDConcept
GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName,
dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt),
YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy, reqReg.dt),
dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate, dbo.DailyAccruals.Value,
DailySchedule.Date, Shifts.Name, dbo.Concepts.Export,
dbo.Shifts.Export, dbo.DailyAccruals.ExpiredDate, dbo.DailyAccruals.BeginPeriod, dbo.DailyAccruals.EndPeriod, dbo.DailyAccruals.StartEnjoymentDate
option (maxrecursion 0)

GO

ALTER PROCEDURE [dbo].[Genius_ConceptsAndScheduleAndIncidences]
@initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint, @causesFilter nvarchar(max) AS
DECLARE @employeeIDs Table(idEmployee int)
DECLARE @conceptIDs Table(idConcept int)
DECLARE @causesIDs Table(idCause int)
DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @pcausesFilter nvarchar(max) = @causesFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros
DECLARE @intdatefirst int
SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'
SET DateFirst @intdatefirst;
insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate
if (@causesFilter is NULL OR @causesFilter = '')
begin
insert into @causesIDs select ID from dbo.Causes
end
else
begin
insert into @causesIDs select * from dbo.SplitToInt(@pcausesFilter,',');
end
insert into @conceptIDs select * from dbo.SplitToInt(@pconceptsFilter,',');
WITH alldays AS (
SELECT @initialDate AS dt
UNION ALL
SELECT DATEADD(dd, 1, dt)
FROM alldays s
WHERE DATEADD(dd, 1, dt) <= @pendDate)
SELECT
reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept,
dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName,
reqReg.conceptName AS ConceptName, reqReg.dt AS Date, reqReg.dt AS Date_ToDateString, 
CASE WHEN dbo.Concepts.DefaultQuery <> 'L' THEN sum(isnull(dbo.DailyAccruals.Value,0))
ELSE sum(dbo.DailyAccruals.Value) END
AS Value, 
CASE WHEN dbo.Concepts.DefaultQuery <> 'L' THEN sum(isnull(dbo.DailyAccruals.Value,0))
ELSE sum(dbo.DailyAccruals.Value) END
AS ValueHHMM_ToHours,
COUNT(*) AS Count,
MONTH(reqReg.dt) AS Mes, YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek,
CASE WHEN DATEPART(iso_week, reqReg.dt) >= 52 AND MONTH(reqReg.dt) = 1 THEN (YEAR(reqReg.dt)-1) * 100 + DATEPART(iso_week, reqReg.dt)
ELSE (YEAR(reqReg.dt)) * 100 + DATEPART(iso_week, reqReg.dt) END AS WeekOfYear,
DATEPART(dy, reqReg.dt) AS DayOfYear, DATEPART(QUARTER, reqReg.dt) AS Quarter,
dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString,
dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS CauseValue, SUM(dbo.DailyCauses.Value) AS CauseValueHHMM,Shifts.Name AS ShiftName, did.Description AS IncidenceName,
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
dbo.GetEmployeeAge(reqReg.idEmployee) As Age     ,
dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString ,
dbo.Concepts.Export as AccrualEquivalence,
dbo.Causes.Export as IncidenceEquivalence,
dbo.Shifts.Export as ScheduleEquivalence,
CASE WHEN dbo.Concepts.DefaultQuery <> 'L' THEN sum(isnull(dbo.DailyAccruals.PositiveValue,0))
ELSE sum(dbo.DailyAccruals.PositiveValue) END
AS PositiveValue, 
CASE WHEN dbo.Concepts.DefaultQuery <> 'L' THEN sum(isnull(dbo.DailyAccruals.PositiveValue,0))
ELSE sum(dbo.DailyAccruals.PositiveValue) END
AS PositiveValueHHMM_ToHours,
ExpiredDate, ExpiredDate as ExpiredDate_ToDateString,
dbo.DailyAccruals.BeginPeriod as BeginAccrualPeriod_ToDateString, dbo.DailyAccruals.EndPeriod as EndAccrualPeriod_ToDateString, dbo.DailyAccruals.BeginPeriod as BeginAccrualPeriod, dbo.DailyAccruals.EndPeriod as EndAccrualPeriod,
dbo.DailyAccruals.ExpiredDate as ConceptExpirationDate, dbo.DailyAccruals.StartEnjoymentDate as StartEnjoymentDate, dbo.DailyAccruals.ExpiredDate as ConceptExpirationDate_ToDateString, dbo.DailyAccruals.StartEnjoymentDate as StartEnjoymentDate_ToDateString
FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,
(select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp,
(select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con
where (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and alldays.dt between BeginDate and EndDate and idpassport = @pidpassport and idemployee = emp.ID and FeaturePermission > 0) > 0
) reqReg
INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailySchedule.IDEmployee = reqReg.idEmployee and dbo.DailySchedule.Date = reqReg.dt
INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate
INNER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee
INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
INNER JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(dbo.DailySchedule.IDShiftUsed,dbo.DailySchedule.IDShift1)
LEFT OUTER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept
LEFT OUTER JOIN dbo.Concepts with (nolock) ON dbo.Concepts.ID = reqReg.idConcept
LEFT JOIN DBO.DailyCauses with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date                                                                          AND (DailyCauses.IDCause in ( select IdCause FROM DBO.ConceptCauses with (nolock) where IDConcept = dbo.DailyAccruals.IDConcept) )
AND DBO.DailyCauses.IDCause IN (SELECT * FROM @causesIDs)
LEFT OUTER JOIN dbo.Causes with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause
LEFT OUTER JOIN dbo.DailyIncidences with (nolock) ON dbo.DailyIncidences.id = dbo.DailyCauses.IDRelatedIncidence and dbo.DailyIncidences.IDEmployee = dbo.DailyCauses.IDEmployee and dbo.DailyIncidences.Date = dbo.DailyCauses.Date
LEFT OUTER JOIN dbo.sysroDailyIncidencesDescription did with (nolock)  on did.IDIncidence = dbo.DailyIncidences.IDType
LEFT OUTER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID
GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName,
dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt),
YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy, reqReg.dt),
dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate, dbo.DailyAccruals.Value,
dbo.Causes.Name, dbo.DailyCauses.IDCause, did.Description, DailySchedule.Date, Shifts.Name, dbo.Concepts.Export,
dbo.Causes.Export, dbo.Shifts.Export, dbo.DailyAccruals.ExpiredDate, dbo.DailyAccruals.BeginPeriod, dbo.DailyAccruals.EndPeriod,
StartEnjoymentDate, DefaultQuery
UNION
SELECT
emp.ID As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, NULL AS IDConcept,
dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, emp.Name AS EmployeeName,
'' AS ConceptName, ds.Date AS Date, ds.Date AS Date_ToDateString, 0 AS Value, 0 AS ValueHHMM_ToHours,COUNT(*) AS Count, MONTH(ds.Date) AS Mes,
YEAR(ds.Date) AS Año, (DATEPART(dw, ds.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek,
CASE WHEN DATEPART(iso_week, ds.Date) >= 52 AND MONTH(ds.Date) = 1 THEN (YEAR(ds.Date)-1) * 100 + DATEPART(iso_week, ds.Date)
ELSE (YEAR(ds.Date)) * 100 + DATEPART(iso_week, ds.Date) END AS WeekOfYear, DATEPART(dy, ds.Date) AS DayOfYear, DATEPART(QUARTER, ds.Date) AS Quarter, dbo.sysroEmployeeGroups.Path,
dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString,
dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS CauseValue, SUM(dbo.DailyCauses.Value) AS CauseValueHHMM,Shifts.Name AS ShiftName,
did.Description AS IncidenceName,
dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),ds.Date) As UserField1,
dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),ds.Date) As UserField2,
dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),ds.Date) As UserField3,
dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),ds.Date) As UserField4,
dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),ds.Date) As UserField5,
dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),ds.Date) As UserField6,
dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),ds.Date) As UserField7,
dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),ds.Date) As UserField8,
dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),ds.Date) As UserField9,
dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),ds.Date) As UserField10,
dbo.GetEmployeeAge(emp.ID) As Age     ,
dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,
NULL as AccrualEquivalence,
dbo.Causes.Export as IncidenceEquivalence,
dbo.Shifts.Export as ScheduleEquivalence,
0 AS PositiveValue, 0 AS PositiveValueHHMM_ToHours, NULL as ExpiredDate, null as ExpiredDate_ToDateString,
NULL as BeginAccrualPeriod_ToDateString, NULL as EndAccrualPeriod_ToDateString, NULL as BeginAccrualPeriod, NULL as EndAccrualPeriod,
NULL as ConceptExpirationDate, NULL as StartEnjoymentDate, NULL as ConceptExpirationDate_ToDateString, NULL as StartEnjoymentDate_ToDateString
FROM dbo.DailySchedule ds with (nolock)
INNER JOIN dbo.Employees emp on ds.IDEmployee = emp.ID
INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = emp.id AND ds.Date between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate
INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = emp.ID AND ds.Date between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
LEFT JOIN DBO.DailyCauses with (nolock) ON dbo.DailyCauses.IDEmployee = emp.id AND dbo.DailyCauses.Date = ds.Date AND (DailyCauses.IDCause NOT IN (SELECT IDCause FROM DBO.ConceptCauses))
AND DBO.DailyCauses.IDCause IN (SELECT * FROM @causesIDs)
INNER JOIN dbo.Causes with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause
INNER JOIN dbo.DailyIncidences with (nolock) ON dbo.DailyIncidences.id = dbo.DailyCauses.IDRelatedIncidence and dbo.DailyIncidences.IDEmployee = dbo.DailyCauses.IDEmployee and dbo.DailyIncidences.Date = dbo.DailyCauses.Date
INNER JOIN dbo.sysroDailyIncidencesDescription did with (nolock)  on did.IDIncidence = dbo.DailyIncidences.IDType
INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID
LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(ds.IDShiftUsed,ds.IDShift1)
LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID
WHERE ds.Date between @pinitialDate and @pendDate AND
(select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and ds.Date between BeginDate and EndDate and idpassport = @pidpassport and idemployee = emp.ID and FeaturePermission > 0) > 0
and emp.ID in (select idEmployee from @employeeIDs)
GROUP BY emp.ID, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract,
dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, emp.Name, ds.date, MONTH(ds.Date),
YEAR(ds.Date), (DATEPART(dw, ds.Date) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, ds.Date), DATEPART(dy,
ds.Date), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate,
dbo.Causes.Name, dbo.DailyCauses.IDCause, did.Description, ds.Date, Shifts.Name, dbo.Causes.Export, dbo.Shifts.Export
option (maxrecursion 0,USE HINT('QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150'))

GO

ALTER PROCEDURE [dbo].[Genius_ConceptsAndScheduleAndIncidencesAndPunches]
           @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint, @causesFilter nvarchar(max) AS 
   DECLARE @employeeIDs Table(idEmployee int) 
   DECLARE @conceptIDs Table(idConcept int) 
   DECLARE @causesIDs Table(idCause int) 
   DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @pcausesFilter nvarchar(max) = @causesFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros 
   DECLARE @intdatefirst int 
   SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS' 
   SET DateFirst @intdatefirst;   
   insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate    
   
	if (@causesFilter is NULL OR @causesFilter = '')
	begin
		insert into @causesIDs select ID from dbo.Causes
	end
	else
	begin
		insert into @causesIDs select * from dbo.SplitToInt(@pcausesFilter,',');   
	end
	
	insert into @conceptIDs select * from dbo.SplitToInt(@pconceptsFilter,',');   

   WITH alldays AS ( 
   SELECT @initialDate AS dt 
   UNION ALL 
   SELECT DATEADD(dd, 1, dt) 
   FROM alldays s 
   WHERE DATEADD(dd, 1, dt) <= @pendDate)
   SELECT  
   reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept,  
   dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName,  
   reqReg.conceptName AS ConceptName, reqReg.dt AS Date, reqReg.dt AS Date_ToDateString, 
   CASE WHEN dbo.Concepts.DefaultQuery <> 'L' THEN sum(isnull(dbo.DailyAccruals.Value,0))
	ELSE sum(dbo.DailyAccruals.Value) END
	AS Value, 
	CASE WHEN dbo.Concepts.DefaultQuery <> 'L' THEN sum(isnull(dbo.DailyAccruals.Value,0))
	ELSE sum(dbo.DailyAccruals.Value) END
	AS ValueHHMM_ToHours,
   COUNT(*) AS Count,  
   MONTH(reqReg.dt) AS Mes, YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 
   CASE WHEN DATEPART(iso_week, reqReg.dt) >= 52 AND MONTH(reqReg.dt) = 1 THEN (YEAR(reqReg.dt)-1) * 100 + DATEPART(iso_week, reqReg.dt)
   ELSE (YEAR(reqReg.dt)) * 100 + DATEPART(iso_week, reqReg.dt) END AS WeekOfYear, 
   DATEPART(dy, reqReg.dt) AS DayOfYear, DATEPART(QUARTER, reqReg.dt) AS Quarter,  
   dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, 
   dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString, 
   dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS CauseValue, SUM(dbo.DailyCauses.Value) AS CauseValueHHMM,Shifts.Name AS ShiftName, did.Description AS IncidenceName, 
   dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date) as Punches,
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
   dbo.GetEmployeeAge(reqReg.idEmployee) As Age     ,
   dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,
   dbo.Concepts.Export as AccrualEquivalence,
   dbo.Causes.Export as IncidenceEquivalence,
   dbo.Shifts.Export as ScheduleEquivalence,
    CASE WHEN dbo.Concepts.DefaultQuery <> 'L' THEN sum(isnull(dbo.DailyAccruals.PositiveValue,0))
ELSE sum(dbo.DailyAccruals.PositiveValue) END
AS PositiveValue, 
CASE WHEN dbo.Concepts.DefaultQuery <> 'L' THEN sum(isnull(dbo.DailyAccruals.PositiveValue,0))
ELSE sum(dbo.DailyAccruals.PositiveValue) END
AS PositiveValueHHMM_ToHours,
	ExpiredDate, ExpiredDate as ExpiredDate_ToDateString,
   (select count(*) from punches where IDEmployee = reqReg.idEmployee and ShiftDate = reqReg.dt and ActualType = 1) as InPunches,
   (select count(*) from punches where IDEmployee = reqReg.idEmployee and ShiftDate = reqReg.dt and ActualType = 2) as OutPunches,
   dbo.DailyAccruals.BeginPeriod as BeginAccrualPeriod_ToDateString, dbo.DailyAccruals.EndPeriod as EndAccrualPeriod_ToDateString, dbo.DailyAccruals.BeginPeriod as BeginAccrualPeriod, dbo.DailyAccruals.EndPeriod as EndAccrualPeriod,
   dbo.DailyAccruals.ExpiredDate as ConceptExpirationDate, dbo.DailyAccruals.StartEnjoymentDate as StartEnjoymentDate, dbo.DailyAccruals.ExpiredDate as ConceptExpirationDate_ToDateString, dbo.DailyAccruals.StartEnjoymentDate as StartEnjoymentDate_ToDateString
   FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,  
   (select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp, 
   (select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con
   where (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and alldays.dt between BeginDate and EndDate and idpassport = @pidpassport and idemployee = emp.ID and FeaturePermission > 0) > 0
   ) reqReg 
   INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate 
   INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
   INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailySchedule.IDEmployee = reqReg.idEmployee and dbo.DailySchedule.Date = reqReg.dt 
   INNER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee    
   INNER JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(dbo.DailySchedule.IDShiftUsed,dbo.DailySchedule.IDShift1) 
   LEFT OUTER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept 
   LEFT OUTER JOIN dbo.Concepts with (nolock) ON dbo.Concepts.ID = reqReg.IDConcept    
   LEFT JOIN DBO.DailyCauses with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date                                                                          AND (DailyCauses.IDCause in ( select IdCause FROM DBO.ConceptCauses with (nolock) where IDConcept = dbo.DailyAccruals.IDConcept) ) AND DBO.DailyCauses.IDCause IN (SELECT * FROM @causesIDs)
   LEFT JOIN dbo.Causes with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause
   LEFT OUTER JOIN dbo.DailyIncidences with (nolock) ON dbo.DailyIncidences.id = dbo.DailyCauses.IDRelatedIncidence and dbo.DailyIncidences.IDEmployee = dbo.DailyCauses.IDEmployee and dbo.DailyIncidences.Date = dbo.DailyCauses.Date 
   LEFT OUTER JOIN dbo.sysroDailyIncidencesDescription did with (nolock)  on did.IDIncidence = dbo.DailyIncidences.IDType 
   LEFT OUTER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID        
   GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName, 
   dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt), 
   YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy, reqReg.dt),
   dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
   dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate, dbo.DailyAccruals.Value,
   dbo.Causes.Name, dbo.DailyCauses.IDCause, did.Description, DailySchedule.Date, Shifts.Name,
   dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date), dbo.Concepts.Export, dbo.Causes.Export, dbo.Shifts.Export,
     dbo.DailyAccruals.ExpiredDate, dbo.DailyAccruals.BeginPeriod, dbo.DailyAccruals.EndPeriod, StartEnjoymentDate, DefaultQuery
   UNION
   SELECT 
   ds.IDEmployee As IDEmployee
   , dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, NULL AS IDConcept, 
   dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, emp.Name AS EmployeeName, 
   '' AS ConceptName, ds.Date AS Date, ds.Date AS Date_ToDateString, 0 AS Value, 0 AS ValueHHMM_ToHours,COUNT(*) AS Count, MONTH(ds.Date) AS Mes, 
   YEAR(ds.Date) AS Año, (DATEPART(dw, ds.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 
   CASE WHEN DATEPART(iso_week, ds.Date) >= 52 AND MONTH(ds.Date) = 1 THEN (YEAR(ds.Date)-1) * 100 + DATEPART(iso_week, ds.Date)
   ELSE (YEAR(ds.Date)) * 100 + DATEPART(iso_week, ds.Date) END AS WeekOfYear, 
   DATEPART(dy, ds.Date) AS DayOfYear, DATEPART(QUARTER, ds.Date) AS Quarter, dbo.sysroEmployeeGroups.Path, 
   dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString,
   dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
   dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS CauseValue, SUM(dbo.DailyCauses.Value) AS CauseValueHHMM,Shifts.Name AS ShiftName,
   did.Description AS IncidenceName,
   dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,ds.Date) as Punches,
   dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
   dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
   dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
   dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
   dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
   dbo.GetEmployeeUserFieldValueMin(ds.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),ds.Date) As UserField1,
   dbo.GetEmployeeUserFieldValueMin(ds.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),ds.Date) As UserField2,
   dbo.GetEmployeeUserFieldValueMin(ds.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),ds.Date) As UserField3,
   dbo.GetEmployeeUserFieldValueMin(ds.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),ds.Date) As UserField4,
   dbo.GetEmployeeUserFieldValueMin(ds.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),ds.Date) As UserField5,
   dbo.GetEmployeeUserFieldValueMin(ds.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),ds.Date) As UserField6,
   dbo.GetEmployeeUserFieldValueMin(ds.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),ds.Date) As UserField7,
   dbo.GetEmployeeUserFieldValueMin(ds.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),ds.Date) As UserField8,
   dbo.GetEmployeeUserFieldValueMin(ds.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),ds.Date) As UserField9,
   dbo.GetEmployeeUserFieldValueMin(ds.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),ds.Date) As UserField10,
   dbo.GetEmployeeAge(ds.IDEmployee) As Age     ,
   dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,
   NULL as AccrualEquivalence,
   dbo.Causes.Export as IncidenceEquivalence,
   dbo.Shifts.Export as ScheduleEquivalence,
   0 AS PositiveValue, 0 AS PositiveValueHHMM_ToHours, NULL, NULL as ExpiredDate_ToDateString,
   (select count(*) from punches where IDEmployee = ds.IDEmployee and ShiftDate = ds.Date and ActualType = 1) as InPunches,
   (select count(*) from punches where IDEmployee = ds.IDEmployee and ShiftDate = ds.Date and ActualType = 2) as OutPunches,
NULL as BeginAccrualPeriod_ToDateString, NULL as EndAccrualPeriod_ToDateString, NULL as BeginAccrualPeriod, NULL as EndAccrualPeriod,
				NULL as ConceptExpirationDate, NULL as StartEnjoymentDate, NULL as ConceptExpirationDate_ToDateString, NULL as StartEnjoymentDate_ToDateString
                FROM dbo.DailySchedule ds with (nolock)
				inner JOIN DBO.DailyCauses with (nolock) ON dbo.DailyCauses.IDEmployee = ds.IDEmployee AND dbo.DailyCauses.Date = ds.Date AND (DailyCauses.IDCause NOT IN (SELECT IDCause FROM DBO.ConceptCauses)) AND DBO.DailyCauses.IDCause IN (SELECT * FROM @causesIDs)
				inner JOIN dbo.DailyIncidences with (nolock) ON dbo.DailyIncidences.id = dbo.DailyCauses.IDRelatedIncidence and dbo.DailyIncidences.IDEmployee = dbo.DailyCauses.IDEmployee and dbo.DailyIncidences.Date = dbo.DailyCauses.Date
                                             INNER JOIN dbo.Employees emp on ds.IDEmployee = emp.ID
             INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = emp.id
             INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = emp.ID AND ds.Date between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate			 
			 LEFT JOIN dbo.Causes with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause                                            
                                             INNER JOIN dbo.sysroDailyIncidencesDescription did with (nolock)  on did.IDIncidence = dbo.DailyIncidences.IDType
                                             INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID
                                             LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(ds.IDShiftUsed,ds.IDShift1)
                                             LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID			 
                WHERE ds.Date between @pinitialDate and @pendDate 
				AND (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and ds.Date between BeginDate and EndDate and idpassport = @pidpassport and idemployee = ds.IDEmployee and FeaturePermission > 0) > 0
 			   and ds.IDEmployee in (select idEmployee from @employeeIDs)
                 GROUP BY ds.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract,
                              dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, emp.Name, ds.date, MONTH(ds.Date), 
                               YEAR(ds.Date), (DATEPART(dw, ds.Date) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, ds.Date), DATEPART(dy, 
                               ds.Date), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
                               dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate,
                               dbo.Causes.Name, dbo.DailyCauses.IDCause, did.Description, ds.Date, Shifts.Name,
 							  dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,ds.Date), dbo.Causes.Export, dbo.Shifts.Export
                option (maxrecursion 0)

GO

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
  DECLARE @annualConceptNamesWithZero NVARCHAR(MAX)
  DECLARE @annualConceptNamesWithZeroToHours NVARCHAR(MAX)
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
 	(select convert(nvarchar(max), 
	(SELECT ',ISNULL([' + ST1.name +'],0) AS [' + ST1.name +']' 
	AS [data()]
                 FROM dbo.concepts ST1 WITH(NOLOCK)
 				where ST1.DefaultQuery <> 'L' AND st1.ID in(select Value from dbo.SplitToInt(@pconceptsFilter,','))
                 FOR XML PATH(''))) as conceptNames) tmp

	select @annualConceptNamesWithZero = substring(tmp.conceptNames,2, LEN(tmp.conceptNames) -1) from 
 	(select convert(nvarchar(max), 
	(SELECT ',[' + ST1.name +'] AS [' + ST1.name +']' 
	AS [data()]
                 FROM dbo.concepts ST1 WITH(NOLOCK)
 				where ST1.DefaultQuery = 'L' AND st1.ID in(select Value from dbo.SplitToInt(@pconceptsFilter,','))
                 FOR XML PATH(''))) as conceptNames) tmp
				 

   select @conceptNamesToHours = substring(tmp.conceptNames,2, LEN(tmp.conceptNames) -1) from 
 	(select convert(nvarchar(max), (SELECT ',[' + ST1.name +'] as [' + st1.name + '(HH:MM)_ToHours]' AS [data()]
                 FROM dbo.concepts ST1 WITH(NOLOCK)
 				where st1.ID in(select Value from dbo.SplitToInt(@pconceptsFilter,','))
                 FOR XML PATH(''))) as conceptNames) tmp

	select @conceptNamesWithZeroToHours = substring(tmp.conceptNames,2, LEN(tmp.conceptNames) -1) from 
 	(select convert(nvarchar(max), (SELECT ',ISNULL([' + ST1.name +'],0) as [' + st1.name + '(HH:MM)_ToHours]' AS [data()]
                 FROM dbo.concepts ST1 WITH(NOLOCK)
 				where st1.DefaultQuery <> 'L' AND st1.ID in(select Value from dbo.SplitToInt(@pconceptsFilter,','))
                 FOR XML PATH(''))) as conceptNames) tmp

	 select @annualConceptNamesWithZeroToHours = substring(tmp.conceptNames,2, LEN(tmp.conceptNames) -1) from 
 	(select convert(nvarchar(max), (SELECT ',[' + ST1.name +'] as [' + st1.name + '(HH:MM)_ToHours]' AS [data()]
                 FROM dbo.concepts ST1 WITH(NOLOCK)
 				where st1.DefaultQuery = 'L' AND st1.ID in(select Value from dbo.SplitToInt(@pconceptsFilter,','))
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
 		' + @conceptNamesWithZero + ',' + @conceptNamesWithZeroToHours + ',' + @annualConceptNamesWithZero + ',' + @annualConceptNamesWithZero + ', dbo.GetPunchesByEmployeeAndDate(emp.ID,accruals.Date) as punches,
		dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,
		(select count(*) from punches where IDEmployee = emp.ID and ShiftDate = accruals.Date and ActualType = 1) as InPunches,
		(select count(*) from punches where IDEmployee = emp.ID and ShiftDate = accruals.Date and ActualType = 2) as OutPunches,
		accruals.BeginPeriod as BeginAccrualPeriod_ToDateString, accruals.EndPeriod as EndAccrualPeriod_ToDateString, accruals.BeginPeriod as BeginAccrualPeriod, accruals.EndPeriod as EndAccrualPeriod,
		accruals.ExpiredDate as ConceptExpirationDate, accruals.StartEnjoymentDate as StartEnjoymentDate, accruals.ExpiredDate as ConceptExpirationDate_ToDateString, accruals.StartEnjoymentDate as StartEnjoymentDate_ToDateString
 	FROM(
     select alldays.dt as Date, employees.ID as IDEmployee, BeginPeriod, EndPeriod, StartEnjoymentDate, ExpiredDate, '+ @conceptNames +' from alldays
 	cross join Employees
 	left outer join (SELECT *
       FROM
          (SELECT Concepts.name AS ConceptName, Value, IDEmployee, Date, BeginPeriod, EndPeriod, ExpiredDate, StartEnjoymentDate from DailyAccruals WITH(NOLOCK) inner join Concepts WITH(NOLOCK) on dailyaccruals.IDConcept = Concepts.ID
 		  WHERE Date between ''' + convert(nvarchar(max),@pinitialDate,112) + ''' and convert(smalldatetime,''' + convert(nvarchar(max),@pendDate,112) + ''',120) 
 		 ) I
          PIVOT( Sum(Value) FOR ConceptName in ('+ @conceptNames +') ) P) tmp
 		 on alldays.dt = tmp.Date and tmp.IDEmployee = employees.ID
 	 WHERE employees.ID IN(' + @employeeIDFilter + ') 
	 and (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and alldays.dt between pef.BeginDate and pef.EndDate and idpassport = ' + CAST(@pidpassport as nvarchar(100)) + ' and pef.idemployee = employees.ID and FeaturePermission > 0) > 0
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

ALTER PROCEDURE [dbo].[GetPivotAccruals]
   @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max) 
 AS
 BEGIN
  DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter  
  DECLARE @employeeIDs Table(idEmployee int)
  DECLARE @conceptNames NVARCHAR(MAX)
  DECLARE @conceptNamesToHours NVARCHAR(MAX)
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
				 select @conceptNamesToHours = substring(tmp.conceptNames,2, LEN(tmp.conceptNames) -1) from 
 	(select convert(nvarchar(max), (SELECT ',[' + ST1.name +'] as [' + st1.name + '(HH:MM)_ToHours]' AS [data()]
                 FROM dbo.concepts ST1 WITH(NOLOCK)
 				where st1.ID in(select Value from dbo.SplitToInt(@pconceptsFilter,','))
                 FOR XML PATH(''))) as conceptNames) tmp
   DECLARE @SqlStatement NVARCHAR(MAX)
   SET @SqlStatement = N'
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
 		' + @conceptNames + ',' + @conceptNamesToHours + ', dbo.GetPunchesByEmployeeAndDate(emp.ID,accruals.Date) as punches,
		dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,
		(select count(*) from punches where IDEmployee = emp.ID and ShiftDate = accruals.Date and ActualType = 1) as InPunches,
		(select count(*) from punches where IDEmployee = emp.ID and ShiftDate = accruals.Date and ActualType = 2) as OutPunches,
		accruals.BeginPeriod as BeginAccrualPeriod_ToDateString, accruals.EndPeriod as EndAccrualPeriod_ToDateString, accruals.BeginPeriod as BeginAccrualPeriod, accruals.EndPeriod as EndAccrualPeriod,
		accruals.ExpiredDate as ConceptExpirationDate, accruals.StartEnjoymentDate as StartEnjoymentDate, accruals.ExpiredDate as ConceptExpirationDate_ToDateString, accruals.StartEnjoymentDate as StartEnjoymentDate_ToDateString
 	from (SELECT * FROM
          (SELECT Concepts.name AS ConceptName, Value, IDEmployee, Date, BeginPeriod, EndPeriod, ExpiredDate, StartEnjoymentDate FROM DailyAccruals WITH(NOLOCK) inner join Concepts WITH(NOLOCK) on dailyaccruals.IDConcept = Concepts.ID
 		  WHERE Date between ''' + convert(nvarchar(max),@pinitialDate,112) + ''' and ''' + convert(nvarchar(max),@pendDate,112) + ''' 
 		  and IDEmployee IN(' + @employeeIDFilter + ') 		  
		  and (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and Date between pef.BeginDate and pef.EndDate and idpassport = ' + CAST(@pidpassport as nvarchar(100)) + ' and pef.idemployee = DailyAccruals.IDEmployee and FeaturePermission > 0) > 0
 		 ) I
          PIVOT( Sum(Value) FOR ConceptName in ('+ @conceptNames +') ) P) accruals
 	INNER JOIN dbo.Employees emp with (nolock) ON emp.ID = accruals.IDEmployee 
 	INNER JOIN dbo.sysroEmployeeGroups eg with (nolock) ON eg.IDEmployee = accruals.IDEmployee AND accruals.Date between eg.BeginDate and eg.EndDate  
  	INNER JOIN dbo.EmployeeContracts ec with (nolock) ON ec.IDEmployee = accruals.IDEmployee AND accruals.Date between ec.BeginDate and ec.EndDate  
	LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID	
    ORDER BY accruals.IDEmployee,accruals.Date ASC';
   EXEC(@SqlStatement)
 END

GO



-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='865' WHERE ID='DBVersion'

GO
