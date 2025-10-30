ALTER VIEW [dbo].[sysrovwAllEmployeeMobilities]
	AS
		SELECT IDEmployee, IDGroup, BeginDate, EndDate, Groups.Path FROM dbo.Groups inner JOIN dbo.sysrovwPastEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwPastEmployeeGroups.IDGroup
		UNION
		SELECT IDEmployee, IDGroup, BeginDate, EndDate, Groups.Path FROM dbo.Groups inner JOIN dbo.sysrovwCurrentEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwCurrentEmployeeGroups.IDGroup
		UNION
		SELECT IDEmployee, IDGroup, BeginDate, EndDate, Groups.Path FROM dbo.Groups INNER JOIN dbo.sysrovwFutureEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwFutureEmployeeGroups.IDGroup
GO

CREATE FUNCTION [dbo].[EmployeesWithContractOnGroup]
  (				
	@datebeginpar smalldatetime,
	@dateendpar smalldatetime,
	@idgrouppar int,
	@recursivepar bit
  )
  RETURNS @ValueTable table(IDEmployee int, IDGroup int, DateBegin smalldatetime, DateEnd smalldatetime, Path nvarchar(100)) 
  AS
  BEGIN
    declare @iniperiod smalldatetime 
	declare @endperiod smalldatetime
	declare @idgroup int
	declare @recursive bit

	SET @iniperiod = @datebeginpar
	SET @endperiod = @dateendpar
	SET @idgroup = @idgrouppar
	SET @recursive = @recursivepar

  	INSERT INTO @ValueTable
	select IDEmployee, IDGroup, case when @iniperiod >= temp.mindate then @iniperiod else temp.mindate end as BeginDate, case when @endperiod >= temp.maxdate then temp.maxdate else @endperiod end as EndDate, temp.Path  
	from 
	(
	select ec.idemployee, aeg.idgroup, case when ec.BeginDate >= aeg.begindate then ec.BeginDate else aeg.begindate end as mindate, case when ec.EndDate >= aeg.Enddate then aeg.Enddate else ec.enddate end as maxdate, aeg.Path 
	from dbo.EmployeeContracts ec
	inner join dbo.sysrovwAllEmployeeMobilities aeg on 
			ec.IDEmployee = aeg.idemployee 
		and ec.BeginDate <= aeg.enddate 
		and ec.EndDate >= aeg.begindate 
		and (aeg.idgroup= @idgroup OR aeg.path like concat((select Path from Groups where id= @idgroup),'\%') or @idgroup=-1)
	) temp
	where @iniperiod <= temp.maxdate and @endperiod >= temp.mindate
	IF (@recursive <> 1 and @idgroup<>-1) DELETE @ValueTable WHERE IDGroup <> @idgroup
	RETURN
  END
GO

-- Reglas de saldo negativo para previsiones de ausencias ppor dias y por horas
ALTER TABLE dbo.Causes 
ADD ApplyWorkDaysOnConcept
 bit null DEFAULT(0)
GO

UPDATE dbo.Causes Set ApplyWorkDaysOnConcept=0 where ApplyWorkDaysOnConcept is null
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 7 and IDRuleType=1)
	INSERT INTO sysroRequestRuleTypes (IDRequestType, IDRuleType) values (7,1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 9 and IDRuleType=1)
	INSERT INTO sysroRequestRuleTypes (IDRequestType, IDRuleType) values (9,1)
GO

-- Nueva notificacion para enviar mail de aviso de alerta de incumplimiento de regla de planificación
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 66)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,OnlySystem, Feature,FeatureType) VALUES(66,'Advice for schedule rule non-compliance',null, 120, 0,'Calendar','U')
GO

INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (66,1,'Body',1,'EmployeeName','ScheduleRuleIndictment')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (66,1,'Body',2,'Rulename','ScheduleRuleIndictment')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (66,1,'Body',3,'BeginDate','ScheduleRuleIndictment')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (66,1,'Body',4,'EndDate','ScheduleRuleIndictment')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (66,1,'Body',5,'Detail','ScheduleRuleIndictment')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (66,1,'Subject',1,'Rulename','ScheduleRuleIndictment')
GO

 CREATE VIEW [dbo].[sysrovwAllEmployeeGroupsFull]
 AS
 SELECT        dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                          dbo.Employees.Name AS EmployeeName, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, 
                          COUNT(dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee) AS CurrentEmployee, dbo.Employees.AttControlled, dbo.Employees.AccControlled, 
                          dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, dbo.GetFullGroupPathName(dbo.EmployeeGroups.IDGroup) 
                          AS FullGroupName,  (SELECT CASE LEFT(Path, CHARINDEX('\', Path)) WHEN '' THEN Path ELSE CONVERT(INT, LEFT(Path, CHARINDEX('\', Path) - 1)) END FROM Groups WHERE ID = (dbo.Groups.ID)) AS IDCompany, 
						  dbo.EmployeeGroups.IsTransfer,  dbo.GetEmployeeBusinessCenterNameOnDate(EmployeeGroups.IDEmployee,EmployeeGroups.BeginDate) As CostCenter
 FROM            dbo.EmployeeGroups INNER JOIN
                          dbo.Employees ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID INNER JOIN
                          dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                          dbo.sysrosubvwCurrentEmployeePeriod ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
 GROUP BY dbo.Groups.Name, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
						  dbo.Employees.Name, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, dbo.Employees.AttControlled, dbo.Employees.AccControlled, 
						  dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, dbo.EmployeeGroups.IsTransfer, Groups.ID
GO


-- Valores iniciales con fecha de fin de periodo a partir de campo de la ficha
IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[startupvalues]') AND name = 'ApplyEndCustomPeriod')
ALTER TABLE dbo.StartupValues ADD ApplyEndCustomPeriod bit NULL DEFAULT(0)
GO

UPDATE dbo.StartupValues Set ApplyEndCustomPeriod=0 where ApplyEndCustomPeriod is null
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[startupvalues]') AND name = 'EndCustomPeriodUserField')
ALTER TABLE dbo.StartupValues ADD EndCustomPeriodUserField nvarchar(100) NULL
GO


IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[startupvalues]') AND name = 'ScalingCoefficientUserField')
alter table  [dbo].[startupvalues] add ScalingCoefficientUserField nvarchar(100) default null
go

update dbo.sysroGUI set LanguageReference='DataLinkBusiness', RequiredFeatures='Forms\Datalink', IconURL='ImportExport.png',Parameters='' where IDPath='Portal\General\DataLinkBusiness'
GO

update dbo.sysroGUI set Parameters='', RequiredFeatures='Forms\Datalink;!Feature\DatalinkBusiness' where IDPath='Portal\General\DataLink'
GO

UPDATE dbo.sysroLiveAdvancedParameters SET	Value = '' WHERE ParameterName ='VTLive.AdvancedDatalink'
GO

IF NOT EXISTS (SELECT * FROM sysroGUI_Actions WHERE  IDPath = 'AccessMonitorV2')
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('AccessMonitorV2','Portal\Access\AccessStatus\Child','tbAccMonitor','Forms\Access','U:Access.Zones.Supervision=Read','showAccessStatusMonitorV2','btnTbMonitor3',0,5)
GO

ALTER TABLE dbo.ExportGuides ADD
	[Enabled] bit NULL
GO

ALTER TABLE dbo.Causes ADD	[IDZone] [tinyint] NULL
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'AccessMonitor.User')
insert into sysroLiveAdvancedParameters values ('AccessMonitor.User', 'accessmonitor')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'AccessMonitor.Pass')
insert into sysroLiveAdvancedParameters values ('AccessMonitor.Pass', '4cc3ssm0n1t0r')
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='466' WHERE ID='DBVersion'
GO
