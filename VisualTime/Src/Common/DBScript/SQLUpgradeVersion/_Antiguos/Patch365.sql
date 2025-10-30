IF NOT EXISTS (SELECT * FROM sys.columns   WHERE  object_id = OBJECT_ID(N'[dbo].[Causes]')  AND name = 'DefaultValuesAbsences')
	BEGIN
		ALTER TABLE [dbo].[Causes] ADD DefaultValuesAbsences [nvarchar](MAX) default ''
		UPDATE [dbo].[Causes] SET DefaultValuesAbsences = '' WHERE DefaultValuesAbsences is null
	END
GO

IF NOT EXISTS (SELECT * FROM sys.columns   WHERE  object_id = OBJECT_ID(N'[dbo].[EmployeeJobMoves]')  AND name = 'IsExported')
	ALTER TABLE [dbo].[EmployeeJobMoves] ADD [IsExported] [bit] NULL default (0)
GO

IF NOT EXISTS (SELECT * FROM sys.columns   WHERE  object_id = OBJECT_ID(N'[dbo].[TeamJobMoves]')  AND name = 'IsExported')
	ALTER TABLE [dbo].[TeamJobMoves] ADD [IsExported] [bit] NULL default (0)
GO

IF NOT EXISTS (SELECT * FROM sys.columns   WHERE  object_id = OBJECT_ID(N'[dbo].[sysroNotificationTypes]')  AND name = 'Feature')
	BEGIN
		ALTER TABLE dbo.sysroNotificationTypes ADD Feature [nvarchar](MAX) default ''
		UPDATE dbo.sysroNotificationTypes Set Feature = '' WHERE Feature IS NULL
		UPDATE dbo.sysroNotificationTypes Set Feature = '' WHERE Feature IS NULL
		UPDATE dbo.sysroNotificationTypes Set Feature = 'Employees.Contract' WHERE ID in(1,2)
		UPDATE dbo.sysroNotificationTypes Set Feature = 'Calendar.Punches' WHERE ID in(3,4,5,6,7)
		UPDATE dbo.sysroNotificationTypes Set Feature = 'Calendar.Punches.Punches' WHERE ID in(8)
		UPDATE dbo.sysroNotificationTypes Set Feature = 'Access' WHERE ID in(9)
		UPDATE dbo.sysroNotificationTypes Set Feature = 'Terminals.Definition' WHERE ID in(11)
		UPDATE dbo.sysroNotificationTypes Set Feature = 'Calendar.Scheduler' WHERE ID in(13,14)
		UPDATE dbo.sysroNotificationTypes Set Feature = 'Calendar' WHERE ID in(15)
		UPDATE dbo.sysroNotificationTypes Set Feature = 'Employees.UserFields' WHERE ID in(16,17)
		UPDATE dbo.sysroNotificationTypes Set Feature = 'Calendar.Punches.Punches' WHERE ID in(19,20,35)
		UPDATE dbo.sysroNotificationTypes Set Feature = 'Calendar.JustifyIncidences' WHERE ID in(21)
		UPDATE dbo.sysroNotificationTypes Set Feature = 'Employees.IdentifyMethods' WHERE ID in(22)
		UPDATE dbo.sysroNotificationTypes Set Feature = 'Tasks.Definition' WHERE ID in(23,24,25,26,29,30)
		UPDATE dbo.sysroNotificationTypes Set Feature = 'Calendar.Requests.Vacations' WHERE ID in(27)
		UPDATE dbo.sysroNotificationTypes Set Feature = 'Calendar.Requests.ShiftChange' WHERE ID in(28)
		UPDATE dbo.sysroNotificationTypes Set Feature = 'Employees' WHERE ID in(32,33,34,18)
	END
GO

IF NOT EXISTS (SELECT * FROM sys.columns   WHERE  object_id = OBJECT_ID(N'[dbo].[sysroNotificationTypes]')  AND name = 'FeatureType')
	BEGIN
		ALTER TABLE dbo.sysroNotificationTypes ADD FeatureType [nvarchar](MAX) default ''
		UPDATE dbo.sysroNotificationTypes Set FeatureType = '' WHERE FeatureType IS NULL
		UPDATE dbo.sysroNotificationTypes Set FeatureType = 'E' WHERE ID in(1,2,3,4,5,6,7,8,13,14,15,16,17,19,20,21,22,27,28,34,18,32,33,35,9)
		UPDATE dbo.sysroNotificationTypes SET FeatureType = 'U' WHERE FeatureType = 'E'
		update dbo.sysroNotificationTypes set FeatureType = 'U' where feature = 'Calendar.Punches.Punches'
	END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_TerminalReaders_Activities')
	ALTER TABLE dbo.TerminalReaders ADD CONSTRAINT
	FK_TerminalReaders_Activities FOREIGN KEY
	(
	IDActivity
	) REFERENCES dbo.Activities
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO

delete dbo.sysroNotificationTasks where ID in ( 
select noti.id from (select * from sysroNotificationTasks where IDNotification = 1001) noti 
full outer join 
(select * from sysrovwIncompletedDays) as incom 
on incom.IDEmployee = noti.Key1Numeric and incom.date = noti.Key3DateTime 
where incom.IDEmployee Is Null)

delete sysroNotificationTasks where IDNotification = 1001 and  Key3DateTime < DATEADD(d,-30,getdate())
GO

delete dbo. sysroNotificationTasks where ID in (
select noti.id from
(select * from sysroNotificationTasks where IDNotification = 1002) noti
full outer join
(select * from punches where IsNotReliable = 0) as punch
on punch.IDEmployee = noti.Key1Numeric and punch.shiftdate = noti.Key3DateTime
Where punch.IDEmployee Is Null
)
GO

delete dbo.sysroNotificationTasks where IDNotification = 1002 and  Key3DateTime < DATEADD(d,-30,getdate())
GO

delete dbo.sysroNotificationTasks where ID in ( 
select noti.id from ( 
select * from sysroNotificationTasks where IDNotification = 1003) noti 
full outer join (
select * from DailyCauses where IDCause = 0) as caus 
on caus.IDEmployee = noti.Key1Numeric and caus.Date = noti.Key3DateTime 
Where caus.IDEmployee Is Null)
GO

delete dbo.sysroNotificationTasks where IDNotification = 1003 and  Key3DateTime < DATEADD(d,-30,getdate())
GO

delete dbo.sysroNotificationTasks where IDNotification = 1010 and  Key3DateTime < DATEADD(d,-1,getdate())
GO

IF NOT EXISTS (SELECT * FROM [dbo].[sysroNotificationTypes] WHERE [ID] = 40)
   BEGIN
       INSERT INTO [dbo].sysroNotificationTypes ([ID],[Name],[Description],[Scheduler],[Feature],[FeatureType],[OnlySystem]) VALUES(40, 'Requests Pending',NULL, 120,'*Requests*','U',1)
   END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[sysroNotificationTypes] WHERE [ID] = 41)
   BEGIN
       INSERT INTO [dbo].sysroNotificationTypes ([ID],[Name],[Description],[Scheduler],[Feature],[FeatureType],[OnlySystem]) VALUES(41, 'Employee Present With Expired Docs',NULL,120,'Employees.UserFields.Information','U',1)
   END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[sysroNotificationTypes] WHERE [ID] = 43)
   BEGIN
       INSERT INTO [dbo].[sysroNotificationTypes]([ID],[Name],[Description],[Scheduler],[Feature],[FeatureType],[OnlySystem]) VALUES (43,'Mobility Update',NULL,10,'Employees.GroupMobility','U',0)
   END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[sysroNotificationTypes] WHERE [ID] = 44)
   BEGIN
       INSERT INTO [dbo].[sysroNotificationTypes]([ID],[Name],[Description],[Scheduler],[Feature],[FeatureType],[OnlySystem]) VALUES (44,'Mobility Execution',NULL,360,'Employees.GroupMobility','U',0)
   END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[sysroNotificationTypes] WHERE [ID] = 45)
   BEGIN
       INSERT INTO [dbo].[sysroNotificationTypes] ([ID],[Name],[Description],[Scheduler],[Feature],[FeatureType],[OnlySystem]) VALUES (45,'Visit Update',null,360,'Employees','U',1)
   END
GO

