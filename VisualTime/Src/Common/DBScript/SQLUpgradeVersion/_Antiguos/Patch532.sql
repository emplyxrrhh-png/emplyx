ALTER TABLE dbo.sysroLiveTasks ADD
	IsAliveAt datetime NULL
GO

ALTER TABLE dbo.dailySchedule ADD
	[GUID] nvarchar(max) NULL
GO

UPDATE [dbo].[sysroGui] SET Parameters='SaaSEnabled' WHERE URL LIKE '%Surveys%'
GO

update [dbo].[sysroGui] set Parameters = 'SaaSEnabled' where  IDPath= 'Portal\Reports\AdvReport'
GO


IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 79)
BEGIN
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem) VALUES(79,'Alert for user break not started',null, 1,'Calendar.Scheduler','U', 1)
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (79,1,'Subject',0,'','BreakStart')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (79,1,'Body',1,'EmployeeName','BreakStart')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (79,1,'Body',2,'Duration','BreakStart')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (79,1,'Body',3,'Penalty','BreakStart')
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notifications] WHERE ID = 1600)
	INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal) VALUES(1600, 79, 'Aviso de usuario no ha iniciado descanso','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',1)
GO

-- Nueva notificación de cambio en estado de teletrabajo
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 80)
BEGIN
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem) VALUES(80,'Alert for user break not finished',null, 1,'Calendar.Scheduler','U', 1)
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (80,1,'Subject',0,'','BreakFinish')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (80,1,'Body',1,'EmployeeName','BreakFinish')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (80,1,'Body',2,'EndDate','BreakFinish')
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notifications] WHERE ID = 1601)
	INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal) VALUES(1601, 80, 'Aviso de fin de descanso no finalizado','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',1)
GO

-- Nueva notificación de cambio en estado de teletrabajo
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 81)
BEGIN
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem) VALUES(81,'Break not taken',null, 1,'Calendar.Scheduler','U', 1)
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (81,1,'Subject',0,'','BreakNotTaken')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (81,1,'Body',1,'EmployeeName','BreakNotTaken')
    INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (81,1,'Body',2,'BreakDate','BreakNotTaken')
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notifications] WHERE ID = 1602)
	INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID,AllowMail, AllowPortal) VALUES(1602, 81, 'Aviso de descanso no realizado','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',1,1)
GO

ALTER TABLE dbo.Shifts ADD
    WhoToNotifyBefore int NULL,
	WhoToNotifyAfter int NULL,
	NotifyEmployeeBeforeAt int NULL,
	NotifyEmployeeAfterAt int NULL,
    EnableNotifyBefore bit NULL,
	EnableNotifyAfter bit NULL
GO

ALTER table EmployeeStatus ADD  StartLimit smalldatetime default null
GO

insert into sysroNotificationTypes values (84, 'Employee Not Arrived Before Start Limit', null, 1, 'Calendar', 'U', 1, 6)
GO

INSERT [dbo].[Notifications] ([ID], [IDType], [Name], [Condition], [Destination], [Schedule], [Activated], [CreatorID], [AllowPortal], [IDPassportDestination], [AllowMail], [ShowOnDesktop], [LastTaskDeleted], [AllowVTPortal], [InProgress], [GUID], [LastCheck]) VALUES (1017, 84, N'Usuario no ha fichado antes de la hora límite', N'<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', NULL, 0, N'SYSTEM', 1, 0, 0, 1, NULL, NULL, 0, NULL, NULL)
GO


INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (84,1,'Body',1,'EmployeeName','EmployeeNotArrivedBeforeStartLimitForSupervisor')
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (84,1,'Subject',1,'EmployeeName','EmployeeNotArrivedBeforeStartLimitForSupervisor')
GO

IF NOT EXISTS (SELECT 1 FROM Notifications WHERE ID = 4920 AND IDType = 83)
	INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) 
	VALUES(4920, 83, 'Fichajes antes inicio jornada','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO

IF NOT EXISTS (SELECT 1 FROM sysroNotificationTypes WHERE ID = 83)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem) VALUES(83,'Alert for punch before start',null, 1,'Calendar.Shceduler','U', 1)
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (83,1,'Body',1,'EmployeeName','PunchBeforeStart')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (83,1,'Subject',0,'','PunchBeforeStart')

GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='532' WHERE ID='DBVersion'
GO
