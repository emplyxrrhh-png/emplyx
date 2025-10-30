IF NOT EXISTS (SELECT 1 FROM sysroNotificationTypes WHERE ID = 91)
BEGIN
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem, IDCategory, RequiresPermissionOverEmployees) VALUES(91,'Alert that a document has been rejected',null, -1,'*Documents*','U', 1, 6, 1)
END 

IF NOT EXISTS (SELECT 1 FROM Notifications WHERE ID = 703 AND IDType = 91)
BEGIN
	INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowMail) 
	VALUES(703, 91, 'Documento rechazado','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',1)

	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,1,'Subject',0,'','DocumentRejected.Supervisor')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,1,'Body',1,'EmployeeName','DocumentRejected.Supervisor')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,1,'Body',2,'DocumentType','DocumentRejected.Supervisor')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,1,'Body',3,'Remarks','DocumentRejected.Supervisor')

INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,2,'Subject',0,'','ForecastDocumentRejected.Supervisor')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,2,'Body',1,'EmployeeName','ForecastDocumentRejected.Supervisor')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,2,'Body',2,'DocumentType','ForecastDocumentRejected.Supervisor')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,2,'Body',3,'Remarks','ForecastDocumentRejected.Supervisor')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,2,'Body',4,'DocumentReference','ForecastDocumentRejected.Supervisor')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,2,'Body',5,'BeginDate','ForecastDocumentRejected.Supervisor')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,2,'Body',6,'EndDate','ForecastDocumentRejected.Supervisor')

INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,3,'Subject',0,'','HoursForecastDocumentRejected.Supervisor')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,3,'Body',1,'EmployeeName','HoursForecastDocumentRejected.Supervisor')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,3,'Body',2,'DocumentType','HoursForecastDocumentRejected.Supervisor')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,3,'Body',3,'Remarks','HoursForecastDocumentRejected.Supervisor')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,3,'Body',4,'DocumentReference','HoursForecastDocumentRejected.Supervisor')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,3,'Body',5,'Date','HoursForecastDocumentRejected.Supervisor')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,3,'Body',6,'BeginTime','HoursForecastDocumentRejected.Supervisor')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,3,'Body',7,'EndTime','HoursForecastDocumentRejected.Supervisor')

	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,4,'Subject',0,'','DocumentRejected.Employee')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,4,'Body',1,'DocumentType','DocumentRejected.Employee')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,4,'Body',2,'Remarks','DocumentRejected.Employee')

INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,5,'Subject',0,'','ForecastDocumentRejected.Employee')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,5,'Body',1,'DocumentType','ForecastDocumentRejected.Employee')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,5,'Body',2,'Remarks','ForecastDocumentRejected.Employee')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,5,'Body',3,'DocumentReference','ForecastDocumentRejected.Employee')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,5,'Body',4,'BeginDate','ForecastDocumentRejected.Employee')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,5,'Body',5,'EndDate','ForecastDocumentRejected.Employee')

INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,6,'Subject',0,'','HoursForecastDocumentRejected.Employee')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,6,'Body',1,'DocumentType','HoursForecastDocumentRejected.Employee')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,6,'Body',2,'Remarks','HoursForecastDocumentRejected.Employee')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,6,'Body',3,'DocumentReference','HoursForecastDocumentRejected.Employee')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,6,'Body',4,'Date','HoursForecastDocumentRejected.Employee')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,6,'Body',5,'BeginTime','HoursForecastDocumentRejected.Employee')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (91,6,'Body',5,'EndTime','HoursForecastDocumentRejected.Employee')

END
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='777' WHERE ID='DBVersion'
GO
