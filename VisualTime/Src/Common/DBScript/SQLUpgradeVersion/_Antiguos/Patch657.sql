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
   reqReg.conceptName AS ConceptName, reqReg.dt AS Date, reqReg.dt AS Date_ToDateString, isnull(dbo.DailyAccruals.Value,0) AS Value, isnull(dbo.DailyAccruals.Value,0) AS ValueHHMM_ToHours,COUNT(*) AS Count,  
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
   sum(isnull(dbo.DailyAccruals.PositiveValue,0)) AS PositiveValue, sum(isnull(dbo.DailyAccruals.PositiveValue,0)) AS PositiveValueHHMM_ToHours, ExpiredDate, ExpiredDate as ExpiredDate_ToDateString
   FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,  
   (select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp, 
   (select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con
   where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1
   ) reqReg 
   INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate 
   INNER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee 
   INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
   INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailySchedule.IDEmployee = reqReg.idEmployee and dbo.DailySchedule.Date = reqReg.dt 
   INNER JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(dbo.DailySchedule.IDShiftUsed,dbo.DailySchedule.IDShift1) 
   LEFT OUTER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept 
   LEFT OUTER JOIN dbo.Concepts with (nolock) ON dbo.Concepts.ID = dbo.DailyAccruals.IDConcept 
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
   dbo.Causes.Export, dbo.Shifts.Export, dbo.DailyAccruals.ExpiredDate
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
   0 AS PositiveValue, 0 AS PositiveValueHHMM_ToHours, NULL as ExpiredDate, null as ExpiredDate_ToDateString
                FROM dbo.DailySchedule ds with (nolock)
                                             INNER JOIN dbo.Employees emp on ds.IDEmployee = emp.ID
             INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = emp.id
             INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = emp.ID AND ds.Date between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
                                             LEFT JOIN DBO.DailyCauses with (nolock) ON dbo.DailyCauses.IDEmployee = emp.id AND dbo.DailyCauses.Date = ds.Date AND (DailyCauses.IDCause NOT IN (SELECT IDCause FROM DBO.ConceptCauses)) 
											 AND DBO.DailyCauses.IDCause IN (SELECT * FROM @causesIDs)
                                             INNER JOIN dbo.Causes with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause
                                             INNER JOIN dbo.DailyIncidences with (nolock) ON dbo.DailyIncidences.id = dbo.DailyCauses.IDRelatedIncidence and dbo.DailyIncidences.IDEmployee = dbo.DailyCauses.IDEmployee and dbo.DailyIncidences.Date = dbo.DailyCauses.Date
                                             INNER JOIN dbo.sysroDailyIncidencesDescription did with (nolock)  on did.IDIncidence = dbo.DailyIncidences.IDType
                                             INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID
                                             LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(ds.IDShiftUsed,ds.IDShift1)
                                             LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID			
                WHERE ds.Date between @pinitialDate and @pendDate AND dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,ds.Date) > 1
 			   and emp.ID in (select idEmployee from @employeeIDs)
                 GROUP BY emp.ID, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract,
                              dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, emp.Name, ds.date, MONTH(ds.Date), 
                               YEAR(ds.Date), (DATEPART(dw, ds.Date) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, ds.Date), DATEPART(dy, 
                               ds.Date), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
                               dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate,
                               dbo.Causes.Name, dbo.DailyCauses.IDCause, did.Description, ds.Date, Shifts.Name, dbo.Causes.Export, dbo.Shifts.Export
                option (maxrecursion 0)

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
   reqReg.conceptName AS ConceptName, reqReg.dt AS Date, reqReg.dt AS Date_ToDateString, isnull(dbo.DailyAccruals.Value,0) AS Value, isnull(dbo.DailyAccruals.Value,0) AS ValueHHMM_ToHours,COUNT(*) AS Count,  
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
    sum(isnull(dbo.DailyAccruals.PositiveValue,0)) AS PositiveValue, sum(isnull(dbo.DailyAccruals.PositiveValue,0)) AS PositiveValueHHMM_ToHours, ExpiredDate, ExpiredDate as ExpiredDate_ToDateString,
   (select count(*) from punches where IDEmployee = reqReg.idEmployee and ShiftDate = reqReg.dt and ActualType = 1) as InPunches,
   (select count(*) from punches where IDEmployee = reqReg.idEmployee and ShiftDate = reqReg.dt and ActualType = 2) as OutPunches
   FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,  
   (select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp, 
   (select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con
   where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1
   ) reqReg 
   INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate 
   INNER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee 
   INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
   INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailySchedule.IDEmployee = reqReg.idEmployee and dbo.DailySchedule.Date = reqReg.dt 
   INNER JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(dbo.DailySchedule.IDShiftUsed,dbo.DailySchedule.IDShift1) 
   LEFT OUTER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept 
   LEFT OUTER JOIN dbo.Concepts with (nolock) ON dbo.Concepts.ID = dbo.DailyAccruals.IDConcept 
   INNER JOIN DBO.DailyCauses with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date                                                                          AND (DailyCauses.IDCause in ( select IdCause FROM DBO.ConceptCauses with (nolock) where IDConcept = dbo.DailyAccruals.IDConcept) ) AND DBO.DailyCauses.IDCause IN (SELECT * FROM @causesIDs)
   INNER JOIN dbo.Causes with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause
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
     dbo.DailyAccruals.ExpiredDate	 
   UNION
   SELECT 
   emp.ID As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, NULL AS IDConcept, 
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
   0 AS PositiveValue, 0 AS PositiveValueHHMM_ToHours, NULL, NULL as ExpiredDate_ToDateString,
   (select count(*) from punches where IDEmployee = emp.ID and ShiftDate = ds.Date and ActualType = 1) as InPunches,
   (select count(*) from punches where IDEmployee = emp.ID and ShiftDate = ds.Date and ActualType = 2) as OutPunches
                FROM dbo.DailySchedule ds with (nolock)
                                             INNER JOIN dbo.Employees emp on ds.IDEmployee = emp.ID
             INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = emp.id
             INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = emp.ID AND ds.Date between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
                                             INNER JOIN DBO.DailyCauses with (nolock) ON dbo.DailyCauses.IDEmployee = emp.id AND dbo.DailyCauses.Date = ds.Date AND (DailyCauses.IDCause NOT IN (SELECT IDCause FROM DBO.ConceptCauses)) AND DBO.DailyCauses.IDCause IN (SELECT * FROM @causesIDs)
                                             INNER JOIN dbo.Causes with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause
                                             INNER JOIN dbo.DailyIncidences with (nolock) ON dbo.DailyIncidences.id = dbo.DailyCauses.IDRelatedIncidence and dbo.DailyIncidences.IDEmployee = dbo.DailyCauses.IDEmployee and dbo.DailyIncidences.Date = dbo.DailyCauses.Date
                                             INNER JOIN dbo.sysroDailyIncidencesDescription did with (nolock)  on did.IDIncidence = dbo.DailyIncidences.IDType
                                             INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID
                                             LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(ds.IDShiftUsed,ds.IDShift1)
                                             LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID
                WHERE ds.Date between @pinitialDate and @pendDate AND dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,ds.Date) > 1
 			   and emp.ID in (select idEmployee from @employeeIDs)
                 GROUP BY emp.ID, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract,
                              dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, emp.Name, ds.date, MONTH(ds.Date), 
                               YEAR(ds.Date), (DATEPART(dw, ds.Date) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, ds.Date), DATEPART(dy, 
                               ds.Date), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
                               dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate,
                               dbo.Causes.Name, dbo.DailyCauses.IDCause, did.Description, ds.Date, Shifts.Name,
 							  dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,ds.Date), dbo.Causes.Export, dbo.Shifts.Export							  
                option (maxrecursion 0)

