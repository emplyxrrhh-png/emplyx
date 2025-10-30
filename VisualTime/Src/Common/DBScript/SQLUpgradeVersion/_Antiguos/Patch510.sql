 ALTER PROCEDURE [dbo].[GenerateAllRequestPassportPermission]  
    AS  
    BEGIN  
      
  /* Rellenamos una tabla  con todos los passaportes de tipo grupo y cada una de las peticiones sobre las que tiene permisos */  
		DELETE FROM sysroPermissionsOverRequests
		
		INSERT INTO sysroPermissionsOverRequests
		select pof.PassportID as passportid, r.ID as IDRequest, pog.EmployeeGroupID from Requests r
		inner join sysroFeatures srf on srf.AliasID = r.RequestType
		inner join sysrovwCurrentEmployeeGroups ceg on ceg.IDEmployee = r.IDEmployee
		inner join sysroPermissionsOverFeatures pof on pof.FeatureID = srf.ID and pof.Permission > 3
		inner join sysroPermissionsOverGroups pog on pof.PassportID = pog.PassportID and pog.EmployeeGroupID = ceg.IDGroup and pog.Permission > 3 and pog.EmployeeFeatureID = srf.EmployeeFeatureID
		left join sysroPermissionsOverEmployeesExceptions poe on poe.PassportID = pof.PassportID and poe.EmployeeID = r.IDEmployee and poe.EmployeeFeatureID = srf.EmployeeFeatureID
		where isnull(poe.Permission,6) > 0
       
    END
GO

 
   
 ALTER PROCEDURE [dbo].[InsertRequestPassportPermission]    
    (@IDRequest int) AS /* Añadimos todos los pasaportes que tienen permisos para una solicitud a la tabla temporal */    
    BEGIN    
		DECLARE @pIDRequest int = @IDRequest

		DELETE FROM sysroPermissionsOverRequests where IDRequest = @pIDRequest
		
		INSERT INTO sysroPermissionsOverRequests
		select pof.PassportID as passportid, r.ID as IDRequest, pog.EmployeeGroupID from Requests r
		inner join sysroFeatures srf on srf.AliasID = r.RequestType
		inner join sysrovwCurrentEmployeeGroups ceg on ceg.IDEmployee = r.IDEmployee
		inner join sysroPermissionsOverFeatures pof on pof.FeatureID = srf.ID and pof.Permission > 3
		inner join sysroPermissionsOverGroups pog on pof.PassportID = pog.PassportID and pog.EmployeeGroupID = ceg.IDGroup and pog.Permission > 3 and pog.EmployeeFeatureID = srf.EmployeeFeatureID
		left join sysroPermissionsOverEmployeesExceptions poe on poe.PassportID = pof.PassportID and poe.EmployeeID = r.IDEmployee and poe.EmployeeFeatureID = srf.EmployeeFeatureID
		where r.ID = @pIDRequest and isnull(poe.Permission,6) > 0

	END
GO

ALTER PROCEDURE [dbo].[InsertPassportRequestsPermission]    
    (    
    @IDParentPassport int    
    )    
    AS    
    /* Al crear un nuevo pasaporte debemos añadir todos los permisos que tiene este sobre las peticiones */    
    BEGIN    
		DECLARE @pIDParentPassport int = @IDParentPassport  
		DELETE FROM sysroPermissionsOverRequests where IDParentPassport = @pIDParentPassport


		INSERT INTO sysroPermissionsOverRequests
			select pof.PassportID as passportid, r.ID as IDRequest, pog.EmployeeGroupID from Requests r
			inner join sysroFeatures srf on srf.AliasID = r.RequestType
			inner join sysrovwCurrentEmployeeGroups ceg on ceg.IDEmployee = r.IDEmployee
			inner join sysroPermissionsOverFeatures pof on pof.FeatureID = srf.ID and pof.Permission > 3
			inner join sysroPermissionsOverGroups pog on pof.PassportID = pog.PassportID and pog.EmployeeGroupID = ceg.IDGroup and pog.Permission > 3 and pog.EmployeeFeatureID = srf.EmployeeFeatureID
			left join sysroPermissionsOverEmployeesExceptions poe on poe.PassportID = pof.PassportID and poe.EmployeeID = r.IDEmployee and poe.EmployeeFeatureID = srf.EmployeeFeatureID
			where pof.PassportID = @pIDParentPassport and isnull(poe.Permission,6) > 0  
	END
GO

