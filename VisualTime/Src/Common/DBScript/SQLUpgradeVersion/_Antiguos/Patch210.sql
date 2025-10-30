-- VT Live
ALTER TABLE dbo.sysroGUI ADD
	RequiredFunctionalities nvarchar(200) NULL
GO

CREATE TABLE [dbo].[sysroFeatures] (
	[ID] [int] NOT NULL ,
	[IDParent] [int] NULL ,
	[Alias] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Description] [nvarchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Type] [varchar] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[PermissionTypes] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[AppHasPermissionsOverEmployees] [bit] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[sysroPassports] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[IDParentPassport] [int] NULL ,
	[GroupType] [varchar] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Description] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Email] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[IDUser] [tinyint] NULL ,
	[IDEmployee] [int] NULL ,
	[IDLanguage] [int] NOT NULL ,
	[LevelOfAuthority] [tinyint] NULL ,
	[ConfData] [text] NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[sysroPassports_AuthenticationMethods] (
	[IDPassport] [int] NOT NULL ,
	[Method] [int] NOT NULL ,
	[Credential] [nvarchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[Password] [nvarchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[StartDate] [datetime] NULL ,
	[ExpirationDate] [datetime] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[sysroPassports_PermissionsOverEmployees] (
	[IDPassport] [int] NOT NULL ,
	[IDEmployee] [int] NOT NULL ,
	[IDApplication] [int] NOT NULL ,
	[Permission] [tinyint] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[sysroPassports_PermissionsOverFeatures] (
	[IDPassport] [int] NOT NULL ,
	[IDFeature] [int] NOT NULL ,
	[Permission] [tinyint] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[sysroPassports_PermissionsOverGroups] (
	[IDPassport] [int] NOT NULL ,
	[IDGroup] [int] NOT NULL ,
	[IDApplication] [int] NOT NULL ,
	[Permission] [tinyint] NOT NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[sysroFeatures] WITH NOCHECK ADD 
	CONSTRAINT [PK_sysroFeatures] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[sysroPassports] WITH NOCHECK ADD 
	CONSTRAINT [PK_sysroPassports] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[sysroPassports_AuthenticationMethods] WITH NOCHECK ADD 
	CONSTRAINT [PK_sysroPassports_AuthenticationMethods] PRIMARY KEY  CLUSTERED 
	(
		[IDPassport],
		[Method]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[sysroPassports_PermissionsOverEmployees] WITH NOCHECK ADD 
	CONSTRAINT [PK_sysroPassports_PermissionsOverEmployees_1] PRIMARY KEY  CLUSTERED 
	(
		[IDPassport],
		[IDEmployee],
		[IDApplication]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[sysroPassports_PermissionsOverFeatures] WITH NOCHECK ADD 
	CONSTRAINT [PK_sysroPassports_PermissionsOverFeatures] PRIMARY KEY  CLUSTERED 
	(
		[IDPassport],
		[IDFeature]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[sysroPassports_PermissionsOverGroups] WITH NOCHECK ADD 
	CONSTRAINT [PK_sysroPassports_PermissionsOverGroups_1] PRIMARY KEY  CLUSTERED 
	(
		[IDPassport],
		[IDGroup],
		[IDApplication]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[sysroFeatures] ADD 
	CONSTRAINT [DF_sysroFeatures_Alias] DEFAULT ('') FOR [Alias],
	CONSTRAINT [DF_sysroFeatures_Name] DEFAULT ('') FOR [Name],
	CONSTRAINT [DF_sysroFeatures_Description] DEFAULT ('') FOR [Description],
	CONSTRAINT [DF_sysroFeatures_Type] DEFAULT ('') FOR [Type],
	CONSTRAINT [DF_sysroFeatures_PermissionTypes] DEFAULT ('') FOR [PermissionTypes]
GO

 CREATE  INDEX [IX_sysroFeatures_IDParent] ON [dbo].[sysroFeatures]([IDParent]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[sysroPassports] ADD 
	CONSTRAINT [DF_sysroPassports_Name] DEFAULT ('') FOR [Name],
	CONSTRAINT [DF_sysroPassports_Description] DEFAULT ('') FOR [Description],
	CONSTRAINT [DF_sysroPassports_Email] DEFAULT ('') FOR [Email]
GO

 CREATE  INDEX [IX_sysroPassports_IDEmployee] ON [dbo].[sysroPassports]([IDEmployee]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_sysroPassports_IDLanguage] ON [dbo].[sysroPassports]([IDLanguage]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_sysroPassports_IDParentPassport] ON [dbo].[sysroPassports]([IDParentPassport]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_sysroPassports_IDUser] ON [dbo].[sysroPassports]([IDUser]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_sysroPassports_AuthenticationMethods_Credential] ON [dbo].[sysroPassports_AuthenticationMethods]([Credential]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_sysroPassports_PermissionsOverEmployees_IDPassport] ON [dbo].[sysroPassports_PermissionsOverEmployees]([IDPassport]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_sysroPassports_PermissionsOverEmployees_IDEmployee] ON [dbo].[sysroPassports_PermissionsOverEmployees]([IDEmployee]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_sysroPassports_PermissionsOverEmployees_IDApplication] ON [dbo].[sysroPassports_PermissionsOverEmployees]([IDApplication]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_sysroPassports_PermissionsOverFeatures_IDPassport] ON [dbo].[sysroPassports_PermissionsOverFeatures]([IDPassport]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_sysroPassports_PermissionsOverFeatures_IDFeature] ON [dbo].[sysroPassports_PermissionsOverFeatures]([IDFeature]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_sysroPassports_PermissionsOverGroups_IDPassport] ON [dbo].[sysroPassports_PermissionsOverGroups]([IDPassport]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_sysroPassports_PermissionsOverGroups_IDGroup] ON [dbo].[sysroPassports_PermissionsOverGroups]([IDGroup]) ON [PRIMARY]
GO

 CREATE  INDEX [IX_sysroPassports_PermissionsOverGroups_IDApplication] ON [dbo].[sysroPassports_PermissionsOverGroups]([IDApplication]) ON [PRIMARY]
GO

ALTER TABLE [dbo].[sysroFeatures] ADD 
	CONSTRAINT [FK_sysroFeatures_sysroFeatures] FOREIGN KEY 
	(
		[IDParent]
	) REFERENCES [dbo].[sysroFeatures] (
		[ID]
	)
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[sysroUsers]') AND name = N'IX_sysroUsers_IDUser')
DROP INDEX [IX_sysroUsers_IDUser] ON [dbo].[sysroUsers] 
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_sysroUsers_IDUser] ON [dbo].[sysroUsers] 
(
	[IDUser] ASC
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[sysroPassports] ADD 
	CONSTRAINT [FK_sysroPassports_Employees] FOREIGN KEY 
	(
		[IDEmployee]
	) REFERENCES [dbo].[Employees] (
		[ID]
	),
	CONSTRAINT [FK_sysroPassports_sysroPassports] FOREIGN KEY 
	(
		[IDParentPassport]
	) REFERENCES [dbo].[sysroPassports] (
		[ID]
	),
	CONSTRAINT [FK_sysroPassports_sysroUsers] FOREIGN KEY 
	(
		[IDUser]
	) REFERENCES [dbo].[sysroUsers] (
		[IDUser]
	)
GO


ALTER TABLE [dbo].[sysroPassports_AuthenticationMethods] ADD 
	CONSTRAINT [FK_sysroPassports_AuthenticationMethods_sysroPassports] FOREIGN KEY 
	(
		[IDPassport]
	) REFERENCES [dbo].[sysroPassports] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
GO

ALTER TABLE [dbo].[sysroPassports_PermissionsOverEmployees] ADD 
	CONSTRAINT [FK_sysroPassports_PermissionsOverEmployees_Employees] FOREIGN KEY 
	(
		[IDEmployee]
	) REFERENCES [dbo].[Employees] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE ,
	CONSTRAINT [FK_sysroPassports_PermissionsOverEmployees_sysroFeatures] FOREIGN KEY 
	(
		[IDApplication]
	) REFERENCES [dbo].[sysroFeatures] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE ,
	CONSTRAINT [FK_sysroPassports_PermissionsOverEmployees_sysroPassports] FOREIGN KEY 
	(
		[IDPassport]
	) REFERENCES [dbo].[sysroPassports] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
GO

ALTER TABLE [dbo].[sysroPassports_PermissionsOverFeatures] ADD 
	CONSTRAINT [FK_sysroPassports_PermissionsOverFeatures_sysroFeatures] FOREIGN KEY 
	(
		[IDFeature]
	) REFERENCES [dbo].[sysroFeatures] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE ,
	CONSTRAINT [FK_sysroPassports_PermissionsOverFeatures_sysroPassports] FOREIGN KEY 
	(
		[IDPassport]
	) REFERENCES [dbo].[sysroPassports] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
GO

ALTER TABLE [dbo].[sysroPassports_PermissionsOverGroups] ADD 
	CONSTRAINT [FK_sysroPassports_PermissionsOverGroups_Groups] FOREIGN KEY 
	(
		[IDGroup]
	) REFERENCES [dbo].[Groups] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE ,
	CONSTRAINT [FK_sysroPassports_PermissionsOverGroups_sysroFeatures] FOREIGN KEY 
	(
		[IDApplication]
	) REFERENCES [dbo].[sysroFeatures] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE ,
	CONSTRAINT [FK_sysroPassports_PermissionsOverGroups_sysroPassports] FOREIGN KEY 
	(
		[IDPassport]
	) REFERENCES [dbo].[sysroPassports] (
		[ID]
	) ON DELETE CASCADE  ON UPDATE CASCADE 
GO

INSERT INTO [sysroPassports] ([IDParentPassport],[GroupType],[Name],[Description],[Email],[IDUser],[IDEmployee],[IDLanguage],[LevelOfAuthority])
VALUES(NULL,'','Admin','Administrator','',NULL,NULL,0,NULL)
DECLARE @ID int
SET @ID = @@IDENTITY

INSERT INTO [sysroPassports_AuthenticationMethods] ([IDPassport],[Method],[Credential],[Password],[StartDate],[ExpirationDate])VALUES(@ID,1,'Admin','',NULL,NULL)
INSERT INTO [sysroUsers] ([Login],[Password],[IDSecurityGroup],[UserConfData])VALUES('´WebLogin´ : ' + CAST(@ID AS varchar(10)),NULL,'',NULL)
DECLARE @IDUser int
SET @IDUser = @@IDENTITY

UPDATE sysroPassports SET IDUser = @IDUser WHERE ID = @ID
GO

INSERT INTO [sysroPassports] ([IDParentPassport],[GroupType],[Name],[Description],[Email],[IDUser],[IDEmployee],[IDLanguage],[LevelOfAuthority])
VALUES(NULL,'','User','User','',NULL,NULL,0,NULL)
DECLARE @ID int
SET @ID = @@IDENTITY

INSERT INTO [sysroPassports_AuthenticationMethods] ([IDPassport],[Method],[Credential],[Password],[StartDate],[ExpirationDate])VALUES(@ID,2,'User','',NULL,NULL)
INSERT INTO [sysroUsers] ([Login],[Password],[IDSecurityGroup],[UserConfData])VALUES('´WebLogin´ : ' + CAST(@ID AS varchar(10)),NULL,'',NULL)
DECLARE @IDUser int
SET @IDUser = @@IDENTITY

UPDATE sysroPassports SET IDUser = @IDUser WHERE ID = @ID
GO

ALTER TABLE dbo.sysroPassports_AuthenticationMethods ADD
	BiometricID int NULL,
	IDCard int NULL,
	WindowsSession nchar(255) NULL,
	WebLogin nchar(255) NULL,
	WebPassword nchar(255) NULL
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

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetEmployeeGroup]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[GetEmployeeGroup]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetEmployeeGroupChilds]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[GetEmployeeGroupChilds]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetEmployeeGroupParent]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[GetEmployeeGroupParent]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetEmployeeGroupTree]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[GetEmployeeGroupTree]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetPassportChilds]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[GetPassportChilds]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetPassportParents]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[GetPassportParents]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Split]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[Split]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SplitInt]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[SplitInt]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_GetPassportPermissionOverGroup]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[WebLogin_GetPassportPermissionOverGroup]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_GetPermissionOverEmployee]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[WebLogin_GetPermissionOverEmployee]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_GetPermissionOverFeature]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[WebLogin_GetPermissionOverFeature]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[WebLogin_GetPermissionOverGroup]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[WebLogin_GetPermissionOverGroup]
GO

