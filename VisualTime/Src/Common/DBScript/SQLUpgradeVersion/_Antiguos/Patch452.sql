--Tabla de definición de mensajes para notificaciones
CREATE TABLE [dbo].[NotificationMessageParameters](
	[IDNotificationType] [smallint] NOT NULL,
	[Scenario] [int] NOT NULL,
	[Scope] [varchar](15) NOT NULL,
	[Parameter] [int] NOT NULL,
	[ParameterLanguageKey] [nvarchar](50) NOT NULL,
	[NotificationLanguageKey] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_NotificationMessages] PRIMARY KEY CLUSTERED 
(
	[IDNotificationType] ASC,
	[Scenario] ASC,
	[Scope] ASC,
	[Parameter] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER PROCEDURE [dbo].[Analytics_Requests_Schedule]      
      @initialDate smalldatetime,      
      @endDate smalldatetime,      
      @idpassport int,      
      @employeeFilter nvarchar(max),      
      @userFieldsFilter nvarchar(max),    
   @requestTypesFilter nvarchar(max)      
 AS      
      DECLARE @employeeIDs Table(idEmployee int)      
   DECLARE @requestTypeFilter Table(idRequest int)      
      insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter,@initialDate,@endDate;      
   insert into @requestTypeFilter select * from dbo.SplitToInt(@requestTypesFilter,',');       
      SELECT     dbo.requests.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee,       
                            dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Requests.RequestDate as 'Date',       
                            dbo.Requests.Status AS RequestsStatus,       
           Case       
        WHEN Requests.Status IN(2,3) THEN (SELECT top 1 isnull(sysropassports.Name,'') from RequestsApprovals inner join sysroPassports on RequestsApprovals.IDPassport = sysroPassports.ID  where IDRequest = Requests.ID AND Status in(2,3) )      
        ELSE '---'      
        END as 'ApproveRefusePassport',      
        Case       
         WHEN Requests.Status IN(0,1) Then (select dbo.GetRequestNextLevelPassports(Requests.ID))      
         ELSE '---'      
        END as 'PendingPassport',      
        dbo.Requests.RequestType,      
        isnull(CONVERT(NVARCHAR(4000), dbo.Requests.Date1, 120),'') as 'BeginDateRequest',      
        isnull(CONVERT(NVARCHAR(4000), dbo.Requests.FromTime, 8),'') as 'BeginHourRequest',      
        isnull(CONVERT(NVARCHAR(4000), dbo.Requests.Date2, 120),'') as 'EndDateRequest',      
        isnull(CONVERT(NVARCHAR(4000), dbo.Requests.ToTime, 8),'') as 'EndHourRequest',      
        ISNULL((SELECT Name from dbo.Causes WHERE ID = dbo.Requests.IDCause),'') as 'CauseNameRequest',      
        ISNULL((SELECT Name from dbo.Shifts WHERE ID = dbo.Requests.IDShift),'') as 'ShiftNameRequest',      
        CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')) as CommentsRequest,      
                            CONVERT(nvarchar(8), dbo.Requests.RequestDate, 108) AS Time, DAY(dbo.Requests.RequestDate) AS Day,       
                            MONTH(dbo.Requests.RequestDate) AS Month, YEAR(dbo.Requests.RequestDate) AS Año, (DATEPART(dw, dbo.Requests.RequestDate) + @@DATEFIRST - 1 - 1)       
                            % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate,       
                            dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,       
             dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,      
           dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,    
     dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,      
           dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,    
     dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,      
           dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,    
     dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,      
           dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,    
     dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,      
           dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,    
     dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,      
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),@endDate) As UserField1,      
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),@endDate) As UserField2,      
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),@endDate) As UserField3,      
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),@endDate) As UserField4,      
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),@endDate) As UserField5,      
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),@endDate) As UserField6,      
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),@endDate) As UserField7,      
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),@endDate) As UserField8,      
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),@endDate) As UserField9,      
           dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),@endDate) As UserField10      
                
      FROM         dbo.sysroEmployeeGroups with (nolock) INNER JOIN      
                            dbo.requests with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.requests.IDEmployee AND dbo.requests.RequestDate >= dbo.sysroEmployeeGroups.BeginDate AND       
                            dbo.requests.RequestDate <= dbo.sysroEmployeeGroups.EndDate INNER JOIN      
                            dbo.EmployeeContracts with (nolock) ON dbo.requests.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.requests.RequestDate >= dbo.EmployeeContracts.BeginDate AND       
                            dbo.requests.RequestDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN      
                            dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID       
      WHERE dbo.GetRequestPassportPermission(@idpassport, Requests.ID) > 1      
        AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.requests.RequestDate between @initialDate and @endDate      
        and dbo.Requests.RequestType in (select idRequest from @requestTypeFilter)    
      GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.requests.RequestDate), YEAR(dbo.requests.RequestDate), dbo.requests.ID,       
                            dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.requests.RequestDate), dbo.requests.RequestDate, CONVERT(nvarchar(8), dbo.requests.RequestDate, 108),       
                            dbo.requests.RequestDate,  dbo.EmployeeContracts.IDContract, dbo.Employees.Name,       
                            (DATEPART(dw, dbo.requests.RequestDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.requests.RequestType, dbo.requests.Status, dbo.requests.Date1, dbo.requests.FromTime, dbo.requests.Date2, dbo.requests.ToTime, dbo.requests.IDCause, dbo.requests.IDShift,CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')), dbo.sysroEmployeeGroups.Path,       
                            dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,       
        dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate      
      HAVING      (dbo.Requests.RequestType IN(2,3,4,5,6,7,8,9,11,14,15))
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRuleType] WHERE Type = 'AutomaticRejection')
INSERT INTO [dbo].[sysroRuleType] (ID, TYPE) VALUES(9,'AutomaticRejection')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 6 and IDRuleType=9)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(6,9)
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 13 and IDRuleType=9)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(13,9)
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 7 and IDRuleType=9)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(7,9)
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroScheduleRulesTypes] WHERE ID = 12 )
INSERT INTO [dbo].[sysroScheduleRulesTypes] ([ID], [Name], [System]) VALUES(12,'MinMaxDaysSequence', 0)
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[startupvalues]') AND name = 'ScalingValues')
alter table  [dbo].[startupvalues] add ScalingValues nvarchar(100) default null
go
IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[startupvalues]') AND name = 'ScalingUserField')
alter table  [dbo].[startupvalues] add ScalingUserField nvarchar(100) default null
go
IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'[dbo].[startupvalues]') AND name = 'CalculatedType')
alter table  [dbo].[startupvalues] add CalculatedType int default 0
go