GO

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
					 dbo.Causes.Export as IncidenceEquivalence
            FROM		 dbo.sysroEmployeeGroups with (nolock)   
                           INNER JOIN dbo.Causes with (nolock)   
                           INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause  ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.DailyCauses.Date between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate   AND DBO.DailyCauses.IDCause IN (SELECT * FROM @causesIDs)     
   						INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyCauses.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyCauses.Date <= dbo.EmployeeContracts.EndDate   
   						INNER JOIN dbo.Groups with (nolock) ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID   
   						LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.Employees.ID    
   						LEFT OUTER JOIN dbo.BusinessCenters with (nolock) ON ISNULL(dbo.DailyCauses.IDCenter,0) = dbo.BusinessCenters.ID   
 						LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
             where dbo.dailycauses.date between @pinitialDate and @pendDate 
   					AND isnull(dbo.DailyCauses.IDCenter,0) in (select idBusinessCenter from @businessCenterIDs)   
   					AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs)
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
					 dbo.Causes.Export as IncidenceEquivalence
            FROM 
			(select alldays.dt,emp.ID as idEmployee, emp.Name as employeeName, CAU.ID AS IdCause from alldays,  
   (select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp   ,
   (select ID, Name from Causes with(nolock)) cau
   where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1
   ) reqReg 
   INNER JOIN dbo.sysroEmployeeGroups ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate 			
                         LEFT JOIN dbo.Causes ON dbo.Causes.ID = reqReg.IdCause
						   LEFT JOIN dbo.DailyCauses ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.DailyCauses.Date = reqReg.dt and dbo.DailyCauses.IDCause = reqReg.IdCause
   						INNER JOIN dbo.EmployeeContracts  ON reqReg.IDEmployee = dbo.EmployeeContracts.IDEmployee AND reqReg.dt >= dbo.EmployeeContracts.BeginDate AND reqReg.dt <= dbo.EmployeeContracts.EndDate   
   						INNER JOIN dbo.Groups  ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID   
   						LEFT OUTER JOIN dbo.Employees  ON reqReg.idEmployee = dbo.Employees.ID    
   						LEFT OUTER JOIN dbo.BusinessCenters  ON ISNULL(dbo.DailyCauses.IDCenter,0) = dbo.BusinessCenters.ID   
 						LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
             where reqReg.dt between @pinitialDate and @pendDate 
   					AND isnull(dbo.DailyCauses.IDCenter,0) in (select idBusinessCenter from @businessCenterIDs)   
   					AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs)
		END

