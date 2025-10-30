IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.MaxAllowedFileSize')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.MaxAllowedFileSize','256')
GO

ALTER TABLE dbo.sysroPassports_Data ADD
	RecoverKey nvarchar(MAX) NULL
GO

-- nueva notificacion de cobertura insuficiente
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 54)
	INSERT INTO [dbo].[sysroNotificationTypes] ([ID],[Name],[Description],[Scheduler],[Feature],[FeatureType],[OnlySystem])
     VALUES (54,'Productive Unit Under Coverage',NULL,30,'Budgets','U',1)
GO

-- nueva notificacion para enviar mail de recuperación de contraseña
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 55)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,OnlySystem) VALUES(55,'Advice for recover password',null, 1, 1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notifications] WHERE ID = 1904)
	INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal) VALUES(1904, 55, 'Aviso de recuperación de contraseña','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',0)
GO

-- nueva notificacion para enviar mail de aviso de fichaje durante ausencia por dias
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 56)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,OnlySystem, Feature,FeatureType) VALUES(56,'Advice for punch during programmed absence',null, 120, 0,'Employees','U')
GO

UPDATE [dbo].[sysroNotificationTypes] SET OnlySystem = 1 WHERE ID=52
GO

UPDATE dbo.sysroLiveAdvancedParameters SET Value = 5 WHERE ParameterName='VTPortalApiVersion'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTPortal.ShowForbiddenSections')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTPortal.ShowForbiddenSections','true')
GO

-- Campo para informar el tipo de error
ALTER TABLE dbo.InvalidEntries ADD ErrorText nvarchar(max) NULL
GO

UPDATE dbo.sysroParameters SET Data='430' WHERE ID='DBVersion'
GO