ALTER TABLE [dbo].[Punches] ADD DebugInfo nvarchar(100) 
GO

DELETE dbo.NotificationMessageParameters
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (1,1,'Body',1,'EmployeeName','BeforeBeginContract')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (1,1,'Body',2,'BeginDate','BeforeBeginContract')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (1,1,'Subject',0,'','BeforeBeginContract')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (2,1,'Body',1,'EmployeeName','BeforeFinishContract')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (2,1,'Body',2,'EndDate','BeforeFinishContract')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (2,1,'Subject',0,'','BeforeFinishContract')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (3,1,'Body',1,'EmployeeName','BeforeProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (3,1,'Body',2,'BeginDate','BeforeProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (3,1,'Body',3,'Cause','BeforeProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (3,1,'Subject',0,'','BeforeProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (4,1,'Body',1,'EmployeeName','BeginProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (4,1,'Body',2,'Cause','BeginProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (4,1,'Subject',0,'','BeginProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (5,1,'Body',1,'EmployeeName','BeforeFinishProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (5,1,'Body',2,'EndDate','BeforeFinishProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (5,1,'Body',3,'EndDate','BeforeFinishProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (5,1,'Subject',0,'','BeforeFinishProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (6,1,'Body',1,'EmployeeName','NoWorkingDays')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (6,1,'Body',2,'Day','NoWorkingDays')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (6,1,'Subject',0,'','NoWorkingDays')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (7,1,'Body',1,'EmployeeName','CutProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (7,1,'Body',2,'Day','CutProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (7,1,'Subject',0,'','CutProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (8,1,'Body',1,'EmployeeName','PunchesWithIncidences')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (8,1,'Body',2,'Day','PunchesWithIncidences')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (8,1,'Body',3,'Cause','PunchesWithIncidences')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (8,1,'Subject',0,'','PunchesWithIncidences')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (9,1,'Body',1,'EmployeeName','InvalidAccess')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (9,1,'Body',2,'Zone','InvalidAccess')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (9,1,'Body',3,'Time','InvalidAccess')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (9,1,'Subject',0,'','InvalidAccess')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (10,1,'Body',0,'','ErrorMessages')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (10,1,'Subject',0,'','ErrorMessages')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (11,1,'Body',1,'TerminalName','TerminalConnected')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (11,1,'Body',2,'Time','TerminalConnected')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (11,1,'Subject',0,'','TerminalConnected')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (11,2,'Body',1,'TerminalName','TerminalDisconnected')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (11,2,'Body',2,'Time','TerminalDisconnected')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (11,2,'Subject',0,'','TerminalDisconnected')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (12,1,'Body',1,'PassportName','AccessDenniedFramework')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (12,1,'Body',2,'Time','AccessDenniedFramework')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (12,1,'Body',3,'Location','AccessDenniedFramework')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (12,1,'Subject',0,'','AccessDenniedFramework')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (15,1,'Body',1,'EmployeeName','EmployeeEarlyExit')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (15,1,'Subject',0,'','EmployeeEarlyExit')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (15,2,'Body',1,'EmployeeName','EmployeeLater')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (15,2,'Subject',0,'','EmployeeLater')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (16,1,'Body',1,'EmployeeName','EmployeeFieldExpired')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (16,1,'Body',2,'FieldName','EmployeeFieldExpired')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (16,1,'Body',3,'Day','EmployeeFieldExpired')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (16,1,'Subject',0,'','EmployeeFieldExpired')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (17,1,'Body',1,'CompanyName','CompanyFieldExpired')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (17,1,'Body',2,'FieldName','CompanyFieldExpired')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (17,1,'Body',3,'Day','CompanyFieldExpired')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (17,1,'Subject',0,'','CompanyFieldExpired')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (18,1,'Body',1,'EmployeeName','ConceptNotReached')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (18,1,'Body',2,'Day','ConceptNotReached')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (18,1,'Body',3,'ConceptName','ConceptNotReached')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (18,1,'Subject',0,'','ConceptNotReached')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (19,1,'Body',1,'EmployeeName','Daywithunmatchedtimerecord')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (19,1,'Body',2,'Day','Daywithunmatchedtimerecord')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (19,1,'Subject',0,'','Daywithunmatchedtimerecord')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (20,1,'Body',1,'EmployeeName','Daywithunreliabletimerecord')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (20,1,'Body',2,'Day','Daywithunreliabletimerecord')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (20,1,'Subject',0,'','Daywithunreliabletimerecord')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (21,1,'Body',1,'EmployeeName','Nonjustifiedincident')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (21,1,'Body',2,'Day','Nonjustifiedincident')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (21,1,'Subject',0,'','Nonjustifiedincident')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (22,1,'Body',0,'','IDCardnotassignedtoemployee')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (22,1,'Subject',0,'','IDCardnotassignedtoemployee')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (23,1,'Body',1,'TaskName','Taskclosetofinish')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (23,1,'Body',2,'EndDate','Taskclosetofinish')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (23,1,'Subject',0,'','Taskclosetofinish')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (24,1,'Body',1,'TaskName','Taskclosetostart')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (24,1,'Body',2,'BeginDate','Taskclosetostart')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (24,1,'Subject',0,'','Taskclosetostart')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (25,1,'Body',1,'TaskName','Taskexceedingplannedtime')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (25,1,'Subject',0,'','Taskexceedingplannedtime')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (26,1,'Body',1,'TaskName','Taskexceedingfinisheddate')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (26,1,'Body',2,'EndDate','Taskexceedingfinisheddate')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (26,1,'Subject',0,'','Taskexceedingfinisheddate')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (29,1,'Body',1,'TaskName','TaskexceedingStarteddate')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (29,1,'Body',2,'BeginDate','TaskexceedingStarteddate')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (29,1,'Subject',0,'','TaskexceedingStarteddate')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (30,1,'Body',1,'TaskName','TaskwithAlert')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (30,1,'Body',2,'Day','TaskwithAlert')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (30,1,'Subject',0,'','TaskwithAlert')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (32,1,'Body',1,'EmployeeName','AbsencedocumentNotDelivered')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (32,1,'Body',2,'DocumentName','AbsencedocumentNotDelivered')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (32,1,'Body',3,'Day','AbsencedocumentNotDelivered')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (32,1,'Subject',0,'','AbsencedocumentNotDelivered')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (35,1,'Body',2,'Day','AdviceDaywithunmatchedtimerecord')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (35,1,'Subject',0,'','AdviceDaywithunmatchedtimerecord')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (36,1,'Body',1,'PassportName','AdviceTemporaryBloquedAccount')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (36,1,'Subject',0,'','AdviceTemporaryBloquedAccount')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (37,1,'Body',1,'PassportName','AdviceBloquedAccount')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (37,1,'Subject',0,'','AdviceBloquedAccount')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (38,1,'Body',1,'PassportName','AdviceValidationCode')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (38,1,'Body',2,'ValidationCode','AdviceValidationCode')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (38,1,'Subject',0,'','AdviceValidationCode')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (39,1,'Body',1,'PassportName','AdviceNewPassword')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (39,1,'Body',2,'Password','AdviceNewPassword')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (39,1,'Subject',0,'','AdviceNewPassword')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (39,2,'Body',1,'PassportName','AdviceNewPasswordAD')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (39,2,'Subject',0,'','AdviceNewPasswordAD')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (40,1,'Body',1,'EmployeeName','RequestPending')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (40,1,'Body',2,'RequestType','RequestPending')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (40,1,'Subject',1,'RequestType','RequestPending')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (43,1,'Body',1,'EmployeeName','EmployeeMovilitiesUpdated')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (43,1,'Body',2,'Day','EmployeeMovilitiesUpdated')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (43,1,'Subject',0,'','EmployeeMovilitiesUpdated')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (44,1,'Body',1,'EmployeeName','EmployeeMovilityExecuted')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (44,1,'Body',2,'DepartmentName','EmployeeMovilityExecuted')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (44,1,'Subject',0,'','EmployeeMovilityExecuted')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (45,1,'Body',1,'Day','Visits.New')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (45,1,'Body',2,'DetailLink','Visits.New')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (45,1,'Subject',0,'','Visits.New')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (45,2,'Body',1,'Day','Visits.Update')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (45,2,'Body',2,'DetailLink','Visits.Update')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (45,2,'Subject',0,'','Visits.Update')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (45,3,'Body',1,'Day','Visits.Delete')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (45,3,'Subject',0,'','Visits.Delete')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (46,1,'Body',1,'EmployeeName','ConceptValueExceededMax')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (46,1,'Body',2,'ConceptName','ConceptValueExceededMax')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (46,1,'Subject',0,'','ConceptValueExceededMax')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (47,1,'Body',1,'EmployeeName','ConceptValueNotReachedMin')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (47,1,'Body',2,'ConceptName','ConceptValueNotReachedMin')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (47,1,'Subject',0,'','ConceptValueNotReachedMin')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (48,1,'Body',1,'RequestType','EmployeeRequestApproved')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (48,1,'Body',2,'RequestDetail','EmployeeRequestApproved')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (48,1,'Body',3,'RequestComments','EmployeeRequestApproved')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (48,1,'Subject',1,'RequestType','EmployeeRequestApproved')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (48,2,'Body',1,'RequestType','EmployeeRequestRejected')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (48,2,'Body',2,'RequestDetail','EmployeeRequestRejected')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (48,2,'Body',3,'RequestComments','EmployeeRequestRejected')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (48,2,'Subject',1,'RequestType','EmployeeRequestRejected')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,1,'Body',1,'EmployeeName','NewDocumentDelivered')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,1,'Body',2,'DocumentName','NewDocumentDelivered')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,1,'Body',3,'Remarks','NewDocumentDelivered')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,1,'Subject',0,'','NewDocumentDelivered')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,2,'Subject',0,'','NewDocumentDelivered.DaysAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,2,'Body',1,'EmployeeName','NewDocumentDelivered.DaysAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,2,'Body',2,'DocumentName','NewDocumentDelivered.DaysAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,2,'Body',3,'Remarks','NewDocumentDelivered.DaysAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,2,'Body',4,'Cause','NewDocumentDelivered.DaysAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,2,'Body',5,'BeginDate','NewDocumentDelivered.DaysAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,2,'Body',6,'EndDate','NewDocumentDelivered.DaysAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,3,'Body',1,'EmployeeName','NewDocumentDelivered.HourAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,3,'Body',2,'DocumentName','NewDocumentDelivered.HourAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,3,'Body',3,'Remarks','NewDocumentDelivered.HourAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,3,'Body',4,'Cause','NewDocumentDelivered.HourAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,3,'Body',5,'Day','NewDocumentDelivered.HourAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,3,'Subject',0,'','NewDocumentDelivered.HourAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,4,'Body',1,'EmployeeName','NewDocumentDelivered.Overtime')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,4,'Body',2,'DocumentName','NewDocumentDelivered.Overtime')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,4,'Body',3,'Remarks','NewDocumentDelivered.Overtime')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,4,'Body',4,'Cause','NewDocumentDelivered.Overtime')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,4,'Body',5,'Day','NewDocumentDelivered.Overtime')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (49,4,'Subject',0,'','NewDocumentDelivered.Overtime')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (50,1,'Body',1,'DocumentName','DocumentNextToBeRequired')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (50,1,'Body',2,'Day','DocumentNextToBeRequired')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (50,1,'Subject',0,'','DocumentNextToBeRequired')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (50,2,'Body',1,'DocumentName','DocumentNextToBeRequired_Past')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (50,2,'Body',2,'Day','DocumentNextToBeRequired_Past')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (50,2,'Subject',0,'','DocumentNextToBeRequired_Past')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (50,3,'Body',1,'DocumentName','DocumentNextToBeRequired_Today')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (50,3,'Subject',0,'','DocumentNextToBeRequired_Today')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (51,1,'Body',1,'Shift','ShiftScheduled')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (51,1,'Body',2,'Day','ShiftScheduled')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (51,1,'Body',3,'EmployeeName','ShiftScheduled')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (51,1,'Subject',0,'','ShiftScheduled')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (53,1,'Body',1,'CompanyName','DocumentNextToBeRequired.Company.Supervisor')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (53,1,'Body',2,'DocumentName','DocumentNextToBeRequired.Company.Supervisor')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (53,1,'Subject',0,'','DocumentNextToBeRequired.Supervisor')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (53,2,'Body',1,'EmployeeName','DocumentNextToBeRequired.Employee.Supervisor')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (53,2,'Body',2,'DocumentName','DocumentNextToBeRequired.Employee.Supervisor')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (55,1,'Body',1,'PassportName','RecoverPassword')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (55,1,'Body',2,'RecoverCode','RecoverPassword')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (55,1,'Subject',0,'','RecoverPassword')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (56,1,'Body',1,'EmployeeName','PunchOnProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (56,1,'Body',2,'Day','PunchOnProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (56,1,'Subject',0,'','PunchOnProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (61,1,'Body',1,'ImportGuideName','DataImportExecuted')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (61,1,'Body',2,'ResultDetail','DataImportExecuted')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (61,1,'Subject',0,'','DataImportExecuted')
GO




IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 59)
	insert into dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem)
		Values (31,'Kpi Limit OverTaken', NULL,360,'','',1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 59)
	insert into dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem)
		Values (57,'InvalidPortalConsents', NULL,360,'','',1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 59)
	insert into dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem)
		Values (58,'InvalidDesktopConsents', NULL,360,'','',1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 59)
	insert into dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem)
		Values (59,'InvalidTerminalConsents', NULL,360,'','',1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 60)
	insert into dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem)
		Values (60,'InvalidVisitsConsents', NULL,360,'','',1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 62)
	insert into dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem)
		Values (62,'LabAgree Max Exceeded', NULL,360,'','',1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 63)
	insert into dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem)
		Values (63,'LabAgree Min Reached', NULL,360,'','',1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 64)
	insert into dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem)
		Values (64,'Tasks Request complete', NULL,360,'','',1)