ALTER PROCEDURE [dbo].[AlterPassportRequestsPermission]    
    (    
    @IDParentPassport int    
    )    
    AS    
      /* Al modificar un pasaporte debemos actualizar todos los permisos que tiene este sobre las peticiones */    
    BEGIN    
		DECLARE @renewPassports table(IDPassport int)
		DECLARE @pIDParentPassport int = @IDParentPassport    

		/* Obtenemos todos los pasaportes de tipo grupo que pueden supervisar y que estan dentro del arbol de grupos del pasaporte a actualizar */    
		INSERT INTO @renewPassports(IDPassport)  
			SELECT ID from sysroPassports where GroupType = 'U' and ID NOT IN(SELECT IDPassport FROM dbo.GetAllPassportParentsEmployeeType()) and (ID in(SELECT ID FROM dbo.GetPassportChilds(@pIDParentPassport)) or ID = @pIDParentPassport)    

		DELETE FROM sysroPermissionsOverRequests where IDParentPassport in (SELECT IDPassport from @renewPassports)


		INSERT INTO sysroPermissionsOverRequests
			select pof.PassportID as passportid, r.ID as IDRequest, pog.EmployeeGroupID from Requests r
			inner join sysroFeatures srf on srf.AliasID = r.RequestType
			inner join sysrovwCurrentEmployeeGroups ceg on ceg.IDEmployee = r.IDEmployee
			inner join sysroPermissionsOverFeatures pof on pof.FeatureID = srf.ID and pof.Permission > 3
			inner join sysroPermissionsOverGroups pog on pof.PassportID = pog.PassportID and pog.EmployeeGroupID = ceg.IDGroup and pog.Permission > 3 and pog.EmployeeFeatureID = srf.EmployeeFeatureID
			left join sysroPermissionsOverEmployeesExceptions poe on poe.PassportID = pof.PassportID and poe.EmployeeID = r.IDEmployee and poe.EmployeeFeatureID = srf.EmployeeFeatureID
			where pof.PassportID in (SELECT IDPassport from @renewPassports) and isnull(poe.Permission,6) > 0  
    END
GO

ALTER PROCEDURE [dbo].[GenerateChangeDayRequestPassportPermission]  
    AS  
    BEGIN  
      
		/* Rellenamos una tabla  con todos los passaportes de tipo grupo y cada una de las peticiones sobre las que tiene permisos */  
		DECLARE @idRequest int  
		DECLARE @renewRequests table(IDRequest int)  
		INSERT INTO @renewRequests(IDRequest)  
		select Requests.Id from Requests  
			left outer join sysroPermissionsOverRequests on sysroPermissionsOverRequests.IDRequest = Requests.ID  
			left outer join sysrovwAllEmployeeGroups on Requests.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee   
			where (sysroPermissionsOverRequests.IDRequest is null) or (sysroPermissionsOverRequests.EmployeeGroupID <> sysrovwAllEmployeeGroups.IDGroup)  
  
		DELETE FROM sysroPermissionsOverRequests where IDRequest in (SELECT IDRequest from @renewRequests)  

		INSERT INTO sysroPermissionsOverRequests
		select pof.PassportID as passportid, r.ID as IDRequest, pog.EmployeeGroupID from Requests r
		inner join sysroFeatures srf on srf.AliasID = r.RequestType
		inner join sysrovwCurrentEmployeeGroups ceg on ceg.IDEmployee = r.IDEmployee
		inner join sysroPermissionsOverFeatures pof on pof.FeatureID = srf.ID and pof.Permission > 3
		inner join sysroPermissionsOverGroups pog on pof.PassportID = pog.PassportID and pog.EmployeeGroupID = ceg.IDGroup and pog.Permission > 3 and pog.EmployeeFeatureID = srf.EmployeeFeatureID
		left join sysroPermissionsOverEmployeesExceptions poe on poe.PassportID = pof.PassportID and poe.EmployeeID = r.IDEmployee and poe.EmployeeFeatureID = srf.EmployeeFeatureID
		where r.ID in (SELECT IDRequest from @renewRequests) and isnull(poe.Permission,6) > 0

	END
GO

