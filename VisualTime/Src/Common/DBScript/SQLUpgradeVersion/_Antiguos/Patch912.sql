DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'Suprema', 1, N'TA', N'1', N'Blind', N'E', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', 'Remote', 1, NULL, NULL)
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'Suprema', 1, N'TA', N'1', N'Blind', N'S', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', 'Remote', 1, NULL, NULL)
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'Suprema', 1, N'TA', N'1', N'Fast', N'X', N'Local', N'1,0', N'1,0', N'0', N'1,0', N'0', 'Remote', 1, NULL, NULL)
GO

DELETE sysroReaderTemplates WHERE Type = 'SAIWALL'
GO
UPDATE sysroReaderTemplates SET Direction = 'remote' WHERE Type = 'masterASP'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='912' WHERE ID='DBVersion'
GO
