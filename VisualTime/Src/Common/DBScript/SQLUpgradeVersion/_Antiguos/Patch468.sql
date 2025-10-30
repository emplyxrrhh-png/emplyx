ALTER TABLE [dbo].[EventAccessAuthorization] DROP CONSTRAINT [PK_EventAccessAuthorization] WITH ( ONLINE = OFF )
GO

ALTER TABLE [dbo].[EventAccessAuthorization] ADD [Date] smalldatetime NOT NULL DEFAULT (convert(smalldatetime,'1900-01-01',120))
GO

UPDATE eaa
SET eaa.Date = es.Date
FROM [dbo].[EventAccessAuthorization] as eaa
INNER JOIN [dbo].[EventsScheduler] as es ON es.ID = eaa.IDEvent
GO

ALTER TABLE [dbo].[EventAccessAuthorization] ADD  CONSTRAINT [PK_EventAccessAuthorization] PRIMARY KEY CLUSTERED 
(
	[IDEvent] ASC,
	[IDAuthorization] ASC,
	[Date] ASC
)
GO

ALTER TABLE [dbo].[EventsScheduler] ADD [EndDate] smalldatetime NULL
GO

ALTER TABLE [dbo].[EventsScheduler] ADD [MainDate] smalldatetime NULL
GO

UPDATE [dbo].[EventsScheduler] SET EndDate = Date
GO

UPDATE [dbo].[EventsScheduler] SET MainDate = EndDate
GO

alter table [dbo].[groups]
add FullGroupName nvarchar(max)
GO

update [dbo].[groups]
set FullGroupName= dbo.GetFullGroupPathName(id)
GO

