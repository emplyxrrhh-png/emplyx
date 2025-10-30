IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 73)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,OnlySystem, Feature,FeatureType,IDCategory) VALUES(73,'Employee with high temperature',null, 1, 0,'Calendar.Punches','U',6)
GO
update sysronotificationtypes set Name='Employee without mask' where id=69
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'VTPortal.RequestOnForgottenPunch')
insert into sysroLiveAdvancedParameters values ('VTPortal.RequestOnForgottenPunch', 'True')
GO

--Gestión del menú para Gestión Documental
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI] WHERE [URL] = '/SSO')
    insert into [dbo].sysroGUI(IDPath,LanguageReference,URL,IconURL,Type,Parameters,RequiredFeatures,SecurityFlags,Priority,AllowedSecurity,RequiredFunctionalities)
         values ('Portal\Configuration\SSO','SSO','/SSO','SSO.png',NULL,'MonoTenant',NULL,NULL,106,NULL,'U:Administration.SecurityOptions=Write')
GO

UPDATE sysroParameters SET Data='516' WHERE ID='DBVersion'
GO

 