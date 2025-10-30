
 ALTER PROCEDURE [dbo].[InsertRequestPassportPermission]
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
			@bInsert int
 	/* Obtenemos el empleado, feature y alias requeridos para comprobar permisos */
 	SELECT @employeeID = Requests.IDEmployee, @alias = sysroFeatures.Alias, @featureID = sysroFeatures.EmployeeFeatureID
 		FROM Requests inner join sysroFeatures on sysroFeatures.AliasID = Requests.RequestType where Requests.ID = @IDRequest
 	DELETE FROM TmpRequestPassportPermissions WHERE IDRequest = @IDRequest
 	/* Obtenemos todos los pasaportes de tipo grupo que pueden tener permisos sobre la petición */
 	DECLARE db_cursor CURSOR FOR  
 	SELECT ID from sysroPassports where GroupType = 'U' and ID NOT IN(SELECT IDPassport FROM dbo.GetAllPassportParentsEmployeeType())
  	
  	OPEN db_cursor  
  	FETCH NEXT FROM db_cursor INTO @idPassport  
  	WHILE @@FETCH_STATUS = 0  
  	BEGIN  
 		/* Si el pasaporte tiene permisos sobre la petición lo guardamos en la tabla temporal */
 		if dbo.WebLogin_GetPermissionOverFeature(@idPassport,@alias, 'U', 0) > 3 
 			AND dbo.WebLogin_GetPermissionOverEmployee(@idPassport,@employeeID,@featureID,2,1,getdate()) > 3 
 		begin
			SET @bInsert = (select count(*) from sysroPassports where isnull(GroupType,'') <> 'U' and IDParentPassport = @idPassport)

			if @bInsert > 0
			begin
 				INSERT INTO TmpRequestPassportPermissions VALUES (@idPassport, @idRequest)
			end
 			
 		end
  	    		   
  		FETCH NEXT FROM db_cursor INTO @idPassport  
  	END  
  	CLOSE db_cursor  
  	DEALLOCATE db_cursor 
   	
   END
   GO

update dbo.sysroPassports set DataUser = null where DataUser is not null
GO
insert into dbo.sysroFeatures values(2530,2500,'Calendar.Requests.CancelHolidays','Cancelar vacaciones','','U','RWA',NULL,11,2)
GO
insert into dbo.sysroPassports_PermissionsOverFeatures  select IDPassport, 2530, Permission  from dbo.sysroPassports_PermissionsOverFeatures where IDFeature = 2520
GO

-- Notifición: Aviso de solicitud
update [dbo].[sysroNotificationTypes] set [OnlySystem]=0 where id=40
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='329' WHERE ID='DBVersion'
GO