GO

ALTER PROCEDURE [dbo].[Genius_Incidences]
            @initialDate smalldatetime,    
            @endDate smalldatetime,    
            @idpassport int,    
            @employeeFilter nvarchar(max),    
            @userFieldsFilter nvarchar(max),
			@causesFilter nvarchar(max)
            AS    
           DECLARE @employeeIDs Table(idEmployee int)        
         DECLARE @pinitialDate smalldatetime = @initialDate,        
            @pendDate smalldatetime = @endDate,        
            @pidpassport int = @idpassport,        
            @pemployeeFilter nvarchar(max) = @employeeFilter,        
            @puserFieldsFilter nvarchar(max) = @userFieldsFilter      ,
			@pcausesFilter nvarchar(max) = @causesFilter
			DECLARE @causesIDs Table(idCause int) 
      DECLARE @intdatefirst int  
      SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'  
      SET DateFirst @intdatefirst;  
      insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;       
	  if (@causesFilter is NULL OR @causesFilter = '')
	begin
		insert into @causesIDs select ID from dbo.Causes
	end
	else
	begin
		insert into @causesIDs select * from dbo.SplitToInt(@pcausesFilter,',');   
	end
            SELECT     CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)     
                                  + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup,     
                                  dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.DailySchedule.Date AS Date_ToDateString, dbo.sysroDailyIncidencesDescription.Description AS IncidenceName,     
                                  dbo.TimeZones.Name AS ZoneTime, dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS Value, SUM(dbo.DailyCauses.Value) AS ValueHHMM_ToHours, MONTH(dbo.DailySchedule.Date) AS Mes, YEAR(dbo.DailySchedule.Date) AS Año,     
                                  (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 
								  CASE WHEN DATEPART(iso_week, dbo.DailySchedule.Date) >= 52 AND MONTH(dbo.DailySchedule.Date) = 1 THEN (YEAR(dbo.DailySchedule.Date)-1) * 100 + DATEPART(iso_week, dbo.DailySchedule.Date)
								  ELSE (YEAR(dbo.DailySchedule.Date)) * 100 + DATEPART(iso_week, dbo.DailySchedule.Date) END AS WeekOfYear, 
								  DATEPART(dy, dbo.DailySchedule.Date) AS DayOfYear, 
                                  DATEPART(QUARTER, dbo.DailySchedule.Date) as Quarter, CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName,     
                                  dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.FullGroupName AS FullGroupName, dbo.sysroEmployeeGroups.Path,     
                                  dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,    
                 dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString, dbo.DailySchedule.IDAssignment,    
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
  			   dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age, convert(nvarchar(max),dbo.sysroRemarks.Text) as Remark,
 			   dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,
 			   dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date) as Punches,			   
   dbo.Causes.Export as IncidenceEquivalence,
   dbo.Shifts.Export as ScheduleEquivalence
            FROM         dbo.sysroEmployeeGroups with (nolock)    
                INNER JOIN dbo.Causes with (nolock)    
                INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause AND DBO.DailyCauses.IDCause IN (SELECT * FROM @causesIDs)     
                INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailySchedule.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate    
                INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailySchedule.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailySchedule.Date between dbo.EmployeeContracts.BeginDate AND dbo.EmployeeContracts.EndDate     
                LEFT OUTER JOIN dbo.sysroDailyIncidencesDescription with (nolock)    
                INNER JOIN dbo.DailyIncidences with (nolock)    
                INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID ON dbo.sysroDailyIncidencesDescription.IDIncidence = dbo.DailyIncidences.IDType ON dbo.DailyCauses.IDEmployee = dbo.DailyIncidences.IDEmployee AND dbo.DailyCauses.Date = dbo.DailyIncidences.Date AND dbo.DailyCauses.IDRelatedIncidence = dbo.DailyIncidences.ID AND dbo.DailyIncidences.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate    
                LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.DailySchedule.IDShiftUsed = dbo.Shifts.ID     
                LEFT OUTER JOIN dbo.Shifts AS Shifts_1 with (nolock) ON dbo.DailySchedule.IDShift1 = Shifts_1.ID    
                LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID    
             LEFT OUTER JOIN dbo.sysroRemarks on dbo.sysroRemarks.ID = dbo.DailySchedule.Remarks    
 			LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
           WHERE dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.DailyCauses.IDEmployee,2,0,0,dbo.DailyCauses.date) > 1    
              AND dbo.sysroEmployeeGroups.IDEmployee in(select idEmployee from @employeeIDs)     
             and dbo.dailyCauses.Date between @pinitialDate and @pendDate    
            GROUP BY dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name,     
                                  YEAR(dbo.DailySchedule.Date), CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)     
                      + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120), CASE WHEN DailySchedule.Date <= GETDATE()     
                                  THEN Shifts.Name ELSE Shifts_1.Name END, dbo.Employees.Name, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, DATEPART(iso_week,     
                                  dbo.DailySchedule.Date), DATEPART(dy, dbo.DailySchedule.Date), (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1,     
                                  dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,     
                                  dbo.sysroEmployeeGroups.CurrentEmployee, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate, dbo.DailySchedule.IDAssignment, dbo.DailySchedule.IDDailyBudgetPosition, convert(nvarchar(max),dbo.sysroRemarks.Text),
 								 dbo.sysrovwEmployeeLockDate.LockDate,
 								 dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date),
								 dbo.Causes.Export, dbo.Shifts.Export