ALTER PROCEDURE [dbo].[sysro_GenerateAllPermissionsOverGroups]  
	(   
	@Version int    
    )  
    AS  
    BEGIN  
		DECLARE @pVersion int = @Version
		DELETE FROM sysroPermissionsOverGroups 

		if (@pVersion = 3)  
		 -- si es seguridad v3  
		BEGIN
			INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission)   
			select supPassports.IDParentPassport, Groups.ID, features.EmployeeFeatureID, 1,  
			  isnull(dbo.sysro_GetPassportPermissionOverGroup(supPassports.IDParentPassport,Groups.ID,features.EmployeeFeatureID,0),0)  
			from sysroPassports supPassports, Groups, (SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null) as features  
			where supPassports.IsSupervisor = 1 
		END
		if (@pVersion = 2) 
		BEGIN
			INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission)   
			   select supPassports.ID, Groups.ID, features.EmployeeFeatureID, dbo.GetPassportLevelOfAuthority(supPassports.ID,0,0,1),  
				 isnull(dbo.sysro_GetPassportPermissionOverGroup(supPassports.ID,Groups.ID,features.EmployeeFeatureID,0),0)  
			   from sysroPassports supPassports, Groups, (SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null) as features  
			   where supPassports.GroupType = 'U' 
		END

		if (@pVersion <> 2 and @pVersion <> 3) 
		BEGIN
			INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission)   
			   select supPassports.ID, Groups.ID, features.EmployeeFeatureID, dbo.GetPassportLevelOfAuthority(supPassports.ID,0,0,1),  
				 dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,features.EmployeeFeatureID,0)  
			   from sysroPassports supPassports, Groups, (SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null) as features  
			   where supPassports.GroupType = 'U' 
		END

	END
GO

