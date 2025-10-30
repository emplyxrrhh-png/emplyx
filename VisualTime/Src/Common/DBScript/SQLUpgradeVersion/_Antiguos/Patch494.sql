
ALTER TABLE [dbo].[TerminalsSyncCardsData] ALTER COLUMN CardId [nvarchar](max)
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFP', 1, N'TA', N'1', N'Blind', N'E,S,X', N'Local', N'1,0', N'0', N'0', N'1,0', N'0', NULL, 1, 1, N'')
GO
DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFP', 1, N'ACC', N'1', N'Blind', N'E,S,X', N'Local', N'1,0', N'0', N'0', N'0', N'0', NULL, 1, 0, N'')
GO
DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFP', 1, N'ACCTA', N'1', N'Blind', N'E,S,X', N'Local', N'1,0', N'0', N'0', N'0', N'0', NULL, 1, 0, N'')
GO

UPDATE sysroParameters SET Data='494' WHERE ID='DBVersion'
GO