CREATE FUNCTION dbo.GetEmployeeGroup 
	(
		@idEmployee int,
		@date smalldatetime
	)
RETURNS int
AS
	BEGIN
		
		DECLARE @Result int
	
		SELECT @Result = IDGroup
		FROM EmployeeGroups
		WHERE IDEmployee = @idEmployee AND
			@date >= BeginDate AND @date <= EndDate
			
	RETURN @Result
	END
GO

CREATE FUNCTION dbo.GetEmployeeGroupChilds 
	(
		@idEmployeeGroup int
	)
RETURNS @ValueTable table(ID int)
AS
	BEGIN
		DECLARE @Path nvarchar(16)
		SELECT @Path = Path
		FROM Groups
		WHERE ID = @idEmployeeGroup
	
		INSERT INTO @ValueTable
			SELECT ID
			FROM Groups
			WHERE Path LIKE @Path + '\%'
			ORDER BY ID
		
	RETURN
	END
GO

CREATE FUNCTION dbo.GetEmployeeGroupParent 
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
		DECLARE @Path nvarchar(16)
		SELECT @Path = Path
		FROM Groups
		WHERE ID = @idEmployeeGroup
		
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
GO

CREATE FUNCTION dbo.GetEmployeeGroupTree
	(
		@idEmployee int,
		@idEmployeeGroup int,
		@date datetime
	)
