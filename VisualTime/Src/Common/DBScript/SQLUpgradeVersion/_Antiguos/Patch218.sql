ALTER TABLE dbo.Causes ADD
	CauseType smallint NOT NULL CONSTRAINT DF_Causes_CauseType DEFAULT 0,
	PunchCloseProgrammedAbsence bit NOT NULL CONSTRAINT DF_Causes_PunchCloseProgrammedAbsence DEFAULT 0,
	VisibilityPermissions smallint NOT NULL CONSTRAINT DF_Causes_VisibilityPermissions DEFAULT 0,
	VisibilityCriteria text NULL,
	InputPermissions smallint NOT NULL CONSTRAINT DF_Causes_InputPermissions DEFAULT 0,
	InputCriteria text NULL,
	ApplyJustifyPeriod bit NOT NULL CONSTRAINT DF_Causes_ApplyJustifyPeriod DEFAULT 0,
	JustifyPeriodStart int NULL,
	JustifyPeriodEnd int NULL,
	JustifyPeriodType bit NULL
GO
ALTER TABLE dbo.Causes ADD CONSTRAINT
	DF_Causes_JustifyPeriodType DEFAULT 0 FOR JustifyPeriodType
GO

ALTER TABLE dbo.ConceptCauses ADD
	Composition text NULL
GO

-- *************************
ALTER TABLE dbo.sysroLanguages ADD
	Parameters text NULL
GO

UPDATE sysroLanguages SET Parameters = '<?xml version="1.0"?><roCollection version="2.0"><Item key="ExtLanguage" type="8">sp</Item><Item key="ExtDatePickerFormat" type="8">d/m/Y</Item><Item key="ExtDatePickerStartDay" type="8">1</Item></roCollection>' WHERE ID = 0
GO
UPDATE sysroLanguages SET Parameters = '<?xml version="1.0"?><roCollection version="2.0"><Item key="ExtLanguage" type="8">ca</Item><Item key="ExtDatePickerFormat" type="8">d/m/Y</Item><Item key="ExtDatePickerStartDay" type="8">1</Item></roCollection>' WHERE ID = 1
GO
UPDATE sysroLanguages SET Parameters = '<?xml version="1.0"?><roCollection version="2.0"><Item key="ExtLanguage" type="8">en</Item><Item key="ExtDatePickerFormat" type="8">n/j/Y</Item><Item key="ExtDatePickerStartDay" type="8">0</Item></roCollection>' WHERE ID = 2
GO


-- *************************
CREATE TABLE dbo.sysroPassports_Sessions
	(
	ID nvarchar(50) NOT NULL,
	IDPassport int NOT NULL,
	StartDate datetime NOT NULL,
	UpdateDate datetime NOT NULL,
	ClientLocation nvarchar(50) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.sysroPassports_Sessions ADD CONSTRAINT
	PK_sysroPassport_Sessions PRIMARY KEY CLUSTERED 
	(
	ID
	) ON [PRIMARY]

GO
ALTER TABLE dbo.sysroPassports_Sessions ADD CONSTRAINT
	FK_sysroPassport_Sessions_sysroPassport_Sessions FOREIGN KEY
	(
	ID
	) REFERENCES dbo.sysroPassports_Sessions
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
-- *************************
ALTER TABLE dbo.sysroUserFields ADD
	Category nvarchar(50) NULL,
	ListValues text NULL
GO
-- *************************
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEmployeeGroupParent]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetEmployeeGroupParent]

GO

 CREATE FUNCTION [dbo].[GetEmployeeGroupParent] 
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
 		DECLARE @Path nvarchar(2000)
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
-- *************************

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEmployeeGroup]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetEmployeeGroup]

GO

 CREATE FUNCTION [dbo].[GetEmployeeGroup] 
 	(
 		@idEmployee int,
 		@date smalldatetime
 	)
 RETURNS int
 AS
 	BEGIN
 		
 		DECLARE @Result int
 	
		SELECT @date = CONVERT(smalldatetime, CONVERT(varchar, @date, 112))

 		SELECT @Result = IDGroup
 		FROM EmployeeGroups
 		WHERE IDEmployee = @idEmployee AND
 			@date >= BeginDate AND @date <= EndDate
			
 	RETURN @Result
 	END

GO

-- *************************

DELETE FROM sysroGUI WHERE IDPath LIKE 'Portal%'
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,RequiredFunctionalities) VALUES ('Portal',null,null,null,1,null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Attendance','Attendance',null,'Presencia.ico',1001,'NWR',null,null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Attendance\Causes','Causes','Causes/Causes.aspx','Causes.png',660,'NWR','U:Causes.Definition=Read','Forms\Causes')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Attendance\Accruals','Accruals','Concepts/Concepts.aspx','Concepts.png',650,'NWR','U:Concepts.Definition=Read','Forms\Concepts')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Attendance\Employees','InfoEmployees','Employees/Employees.aspx','Employees.png',600,'NWR','U:Employees=Read','Forms\Employees')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Attendance\Scheduler','Scheduler','Scheduler/Scheduler.aspx','Scheduler.png',610,'NWR','U:Calendar=Read','Forms\Calendar')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Attendance\Shifts','Shifts','Shifts/Shifts.aspx','Shifts.png',640,'NWR','U:Shifts.Definition=Read','Forms\Shifts')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Configuration','Configuration',null,'Administracion.ico',1004,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Configuration\AttendanceOptions','AttendanceOptions','Options/AttendanceOptions.aspx','AttendanceOptions.png',120,'NWR','U:Administration.Options.Attendance=Read OR U:Employees.UserFields.Definition=Read','Forms\Options')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Configuration\Options','ConfigurationOptions','Options/ConfigurationOptions.aspx','Options.png',110,'NWR','U:Administration.Options.General=Read','Forms\Options')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities) VALUES ('Portal\Security','Security',null,'Security.png',1005,'NWR',null)
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Security\Audit','Audit','Audit/Audit.aspx','Audit.png',130,'NWR','U:Administration.Audit=Read','Forms\Audit')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Security\Passports','Passports','Security/Passports.aspx','Passports.png',130,'NWR','U:Administration.Security=Read','Forms\Passports')
GO

