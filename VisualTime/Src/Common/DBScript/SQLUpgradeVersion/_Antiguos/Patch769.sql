ALTER VIEW [dbo].[sysrovwCommuniquesEmployees]  
AS  
SELECT IdCommunique, 
       IdEmployee, IdCreatedBy 
FROM [dbo].[CommuniqueEmployees]  
LEFT JOIN [dbo].[Communiques] ON Communiques.Id = CommuniqueEmployees.IdCommunique  
UNION  
SELECT IdCommunique, 
       CEG.IDEmployee, IdCreatedBy 
FROM Communiques  
INNER JOIN [dbo].[sysroPassports] SRP ON Communiques.IdCreatedBy = SRP.Id  
INNER JOIN [dbo].[CommuniqueGroups] CG ON Communiques.Id = CG.IdCommunique  
INNER JOIN [dbo].[Groups] ON Groups.ID =  CG.IdGroup  
INNER JOIN [dbo].[sysrovwCurrentEmployeeGroups] CEG ON (CEG.Path = Groups.Path OR CEG.Path LIKE Groups.Path + '\%') AND CEG.CurrentEmployee = 1  
AND (SELECT COUNT(IDPassport) FROM sysroPassports_Employees POE where POE.IDPassport = SRP.ID and POE.IDEmployee = CEG.IDEmployee) = 0  
AND (SELECT COUNT(IDPassport) FROM sysrovwSecurity_PermissionOverGroups POG WHERE POG.IdPassport = SRP.ID AND POG.IdGroup = CEG.IdGroup) > 0  
GO

ALTER VIEW [dbo].[sysrovwSurveysEmployees]  
AS  
SELECT IdSurvey, 
       SurveyEmployees.IdEmployee 
FROM SurveyEmployees  
INNER JOIN Surveys ON Surveys.Id = SurveyEmployees.IdSurvey  
UNION  
SELECT 
	   SG.IdSurvey, 
	   CEG.IdEmployee 
FROM Surveys  
INNER JOIN [dbo].[sysroPassports] SRP ON Surveys.IdCreatedBy = SRP.Id  
INNER JOIN [dbo].[SurveyGroups] SG on Surveys.Id = SG.IdSurvey  
INNER JOIN [dbo].[Groups] ON SG.idgroup = Groups.id  
INNER JOIN [dbo].[sysrovwCurrentEmployeeGroups] CEG ON (CEG.Path = Groups.Path OR CEG.Path LIKE Groups.Path + '\%') AND CEG.CurrentEmployee = 1  
AND (SELECT COUNT(IDPassport) FROM sysroPassports_Employees POE where POE.IDPassport = SRP.ID and POE.IDEmployee = CEG.IDEmployee) = 0  
AND (SELECT COUNT(IDPassport) FROM sysrovwSecurity_PermissionOverGroups POG WHERE POG.IdPassport = SRP.ID AND POG.IdGroup = CEG.IdGroup) > 0 
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='769' WHERE ID='DBVersion'
GO