ALTER PROCEDURE [dbo].[InsertGroupPassportPermission]      
    (      
    @idGroup int  ,@Version int    
    )      
    AS      
    BEGIN      
		DECLARE @pidGroup int = @idGroup  , @pVersion int = @Version;      
		DELETE FROM sysroPermissionsOverGroups WHERE EmployeeGroupID = @pidGroup     
    	
		if (@pVersion = 3)    
			-- si es seguridad v3    
		BEGIN
			DELETE FROM [dbo].[sysroPassports_PermissionsOverGroups] WHERE IDGroup = @pidGroup  

			INSERT INTO [dbo].[sysroPassports_PermissionsOverGroups] (IDPassport, IDGroup, IDApplication, Permission)   
				select * from (select supPassports.IDParentPassport, Groups.ID as GroupID, features.EmployeeFeatureID as Feature,   
				(select Case When (select count(*) from sysroPassports_Groups where IDPassport = supPassports.ID and IDGroup IN (select * from SplitToInt(Groups.Path,'\')) ) > 0 Then 6 Else 0 End) as Permission  
				from sysroPassports supPassports, Groups, (SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null) as features
				where supPassports.IsSupervisor = 1 AND Groups.ID = @pidGroup) tmpPerm where tmpPerm.Permission = 0  
		   
			INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission)       
				select supPassports.ID, Groups.ID, features.EmployeeFeatureID, 1, isnull(dbo.sysro_GetPassportPermissionOverGroup(supPassports.ID,Groups.ID,features.EmployeeFeatureID,0),0)      
				from sysroPassports supPassports, Groups,(SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null) as features  
				where supPassports.GroupType = 'U' AND Groups.ID = @pidGroup      
		END    
		if (@pVersion <> 3)    
		-- si es seguridad v1 o v2    
		BEGIN
			if (@pVersion = 2)    
			-- si es seguridad v1 o v2    
			BEGIN
				INSERT INTO [dbo].[sysroPassports_PermissionsOverGroups] (IDPassport, IDGroup, IDApplication, Permission)   
						select IDParentPassport,@pidGroup,features.EmployeeFeatureID,0 from sysroPassports ,(SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null) as features
							where ID not in(
							select IDPassport from sysroSecurityNode_Passports where IDSecurityNode in(
								select * from SplitToInt((select path from sysroSecurityNodes where ID = (select idsecuritynode from sysroSecurityNode_Groups where IDGroup IN (select * from SplitToInt((select Path from groups where id = @pidGroup),'\')))),'\')
							)) and IsSupervisor = 1

				INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission)       
					select supPassports.ID, Groups.ID, features.EmployeeFeatureID, dbo.GetPassportLevelOfAuthority(supPassports.IDParentPassport,0,0,1),      
						isnull(dbo.sysro_GetPassportPermissionOverGroup(supPassports.ID,Groups.ID,features.EmployeeFeatureID,0),0)      
					from sysroPassports supPassports, Groups,(SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null) as features	
					where supPassports.GroupType = 'U' AND Groups.ID = @pidGroup
			END
			ELSE
			BEGIN
				INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission)       
					select supPassports.ID, Groups.ID, features.EmployeeFeatureID, dbo.GetPassportLevelOfAuthority(supPassports.ID,0,0,1),      
						dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,features.EmployeeFeatureID,0)      
					from sysroPassports supPassports, Groups,(SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null) as features	
					where supPassports.GroupType = 'U' AND Groups.ID = @pidGroup      
			END
		END    
		    
	END
GO


ALTER PROCEDURE [dbo].[sysro_UpdatePassportPermissionsOverGroups]    
	(    
	@idPassport int ,@Version int     
	)    
	AS    
	BEGIN    
    
		DECLARE @pidPassport int = @idPassport  , @pVersion int = @Version;  
		DECLARE @updatePassportIDs table(IDPassport int)    
		;WITH cte AS     
		(    
		SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType    
		FROM sysroPassports a    
		WHERE Id = @pidPassport and GroupType = 'U'    
		UNION ALL    
		SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType    
		FROM sysropassports a JOIN cte c ON a.IDParentPassport = c.id    
		where a.GroupType = 'U'    
		)    
		INSERT INTO @updatePassportIDs SELECT Id FROM cte    
    
		DELETE FROM [dbo].[sysroPermissionsOverGroups] where PassportID in (select IDPassport from @updatePassportIDs)  
		if (@pVersion = 3)  
		 -- si es seguridad v3  
		BEGIN
			INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission)   
				select supPassports.ID, Groups.ID, features.EmployeeFeatureID, 1,  
				  isnull(dbo.sysro_GetPassportPermissionOverGroup(supPassports.ID,Groups.ID,features.EmployeeFeatureID,0),0)  
				from sysroPassports supPassports, Groups, (SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null) as features  
				where supPassports.GroupType = 'U' and supPassports.id in(select IDPassport from @updatePassportIDs)
		END
		if (@pVersion <> 3) 
		BEGIN
			if (@pVersion = 2) 
			BEGIN
				INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission)   
				   select supPassports.ID, Groups.ID, features.EmployeeFeatureID, dbo.GetPassportLevelOfAuthority(supPassports.ID,0,0,1),  
					 isnull(dbo.sysro_GetPassportPermissionOverGroup(supPassports.ID,Groups.ID,features.EmployeeFeatureID,0),0)  
				   from sysroPassports supPassports, Groups, (SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null) as features  
				   where supPassports.GroupType = 'U' and supPassports.id in(select IDPassport from @updatePassportIDs)
			END
			ELSE
			BEGIN
				INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission)   
				   select supPassports.ID, Groups.ID, features.EmployeeFeatureID, dbo.GetPassportLevelOfAuthority(supPassports.ID,0,0,1),  
					 dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,features.EmployeeFeatureID,0)  
				   from sysroPassports supPassports, Groups, (SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null) as features  
				   where supPassports.GroupType = 'U' and supPassports.id in(select IDPassport from @updatePassportIDs)
			END
		END
   END
GO

ALTER PROCEDURE [dbo].[sysro_GenerateAllPermissionsOverFeatures]  
     AS  
     BEGIN  
      
		DELETE FROM sysroPermissionsOverFeatures  
		
		INSERT INTO [dbo].[sysroPermissionsOverFeatures] (PassportID, FeatureID, Permission)   
			select supPassports.ID, features.ID, dbo.sysro_GetPermissionOverFeature(supPassports.ID,features.Alias,features.Type,0)  
			from sysroPassports supPassports, (select distinct ID,Alias,Type from sysroFeatures WHERE Type = 'U') features
			where supPassports.GroupType = 'U'   order by supPassports.id
         
    END
GO


ALTER PROCEDURE [dbo].[sysro_UpdatePassportPermissionsOverFeatures]    
	(    
	@idPassport int    
	)    
    AS    
    BEGIN    
		DECLARE @pidPassport int = @idPassport     
		DECLARE @updatePassportIDs table(IDPassport int)    
		;WITH cte AS     
		(    
		SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType    
		FROM sysroPassports a    
		WHERE Id = @pidPassport and GroupType = 'U'    
		UNION ALL    
		SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType    
		FROM sysropassports a JOIN cte c ON a.IDParentPassport = c.id    
		where a.GroupType = 'U'    
		)    
		INSERT INTO @updatePassportIDs SELECT Id FROM cte    

		DELETE FROM [dbo].[sysroPermissionsOverFeatures] where PassportID in (select IDPassport from @updatePassportIDs)  

		INSERT INTO [dbo].[sysroPermissionsOverFeatures] (PassportID, FeatureID, Permission)     
			select supPassports.ID, features.ID, dbo.sysro_GetPermissionOverFeature(supPassports.ID,features.Alias,features.Type,0)    
			from sysroPassports supPassports, (select distinct ID,Alias,Type from sysroFeatures WHERE Type = 'U') features
			where supPassports.GroupType = 'U' and supPassports.id in(select IDPassport from @updatePassportIDs)      
	END
GO

DECLARE @sql NVARCHAR(MAX)
WHILE 1=1
BEGIN
    SELECT TOP 1 @sql = N'alter table [dbo].[sysroSchedulerViews] drop constraint ['+dc.NAME+N']'
    from sys.default_constraints dc
    JOIN sys.columns c
        ON c.default_object_id = dc.object_id
    WHERE 
        dc.parent_object_id = OBJECT_ID('sysroSchedulerViews')
    AND c.name = N'Employees'
    IF @@ROWCOUNT = 0 BREAK
    EXEC (@sql)
END
GO

ALTER TABLE [dbo].[sysroSchedulerViews] ALTER COLUMN Employees nvarchar(max)
GO
ALTER TABLE [dbo].[sysroSchedulerViews] ADD  DEFAULT ('') FOR [Employees]
GO   
  CREATE PROCEDURE  [dbo].[Analytics_Biometric]      
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
       select * from (  select CAST(emp.ID AS varchar) + '-' + CAST(eg.IDGroup AS varchar) + '-' + ec.IDContract AS KeyView,     
    emp.Id as IDEmployee, emp.Name as EmployeeName, spam.method as Method, spam.credential as Credential, spam.version as Version,     
    CASE    
   WHEN len(CONVERT(VARCHAR(MAX), CONVERT(VARBINARY(MAX),spam.BiometricData))) > 0 THEN 'X'    
  ELSE ''    
  END AS BiometricData,    
    spam.Enabled as Enabled, spam.TimeStamp as TimeStamp,    
    t.Description as Terminal, spam.BiometricAlgorithm as BiometricAlgorithm,     
g.name as GroupName, g.FullGroupName, ec.IDContract, ec.BeginDate as BeginContract, ec.EndDate as EndContract, eg.BeginDate as BeginDate, eg.EndDate as EndDate,  
   dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,2,'\') As Nivel2,      
     dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,4,'\') As Nivel4,      
     dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,6,'\') As Nivel6,      
     dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,8,'\') As Nivel8,      
     dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,10,'\') As Nivel10,      
        dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),GetDate()) As UserField1,      
     dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),GetDate()) As UserField2,      
     dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),GetDate()) As UserField3,      
     dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),GetDate()) As UserField4,      
     dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),GetDate()) As UserField5,      
     dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),GetDate()) As UserField6,      
     dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),GetDate()) As UserField7,      
     dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),GetDate()) As UserField8,      
     dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),GetDate()) As UserField9,      
     dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),GetDate()) As UserField10    
       FROM         employees emp    
