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
   where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1
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

UPDATE sysroParameters SET Data='659' WHERE ID='DBVersion'
GO
