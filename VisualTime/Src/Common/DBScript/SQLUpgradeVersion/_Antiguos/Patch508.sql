 ALTER PROCEDURE [dbo].[InsertGroupPassportPermission]    
    (    
     @idGroup int  ,@Version int  
    )    
       AS    
       BEGIN    
    DECLARE @pidGroup int = @idGroup  , @pVersion int = @Version;    
    DELETE FROM sysroPermissionsOverGroups WHERE EmployeeGroupID = @pidGroup   
	
	if (@pVersion = 3)  
	begin
		DELETE FROM [dbo].[sysroPassports_PermissionsOverGroups] WHERE IDGroup = @pidGroup
	end

    DECLARE @featureEmployeeID int    
     DECLARE EmployeeFeatureCursor CURSOR    
     FOR     
      SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null    
     OPEN EmployeeFeatureCursor    
     FETCH NEXT FROM EmployeeFeatureCursor INTO @featureEmployeeID    
     WHILE @@FETCH_STATUS = 0    
     BEGIN     
   
   if (@pVersion = 3)  
     -- si es seguridad v3  
     begin  

	 INSERT INTO [dbo].[sysroPassports_PermissionsOverGroups] (IDPassport, IDGroup, IDApplication, Permission) 
        select * from (select supPassports.IDParentPassport, Groups.ID as GroupID, @featureEmployeeID as Feature, 
			(select Case When (select count(*) from sysroPassports_Groups where IDPassport = supPassports.ID and IDGroup IN (select * from SplitToInt(Groups.Path,'\')) ) > 0 Then 6 Else 0 End) as Permission
        from sysroPassports supPassports, Groups where supPassports.IsSupervisor = 1 AND Groups.ID = @pidGroup) tmpPerm where tmpPerm.Permission = 0

    INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission)     
      select supPassports.ID, Groups.ID, @featureEmployeeID, 1, dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,@featureEmployeeID,0)    
      from sysroPassports supPassports, Groups  where supPassports.GroupType = 'U' AND Groups.ID = @pidGroup    
   end  
   if (@pVersion <> 3)  
   -- si es seguridad v1 o v2  
   begin  
    INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission)     
     select supPassports.ID, Groups.ID, @featureEmployeeID, dbo.GetPassportLevelOfAuthority(supPassports.ID,0,0,1),    
    dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,@featureEmployeeID,0)    
     from sysroPassports supPassports, Groups    
     where supPassports.GroupType = 'U' AND Groups.ID = @pidGroup    
   end  
      FETCH NEXT FROM EmployeeFeatureCursor     
      INTO @featureEmployeeID    
     END     
     CLOSE EmployeeFeatureCursor    
     DEALLOCATE EmployeeFeatureCursor    
END      
GO

ALTER PROCEDURE [dbo].[ExecuteRequestPassportPermissionsAction]
	(
    @IDAction int,
    @IDObject int,@Version int
	)
	AS
	/* Añadimos todos los pasaportes que tienen permisos para una solicitud a la tabla temporal */
   	BEGIN
 		DECLARE @pIDAction int = @IDAction,  
			@pIDObject int = @IDObject, @pVersion int = @Version; 
   		IF @pIDAction = -2
   		BEGIN
  			exec dbo.sysro_GenerateAllPermissionsOverGroups @pVersion
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
  			exec dbo.sysro_UpdatePassportPermissionsOverGroups @pIDObject,@pVersion
  			exec dbo.sysro_UpdatePassportPermissionsOverFeatures @pIDObject
  			exec dbo.sysro_UpdatePassportPermissionsOverEmployeesExceptions @pIDObject
			exec dbo.InsertPassportRequestsPermission @pIDObject
   		END
   		IF @pIDAction = 1 -- Modificación passport
   		BEGIN
  			exec dbo.sysro_UpdatePassportPermissionsOverGroups @pIDObject,@pVersion
  			exec dbo.sysro_UpdatePassportPermissionsOverFeatures @pIDObject
  			exec dbo.sysro_UpdatePassportPermissionsOverEmployeesExceptions @pIDObject
			exec dbo.AlterPassportRequestsPermission @pIDObject
   		END
   		IF @pIDAction = 2 -- Creación solicitud
   		BEGIN
   			exec dbo.InsertRequestPassportPermission @pIDObject
   		END
   		IF @pIDAction = 3 -- Creación grupo de empleados
   		BEGIN
   			exec dbo.InsertGroupPassportPermission @pIDObject,@pVersion
   		END

		IF @pVersion > 1
		BEGIN
			exec dbo.RemoveInactivePassportPermissions
		END
		
   	END	
GO


-- Reseteamos el contador de informes para que en casos muy extremos no lleguen al ID 32000 y la CRUFLA de problemas
DELETE from sysroReportTasks
GO

DBCC CHECKIDENT ('sysroReportTasks', RESEED, 1) 
GO


ALTER PROCEDURE [dbo].[sysro_GenerateAllPermissionsOverGroups]
 (	
		@Version int 	
   )
     AS
     BEGIN
   	
 	DELETE FROM sysroPermissionsOverGroups

 	DECLARE @featureEmployeeID int, @pVersion int = @Version;
  	DECLARE EmployeeFeatureCursor CURSOR
  	FOR 
  		SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null
  	OPEN EmployeeFeatureCursor
  	FETCH NEXT FROM EmployeeFeatureCursor INTO @featureEmployeeID
  	WHILE @@FETCH_STATUS = 0
  	BEGIN	
		DELETE FROM sysroPermissionsOverGroups WHERE EmployeeFeatureID=@featureEmployeeID

  		if (@pVersion = 3)
	   -- si es seguridad v3
	   begin
  			INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission) 
  					select supPassports.ID, Groups.ID, @featureEmployeeID, 1,
  							dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,@featureEmployeeID,0)
  					from sysroPassports supPassports, Groups
  					where supPassports.GroupType = 'U'
		end

		if (@pVersion <> 3)
		-- si es seguridad v1 o v2
		begin
			INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission) 
 					select supPassports.ID, Groups.ID, @featureEmployeeID, dbo.GetPassportLevelOfAuthority(supPassports.ID,0,0,1),
 							dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,@featureEmployeeID,0)
 					from sysroPassports supPassports, Groups
 					where supPassports.GroupType = 'U'

  		end

  		FETCH NEXT FROM EmployeeFeatureCursor 
  		INTO @featureEmployeeID
  	END 
  	CLOSE EmployeeFeatureCursor
  	DEALLOCATE EmployeeFeatureCursor
 	    	
END
GO

UPDATE [Causes] SET MinLevelOfAuthority=11 WHERE MinLevelOfAuthority IS NULL OR  MinLevelOfAuthority = 0
GO
UPDATE [Causes] SET ApprovedAtLevel=1 WHERE ApprovedAtLevel IS NULL OR  ApprovedAtLevel = 0
GO
UPDATE [Causes] SET IDCategory=6 WHERE IDCategory IS NULL OR  IDCategory = 0
GO

UPDATE sysroParameters SET Data='508' WHERE ID='DBVersion'
GO