inner join sysroPassports sp on sp.IDEmployee = emp.id    
inner join sysroPassports_AuthenticationMethods spam on spam.IDPassport = sp.ID    
inner join EmployeeContracts ec on emp.id = ec.IDEmployee    
inner join sysroEmployeeGroups eg on eg.IDEmployee = emp.id    
inner join Groups g on g.id = eg.IDGroup    
left join Terminals t on t.id = spam.BiometricTerminalId    
    WHERE   dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.Id,2,0,0,GetDate()) > 1      
     AND emp.Id in( select idEmployee from @employeeIDs) and @pinitialDate >= ec.BeginDate and @pendDate <= ec.EndDate  and Version != 'RXA200'  ) as query
	 group by keyview, IDEmployee, employeename, method, credential, version, biometricdata,  enabled, timestamp, terminal, BiometricAlgorithm, groupname, FullGroupName,idcontract, 
	 begincontract, endcontract, 
	 begindate, enddate, nivel1, nivel2, nivel3, nivel4, nivel5, nivel6, nivel7, nivel8, nivel9, nivel10, UserField1, UserField2, UserField3, UserField4, UserField5, UserField6, UserField7, UserField8,
	 UserField9, UserField10
GO
 alter PROCEDURE [dbo].[Analytics_Concepts]  
       @initialDate smalldatetime,  
      @endDate smalldatetime,  
      @idpassport int,  
      @employeeFilter nvarchar(max),  
      @conceptsFilter nvarchar(max),  
      @userFieldsFilter nvarchar(max),  
      @includeZeros tinyint  
      AS  
      DECLARE @employeeIDs Table(idEmployee int)  
      DECLARE @conceptIDs Table(idConcept int)  
     DECLARE @pinitialDate smalldatetime = @initialDate,  
      @pendDate smalldatetime = @endDate,  
      @pidpassport int = @idpassport,  
      @pemployeeFilter nvarchar(max) = @employeeFilter,  
      @pconceptsFilter nvarchar(max) = @conceptsFilter,  
      @puserFieldsFilter nvarchar(max) = @userFieldsFilter,  
      @pincludeZeros tinyint = @includeZeros  
  
  DECLARE @intdatefirst int  
  SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'  
  SET DateFirst @intdatefirst;  
  
      insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate  
      insert into @conceptIDs select * from dbo.SplitToInt(@pconceptsFilter,',');  
      IF @pincludeZeros = 1   
       BEGIN  
        WITH alldays AS (  
         SELECT @initialDate AS dt  
         UNION ALL  
         SELECT DATEADD(dd, 1, dt)  
        FROM alldays s  
          WHERE DATEADD(dd, 1, dt) <= @pendDate)  
      SELECT CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120), 10) AS KeyView,   
        reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept,   
        dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName,   
        reqReg.conceptName AS ConceptName, reqReg.dt AS Date, sum(isnull(dbo.DailyAccruals.Value,0)) AS Value,COUNT(*) AS Count, MONTH(reqReg.dt) AS Mes,   
        YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week,   
        reqReg.dt) AS WeekOfYear, DATEPART(dy, reqReg.dt) AS DayOfYear, dbo.sysroEmployeeGroups.Path,   
        dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
        dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @pendDate AS EndPeriod,  
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
        dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),reqReg.dt) As UserField10  
      FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,   
         (select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp,   
         (select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con  
         where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1  
        ) reqReg  
        INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate  
        INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate  
        LEFT OUTER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept  
      GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName,   
        dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt),   
        YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy,   
        reqReg.dt), CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar)   
        + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120),   
        10), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
        dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  
      option (maxrecursion 0)  
      END  
      ELSE  
       BEGIN  
       WITH alldays AS (  
         SELECT @initialDate AS dt  
         UNION ALL  
         SELECT DATEADD(dd, 1, dt)  
        FROM alldays s  
          WHERE DATEADD(dd, 1, dt) <= @pendDate)  
      SELECT CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120), 10) AS KeyView,   
        reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept,   
        dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName,   
        reqReg.conceptName AS ConceptName, reqReg.dt AS Date, sum(isnull(dbo.DailyAccruals.Value,0)) AS Value, COUNT(*) AS Count, MONTH(reqReg.dt) AS Mes,   
        YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week,   
        reqReg.dt) AS WeekOfYear, DATEPART(dy, reqReg.dt) AS DayOfYear, dbo.sysroEmployeeGroups.Path,   
        dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
        dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @pendDate AS EndPeriod,  
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
        dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),reqReg.dt) As UserField10  
      FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,   
         (select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp,   
         (select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con  
         where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1  
        ) reqReg  
        INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate  
        INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate  
        INNER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept  
      GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName,   
        dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt),   
        YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy,   
        reqReg.dt), CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar)   
        + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120),   
        10), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
        dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  
      option (maxrecursion 0)  
      END  
