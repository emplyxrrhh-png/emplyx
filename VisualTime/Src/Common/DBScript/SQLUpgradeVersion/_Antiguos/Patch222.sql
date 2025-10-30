CREATE TABLE dbo.EmployeeUserFieldValues
	(
	IDEmployee int NOT NULL,
	FieldName nvarchar(50) NOT NULL,
	Date smalldatetime NOT NULL,
	Value text NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.EmployeeUserFieldValues ADD CONSTRAINT
	PK_EmployeeUserFieldValues PRIMARY KEY CLUSTERED 
	(
	IDEmployee,
	FieldName,
	Date
	) ON [PRIMARY]

GO
ALTER TABLE dbo.EmployeeUserFieldValues ADD CONSTRAINT
	FK_EmployeeUserFieldValues_Employees FOREIGN KEY
	(
	IDEmployee
	) REFERENCES dbo.Employees
	(
	ID
	) 
GO

ALTER TABLE [dbo].[EmployeeContracts] ADD
	IDLabAgree int NULL
GO

ALTER TABLE [dbo].[EmployeeContracts]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeContracts_LabAgree] FOREIGN KEY([IDLabAgree])
REFERENCES [dbo].[LabAgree] ([ID])
GO
ALTER TABLE [dbo].[EmployeeContracts] CHECK CONSTRAINT [FK_EmployeeContracts_LabAgree]
GO

/* ***************************************************************************************************************************** */

ALTER TABLE dbo.sysroUserFields ADD
	History bit NULL
GO

/* ***************************************************************************************************************************** */

