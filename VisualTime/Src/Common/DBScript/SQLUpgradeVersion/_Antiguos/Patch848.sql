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
sum(isnull(dbo.DailyAccruals.PositiveValue,0)) AS PositiveValue, sum(isnull(dbo.DailyAccruals.PositiveValue,0)) AS PositiveValueHHMM_ToHours, ExpiredDate, ExpiredDate as ExpiredDate_ToDateString,
dbo.sysrovwEmployeesAnnualWorkPeriods.BeginPeriod as BeginAccrualPeriod_ToDateString, dbo.sysrovwEmployeesAnnualWorkPeriods.EndPeriod as EndAccrualPeriod_ToDateString
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
LEFT JOIN DBO.DailyCauses with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date                                                                          AND (DailyCauses.IDCause in ( select IdCause FROM DBO.ConceptCauses with (nolock) where IDConcept = dbo.DailyAccruals.IDConcept) )
AND DBO.DailyCauses.IDCause IN (SELECT * FROM @causesIDs)
LEFT OUTER JOIN dbo.Causes with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause
LEFT OUTER JOIN dbo.DailyIncidences with (nolock) ON dbo.DailyIncidences.id = dbo.DailyCauses.IDRelatedIncidence and dbo.DailyIncidences.IDEmployee = dbo.DailyCauses.IDEmployee and dbo.DailyIncidences.Date = dbo.DailyCauses.Date
LEFT OUTER JOIN dbo.sysroDailyIncidencesDescription did with (nolock)  on did.IDIncidence = dbo.DailyIncidences.IDType
LEFT OUTER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID
LEFT OUTER JOIN sysrovwEmployeesAnnualWorkPeriods with (nolock) ON sysrovwEmployeesAnnualWorkPeriods.IdEmployee = reqReg.idEmployee and reqReg.dt between sysrovwEmployeesAnnualWorkPeriods.beginperiod and sysrovwEmployeesAnnualWorkPeriods.endperiod AND DBO.Concepts.DefaultQuery = 'L'
GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName,
dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt),
YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy, reqReg.dt),
dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate, dbo.DailyAccruals.Value,
dbo.Causes.Name, dbo.DailyCauses.IDCause, did.Description, DailySchedule.Date, Shifts.Name, dbo.Concepts.Export,
dbo.Causes.Export, dbo.Shifts.Export, dbo.DailyAccruals.ExpiredDate, dbo.sysrovwEmployeesAnnualWorkPeriods.BeginPeriod, dbo.sysrovwEmployeesAnnualWorkPeriods.EndPeriod
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
sysrovwEmployeesAnnualWorkPeriods.BeginPeriod as BeginAccrualPeriod_ToDateString, sysrovwEmployeesAnnualWorkPeriods.EndPeriod as EndAccrualPeriod_ToDateString
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
LEFT OUTER JOIN sysrovwEmployeesAnnualWorkPeriods with (nolock) ON sysrovwEmployeesAnnualWorkPeriods.IdEmployee = emp.ID and ds.Date between sysrovwEmployeesAnnualWorkPeriods.beginperiod and sysrovwEmployeesAnnualWorkPeriods.endperiod
WHERE ds.Date between @pinitialDate and @pendDate AND
(select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and ds.Date between BeginDate and EndDate and idpassport = @pidpassport and idemployee = emp.ID and FeaturePermission > 0) > 0
and emp.ID in (select idEmployee from @employeeIDs)
GROUP BY emp.ID, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract,
dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, emp.Name, ds.date, MONTH(ds.Date),
YEAR(ds.Date), (DATEPART(dw, ds.Date) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, ds.Date), DATEPART(dy,
ds.Date), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate,
dbo.Causes.Name, dbo.DailyCauses.IDCause, did.Description, ds.Date, Shifts.Name, dbo.Causes.Export, dbo.Shifts.Export,
sysrovwEmployeesAnnualWorkPeriods.BeginPeriod, sysrovwEmployeesAnnualWorkPeriods.EndPeriod
option (maxrecursion 0,USE HINT('QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150'))