create view [dbo].[sysrovwGetEmployeeGroup] as
select t1.idemployee, t1.idgroup
,case when t1.rowid=1 then convert(smalldatetime,'1900.01.01',120) else t1.begindate  end as begindate
, t1.enddate, t1.IsTransfer
, t3.Name as GroupName,  t3.FullGroupName, t3.IDCenter
, SUBSTRING (path,0, CHARINDEX('\', Path+'\')) as IDCompany
, t1.begindate As RealBeginDate
from (select *, ROW_NUMBER () over (partition by idemployee order by begindate asc ) as rowid
		from EmployeeGroups ) T1
	inner join Groups T3
	on T1.IDGroup = T3.ID 
GO

 ALTER VIEW [dbo].[sysrovwCurrentEmployeeGroups]
 AS
 SELECT        dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                          dbo.Employees.Name AS EmployeeName, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, 
                          COUNT(dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee) AS CurrentEmployee, dbo.Employees.AttControlled, dbo.Employees.AccControlled, 
                          dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, FullGroupName, dbo.EmployeeGroups.IsTransfer
 FROM            dbo.EmployeeGroups 
 INNER JOIN dbo.Employees 
ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID 
 INNER JOIN  dbo.Groups
 ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID
 LEFT OUTER JOIN dbo.sysrosubvwCurrentEmployeePeriod 
 ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
 GROUP BY dbo.Groups.Name, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, dbo.Employees.Name, 
                          dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, dbo.Employees.AttControlled, dbo.Employees.AccControlled, dbo.Employees.JobControlled, 
                          dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, dbo.EmployeeGroups.IsTransfer, FullGroupName
 HAVING        CONVERT(date, getdate()) between dbo.EmployeeGroups.BeginDate and dbo.EmployeeGroups.EndDate
GO

 ALTER VIEW [dbo].[sysrovwFutureEmployeeGroups]
 AS
 SELECT        dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                          dbo.Employees.Name AS EmployeeName, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, '0' AS CurrentEmployee, dbo.Employees.AttControlled, 
                          dbo.Employees.AccControlled, dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, 
                          FullGroupName, dbo.EmployeeGroups.IsTransfer
 FROM            dbo.EmployeeGroups INNER JOIN
                          dbo.Employees ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID INNER JOIN
                          dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                          dbo.sysrosubvwCurrentEmployeePeriod ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
 WHERE        dbo.EmployeeGroups.BeginDate > CONVERT(date, getdate())
GO

 ALTER VIEW [dbo].[sysrovwAllEmployeeGroups]
 AS
 SELECT        geg.GroupName, sysrovwCurrentEmployeeGroups.Path, sysrovwCurrentEmployeeGroups.SecurityFlags, geg.IDEmployee, geg.IDGroup, EmployeeName, geg.RealBeginDate as BeginDate, geg.EndDate
 , CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled
 , geg.FullGroupName, geg.IDCompany, 
    sysrovwCurrentEmployeeGroups.isTransfer,'ERROR' As CostCenter
 FROM            dbo.sysrovwGetEmployeeGroup geg
 INNER JOIN dbo.sysrovwCurrentEmployeeGroups
 ON geg.IDGroup = dbo.sysrovwCurrentEmployeeGroups.IDGroup and geg.IDEmployee=dbo.sysrovwCurrentEmployeeGroups.IDEmployee
 UNION
 SELECT        geg.GroupName, sysrovwFutureEmployeeGroups.Path, sysrovwFutureEmployeeGroups.SecurityFlags, geg.IDEmployee, geg.IDGroup, EmployeeName, geg.RealBeginDate, geg.EndDate
 , CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled
 , geg.FullGroupName, geg.IDCompany, 
    sysrovwFutureEmployeeGroups.isTransfer,'ERROR' As CostCenter
 FROM            dbo.sysrovwGetEmployeeGroup geg
 INNER JOIN dbo.sysrovwFutureEmployeeGroups 
 ON geg.IDgroup = dbo.sysrovwFutureEmployeeGroups.IDGroup and geg.IDEmployee=dbo.sysrovwFutureEmployeeGroups.IDEmployee
GO

CREATE VIEW [dbo].[sysrovwAllEmployeeGroupsWithCostCenter]
 AS
 SELECT        geg.GroupName, sysrovwCurrentEmployeeGroups.Path, sysrovwCurrentEmployeeGroups.SecurityFlags, geg.IDEmployee, geg.IDGroup, EmployeeName, geg.RealBeginDate as BeginDate, geg.EndDate
 , CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled
 , geg.FullGroupName, geg.IDCompany, 
    sysrovwCurrentEmployeeGroups.isTransfer,dbo.GetEmployeeBusinessCenterNameOnDate(sysrovwCurrentEmployeeGroups.IDEmployee,sysrovwCurrentEmployeeGroups.BeginDate) As CostCenter
 FROM            dbo.sysrovwGetEmployeeGroup geg
 INNER JOIN dbo.sysrovwCurrentEmployeeGroups
 ON geg.IDGroup = dbo.sysrovwCurrentEmployeeGroups.IDGroup and geg.IDEmployee=dbo.sysrovwCurrentEmployeeGroups.IDEmployee
 UNION
 SELECT        geg.GroupName, sysrovwFutureEmployeeGroups.Path, sysrovwFutureEmployeeGroups.SecurityFlags, geg.IDEmployee, geg.IDGroup, EmployeeName, geg.RealBeginDate, geg.EndDate
 , CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled
 , geg.FullGroupName, geg.IDCompany, 
    sysrovwFutureEmployeeGroups.isTransfer,dbo.GetEmployeeBusinessCenterNameOnDate(sysrovwFutureEmployeeGroups.IDEmployee,sysrovwFutureEmployeeGroups.BeginDate) As CostCenter
 FROM            dbo.sysrovwGetEmployeeGroup geg
 INNER JOIN dbo.sysrovwFutureEmployeeGroups 
 ON geg.IDgroup = dbo.sysrovwFutureEmployeeGroups.IDGroup and geg.IDEmployee=dbo.sysrovwFutureEmployeeGroups.IDEmployee
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI] WHERE IDPath = 'Portal\Company')
insert into [dbo].[sysroGUI](IDPath,LanguageReference,URL,IconURL,Type,Parameters,RequiredFeatures,SecurityFlags,Priority,AllowedSecurity,RequiredFunctionalities)
values ('Portal\Company','Company',NULL,'Company.png',NULL,NULL,NULL,NULL,1002,NULL,NULL)
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI] WHERE IDPath = 'Portal\Company\Groups')
insert into [dbo].[sysroGUI](IDPath,LanguageReference,URL,IconURL,Type,Parameters,RequiredFeatures,SecurityFlags,Priority,AllowedSecurity,RequiredFunctionalities)
values ('Portal\Company\Groups','Gui.Groups','Employees/Groups.aspx','Groups.png',NULL,NULL,'Forms\Employees',NULL,101,NULL,'U:Employees=Read')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI] WHERE IDPath = 'Portal\Company\Company')
insert into [dbo].[sysroGUI](IDPath,LanguageReference,URL,IconURL,Type,Parameters,RequiredFeatures,SecurityFlags,Priority,AllowedSecurity,RequiredFunctionalities)
values ('Portal\General\Company','Gui.Company','Employees/Company.aspx','Company.png',NULL,NULL,'Forms\Employees',NULL,101,NULL,'U:Employees=Read')
GO

