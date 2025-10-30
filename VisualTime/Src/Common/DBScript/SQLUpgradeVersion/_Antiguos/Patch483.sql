DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 2, N'', NULL, NULL, NULL, NULL, N'0', NULL, NULL, NULL, NULL, NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'TA', N'1', N'Fast', N'X', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'TA', N'1', N'Interactive', N'X', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'ACC', N'1', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'ACCTA', N'1', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'DIN', N'1', N'Interactive', N'X', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'TA', N'1', N'Fast', N'E', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'TA', N'1', N'Fast', N'S', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'CO', N'1', N'Fast', N'X', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'TACO', N'1', N'Fast', N'X', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'TADIN', N'1', N'Fast', N'X', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'TADIN', N'1', N'Interactive', N'X', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'ACCTA', N'1', N'Blind', N'E,S', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'0', N'')
GO

ALTER TABLE [dbo].[ReportPlannedExecutions]
ALTER COLUMN [NextExecutionDate] [datetime] NULL
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='483' WHERE ID='DBVersion'
GO

