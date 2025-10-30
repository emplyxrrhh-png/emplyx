----- IMPORTACIÓN AUSENCIAS PROLONGADAS
INSERT INTO [dbo].[ImportGuides]
           ([ID]
           ,[Name]
           ,[Template]
           ,[Mode]
           ,[Type]
           ,[FormatFilePath]
           ,[SourceFilePath]
           ,[Separator]
           ,[CopySource]
           ,[LastLog])
     VALUES
           (10,'Carga de ausencias prolongadas', 0,1,2,'','',';',1,'')
GO

--- Tiempo máximo de sesión 30 minutos
DECLARE @STRPARAMETERS  nvarchar(MAX)
SET @STRPARAMETERS =  (SELECT DATA FROM dbo.sysroParameters WHERE ID='OPTIONS')
SET @STRPARAMETERS = REPLACE(@STRPARAMETERS,'SessionTimeout" type="2">5','SessionTimeout" type="2">30')
UPDATE dbo.sysroParameters SET Data = @STRPARAMETERS WHERE ID='OPTIONS'
GO
 
 --- Permisos V2 en tablas
 
 --Creación de la nueva tabla de permisos calculados sobre grupos
CREATE TABLE dbo.sysroPermissionsOverFeatures
	(
	PassportID int NOT NULL,
	FeatureID int NOT NULL,
	Permission int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.sysroPermissionsOverFeatures ADD CONSTRAINT
	PK_sysroPermissionsOverFeatures PRIMARY KEY CLUSTERED 
	(
	PassportID,
	FeatureID
	)  ON [PRIMARY]
GO

--Creación de la nueva tabla de permisos calculados sobre grupos
CREATE TABLE dbo.sysroPermissionsOverGroups
	(
	PassportID int NOT NULL,
	EmployeeGroupID int NOT NULL,
	EmployeeFeatureID int NOT NULL,
	LevelOfAuthority int NOT NULL,
	Permission int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.sysroPermissionsOverGroups ADD CONSTRAINT
	PK_sysroPermissionsOverGroups PRIMARY KEY CLUSTERED 
	(
	PassportID,
	EmployeeGroupID,
	EmployeeFeatureID
	)  ON [PRIMARY]
GO

--Creación de la nueva tabla de permisos calculados de excepciones sobre empleados
CREATE TABLE dbo.sysroPermissionsOverEmployeesExceptions
	(
	PassportID int NOT NULL,
	EmployeeID int NOT NULL,
	EmployeeFeatureID int NOT NULL,
	Permission int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.sysroPermissionsOverEmployeesExceptions ADD CONSTRAINT
	PK_sysroPermissionsOverEmployeesExceptions PRIMARY KEY CLUSTERED 
	(
	PassportID,
	EmployeeID,
	EmployeeFeatureID
	)  ON [PRIMARY]
GO

--Creamos una vista que contenga todas las movilidades de los empleados, pasadas, actuales y futuras
CREATE VIEW [dbo].[sysrovwAllEmployeeMobilities]
  AS
  SELECT IDEmployee, IDGroup, BeginDate, EndDate FROM dbo.Groups inner JOIN dbo.sysrovwPastEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwPastEmployeeGroups.IDGroup
	UNION
  SELECT IDEmployee, IDGroup, BeginDate, EndDate FROM dbo.Groups inner JOIN dbo.sysrovwCurrentEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwCurrentEmployeeGroups.IDGroup
	UNION
  SELECT IDEmployee, IDGroup, BeginDate, EndDate FROM dbo.Groups INNER JOIN dbo.sysrovwFutureEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwFutureEmployeeGroups.IDGroup
GO

--Creación de las nuevas funciones de obtención de permisos a partir de las sysropassports_permissionsover*
CREATE FUNCTION [dbo].[sysro_GetPassportPermissionOverGroup]
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
 	DECLARE @ParentGroup int
 	
 	IF @mode <> 2
 		SELECT @Result = Permission
 		FROM sysroPassports_PermissionsOverGroups
 		WHERE IDPassport = @idPassport AND
 			IDGroup = @idGroup AND
 			IDApplication = @idApplication
 	
 	IF @mode <> 1
 	BEGIN
 		SELECT @ParentGroup = dbo.GetEmployeeGroupParent(@idGroup)
 		
 		WHILE @Result IS NULL AND NOT @ParentGroup IS NULL
 		BEGIN
 			SELECT @Result = Permission
 			FROM sysroPassports_PermissionsOverGroups
 			WHERE IDPassport = @idPassport AND
 				IDGroup = @ParentGroup AND
 				IDApplication = @idApplication
 			
 			SELECT @ParentGroup = dbo.GetEmployeeGroupParent(@ParentGroup)
 		END
 	END
 	
 RETURN @Result
 END
GO

CREATE FUNCTION [dbo].[sysro_GetPermissionOverGroup] 
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
   	DECLARE @GroupType nvarchar(50)
  	
  	/* Return the most restricted passport permission by looking at parent
  	passport's permissions one by one */
  	
  	IF @mode <> 3
  		SELECT @Result = dbo.sysro_GetPassportPermissionOverGroup(@idPassport, @idGroup, @idApplication, @mode)
  	
  	IF @mode <> 1
  	BEGIN
  		
 		IF @mode > 0 
		BEGIN
			SELECT @ParentPassport = IDParentPassport
			FROM sysroPassports
			WHERE ID = @idPassport
		END
		ELSE
		BEGIN
 			SELECT @GroupType = isnull(GroupType, '')
   			FROM sysroPassports
   			WHERE ID = @idPassport
   		
   			if @GroupType = 'U'
   			begin
   				SET @ParentPassport = @idPassport
   			end
   			else
   			begin
   				SELECT @ParentPassport = IDParentPassport
   				FROM sysroPassports
   				WHERE ID = @idPassport
   			end
 		END	
 		 		
  		/* If looking for inherited only, the constraint is for first check only.
  		Other queries should be threated as normal */
  		WHILE NOT @ParentPassport IS NULL
  		BEGIN
  			SELECT @NewResult = dbo.sysro_GetPassportPermissionOverGroup(@ParentPassport, @idGroup, @idApplication, 0)
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
GO

CREATE FUNCTION [dbo].[sysro_GetPermissionOverEmployee] 
  	(
  		@idPassport int,
  		@idEmployee int,
  		@idApplication int,
  		@mode int,
  		@includeGroups bit,
  		@date datetime
  	)
  RETURNS int
  AS
  BEGIN
  	DECLARE @Result int
  	DECLARE @LoopPassport int
  	DECLARE @IDGroup int
  	DECLARE @GroupType nvarchar(50)
  	
  	/* First look at employees exceptions, on specified passport then on 
  	each of it's parents. If nothing is found, look at groups permissions. */
  	
  	/* Check permissions */
  	IF @mode <> 2
  		SET @LoopPassport = @idPassport
  	ELSE
  		IF @mode > 0
		BEGIN
			SELECT @LoopPassport = IDParentPassport
			FROM sysroPassports
			WHERE ID = @idPassport
		END
		ELSE
		BEGIN
  			SELECT @GroupType = isnull(GroupType, '')
  			FROM sysroPassports
  			WHERE ID = @idPassport
  		
  			if @GroupType = 'U'
  			begin
  				SET @LoopPassport = @idPassport
  			end
  			else
  			begin
  				SELECT @LoopPassport = @idPassport
  				FROM sysroPassports
  				WHERE ID = @idPassport
  			end
  		END
  	
  	
  	WHILE @Result IS NULL AND NOT @LoopPassport IS NULL
  	BEGIN
  		SELECT @Result = Permission
  		FROM sysroPassports_PermissionsOverEmployees
  		WHERE IDPassport = @LoopPassport AND
  			IDApplication = @idApplication AND
  			IDEmployee = @idEmployee
  		
  		SELECT @LoopPassport = IDParentPassport
  		FROM sysroPassports
  		WHERE ID = @LoopPassport
  	END
  	
  	IF @Result IS NULL AND @includeGroups = 1
  	BEGIN
  		/* If nothing is found directly on employee, 
  		look at groups permissions */
  		IF @Result IS NULL
  		BEGIN
  			SELECT @IDGroup = dbo.GetEmployeeGroup(@idEmployee, @date)
  			IF NOT @IDGroup IS NULL
  				SELECT @Result = dbo.sysro_GetPermissionOverGroup(@idPassport, @IDGroup, @idApplication, @mode)
  		END
  	END
  	
  	
  	/* Return result */
  	IF @Result IS NULL
  		SET @Result = 0
  		
  	RETURN @Result
  END
GO

CREATE FUNCTION [dbo].[sysro_GetPermissionOverFeature] 
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
 	
 	/* Get list of passports to check */
 	DECLARE @Passports table(ID int)
 	IF @mode <> 2
 		INSERT INTO @Passports VALUES (@idPassport)
 	IF @mode <> 1
 		INSERT INTO @Passports SELECT ID FROM dbo.GetPassportParents(@idPassport)
 	
 	/* Get feature ID */
 	DECLARE @IDFeature int
 	SELECT @IDFeature = ID
 	FROM sysroFeatures
 	WHERE Alias = @featureAlias AND
 		Type = @featureType
 	
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
GO

CREATE PROCEDURE [dbo].[sysro_PermissionsOverEmployees_Get] 
 	(
 		@idPassport int,
 		@idEmployee int,
 		@idApplication int,
 		@mode int,
 		@includeGroups bit
 	)
 AS
 	DECLARE @Date datetime
 	SET @Date = GetDate()
 	SELECT dbo.sysro_GetPermissionOverEmployee(@idPassport, @idEmployee, @idApplication, @mode, @includeGroups, @Date) AS Permission
 	
 	RETURN
GO

CREATE PROCEDURE [dbo].[sysro_PermissionsOverFeatures_Get]
 	(
 		@idPassport int,
 		@featureAlias nvarchar(100),
 		@featureType varchar(1),
 		@mode int
 	)
 AS
 	SELECT dbo.sysro_GetPermissionOverFeature(@idPassport, @featureAlias, @featureType, @mode) AS Permission
 	
 	RETURN
GO

CREATE PROCEDURE [dbo].[sysro_PermissionsOverGroups_Get] 
 	(
 		@idPassport int,
 		@idGroup int,
 		@idApplication int,
 		@mode int
 	)
 AS
 	SELECT dbo.sysro_GetPermissionOverGroup(@idPassport, @idGroup, @idApplication, @mode) AS Permission
 	
 	RETURN
GO


--Modificación de las actuales funciones de obtención de permisos a partir de las sysropassports_permissionsover* para que utilicen las nuevas tablas
ALTER FUNCTION [dbo].[WebLogin_GetPassportPermissionOverGroup]
 	(
 		@idPassport int,
 		@idGroup int,
 		@idApplication int,
		@mode int --Esto no se utiliza pero se mantiene para que la definición de la función sea la misma
 	)
 RETURNS int
 AS
 BEGIN
 	DECLARE @Result int
	DECLARE @parentPassport int
  	DECLARE @GroupType nvarchar(50)
 	
	SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @idPassport
  		
  	if @GroupType = 'U'
  	begin
  		SET @parentPassport = @idPassport
  	end
  	else
  	begin
  		SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @idPassport
  	end

	SELECT @Result = Permission FROM sysroPermissionsOverGroups WHERE PassportID = @parentPassport AND EmployeeGroupID = @idGroup AND EmployeeFeatureID = @idApplication
 	
	RETURN @Result
 END
GO

ALTER FUNCTION [dbo].[WebLogin_GetPermissionOverEmployee] 
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
  	DECLARE @GroupType nvarchar(50)
 	
	SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @idPassport
  		
  	if @GroupType = 'U'
  	begin
  		SET @parentPassport = @idPassport
  	end
  	else
  	begin
  		SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @idPassport
  	end

	SELECT @Result = isnull(sysroPermissionsOverEmployeesExceptions.Permission,sysroPermissionsOverGroups.Permission) FROM sysroPermissionsOverGroups
		inner join sysrovwAllEmployeeMobilities on sysroPermissionsOverGroups.EmployeeGroupID=sysrovwAllEmployeeMobilities.IDGroup and @date between BeginDate and EndDate and sysrovwAllEmployeeMobilities.IDEmployee = @idEmployee
		left outer join sysroPermissionsOverEmployeesExceptions on sysrovwAllEmployeeMobilities.IDEmployee=sysroPermissionsOverEmployeesExceptions.EmployeeID 
		and sysroPermissionsOverEmployeesExceptions.PassportID = @parentPassport  and sysroPermissionsOverEmployeesExceptions.EmployeeFeatureID = @idApplication
		where sysroPermissionsOverGroups.PassportID = @parentPassport  and sysroPermissionsOverGroups.EmployeeFeatureID = @idApplication
			
	RETURN @Result
  END
GO

ALTER FUNCTION [dbo].[WebLogin_GetPermissionOverFeature] 
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
  	DECLARE @GroupType nvarchar(50)
 	
	SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @idPassport
  		
  	if @GroupType = 'U'
  	begin
  		SET @parentPassport = @idPassport
  	end
  	else
  	begin
  		SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @idPassport
  	end



	IF @featureType = 'U'
	BEGIN
 		SELECT @IDFeature = ID FROM sysroFeatures WHERE Alias = @featureAlias AND Type = @featureType
 		SELECT @Result = Permission FROM sysroPermissionsOverFeatures WHERE PassportID = @parentPassport AND FeatureID = @IDFeature
 	END
	ELSE
	BEGIN
		SELECT @Result = dbo.sysro_GetPermissionOverFeature(@parentPassport,@featureAlias,@featureType,@mode)
	END

 	RETURN @Result
 END
GO

ALTER FUNCTION [dbo].[WebLogin_GetPermissionOverGroup] 
  	(
  		@idPassport int,
  		@idGroup int,
  		@idApplication int,
  		@mode int --Esto no se utiliza pero se mantiene para que la definición de la función sea la misma
  	)
  RETURNS int
  AS
  BEGIN
  	DECLARE @Result int
	DECLARE @parentPassport int
  	DECLARE @GroupType nvarchar(50)
 	
	SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @idPassport
  		
  	if @GroupType = 'U'
  	begin
  		SET @parentPassport = @idPassport
  	end
  	else
  	begin
  		SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @idPassport
  	end

	SELECT @Result = Permission FROM sysroPermissionsOverGroups WHERE PassportID = @parentPassport AND EmployeeGroupID = @idGroup AND EmployeeFeatureID = @idApplication
 	
	RETURN @Result
  END
GO

 ALTER PROCEDURE [dbo].[WebLogin_PermissionsOverGroups_MaxConfigurable] 
 	(
 		@idPassport int,
 		@idGroup int,
 		@idApplication int
 	)
 AS
 	/* Returns the highest permission that can be set over specified group.
 	A permission can never be set higher than the lowest parent value.
 	If the passport does not have a parent, there is no restriction. */
 	
 	/* Check whether passport exists */
 	IF NOT EXISTS (
 		SELECT ID FROM sysroPassports 
 		WHERE ID = @idPassport)
 		SELECT 0 AS Permission
 	/* If there is no parent passport, there is not restrictions. */
 	ELSE IF EXISTS (
   		SELECT ID FROM sysroPassports 
 		WHERE ID = @idPassport AND IDParentPassport IS NULL)
 		SELECT 6 AS Permission
 	/* Return inherited value */
 	ELSE
 		SELECT dbo.sysro_GetPermissionOverGroup(@idPassport, @idGroup, @idApplication, 3) AS Permission
 	
 	RETURN
GO

 ALTER PROCEDURE [dbo].[WebLogin_PermissionsOverFeatures_MaxConfigurable] 
 	(
 		@idPassport int,
 		@featureAlias nvarchar(100),
 		@featureType varchar(1)
 	)
 AS
 	/* Returns the highest permission that can be set for specified passport.
 	A permission can never be set higher than the lowest parent value.
 	If the passport does not have a parent, there is no restriction.
 	Note: This does not take PermissionTypes into account, that field
 		is only used in the visual interface.*/
 	
 	/* Check whether passport exists */
 	IF NOT EXISTS (
 		SELECT ID FROM sysroPassports 
 		WHERE ID = @idPassport)
 		SELECT 0 AS Permission
 	/* If there is no parent passport, there is no restriction. */
 	ELSE IF EXISTS (
 		SELECT ID FROM sysroPassports 
 		WHERE ID = @idPassport AND IDParentPassport IS NULL)
 		SELECT 9 AS Permission
 	/* Return inherited value */
 	ELSE
 		SELECT dbo.sysro_GetPermissionOverFeature(@idPassport, @featureAlias, @featureType, 2) AS Permission
 	
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
  	SELECT @Permission = dbo.sysro_GetPermissionOverEmployee(@idPassport, @idEmployee, @idApplication, 0, 1, @Date)
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
  	SELECT @Permission = dbo.sysro_GetPermissionOverFeature(@idPassport, @featureAlias, @featureType, 0)
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
  	SELECT @Permission = dbo.sysro_GetPermissionOverGroup(@idPassport, @idGroup, @idApplication, 0)
  	
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


-- Creación de funciones para rellenar las tablas de permisos por primera vez
CREATE PROCEDURE [dbo].[sysro_GenerateAllPermissionsOverGroups]
    AS
    BEGIN
  	
	DELETE FROM sysroPermissionsOverGroups

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
 				where supPassports.GroupType = 'U'
 		
 		FETCH NEXT FROM EmployeeFeatureCursor 
 		INTO @featureEmployeeID
 	END 
 	CLOSE EmployeeFeatureCursor
 	DEALLOCATE EmployeeFeatureCursor
	    	
    END
GO

CREATE PROCEDURE [dbo].[sysro_UpdatePassportPermissionsOverGroups]
	(
		@idPassport int
	)
    AS
    BEGIN
  	
	DECLARE @featureEmployeeID int
	DECLARE @updatePassportIDs table(IDPassport int)

	;WITH cte AS 
	(
	SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType
	FROM sysroPassports a
	WHERE Id = @idPassport and GroupType = 'U'
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
GO

CREATE PROCEDURE [dbo].[sysro_GenerateAllPermissionsOverFeatures]
    AS
    BEGIN
  	
	DELETE FROM sysroPermissionsOverFeatures

	DECLARE @featureID int
	DECLARE @featureAlias nvarchar(100)
 	DECLARE @featureType varchar(1)
 	
	DECLARE EmployeeFeatureCursor CURSOR
 	FOR SELECT distinct ID,Alias,Type from sysroFeatures WHERE Type = 'U'
 	OPEN EmployeeFeatureCursor
 	FETCH NEXT FROM EmployeeFeatureCursor INTO @featureID,@featureAlias,@featureType
 	WHILE @@FETCH_STATUS = 0
 	BEGIN	
 		
 		INSERT INTO [dbo].[sysroPermissionsOverFeatures] (PassportID, FeatureID, Permission) 
 				select supPassports.ID, @featureID, dbo.sysro_GetPermissionOverFeature(supPassports.ID,@featureAlias,@featureType,0)
 				from sysroPassports supPassports
 				where supPassports.GroupType = 'U'
 		
 		FETCH NEXT FROM EmployeeFeatureCursor 
 		INTO @featureID,@featureAlias,@featureType
 	END 
 	CLOSE EmployeeFeatureCursor
 	DEALLOCATE EmployeeFeatureCursor
	    	
    END

GO

CREATE PROCEDURE [dbo].[sysro_UpdatePassportPermissionsOverFeatures]
	(
		@idPassport int
	)
    AS
    BEGIN
  	
	DECLARE @featureID int
	DECLARE @featureAlias nvarchar(100)
 	DECLARE @featureType varchar(1)

	DECLARE @updatePassportIDs table(IDPassport int)

	;WITH cte AS 
	(
	SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType
	FROM sysroPassports a
	WHERE Id = @idPassport and GroupType = 'U'
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
GO

CREATE PROCEDURE [dbo].[sysro_GenerateAllPermissionsOverEmployeesExceptions]
    AS
    BEGIN
  	
	DELETE FROM sysroPermissionsOverEmployeesExceptions

	DECLARE @parentPassportID int
	DECLARE @employeeID int
	DECLARE @applicationID int
	DECLARE @permission int
	
	DECLARE @insertPassportIDs table(IDPassport int)
	DECLARE @passportID int

	DECLARE @oldPermission int

 	DECLARE ExceptionsCursor CURSOR FOR 
 		SELECT IDPassport,IDEmployee,IDApplication,Permission FROM sysroPassports_PermissionsOverEmployees
 	OPEN ExceptionsCursor
 	FETCH NEXT FROM ExceptionsCursor INTO @parentPassportID,@employeeID,@applicationID,@permission
 	WHILE @@FETCH_STATUS = 0
 	BEGIN	
 		

		;WITH cte AS 
		(
		SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType
		FROM sysroPassports a
		WHERE Id = @parentPassportID and GroupType = 'U'
		UNION ALL
		SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType
		FROM sysropassports a JOIN cte c ON a.IDParentPassport = c.id
		where a.GroupType = 'U'
		)
		INSERT INTO @insertPassportIDs SELECT Id FROM cte


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

GO

CREATE PROCEDURE [dbo].[sysro_UpdatePassportPermissionsOverEmployeesExceptions]
	(
		@idPassport int
	)
    AS
    BEGIN
  	
	DECLARE @updatePassportIDs table(IDPassport int)

	;WITH cteUpdate AS 
	(
	SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType
	FROM sysroPassports a
	WHERE Id = @idPassport and GroupType = 'U'
	UNION ALL
	SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType
	FROM sysropassports a JOIN cteUpdate c ON a.IDParentPassport = c.id
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
GO

--Funciones para regenerar la tabla de permisos sobre solicitudes

CREATE TABLE [dbo].[sysroPermissionsOverRequests](
	[IDParentPassport] [int] NOT NULL,
	[IDRequest] [int] NOT NULL,
	[EmployeeGroupID] [int] NOT NULL
 CONSTRAINT [PK_sysroPermissionsOverRequests] PRIMARY KEY CLUSTERED 
(
	[IDParentPassport] ASC,
	[IDRequest] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER PROCEDURE [dbo].[GenerateAllRequestPassportPermission]
    AS
    BEGIN
  	
	/* Rellenamos una tabla  con todos los passaportes de tipo grupo y cada una de las peticiones sobre las que tiene permisos */
    DECLARE @idRequest int

	DELETE FROM sysroPermissionsOverRequests

  	DECLARE db_cursor CURSOR FOR  
  	SELECT ID from Requests
   	
   	OPEN db_cursor  
   	FETCH NEXT FROM db_cursor INTO @idRequest  
   	WHILE @@FETCH_STATUS = 0  
   	BEGIN  

   		exec dbo.InsertRequestPassportPermission @idRequest
   				   
   		FETCH NEXT FROM db_cursor INTO @idRequest  
   	END  
   	CLOSE db_cursor  
   	DEALLOCATE db_cursor 
    	
    END
GO

CREATE PROCEDURE [dbo].[GenerateChangeDayRequestPassportPermission]
    AS
    BEGIN
  	
	/* Rellenamos una tabla  con todos los passaportes de tipo grupo y cada una de las peticiones sobre las que tiene permisos */
    DECLARE @idRequest int
	DECLARE @renewRequests table(IDRequest int)

	INSERT INTO @renewRequests(IDRequest)
		select Requests.Id
				from Requests
					left outer join sysroPermissionsOverRequests on sysroPermissionsOverRequests.IDRequest = Requests.ID
					left outer join sysrovwAllEmployeeGroups on Requests.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee 
					where (sysroPermissionsOverRequests.IDRequest is null) or (sysroPermissionsOverRequests.EmployeeGroupID <> sysrovwAllEmployeeGroups.IDGroup)



	DELETE FROM sysroPermissionsOverRequests WHERE IDRequest in (SELECT IDRequest from @renewRequests)

  	DECLARE db_cursor CURSOR FOR  
  	SELECT IDRequest from @renewRequests
   	
   	OPEN db_cursor  
   	FETCH NEXT FROM db_cursor INTO @idRequest  
   	WHILE @@FETCH_STATUS = 0  
   	BEGIN  

   		exec dbo.InsertRequestPassportPermission @idRequest
   				   
   		FETCH NEXT FROM db_cursor INTO @idRequest  
   	END  
   	CLOSE db_cursor  
   	DEALLOCATE db_cursor 
    	
    END
GO

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
  	DELETE FROM sysroPermissionsOverRequests WHERE IDRequest = @IDRequest
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
  				INSERT INTO sysroPermissionsOverRequests VALUES (@idPassport, @idRequest, dbo.GetEmployeeGroup(@employeeID,getdate()))
 			end
  			
  		end
   	    		   
   		FETCH NEXT FROM db_cursorInsertRequestPassportPermission INTO @idPassport  
   	END  
   	CLOSE db_cursorInsertRequestPassportPermission  
   	DEALLOCATE db_cursorInsertRequestPassportPermission 
    	
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
   	DECLARE db_cursorInsertRequestPassportPermission CURSOR FOR  
  	SELECT Requests.ID, Requests.IDEmployee, sysroFeatures.Alias, sysroFeatures.EmployeeFeatureID
  	FROM Requests inner join sysroFeatures on sysroFeatures.AliasID = Requests.RequestType
  	/* Borramos los datos actuales del passport */
  	DELETE FROM sysroPermissionsOverRequests WHERE IDParentPassport = @IDParentPassport
  	OPEN db_cursorInsertRequestPassportPermission  
   		FETCH NEXT FROM db_cursorInsertRequestPassportPermission INTO @idRequest, @employeeID, @alias , @featureID
   		WHILE @@FETCH_STATUS = 0  
   		BEGIN
  			/* Insertamos en el caso que sea necesario sobre la tabla de permisos */
  			if dbo.WebLogin_GetPermissionOverFeature(@IDParentPassport,@alias, 'U', 0) > 3 
  				AND dbo.WebLogin_GetPermissionOverEmployee(@IDParentPassport,@employeeID,@featureID,2,1,getdate()) > 3 
  			begin
 				SET @bInsert = (select count(*) from sysroPassports where isnull(GroupType,'') <> 'U' and IDParentPassport = @IDParentPassport)
 				if @bInsert > 0
 				begin
  					INSERT INTO sysroPermissionsOverRequests VALUES (@IDParentPassport, @idRequest,dbo.GetEmployeeGroup(@employeeID,getdate()))
 				end
  			end
   	        	
  	 		FETCH NEXT FROM db_cursorInsertRequestPassportPermission INTO  @idRequest, @employeeID, @alias , @featureID
   		END
   	CLOSE db_cursorInsertRequestPassportPermission
   	DEALLOCATE db_cursorInsertRequestPassportPermission
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
 		(SELECT dbo.GetPassportLevelOfAuthority(IDParentPassport) AS LevelOfAuthority, IDRequest FROM sysroPermissionsOverRequests WHERE IDRequest = @idRequest)trpp
 		WHERE trpp.LevelOfAuthority > @LevelOfAuthority AND trpp.LevelOfAuthority <= @RequestLevel
 	)
     IF @RequestLevel = 11  SET @LevelsBelow = @LevelsBelow + 1
    	
    	RETURN @LevelsBelow
  END
GO

CREATE NONCLUSTERED INDEX PX_sysroPermissionsOverRequests
ON [dbo].[sysroPermissionsOverRequests] ([IDRequest])
GO

CREATE NONCLUSTERED INDEX PX_SYSROPASSPORTS_GROUPTYPE
ON [dbo].[sysroPassports] ([GroupType])
INCLUDE ([ID])
GO

-- Funcion que nos devuelve un array de empleados a los que supervisa un passport
ALTER FUNCTION [dbo].[GetSupervisedEmployeesByPassport]
 (
 	@idPassport int,
 	@featureAlias nvarchar(50)
 )
 RETURNS @result table (ID int PRIMARY KEY)
 AS
 	BEGIN
 		DECLARE @EmployeeID int
 		DECLARE @SupervisorLevel int
 		DECLARE @featureEmployeeID int
 		DECLARE @featurePermission int
 		DECLARE @parentPassport int
  		
		DECLARE @GroupType nvarchar(50)
 	
		SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @idPassport
  		
  		if @GroupType = 'U'
  		begin
  			SET @parentPassport = @idPassport
  		end
  		else
  		begin
  			SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @idPassport
  		end

 		
 		SELECT @SupervisorLevel = (SELECT dbo.GetPassportLevelOfAuthority(@idPassport))
 		SELECT @featureEmployeeID = (SELECT EmployeeFeatureID from sysroFeatures where Alias = @featureAlias)
 		SELECT @featurePermission = (dbo.WebLogin_GetPermissionOverFeature(@idPassport, @featureAlias,'U',2))
 		
 		IF @featurePermission > 3
 			BEGIN
 				INSERT INTO @result
					select IDEmployee from (
						select IDEmployee from sysrovwCurrentEmployeeGroups where IDGroup in (
							SELECT EmployeeGroupID from (select EmployeeGroupID, MAX(LevelOfAuthority) as MaxLevel, PassportID from [dbo].[sysroPermissionsOverGroups]
 												where Permission > 3 and  EmployeeFeatureID  = @featureEmployeeID  group by EmployeeGroupID,PassportID) allPerm
							WHERE allPerm.PassportID = @parentPassport and allPerm.MaxLevel = @SupervisorLevel)) allEmployees
						WHERE IDEmployee not in (select IDEmployee from sysroPermissionsOverEmployeesExceptions where PassportID = @parentPassport and EmployeeFeatureID = @featureEmployeeID and Permission <= 3)

 			END
 		RETURN
 	END
GO

-- Funcion encargada de lanzar todas las opciones de recalculo de permisos
ALTER PROCEDURE [dbo].[ExecuteRequestPassportPermissionsAction]
    (
  	@IDAction int,
    	@IDObject int
    )
    AS
    /* Añadimos todos los pasaportes que tienen permisos para una solicitud a la tabla temporal */
 	BEGIN
 		IF @IDAction = -2
 		BEGIN
			exec dbo.sysro_GenerateAllPermissionsOverGroups
			exec dbo.sysro_GenerateAllPermissionsOverFeatures
 			exec dbo.sysro_GenerateAllPermissionsOverEmployeesExceptions
			exec dbo.GenerateAllRequestPassportPermission
 		END
		IF @IDAction = -1 -- Cambio de dia
 		BEGIN
			exec dbo.GenerateChangeDayRequestPassportPermission
 		END
 		IF @IDAction = 0 -- Creación passport
 		BEGIN
 			exec dbo.InsertPassportRequestsPermission @IDObject
			exec dbo.sysro_UpdatePassportPermissionsOverGroups @IDObject
			exec dbo.sysro_UpdatePassportPermissionsOverFeatures @IDObject
			exec dbo.sysro_UpdatePassportPermissionsOverEmployeesExceptions @IDObject
 		END
 		IF @IDAction = 1 -- Modificación passport
 		BEGIN
 			exec dbo.AlterPassportRequestsPermission @IDObject
			exec dbo.sysro_UpdatePassportPermissionsOverGroups @IDObject
			exec dbo.sysro_UpdatePassportPermissionsOverFeatures @IDObject
			exec dbo.sysro_UpdatePassportPermissionsOverEmployeesExceptions @IDObject
 		END
 		IF @IDAction = 2 -- Creación solicitud
 		BEGIN
 			exec dbo.InsertRequestPassportPermission @IDObject
 		END
 	END

GO

EXEC dbo.ExecuteRequestPassportPermissionsAction -2,1
GO

--- Fin permisos V2 en tablas
 

--- Nuevos indices para mejorar rendimiento y posibles timeouts de BBDD
CREATE NONCLUSTERED INDEX PX_DS_Shift2 
ON [dbo].[DailySchedule] ([IDShift2],[Date])
INCLUDE ([Status])
GO

CREATE NONCLUSTERED INDEX PX_DS_Shift3
ON [dbo].[DailySchedule] ([IDShift3],[Date])
INCLUDE ([Status])
GO

CREATE NONCLUSTERED INDEX PX_DS_ShiftBase
ON [dbo].[DailySchedule] ([IDShiftBase],[Date])
INCLUDE ([Status])
GO

CREATE NONCLUSTERED INDEX PX_DS_Covered
ON [dbo].[DailySchedule] ([Date],[IDEmployeeCovered])
GO


 
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='331' WHERE ID='DBVersion'
GO


