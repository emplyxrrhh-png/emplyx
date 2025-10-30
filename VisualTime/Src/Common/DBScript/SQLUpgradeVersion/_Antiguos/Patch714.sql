-- No borréis esta línea
IF NOT EXISTS (SELECT 1 FROM sysroNotificationTypes WHERE ID = 89)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem) VALUES(89,'Alert New Message From Employee In Channel',null, 1,'Employees.Channels','U', 1)
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (89,1,'Body',1,'EmployeeName','NewAnswerFromEmployee')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (89,1,'Body',2,'ChannelName','NewAnswerFromEmployee')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (89,1,'Body',3,'ConversationName','NewAnswerFromEmployee')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (89,1,'Body',4,'ConversationReference','NewAnswerFromEmployee')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (89,1,'Subject',0,'','NewAnswerFromEmployee')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (89,2,'Body',1,'EmployeeName','NewAnswerFromEmployee.NewConversation')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (89,2,'Body',2,'ChannelName','NewAnswerFromEmployee')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (89,2,'Body',3,'ConversationName','NewAnswerFromEmployee')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (89,2,'Body',4,'ConversationReference','NewAnswerFromEmployee')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (89,2,'Subject',0,'','NewAnswerFromEmployee.NewConversation')
GO


IF NOT EXISTS (SELECT 1 FROM Notifications WHERE ID = 4980 AND IDType = 89)
	INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowMail) 
	VALUES(4980, 89, 'Nuevo mensaje del empleado en el canal','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',1)
GO




IF NOT EXISTS (SELECT 1 FROM sysroNotificationTypes WHERE ID = 90)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem) VALUES(90,'Advice New conversation created',null, 1,'','', 1)
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (90,1,'Body',1,'ChannelName','AdviceForNewConversation')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (90,1,'Body',2,'ConversationName','AdviceForNewConversation')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (90,1,'Body',3,'ConversationReference','AdviceForNewConversation')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (90,1,'Subject',0,'','AdviceForNewConversation')
GO


IF NOT EXISTS (SELECT 1 FROM Notifications WHERE ID = 4990 AND IDType = 90)
	INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowMail) 
	VALUES(4990, 90, 'Aviso nueva conversación creada','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',1)
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='714' WHERE ID='DBVersion'
GO