GO

ALTER PROCEDURE [dbo].[Report_calendario_anual_usuarios_resumen]
@monthStart nvarchar(2) = '1',
@yearStart nvarchar(4) = '2021',
@monthEnd nvarchar(2) = '04',
@yearEnd nvarchar(4) = '2021',
@IDEmployee nvarchar(max) = '777',
@IDGroup nvarchar(9) = '364',
@IDPassport nvarchar(100) = '0'
AS
SET @monthStart = FORMAT(CAST(@monthStart as int), '00');
SET @yearStart = FORMAT(CAST(@yearStart as int), '0000');
SET @monthEnd = FORMAT(CAST(@monthEnd as int), '00');
SET @yearEnd = FORMAT(CAST(@yearEnd as int), '0000');
DECLARE @fechaStart date = DATEFROMPARTS(@yearStart,@monthStart,'01');
declare @fechaEnd datetime = EOMONTH(DATEFROMPARTS(@yearEnd,@monthEnd,'01'));
select ISNULL(ds.ShiftName1,s.Name) AS Name, s.ShortName, ISNULL(ds.ShiftColor1,s.Color) AS Color, COUNT(ISNULL(ds.IDShiftUsed,ds.IDShift1)) as Days, SUM(CASE WHEN s.ShiftType = 2 THEN 0 ELSE ISNULL(ds.ExpectedWorkingHours,s.ExpectedWorkingHours) end) as ExpectedWorkingHours from DailySchedule ds
inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) 
On poe.IDPassport = @IDPassport 
And poe.IDEmployee = ds.IDEmployee 
And ds.Date between poe.BeginDate And poe.EndDate 
inner join shifts s on (IDShiftUsed is not null and IDShiftUsed = s.ID) or (IDShift1 = s.ID)
inner join EmployeeGroups as eg on ds.IDEmployee = eg.IDEmployee and ds.Date between eg.BeginDate and eg.EndDate
and ds.Date between eg.BeginDate and eg.EndDate
left join ProgrammedAbsences as pa
on ds.IDEmployee = pa.IDEmployee
and ds.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
where ds.IDEmployee IN (select * from split(@IDEmployee,','))
and Date between @fechaStart and EOMONTH(@fechaEnd)
and eg.IDGroup IN (select * from split(@IDGroup,','))
GROUP BY ISNULL(ds.ShiftName1,s.Name), s.ShortName, ISNULL(ds.ShiftColor1,s.Color)
option (USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'));
RETURN NULL
GO
 
CREATE OR ALTER PROCEDURE [dbo].[Report_justificaciones_detalle_totalizador]
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '0',
@causes nvarchar(max) = '0',
@ContractID nvarchar(max) = '0',
@GroupId nvarchar(max) = '0',
@FechaInicio datetime2 = '20210301',
@FechaFin datetime2 = '20210301'
AS
select cs.Name AS CauseName, sum(ds.Value) AS CauseValue
from DailyCauses ds
inner join (select * from split(@IDEmployee,',')) tmp on ds.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = ds.IDEmployee and ds.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 3 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
inner join Employees emp on emp.ID = ds.IDEmployee
inner join EmployeeContracts ec on ec.IDEmployee = ds.IDEmployee and ds.Date between ec.BeginDate and ec.EndDate
inner join EmployeeGroups eg on eg.IDEmployee = ds.IDEmployee and ds.Date between eg.BeginDate and eg.EndDate
inner join Groups g on eg.IDGroup = g.ID
left join Causes cs on ds.IDCause = cs.ID
where cast(ds.Date as date) between cast(@FechaInicio as date) and cast(@FechaFin as date) and ds.IDCause in (select * from split(@causes,',')) and ec.IDContract = @ContractID and eg.IDGroup = @GroupId
group by cs.name
OPTION (USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'))
RETURN NULL
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='848' WHERE ID='DBVersion'

GO
