IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 85)
BEGIN
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem) VALUES(85,'Alert for genius analytic result',null, 360,'','', 1)
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (85,1,'Subject',0,'','GeniusFinished')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (85,1,'Body',1,'StudyName','GeniusFinished')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (85,1,'Body',2,'URL','GeniusFinished')
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notifications] WHERE ID = 1560)
	INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal) VALUES(1560, 85, 'Aviso de estudio finalizado','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',1)
GO

UPDATE sysroLiveAdvancedParameters SET Value = 19 WHERE ParameterName='VTPortalApiVersion'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='563' WHERE ID='DBVersion'
GO
