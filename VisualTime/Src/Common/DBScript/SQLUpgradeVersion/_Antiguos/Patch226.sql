-- *************************
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UsersAdmin_Features_List]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
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

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetEmployeeGroupParent]') AND xtype in (N'FN', N'IF', N'TF'))
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

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetEmployeeGroup]') AND xtype in (N'FN', N'IF', N'TF'))
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

		IF @Result IS NULL 
			BEGIN

				SELECT TOP 1 @Result = IDGroup
				FROM EmployeeGroups
				WHERE IDEmployee = @idEmployee AND 
					  BeginDate > @date
				ORDER BY BeginDate

			END
 			
 	RETURN @Result
 	END

GO

-- *************************

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetFullActivityCompanyParentID]') AND xtype in (N'FN', N'IF', N'TF'))
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

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetFullActivityCompanyParentName]') AND xtype in (N'FN', N'IF', N'TF'))
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

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[sysroActivityCompanies]') AND OBJECTPROPERTY(id, N'IsView') = 1)
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

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[ActivityDocuments]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
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

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[TerminalDocuments]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
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

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[sysroInvalidAccessMoves]') AND OBJECTPROPERTY(id, N'IsView') = 1)
DROP VIEW [dbo].[sysroInvalidAccessMoves]
GO

CREATE VIEW [dbo].[sysroInvalidAccessMoves]
AS
	SELECT ID,IDEmployee,DateTime,IDTerminal, IDReader, Type, CONVERT(VARCHAR(1000),Detail) Detail, 
		IDZone,IDCapture
	FROM InvalidAccessMoves
GO

/* ***************************************************************************************************************************** */

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetAllEmployeeAllUserFieldValue]') AND xtype in (N'FN', N'IF', N'TF'))
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
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetAllEmployeeUserFieldValue]') AND xtype in (N'FN', N'IF', N'TF'))
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
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetEmployeeAllUserFieldValue]') AND xtype in (N'FN', N'IF', N'TF'))
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
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetEmployeeUserFieldValue]') AND xtype in (N'FN', N'IF', N'TF'))
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

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[CompanyDocuments]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
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
							ORDER BY CASE [Value] WHEN '' THEN '2079/01/01' ELSE [Value] END ASC
 							IF @Value <> ''
 								SELECT @ValidityFrom = CONVERT(smalldatetime, @Value, 120)								
 							
 							SELECT TOP 1 @Value = [Value] FROM dbo.Split(@Validity, '*')
							ORDER BY CASE [Value] WHEN '' THEN '2079/01/01' ELSE [Value] END DESC
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

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[EmployeeDocuments]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
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
 						@SQL nvarchar(1000), @ParamDefinition nvarchar(1000),
						@IsDate bit
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
							ORDER BY CASE [Value] WHEN '' THEN '2079/01/01' ELSE [Value] END ASC
 							IF @Value <> ''

								SELECT @ValidityFrom = CONVERT(smalldatetime, @Value, 120)								
 							
 							SELECT TOP 1 @Value = [Value] FROM dbo.Split(@Validity, '*')
							ORDER BY CASE [Value] WHEN '' THEN '2079/01/01' ELSE [Value] END DESC
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

-- Actualizamos los permisos de la sysroGui para que se puedan administrar los accesos y la pantalla del enlace de datos
UPDATE sysroGui SET AllowedSecurity=AllowedSecurity+'A' WHERE IDPath LIKE '%NavBar\Access%' AND LanguageReference NOT LIKE 'AccessStatus' AND CharIndex('A',AllowedSecurity)=0
GO
UPDATE sysroGui SET AllowedSecurity=AllowedSecurity+'A' WHERE IDPath LIKE '%DataLink%' AND CharIndex('A',AllowedSecurity)=0
GO
/* ***************************************************************************************************************************** */


/* ***************************************************************************************************************************** */
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='226' WHERE ID='DBVersion'
GO
