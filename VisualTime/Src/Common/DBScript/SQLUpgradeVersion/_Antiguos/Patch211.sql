ALTER TABLE dbo.sysroPassports ADD ConfData text NULL
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UsersAdmin_Employees_List]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[UsersAdmin_Employees_List]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UsersAdmin_Features_List]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[UsersAdmin_Features_List]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UsersAdmin_Groups_List]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[UsersAdmin_Groups_List]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Authenticate]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Authenticate]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_CredentialExists]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_CredentialExists]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Employees_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Employees_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Employees_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Employees_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Features_SelectApplications]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Features_SelectApplications]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Features_SelectByAlias]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Features_SelectByAlias]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Features_SelectById]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Features_SelectById]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Features_SelectByType]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Features_SelectByType]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_AuthenticationMethods_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_AuthenticationMethods_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_AuthenticationMethods_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_AuthenticationMethods_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_AuthenticationMethods_Select]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_AuthenticationMethods_Select]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_AuthenticationMethods_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_AuthenticationMethods_Update]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_Select]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_Select]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_SelectByIDEmployee]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_SelectByIDEmployee]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_SelectByIDUser]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_SelectByIDUser]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_SelectByParent]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_SelectByParent]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_SelectRoot]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_SelectRoot]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_Update]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_PermissionsOverEmployees_Get]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_PermissionsOverEmployees_Get]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_PermissionsOverEmployees_Remove]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_PermissionsOverEmployees_Remove]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_PermissionsOverEmployees_Set]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_PermissionsOverEmployees_Set]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_PermissionsOverFeatures_Get]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_PermissionsOverFeatures_Get]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_PermissionsOverFeatures_GetMax]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_PermissionsOverFeatures_GetMax]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_PermissionsOverFeatures_MaxConfigurable]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_PermissionsOverFeatures_MaxConfigurable]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_PermissionsOverFeatures_Remove]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_PermissionsOverFeatures_Remove]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_PermissionsOverFeatures_Set]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_PermissionsOverFeatures_Set]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_PermissionsOverGroups_Get]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_PermissionsOverGroups_Get]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_PermissionsOverGroups_MaxConfigurable]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_PermissionsOverGroups_MaxConfigurable]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_PermissionsOverGroups_Remove]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_PermissionsOverGroups_Remove]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_PermissionsOverGroups_Set]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_PermissionsOverGroups_Set]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Users_Delete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Users_Delete]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Users_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Users_Insert]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebPortal_Gui_SelectByApplication]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebPortal_Gui_SelectByApplication]
GO

CREATE PROCEDURE dbo.UsersAdmin_Employees_List 
	(
		@idPassport int,
		@idApplication int
	)
AS
	SELECT DISTINCT e.ID AS IDEmployee,
		e.Name
	FROM sysroPassports_PermissionsOverEmployees p
	LEFT JOIN Employees e ON p.IDEmployee = e.ID
	WHERE (p.IDPassport = @idPassport OR 
		p.IDPassport IN (SELECT ID FROM dbo.GetPassportParents(@idPassport))) AND
		p.IDApplication = @idApplication
	ORDER BY e.Name
GO

CREATE PROCEDURE dbo.UsersAdmin_Features_List
	(
		@featureType varchar(1)
	)
AS	
	SELECT ID,
		IDParent,
		Alias,
		Name,
		Type,
		CASE WHEN EXISTS (
			SELECT ID FROM sysroFeatures sub
			WHERE sub.IDParent = f.ID)
			THEN 1 ELSE 0 END AS IsGroup,
		PermissionTypes
	FROM sysroFeatures f
	WHERE Type = '' OR 
		Type = @featureType
	ORDER BY Name
	
	RETURN
GO

CREATE PROCEDURE dbo.UsersAdmin_Groups_List 
AS
	DECLARE @Date datetime
	SET @Date = GetDate()	

	SELECT ID AS IDGroup,
		dbo.GetEmployeeGroupParent(ID) AS IDParentGroup,
		Name
	FROM Groups
	ORDER BY Name
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Authenticate 
	(
		@method int,
		@credential nvarchar(255),
		@password nvarchar(4000)
	)
