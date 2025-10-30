
IF NOT EXISTS (SELECT * FROM [dbo].[sysroNotificationTypes] WHERE [ID] = 46)
   BEGIN
       INSERT INTO [dbo].[sysroNotificationTypes] ([ID],[Name],[Description],[Scheduler],[Feature],[FeatureType],[OnlySystem]) VALUES (46,'Advice for concept value exceeded',null,360,'Employees','U',1)
   END
GO


INSERT INTO [dbo].[Notifications]([ID],[IDType],[Name],[Condition],[Destination],[Schedule],[Activated],[CreatorID],[AllowPortal],[IDPassportDestination],[AllowMail],[ShowOnDesktop],[LastTaskDeleted])
     VALUES(4600,46,'Aviso de valor del saldo por encima del máximo','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>'
	 ,'<?xml version="1.0"?><roCollection version="2.0"></roCollection>',null,0,'SYSTEM',0,0,0,0,null)
GO


IF NOT EXISTS (SELECT * FROM [dbo].[sysroNotificationTypes] WHERE [ID] = 47)
   BEGIN
       INSERT INTO [dbo].[sysroNotificationTypes] ([ID],[Name],[Description],[Scheduler],[Feature],[FeatureType],[OnlySystem]) VALUES (47,'Advice for concept value not reached',null,360,'Employees','U',1)
   END
GO


INSERT INTO [dbo].[Notifications]([ID],[IDType],[Name],[Condition],[Destination],[Schedule],[Activated],[CreatorID],[AllowPortal],[IDPassportDestination],[AllowMail],[ShowOnDesktop],[LastTaskDeleted])
     VALUES(4601,47,'Aviso de valor del saldo por debajo del mínimo','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>'
	 ,'<?xml version="1.0"?><roCollection version="2.0"></roCollection>',null,0,'SYSTEM',0,0,0,0,null)
GO
 
 -- No se detectaban tarjetas ya asignadas si se asignaban por CardAliases
  ALTER PROCEDURE [dbo].[WebLogin_CredentialExists] 
  	(
  		@credential nvarchar(255),
 		@method int,
 		@version nvarchar(50),
  		@idPassport int = NULL		
  	)
  AS
  	IF EXISTS (
  		SELECT ID
  		FROM sysroPassports p
  			 LEFT JOIN sysroPassports_AuthenticationMethods a ON p.ID = a.IDPassport			 			 
  		WHERE a.Method = @method AND
 			  a.version = @version AND
  			  (a.Credential = @credential OR (a.credential IN (select convert(nvarchar,IDCard) from cardaliases where convert(numeric(20,0),RealValue) = convert(numeric(20,0),@credential))))			  
			  AND
  			  (@idPassport IS NULL OR IDPassport <> @idPassport) AND
 			  a.Enabled = 1 AND
 			  (p.IDEmployee IS NULL OR
 			  ISNULL((SELECT COUNT(*) FROM EmployeeContracts 
 					  WHERE EmployeeContracts.IDEmployee = p.IDEmployee AND
 							EmployeeContracts.BeginDate <= GETDATE() AND EmployeeContracts.EndDate >= GETDATE()), 0) > 0) 
  	)
  		SELECT 1
  	ELSE
 		SELECT 0
  	
  	RETURN
GO
 
 -- Soporte rxC PUSH
 ALTER TABLE dbo.Terminals ADD PunchStamp varchar(10)
GO
ALTER TABLE dbo.Terminals ADD OperStamp varchar(10)
GO
--Comportamientos disponibles 
INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxCP',1,99,'TA',1,'Blind','E,S,X','Local','1,0',0,0,'1,0','0',NULL)
GO

INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxCP',1,100,'ACC',1,'Blind','E,S,X','Local','1,0',0,0,'1','0',NULL)
GO

INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxCP',1,101,'ACCTA',1,'Blind','E,S,X','Local','1,0',0,0,'1','0',NULL)
GO

--Habilitamos PRL en terminales de control de acceso 
UPDATE sysroReaderTemplates set OHP=1 where type in ('rxC','mxC','rxCe','rxF') and ScopeMode like '%ACC%'
GO

--En algunas instalaciones antiguas no se generan alertas porque el tipo de las funcionalidades estaba configurado como de empleado, cuando debe ser de usaurio
UPDATE dbo.sysroNotificationTypes SET FeatureType = 'U' WHERE FeatureType = 'E'
GO


IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'ImportGuides' AND COLUMN_NAME = 'Destination') 
BEGIN
	ALTER TABLE ImportGuides ADD Destination INT CONSTRAINT [DF_ImportGuides_Destination] DEFAULT ((0)) NOT NULL
END
GO

UPDATE dbo.sysroParameters SET Data='390' WHERE ID='DBVersion'
GO
