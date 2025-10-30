ALTER TABLE dbo.sysroPassports_AuthenticationMethods
	DROP CONSTRAINT FK_sysroPassports_AuthenticationMethods_sysroPassports
GO
CREATE TABLE dbo.Tmp_sysroPassports_AuthenticationMethods
	(
	IDPassport int NOT NULL,
	Method int NOT NULL,
	Version nvarchar(50) NOT NULL,
	Credential nvarchar(255) NOT NULL,
	Password nvarchar(1000) NOT NULL,
	StartDate datetime NULL,
	ExpirationDate datetime NULL,
	BiometricID int NOT NULL,
	BiometricData image NULL,
	TimeStamp smalldatetime NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_sysroPassports_AuthenticationMethods ADD CONSTRAINT
	DF_sysroPassports_AuthenticationMethods_Version DEFAULT '' FOR Version
GO
ALTER TABLE dbo.Tmp_sysroPassports_AuthenticationMethods ADD CONSTRAINT
	DF_sysroPassports_AuthenticationMethods_BiometricID DEFAULT 0 FOR BiometricID
GO
IF EXISTS(SELECT * FROM dbo.sysroPassports_AuthenticationMethods)
	 EXEC('INSERT INTO dbo.Tmp_sysroPassports_AuthenticationMethods (IDPassport, Method, Credential, Password, StartDate, ExpirationDate, BiometricID)
		SELECT IDPassport, Method, Credential, Password, StartDate, ExpirationDate, 0 FROM dbo.sysroPassports_AuthenticationMethods WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.sysroPassports_AuthenticationMethods
GO
EXECUTE sp_rename N'dbo.Tmp_sysroPassports_AuthenticationMethods', N'sysroPassports_AuthenticationMethods', 'OBJECT' 
GO
ALTER TABLE dbo.sysroPassports_AuthenticationMethods ADD CONSTRAINT
	PK_sysroPassports_AuthenticationMethods PRIMARY KEY CLUSTERED 
	(
	IDPassport,
	Method,
	Version,
	BiometricID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.sysroPassports_AuthenticationMethods ADD CONSTRAINT
	FK_sysroPassports_AuthenticationMethods_sysroPassports FOREIGN KEY
	(
	IDPassport
	) REFERENCES dbo.sysroPassports
	(
	ID
	) ON UPDATE  CASCADE 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.sysroPassports_AuthenticationMethods ADD
	Enabled bit NOT NULL CONSTRAINT DF_sysroPassports_AuthenticationMethods_Enabled DEFAULT 1
GO
ALTER TABLE dbo.sysroPassports ADD
	AuthenticationMerge nvarchar(50) NULL,
	StartDate smalldatetime NULL,
	ExpirationDate smalldatetime NULL,
	State smallint NULL
GO
ALTER TABLE dbo.sysroPassports ADD CONSTRAINT
	DF_sysroPassports_State DEFAULT 1 FOR State
GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Authenticate]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Authenticate]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_CredentialExists]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_CredentialExists]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_AuthenticationMethod_Select]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_AuthenticationMethod_Select]
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
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_Insert]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_Select]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_Select]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_SelectAll]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_SelectAll]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_SelectAllEmployee]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_SelectAllEmployee]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_SelectAllUser]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_SelectAllUser]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_SelectByIDEmployee]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_SelectByIDEmployee]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_SelectByIDUser]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_SelectByIDUser]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_Passports_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_Passports_Update]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_PasswordExists]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_PasswordExists]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_PassportEmployee_ActiveContract]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[WebLogin_PassportEmployee_ActiveContract]
GO

 CREATE PROCEDURE [dbo].[WebLogin_Authenticate] 
 	(
 		@method int,
		@version nvarchar(50),
		@biometricID int,
 		@credential nvarchar(255),
 		@password nvarchar(4000)
 	)
 AS
 	SELECT TOP 1 p.ID
 	FROM sysroPassports_AuthenticationMethods a
 	LEFT JOIN sysroPassports p ON a.IDPassport = p.ID
 	WHERE a.Method = @method AND
		  a.Version = @version AND
		  a.BiometricID = @biometricID AND 
 		  a.Credential = @credential AND
 		  a.Password = @password COLLATE SQL_Latin1_General_CP1_CS_AS AND
 		  (a.StartDate IS NULL OR a.StartDate <= GetDate()) AND
 		  (a.ExpirationDate IS NULL OR a.ExpirationDate > GetDate()) AND
		  a.Enabled = 1
 	
 	RETURN
