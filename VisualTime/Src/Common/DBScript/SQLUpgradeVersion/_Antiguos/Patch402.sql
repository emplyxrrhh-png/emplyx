-- Aparece Chino en lista de idiomas (no instalado)
INSERT INTO dbo.[sysroLanguages] ([ID],[LanguageKey],[Culture],[Parameters]) VALUES (7, 'CHN', 'zh-CH', '<?xml version="1.0"?><roCollection version="2.0"><Item key="ExtLanguage" type="8">fr</Item><Item key="ExtDatePickerFormat" type="8">d/m/Y</Item><Item key="ExtDatePickerStartDay" type="8">1</Item></roCollection>')
GO
 
--Se requiere permiso de administración de autorizaciones de acceso para poder crear nuevas
update dbo.sysroGUI_Actions set RequieredFunctionalities = 'U:Access.Groups.Assign=Admin' where IDGUIPath = 'Portal\AccessManagement\AccessGroups\management' and LanguageTag = 'tbAddNewGroup'
GO 

--Analítica de Accesos tiene en cuenta permisos sobre zonas a través de autorizaciones (sólo AMCI)
 ALTER PROCEDURE [dbo].[Analytics_Access]
    	@initialDate smalldatetime,
    	@endDate smalldatetime,
    	@idpassport int,
    	@employeeFilter nvarchar(max),
    	@userFieldsFilter nvarchar(max)
     AS
 	DECLARE @employeeIDs Table(idEmployee int)
 	DECLARE @zonesIDs Table(idZone int)
 	DECLARE @idParentPassport int
 	insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter, @initialDate, @endDate;
 	select @idParentPassport = IDParentPassport from sysroPassports where id = @idpassport
 	if ((select value from sysroLiveAdvancedParameters where ParameterName = 'AccessGroupsMode') = '1')		
 		insert into @zonesIDs select ID from Zones
 					where ID in (SELECT IDZone FROM AccessGroupsPermissions
 								where IDAccessGroup in (select IDAccessGroup from sysroPassports_AccessGroup where idpassport = @idParentPassport)) or IDParent is null
 		
 	else		
 		insert into @zonesIDs select ID from Zones
 		
      SELECT     dbo.Punches.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee, 
                            dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, CONVERT(nvarchar(8), 
                            dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day, MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Year, 
                            (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, 
                            dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName, dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL 
                            THEN 0 ELSE 1 END AS IsInvalid, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails) AS Details, dbo.sysroEmployeeGroups.Path, 
                            dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,
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
      FROM         dbo.sysroEmployeeGroups 
    						INNER JOIN dbo.Punches ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee 
    						INNER JOIN dbo.EmployeeContracts ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate 
    						LEFT OUTER JOIN dbo.Zones ON dbo.Punches.IDZone = dbo.Zones.ID 
    						LEFT OUTER JOIN dbo.Terminals ON dbo.Punches.IDTerminal = dbo.Terminals.ID 
    						LEFT OUTER JOIN dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
      WHERE     dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.sysroEmployeeGroups.IDEmployee,9,0,0,dbo.Punches.ShiftDate) > 1
    			AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @initialDate and @endDate
				and (dbo.Punches.ShiftDate between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate)
 			    AND dbo.Zones.ID IN (SELECT idZone FROM @zonesIDs)
      GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID, 
                            dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108), 
                            dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name, 
                            (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name, 
                            CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path, 
                            dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, 
                            dbo.sysroEmployeeGroups.IDGroup,dbo.EmployeeContracts.BeginDate , dbo.EmployeeContracts.EndDate
      HAVING (dbo.Punches.Type = 5) OR (dbo.Punches.Type = 6) OR (dbo.Punches.Type = 7)
GO

 
 
ALTER PROCEDURE [dbo].[Analytics_Authorizations]
   	@idpassport int,
   	@employeeFilter nvarchar(max),
   	@userFieldsFilter nvarchar(max)
    AS
 	
     DECLARE @employeeIDs Table(idEmployee int)
 	 DECLARE @autorizationsIDs Table(idAccess int)
	 DECLARE @zonesIDs Table(idZone int)
     DECLARE @cDate as Date, @idParentPassport int
 	select @idParentPassport = IDParentPassport from sysroPassports where id = @idpassport
 	if ((select value from sysroLiveAdvancedParameters where ParameterName = 'AccessGroupsMode') = '1')
		begin
 			insert into @autorizationsIDs select IDAccessGroup from sysroPassports_AccessGroup where IDPassport = @idParentPassport
			insert into @zonesIDs select ID from Zones
						where ID in (SELECT IDZone FROM AccessGroupsPermissions
									where IDAccessGroup in (select IDAccessGroup from sysroPassports_AccessGroup where idpassport = @idParentPassport)) or IDParent is null
		end
 	else
		BEGIN
 			insert into @autorizationsIDs select id from AccessGroups
			insert into @zonesIDs select ID from Zones
		END
     
	SET @cDate = GETDATE()
 	insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter, @cDate, @cDate;
 	
    SELECT dbo.sysroEmployeeGroups.IDEmployee AS IDEmployee, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.IDGroup AS IDGroup, dbo.sysroEmployeeGroups.FullGroupName As FullGroupName, dbo.sysroEmployeeGroups.Path AS GroupPath, 
           dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee, dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract AS IDContract, dbo.AccessGroups.Name AS AuthorizationName, 
           Zones.Name AS ZoneName, zones.IsWorkingZone AS IsWorkingZone, AccessPeriods.Name As AccessPeriodName, 1 AS BelongsToGroup,
   		dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract,
   		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
   		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
   		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
   		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
   		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
   		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),getdate()) As UserField1,
   		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),getdate()) As UserField2,
   		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),getdate()) As UserField3,
   		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),getdate()) As UserField4,
   		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),getdate()) As UserField5,
   		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),getdate()) As UserField6,
   		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),getdate()) As UserField7,
   		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),getdate()) As UserField8,
   		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),getdate()) As UserField9,
   		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),getdate()) As UserField10
        FROM dbo.sysroEmployeeGroups 
         INNER JOIN dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
         INNER JOIN dbo.EmployeeContracts ON dbo.Employees.ID = dbo.EmployeeContracts.IDEmployee and getdate() between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
         INNER JOIN dbo.AccessGroups ON dbo.AccessGroups.ID IN
                        (SELECT        dbo.Employees.IDAccessGroup
                        UNION
                        SELECT        IDAuthorization
                        FROM            dbo.EmployeeAccessAuthorization
                        WHERE        (IDEmployee = dbo.Employees.ID))
         INNER JOIN dbo.AccessGroupsPermissions ON AccessGroupsPermissions.IDAccessGroup = AccessGroups.ID
         INNER JOIN dbo.AccessPeriods ON AccessPeriods.ID = AccessGroupsPermissions.IDAccessPeriod
         INNER JOIN dbo.Zones on Zones.ID = AccessGroupsPermissions.IDZone
   	WHERE dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.sysroEmployeeGroups.IDEmployee,9,0,0,getdate()) > 1
   			AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) AND getdate() between sysroEmployeeGroups.BeginDate and sysroEmployeeGroups.EndDate
 			AND dbo.AccessGroups.ID IN (SELECT idAccess FROM @autorizationsIDs)
			AND dbo.Zones.ID in (select idZone from @zonesIDs)
GO

-- Asignamos todos los grupos de acceso a los usuarios del grupo Administradores
INSERT INTO dbo.sysroPassports_AccessGroup
SELECT sp.id, ag.id 
FROM dbo.AccessGroups ag, dbo.sysroPassports sp
WHERE sp.Name = 'Administradores' AND sp.GroupType = 'U' 
GO
 
UPDATE dbo.sysroParameters SET Data='402' WHERE ID='DBVersion'
GO