AS
	SELECT TOP 1 p.ID
	FROM sysroPassports_AuthenticationMethods a
	LEFT JOIN sysroPassports p ON a.IDPassport = p.ID
	WHERE a.Method = @method AND
		a.Credential = @credential AND
		a.Password = @password COLLATE SQL_Latin1_General_CP1_CS_AS AND
		(StartDate IS NULL OR StartDate <= GetDate()) AND
		(ExpirationDate IS NULL OR ExpirationDate > GetDate())
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_CredentialExists 
	(
		@credential nvarchar(255),
		@method int,
		@idPassport int = NULL
	)
AS
	IF EXISTS (
		SELECT ID
		FROM sysroPassports p
		LEFT JOIN sysroPassports_AuthenticationMethods a ON p.ID = a.IDPassport
		WHERE a.Method = @method AND
			a.Credential = @credential AND
			(@idPassport IS NULL OR IDPassport = @idPassport)
	)
		SELECT 1
	ELSE
		SELECT 0
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Employees_Delete 
	(
		@id int
	)
AS
	SET NOCOUNT ON
	
	DELETE FROM Employees
	WHERE ID = @id
	
	SET NOCOUNT OFF
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Employees_Insert
	(
		@id int OUTPUT,
		@name nvarchar(50),
		@type char(1)
	)
AS
	INSERT INTO Employees (
		Name,
		Type
	)
	VALUES (
		@name,
		@type
	)
	
	SET @id = @@IDENTITY
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Features_SelectApplications
AS
	SELECT ID,
		IDParent,
		Alias,
		Name,
		Description,
		Type,
		PermissionTypes,
		AppHasPermissionsOverEmployees
	FROM sysroFeatures
	WHERE IDParent IS NULL
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Features_SelectByAlias
	(
		@featureAlias varchar(50),
		@featureType as varchar(1)
	)
AS
	SELECT ID,
		IDParent,
		Alias,
		Name,
		Description,
		Type,
		PermissionTypes,
		AppHasPermissionsOverEmployees
	FROM sysroFeatures
	WHERE Alias = @featureAlias AND
		Type = @featureType
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Features_SelectById
	(
		@idFeature int
	)
AS
	SELECT ID,
		IDParent,
		Alias,
		Name,
		Description,
		Type,
		PermissionTypes,
		AppHasPermissionsOverEmployees
	FROM sysroFeatures
	WHERE ID = @idFeature
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Features_SelectByType 
	(
		@featureType varchar(1)
	)
AS
	SELECT ID,
		IDParent,
		Alias,
		Name,
		Description,
		Type,
		PermissionTypes,
		AppHasPermissionsOverEmployees
	FROM sysroFeatures
	WHERE Type = @featureType
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Passports_AuthenticationMethods_Delete 
	(
		@idPassport int,
		@method int
	)
AS
	SET NOCOUNT ON
	
	DELETE FROM sysroPassports_AuthenticationMethods
	WHERE IDPassport = @idPassport AND
		Method = @method
	
	SET NOCOUNT OFF
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Passports_AuthenticationMethods_Insert 
	(
		@idPassport int,
		@method int,
		@credential nvarchar(255),
		@password nvarchar(1000),
		@startDate datetime,
		@expirationDate datetime
	)
AS
	INSERT INTO sysroPassports_AuthenticationMethods (
		IDPassport,
		Method,
		Credential,
		Password,
		StartDate,
		ExpirationDate
	)
	VALUES (
		@idPassport,
		@method,
		@credential,
		@password,
		@startDate,
		@expirationDate
	)
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Passports_AuthenticationMethods_Select 
	(
		@idPassport int
	)
AS
	SELECT IDPassport,
		Method,
		Credential,
		Password,
		StartDate,
		ExpirationDate
	FROM sysroPassports_AuthenticationMethods
	WHERE IDPassport = @idPassport
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Passports_AuthenticationMethods_Update
	(
		@idPassport int,
		@method int,
		@credential nvarchar(255),
		@password nvarchar(1000),
		@startDate datetime,
		@expirationDate datetime
	)
AS
	UPDATE sysroPassports_AuthenticationMethods SET
		Credential = @credential,
		Password = @password,
		StartDate = @startDate,
		ExpirationDate = @expirationDate
	WHERE IDPassport = @idPassport AND
		Method = @method
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Passports_Delete 
	(
		@id int
	)
