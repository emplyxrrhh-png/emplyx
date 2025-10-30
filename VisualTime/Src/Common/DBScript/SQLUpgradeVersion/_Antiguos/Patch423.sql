--Corrección de features de documentación
update dbo.sysroFeatures set employeefeatureID = 32 where ID in (32310,32320,32330)
GO

-- Campo DNI en visitas
insert into [dbo].[sysroLiveAdvancedParameters] (ParameterName, Value) values ('vst_DocumentNumberField','-1')
GO

-- Corrección de analítica de fichajes para fichajes ACCTA consecutivos
   ALTER PROCEDURE [dbo].[Analytics_Punches]
   	@initialDate smalldatetime,
   	@endDate smalldatetime,
   	@idpassport int,
   	@employeeFilter nvarchar(max),
   	@userFieldsFilter nvarchar(max)
    AS
    DECLARE @employeeIDs Table(idEmployee int)
    insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter,@initialDate,@endDate;
    SELECT     dbo.Punches.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee, 
                          dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Punches.ActualType AS PunchDirection, 
                          dbo.Punches.TypeData AS PunchIDTypeData, CASE WHEN dbo.Punches.TypeData <> 0 THEN dbo.Causes.Name ELSE '' END AS PunchTypeData, 
                          dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day, 
                          MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Año, (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) 
                          % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName, 
                          dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END AS IsInvalid, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails) 
                          AS Details, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, 
                          dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, dbo.Punches.IsNotReliable, dbo.Punches.Location AS PunchLocation, 
                          dbo.Punches.LocationZone AS PunchLocationZone, dbo.Punches.TimeZone AS PunchTimeZone, dbo.sysroPassports.Name AS EditorName,
   					   dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,
   						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
   						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
   						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
   						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
   						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),@endDate) As UserField1,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),@endDate) As UserField2,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),@endDate) As UserField3,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),@endDate) As UserField4,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),@endDate) As UserField5,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),@endDate) As UserField6,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),@endDate) As UserField7,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),@endDate) As UserField8,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),@endDate) As UserField9,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),@endDate) As UserField10
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
    WHERE	dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.sysroEmployeeGroups.IDEmployee,2,0,0,dbo.Punches.ShiftDate) > 1
   			AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @initialDate and @endDate
    GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID, 
                          dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108), 
                          dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name, 
                          (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name, 
                          CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path, 
                          dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, 
                          dbo.Punches.IsNotReliable, dbo.Punches.Location, dbo.Punches.LocationZone, dbo.Punches.TimeZone, dbo.sysroPassports.Name, dbo.Punches.ActualType, 
                          dbo.Causes.Name, dbo.Punches.TypeData, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate
    HAVING      (dbo.Punches.Type = 1) OR
                          (dbo.Punches.Type = 2) OR
                          (dbo.Punches.Type = 3) OR
                          (dbo.Punches.Type = 7 AND (dbo.Punches.ActualType = 1 OR dbo.Punches.ActualType = 2))
GO


-- Gestión de festivos
ALTER TABLE dbo.DailySchedule ADD FeastDay bit default(0)
GO

UPDATE dbo.DailySchedule Set FeastDay = 0 WHERE FeastDay IS NULL
GO

ALTER TABLE dbo.sysroScheduleTemplates_Detail ADD Description nvarchar(MAX) NULL
GO

ALTER TABLE dbo.sysroScheduleTemplates ADD FeastTemplate bit NULL default(0)
GO

UPDATE dbo.sysroScheduleTemplates SET FeastTemplate = 0 WHERE FeastTemplate IS NULL
GO


ALTER TABLE dbo.sysroScheduleTemplates ADD Mode smallint NULL default(0)
GO

UPDATE dbo.sysroScheduleTemplates SET Mode = 0 WHERE Mode IS NULL
GO


UPDATE dbo.sysroParameters SET Data='423' WHERE ID='DBVersion'
GO
