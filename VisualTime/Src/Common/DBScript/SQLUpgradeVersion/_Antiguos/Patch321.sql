ALTER PROCEDURE [dbo].[NextExpirations]
 (	@employeesWhere nvarchar(4000),
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
	select  @date=replace(@date,'-','/')
	if CHARINDEX('/',@date,1)>4 
		begin
			SELECT @newDate = CONVERT(smalldatetime,@date,120)	
		end
		else
		begin
			SELECT @newDate = CONVERT(varchar,@date,120)
		end
	 	   	
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
   
-- Nuevos modos para mx8 (mx8PLUS)
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput,Direction,AllowAccessPeriods,SupportedSirens) 
VALUES('mx8',1,53,'EIP','1','Interactive',NULL,'Server','1,0','0','0','0','0',NULL,1,NULL)
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput,Direction,AllowAccessPeriods,SupportedSirens) 
VALUES('mx8',1,54,'TAEIP','1','Interactive',NULL,'Server','1,0','0','0','0','0',NULL,1,NULL)
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput,Direction,AllowAccessPeriods,SupportedSirens) 
VALUES('mx8',1,55,'ACCTAEIP','1','Interactive',NULL,'Local','1,0','1,0','0','1,0','1,0',NULL,1,1)
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput,Direction,AllowAccessPeriods,SupportedSirens) 
VALUES('mx8',1,56,'ACCEIP','1','Interactive',NULL,'Local','1,0','1,0','0','1,0','1,0',NULL,1,1)
GO

-- Campo modelo
ALTER TABLE [dbo].[Terminals] ADD [Model] [nvarchar](20) NULL
GO

-- Corrección de detección de empleado presente para VisualTime Visitas
 ALTER PROCEDURE [dbo].[Visits_Employee_IsIn]
    	@employeeId int
    AS
    	DECLARE @lastAccessDate smalldatetime;
    	DECLARE @lastPunchIn smalldatetime;
    	DECLARE @lastPunchOut smalldatetime;
   	DECLARE @isWorkingZone bit;
  	DECLARE @isLivePunches bit;
  	if exists (select * from Punches)
  		BEGIN
  			set @isLivePunches = 1
  		END
  	ELSE
  		BEGIN
  			set @isLivePunches = 0
  		END
      if (@isLivePunches = 0)
  		begin
  			 -- ULTIMO FICHAJE DE ACCESOS
  			Select Top 1 @lastAccessDate = DateTime, @isWorkingZone = IsWorkingZone  From AccessMoves,Zones, sysrosubvwCurrentEmployeePeriod Where AccessMoves.IDZone = Zones.ID And AccessMoves.IDEmployee = str(@employeeId) And AccessMoves.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee  Order by DateTime desc
  			 -- ULTIMO FICHAJE DE ENTRADA DE PRESENCIA
  			Select Top 1 @lastPunchIn = InDateTime From Moves, sysrosubvwCurrentEmployeePeriod Where Moves.IDEmployee = str(@employeeId)  AND Moves.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee Order by InDateTime desc
  			 -- ULTIMO FICHAJE DE SALIDA DE PRESENCIA
  			Select Top 1 @lastPunchOut = OutDateTime From Moves, sysrosubvwCurrentEmployeePeriod Where Moves.IdEmployee = str(@employeeId)  AND Moves.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee Order by OutDateTime desc
  		end
  	else
  		BEGIN
  			 -- ULTIMO FICHAJE DE ACCESOS
  			Select Top 1 @lastAccessDate = DateTime, @isWorkingZone = IsWorkingZone  From Punches,Zones, sysrosubvwCurrentEmployeePeriod Where Punches.IDZone = Zones.ID And Punches.IDEmployee = str(@employeeId) And Punches.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee AND (Type = 5 OR Type=7) Order by DateTime desc
  			 -- ULTIMO FICHAJE DE ENTRADA DE PRESENCIA
  			Select Top 1 @lastPunchIn = DateTime From Punches, sysrosubvwCurrentEmployeePeriod Where Punches.IDEmployee = str(@employeeId)  AND Punches.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee AND Type = 1 Order by DateTime desc
  			 -- ULTIMO FICHAJE DE SALIDA DE PRESENCIA
  			Select Top 1 @lastPunchOut = DateTime From Punches, sysrosubvwCurrentEmployeePeriod Where Punches.IdEmployee = str(@employeeId)  AND Punches.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee AND Type = 2 Order by DateTime desc
  		END
  			
   	
 	IF ISDATE(@lastAccessDate) <> 1 and ISDATE(@lastPunchIn) <> 1 and ISDATE(@lastPunchIn) <> 1
   		-- Si no hay ningún fichaje de ningún tipo, estoy ausente
   		BEGIN
   			RETURN 0
   		END
   	ELSE
   		IF ISDATE(@lastAccessDate) <> 1 set @lastAccessDate = convert(smalldatetime,'1900-01-01 00:00',120)
   		IF ISDATE(@lastPunchIn) <> 1 set @lastPunchIn = convert(smalldatetime,'1900-01-01 00:00',120)
   		IF ISDATE(@lastPunchOut) <> 1 set @lastPunchOut = convert(smalldatetime,'1900-01-01 00:00',120)
   		-- CALCULOS
    		IF @lastPunchIn >= @lastPunchOut and @lastPunchIn >= @lastAccessDate
    			-- Si lo último es una entrada de presencia ... estoy presente
   			BEGIN
   				RETURN 1
   			END
    		ELSE
   			BEGIN
   				IF @lastPunchOut >= @lastPunchIn and @lastPunchOut >= @lastAccessDate 
   					-- Si lo último es una salida de presencia ... estoy ausente
   					BEGIN
   						RETURN 0
   					END
   				ELSE
   					BEGIN
    						IF @lastAccessDate >= @lastPunchOut and @lastAccessDate >= @lastPunchIn and @isWorkingZone = 1 
   							-- Si lo último es un fichaje de accesos y es a una zona de trabajo ... estoy presente
   							BEGIN
   								RETURN 1
   							END
    						ELSE 
   							BEGIN
   								-- Si lo último es un fichaje de accesos y es a una zona de NO trabajo ... estoy ausente
   								RETURN 0
   							END
   					END
   			END
GO
   
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='321' WHERE ID='DBVersion'
GO