GO

 CREATE PROCEDURE [dbo].[WebLogin_CredentialExists] 
 	(
 		@credential nvarchar(255),
		@method int,
		@version nvarchar(50),
 		@idPassport int = NULL		
 	)
 AS
 	IF EXISTS (
 		SELECT ID
 		FROM sysroPassports p
 			 LEFT JOIN sysroPassports_AuthenticationMethods a ON p.ID = a.IDPassport			 			 
 		WHERE a.Method = @method AND
			  a.version = @version AND
 			  a.Credential = @credential AND			  
 			  (@idPassport IS NULL OR IDPassport <> @idPassport) AND
			  a.Enabled = 1 AND
			  (p.IDEmployee IS NULL OR
			  ISNULL((SELECT COUNT(*) FROM EmployeeContracts 
					  WHERE EmployeeContracts.IDEmployee = p.IDEmployee AND
							EmployeeContracts.BeginDate <= GETDATE() AND EmployeeContracts.EndDate >= GETDATE()), 0) > 0) /*AND
			  ((@idPassport IS NOT NULL AND
			  ISNULL((SELECT COUNT(*) 
					  FROM EmployeeContracts INNER JOIN sysroPassports
							ON EmployeeContracts.IDEmployee = sysroPassports.IDEmployee
					  WHERE sysroPassports.ID = @idPassport AND
							EmployeeContracts.BeginDate <= GETDATE() AND EmployeeContracts.EndDate >= GETDATE()), 0) > 0)
			  )	*/

 	)
 		SELECT 1
 	ELSE
		SELECT 0
 	
 	RETURN
GO

 CREATE PROCEDURE [dbo].[WebLogin_Passports_AuthenticationMethod_Select] 
 	(
 		@method int,
		@version nvarchar(50)		
 	)
 AS
 	SELECT IDPassport,
 		Method,
		Version,
 		Credential,
 		Password,
 		StartDate,
 		ExpirationDate,
		BiometricID,
		BiometricData,
		TimeStamp,
		Enabled
 	FROM sysroPassports_AuthenticationMethods
 	WHERE Method = @method AND
		  Version = @version AND		  
 		  (StartDate IS NULL OR StartDate <= GetDate()) AND
 		  (ExpirationDate IS NULL OR ExpirationDate > GetDate())
	
 	
 	RETURN
GO

 CREATE PROCEDURE [dbo].[WebLogin_Passports_AuthenticationMethods_Delete] 
 	(
 		@idPassport int,
 		@method int,
		@version nvarchar(50),
		@biometricID int
 	)
 AS
 	SET NOCOUNT ON
 	
 	DELETE FROM sysroPassports_AuthenticationMethods
 	WHERE IDPassport = @idPassport AND
 		  Method = @method AND
		  Version = @version AND
		  BiometricID = @biometricID
 	
 	SET NOCOUNT OFF
 	
 	RETURN
GO

 CREATE PROCEDURE [dbo].[WebLogin_Passports_AuthenticationMethods_Insert] 
 	(
 		@idPassport int,
 		@method int,
		@version nvarchar(50),
 		@credential nvarchar(255),
 		@password nvarchar(1000),
 		@startDate datetime,
 		@expirationDate datetime,
		@biometricID int,		
		@timestamp datetime,
		@enabled bit
 	)
 AS
 	INSERT INTO sysroPassports_AuthenticationMethods (
 		IDPassport,
 		Method,
		Version,
 		Credential,
 		Password,
 		StartDate,
 		ExpirationDate,
		BiometricID,
		BiometricData,
		TimeStamp,
		Enabled
 	)
 	VALUES (
 		@idPassport,
 		@method,
		@version,
 		@credential,
 		@password,
 		@startDate,
 		@expirationDate,
		@biometricID,
		null,
		@timestamp,
		@enabled
 	)
 	
 	RETURN
GO

 CREATE PROCEDURE [dbo].[WebLogin_Passports_AuthenticationMethods_Select] 
 	(
 		@idPassport int,
 		@method int,
		@version nvarchar(50)
 	)
 AS
 	SELECT IDPassport,
 		Method,
		Version,
 		Credential,
 		Password,
 		StartDate,
 		ExpirationDate,
		BiometricID,
		BiometricData,
		TimeStamp,
		Enabled
 	FROM sysroPassports_AuthenticationMethods
 	WHERE IDPassport = @idPassport AND
		  (@method IS NULL OR Method = @method) AND
		  (@version IS NULL OR Version = @version)
 	
 	RETURN
GO

 CREATE PROCEDURE [dbo].[WebLogin_Passports_AuthenticationMethods_Update]
 	(
 		@idPassport int,
 		@method int,
		@version nvarchar(50),
 		@credential nvarchar(255),
 		@password nvarchar(1000),
 		@startDate datetime,
 		@expirationDate datetime,
		@biometricID int,		
		@timestamp datetime,
		@enabled bit
 	)
 AS
 	UPDATE sysroPassports_AuthenticationMethods SET
 		Credential = @credential,
 		Password = @password,
 		StartDate = @startDate,
 		ExpirationDate = @expirationDate,		
		TimeStamp = @timestamp,
		Enabled = @enabled
 	WHERE IDPassport = @idPassport AND
 		Method = @method AND
		Version = @version AND
		BiometricID = @biometricID
 	
 	RETURN
