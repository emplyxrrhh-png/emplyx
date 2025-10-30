delete from dbo.sysroShiftTimeZones where IDShift in (select id from dbo.shifts where IsTemplate=1)
GO

delete from dbo.sysroShiftsLayers where IDShift in (select id from dbo.shifts where IsTemplate=1)
GO

delete from dbo.shifts where IsTemplate=1
GO

ALTER TABLE [dbo].[ReportLayoutCategories] DROP CONSTRAINT [FK_ReportLayoutsCategories_ReportCategories]
GO
ALTER TABLE [dbo].[ReportLayoutCategories]  WITH CHECK ADD  CONSTRAINT [FK_ReportLayoutsCategories_ReportCategories] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[sysroCategoryTypes] ([ID])
GO
ALTER TABLE [dbo].[ReportLayoutCategories] CHECK CONSTRAINT [FK_ReportLayoutsCategories_ReportCategories]
GO

DROP TABLE [dbo].[ReportCategories]
GO

ALTER TABLE dbo.Communiques ALTER COLUMN Subject nvarchar(50)
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFP', 1, N'TA', N'1', N'Blind', N'E,S,X', N'Local', N'1,0', N'0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFP', 1, N'CO', N'1', N'Blind', N'E,S,X', N'Local', N'1,0', N'0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFP', 1, N'ACC', N'1', N'Blind', N'E,S,X', N'Local', N'0', N'0', N'0', N'1', N'1,0', NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFP', 1, N'ACCTA', N'1', N'Blind', N'E,S,X', N'Local', N'0', N'0', N'0', N'1', N'1,0', NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFPTD', 1, N'TA', N'1', N'Blind', N'E,S,X', N'Local', N'1,0', N'0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFPTD', 1, N'CO', N'1', N'Blind', N'E,S,X', N'Local', N'1,0', N'0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFPTD', 1, N'ACC', N'1', N'Blind', N'E,S,X', N'Local', N'0', N'0', N'0', N'1', N'1,0', NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFPTD', 1, N'ACCTA', N'1', N'Blind', N'E,S,X', N'Local', N'0', N'0', N'0', N'1', N'1,0', NULL, 1, N'0', N'')
GO


DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFL', 1, N'TA', N'1', N'Blind', N'E,S,X', N'Local', N'1,0', N'0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFL', 1, N'CO', N'1', N'Blind', N'E,S,X', N'Local', N'1,0', N'0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFL', 1, N'ACC', N'1', N'Blind', N'E,S,X', N'Local', N'0', N'0', N'0', N'1', N'1,0', NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFL', 1, N'ACCTA', N'1', N'Blind', N'E,S,X', N'Local', N'0', N'0', N'0', N'1', N'1,0', NULL, 1, N'0', N'')
GO

ALTER TABLE [dbo].[TerminalsSyncBiometricData] ADD Version NVARCHAR(15) NULL
GO

ALTER TABLE [dbo].[Punches] ADD MaskAlert BIT
GO

ALTER TABLE [dbo].[Punches] ADD TemperatureAlert BIT
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 69)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,OnlySystem, Feature,FeatureType,IDCategory) VALUES(69,'Employee without mask',null, 1, 0,'Calendar.Punches','U',6)
GO

ALTER TABLE [dbo].[Punches] ADD VerificationType tinyint
GO

ALTER TABLE [dbo].[Terminals] ADD SecurityOptions NVARCHAR(max)
GO

UPDATE [dbo].[TerminalsSyncBiometricData] SET Version = 'RXFFNG' WHERE Version IS NULL OR Version = ''
GO

ALTER TABLE [dbo].[sysroPassports_AuthenticationMethods] ADD BiometricAlgorithm NVARCHAR(20)
GO

ALTER TABLE [dbo].[sysroPassports_AuthenticationMethods] ADD BiometricTerminalId TINYINT
GO

ALTER PROCEDURE [dbo].[WebLogin_Passports_AuthenticationMethods_Select] 
(
   	@idPassport int,
   	@method int,
  	@version nvarchar(50)
)
AS
SELECT IDPassport,
   	Method,
  	Version,
   	Credential,
   	Password,
   	StartDate,
   	ExpirationDate,
  	BiometricID,
  	BiometricData,
  	TimeStamp,
  	Enabled,
 	LastUpdatePassword,
 	BloquedAccessApp ,
 	InvalidAccessAttemps, 
 	LastDateInvalidAccessAttempted,
	BiometricAlgorithm,
	BiometricTerminalId
  		
