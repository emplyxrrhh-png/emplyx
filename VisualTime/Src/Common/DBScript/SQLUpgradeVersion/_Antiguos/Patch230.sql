IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[CompanyDocuments]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CompanyDocuments]
GO

 /* ***************************************************************************************************************************** */
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
  										SELECT @Expired = 1
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

/* ***************************************************************************************************************************** */
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
  										SET @Expired = 1							
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
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='230' WHERE ID='DBVersion'
GO
