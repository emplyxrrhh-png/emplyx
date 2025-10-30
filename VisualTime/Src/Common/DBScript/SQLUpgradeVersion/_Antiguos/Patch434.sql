 ALTER PROCEDURE [dbo].[ExecuteRequestPassportPermissionsAction]
     (
   	@IDAction int,
     	@IDObject int
     )
     AS
     /* Añadimos todos los pasaportes que tienen permisos para una solicitud a la tabla temporal */
  	BEGIN
	DECLARE @pIDAction int = @IDAction,  
      @pIDObject int = @IDObject

  		IF @pIDAction = -2
  		BEGIN
 			exec dbo.sysro_GenerateAllPermissionsOverGroups
 			exec dbo.sysro_GenerateAllPermissionsOverFeatures
  			exec dbo.sysro_GenerateAllPermissionsOverEmployeesExceptions
 			exec dbo.GenerateAllRequestPassportPermission
  		END
 		IF @pIDAction = -1 -- Cambio de dia
  		BEGIN
 			exec dbo.GenerateChangeDayRequestPassportPermission
  		END
  		IF @pIDAction = 0 -- Creación passport
  		BEGIN
  			exec dbo.InsertPassportRequestsPermission @pIDObject
 			exec dbo.sysro_UpdatePassportPermissionsOverGroups @pIDObject
 			exec dbo.sysro_UpdatePassportPermissionsOverFeatures @pIDObject
 			exec dbo.sysro_UpdatePassportPermissionsOverEmployeesExceptions @pIDObject
  		END
  		IF @pIDAction = 1 -- Modificación passport
  		BEGIN
  			exec dbo.AlterPassportRequestsPermission @pIDObject
 			exec dbo.sysro_UpdatePassportPermissionsOverGroups @pIDObject
 			exec dbo.sysro_UpdatePassportPermissionsOverFeatures @pIDObject
 			exec dbo.sysro_UpdatePassportPermissionsOverEmployeesExceptions @pIDObject
  		END
  		IF @pIDAction = 2 -- Creación solicitud
  		BEGIN
  			exec dbo.InsertRequestPassportPermission @pIDObject
  		END
  		IF @pIDAction = 3 -- Creación grupo de empleados
  		BEGIN
  			exec dbo.InsertGroupPassportPermission @pIDObject
  		END
  	END	
go

 alter PROCEDURE [dbo].[InsertPassportRequestsPermission]  
     (  
      @IDParentPassport int  
     )  
     AS  
     /* Al crear un nuevo pasaporte debemos añadir todos los permisos que tiene este sobre las peticiones */  
     BEGIN  
      DECLARE @idRequest int,  
      @featureID int,  
      @employeeID int,  
      @alias varchar(100),  
     @bInsert int,
	 @pIDParentPassport int = @IDParentPassport
      
    /* Obtenemos todas las solicitudes con sus datos de features */  
     DECLARE db_cursorInsertRequestPassportPermission CURSOR FOR    
    SELECT Requests.ID, Requests.IDEmployee, sysroFeatures.Alias, sysroFeatures.EmployeeFeatureID  
    FROM Requests inner join sysroFeatures on sysroFeatures.AliasID = Requests.RequestType  
    /* Borramos los datos actuales del passport */  
    DELETE FROM sysroPermissionsOverRequests WHERE IDParentPassport = @pIDParentPassport  
    OPEN db_cursorInsertRequestPassportPermission    
      FETCH NEXT FROM db_cursorInsertRequestPassportPermission INTO @idRequest, @employeeID, @alias , @featureID  
      WHILE @@FETCH_STATUS = 0    
      BEGIN  
      /* Insertamos en el caso que sea necesario sobre la tabla de permisos */  
      if dbo.WebLogin_GetPermissionOverFeature(@pIDParentPassport,@alias, 'U', 0) > 3   
       AND dbo.WebLogin_GetPermissionOverEmployee(@pIDParentPassport,@employeeID,@featureID,2,1,getdate()) > 3   
      begin  
      SET @bInsert = (select count(*) from sysroPassports where isnull(GroupType,'') <> 'U' and IDParentPassport = @pIDParentPassport)  
      if @bInsert > 0  
      begin  
        INSERT INTO sysroPermissionsOverRequests VALUES (@pIDParentPassport, @idRequest,dbo.GetEmployeeGroup(@employeeID,getdate()))  
      end  
      end  
                
       FETCH NEXT FROM db_cursorInsertRequestPassportPermission INTO  @idRequest, @employeeID, @alias , @featureID  
      END  
     CLOSE db_cursorInsertRequestPassportPermission  
     DEALLOCATE db_cursorInsertRequestPassportPermission  
     END
go

 alter PROCEDURE [dbo].[sysro_UpdatePassportPermissionsOverGroups]  
  (  
   @idPassport int  
  )  
     AS  
     BEGIN  
      
  DECLARE @featureEmployeeID int, 
  @pidPassport int = @idPassport  

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
  DELETE FROM sysroPermissionsOverGroups where PassportID in (select IDPassport from @updatePassportIDs)  
   DECLARE EmployeeFeatureCursor CURSOR  
   FOR   
    SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null  
   OPEN EmployeeFeatureCursor  
   FETCH NEXT FROM EmployeeFeatureCursor INTO @featureEmployeeID  
   WHILE @@FETCH_STATUS = 0  
   BEGIN   
      
    INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission)   
      select supPassports.ID, Groups.ID, @featureEmployeeID, dbo.GetPassportLevelOfAuthority(supPassports.ID),  
        dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,@featureEmployeeID,0)  
      from sysroPassports supPassports, Groups  
      where supPassports.GroupType = 'U' and supPassports.id in(select IDPassport from @updatePassportIDs)  
      
    FETCH NEXT FROM EmployeeFeatureCursor   
    INTO @featureEmployeeID  
   END   
   CLOSE EmployeeFeatureCursor  
   DEALLOCATE EmployeeFeatureCursor  
         
     END
go
	 
	   
 alter PROCEDURE [dbo].[sysro_UpdatePassportPermissionsOverFeatures]  
  (  
   @idPassport int  
  )  
     AS  
     BEGIN  
      
  DECLARE @featureID int,
  @pidPassport int = @idPassport  
  DECLARE @featureAlias nvarchar(100)  
   DECLARE @featureType varchar(1)  
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
  DELETE FROM sysroPermissionsOverFeatures where PassportID in (select IDPassport from @updatePassportIDs)  
   DECLARE EmployeeFeatureCursor CURSOR  
   FOR SELECT distinct ID,Alias,Type from sysroFeatures WHERE Type = 'U'  
   OPEN EmployeeFeatureCursor  
   FETCH NEXT FROM EmployeeFeatureCursor INTO @featureID,@featureAlias,@featureType  
   WHILE @@FETCH_STATUS = 0  
   BEGIN   
      
    INSERT INTO [dbo].[sysroPermissionsOverFeatures] (PassportID, FeatureID, Permission)   
      select supPassports.ID, @featureID, dbo.sysro_GetPermissionOverFeature(supPassports.ID,@featureAlias,@featureType,0)  
      from sysroPassports supPassports  
      where supPassports.GroupType = 'U' and supPassports.id in(select IDPassport from @updatePassportIDs)  
      
    FETCH NEXT FROM EmployeeFeatureCursor   
    INTO @featureID,@featureAlias,@featureType  
   END   
   CLOSE EmployeeFeatureCursor  
   DEALLOCATE EmployeeFeatureCursor  
       
 END
go
 
   
 alter FUNCTION [dbo].[sysro_GetPermissionOverFeature]   
   (  
    @idPassport int,  
    @featureAlias nvarchar(100),  
    @featureType varchar(1),  
    @mode int  
   )  
  RETURNS int  
  AS  
  BEGIN  
   DECLARE @Result int,
    @pidPassport int = @idPassport,  
    @pfeatureAlias nvarchar(100) = @featureAlias,  
    @pfeatureType varchar(1) = @featureType,  
    @pmode int = @mode  
     
   /* Get list of passports to check */  
   DECLARE @Passports table(ID int)  
   IF @pmode <> 2  
    INSERT INTO @Passports VALUES (@pidPassport)  
   IF @pmode <> 1  
    INSERT INTO @Passports SELECT ID FROM dbo.GetPassportParents(@pidPassport)  
     
   /* Get feature ID */  
   DECLARE @IDFeature int  
   SELECT @IDFeature = ID  
   FROM sysroFeatures  
   WHERE Alias = @pfeatureAlias AND  
    Type = @pfeatureType  
     
   /* Check permissions */  
   SELECT TOP 1 @Result = Permission  
   FROM sysroPassports_PermissionsOverFeatures  
   WHERE IDPassport IN (SELECT * FROM @Passports) AND  
    IDFeature = @IDFeature   
   ORDER BY Permission  
     
   /* Return result */  
   IF @Result IS NULL  
    SET @Result = 0  
   RETURN @Result  
  END
