ALTER TABLE dbo.Concepts ADD
	EmployeesPermission smallint NULL,
	EmployeesCriteria text NULL
GO
ALTER TABLE dbo.Concepts ADD CONSTRAINT
	DF_Concepts_RequestPermissions DEFAULT ((0)) FOR EmployeesPermission
GO

UPDATE Concepts
SET EmployeesPermission = CASE ViewInTerminals WHEN 0 THEN 1 ELSE 0 END
GO

/* Entrada a SysroGUI LivePortal */
DELETE FROM sysroGUI WHERE IDPath LIKE 'LivePortal%'
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,[Type], [Parameters], RequiredFeatures, SecurityFlags, Priority,AllowedSecurity,RequiredFunctionalities)
VALUES ('LivePortal',NULL, '',	NULL,	NULL,	NULL,	NULL,	NULL,	1,	NULL,	NULL)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,[Type], [Parameters], RequiredFeatures, SecurityFlags, Priority,AllowedSecurity,RequiredFunctionalities)
VALUES('LivePortal\MyRequests',	'LivePortal.MyRequests',	'Requests/Requests.aspx',	'MyRequests.png',	NULL	,NULL	,NULL	,NULL	,110	,NULL	,'E:Punches.Requests=Read OR E:Planification.Requests=Read OR E:UserFields.Requests=Read')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,[Type], [Parameters], RequiredFeatures, SecurityFlags, Priority,AllowedSecurity,RequiredFunctionalities)
VALUES('LivePortal\MyUserFields',	'LivePortal.MyUserFields',	'UserFields/UserFields.aspx',	'MyUserFields.png',	NULL	,NULL	,NULL	,NULL	,310	,NULL	,'E:UserFields.Query=Read OR E:UserFields.Requests=Write')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,[Type], [Parameters], RequiredFeatures, SecurityFlags, Priority,AllowedSecurity,RequiredFunctionalities)
VALUES('LivePortal\TimeControl',	'LivePortal.TimeControl', '', 'TimeControl.png',	NULL	,NULL	,NULL	,NULL	,410	,NULL	,NULL)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,[Type], [Parameters], RequiredFeatures, SecurityFlags, Priority,AllowedSecurity,RequiredFunctionalities)
VALUES('LivePortal\TimeControl\MyAccruals',	'LivePortal.MyAccruals',	'Querys/AccrualsQuery.aspx',	'Querys.png',	NULL	,NULL	,NULL	,NULL	,610	,NULL	,NULL)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,[Type], [Parameters], RequiredFeatures, SecurityFlags, Priority,AllowedSecurity,RequiredFunctionalities)
VALUES('LivePortal\TimeControl\MyCalendar',	'LivePortal.MyCalendar',	'Calendar/Calendar.aspx',	'Calendar.png',	NULL	,NULL	,NULL	,NULL	,510	,NULL	,'E:Planification.Query=Read')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,[Type], [Parameters], RequiredFeatures, SecurityFlags, Priority,AllowedSecurity,RequiredFunctionalities)
VALUES('LivePortal\TimeControl\MyPunches',	'LivePortal.MyPunches',	'Punches/Punches.aspx',	'Punches.png'	,NULL	,NULL	,NULL	,NULL	,710	,NULL	,'E:Punches.Query=Read')
GO

/* Entrada s sysroGUI pantalla Requests */
DELETE FROM sysroGUI WHERE IDPath = 'Portal\General\Requests'
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,[Type], [Parameters], RequiredFeatures, SecurityFlags, Priority,AllowedSecurity,RequiredFunctionalities)
VALUES('Portal\General\Requests', 'Supervisor.Requests', 'Requests/Requests.aspx',	'Requests.png'	,NULL	,NULL	,'Feature\LivePortal',NULL	,604	,NULL	,'U:Employees.UserFields.Requests=Read OR U:Calendar.Punches.Requests=Read OR U:Calendar.Requests=Read')
GO