update [dbo].[sysroGUI] set Priority=102 where IDPath='Portal\Company\Groups'
GO
update [dbo].[sysroGUI] set IDPath = 'Portal\Company\SecurityChart',Priority=105 where IDPath='Portal\General\SecurityChart'
GO
update [dbo].[sysroGUI] set IDPath = 'Portal\Company\SecurityFunctions',Priority=104 where IDPath='Portal\Security\SecurityFunctions'
GO
update [dbo].[sysroGUI] set IDPath = 'Portal\Company\supervisors',Priority=103 where IDPath='Portal\Security\supervisors'
GO
update [dbo].[sysroGUI] set IDPath = 'Portal\Company\Passports', IconURL = 'Supervisors.png' ,Priority=103 where IDPath='Portal\Security\Passports'
GO


update [dbo].[sysroGUI_Actions] set IDGUIPath='Portal\General\Employees\GroupsLite' where IDPath ='MaxMinimize' AND IDGUIPath = 'Portal\General\Employees\Groups'
GO
update [dbo].[sysroGUI_Actions] set IDGUIPath='Portal\General\Employees\GroupsLite' where IDPath ='Reports' AND IDGUIPath = 'Portal\General\Employees\Groups'
GO
delete [dbo].[sysroGUI_Actions] where IDPath ='NewGroup' AND IDGUIPath = 'Portal\General\Employees\Groups'
GO
delete [dbo].[sysroGUI_Actions] where IDPath ='NewCompany' AND IDGUIPath = 'Portal\General\Employees\Groups'
GO
delete [dbo].[sysroGUI_Actions] where IDPath ='DeleteGroup' AND IDGUIPath = 'Portal\General\Employees\Groups'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI_Actions] WHERE [IDPath] = 'MaxMinimize' AND [IDGUIPath] = 'Portal\Company\Groups')
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) 
	VALUES ('MaxMinimize','Portal\Company\Groups','tbMaximize','Forms\Employees',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI_Actions] WHERE [IDPath] = 'DeleteGroup' AND [IDGUIPath] = 'Portal\Company\Groups')
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) 
	VALUES ('DeleteGroup','Portal\Company\Groups','tbdelGroup','Forms\Employees','U:Employees=Admin','ShowRemoveGroup(''#ID#'')','btnTbDel2',0,4)
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI_Actions] WHERE [IDPath] = 'Reports' AND [IDGUIPath] = 'Portal\Company\Groups')
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) 
	VALUES ('Reports','Portal\Company\Groups','tbShowReports','Forms\Employees',NULL,'ShowReports','btnTbPrint2',0,5)
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI_Actions] WHERE [IDPath] = 'NewGroup' AND [IDGUIPath] = 'Portal\Company\Groups')
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) 
	VALUES ('NewGroup','Portal\Company\Groups','tbAddNewGroup','Forms\Employees','U:Employees.Groups=Admin','ShowNewGroupWizard()','btnTbAddGroup2',0,2)
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI_Actions] WHERE [IDPath] = 'NewCompany' AND [IDGUIPath] = 'Portal\Company\Groups')
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) 
	VALUES ('NewCompany','Portal\Company\Groups','tbAddNewCompany','Feature\MultiCompany','U:Employees.Groups=Admin','ShowNewCompanyWizard()','btnTbAddCompany2',0,3)
