-- Columna para mantener la versión de firmware
ALTER Table dbo.Terminals ADD FirmVersion nvarchar(50) NULL
GO


-- CORRECCION PROCEDURE PARA OBTENER EMPLEADOS EN DEPARTAMENTOS UTILIZADO EN ANALITICA
ALTER PROCEDURE [dbo].[ObtainEmployeeIDsFromFilter]
  	@employeeFilter nvarchar(max),
 	@initialDate smalldatetime,
  	@endDate smalldatetime
   AS 
  begin
  	DECLARE @SQLString nvarchar(MAX);
  	 SET @SQLString = 'SELECT DISTINCT IDEmployee FROM sysroEmployeeGroups WHERE (' + @employeeFilter 
 	 SET @SQLString = @SQLString + ') AND ((''' + CONVERT(VARCHAR(10), @initialDate, 112) + ''' >= dbo.sysroEmployeeGroups.BeginDate and ''' + CONVERT(VARCHAR(10), @initialDate, 112) + ''' <= dbo.sysroEmployeeGroups.EndDate) OR (''' + CONVERT(VARCHAR(10), @endDate, 112) + ''' >= dbo.sysroEmployeeGroups.BeginDate and ''' + CONVERT(VARCHAR(10), @endDate, 112) + ''' <= dbo.sysroEmployeeGroups.BeginDate) OR (''' + CONVERT(VARCHAR(10), @initialDate, 112) + ''' <= dbo.sysroEmployeeGroups.BeginDate and  ''' + CONVERT(VARCHAR(10), @enddate, 112) + ''' >= dbo.sysroEmployeeGroups.BeginDate ))'
 	 SET @SQLString = @SQLString + ' ORDER BY IDEmployee'
    exec sp_executesql @SQLString
  end
GO

--Eliminamos tablas innecesarias y modificadas
DROP TABLE dbo.ActivityCompanies
GO

DROP VIEW [dbo].[sysroActivityCompanies]
GO

ALTER TABLE [dbo].[TerminalReaders] DROP CONSTRAINT [FK_TerminalReaders_Activities]
GO

DROP TABLE [dbo].[Activities]
GO

DROP TABLE [dbo].CompanyActivities
GO

DROP TABLE [dbo].EmployeeActivities
GO

DROP TABLE [dbo].Documents
GO

DROP TABLE [dbo].DocumentTemplates
GO

--Plantillas
CREATE TABLE dbo.DocumentTemplates (Id INT NOT NULL PRIMARY KEY,
								    Name NVARCHAR(50) NOT NULL,
								    ShortName NVARCHAR(5) NOT NULL,
								    Description NVARCHAR(MAX) NULL,
									Scope SMALLINT NOT NULL,
									Area INT NOT NULL,
									AccessValidation INT NOT NULL,
									BeginValidity DATETIME NOT NULL,
									EndValidity DATETIME NOT NULL,
									ApprovalLevelRequired tinyint not null,
									EmployeeDeliverAllowed bit not null,
									SupervisorDeliverAllowed bit not null,
									IsSystem bit not null,
									DefaultExpiration nvarchar(6),
									ExpirePrevious bit not null,
									Compulsory bit not null default(0),
									LOPDAccessLevel tinyint not null default (1),
									DaysBeforeDelete smallint not null,
									Notifications nvarchar(50) null
									)
GO

--Documentos
CREATE TABLE dbo.Documents (Id INT NOT NULL PRIMARY KEY,
								Title NVARCHAR(50) NOT NULL,
								IdDocumentTemplate INT NOT NULL FOREIGN KEY REFERENCES dbo.DocumentTemplates(Id),
								IdEmployee INT NULL,
								IdCompany INT NULL,
								IdPunch int null,
								IdContract nvarchar(50) null,
								IdDaysAbsence int null,
								IdHoursAbsence int null,
								Document Image NOT NULL,
								DocumentType nvarchar(20) NOT NULL,
								DeliveryDate Datetime NOT NULL,
								DeliveryChannel NVARCHAR(50) NOT NULL,
								DeliveredBy NVARCHAR(30) NOT NULL,
								Status SMALLINT NOT NULL,
								StatusLevel INT NOT NULL,
								LastStatusChange Datetime NOT NULL,
								IdLastStatusSupervisor INT NULL,
								BeginDate Datetime NOT NULL,
								EndDate Datetime NOT NULL)
GO

-- Estado global de documento
ALTER TABLE dbo.Documents ADD CurrentlyValid SMALLINT NOT NULL default(0)
GO


-- Ficha de empleado
ALTER TABLE dbo.sysroUserFields ADD DocumentTemplateId INT
GO

--Observaciones al subir un fichero
ALTER TABLE dbo.Documents ADD
	Remarks nvarchar(MAX) NULL
GO

-- GUI
DELETE dbo.sysroGUI where IDPath = 'Portal\GeneralManagement\Activities'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI] WHERE IDPath = 'Portal\GeneralManagement\Documents')
	INSERT INTO [dbo].[sysroGUI] ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
		VALUES ('Portal\GeneralManagement\Documents','Documents','Documents/DocumentTemplate.aspx','Documents.png',NULL,NULL,'Feature\Documents',NULL,106,NULL,'U:Documents.DocumentsDefinition=Read')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =32)
BEGIN
	INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
		VALUES (32,NULL,'Documents','Documentos','','U','RWA',1,NULL,32)	

	declare @usrId int
	SELECT @usrId = ID FROM sysroPassports where name = 'Administradores'

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] VALUES (@usrId,32,9)	
END
GO
	
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =32100)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(32100,32,'Documents.DocumentsDefinition','Definición Documentos','','U','RWA',NULL,NULL,32)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 32100, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 32	
END
GO


IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =32200)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(32200,32,'Documents.Permision','Permisos','','U','RWA',NULL,NULL,32)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 32200, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 32
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =32210)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(32210,32200,'Documents.Permision.Prevention','Prevención','','U','RWA',NULL,NULL,32)
	
	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 32210, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 32
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =32220)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(32220,32200,'Documents.Permision.Labor','Laboral','','U','RWA',NULL,NULL,32)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 32220, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 32
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =32430)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(32230,32200,'Documents.Permision.Legal','Legal','','U','RWA',NULL,NULL,32)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 32230, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 32
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =32440)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(32240,32200,'Documents.Permision.Security','Seguridad','','U','RWA',NULL,NULL,32)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 32240, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 32
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =32450)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(32250,32200,'Documents.Permision.Quality','Calidad','','U','RWA',NULL,NULL,32)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 32250, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 32
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =32300)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(32300,32,'Documents.AccessLevel','Nivel de Acceso','','U','RWA',NULL,NULL,32)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 32300, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 32
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =32310)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(32310,32300,'Documents.AccessLevel.Low','Nivel básico','','U','RWA',NULL,NULL,32300)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 32310, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 32300
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =32320)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(32320,32300,'Documents.AccessLevel.Medium','Nivel medio','','U','RWA',NULL,NULL,32300)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 32320, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 32300
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =32330)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(32330,32300,'Documents.AccessLevel.High','Nivel alto','','U','RWA',NULL,NULL,32300)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 32330, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 32300
END
GO

EXEC dbo.sysro_GenerateAllPermissionsOverFeatures
GO


EXEC dbo.sysro_GenerateAllPermissionsOverGroups
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].sysroGUI_Actions WHERE IDPath = 'NewDocument' AND IDGUIPath = 'Portal\GeneralManagement\Documents\Template')
	INSERT INTO sysroGUI_Actions ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
	VALUES ('NewDocument', 'Portal\GeneralManagement\Documents\Template','tbAddNewDocument','Feature\Documents',NULL,'newDocumentTemplate()','btnTbAddTaskTemplate',0,1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].sysroGUI_Actions WHERE IDPath = 'Del' AND IDGUIPath = 'Portal\GeneralManagement\Documents\Template')
	INSERT INTO [dbo].sysroGUI_Actions ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
	VALUES ('Del', 'Portal\GeneralManagement\Documents\Template','tbDelDocument','Feature\Documents',NULL,'ShowRemoveDocumentTemplate(''#ID#'')','btnTbDel2',0,2)
GO


IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 49)
	INSERT INTO [dbo].[sysroNotificationTypes] ([ID],[Name],[Description],[Scheduler],[Feature],[FeatureType],[OnlySystem])
     VALUES (49,'Document delivered',NULL,360,'Documents','U',1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notifications] WHERE ID = 700)
	INSERT INTO [dbo].[Notifications] ([ID],[IDType],[Name],[Condition],[Destination],[Schedule],[Activated],[CreatorID],[AllowPortal],[IDPassportDestination],[AllowMail],[ShowOnDesktop],[LastTaskDeleted])
     VALUES (700,49,'Aviso de nuevo documento entregado','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>',NULL,1,'SYSTEM',0,0,0,0,NULL)
GO


UPDATE dbo.sysroParameters SET Data='412' WHERE ID='DBVersion'
GO