IF NOT EXISTS (SELECT * FROM sys.tables   WHERE name = 'AbsenceTracking')
	CREATE TABLE [dbo].[AbsenceTracking](
		[ID] [numeric](16, 0) IDENTITY(1,1) NOT NULL,
		[TypeAbsence] [tinyint] NOT NULL,
		[IDEmployee] [int] NOT NULL,
		[IDCause] [smallint] NOT NULL,
		[Date] [smalldatetime] NOT NULL,
		[IDAbsence] [int] NULL DEFAULT(0),
		[TrackDay] [smalldatetime] NOT NULL,
		[IDDocument] [int] NOT NULL,
		[DeliveryDate] [smalldatetime] NULL,
		[IDPassport] [int] NULL,
		[NotificationDate] [smalldatetime] NULL,
		[NotificationHistory] [nvarchar](max) NULL,
		[AttachmentFile] [image] NULL,
		[Comments] [nvarchar](max) NULL,

	 CONSTRAINT [PK_AbsenceTracking] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	) ON [PRIMARY]
	) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.tables   WHERE name = 'CausesDocuments')
	CREATE TABLE [dbo].[CausesDocuments](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[IDCause] [int] NOT NULL,
		[IDLabAgree] [int] NOT NULL,
		[IDDocument] [int] NOT NULL,
		[Parameters] [nvarchar](max) NULL,
	 CONSTRAINT [PK_CausesDocuments] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	) ON [PRIMARY]
	) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.tables   WHERE name = 'DocumentsAbsences')
	CREATE TABLE [dbo].[DocumentsAbsences](
					[ID] [int] NOT NULL,
					[Name] [nvarchar](50) NOT NULL,
					[ShortName] [nvarchar](3) NULL,
					[Description] [nvarchar](max) NULL,
					[RememberText] [nvarchar](max) NULL,
	 CONSTRAINT [PK_DocumentsAbsences] PRIMARY KEY CLUSTERED 
	(
					[ID] ASC
	) ON [PRIMARY]
	) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DocumentsAbsencesAdvices')
	CREATE TABLE [dbo].[DocumentsAbsencesAdvices](
					[ID] [int] IDENTITY(1,1) NOT NULL,
					[IDDocumentAbsence] [int] NULL,
					[Name] [nvarchar](50) NULL,
					[Advice] [nvarchar](max) NULL,
	 CONSTRAINT [PK_DocumentsAbsencesAdvices] PRIMARY KEY CLUSTERED 
	(
					[ID] ASC
	) ON [PRIMARY]
	) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DiningRoomTurns')
	BEGIN
		CREATE TABLE [DiningRoomTurns](
			[ID] [smallint] NOT NULL,
			[IDDiningRoom] [smallint] NULL,
			[Name] [nvarchar](128) NULL,
			[EmployeeSelection] [text] NULL,
			[BeginTime] [smalldatetime] NULL,
			[EndTime] [smalldatetime] NULL,
			[DaysOfWeek] [nvarchar](7) NULL)
		ALTER TABLE [DiningRoomTurns] WITH NOCHECK ADD CONSTRAINT [PK_DiningRoomTurns] PRIMARY KEY NONCLUSTERED (ID) 
	END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmployeeBiometricFaceDataZK')
	CREATE TABLE [dbo].[EmployeeBiometricFaceDataZK](
		[IDEmployee] [int] NOT NULL,
		[Data] [image] NULL,
		[TimeStamp] [smalldatetime] NULL,
		[IDTerminal] [int] NULL,
	 CONSTRAINT [PK_EmployeeBiometricFaceDataZK] PRIMARY KEY CLUSTERED 
	(
		[IDEmployee] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.columns   WHERE  object_id = OBJECT_ID(N'[dbo].[AbsenceTracking]')  AND name = 'AttachmentFileName')
	ALTER TABLE dbo.AbsenceTracking ADD	AttachmentFileName nvarchar(50) NULL
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'sysroReportTasks')
	CREATE TABLE [dbo].[sysroReportTasks](
		[ID] [numeric](16, 0) IDENTITY(1,1) NOT NULL,
		[IDPassport] [int] NOT NULL,
		[ReportName] [nvarchar](max) NOT NULL,
		[FileName] [nvarchar](max) NULL,
		[Status] [int] NOT NULL DEFAULT (0),
		[Parameters] [nvarchar](max) NOT NULL,
		[Culture] [nvarchar](max) NULL,
		[UploadFile] [nvarchar](max) NULL,
		[ExportFormatType] [int] NULL DEFAULT (5),
		[TimeStamp] [datetime] NULL DEFAULT (getdate()),
	 CONSTRAINT [PK_sysroReportTasks] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	) ON [PRIMARY]
	) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'sysroSchedulerViews')
	BEGIN
		CREATE TABLE [dbo].[sysroSchedulerViews] (
			[ID] [int] NOT NULL,
			[IdView] [int] NOT NULL,
			[IdPassport] [int] NOT NULL,
			[NameView] [nvarchar](100) NOT NULL,
			[Description] [nvarchar](500) NULL DEFAULT (''),
			[DateView] [datetime] NOT NULL,
			[Employees] [nvarchar](4000) NULL DEFAULT (''),
			[DateInf] [datetime] NOT NULL,
			[DateSup] [datetime] NOT NULL,
			[CubeLayout] [nvarchar](MAX) NULL DEFAULT ('') ,
		 CONSTRAINT [PK_sysroSchedulerViews] PRIMARY KEY NONCLUSTERED 								
		(								
			[ID] ASC							
		) ON [PRIMARY]								
		) ON [PRIMARY]								

		ALTER TABLE [dbo].[sysroSchedulerViews] ADD [TypeView] [nvarchar](1) default ''
		ALTER TABLE [dbo].[sysroSchedulerViews] ADD [FilterData] [nvarchar](100) default ''
		ALTER TABLE [dbo].[sysroSchedulerViews] ADD [Concepts] nvarchar(4000) NULL DEFAULT('')

		
		INSERT INTO [dbo].[sysroSchedulerViews] ([ID],[IdView],[IdPassport],[NameView],[Description],[DateView],[Employees],[DateInf],[DateSup],[CubeLayout],[TypeView],[FilterData],[Concepts]) VALUES 
  				( ISNULL((select MAX(ID) + 1 from dbo.sysroSchedulerViews),1),1,-1,'Diario','',convert(smalldatetime,'2013-05-27',120),'',convert(smalldatetime,'2013-05-01',120),convert(smalldatetime,'2013-06-24',120),
				'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8gGgd/IdAKF38iEA8gGgd/IdAKF38iIA8gKgd/IdAKB38h6goHfyI/ICoHfyJHfyJaB38iZ38iehd/J78gDyAPJ8DiNMYXlvdXRWZXJzaW9uABNPcHRpb25zTG9hZGluZ1BhbmVsBFRleHQQQ2FyZ2FuZG8maGVsbGlwOwxPcHRpb25zUGFnZXILUm93c1BlclBhZ2ULT3B0aW9uc1ZpZXcXU2hvd0hvcml6b250YWxTY3JvbGxCYXIGRmllbGRzBUl0ZW0xBUluZGV4EVNvcnRCeVN1bW1hcnlJbmZvEkZpZWxkQ29tcG9uZW50TmFtZQpDb25kaXRpb25zDEZpbHRlclZhbHVlcwZWYWx1ZXMOflh0cmEjQXJyYXkwLCAKU2hvd0JsYW5rcwpGaWx0ZXJUeXBlCEV4Y2x1ZGVkDEN1c3RvbVRvdGFscwlGaWVsZE5hbWUJR3JvdXBOYW1lBE5hbWUPZmllbGRHcm91cE5hbWUxBEFyZWEHUm93QXJlYQ9WYWx1ZVRvdGFsU3R5bGUJVmlld1N0YXRlE1RvcEFsaWduZWRSb3dWYWx1ZXMCSUQLSGVhZGVyU3R5bGUJQ2VsbFN0eWxlClZhbHVlU3R5bGUJQXJlYUluZGV4DkFjdHVhbFNvcnRNb2RlB0RlZmF1bHQQU3VtbWFyeVZhcmlhdGlvbgROb25lBUl0ZW0yDEVtcGxveWVlTmFtZRJmaWVsZEVtcGxveWVlTmFtZTEFSXRlbTMJRGF0ZV9ZZWFyDmZpZWxkRGF0ZVllYXIxCkNvbHVtbkFyZWEFSXRlbTQKRGF0ZV9Nb250aA9maWVsZERhdGVNb250aDEFSXRlbTUIRGF0ZV9EYXkNZmllbGREYXRlRGF5MQVJdGVtNgREYXRlCmZpZWxkRGF0ZTEFSXRlbTcKQ2VudGVyTmFtZRBmaWVsZENlbnRlck5hbWUxBUl0ZW04CFRhc2tOYW1lDmZpZWxkVGFza05hbWUxBUl0ZW05C0luaXRpYWxUaW1lEWZpZWxkSW5pdGlhbFRpbWUxCERhdGFBcmVhB1Zpc2libGUGSXRlbTEwC0ZpZWxkMV9UYXNrEGZpZWxkRmllbGQxVGFzazEGSXRlbTExC0ZpZWxkMl9UYXNrEGZpZWxkRmllbGQyVGFzazEGSXRlbTEyC0ZpZWxkM19UYXNrEGZpZWxkRmllbGQzVGFzazEGSXRlbTEzCkNlbGxGb3JtYXQMRm9ybWF0U3RyaW5nCCMsIyMwLjAwCkZvcm1hdFR5cGUHTnVtZXJpYwtGaWVsZDRfVGFzaxBmaWVsZEZpZWxkNFRhc2sxBkl0ZW0xNAtGaWVsZDVfVGFzaxBmaWVsZEZpZWxkNVRhc2sxBkl0ZW0xNQtGaWVsZDZfVGFzaxBmaWVsZEZpZWxkNlRhc2sxBkl0ZW0xNgxGaWVsZDFfVG90YWwRZmllbGRGaWVsZDFUb3RhbDEGSXRlbTE3DEZpZWxkMl9Ub3RhbBFmaWVsZEZpZWxkMlRvdGFsMQZJdGVtMTgMRmllbGQzX1RvdGFsEWZpZWxkRmllbGQzVG90YWwxBkl0ZW0xOQxGaWVsZDRfVG90YWwRZmllbGRGaWVsZDRUb3RhbDEGSXRlbTIwDEZpZWxkNV9Ub3RhbBFmaWVsZEZpZWxkNVRvdGFsMQZJdGVtMjEMRmllbGQ2X1RvdGFsEWZpZWxkRmllbGQ2VG90YWwxBkl0ZW0yMgVWYWx1ZQtmaWVsZFZhbHVlMQdPcHRpb25zCUFsbG93RHJhZwVGYWxzZQZJdGVtMjMHUHJvamVjdA1maWVsZFByb2plY3QxBkl0ZW0yNANUYWcJZmllbGRUYWcxBkl0ZW0yNQZFc3RhZG8MZmllbGRFc3RhZG8xBkdyb3Vwcw==',
				'T',NULL,NULL)
		INSERT INTO [dbo].[sysroSchedulerViews] ([ID],[IdView],[IdPassport],[NameView],[Description],[DateView],[Employees],[DateInf],[DateSup],[CubeLayout],[TypeView],[FilterData],[Concepts]) VALUES 
				( ISNULL((select MAX(ID) + 1 from dbo.sysroSchedulerViews),1),1,-1,'Totales - desvios','',convert(smalldatetime,'2013-05-27',120),'',convert(smalldatetime,'2013-05-01',120),convert(smalldatetime,'2013-06-24',120),
				'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eWxlClZhbHVlU3R5bGUJQXJlYUluZGV4DkFjdHVhbFNvcnRNb2RlB0RlZmF1bHQQU3VtbWFyeVZhcmlhdGlvbgROb25lBUl0ZW0yDEVtcGxveWVlTmFtZRJmaWVsZEVtcGxveWVlTmFtZTEFSXRlbTMJRGF0ZV9ZZWFyDmZpZWxkRGF0ZVllYXIxBUl0ZW00CkRhdGVfTW9udGgPZmllbGREYXRlTW9udGgxBUl0ZW01CERhdGVfRGF5DWZpZWxkRGF0ZURheTEFSXRlbTYERGF0ZQpmaWVsZERhdGUxBUl0ZW03CkNlbnRlck5hbWUQZmllbGRDZW50ZXJOYW1lMQRBcmVhB1Jvd0FyZWEFSXRlbTgIVGFza05hbWUOZmllbGRUYXNrTmFtZTEFSXRlbTkLSW5pdGlhbFRpbWURZmllbGRJbml0aWFsVGltZTEGSXRlbTEwC0ZpZWxkMV9UYXNrEGZpZWxkRmllbGQxVGFzazEHVmlzaWJsZQZJdGVtMTELRmllbGQyX1Rhc2sQZmllbGRGaWVsZDJUYXNrMQZJdGVtMTILRmllbGQzX1Rhc2sQZmllbGRGaWVsZDNUYXNrMQZJdGVtMTMKQ2VsbEZvcm1hdAxGb3JtYXRTdHJpbmcIIywjIzAuMDAKRm9ybWF0VHlwZQdOdW1lcmljC0ZpZWxkNF9UYXNrEGZpZWxkRmllbGQ0VGFzazEGSXRlbTE0C0ZpZWxkNV9UYXNrEGZpZWxkRmllbGQ1VGFzazEGSXRlbTE1C0ZpZWxkNl9UYXNrEGZpZWxkRmllbGQ2VGFzazEGSXRlbTE2DEZpZWxkMV9Ub3RhbBFmaWVsZEZpZWxkMVRvdGFsMQZJdGVtMTcMRmllbGQyX1RvdGFsEWZpZWxkRmllbGQyVG90YWwxBkl0ZW0xOAxGaWVsZDNfVG90YWwRZmllbGRGaWVsZDNUb3RhbDEGSXRlbTE5DEZpZWxkNF9Ub3RhbBFmaWVsZEZpZWxkNFRvdGFsMQZJdGVtMjAMRmllbGQ1X1RvdGFsEWZpZWxkRmllbGQ1VG90YWwxBkl0ZW0yMQxGaWVsZDZfVG90YWwRZmllbGRGaWVsZDZUb3RhbDEGSXRlbTIyBVZhbHVlC2ZpZWxkVmFsdWUxCERhdGFBcmVhB09wdGlvbnMJQWxsb3dEcmFnBUZhbHNlBkl0ZW0yMwdQcm9qZWN0DWZpZWxkUHJvamVjdDEGSXRlbTI0A1RhZwlmaWVsZFRhZzEGSXRlbTI1F1RpbWVDaGFuZ2VkUmVxdWlyZW1lbnRzHWZpZWxkVGltZUNoYW5nZWRSZXF1aXJlbWVudHMxBkl0ZW0yNhFGb3JlY2FzdEVycm9yVGltZRdmaWVsZEZvcmVjYXN0RXJyb3JUaW1lMQZJdGVtMjcaTm9uUHJvZHVjdGl2ZVRpbWVJbmNpZGVuY2UgZmllbGROb25Qcm9kdWN0aXZlVGltZUluY2lkZW5jZTEGSXRlbTI4DEVtcGxveWVlVGltZRJmaWVsZEVtcGxveWVlVGltZTEGSXRlbTI5CFRlYW1UaW1lDmZpZWxkVGVhbVRpbWUxBkl0ZW0zMAxNYXRlcmlhbFRpbWUSZmllbGRNYXRlcmlhbFRpbWUxBkl0ZW0zMQlPdGhlclRpbWUPZmllbGRPdGhlclRpbWUxBkl0ZW0zMghEdXJhdGlvbg5maWVsZER1cmF0aW9uMQZJdGVtMzMGRXN0YWRvDGZpZWxkRXN0YWRvMQZHcm91cHM=',
				'T',NULL,NULL)
		INSERT INTO [dbo].[sysroSchedulerViews] ([ID],[IdView],[IdPassport],[NameView],[Description],[DateView],[Employees],[DateInf],[DateSup],[CubeLayout],[TypeView],[FilterData],[Concepts]) VALUES 
  				( ISNULL((select MAX(ID) + 1 from dbo.sysroSchedulerViews),1),1,-1,'Totales - teórico','',convert(smalldatetime,'2013-05-27',120),'',convert(smalldatetime,'2013-05-01',120),convert(smalldatetime,'2013-06-24',120),
				'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cmVhSW5kZXgOQWN0dWFsU29ydE1vZGUHRGVmYXVsdBBTdW1tYXJ5VmFyaWF0aW9uBE5vbmUFSXRlbTIMRW1wbG95ZWVOYW1lEmZpZWxkRW1wbG95ZWVOYW1lMQVJdGVtMwlEYXRlX1llYXIOZmllbGREYXRlWWVhcjEFSXRlbTQKRGF0ZV9Nb250aA9maWVsZERhdGVNb250aDEFSXRlbTUIRGF0ZV9EYXkNZmllbGREYXRlRGF5MQVJdGVtNgREYXRlCmZpZWxkRGF0ZTEFSXRlbTcKQ2VudGVyTmFtZRBmaWVsZENlbnRlck5hbWUxBEFyZWEHUm93QXJlYQVJdGVtOAhUYXNrTmFtZQ5maWVsZFRhc2tOYW1lMQVJdGVtOQtJbml0aWFsVGltZRFmaWVsZEluaXRpYWxUaW1lMQZJdGVtMTALRmllbGQxX1Rhc2sQZmllbGRGaWVsZDFUYXNrMQdWaXNpYmxlBkl0ZW0xMQtGaWVsZDJfVGFzaxBmaWVsZEZpZWxkMlRhc2sxBkl0ZW0xMgtGaWVsZDNfVGFzaxBmaWVsZEZpZWxkM1Rhc2sxBkl0ZW0xMwpDZWxsRm9ybWF0DEZvcm1hdFN0cmluZwgjLCMjMC4wMApGb3JtYXRUeXBlB051bWVyaWMLRmllbGQ0X1Rhc2sQZmllbGRGaWVsZDRUYXNrMQZJdGVtMTQLRmllbGQ1X1Rhc2sQZmllbGRGaWVsZDVUYXNrMQZJdGVtMTULRmllbGQ2X1Rhc2sQZmllbGRGaWVsZDZUYXNrMQZJdGVtMTYMRmllbGQxX1RvdGFsEWZpZWxkRmllbGQxVG90YWwxBkl0ZW0xNwxGaWVsZDJfVG90YWwRZmllbGRGaWVsZDJUb3RhbDEGSXRlbTE4DEZpZWxkM19Ub3RhbBFmaWVsZEZpZWxkM1RvdGFsMQZJdGVtMTkMRmllbGQ0X1RvdGFsEWZpZWxkRmllbGQ0VG90YWwxBkl0ZW0yMAxGaWVsZDVfVG90YWwRZmllbGRGaWVsZDVUb3RhbDEGSXRlbTIxDEZpZWxkNl9Ub3RhbBFmaWVsZEZpZWxkNlRvdGFsMQZJdGVtMjIFVmFsdWULZmllbGRWYWx1ZTEIRGF0YUFyZWEHT3B0aW9ucwlBbGxvd0RyYWcFRmFsc2UGSXRlbTIzB1Byb2plY3QNZmllbGRQcm9qZWN0MQZJdGVtMjQDVGFnCWZpZWxkVGFnMQZJdGVtMjUXVGltZUNoYW5nZWRSZXF1aXJlbWVudHMdZmllbGRUaW1lQ2hhbmdlZFJlcXVpcmVtZW50czEGSXRlbTI2EUZvcmVjYXN0RXJyb3JUaW1lF2ZpZWxkRm9yZWNhc3RFcnJvclRpbWUxBkl0ZW0yNxpOb25Qcm9kdWN0aXZlVGltZUluY2lkZW5jZSBmaWVsZE5vblByb2R1Y3RpdmVUaW1lSW5jaWRlbmNlMQZJdGVtMjgMRW1wbG95ZWVUaW1lEmZpZWxkRW1wbG95ZWVUaW1lMQZJdGVtMjkIVGVhbVRpbWUOZmllbGRUZWFtVGltZTEGSXRlbTMwDE1hdGVyaWFsVGltZRJmaWVsZE1hdGVyaWFsVGltZTEGSXRlbTMxCU90aGVyVGltZQ9maWVsZE90aGVyVGltZTEGSXRlbTMyCER1cmF0aW9uDmZpZWxkRHVyYXRpb24xBkl0ZW0zMwZFc3RhZG8MZmllbGRFc3RhZG8xBkdyb3Vwcw==',
				'T',NULL,NULL)
	END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TmpAnnualConceptsEmployee')
	CREATE TABLE [TmpAnnualConceptsEmployee](
		[IDPassport] [int] NOT NULL,
		[IDEmployee] [int] NOT NULL,
		[Ejercicio] [int] NOT NULL,
		[Periodo] [int] NOT NULL,
		[IdGroup] [int] NULL,
		[GroupName] [nvarchar](200) NULL,
		[EmployeeName] [nvarchar](50) NULL,
		[IDConcept1] [smallint] NULL,
		[ConceptColor1] [nvarchar](10) NULL,
		[ConceptName1] [nvarchar](3) NULL,
		[ConceptValue1] [numeric](19, 6) NULL,
		[ConceptValueTotal1] [numeric](19, 6) NULL,
		[IDConcept2] [smallint] NULL,
		[ConceptColor2] [nvarchar](10) NULL,
		[ConceptName2] [nvarchar](3) NULL,
		[ConceptValue2] [numeric](19, 6) NULL,
		[ConceptValueTotal2] [numeric](19, 6) NULL,
		[IDConcept3] [smallint] NULL,
		[ConceptColor3] [nvarchar](10) NULL,
		[ConceptName3] [nvarchar](3) NULL,
		[ConceptValue3] [numeric](19, 6) NULL,
		[ConceptValueTotal3] [numeric](19, 6) NULL,
		[IDConcept4] [smallint] NULL,
		[ConceptColor4] [nvarchar](10) NULL,
		[ConceptName4] [nvarchar](3) NULL,
		[ConceptValue4] [numeric](19, 6) NULL,
		[ConceptValueTotal4] [numeric](19, 6) NULL,
		[IDConcept5] [smallint] NULL,
		[ConceptColor5] [nvarchar](10) NULL,
		[ConceptName5] [nvarchar](3) NULL,
		[ConceptValue5] [numeric](19, 6) NULL,
		[ConceptValueTotal5] [numeric](19, 6) NULL,
		[IDConcept6] [smallint] NULL,
		[ConceptColor6] [nvarchar](10) NULL,
		[ConceptName6] [nvarchar](3) NULL,
		[ConceptValue6] [numeric](19, 6) NULL,
		[ConceptValueTotal6] [numeric](19, 6) NULL,
		[IDConcept7] [smallint] NULL,
		[ConceptColor7] [nvarchar](10) NULL,
		[ConceptName7] [nvarchar](3) NULL,
		[ConceptValue7] [numeric](19, 6) NULL,
		[ConceptValueTotal7] [numeric](19, 6) NULL,
		[IDConcept8] [smallint] NULL,
		[ConceptColor8] [nvarchar](10) NULL,
		[ConceptName8] [nvarchar](3) NULL,
		[ConceptValue8] [numeric](19, 6) NULL,
		[ConceptValueTotal8] [numeric](19, 6) NULL,
		[IDConcept9] [smallint] NULL,
		[ConceptColor9] [nvarchar](10) NULL,
		[ConceptName9] [nvarchar](3) NULL,
		[ConceptValue9] [numeric](19, 6) NULL,
		[ConceptValueTotal9] [numeric](19, 6) NULL,
		[IDConcept10] [smallint] NULL,
		[ConceptColor10] [nvarchar](10) NULL,
		[ConceptName10] [nvarchar](3) NULL,
		[ConceptValue10] [numeric](19, 6) NULL,
		[ConceptValueTotal10] [numeric](19, 6) NULL,
		[IDConcept11] [smallint] NULL,
		[ConceptColor11] [nvarchar](10) NULL,
		[ConceptName11] [nvarchar](3) NULL,
		[ConceptValue11] [numeric](19, 6) NULL,
		[ConceptValueTotal11] [numeric](19, 6) NULL,
		[IDConcept12] [smallint] NULL,
		[ConceptColor12] [nvarchar](10) NULL,
		[ConceptName12] [nvarchar](3) NULL,
		[ConceptValue12] [numeric](19, 6) NULL,
		[ConceptValueTotal12] [numeric](19, 6) NULL,
		[IDConcept13] [smallint] NULL,
		[ConceptColor13] [nvarchar](10) NULL,
		[ConceptName13] [nvarchar](3) NULL,
		[ConceptValue13] [numeric](19, 6) NULL,
		[ConceptValueTotal13] [numeric](19, 6) NULL,
		[IDConcept14] [smallint] NULL,
		[ConceptColor14] [nvarchar](10) NULL,
		[ConceptName14] [nvarchar](3) NULL,
		[ConceptValue14] [numeric](19, 6) NULL,
		[ConceptValueTotal14] [numeric](19, 6) NULL,
		[IDConcept15] [smallint] NULL,
		[ConceptColor15] [nvarchar](10) NULL,
		[ConceptName15] [nvarchar](3) NULL,
		[ConceptValue15] [numeric](19, 6) NULL,
		[ConceptValueTotal15] [numeric](19, 6) NULL,
		[IDConcept16] [smallint] NULL,
		[ConceptColor16] [nvarchar](10) NULL,
		[ConceptName16] [nvarchar](3) NULL,
		[ConceptValue16] [numeric](19, 6) NULL,
		[ConceptValueTotal16] [numeric](19, 6) NULL,
		[IDConcept17] [smallint] NULL,
		[ConceptColor17] [nvarchar](10) NULL,
		[ConceptName17] [nvarchar](3) NULL,
		[ConceptValue17] [numeric](19, 6) NULL,
		[ConceptValueTotal17] [numeric](19, 6) NULL,
		[IDConcept18] [smallint] NULL,
		[ConceptColor18] [nvarchar](10) NULL,
		[ConceptName18] [nvarchar](3) NULL,
		[ConceptValue18] [numeric](19, 6) NULL,
		[ConceptValueTotal18] [numeric](19, 6) NULL,
		[IDConcept19] [smallint] NULL,
		[ConceptColor19] [nvarchar](10) NULL,
		[ConceptName19] [nvarchar](3) NULL,
		[ConceptValue19] [numeric](19, 6) NULL,
		[ConceptValueTotal19] [numeric](19, 6) NULL,
		[IDConcept20] [smallint] NULL,
		[ConceptColor20] [nvarchar](10) NULL,
		[ConceptName20] [nvarchar](3) NULL,
		[ConceptValue20] [numeric](19, 6) NULL,
		[ConceptValueTotal20] [numeric](19, 6) NULL,
		[IDConcept21] [smallint] NULL,
		[ConceptColor21] [nvarchar](10) NULL,
		[ConceptName21] [nvarchar](3) NULL,
		[ConceptValue21] [numeric](19, 6) NULL,
		[ConceptValueTotal21] [numeric](19, 6) NULL,
		[IDConcept22] [smallint] NULL,
		[ConceptColor22] [nvarchar](10) NULL,
		[ConceptName22] [nvarchar](3) NULL,
		[ConceptValue22] [numeric](19, 6) NULL,
		[ConceptValueTotal22] [numeric](19, 6) NULL,
		[IDConcept23] [smallint] NULL,
		[ConceptColor23] [nvarchar](10) NULL,
		[ConceptName23] [nvarchar](3) NULL,
		[ConceptValue23] [numeric](19, 6) NULL,
		[ConceptValueTotal23] [numeric](19, 6) NULL,
		[IDConcept24] [smallint] NULL,
		[ConceptColor24] [nvarchar](10) NULL,
		[ConceptName24] [nvarchar](3) NULL,
		[ConceptValue24] [numeric](19, 6) NULL,
		[ConceptValueTotal24] [numeric](19, 6) NULL,
		[IDConcept25] [smallint] NULL,
		[ConceptColor25] [nvarchar](10) NULL,
		[ConceptName25] [nvarchar](3) NULL,
		[ConceptValue25] [numeric](19, 6) NULL,
		[ConceptValueTotal25] [numeric](19, 6) NULL,
		[IDConcept26] [smallint] NULL,
		[ConceptColor26] [nvarchar](10) NULL,
		[ConceptName26] [nvarchar](3) NULL,
		[ConceptValue26] [numeric](19, 6) NULL,
		[ConceptValueTotal26] [numeric](19, 6) NULL,
		[IDConcept27] [smallint] NULL,
		[ConceptColor27] [nvarchar](10) NULL,
		[ConceptName27] [nvarchar](3) NULL,
		[ConceptValue27] [numeric](19, 6) NULL,
		[ConceptValueTotal27] [numeric](19, 6) NULL,
		[IDConcept28] [smallint] NULL,
		[ConceptColor28] [nvarchar](10) NULL,
		[ConceptName28] [nvarchar](3) NULL,
		[ConceptValue28] [numeric](19, 6) NULL,
		[ConceptValueTotal28] [numeric](19, 6) NULL,
		[IDConcept29] [smallint] NULL,
		[ConceptColor29] [nvarchar](10) NULL,
		[ConceptName29] [nvarchar](3) NULL,
		[ConceptValue29] [numeric](19, 6) NULL,
		[ConceptValueTotal29] [numeric](19, 6) NULL,
		[IDConcept30] [smallint] NULL,
		[ConceptColor30] [nvarchar](10) NULL,
		[ConceptName30] [nvarchar](3) NULL,
		[ConceptValue30] [numeric](19, 6) NULL,
		[ConceptValueTotal30] [numeric](19, 6) NULL,
		[IDConcept31] [smallint] NULL,
		[ConceptColor31] [nvarchar](10) NULL,
		[ConceptName31] [nvarchar](3) NULL,
		[ConceptValue31] [numeric](19, 6) NULL,
		[ConceptValueTotal31] [numeric](19, 6) NULL,
		[AbsencesTemplate] [nvarchar](31) NULL,
	 CONSTRAINT [PK_TmpAnnualConceptsEmployee] PRIMARY KEY CLUSTERED 
	(
		[IDPassport] ASC,
		[IDEmployee] ASC,
		[Ejercicio] ASC,
		[Periodo] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TmpAnnualConceptsEmployee')
	BEGIN

		CREATE TABLE TMPCALENDAREMPLOYEE (
			[IDEmployee] [numeric](18, 0) not NULL,
			[EMPLEADO] [nvarchar] (50) null,
			[MES] [int] NOT NULL ,
			[NOMBRE] [nvarchar] (50) NULL ,
			[DIAS] [int] NULL ,
			[horasdia1] [numeric] (9, 6) NOT NULL ,
			[horasdia2] [numeric] (9, 6) NOT NULL ,
			[horasdia3] [numeric] (9, 6) NOT NULL ,
			[horasdia4] [numeric] (9, 6) NOT NULL ,
			[horasdia5] [numeric] (9, 6) NOT NULL ,
			[horasdia6] [numeric] (9, 6) NOT NULL ,
			[horasdia7] [numeric] (9, 6) NOT NULL ,
			[horasdia8] [numeric] (9, 6) NOT NULL ,
			[horasdia9] [numeric] (9, 6) NOT NULL ,
			[horasdia10] [numeric] (9, 6) NOT NULL ,
			[horasdia11] [numeric] (9, 6) NOT NULL ,
			[horasdia12] [numeric] (9, 6) NOT NULL ,
			[horasdia13] [numeric] (9, 6) NOT NULL ,
			[horasdia14] [numeric] (9, 6) NOT NULL ,
			[horasdia15] [numeric] (9, 6) NOT NULL ,
			[horasdia16] [numeric] (9, 6) NOT NULL ,
			[horasdia17] [numeric] (9, 6) NOT NULL ,
			[horasdia18] [numeric] (9, 6) NOT NULL ,
			[horasdia19] [numeric] (9, 6) NOT NULL ,
			[horasdia20] [numeric] (9, 6) NOT NULL ,
			[horasdia21] [numeric] (9, 6) NOT NULL ,
			[horasdia22] [numeric] (9, 6) NOT NULL ,
			[horasdia23] [numeric] (9, 6) NOT NULL ,
			[horasdia24] [numeric] (9, 6) NOT NULL ,
			[horasdia25] [numeric] (9, 6) NOT NULL ,
			[horasdia26] [numeric] (9, 6) NOT NULL ,
			[horasdia27] [numeric] (9, 6) NOT NULL ,
			[horasdia28] [numeric] (9, 6) NOT NULL ,
			[horasdia29] [numeric] (9, 6) NOT NULL ,
			[horasdia30] [numeric] (9, 6) NOT NULL ,
			[horasdia31] [numeric] (9, 6) NOT NULL ,
			[TOTALHORAS] [nvarchar] (50) NULL ,
			[TOTALDIAS] [int] NULL ,
			[TotalHorasAnuales] [numeric](18, 2)  NULL,
			[TotalDiasAnuales] [numeric](18, 0)  NULL
		) ON [PRIMARY]

		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] WITH NOCHECK ADD 
			CONSTRAINT [PK_TMPCALENDAREMPLOYEE] PRIMARY KEY  NONCLUSTERED 
			([IDEmployee],[MES])  ON [PRIMARY] 

		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia1]  DEFAULT ((0)) FOR [horasdia1]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia2]  DEFAULT ((0)) FOR [horasdia2]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia3]  DEFAULT ((0)) FOR [horasdia3]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia4]  DEFAULT ((0)) FOR [horasdia4]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia5]  DEFAULT ((0)) FOR [horasdia5]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia6]  DEFAULT ((0)) FOR [horasdia6]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia7]  DEFAULT ((0)) FOR [horasdia7]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia8]  DEFAULT ((0)) FOR [horasdia8]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia9]  DEFAULT ((0)) FOR [horasdia9]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia10]  DEFAULT ((0)) FOR [horasdia10]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia11]  DEFAULT ((0)) FOR [horasdia11]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia12]  DEFAULT ((0)) FOR [horasdia12]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia13]  DEFAULT ((0)) FOR [horasdia13]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia14]  DEFAULT ((0)) FOR [horasdia14]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia15]  DEFAULT ((0)) FOR [horasdia15]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia16]  DEFAULT ((0)) FOR [horasdia16]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia17]  DEFAULT ((0)) FOR [horasdia17]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia18]  DEFAULT ((0)) FOR [horasdia18]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia19]  DEFAULT ((0)) FOR [horasdia19]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia20]  DEFAULT ((0)) FOR [horasdia20]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia21]  DEFAULT ((0)) FOR [horasdia21]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia22]  DEFAULT ((0)) FOR [horasdia22]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia23]  DEFAULT ((0)) FOR [horasdia23]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia24]  DEFAULT ((0)) FOR [horasdia24]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia25]  DEFAULT ((0)) FOR [horasdia25]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia26]  DEFAULT ((0)) FOR [horasdia26]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia27]  DEFAULT ((0)) FOR [horasdia27]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia28]  DEFAULT ((0)) FOR [horasdia28]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia29]  DEFAULT ((0)) FOR [horasdia29]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia30]  DEFAULT ((0)) FOR [horasdia30]
		ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] ADD  CONSTRAINT [DF_TMPCALENDAREMPLOYEE_horasdia31]  DEFAULT ((0)) FOR [horasdia31]


	END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TMPCALENDAREMPLOYEEByContract')
	CREATE TABLE [dbo].[TMPCALENDAREMPLOYEEByContract](
		[IDEmployee] [numeric](18, 0) NOT NULL,
		[EMPLEADO] [nvarchar](50) NULL,
		[idContract] [nvarchar](50) NOT NULL,
		[MES] [int] NOT NULL,
		[NOMBRE] [nvarchar](50) NULL,
		[DIAS] [int] NULL,
		[horasdia1] [numeric](9, 6) NOT NULL,
		[horasdia2] [numeric](9, 6) NOT NULL,
		[horasdia3] [numeric](9, 6) NOT NULL,
		[horasdia4] [numeric](9, 6) NOT NULL,
		[horasdia5] [numeric](9, 6) NOT NULL,
		[horasdia6] [numeric](9, 6) NOT NULL,
		[horasdia7] [numeric](9, 6) NOT NULL,
		[horasdia8] [numeric](9, 6) NOT NULL,
		[horasdia9] [numeric](9, 6) NOT NULL,
		[horasdia10] [numeric](9, 6) NOT NULL,
		[horasdia11] [numeric](9, 6) NOT NULL,
		[horasdia12] [numeric](9, 6) NOT NULL,
		[horasdia13] [numeric](9, 6) NOT NULL,
		[horasdia14] [numeric](9, 6) NOT NULL,
		[horasdia15] [numeric](9, 6) NOT NULL,
		[horasdia16] [numeric](9, 6) NOT NULL,
		[horasdia17] [numeric](9, 6) NOT NULL,
		[horasdia18] [numeric](9, 6) NOT NULL,
		[horasdia19] [numeric](9, 6) NOT NULL,
		[horasdia20] [numeric](9, 6) NOT NULL,
		[horasdia21] [numeric](9, 6) NOT NULL,
		[horasdia22] [numeric](9, 6) NOT NULL,
		[horasdia23] [numeric](9, 6) NOT NULL,
		[horasdia24] [numeric](9, 6) NOT NULL,
		[horasdia25] [numeric](9, 6) NOT NULL,
		[horasdia26] [numeric](9, 6) NOT NULL,
		[horasdia27] [numeric](9, 6) NOT NULL,
		[horasdia28] [numeric](9, 6) NOT NULL,
		[horasdia29] [numeric](9, 6) NOT NULL,
		[horasdia30] [numeric](9, 6) NOT NULL,
		[horasdia31] [numeric](9, 6) NOT NULL,
		[TOTALHORAS] [nvarchar](50) NULL,
		[TOTALDIAS] [int] NULL,
		[TotalHorasAnuales] [numeric](18, 2) NULL,
		[TotalDiasAnuales] [numeric](18, 0) NULL,
	 CONSTRAINT [PK_TMPCALENDAREMPLOYEEByContract] PRIMARY KEY CLUSTERED 
	(
		[IDEmployee] ASC,
		[idContract] ASC,
		[MES] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TMPANNUALINDIVIDUALCALENDARByContract')
	BEGIN
		CREATE TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract](
			[IDEmployee] [numeric](18, 0) NOT NULL,	
			[EMPLEADO] [nvarchar](50) NULL,
			[MES] [int] NOT NULL,
			[AÑO] [int] NOT NULL,
			[idContract] [nvarchar](50) NOT NULL,
			[NOMBRE] [nvarchar](50) NULL,
			[DIAS] [int] NULL,
			[horasdia1] [numeric](9, 6) NOT NULL,
			[horasdia2] [numeric](9, 6) NOT NULL,
			[horasdia3] [numeric](9, 6) NOT NULL,
			[horasdia4] [numeric](9, 6) NOT NULL,
			[horasdia5] [numeric](9, 6) NOT NULL,
			[horasdia6] [numeric](9, 6) NOT NULL,
			[horasdia7] [numeric](9, 6) NOT NULL,
			[horasdia8] [numeric](9, 6) NOT NULL,
			[horasdia9] [numeric](9, 6) NOT NULL,
			[horasdia10] [numeric](9, 6) NOT NULL,
			[horasdia11] [numeric](9, 6) NOT NULL,
			[horasdia12] [numeric](9, 6) NOT NULL,
			[horasdia13] [numeric](9, 6) NOT NULL,
			[horasdia14] [numeric](9, 6) NOT NULL,
			[horasdia15] [numeric](9, 6) NOT NULL,
			[horasdia16] [numeric](9, 6) NOT NULL,
			[horasdia17] [numeric](9, 6) NOT NULL,
			[horasdia18] [numeric](9, 6) NOT NULL,
			[horasdia19] [numeric](9, 6) NOT NULL,
			[horasdia20] [numeric](9, 6) NOT NULL,
			[horasdia21] [numeric](9, 6) NOT NULL,
			[horasdia22] [numeric](9, 6) NOT NULL,
			[horasdia23] [numeric](9, 6) NOT NULL,
			[horasdia24] [numeric](9, 6) NOT NULL,
			[horasdia25] [numeric](9, 6) NOT NULL,
			[horasdia26] [numeric](9, 6) NOT NULL,
			[horasdia27] [numeric](9, 6) NOT NULL,
			[horasdia28] [numeric](9, 6) NOT NULL,
			[horasdia29] [numeric](9, 6) NOT NULL,
			[horasdia30] [numeric](9, 6) NOT NULL,
			[horasdia31] [numeric](9, 6) NOT NULL,
			[TOTALHORAS] [numeric](9, 6) NOT NULL,
			[TOTALDIAS] [int] NULL,
			[TotalHorasAnuales] [numeric](18, 2) NULL,
			[TotalDiasAnuales] [numeric](18, 0) NULL,
		CONSTRAINT [PK_TMPANNUALINDIVIDUALCALENDARByContract] PRIMARY KEY NONCLUSTERED 
			([IDEmployee] ASC,[MES] ASC,[AÑO] ASC,[IDContract] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia1]  DEFAULT ((0)) FOR [horasdia1]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia2]  DEFAULT ((0)) FOR [horasdia2]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia3]  DEFAULT ((0)) FOR [horasdia3]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia4]  DEFAULT ((0)) FOR [horasdia4]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia5]  DEFAULT ((0)) FOR [horasdia5]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia6]  DEFAULT ((0)) FOR [horasdia6]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia7]  DEFAULT ((0)) FOR [horasdia7]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia8]  DEFAULT ((0)) FOR [horasdia8]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia9]  DEFAULT ((0)) FOR [horasdia9]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia10]  DEFAULT ((0)) FOR [horasdia10]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia11]  DEFAULT ((0)) FOR [horasdia11]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia12]  DEFAULT ((0)) FOR [horasdia12]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia13]  DEFAULT ((0)) FOR [horasdia13]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia14]  DEFAULT ((0)) FOR [horasdia14]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia15]  DEFAULT ((0)) FOR [horasdia15]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia16]  DEFAULT ((0)) FOR [horasdia16]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia17]  DEFAULT ((0)) FOR [horasdia17]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia18]  DEFAULT ((0)) FOR [horasdia18]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia19]  DEFAULT ((0)) FOR [horasdia19]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia20]  DEFAULT ((0)) FOR [horasdia20]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia21]  DEFAULT ((0)) FOR [horasdia21]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia22]  DEFAULT ((0)) FOR [horasdia22]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia23]  DEFAULT ((0)) FOR [horasdia23]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia24]  DEFAULT ((0)) FOR [horasdia24]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia25]  DEFAULT ((0)) FOR [horasdia25]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia26]  DEFAULT ((0)) FOR [horasdia26]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia27]  DEFAULT ((0)) FOR [horasdia27]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia28]  DEFAULT ((0)) FOR [horasdia28]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia29]  DEFAULT ((0)) FOR [horasdia29]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia30]  DEFAULT ((0)) FOR [horasdia30]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia31]  DEFAULT ((0)) FOR [horasdia31]
		ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_TOTALHORAS]  DEFAULT ((0)) FOR [TOTALHORAS]
	END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TMPCONCEPT')
	CREATE TABLE [dbo].[TMPCONCEPT] (
		[ConceptName] [nvarchar] (64) NULL ,
		[ShortName] [nvarchar] (3) NULL
	) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TMPDetailedCalendarEmployee')
	BEGIN
		CREATE TABLE [dbo].[TMPDetailedCalendarEmployee](
			[IdGroup] [int] NOT NULL,
			[IDEmployee] [int] NOT NULL,
			[DatePlanification] [smalldatetime] NOT NULL,
			[GroupName] [nvarchar](200) NULL,
			[EmployeeName] [nvarchar](50) NULL,
			[PuncheIn1] [smalldatetime] NULL,
			[PuncheOut1] [smalldatetime] NULL,
			[PuncheIn2] [smalldatetime] NULL,
			[PuncheOut2] [smalldatetime] NULL,
			[PuncheIn3] [smalldatetime] NULL,
			[PuncheOut3] [smalldatetime] NULL,
			[PuncheIn4] [smalldatetime] NULL,
			[PuncheOut4] [smalldatetime] NULL,
			[PuncheIn5] [smalldatetime] NULL,
			[PuncheOut5] [smalldatetime] NULL,
			[IdShift] [smallint] NOT NULL,
			[IDConcept1] [smallint] NOT NULL,
			[ConceptName1] [nvarchar](3) NULL,
			[FormatConcept1] [nvarchar](1) NULL,
			[ValueConcept1] [numeric](19, 6) NOT NULL,
			[IDConcept2] [smallint] NOT NULL,
			[ConceptName2] [nvarchar](3) NULL,
			[FormatConcept2] [nvarchar](1) NULL,
			[ValueConcept2] [numeric](19, 6) NOT NULL,
			[IDConcept3] [smallint] NOT NULL,
			[ConceptName3] [nvarchar](3) NULL,
			[FormatConcept3] [nvarchar](1) NULL,
			[ValueConcept3] [numeric](19, 6) NOT NULL,
			[IDConcept4] [smallint] NOT NULL,
			[ConceptName4] [nvarchar](3) NULL,
			[FormatConcept4] [nvarchar](1) NULL,
			[ValueConcept4] [numeric](19, 6) NOT NULL,
			[IDConcept5] [smallint] NOT NULL,
			[ConceptName5] [nvarchar](3) NULL,
			[FormatConcept5] [nvarchar](1) NULL,
			[ValueConcept5] [numeric](19, 6) NOT NULL,
			[IDConcept6] [smallint] NOT NULL,
			[ConceptName6] [nvarchar](3) NULL,
			[FormatConcept6] [nvarchar](1) NULL,
			[ValueConcept6] [numeric](19, 6) NOT NULL,
			[IDConcept7] [smallint] NOT NULL,
			[ConceptName7] [nvarchar](3) NULL,
			[FormatConcept7] [nvarchar](1) NULL,
			[ValueConcept7] [numeric](19, 6) NOT NULL,
			[IDConcept8] [smallint] NOT NULL,
			[ConceptName8] [nvarchar](3) NULL,
			[FormatConcept8] [nvarchar](1) NULL,
			[ValueConcept8] [numeric](19, 6) NOT NULL,
			[Cause] [tinyint] NOT NULL,
		 CONSTRAINT [PK_TMPDetailedCalendarEmployee] PRIMARY KEY CLUSTERED 
		(
			[IdGroup] ASC,
			[IDEmployee] ASC,
			[DatePlanification] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		) ON [PRIMARY]
		
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDEmployee]  DEFAULT ((0)) FOR [IDEmployee]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IdShift]  DEFAULT ((0)) FOR [IdShift]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDConcept1]  DEFAULT ((0)) FOR [IDConcept1]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_ValueConcept1]  DEFAULT ((0)) FOR [ValueConcept1]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDConcept2]  DEFAULT ((0)) FOR [IDConcept2]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_ValueConcept2]  DEFAULT ((0)) FOR [ValueConcept2]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDConcept3]  DEFAULT ((0)) FOR [IDConcept3]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_ValueConcept3]  DEFAULT ((0)) FOR [ValueConcept3]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDConcept4]  DEFAULT ((0)) FOR [IDConcept4]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_ValueConcept4]  DEFAULT ((0)) FOR [ValueConcept4]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDConcept5]  DEFAULT ((0)) FOR [IDConcept5]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_ValueConcept5]  DEFAULT ((0)) FOR [ValueConcept5]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDConcept6]  DEFAULT ((0)) FOR [IDConcept6]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_ValueConcept6]  DEFAULT ((0)) FOR [ValueConcept6]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDConcept7]  DEFAULT ((0)) FOR [IDConcept7]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_ValueConcept7]  DEFAULT ((0)) FOR [ValueConcept7]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDConcept8]  DEFAULT ((0)) FOR [IDConcept8]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_ValueConcept8]  DEFAULT ((0)) FOR [ValueConcept8]
		ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_Cause]  DEFAULT ((0)) FOR [Cause]
	END