GO

 CREATE PROCEDURE [dbo].[WebLogin_Passports_Insert] 
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
 		@ConfData text,
		@AuthenticationMerge nvarchar(50),
		@StartDate smalldatetime,
		@ExpirationDate smalldatetime,
		@State smallint
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
 		ConfData,
		AuthenticationMerge,
		StartDate,
		ExpirationDate,
		[State]
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
 		@ConfData,
		@AuthenticationMerge,
		@StartDate,
		@ExpirationDate,
		@State
 	)
 	
 	SELECT @id = @@IDENTITY
 	
 	RETURN
GO

 CREATE PROCEDURE [dbo].[WebLogin_Passports_Select] 
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
 		ConfData,
		AuthenticationMerge,
		StartDate,
		ExpirationDate,
		[State]
 	FROM sysroPassports
 	WHERE ID = @idPassport
 	
 	RETURN
GO

 CREATE PROCEDURE [dbo].[WebLogin_Passports_SelectAll] 
 	
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
		ConfData,
		AuthenticationMerge,
		StartDate,
		ExpirationDate,
		[State]
 	FROM sysroPassports
 	
 	RETURN
GO

 CREATE PROCEDURE [dbo].[WebLogin_Passports_SelectAllEmployee] 
 	
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
		ConfData,
		AuthenticationMerge,
		StartDate,
		ExpirationDate,
		[State]
 	FROM sysroPassports
 	WHERE IDEmployee IS NOT NULL
 	
 	RETURN
GO

 CREATE PROCEDURE [dbo].[WebLogin_Passports_SelectAllUser] 
 	
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
		ConfData,
		AuthenticationMerge,
		StartDate,
		ExpirationDate,
		[State]
 	FROM sysroPassports
 	WHERE IDUser IS NOT NULL
 	
 	RETURN
GO

 CREATE PROCEDURE [dbo].[WebLogin_Passports_SelectByIDEmployee] 
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
 		LevelOfAuthority,
		ConfData,
		AuthenticationMerge,
		StartDate,
		ExpirationDate,
		[State]
 	FROM sysroPassports
 	WHERE IDEmployee = @idEmployee
 	
 	RETURN
GO

 CREATE PROCEDURE [dbo].[WebLogin_Passports_SelectByIDUser]
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
 		LevelOfAuthority,
		ConfData,
		AuthenticationMerge,
		StartDate,
		ExpirationDate,
		[State]
 	FROM sysroPassports
 	WHERE IDUser = @idUser
 	
 	RETURN
GO

 CREATE PROCEDURE [dbo].[WebLogin_Passports_Update] 
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
 		@ConfData text,
		@AuthenticationMerge nvarchar(50),
		@StartDate smalldatetime,
		@ExpirationDate smalldatetime,
		@State smallint
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
 		ConfData = @ConfData,
		AuthenticationMerge = @AuthenticationMerge,
		StartDate = @StartDate,
		ExpirationDate = @ExpirationDate,
		[State] = @State
 	WHERE ID = @id
 	
 	RETURN
GO

 CREATE PROCEDURE [dbo].[WebLogin_PasswordExists] 
 	(
 		@password nvarchar(1000),
		@method int,
		@version nvarchar(50),
 		@idPassport int = NULL
 	)
 AS
 	IF EXISTS (
 		SELECT ID
 		FROM sysroPassports p
 		LEFT JOIN sysroPassports_AuthenticationMethods a ON p.ID = a.IDPassport
 		WHERE a.Method = @method AND
			  a.version = @version AND
 			  a.Password = @password AND			  
 			  (@idPassport IS NULL OR IDPassport <> @idPassport) AND
			  a.Enabled = 1
 	)
 		SELECT 1
 	ELSE
 		SELECT 0
 	
 	RETURN
GO

CREATE PROCEDURE [dbo].[WebLogin_PassportEmployee_ActiveContract] 
 	(
 		@idPassport int
 	)
 AS

		SELECT *
		FROM EmployeeContracts INNER JOIN sysroPassports
				ON EmployeeContracts.IDEmployee = sysroPassports.IDEmployee
		WHERE sysroPassports.ID = @idPassport AND
			  EmployeeContracts.BeginDate <= GETDATE() AND EmployeeContracts.EndDate >= GETDATE()
			  
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='213' WHERE ID='DBVersion'
GO
