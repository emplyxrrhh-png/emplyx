
ALTER PROCEDURE [dbo].[GenerateAllRequestPassportPermission]
   AS
   BEGIN
 	/* Rellenamos una tabla  con todos los passaportes de tipo grupo y cada una de las peticiones sobre las que tiene permisos */
   	DECLARE @idRequest int,
 			@idPassport int,
 			@featureID int,
 			@employeeID int,
 			@alias varchar(100),
			@bInsert int
 	DECLARE db_cursor CURSOR FOR  
 	
 	/* Obtenemos todos los pasaportes de tipo grupo */
 	SELECT ID from sysroPassports where GroupType = 'U' and ID NOT IN(SELECT IDPassport  FROM GetAllPassportParentsEmployeeType())
  	
  	OPEN db_cursor  
  	FETCH NEXT FROM db_cursor INTO @idPassport  
  	WHILE @@FETCH_STATUS = 0  
  	BEGIN  
  	 	/* Obtenemos todos los peticiones con sus alias y features requeridas */
 		DECLARE db_cursorb CURSOR FOR  
 		SELECT Requests.ID, Requests.IDEmployee, sysroFeatures.Alias, sysroFeatures.EmployeeFeatureID
 		FROM Requests inner join sysroFeatures on sysroFeatures.AliasID = Requests.RequestType
 		DELETE FROM TmpRequestPassportPermissions WHERE IDParentPassport = @idPassport
 		OPEN db_cursorb  
  			FETCH NEXT FROM db_cursorb INTO @idRequest, @employeeID, @alias , @featureID
  			WHILE @@FETCH_STATUS = 0  
  			BEGIN
 				/* Si el pasaporte actual tiene permisos sobre la feature y sobre el empleado de la peticion lo añadimos a la tabla de permisos*/
 				if dbo.WebLogin_GetPermissionOverFeature(@idPassport,@alias, 'U', 0) > 3 
 					AND dbo.WebLogin_GetPermissionOverEmployee(@idPassport,@employeeID,@featureID,2,1,getdate()) > 3 
 				begin
					SET @bInsert = (select count(*) from sysroPassports where isnull(GroupType,'') <> 'U' and IDParentPassport = @idPassport)

					if @bInsert > 0
					begin
 						INSERT INTO TmpRequestPassportPermissions VALUES (@idPassport, @idRequest)
					end
 				end
  	        	
 	 		   FETCH NEXT FROM db_cursorb INTO  @idRequest, @employeeID, @alias , @featureID
  			END
  		CLOSE db_cursorb
  		DEALLOCATE db_cursorb
  			
  				   
  		FETCH NEXT FROM db_cursor INTO @idPassport  
  	END  
  	CLOSE db_cursor  
  	DEALLOCATE db_cursor 
   	
   END
   GO
   
   ALTER PROCEDURE [dbo].[InsertPassportRequestsPermission]
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
			@bInsert int
 	
 	/* Obtenemos todas las solicitudes con sus datos de features */
  	DECLARE db_cursorb CURSOR FOR  
 	SELECT Requests.ID, Requests.IDEmployee, sysroFeatures.Alias, sysroFeatures.EmployeeFeatureID
 	FROM Requests inner join sysroFeatures on sysroFeatures.AliasID = Requests.RequestType
 	/* Borramos los datos actuales del passport */
 	DELETE FROM TmpRequestPassportPermissions WHERE IDParentPassport = @IDParentPassport
 	OPEN db_cursorb  
  		FETCH NEXT FROM db_cursorb INTO @idRequest, @employeeID, @alias , @featureID
  		WHILE @@FETCH_STATUS = 0  
  		BEGIN
 			/* Insertamos en el caso que sea necesario sobre la tabla de permisos */
 			if dbo.WebLogin_GetPermissionOverFeature(@IDParentPassport,@alias, 'U', 0) > 3 
 				AND dbo.WebLogin_GetPermissionOverEmployee(@IDParentPassport,@employeeID,@featureID,2,1,getdate()) > 3 
 			begin
				SET @bInsert = (select count(*) from sysroPassports where isnull(GroupType,'') <> 'U' and IDParentPassport = @IDParentPassport)

				if @bInsert > 0
				begin
 					INSERT INTO TmpRequestPassportPermissions VALUES (@IDParentPassport, @idRequest)
				end
 			end
  	        	
 	 		FETCH NEXT FROM db_cursorb INTO  @idRequest, @employeeID, @alias , @featureID
  		END
  	CLOSE db_cursorb
  	DEALLOCATE db_cursorb
   END
   GO
   
   ALTER PROCEDURE [dbo].[AlterPassportRequestsPermission]
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
			@bInsert int

 	/* Obtenemos todos los pasaportes de tipo grupo que pueden supervisar y que estan dentro del arbol de grupos del pasaporte a actualizar */
 	DECLARE db_cursor CURSOR FOR  
 	SELECT ID from sysroPassports where GroupType = 'U' and ID NOT IN(SELECT IDPassport FROM dbo.GetAllPassportParentsEmployeeType()) and (ID in(SELECT ID FROM dbo.GetPassportChilds(@IDParentPassport)) or ID = @IDParentPassport)
  	
  	OPEN db_cursor  
  	FETCH NEXT FROM db_cursor INTO @idPassport  
  	WHILE @@FETCH_STATUS = 0  
  	BEGIN  
 		/* Borramos los datos actuales del passport */
 		DELETE FROM TmpRequestPassportPermissions WHERE IDParentPassport = @idPassport
 		/* Obtenemos todas las solicitudes con sus datos de features */
  	 	DECLARE db_cursorb CURSOR FOR  
 		SELECT Requests.ID, Requests.IDEmployee, sysroFeatures.Alias, sysroFeatures.EmployeeFeatureID
 		FROM Requests inner join sysroFeatures on sysroFeatures.AliasID = Requests.RequestType  WHERE Requests.Status in (0,1)
 		OPEN db_cursorb  
  			FETCH NEXT FROM db_cursorb INTO @idRequest, @employeeID, @alias , @featureID
  			WHILE @@FETCH_STATUS = 0  
  			BEGIN
 				/* Insertamos en el caso que sea necesario sobre la tabla de permisos */
 				if dbo.WebLogin_GetPermissionOverFeature(@idPassport,@alias, 'U', 0) > 3 
 					AND dbo.WebLogin_GetPermissionOverEmployee(@idPassport,@employeeID,@featureID,2,1,getdate()) > 3 
 				begin
					SET @bInsert = (select count(*) from sysroPassports where isnull(GroupType,'') <> 'U' and IDParentPassport = @idPassport)

					if @bInsert > 0
					begin
 						INSERT INTO TmpRequestPassportPermissions VALUES (@idPassport, @idRequest)
					end
 					
 				end
  	        	
 	 		   FETCH NEXT FROM db_cursorb INTO  @idRequest, @employeeID, @alias , @featureID
  			END
  		CLOSE db_cursorb
  		DEALLOCATE db_cursorb
  			
  				   
  		FETCH NEXT FROM db_cursor INTO @idPassport  
  	END  
  	CLOSE db_cursor  
  	DEALLOCATE db_cursor 
   	
   END
   GO

-- Tiempo de muestreo de alertas
update sysroNotificationTypes set scheduler = 5 where ID = 41
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='328' WHERE ID='DBVersion'
GO

