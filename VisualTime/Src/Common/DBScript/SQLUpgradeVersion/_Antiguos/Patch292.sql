ALTER PROCEDURE [dbo].[NextExpirations]
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

--Asignamos la fecha que nos viene como un string la transformamos a formato fecha
SET @newDate = CONVERT(smalldatetime,@date,120)

--Buscamos a los empleados
SET @sql = 'INSERT INTO #EmployeesTable 
			SELECT sysrovwAllEmployeeGroups.IDEmployee
			FROM sysroActivityCompanies 
			INNER JOIN sysrovwAllEmployeeGroups ON sysroActivityCompanies.CompanyID = sysrovwAllEmployeeGroups.IDCompany
			INNER JOIN Employees ON Employees.ID = sysrovwAllEmployeeGroups.IDEmployee
			WHERE Employees.RiskControlled = 1 AND 
			sysrovwAllEmployeeGroups.BeginDate <= CONVERT(smalldatetime,''' + @date + ''',120) AND 
			sysrovwAllEmployeeGroups.EndDate >= CONVERT(smalldatetime,''' + @date + ''',120)'
			
			--sysrovwAllEmployeeGroups.BeginDate <= CONVERT(smalldatetime,''' + convert(varchar,@newDate,120) + ''',120) AND 
			--sysrovwAllEmployeeGroups.EndDate >= CONVERT(smalldatetime,''' + convert(varchar,@newDate,120) + ''',120)'

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

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='292' WHERE ID='DBVersion'
GO