GO

ALTER VIEW [dbo].[sysrovwGetPermissionOverEmployee] AS
SELECT        pp.idPassport AS PassportID, pp.idParentPassport AS ParentPassportID, pog.EmployeeFeatureID, pog.EmployeeGroupID, eg.IDEmployee AS EmployeeID, pog.Permission, pog.LevelOfAuthority, ISNULL(poe.Permission, 9) 
                         AS EmployeesExceptionsPermission, CASE WHEN pog.Permission > isnull(poe.Permission, 9) THEN isnull(poe.Permission, 9) ELSE pog.Permission END AS CalculatedPermission, eg.BeginDate, eg.EndDate
FROM            dbo.sysrovwParentPassports AS pp
INNER JOIN dbo.sysroPermissionsOverGroups AS pog 
ON pp.idParentPassport = pog.PassportID 
INNER JOIN dbo.sysrovwGetEmployeeGroup AS eg 
ON pog.EmployeeGroupID = eg.IDGroup
LEFT OUTER JOIN dbo.sysroPermissionsOverEmployeesExceptions AS poe 
ON eg.IDEmployee = poe.EmployeeID AND poe.EmployeeFeatureID = pog.EmployeeFeatureID AND poe.PassportID = pog.PassportID
GO

 ALTER VIEW [dbo].[sysrovwEmployeeUserFieldValues]
 AS
 select efv1.[IDEmployee],efv1.FieldName,CONVERT(NVARCHAR(4000),efv1.[Value]) as [Value],efv1.[Date] as BeginDate, isnull(dateadd(n,-1,efv2.Date),convert(smalldatetime,'2079-01-01',120)) as EndDate
   from (SELECT [IDEmployee],FieldName,[Value],[Date]
 	  , ROW_NUMBER() over (partition by [IDEmployee], fieldname order by date desc) as rowid
   FROM [EmployeeUserFieldValues]
   ) efv1
   left outer join 
   (SELECT [IDEmployee],FieldName,[Value],[Date]
 	  , ROW_NUMBER() over (partition by [IDEmployee], fieldname order by date desc) as rowid
   FROM [EmployeeUserFieldValues]
   ) efv2
   on efv1.IDEmployee=efv2.IDEmployee 
   and efv1.FieldName=efv2.FieldName
   and efv1.rowid-1 = efv2.rowid 
GO

CREATE VIEW sysrovwGetPermissionOverFeature AS
select pp.idPassport, pof.FeatureID, rf.Alias, Permission
from  dbo.sysrovwParentPassports AS pp
inner join sysroPermissionsOverFeatures pof
on pof.PassportID= pp.idParentPassport 
left outer join sysroFeatures rf
on rf.id=pof.FeatureID
where rf.Type = 'U'
GO

CREATE VIEW sysrovwGetPermissionOverGroup as
select pp.idPassport, pog.EmployeeGroupID, pog.EmployeeFeatureID, LevelOfAuthority, Permission
from dbo.sysrovwParentPassports AS pp
INNER JOIN dbo.sysroPermissionsOverGroups AS pog 
ON pp.idParentPassport = pog.PassportID 
GO

-- Nueva notificacion para enviar mail a empleado de suplantación en solicitud
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 67)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,OnlySystem, Feature,FeatureType) VALUES(67,'Advice for request impersonation',null, 360, 0,'','')
GO

-- Textos
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (67,1,'Body',1,'SupervisorName','RequestImpersonation')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (67,1,'Body',2,'RequestType','RequestImpersonation')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (67,1,'Body',3,'RequestDetail','RequestImpersonation')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (67,1,'Body',4,'RequestComments','RequestImpersonation')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (67,1,'Subject',1,'RequestType','RequestImpersonation')
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='468' WHERE ID='DBVersion'
GO
