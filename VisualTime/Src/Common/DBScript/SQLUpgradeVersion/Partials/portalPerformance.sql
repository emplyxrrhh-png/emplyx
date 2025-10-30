--Remember to add the file to the Updates folder 

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTPortal.RefreshConfiguration')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTPortal.RefreshConfiguration','{"causes":{"refresh":1,"interval":1440,"off":""},"zones":{"refresh":1,"interval":1440,"off":""},"status":{"refresh":1,"interval":1,"off":""},"punches":{"refresh":1,"interval":1,"off":""},"accruals":{"refresh":1,"interval":-1,"off":"07:50-08:10@13:50-14:10@16:50-19:10"},"notifications":{"refresh":1,"interval":30,"off":""},"dailyrecord":{"refresh":1,"interval":-1,"off":""},"communiques":{"refresh":1,"interval":60,"off":""},"channels":{"refresh":1,"interval":60,"off":""},"telecommute":{"refresh":1,"interval":-1,"off":""},"tcinfo":{"refresh":1,"interval":-1,"off":""},"surveys":{"refresh":1,"interval":60,"off":""}}')
GO

--IF EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'Customization' And Value = 'CRUZROJA')
--	update dbo.sysroLiveAdvancedParameters Set Value = '{"causes":{"refresh":1,"interval":1440,"off":""},"zones":{"refresh":1,"interval":1440,"off":""},"status":{"refresh":1,"interval":1,"off":""},"punches":{"refresh":1,"interval":1,"off":""},"accruals":{"refresh":1,"interval":-1,"off":"07:50-08:10@13:50-14:10@16:50-17:10"},"notifications":{"refresh":1,"interval":30,"off":"07:50-08:10@13:50-14:10@16:50-17:10"},"dailyrecord":{"refresh":1,"interval":-1,"off":"07:50-08:10@13:50-14:10@16:50-17:10"},"communiques":{"refresh":1,"interval":60,"off":"07:50-08:10@13:50-14:10@16:50-17:10"},"channels":{"refresh":1,"interval":60,"off":"07:50-08:10@13:50-14:10@16:50-17:10"},"telecommute":{"refresh":1,"interval":-1,"off":"07:50-08:10@13:50-14:10@16:50-17:10"},"tcinfo":{"refresh":1,"interval":-1,"off":"07:50-08:10@13:50-14:10@16:50-17:10"},"surveys":{"refresh":1,"interval":60,"off":"07:50-08:10@13:50-14:10@16:50-17:10"}}' where ParameterName = 'VTPortal.RefreshConfiguration'
--GO


CREATE OR ALTER VIEW [dbo].[sysrovwSurveysEmployees]    
AS    
	SELECT IdSurvey, SurveyEmployees.IdEmployee    
	FROM SurveyEmployees  WITH (NOLOCK)  
		INNER JOIN Surveys WITH (NOLOCK) ON Surveys.Id = SurveyEmployees.IdSurvey    
	UNION    
	SELECT distinct SG.IdSurvey, CEG.IdEmployee    
	FROM Surveys WITH (NOLOCK)  
		INNER JOIN [dbo].[SurveyGroups] SG WITH (NOLOCK) on Surveys.Id = SG.IdSurvey    
		INNER JOIN [dbo].[Groups] WITH (NOLOCK) ON SG.idgroup = Groups.id    
		INNER JOIN [dbo].[sysrovwCurrentEmployeeGroups]  CEG WITH (NOLOCK) ON (CEG.Path = Groups.Path OR CEG.Path LIKE Groups.Path + '\%') AND CEG.CurrentEmployee = 1  
		INNER JOIN [dbo].[sysrovwSecurity_PermissionOverEmployees] POE WITH (NOLOCK) on CEG.IDEmployee = POE.IdEmployee and POE.IdPassport = Surveys.IdCreatedBy and convert(date, getdate()) between poe.BeginDate and poe.EndDate 
GO
