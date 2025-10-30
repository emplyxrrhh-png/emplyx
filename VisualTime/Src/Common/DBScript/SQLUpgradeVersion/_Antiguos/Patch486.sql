---------------------------------------------------------PRODUCTIV-------------------------------------------------------
DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'TATSK', N'1', N'Interactive', N'X', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'TSK', N'1', N'Interactive', N'X', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'TATSK', N'1', N'Fast', N'X', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'TATSK', N'1', N'Fast', N'E', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'TATSK', N'1', N'Fast', N'S', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'TATSKCO', N'1', N'Interactive', N'X', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (@MaxId, N'mx9', 1, N'TATSKCO', N'1', N'Fast', N'X', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

UPDATE dbo.sysroLiveAdvancedParameters SET Value = 10 WHERE ParameterName='VTPortalApiVersion'
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='486' WHERE ID='DBVersion'
GO

