CREATE TABLE [dbo].[TerminalsSyncBiometricData](
	[EmployeeId] [int] NULL,
	[FingerId] [int] NULL,
	[FingerData] varbinary(max) NULL,
	[TerminalId] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[TerminalsSyncCardsData](
	[EmployeeId] [int] NULL,
	[CardID] [numeric](28,0) NULL,
	[TerminalID] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TerminalsSyncDocumentsData](
	[EmployeeId] [int] NULL,
	[ReaderId] [int] NULL,
	[Name] [nvarchar](max) NULL,
	[BeginDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[DenyAccess] [bit] NULL,
	[Company] [nvarchar](max) NULL,
	[TerminalId] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[TerminalsSyncEmployeesData](
	[EmployeeId] [int] NULL,
	[Name] [nvarchar](max) NULL,
	[Language] [nvarchar](max) NULL,
	[PIN] [nvarchar](max) NULL,
	[MergeMethod] [int] NULL,
	[AllowedCauses] [nvarchar](max) NULL,
	[IsOnline] [bit] NULL,
	[ConsentRequired] [bit] NULL,
	[ImageCRC] [nvarchar](max) NULL,
	[TerminalId] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[TerminalsSyncGroupsData](
	[EmployeeId] [int] NULL,
	[GroupId] [int] NULL,
	[TerminalID] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TerminalsSyncConfigData](
	[Name] [nvarchar](max) NULL,
	[Value] [nvarchar](max) NULL,
	[TerminalId] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[TerminalsSyncSirensData](
	[DayOf] [nvarchar](max) NULL,
	[StartDate] [datetime] NULL,
	[Relay] [nvarchar](max) NULL,
	[Duration] [int] NULL,
	[TerminalId] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[TerminalsSyncAccessData](
	[GroupId] [int] NULL,
	[ReaderId] [int] NULL,
	[Permission] [nvarchar](1) NULL,
	[TerminalId] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TerminalsSyncTimeZonesData](
	[GroupId] [int] NULL,
	[ReaderId] [int] NULL,
	[DayOf] [nvarchar](max) NULL,
	[BeginTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[TerminalId] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[TerminalsSyncCausesData](
	[CauseId] [int] NULL,
	[Name] [nvarchar](max) NULL,
	[ReaderInputCode] [int] NULL,
	[WorkingType] [bit] NULL,
	[TerminalId] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[TerminalsSyncPushTimeZonesData](
	[IDTimeZone] [int] NULL,
	[IDDayOrHol] [int] NULL,
	[BeginTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[TerminalId] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TerminalsSyncPushEmployeeTimeZonesData](
	[IDEmployee] [int] NULL,
	[TZs] [nvarchar](max) NULL,
	[TerminalId] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[TerminalsSyncPushEmployeeTimeZonesData] ADD  CONSTRAINT [DF_TerminalsSyncPushEmployeeTimeZonesData_TimeStamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO
ALTER TABLE [dbo].[TerminalsSyncPushTimeZonesData] ADD  CONSTRAINT [DF_TerminalsSyncPushTimeZonesData_TimeStamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO
ALTER TABLE [dbo].[TerminalsSyncCausesData] ADD  CONSTRAINT [DF_TerminalsSyncCausesData_TimeStamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO
ALTER TABLE [dbo].[TerminalsSyncTimeZonesData] ADD  CONSTRAINT [DF_TerminalsSyncTimeZonesData_TimeStamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO
ALTER TABLE [dbo].[TerminalsSyncAccessData] ADD  CONSTRAINT [DF_TerminalsSyncAccessData_TimeStamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO
ALTER TABLE [dbo].[TerminalsSyncSirensData] ADD  CONSTRAINT [DF_TerminalsSyncSirensData_TimeStamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO
ALTER TABLE [dbo].[TerminalsSyncConfigData] ADD  CONSTRAINT [DF_TerminalsSyncConfigData_TimeStamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO
ALTER TABLE [dbo].[TerminalsSyncBiometricData] ADD  CONSTRAINT [DF_TerminalsSyncBiometricData_TimeStamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO
ALTER TABLE [dbo].[TerminalsSyncCardsData] ADD  CONSTRAINT [DF_TerminalsSyncCardsData_TimeStamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO
ALTER TABLE [dbo].[TerminalsSyncDocumentsData] ADD  CONSTRAINT [DF_TerminalsSyncDocumentsData_TimeStamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO
ALTER TABLE [dbo].[TerminalsSyncEmployeesData] ADD  CONSTRAINT [DF_TerminalsSyncEmployeesData_TimeStamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO
ALTER TABLE [dbo].[TerminalsSyncGroupsData] ADD  CONSTRAINT [DF_TerminalsSyncGroupsData_TimeStamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO

ALTER TABLE [dbo].[Terminals]
ADD FirmwareUpgradeAvailable bit DEFAULT 0
GO

DECLARE @MaxId int; SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'TACO', N'1', N'Interactive', N'X', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int; SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx8', 1, N'TACO', N'1', N'Interactive', N'X', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

update sysroGUI set Parameters = 'MonoTenant' where IDPath = 'Portal\Configuration\Routes'
GO
update sysroGUI set Parameters = 'MonoTenant' where IDPath = 'Portal\GeneralManagement\ReportScheduler'
GO
INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) 
	VALUES ('Portal\GeneralManagement\AnalyticScheduler','AnalyticScheduler','ReportScheduler/ReportScheduler.aspx','ReportScheduler.png',NULL,'MultiTenant','',NULL,105,'NWR','U:Administration.ReportScheduler.Definition=Read') 
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) 
	VALUES ('MaxMinimize','Portal\GeneralManagement\AnalyticScheduler\Management','tbMaximize','Process\ReportServer',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions] ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES ('NewAna','Portal\GeneralManagement\AnalyticScheduler\Management','tbAddNewReportAnalitycsScheduler','Process\ReportServer','U:Administration.ReportScheduler.Definition=Admin','newReportAnalitycsScheduler()','btnTbAddAnalitycsReport',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) 
	VALUES ('Del','Portal\GeneralManagement\AnalyticScheduler\Management','tbDelReportScheduler','Process\ReportServer','U:Administration.ReportScheduler.Definition=Admin','ShowRemoveReportScheduler(''#ID#'')','btnTbDel2',0,3)
GO

IF not exists (select * from [dbo].[sysroLiveAdvancedParameters] where ParameterName = 'VTLive.DefaultReportsVersions')
	insert into [dbo].[sysroLiveAdvancedParameters] values ('VTLive.DefaultReportsVersions', '1')
GO

IF not exists (select * from [dbo].[sysroLiveAdvancedParameters] where ParameterName = 'CTaimaApiVersion')
	insert into [dbo].[sysroLiveAdvancedParameters] values ('CTaimaApiVersion', '1.0')
GO

IF not exists (select * from [dbo].[sysroLiveAdvancedParameters] where ParameterName = 'CTaimaSyncPunchesEnabled')
	insert into [dbo].[sysroLiveAdvancedParameters] values ('CTaimaSyncPunchesEnabled', 'True')
GO

IF not exists (select * from [dbo].[sysroLiveAdvancedParameters] where ParameterName = 'CTaimaCenterId')
	insert into [dbo].[sysroLiveAdvancedParameters] values ('CTaimaCenterId', '')
GO

IF not exists (select * from [dbo].[sysroLiveAdvancedParameters] where ParameterName = 'CTaimaTenantId')
	insert into [dbo].[sysroLiveAdvancedParameters] values ('CTaimaTenantId', '')
GO

IF not exists (select * from [dbo].[sysroLiveAdvancedParameters] where ParameterName = 'CTaimaOcmApimSubscriptionKey')
	insert into [dbo].[sysroLiveAdvancedParameters] values ('CTaimaOcmApimSubscriptionKey', '')
GO

UPDATE sysroParameters SET Data='491' WHERE ID='DBVersion'
GO