FROM sysroPassports_AuthenticationMethods
WHERE IDPassport = @idPassport AND
  		(@method IS NULL OR Method = @method) AND
  		(@version IS NULL OR Version = @version)
   	
RETURN
GO

ALTER PROCEDURE [dbo].[WebLogin_Passports_AuthenticationMethod_Select] 
(
   	@method int,
  	@version nvarchar(50)		
)
AS
SELECT IDPassport,
   	Method,
  	Version,
   	Credential,
   	Password,
   	StartDate,
   	ExpirationDate,
  	BiometricID,
  	BiometricData,
  	TimeStamp,
  	Enabled,
 	LastUpdatePassword,
 	BloquedAccessApp ,
 	InvalidAccessAttemps, 
 	LastDateInvalidAccessAttempted,
	BiometricAlgorithm,
	BiometricTerminalId
FROM sysroPassports_AuthenticationMethods
WHERE Method = @method AND
  		Version = @version AND		  
   		(StartDate IS NULL OR StartDate <= GetDate()) AND
   		(ExpirationDate IS NULL OR ExpirationDate > GetDate())
RETURN
GO

ALTER PROCEDURE [dbo].[WebLogin_Passports_AuthenticationMethods_Update]
(
   	@idPassport int,
   	@method int,
  	@version nvarchar(50),
   	@credential nvarchar(255),
   	@password nvarchar(1000),
   	@startDate datetime,
   	@expirationDate datetime,
  	@biometricID int,		
  	@timestamp datetime,
  	@enabled bit,
 	@LastUpdatePassword datetime,
 	@BloquedAccessApp bit,
 	@InvalidAccessAttemps int, 
 	@LastDateInvalidAccessAttempted datetime,
	@biometricalgorithm nvarchar(20) = NULL,
	@biometricterminalid int = NULL	 		
  		
)
AS
UPDATE sysroPassports_AuthenticationMethods SET
   	Credential = @credential,
   	Password = @password,
   	StartDate = @startDate,
   	ExpirationDate = @expirationDate,		
  	TimeStamp = @timestamp,
  	Enabled = @enabled,
 	LastUpdatePassword = @LastUpdatePassword ,
 	BloquedAccessApp = @BloquedAccessApp ,
 	InvalidAccessAttemps = @InvalidAccessAttemps , 
 	LastDateInvalidAccessAttempted = @LastDateInvalidAccessAttempted  ,
	BiometricAlgorithm = @biometricalgorithm ,
	BiometricTerminalId	= @biometricterminalid			
  		
WHERE IDPassport = @idPassport AND
   	Method = @method AND
  	Version = @version AND
  	BiometricID = @biometricID
RETURN
GO

ALTER PROCEDURE [dbo].[WebLogin_Passports_AuthenticationMethods_Insert] 
(
   	@idPassport int,
   	@method int,
  	@version nvarchar(50),
   	@credential nvarchar(255),
   	@password nvarchar(1000),
   	@startDate datetime,
   	@expirationDate datetime,
  	@biometricID int,		
  	@timestamp datetime,
  	@enabled bit,
 	@LastUpdatePassword datetime,
 	@BloquedAccessApp bit,
 	@InvalidAccessAttemps int, 
 	@LastDateInvalidAccessAttempted datetime,
	@biometricalgorithm nvarchar(20) = NULL,
	@biometricterminalid int = NULL		
)
AS
INSERT INTO sysroPassports_AuthenticationMethods (
   	IDPassport,
   	Method,
  	Version,
   	Credential,
   	Password,
   	StartDate,
   	ExpirationDate,
  	BiometricID,
  	BiometricData,
  	TimeStamp,
  	Enabled,
 	LastUpdatePassword ,
 	BloquedAccessApp ,
 	InvalidAccessAttemps , 
 	LastDateInvalidAccessAttempted ,
	BiometricAlgorithm ,
	BiometricTerminalId
)
VALUES (
   	@idPassport,
   	@method,
  	@version,
   	@credential,
   	@password,
   	@startDate,
   	@expirationDate,
  	@biometricID,
  	null,
  	@timestamp,
  	@enabled,
 	@LastUpdatePassword,
 	@BloquedAccessApp,
 	@InvalidAccessAttemps, 
 	@LastDateInvalidAccessAttempted,
	@biometricalgorithm,
	@biometricterminalid
)
   	
RETURN
GO

ALTER TABLE punches ALTER COLUMN VerificationType int
GO

UPDATE sysroParameters SET Data='497' WHERE ID='DBVersion'
GO

