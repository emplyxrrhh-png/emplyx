CREATE TABLE [dbo].[sysroPassports_AccessGroup](
	[IDPassport] [int] NOT NULL,
	[IDAccessGroup] [smallint] NOT NULL,
 CONSTRAINT [PK_sysroPassports_AccessGroup] PRIMARY KEY CLUSTERED 
(
	[IDPassport] ASC,
	[IDAccessGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO dbo.sysroLiveAdvancedParameters ([ParameterName],[Value]) VALUES ('AccessGroupsMode',0)
GO

update dbo.sysroFeatures set Name = 'Autorizaciones' where id = 9000
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
 VALUES(9020,9000,'Access.Groups.Assign','Asignación a empleados','','U','RWA',NULL,NULL,9) 
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 9020, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 9000
GO
  
update dbo.sysrogui set RequiredFunctionalities = 'U:Calendar=Read' where idpath = 'Portal\ShiftControl\AbsencesStatus'
GO

update dbo.sysroGUI_Actions set RequieredFunctionalities = 'U:Access.Groups.Assign=Write'
where IDPath = 'New' and IDGUIPath = 'Portal\AccessManagement\AccessGroups\management'

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
 VALUES(7700,7,'Administration.Alerts','Alertas','','U','RWA',NULL,NULL,NULL) 
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 7700, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 7
GO

UPDATE dbo.sysroGUI
SET RequiredFunctionalities = 'U:Administration.Alerts=Read'
WHERE IDPath = 'Portal\General\Alerts'
GO

create table dbo.sysroReportProfileParameters
(
	IdReportProfile int,	
	ParameterName nvarchar(10),
	ParameterValue nvarchar(20)
)
GO

ALTER PROCEDURE [dbo].[Analytics_Authorizations]
  	@idpassport int,
  	@employeeFilter nvarchar(max),
  	@userFieldsFilter nvarchar(max)
   AS
	
    DECLARE @employeeIDs Table(idEmployee int)
	DECLARE @autorizationsIDs Table(idAccess int)
    DECLARE @cDate as Date, @idParentPassport int
	select @idParentPassport = IDParentPassport from sysroPassports where id = @idpassport
	if ((select value from sysroLiveAdvancedParameters where ParameterName = 'AccessGroupsMode') = '1')
		insert into @autorizationsIDs select IDAccessGroup from sysroPassports_AccessGroup where IDPassport = @idParentPassport
	else
		insert into @autorizationsIDs select id from AccessGroups

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

GO

ALTER PROCEDURE [dbo].[Analytics_Access]
  	@initialDate smalldatetime,
  	@endDate smalldatetime,
  	@idpassport int,
  	@employeeFilter nvarchar(max),
  	@userFieldsFilter nvarchar(max)
   AS
    DECLARE @employeeIDs Table(idEmployee int)
    insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter, @initialDate, @endDate;
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
    GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID, 
                          dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108), 
                          dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name, 
                          (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name, 
                          CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path, 
                          dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, 
                          dbo.sysroEmployeeGroups.IDGroup,dbo.EmployeeContracts.BeginDate , dbo.EmployeeContracts.EndDate
    HAVING (dbo.Punches.Type = 5) OR (dbo.Punches.Type = 6) OR (dbo.Punches.Type = 7)
GO

exec dbo.sysro_GenerateAllPermissionsOverFeatures
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
            WHERE Name = N'Export' AND Object_ID = Object_ID(N'Assignments'))
BEGIN
	ALTER TABLE Assignments ADD Export nvarchar(5) NULL
END
GO

IF EXISTS(SELECT * FROM sys.columns 
            WHERE Name = N'Export' AND Object_ID = Object_ID(N'Assignments'))
BEGIN

	DECLARE @ExportName varchar(3), @IdAssignments int, @nullCount int, @nullName varchar(3)
	set @nullCount = 1
	set @nullName = 'SNC'	

	DECLARE cAssignments CURSOR FOR    
	select id,ShortName from Assignments

	OPEN cAssignments

	FETCH cAssignments INTO @IdAssignments, @ExportName
	WHILE (@@FETCH_STATUS = 0 )
	BEGIN
		
		DECLARE @counter INT, @counterExport int, @newExportName varchar(5), @newNullExportName varchar(5)
		--Miro cuantas veces se reptie el nombre corto
		IF (@ExportName IS NOT NULL)
		BEGIN
			select @counter = COUNT(ShortName) from Assignments
			WHERE ShortName = @ExportName
			group by ShortName
		
			--si es solo 1 actualizo directamente el export
			IF(@counter = 1)
			BEGIN
				UPDATE Assignments 
				SET Export = @ExportName
				WHERE ID = @IdAssignments
			END
			--si ya existe creo un secuencial para añadir al nombre corto
			ELSE
			BEGIN
				SELECT @counterExport = COUNT(Export) FROM Assignments
				WHERE Export LIKE '%' + @ExportName + '%' 			
				set @newExportName = @ExportName + CONVERT(varchar(2),@counterExport+1)
				SET @newExportName = right('DDDDD'+convert(varchar(5), @newExportName), 5) 
			
				UPDATE Assignments 
				SET Export = LTRIM(RTRIM(@newExportName))
				WHERE ID = @IdAssignments
			END		
		END
		--si el nombre corto es nulo, asigno un nombre estándar con un secuencial
		ELSE
		BEGIN
			set @newNullExportName = @nullName +  CONVERT(varchar(2),@nullCount) 
			UPDATE Assignments 
			SET Export = LTRIM(RTRIM(@newNullExportName))
			WHERE ID = @IdAssignments
			set @nullCount = @nullCount + 1
		END
		

		FETCH cAssignments INTO @IdAssignments, @ExportName		

	END
	CLOSE cAssignments
	DEALLOCATE cAssignments
END
GO

ALTER TABLE Assignments ALTER COLUMN Export nvarchar(5) NOT NULL
GO

ALTER TABLE Assignments
ADD CONSTRAINT DF_Assignments_ExportValue UNIQUE (Export); 
GO

 
UPDATE dbo.sysroParameters SET Data='400' WHERE ID='DBVersion'
GO
