---- Generamos columnas para marcar los fichajes para la premigracion
alter table AccessMoves ADD checked bit not null default 0 
GO

alter table InvalidAccessMoves ADD checked bit not null default 0 
GO

-- Posibilidad de configurar Prevención de Riesgos Laborales conjuntamente con Accesos Integrados con Presencia
UPDATE sysroReaderTemplates SET OHP = '1,0' WHERE TYPE in ( 'rxa','rxb') AND ScopeMode = 'ACCTA'
GO
-- Terminal rxF no permite Prevención de riesgos laborales
UPDATE sysroReaderTemplates SET OHP = '0' WHERE TYPE = 'rxF' AND ScopeMode = 'ACCTA'
GO

-- Permisos de consulta de saldos para empleados
INSERT INTO sysroFeatures VALUES (26, NULL,	'Totals','Saldos','','E','R',NULL)
GO
INSERT INTO sysroFeatures VALUES (26001, 26, 'Totals.Query', 'Consulta de Saldos para empleados','','E','R',NULL)
GO

-- Modificamos Stored para Informe Empleados Autorizados
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

 ALTER PROCEDURE [dbo].[EmployeesAuthorized]
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


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='286' WHERE ID='DBVersion'
GO
