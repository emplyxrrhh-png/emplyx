IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 86)
BEGIN
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem) VALUES(86,'Alert for max capacity reached',null, 360,'','', 1)
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (86,1,'Subject',0,'','CapacityReached.Reached')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (86,1,'Body',1,'Zone','CapacityReached.Reached')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (86,1,'Body',2,'Date','CapacityReached.Reached')

	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (86,2,'Subject',0,'','CapacityReached.Available')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (86,2,'Body',1,'Zone','CapacityReached.Available')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (86,2,'Body',2,'Date','CapacityReached.Available')
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notifications] WHERE ID = 1310)
	INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal) VALUES(1310, 86, 'Aviso de aforo máximo alcanzado','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',1)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO
UPDATE sysroParameters SET Data='593' WHERE ID='DBVersion'
GO

