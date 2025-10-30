IF NOT EXISTS(SELECT * FROM sys.tables WHERE NAME = 'sysroQueries')
	CREATE TABLE [dbo].[sysroQueries]
	(
		[Id ]INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
		[Name] NVARCHAR(100) NOT NULL,
		[Description] NVARCHAR(MAX) NOT NULL,
		[Value] NVARCHAR(MAX) NOT NULL,
		[Parameters] NVARCHAR(MAX) NULL
	)
GO

--Visualizar tareas pendientes colgadas de los terminales tras realizar broadcast
INSERT INTO dbo.sysroQueries (Name, Description, Value, Parameters) VALUES ('Visualizar tareas pendientes colgadas de los terminales tras realizar broadcast', 'Visualizar tareas pendientes colgadas de los terminales tras realizar broadcast', 'SELECT Terminals.ID ''Identificador'', Terminals.Description ''Descripción'', Terminals.Type ''Tipo'', Terminals.LastStatus ''ÚltimoEstado'', (SELECT COUNT(*) FROM TerminalsSyncTasks WHERE IDTerminal = ID) ''Tareas'' FROM dbo.Terminals WHERE Type NOT in (''LivePortal'', ''Virtual'') ORDER BY Terminals.Description', NULL)
--Saber si hay algún informe planificado ejecutándose
INSERT INTO dbo.sysroQueries (Name, Description, Value, Parameters) VALUES ('Saber si hay algún informe planificado ejecutándose', 'Saber si hay algún informe planificado ejecutándose', 'SELECT ''Manual'' AS Tipo, sysroReportTasks.ReportName ''Nombre'', sysroReportTasks.ExecutionDate ''ÚltimaEjecución'', sysroPassports.Name ''Peticionario'' FROM dbo.sysroReportTasks INNER JOIN dbo.sysroPassports ON sysroPassports.ID = sysroReportTasks.IDPassport WHERE EndDate IS NULL UNION ALL SELECT ''Planificado'', ReportsScheduler.Name, ReportsScheduler.LastExecution, sysroPassports.Name FROM dbo.ReportsScheduler INNER JOIN dbo.sysroPassports ON sysroPassports.ID = ReportsScheduler.IDPassport WHERE ReportsScheduler.State != 3 AND ReportsScheduler.ExecuteTask = 1', NULL)
--Saber si el fichero de correlación está correctamente introducido en base de datos
INSERT INTO dbo.sysroQueries (Name, Description, Value, Parameters) VALUES ('Saber si el fichero de correlación está correctamente introducido en base de datos', 'Saber si el fichero de correlación está correctamente introducido en base de datos', 'SELECT CardAliases.IDCard ''Identificador'', CardAliases.RealValue ''Valor'', Employees.ID ''IdEmpleado'', Employees.Name ''NombreEmpleado'' FROM CardAliases LEFT JOIN dbo.sysroPassports_AuthenticationMethods ON sysroPassports_AuthenticationMethods.Credential = CAST(CardAliases.IDCard AS NVARCHAR) LEFT JOIN dbo.sysroPassports ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport LEFT JOIN dbo.Employees ON Employees.ID = sysroPassports.IDEmployee WHERE Employees.Name LIKE CONCAT(''%'', @pNombreEmpleado, ''%'')', '<?xml version="1.0" encoding="utf-16"?><ArrayOfParameter xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Parameter><Name>pNombreEmpleado</Name><Description>Nombre empleado</Description><Type>Text</Type></Parameter></ArrayOfParameter>')
--Comprobar si el recálculo tras los cambios realizados en VisualTime está funcionando correctamente
INSERT INTO dbo.sysroQueries (Name, Description, Value, Parameters) VALUES ('Comprobar si el recálculo tras los cambios realizados en VisualTime está funcionando correctamente', 'Comprobar si el recálculo tras los cambios realizados en VisualTime está funcionando correctamente', 'SELECT COUNT(*) ''Pendientes'', Status ''Estado'' from dbo.DailySchedule WHERE Date <= GETDATE() AND Status < 70 GROUP BY Status', NULL)
--Ver los fichajes que ha habido en una fecha en concreto (poder escoger fecha)
INSERT INTO dbo.sysroQueries (Name, Description, Value, Parameters) VALUES ('Ver los fichajes que ha habido en una fecha en concreto (poder escoger fecha)', 'Ver los fichajes que ha habido en una fecha en concreto (poder escoger fecha)', 'SELECT Employees.Id ''IdEmpleado'', Employees.Name ''NombreEmpleado'', Punches.ShiftDate ''Turno'', Punches.DateTime ''Fichaje'', Terminals.ID ''IdTerminal'', Terminals.Description ''NombreTerminal'' FROM dbo.Punches INNER JOIN dbo.Employees ON  Employees.ID = Punches.IDEmployee INNER JOIN dbo.Terminals ON Terminals.ID = Punches.IDTerminal WHERE CAST(ShiftDate AS DATE) = CAST(@pFechaFichaje AS DATE) AND Employees.Name LIKE CONCAT(''%'', @pNombreEmpleado, ''%'') ORDER BY Punches.DateTime, Employees.Name', '<?xml version="1.0" encoding="utf-16"?><ArrayOfParameter xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Parameter><Name>pFechaFichaje</Name><Description>Fecha fichaje</Description><Type>Date</Type></Parameter><Parameter><Name>pNombreEmpleado</Name><Description>Nombre empleado</Description><Type>Text</Type></Parameter></ArrayOfParameter>')
--Comprobar el grupo al que pertenecen los empleados. Ruta corta y completa
INSERT INTO dbo.sysroQueries (Name, Description, Value, Parameters) VALUES ('Comprobar el grupo al que pertenecen los empleados. Ruta corta y completa', 'Comprobar el grupo al que pertenecen los empleados. Ruta corta y completa', 'SELECT sysrovwCurrentEmployeeGroups.GroupName ''Grupo'', sysrovwCurrentEmployeeGroups.FullGroupName ''GrupoCompleto'', sysrovwCurrentEmployeeGroups.IDEmployee ''IdEmpleado'', sysrovwCurrentEmployeeGroups.EmployeeName ''NombreEmpleado'' FROM sysrovwCurrentEmployeeGroups WHERE sysrovwCurrentEmployeeGroups.CurrentEmployee = 1 AND sysrovwCurrentEmployeeGroups.EmployeeName LIKE CONCAT(''%'', @pNombreEmpleado, ''%'') AND sysrovwCurrentEmployeeGroups.GroupName LIKE CONCAT(''%'', @pGrupo, ''%'') ORDER BY sysrovwCurrentEmployeeGroups.FullGroupName, sysrovwCurrentEmployeeGroups.EmployeeName', '<?xml version="1.0" encoding="utf-16"?><ArrayOfParameter xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Parameter><Name>pNombreEmpleado</Name><Description>Nombre empleado</Description><Type>Text</Type></Parameter><Parameter><Name>pGrupo</Name><Description>Grupo</Description><Type>Text</Type></Parameter></ArrayOfParameter>')
--Cerciorarnos de cuánto le queda al recalculo que realiza VisualTime y a cuántos empleados afecta
INSERT INTO dbo.sysroQueries (Name, Description, Value, Parameters) VALUES ('Cerciorarnos de cuánto le queda al recalculo que realiza VisualTime y a cuántos empleados afecta', 'Cerciorarnos de cuánto le queda al recalculo que realiza VisualTime y a cuántos empleados afecta', 'SELECT COUNT(*) ''Pendientes'', IDEmployee ''IdEmpleado'' FROM dbo.DailySchedule WHERE DATE <= GETDATE() AND Status < 70 GROUP BY IDEmployee', NULL)
--Comprobar si cliente tiene GPA
INSERT INTO dbo.sysroQueries (Name, Description, Value, Parameters) VALUES ('Comprobar si cliente tiene GPA', 'Comprobar si cliente tiene GPA', 'SELECT CASE WHEN SUM(Total) > 0 THEN ''Con licencia GPA'' ELSE ''Sin licencia GPA'' END AS ''Estado'' FROM (SELECT COUNT(*) Total FROM dbo.AbsenceTracking UNION SELECT COUNT(*) Total FROM dbo.DocumentsAbsences UNION SELECT COUNT(*) Total FROM dbo.CausesDocuments) LISTA', NULL)
--Comprobar que hay alguna tarea que no ha podido ejecutarse con normalidad
INSERT INTO dbo.sysroQueries (Name, Description, Value, Parameters) VALUES ('Comprobar que hay alguna tarea que no ha podido ejecutarse con normalidad', 'Comprobar que hay alguna tarea que no ha podido ejecutarse con normalidad', 'SELECT * FROM sysroTasks WHERE DATEDIFF(DAY, DateCreated, CURRENT_TIMESTAMP) >= @pCantidadDias', '<?xml version="1.0" encoding="utf-16"?><ArrayOfParameter xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Parameter><Name>pCantidadDias</Name><Description>Cantidad de días desde creación</Description><Type>Integer</Type></Parameter></ArrayOfParameter>')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 61)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,OnlySystem, Feature,FeatureType) VALUES(61,'Data Import executed',null, 1, 0,'','')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.Security.Load')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.Security.Load','70000000')
GO