GO

-- Campo para guardar la fecha de rechazo de la solicitud
ALTER TABLE [dbo].[Requests] ADD 
	[RejectedDate] [smalldatetime] NULL
GO

-- Validacion automatica para previsiones de vacaciones por dias
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 6 and IDRuleType=4)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(6,4)
GO

-- reglas de solicitud para previsiones de vacaciones por dias
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 6 and IDRuleType=5)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(6,5)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 6 and IDRuleType=6)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(6,6)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 6 and IDRuleType=7)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(6,7)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 6 and IDRuleType=8)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(6,8)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 6 and IDRuleType=9)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(6,9)
GO

-- Validacion automatica para previsiones de ausencia por horas
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 9 and IDRuleType=4)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(9,4)
GO

-- reglas de solicitud para previsiones de ausencias por horas
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 9 and IDRuleType=2)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(9,2)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 9 and IDRuleType=5)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(9,5)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 9 and IDRuleType=6)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(9,6)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 9 and IDRuleType=7)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(9,7)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 9 and IDRuleType=9)
INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(9,9)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTPortal.RequestsHasRanquings')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTPortal.RequestsHasRanquings','false')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].sysroFeatures WHERE Alias = 'Planification.Forecast')	
insert into dbo.sysroFeatures values (21200, 21, 'Planification.Forecast', 'Cancelación de ausencias previstas', '', 'E', 'RW', null, null, null)
GO


