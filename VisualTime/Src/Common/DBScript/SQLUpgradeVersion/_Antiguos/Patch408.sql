ALTER TABLE dbo.sysroPassports_Data ADD
	TimeZone nvarchar(200) NULL
GO

IF OBJECT_ID('dbo.TMPVisitsLocation', 'U') IS NOT NULL
	DROP TABLE dbo.TMPVisitsLocation;
GO

CREATE TABLE [dbo].[TMPVisitsLocation](
	[IDVisitor] [nvarchar](40) NULL,
	[VisitorName] [nvarchar](200) NULL,
	[IDEmployee] [int] NULL,
	[EmployeeName] [nvarchar](200) NULL,
	[IDVisit] [nvarchar](40) NULL,
	[Location] [nvarchar](40) NULL,
	[DateTime] [datetime] NULL,
	[IDReportTask] [numeric](16, 0) NULL,
	[VisitFieldName1] [nvarchar](40) NULL,
	[VisitFieldValue1] [nvarchar](40) NULL,
	[VisitFieldName2] [nvarchar](40) NULL,
	[VisitFieldValue2] [nvarchar](40) NULL,
	[VisitorFieldName1] [nvarchar](40) NULL,
	[VisitorFieldValue1] [nvarchar](40) NULL,
	[VisitorFieldName2] [nvarchar](40) NULL,
	[VisitorFieldValue2] [nvarchar](40) NULL,
	[EndDate] [datetime] NULL
) ON [PRIMARY]
GO

UPDATE dbo.sysroLiveAdvancedParameters SET [Value] = '1' WHERE [ParameterName] = 'OnlyFirstNotificationBetweenSupervisors'
GO

INSERT INTO dbo.sysroLiveAdvancedParameters ([ParameterName],[Value]) VALUES ('AllSupervisorsLevelAreNotificated','0')
GO

UPDATE dbo.sysroParameters SET Data='408' WHERE ID='DBVersion'
GO