AS
	SET NOCOUNT ON
	
	DELETE FROM sysroPassports
	WHERE ID = @id
	
	SET NOCOUNT OFF
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Passports_Insert 
	(
		@id int OUTPUT,
		@idParentPassport int,
		@groupType varchar(1),
		@name nvarchar(50),
		@description nvarchar(100),
		@email nvarchar(100),
		@idUser int,
		@idEmployee int,
		@idLanguage int,
		@levelOfAuthority tinyint, 
		@ConfData text
	)
AS
	INSERT INTO sysroPassports (
		IDParentPassport,
		GroupType,
		Name,
		Description,
		Email,
		IDUser,
		IDEmployee,
		IDLanguage,
		LevelOfAuthority, 
		ConfData
	)
	VALUES (
		@idParentPassport,
		@groupType,
		@name,
		@description,
		@email,
		@idUser,
		@idEmployee,
		@idLanguage,
		@levelOfAuthority,
		@ConfData
	)
	
	SELECT @id = @@IDENTITY
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Passports_Select 
	(
		@idPassport int
	)
AS
	SELECT ID,
		IDParentPassport,
		GroupType,
		Name,
		Description,
		Email,
		IDUser,
		IDEmployee,
		IDLanguage,
		LevelOfAuthority,
		ConfData
	FROM sysroPassports
	WHERE ID = @idPassport
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Passports_SelectByIDEmployee 
	(
		@idEmployee int
	)
AS
	SELECT ID,
		IDParentPassport,
		GroupType,
		Name,
		Description,
		Email,
		IDUser,
		IDEmployee,
		IDLanguage,
		LevelOfAuthority
	FROM sysroPassports
	WHERE IDEmployee = @idEmployee
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Passports_SelectByIDUser
	(
		@idUser int
	)
AS
	SELECT ID,
		IDParentPassport,
		GroupType,
		Name,
		Description,
		Email,
		IDUser,
		IDEmployee,
		IDLanguage,
		LevelOfAuthority
	FROM sysroPassports
	WHERE IDUser = @idUser
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Passports_SelectByParent
	(
		@idParentPassport int = NULL,
		@groupType varchar(1) = NULL
	)
AS
	SELECT ID
	FROM sysroPassports
	WHERE ((IDParentPassport IS NULL AND @idParentPassport IS NULL) OR
		IDParentPassport = @idParentPassport) AND
		(@groupType IS NULL OR GroupType = @groupType)
	ORDER BY GroupType, Name
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Passports_SelectRoot
AS
	SELECT ID
	FROM sysroPassports
	WHERE IDParentPassport IS NULL
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Passports_Update 
	(
		@id int,
		@idParentPassport int,
		@groupType varchar(1),
		@name nvarchar(50),
		@description nvarchar(100),
		@email nvarchar(100),
		@idUser int,
		@idEmployee int,
		@idLanguage int,
		@levelOfAuthority tinyint,
		@ConfData text
	)
AS
	UPDATE sysroPassports SET
		IDParentPassport = @idParentPassport,
		GroupType = @groupType,
		Name = @name,
		Description = @description,
		Email = @email,
		IDUser = @idUser,
		IDEmployee = @idEmployee,
		IDLanguage = @idLanguage,
		LevelOfAuthority = @levelOfAuthority,
		ConfData = @ConfData
	WHERE ID = @id
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_PermissionsOverEmployees_Get 
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
	SELECT dbo.WebLogin_GetPermissionOverEmployee(@idPassport, @idEmployee, @idApplication, @mode, @includeGroups, @Date) AS Permission
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_PermissionsOverEmployees_Remove 
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
	
	/* Ensure child passports permissions remain valid */
	DELETE FROM sysroPassports_PermissionsOverEmployees
	WHERE IDApplication = @idApplication AND
		IDPassport IN (SELECT ID FROM dbo.GetPassportChilds(@idPassport)) AND
		IDEmployee = @idEmployee AND
		Permission >= @Permission
		
	SET NOCOUNT OFF
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_PermissionsOverEmployees_Set 
	(
		@idPassport int,
		@idEmployee int,
		@idApplication int,
		@permission tinyint
	)