CREATE TABLE [dbo].[LabAgreeStartupValues](
	[IDLabAgree] [int] NOT NULL,
	[IDConcept] [smallint] NOT NULL,
 CONSTRAINT [PK_LabAgreeStartupLimits] PRIMARY KEY CLUSTERED 
(
	[IDLabAgree] ASC,
	[IDConcept] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[LabAgreeAccrualsRules](
	[IDLabAgree] [int] NOT NULL,
	[IdAccrualsRules] [smallint] NOT NULL,
	[BeginDate] [smalldatetime] NOT NULL,
	[EndDate] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_LabAgreeAccrualsRules] PRIMARY KEY CLUSTERED 
(
	[IDLabAgree] ASC,
	[IdAccrualsRules] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[LabAgree](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](64) NOT NULL,
	[Description] [text] NULL,
 CONSTRAINT [PK_LabAgree] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[StartupValues](
	[IDConcept] [smallint] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[StartValueType] [int] NULL CONSTRAINT [DF_StartupLimits_StartValueType]  DEFAULT ((0)),
	[StartValue] [nvarchar](50) NULL,
	[MaximumValueType] [int] NULL CONSTRAINT [DF_StartupLimits_AlertValueType]  DEFAULT ((0)),
	[MaximumValue] [nvarchar](50) NULL,
	[MinimumValueType] [int] NULL CONSTRAINT [DF_StartupLimits_MinimumValueType]  DEFAULT ((0)),
	[MinimumValue] [nvarchar](50) NULL,
 CONSTRAINT [PK_StartupValues] PRIMARY KEY CLUSTERED 
(
	[IDConcept] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[StartupValues]  WITH CHECK ADD  CONSTRAINT [FK_StartupValues_Concepts] FOREIGN KEY([IDConcept])
REFERENCES [dbo].[Concepts] ([ID])
GO
ALTER TABLE [dbo].[StartupValues] CHECK CONSTRAINT [FK_StartupValues_Concepts]
GO

CREATE TABLE dbo.Tmp_AccrualsRules
	(
	IdAccrualsRule smallint NOT NULL,
	Name nvarchar(50) NULL,
	Description text NULL,
	Definition text NULL,
	Schedule text NULL,
	BeginDate datetime NULL,
	EndDate datetime NULL,
	Priority smallint NULL,
	ExecuteFromLastExecDay bit NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
IF EXISTS(SELECT * FROM dbo.AccrualsRules)
	 EXEC('INSERT INTO dbo.Tmp_AccrualsRules (IdAccrualsRule, Name, Description, Definition, Schedule, BeginDate, EndDate, Priority, ExecuteFromLastExecDay)
		SELECT IdAccrualsRule, Name, Description, Definition, Schedule, BeginDate, EndDate, Priority, ExecuteFromLastExecDay FROM dbo.AccrualsRules WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.AccrualsRules
GO
EXECUTE sp_rename N'dbo.Tmp_AccrualsRules', N'AccrualsRules', 'OBJECT' 
GO
ALTER TABLE dbo.AccrualsRules ADD CONSTRAINT
	PK_AccrualsRules PRIMARY KEY CLUSTERED 
	(
	IdAccrualsRule
	) ON [PRIMARY]

GO

-- sysroFeatures
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (11,NULL,'LabAgree','Convenios','','U','RWA',1)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (11100,11,'LabAgree.Definition','Definición','','U','RWA',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (11200,11,'LabAgree.EmployeeAssign','Asignación de empleados','','U','R',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (11300,11,'LabAgree.AccrualRules','Reglas de convenios','','U','RWA',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (11310,11300,'LabAgree.AccrualRules.Definition','Definición','','U','RWA',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (11400,11,'LabAgree.StartupValues','Valores Iniciales','','U','RWA',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (11410,11400,'LabAgree.StartupValues.Definition','Definición','','U','RWA',NULL)
GO

/* Damos permisos a la pantallas convenios */
INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT sysroPassports.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM sysroFeatures, sysroPassports		
WHERE sysroFeatures.ID IN (11, 11100, 11200, 11300, 11310, 11400, 11410) AND
	  sysroPassports.IDParentPassport IS NULL AND sysroPassports.GroupType = 'U'
GO

/* Damos permisos a los grupos de empleados para la funcionalidad de convenios*/
INSERT INTO sysroPassports_PermissionsOverGroups (IDPassport, IDGroup, IDApplication, Permission)
SELECT sysroPassports.ID, Groups.ID, sysroFeatures.ID, 3
FROM sysroFeatures, Groups, sysroPassports
WHERE sysroFeatures.ID IN (11) AND sysroFeatures.AppHasPermissionsOverEmployees = 1 AND Groups.Path = CONVERT(varchar, Groups.ID) AND
	  sysroPassports.GroupType = 'U'

GO

/* ***************************************************************************************************************************** */
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllEmployeeAllUserFieldValue]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetAllEmployeeAllUserFieldValue]
GO
CREATE FUNCTION [dbo].[GetAllEmployeeAllUserFieldValue]
(				
	@Date smalldatetime
)
RETURNS @ValueTable table(idEmployee int, FieldName nvarchar(50), [value] varchar(4000), [Date] smalldatetime) 
AS
BEGIN

	INSERT INTO @ValueTable
	SELECT Employees.ID, sysroUserFields.FieldName,
			(SELECT TOP 1 CONVERT(varchar, [Value])
			 FROM EmployeeUserFieldValues
			 WHERE EmployeeUserFieldValues.IDEmployee = Employees.ID AND 
			  	   EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND
				   EmployeeUserFieldValues.Date <= @Date
			 ORDER BY EmployeeUserFieldValues.Date DESC),
			ISNULL((SELECT TOP 1 [Date]
				    FROM EmployeeUserFieldValues
			        WHERE EmployeeUserFieldValues.IDEmployee = Employees.ID AND 
			  	          EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND
				          EmployeeUserFieldValues.Date <= @Date
			        ORDER BY EmployeeUserFieldValues.Date DESC), CONVERT(smalldatetime, '1900/01/01', 120))
	FROM Employees, sysroUserFields
	WHERE sysroUserFields.Type = 0 AND sysroUserFields.Used = 1


	RETURN

END
GO
/* ***************************************************************************************************************************** */
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllEmployeeUserFieldValue]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetAllEmployeeUserFieldValue]
GO
CREATE FUNCTION [dbo].[GetAllEmployeeUserFieldValue]
(			
	@FieldName nvarchar(50),
	@Date smalldatetime
)
RETURNS @ValueTable table(idEmployee int, [value] varchar(4000), [Date] smalldatetime) 
AS
BEGIN

	INSERT INTO @ValueTable
	SELECT Employees.ID, 
			(SELECT TOP 1 CONVERT(varchar, [Value])
			 FROM EmployeeUserFieldValues
			 WHERE EmployeeUserFieldValues.IDEmployee = Employees.ID AND 
			  	   EmployeeUserFieldValues.FieldName = @FieldName AND
				   EmployeeUserFieldValues.Date <= @Date
			 ORDER BY EmployeeUserFieldValues.Date DESC),
			ISNULL((SELECT TOP 1 [Date]
				    FROM EmployeeUserFieldValues
			        WHERE EmployeeUserFieldValues.IDEmployee = Employees.ID AND 
			  	          EmployeeUserFieldValues.FieldName = @FieldName AND
				          EmployeeUserFieldValues.Date <= @Date
			        ORDER BY EmployeeUserFieldValues.Date DESC), CONVERT(smalldatetime, '1900/01/01', 120))
	FROM Employees, sysroUserFields
	WHERE sysroUserFields.Type = 0 AND sysroUserFields.Used = 1 AND
		  sysroUserFields.FieldName = @FieldName


	RETURN

END
GO
/* ***************************************************************************************************************************** */
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEmployeeAllUserFieldValue]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetEmployeeAllUserFieldValue]
GO
CREATE FUNCTION [dbo].[GetEmployeeAllUserFieldValue]
(		
	@idEmployee int,	
	@Date smalldatetime
)
RETURNS @ValueTable table(FieldName nvarchar(50), [value] varchar(4000), [Date] smalldatetime) 
AS
BEGIN

	INSERT INTO @ValueTable
	SELECT sysroUserFields.FieldName,
		   (SELECT TOP 1 CONVERT(varchar, [Value])
			FROM EmployeeUserFieldValues				
			WHERE EmployeeUserFieldValues.IDEmployee = @idEmployee AND 
				  EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND
				  EmployeeUserFieldValues.Date <= @Date
			ORDER BY EmployeeUserFieldValues.Date DESC),
		   ISNULL((SELECT TOP 1 [Date]
				   FROM EmployeeUserFieldValues				
				   WHERE EmployeeUserFieldValues.IDEmployee = @idEmployee AND 
				         EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND
				         EmployeeUserFieldValues.Date <= @Date
			       ORDER BY EmployeeUserFieldValues.Date DESC), CONVERT(smalldatetime, '1900/01/01', 120))
	FROM sysroUserFields
	WHERE sysroUserFields.Type = 0 AND sysroUserFields.Used = 1 



	RETURN

END
GO
/* ***************************************************************************************************************************** */
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEmployeeUserFieldValue]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetEmployeeUserFieldValue]
GO
CREATE FUNCTION [dbo].[GetEmployeeUserFieldValue]
(		
	@idEmployee int,
	@FieldName nvarchar(50),
	@Date smalldatetime
)
RETURNS @ValueTable table([value] varchar(4000), [Date] smalldatetime) 
AS
BEGIN

	INSERT INTO @ValueTable
	SELECT (SELECT TOP 1 CONVERT(varchar, [Value])
			FROM EmployeeUserFieldValues				
			WHERE EmployeeUserFieldValues.IDEmployee = @idEmployee AND 
				  EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND
				  EmployeeUserFieldValues.Date <= @Date
			ORDER BY EmployeeUserFieldValues.Date DESC),
		   ISNULL((SELECT TOP 1 [Date]
				   FROM EmployeeUserFieldValues				
				   WHERE EmployeeUserFieldValues.IDEmployee = @idEmployee AND 
				         EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND
				         EmployeeUserFieldValues.Date <= @Date
			       ORDER BY EmployeeUserFieldValues.Date DESC), CONVERT(smalldatetime, '1900/01/01', 120))
	FROM sysroUserFields
	WHERE sysroUserFields.Type = 0 AND sysroUserFields.Used = 1 AND
		  sysroUserFields.FieldName = @FieldName



	RETURN

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
 --RETURNS @DocumentsTable table ([Name] nvarchar(50), [Description] nvarchar(4000) NULL, ValidityFrom smalldatetime NULL, ValidityTo smalldatetime NULL, AccessValidation smallint)
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
					
					SELECT @Validity = [Value]
					FROM dbo.GetEmployeeUserFieldValue(@idEmployee, @FieldName, @date)

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

INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\LabAgree','LabAgree',null,'LabAgree.png',2002,'NWR','U:LabAgree=Read','Feature\ConcertRules')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\LabAgree\LabAgree','LabAgree','LabAgree/LabAgree.aspx','LabAgree.png',2010,'NWR','U:LabAgree.Definition=Read','Feature\ConcertRules')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\LabAgree\LabAgreeRules','LabAgreeRules','LabAgree/LabAgreeRules.aspx','LabAgreeRules.png',2020,'NWR','U:LabAgree.AccrualRules=Read','Feature\ConcertRules')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\LabAgree\StartupValues','StartupValues','LabAgree/StartupValues.aspx','StartupValues.png',2030,'NWR','U:LabAgree.StartupValues=Read','Feature\ConcertRules')
GO

UPDATE sysroGUI SET Priority = 1001 WHERE IDPath = 'Portal\Attendance'
GO
UPDATE sysroGUI SET Priority = 1002 WHERE IDPath = 'Portal\LabAgree'
GO
UPDATE sysroGUI SET Priority = 1003 WHERE IDPath = 'Portal\Access'
GO
UPDATE sysroGUI SET Priority = 1004 WHERE IDPath = 'Portal\OHP'
GO
UPDATE sysroGUI SET Priority = 1005 WHERE IDPath = 'Portal\Security'
GO
UPDATE sysroGUI SET Priority = 1006 WHERE IDPath = 'Portal\Configuration'
GO

/* ***************************************************************************************************************************** */
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransformEmployeeUserFieldValues]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TransformEmployeeUserFieldValues]
GO
CREATE PROCEDURE [dbo].[TransformEmployeeUserFieldValues]
AS
BEGIN
		DECLARE @idEmployee int, 
				@FieldName nvarchar(50),
				@FieldType smallint,
				@FieldValue varchar(2000),
				@ValueText varchar(2000), @ValueInt int, @ValueDecimal Numeric(16,6), @ValueDateTime smalldatetime, 
				@ValueDatePeriod nvarchar(21), @ValueTimePeriod nvarchar(11),
				@SQL nvarchar(1000), @ParamDefinition nvarchar(1000)

		DECLARE EmployeeFieldsCursor CURSOR
			FOR SELECT Employees.ID, sysroUserFields.FieldName, sysroUserFields.FieldType
				FROM Employees, sysroUserFields
				WHERE sysroUserFields.Type = 0 AND sysroUserFields.Used = 1 

		OPEN EmployeeFieldsCursor

		FETCH NEXT FROM EmployeeFieldsCursor
		INTO @idEmployee, @FieldName, @FieldType

		WHILE @@FETCH_STATUS = 0
		BEGIN
			
			SET @SQL = N'SELECT @Value = [USR_' + @FieldName + '] FROM Employees WHERE [ID] = @EmployeeID ';

			IF @FieldType = 0 OR @FieldType = 5 --Text o List
				BEGIN
					SET @ParamDefinition = N'@EmployeeID int, @Value varchar(2000) OUTPUT';
					EXECUTE sp_ExecuteSql @SQL, @ParamDefinition, @EmployeeID = @idEmployee, @Value = @FieldValue OUTPUT;
				END

			IF @FieldType = 1 --Numeric
				BEGIN
					SET @ParamDefinition = N'@EmployeeID int, @Value int OUTPUT';
					EXECUTE sp_ExecuteSql @SQL, @ParamDefinition, @EmployeeID = @idEmployee, @Value = @ValueInt OUTPUT;
					SET @FieldValue = CONVERT(varchar, @ValueInt)
				END

			IF @FieldType = 3  --Decimal
				BEGIN
					SET @ParamDefinition = N'@EmployeeID int, @Value Numeric(16,6) OUTPUT';
					EXECUTE sp_ExecuteSql @SQL, @ParamDefinition, @EmployeeID = @idEmployee, @Value = @ValueDecimal OUTPUT;
					SET @FieldValue = CONVERT(varchar, @ValueDecimal)					
				END

			IF @FieldType = 2  --Date
				BEGIN
					SET @ParamDefinition = N'@EmployeeID int, @Value smalldatetime OUTPUT';
					EXECUTE sp_ExecuteSql @SQL, @ParamDefinition, @EmployeeID = @idEmployee, @Value = @ValueDatetime OUTPUT;					
					SET @FieldValue = REPLACE(SUBSTRING(CONVERT(varchar, @ValueDatetime, 120), 0, 11), '-', '/')					
					--SELECT REPLACE(SUBSTRING(CONVERT(varchar, [USR_CampoFecha], 120), 0, 11), '-', '/') FROM Employees WHERE [USR_CampoFecha] IS NULL
				END

			IF @FieldType = 4  --Time
				BEGIN
					SET @ParamDefinition = N'@EmployeeID int, @Value smalldatetime OUTPUT';
					EXECUTE sp_ExecuteSql @SQL, @ParamDefinition, @EmployeeID = @idEmployee, @Value = @ValueDatetime OUTPUT;
					SET @FieldValue = SUBSTRING(CONVERT(varchar, @ValueDatetime, 120), 12, 8)					
				END

			IF @FieldType = 6 --DatePeriod, TimePeriod
				BEGIN
					SET @ParamDefinition = N'@EmployeeID int, @Value nvarchar(21) OUTPUT';
					EXECUTE sp_ExecuteSql @SQL, @ParamDefinition, @EmployeeID = @idEmployee, @Value = @ValueDatePeriod OUTPUT;
					SET @FieldValue = @ValueDatePeriod
				END
				
			IF @FieldType = 7  --TimePeriod
				BEGIN
					SET @ParamDefinition = N'@EmployeeID int, @Value nvarchar(11) OUTPUT';
					EXECUTE sp_ExecuteSql @SQL, @ParamDefinition, @EmployeeID = @idEmployee, @Value = @ValueTimePeriod OUTPUT;
					SET @FieldValue = @ValueTimePeriod
				END
						
			
			INSERT INTO EmployeeUserFieldValues (IDEmployee, FieldName, Date, [Value])
			VALUES (@idEmployee, @FieldName, CONVERT(smalldatetime, '1900/01/01', 120), @FieldValue)

				
			FETCH NEXT FROM EmployeeFieldsCursor 
			INTO @idEmployee, @FieldName, @FieldType

		END 
		CLOSE EmployeeFieldsCursor
		DEALLOCATE EmployeeFieldsCursor

END
GO
EXECUTE [dbo].[TransformEmployeeUserFieldValues]
GO

/* ***************************************************************************************************************************** */

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='222' WHERE ID='DBVersion'
GO
