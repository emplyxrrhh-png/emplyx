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
		DocumentName nvarchar(50) NOT NULL,
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
	SELECT @employeesWhere = left(@employeesWhere,charindex(' OR ({sysrovwAllEmployeeGroups.Path',@employeesWhere)) + '))'
	--PRINT @employeesWhere
	
	--Quitamos los corchetes que nos han pasado como parametro en los empleados
	while (charindex('{',@employeesWhere) > 0 OR charindex('}',@employeesWhere) > 0 OR charindex('[',@employeesWhere) > 0 OR charindex(']',@employeesWhere) > 0)
	begin
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
	end
	print @employeesWhere

	--Asignamos la fecha que nos viene como un string la transformamos a formato fecha
	SELECT @newDate = CONVERT(varchar,@date,120)
	--PRINT convert(varchar,@newDate,120)

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
	--PRINT @sql
	EXECUTE (@sql)

	--Para cada empleado seleccionado
	DECLARE @idActivity nvarchar(10), @activityName nvarchar(1000), @idCompany nvarchar(10), @companyName nvarchar(1000), @companyPath nvarchar(1000), @companyParent nvarchar(1000), @idEmployee int, @employeeName nvarchar(1000)
  	DECLARE EmployeesCursor CURSOR 
		FOR SELECT sysroActivityCompanies.IDActivity, sysroActivityCompanies.ActivityName, 
				sysroActivityCompanies.CompanyID, sysrovwAllEmployeeGroups.GroupName, sysroActivityCompanies.Path,
				sysroActivityCompanies.FullParentName, sysrovwAllEmployeeGroups.IDEmployee, Employees.Name
			FROM sysroActivityCompanies 
				INNER JOIN sysrovwAllEmployeeGroups ON sysroActivityCompanies.CompanyID = sysrovwAllEmployeeGroups.IDCompany
				INNER JOIN Employees ON Employees.ID = sysrovwAllEmployeeGroups.IDEmployee
				INNER JOIN #EmployeesTable ON #EmployeesTable.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee
	OPEN EmployeesCursor
  		FETCH NEXT FROM EmployeesCursor
  		INTO @idActivity, @activityName, @idCompany, @companyName, @companyPath, @companyParent, @idEmployee, @employeeName
  		WHILE @@FETCH_STATUS = 0
  			BEGIN
				-- Obtenemos los documentos del empleado
  				DELETE FROM #DocumentsTable
  				INSERT INTO #DocumentsTable
  				EXEC dbo.EmployeeDocuments @idEmployee, @newDate
  				INSERT INTO #EmployeeActivityDocumentsTable
  				SELECT @idActivity, @activityName, @idCompany, @companyName, @companyParent, @idEmployee, @employeeName,
					#DocumentsTable.Name, #DocumentsTable.ValidityTo,0
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
  						DELETE FROM #DocumentsTable
  						INSERT INTO #DocumentsTable
  						EXEC dbo.CompanyDocuments @idSubCompany, @newDate
  						INSERT INTO #EmployeeActivityDocumentsTable
  						SELECT @idActivity, @activityName, @idCompany, @companyName, @companyParent, @idEmployee, @employeeName,
							#DocumentsTable.Name, #DocumentsTable.ValidityTo,1
  						FROM #DocumentsTable
  						WHERE #DocumentsTable.Expired = 1
  						FETCH NEXT FROM CompaniesCursor 
  						INTO @idSubCompany
  					END
  				CLOSE CompaniesCursor
  				DEALLOCATE CompaniesCursor
	  							
  				FETCH NEXT FROM EmployeesCursor 
  				INTO @idActivity, @activityName, @idCompany, @companyName, @companyPath, @companyParent, @idEmployee, @employeeName
  			END 
  	CLOSE EmployeesCursor
  	DEALLOCATE EmployeesCursor
  	SELECT * FROM #EmployeeActivityDocumentsTable
