CREATE PROCEDURE [dbo].[Genius_ConceptsAndSchedule]
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
   GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName, 
   dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt), 
   YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy, reqReg.dt),
   dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
   dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate, dbo.DailyAccruals.Value,
   DailySchedule.Date, Shifts.Name, dbo.Concepts.Export,   
   dbo.Shifts.Export, dbo.DailyAccruals.ExpiredDate   
                option (maxrecursion 0)

GO


IF NOT EXISTS (SELECT * FROM [dbo].[GeniusViews] WHERE Name = 'scheduleAndConcepts' and IdPassport = -1)
INSERT INTO [dbo].[GeniusViews]
           ([IdPassport]
           ,[Name]
           ,[Description]
           ,[DS]
           ,[TypeView]
           ,[CreatedOn]
           ,[Employees]
           ,[DateFilterType]
           ,[DateInf]
           ,[DateSup]
           ,[CubeLayout]
           ,[Concepts]
           ,[UserFields]
           ,[BusinessCenters]
           ,[CustomFields]
           ,[DSFunction]
           ,[Feature]
           ,[RequieredFeature])
     VALUES
           (-1,	'scheduleAndConcepts', '','S',	1,	'20220518',''	,	0,	'20220518',	'20220518',	'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"IncidenceName"}],"rows":[{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"},{"uniqueName":"ShiftName"},{"uniqueName":"ConceptName"},{"uniqueName":"ValueHHMM_ToHours"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Hours_ToHours","formula":"sum(\"Value\")","caption":"Hours_ToHours"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"off"}},"creationDate":"2022-01-20T09:37:50.150Z"}'		,'','','',		'{"IncludeZeros":true,"IncludeZeroBusinessCenter":true,"LanguageKey":"ESP","RequestTypes":""}',	'Genius_ConceptsAndSchedule(@initialDate,@endDate,@idpassport,@employeeFilter,@conceptsFilter,@userFieldsFilter,@includeZeros))',	'Calendar',	'Calendar.Analytics')

GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE Class = 'hosa')
INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('hosa'
           ,'scheduleAndConcepts')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 7 and Class = 'hosa')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (7
           ,'hosa')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 6 and Class = 'hosa')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (6
           ,'hosa')
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO


UPDATE sysroParameters SET Data='553' WHERE ID='DBVersion'
GO