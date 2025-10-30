ALTER TABLE dbo.DailySchedule
	DROP CONSTRAINT FK_DailySchedule_sysroRemarks
GO


ALTER TABLE dbo.DailySchedule  ADD CONSTRAINT
	FK_DailySchedule_sysroRemarks FOREIGN KEY
	(
	Remarks
	) REFERENCES dbo.sysroRemarks
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  SET NULL 
	
GO

-- IsTransfer
ALTER VIEW [dbo].[sysrovwCurrentEmployeeGroups]
AS
SELECT        dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                         dbo.Employees.Name AS EmployeeName, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, 
                         COUNT(dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee) AS CurrentEmployee, dbo.Employees.AttControlled, dbo.Employees.AccControlled, 
                         dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, dbo.GetFullGroupPathName(dbo.EmployeeGroups.IDGroup) 
                         AS FullGroupName, dbo.EmployeeGroups.IsTransfer
FROM            dbo.EmployeeGroups INNER JOIN
                         dbo.Employees ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID INNER JOIN
                         dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                         dbo.sysrosubvwCurrentEmployeePeriod ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
GROUP BY dbo.Groups.Name, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, dbo.Employees.Name, 
                         dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, dbo.Employees.AttControlled, dbo.Employees.AccControlled, dbo.Employees.JobControlled, 
                         dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, dbo.EmployeeGroups.IsTransfer
HAVING        (dbo.EmployeeGroups.EndDate >= CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102)) AND 
                         (dbo.EmployeeGroups.BeginDate <= CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102))

GO

ALTER VIEW [dbo].[sysrovwFutureEmployeeGroups]
AS
SELECT        dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                         dbo.Employees.Name AS EmployeeName, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, '0' AS CurrentEmployee, dbo.Employees.AttControlled, 
                         dbo.Employees.AccControlled, dbo.Employees.JobControlled, dbo.Employees.ExtControlled, dbo.Employees.RiskControlled, 
                         dbo.GetFullGroupPathName(dbo.EmployeeGroups.IDGroup) AS FullGroupName, dbo.EmployeeGroups.IsTransfer
FROM            dbo.EmployeeGroups INNER JOIN
                         dbo.Employees ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID INNER JOIN
                         dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                         dbo.sysrosubvwCurrentEmployeePeriod ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
WHERE        (dbo.EmployeeGroups.BeginDate > CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102))

GO

ALTER VIEW [dbo].[sysrovwAllEmployeeGroups]
AS
SELECT        GroupName, sysrovwCurrentEmployeeGroups.Path, sysrovwCurrentEmployeeGroups.SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, 
                         CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName, sysrovwCurrentEmployeeGroups.isTransfer, 
                             (SELECT        CASE LEFT(Path, CHARINDEX('\', Path)) WHEN '' THEN Path ELSE CONVERT(INT, LEFT(Path, CHARINDEX('\', Path) - 1)) END
                               FROM            Groups
                               WHERE        ID = (dbo.Groups.ID)) AS IDCompany
FROM            dbo.Groups INNER JOIN
                         dbo.sysrovwCurrentEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwCurrentEmployeeGroups.IDGroup
UNION
SELECT        GroupName, sysrovwFutureEmployeeGroups.Path, sysrovwFutureEmployeeGroups.SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, 
                         CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName,sysrovwFutureEmployeeGroups .isTransfer,
                             (SELECT        CASE LEFT(Path, CHARINDEX('\', Path)) WHEN '' THEN Path ELSE CONVERT(INT, LEFT(Path, CHARINDEX('\', Path) - 1)) END
                               FROM            Groups
                               WHERE        ID = (dbo.Groups.ID)) AS IDCompany
FROM            dbo.Groups INNER JOIN
                         dbo.sysrovwFutureEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwFutureEmployeeGroups.IDGroup

GO

ALTER TABLE dbo.Visit_Fields ADD [visible] [bit] NOT NULL DEFAULT 1
GO

ALTER TABLE dbo.Visitor_Fields ADD [visible] [bit] NOT NULL DEFAULT 1
GO

ALTER TABLE dbo.Visitor_Fields_Value ADD
	Modified smalldatetime NOT NULL CONSTRAINT DF_Visitor_Fields_Value_Modified DEFAULT getdate()
GO

ALTER TABLE dbo.Visit ADD Ticket smallint NULL
GO

INSERT INTO [dbo].[sysroNotificationTypes] ([ID],[Name],[Description],[Scheduler],[Feature],[FeatureType],[OnlySystem])     
VALUES(45,'Visit Update',null,360,'Employees','U',1)
GO

INSERT INTO [dbo].[Notifications]([ID],[IDType],[Name],[Condition],[Destination],[Schedule],[Activated],[CreatorID],[AllowPortal],[IDPassportDestination],[AllowMail],[ShowOnDesktop],[LastTaskDeleted])
     VALUES(4501,45,'Modificacion de visitas','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>'
	 ,'<?xml version="1.0"?><roCollection version="2.0"></roCollection>',null,1,'SYSTEM',0,0,0,0,null)
GO



 
UPDATE dbo.sysroParameters SET Data='360' WHERE ID='DBVersion'
GO