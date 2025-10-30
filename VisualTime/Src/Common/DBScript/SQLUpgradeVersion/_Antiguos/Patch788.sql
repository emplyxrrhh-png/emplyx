 ALTER PROCEDURE [dbo].[Genius_IncompletePunches]
            @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint AS 
    DECLARE @employeeIDs Table(idEmployee int)    
    DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros 
    DECLARE @intdatefirst int 
    SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS' 
    SET DateFirst @intdatefirst;   
    insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate   ; 
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
      (select count(*) from punches where IDEmployee = reqReg.idEmployee and ShiftDate = reqReg.dt and ActualType = 1) as InPunches,
    (select count(*) from punches where IDEmployee = reqReg.idEmployee and ShiftDate = reqReg.dt and ActualType = 2) as OutPunches
    FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,  
    (select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp, 
    (select ID, Name from Concepts with (nolock))con
    where (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and alldays.dt between BeginDate and EndDate and idpassport = @pidpassport and idemployee = emp.Id and FeaturePermission > 0) > 0
    ) reqReg 
    INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate 
    INNER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee 
    INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
    LEFT OUTER JOIN dbo.DailySchedule with (nolock) ON dbo.DailySchedule.IDEmployee = reqReg.idEmployee and dbo.DailySchedule.Date = reqReg.dt 
    LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(dbo.DailySchedule.IDShiftUsed,dbo.DailySchedule.IDShift1) 
    LEFT OUTER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept 
    LEFT OUTER JOIN dbo.Concepts with (nolock) ON dbo.Concepts.ID = dbo.DailyAccruals.IDConcept 
    LEFT OUTER JOIN DBO.DailyCauses with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date                                                                          AND (DailyCauses.IDCause in ( select IdCause FROM DBO.ConceptCauses with (nolock) where IDConcept = dbo.DailyAccruals.IDConcept) ) 
    LEFT OUTER JOIN dbo.Causes with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause
    LEFT OUTER JOIN dbo.DailyIncidences with (nolock) ON dbo.DailyIncidences.id = dbo.DailyCauses.IDRelatedIncidence and dbo.DailyIncidences.IDEmployee = dbo.DailyCauses.IDEmployee and dbo.DailyIncidences.Date = dbo.DailyCauses.Date 
    LEFT OUTER JOIN dbo.sysroDailyIncidencesDescription did with (nolock)  on did.IDIncidence = dbo.DailyIncidences.IDType 
    LEFT OUTER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID     
    GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName, 
    dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt), 
    YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy, reqReg.dt),
    dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
    dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate, dbo.DailyAccruals.Value,
    dbo.Causes.Name, dbo.DailyCauses.IDCause, did.Description, DailySchedule.Date, Shifts.Name,
    dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date), dbo.Concepts.Export, dbo.Causes.Export, dbo.Shifts.Export
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
   (select count(*) from punches where IDEmployee = emp.ID and ShiftDate = ds.Date and ActualType = 1) as InPunches,
    (select count(*) from punches where IDEmployee = emp.ID and ShiftDate = ds.Date and ActualType = 2) as OutPunches
                 FROM dbo.DailySchedule ds with (nolock)
                                              INNER JOIN dbo.Employees emp on ds.IDEmployee = emp.ID
              INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = emp.id
              INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = emp.ID AND ds.Date between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
                                              LEFT OUTER JOIN DBO.DailyCauses with (nolock) ON dbo.DailyCauses.IDEmployee = emp.id AND dbo.DailyCauses.Date = ds.Date AND (DailyCauses.IDCause NOT IN (SELECT IDCause FROM DBO.ConceptCauses))
                                              INNER JOIN dbo.Causes with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause
                                              INNER JOIN dbo.DailyIncidences with (nolock) ON dbo.DailyIncidences.id = dbo.DailyCauses.IDRelatedIncidence and dbo.DailyIncidences.IDEmployee = dbo.DailyCauses.IDEmployee and dbo.DailyIncidences.Date = dbo.DailyCauses.Date
                                              INNER JOIN dbo.sysroDailyIncidencesDescription did with (nolock)  on did.IDIncidence = dbo.DailyIncidences.IDType
                                              INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID
                                              LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(ds.IDShiftUsed,ds.IDShift1)
                                              LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID
                 WHERE ds.Date between @pinitialDate and @pendDate AND 				 
				 (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and ds.date between BeginDate and EndDate and idpassport = @pidpassport and idemployee = emp.id and FeaturePermission > 0) > 0
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
WHERE 
(select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and dbo.DailyCauses.date between BeginDate and EndDate and idpassport = @idpassport and idemployee = dbo.DailyCauses.IDEmployee and FeaturePermission > 0) > 0
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
WHERE (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and dbo.DailyCauses.date between BeginDate and EndDate and idpassport = @idpassport and idemployee = dbo.DailyCauses.IDEmployee and FeaturePermission > 0) > 0
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
       					dbo.Punches.TypeData AS PunchIDTypeData, CASE WHEN dbo.Punches.TypeData <> 0 THEN dbo.Causes.Name ELSE '' END AS PunchTypeData,   CASE WHEN ISNULL(dbo.Punches.InTelecommute,0) = 1 THEN 'X' ELSE '' END AS Telecommute,
       					dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, dbo.Punches.ShiftDate AS Date_ToDateString, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day,   
       					MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Año, (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1)   
       					% 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName,   
       					dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END AS IsInvalid, CONVERT(nvarchar(MAX), dbo.Punches.TypeDetails)   
       					AS Details, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString,
       					dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.IDGroup, dbo.Punches.IsNotReliable, dbo.Punches.Location AS PunchLocation,   
       					dbo.Punches.LocationZone AS PunchLocationZone, dbo.Punches.FullAddress , dbo.Punches.TimeZone AS PunchTimeZone, dbo.sysroPassports.Name AS EditorName,  
       					dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString, 
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
    					dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age,
   					dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,					
     dbo.Causes.Export as IncidenceEquivalence,
 	(select count(*) from punches where IDEmployee = dbo.sysroEmployeeGroups.IDEmployee and ShiftDate = dbo.Punches.ShiftDate and ActualType = 1) as InPunches,
 	(select count(*) from punches where IDEmployee = dbo.sysroEmployeeGroups.IDEmployee and ShiftDate = dbo.Punches.ShiftDate and ActualType = 2) as OutPunches,
 	CASE WHEN dbo.Punches.ZoneIsNotReliable = 1 THEN 'No fiable' ELSE 'Fiable' END AS ZoneIsNotReliable
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
   						LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
             WHERE (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and dbo.Punches.ShiftDate between BeginDate and EndDate and idpassport = @pidpassport and idemployee = dbo.sysroEmployeeGroups.IDEmployee and FeaturePermission > 0) > 0
       					AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @pinitialDate and @pendDate  
             GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID,   
       					dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108),   
       					dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name,   
       					(DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name,   
       					CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(nvarchar(MAX), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path,   
       					dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,   
       					dbo.Punches.IsNotReliable, dbo.Punches.Location, dbo.Punches.LocationZone, dbo.Punches.FullAddress, dbo.Punches.TimeZone, dbo.sysroPassports.Name, dbo.Punches.ActualType,   
       					dbo.Causes.Name, dbo.Punches.TypeData, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate, dbo.Punches.InTelecommute,
  						dbo.Causes.Export, dbo.punches.ZoneIsNotReliable
             HAVING (dbo.Punches.Type = 1) OR  
       					(dbo.Punches.Type = 2) OR  
       					(dbo.Punches.Type = 3) OR  
       					(dbo.Punches.Type = 7 AND (dbo.Punches.ActualType = 1 OR dbo.Punches.ActualType = 2))
GO

ALTER PROCEDURE [dbo].[Genius_Requests_Schedule]
@initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @requestTypesFilter nvarchar(max) AS

DECLARE @employeeIDs Table(idEmployee int)
DECLARE @requestTypeFilter Table(idRequest int)

insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter,@initialDate,@endDate;
insert into @requestTypeFilter select * from dbo.SplitToInt(@requestTypesFilter,',');
SELECT dbo.requests.ID AS KeyView, isnull(case when aux.NumeroDias > 0 then aux.NumeroDias else DATEDIFF(day,dbo.Requests.Date1, dbo.Requests.Date2) + 1 end,0) as NumeroDias, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee,
dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Requests.RequestDate as 'Date', dbo.Requests.RequestDate as 'Date_ToDateString',
dbo.Requests.Status AS RequestsStatus,
Case WHEN Requests.Status IN(2,3) THEN (SELECT top 1 isnull(sysropassports.Name,'') from RequestsApprovals inner join sysroPassports on RequestsApprovals.IDPassport = sysroPassports.ID  where IDRequest = Requests.ID AND Status in(2,3) ) ELSE '---' END as 'ApproveRefusePassport',
Case WHEN Requests.Status IN(0,1) Then (
SELECT 
                STUFF((
                    SELECT ', ' + sp.Name
                    FROM sysrovwSecurity_PendingRequestsDependencies AS pr
                    INNER JOIN sysroPassports AS sp ON pr.IdPassport = sp.id 
                    WHERE DirectDependence = 1 
                        AND IdRequest = Requests.ID 
                        AND IsRoboticsUser = 0 
                        AND sp.GroupType <> 'U' 
                        AND sp.IDUser IS NOT NULL 
                        AND sp.Description NOT LIKE '%@@ROBOTICS@@%' 
                        AND PR.Permission > 3
                    ORDER BY SP.Name
                    FOR XML PATH('')), 1, 2, '')
) ELSE '---' END as 'PendingPassport',
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
1 AS Value, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString,
dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.IDGroup,
dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @endDate AS EndPeriod, @endDate AS EndPeriod_ToDateString,
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
dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age,
dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
FROM dbo.sysroEmployeeGroups with (nolock)
INNER JOIN dbo.requests with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.requests.IDEmployee AND dbo.requests.RequestDate >= dbo.sysroEmployeeGroups.BeginDate AND dbo.requests.RequestDate <= dbo.sysroEmployeeGroups.EndDate
INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.requests.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.requests.RequestDate >= dbo.EmployeeContracts.BeginDate AND dbo.requests.RequestDate <= dbo.EmployeeContracts.EndDate
LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
left join (SELECT IDRequest, COUNT(*) NumeroDias FROM sysroRequestDays GROUP BY IDRequest) aux on aux.IDRequest = requests.id
LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
WHERE (select count(*) from sysrovwSecurity_PermissionOverRequests as pr where pr.IdPassport = @idpassport and pr.IdRequest = Requests.ID and pr.Permission > 0) > 0
AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.requests.RequestDate between @initialDate and @endDate
AND dbo.Requests.RequestType in (select idRequest from @requestTypeFilter)
GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.requests.RequestDate), YEAR(dbo.requests.RequestDate), dbo.requests.ID,
dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.requests.RequestDate), dbo.requests.RequestDate, CONVERT(nvarchar(8), dbo.requests.RequestDate, 108),
dbo.requests.RequestDate, dbo.EmployeeContracts.IDContract,
dbo.Employees.Name, (DATEPART(dw, dbo.requests.RequestDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.requests.RequestType, dbo.requests.Status, dbo.requests.Date1, dbo.requests.FromTime, dbo.requests.Date2, dbo.requests.ToTime, dbo.requests.IDCause,CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldName, '')),CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldValue, '')), dbo.requests.IDShift,CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')),
dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate , aux.NumeroDias, dbo.sysrovwEmployeeLockDate.LockDate

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
            sreg.Path, sreg.CurrentEmployee, sreg.BeginDate, sreg.BeginDate AS BeginDate_ToDateString, sreg.EndDate, sreg.EndDate AS EndDate_ToDateString,
            ec.BeginDate AS BeginContract, ec.BeginDate AS BeginContract_ToDateString, ec.EndDate AS EndContract, ec.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,    
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
            dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@pendDate) As UserField10    ,
 		   dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
          FROM   
     dbo.sysroEmployeeGroups as sreg with (nolock)  
      INNER JOIN dbo.Employees emp  with (nolock) on sreg.IDEmployee = emp.ID  
	  INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature as pef ON pef.IdFeature = 1 and @pendDate between PEF.BeginDate and PEF.EndDate and PEF.idpassport = @pidpassport and PEF.idemployee = emp.ID and PEF.IsRoboticsUser = 0 and PEF.FeaturePermission > 0
            INNER JOIN dbo.EmployeeContracts ec with (nolock) ON ec.IDEmployee = emp.ID AND @pendDate between ec.BeginDate and ec.EndDate    
 		   LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID
          WHERE  		  
		  sreg.IDEmployee in( select idEmployee from @employeeIDs) and CurrentEmployee = 1 and @pendDate between sreg.BeginDate and sreg.EndDate) salaryGap
GO



-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='788' WHERE ID='DBVersion'
GO