GO

CREATE VIEW dbo.sysrovwAbsences
AS
SELECT row_number() OVER(order by x.BeginDate) AS ID, x.* ,emp.Name as EmployeeName, cause.Name as CauseName, grp.IDGroup, grp.FullGroupName,
        grp.Path, grp.CurrentEmployee, grp.BeginDate AS BeginDateMobility, grp.EndDate AS EndDateMobility
 FROM (
 (SELECT 	NULL AS IDRelatedObject,
 			IDCause AS IDCause, 
 			IDEmployee AS IDEmployee, 
 			BeginDate AS BeginDate, 
 			ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) AS FinishDate, 
 			NULL AS BeginTime, 
 			NULL AS EndTime, 
 			CONVERT(NVARCHAR(4000),Description) AS Description ,
 			(SELECT CASE WHEN (select COUNT(*) from AbsenceTracking at where TrackDay < GETDATE() and DeliveryDate is null and at.IDCause = pa.IDCause and at.IDEmployee = pa.IDEmployee and at.Date = pa.BeginDate) > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered, 
 			(SELECT CASE WHEN GETDATE() between BeginDate and DATEADD(DAY,1,(ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )))) THEN '1' WHEN ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) < GETDATE()  THEN '2'  ELSE '0' END) AS Status,
 			'ProgrammedAbsence' AS AbsenceType 
  FROM dbo.ProgrammedAbsences pa) union
 (SELECT     ID AS IDRelatedObject,
 			IDCause AS IDCause, 
 			IDEmployee AS IDEmployee, 
 			Date AS BeginDate, 
 			ISNULL(FinishDate,Date) AS FinishDate, 
 			BeginTime AS BeginTime, 
 			EndTime AS EndTime, 
 			CONVERT(NVARCHAR(4000),Description) AS Description, 
 			(SELECT CASE WHEN (select COUNT(*) from AbsenceTracking at where TrackDay < GETDATE() and DeliveryDate is null and at.IDCause = pc.IDCause and at.IDEmployee = pc.IDEmployee and at.Date = pc.Date and at.IDAbsence = pc.ID) > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered, 
 			(SELECT CASE WHEN GETDATE() between Date and DATEADD(DAY,1,(ISNULL(FinishDate, Date))) THEN '1' WHEN  DATEADD(DAY,1,ISNULL(FinishDate,Date)) < GETDATE()  THEN '2'  ELSE '0' END) AS Status,
 			'ProgrammedCause' AS AbsenceType 
  FROM dbo.ProgrammedCauses pc) union
 (SELECT     ID AS IDRelatedObject,
 			IDCause AS IDCause, 
 			IDEmployee AS IDEmployee, 
 			Date1 AS BeginDate, 
 			ISNULL(Date2,Date1) AS FinishDate, 
 			FromTime AS BeginTime, 
 			ToTime AS EndTime, 
 			CONVERT(NVARCHAR(4000),Comments) AS Description, 
 			1 AS DocumentsDelivered,
 			(SELECT CASE WHEN Status = 0 THEN '-1' ELSE '-2' END) AS Status,
 			(SELECT CASE WHEN RequestType = 7 THEN 'RequestAbsence' ELSE 'RequestCause' END) AS AbsenceType 
  FROM dbo.Requests where RequestType in(7,9) and Status in(0,1))
  ) x
  inner join dbo.Employees as emp on x.IDEmployee = emp.ID
  inner join dbo.Causes as cause on x.IDCause = cause.ID
  inner join dbo.sysrovwCurrentEmployeeGroups as grp on x.IDEmployee = grp.IDEmployee
