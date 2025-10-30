--Planification - Genius_Schedule

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE  [dbo].[Genius_Schedule]
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
CASE WHEN (CASE WHEN ProgrammedAbsences.IDEmployee IS NULL THEN 0 ELSE 1 END) = 1 OR (CASE WHEN Shifts.ID IS NULL OR Shifts.ShiftType = 2 OR Shifts.ExpectedWorkingHours = 0 THEN 1 ELSE 0 END) = 1 THEN '' ELSE CASE WHEN (ISNULL(sysroEmployeesShifts.Telecommuting, CASE WHEN (sysrovwTelecommutingAgreement.Telecommuting = 1 AND CHARINDEX(CONVERT(VARCHAR, (DATEPART(dw, CurrentDate) + @@DATEFIRST - 1 ) % 7), sysrovwTelecommutingAgreement.TelecommutingMandatoryDays) > 0) THEN 1 ELSE 0 END)) = 1 THEN 'X' ELSE '' END END AS TelecommutingExpected,
dbo.sysroEmployeesShifts.CurrentDate AS Date, dbo.sysroEmployeesShifts.CurrentDate AS Date_ToDateString, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours,SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHoursAbs) as HoursAbs, COUNT(*) AS Count,
MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año, (DATEPART(dw,
dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek,
CASE WHEN DATEPART(iso_week, dbo.sysroEmployeesShifts.CurrentDate) >= 52 AND MONTH(dbo.sysroEmployeesShifts.CurrentDate) = 1 THEN (YEAR(dbo.sysroEmployeesShifts.CurrentDate)-1) * 100 + DATEPART(iso_week, dbo.sysroEmployeesShifts.CurrentDate)
ELSE (YEAR(dbo.sysroEmployeesShifts.CurrentDate)) * 100 + DATEPART(iso_week, dbo.sysroEmployeesShifts.CurrentDate) END AS WeekOfYear,
DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear, DATEPART(QUARTER, dbo.sysroEmployeesShifts.CurrentDate) as Quarter, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee,
dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.endDate, dbo.sysroEmployeeGroups.endDate AS endDate_ToDateString,
dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.endDate AS EndContract, dbo.EmployeeContracts.endDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
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
dbo.GetEmployeeAge(sysroEmployeesShifts.idEmployee) As Age, (SELECT LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = sysroEmployeesShifts.IDEmployee) as LockDate,
(SELECT LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = sysroEmployeesShifts.IDEmployee) as LockDate_ToDateString,
dbo.Shifts.Export as ScheduleEquivalence
FROM dbo.sysroEmployeesShifts with (nolock)
INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.sysroEmployeeGroups.endDate
INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.EmployeeContracts.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.EmployeeContracts.endDate
LEFT JOIN dbo.sysrovwTelecommutingAgreement ON sysrovwTelecommutingAgreement.IDContract = EmployeeContracts.IDContract and sysrovwTelecommutingAgreement.IDEmployee = EmployeeContracts.IDEmployee AND sysroEmployeesShifts.CurrentDate BETWEEN sysrovwTelecommutingAgreement.TelecommutingAgreementStart AND sysrovwTelecommutingAgreement.TelecommutingAgreementEnd
LEFT JOIN Shifts WITH(NOLOCK) ON Shifts.ID = sysroEmployeesShifts.IDShift
LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
LEFT JOIN ProgrammedAbsences WITH(NOLOCK) ON ProgrammedAbsences.IDEmployee = EmployeeContracts.IDEmployee AND CurrentDate BETWEEN ProgrammedAbsences.BeginDate AND isnull(ProgrammedAbsences.FinishDate,DATEADD(day,ProgrammedAbsences.maxlastingdays,ProgrammedAbsences.BeginDate))
WHERE (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and dbo.sysroEmployeesShifts.CurrentDate between BeginDate and EndDate and idpassport = @pidpassport and idemployee = dbo.sysroEmployeesShifts.IDEmployee and FeaturePermission > 0) > 0  
AND dbo.sysroEmployeesShifts.IDEmployee in( select idEmployee from @employeeIDs) and dbo.sysroEmployeesShifts.CurrentDate between @pinitialDate and @pendDate
GROUP BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name,
dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate, MONTH(dbo.sysroEmployeesShifts.CurrentDate),
YEAR(dbo.sysroEmployeesShifts.CurrentDate), (DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week,
dbo.sysroEmployeesShifts.CurrentDate), DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate), dbo.sysroEmployeeGroups.IDGroup,
dbo.EmployeeContracts.IDContract, CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)
+ '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10), dbo.sysroEmployeeGroups.Path,
dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.endDate,
dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.endDate,dbo.sysroEmployeesShifts.IDAssignment,dbo.sysroEmployeesShifts.AssignmentName,dbo.sysroEmployeesShifts.IDProductiveUnit,dbo.sysroEmployeesShifts.ProductiveUnitName, convert(nvarchar(max),dbo.sysroEmployeesShifts.Remark), dbo.Shifts.Export,
CASE WHEN (CASE WHEN ProgrammedAbsences.IDEmployee IS NULL THEN 0 ELSE 1 END) = 1 OR (CASE WHEN Shifts.ID IS NULL OR Shifts.ShiftType = 2 OR Shifts.ExpectedWorkingHours = 0 THEN 1 ELSE 0 END) = 1 THEN '' ELSE CASE WHEN (ISNULL(sysroEmployeesShifts.Telecommuting, CASE WHEN (sysrovwTelecommutingAgreement.Telecommuting = 1 AND CHARINDEX(CONVERT(VARCHAR, (DATEPART(dw, CurrentDate) + @@DATEFIRST - 1 ) % 7), sysrovwTelecommutingAgreement.TelecommutingMandatoryDays) > 0) THEN 1 ELSE 0 END)) = 1 THEN 'X' ELSE '' END END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--Planificacion y Fichajes - Genius_ScheduleAndPunches

 ALTER   PROCEDURE [dbo].[Genius_ScheduleAndPunches]
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
    where (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and alldays.dt between BeginDate and EndDate and idpassport = @pidpassport and idemployee = emp.ID and FeaturePermission > 0) > 0  
    ) reqReg 
    INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate 
    INNER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee 
    INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
    INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailySchedule.IDEmployee = reqReg.idEmployee and dbo.DailySchedule.Date = reqReg.dt 
    INNER JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(dbo.DailySchedule.IDShiftUsed,dbo.DailySchedule.IDShift1) 
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
                 WHERE ds.Date between @pinitialDate and @pendDate AND (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and ds.Date between BeginDate and EndDate and idpassport = @pidpassport and idemployee = emp.ID and FeaturePermission > 0) > 0  
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

