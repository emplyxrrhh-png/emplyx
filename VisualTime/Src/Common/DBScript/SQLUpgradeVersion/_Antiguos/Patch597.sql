IF EXISTS (SELECT 1 FROM sysroNotificationTypes WHERE ID = 83)
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (83,2,'Body',1,'EmployeeName','PunchBeforeStartSupervisor')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (83,2,'Subject',1,'EmployeeName','PunchBeforeStartSupervisor')
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO
UPDATE sysroParameters SET Data='597' WHERE ID='DBVersion'
GO