RETURNS @ValueTable table (ID int)
AS
	BEGIN
		/* Returns the list of groups and sub-groups employee belongs to */
		/* Only one of idEmployee or idEmployeeGroup should be specified */
		
		DECLARE @Path nvarchar(16)

		/* Find out in which group employee is directly */
		IF NOT @idEmployee IS NULL
			SELECT @idEmployeeGroup = dbo.GetEmployeeGroup(@idEmployee, @date)
		
		/* If employee is in a group, return groups tree */
		IF NOT @idEmployeeGroup IS NULL
		BEGIN
			SELECT @Path = Path
			FROM Groups
			WHERE ID = @idEmployeeGroup
			
			INSERT INTO @ValueTable 
				SELECT * 
				FROM dbo.SplitInt(@Path, '\')
		END

		RETURN
	END
GO

CREATE FUNCTION dbo.GetPassportChilds
	(
		@idPassport int
	)
RETURNS @result table (ID int PRIMARY KEY)
AS
BEGIN
	DECLARE @Childs table (ID int PRIMARY KEY)
	DECLARE @ChildsOld table (ID int PRIMARY KEY)
	
	/* Returns all childs of specified passport */
	INSERT INTO @Childs 
		SELECT ID 
		FROM sysroPassports
		WHERE IDParentPassport = @idPassport
	
	WHILE (SELECT COUNT(*) FROM @Childs) > 0
	BEGIN
		INSERT INTO @result SELECT ID FROM @Childs
		DELETE FROM @ChildsOld
		INSERT INTO @ChildsOld SELECT ID FROM @Childs

		DELETE FROM @Childs
		INSERT INTO @Childs
			SELECT ID
			FROM sysroPassports
			WHERE IDParentPassport IN (SELECT ID FROM @ChildsOld)
	END
	
	RETURN
END
GO

CREATE FUNCTION dbo.GetPassportParents
	(
		@idPassport int
	)
RETURNS @result table (ID int PRIMARY KEY)
AS
BEGIN
	/* Returns all parents of specified passport */
	SELECT @idPassport = IDParentPassport
	FROM sysroPassports
	WHERE ID = @idPassport
	
	WHILE NOT @idPassport IS NULL
	BEGIN
		INSERT INTO @result VALUES (@idPassport)
		
		SELECT @idPassport = IDParentPassport
		FROM sysroPassports
		WHERE ID = @idPassport
	END
	
	RETURN
END
GO

create function dbo.Split(
 @String nvarchar (4000),
 @Delimiter nvarchar (10)
 )
returns @ValueTable table ([Value] nvarchar(4000))
begin
 declare @NextString nvarchar(4000)
 declare @Pos int
 declare @NextPos int
 declare @CommaCheck nvarchar(1)
 
 --Initialize
 set @NextString = ''
 set @CommaCheck = right(@String,1) 
 
 --Check for trailing Comma, if not exists, INSERT
 --if (@CommaCheck <> @Delimiter )
 set @String = @String + @Delimiter
 
 --Get position of first Comma
 set @Pos = charindex(@Delimiter,@String)
 set @NextPos = 1
 
 --Loop while there is still a comma in the String of levels
 while (@pos <>  0)  
 begin
  set @NextString = substring(@String,1,@Pos - 1)
 
  insert into @ValueTable ( [Value]) Values (@NextString)
 
  set @String = substring(@String,@pos +1,len(@String))
  
  set @NextPos = @Pos
  set @pos  = charindex(@Delimiter,@String)
 end
 
 return
end

GO

CREATE function dbo.SplitInt(
 @String nvarchar (4000),
 @Delimiter nvarchar (10)
 )
returns @ValueTable table ([Value] int)
begin
 declare @NextString nvarchar(4000)
 declare @Pos int
 declare @NextPos int
 declare @CommaCheck nvarchar(1)
 
 --Initialize
 set @NextString = ''
 set @CommaCheck = right(@String,1) 
 
 --Check for trailing Comma, if not exists, INSERT
 --if (@CommaCheck <> @Delimiter )
 set @String = @String + @Delimiter
 
 --Get position of first Comma
 set @Pos = charindex(@Delimiter,@String)
 set @NextPos = 1
 
 --Loop while there is still a comma in the String of levels
 while (@pos <>  0)  
 begin
  set @NextString = substring(@String,1,@Pos - 1)
 
  insert into @ValueTable ( [Value]) Values (cast(@NextString as int))
 
  set @String = substring(@String,@pos +1,len(@String))
  
  set @NextPos = @Pos
  set @pos  = charindex(@Delimiter,@String)
 end
 
 return
end

GO

CREATE FUNCTION dbo.WebLogin_GetPassportPermissionOverGroup
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

CREATE FUNCTION dbo.WebLogin_GetPermissionOverEmployee 
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
	
	/* First look at employees exceptions, on specified passport then on 
	each of it's parents. If nothing is found, look at groups permissions. */
	
	/* Check permissions */
	IF @mode <> 2
		SET @LoopPassport = @idPassport
	ELSE
		SELECT @LoopPassport = IDParentPassport
		FROM sysroPassports
		WHERE ID = @idPassport
	
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
				SELECT @Result = dbo.WebLogin_GetPermissionOverGroup(@idPassport, @IDGroup, @idApplication, @mode)
		END
	END
	
	
	/* Return result */
	IF @Result IS NULL
		SET @Result = 0
		
	RETURN @Result
END
GO

CREATE FUNCTION dbo.WebLogin_GetPermissionOverFeature 
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

CREATE FUNCTION dbo.WebLogin_GetPermissionOverGroup 
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
	
	/* Return the most restricted passport permission by looking at parent
	passport's permissions one by one */
	
	IF @mode <> 3
		SELECT @Result = dbo.WebLogin_GetPassportPermissionOverGroup(@idPassport, @idGroup, @idApplication, @mode)
	
	IF @mode <> 1
	BEGIN
		SELECT @ParentPassport = IDParentPassport
		FROM sysroPassports
		WHERE ID = @idPassport
		
		/* If looking for inherited only, the constraint is for first check only.
		Other queries should be threated as normal */
		WHILE NOT @ParentPassport IS NULL
		BEGIN
			SELECT @NewResult = dbo.WebLogin_GetPassportPermissionOverGroup(@ParentPassport, @idGroup, @idApplication, 0)
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

ALTER TABLE dbo.sysroUserFields ADD
	AccessLevel smallint NULL
GO

ALTER TABLE dbo.Employees ADD
	[AttControlled] [bit] NULL,
	[AccControlled] [bit] NULL,
	[JobControlled] [bit] NULL,
	[ExtControlled] [bit] NULL,
	[RiskControlled] [bit] NULL	
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetFullGroupPathName]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetFullGroupPathName]