GO

CREATE VIEW [dbo].[sysroCurrentTasksStatus]
AS
SELECT     dbo.Tasks.ID, dbo.BusinessCenters.Name AS Center, dbo.BusinessCenters.ID AS IDCenter, dbo.Tasks.Name, dbo.Tasks.Status, dbo.Tasks.Color, dbo.Tasks.Project, 
                      dbo.Tasks.Tag, dbo.Tasks.Priority, dbo.Tasks.ActivationTask, Tasks_1.Color AS ColorTaskPrev, CONVERT(numeric(25, 4), ISNULL(dbo.Tasks.InitialTime, 0) 
                      + ISNULL(dbo.Tasks.TimeChangedRequirements, 0) + ISNULL(dbo.Tasks.ForecastErrorTime, 0) + ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) 
                      + ISNULL(dbo.Tasks.EmployeeTime, 0) + ISNULL(dbo.Tasks.TeamTime, 0) + ISNULL(dbo.Tasks.MaterialTime, 0) + ISNULL(dbo.Tasks.OtherTime, 0)) AS Duration,
                          (SELECT     ISNULL(CONVERT(numeric(25, 4), SUM(Value)), 0) AS Expr1
                            FROM          dbo.DailyTaskAccruals
                            WHERE      (IDTask = dbo.Tasks.ID)) AS Worked, CASE WHEN CONVERT(numeric(25, 4), isnull(dbo.Tasks.InitialTime, 0) 
                      + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0) + isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) 
                      + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0) + isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) 
                      > 0 THEN CASE WHEN
                          (SELECT     isnull(CONVERT(numeric(25, 4), SUM(Value)), 0)
                            FROM          DailyTaskAccruals
                            WHERE      DailyTaskAccruals.IDTask = dbo.Tasks.ID) * 100 / CONVERT(numeric(25, 4), isnull(dbo.Tasks.InitialTime, 0) 
                      + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0) + isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) 
                      + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0) + isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) 
                      > 100.00 THEN 100 ELSE
                          (SELECT     isnull(CONVERT(numeric(25, 4), SUM(Value)), 0)
                            FROM          DailyTaskAccruals
                            WHERE      DailyTaskAccruals.IDTask = dbo.Tasks.ID) * 100 / CONVERT(numeric(25, 4), isnull(dbo.Tasks.InitialTime, 0) 
                      + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0) + isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) 
                      + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0) + isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) 
                      END ELSE 0 END AS Progress, CASE WHEN CONVERT(numeric(25, 4), isnull(dbo.Tasks.InitialTime, 0) + isnull(dbo.Tasks.TimeChangedRequirements, 0) 
                      + isnull(dbo.Tasks.ForecastErrorTime, 0) + isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0) 
                      + isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) > 0 THEN CASE WHEN
                          (SELECT     isnull(CONVERT(numeric(25, 4), SUM(Value)), 0)
                            FROM          DailyTaskAccruals
                            WHERE      DailyTaskAccruals.IDTask = dbo.Tasks.ID) * 100 / CONVERT(numeric(25, 4), isnull(dbo.Tasks.InitialTime, 0) 
                      + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0) + isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) 
                      + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0) + isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) 
                      > 100.00 THEN 1 ELSE 0 END ELSE 0 END AS Exceeded, dbo.Tasks.IDPassport, dbo.Tasks.ExpectedStartDate, dbo.Tasks.ExpectedEndDate, 
                      Tasks_1.Name AS NameTaskPrev, dbo.Tasks.ShortName, CONVERT(nvarchar(200),
                          (SELECT     COUNT(DISTINCT LastPunch.IDEmployee) AS Expr1
                            FROM          (SELECT     ID AS IDEmployee,
                                                                               (SELECT     TOP (1) TypeData
                                                                                 FROM          dbo.Punches
                                                                                 WHERE      (ActualType = 4) AND (IDEmployee = dbo.Employees.ID)
                                                                                 ORDER BY DateTime DESC) AS IDTask
                                                    FROM          dbo.Employees) AS LastPunch INNER JOIN
                                                   dbo.EmployeeStatus ON LastPunch.IDEmployee = dbo.EmployeeStatus.IDEmployee
                            WHERE      (LastPunch.IDTask = dbo.Tasks.ID) AND (dbo.EmployeeStatus.LastPunch >= DATEADD(day, - 4, GETDATE())))) + '@' + CONVERT(nvarchar(200),
                          (SELECT     COUNT(DISTINCT LastPunch_2.IDEmployee) AS Expr1
                            FROM          (SELECT     ID AS IDEmployee,
                                                                               (SELECT     TOP (1) DateTime
                                                                                 FROM          dbo.Punches AS Punches_2
                                                                                 WHERE      (ActualType = 4) AND (IDEmployee = Employees_2.ID) AND (TypeData = dbo.Tasks.ID)
                                                                                 ORDER BY DateTime DESC) AS PunchDate
                                                    FROM          dbo.Employees AS Employees_2) AS LastPunch_2 INNER JOIN
                                                   dbo.EmployeeStatus AS EmployeeStatus_2 ON LastPunch_2.IDEmployee = EmployeeStatus_2.IDEmployee
                            WHERE      (LastPunch_2.PunchDate > DATEADD(day, - 7, GETDATE())) AND (LastPunch_2.PunchDate IS NOT NULL) AND (LastPunch_2.IDEmployee NOT IN
                                                       (SELECT DISTINCT LastPunch_1.IDEmployee
                                                         FROM          (SELECT     ID AS IDEmployee,
                                                                                                            (SELECT     TOP (1) TypeData
                                                                                                              FROM          dbo.Punches AS Punches_1
                                                                                                              WHERE      (ActualType = 4) AND (IDEmployee = Employees_1.ID)
                                                                                                              ORDER BY DateTime DESC) AS IDTask
                                                                                 FROM          dbo.Employees AS Employees_1) AS LastPunch_1 INNER JOIN
                                                                                dbo.EmployeeStatus AS EmployeeStatus_1 ON LastPunch_1.IDEmployee = EmployeeStatus_1.IDEmployee
                                                         WHERE      (LastPunch_1.IDTask = dbo.Tasks.ID) AND (EmployeeStatus_1.LastPunch >= DATEADD(day, - 4, GETDATE())))))) AS Employees,
						(select COUNT(*) FROM dbo.AlertsTask WHERE dbo.AlertsTask.IDTask = dbo.Tasks.ID AND IsReaded =0) as Alerts                                                          
