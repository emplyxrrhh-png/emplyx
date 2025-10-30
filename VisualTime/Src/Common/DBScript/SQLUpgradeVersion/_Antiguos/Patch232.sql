--CREAMOS LA TABLA DEL ANTIPASSBACK POR ZONAS
CREATE TABLE [dbo].[ZonesRules](
	[IDZone] [int] NOT NULL,
	[IDPreviousZone] [int] NOT NULL,
 CONSTRAINT [PK_ZonesRules] PRIMARY KEY CLUSTERED 
(
	[IDZone] ASC,
	[IDPreviousZone] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

--ACTUALIZAMOS LA VISTA DE MOVIMIENTOS DE EMPLEADO (EN EL APARTADO DE ACCESOS INDICAMOS LA ZONA)
ALTER view [dbo].[sysrovwEmployeesInOutMoves]
as
SELECT dbo.Moves.IDEmployee, MAX(dbo.Moves.InDateTime) AS DateTime, dbo.Moves.InIDReader AS IDReader, '0' as IDZone, 'IN' AS Status, 'Att' AS MoveType
FROM dbo.Moves 
WHERE (dbo.Moves.InDateTime IS NOT NULL) 
GROUP BY dbo.Moves.IDEmployee, dbo.Moves.InIDReader
UNION
SELECT dbo.Moves.IDEmployee, MAX(dbo.Moves.OutDateTime) AS DateTime, dbo.Moves.OutIDReader AS IDReader, '0' as IDZone, 'OUT' AS Status, 'Att' AS MoveType
FROM dbo.Moves 
WHERE (dbo.Moves.OutDateTime IS NOT NULL)
GROUP BY dbo.Moves.IDEmployee, dbo.Moves.OutIDReader, dbo.Moves.OutIDReader
UNION
SELECT am.IDEmployee, MAX(am.DateTime) AS DateTime, am.IDReader, IDZone, CASE zo.IsWorkingZone WHEN 1 THEN 'IN' ELSE 'OUT' END AS Status, 'Acc' AS MoveType
FROM dbo.AccessMoves AS am 
 	INNER JOIN dbo.Zones AS zo ON am.IDZone = zo.ID 
WHERE (DateTime IS NOT NULL)
GROUP BY am.IDEmployee, am.IDReader, IDZone, zo.IsWorkingZone, am.DateTime 
GO

--CREAMOS LA VISTA QUE USARÁ EL INFORME DE EMERGENCIA DE ACCESOS
CREATE view [dbo].[sysrovwCurrentEmployeesAccessStatus]
 as
 SELECT DISTINCT IDEmployee, EmployeeName,
  (SELECT TOP 1 DateTime FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee AND Status='IN' AND MoveType='Acc' ORDER BY DateTime DESC) AS DateTime,
  (SELECT TOP 1 IDReader FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee AND Status='IN' AND MoveType='Acc' ORDER BY DateTime DESC) AS IDReader,
  (SELECT TOP 1 IDZone FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee AND Status='IN' AND MoveType='Acc' ORDER BY DateTime DESC) AS IDZone,  
  (SELECT TOP 1 Status FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee AND Status='IN' AND MoveType='Acc' ORDER BY DateTime DESC) AS Status
FROM dbo.sysrovwCurrentEmployeeGroups sc
GO

ALTER view [dbo].[sysrovwCurrentEmployeesPresenceStatus]
  as
  SELECT DISTINCT IDEmployee, EmployeeName,
	(SELECT TOP 1 DateTime FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee ORDER BY DateTime DESC) AS DateTime,
	(SELECT TOP 1 IDReader FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee ORDER BY DateTime DESC) AS IDReader,
	(SELECT TOP 1 MoveType FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee ORDER BY DateTime DESC) AS MoveType,
	(SELECT TOP 1 IDZone FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee AND MoveType='Acc' ORDER BY DateTime DESC) AS IDZone,
	(SELECT TOP 1 Status FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee ORDER BY DateTime DESC) AS Status
FROM dbo.sysrovwCurrentEmployeeGroups sc
GO

--CREAMOS LA TABLA TEMPORAL A PARTIR DE DONDE SE PINTARÁ EL INFORME DE EMERGENCIA DE ACCESOS
CREATE TABLE [dbo].[TMPEmergency](
	[IDEmployee] [int] NOT NULL,
	[UserFieldValue] [nvarchar](100) NOT NULL,
	[State] [nvarchar](50) NULL,
 CONSTRAINT [PK_TMPEmergency] PRIMARY KEY CLUSTERED 
(
	[IDEmployee] ASC,
	[UserFieldValue] ASC
) ON [PRIMARY]
) ON [PRIMARY]
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

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

DROP VIEW dbo.sysrovwEmergencyReport
GO

CREATE VIEW dbo.sysrovwEmergencyReport
AS
--- Selecciona ultima entrada de presencia sin salida
Select 'Empleados' as Type, employees.name, indatetime, case charindex('#',description) when 0 then description else left(description, charindex('#',description)-1) end as Terminal, terminals.id as IDTerminal, GroupName, CGR.IDEmployee 
 from moves m1, employees, terminals,  sysrovwcurrentemployeegroups CGR 
where outdatetime is null and 
	  indatetime in (select max(m2.indatetime) from moves m2 where m1.idemployee = m2.idemployee) 
	and m1.idemployee = employees.id and m1.inidreader = terminals.id and m1.idemployee = CGR.IDEmployee 

UNION 

-- Selecciona ultima entrada de Visitas sin salida
Select 'Visitantes' as Type, VT.name, vm.begintime, case charindex('#',description) when 0 then description else left(description, charindex('#',description)-1) end as Terminal, TR.id as IDTerminal, Employeename , CGR.IDEmployee 
From visitplan VP, visitmoves VM, VISITORS VT, Employees emp, moves, terminals TR, sysrovwcurrentemployeegroups CGR  
where vp.id = vm.visitplanid and vp.visitorid = VT.id and vp.empvisitedid = emp.id 
	and vm.endtime is null and vp.empvisitedid = moves.idEmployee 
	and moves.indatetime = (select max(indatetime) from moves m1 where moves.IdEmployee = m1.idEmployee) 
	and moves.inidreader = TR.id and vp.empvisitedid = cgr.idemployee 


UNION 
-- Selecciona la ultma entrada sin salida a fichajes que no tienen terminal relacionado (WEBTERMINAL)
Select 'Empleados' as Type,CGR.Employeename, indatetime, 'Indeterminado' as Terminal, '' as IDTerminal , groupname, CGR.IDEmployee 
From Moves m1, sysrovwcurrentemployeegroups CGR 
Where outdatetime is null  
	and m1.indatetime in (Select max(indatetime) from moves m2 where m1.idemployee = m2.idemployee) 
	and m1.idemployee = CGR.idemployee and not exists (select 1 from terminals T where m1.inidreader = T.id) 

UNION 
-- Selecciona la ultma entrada sin salida de Visitas que no tienen terminal relacionado (VISITAS)
Select 'Visitantes' as Type, convert(nvarchar(50), VT.name) , vm.begintime, 'Indeterminado' as Terminal,0, groupname,  CGR.IDEmployee
From visitplan VP, visitmoves VM, VISITORS VT, Employees emp, moves, terminals TERM, sysrovwcurrentemployeegroups CGR  
where vp.id = vm.visitplanid and vp.visitorid = VT.id and vp.empvisitedid = emp.id 
and vm.endtime is null and vp.empvisitedid = moves.idEmployee 
and moves.indatetime = (select max(indatetime) from moves m1 where moves.IdEmployee = m1.idEmployee) 
and vp.empvisitedid = cgr.idemployee and not exists (select 1 from terminals where moves.inidreader = terminals.id) 

union

-- Selecciona el ultimo acceso a una zona de trabajo 
Select 'Empleados' as Type, employees.name as EmployeeName, datetime, case charindex('#',terminals.description) when 0 then terminals.description else left(terminals.description, charindex('#',terminals.description)-1) end as Terminal, terminals.id as IDTerminal, GroupName   , CGR.IDEmployee
from Accessmoves m1, employees, terminals,  sysrovwcurrentemployeegroups CGR , Zones
where Datetime in (select max(m2.Datetime) from Accessmoves m2 where m1.idemployee = m2.idemployee) 
	and m1.idemployee = employees.id and m1.idreader = terminals.id and m1.idemployee = CGR.IDEmployee 
	and Zones.ID = m1.IDZone
	and zones.ISWorkingZone = 1
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[sysrovwPastEmployeeGroups]') AND OBJECTPROPERTY(id, N'IsView') = 1)
DROP VIEW [dbo].[sysrovwPastEmployeeGroups]
GO

CREATE VIEW [dbo].[sysrovwPastEmployeeGroups]
AS
SELECT     dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                      dbo.Employees.Name AS EmployeeName, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, 
                      COUNT(dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee) AS CurrentEmployee, dbo.Employees.AttControlled, dbo.Employees.AccControlled, 
                      dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, 
                      dbo.GetFullGroupPathName(dbo.EmployeeGroups.IDGroup) AS FullGroupName
FROM         dbo.EmployeeGroups INNER JOIN
                      dbo.Employees ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID INNER JOIN
                      dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                      dbo.sysrosubvwCurrentEmployeePeriod ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
GROUP BY dbo.Groups.Name, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                      dbo.Employees.Name, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, dbo.Employees.AttControlled, dbo.Employees.AccControlled, 
                      dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled
HAVING      (dbo.EmployeeGroups.EndDate < CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102))

GO

/* ***************************************************************************************************************************** */

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[sysrovwEmployeesInAllGroups]') AND OBJECTPROPERTY(id, N'IsView') = 1)
DROP VIEW [dbo].[sysrovwEmployeesInAllGroups]
GO

CREATE VIEW [dbo].[sysrovwEmployeesInAllGroups]
  AS
SELECT GroupName, sysrovwPastEmployeeGroups.Path, sysrovwPastEmployeeGroups.SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, CurrentEmployee, AttControlled, AccControlled, 
 	JobControlled, ExtControlled, RiskControlled, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName,
 		(SELECT CASE LEFT(Path,CHARINDEX('\',Path)) WHEN '' THEN Path ELSE CONVERT(INT,LEFT(Path,CHARINDEX('\',Path)-1)) END FROM Groups WHERE ID = (dbo.Groups.ID)) AS IDCompany
  FROM dbo.Groups        
 	inner JOIN dbo.sysrovwPastEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwPastEmployeeGroups.IDGroup
UNION
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

ALTER TABLE dbo.DailySchedule ADD
	LockedDay bit NULL
GO
ALTER TABLE dbo.DailySchedule ADD CONSTRAINT
	DF_DailySchedule_LockedDay DEFAULT 0 FOR LockedDay
GO

/* ***************************************************************************************************************************** */

CREATE TABLE [dbo].[sysroScheduleTemplates](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [text] NULL,
 CONSTRAINT [PK_sysroScheduleTemplates] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/* ***************************************************************************************************************************** */

CREATE TABLE dbo.sysroScheduleTemplates_Detail
	(
	IDTemplate int NOT NULL,
	ScheduleDate smalldatetime NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.sysroScheduleTemplates_Detail ADD CONSTRAINT
	PK_sysroScheduleTemplates_Detail PRIMARY KEY CLUSTERED 
	(
	IDTemplate,
	ScheduleDate
	) ON [PRIMARY]

GO
ALTER TABLE dbo.sysroScheduleTemplates_Detail ADD CONSTRAINT
	FK_sysroScheduleTemplates_Detail_sysroScheduleTemplates FOREIGN KEY
	(
	IDTemplate
	) REFERENCES dbo.sysroScheduleTemplates
	(
	ID
	) 	 	
GO

/* ***************************************************************************************************************************** */

DELETE FROM sysroReaderTemplates WHERE Type IN ('mx7')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7', 1, 1,'ACC', '0','Blind', 'X', 'Local', '0', '1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7',1,2,'ACC','1','Blind','X','Local','0','1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7',1,3,'ACCTA','0','Blind','X','Local','0','1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7',1,4,'ACCTA','1','Blind','X','Local','0','1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7',1,5,'TA','1','Blind','E,S,X','LocalServer,ServerLocal,Server,Local','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7',1,6,'TA','1','Fast',NULL,'LocalServer,ServerLocal,Server,Local','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7',1,7,'TA','1','Interactive',NULL,'ServerLocal,Server','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7',1,8,'TAEIP','1','Interactive',NULL,'ServerLocal,Server','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7',1,9,'TATSK','1','Interactive',NULL,'ServerLocal,Server','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7',1,10,'TATSKEIP','1','Interactive',NULL,'ServerLocal,Server','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7',1,11,'TSK','1','Interactive',NULL,'ServerLocal,Server','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7',1,12,'TSKEIP','1','Interactive',NULL,'ServerLocal,Server','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7',1,13,'EIP','1','Interactive',NULL,'ServerLocal,Server','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7',2,14,'',NULL,NULL,NULL,NULL,'0',NULL,NULL,NULL,NULL)
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7',2,15,'ACC','0','Blind','X','Local','0','1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7',2,16,'ACC','1','Blind','X','Local','0','1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7',2,17,'ACCTA','0','Blind','X','Local','0','1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx7',2,18,'ACCTA','1','Blind','X','Local','0','1,0','0','1,0','1,0')
GO

/* ***************************************************************************************************************************** */

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='232' WHERE ID='DBVersion'
GO
