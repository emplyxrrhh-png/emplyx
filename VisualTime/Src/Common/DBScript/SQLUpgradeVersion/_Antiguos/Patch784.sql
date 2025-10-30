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
DELETE FROM @businessCenterIDs WHERE (SELECT COUNT(*) FROM sysrovwSecurity_PermissionOverCostCenters as pc where pc.IDCostCenter = idBusinessCenter and pc.idpassport = @idpassport) = 0 and idBusinessCenter > 0
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
AND (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and DailyCauses.Date between BeginDate and EndDate and idpassport = @idpassport and idemployee = DailyCauses.IDEmployee and FeaturePermission > 0) > 0
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
where  (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and alldays.dt between BeginDate and EndDate and idpassport = @idpassport and idemployee = EMP.ID and FeaturePermission > 0) > 0
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
                   dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,   
                   dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
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
 					LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
           where   
		   (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and CAST(GETDATE() AS DATE) between BeginDate and EndDate and idpassport = @idpassport and idemployee = dbo.sysrovwEmployeeStatus.IDEmployee and FeaturePermission > 0) > 0
   				AND isnull(dbo.sysrovwEmployeeStatus.CostIDCenter,0) in (select idBusinessCenter from @businessCenterIDs)   
   				AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs)

GO

ALTER PROCEDURE [dbo].[Genius_Dining_Punches]
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
      					dbo.Punches.TypeData AS PunchIDTypeData, CASE WHEN ISNULL(dbo.Punches.InTelecommute,0) = 1 THEN 'X' ELSE '' END AS Telecommute,
      					dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, dbo.Punches.ShiftDate AS Date_ToDateString, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day,   
      					MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Año, (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1)   
      					% 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName,   
      					dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL THEN '' ELSE 'Offline' END AS Type, CONVERT(nvarchar(MAX), dbo.Punches.TypeDetails)   
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
 					DiningRoomTurns.Name AS DiningTurn
            FROM         dbo.sysroEmployeeGroups with (nolock) INNER JOIN  
      					dbo.Punches with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee AND dbo.Punches.ShiftDate >= dbo.sysroEmployeeGroups.BeginDate AND   
      					dbo.Punches.ShiftDate <= dbo.sysroEmployeeGroups.EndDate 
 						INNER JOIN DiningRoomTurns ON Punches.TypeData=DiningRoomTurns.ID
 						INNER JOIN  
      					dbo.EmployeeContracts with (nolock) ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND   
      					dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN  
      					dbo.Zones with (nolock) ON dbo.Punches.IDZone = dbo.Zones.ID LEFT OUTER JOIN  
      					dbo.Terminals with (nolock) ON dbo.Punches.IDTerminal = dbo.Terminals.ID LEFT OUTER JOIN  
      					dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID LEFT OUTER JOIN  
      					dbo.sysroPassports with (nolock) ON dbo.Punches.IDPassport = dbo.sysroPassports.ID 
  						LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
            WHERE 
			(select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and Punches.ShiftDate between BeginDate and EndDate and idpassport = @idpassport and idemployee = dbo.sysroEmployeeGroups.IDEmployee and FeaturePermission > 0) > 0
      					AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @pinitialDate and @pendDate  
 						AND Punches.Type=10
            GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID,   
      					dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108),   
      					dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name,   
      					(DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name,   
      					CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(nvarchar(MAX), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path,   
      					dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,   
      					dbo.Punches.IsNotReliable, dbo.Punches.Location, dbo.Punches.LocationZone, dbo.Punches.FullAddress, dbo.Punches.TimeZone, dbo.sysroPassports.Name, dbo.Punches.ActualType,   
      					dbo.Punches.TypeData, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate, dbo.Punches.InTelecommute, DiningRoomTurns.Name

GO

ALTER PROCEDURE [dbo].[Genius_EfectiveWork] @initialDate smalldatetime, @endDate smalldatetime, @idpassport int,  @userFieldsFilter nvarchar(max), @employeeFilter nvarchar(max) AS  
    		--DECLARE @employeeIDs Table(idEmployee int)
  		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @puserFieldsFilter nvarchar(max) = @userFieldsFilter
  		DECLARE @pemployeeFilter nvarchar(max) = @employeeFilter
  		DECLARE @intdatefirst int  
    		SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'  
    		SET DateFirst @intdatefirst;  
    		
  		CREATE TABLE #TMP_EffectiveHours (idemployee INT)
  		--INSERT INTO @employeeIDs EXEC dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
  		INSERT INTO #TMP_EffectiveHours EXEC dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
  		
  		SELECT	IDEmployee,
  				EmployeeName,
  				GroupName, 
  				FullGroupName,
  				WorkCenter,
  				IDContract,
 				BeginContract,
  				BeginContract_ToDateString,
 				EndContract,
  				EndContract_ToDateString,
  				Age, 
  				RegDate AS Date_ToDateString,
  				Mes,
  				Año,
  				DayOfWeek,
  				WeekOfYear,
  				DayOfYear,
  				Quarter,
  				Nivel1, 
  				Nivel2, 
  				Nivel3, 
  				Nivel4, 
  				Nivel5, 
  				Nivel6,
  				Nivel7, 
  				Nivel8, 
  				Nivel9, 
  				Nivel10, 
  				UserField1, 
  				UserField2, 
  				UserField3, 
  				UserField4, 
  				UserField5, 
  				UserField6, 
  				UserField7, 
  				UserField8, 
  				UserField9, 
  				UserField10, 
  				Office as Office_ToHours, 
  				ISNULL(Telecommute,0) AS Telecommute_ToHours,
 				ScheduleEquivalence
  		FROM
  		(
  			SELECT Employees.Id AS IdEmployee,
  				dt AS RegDate, 
  				MONTH(dt) AS Mes,   
    				YEAR(dt) AS Año, 
  				(DATEPART(dw, dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 
  				CASE WHEN DATEPART(iso_week, dt) >= 52 AND MONTH(dt) = 1 THEN (YEAR(dt)-1) * 100 + DATEPART(iso_week, dt)
 				ELSE (YEAR(dt)) * 100 + DATEPART(iso_week, dt) END AS WeekOfYear,
  				DATEPART(dy, dt) AS DayOfYear, 
  				DATEPART(QUARTER, dt) AS Quarter, 
  				Employees.Name as EmployeeName, 
  				dbo.sysroEmployeeGroups.GroupName, 
  				dbo.sysroEmployeeGroups.FullGroupName, 
  				EmployeeContracts.Enterprise As WorkCenter,
  				EmployeeContracts.IDContract AS IDContract,
 				EmployeeContracts.BeginDate AS BeginContract,
  				EmployeeContracts.BeginDate AS BeginContract_ToDateString,
 				EmployeeContracts.EndDate AS EndContract,
  				EmployeeContracts.EndDate AS EndContract_ToDateString,
  				ISNULL(sysrovwDailyEfectiveWorkingHours.Value,0.0) As Value, 
  				CASE WHEN sysrovwDailyEfectiveWorkingHours.InTelecommute = 1 THEN 'Telecommute' ELSE 'Office' END AS TimeType,
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
    				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dt) As UserField1,  
    				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dt) As UserField2,  
    				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dt) As UserField3,  
    				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dt) As UserField4,  
    				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dt) As UserField5,  
    				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dt) As UserField6,  
    				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dt) As UserField7,  
    				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dt) As UserField8,  
    				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dt) As UserField9,  
    				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dt) As UserField10,
   				dbo.GetEmployeeAge(Employees.Id) As Age,				
    dbo.Shifts.Export as ScheduleEquivalence
  			FROM [dbo].[Alldays] (@pinitialDate, @pendDate)
  			LEFT JOIN EmployeeContracts with (nolock) ON dt BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate
  			LEFT JOIN sysroEmployeeGroups with (nolock) ON dt BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate AND EmployeeContracts.IDEmployee = sysroEmployeeGroups.IDEmployee
  			LEFT JOIN Employees with (nolock) ON Employees.ID = sysroEmployeeGroups.IDEmployee
  			LEFT JOIN DailySchedule with (nolock) ON DailySchedule.IDEmployee =  Employees.ID AND DailySchedule.Date = dt
  			LEFT JOIN sysrovwDailyEfectiveWorkingHours with (nolock) ON sysrovwDailyEfectiveWorkingHours.Date = dt AND sysrovwDailyEfectiveWorkingHours.IdEmployee = Employees.ID
 			LEFT JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(dbo.DailySchedule.IDShiftUsed,dbo.DailySchedule.IDShift1)   			
  			WHERE sysroEmployeeGroups.IdEmployee IN (SELECT idemployee FROM #TMP_EffectiveHours)
  			AND (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and dt between BeginDate and EndDate and idpassport = @idpassport and idemployee = Employees.Id and FeaturePermission > 0) > 0
  		) TMP
  		PIVOT (AVG(Value) for TimeType in (Office,Telecommute)) AS Accrual
  		ORDER BY IdEmployee ASC, RegDate ASC

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
WHERE 
(select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and dbo.DailyCauses.date between BeginDate and EndDate and idpassport = @idpassport and idemployee = dbo.DailyCauses.IDEmployee and FeaturePermission > 0) > 0
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

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='784' WHERE ID='DBVersion'
GO