go
  
  
      
 alter PROCEDURE [dbo].[sysro_UpdatePassportPermissionsOverEmployeesExceptions]  
   (  
    @idPassport int  
   )  
      AS  
      BEGIN  
   DECLARE @pidPassport int = @idPassport    
   DECLARE @updatePassportIDs table(IDPassport int)  
   ;WITH cteUpdate AS   
   (  
   SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType  
   FROM sysroPassports a  
   WHERE Id = @pidPassport and GroupType = 'U'  
   UNION ALL  
   SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType  
   FROM sysropassports a JOIN cteUpdate c ON c.IDParentPassport = a.id  
   where a.GroupType = 'U'  
   )  
   INSERT INTO @updatePassportIDs SELECT Id FROM cteUpdate  
   DELETE FROM sysroPermissionsOverEmployeesExceptions where PassportID in (select IDPassport from @updatePassportIDs)  
    DECLARE @parentPassportID int  
   DECLARE @employeeID int  
   DECLARE @applicationID int  
   DECLARE @permission int  
     
   DECLARE @insertPassportIDs table(IDPassport int)  
   DECLARE @passportID int  
   DECLARE @oldPermission int  
    DECLARE ExceptionsCursor CURSOR FOR   
     SELECT IDPassport,IDEmployee,IDApplication,Permission FROM sysroPassports_PermissionsOverEmployees WHERE IDPassport in (select IDPassport from @updatePassportIDs)  
    OPEN ExceptionsCursor  
    FETCH NEXT FROM ExceptionsCursor INTO @parentPassportID,@employeeID,@applicationID,@permission  
    WHILE @@FETCH_STATUS = 0  
    BEGIN   
       
    ;WITH cteInsert AS   
    (  
    SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType  
    FROM sysroPassports a  
    WHERE Id = @parentPassportID and GroupType = 'U'  
    UNION ALL  
    SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType  
    FROM sysropassports a JOIN cteInsert c ON a.IDParentPassport = c.id  
    where a.GroupType = 'U'  
    )  
    INSERT INTO @insertPassportIDs SELECT Id FROM cteInsert  
    DECLARE InsertPassportsCursor CURSOR FOR  
     SELECT IDPassport from @insertPassportIDs  
    OPEN InsertPassportsCursor  
    FETCH NEXT FROM InsertPassportsCursor INTO @passportID  
    WHILE @@FETCH_STATUS = 0  
    BEGIN  
       
       
     IF EXISTS (SELECT Permission FROM sysroPermissionsOverEmployeesExceptions WHERE PassportID = @passportID AND EmployeeFeatureID = @applicationID AND EmployeeID = @employeeID)  
     BEGIN  
      SELECT @oldPermission = isnull(Permission,-1) FROM sysroPermissionsOverEmployeesExceptions WHERE PassportID = @passportID AND EmployeeFeatureID = @applicationID AND EmployeeID = @employeeID  
      IF @oldPermission > -1  
       IF @oldPermission > @permission  
          UPDATE [dbo].[sysroPermissionsOverEmployeesExceptions] SET Permission = @permission WHERE PassportID = @passportID AND EmployeeFeatureID = @applicationID AND EmployeeID = @employeeID  
     END  
       ELSE  
     BEGIN  
        INSERT INTO [dbo].[sysroPermissionsOverEmployeesExceptions] (PassportID, EmployeeID, EmployeeFeatureID, Permission) VALUES (@passportID,@employeeID,@applicationID,@permission)  
     END  
       
     FETCH NEXT FROM InsertPassportsCursor INTO @passportID  
    END  
    CLOSE InsertPassportsCursor  
     DEALLOCATE InsertPassportsCursor  
       
    DELETE FROM @insertPassportIDs  
     FETCH NEXT FROM ExceptionsCursor INTO @parentPassportID,@employeeID,@applicationID,@permission  
    END   
    CLOSE ExceptionsCursor  
    DEALLOCATE ExceptionsCursor  
        
  END
go
  
   
 alter PROCEDURE [dbo].[AlterPassportRequestsPermission]  
      (  
       @IDParentPassport int  
      )  
     AS  
     /* Al modificar un pasaporte debemos actualizar todos los permisos que tiene este sobre las peticiones */  
     BEGIN  
      DECLARE @idPassport int,  
      @idRequest int,  
      @featureID int,  
      @employeeID int,  
      @alias varchar(100),  
     @bInsert int,
	 @pIDParentPassport int = @IDParentPassport  
    /* Obtenemos todos los pasaportes de tipo grupo que pueden supervisar y que estan dentro del arbol de grupos del pasaporte a actualizar */  
    DECLARE db_cursor CURSOR FOR    
    SELECT ID from sysroPassports where GroupType = 'U' and ID NOT IN(SELECT IDPassport FROM dbo.GetAllPassportParentsEmployeeType()) and (ID in(SELECT ID FROM dbo.GetPassportChilds(@pIDParentPassport)) or ID = @pIDParentPassport)  
       
     OPEN db_cursor    
     FETCH NEXT FROM db_cursor INTO @idPassport    
     WHILE @@FETCH_STATUS = 0    
     BEGIN    
     /* Borramos los datos actuales del passport */  
     DELETE FROM sysroPermissionsOverRequests WHERE IDParentPassport = @idPassport  
     /* Obtenemos todas las solicitudes con sus datos de features */  
       DECLARE db_cursorInsertRequestPassportPermission CURSOR FOR    
     SELECT Requests.ID, Requests.IDEmployee, sysroFeatures.Alias, sysroFeatures.EmployeeFeatureID  
     FROM Requests inner join sysroFeatures on sysroFeatures.AliasID = Requests.RequestType  WHERE Requests.Status in (0,1)  
     OPEN db_cursorInsertRequestPassportPermission    
       FETCH NEXT FROM db_cursorInsertRequestPassportPermission INTO @idRequest, @employeeID, @alias , @featureID  
       WHILE @@FETCH_STATUS = 0    
       BEGIN  
       /* Insertamos en el caso que sea necesario sobre la tabla de permisos */  
       if dbo.WebLogin_GetPermissionOverFeature(@idPassport,@alias, 'U', 0) > 3   
        AND dbo.WebLogin_GetPermissionOverEmployee(@idPassport,@employeeID,@featureID,2,1,getdate()) > 3   
       begin  
       SET @bInsert = (select count(*) from sysroPassports where isnull(GroupType,'') <> 'U' and IDParentPassport = @idPassport)  
       if @bInsert > 0  
       begin  
         INSERT INTO sysroPermissionsOverRequests VALUES (@idPassport, @idRequest, dbo.GetEmployeeGroup(@employeeID,getdate()))  
       end  
          
       end  
                
          FETCH NEXT FROM db_cursorInsertRequestPassportPermission INTO  @idRequest, @employeeID, @alias , @featureID  
       END  
      CLOSE db_cursorInsertRequestPassportPermission  
      DEALLOCATE db_cursorInsertRequestPassportPermission  
         
             
      FETCH NEXT FROM db_cursor INTO @idPassport    
     END    
     CLOSE db_cursor    
     DEALLOCATE db_cursor   
        
     END