AS
	SET NOCOUNT ON
	
	IF EXISTS (
		SELECT IDPassport
		FROM sysroPassports_PermissionsOverEmployees
		WHERE IDPassport = @idPassport AND
			IDEmployee = @idEmployee AND
			IDApplication = @idApplication)
		
		UPDATE sysroPassports_PermissionsOverEmployees SET
			Permission = @permission
		WHERE IDPassport = @idPassport AND
			IDEmployee = @idEmployee AND
			IDApplication = @idApplication
	ELSE
		INSERT INTO sysroPassports_PermissionsOverEmployees (
			IDPassport,
			IDEmployee,
			IDApplication,
			Permission
		)
		VALUES (
			@idPassport,
			@idEmployee,
			@idApplication,
			@permission
		)
	
	
	/* Remove all exceptions defined on child passports for employee 
	This operation is optional but improves usability. */
	DELETE FROM sysroPassports_PermissionsOverEmployees
	WHERE IDApplication = @idApplication AND
		IDPassport IN (SELECT ID FROM dbo.GetPassportChilds(@idPassport)) AND
		IDEmployee = @idEmployee
		
	SET NOCOUNT OFF
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_PermissionsOverFeatures_Get
	(
		@idPassport int,
		@featureAlias nvarchar(100),
		@featureType varchar(1),
		@mode int
	)
AS
	SELECT dbo.WebLogin_GetPermissionOverFeature(@idPassport, @featureAlias, @featureType, @mode) AS Permission
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_PermissionsOverFeatures_GetMax
	(
		@idPassport int,
		@featureAlias nvarchar(100),
		@featureType varchar(1),
		@mode int
	)
AS
	DECLARE @Result int

	/* Get list of passports to check */
	DECLARE @Passports table(ID int)
	IF @mode <> 2
		INSERT INTO @Passports VALUES (@idPassport)
	IF @mode <> 1
		INSERT INTO @Passports SELECT ID FROM dbo.GetPassportParents(@idPassport)
	
	/* Get list of features */
	DECLARE @Features table(ID int)
	INSERT INTO @Features
		SELECT ID
		FROM sysroFeatures
		WHERE Alias LIKE @featureAlias + '.%' AND
			Type = @featureType
	
	/* Check permissions */
	SELECT TOP 1 @Result = Permission
	FROM sysroPassports_PermissionsOverFeatures
	WHERE IDPassport IN (SELECT * FROM @Passports) AND
		IDFeature IN (SELECT * FROM @Features)
	ORDER BY Permission
		
	/* Return result */
	IF NOT @Result IS NULL
		SELECT @Result AS Permission
	ELSE
		SELECT 0 AS Permission

	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_PermissionsOverFeatures_MaxConfigurable 
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
		SELECT dbo.WebLogin_GetPermissionOverFeature(@idPassport, @featureAlias, @featureType, 2) AS Permission
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_PermissionsOverFeatures_Remove 
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
	
	/* Ensure permissions validity */
	DELETE FROM sysroPassports_PermissionsOverFeatures
	WHERE IDPassport IN (SELECT ID FROM dbo.GetPassportChilds(@idPassport)) AND
		IDFeature = @IDFeature AND
		Permission >= @Permission
	
	SET NOCOUNT OFF
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_PermissionsOverFeatures_Set 
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
	SELECT @IDFeature = ID
	FROM sysroFeatures
	WHERE Alias = @featureAlias AND
		Type = @featureType

	/* Set permission (insert or update) */
	IF EXISTS
		(SELECT IDPassport
		FROM sysroPassports_PermissionsOverFeatures
		WHERE IDPassport = @idPassport AND
			IDFeature = @IDFeature)
		
		UPDATE sysroPassports_PermissionsOverFeatures SET
			Permission = @permission
		WHERE IDPassport = @idPassport AND
			IDFeature = @IDFeature
	
	ELSE
		INSERT INTO sysroPassports_PermissionsOverFeatures (
			IDPassport,
			IDFeature,
			Permission
		)
		VALUES (
			@idPassport,
			@IDFeature,
			@permission
		)
	
	/* Ensure child passports permissions remain valid */
	DELETE FROM sysroPassports_PermissionsOverFeatures
	WHERE IDPassport IN (SELECT ID FROM dbo.GetPassportChilds(@idPassport)) AND
		IDFeature = @IDFeature AND
		Permission >= @permission
		
	SET NOCOUNT OFF
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_PermissionsOverGroups_Get 
	(
		@idPassport int,
		@idGroup int,
		@idApplication int,
		@mode int
	)
