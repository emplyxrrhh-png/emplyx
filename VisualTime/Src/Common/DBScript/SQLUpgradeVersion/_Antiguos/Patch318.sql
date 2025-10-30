-- Algunas tareas para Broadcaster (desde Driver RX) generaba contextos demasiado largos
alter table [dbo].sysrotasks alter column [Context] [nvarchar](MAX) NOT NULL
GO


-- NUevas tablas y storeds para calcular los permisos sobre solicitudes
CREATE TABLE [dbo].[TmpRequestPassportPermissions](
	[IDParentPassport] [int] NOT NULL,
	[IDRequest] [int] NOT NULL,
 CONSTRAINT [PK_TmpRequestPassportPermissions] PRIMARY KEY CLUSTERED 
(
	[IDParentPassport] ASC,
	[IDRequest]
) ON [PRIMARY]
) ON [PRIMARY] 

GO

CREATE TABLE [dbo].[RequestPassportPermissionsPending](
	[ID] [numeric](16, 0) IDENTITY(1,1) NOT NULL,
	[IDAction] [int] NOT NULL,
	[IDObject] [int] NOT NULL
 CONSTRAINT [PK_[RequestPassportPermissionsPending] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY] 

GO

CREATE FUNCTION [dbo].[GetAllPassportParentsEmployeeType]
  	(
  		
  	)
  RETURNS @result table (IDPassport int)
  AS
  BEGIN
  	/* Obtenemos todos los passports que tienen el acceso prohibido a VTSupervisor
	   Solo serán passports de tipo 'U' que són los unicos donde se puede definir la restricción */
  	Declare @idPassport int
  	Declare @idRunningPassport int
  	Declare @BlockAccessVTSupervisor int
  	
  	DECLARE db_cursor CURSOR FOR  
	SELECT ID FROM (SELECT ID, sysroSecurityParameters.BlockAccessVTSupervisor
 	FROM sysroPassports INNER JOIN sysroSecurityParameters ON sysroSecurityParameters.IDPassport =  sysroPassports.ID) sp
 	WHERE ISNULL(BlockAccessVTSupervisor,0) = 1
 	
 	OPEN db_cursor  
 	FETCH NEXT FROM db_cursor INTO @idPassport  
 	WHILE @@FETCH_STATUS = 0  
 	BEGIN  
 	
		/* Insertamos el passport que tiene la restricción directa */
		INSERT INTO @result VALUES (@idPassport)

     	DECLARE db_cursorb CURSOR FOR  
		SELECT ID FROM GetPassportChilds(@idPassport)
		WHERE ID in(SELECT ID FROM SYSROPASSPORTS WHERE GroupType = 'U')
		OPEN db_cursorb  
 			FETCH NEXT FROM db_cursorb INTO @idRunningPassport  
 			WHILE @@FETCH_STATUS = 0  
 			BEGIN   			
				/* Añadimos todos los passports que tienen como padre el passport que tiene la restricción directa */
 				INSERT INTO @result VALUES (@idRunningPassport)
	 			FETCH NEXT FROM db_cursorb INTO @idRunningPassport  
 			END
 		CLOSE db_cursorb
 		DEALLOCATE db_cursorb
 				   
 		FETCH NEXT FROM db_cursor INTO @idPassport  
 	END  
 	CLOSE db_cursor  
 	DEALLOCATE db_cursor 
  	
  	RETURN 
  END
GO

ALTER FUNCTION [dbo].[GetAllPassportParentsRequestPermissions]
  	(
  		
  	)
  RETURNS @result table (IDPassport int, IDParentPassport int, RequestPermissionCount int)
  AS
  BEGIN
  	/* Returns all parents of specified passport */
  	
  	Declare @idPassport int
  	Declare @idRunningPassport int
  	Declare @permissionCount int

  	DECLARE db_cursor CURSOR FOR  
 	SELECT ID
 	FROM sysroPassports
 	WHERE GroupType <> 'U' And IDUser is not null and IDParentPassport NOT IN(SELECT IDPassport  FROM GetAllPassportParentsEmployeeType())
 	OPEN db_cursor  
 	FETCH NEXT FROM db_cursor INTO @idPassport  
 	WHILE @@FETCH_STATUS = 0  
 	BEGIN  
 		   set @idRunningPassport = @idPassport
 		   
 		   SELECT @idRunningPassport = IDParentPassport FROM sysroPassports WHERE ID = @idRunningPassport
 		   
 		   WHILE NOT @idRunningPassport IS NULL
 			BEGIN

				select @permissionCount = (select COUNT(*) from sysroPassports_PermissionsOverFeatures where IDPassport = @idRunningPassport and IDFeature in (1560,2321,2322,2323,2510,2520,2540,2550,25800))

 				INSERT INTO @result VALUES (@idPassport,@idRunningPassport, @permissionCount)
 				
 				SELECT @idRunningPassport = IDParentPassport
 				FROM sysroPassports
 				WHERE ID = @idRunningPassport
 			END
 			
 				   
 		   FETCH NEXT FROM db_cursor INTO @idPassport  
 	END  
 	CLOSE db_cursor  
 	DEALLOCATE db_cursor 
  	
  	RETURN 
  END
GO

CREATE PROCEDURE [dbo].[GenerateAllRequestPassportPermission]
  AS
  BEGIN
	/* Rellenamos una tabla  con todos los passaportes de tipo grupo y cada una de las peticiones sobre las que tiene permisos */
  	DECLARE @idRequest int,
			@idPassport int,
			@featureID int,
			@employeeID int,
			@alias varchar(100)

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
					INSERT INTO TmpRequestPassportPermissions VALUES (@idPassport, @idRequest)
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

CREATE PROCEDURE [dbo].[InsertRequestPassportPermission]
  	(
  		@IDRequest int
  	)
  AS
  /* Añadimos todos los pasaportes que tienen permisos para una solicitud a la tabla temporal */
  BEGIN
  	DECLARE @idPassport int,
			@featureID int,
			@employeeID int,
			@alias varchar(100)


	/* Obtenemos el empleado, feature y alias requeridos para comprobar permisos */
	SELECT @employeeID = Requests.IDEmployee, @alias = sysroFeatures.Alias, @featureID = sysroFeatures.EmployeeFeatureID
		FROM Requests inner join sysroFeatures on sysroFeatures.AliasID = Requests.RequestType

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
			INSERT INTO TmpRequestPassportPermissions VALUES (@idPassport, @idRequest)
		end
 	    		   
 		FETCH NEXT FROM db_cursor INTO @idPassport  
 	END  
 	CLOSE db_cursor  
 	DEALLOCATE db_cursor 
  	
  END
GO

CREATE PROCEDURE [dbo].[InsertPassportRequestsPermission]
  	(
  		@IDParentPassport int
  	)
  AS
  /* Al crear un nuevo pasaporte debemos añadir todos los permisos que tiene este sobre las peticiones */
  BEGIN
  	DECLARE @idRequest int,
			@featureID int,
			@employeeID int,
			@alias varchar(100)

	
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
				INSERT INTO TmpRequestPassportPermissions VALUES (@IDParentPassport, @idRequest)
			end
 	        	
	 		FETCH NEXT FROM db_cursorb INTO  @idRequest, @employeeID, @alias , @featureID
 		END


 	CLOSE db_cursorb
 	DEALLOCATE db_cursorb

  END
GO

CREATE PROCEDURE [dbo].[AlterPassportRequestsPermission]
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
			@alias varchar(100)

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
					INSERT INTO TmpRequestPassportPermissions VALUES (@idPassport, @idRequest)
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

ALTER FUNCTION [dbo].[GetRequestLevelsBelow]
   	(
   	@idPassport int,
   	@idRequest int
   	)
   RETURNS int
   AS
   BEGIN
   	DECLARE @LevelsBelow int,
   			@LevelOfAuthority int,
   			@RequestLevel int
   	SELECT @LevelOfAuthority = dbo.GetPassportLevelOfAuthority(@idPassport)
   	
   	SELECT @RequestLevel = ISNULL(Requests.StatusLevel, 11)
      FROM Requests
   	WHERE Requests.ID = @idRequest
   	

   	/* Obtiene el número de niveles que hay entre el nivel del validador (@idPassport) y el nivel actual de la solicitud (@RequestLevel) */
   	SELECT @LevelsBelow = 
	(
		SELECT COUNT( DISTINCT trpp.LevelOfAuthority) from 
		(SELECT dbo.GetPassportLevelOfAuthority(IDParentPassport) AS LevelOfAuthority, IDRequest FROM TmpRequestPassportPermissions WHERE IDRequest = @idRequest)trpp
		WHERE trpp.LevelOfAuthority > @LevelOfAuthority AND trpp.LevelOfAuthority <= @RequestLevel
	)
    
    IF @RequestLevel = 11  SET @LevelsBelow = @LevelsBelow + 1
   	
   	RETURN @LevelsBelow
   
 END
GO
 
ALTER PROCEDURE [dbo].[WebLogin_Passports_Insert] 
   	(
   		@id int OUTPUT, @idParentPassport int, @groupType varchar(1), @name nvarchar(50), @description nvarchar(100), @email nvarchar(100), @idUser int, @idEmployee int, @idLanguage int,
   		@levelOfAuthority tinyint, @ConfData text, @AuthenticationMerge nvarchar(50), @StartDate smalldatetime, @ExpirationDate smalldatetime, @State smallint, @EnabledVTDesktop bit,
 		@EnabledVTPortal bit, @EnabledVTSupervisor bit, @EnabledVTPortalApp bit , @EnabledVTSupervisorApp bit , @NeedValidationCode bit , @TimeStampValidationCode smalldatetime ,
 		@ValidationCode nvarchar(100)
   	)
   AS
   	INSERT INTO sysroPassports (
   		IDParentPassport, GroupType, Name, Description, Email, IDUser, IDEmployee, IDLanguage, LevelOfAuthority, ConfData, AuthenticationMerge, StartDate, ExpirationDate, [State],
  		EnabledVTDesktop, EnabledVTPortal, EnabledVTSupervisor, EnabledVTPortalApp, EnabledVTSupervisorApp, NeedValidationCode, TimeStampValidationCode, ValidationCode
   	)
   	VALUES (
   		@idParentPassport, @groupType, @name, @description, @email, @idUser, @idEmployee, @idLanguage, @levelOfAuthority, @ConfData, @AuthenticationMerge, @StartDate, @ExpirationDate,
  		@State, @EnabledVTDesktop, @EnabledVTPortal, @EnabledVTSupervisor, @EnabledVTPortalApp, @EnabledVTSupervisorApp, @NeedValidationCode, @TimeStampValidationCode, @ValidationCode 
   	)
   	
   	SELECT @id = @@IDENTITY
   	
	IF @groupType = 'U' 
  	BEGIN
		-- 0 / Actualizar los permisos del grupo y sus hijos
  		insert into RequestPassportPermissionsPending Values(0, @id)
  	END

   	RETURN
GO

ALTER PROCEDURE [dbo].[WebLogin_Passports_Delete] 
  	(
  		@id int
  	)
  AS
  	SET NOCOUNT ON
  	
	DELETE FROM TmpRequestPassportPermissions
	WHERE IDParentPassport = @id

  	DELETE FROM sysroSecurityParameters
  	WHERE IDPassport = @id
  	 	
  	DELETE FROM sysroPassports_PasswordHistory
  	WHERE IDPassport = @id
  	
 	DELETE FROM sysroPassports_AuthorizedAdress
  	WHERE IDPassport = @id
  	
	DELETE FROM sysroPassports
  	WHERE ID = @id
  	
  	SET NOCOUNT OFF
  	
  	RETURN
GO

 ALTER PROCEDURE [dbo].[WebLogin_PermissionsOverEmployees_Set] 
 	(
 		@idPassport int,
 		@idEmployee int,
 		@idApplication int,
 		@permission tinyint
 	)
 AS
 	SET NOCOUNT ON
 	
	DECLARE @PassportType varchar(1)
	SELECT @PassportType = ISNULL(GroupType,'') FROM sysroPassports WHERE ID = @idPassport

 	IF EXISTS ( SELECT IDPassport FROM sysroPassports_PermissionsOverEmployees WHERE IDPassport = @idPassport AND IDEmployee = @idEmployee AND IDApplication = @idApplication)
 		UPDATE sysroPassports_PermissionsOverEmployees SET Permission = @permission WHERE IDPassport = @idPassport AND IDEmployee = @idEmployee AND IDApplication = @idApplication
 	ELSE
 		INSERT INTO sysroPassports_PermissionsOverEmployees ( IDPassport, IDEmployee, IDApplication, Permission ) VALUES ( @idPassport, @idEmployee, @idApplication, @permission )
 	
 	
 	/* Remove all exceptions defined on child passports for employee 
 	This operation is optional but improves usability. */
 	DELETE FROM sysroPassports_PermissionsOverEmployees WHERE IDApplication = @idApplication AND IDPassport IN (SELECT ID FROM dbo.GetPassportChilds(@idPassport)) AND IDEmployee = @idEmployee
 	
	IF @PassportType = 'U' 
  	BEGIN
		-- 1 / Actualizar los permisos del grupo y sus hijos
		IF NOT EXISTS ( SELECT ID FROM RequestPassportPermissionsPending WHERE IDAction = 1 AND IDObject = @idPassport)
			insert into RequestPassportPermissionsPending Values(1, @idPassport)
  	END
		
 	SET NOCOUNT OFF
 	
 	RETURN
GO

 ALTER PROCEDURE [dbo].[WebLogin_PermissionsOverFeatures_Set] 
 	(
 		@idPassport int,
 		@featureAlias nvarchar(100),
 		@featureType varchar(1),
 		@permission tinyint
 	)
 AS
 	SET NOCOUNT ON
 	/* Get feature ID */
 	DECLARE @IDFeature int
	DECLARE @PassportType varchar(1)
 	SELECT @IDFeature = ID 	FROM sysroFeatures 	WHERE Alias = @featureAlias AND Type = @featureType
	SELECT @PassportType = ISNULL(GroupType,'') FROM sysroPassports WHERE ID = @idPassport
 	
	/* Set permission (insert or update) */
 	IF EXISTS (SELECT IDPassport FROM sysroPassports_PermissionsOverFeatures WHERE IDPassport = @idPassport AND IDFeature = @IDFeature)
 		UPDATE sysroPassports_PermissionsOverFeatures SET Permission = @permission WHERE IDPassport = @idPassport AND IDFeature = @IDFeature
 	ELSE
 		INSERT INTO sysroPassports_PermissionsOverFeatures(IDPassport, IDFeature, Permission) VALUES (@idPassport, @IDFeature, @permission)
 	
 	/* Ensure child passports permissions remain valid */
 	DELETE FROM sysroPassports_PermissionsOverFeatures
 	WHERE IDPassport IN (SELECT ID FROM dbo.GetPassportChilds(@idPassport)) AND
 		IDFeature = @IDFeature AND
 		Permission >= @permission
 	
	IF @PassportType = 'U' 
  	BEGIN
		-- 1 / Actualizar los permisos del grupo y sus hijos
		IF NOT EXISTS ( SELECT ID FROM RequestPassportPermissionsPending WHERE IDAction = 1 AND IDObject = @idPassport)
			insert into RequestPassportPermissionsPending Values(1, @idPassport)
  	END
		
 	SET NOCOUNT OFF
 	
 	RETURN
GO

 ALTER PROCEDURE [dbo].[WebLogin_PermissionsOverGroups_Set] 
 	(
 		@idPassport int,
 		@idGroup int,
 		@idApplication int,
 		@permission tinyint
 	)
 AS
 	/* Set permission (insert or update) */
 	SET NOCOUNT ON
 	DECLARE @PassportType varchar(1)
	SELECT @PassportType = ISNULL(GroupType,'') FROM sysroPassports WHERE ID = @idPassport

	IF EXISTS (SELECT IDPassport FROM sysroPassports_PermissionsOverGroups WHERE IDPassport = @idPassport AND IDGroup = @idGroup AND IDApplication = @idApplication)
 		UPDATE sysroPassports_PermissionsOverGroups SET Permission = @permission WHERE IDPassport = @idPassport AND IDGroup = @idGroup AND IDApplication = @idApplication
 	ELSE
 		INSERT INTO sysroPassports_PermissionsOverGroups (IDPassport, IDGroup, IDApplication, Permission) VALUES (@idPassport,@idGroup,@idApplication,@permission)
 	
 	/* Ensure permissions validity */
 	DELETE FROM sysroPassports_PermissionsOverGroups
 	WHERE IDApplication = @idApplication AND
 		(
 			/* On same passport, delete childs(by emp groups) having same permission */
 			IDPassport = @idPassport AND
 			IDGroup IN (SELECT ID FROM dbo.GetEmployeeGroupChilds(@idGroup)) AND
 			Permission = @permission
 		)
 		OR
 		(
 			/* On child passports, delete childs(by emp groups) having >= permission */	
 			IDPassport IN (SELECT ID FROM dbo.GetPassportChilds(@idPassport)) AND
 			IDGroup IN (SELECT ID FROM dbo.GetEmployeeGroupChilds(@idGroup)) AND
 			Permission >= @permission
 		)
	
	IF @PassportType = 'U' 
  	BEGIN
		-- 1 / Actualizar los permisos del grupo y sus hijos
		IF NOT EXISTS ( SELECT ID FROM RequestPassportPermissionsPending WHERE IDAction = 1 AND IDObject = @idPassport)
			insert into RequestPassportPermissionsPending Values(1, @idPassport)
  	END
	 				
 	SET NOCOUNT OFF
 	RETURN
GO

CREATE PROCEDURE [dbo].[ExecuteRequestPassportPermissionsAction]
  	(
		@IDAction int,
  		@IDObject int
  	)
  AS
  /* Añadimos todos los pasaportes que tienen permisos para una solicitud a la tabla temporal */
  BEGIN
	IF @IDAction = -1
	BEGIN
		exec dbo.GenerateAllRequestPassportPermission
	END

  	IF @IDAction = 0
	BEGIN
		exec dbo.InsertPassportRequestsPermission @IDObject
	END

	IF @IDAction = 1
	BEGIN
		exec dbo.AlterPassportRequestsPermission @IDObject  
	END

	IF @IDAction = 2
	BEGIN
		exec dbo.InsertRequestPassportPermission @IDObject
	END
  	
  END
GO

ALTER PROCEDURE [dbo].[WebLogin_PermissionsOverFeatures_Remove] 
 	(
 		@idPassport int,
 		@featureAlias nvarchar(100),
 		@featureType varchar(1)
 	)
 AS
 	SET NOCOUNT ON
 	
 	/* Get feature ID */
 	DECLARE @IDFeature int
 	SELECT @IDFeature = ID
 	FROM sysroFeatures
 	WHERE Alias = @featureAlias AND
 		Type = @featureType
 	/* Remove permission */
 	DELETE FROM sysroPassports_PermissionsOverFeatures
 	WHERE IDPassport = @idPassport AND
 		IDFeature = @IDFeature
 		
 		
 	/* Ensure child passports permissions remain valid */
 	/* Get current permission on feature */
 	DECLARE @Permission int
 	SELECT @Permission = dbo.WebLogin_GetPermissionOverFeature(@idPassport, @featureAlias, @featureType, 0)
	DECLARE @PassportType varchar(1)
 	SELECT @PassportType = ISNULL(GroupType,'') FROM sysroPassports WHERE ID = @idPassport

 	
 	/* Ensure permissions validity */
 	DELETE FROM sysroPassports_PermissionsOverFeatures
 	WHERE IDPassport IN (SELECT ID FROM dbo.GetPassportChilds(@idPassport)) AND
 		IDFeature = @IDFeature AND
 		Permission >= @Permission
 		
 		
 	IF @PassportType = 'U' 
  	BEGIN
		-- 1 / Actualizar los permisos del grupo y sus hijos
		IF NOT EXISTS ( SELECT ID FROM RequestPassportPermissionsPending WHERE IDAction = 1 AND IDObject = @idPassport)
			insert into RequestPassportPermissionsPending Values(1, @idPassport)
  	END
 	
 	SET NOCOUNT OFF
 	
 	RETURN
GO

 ALTER PROCEDURE [dbo].[WebLogin_PermissionsOverEmployees_Remove] 
 	(
 		@idPassport int,
 		@idEmployee int,
 		@idApplication int
 	)
 AS
 	SET NOCOUNT ON
 	
 	DELETE FROM sysroPassports_PermissionsOverEmployees
 	WHERE IDPassport = @idPassport AND
 		IDEmployee = @idEmployee AND
 		IDApplication = @idApplication
 	
 	
 	/* Get current permission on employee */		
 	DECLARE @Date datetime
 	SET @Date = GetDate()
 	DECLARE @Permission int
 	SELECT @Permission = dbo.WebLogin_GetPermissionOverEmployee(@idPassport, @idEmployee, @idApplication, 0, 1, @Date)
	DECLARE @PassportType varchar(1)
	SELECT @PassportType = ISNULL(GroupType,'') FROM sysroPassports WHERE ID = @idPassport

 	
 	/* Ensure child passports permissions remain valid */
 	DELETE FROM sysroPassports_PermissionsOverEmployees
 	WHERE IDApplication = @idApplication AND
 		IDPassport IN (SELECT ID FROM dbo.GetPassportChilds(@idPassport)) AND
 		IDEmployee = @idEmployee AND
 		Permission >= @Permission
 		
	IF @PassportType = 'U' 
  	BEGIN
		-- 1 / Actualizar los permisos del grupo y sus hijos
		IF NOT EXISTS ( SELECT ID FROM RequestPassportPermissionsPending WHERE IDAction = 1 AND IDObject = @idPassport)
			insert into RequestPassportPermissionsPending Values(1, @idPassport)
  	END
 		
 		
 	SET NOCOUNT OFF
 	
 	RETURN
GO

 ALTER PROCEDURE [dbo].[WebLogin_PermissionsOverGroups_Remove] 
 	(
 		@idPassport int,
 		@idGroup int,
 		@idApplication int
 	)
 AS
 	SET NOCOUNT ON
 	
 	DELETE FROM sysroPassports_PermissionsOverGroups
 	WHERE IDPassport = @idPassport AND
 		IDGroup = @idGroup AND
 		IDApplication = @idApplication
 		
 	/* Ensure child passports permissions remain valid */
 	/* Get current permission on group */
 	DECLARE @Permission int
 	SELECT @Permission = dbo.WebLogin_GetPermissionOverGroup(@idPassport, @idGroup, @idApplication, 0)
 	
	DECLARE @PassportType varchar(1)
	SELECT @PassportType = ISNULL(GroupType,'') FROM sysroPassports WHERE ID = @idPassport

 	
 	/* Ensure permissions validity */
 	DELETE FROM sysroPassports_PermissionsOverGroups
 	WHERE IDApplication = @idApplication AND
 		(
 			/* On same passport, delete childs(by emp groups) having same permission */
 			IDPassport = @idPassport AND
 			IDGroup IN (SELECT ID FROM dbo.GetEmployeeGroupChilds(@idGroup)) AND
 			Permission = @permission
 		)
 		OR
 		(
 			/* On child passports, delete childs(by emp groups) having >= permission */	
 			IDPassport IN (SELECT ID FROM dbo.GetPassportChilds(@idPassport)) AND
 			IDGroup IN (SELECT ID FROM dbo.GetEmployeeGroupChilds(@idGroup)) AND
 			Permission >= @permission
 		)
 		
 	IF @PassportType = 'U' 
  	BEGIN
		-- 1 / Actualizar los permisos del grupo y sus hijos
		IF NOT EXISTS ( SELECT ID FROM RequestPassportPermissionsPending WHERE IDAction = 1 AND IDObject = @idPassport)
			insert into RequestPassportPermissionsPending Values(1, @idPassport)
  	END
	
 		
 		
 	SET NOCOUNT OFF
 	
 	RETURN
GO


exec dbo.GenerateAllRequestPassportPermission
GO

DROP VIEW sysrovwCurrentEmployeesPresenceStatusPunches
GO

-- Crea la vista de nuevo para que determinadas salidas justificadas no se consideren SALIDA sino ENTRADA
-- Las justificaciones que cuando se fichen como salida deben ser consideradas como entrada son aquellas cuya descripción contiene el texto #EmergencyIn#
CREATE VIEW sysrovwCurrentEmployeesPresenceStatusPunches As
SELECT     p.IDEmployee, em.Name AS EmployeeName, p.DateTime, p.IDTerminal AS IDReader, CASE WHEN ActualType IN (1, 2) 
                      THEN 'Att' ELSE 'Acc' END AS MoveType, ISNULL(p.IDZone, 0) AS IDZone, 
                      CASE WHEN ActualType = 1 THEN 'IN' WHEN ActualType = 2 THEN (CASE WHEN CHARINDEX('#EmergencyIn#', dbo.Causes.Description) 
                      > 0 THEN 'IN' ELSE 'OUT' END) ELSE CASE zo.IsWorkingZone WHEN 1 THEN 'IN' ELSE (CASE WHEN CHARINDEX('#EmergencyIn#', dbo.Causes.Description) 
                      > 0 THEN 'IN' ELSE 'OUT' END) END END AS Status
FROM         dbo.Punches AS p INNER JOIN
                          (SELECT     dbo.Punches.IDEmployee, MAX(dbo.Punches.ID) AS idp
                            FROM          dbo.Punches INNER JOIN
                                                       (SELECT     IDEmployee, MAX(DateTime) AS dat
                                                         FROM          dbo.Punches AS Punches_1
                                                         WHERE      (ActualType IN (1, 2)) OR
                                                                                (Type = 5)
                                                         GROUP BY IDEmployee) AS maxidat ON dbo.Punches.IDEmployee = maxidat.IDEmployee AND dbo.Punches.DateTime = maxidat.dat
                            WHERE      (dbo.Punches.ActualType IN (1, 2)) OR
                                                   (dbo.Punches.Type = 5)
                            GROUP BY dbo.Punches.IDEmployee) AS maxPunch ON p.IDEmployee = maxPunch.IDEmployee AND p.ID = maxPunch.idp LEFT OUTER JOIN
                      dbo.Causes ON p.TypeData = dbo.Causes.ID LEFT OUTER JOIN
                      dbo.Zones AS zo ON p.IDZone = zo.ID LEFT OUTER JOIN
                      dbo.Employees AS em ON p.IDEmployee = em.ID
					  
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='318' WHERE ID='DBVersion'
GO