go
	 
 alter PROCEDURE [dbo].[InsertRequestPassportPermission]  
      (  
       @IDRequest int  
      )  
     AS  
     /* Añadimos todos los pasaportes que tienen permisos para una solicitud a la tabla temporal */  
     BEGIN  
      DECLARE @idPassport int,  
      @featureID int,  
      @employeeID int,  
      @alias varchar(100),  
     @bInsert int,
	 @pIDRequest int = @IDRequest  
    /* Obtenemos el empleado, feature y alias requeridos para comprobar permisos */  
    SELECT @employeeID = Requests.IDEmployee, @alias = sysroFeatures.Alias, @featureID = sysroFeatures.EmployeeFeatureID  
     FROM Requests inner join sysroFeatures on sysroFeatures.AliasID = Requests.RequestType where Requests.ID = @pIDRequest  
    DELETE FROM sysroPermissionsOverRequests WHERE IDRequest = @pIDRequest  
    /* Obtenemos todos los pasaportes de tipo grupo que pueden tener permisos sobre la petición */  
    DECLARE db_cursorInsertRequestPassportPermission CURSOR FOR    
    SELECT ID from sysroPassports where GroupType = 'U' and ID NOT IN(SELECT IDPassport FROM dbo.GetAllPassportParentsEmployeeType())  
       
     OPEN db_cursorInsertRequestPassportPermission    
     FETCH NEXT FROM db_cursorInsertRequestPassportPermission INTO @idPassport    
     WHILE @@FETCH_STATUS = 0    
     BEGIN    
     /* Si el pasaporte tiene permisos sobre la petición lo guardamos en la tabla temporal */  
     if dbo.WebLogin_GetPermissionOverFeature(@idPassport,@alias, 'U', 0) > 3   
      AND dbo.WebLogin_GetPermissionOverEmployee(@idPassport,@employeeID,@featureID,2,1,getdate()) > 3   
     begin  
     SET @bInsert = (select count(*) from sysroPassports where isnull(GroupType,'') <> 'U' and IDParentPassport = @idPassport)  
     if @bInsert > 0  
     begin  
       INSERT INTO sysroPermissionsOverRequests VALUES (@idPassport, @pIDRequest, dbo.GetEmployeeGroup(@employeeID,getdate()))  
     end  
        
     end  
                
      FETCH NEXT FROM db_cursorInsertRequestPassportPermission INTO @idPassport    
     END    
     CLOSE db_cursorInsertRequestPassportPermission    
     DEALLOCATE db_cursorInsertRequestPassportPermission   
        
     END
go
	   
 alter FUNCTION dbo.GetPassportLevelOfAuthority  
 (  
  @idPassport int  
 )  
 RETURNS int  
 AS  
 BEGIN  
  declare @levelOfAuthority int  
	, @pidPassport int = @idPassport
  ;with cte (id,idparentpassport,levelofauthority)  
  as  
  (  
  select ID,IDParentPassport, LevelOfAuthority from sysroPassports where id = @pidPassport   
   union all   
  select t.id,t.IDParentPassport, t.LevelOfAuthority  
  from sysroPassports t join cte c on t.id = c.idparentpassport  
  )  
  select @levelOfAuthority =  (select top 1 levelofauthority from cte where not levelofauthority is null)  
  IF @levelOfAuthority is null set @levelOfAuthority = 1  
  RETURN @levelOfAuthority  
 END
go
    alter PROCEDURE [dbo].[Analytics_Incidences]    
     @initialDate smalldatetime,    
     @endDate smalldatetime,    
     @idpassport int,    
     @employeeFilter nvarchar(max),    
     @userFieldsFilter nvarchar(max)    
     AS    
     create table #employeeIDs (idEmployee int)    
	 DECLARE @pinitialDate smalldatetime = @initialDate,    
     @pendDate smalldatetime = @endDate,    
     @pidpassport int = @idpassport,    
     @pemployeeFilter nvarchar(max) = @employeeFilter,    
     @puserFieldsFilter nvarchar(max) = @userFieldsFilter    
         insert into #employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;    
     SELECT     CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)     
                           + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup,     
                           dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description AS IncidenceName,     
                           dbo.TimeZones.Name AS ZoneTime, dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS Value, YEAR(dbo.DailySchedule.Date) AS Año,     
                           (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailySchedule.Date) AS WeekOfYear, DATEPART(dy,     
                           dbo.DailySchedule.Date) AS DayOfYear, CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName,     
                           dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.FullGroupName AS FullGroupName, dbo.sysroEmployeeGroups.Path,     
                           dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,    
          dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,    
          dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,    
          dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,    
          dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,    
          dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,    
          dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,    
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),@pendDate) As UserField1,    
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),@pendDate) As UserField2,    
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),@pendDate) As UserField3,    
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),@pendDate) As UserField4,    
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),@pendDate) As UserField5,    
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),@pendDate) As UserField6,    
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),@pendDate) As UserField7,    
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),@pendDate) As UserField8,    
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),@pendDate) As UserField9,    
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@pendDate) As UserField10    
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
    WHERE dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.DailyCauses.IDEmployee,2,0,0,dbo.DailyCauses.date) > 1    
       AND dbo.sysroEmployeeGroups.IDEmployee in(select idEmployee from #employeeIDs)     
      and dbo.dailyCauses.Date between @pinitialDate and @pendDate    
     GROUP BY dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name,     
                           YEAR(dbo.DailySchedule.Date), CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)     
                           + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120), CASE WHEN DailySchedule.Date <= GETDATE()     
                           THEN Shifts.Name ELSE Shifts_1.Name END, dbo.Employees.Name, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, DATEPART(wk,     
                           dbo.DailySchedule.Date), DATEPART(dy, dbo.DailySchedule.Date), (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1,     
                           dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,     
                           dbo.sysroEmployeeGroups.CurrentEmployee, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate
GO

 alter PROCEDURE [dbo].[Analytics_CostCenters_Detail]  
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
                            (DATEPART(dw, dbo.DailyCauses.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailyCauses.Date) AS WeekOfYear, DATEPART(dy,   
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
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),@pendDate) As UserField1,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),@pendDate) As UserField2,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),@pendDate) As UserField3,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),@pendDate) As UserField4,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),@pendDate) As UserField5,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),@pendDate) As UserField6,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),@pendDate) As UserField7,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),@pendDate) As UserField8,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),@pendDate) As UserField9,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@pendDate) As UserField10  
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
go
	 
	 alter PROCEDURE [dbo].[InsertGroupPassportPermission]  
  (  
   @idGroup int  
  )  
     AS  
     BEGIN  
  DECLARE @pidGroup int = @idGroup    
  DELETE FROM sysroPermissionsOverGroups WHERE EmployeeGroupID = @pidGroup  
  DECLARE @featureEmployeeID int  
   DECLARE EmployeeFeatureCursor CURSOR  
   FOR   
    SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null  
   OPEN EmployeeFeatureCursor  
   FETCH NEXT FROM EmployeeFeatureCursor INTO @featureEmployeeID  
   WHILE @@FETCH_STATUS = 0  
   BEGIN   
      
    INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission)   
      select supPassports.ID, Groups.ID, @featureEmployeeID, dbo.GetPassportLevelOfAuthority(supPassports.ID),  
        dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,@featureEmployeeID,0)  
      from sysroPassports supPassports, Groups  
      where supPassports.GroupType = 'U' AND Groups.ID = @pidGroup  
      
    FETCH NEXT FROM EmployeeFeatureCursor   
    INTO @featureEmployeeID  
   END   
   CLOSE EmployeeFeatureCursor  
   DEALLOCATE EmployeeFeatureCursor  
         
     END
GO
	 
 alter PROCEDURE [dbo].[Analytics_CostCenters]  
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
   insert into @businessCenterIDs select * from dbo.SplitToInt(@pbusinessCenterFilter,',');  
       SELECT     CONVERT(varchar(10), dbo.DailyCauses.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)   
                             + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailyCauses.Date, 120) AS KeyView, dbo.DailyCauses.Date,  
                             dbo.Causes.Name AS CauseName, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor,  isnull(dbo.BusinessCenters.ID, 0) as IDCenter,   
           isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,    
           isnull(dbo.BusinessCenters.Field4, '') AS Field4, isnull(dbo.BusinessCenters.Field5, '') AS Field5, dbo.DailyCauses.Value AS Value, YEAR(dbo.DailyCauses.Date) AS Año,   
                             (DATEPART(dw, dbo.DailyCauses.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailyCauses.Date) AS WeekOfYear, DATEPART(dy,   
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
     and dbo.dailycauses.date between @pinitialDate and @pendDate 
GO

alter PROCEDURE [dbo].[Analytics_Authorizations]  
      @idpassport int,  
      @employeeFilter nvarchar(max),  
      @userFieldsFilter nvarchar(max)  
      AS  
      
       DECLARE @employeeIDs Table(idEmployee int)  
     DECLARE @autorizationsIDs Table(idAccess int)  
    DECLARE @zonesIDs Table(idZone int)  
	declare   @pidpassport int = @idpassport,    
     @pemployeeFilter nvarchar(max) = @employeeFilter,    
     @puserFieldsFilter nvarchar(max) = @userFieldsFilter
     DECLARE @cDate as Date, @idParentPassport int  
    select @idParentPassport = IDParentPassport from sysroPassports where id = @pidpassport  
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
    insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter, @cDate, @cDate;  
      
      SELECT dbo.sysroEmployeeGroups.IDEmployee AS IDEmployee, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.IDGroup AS IDGroup, dbo.sysroEmployeeGroups.FullGroupName As FullGroupName, dbo.sysroEmployeeGroups.Path AS GroupPath,  
 
             dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee, dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract AS IDContract, dbo.AccessGroups.Name AS AuthorizationName,   
             Zones.Name AS ZoneName, zones.IsWorkingZone AS IsWorkingZone, AccessPeriods.Name As AccessPeriodName, 1 AS BelongsToGroup,  
       dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract,  
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
       dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),getdate()) As UserField10  
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
      WHERE dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeeGroups.IDEmployee,9,0,0,getdate()) > 1  
        AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) AND getdate() between sysroEmployeeGroups.BeginDate and sysroEmployeeGroups.EndDate  
      AND dbo.AccessGroups.ID IN (SELECT idAccess FROM @autorizationsIDs)  
     AND dbo.Zones.ID in (select idZone from @zonesIDs)
