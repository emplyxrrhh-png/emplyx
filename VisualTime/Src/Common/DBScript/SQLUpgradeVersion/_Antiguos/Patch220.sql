/* ***************************************************************************************************************************** */

ALTER TABLE dbo.sysroUserFields ADD
	[Type] smallint NULL,
	[Description] text NULL
GO

ALTER TABLE dbo.sysroUserFields ADD CONSTRAINT
	DF_sysroUserFields_Type DEFAULT 0 FOR Type
GO

UPDATE sysroUserFields
SET [Type] = 0
WHERE [Type] IS NULL
GO


ALTER TABLE dbo.sysroUserFields
	DROP CONSTRAINT DF_sysroCustomFields_Used
GO
ALTER TABLE dbo.sysroUserFields
	DROP CONSTRAINT DF_sysroUserFields_Type
GO
CREATE TABLE dbo.Tmp_sysroUserFields
	(
	FieldName nvarchar(50) NOT NULL,
	FieldType smallint NULL,
	Used bit NOT NULL,
	AccessLevel smallint NULL,
	Pos smallint NULL,
	Category nvarchar(50) NULL,
	ListValues text NULL,
	Type smallint NOT NULL,
	Description text NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_sysroUserFields ADD CONSTRAINT
	DF_sysroCustomFields_Used DEFAULT (0) FOR Used
GO
ALTER TABLE dbo.Tmp_sysroUserFields ADD CONSTRAINT
	DF_sysroUserFields_Type DEFAULT ((0)) FOR Type
GO
IF EXISTS(SELECT * FROM dbo.sysroUserFields)
	 EXEC('INSERT INTO dbo.Tmp_sysroUserFields (FieldName, FieldType, Used, AccessLevel, Pos, Category, ListValues, Type, Description)
		SELECT FieldName, FieldType, Used, AccessLevel, Pos, Category, ListValues, Type, Description FROM dbo.sysroUserFields WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.sysroUserFields
GO
EXECUTE sp_rename N'dbo.Tmp_sysroUserFields', N'sysroUserFields', 'OBJECT' 
GO
ALTER TABLE dbo.sysroUserFields ADD CONSTRAINT
	PK_sysroCustomFields PRIMARY KEY NONCLUSTERED 
	(
	FieldName,
	Type
	) ON [PRIMARY]

GO

ALTER TABLE dbo.sysroUserFields ADD
	AccessValidation smallint NULL
GO

/* ***************************************************************************************************************************** */

UPDATE sysroFeatures
SET PermissionTypes = 'RWA'
WHERE ID = 7110 AND Alias = 'Administration.Options.General'
GO

UPDATE sysroFeatures
SET PermissionTypes = 'RW'
WHERE ID = 7120 AND Alias = 'Administration.Options.Attendance'
GO

UPDATE sysroPassports_PermissionsOverFeatures
SET Permission = 9
WHERE IDFeature = 7110 AND Permission = 6
GO

UPDATE sysroPassports_PermissionsOverFeatures
SET Permission = 6
WHERE IDFeature = 7120 AND Permission = 9
GO

/* ***************************************************************************************************************************** */

CREATE TABLE dbo.Activities
	(
	ID smallint NOT NULL,
	Name nvarchar(50) NOT NULL,
	Description text NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Activities ADD CONSTRAINT
	PK_Activities PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

CREATE TABLE dbo.ActivityCompanies
	(
	IDActivity smallint NOT NULL,
	IDGroup int NOT NULL,
	IDParent int NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.ActivityCompanies ADD CONSTRAINT
	PK_ActivityCompanies PRIMARY KEY CLUSTERED 
	(
	IDActivity,
	IDGroup
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.ActivityCompanies ADD CONSTRAINT
	FK_ActivityCompanies_Groups FOREIGN KEY
	(
	IDGroup
	) REFERENCES dbo.Groups
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ActivityCompanies ADD CONSTRAINT
	FK_ActivityCompanies_GroupsParent FOREIGN KEY
	(
	IDParent
	) REFERENCES dbo.Groups
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

/* ***************************************************************************************************************************** */

DELETE FROM sysroGUI WHERE IDPath = 'Portal\OHP' OR IDPath LIKE 'Portal\OHP\%'
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\OHP','OHP',NULL,'OHP.png',1002,NULL,NULL,'Feature\OHP')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\OHP\Activities','Activities','Activities/Activities.aspx','Activity.png',670,NULL,'U:Activities.Definition=Read','Forms\Activities')
GO
UPDATE sysroGUI SET RequiredFeatures = 'Forms\Calendar' WHERE IDPath = 'Portal\Configuration\AttendanceOptions'
GO

/* ***************************************************************************************************************************** */

DELETE FROM sysroFeatures WHERE Alias LIKE 'Activities%'
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (10,NULL,'Activities','Actividades','','U','RWA',0)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (10000,10,'Activities.Definition','Definición','','U','RWA',null)
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT sysroPassports.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM sysroFeatures, sysroPassports		
WHERE sysroFeatures.ID IN (10, 10000) AND
	  sysroPassports.IDParentPassport IS NULL AND sysroPassports.GroupType = 'U'
GO

/* ***************************************************************************************************************************** */

ALTER TABLE dbo.TerminalReaders ADD
	IDActivity smallint NULL
GO
ALTER TABLE dbo.TerminalReaders ADD CONSTRAINT
	FK_TerminalReaders_Activities FOREIGN KEY
	(
	IDActivity
	) REFERENCES dbo.Activities
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

/* ***************************************************************************************************************************** */

CREATE TABLE dbo.InvalidAccessMoves
	(
	ID int NOT NULL IDENTITY (1, 1),
	IDEmployee int NOT NULL,
	DateTime datetime NULL,
	IDTerminal tinyint NULL,
	IDReader tinyint NULL,
	Type smallint NOT NULL,
	Detail text NULL,
	IDZone tinyint NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.InvalidAccessMoves ADD CONSTRAINT
	DF_InvalidAccessMoves_DateTime DEFAULT (getdate()) FOR DateTime
GO
ALTER TABLE dbo.InvalidAccessMoves ADD CONSTRAINT
	PK_InvalidAccessMoves PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.InvalidAccessMoves ADD CONSTRAINT
	FK_InvalidAccessMoves_InvalidAccessMoves FOREIGN KEY
	(
	ID
	) REFERENCES dbo.InvalidAccessMoves
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.InvalidAccessMoves ADD CONSTRAINT
	FK_InvalidAccessMoves_Employees FOREIGN KEY
	(
	IDEmployee
	) REFERENCES dbo.Employees
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

/* ***************************************************************************************************************************** */

CREATE TABLE dbo.Captures
	(
	ID int NOT NULL,
	DateTime smalldatetime NOT NULL,
	Capture image NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Captures ADD CONSTRAINT
	PK_Captures PRIMARY KEY CLUSTERED 
	(
	ID
	) ON [PRIMARY]

GO

ALTER TABLE dbo.InvalidAccessMoves ADD
	IDCapture int NULL
GO
ALTER TABLE dbo.InvalidAccessMoves ADD CONSTRAINT
	FK_InvalidAccessMoves_Captures FOREIGN KEY
	(
	IDCapture
	) REFERENCES dbo.Captures
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.AccessMoves ADD
	IDCapture int NULL
GO
ALTER TABLE dbo.AccessMoves ADD CONSTRAINT
	FK_AccessMoves_Captures FOREIGN KEY
	(
	IDCapture
	) REFERENCES dbo.Captures
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

/* ***************************************************************************************************************************** */

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetFullActivityCompanyParentID]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetFullActivityCompanyParentID]
GO

CREATE FUNCTION [dbo].[GetFullActivityCompanyParentID] (@GroupId int, @ActivityId int)
  	RETURNS varchar(300) 
AS
BEGIN
	-- Declaramos las variables
	declare @parentId int;
	declare @parentName varchar(200);
  	declare @fillName varchar(200);
  	declare @resultado varchar(300);
  	-- Seteamos las variables a usar
	set @parentId=0
	set @parentName=''
	set @fillName=''
  	set @resultado=''
  	
  	-- Seleccionamos el Path del grupo que nos manda el usuario
  	SELECT @parentId=ISNULL(IDParent,0) FROM ActivityCompanies WHERE IDGroup=@GroupId AND IDActivity=@ActivityId 
  	
	-- Si la longitud es 0 quiere decir que es una Empresa
	IF (@parentId=0)
	BEGIN
		-- Directamente devolvemos el nombre
		SELECT @resultado=ID FROM Groups WHERE ID=@GroupId
	END
  	ELSE
	BEGIN
		--Primero recogemos el nombre del hijo
		SELECT @fillName=ID FROM Groups WHERE ID=@GroupId
		SELECT @resultado=@fillName

		-- Si tiene valor en el campo Parent nos indica quien es su padre
  		WHILE (@parentId<>0)
  		BEGIN
			-- Buscamos el nombre del padre y lo agregamos a la cadena final
			SELECT @parentName=ID FROM Groups WHERE ID=@parentId
			SELECT @resultado=@parentName+'\'+@resultado

			-- Buscamos el ID del abuelo
			SELECT @parentId=ISNULL(IDParent,0) FROM ActivityCompanies WHERE IDGroup=@parentId AND IDActivity=@ActivityId
		END
	END	
	RETURN (@resultado)
END

GO

/* ***************************************************************************************************************************** */

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetFullActivityCompanyParentName]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetFullActivityCompanyParentName]
GO

CREATE FUNCTION [dbo].[GetFullActivityCompanyParentName] (@GroupId int, @ActivityId int)
  	RETURNS varchar(300) 
AS
BEGIN
	-- Declaramos las variables
	declare @parentId int;
	declare @parentName varchar(200);
  	declare @fillName varchar(200);
  	declare @resultado varchar(300);
  	-- Seteamos las variables a usar
	set @parentId=0
	set @parentName=''
	set @fillName=''
  	set @resultado=''
  	
  	-- Seleccionamos el Path del grupo que nos manda el usuario
  	SELECT @parentId=ISNULL(IDParent,0) FROM ActivityCompanies WHERE IDGroup=@GroupId AND IDActivity=@ActivityId 
  	
	-- Si la longitud es 0 quiere decir que es una Empresa
	IF (@parentId=0)
	BEGIN
		-- Directamente devolvemos el nombre
		SELECT @resultado=Name FROM Groups WHERE ID=@GroupId
	END
  	ELSE
	BEGIN
		--Primero recogemos el nombre del hijo
		SELECT @fillName=Name FROM Groups WHERE ID=@GroupId
		SELECT @resultado=@fillName

		-- Si tiene valor en el campo Parent nos indica quien es su padre
  		WHILE (@parentId<>0)
  		BEGIN
			-- Buscamos el nombre del padre y lo agregamos a la cadena final
			SELECT @parentName=Name FROM Groups WHERE ID=@parentId
			SELECT @resultado=@parentName+'\'+@resultado

			-- Buscamos el ID del abuelo
			SELECT @parentId=ISNULL(IDParent,0) FROM ActivityCompanies WHERE IDGroup=@parentId AND IDActivity=@ActivityId
		END
	END	
	RETURN (@resultado)
END

GO

/* ***************************************************************************************************************************** */

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[sysroActivityCompanies]'))
DROP VIEW [dbo].[sysroActivityCompanies]
GO

CREATE VIEW [dbo].[sysroActivityCompanies]
AS
SELECT Activities.ID IDActivity,Activities.Name ActivityName,Groups.ID CompanyID, Groups.Name GroupName, 
	dbo.GetFullActivityCompanyParentID(dbo.Groups.ID, dbo.Activities.ID) AS Path,
	dbo.GetFullActivityCompanyParentName(dbo.Groups.ID, dbo.Activities.ID) AS FullParentName
FROM Activities
	INNER JOIN ActivityCompanies ON ActivityCompanies.IDActivity = Activities.ID
	INNER JOIN Groups ON Groups.ID = ActivityCompanies.IDGroup
GO

/* ***************************************************************************************************************************** */

ALTER VIEW [dbo].[sysroEmployeeGroups]
 AS
 SELECT     dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, 
                       dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName,
					   (SELECT CASE LEFT(Path,CHARINDEX('\',Path)) WHEN '' THEN Path ELSE CONVERT(INT,LEFT(Path,CHARINDEX('\',Path)-1)) END FROM Groups WHERE ID = (dbo.Groups.ID)) AS IDCompany
 FROM         dbo.Groups INNER JOIN
                       dbo.EmployeeGroups ON dbo.Groups.ID = dbo.EmployeeGroups.IDGroup
GO

/* ***************************************************************************************************************************** */

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ActivityDocuments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ActivityDocuments]
GO

CREATE PROCEDURE [dbo].[ActivityDocuments]
 	(
 		@idActivity smallint,
		@date smalldatetime,
		@OnlyExpired bit
 	)
 AS
 	BEGIN

		CREATE TABLE #ActivityDocumentsTable
		(
			[IDEmployee] int NOT NULL,
			[Type] smallint NOT NULL,
			[Name] nvarchar(50) NOT NULL,
			[Description] nvarchar(4000) NULL, 
			ValidityFrom smalldatetime NULL, 
			ValidityTo smalldatetime NULL, 
			AccessValidation smallint NOT NULL,
			Expired bit NOT NULL,
			IDCompany int NULL
		)

		CREATE TABLE #DocumentsTable
		(			
			[Name] nvarchar(50) NOT NULL,
			[Description] nvarchar(4000) NULL, 
			ValidityFrom smalldatetime NULL, 
			ValidityTo smalldatetime NULL, 
			AccessValidation smallint NOT NULL,
			Expired bit NOT NULL
		)


		DECLARE @idEmployee int, @idCompany int, @Path nvarchar(1000), @idSubCompany int

		DECLARE EmployeesCursor CURSOR
			FOR SELECT sysroEmployeeGroups.IDEmployee, sysroEmployeeGroups.IDCompany
				FROM sysroEmployeeGroups INNER JOIN Employees
						ON sysroEmployeeGroups.IDEmployee = Employees.ID
						INNER JOIN ActivityCompanies ON sysroEmployeeGroups.IDCompany = ActivityCompanies.IDGroup
				WHERE sysroEmployeeGroups.BeginDate <= @date AND sysroEmployeeGroups.EndDate >= @date AND
					  Employees.RiskControlled = 1 AND
					  ActivityCompanies.IDActivity = @idActivity


		OPEN EmployeesCursor

		FETCH NEXT FROM EmployeesCursor
		INTO @idEmployee, @idCompany

		WHILE @@FETCH_STATUS = 0
		BEGIN

			-- Obtenemos los documentos del empleado
			DELETE FROM #DocumentsTable

			INSERT INTO #DocumentsTable
			EXEC dbo.EmployeeDocuments @idEmployee, @date

			INSERT INTO #ActivityDocumentsTable
			SELECT @idEmployee, 0, #DocumentsTable.*, NULL
			FROM #DocumentsTable
			WHERE (@OnlyExpired = 0 OR #DocumentsTable.Expired = 1)


			SELECT @Path = sysroActivityCompanies.Path
			FROM sysroActivityCompanies
			WHERE IDActivity = @idActivity AND
				  CompanyID = @idCompany

			/*
			-- Obtenemos los documentos de la empresa del empleado
			DELETE FROM #DocumentsTable

			INSERT INTO #DocumentsTable
			EXEC dbo.CompanyDocuments @idCompany, @date

			INSERT INTO #ActivityDocumentsTable
			SELECT @idEmployee, 1, #DocumentsTable.*, @idCompany
			FROM #DocumentsTable
			WHERE (@OnlyExpired = 0 OR #DocumentsTable.Expired = 1)
			*/

			-- recorremos las subempresas (en función de la actividad)
			DECLARE CompaniesCursor CURSOR
					FOR		SELECT [Value]
							FROM dbo.SplitInt(@Path, '\')

			OPEN CompaniesCursor

			FETCH NEXT FROM CompaniesCursor
			INTO @idSubCompany

			WHILE @@FETCH_STATUS = 0
			BEGIN

				DELETE FROM #DocumentsTable

				INSERT INTO #DocumentsTable
				EXEC dbo.CompanyDocuments @idSubCompany, @date

				INSERT INTO #ActivityDocumentsTable
				SELECT @idEmployee, 1, #DocumentsTable.*, @idSubCompany
				FROM #DocumentsTable
				WHERE (@OnlyExpired = 0 OR #DocumentsTable.Expired = 1)


				FETCH NEXT FROM CompaniesCursor 
				INTO @idSubCompany

			END
			CLOSE CompaniesCursor
			DEALLOCATE CompaniesCursor

							
			FETCH NEXT FROM EmployeesCursor 
			INTO @idEmployee, @idCompany

		END 
		CLOSE EmployeesCursor
		DEALLOCATE EmployeesCursor

		SELECT * FROM #ActivityDocumentsTable

 	END
GO

/* ***************************************************************************************************************************** */

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CompanyDocuments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CompanyDocuments]
GO

 CREATE PROCEDURE [dbo].[CompanyDocuments]
 	(
 		@idCompany int,
		@date smalldatetime
 	)
 AS
 	BEGIN

		SELECT @date = CONVERT(smalldatetime, CONVERT(varchar, @date, 112))

		CREATE TABLE #DocumentsTable
		(
			[Name] nvarchar(50) NOT NULL,
			[Description] nvarchar(4000) NULL, 
			ValidityFrom smalldatetime NULL, 
			ValidityTo smalldatetime NULL, 
			AccessValidation smallint NOT NULL,
			Expired bit NOT NULL
		)

		-- Miramos que la empresa este definida
		IF (SELECT COUNT(*) FROM Groups WHERE [ID] = @idCompany) = 1
			BEGIN

				DECLARE @FieldName nvarchar(50), 
						@Description nvarchar(4000),
						@AccessValidation smallint,
						@Validity nvarchar(21),
						@ValidityFrom smalldatetime,
						@ValidityTo smalldatetime,
						@Expired bit,
						@SQL nvarchar(1000), @ParamDefinition nvarchar(1000)

				DECLARE GroupFieldsCursor CURSOR
					FOR SELECT sysroUserFields.FieldName, sysroUserFields.Description, sysroUserFields.AccessValidation
						FROM sysroUserFields
						WHERE [Type] = 1 AND Used = 1 AND 
							  AccessValidation > 0
						ORDER BY FieldName	

				OPEN GroupFieldsCursor

				FETCH NEXT FROM GroupFieldsCursor
				INTO @FieldName, @Description, @AccessValidation

				WHILE @@FETCH_STATUS = 0
				BEGIN
					
					SET @SQL = N'SELECT @Value = [USR_' + @FieldName + '] FROM Groups WHERE [ID] = @GroupID ';
					SET @ParamDefinition = N'@GroupID int, @Value nvarchar(21) OUTPUT';

					EXECUTE sp_ExecuteSql @SQL, @ParamDefinition, @GroupID = @idCompany, @Value = @Validity OUTPUT;
					
					SET @ValidityFrom = NULL;
					SET @ValidityTo = NULL;
					SET @Expired = 0;

					IF ISNULL(@Validity,'') <> ''
						BEGIN
							
							DECLARE @Value nvarchar(4000)

							SELECT TOP 1 @Value = [Value] FROM dbo.Split(@Validity, '*')
							IF @Value <> ''
								SELECT @ValidityFrom = CONVERT(smalldatetime, @Value, 120)
							
							SELECT TOP 2 @Value = [Value] FROM dbo.Split(@Validity, '*')
							IF @Value <> ''
								SELECT @ValidityTo = CONVERT(smalldatetime, @Value, 120)

						END

					-- Miramos si el documento está caducado
					IF @ValidityFrom IS NULL AND @ValidityTo IS NULL
						BEGIN
							SET @Expired = 1
						END
					ELSE
						BEGIN
							IF NOT @ValidityFrom IS NULL
								BEGIN
									IF @ValidityFrom > @date
										SELECT @Expired = 0
								END
							IF NOT @ValidityTo IS NULL
								BEGIN
									IF @ValidityTo < @date
										SELECT @Expired = 1							
								END
						END

					INSERT INTO #DocumentsTable
					SELECT @FieldName, @Description, @ValidityFrom, @ValidityTo, @AccessValidation, @Expired

						
					FETCH NEXT FROM GroupFieldsCursor 
					INTO @FieldName, @Description, @AccessValidation

				END 
				CLOSE GroupFieldsCursor
				DEALLOCATE GroupFieldsCursor

			END

		SELECT * FROM #DocumentsTable

 	END

GO

/* ***************************************************************************************************************************** */

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployeeDocuments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[EmployeeDocuments]
GO

 CREATE PROCEDURE [dbo].[EmployeeDocuments]
 	(
 		@idEmployee int,
		@date smalldatetime
 	)
 AS
 	BEGIN

		SELECT @date = CONVERT(smalldatetime, CONVERT(varchar, @date, 112))

		CREATE TABLE #DocumentsTable
		(
			[Name] nvarchar(50) NOT NULL,
			[Description] nvarchar(4000) NULL, 
			ValidityFrom smalldatetime NULL, 
			ValidityTo smalldatetime NULL, 
			AccessValidation smallint NOT NULL,
			Expired bit NOT NULL
		)

		-- Miramos que el empleado este definido
		IF (SELECT COUNT(*) FROM Employees WHERE [ID] = @idEmployee) = 1
			BEGIN

				DECLARE @FieldName nvarchar(50), 
						@Description nvarchar(4000),
						@AccessValidation smallint,
						@Validity nvarchar(21),
						@ValidityFrom smalldatetime,
						@ValidityTo smalldatetime,
						@Expired bit,
						@SQL nvarchar(1000), @ParamDefinition nvarchar(1000)

				DECLARE EmployeeFieldsCursor CURSOR
					FOR SELECT sysroUserFields.FieldName, sysroUserFields.Description, sysroUserFields.AccessValidation
						FROM sysroUserFields
						WHERE [Type] = 0 AND Used = 1 AND 
							  AccessValidation > 0
						ORDER BY FieldName	

				OPEN EmployeeFieldsCursor

				FETCH NEXT FROM EmployeeFieldsCursor
				INTO @FieldName, @Description, @AccessValidation

				WHILE @@FETCH_STATUS = 0
				BEGIN
					
					SET @SQL = N'SELECT @Value = [USR_' + @FieldName + '] FROM Employees WHERE [ID] = @EmployeeID ';
					SET @ParamDefinition = N'@EmployeeID int, @Value nvarchar(21) OUTPUT';

					EXECUTE sp_ExecuteSql @SQL, @ParamDefinition, @EmployeeID = @idEmployee, @Value = @Validity OUTPUT;
					
					SET @ValidityFrom = NULL;
					SET @ValidityTo = NULL;
					SET @Expired = 0;

					IF ISNULL(@Validity,'') <> ''
						BEGIN
							
							DECLARE @Value nvarchar(4000)

							SELECT TOP 1 @Value = [Value] FROM dbo.Split(@Validity, '*')
							IF @Value <> ''
								SELECT @ValidityFrom = CONVERT(smalldatetime, @Value, 120)
							
							SELECT TOP 2 @Value = [Value] FROM dbo.Split(@Validity, '*')
							IF @Value <> ''
								SELECT @ValidityTo = CONVERT(smalldatetime, @Value, 120)

						END

					-- Miramos si el documento está caducado
					IF @ValidityFrom IS NULL AND @ValidityTo IS NULL
						BEGIN
							SET @Expired = 1
						END
					ELSE
						BEGIN
							IF NOT @ValidityFrom IS NULL
								BEGIN
									IF @ValidityFrom > @date
										SET @Expired = 0							
								END
							IF NOT @ValidityTo IS NULL
								BEGIN
									IF @ValidityTo < @date
										SET @Expired = 1
								END
						END


					INSERT INTO #DocumentsTable
					SELECT @FieldName, @Description, @ValidityFrom, @ValidityTo, @AccessValidation, @Expired

						
					FETCH NEXT FROM EmployeeFieldsCursor 
					INTO @FieldName, @Description, @AccessValidation

				END 
				CLOSE EmployeeFieldsCursor
				DEALLOCATE EmployeeFieldsCursor

			END

		SELECT * FROM #DocumentsTable

 	END

GO
/* ***************************************************************************************************************************** */
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TerminalDocuments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TerminalDocuments]
GO

 CREATE PROCEDURE [dbo].[TerminalDocuments]
 	(
 		@idTerminal smallint,
		@date smalldatetime,
		@OnlyExpired bit
 	)
 AS
 	BEGIN

		CREATE TABLE #TerminalDocumentsTable
		(
			[IDReader] tinyint NOT NULL,
			[IDActivity] smallint NOT NULL,
			[IDEmployee] int NOT NULL,
			[Type] smallint NOT NULL,
			[Name] nvarchar(50) NOT NULL,
			[Description] nvarchar(4000) NULL, 
			ValidityFrom smalldatetime NULL, 
			ValidityTo smalldatetime NULL, 
			AccessValidation smallint NOT NULL,
			Expired bit NOT NULL,
			IDCompany int NULL,
			CompanyName nvarchar(64) NULL
		)

		CREATE TABLE #DocumentsTable
		(			
			[Name] nvarchar(50) NOT NULL,
			[Description] nvarchar(4000) NULL, 
			ValidityFrom smalldatetime NULL, 
			ValidityTo smalldatetime NULL, 
			AccessValidation smallint NOT NULL,
			Expired bit NOT NULL
		)


		DECLARE @idEmployee int, @idReader tinyint, @idActivity smallint, @idCompany int, @Path nvarchar(1000), @idSubCompany int

		DECLARE EmployeesCursor CURSOR
			FOR SELECT sysroEmployeeGroups.IDEmployee, ActivityCompanies.IDActivity, sysroEmployeeGroups.IDCompany, TerminalReaders.ID
				FROM sysroEmployeeGroups INNER JOIN Employees
						ON sysroEmployeeGroups.IDEmployee = Employees.ID
						INNER JOIN ActivityCompanies ON sysroEmployeeGroups.IDCompany = ActivityCompanies.IDGroup
						INNER JOIN TerminalReaders ON ActivityCompanies.IDActivity = TerminalReaders.IDActivity
				WHERE sysroEmployeeGroups.BeginDate <= @date AND sysroEmployeeGroups.EndDate >= @date AND
					  Employees.RiskControlled = 1 AND
					  TerminalReaders.IDTerminal = @idTerminal


		OPEN EmployeesCursor

		FETCH NEXT FROM EmployeesCursor
		INTO @idEmployee, @idActivity, @idCompany, @idReader

		WHILE @@FETCH_STATUS = 0
		BEGIN

			-- Obtenemos los documentos del empleado
			DELETE FROM #DocumentsTable

			INSERT INTO #DocumentsTable
			EXEC dbo.EmployeeDocuments @idEmployee, @date

			INSERT INTO #TerminalDocumentsTable
			SELECT @idReader, @idActivity, @idEmployee, 0, #DocumentsTable.*, NULL, NULL
			FROM #DocumentsTable
			WHERE (@OnlyExpired = 0 OR #DocumentsTable.Expired = 1)


			SELECT @Path = sysroActivityCompanies.Path
			FROM sysroActivityCompanies
			WHERE IDActivity = @idActivity AND
				  CompanyID = @idCompany

			-- recorremos las subempresas (en función de la actividad)
			DECLARE CompaniesCursor CURSOR
					FOR		SELECT [Value]
							FROM dbo.SplitInt(@Path, '\')

			OPEN CompaniesCursor

			FETCH NEXT FROM CompaniesCursor
			INTO @idSubCompany

			WHILE @@FETCH_STATUS = 0
			BEGIN

				DELETE FROM #DocumentsTable

				INSERT INTO #DocumentsTable
				EXEC dbo.CompanyDocuments @idSubCompany, @date

				INSERT INTO #TerminalDocumentsTable
				SELECT @idReader, @idActivity, @idEmployee, 1, #DocumentsTable.*, @idSubCompany, Groups.Name
				FROM #DocumentsTable, Groups
				WHERE (@OnlyExpired = 0 OR #DocumentsTable.Expired = 1) AND
					  Groups.ID = @idSubCompany


				FETCH NEXT FROM CompaniesCursor 
				INTO @idSubCompany

			END
			CLOSE CompaniesCursor
			DEALLOCATE CompaniesCursor

							
			FETCH NEXT FROM EmployeesCursor 
			INTO @idEmployee, @idActivity, @idCompany, @idReader

		END 
		CLOSE EmployeesCursor
		DEALLOCATE EmployeesCursor

		SELECT * FROM #TerminalDocumentsTable

 	END

GO

/* ***************************************************************************************************************************** */

INSERT INTO [sysroLanguages] ([ID],[LanguageKey],[Culture],[Parameters]) VALUES (3, 'GAL', 'gl-ES', '<?xml version="1.0"?><roCollection version="2.0"><Item key="ExtLanguage" type="8">sp</Item><Item key="ExtDatePickerFormat" type="8">d/m/Y</Item><Item key="ExtDatePickerStartDay" type="8">1</Item></roCollection>')
GO
INSERT INTO [sysroLanguages] ([ID],[LanguageKey],[Culture],[Parameters]) VALUES (4, 'EKR', 'eu-ES', '<?xml version="1.0"?><roCollection version="2.0"><Item key="ExtLanguage" type="8">sp</Item><Item key="ExtDatePickerFormat" type="8">d/m/Y</Item><Item key="ExtDatePickerStartDay" type="8">1</Item></roCollection>')
GO
INSERT INTO [sysroLanguages] ([ID],[LanguageKey],[Culture],[Parameters]) VALUES (5, 'ITA', 'it-IT', '<?xml version="1.0"?><roCollection version="2.0"><Item key="ExtLanguage" type="8">it</Item><Item key="ExtDatePickerFormat" type="8">d/m/Y</Item><Item key="ExtDatePickerStartDay" type="8">1</Item></roCollection>')
GO
INSERT INTO [sysroLanguages] ([ID],[LanguageKey],[Culture],[Parameters]) VALUES (6, 'FRA', 'fr-FR', '<?xml version="1.0"?><roCollection version="2.0"><Item key="ExtLanguage" type="8">fr</Item><Item key="ExtDatePickerFormat" type="8">d/m/Y</Item><Item key="ExtDatePickerStartDay" type="8">1</Item></roCollection>')
GO

/* ***************************************************************************************************************************** */

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[sysroInvalidAccessMoves]'))
DROP VIEW [dbo].[sysroInvalidAccessMoves]
GO

CREATE VIEW [dbo].[sysroInvalidAccessMoves]
AS
	SELECT ID,IDEmployee,DateTime,IDTerminal, IDReader, Type, CONVERT(VARCHAR(1000),Detail) Detail, 
		IDZone,IDCapture
	FROM InvalidAccessMoves
GO

/* ***************************************************************************************************************************** */

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='220' WHERE ID='DBVersion'
GO