AS
	SELECT dbo.WebLogin_GetPermissionOverGroup(@idPassport, @idGroup, @idApplication, @mode) AS Permission
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_PermissionsOverGroups_MaxConfigurable 
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
		SELECT dbo.WebLogin_GetPermissionOverGroup(@idPassport, @idGroup, @idApplication, 3) AS Permission
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_PermissionsOverGroups_Remove 
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
		
	SET NOCOUNT OFF
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_PermissionsOverGroups_Set 
	(
		@idPassport int,
		@idGroup int,
		@idApplication int,
		@permission tinyint
	)
AS
	/* Set permission (insert or update) */
	SET NOCOUNT ON
	IF EXISTS
		(SELECT IDPassport
		FROM sysroPassports_PermissionsOverGroups
		WHERE IDPassport = @idPassport AND
			IDGroup = @idGroup AND
			IDApplication = @idApplication)
		
		UPDATE sysroPassports_PermissionsOverGroups SET
			Permission = @permission
		WHERE IDPassport = @idPassport AND
			IDGroup = @idGroup AND
			IDApplication = @idApplication
	ELSE
		INSERT INTO sysroPassports_PermissionsOverGroups (
			IDPassport,
			IDGroup,
			IDApplication,
			Permission
		)
		VALUES (
			@idPassport,
			@idGroup,
			@idApplication,
			@permission
		)
	
	
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
				
	SET NOCOUNT OFF

	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Users_Delete 
	(
		@idUser int
	)
AS
	DELETE FROM sysroUsers
	WHERE IDUser = @idUser
	
	RETURN
GO

CREATE PROCEDURE dbo.WebLogin_Users_Insert 
	(
		@login nvarchar(25),
		@password nvarchar(50),
		@idSecurityGroup char(2),
		@idUser int OUTPUT
	)
AS
	INSERT INTO sysroUsers (
		Login,
		Password,
		IDSecurityGroup,
		UserConfData
	)
	VALUES (
		@login,
		@password,
		@idSecurityGroup,
		null
	)
	
	SET @idUser = @@IDENTITY
	
	RETURN
GO

CREATE PROCEDURE dbo.WebPortal_Gui_SelectByApplication
	(
		@applicationAlias nvarchar(200)
	)
AS
	SELECT IDPath,
		LanguageReference,
		URL,
		IconURL,
		Parameters,
		Priority,
		RequiredFeatures,
		RequiredFunctionalities
	FROM sysroGUI
	WHERE IDPath = @applicationAlias OR 
		IDPath LIKE @applicationAlias + '\%'
	ORDER BY Priority

	RETURN
GO

DELETE FROM sysroGUI WHERE IDPath LIKE 'Portal%'
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,RequiredFunctionalities) VALUES ('Portal',null,null,null,1,null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Configuration','Configuration',null,'Administracion.ico',1004,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Attendance','Attendance',null,'Presencia.ico',1001,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Configuration\Audit','Audit','Audit/Audit.aspx','Audit.png',130,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Attendance\Causes','Causes','Causes/Causes.aspx','Causes.png',660,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Attendance\Accruals','Accruals','Concepts/Concepts.aspx','Concepts.png',650,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Attendance\Employees','InfoEmployees','Employees/Employees.aspx','Employees.png',600,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Configuration\Options','ConfigurationOptions','Options/ConfigurationOptions.aspx','Options.png',110,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Configuration\AttendanceOptions','AttendanceOptions','Options/AttendanceOptions.aspx','AttendanceOptions.png',120,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Attendance\Scheduler','Scheduler','Scheduler/Scheduler.aspx','Scheduler.png',610,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Attendance\Shifts','Shifts','Shifts/Shifts.aspx','Shifts.png',640,'NWR',null)
GO

UPDATE [sysroPassports_AuthenticationMethods] SET [Method] = 1 WHERE [Credential] = 'User' AND [Method] = 2
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='211' WHERE ID='DBVersion'
GO
