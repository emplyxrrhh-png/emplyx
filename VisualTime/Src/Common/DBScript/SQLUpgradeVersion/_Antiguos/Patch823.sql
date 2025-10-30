-- No borréis esta línea
DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFe', 1, N'TA', N'1', N'Blind', N'E,S,X', N'Local', N'1,0', N'0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFe', 1, N'CO', N'1', N'Blind', N'E,S,X', N'Local', N'1,0', N'0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFe', 1, N'ACC', N'1', N'Blind', N'E,S,X', N'Local', N'0', N'0', N'0', N'1', N'1,0', NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFe', 1, N'ACCTA', N'1', N'Blind', N'E,S,X', N'Local', N'0', N'0', N'0', N'1', N'1,0', NULL, 1, N'0', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFe', 1, N'TA', N'1', N'Fast', N'X', N'Local', N'1,0', N'0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='823' WHERE ID='DBVersion'
GO
