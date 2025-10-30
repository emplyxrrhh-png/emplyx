-- Tabla de autorizaciones de accesos por Grupo
CREATE TABLE [dbo].[GroupsAccessAuthorization](
	[IDGroup] [int] NOT NULL,
	[IDAuthorization] [smallint] NOT NULL,
 CONSTRAINT [PK_GroupAccessAuthorization] PRIMARY KEY CLUSTERED 
(
	[IDGroup] ASC,
	[IDAuthorization] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[GroupsAccessAuthorization]  WITH CHECK ADD  CONSTRAINT [FK_GroupsAccessAuthorization_Authorization] FOREIGN KEY([IDAuthorization])
REFERENCES [dbo].[AccessGroups] ([ID])
GO

ALTER TABLE [dbo].[GroupsAccessAuthorization] CHECK CONSTRAINT [FK_GroupsAccessAuthorization_Authorization]
GO

ALTER TABLE [dbo].[GroupsAccessAuthorization]  WITH CHECK ADD  CONSTRAINT [FK_GroupsAccessAuthorization_Groups] FOREIGN KEY([IDGroup])
REFERENCES [dbo].[Groups] ([ID])
GO

ALTER TABLE [dbo].[GroupsAccessAuthorization] CHECK CONSTRAINT [FK_GroupsAccessAuthorization_Groups]
GO

-- Vista de Autorizaciones de Accesos por Empleados y Grupos
 CREATE VIEW [dbo].[sysrovwAccessAuthorizations]
 AS
	select IDEmployee, IDAuthorization, '0' as GroupPath from EmployeeAccessAuthorization
	UNION 
	SELECT IDEmployee, IDAuthorization, gr.Path as GroupPath  FROM GroupsAccessAuthorization ga
	join Groups gr on ga.idgroup = gr.id
	join sysrovwCurrentEmployeeGroups ce2 on (ce2.Path = gr.Path or ce2.Path like gr.Path + '%' )
GO

-- Analítica de accesos: Autorizaciones
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
  			insert into @autorizationsIDs select IDAccessGroup from sysroPassports_AccessGroup with (nolock) where IDPassport = @idParentPassport
 			insert into @zonesIDs select ID from Zones
 						where ID in (SELECT IDZone FROM AccessGroupsPermissions with (nolock)
 									where IDAccessGroup in (select IDAccessGroup from sysroPassports_AccessGroup with (nolock) where idpassport = @idParentPassport)) or IDParent is null
 		end
  	else
 		BEGIN
  			insert into @autorizationsIDs select id from AccessGroups with (nolock)
 			insert into @zonesIDs select ID from Zones with (nolock)
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
         FROM dbo.sysroEmployeeGroups with (nolock) 
          INNER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
          INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.Employees.ID = dbo.EmployeeContracts.IDEmployee and getdate() between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
          INNER JOIN dbo.AccessGroups with (nolock) ON dbo.AccessGroups.ID IN
                         (SELECT        dbo.Employees.IDAccessGroup
                         UNION
                         SELECT        IDAuthorization
                         FROM            dbo.sysrovwAccessAuthorizations with (nolock)
                         WHERE        (IDEmployee = dbo.Employees.ID))
          INNER JOIN dbo.AccessGroupsPermissions with (nolock) ON AccessGroupsPermissions.IDAccessGroup = AccessGroups.ID
          INNER JOIN dbo.AccessPeriods with (nolock) ON AccessPeriods.ID = AccessGroupsPermissions.IDAccessPeriod
          INNER JOIN dbo.Zones with (nolock) on Zones.ID = AccessGroupsPermissions.IDZone
    	WHERE dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.sysroEmployeeGroups.IDEmployee,9,0,0,getdate()) > 1
    			AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) AND getdate() between sysroEmployeeGroups.BeginDate and sysroEmployeeGroups.EndDate
  			AND dbo.AccessGroups.ID IN (SELECT idAccess FROM @autorizationsIDs)
 			AND dbo.Zones.ID in (select idZone from @zonesIDs)
GO

update dbo.sysroGUI set RequiredFeatures = 'Version\Live;!Feature\ONE' where IDPath = 'Portal\GeneralManagement\Cameras'
GO
update dbo.sysroGUI set RequiredFeatures = 'Process\Notifier;!Feature\ONE' where IDPath = 'Portal\GeneralManagement\Notifications'
GO

UPDATE dbo.sysroParameters SET Data='424' WHERE ID='DBVersion'
GO
