 ALTER VIEW [dbo].[sysrovwCommuniquesEmployees]      
 AS 
	SELECT IdCommunique, IdEmployee FROM [dbo].[CommuniqueEmployees]        
		LEFT JOIN [dbo].[Communiques] ON Communiques.Id = CommuniqueEmployees.IdCommunique        
    UNION   
	select IdCommunique, ceg.IDEmployee from communiques 
		INNER JOIN [dbo].[sysroPassports] srp ON Communiques.IdCreatedBy = srp.Id 
		INNER JOIN [dbo].[CommuniqueGroups] cg on Communiques.Id = cg.IdCommunique 
		INNER JOIN [dbo].[Groups] ON Groups.ID =  cg.idgroup    
		INNER JOIN [dbo].[sysrovwCurrentEmployeeGroups] ceg ON (ceg.Path = Groups.Path OR ceg.Path LIKE Groups.Path + '\%') 
						and ceg.CurrentEmployee = 1 
						and (select count(PassportId) from sysroPermissionsOverEmployeesExceptions poe where poe.PassportID = srp.IDParentPassport and poe.EmployeeFeatureID = 1 and poe.EmployeeID = ceg.IDEmployee) = 0
						and (select Permission from sysroPermissionsOverGroups pog where pog.PassportID = srp.IDParentPassport and pog.EmployeeFeatureID = 1 and pog.EmployeeGroupID = ceg.IdGroup) >= 3
GO


ALTER VIEW [dbo].[sysrovwSurveysEmployees]      
  AS      
     SELECT IdSurvey, SurveyEmployees.IdEmployee FROM SurveyEmployees  
			INNER JOIN Surveys ON Surveys.Id = SurveyEmployees.IdSurvey  
     UNION      
     SELECT sg.IdSurvey, ceg.IdEmployee FROM Surveys
		 INNER JOIN [dbo].[sysroPassports] srp ON Surveys.IdCreatedBy = srp.Id 
		 INNER JOIN [dbo].[SurveyGroups] sg on Surveys.Id = sg.IdSurvey
		 INNER JOIN [dbo].[Groups] ON sg.idgroup = Groups.id  
		 INNER JOIN [dbo].[sysrovwCurrentEmployeeGroups] ceg ON (ceg.Path = Groups.Path OR ceg.Path LIKE Groups.Path + '\%') 
						and ceg.CurrentEmployee = 1 
						and (select count(PassportId) from sysroPermissionsOverEmployeesExceptions poe where poe.PassportID = srp.IDParentPassport and poe.EmployeeFeatureID = 1 and poe.EmployeeID = ceg.IDEmployee) = 0
						and (select Permission from sysroPermissionsOverGroups pog where pog.PassportID = srp.IDParentPassport and pog.EmployeeFeatureID = 1 and pog.EmployeeGroupID = ceg.IdGroup) >= 3
GO

ALTER VIEW [dbo].[sysrovwSurveyStatistics]  
  AS  
   SELECT s.Id as IDSurvey,   
       s.Title,   
       se.IdEmployee,   
       ceg.EmployeeName as Name,   
	   ceg.GroupName,  
       CASE WHEN ResponseTimeStamp IS NULL THEN 0 ELSE 1 END as SurveyStatus,   
       er.ResponseTimeStamp,   
       s.Mandatory as RequiresResponse,  
       er.SurveyResponse  
   FROM sysrovwSurveysEmployees se
	INNER JOIN Surveys s ON se.IdSurvey = s.Id  
	INNER JOIN sysrovwCurrentEmployeeGroups ceg ON ceg.IDEmployee = se.IdEmployee  
	LEFT JOIN SurveyEmployeeResponses er ON er.IdSurvey = s.Id AND se.IdEmployee = er.IdEmployee
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='545' WHERE ID='DBVersion'
GO