FROM         dbo.Tasks LEFT OUTER JOIN
                      dbo.Tasks AS Tasks_1 ON dbo.Tasks.ActivationTask = Tasks_1.ID INNER JOIN
                      dbo.BusinessCenters ON dbo.Tasks.IDCenter = dbo.BusinessCenters.ID

GO

CREATE VIEW dbo.sysroScheduleCube1
AS
SELECT     TOP (100) PERCENT CAST(dbo.DailyAccruals.IDEmployee AS varchar) + '-' + CAST(dbo.DailyAccruals.IDConcept AS varchar) 
                      + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.DailyAccruals.Date, 120), 
                      10) AS KeyView, dbo.DailyAccruals.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.DailyAccruals.IDConcept, 
                      dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name AS EmployeeName, 
                      dbo.Concepts.Name AS ConceptName, dbo.DailyAccruals.Date, SUM(dbo.DailyAccruals.Value) AS Value, COUNT(*) AS Count, MONTH(dbo.DailyAccruals.Date) AS Mes, 
                      YEAR(dbo.DailyAccruals.Date) AS A�o, (DATEPART(dw, dbo.DailyAccruals.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, 
                      dbo.DailyAccruals.Date) AS WeekOfYear, DATEPART(dy, dbo.DailyAccruals.Date) AS DayOfYear, dbo.sysroEmployeeGroups.Path, 
                      dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate
FROM         dbo.DailyAccruals INNER JOIN
                      dbo.sysroEmployeeGroups ON dbo.DailyAccruals.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND 
                      dbo.DailyAccruals.Date >= dbo.sysroEmployeeGroups.BeginDate AND dbo.DailyAccruals.Date <= dbo.sysroEmployeeGroups.EndDate INNER JOIN
                      dbo.EmployeeContracts ON dbo.DailyAccruals.IDEmployee = dbo.EmployeeContracts.IDEmployee AND 
                      dbo.DailyAccruals.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyAccruals.Date <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                      dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID LEFT OUTER JOIN
                      dbo.Concepts ON dbo.DailyAccruals.IDConcept = dbo.Concepts.ID
GROUP BY dbo.DailyAccruals.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.DailyAccruals.IDConcept, dbo.Concepts.Name, 
                      dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name, dbo.DailyAccruals.Date, MONTH(dbo.DailyAccruals.Date), 
                      YEAR(dbo.DailyAccruals.Date), (DATEPART(dw, dbo.DailyAccruals.Date) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, dbo.DailyAccruals.Date), DATEPART(dy, 
                      dbo.DailyAccruals.Date), CAST(dbo.DailyAccruals.IDEmployee AS varchar) + '-' + CAST(dbo.DailyAccruals.IDConcept AS varchar) 
                      + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.DailyAccruals.Date, 120), 
                      10), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, 
                      dbo.sysroEmployeeGroups.EndDate