CREATE TABLE [dbo].[Visit_Print_Config](
	[Id] int NOT NULL,
	[Field1] [nvarchar](40) ,
	[Field2] [nvarchar](40) ,
	[Field3] [nvarchar](40) ,
	[Field4] [nvarchar](40) ,
	[Custom1] [nvarchar](200) ,
	[Custom2] [nvarchar](200)
)
GO

insert into sysroGUI_Actions values('CurrentLogged','Portal\Security\Passports\Management','tbCurrentLoggedUsers','Forms\Passports','U:Administration.Security=Admin','ShowCurrentLoggedUsers()','btnTbCurrent3',0,5)
GO

CREATE TABLE [dbo].[sysroConcurrentLicenses](
	[DateTime] [datetime] NOT NULL,
	[Count] [int] NOT NULL,
	[VTLive] decimal(5,2) NULL,
	[VTSupervisor] decimal(5,2) NULL,
	[VTPortal] decimal(5,2) NULL,
	[Status] [nvarchar](50) NULL
)
GO
--Comportamientos disponibles para rx1
INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rX1',1,106,'TA',1,'Blind','E,S,X','Local','1,0',0,0,'1,0','0',NULL)
GO

INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rX1',1,107,'ACC',1,'Blind','E,S,X','Local','1,0',0,0,'1','0',NULL)
GO

INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rX1',1,108,'ACCTA',1,'Blind','E,S,X','Local','1,0',0,0,'1','0',NULL)
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='445' WHERE ID='DBVersion'
GO