GO

 CREATE FUNCTION [dbo].[GetFullGroupPathName] (@GroupId int)
 	RETURNS varchar(300) 
 AS
 BEGIN
 	declare @path varchar(200);
 	declare @index int;
 	declare @delimiter char(1);
 	declare @i int;
 	declare @resultado varchar(300);
 	declare @id varchar(100);
 	declare @nombre varchar(200);
 	declare @longitud varchar(100);
 	declare @contador int;
 	declare @anterior int;
 	declare @siguiente int;
 	declare @digitos int;
 	set @contador=0
 	set @resultado=''
 	set @index=1
 	set @id=''
 	set @i=1
 	set @delimiter='\'
 	-- Seleccionamos el Path del grupo que nos manda el usuario
 	select @path=Path from Groups where ID=str(@GroupId)
 	-- print 'El path es: '+@path
 	-- Miramos la longitud total del Path
 	select @longitud = len(@path)
 	--print 'La longitud es: '+str(@longitud)
 	-- Controlamos el primer valor del grupo
 	while (@contador=0)
 	begin
 		select @id=left(@path,1)
 		select @contador=@contador+1
 		select @nombre=Name from Groups where ID=@id
 		select @resultado=@nombre
 	end
 	-- Controlamos que hay alguna \ en el path
 	while (@index!=0)
 	begin
 		-- Buscamos la posicion de la \
 		select @index = CHARINDEX(@delimiter,@path,@i)
 		-- Guardamos la localizacion de la primera \
 		select @anterior = @index
 		-- Si encuentra la \
 		if (@index!=0) 
 		begin
 			-- Colocamos el cursor en la siguiente posicion		
 			select @i=@index+1
 			-- Volvemos a buscar \ por si es la ultima
 			select @index = CHARINDEX(@delimiter,@path,@i)
 			--print 'Posicion de la \: '+str(@index)
 			-- Si existe otra barra por delante
 			if (@index!=0)
 			begin
 				-- Restamos 1 porque entre las \ albergan minimo 2 espacios
 				select @digitos=@index-@anterior-1
 				--Comprobamos si el grupo tiene 1 digito
 				if (@digitos=1)
 				begin
 					select @id=right(left(@path,@i),1)
 					-- Buscamos el id en la base de datos
 					select @nombre=Name from Groups where ID=@id
 					--print 'Grupo: '+@nombre
 					-- Concatenamos el valor al resultado final
 					select @resultado=@resultado+' \ '+@nombre
 				end
 				-- Si el id del grupo esta formado por dos digitos
 				if (@digitos=2)
 				begin
 					-- Movemos el puntero para poder recortar el id del grupo
 					select @i=@i+1
 					select @id=right(left(@path,@i),2)
 					select @nombre=Name from Groups where ID=@id
 					-- Concatenamos el valor al resultado final
 					select @resultado=@resultado+' \ '+@nombre
 				end
 				-- Si el id del grupo esta formado por tres digitos
 				if (@digitos=3)
 				begin
 					select @i=@i+2
 					select @id=right(left(@path,@i),3)
 					select @nombre=Name from Groups where ID=@id
 					-- Concatenamos el valor al resultado final
 					select @resultado=@resultado+' \ '+@nombre
 				end
 			end
 			-- Sino no encuentra \
 			else
 			begin
 				select @digitos=@longitud-@anterior
 				-- Si el id del grupo esta formado por un digito
 				if (@digitos=1)
 				begin
 				-- Movemos el puntero para poder recortar el id del grupo
 				select @i=@i+2
 				select @id=right(left(@path,@i),1)
 				select @nombre=Name from Groups where ID=@id
 				-- Concatenamos el valor al resultado final
 				select @resultado=@resultado+' \ '+@nombre
 				end
 				-- Si el id del grupo esta formado por dos digitos
 				if (@digitos=2)
 				begin
 					select @i=@i+3
 					select @id=right(left(@path,@i),2)
 					select @nombre=Name from Groups where ID=@id
 					-- Concatenamos el valor al resultado final
 					select @resultado=@resultado+' \ '+@nombre
 				end
 				-- Si el id del grupo esta formado por tres digitos
 				if (@digitos=3)
 				begin
 				select @i=@i+2
 				select @id=right(left(@path,@i),3)
 				select @nombre=Name from Groups where ID=@id
 				-- Concatenamos el valor al resultado final
 				select @resultado=@resultado+' \ '+@nombre
 				return @resultado
 				--return 'El nombre es:'+@resultado
 				end
 			end
 		end
 	end
 	return (@resultado)
 END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[sysrovwCurrentEmployeeGroups]') AND OBJECTPROPERTY(id, N'IsView') = 1)