ORDER BY dbo.DailyAccruals.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.DailyAccruals.IDConcept
GO

CREATE VIEW dbo.sysroScheduleCube2
AS
SELECT     CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
                      + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10) AS KeyView, 
                      dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, 
                      dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeesShifts.ShiftName, 
                      dbo.sysroEmployeesShifts.CurrentDate AS Date, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours, COUNT(*) AS Count, 
                      MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS A�o, (DATEPART(dw, 
                      dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear, 
                      DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, 
                      dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate
FROM         dbo.sysroEmployeesShifts INNER JOIN
                      dbo.sysroEmployeeGroups ON dbo.sysroEmployeesShifts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND 
                      dbo.sysroEmployeesShifts.CurrentDate >= dbo.sysroEmployeeGroups.BeginDate AND 
                      dbo.sysroEmployeesShifts.CurrentDate <= dbo.sysroEmployeeGroups.EndDate INNER JOIN
                      dbo.EmployeeContracts ON dbo.sysroEmployeesShifts.IDEmployee = dbo.EmployeeContracts.IDEmployee AND 
                      dbo.sysroEmployeesShifts.CurrentDate >= dbo.EmployeeContracts.BeginDate AND 
                      dbo.sysroEmployeesShifts.CurrentDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                      dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
GROUP BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name, 
                      dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate, MONTH(dbo.sysroEmployeesShifts.CurrentDate), 
                      YEAR(dbo.sysroEmployeesShifts.CurrentDate), (DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, 
                      dbo.sysroEmployeesShifts.CurrentDate), DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate), dbo.sysroEmployeeGroups.IDGroup, 
                      dbo.EmployeeContracts.IDContract, CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
                      + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10), dbo.sysroEmployeeGroups.Path, 
                      dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate
GO

CREATE VIEW dbo.sysroScheduleCube3
AS
SELECT     CONVERT(varchar(10), dbo.DailySchedule.IDEmployee, 120) + '-' + CAST(dbo.EmployeeGroups.IDGroup AS varchar) 
                      + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120) AS KeyView, dbo.EmployeeGroups.IDGroup, 
                      dbo.EmployeeContracts.IDContract, dbo.DailySchedule.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description AS IncidenceName, 
                      dbo.TimeZones.Name AS ZoneTime, dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS Value, YEAR(dbo.DailySchedule.Date) AS A�o, 
                      (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailySchedule.Date) AS WeekOfYear, DATEPART(dy, 
                      dbo.DailySchedule.Date) AS DayOfYear, CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName, 
                      dbo.Employees.Name AS EmployeeName, dbo.Groups.Name AS GroupName, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName, dbo.Groups.Path, 
                      dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee AS CurrentEmployee
FROM         dbo.EmployeeGroups INNER JOIN
                      dbo.Causes INNER JOIN
                      dbo.DailyCauses ON dbo.Causes.ID = dbo.DailyCauses.IDCause INNER JOIN
                      dbo.DailySchedule ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date ON 
                      dbo.EmployeeGroups.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.EmployeeGroups.BeginDate <= dbo.DailySchedule.Date AND 
                      dbo.EmployeeGroups.EndDate >= dbo.DailySchedule.Date INNER JOIN
                      dbo.EmployeeContracts ON dbo.DailySchedule.IDEmployee = dbo.EmployeeContracts.IDEmployee AND 
                      dbo.DailySchedule.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailySchedule.Date <= dbo.EmployeeContracts.EndDate INNER JOIN
                      dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                      dbo.sysrosubvwCurrentEmployeePeriod ON dbo.DailySchedule.IDEmployee = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee LEFT OUTER JOIN
                      dbo.Employees ON dbo.DailySchedule.IDEmployee = dbo.Employees.ID LEFT OUTER JOIN
                      dbo.sysroDailyIncidencesDescription INNER JOIN
                      dbo.DailyIncidences INNER JOIN
                      dbo.TimeZones ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID ON dbo.sysroDailyIncidencesDescription.IDIncidence = dbo.DailyIncidences.IDType ON 
                      dbo.DailyCauses.IDEmployee = dbo.DailyIncidences.IDEmployee AND dbo.DailyCauses.Date = dbo.DailyIncidences.Date AND 
                      dbo.DailyCauses.IDRelatedIncidence = dbo.DailyIncidences.ID LEFT OUTER JOIN
                      dbo.Shifts ON dbo.DailySchedule.IDShiftUsed = dbo.Shifts.ID LEFT OUTER JOIN
                      dbo.Shifts AS Shifts_1 ON dbo.DailySchedule.IDShift1 = Shifts_1.ID
