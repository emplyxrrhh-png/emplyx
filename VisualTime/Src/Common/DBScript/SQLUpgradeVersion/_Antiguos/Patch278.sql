UPDATE sysroReaderTemplates SET OHP ='1,0' WHERE Type like 'rx%' and IDReader=1 and ScopeMode = 'ACC'
GO

INSERT INTO ImportGuides ([ID],[Name],[Template],[Mode],[Type],[FormatFilePath],[SourceFilePath],[Separator],[CopySource],[LastLog])
VALUES (3,'Carga de dotación teórica',0,1,2,'','',';',0,'')
GO

DROP TABLE TMPDailyPartialAccruals
GO

CREATE TABLE [dbo].[TMPDailyPartialAccruals](
	[FullGroupName] [nvarchar](500) NOT NULL,
	[IDGroup] [int] NOT NULL,
	[GroupName] [nvarchar](200) NOT NULL,
	[Position] [int] NULL,
	[IDReportGroup] [int] NULL,
	[TotalValueGroup] [numeric](16, 2) NULL,
	[Path] [nvarchar](200) NULL,
	[IDEmployee] [int] NOT NULL,
	[EmployeeName] [nvarchar](100) NOT NULL,
	[TotalValueEmployee] [numeric](16, 2) NULL,
	[Day] [smalldatetime] NULL,
	[ShiftName] [nvarchar](200) NULL,
	[ExpectedWorkingHours] [numeric](16, 2) NULL,
	[ShortName] [nvarchar](3) NULL,
	[Acum1] [numeric](16, 5) NULL,
	[Acum2] [numeric](16, 5) NULL,
	[Acum3] [numeric](16, 5) NULL,
	[Acum4] [numeric](16, 5) NULL,
	[Acum5] [numeric](16, 5) NULL,
	[Acum6] [numeric](16, 5) NULL,
	[Acum7] [numeric](16, 5) NULL,
	[Acum8] [numeric](16, 5) NULL,
	[Acum9] [numeric](16, 5) NULL,
	[Acum10] [numeric](16, 5) NULL,
	[Acum11] [numeric](16, 5) NULL,
	[Acum12] [numeric](16, 5) NULL,
	[Acum13] [numeric](16, 5) NULL,
	[Acum14] [numeric](16, 5) NULL,
	[Acum15] [numeric](16, 5) NULL
) ON [PRIMARY]
GO