-- *************************

DELETE FROM sysroFeatures
GO

INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (1,NULL,'Employees','Empleados','','U','RWA',1)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (2,NULL,'Calendar','Calendario','','U','RWA',1)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (3,NULL,'Causes','Justificaciones','','U','RWA',0)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (4,NULL,'Shifts','Horarios','','U','RWA',0)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (5,NULL,'Concepts','Acumulados','','U','RWA',0)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (7,NULL,'Administration','Administración','','U','RWA',0)
GO

INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (1000,1,'Employees.Type','Tipo','','U','RW',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (1100,1,'Employees.NameFoto','Nombre y foto','','U','RW',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (1200,1,'Employees.Contract','Contrato','','U','RWA',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (1300,1,'Employees.GroupMobility','Grupo y mobilidad','','U','RW',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (1400,1,'Employees.IdentifyMethods','Métodos de identificación','','U','RW',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (1500,1,'Employees.UserFields','Ficha','','U','RWA',null)
GO

INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (1510,1500,'Employees.UserFields.Definition','Definición campos de la ficha','','U','RWA',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (1520,1500,'Employees.UserFields.Information','Información','','U','RW',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (1530,1520,'Employees.UserFields.Information.Low','Nivel bajo','','U','RW',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (1540,1520,'Employees.UserFields.Information.Medium','Nivel medio','','U','RW',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (1550,1520,'Employees.UserFields.Information.High','Nivel alto','','U','RW',null)
GO

INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (2000,2,'Calendar.Scheduler','Planificación','','U','RW',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (2100,2,'Calendar.Highlight','Resaltes','','U','RW',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (2200,2,'Calendar.JustifyIncidences','Justificar incidencias','','U','RW',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (2300,2,'Calendar.Punches','Fichajes','','U','RWA',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (2400,2,'Calendar.DirectCauses','Justificaciones directas','','U','RWA',null)
GO

INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (3000,3,'Causes.Definition','Definición','','U','RWA',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (3100,3,'Causes.DirectCauses','Asignación a empleados y reglas','','U','R',null)
GO

INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (4000,4,'Shifts.Definition','Definición','','U','RWA',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (4100,4,'Shifts.EmployeesAssign','Asignación a empleados','','U','R',null)
GO

INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (5000,5,'Concepts.Definition','Definición','','U','RWA',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (5100,5,'Concepts.EmployeeAssign','Asignación a empleados','','U','R',null)
GO

INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (7000,7,'Administration.Security','Seguridad','','U','RWA',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (7100,7,'Administration.Options','Opciones','','U','RWA',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (7200,7,'Administration.Audit','Auditoría','','U','R',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (7300,7,'Administration.UserTasks','Tareas de usuario','','U','RW',null)
GO

INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (7110,7100,'Administration.Options.General','Opciones generales','','U','RW',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (7120,7100,'Administration.Options.Attendance','Opciones de presencia','','U','RWA',null)
GO

-- *************************

INSERT INTO [sysroPassports] ([IDParentPassport],[GroupType],[Name],[Description],[Email],[IDUser],[IDEmployee],[IDLanguage],[LevelOfAuthority])
VALUES(NULL,'U','Administradores','Administradores','',NULL,NULL,0,5)
DECLARE @ID int
SET @ID = @@IDENTITY

UPDATE sysroPassports SET IDParentPassport=@ID WHERE ID <> @ID AND IDParentPassport IS NULL

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT @ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM sysroFeatures

INSERT INTO sysroPassports_PermissionsOverGroups (IDPassport, IDGroup, IDApplication, Permission)
SELECT @ID, Groups.ID, sysroFeatures.ID, 6
FROM sysroFeatures, Groups
WHERE sysroFeatures.AppHasPermissionsOverEmployees = 1 AND Groups.Path = CONVERT(varchar, Groups.ID)

GO

-- *************************
IF (OBJECT_ID(N'[dbo].[PK_sysroPassports_AuthenticationMethods]') IS NULL)

ALTER TABLE dbo.sysroPassports_AuthenticationMethods ADD CONSTRAINT
	PK_sysroPassports_AuthenticationMethods PRIMARY KEY CLUSTERED 
	(
	IDPassport,
	Method,
	Version,
	BiometricID
	)  ON [PRIMARY]

GO

-- *************************

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UsersAdmin_Features_List]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UsersAdmin_Features_List]

GO

 CREATE PROCEDURE [dbo].[UsersAdmin_Features_List]
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
 		PermissionTypes,
		[Description],
		AppHasPermissionsOverEmployees
 	FROM sysroFeatures f
 	WHERE Type = '' OR 
 		Type = @featureType
 	ORDER BY Name
 	
 	RETURN

GO

-- *************************

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='218' WHERE ID='DBVersion'
GO