GROUP BY dbo.DailySchedule.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name, 
                      YEAR(dbo.DailySchedule.Date), CONVERT(varchar(10), dbo.DailySchedule.IDEmployee, 120) + '-' + CAST(dbo.EmployeeGroups.IDGroup AS varchar) 
                      + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120), CASE WHEN DailySchedule.Date <= GETDATE() 
                      THEN Shifts.Name ELSE Shifts_1.Name END, dbo.Employees.Name, dbo.Groups.Name, dbo.GetFullGroupPathName(dbo.Groups.ID), DATEPART(wk, 
                      dbo.DailySchedule.Date), DATEPART(dy, dbo.DailySchedule.Date), (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1, 
                      dbo.EmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.Groups.Path, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, 
                      dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
GO

CREATE VIEW [dbo].[sysroAccessCube]
AS
SELECT        dbo.Punches.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee, 
                         dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, CONVERT(nvarchar(8), 
                         dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day, MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Year, 
                         (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, 
                         dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName, dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL 
                         THEN 0 ELSE 1 END AS IsInvalid, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails) AS Details, dbo.sysroEmployeeGroups.Path, 
                         dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, 
                         dbo.sysroEmployeeGroups.IDGroup
FROM            dbo.sysroEmployeeGroups INNER JOIN
                         dbo.Punches ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee AND dbo.Punches.ShiftDate BETWEEN 
                         dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate INNER JOIN
                         dbo.EmployeeContracts ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND 
                         dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                         dbo.Zones ON dbo.Punches.IDZone = dbo.Zones.ID LEFT OUTER JOIN
                         dbo.Terminals ON dbo.Punches.IDTerminal = dbo.Terminals.ID LEFT OUTER JOIN
                         dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID, 
                         dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108), 
                         dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name, 
                         (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name, 
                         CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path, 
                         dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, 
                         dbo.sysroEmployeeGroups.IDGroup
HAVING        (dbo.Punches.Type = 5) OR
                         (dbo.Punches.Type = 6) OR
                         (dbo.Punches.Type = 7)

GO

CREATE VIEW dbo.sysroAccessPlates
AS
SELECT     dbo.Punches.ID, dbo.Punches.IDEmployee, dbo.Punches.DateTime AS DatePunche, CONVERT(VARCHAR(8), dbo.Punches.DateTime, 108) AS TimePunche, 
                      CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails) AS TypeDetails, dbo.Employees.Name AS NameEmployee, dbo.Zones.Name AS NameZone, 
                      dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.IDGroup, 
                      dbo.sysroEmployeeGroups.EndDate
FROM         dbo.Punches INNER JOIN
                      dbo.TerminalReaders ON dbo.Punches.IDTerminal = dbo.TerminalReaders.IDTerminal AND dbo.Punches.IDReader = dbo.TerminalReaders.ID INNER JOIN
                      dbo.sysroEmployeeGroups ON dbo.Punches.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND 
                      dbo.Punches.DateTime >= dbo.sysroEmployeeGroups.BeginDate AND dbo.Punches.DateTime <= dbo.sysroEmployeeGroups.EndDate LEFT OUTER JOIN
                      dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID LEFT OUTER JOIN
                      dbo.Zones ON dbo.Punches.IDZone = dbo.Zones.ID
WHERE     (dbo.TerminalReaders.Type = N'MAT') AND (NOT (dbo.Punches.TypeDetails IS NULL)) AND (dbo.Punches.Type = 5 OR dbo.Punches.Type = 6 OR dbo.Punches.Type = 7)
GO

CREATE VIEW dbo.sysrovwCurrentEmployeesPresenceStatusPunches 
As
SELECT     p.IDEmployee, em.Name AS EmployeeName, p.DateTime, p.IDTerminal AS IDReader, CASE WHEN ActualType IN (1, 2) 
                      THEN 'Att' ELSE 'Acc' END AS MoveType, ISNULL(p.IDZone, 0) AS IDZone, 
                      CASE WHEN ActualType = 1 THEN 'IN' WHEN ActualType = 2 THEN (CASE WHEN CHARINDEX('#EmergencyIn#', dbo.Causes.Description) 
                      > 0 THEN 'IN' ELSE 'OUT' END) ELSE CASE zo.IsWorkingZone WHEN 1 THEN 'IN' ELSE (CASE WHEN CHARINDEX('#EmergencyIn#', dbo.Causes.Description) 
                      > 0 THEN 'IN' ELSE 'OUT' END) END END AS Status
FROM         dbo.Punches AS p INNER JOIN
                          (SELECT     dbo.Punches.IDEmployee, MAX(dbo.Punches.ID) AS idp
                            FROM          dbo.Punches INNER JOIN
                                                       (SELECT     IDEmployee, MAX(DateTime) AS dat
                                                         FROM          dbo.Punches AS Punches_1
                                                         WHERE      (ActualType IN (1, 2)) OR
                                                                                (Type = 5)
                                                         GROUP BY IDEmployee) AS maxidat ON dbo.Punches.IDEmployee = maxidat.IDEmployee AND dbo.Punches.DateTime = maxidat.dat
                            WHERE      (dbo.Punches.ActualType IN (1, 2)) OR
                                                   (dbo.Punches.Type = 5)
                            GROUP BY dbo.Punches.IDEmployee) AS maxPunch ON p.IDEmployee = maxPunch.IDEmployee AND p.ID = maxPunch.idp LEFT OUTER JOIN
                      dbo.Causes ON p.TypeData = dbo.Causes.ID LEFT OUTER JOIN
                      dbo.Zones AS zo ON p.IDZone = zo.ID LEFT OUTER JOIN
                      dbo.Employees AS em ON p.IDEmployee = em.ID
GO

CREATE VIEW [dbo].[sysrovwVisitsPresenceStatusPunches]
AS
SELECT        vp.EmpVisitedId AS IDEmployee, MAX(vm.BeginTime) AS DateTime,
                             (SELECT TOP 1 sc.IDReader
                               FROM            dbo.sysrovwCurrentEmployeesPresenceStatusPunches sc
                               WHERE        vp.EmpVisitedId = sc.IDEmployee
                               ORDER BY sc.DateTime DESC) AS IDReader, 
							  (SELECT TOP 1 sc2.IDZone
                               FROM            dbo.sysrovwCurrentEmployeesPresenceStatusPunches sc2
                               WHERE        vp.EmpVisitedId = sc2.IDEmployee
                               ORDER BY sc2.DateTime DESC) AS IDZone, 'IN' AS Status, vs.ID AS VisitorID, vs.Name AS Visitor
FROM            dbo.VisitPlan AS vp INNER JOIN
                         dbo.Visitors AS vs ON vs.ID = vp.VisitorId INNER JOIN
                         dbo.VisitMoves AS vm ON vm.VisitPlanId = vp.ID
WHERE        (vm.BeginTime IS NOT NULL)
GROUP BY vp.EmpVisitedId, vs.Name, vp.PlannedById, vs.ID
union
SELECT        vp.EmpVisitedId AS IDEmployee, MAX(vm.BeginTime) AS DateTime,
                             (SELECT TOP 1 sc.IDReader
                               FROM            dbo.sysrovwCurrentEmployeesPresenceStatusPunches sc
                               WHERE        vp.EmpVisitedId = sc.IDEmployee
                               ORDER BY sc.DateTime DESC) AS IDReader, 
							  (SELECT TOP 1 sc2.IDZone
                               FROM            dbo.sysrovwCurrentEmployeesPresenceStatusPunches sc2
                               WHERE        vp.EmpVisitedId = sc2.IDEmployee
                               ORDER BY sc2.DateTime DESC) AS IDZone, 'OUT' AS Status, vs.ID AS VisitorID, vs.Name AS Visitor
FROM            dbo.VisitPlan AS vp INNER JOIN
                         dbo.Visitors AS vs ON vs.ID = vp.VisitorId INNER JOIN
                         dbo.VisitMoves AS vm ON vm.VisitPlanId = vp.ID
WHERE        (vm.EndTime IS NOT NULL)
GROUP BY vp.EmpVisitedId, vs.Name, vp.PlannedById, vs.ID
GO

CREATE VIEW [dbo].[sysroDailyAccrualsByContract]
AS
SELECT     TOP (100) PERCENT IDEmployee, Date, IDConcept, Value, CarryOver, StartupValue,
                          (SELECT     TOP (1) IDContract
                            FROM          dbo.EmployeeContracts
                            WHERE      (dbo.DailyAccruals.IDEmployee = IDEmployee) AND (BeginDate <= dbo.DailyAccruals.Date) AND (EndDate >= dbo.DailyAccruals.Date)) AS NumContrato,
                          (SELECT     TOP (1) PERCENT CASE WHEN fixedpay = 0 THEN
                                                       (SELECT     TOP (1) (CAST(CAST(Value AS varchar(10)) AS decimal(10, 6)))
                                                         FROM          EmployeeUserFieldValues
                                                         WHERE      idEmployee = dailyaccruals.idemployee AND FieldName = RIGHT(Concepts.UsedField, LEN(Concepts.usedfield) - 4) AND 
                                                                                EmployeeUserFieldValues.DATE <= dailyaccruals.date
                                                         ORDER BY date DESC) WHEN FixedPay = 1 THEN payvalue END AS Expr1
                            FROM          dbo.Concepts
                            WHERE      (ViewInPays <> 0) AND (dbo.DailyAccruals.IDConcept = ID)) AS CosteHora
FROM         dbo.DailyAccruals

GO

CREATE VIEW [dbo].[sysroDailyScheduleByContract]
AS
SELECT     IDEmployee, Date, IDShift1, IDShift2, IDShift3, IDShift4, IDShiftUsed, Remarks, Status, JobStatus, LockedDay, StartShiftUsed, StartShift1, StartShift2, StartShift3, 
                      StartShift4, IDAssignment, IsCovered, OldIDAssignment, OldIDShift, IDEmployeeCovered, TaskStatus, IDShiftBase, StartShiftBase, IDAssignmentBase, IsHolidays,
                          (SELECT     TOP (1) IDContract
                            FROM          dbo.EmployeeContracts
                            WHERE      (dbo.DailySchedule.IDEmployee = IDEmployee) AND (BeginDate <= dbo.DailySchedule.Date) AND (EndDate >= dbo.DailySchedule.Date)) 
                      AS NumContrato
FROM         dbo.DailySchedule

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = '_dta_index_AuditLive_9_337436276__K3' AND object_id = OBJECT_ID('AuditLive'))
	CREATE NONCLUSTERED INDEX [_dta_index_AuditLive_9_337436276__K3] ON [dbo].[AuditLive] 
	(
		[Date] ASC
	)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = '_dta_index_RequestsApprovals_38_506484883__K1_K3' AND object_id = OBJECT_ID('RequestsApprovals'))
	CREATE NONCLUSTERED INDEX [_dta_index_RequestsApprovals_38_506484883__K1_K3] ON [dbo].[RequestsApprovals] 
	(
		[IDRequest] ASC,
		[DateTime] ASC
	)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

CREATE FUNCTION dbo.GetPassportLevelOfAuthority
(
	@idPassport int
)
RETURNS int
AS
BEGIN
	declare @levelOfAuthority int

	;with cte (id,idparentpassport,levelofauthority)
	as
	(
	select ID,IDParentPassport, LevelOfAuthority from sysroPassports where id = @idPassport 
	 union all 
	select t.id,t.IDParentPassport, t.LevelOfAuthority
	from sysroPassports t join cte c on t.id = c.idparentpassport
	)
	select @levelOfAuthority =  (select top 1 levelofauthority from cte where not levelofauthority is null)

	IF @levelOfAuthority is null set @levelOfAuthority = 1

	RETURN @levelOfAuthority
END
GO

CREATE FUNCTION [dbo].[Access_CheckPunchResult] 
  (
  	@idmployee int,
  	@idterminal tinyint,
  	@idreader tinyint,
  	@punchdate datetime 
  )
  RETURNS nvarchar(4)
  AS
  BEGIN
  	DECLARE @result as nvarchar(4)
  	DECLARE @zone as int
  	DECLARE @acc as int
 	DECLARE @acc2 as int
  	DECLARE @group as int
  	
  	set @acc = (Select count (*) from TerminalReaders where idterminal=@idterminal and id=@idreader and mode like '%ACC%')	
  	set @acc2 = (Select count (*) from Terminals where id=@idterminal and behavior like '%ACC%')	
  	set @zone = (Select isnull(idzone,0) from TerminalReaders where idterminal=@idterminal and id=@idreader)	
  	-- Si no tiene accessos devolvemos un NA 
  	if  (@acc=0 and @acc2=0) or @zone = 0
  		SET @result = 'NA'
  	ELSE
  		BEGIN
  			-- Miramos si pertenece a un grupo asignado a esa zona
  			set @group = (select count(*)
  							from AccessGroupsPermissions agp
  							inner join Employees e
  							on agp.IDAccessGroup=e.IDAccessGroup
  							where agp.IDZone=@zone
  							and e.id=@idmployee)		
  			IF @group=0
  				SET @result = 'AIC'
  			ELSE
  				BEGIN
  					DECLARE @periodo as int
  					DECLARE @periodoHolidays as int
  					-- Miramos si esta dentro del perioro de esa zona
  					SET @periodo = (select count(*)
  										from AccessGroupsPermissions agp
  										inner join AccessPeriodDaily apd
  										on agp.IDAccessPeriod=apd.IDAccessPeriod
  										inner join Employees e
										on agp.IDAccessGroup=e.IDAccessGroup and e.id=@idmployee
										where agp.IDZone=@zone
										and dayofweek=datepart(w,@punchdate)
										and convert(smalldatetime, substring(CONVERT(VARCHAR(20),@punchdate,120),11,9) ,120)
										between convert(smalldatetime, substring(CONVERT(VARCHAR(20),begintime,120),11,9) ,120) 
										and convert(smalldatetime, substring(CONVERT(VARCHAR(20),endtime,120),11,9) ,120))
  					SET @periodoHolidays = (select count(*)
  										from AccessPeriodHolidays aph
  										inner join AccessGroupsPermissions agp
  										on agp.IDAccessPeriod=aph.IDAccessPeriod
  										inner join Employees e
										on agp.IDAccessGroup=e.IDAccessGroup and e.id=@idmployee
 										where agp.IDZone=@zone
  										and [day]=datepart(d,getdate())
  										and [month]=datepart(m,getdate())
  										and convert(smalldatetime, CONVERT(VARCHAR(8),@punchdate,114) ,120)
  										between convert(smalldatetime, CONVERT(VARCHAR(8),begintime,114) ,120) 
  										and convert(smalldatetime, CONVERT(VARCHAR(8),endtime,114) ,120))
  					IF @periodo = 0 and @periodoHolidays=0
  						SET @result = 'AIT'
  					ELSE
  						BEGIN
  							--Miramos si tiene PRL
  							declare @ohp as bit
  							SET @ohp = (Select isnull(ohp,0) from TerminalReaders where idterminal=@idterminal and id=@idreader)	 
  							IF @ohp = 1
  								SET @result = 'PRL'
  							ELSE
  								SET @result = 'AV'
  						END
  				END
  		END
  RETURN @result
  END