--Tareas - Genius_Tasks

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[Genius_Tasks]
@initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @userFieldsFilter nvarchar(max), @employeeFilter nvarchar(max) AS

DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pemployeeFilter nvarchar(max) = @employeeFilter
DECLARE @employeeIDs Table(idEmployee int)


insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter, @pinitialDate, @pendDate;


SELECT  dbo.sysroEmployeeGroups.IDGroup, dbo.sysroEmployeeGroups.FullGroupName AS GroupName, dbo.Employees.ID AS IDEmployee,
dbo.Employees.Name AS EmployeeName, DATEPART(year, dbo.DailyTaskAccruals.Date) AS Date_Year, DATEPART(month, dbo.DailyTaskAccruals.Date) AS Date_Month,
DATEPART(day, dbo.DailyTaskAccruals.Date) AS Date_Day, dbo.DailyTaskAccruals.Date, dbo.DailyTaskAccruals.Date AS Date_ToDateString, dbo.BusinessCenters.ID AS IDCenter, dbo.BusinessCenters.Name AS CenterName,
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
dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
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
dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age  ,
dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
FROM    dbo.sysroEmployeeGroups with (nolock)
INNER JOIN dbo.DailyTaskAccruals with (nolock) ON dbo.DailyTaskAccruals.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.DailyTaskAccruals.Date BETWEEN dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate
INNER JOIN dbo.Tasks with (nolock) ON dbo.Tasks.ID = dbo.DailyTaskAccruals.IDTask
INNER JOIN dbo.BusinessCenters with (nolock) ON dbo.Tasks.IDCenter = dbo.BusinessCenters.ID
INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyTaskAccruals.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyTaskAccruals.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyTaskAccruals.Date <= dbo.EmployeeContracts.EndDate
INNER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
WHERE    (dbo.businesscenters.id is null OR dbo.BusinessCenters.ID IN (SELECT DISTINCT IDCenter FROM sysroPassports_Centers with (nolock) WHERE IDPassport=(SELECT isnull(IdparentPassport,0) from sysroPassports where ID = @pidpassport)) )
AND dbo.DailyTaskAccruals.Date between @pinitialDate and @pendDate
AND (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and dbo.DailyTaskAccruals.Date between BeginDate and EndDate and idpassport = @pidpassport and idemployee = dbo.DailyTaskAccruals.IDEmployee and FeaturePermission > 0) > 0  
AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs)

GO

