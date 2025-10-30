---- Generamos una columna para saber si los terminales tiene periodos de acceso
alter table sysroReaderTemplates ADD AllowAccessPeriods bit not null default 1 
GO

---- Marcamos los rxA100 como terminales que no tienen periodos de acceso
update sysroReaderTemplates set AllowAccessPeriods=0 where [Type] like 'rxa100%'
GO

-- 
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
						--Miramos que las fechas se posicionen correctamente
						IF Right(@Validity,1)= '*' 
							SELECT TOP 1 @Value = [Value] FROM dbo.Split(@Validity, '*') 
							ORDER BY CASE [Value] WHEN '' THEN '2079/01/01' ELSE [Value] END ASC
							IF @Value <> '' and Right(@Validity,1)= '*'
								SELECT @ValidityFrom = CONVERT(smalldatetime, @Value, 120)
						IF Left(@Validity,1)= '*' 
							SELECT TOP 1 @Value = [Value] FROM dbo.Split(@Validity, '*')
							ORDER BY CASE [Value] WHEN '' THEN '2079/01/01' ELSE [Value] END DESC
							IF Left(@Validity,1)= '*'
								SELECT @ValidityTo = CONVERT(smalldatetime, Right(@Validity,10), 120)
						
						IF Right(@Validity,1)<> '*' and Left(@Validity,1)<> '*'
							SELECT TOP 1 @Value = [Value] FROM dbo.Split(@Validity, '*') 
							ORDER BY CASE [Value] WHEN '' THEN '2079/01/01' ELSE [Value] END ASC
							IF @Value <> '' and Right(@Validity,1)<> '*' and Left(@Validity,1)<> '*'
								SELECT @ValidityFrom = CONVERT(smalldatetime, @Value, 120)								
							
							SELECT TOP 1 @Value = [Value] FROM dbo.Split(@Validity, '*')
							ORDER BY CASE [Value] WHEN '' THEN '2079/01/01' ELSE [Value] END DESC
							IF @Value <> '' and Right(@Validity,1)<> '*' and Left(@Validity,1)<> '*'
								SELECT @ValidityTo = CONVERT(smalldatetime, @Value, 120)								
					END
				-- Miramos si el documento está caducado
				-- Miramos si el documento está caducado
				SET @Expired = 1
				IF NOT(@ValidityFrom IS NULL AND @ValidityTo IS NULL)
					IF @ValidityFrom<=@date AND @ValidityTo>=@date
						SET @Expired = 0

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
						--Miramos que las fechas se posicionen correctamente
						IF Right(@Validity,1)= '*' 
							SELECT TOP 1 @Value = [Value] FROM dbo.Split(@Validity, '*') 
							ORDER BY CASE [Value] WHEN '' THEN '2079/01/01' ELSE [Value] END ASC
							IF @Value <> '' and Right(@Validity,1)= '*'
								SELECT @ValidityFrom = CONVERT(smalldatetime, @Value, 120)
						IF Left(@Validity,1)= '*' 
							SELECT TOP 1 @Value = [Value] FROM dbo.Split(@Validity, '*')
							ORDER BY CASE [Value] WHEN '' THEN '2079/01/01' ELSE [Value] END DESC
							IF Left(@Validity,1)= '*'
								SELECT @ValidityTo = CONVERT(smalldatetime, Right(@Validity,10), 120)
						
						IF Right(@Validity,1)<> '*' and Left(@Validity,1)<> '*'
							SELECT TOP 1 @Value = [Value] FROM dbo.Split(@Validity, '*') 
							ORDER BY CASE [Value] WHEN '' THEN '2079/01/01' ELSE [Value] END ASC
							IF @Value <> '' and Right(@Validity,1)<> '*' and Left(@Validity,1)<> '*'
								SELECT @ValidityFrom = CONVERT(smalldatetime, @Value, 120)								
							
							SELECT TOP 1 @Value = [Value] FROM dbo.Split(@Validity, '*')
							ORDER BY CASE [Value] WHEN '' THEN '2079/01/01' ELSE [Value] END DESC
							IF @Value <> '' and Right(@Validity,1)<> '*' and Left(@Validity,1)<> '*'
								SELECT @ValidityTo = CONVERT(smalldatetime, @Value, 120)							
					END
				-- Miramos si el documento está caducado
				SET @Expired = 1
				IF NOT(@ValidityFrom IS NULL AND @ValidityTo IS NULL)
					IF @ValidityFrom<=@date AND @ValidityTo>=@date
						SET @Expired = 0

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


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='280' WHERE ID='DBVersion'
GO