GO

CREATE FUNCTION [dbo].[f_Base64ToBinary]
(
    @Base64 VARCHAR(MAX)
)
RETURNS VARBINARY(MAX)
AS
BEGIN
    DECLARE @Bin VARBINARY(MAX)
    SET @Bin = CAST(N'' AS XML).value('xs:base64Binary(sql:variable("@Base64"))', 'VARBINARY(MAX)')
    RETURN @Bin
END 
GO

CREATE FUNCTION [dbo].[f_BinaryToBase64]
(
    @bin VARBINARY(MAX)
)
RETURNS VARCHAR(MAX)
AS
BEGIN
    DECLARE @Base64 VARCHAR(MAX)
    SET @Base64 = CAST(N'' AS XML).value('xs:base64Binary(xs:hexBinary(sql:variable("@bin")))', 'VARCHAR(MAX)')
    RETURN @Base64
END
GO

CREATE FUNCTION [dbo].[GetFeatureNextLevelByEmployee]
 (	
   	@idEmployee int,
   	@employeeFeatureID int, 
   	@featureAlias nvarchar(100)
 )
 RETURNS int
 AS
 BEGIN
   	DECLARE @NextLevel int
   	SELECT @NextLevel = 
   	(SELECT MAX(dbo.GetPassportLevelOfAuthority(sysroPassports.ID))
   	FROM sysroPassports  
   	WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null AND 
   	      dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@employeeFeatureID,2,1,getdate()) > 3 AND 
		  dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID, @featureAlias,'U',2) > 3   	      
   	      )
   	   		   
   	RETURN @NextLevel
 END

GO

CREATE FUNCTION [dbo].[GetFeatureNextLevelPassportsByEmployee]
  (	
    	@idEmployee int,
    	@featureAlias nvarchar(100),
    	@employeeFeatureId int
  )
  RETURNS nvarchar(1000)
  AS
  BEGIN
    DECLARE @RetNames nvarchar(1000)
	SET @RetNames = ''

	DECLARE @tmpTable Table(PassportID int,LevelOfAuthority int ,Name nvarchar(50))

	insert into @tmpTable 
	SELECT sysroPassports.id AS PassportID, dbo.GetPassportLevelOfAuthority(sysroPassports.ID) AS LevelOfAuthority, sysroPassports.Name
    		FROM sysroPassports  
    		WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null AND 
    			  dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@employeeFeatureID,2,1,getdate()) > 3 AND 
 			  dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID, @featureAlias,'U',2) > 3 AND Description not like '@@ROBOTICS@@%'

	SET @RetNames = (SELECT  Name  + ','  AS [text()] from @tmpTable tt where  tt.LevelOfAuthority = (SELECT max(LevelOfAuthority) from @tmpTable) For XML PATH (''))
	
	IF @RetNames <> ''
    	BEGIN
    		SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
    	END
		
	RETURN @RetNames
   END
GO

CREATE FUNCTION [dbo].[GetFeatureNextLevelPassportsIDsByEmployee]
  (	
    	@idEmployee int,
    	@featureAlias nvarchar(100),
    	@employeeFeatureId int
  )
  RETURNS nvarchar(1000)
  AS
  BEGIN
	DECLARE @RetNames nvarchar(1000)
	SET @RetNames = ''

	DECLARE @tmpTable Table(PassportID int,LevelOfAuthority int ,Name nvarchar(50))

	insert into @tmpTable 
	SELECT sysroPassports.id AS PassportID, dbo.GetPassportLevelOfAuthority(sysroPassports.ID) AS LevelOfAuthority, sysroPassports.Name
    		FROM sysroPassports  
    		WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null AND 
    			  dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@employeeFeatureID,2,1,getdate()) > 3 AND 
 			  dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID, @featureAlias,'U',2) > 3 AND Description not like '@@ROBOTICS@@%'

	SET @RetNames = (SELECT  CONVERT(NVARCHAR(4000),PassportID ) + ','  AS [text()] from @tmpTable tt where  tt.LevelOfAuthority = (SELECT max(LevelOfAuthority) from @tmpTable) For XML PATH (''))

	IF @RetNames <> ''
    	BEGIN
    		SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
    	END
    
	RETURN @RetNames
  END
GO

CREATE FUNCTION [dbo].[GetRequestLevelsBelow]
    	(
    	@idPassport int,
    	@idRequest int
    	)
    RETURNS int
    AS
    BEGIN
    	DECLARE @LevelsBelow int,
    			@LevelOfAuthority int,
    			@RequestLevel int
    	SELECT @LevelOfAuthority = dbo.GetPassportLevelOfAuthority(@idPassport)
    	
    	SELECT @RequestLevel = ISNULL(Requests.StatusLevel, 11)
       FROM Requests
    	WHERE Requests.ID = @idRequest
    	
    	/* Obtiene el número de niveles que hay entre el nivel del validador (@idPassport) y el nivel actual de la solicitud (@RequestLevel) */
    	SELECT @LevelsBelow = 
 	(
 		SELECT COUNT( DISTINCT trpp.LevelOfAuthority) from 
 		(SELECT dbo.GetPassportLevelOfAuthority(IDParentPassport) AS LevelOfAuthority, IDRequest FROM sysroPermissionsOverRequests WHERE IDRequest = @idRequest)trpp
 		WHERE trpp.LevelOfAuthority > @LevelOfAuthority AND trpp.LevelOfAuthority <= @RequestLevel
 	)
     IF @RequestLevel = 11  SET @LevelsBelow = @LevelsBelow + 1
    	
    	RETURN @LevelsBelow
  END
GO

CREATE FUNCTION [dbo].[GetRequestMinStatusLevel]
 (
 	@idPassport int,
 	@featureAlias nvarchar(100),
 	@EmployeefeatureID int,
 	@idEmployee int
 )
 RETURNS int
 AS
 BEGIN
 	/* While looking only at permissions defined directly on passport,
   	returns the first permission found in the employees groups hierarchy */
   	DECLARE @MinStatusLevel int
   	DECLARE @LevelOfAuthority int
  	
  	SELECT @LevelOfAuthority = dbo.GetPassportLevelOfAuthority(@idPassport)
   	
  	SELECT @MinStatusLevel = 
  	(SELECT TOP 1 Parents.LevelOfAuthority
  	 FROM sysroPassports INNER JOIN sysroPassports Parents
  			ON sysroPassports.IDParentPassport = Parents.ID
  	 WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null AND
  	      sysroPassports.Description not like '%@@ROBOTICS@@%' AND 
  	 	  dbo.GetPassportLevelOfAuthority(Parents.ID) > @LevelOfAuthority AND
  		  (select dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID,@FeatureAlias, 'U', 2)) > 3 AND
  		  (select dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@EmployeefeatureID,2,1,getdate())) > 3 
  	 ORDER BY Parents.LevelOfAuthority ASC)
   	
 	RETURN @MinStatusLevel
 END
GO

CREATE FUNCTION [dbo].[GetRequestPassportPermission]
  (
  	@idPassport int,
   	@idRequest int	
  )
  RETURNS int
  AS
  BEGIN
   	DECLARE @FeatureAlias nvarchar(100),
   			@EmployeefeatureID int,
   			@idEmployee int,
  			@RequestType int
  			
   	SELECT @RequestType = Requests.RequestType,
  		   @idEmployee = Requests.IDEmployee
     FROM Requests
  	WHERE Requests.ID = @idRequest
  	
  	SELECT @featureAlias = Alias, 
  		   @EmployeefeatureID = EmployeeFeatureId 
  	FROM dbo.sysroFeatures 
  	WHERE sysroFeatures.AliasId = @RequestType
   	
   	
   	DECLARE @Permission int
   	SET @Permission = (select dbo.WebLogin_GetPermissionOverFeature(@idPassport,@FeatureAlias, 'U', 2)) 
   	
   	IF @Permission > 0 
   		BEGIN
   			SET @Permission = (select dbo.WebLogin_GetPermissionOverEmployee(@idPassport,@idEmployee,@EmployeefeatureID,2,1,getdate()))
   		END
   		   
   	RETURN @Permission
END
  
GO

CREATE FUNCTION [dbo].[GetSupervisedEmployeesByPassport]
  (
  	@idPassport int,
  	@featureAlias nvarchar(50)
  )
  RETURNS @result table (ID int PRIMARY KEY)
  AS
  	BEGIN
  		DECLARE @EmployeeID int
  		DECLARE @SupervisorLevel int
  		DECLARE @featureEmployeeID int
  		DECLARE @featurePermission int
  		DECLARE @parentPassport int
   		
 		DECLARE @GroupType nvarchar(50)
  	
 		SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @idPassport
   		
   		if @GroupType = 'U'
   		begin
   			SET @parentPassport = @idPassport
   		end
   		else
   		begin
   			SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @idPassport
   		end
  		
  		SELECT @SupervisorLevel = (SELECT dbo.GetPassportLevelOfAuthority(@idPassport))
  		SELECT @featureEmployeeID = (SELECT EmployeeFeatureID from sysroFeatures where Alias = @featureAlias)
  		SELECT @featurePermission = (dbo.WebLogin_GetPermissionOverFeature(@idPassport, @featureAlias,'U',2))
  		
  		IF @featurePermission > 3
  			BEGIN
  				INSERT INTO @result
 					select IDEmployee from (
 						select IDEmployee from sysrovwCurrentEmployeeGroups where IDGroup in (
 							SELECT EmployeeGroupID from (select EmployeeGroupID, MAX(LevelOfAuthority) as MaxLevel, PassportID from [dbo].[sysroPermissionsOverGroups]
  												where Permission > 3 and  EmployeeFeatureID  = @featureEmployeeID  group by EmployeeGroupID,PassportID) allPerm
 							WHERE allPerm.PassportID = @parentPassport and allPerm.MaxLevel = @SupervisorLevel)) allEmployees
 						WHERE IDEmployee not in (select EmployeeID from sysroPermissionsOverEmployeesExceptions where PassportID = @parentPassport and EmployeeFeatureID = @featureEmployeeID and Permission <= 3)
  			END
  		RETURN
  	END
GO

update sysroFeatures set EmployeeFeatureID = 2 where dbo.GetMainFeaturePath(ID) = 'Calendar'
GO
update sysroFeatures set EmployeeFeatureID = 9 where dbo.GetMainFeaturePath(ID) = 'Access'
GO
update sysroFeatures set EmployeeFeatureID = 1 where dbo.GetMainFeaturePath(ID) = 'Employees'
GO
update sysroFeatures set EmployeeFeatureID = 11 where dbo.GetMainFeaturePath(ID) = 'LabAgree'
GO
update sysroFeatures set EmployeeFeatureID = 25 where dbo.GetMainFeaturePath(ID) = 'Tasks'
GO

UPDATE dbo.sysroParameters SET Data='365' WHERE ID='DBVersion'
GO