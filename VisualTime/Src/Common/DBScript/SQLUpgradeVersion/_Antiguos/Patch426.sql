IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'CTaimaUserLinkEnabled')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('CTaimaUserLinkEnabled','0')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'CTaimaUserName')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('CTaimaUserName','')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'CTaimaPassword')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('CTaimaPassword','')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'CTaimaWsURL')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('CTaimaWsURL','')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'CTaimaDebug')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('CTaimaDebug','1')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'CTaimaPRLUserField')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('CTaimaPRLUserField','Entra')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'CTaimaCheckPeriod')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('CTaimaCheckPeriod','00:20@60')
GO

update dbo.sysroGUI_Actions set RequieredFunctionalities = 'U:Documents.DocumentsDefinition=Admin' where IDPath='NewDocument' and IDGUIPath='Portal\GeneralManagement\Documents\Template'
GO

update dbo.sysroGUI_Actions set RequieredFunctionalities = 'U:Documents.DocumentsDefinition=Admin' where IDPath='Del' and IDGUIPath='Portal\GeneralManagement\Documents\Template'
GO

CREATE VIEW [dbo].[sysrovwEmployeeAccessAuthorizations]
AS
	select IDEmployee, IDAuthorization from sysrovwAccessAuthorizations
	group by IDEmployee, IDAuthorization 
GO

update dbo.sysroGUI set RequiredFeatures = 'Process\Notifier' where IDPath = 'Portal\GeneralManagement\Notifications'
GO

UPDATE dbo.sysroParameters SET Data='426' WHERE ID='DBVersion'
GO
