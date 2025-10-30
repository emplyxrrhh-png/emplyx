-- No borréis esta línea
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (89,3,'Body',1,'ConversationReference','newanswerfromemployee.NewComplaint')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (89,3,'Subject',0,'','newanswerfromemployee.NewComplaint')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (89,4,'Body',1,'ConversationReference','newanswerfromemployee.NewComplaintMessage')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (89,4,'Subject',0,'','newanswerfromemployee.NewComplaintMessage')
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='739' WHERE ID='DBVersion'
GO