go
	 
  ALTER PROCEDURE [dbo].[Analytics_AccessPlates]  
    @initialDate smalldatetime,  
    @endDate smalldatetime,  
    @idpassport int,  
    @employeeFilter nvarchar(max),  
    @userFieldsFilter nvarchar(max)  
    AS  
	DECLARE @pinitialDate smalldatetime = @initialDate,    
     @pendDate smalldatetime = @endDate,    
     @pidpassport int = @idpassport,    
     @pemployeeFilter nvarchar(max) = @employeeFilter,    
     @puserFieldsFilter nvarchar(max) = @userFieldsFilter    
     DECLARE @employeeIDs Table(idEmployee int)  
     insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
     SELECT     dbo.Punches.ID, dbo.Punches.IDEmployee, dbo.Punches.DateTime AS DatePunche, CONVERT(VARCHAR(8), dbo.Punches.DateTime, 108) AS TimePunche,   
                           CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails) AS TypeDetails, dbo.Employees.Name AS NameEmployee, dbo.Zones.Name AS NameZone,   
                           dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.IDGroup,   
                           dbo.sysroEmployeeGroups.EndDate,  
            dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,  
         dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
         dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
         dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
         dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
         dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),@pendDate) As UserField1,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),@pendDate) As UserField2,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),@pendDate) As UserField3,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),@pendDate) As UserField4,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),@pendDate) As UserField5,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),@pendDate) As UserField6,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),@pendDate) As UserField7,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),@pendDate) As UserField8,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),@pendDate) As UserField9,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@pendDate) As UserField10  
     FROM         dbo.Punches with (nolock)   
         INNER JOIN dbo.TerminalReaders with (nolock) ON dbo.Punches.IDTerminal = dbo.TerminalReaders.IDTerminal AND dbo.Punches.IDReader = dbo.TerminalReaders.ID   
         INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.Punches.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.Punches.DateTime >= dbo.sysroEmployeeGroups.BeginDate AND dbo.Punches.DateTime <= dbo.sysroEmployeeGroups.EndDate   
         INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate   
         LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID   
         LEFT OUTER JOIN dbo.Zones with (nolock) ON dbo.Punches.IDZone = dbo.Zones.ID  
     WHERE     (dbo.TerminalReaders.Type = N'MAT') AND (NOT (dbo.Punches.TypeDetails IS NULL)) AND (dbo.Punches.Type = 5 OR dbo.Punches.Type = 6 OR dbo.Punches.Type = 7)  
      AND dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeeGroups.IDEmployee,2,0,0,dbo.Punches.ShiftDate) > 1  
      AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @pinitialDate and @pendDate