ALTER TABLE [dbo].[ImportGuides] ADD [Parameters] [nvarchar](max) 
GO

ALTER TABLE [dbo].[ImportGuides] ADD [Enabled] [bit]
GO

UPDATE [dbo].[ImportGuides] set [Enabled] = 1 where [Mode] = 2
GO

INSERT INTO [dbo].[ImportGuides] (ID, Name, Template, Mode, Type, FormatFilePath, SourceFilePath, Separator, CopySource, LastLog, RequieredFunctionalities, FeatureAliasID, Destination, Parameters, Enabled)
VALUES (15, 'Carga de foto de empleados', 0, 2, 1,'','','',1,NULL,'Employees.DataLink.Imports.Employees', 'Employees',0,'<?xml version="1.0"?><roCollection version="2.0"><Item key="SourceFile" type="8">*.png,*.jpg</Item><Item key="IDFilenameMask" type="8"></Item><Item key="KeyField" type="8">USR_NIF</Item></roCollection>',0)
GO

ALTER FUNCTION [dbo].[WebLogin_GetPermissionOverEmployee]    
 	(@idPassport int, @idEmployee int, @idApplication int, @mode int, @includeGroups bit, @date datetime)  RETURNS int    
       AS    
 	BEGIN    
 		DECLARE @Result int    
 		DECLARE @parentPassport int    
 		DECLARE @GroupType nvarchar(50),  
 		@pidPassport int = @idPassport,    
         @pidEmployee int = @idEmployee,    
         @pidApplication int = @idApplication,    
         @pmode int = @mode,  
         @pincludeGroups bit = @includeGroups ,  
         @pdate datetime   = @date,
 		@employeeResult int,
 		@calendarlock int
 	SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @pidPassport   
 	select @calendarlock = isnull(COUNT(*),0) from Groups where CloseDate is not null
 	 
     IF @GroupType = 'U'    
     BEGIN    
         SET @parentPassport = @pidPassport    
     END    
     ELSE    
     BEGIN    
         SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @pidPassport    
     END    
 	SELECT @Result = Permission, 
 		@employeeResult = isnull((select poe.Permission from sysroPermissionsOverEmployeesExceptions poe where poe.PassportID = @parentPassport and poe.EmployeeFeatureID = @pidApplication and poe.EmployeeID = @pidEmployee),9)
 		FROM sysroPermissionsOverGroups where EmployeeGroupID = dbo.GetEmployeeGroup(@pidEmployee,@pdate) 
 			and sysroPermissionsOverGroups.EmployeeFeatureID =  @pidApplication and sysroPermissionsOverGroups.PassportID = @parentPassport
 	IF @Result > @employeeResult
 	BEGIN
 	 SET @Result = @employeeResult
 	END
 	IF @calendarlock > 0
 	BEGIN
 		IF @Result > 3 AND @pidApplication = 2 AND dbo.IsBlockDateRestrictionActive(@pidPassport,dbo.GetEmployeeGroup(@pidEmployee,@date),@date) = 1 SET @Result = 3    
 	END
 	
 	RETURN @Result    
 END
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='452' WHERE ID='DBVersion'
GO