ALTER VIEW [dbo].[sysrovwAllEmployeeGroups]
 AS
 SELECT GroupName, sysrovwCurrentEmployeeGroups.Path, sysrovwCurrentEmployeeGroups.SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, CurrentEmployee, AttControlled, AccControlled, 
	JobControlled, ExtControlled, RiskControlled, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName,
		(SELECT CASE LEFT(Path,CHARINDEX('\',Path)) WHEN '' THEN Path ELSE CONVERT(INT,LEFT(Path,CHARINDEX('\',Path)-1)) END FROM Groups WHERE ID = (dbo.Groups.ID)) AS IDCompany
 FROM dbo.Groups        
	inner JOIN dbo.sysrovwCurrentEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwCurrentEmployeeGroups.IDGroup
 UNION
 SELECT GroupName, sysrovwFutureEmployeeGroups.Path, sysrovwFutureEmployeeGroups.SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, CurrentEmployee, AttControlled, AccControlled, 
    JobControlled, ExtControlled, RiskControlled, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName,(SELECT CASE LEFT(Path,CHARINDEX('\',Path)) WHEN '' THEN Path ELSE CONVERT(INT,LEFT(Path,CHARINDEX('\',Path)-1)) END FROM Groups WHERE ID = (dbo.Groups.ID)) AS IDCompany
 FROM dbo.Groups
	INNER JOIN dbo.sysrovwFutureEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwFutureEmployeeGroups.IDGroup
GO
 /* ***************************************************************************************************************************** */

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployeeActivityDocuments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[EmployeeActivityDocuments]
GO
CREATE PROCEDURE [dbo].[EmployeeActivityDocuments]
  	(
  		@idEmployee int,
		@idActivity smallint,
 		@date smalldatetime,
 		@OnlyExpired bit
  	)
	AS
  	BEGIN
 		CREATE TABLE #EmployeeActivityDocumentsTable
 		( 			
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

 		DECLARE @idCompany int, @Path nvarchar(1000), @idSubCompany int
 		DECLARE EmployeesCursor CURSOR
 			FOR SELECT sysroEmployeeGroups.IDCompany
 				FROM sysroEmployeeGroups INNER JOIN Employees
 						ON sysroEmployeeGroups.IDEmployee = Employees.ID
 						INNER JOIN ActivityCompanies ON sysroEmployeeGroups.IDCompany = ActivityCompanies.IDGroup
 				WHERE sysroEmployeeGroups.BeginDate <= @date AND sysroEmployeeGroups.EndDate >= @date AND
 					  Employees.RiskControlled = 1 AND
 					  ActivityCompanies.IDActivity = @idActivity AND
					  sysroEmployeeGroups.IDEmployee = @idEmployee
 		OPEN EmployeesCursor
 		FETCH NEXT FROM EmployeesCursor
 		INTO @idCompany
 		WHILE @@FETCH_STATUS = 0
 		BEGIN
 			-- Obtenemos los documentos del empleado
 			DELETE FROM #DocumentsTable
 			INSERT INTO #DocumentsTable
 			EXEC dbo.EmployeeDocuments @idEmployee, @date
 			INSERT INTO #EmployeeActivityDocumentsTable
 			SELECT 0, #DocumentsTable.*, NULL
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
 				INSERT INTO #EmployeeActivityDocumentsTable
 				SELECT 1, #DocumentsTable.*, @idSubCompany
 				FROM #DocumentsTable
 				WHERE (@OnlyExpired = 0 OR #DocumentsTable.Expired = 1)
 				FETCH NEXT FROM CompaniesCursor 
 				INTO @idSubCompany
 			END
 			CLOSE CompaniesCursor
 			DEALLOCATE CompaniesCursor
 							
 			FETCH NEXT FROM EmployeesCursor 
 			INTO @idCompany
 		END 
 		CLOSE EmployeesCursor
 		DEALLOCATE EmployeesCursor
 		SELECT * FROM #EmployeeActivityDocumentsTable
  	END
GO
 /* ***************************************************************************************************************************** */

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NextExpirations]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[NextExpirations]
GO
CREATE PROCEDURE [dbo].[NextExpirations]
 (
 	@employeesWhere nvarchar(4000),
 	@date nvarchar(50)
 )
 AS
 BEGIN
	CREATE TABLE #EmployeesTable
    	( 			
  		IDEmployee int NOT NULL
  	)
  	CREATE TABLE #EmployeeActivityDocumentsTable
    	( 			
  		IDActivity int NOT NULL,
  		ActivityName nvarchar(1000) NOT NULL,
    		IDCompany int NOT NULL,
  		CompanyName nvarchar(1000) NOT NULL, 
  		CompanyPath nvarchar(1000) NOT NULL,
  		IDEmployee int NOT NULL,
  		EmployeeName nvarchar(1000) NOT NULL,
  		IDGroup int NOT NULL,
  		DocumentName nvarchar(50) NOT NULL,
  		ValidityFrom smalldatetime NULL,
    		ValidityTo smalldatetime NULL,
  		[Type] smallint NOT NULL
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
  	DECLARE @sql nvarchar(4000)
  	DECLARE @newDate smalldatetime
  	--Quitamos del parametro el apartado del path 
  	--SELECT @employeesWhere = left(@employeesWhere,charindex(' OR ({sysrovwAllEmployeeGroups.Path',@employeesWhere)) + '))'
  	--PRINT @employeesWhere
  	
  	--Quitamos los corchetes que nos han pasado como parametro en los empleados
  	--while (charindex('{',@employeesWhere) > 0 OR charindex('}',@employeesWhere) > 0 OR charindex('[',@employeesWhere) > 0 OR charindex(']',@employeesWhere) > 0)
  	--begin
  		if (charindex('{',@employeesWhere) > 0)
  		begin
  			SELECT @employeesWhere = replace(@employeesWhere,'{','')
  		end
  		
  		if (charindex('}',@employeesWhere) > 0)
  		begin
  			SELECT @employeesWhere = replace(@employeesWhere,'}','')
  		end
  		if (charindex('[',@employeesWhere) > 0)
  		begin
  			SELECT @employeesWhere = replace(@employeesWhere,'[','(')
  		end
  		if (charindex(']',@employeesWhere) > 0)
  		begin
  			SELECT @employeesWhere = replace(@employeesWhere,']',')')
  		end
  		if (charindex('startswith',@employeesWhere) > 0)
  		begin
  			SELECT @employeesWhere = replace(@employeesWhere,'startswith','Like')
  		end
  		if (charindex('("',@employeesWhere) > 0) 
  		begin
  			SELECT @employeesWhere = replace(@employeesWhere,'("','(''')
  		end
  		if (charindex('")',@employeesWhere) > 0)
  		begin
  			SELECT @employeesWhere = replace(@employeesWhere,'")','%'')')
  		end
  		if (charindex('","',@employeesWhere) > 0)
  		begin
  			SELECT @employeesWhere = replace(@employeesWhere,'","','%'') or sysrovwAllEmployeeGroups.Path	like (''')
  		end
		if (charindex('EndDate < DateTime (2079, 01, 01, 00, 00, 00)',@employeesWhere) > 0)
  		begin
  			SELECT @employeesWhere = replace(@employeesWhere,'EndDate < DateTime (2079, 01, 01, 00, 00, 00)','EndDate < convert(smalldatetime,''2079-01-01 00:00:00'',120)')
  		end
  		if (charindex('BeginDate > DateTime',@employeesWhere) > 0)
  		begin
  			SELECT @employeesWhere = replace(@employeesWhere,'BeginDate > DateTime','BeginDate > getdate())))--')
  		end
  	--end 
  	
  	--Asignamos la fecha que nos viene como un string la transformamos a formato fecha
  	SELECT @newDate = CONVERT(varchar,@date,120)
  	
  	--Buscamos a los empleados
  	SET @sql = 'INSERT INTO #EmployeesTable 
  				SELECT sysrovwAllEmployeeGroups.IDEmployee
  				FROM sysroActivityCompanies 
  					INNER JOIN sysrovwAllEmployeeGroups ON sysroActivityCompanies.CompanyID = sysrovwAllEmployeeGroups.IDCompany
  					INNER JOIN Employees ON Employees.ID = sysrovwAllEmployeeGroups.IDEmployee
  				WHERE Employees.RiskControlled = 1 AND sysrovwAllEmployeeGroups.BeginDate <= CONVERT(smalldatetime,''' + convert(varchar,@newDate,120) + ''',120)
  					AND sysrovwAllEmployeeGroups.EndDate >= CONVERT(smalldatetime,''' + convert(varchar,@newDate,120) + ''',120)'
  	
  	--Añadimos el filtro de empleados a la consulta
  	SELECT @sql = @sql + ' AND ' + @employeesWhere 
  	
  	EXECUTE (@sql)
  	--Para cada empleado seleccionado
  	DECLARE @idActivity nvarchar(10), @activityName nvarchar(1000), @idCompany nvarchar(10), @companyName nvarchar(1000), @companyPath nvarchar(1000), @companyParent nvarchar(1000), @idEmployee int, @employeeName nvarchar(1000), @idGroup nvarchar(10)
    	DECLARE EmployeesCursor CURSOR 
  		FOR SELECT sysroActivityCompanies.IDActivity, sysroActivityCompanies.ActivityName, 
  				sysroActivityCompanies.CompanyID, sysrovwAllEmployeeGroups.FullGroupName, sysroActivityCompanies.Path,
  				sysroActivityCompanies.FullParentName, sysrovwAllEmployeeGroups.IDEmployee, Employees.Name,
  				sysrovwAllEmployeeGroups.IDGroup
  			FROM sysroActivityCompanies 
  				INNER JOIN sysrovwAllEmployeeGroups ON sysroActivityCompanies.CompanyID = sysrovwAllEmployeeGroups.IDCompany
  				INNER JOIN Employees ON Employees.ID = sysrovwAllEmployeeGroups.IDEmployee
  				INNER JOIN #EmployeesTable ON #EmployeesTable.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee
  	OPEN EmployeesCursor
    		FETCH NEXT FROM EmployeesCursor
    		INTO @idActivity, @activityName, @idCompany, @companyName, @companyPath, @companyParent, @idEmployee, @employeeName, @idGroup
    		WHILE @@FETCH_STATUS = 0
    			BEGIN
  				-- Obtenemos los documentos del empleado
    				DELETE FROM #DocumentsTable
    				INSERT INTO #DocumentsTable
    				EXEC dbo.EmployeeDocuments @idEmployee, @newDate
    				INSERT INTO #EmployeeActivityDocumentsTable
    				SELECT @idActivity, @activityName, @idCompany, @companyName, @companyParent, @idEmployee, @employeeName, @idGroup,
  					#DocumentsTable.Name, #DocumentsTable.ValidityFrom, #DocumentsTable.ValidityTo,0
    				FROM #DocumentsTable
    				WHERE #DocumentsTable.Expired = 1
  			
 				-- Recorremos las subempresas (en función de la actividad)
  				DECLARE @idSubCompany int
    				DECLARE CompaniesCursor CURSOR FOR SELECT [Value] FROM dbo.SplitInt(@companyPath, '\')
    				OPEN CompaniesCursor
    					FETCH NEXT FROM CompaniesCursor
    					INTO @idSubCompany
    					WHILE @@FETCH_STATUS = 0
    					BEGIN
 						--Nos guardamos el nombre y path de la nueva empresa
 						SELECT @companyPath = [Path], @companyParent = FullParentName FROM sysroActivityCompanies WHERE CompanyID = @idSubCompany
    						DELETE FROM #DocumentsTable
    						INSERT INTO #DocumentsTable
    						EXEC dbo.CompanyDocuments @idSubCompany, @newDate
    						INSERT INTO #EmployeeActivityDocumentsTable
    						SELECT @idActivity, @activityName, @idCompany, @companyName, @companyParent, @idEmployee, @employeeName, @idGroup,
  							#DocumentsTable.Name, #DocumentsTable.ValidityFrom, #DocumentsTable.ValidityTo,1
    						FROM #DocumentsTable
    						WHERE #DocumentsTable.Expired = 1
    						FETCH NEXT FROM CompaniesCursor 
    						INTO @idSubCompany
    					END
    				CLOSE CompaniesCursor
    				DEALLOCATE CompaniesCursor
  	  							
    				FETCH NEXT FROM EmployeesCursor 
    				INTO @idActivity, @activityName, @idCompany, @companyName, @companyPath, @companyParent, @idEmployee, @employeeName, @idGroup
    			END 
    	CLOSE EmployeesCursor
    	DEALLOCATE EmployeesCursor
    	SELECT * FROM #EmployeeActivityDocumentsTable
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

ALTER PROCEDURE [dbo].[TerminalDocuments]
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
				  AND TerminalReaders.OHP = 1
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
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='278' WHERE ID='DBVersion'
GO
