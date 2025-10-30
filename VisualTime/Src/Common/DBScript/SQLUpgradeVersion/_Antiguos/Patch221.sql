/* ***************************************************************************************************************************** */

ALTER TABLE dbo.TerminalReaders ADD
	InteractionMode nvarchar(50) NULL,
	InteractionAction nvarchar(10) NULL,	
	InvalidOutput tinyint NULL,
	InvalidDuration tinyint NULL,
	CustomButtons text NULL
GO

ALTER TABLE dbo.TerminalReaders ADD
	UseDispKey bit NULL,
	OHP bit NULL
GO
ALTER TABLE dbo.TerminalReaders ADD CONSTRAINT
	DF_TerminalReaders_UseDispKey DEFAULT 0 FOR UseDispKey
GO
ALTER TABLE dbo.TerminalReaders ADD CONSTRAINT
	DF_TerminalReaders_OHP DEFAULT 0 FOR OHP
GO

ALTER TABLE dbo.TerminalReaders ADD
	ValidationMode nvarchar(50) NULL
GO

ALTER TABLE dbo.TerminalReaders ADD
	InteractiveConfig text NULL
GO

/* ***************************************************************************************************************************** */

CREATE TABLE [dbo].[sysroReaderTemplates](
	[Type] [nvarchar](50) NOT NULL,
	[IDReader] [tinyint] NOT NULL,
	[ID] [smallint] NOT NULL,
	[ScopeMode] [nvarchar](100) NULL,
	[UseDispKey] [nvarchar](3) NULL,
	[InteractionMode] [nvarchar](50) NULL,
	[InteractionAction] [nvarchar](50) NULL,
	[ValidationMode] [nvarchar](50) NULL,
	[EmployeesLimit] [nvarchar](3) NULL,
	[OHP] [nvarchar](3) NULL,
	[CustomButtons] [nvarchar](3) NULL,
	[Output] [nvarchar](3) NULL,
	[InvalidOutput] [nvarchar](3) NULL,
 CONSTRAINT [PK_sysroTerminalReadersTemplates] PRIMARY KEY CLUSTERED 
(
	[Type] ASC,
	[IDReader] ASC,
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
	
DELETE FROM sysroReaderTemplates WHERE Type IN ('mx6', 'rx')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6', 1, 1,'ACC', '0','Blind', 'X', 'Local', '0', '1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6',1,2,'ACC','1','Blind','X','Local','0','1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6',1,3,'ACCTA','0','Blind','X','Local','0','1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6',1,4,'ACCTA','1','Blind','X','Local','0','1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6',1,5,'TA','1','Blind','E,S,X','LocalServer,ServerLocal,Server,Local','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6',1,6,'TA','1','Fast',NULL,'LocalServer,ServerLocal,Server,Local','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6',1,7,'TA','1','Interactive',NULL,'ServerLocal,Server','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6',1,8,'TAEIP','1','Interactive',NULL,'ServerLocal,Server','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6',1,9,'TATSK','1','Interactive',NULL,'ServerLocal,Server','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6',1,10,'TATSKEIP','1','Interactive',NULL,'ServerLocal,Server','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6',1,11,'TSK','1','Interactive',NULL,'ServerLocal,Server','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6',1,12,'TSKEIP','1','Interactive',NULL,'ServerLocal,Server','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6',1,13,'EIP','1','Interactive',NULL,'ServerLocal,Server','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6',2,14,'',NULL,NULL,NULL,NULL,'0',NULL,NULL,NULL,NULL)
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6',2,15,'ACC','0','Blind','X','Local','0','1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6',2,16,'ACC','1','Blind','X','Local','0','1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6',2,17,'ACCTA','0','Blind','X','Local','0','1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx6',2,18,'ACCTA','1','Blind','X','Local','0','1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('rx',1,19,'TA','1','Blind','E,S,X','Local','0','0','0','1,0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('rx',1,20,'ACC','1','Blind','X','Local','0','0','0','1,0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('rx',1,21,'ACCTA','1','Blind','X','Local','0','0','0','1,0','0')
GO

/* ***************************************************************************************************************************** */

CREATE TABLE dbo.TerminalReaderEmployees
	(
	IDTerminal tinyint NOT NULL,
	IDReader tinyint NOT NULL,
	IDEmployee int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.TerminalReaderEmployees ADD CONSTRAINT
	PK_TerminalReaderEmployees PRIMARY KEY CLUSTERED 
	(
	IDTerminal,
	IDReader,
	IDEmployee
	) ON [PRIMARY]

GO
ALTER TABLE dbo.TerminalReaderEmployees ADD CONSTRAINT
	FK_TerminalReaderEmployees_TerminalReaders FOREIGN KEY
	(
	IDTerminal,
	IDReader
	) REFERENCES dbo.TerminalReaders
	(
	IDTerminal,
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

/* ***************************************************************************************************************************** */

CREATE NONCLUSTERED INDEX [_dta_index_AuditLive_9_337436276__K3] ON [dbo].[AuditLive] 
(
	[Date] ASC
)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

/* ***************************************************************************************************************************** */

 ALTER PROCEDURE [dbo].[TerminalDocuments]
 	(
 		@idTerminal smallint,
		@date smalldatetime,
		@OnlyExpired bit
 	)
 --RETURNS @DocumentsTable table ([Name] nvarchar(50), [Description] nvarchar(4000) NULL, ValidityFrom smalldatetime NULL, ValidityTo smalldatetime NULL, AccessValidation smallint)
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
					  TerminalReaders.IDTerminal = @idTerminal AND
					  ISNULL(TerminalReaders.OHP, 0) = 1


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

CREATE TABLE dbo.Tmp_Captures
	(
	ID int NOT NULL,
	DateTime datetime NOT NULL,
	Capture image NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
IF EXISTS(SELECT * FROM dbo.Captures)
	 EXEC('INSERT INTO dbo.Tmp_Captures (ID, DateTime, Capture)
		SELECT ID, CONVERT(datetime, DateTime), Capture FROM dbo.Captures WITH (HOLDLOCK TABLOCKX)')
GO
ALTER TABLE dbo.AccessMoves
	DROP CONSTRAINT FK_AccessMoves_Captures
GO
ALTER TABLE dbo.InvalidAccessMoves
	DROP CONSTRAINT FK_InvalidAccessMoves_Captures
GO
DROP TABLE dbo.Captures
GO
EXECUTE sp_rename N'dbo.Tmp_Captures', N'Captures', 'OBJECT' 
GO
ALTER TABLE dbo.Captures ADD CONSTRAINT
	PK_Captures PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.InvalidAccessMoves ADD CONSTRAINT
	FK_InvalidAccessMoves_Captures FOREIGN KEY
	(
	IDCapture
	) REFERENCES dbo.Captures
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.AccessMoves ADD CONSTRAINT
	FK_AccessMoves_Captures FOREIGN KEY
	(
	IDCapture
	) REFERENCES dbo.Captures
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

/* Actualizamos la fecha y hora de las capturas de accesos  */
UPDATE Captures
SET [Datetime] = AccessMoves.[DateTime]
FROM Captures INNER JOIN AccessMoves ON Captures.ID = AccessMoves.IDCapture
GO

UPDATE Captures
SET [Datetime] = InvalidAccessMoves.[DateTime]
FROM Captures INNER JOIN InvalidAccessMoves ON Captures.ID = InvalidAccessMoves.IDCapture
GO

/* ***************************************************************************************************************************** */
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (8,NULL,'Terminals','Terminales','','U','RWA',0)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (9,NULL,'Access','Accesos','','U','RWA',1)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (8000,8,'Terminals.StatusInfo','Información estado terminal','','U','R',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (8100,8,'Terminals.Definition','Configuración terminales','','U','RWA',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (9000,9,'Access.Groups','Grupos de acceso','','U','RWA',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (9010,9000,'Access.Groups.Definition','Definición','','U','RWA',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (9100,9,'Access.Periods','Periodos de acceso','','U','RWA',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (9110,9100,'Access.Periods.Definition','Definición','','U','RWA',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (9200,9,'Access.Zones','Zonas de acceso','','U','RWA',null)
GO
INSERT INTO sysroFeatures (ID,IDParent,Alias,Name,Description,Type,PermissionTypes,AppHasPermissionsOverEmployees) VALUES (9210,9200,'Access.Zones.Definition','Definición','','U','RWA',null)
GO

/* Damos permisos a la pantalla terminales */
INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT sysroPassports.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM sysroFeatures, sysroPassports		
WHERE sysroFeatures.ID IN (8, 8000) AND
	  sysroPassports.IDParentPassport IS NULL AND sysroPassports.GroupType = 'U'
GO

/* Damos permisos de solo lectura a las pantallas de accesos */
INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT sysroPassports.ID, sysroFeatures.ID, 3
FROM sysroFeatures, sysroPassports		
WHERE sysroFeatures.ID IN (9, 9000, 9010, 9100, 9110, 9200, 9210) AND
	  sysroPassports.IDParentPassport IS NULL AND sysroPassports.GroupType = 'U'
GO

/* Damos permisos a los grupos de empleados para la funcionalidad de accesos*/
INSERT INTO sysroPassports_PermissionsOverGroups (IDPassport, IDGroup, IDApplication, Permission)
SELECT sysroPassports.ID, Groups.ID, sysroFeatures.ID, 3
FROM sysroFeatures, Groups, sysroPassports
WHERE sysroFeatures.ID IN (9) AND sysroFeatures.AppHasPermissionsOverEmployees = 1 AND Groups.Path = CONVERT(varchar, Groups.ID) AND
	  sysroPassports.GroupType = 'U'

GO

/* ***************************************************************************************************************************** */

/* Actualizamos la tabla de zonas añadiendo una zona padre */
ALTER TABLE dbo.Zones ADD
	IDParent tinyint NULL,
	Color int NULL,
	ZoneImage image NULL
GO
INSERT INTO Zones (ID, Name, IsWorkingZone) SELECT CASE 
	WHEN MAX(ID) IS NULL THEN 1 
	ELSE MAX(ID) + 1 
	END, 'Mi Empresa', 0 from Zones
GO
UPDATE Zones SET IDParent = (Select max(ID) From Zones) Where ID <> (Select max(ID) From Zones);
GO

/* ***************************************************************************************************************************** */
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Access','Access',null,'Access.png',1006,'NWR','U:Access=Read','Forms\Access')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Access\AccessStatus','AccessStatus','Access/AccessStatus.aspx','AccessStatus.png',710,'NWR','U:Access.Zones=Read','Forms\Access')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Access\AccessGroups','AccessGroups','Access/AccessGroups.aspx','AccessGroups.png',720,'NWR','U:Access.Groups=Read','Forms\Access')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Access\AccessPeriods','AccessPeriods','Access/AccessPeriods.aspx','AccessPeriods.png',730,'NWR','U:Access.Periods=Read','Forms\Access')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Access\AccessZones','AccessZones','Access/AccessZones.aspx','AccessZones.png',740,'NWR','U:Access.Zones=Read','Forms\Access')
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Configuration\Terminal','Terminals','Terminals/Terminals.aspx','TerminalesRT.png',130,'NWR','U:Terminals=Read','Forms\Terminals')
GO

/* ***************************************************************************************************************************** */

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='221' WHERE ID='DBVersion'
GO