END
GO
CREATE PROCEDURE EmployeesAuthorized
(
  		@employeesWhere nvarchar(4000),
 		@date nvarchar(50)
)
AS
BEGIN
	CREATE TABLE #EmployeesTable
  	( 			
		IDActivity nvarchar(10) NOT NULL,
		IDGroup nvarchar(10) NOT NULL,
		[Path] nvarchar(50) NOT NULL,
		IDEmployee int NOT NULL
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
	CREATE TABLE #EmployeesAuthorized
	(
		IDCompany nvarchar(500) NOT NULL,
		CompanyName nvarchar(500),
		IDEmployee int NOT NULL,
		EmployeeName nvarchar(500)
	)

	DECLARE @sql nvarchar(4000)
	DECLARE @newDate smalldatetime

	--Quitamos del parametro el apartado del path 
	SELECT @employeesWhere = left(@employeesWhere,charindex(' OR ({sysrovwAllEmployeeGroups.Path',@employeesWhere)) + '))'
		
	--Quitamos los corchetes que nos han pasado como parametro en los empleados
	while (charindex('{',@employeesWhere) > 0 OR charindex('}',@employeesWhere) > 0 OR charindex('[',@employeesWhere) > 0 OR charindex(']',@employeesWhere) > 0)
	begin
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
	end
	
	--Seteamos las variables
	DELETE FROM #EmployeesTable
	DELETE FROM #EmployeesAuthorized
	SELECT @newDate = CONVERT(varchar,@date,120)
	SET @sql = 'INSERT INTO #EmployeesTable SELECT sysroActivityCompanies.IDActivity, sysroActivityCompanies.CompanyID, sysroActivityCompanies.Path, sysrovwAllEmployeeGroups.IDEmployee
				FROM sysroActivityCompanies 
					INNER JOIN sysrovwAllEmployeeGroups ON sysroActivityCompanies.CompanyID = sysrovwAllEmployeeGroups.IDCompany
					INNER JOIN Employees ON Employees.ID = sysrovwAllEmployeeGroups.IDEmployee
					INNER JOIN EmployeeContracts ON EmployeeContracts.IDEmployee = Employees.ID
				WHERE Employees.RiskControlled = 1 
					AND EmployeeContracts.BeginDate <= CONVERT(smalldatetime,''' + convert(varchar,@newDate,120) + ''',120)
					AND EmployeeContracts.EndDate >= CONVERT(smalldatetime,''' + convert(varchar,@newDate,120) + ''',120)
					AND sysrovwAllEmployeeGroups.BeginDate <= CONVERT(smalldatetime,''' + convert(varchar,@newDate,120) + ''',120)
					AND sysrovwAllEmployeeGroups.EndDate >= CONVERT(smalldatetime,''' + convert(varchar,@newDate,120) + ''',120)'
	
	--Añadimos el filtro de empleados a la consulta
	SELECT @sql = @sql + ' AND ' + @employeesWhere 
	EXECUTE (@sql)
	
	--Para cada empleado comprobamos si tiene todos los documentos en regla
	DECLARE @idCompany nvarchar(10), @companyName nvarchar(500), @idEmployee int, @employeeName nvarchar(500), @expired bit
	DECLARE @idActivity nvarchar(10), @companyPath nvarchar(1000), @count int, @expiredDocuments int
	DECLARE Employees CURSOR FOR SELECT * FROM #EmployeesTable				
	OPEN Employees
		SET @expired = 0
		FETCH NEXT FROM Employees
  		INTO @idActivity, @idCompany, @companyPath, @idEmployee
  		WHILE @@FETCH_STATUS = 0 
  			BEGIN
				-- Obtenemos los documentos del empleado
				DELETE FROM #DocumentsTable
  				INSERT INTO #DocumentsTable
  				EXEC dbo.EmployeeDocuments @idEmployee, @newDate
				SELECT @expired = COUNT(*) FROM #DocumentsTable WHERE #DocumentsTable.Expired = 1 AND #DocumentsTable.AccessValidation = 1	
				
				--Insertamos al empleado que lo tiene todo en regla
				IF @expired = 0
				BEGIN
					--Miramos si tiene algun documento de empresa caducado
					DECLARE @idSubCompany int
  					DECLARE CompaniesCursor CURSOR FOR SELECT [Value] FROM dbo.SplitInt(@companyPath, '\')
					SELECT @expiredDocuments = 0
  					OPEN CompaniesCursor
						FETCH NEXT FROM CompaniesCursor
  						INTO @idSubCompany
  						WHILE @@FETCH_STATUS = 0 
  						BEGIN
							DELETE FROM #DocumentsTable
  							INSERT INTO #DocumentsTable
							EXEC dbo.CompanyDocuments @idSubCompany, @newDate
  							SELECT @expired = COUNT(*) FROM #DocumentsTable WHERE #DocumentsTable.Expired = 1 AND #DocumentsTable.AccessValidation = 1	
							
							--Insertamos al empleado que lo tiene todo en regla
							IF @expired > 0
							BEGIN
								SELECT @expiredDocuments = 1
  							END
											
  							FETCH NEXT FROM CompaniesCursor 
  							INTO @idSubCompany
  						END
					--SELECT  @count = count(*) from #EmployeesAuthorized where IDEmployee = @idEmployee
					IF (@expiredDocuments = 0)
					BEGIN
						INSERT INTO #EmployeesAuthorized
						SELECT @idCompany, sysroActivityCompanies.GroupName, @idEmployee, Name
						FROM sysrovwAllEmployeeGroups
							INNER JOIN sysroActivityCompanies ON sysroActivityCompanies.CompanyID = sysrovwAllEmployeeGroups.IDCompany
							INNER JOIN Employees ON Employees.ID = sysrovwAllEmployeeGroups.IDEmployee
						WHERE Employees.ID = @idEmployee
					END

  					CLOSE CompaniesCursor
  					DEALLOCATE CompaniesCursor
				END
				FETCH NEXT FROM Employees
				INTO @idActivity, @idCompany, @companyPath, @idEmployee
			END
	CLOSE Employees
  	DEALLOCATE Employees
	SELECT * FROM #EmployeesAuthorized
END
GO

UPDATE sysroFeatures SET Alias = 'Causes.EmployeesAndRulesAssign' Where ID = 3100 And IDParent = 3
GO

/* ***************************************************************************************************************************** */

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='224' WHERE ID='DBVersion'
GO