DROP VIEW [dbo].[sysrovwCurrentEmployeeGroups]
GO

CREATE VIEW [dbo].[sysrovwCurrentEmployeeGroups]
AS
SELECT     dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                      dbo.Employees.Name AS EmployeeName, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, 
                      COUNT(dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee) AS CurrentEmployee, dbo.Employees.AttControlled, dbo.Employees.AccControlled, 
                      dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, 
                      dbo.GetFullGroupPathName(dbo.EmployeeGroups.IDGroup) AS FullGroupName
FROM         dbo.EmployeeGroups INNER JOIN
                      dbo.Employees ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID INNER JOIN
                      dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                      dbo.sysrosubvwCurrentEmployeePeriod ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
GROUP BY dbo.Groups.Name, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                      dbo.Employees.Name, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, dbo.Employees.AttControlled, dbo.Employees.AccControlled, 
                      dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled
HAVING      (dbo.EmployeeGroups.EndDate >= CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102)) AND 
                      (dbo.EmployeeGroups.BeginDate <= CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102))

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[sysrovwFutureEmployeeGroups]') AND OBJECTPROPERTY(id, N'IsView') = 1)
DROP VIEW [dbo].[sysrovwFutureEmployeeGroups]
GO

CREATE VIEW [dbo].[sysrovwFutureEmployeeGroups]
AS
SELECT     dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                      dbo.Employees.Name AS EmployeeName, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, '0' AS CurrentEmployee, 
                      dbo.Employees.AttControlled, dbo.Employees.AccControlled, dbo.Employees.JobControlled, dbo.Employees.ExtControlled, 
                      dbo.Employees.RiskControlled, dbo.GetFullGroupPathName(dbo.EmployeeGroups.IDGroup) AS FullGroupName