go
	 
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
   DECLARE @pinitialDate smalldatetime = @initialDate,    
     @pendDate smalldatetime = @endDate,    
     @pidpassport int = @idpassport,    
     @pemployeeFilter nvarchar(max) = @employeeFilter,    
     @puserFieldsFilter nvarchar(max) = @userFieldsFilter 
     insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter, @pinitialDate, @pendDate;  
   select @idParentPassport = IDParentPassport from sysroPassports where id = @pidpassport  
   if ((select value from sysroLiveAdvancedParameters where ParameterName = 'AccessGroupsMode') = '1')    
    insert into @zonesIDs select ID from Zones  
       where ID in (SELECT IDZone FROM AccessGroupsPermissions with (nolock)   
          where IDAccessGroup in (select IDAccessGroup from sysroPassports_AccessGroup with (nolock) where idpassport = @idParentPassport)) or IDParent is null  
      
   else    
    insert into @zonesIDs select ID from Zones with (nolock)  
      
       SELECT     dbo.Punches.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee,   
                             dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, CONVERT(nvarchar(8),   
                             dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day, MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Year,   
                             (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName,   
                             dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName, dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL   
                             THEN 0 ELSE 1 END AS IsInvalid, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails) AS Details, dbo.sysroEmployeeGroups.Path,   
                             dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,  
              dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,  
           dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
           dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
           dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
           dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
           dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),@pendDate) As UserField1,  
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),@pendDate) As UserField2,  
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),@pendDate) As UserField3,  
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),@pendDate) As UserField4,  
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),@pendDate) As UserField5,  
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),@pendDate) As UserField6,  
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),@pendDate) As UserField7,  
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),@pendDate) As UserField8,  
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),@pendDate) As UserField9,  
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@pendDate) As UserField10  
       FROM         dbo.sysroEmployeeGroups with (nolock)   
           INNER JOIN dbo.Punches with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee   
           INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate   
           LEFT OUTER JOIN dbo.Zones with (nolock) ON dbo.Punches.IDZone = dbo.Zones.ID   
           LEFT OUTER JOIN dbo.Terminals with (nolock) ON dbo.Punches.IDTerminal = dbo.Terminals.ID   
           LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID  
       WHERE     dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeeGroups.IDEmployee,9,0,0,dbo.Punches.ShiftDate) > 1  
        AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @pinitialDate and @pendDate  
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
    ALTER PROCEDURE [dbo].[Analytics_Punches]
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
                           dbo.Punches.TypeData AS PunchIDTypeData, CASE WHEN dbo.Punches.TypeData <> 0 THEN dbo.Causes.Name ELSE '' END AS PunchTypeData, 
                           dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day, 
                           MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Año, (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) 
                           % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName, 
                           dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END AS IsInvalid, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails) 
                           AS Details, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, 
                           dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, dbo.Punches.IsNotReliable, dbo.Punches.Location AS PunchLocation, 
                           dbo.Punches.LocationZone AS PunchLocationZone, dbo.Punches.TimeZone AS PunchTimeZone, dbo.sysroPassports.Name AS EditorName,
    					   dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,
    						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
    						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
    						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
    						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
    						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
    						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),@pendDate) As UserField1,
    						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),@pendDate) As UserField2,
    						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),@pendDate) As UserField3,
    						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),@pendDate) As UserField4,
    						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),@pendDate) As UserField5,
    						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),@pendDate) As UserField6,
    						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),@pendDate) As UserField7,
    						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),@pendDate) As UserField8,
    						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),@pendDate) As UserField9,
    						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@pendDate) As UserField10
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
     WHERE	dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeeGroups.IDEmployee,2,0,0,dbo.Punches.ShiftDate) > 1
    			AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @pinitialDate and @pendDate
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
go
	      
   alter PROCEDURE [dbo].[Analytics_Tasks]  
    @initialDate smalldatetime,  
    @endDate smalldatetime,  
    @idpassport int,  
    @userFieldsFilter nvarchar(max)  
    AS  
	 DECLARE @pinitialDate smalldatetime = @initialDate,  
     @pendDate smalldatetime = @endDate,  
     @pidpassport int = @idpassport,  
     @puserFieldsFilter nvarchar(max) = @userFieldsFilter  
    SELECT     dbo.sysroEmployeeGroups.IDGroup, dbo.sysroEmployeeGroups.FullGroupName AS GroupName, dbo.Employees.ID AS IDEmployee,   
                           dbo.Employees.Name AS EmployeeName, DATEPART(year, dbo.DailyTaskAccruals.Date) AS Date_Year, DATEPART(month, dbo.DailyTaskAccruals.Date)   
                           AS Date_Month, DATEPART(day, dbo.DailyTaskAccruals.Date) AS Date_Day, dbo.DailyTaskAccruals.Date, dbo.BusinessCenters.ID AS IDCenter,   
                           dbo.BusinessCenters.Name AS CenterName, dbo.Tasks.ID AS IDTask, dbo.Tasks.Name AS TaskName, dbo.Tasks.InitialTime, ISNULL(dbo.Tasks.Field1, '')   
                           AS Field1_Task, ISNULL(dbo.Tasks.Field2, '') AS Field2_Task, ISNULL(dbo.Tasks.Field3, '') AS Field3_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field4), 0)   
                           AS Field4_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field5), 0) AS Field5_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field6), 0) AS Field6_Task,   
                           ISNULL(dbo.DailyTaskAccruals.Field1, '') AS Field1_Total, ISNULL(dbo.DailyTaskAccruals.Field2, '') AS Field2_Total, ISNULL(dbo.DailyTaskAccruals.Field3, '')   
                           AS Field3_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field4), 0) AS Field4_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field5),   
                           0) AS Field5_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field6), 0) AS Field6_Total, ISNULL(CONVERT(numeric(25, 6),   
                           dbo.DailyTaskAccruals.Value), 0) AS Value, ISNULL(dbo.Tasks.Project, '') AS Project, ISNULL(dbo.Tasks.Tag, '') AS Tag, dbo.Tasks.IDPassport,   
                           ISNULL(dbo.Tasks.TimeChangedRequirements, 0) AS TimeChangedRequirements, ISNULL(dbo.Tasks.ForecastErrorTime, 0) AS ForecastErrorTime,   
                           ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) AS NonProductiveTimeIncidence, ISNULL(dbo.Tasks.EmployeeTime, 0) AS EmployeeTime,   
                           ISNULL(dbo.Tasks.TeamTime, 0) AS TeamTime, ISNULL(dbo.Tasks.MaterialTime, 0) AS MaterialTime, ISNULL(dbo.Tasks.OtherTime, 0) AS OtherTime,   
                           ISNULL(dbo.Tasks.InitialTime, 0) + ISNULL(dbo.Tasks.TimeChangedRequirements, 0) + ISNULL(dbo.Tasks.ForecastErrorTime, 0)   
                           + ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) + ISNULL(dbo.Tasks.EmployeeTime, 0) + ISNULL(dbo.Tasks.TeamTime, 0) + ISNULL(dbo.Tasks.MaterialTime, 0)   
                           + ISNULL(dbo.Tasks.OtherTime, 0) AS Duration,   
                           CASE WHEN Tasks.Status = 0 THEN 'Activa' ELSE CASE WHEN Tasks.Status = 1 THEN 'Finalizada' ELSE CASE WHEN Tasks.Status = 2 THEN 'Cancelada' ELSE 'Pendiente'  
                            END END END AS Estado, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
            dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,  
         dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
         dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
         dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
         dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
         dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),@pendDate) As UserField1,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),@pendDate) As UserField2,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),@pendDate) As UserField3,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),@pendDate) As UserField4,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),@pendDate) As UserField5,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),@pendDate) As UserField6,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),@pendDate) As UserField7,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),@pendDate) As UserField8,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),@pendDate) As UserField9,  
         dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@pendDate) As UserField10  
     FROM         dbo.sysroEmployeeGroups with (nolock)    
                           INNER JOIN dbo.DailyTaskAccruals with (nolock) ON dbo.DailyTaskAccruals.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.DailyTaskAccruals.Date BETWEEN dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate
   
                           INNER JOIN dbo.Tasks with (nolock) ON dbo.Tasks.ID = dbo.DailyTaskAccruals.IDTask   
                           INNER JOIN dbo.BusinessCenters with (nolock) ON dbo.Tasks.IDCenter = dbo.BusinessCenters.ID   
                           INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyTaskAccruals.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyTaskAccruals.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyTaskAccruals.Date <= dbo.EmployeeContracts.EndDate  
         INNER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID  
     WHERE     (dbo.businesscenters.id is null OR dbo.BusinessCenters.ID IN (SELECT DISTINCT IDCenter FROM sysroPassports_Centers with (nolock) WHERE IDPassport=(SELECT isnull(IdparentPassport,0) from sysroPassports where ID = @pidpassport)) )  
      AND dbo.DailyTaskAccruals.Date between @pinitialDate and @pendDate
go
	 	   
 alter FUNCTION [dbo].[sysro_GetPermissionOverGroup]   
    (  
     @idPassport int,  
     @idGroup int,  
     @idApplication int,  
     @mode int  
    )  
   RETURNS int  
   AS  
   BEGIN  
    /* In addition to the 3 normal modes   
     0=Normal  
     1=Direct Only  
     2=Inherited Only, including group inheritance  
       within current passport  
    there is a 4th mode:  
     3=Inherited Only, excluding group inheritance   
       within current passport (usefull for MaxConfigurable) */  
      
    DECLARE @Result int  
    DECLARE @NewResult int  
    DECLARE @ParentPassport int  
    DECLARE @GroupType nvarchar(50),  
	 @pidPassport int = @idPassport,  
     @pidGroup int = @idGroup,  
     @pidApplication int = @idApplication,  
     @pmode int = @mode  
      
    /* Return the most restricted passport permission by looking at parent  
    passport's permissions one by one */  
      
    IF @pmode <> 3  
     SELECT @Result = dbo.sysro_GetPassportPermissionOverGroup(@pidPassport, @pidGroup, @pidApplication, @pmode)  
      
    IF @pmode <> 1  
    BEGIN  
       
    IF @pmode > 0   
   BEGIN  
    SELECT @ParentPassport = IDParentPassport  
    FROM sysroPassports  
    WHERE ID = @pidPassport  
   END  
   ELSE  
   BEGIN  
     SELECT @GroupType = isnull(GroupType, '')  
       FROM sysroPassports  
       WHERE ID = @pidPassport  
        
       if @GroupType = 'U'  
       begin  
        SET @ParentPassport = @pidPassport  
       end  
       else  
       begin  
        SELECT @ParentPassport = IDParentPassport  
        FROM sysroPassports  
        WHERE ID = @pidPassport  
       end  
    END   
         
     /* If looking for inherited only, the constraint is for first check only.  
     Other queries should be threated as normal */  
     WHILE NOT @ParentPassport IS NULL  
     BEGIN  
      SELECT @NewResult = dbo.sysro_GetPassportPermissionOverGroup(@ParentPassport, @pidGroup, @pidApplication, 0)  
      IF NOT @NewResult IS NULL AND @NewResult < @Result OR @Result IS NULL  
       SET @Result = @NewResult  
      SELECT @ParentPassport = IDParentPassport  
      FROM sysroPassports  
      WHERE ID = @ParentPassport  
     END  
    END  
      
    /* Return result */  
    IF @Result IS NULL  
     SET @Result = 0  
      
    RETURN @Result  
   END
go
   
     alter PROCEDURE  [dbo].[Analytics_Schedule]  
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
     SELECT     CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)   
            + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10) AS KeyView,   
            dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName,   
            dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeesShifts.ShiftName,   
            dbo.sysroEmployeesShifts.CurrentDate AS Date, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours, COUNT(*) AS Count,   
            MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año, (DATEPART(dw,   
            dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear,   
            DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee,   
            dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
          dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,  
          dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
          dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
          dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
          dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
          dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),@pendDate) As UserField1,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),@pendDate) As UserField2,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),@pendDate) As UserField3,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),@pendDate) As UserField4,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),@pendDate) As UserField5,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),@pendDate) As UserField6,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),@pendDate) As UserField7,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),@pendDate) As UserField8,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),@pendDate) As UserField9,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@pendDate) As UserField10  
     FROM         dbo.sysroEmployeesShifts with (nolock)   
            INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate
 <= dbo.sysroEmployeeGroups.EndDate   
            INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.EmployeeContracts.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.EmployeeContracts.EndDate   
            LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID  
     WHERE dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeesShifts.IDEmployee,2,0,0,dbo.sysroEmployeesShifts.CurrentDate) > 1  
      AND dbo.sysroEmployeesShifts.IDEmployee in( select idEmployee from @employeeIDs) and dbo.sysroEmployeesShifts.CurrentDate between @pinitialDate and @pendDate  
     GROUP BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name,   
            dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate, MONTH(dbo.sysroEmployeesShifts.CurrentDate),   
            YEAR(dbo.sysroEmployeesShifts.CurrentDate), (DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk,   
            dbo.sysroEmployeesShifts.CurrentDate), DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate), dbo.sysroEmployeeGroups.IDGroup,   
            dbo.EmployeeContracts.IDContract, CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)   
            + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10), dbo.sysroEmployeeGroups.Path,   
            dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
          dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate
go
   
    alter FUNCTION [dbo].[sysro_GetPassportPermissionOverGroup]  
   (  
    @idPassport int,  
    @idGroup int,  
    @idApplication int,  
    @mode int  
   )  
  RETURNS int  
  AS  
  BEGIN  
   /* While looking only at permissions defined directly on passport,  
   returns the first permission found in the employees groups hierarchy */  
   DECLARE @Result int  
   DECLARE @ParentGroup int,
   @pidPassport int = @idPassport,  
    @pidGroup int = @idGroup,  
    @pidApplication int = @idApplication,  
    @pmode int = @mode   
     
   IF @pmode <> 2  
    SELECT @Result = Permission  
    FROM sysroPassports_PermissionsOverGroups  
    WHERE IDPassport = @pidPassport AND  
     IDGroup = @pidGroup AND  
     IDApplication = @pidApplication  
     
   IF @pmode <> 1  
   BEGIN  
    SELECT @ParentGroup = dbo.GetEmployeeGroupParent(@pidGroup)  
      
    WHILE @Result IS NULL AND NOT @ParentGroup IS NULL  
    BEGIN  
     SELECT @Result = Permission  
     FROM sysroPassports_PermissionsOverGroups  
     WHERE IDPassport = @pidPassport AND  
      IDGroup = @ParentGroup AND  
      IDApplication = @pidApplication  
       
     SELECT @ParentGroup = dbo.GetEmployeeGroupParent(@ParentGroup)  
    END  
   END  
     
  RETURN @Result  
  END
go
  
   
 alter FUNCTION [dbo].[sysro_GetPermissionOverFeature]   
   (  
    @idPassport int,  
    @featureAlias nvarchar(100),  
    @featureType varchar(1),  
    @mode int  
   )  
  RETURNS int  
  AS  
  BEGIN  
   DECLARE @Result int,
     @pidPassport int = @idPassport,  
    @pfeatureAlias nvarchar(100) = @featureAlias,  
    @pfeatureType varchar(1) = @featureType,  
    @pmode int    = @mode
     
   /* Get list of passports to check */  
   DECLARE @Passports table(ID int)  
   IF @pmode <> 2  
    INSERT INTO @Passports VALUES (@pidPassport)  
   IF @pmode <> 1  
    INSERT INTO @Passports SELECT ID FROM dbo.GetPassportParents(@pidPassport)  
     
   /* Get feature ID */  
   DECLARE @IDFeature int  
   SELECT @IDFeature = ID  
   FROM sysroFeatures  
   WHERE Alias = @pfeatureAlias AND  
    Type = @pfeatureType  
     
   /* Check permissions */  
   SELECT TOP 1 @Result = Permission  
   FROM sysroPassports_PermissionsOverFeatures  
   WHERE IDPassport IN (SELECT * FROM @Passports) AND  
    IDFeature = @IDFeature   
   ORDER BY Permission  
     
   /* Return result */  
   IF @Result IS NULL  
    SET @Result = 0  
   RETURN @Result  
  END