-- Teletrabajo - Genius_TelecommutingAgreementCompliance
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[Genius_TelecommutingAgreementCompliance]
@initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max) AS
DECLARE @employeeIDs Table(idEmployee int)
DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter
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
reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract,
dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName,
reqReg.dt AS Date, reqReg.dt AS Date_ToDateString,
COUNT(*) AS Count,
MONTH(reqReg.dt) AS Mes, YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek,
DATEPART(iso_week, reqReg.dt) AS WeekOfYear,
DATEPART(dy, reqReg.dt) AS DayOfYear, DATEPART(QUARTER, reqReg.dt) AS Quarter,
dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString,
dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
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
dbo.Shifts.Export as ScheduleEquivalence,
(select count(*) from punches where IDEmployee = reqReg.idEmployee and ShiftDate = reqReg.dt and ActualType = 1) as InPunches,
(select count(*) from punches where IDEmployee = reqReg.idEmployee and ShiftDate = reqReg.dt and ActualType = 2) as OutPunches,
CASE WHEN (sysrovwEmployeeTelecommuting.TelecommuteOptionalPlanned is null or sysrovwEmployeeTelecommuting.TelecommuteOptionalPlanned <> 0) AND (sysrovwEmployeeTelecommuting.TelecommuteOptionalExpected = 1 OR sysrovwEmployeeTelecommuting.TelecommuteOptionalPlanned = 1) THEN 'Opcional'
WHEN (sysrovwEmployeeTelecommuting.TelecommutePlanned is null or sysrovwEmployeeTelecommuting.TelecommutePlanned <> 0) AND (sysrovwEmployeeTelecommuting.TelecommuteExpected = 1 OR sysrovwEmployeeTelecommuting.TelecommutePlanned = 1) THEN 'Teletrabajo'
ELSE 'Presencial' END AS PlannedDayType,
CASE WHEN EffectiveTCWork.Value > 0 THEN 'Teletrabajo'
ELSE 'Presencial' END AS PunchDayType,
IIF(EffectiveTCWork.Value IS NULL, 0,EffectiveTCWork.Value) AS EffectiveTCTime,
IIF(EffectivePresenceWork.Value IS NULL, 0, EffectivePresenceWork.Value) AS EffectivePresenceTime,
dbo.Shifts.ExpectedWorkingHours,
EffectiveTCWork.Value AS 'EffectiveTCTime(HH:MM)_TOHOURS',
EffectivePresenceWork.Value AS 'EffectivePresenceTime(HH:MM)_TOHOURS',
dbo.Shifts.ExpectedWorkingHours AS 'ExpectedWorkingHours(HH:MM)_TOHOURS',
CASE WHEN (sysrovwEmployeeTelecommuting.TelecommuteOptionalPlanned is null or sysrovwEmployeeTelecommuting.TelecommuteOptionalPlanned <> 0) AND (sysrovwEmployeeTelecommuting.TelecommuteOptionalExpected = 1 OR sysrovwEmployeeTelecommuting.TelecommuteOptionalPlanned = 1) THEN 2
WHEN (sysrovwEmployeeTelecommuting.TelecommutePlanned is null or sysrovwEmployeeTelecommuting.TelecommutePlanned <> 0) AND (sysrovwEmployeeTelecommuting.TelecommuteExpected = 1 OR sysrovwEmployeeTelecommuting.TelecommutePlanned = 1) THEN 1
ELSE 0 END AS IdPlannedDayType,
CASE WHEN EffectiveTCWork.Value > 0 THEN 1
ELSE 0 END AS IdPunchDayType,
dbo.Shifts.Name as ShiftName
FROM (select alldays.dt,emp.ID as idEmployee, emp.Name as employeeName from alldays,
(select ID, Name from Employees with (nolock)) emp
where (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and alldays.dt between BeginDate and EndDate and idpassport = @pidpassport and idemployee = emp.ID and FeaturePermission > 0) > 0  
AND emp.ID in (select idEmployee from @employeeIDs)
) reqReg
INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate
INNER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee
INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailySchedule.IDEmployee = reqReg.idEmployee and dbo.DailySchedule.Date = reqReg.dt
INNER JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(dbo.DailySchedule.IDShiftUsed,dbo.DailySchedule.IDShift1)
LEFT JOIN dbo.sysrovwEmployeeTelecommuting with (nolock) ON sysrovwEmployeeTelecommuting.IDContract = dbo.EmployeeContracts.IDContract and sysrovwEmployeeTelecommuting.IDEmployee = reqReg.idEmployee AND reqReg.DT = sysrovwEmployeeTelecommuting.Date
LEFT JOIN (SELECT * FROM dbo.sysrovwDailyEfectiveWorkingHours WHERE dbo.sysrovwDailyEfectiveWorkingHours.InTelecommute = 1) AS EffectiveTCWork ON EffectiveTCWork.Date = reqReg.dt AND EffectiveTCWork.IdEmployee = reqReg.idEmployee
LEFT JOIN (SELECT * FROM dbo.sysrovwDailyEfectiveWorkingHours WHERE dbo.sysrovwDailyEfectiveWorkingHours.InTelecommute IS NULL OR dbo.sysrovwDailyEfectiveWorkingHours.InTelecommute = 0) AS EffectivePresenceWork ON EffectivePresenceWork.Date = reqReg.dt AND EffectivePresenceWork.IdEmployee = reqReg.idEmployee
WHERE EffectivePresenceWork.Value > 0 or EffectiveTCWork.Value > 0
GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract,
dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt),
YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy, reqReg.dt),
dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate,
DailySchedule.Date, Shifts.Name,
dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date),
dbo.Shifts.Export,
dbo.DailySchedule.TelecommutingOptional,
dbo.DailySchedule.Telecommuting,
dbo.sysrovwEmployeeTelecommuting.TelecommuteOptionalExpected,
dbo.sysrovwEmployeeTelecommuting.TelecommuteExpected,
dbo.sysrovwEmployeeTelecommuting.TelecommuteOptionalPlanned,
dbo.sysrovwEmployeeTelecommuting.TelecommutePlanned,
EffectiveTCWork.Value,
dbo.Shifts.ExpectedWorkingHours,
EffectivePresenceWork.Value
option (maxrecursion 0)
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='790' WHERE ID='DBVersion'
GO