GO

ALTER PROCEDURE [dbo].[Genius_IncidencesByTime]
            @initialDate smalldatetime,    
            @endDate smalldatetime,    
            @idpassport int,    
            @employeeFilter nvarchar(max),    
            @userFieldsFilter nvarchar(max),
 		   @initialTime smalldatetime,
 		   @endTime smalldatetime,
		   @causesFilter nvarchar(max)
            AS    
 		   
           DECLARE @employeeIDs Table(idEmployee int)        
         DECLARE @pinitialDate smalldatetime = (select convert(date, @initialDate)),        
            @pendDate smalldatetime = (select convert(date, @endDate)), 		   
            @pidpassport int = @idpassport,                        
            @puserFieldsFilter nvarchar(max) = @userFieldsFilter,
 		   @pemployeeFilter nvarchar(max) = @employeeFilter,
		   @pcausesFilter nvarchar(max) = @causesFilter
		   DECLARE @causesIDs Table(idCause int) 
      DECLARE @intdatefirst int  
      SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'  
      SET DateFirst @intdatefirst; 
 	 
 	 insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;       
 	 if (@causesFilter is NULL OR @causesFilter = '')
	begin
		insert into @causesIDs select ID from dbo.Causes
	end
	else
	begin
		insert into @causesIDs select * from dbo.SplitToInt(@pcausesFilter,',');   
	end
 	 IF (@endTime > @initialTime)
 	 BEGIN
 		 SELECT *, 
 		 CASE WHEN BeginJustificationTime < EndJustificationTime THEN 
 		 DATEDIFF(SECOND, BeginJustificationTime, EndJustificationTime)*1.0/3600 	 
 		 ELSE (DATEDIFF(SECOND, BeginJustificationTime, CAST('23:59:59' AS TIME))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndJustificationTime)*1.0/3600 
 		 END AS TotalJustificationHHMM_ToHours, 
 		 CASE WHEN BeginTimeInFilter <= EndTimeInFilter THEN DATEDIFF(SECOND, BeginTimeInFilter, EndTimeInFilter)*1.0/3600 
 		 ELSE (DATEDIFF(SECOND, BeginTimeInFilter, (CAST('23:59:59' AS TIME)))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndTimeInFilter)*1.0/3600 
 		 END AS TotalTimeInFilterHHMM_ToHours,
 		 CASE WHEN BeginJustificationTime < EndJustificationTime THEN 
 		 DATEDIFF(SECOND, BeginJustificationTime, EndJustificationTime)*1.0/3600 	 
 		 ELSE (DATEDIFF(SECOND, BeginJustificationTime, CAST('23:59:59' AS TIME))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndJustificationTime)*1.0/3600 
 		 END AS TotalJustification, 
 		 CASE WHEN BeginTimeInFilter <= EndTimeInFilter THEN DATEDIFF(SECOND, BeginTimeInFilter, EndTimeInFilter)*1.0/3600 
 		 ELSE (DATEDIFF(SECOND, BeginTimeInFilter, (CAST('23:59:59' AS TIME)))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndTimeInFilter)*1.0/3600 
 		 END AS TotalTimeInFilter
 		 FROM
 			   (SELECT     
 			   CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)     
 									 + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup,     
 									 dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.DailySchedule.Date AS Date_ToDateString, dbo.sysroDailyIncidencesDescription.Description AS IncidenceName,     
 									 dbo.TimeZones.Name AS ZoneTime, dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS Value, SUM(dbo.DailyCauses.Value) AS ValueHHMM_ToHours, MONTH(dbo.DailySchedule.Date) AS Mes, YEAR(dbo.DailySchedule.Date) AS Año,     
 									 (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 
									 CASE WHEN DATEPART(iso_week, dbo.DailySchedule.Date) >= 52 AND MONTH(dbo.DailySchedule.Date) = 1 THEN (YEAR(dbo.DailySchedule.Date)-1) * 100 + DATEPART(iso_week, dbo.DailySchedule.Date)
									 ELSE (YEAR(dbo.DailySchedule.Date)) * 100 + DATEPART(iso_week, dbo.DailySchedule.Date) END AS WeekOfYear, 
									 DATEPART(dy, dbo.DailySchedule.Date) AS DayOfYear, 
 									 DATEPART(QUARTER, dbo.DailySchedule.Date) as Quarter, CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName,     
 									 dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.FullGroupName AS FullGroupName, dbo.sysroEmployeeGroups.Path,     
 									 dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,    
 					dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString, dbo.DailySchedule.IDAssignment,    
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
  				   dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age, convert(nvarchar(max),dbo.sysroRemarks.Text) as Remark,
 				   dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,
 				   dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date) as Punches,
 				   CAST (dbo.dailyincidences.begintime AS TIME) AS BeginJustificationTime, 
 			   CAST(dbo.dailyincidences.endtime AS TIME) AS EndJustificationTime, 	
 			   CASE WHEN CAST(DBO.DailyIncidences.Date AS DATE) BETWEEN CAST(@INITIALDATE AS DATE) AND CAST(@ENDDATE AS DATE) AND CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME)
 					THEN CAST(dbo.DailyIncidences.BeginTime AS TIME)
 					WHEN CAST(DBO.DailyIncidences.Date AS DATE) BETWEEN CAST(@INITIALDATE AS DATE) AND CAST(@ENDDATE AS DATE) AND CAST(dbo.DailyIncidences.EndTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME) AND CAST(dbo.DailyIncidences.BeginTime AS TIME) NOT BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME)
 					THEN CAST(@initialTime AS TIME)
 					WHEN CAST(DBO.DailyIncidences.Date AS DATE) BETWEEN CAST(@INITIALDATE AS DATE) AND CAST(@ENDDATE AS DATE) AND CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) NOT BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME)
 					THEN CAST(DBO.DailyIncidences.BeginTime AS TIME)
 					WHEN ((CAST(DBO.DailyIncidences.Date AS DATE) BETWEEN CAST(@INITIALDATE AS DATE) AND CAST(@ENDDATE AS DATE) AND CAST(@initialTime AS TIME) BETWEEN CAST(DBO.DailyIncidences.BeginTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) AND CAST(@endTime AS TIME) BETWEEN CAST(DBO.DailyIncidences.BeginTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME))
 					OR (CAST(DBO.DailyIncidences.BeginTime AS TIME) > CAST(DBO.DailyIncidences.EndTime AS TIME) AND CAST(@initialTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) AND CAST(@endTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME)))
 					THEN CAST(@initialTime AS TIME)				
 			   END AS BeginTimeInFilter,
 			   CASE WHEN CAST(DBO.DailyIncidences.Date AS DATE) BETWEEN CAST(@INITIALDATE AS DATE) AND CAST(@ENDDATE AS DATE) AND CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME)
 					THEN CAST(dbo.DailyIncidences.EndTime AS TIME)
 					WHEN CAST(DBO.DailyIncidences.Date AS DATE) BETWEEN CAST(@INITIALDATE AS DATE) AND CAST(@ENDDATE AS DATE) AND CAST(dbo.DailyIncidences.EndTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME) AND CAST(dbo.DailyIncidences.BeginTime AS TIME) NOT BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME)
 					THEN CAST(DBO.DailyIncidences.EndTime AS TIME)
 					WHEN CAST(DBO.DailyIncidences.Date AS DATE) BETWEEN CAST(@INITIALDATE AS DATE) AND CAST(@ENDDATE AS DATE) AND CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) NOT BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME)
 					THEN CAST(@endTime AS TIME)
 					WHEN ((CAST(DBO.DailyIncidences.Date AS DATE) BETWEEN CAST(@INITIALDATE AS DATE) AND CAST(@ENDDATE AS DATE) AND CAST(@initialTime AS TIME) BETWEEN CAST(DBO.DailyIncidences.BeginTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) AND CAST(@endTime AS TIME) BETWEEN CAST(DBO.DailyIncidences.BeginTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME))
 					OR (CAST(DBO.DailyIncidences.BeginTime AS TIME) > CAST(DBO.DailyIncidences.EndTime AS TIME) AND CAST(@initialTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) AND CAST(@endTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME)))
 					THEN CAST(@endTime AS TIME)
 			   END AS EndTimeInFilter,			   
   dbo.Causes.Export as IncidenceEquivalence,
   dbo.Shifts.Export as ScheduleEquivalence
 			   FROM         dbo.sysroEmployeeGroups with (nolock)    
 				   INNER JOIN dbo.Causes with (nolock)    
 				   INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause     AND DBO.DailyCauses.IDCause IN (SELECT * FROM @causesIDs)     
 				   INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailySchedule.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate    
 				   INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailySchedule.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailySchedule.Date between dbo.EmployeeContracts.BeginDate AND dbo.EmployeeContracts.EndDate     
 				   INNER JOIN dbo.sysroDailyIncidencesDescription with (nolock)    
 				   INNER JOIN dbo.DailyIncidences with (nolock)    
 				   INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID ON dbo.sysroDailyIncidencesDescription.IDIncidence = dbo.DailyIncidences.IDType ON dbo.DailyCauses.IDEmployee = dbo.DailyIncidences.IDEmployee AND dbo.DailyCauses.Date = dbo.DailyIncidences.Date			  
 				   AND dbo.DailyCauses.IDRelatedIncidence = dbo.DailyIncidences.ID AND dbo.DailyIncidences.Date  between DATEADD(DAY, -1,dbo.sysroEmployeeGroups.BeginDate) AND dbo.sysroEmployeeGroups.EndDate    
 				   LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.DailySchedule.IDShiftUsed = dbo.Shifts.ID     
 				   LEFT OUTER JOIN dbo.Shifts AS Shifts_1 with (nolock) ON dbo.DailySchedule.IDShift1 = Shifts_1.ID    
 				   LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID    
 				LEFT OUTER JOIN dbo.sysroRemarks on dbo.sysroRemarks.ID = dbo.DailySchedule.Remarks    
 				LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
 			  WHERE dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.DailyCauses.IDEmployee,2,0,0,dbo.DailyCauses.date) > 1                 		  
 				AND dbo.sysroEmployeeGroups.IDEmployee in(select idEmployee from @employeeIDs)     
 				and dbo.dailyCauses.Date between DATEADD(DAY,-1,@pinitialDate) and @pendDate    
 			   GROUP BY dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name,     
 									 YEAR(dbo.DailySchedule.Date), CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)     
 						 + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120), CASE WHEN DailySchedule.Date <= GETDATE()     
 									 THEN Shifts.Name ELSE Shifts_1.Name END, dbo.Employees.Name, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, DATEPART(iso_week,     
 									 dbo.DailySchedule.Date), DATEPART(dy, dbo.DailySchedule.Date), (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1,     
 									 dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,     
 									 dbo.sysroEmployeeGroups.CurrentEmployee, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate, dbo.DailySchedule.IDAssignment, dbo.DailySchedule.IDDailyBudgetPosition, convert(nvarchar(max),dbo.sysroRemarks.Text),
 									 dbo.sysrovwEmployeeLockDate.LockDate,
 									 dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date),
 									 dbo.dailyincidences.begintime, dbo.dailyincidences.endtime,DBO.DailyIncidences.Date, dbo.Causes.Export , dbo.Shifts.Export) AS SUB	
 									 WHERE BeginTimeInFilter IS NOT NULL AND (DATEDIFF(SECOND, BeginTimeInFilter, EndTimeInFilter)*1.0/3600 > 0)								 									 									 
 		END
 		ELSE
 		BEGIN
 			SELECT *, 
 			 CASE WHEN BeginJustificationTime < EndJustificationTime THEN 
 			 DATEDIFF(SECOND, BeginJustificationTime, EndJustificationTime)*1.0/3600 	 
 			 ELSE (DATEDIFF(SECOND, BeginJustificationTime, CAST('23:59:59' AS TIME))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndJustificationTime)*1.0/3600 
 			 END AS TotalJustificationHHMM_ToHours, 
 			 CASE WHEN BeginTimeInFilter <= EndTimeInFilter THEN DATEDIFF(SECOND, BeginTimeInFilter, EndTimeInFilter)*1.0/3600 
 			 ELSE (DATEDIFF(SECOND, BeginTimeInFilter, (CAST('23:59:59' AS TIME)))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndTimeInFilter)*1.0/3600 
 			 END AS TotalTimeInFilterHHMM_ToHours,
 			 CASE WHEN BeginJustificationTime < EndJustificationTime THEN 
 			 DATEDIFF(SECOND, BeginJustificationTime, EndJustificationTime)*1.0/3600 	 
 			 ELSE (DATEDIFF(SECOND, BeginJustificationTime, CAST('23:59:59' AS TIME))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndJustificationTime)*1.0/3600 
 			 END AS TotalJustification, 
 			 CASE WHEN BeginTimeInFilter <= EndTimeInFilter THEN DATEDIFF(SECOND, BeginTimeInFilter, EndTimeInFilter)*1.0/3600 
 			 ELSE (DATEDIFF(SECOND, BeginTimeInFilter, (CAST('23:59:59' AS TIME)))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndTimeInFilter)*1.0/3600 
 			 END AS TotalTimeInFilter
 			 FROM
 				   (SELECT     
 				   CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)     
 										 + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup,     
 										 dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.DailySchedule.Date AS Date_ToDateString, dbo.sysroDailyIncidencesDescription.Description AS IncidenceName,     
 										 dbo.TimeZones.Name AS ZoneTime, dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS Value, SUM(dbo.DailyCauses.Value) AS ValueHHMM_ToHours, MONTH(dbo.DailySchedule.Date) AS Mes, YEAR(dbo.DailySchedule.Date) AS Año,     
 										 (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 
										 CASE WHEN DATEPART(iso_week, dbo.DailySchedule.Date) >= 52 AND MONTH(dbo.DailySchedule.Date) = 1 THEN (YEAR(dbo.DailySchedule.Date)-1) * 100 + DATEPART(iso_week, dbo.DailySchedule.Date)
										 ELSE (YEAR(dbo.DailySchedule.Date)) * 100 + DATEPART(iso_week, dbo.DailySchedule.Date) END AS WeekOfYear, DATEPART(dy, dbo.DailySchedule.Date) AS DayOfYear, 
 										 DATEPART(QUARTER, dbo.DailySchedule.Date) as Quarter, CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName,     
 										 dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.FullGroupName AS FullGroupName, dbo.sysroEmployeeGroups.Path,     
 										 dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,    
 						dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString, dbo.DailySchedule.IDAssignment,    
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
  					   dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age, convert(nvarchar(max),dbo.sysroRemarks.Text) as Remark,
 					   dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,
 					   dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date) as Punches,
 					   CAST (dbo.dailyincidences.begintime AS TIME) AS BeginJustificationTime, 
 				   CAST(dbo.dailyincidences.endtime AS TIME) AS EndJustificationTime, 	
 				   CASE 
 						WHEN ((CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST('23:59:59' AS TIME) AND (CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST('23:59:59' AS TIME))))
 						OR(CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@endTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@endTime AS TIME))
 						OR (CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST('23:59:59' AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@endTime AS TIME))
 						THEN CAST(DBO.DAILYINCIDENCES.BEGINTIME AS TIME)
 						WHEN (((CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST('23:59:59' AS TIME))
 						OR (CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@endTime AS TIME))) AND CAST(DBO.DailyIncidences.BeginTimE AS TIME) < CAST(@initialTIME AS TIME))
 						THEN CAST(@INITIALTIME AS TIME)
 						WHEN (CAST(DBO.DailyIncidences.BEGINTIME AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@ENDTIME AS TIME))
 						OR (CAST(DBO.DailyIncidences.BEGINTIME AS TIME) BETWEEN CAST(@INITIALTIME AS TIME) AND CAST('23:59:59' AS TIME))
 						THEN CAST(DBO.DAILYINCIDENCES.BEGINTIME AS TIME)
 					    WHEN CAST(dbo.DailyIncidences.EndTime AS TIME) < CAST(dbo.DailyIncidences.BeginTime AS TIME) AND CAST(@INITIALTIME AS TIME) BETWEEN CAST(DBO.DailyIncidences.BEGINTIME AS TIME) AND CAST('23:59:59' AS TIME) 
 						AND CAST(@ENDTIME AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME)
 						THEN CAST(@INITIALTIME AS TIME)
 				   END AS BeginTimeInFilter,
 				   CASE 
 						WHEN ((CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST('23:59:59' AS TIME) AND (CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST('23:59:59' AS TIME))))
 						OR(CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@endTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@endTime AS TIME))
 						OR (CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST('23:59:59' AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@endTime AS TIME))
 						THEN CAST(DBO.DAILYINCIDENCES.EndTime AS TIME)
 						WHEN (((CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST('23:59:59' AS TIME))
 						OR (CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@endTime AS TIME))) AND CAST(DBO.DailyIncidences.BeginTimE AS TIME) < CAST(@initialTIME AS TIME))
 						THEN CAST(DBO.DailyIncidences.EndTime AS TIME)
 						WHEN (CAST(DBO.DailyIncidences.BEGINTIME AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@ENDTIME AS TIME))
 						OR (CAST(DBO.DailyIncidences.BEGINTIME AS TIME) BETWEEN CAST(@INITIALTIME AS TIME) AND CAST('23:59:59' AS TIME))
 						THEN CAST(@endTime AS TIME)
 					    WHEN CAST(@INITIALTIME AS TIME) BETWEEN CAST(DBO.DailyIncidences.BEGINTIME AS TIME) AND CAST('23:59:59' AS TIME) 
 						AND CAST(@ENDTIME AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME)
 						THEN CAST(@endTime AS TIME)				   
 				   END AS EndTimeInFilter,				   
   dbo.Causes.Export as IncidenceEquivalence,
   dbo.Shifts.Export as ScheduleEquivalence
 				   FROM         dbo.sysroEmployeeGroups with (nolock)    
 					   INNER JOIN dbo.Causes with (nolock)    
 					   INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause     AND DBO.DailyCauses.IDCause IN (SELECT * FROM @causesIDs)     
 					   INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailySchedule.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate    
 					   INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailySchedule.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailySchedule.Date between dbo.EmployeeContracts.BeginDate AND dbo.EmployeeContracts.EndDate     
 					   INNER JOIN dbo.sysroDailyIncidencesDescription with (nolock)    
 					   INNER JOIN dbo.DailyIncidences with (nolock)    
 					   INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID ON dbo.sysroDailyIncidencesDescription.IDIncidence = dbo.DailyIncidences.IDType ON dbo.DailyCauses.IDEmployee = dbo.DailyIncidences.IDEmployee AND dbo.DailyCauses.Date = dbo.DailyIncidences.Date			  
 					   AND dbo.DailyCauses.IDRelatedIncidence = dbo.DailyIncidences.ID AND CAST(dbo.DailyIncidences.Date AS DATE)  between CAST(@initialDate AS DATE) AND CAST(DATEADD(DAY, 1,@endDate) AS DATE)
 					   LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.DailySchedule.IDShiftUsed = dbo.Shifts.ID     
 					   LEFT OUTER JOIN dbo.Shifts AS Shifts_1 with (nolock) ON dbo.DailySchedule.IDShift1 = Shifts_1.ID    
 					   LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID    
 					LEFT OUTER JOIN dbo.sysroRemarks on dbo.sysroRemarks.ID = dbo.DailySchedule.Remarks    
 					LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
 				  WHERE dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.DailyCauses.IDEmployee,2,0,0,dbo.DailyCauses.date) > 1                 		  
 					AND dbo.sysroEmployeeGroups.IDEmployee in(select idEmployee from @employeeIDs)     
 					and dbo.dailyCauses.Date between DATEADD(DAY,-1,@pinitialDate) and @pendDate    
 				   GROUP BY dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name,     
 										 YEAR(dbo.DailySchedule.Date), CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)     
 							 + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120), CASE WHEN DailySchedule.Date <= GETDATE()     
 										 THEN Shifts.Name ELSE Shifts_1.Name END, dbo.Employees.Name, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, DATEPART(iso_week,     
 										 dbo.DailySchedule.Date), DATEPART(dy, dbo.DailySchedule.Date), (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1,     
 										 dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,     
 										 dbo.sysroEmployeeGroups.CurrentEmployee, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate, dbo.DailySchedule.IDAssignment, dbo.DailySchedule.IDDailyBudgetPosition, convert(nvarchar(max),dbo.sysroRemarks.Text),
 										 dbo.sysrovwEmployeeLockDate.LockDate,
 										 dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date),
 										 dbo.dailyincidences.begintime, dbo.dailyincidences.endtime,DBO.DailyIncidences.Date, dbo.Causes.Export, dbo.Shifts.Export) AS SUB	
 										 WHERE BeginTimeInFilter IS NOT NULL 										 
 										 AND ((BeginTimeInFilter <= EndTimeInFilter AND DATEDIFF(SECOND, BeginTimeInFilter, EndTimeInFilter)*1.0/3600 >0)
 										  OR (BEGINTIMEINFILTER > ENDTIMEINFILTER AND (DATEDIFF(SECOND, BeginTimeInFilter, (CAST('23:59:59' AS TIME)))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndTimeInFilter)*1.0/3600 > 0))
 										 
 		END

GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='657' WHERE ID='DBVersion'
GO