go
   alter PROCEDURE [dbo].[Analytics_Punches]  
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
                           dbo.Punches.TypeData AS PunchIDTypeData, CASE WHEN dbo.Punches.TypeData <> 0 THEN dbo.Causes.Name ELSE '' END AS PunchTypeData,   
                           dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day,   
                           MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Año, (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1)   
                           % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName,   
                           dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END AS IsInvalid, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails)   
                           AS Details, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate,   
                           dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, dbo.Punches.IsNotReliable, dbo.Punches.Location AS PunchLocation,   
                           dbo.Punches.LocationZone AS PunchLocationZone, dbo.Punches.TimeZone AS PunchTimeZone, dbo.sysroPassports.Name AS EditorName,  
            dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,  
          dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
          dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
          dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
          dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
          dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),@pendDate) As UserField1,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),@pendDate) As UserField2,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),@pendDate) As UserField3,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),@pendDate) As UserField4,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),@pendDate) As UserField5,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),@pendDate) As UserField6,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),@pendDate) As UserField7,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),@pendDate) As UserField8,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),@pendDate) As UserField9,  
          dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@pendDate) As UserField10  
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
     WHERE dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeeGroups.IDEmployee,2,0,0,dbo.Punches.ShiftDate) > 1  
       AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @pinitialDate and @pendDate  
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
go
   alter PROCEDURE [dbo].[ObtainEmployeeIDsFromFilter]  
    @employeeFilter nvarchar(max),  
   @initialDate smalldatetime,  
    @endDate smalldatetime  
    AS   
   begin
     declare @pemployeeFilter nvarchar(max) = @employeeFilter  
   declare @pinitialDate smalldatetime   = @initialDate 
    declare @pendDate smalldatetime  = @endDate 
    DECLARE @SQLString nvarchar(MAX);  
       SET @SQLString = 'SELECT DISTINCT IDEmployee FROM sysroEmployeeGroups WHERE (' + @pemployeeFilter   
    SET @SQLString = @SQLString + ') AND ((''' + CONVERT(VARCHAR(10), @pinitialDate, 112) + ''' >= dbo.sysroEmployeeGroups.BeginDate and ''' + CONVERT(VARCHAR(10), @pinitialDate, 112) + ''' <= dbo.sysroEmployeeGroups.EndDate) OR (''' + CONVERT(VARCHAR(10), 
@pendDate, 112) + ''' >= dbo.sysroEmployeeGroups.BeginDate and ''' + CONVERT(VARCHAR(10), @pendDate, 112) + ''' <= dbo.sysroEmployeeGroups.BeginDate) OR (''' + CONVERT(VARCHAR(10), @pinitialDate, 112) + ''' <= dbo.sysroEmployeeGroups.BeginDate and  ''' + CONVERT(VARCHAR(10), @pendDate, 112) + ''' >= dbo.sysroEmployeeGroups.BeginDate ))'  
    SET @SQLString = @SQLString + ' ORDER BY IDEmployee'  
     exec sp_executesql @SQLString  
   end
go
   alter FUNCTION [dbo].[GetEmployeeUserFieldValueMin]  
   (    
    @idEmployee int,  
    @FieldName nvarchar(50),  
    @Date smalldatetime  
   )  
   RETURNS varchar(4000)  
   AS  
   BEGIN  
  DECLARE @value varchar(4000),
    @pidEmployee int = @idEmployee,  
    @pFieldName nvarchar(50) = @FieldName,  
    @pDate smalldatetime =  @Date  
      
  SELECT TOP 1 @Value = CONVERT(varchar(4000), [Value])  
      FROM EmployeeUserFieldValues      
      WHERE EmployeeUserFieldValues.IDEmployee = @pidEmployee AND   
         EmployeeUserFieldValues.FieldName = @pFieldName AND  
         EmployeeUserFieldValues.Date <= @pDate  
      ORDER BY EmployeeUserFieldValues.Date DESC  
      
  RETURN @value  
   END
go  
 alter FUNCTION dbo.UFN_SEPARATES_COLUMNS(  
  @TEXT      varchar(8000)  
 ,@COLUMN    tinyint  
 ,@SEPARATOR char(1)  
 )RETURNS varchar(8000)  
 AS  
   BEGIN  
   declare @pTEXT      varchar(8000)  = @TEXT
 DECLARE @pCOLUMN    tinyint   = @COLUMN
 DECLARE @pSEPARATOR char(1) = @SEPARATOR
        DECLARE @POS_START  int = 1  
        DECLARE @POS_END    int = CHARINDEX(@pSEPARATOR, @pTEXT, @POS_START)  
        WHILE (@pCOLUMN >1 AND @POS_END> 0)  
          BEGIN  
              SET @POS_START = @POS_END + 1  
              SET @POS_END = CHARINDEX(@pSEPARATOR, @pTEXT, @POS_START)  
              SET @pCOLUMN = @pCOLUMN - 1  
          END   
        IF @pCOLUMN > 1  SET @POS_START = LEN(@pTEXT) + 1  
        IF @POS_END = 0 SET @POS_END = LEN(@pTEXT) + 1   
        RETURN SUBSTRING (@pTEXT, @POS_START, @POS_END - @POS_START)  
   END
go
 alter FUNCTION [dbo].[SplitToInt](  
   @String nvarchar (MAX),  
   @Delimiter nvarchar (10)  
   )  
  returns @ValueTable table ([Value] int)  
  begin  
   declare @NextString nvarchar(MAX)  
   declare @Pos int  
   declare @NextPos int  
   declare @CommaCheck nvarchar(1) 
   declare  @pString nvarchar (MAX) = @String,  
   @pDelimiter nvarchar (10) =  @Delimiter
    --Initialize  
   set @NextString = ''  
   set @CommaCheck = right(@pString,1)   
   --Check for trailing Comma, if not exists, INSERT  
   --if (@CommaCheck <> @pDelimiter )  
   set @pString = @pString + @pDelimiter  
   --Get position of first Comma  
   set @Pos = charindex(@pDelimiter,@pString)  
   set @NextPos = 1  
   --Loop while there is still a comma in the String of levels  
   while (@pos <>  0)    
   begin  
    set @NextString = substring(@pString,1,@Pos - 1)  
    insert into @ValueTable ( [Value]) Values (convert(int,@NextString))  
    set @pString = substring(@pString,@pos +1,len(@pString))  
    set @NextPos = @Pos  
    set @pos  = charindex(@pDelimiter,@pString)  
   end  
   return  
  end
go
  
  
   ALTER PROCEDURE [dbo].[Analytics_Concepts]
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
   	SELECT	CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120), 10) AS KeyView, 
   			reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept, 
   			dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName, 
   			reqReg.conceptName AS ConceptName, reqReg.dt AS Date, sum(isnull(dbo.DailyAccruals.Value,0)) AS Value,COUNT(*) AS Count, MONTH(reqReg.dt) AS Mes, 
   			YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, 
   			reqReg.dt) AS WeekOfYear, DATEPART(dy, reqReg.dt) AS DayOfYear, dbo.sysroEmployeeGroups.Path, 
   			dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
   			dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @pendDate AS EndPeriod,
   			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
   			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
   			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
   			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
   			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),@pendDate) As UserField1,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),@pendDate) As UserField2,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),@pendDate) As UserField3,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),@pendDate) As UserField4,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),@pendDate) As UserField5,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),@pendDate) As UserField6,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),@pendDate) As UserField7,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),@pendDate) As UserField8,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),@pendDate) As UserField9,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@pendDate) As UserField10
   	FROM	(select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays, 
   				(select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp, 
   				(select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con
   				where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1
   			) reqReg
   			INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate
   			INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
   			LEFT OUTER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept
   	GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName, 
   			dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt), 
   			YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, reqReg.dt), DATEPART(dy, 
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
   	SELECT	CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120), 10) AS KeyView, 
   			reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept, 
   			dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName, 
   			reqReg.conceptName AS ConceptName, reqReg.dt AS Date, sum(isnull(dbo.DailyAccruals.Value,0)) AS Value, COUNT(*) AS Count, MONTH(reqReg.dt) AS Mes, 
   			YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, 
   			reqReg.dt) AS WeekOfYear, DATEPART(dy, reqReg.dt) AS DayOfYear, dbo.sysroEmployeeGroups.Path, 
   			dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
   			dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @pendDate AS EndPeriod,
   			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
   			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
   			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
   			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
   			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),@pendDate) As UserField1,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),@pendDate) As UserField2,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),@pendDate) As UserField3,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),@pendDate) As UserField4,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),@pendDate) As UserField5,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),@pendDate) As UserField6,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),@pendDate) As UserField7,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),@pendDate) As UserField8,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),@pendDate) As UserField9,
   			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@pendDate) As UserField10
   	FROM	(select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays, 
   				(select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp, 
   				(select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con
   				where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1
   			) reqReg
   			INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate
   			INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
   			INNER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept
   	GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName, 
   			dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt), 
   			YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, reqReg.dt), DATEPART(dy, 
   			reqReg.dt), CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar) 
   			+ '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120), 
   			10), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
   			dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate
   	option (maxrecursion 0)
    END
go

alter FUNCTION [dbo].[GetCompanyGroup]   
    (  
     @IDGroup int  
    )  
   RETURNS int  
   AS  
    BEGIN  
       
     /* Get group path */  
     DECLARE @UO nvarchar(max)  ,
	  @pIDGroup int = @IDGroup
     SELECT  @UO = ISNULL(Path,'0') From Groups WHERE ID = @pIDGroup  
       
     DECLARE @Pos int, @CurrentSep int  
     SET @CurrentSep = 0  
     SELECT @Pos = CHARINDEX('\', @UO)  
     IF @Pos > 0    
   BEGIN  
    SET @Pos = CHARINDEX('\', @UO)  
      SET @UO = SUBSTRING(@UO,0, @Pos)  
   END  
       
    RETURN CONVERT(int,@UO)  
 END
go
    
 alter FUNCTION dbo.GetPassportParents  
  (  
   @idPassport int  
  )  
 RETURNS @result table (ID int PRIMARY KEY)  
 AS  
 BEGIN  
 declare @pidPassport int = @idPassport
  /* Returns all parents of specified passport */  
  SELECT @pidPassport = IDParentPassport  
  FROM sysroPassports  
  WHERE ID = @pidPassport  
    
  WHILE NOT @pidPassport IS NULL  
  BEGIN  
   INSERT INTO @result VALUES (@pidPassport)  
     
   SELECT @pidPassport = IDParentPassport  
   FROM sysroPassports  
   WHERE ID = @pidPassport  
  END  
    
  RETURN  
 END
go

alter FUNCTION [dbo].[IsBlockDateRestrictionActive]  
   (   
   @idPassport int,  
  @idGroup int,  
  @date datetime  
   )  
   RETURNS int  
   AS  
   BEGIN  
        
   DECLARE @GroupCloseDate date ,
     @pidPassport int = @idPassport,  
  @pidGroup int = @idGroup,  
  @pdate datetime = @date   
   DECLARE @RestrictionDisabled int  
   DECLARE @BlockDateActive int  
   Set @BlockDateActive = 0  
   -- Si @RestrictionDisabled = 0, no debemos aplicar la restricción de fecha  
      SELECT @RestrictionDisabled = COUNT(*) FROM sysroSecurityParameters WHERE (IDPassport IN (SELECT ID FROM GetPassportParents(@pidPassport)) OR IDPassport IN (0,@pidPassport)) AND CalendarLock = 0  
                
   IF @RestrictionDisabled = 0   
   BEGIN  
    select @GroupCloseDate = CloseDate from Groups WHERE ID = dbo.GetCompanyGroup(@pidGroup)  
    IF @GroupCloseDate IS NOT NULL   
    BEGIN  
     IF @GroupCloseDate >= @pdate SET @BlockDateActive = 1  
    END  
      
   END  
   return @BlockDateActive  
   END
go

alter FUNCTION [dbo].[GetEmployeeGroup]   
   (  
    @idEmployee int,  
    @date smalldatetime  
   )  
  RETURNS int  
  AS  
   BEGIN  
      
    DECLARE @Result int ,
	 @pidEmployee int = @idEmployee ,  
    @pdate smalldatetime =  @date
     
   SELECT @pdate = CONVERT(smalldatetime, CONVERT(varchar, @pdate, 112))  
    SELECT @Result = IDGroup  
    FROM EmployeeGroups  
    WHERE IDEmployee = @pidEmployee AND  
     @pdate >= BeginDate AND @pdate <= EndDate  
   IF @Result IS NULL   
    BEGIN  
     SELECT TOP 1 @Result = IDGroup  
     FROM EmployeeGroups  
     WHERE IDEmployee = @pidEmployee AND   
        BeginDate > @pdate  
     ORDER BY BeginDate  
    END  
       
   RETURN @Result  
   END
go

alter FUNCTION [dbo].[WebLogin_GetPermissionOverEmployee]   
      (  
       @idPassport int,  
       @idEmployee int,  
       @idApplication int,  
       @mode int, --Esto no se utiliza pero se mantiene para que la definición de la función sea la misma  
       @includeGroups bit, --Esto no se utiliza pero se mantiene para que la definición de la función sea la misma  
       @date datetime  
      )  
   RETURNS int  
     AS  
     BEGIN  
      DECLARE @Result int  
     DECLARE @parentPassport int  
      DECLARE @GroupType nvarchar(50),
	   @pidPassport int = @idPassport,  
       @pidEmployee int = @idEmployee,  
       @pidApplication int = @idApplication,  
       @pmode int = @mode,
       @pincludeGroups bit = @includeGroups ,
       @pdate datetime   = @date  
       
     SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @pidPassport  
         
   IF @GroupType = 'U'  
   BEGIN  
       SET @parentPassport = @pidPassport  
   END  
   ELSE  
   BEGIN  
       SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @pidPassport  
   END  
     SELECT @Result = isnull(sysroPermissionsOverEmployeesExceptions.Permission,sysroPermissionsOverGroups.Permission) FROM sysroPermissionsOverGroups  
      inner join (select sysrovwAllEmployeeMobilities.*, eg.StartupDate from sysrovwAllEmployeeMobilities   
      inner join(select IDEmployee, min(beginDate) as StartupDate from sysrovwAllEmployeeMobilities where BeginDate > @pdate or @pdate between BeginDate and EndDate group by IDEmployee)eg  
      on eg.IDEmployee = sysrovwAllEmployeeMobilities.IDEmployee) em  
      on sysroPermissionsOverGroups.EmployeeGroupID=em.IDGroup and ((@pdate between em.BeginDate and em.EndDate) or em.StartupDate >@date) and em.IDEmployee = @pidEmployee  
     left outer join sysroPermissionsOverEmployeesExceptions on em.IDEmployee=sysroPermissionsOverEmployeesExceptions.EmployeeID   
      and sysroPermissionsOverEmployeesExceptions.PassportID = @parentPassport and sysroPermissionsOverEmployeesExceptions.EmployeeFeatureID =  @pidApplication  
      where sysroPermissionsOverGroups.PassportID = @parentPassport and sysroPermissionsOverGroups.EmployeeFeatureID =  @pidApplication  
    
   IF @Result > 3 AND @pidApplication = 2 AND dbo.IsBlockDateRestrictionActive(@pidPassport,dbo.GetEmployeeGroup(@pidEmployee,@date),@date) = 1 SET @Result = 3  
           
     RETURN @Result  
     END
go

alter FUNCTION [dbo].[WebLogin_GetPermissionOverFeature]   
    (  
     @idPassport int,  
     @featureAlias nvarchar(100),  
     @featureType varchar(1),  
     @mode int  
    )  
   RETURNS int  
   AS  
   BEGIN  
    DECLARE @Result int  
    DECLARE @IDFeature int  
   DECLARE @parentPassport int  
     DECLARE @GroupType nvarchar(50) ,
	  @pidPassport int = @idPassport,  
     @pfeatureAlias nvarchar(100) = @featureAlias,  
     @pfeatureType varchar(1) = @featureType,  
     @pmode int   = @mode
      
   SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @pidPassport  
        
       
   IF @pfeatureType = 'U'  
   BEGIN  
    IF @GroupType = 'U'  
      BEGIN  
       SET @parentPassport = @pidPassport  
      END  
      ELSE  
      BEGIN  
       SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @pidPassport  
      END  
     SELECT @IDFeature = ID FROM sysroFeatures WHERE Alias = @pfeatureAlias AND Type = @pfeatureType  
     SELECT @Result = Permission FROM sysroPermissionsOverFeatures WHERE PassportID = @parentPassport AND FeatureID = @IDFeature  
    END  
   ELSE  
   BEGIN  
    SELECT @Result = dbo.sysro_GetPermissionOverFeature(@pidPassport,@pfeatureAlias,@pfeatureType,@pmode)  
   END  
    RETURN @Result  
   END
go

alter PROCEDURE [dbo].[InsertRequestPassportPermission]  
      (  
       @IDRequest int  
      )  
     AS  
     /* Añadimos todos los pasaportes que tienen permisos para una solicitud a la tabla temporal */  
     BEGIN  
      DECLARE @idPassport int,  
      @featureID int,  
      @employeeID int,  
      @alias varchar(100),  
     @bInsert int,
	 @pIDRequest int = @IDRequest  
    /* Obtenemos el empleado, feature y alias requeridos para comprobar permisos */  
    SELECT @employeeID = Requests.IDEmployee, @alias = sysroFeatures.Alias, @featureID = sysroFeatures.EmployeeFeatureID  
     FROM Requests inner join sysroFeatures on sysroFeatures.AliasID = Requests.RequestType where Requests.ID = @pIDRequest  
    DELETE FROM sysroPermissionsOverRequests WHERE IDRequest = @pIDRequest  
    /* Obtenemos todos los pasaportes de tipo grupo que pueden tener permisos sobre la petición */  
    DECLARE db_cursorInsertRequestPassportPermission CURSOR FOR    
    SELECT ID from sysroPassports where GroupType = 'U' and ID NOT IN(SELECT IDPassport FROM dbo.GetAllPassportParentsEmployeeType())  
       
     OPEN db_cursorInsertRequestPassportPermission    
     FETCH NEXT FROM db_cursorInsertRequestPassportPermission INTO @idPassport    
     WHILE @@FETCH_STATUS = 0    
     BEGIN    
     /* Si el pasaporte tiene permisos sobre la petición lo guardamos en la tabla temporal */  
     if dbo.WebLogin_GetPermissionOverFeature(@idPassport,@alias, 'U', 0) > 3   
      AND dbo.WebLogin_GetPermissionOverEmployee(@idPassport,@employeeID,@featureID,2,1,getdate()) > 3   
     begin  
     SET @bInsert = (select count(*) from sysroPassports where isnull(GroupType,'') <> 'U' and IDParentPassport = @idPassport)  
     if @bInsert > 0  
     begin  
       INSERT INTO sysroPermissionsOverRequests VALUES (@idPassport, @pidRequest, dbo.GetEmployeeGroup(@employeeID,getdate()))  
     end  
        
     end  
                
      FETCH NEXT FROM db_cursorInsertRequestPassportPermission INTO @idPassport    
     END    
     CLOSE db_cursorInsertRequestPassportPermission    
     DEALLOCATE db_cursorInsertRequestPassportPermission   
        
     END
go

alter FUNCTION [dbo].[GetEmployeeGroupParent]   
   (  
    @idEmployeeGroup int  
   )  
  RETURNS int  
  AS  
   BEGIN  
    /* Returns an employee group's parent */  
    /* In Groups table, instead of having a IDParent field,  
    there is a Path field that looks like this: "2/14/20".  
    This should be changed, but would require too much changes in VisualTime.  
    Meanwhile, this function converts the Path field into what  
    IDParent should contain, by returning the 2nd last number.  
    Ex: 2/14/20 -> 14      3/50/90/91 -> 90      4 -> NULL*/  
      
    /* Get group path */  
    DECLARE @Path nvarchar(2000),
	  @pidEmployeeGroup int = @idEmployeeGroup
    SELECT @Path = Path  
    FROM Groups  
    WHERE ID = @pidEmployeeGroup  
      
    /* Find 2 last '\' positions */  
    DECLARE @Pos int, @CurrentSep int, @PreviousSep int  
    /* If there is only one '\', beginning of string will be considered as a separator */  
    SET @CurrentSep = 0  
    SELECT @Pos = CHARINDEX('\', @Path)  
    WHILE (@Pos > 0)  
    BEGIN  
     SET @PreviousSep = @CurrentSep  
     SET @CurrentSep = @Pos  
     SET @Pos = CHARINDEX('\', @Path, @Pos + 1)  
    END  
      
    /* Get value between those 2 '\' */  
    DECLARE @Value nvarchar(16)  
    SET @Value = SUBSTRING(@Path, @PreviousSep + 1, @CurrentSep - @PreviousSep - 1)  
      
   RETURN CAST(@Value AS int)  
   END
go

UPDATE [dbo].[TerminalReaders] set ValidationMode = 'Local' where idterminal in (select id from [dbo].[Terminals] where type = 'MX8')
GO

UPDATE dbo.sysroParameters SET Data='434' WHERE ID='DBVersion'
GO