GO
 ALTER PROCEDURE [dbo].[Analytics_CostCenters]  
        @initialDate smalldatetime,  
        @endDate smalldatetime,  
        @idpassport int,  
        @userFieldsFilter nvarchar(max),  
     @businessCenterFilter nvarchar(max)  
        AS  
     DECLARE @businessCenterIDs Table(idBusinessCenter int)  
      declare @pinitialDate smalldatetime = @initialDate,    
       @pendDate smalldatetime = @endDate,    
       @pidpassport int = @idpassport,    
       @pbusinessCenterFilter nvarchar(max) = @businessCenterFilter,    
       @puserFieldsFilter nvarchar(max) = @userFieldsFilter    
	   	DECLARE @intdatefirst int
		SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'
		SET DateFirst @intdatefirst;
     insert into @businessCenterIDs select * from dbo.SplitToInt(@pbusinessCenterFilter,',');  
         SELECT     CONVERT(varchar(10), dbo.DailyCauses.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)   
                               + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailyCauses.Date, 120) AS KeyView, dbo.DailyCauses.Date,  
                               dbo.Causes.Name AS CauseName, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor,  isnull(dbo.BusinessCenters.ID, 0) as IDCenter,   
             isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,    
             isnull(dbo.BusinessCenters.Field4, '') AS Field4, isnull(dbo.BusinessCenters.Field5, '') AS Field5, dbo.DailyCauses.Value AS Value, YEAR(dbo.DailyCauses.Date) AS Año,   
                               (DATEPART(dw, dbo.DailyCauses.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, dbo.DailyCauses.Date) AS WeekOfYear, DATEPART(dy,   
                               dbo.DailyCauses.Date) AS DayOfYear, dbo.DailyCauses.DefaultCenter, dbo.DailyCauses.ManualCenter ,                              
             dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroCostHour'),DailyCauses.date ) as Cost,   
             dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPriceHour'), DailyCauses.date) as PVP  
         FROM         dbo.sysroEmployeeGroups with (nolock)   
                               INNER JOIN dbo.Causes with (nolock)   
                               INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause  ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.DailyCauses.Date between dbo.sysroEmployeeGroups.BeginDate AND dbo
  .sysroEmployeeGroups.EndDate   
             INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyCauses.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyCauses.Date <= dbo.EmployeeContracts.EndDate   
             INNER JOIN dbo.Groups with (nolock) ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID   
             LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.Employees.ID    
             LEFT OUTER JOIN dbo.BusinessCenters with (nolock) ON isnull(dbo.DailyCauses.IDCenter,0) = dbo.BusinessCenters.ID   
          where     isnull(dbo.DailyCauses.IDCenter,0) in (select idBusinessCenter from @businessCenterIDs)   
 		 and isnull(dbo.DailyCauses.IDCenter,0) in (select idcenter from sysroPassports_Centers where IDPassport = (select idparentpassport from sysroPassports where id = @pidpassport))
       and dbo.dailycauses.date between @pinitialDate and @pendDate 
GO
 ALTER PROCEDURE [dbo].[Analytics_CostCenters_Detail]
       @initialDate smalldatetime,  
       @endDate smalldatetime,  
       @idpassport int,  
    @employeeFilter nvarchar(max),  
       @userFieldsFilter nvarchar(max),  
    @businessCenterFilter nvarchar(max)  
       AS  
    DECLARE @employeeIDs Table(idEmployee int)  
    DECLARE @pinitialDate smalldatetime = @initialDate,    
       @pendDate smalldatetime = @endDate,    
       @pidpassport int = @idpassport,    
       @pemployeeFilter nvarchar(max) = @employeeFilter,    
       @puserFieldsFilter nvarchar(max) = @userFieldsFilter,
  	 @pbusinessCenterFilter nvarchar(max) = @businessCenterFilter
    insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
    DECLARE @businessCenterIDs Table(idBusinessCenter int)  
    insert into @businessCenterIDs select * from dbo.SplitToInt(@pbusinessCenterFilter,',');  
        SELECT     CONVERT(varchar(10), dbo.DailyCauses.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)   
                              + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailyCauses.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup,   
                              dbo.EmployeeContracts.IDContract, dbo.DailyCauses.IDEmployee, dbo.DailyCauses.Date,  
                              dbo.Causes.Name AS CauseName, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor, isnull(dbo.BusinessCenters.ID, 0) as IDCenter,   
            isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,    
            isnull(dbo.BusinessCenters.Field4, '') AS Field4, isnull(dbo.BusinessCenters.Field5, '') AS Field5, dbo.DailyCauses.Value AS Value, YEAR(dbo.DailyCauses.Date) AS Año,   
                              (DATEPART(dw, dbo.DailyCauses.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, dbo.DailyCauses.Date) AS WeekOfYear, DATEPART(dy,   
                              dbo.DailyCauses.Date) AS DayOfYear, dbo.DailyCauses.DefaultCenter, dbo.DailyCauses.ManualCenter ,   
                              dbo.Employees.Name AS EmployeeName, dbo.Groups.Name AS GroupName, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName, dbo.Groups.Path,   
                              dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,   
            dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroCostHour'),DailyCauses.date ) as Cost,   
            dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPriceHour'), DailyCauses.date) as PVP,  
               dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,  
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
            dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.DailyCauses.Date) As UserField10  
        FROM         dbo.sysroEmployeeGroups with (nolock)   
                              INNER JOIN dbo.Causes with (nolock)   
                              INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause  ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.DailyCauses.Date between dbo.sysroEmployeeGroups.BeginDate AND dbo.
  sysroEmployeeGroups.EndDate   
            INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyCauses.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyCauses.Date <= dbo.EmployeeContracts.EndDate   
            INNER JOIN dbo.Groups with (nolock) ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID   
            LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.Employees.ID    
            LEFT OUTER JOIN dbo.BusinessCenters with (nolock) ON ISNULL(dbo.DailyCauses.IDCenter,0) = dbo.BusinessCenters.ID   
         where       
          dbo.dailycauses.date between @pinitialDate and @pendDate  
       and  isnull(dbo.DailyCauses.IDCenter,0) in (select idBusinessCenter from @businessCenterIDs)   
       AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs)
