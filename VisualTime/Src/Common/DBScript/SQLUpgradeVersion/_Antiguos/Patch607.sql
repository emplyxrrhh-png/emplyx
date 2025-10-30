INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (84,2,'Body',1,'EmployeeName','EmployeeNotArrivedBeforeStartLimit')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (84,2,'Subject',1,'EmployeeName','EmployeeNotArrivedBeforeStartLimit')
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='607' WHERE ID='DBVersion'
GO
