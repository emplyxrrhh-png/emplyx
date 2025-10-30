-- No borréis esta línea
--Remember to add the file to the Updates folder 
DELETE [dbo].[sysroReaderTemplates] WHERE [Type] = 'SharedPortal'
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  MAX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'Time Gate', 1, N'TA', N'1', N'Fast', N'X', N'Server', N'1,0', N'1,0', N'0', N'1,0', N'0', NULL, 1, NULL, NULL)


SET @MaxId = (SELECT  MAX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'Time Gate', 1, N'EIP', N'1', N'Fast', N'E,S', N'Server', N'1,0', N'0', N'0', N'0', N'0', 'SharedPortal', 1, NULL, NULL)
GO

UPDATE [dbo].[Terminals] SET Type = N'Time Gate' WHERE Type = N'SharedPortal'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='937' WHERE ID='DBVersion'
GO
