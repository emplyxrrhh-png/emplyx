IF NOT EXISTS (SELECT 1 FROM [dbo].[NotificationMessageParameters] WHERE NotificationLanguageKey = 'NewDocumentDelivered.DaysAbsence')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,2,'Body',7,'DeliveredBy','NewDocumentDelivered.DaysAbsence')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[NotificationMessageParameters] WHERE NotificationLanguageKey = 'NewDocumentDelivered.Overtime')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,4,'Body',6,'DeliveredBy','NewDocumentDelivered.Overtime')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[NotificationMessageParameters] WHERE NotificationLanguageKey = 'NewDocumentDelivered.HourAbsence')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,3,'Body',6,'DeliveredBy','NewDocumentDelivered.HourAbsence')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.ExchangeShiftBetweenEmployees.CompensationRequiered')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.ExchangeShiftBetweenEmployees.CompensationRequiered','false')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 8 and IDRuleType=4)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(8,4)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 13 and IDRuleType=2)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(13,2)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 13 and IDRuleType=4)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(13,4)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 13 and IDRuleType=5)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(13,5)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 13 and IDRuleType=6)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(13,6)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 13 and IDRuleType=7)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(13,7)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 13 and IDRuleType=8)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(13,8)
GO

ALTER TABLE dbo.Causes ADD
	AbsenceMandatoryDays int NULL
GO

ALTER TABLE dbo.Causes ADD CONSTRAINT
	DF_Causes_AbsenceMandatoryDays DEFAULT -1 FOR AbsenceMandatoryDays
GO

UPDATE dbo.Causes SET AbsenceMandatoryDays = -1
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.ExchangeShiftBetweenEmployees.SoftmodeCheck')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.ExchangeShiftBetweenEmployees.SoftmodeCheck','false')
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='455' WHERE ID='DBVersion'
GO