FROM         dbo.EmployeeGroups INNER JOIN
                      dbo.Employees ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID INNER JOIN
                      dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                      dbo.sysrosubvwCurrentEmployeePeriod ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
WHERE     (dbo.EmployeeGroups.BeginDate > CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102)) AND (dbo.Employees.ID NOT IN
                          (SELECT     IDEmployee
                            FROM          dbo.sysrovwCurrentEmployeeGroups))

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[sysrovwAllEmployeeGroups]') AND OBJECTPROPERTY(id, N'IsView') = 1)
DROP VIEW [dbo].[sysrovwAllEmployeeGroups]
GO

CREATE VIEW [dbo].[sysrovwAllEmployeeGroups]
AS
SELECT     GroupName, Path, SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, CurrentEmployee, AttControlled, AccControlled, 
                      JobControlled, ExtControlled, RiskControlled, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName
FROM         dbo.sysrovwCurrentEmployeeGroups
UNION
SELECT     GroupName, Path, SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, CurrentEmployee, AttControlled, AccControlled, 
                      JobControlled, ExtControlled, RiskControlled, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName
FROM         dbo.sysrovwFutureEmployeeGroups

GO

CREATE TABLE dbo.sysroLanguages
	(
	ID int NOT NULL,
	LanguageKey varchar(3) NOT NULL,
	Culture varchar(10) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.sysroLanguages ADD CONSTRAINT
	PK_sysroLanguages PRIMARY KEY CLUSTERED 
	(
	ID
	)  ON [PRIMARY]

GO

INSERT INTO [sysroLanguages] ([ID],[LanguageKey],[Culture]) VALUES (0, 'ESP', 'es-ES')
GO
INSERT INTO [sysroLanguages] ([ID],[LanguageKey],[Culture]) VALUES (1, 'CAT', 'ca-es')
GO
INSERT INTO [sysroLanguages] ([ID],[LanguageKey],[Culture]) VALUES (2, 'ENG', 'en-US')
GO

CREATE TABLE [dbo].[AuditLive](
	[ID] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
	[PassportName] [nvarchar](50) NOT NULL,
	[Date] [datetime] NOT NULL,
	[ActionID] [smallint] NOT NULL,
	[ElementID] [smallint] NOT NULL,
	[ElementName] [nvarchar](4000) NULL,
	[MessageParameters] [nvarchar](4000) NULL,
	[SessionID] [int] NOT NULL,
	[ClientLocation] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_AuditLive] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,RequiredFunctionalities) VALUES ('Portal',null,null,null,1,null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Configuration','Configuration',null,'Administracion.ico',1004,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Attendance','Attendance',null,'Presencia.gif',1001,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Configuration\Audit','Audit','Audit/Audit.aspx','Audit.gif',130,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Attendance\Causes','Causes','Causes/Causes.aspx','Justificaciones_24.gif',660,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Attendance\Accruals','Accruals','Concepts/Concepts.aspx','Acumulados_24.gif',650,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Attendance\Employees','InfoEmployees','Employees/Employees.aspx','Empleado_24.gif',600,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Configuration\Options','ConfigurationOptions','Options/ConfigurationOptions.aspx','Configuracion.gif',110,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Configuration\AttendanceOptions','AttendanceOptions','Options/AttendanceOptions.aspx','Configuracion.gif',120,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Attendance\Scheduler','Scheduler','Scheduler/Scheduler.aspx','Calendario_24.gif',610,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Attendance\Shifts','Shifts','Shifts/Shifts.aspx','Horarios_24.gif',640,'NWR',null)
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='210' WHERE ID='DBVersion'
GO
