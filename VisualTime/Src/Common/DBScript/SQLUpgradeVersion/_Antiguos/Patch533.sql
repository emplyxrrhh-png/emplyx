
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO



DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'mxS', 1, N'', NULL, NULL, NULL, NULL, N'0', NULL, NULL, NULL, NULL, NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'mxS', 2, N'', NULL, NULL, NULL, NULL, N'0', NULL, NULL, NULL, NULL, NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'mxS', 3, N'', NULL, NULL, NULL, NULL, N'0', NULL, NULL, NULL, NULL, NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'mxS', 4, N'', NULL, NULL, NULL, NULL, N'0', NULL, NULL, NULL, NULL, NULL, 1, N'0', N'')
GO


DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'mxS', 1, N'ACC', N'1', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'mxS', 1, N'ACCTA', N'1', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'mxS', 2, N'ACC', N'1', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'mxS', 2, N'ACCTA', N'1', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'mxS', 3, N'ACC', N'1', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'mxS', 3, N'ACCTA', N'1', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'mxS', 4, N'ACC', N'1', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'mxS', 4, N'ACCTA', N'1', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'0', N'')
GO

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo'
				 AND TABLE_NAME = 'TerminalsSyncEmployeeAccessLevelData'))
BEGIN
    CREATE TABLE [dbo].[TerminalsSyncEmployeeAccessLevelData](
	[IDEmployee] [int] NULL,
	[IDTimeZone] [int] NULL,
	[Door1] [bit] NULL,
	[Door2] [bit] NULL,
	[Door3] [bit] NULL,
	[Door4] [bit] NULL,
	[TerminalId] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL
	)
END
GO

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo'
				 AND TABLE_NAME = 'TerminalsSyncTimeZonesInbioData'))
BEGIN
    CREATE TABLE [dbo].[TerminalsSyncTimeZonesInbioData](
	[IDTimeZone] [int] NULL,
	[IdDayOrHol] [int] NULL,
	[BeginTime1] [datetime] NULL,
	[EndTime1]	 [datetime] NULL,
	[BeginTime2] [datetime] NULL,
	[EndTime2]   [datetime] NULL,
	[BeginTime3] [datetime] NULL,
	[EndTime3]   [datetime] NULL,
	[TerminalId] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL
	)
END
GO


INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'rxFP', 1, 199, N'DIN', N'1', N'Blind', N'E,S', N'Local', N'1,0', N'0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

UPDATE sysroParameters SET Data='533' WHERE ID='DBVersion'
GO