GO
 ALTER PROCEDURE [dbo].[Analytics_Incidences]  
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
        SELECT     CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)   
                              + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup,   
                              dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description AS IncidenceName,   
                              dbo.TimeZones.Name AS ZoneTime, dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS Value, YEAR(dbo.DailySchedule.Date) AS Año,   
                              (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, dbo.DailySchedule.Date) AS WeekOfYear, DATEPART(dy,   
                              dbo.DailySchedule.Date) AS DayOfYear, CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName,   
                              dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.FullGroupName AS FullGroupName, dbo.sysroEmployeeGroups.Path,   
                              dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,  
             dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod, dbo.DailySchedule.IDAssignment,  
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
          convert(nvarchar(max),dbo.sysroRemarks.Text) as Remark  
        FROM         dbo.sysroEmployeeGroups with (nolock)  
            INNER JOIN dbo.Causes with (nolock)  
            INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause   
            INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailySchedule.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate  
            INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailySchedule.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailySchedule.Date between dbo.EmployeeContracts.BeginDate AND dbo.EmployeeContracts.EndDate   
            LEFT OUTER JOIN dbo.sysroDailyIncidencesDescription with (nolock)  
            INNER JOIN dbo.DailyIncidences with (nolock)  
            INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID ON dbo.sysroDailyIncidencesDescription.IDIncidence = dbo.DailyIncidences.IDType ON dbo.DailyCauses.IDEmployee = dbo.DailyIncidences.IDEmployee AND dbo.DailyCauses.Date = dbo.DailyIncidences.Date AND dbo.DailyCauses.IDRelatedIncidence = dbo.DailyIncidences.ID AND dbo.DailyIncidences.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate  
            LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.DailySchedule.IDShiftUsed = dbo.Shifts.ID   
            LEFT OUTER JOIN dbo.Shifts AS Shifts_1 with (nolock) ON dbo.DailySchedule.IDShift1 = Shifts_1.ID  
            LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID  
         LEFT OUTER JOIN dbo.sysroRemarks on dbo.sysroRemarks.ID = dbo.DailySchedule.Remarks  
       WHERE dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.DailyCauses.IDEmployee,2,0,0,dbo.DailyCauses.date) > 1  
          AND dbo.sysroEmployeeGroups.IDEmployee in(select idEmployee from @employeeIDs)   
         and dbo.dailyCauses.Date between @pinitialDate and @pendDate  
        GROUP BY dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name,   
                              YEAR(dbo.DailySchedule.Date), CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)   
                  + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120), CASE WHEN DailySchedule.Date <= GETDATE()   
                              THEN Shifts.Name ELSE Shifts_1.Name END, dbo.Employees.Name, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, DATEPART(iso_week,   
                              dbo.DailySchedule.Date), DATEPART(dy, dbo.DailySchedule.Date), (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1,   
                              dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,   
                              dbo.sysroEmployeeGroups.CurrentEmployee, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate, dbo.DailySchedule.IDAssignment, dbo.DailySchedule.IDDailyBudgetPosition, convert(nvarchar(max),dbo.sysroRemarks.Text)
GO
 ALTER PROCEDURE  [dbo].[Analytics_Schedule]
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
   		dbo.sysroEmployeesShifts.CurrentDate AS Date, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours,SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHoursAbs) as HoursAbs, COUNT(*) AS Count, 
   		MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año, (DATEPART(dw, 
   		dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear, 
   		DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, 
   		dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.endDate,
   		dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.endDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,
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
 		(SELECT LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = sysroEmployeesShifts.IDEmployee) as LockDate
       FROM         dbo.sysroEmployeesShifts with (nolock) 
   		INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.sysroEmployeeGroups.endDate 
   		INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.EmployeeContracts.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.EmployeeContracts.endDate 
   		LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
   	WHERE  dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeesShifts.IDEmployee,2,0,0,dbo.sysroEmployeesShifts.CurrentDate) > 1
   		AND dbo.sysroEmployeesShifts.IDEmployee in( select idEmployee from @employeeIDs) and dbo.sysroEmployeesShifts.CurrentDate between @pinitialDate and @pendDate
   	GROUP BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name, 
   		dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate, MONTH(dbo.sysroEmployeesShifts.CurrentDate), 
   		YEAR(dbo.sysroEmployeesShifts.CurrentDate), (DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, 
   		dbo.sysroEmployeesShifts.CurrentDate), DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate), dbo.sysroEmployeeGroups.IDGroup, 
   		dbo.EmployeeContracts.IDContract, CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
   		+ '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10), dbo.sysroEmployeeGroups.Path, 
   		dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.endDate,
   		dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.endDate,dbo.sysroEmployeesShifts.IDAssignment,dbo.sysroEmployeesShifts.AssignmentName,dbo.sysroEmployeesShifts.IDProductiveUnit,dbo.sysroEmployeesShifts.ProductiveUnitName, convert(nvarchar(max),dbo.sysroEmployeesShifts.Remark)
GO
UPDATE sysroLiveAdvancedParameters SET Value = 12 WHERE ParameterName='VTPortalApiVersion'
GO
UPDATE sysroParameters SET Data='510' WHERE ID='DBVersion'
GO