/* sysroReaderTemplates */
INSERT INTO sysroReaderTemplates([Type], IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, [Output], InvalidOutput, Direction)
VALUES ('LivePortal', 1, 31, 'TA', '1', 'Interactive', NULL, 'Server', '1,0', '0', '0', '0', '0', 'web')
GO
INSERT INTO sysroReaderTemplates([Type], IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, [Output], InvalidOutput, Direction)
VALUES ('LivePortal', 1, 32, 'TAEIP', '1', 'Interactive', NULL,	'Server', '1,0', '0', '0', '0', '0', 'web')
GO
INSERT INTO sysroReaderTemplates([Type], IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, [Output], InvalidOutput, Direction)
VALUES ('LivePortal', 1, 33, 'EIP', '1', 'Interactive', NULL, 'Server', '1,0', '0', '0', '0', '0', 'web')
GO

/* sysroFeatures */
INSERT INTO sysroFeatures(ID, IDParent, Alias, Name, Description, Type, PermissionTypes, AppHasPermissionsOverEmployees)
VALUES (20001, 20, 'Punches.Punches', 'Fichar', '',	'E', 'W', NULL)
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

  ALTER PROCEDURE [dbo].[WebLogin_Passports_Select] 
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
  		CASE WHEN LevelOfAuthority IS NULL THEN 
			(SELECT Parent.LevelOfAuthority FROM sysroPassports Parent WHERE Parent.ID = sysroPassports.IDParentPassport)
			ELSE LevelOfAuthority END AS LevelOfAuthority,
  		ConfData,
 		AuthenticationMerge,
 		StartDate,
 		ExpirationDate,
 		[State]
  	FROM sysroPassports
  	WHERE ID = @idPassport
  	
  	RETURN
GO

ALTER TABLE dbo.Requests ADD
	NotReaded bit NULL
GO

ALTER TABLE dbo.RequestsApprovals ADD
	Comments text NULL
GO

CREATE FUNCTION [dbo].[GetRequestMinStatusLevel]
 	(
 		@idPassport int,
		@featureAlias nvarchar(100),
		@EmployeefeatureID int,
 		@idEmployee int
 	)
 RETURNS int
 AS
 BEGIN
 	DECLARE @MinStatusLevel int
 	DECLARE @LevelOfAuthority int

	SELECT @LevelOfAuthority = CASE GroupType WHEN 'U' THEN sysroPassports.LevelOfAuthority ELSE (SELECT Parent.LevelOfAuthority FROM sysroPassports Parent WHERE Parent.ID = sysroPassports.IDParentPassport) END
	FROM sysroPassports 
	WHERE sysroPassports.ID = @idPassport
 	

	SELECT @MinStatusLevel = 
	(SELECT TOP 1 Parents.LevelOfAuthority
	 FROM sysroPassports INNER JOIN sysroPassports Parents
			ON sysroPassports.IDParentPassport = Parents.ID
	 WHERE sysroPassports.GroupType = '' AND
	 	  Parents.LevelOfAuthority IS NOT NULL AND
	 	  Parents.LevelOfAuthority > @LevelOfAuthority AND
		  (select dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID,@FeatureAlias, 'U', 2)) > 0 AND
		  (select dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@EmployeefeatureID,2,1,getdate())) > 0 
	 ORDER BY Parents.LevelOfAuthority ASC)


 	
 RETURN @MinStatusLevel
 END
 GO
 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

  ALTER PROCEDURE [dbo].[WebLogin_Passports_Update] 
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

	IF @groupType <> 'U' 
		BEGIN
			SET @levelOfAuthority = NULL
		END

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

ALTER TABLE dbo.Shifts ADD
	ShiftType smallint NULL
GO
ALTER TABLE dbo.Shifts ADD CONSTRAINT
	DF_Shifts_ShiftType DEFAULT 0 FOR ShiftType
GO
 
/* ***************************************************************************************************************************** */
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='246' WHERE ID='DBVersion'
GO
