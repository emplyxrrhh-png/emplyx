-- No borréis esta línea
CREATE OR ALTER VIEW [dbo].[sysrovwSecurity_PermissionOverGroups]
AS
		  SELECT POG.IdPassport,
				 Groups.Id AS IdGroup,
				 CASE Charindex('\', Groups.Path) WHEN 0 THEN 1 ELSE 0 END AS IsCompany,
				 CONVERT(smalldatetime,'1900-01-01',120) AS BeginDate,
				 CONVERT(smalldatetime,'2079-01-01',120) AS EndDate,
				 CASE WHEN Groups.Path = POG.Path THEN 1 ELSE 0 END AS DirectPermission
		  FROM   groups WITH (nolock)
				 INNER JOIN (SELECT idgroup, idpassport, path
							 FROM   sysropassports_groups SPG WITH (nolock)
							 INNER JOIN groups ON groups.id = SPG.idgroup
							 INNER JOIN sysroPassports SP ON SP.Id = SPG.IDPassport AND SP.IsSupervisor = 1 AND SP.State = 1 AND CONVERT(Date,GETDATE()) BETWEEN ISNULL(SP.StartDate, CONVERT(Date, GETDATE())) AND ISNULL(SP.ExpirationDate, CONVERT(Date, GETDATE()))
							) POG ON groups.path = POG.path OR groups.path LIKE POG.path + '\%'
GO 


CREATE OR ALTER FUNCTION [dbo].[sysrofnSecurity_PermissionsOverDocumentTemplates]
   (			
     @iddocumenttemplate INT,
 	 @idemployee INT,
	 @idcompany INT,
	 @idpassport INT
   )

   RETURNS @ValueTable table(IdDocumentTemplate int, IdEmployee INT, IdCompany INT, IdPassport int, IsRoboticsUser INT, PermissionOverArea INT, PermissionOverLOPDLevel INT, SupervisorLevelOfAuthority INT) 
   AS
   BEGIN
			DECLARE @iddocumenttemplatepar INT = @iddocumenttemplate
			DECLARE @idemployeepar INT = @idemployee
			DECLARE @idcompanypar INT = @idcompany
			DECLARE @idpassportpar INT = @idpassport
			

      		INSERT INTO @ValueTable
			SELECT  IdDocumentTemplate, IdEmployee, IdCompany, IdPassport, IsRoboticsUser, PermissionOverArea, PermissionOverLOPDLevel, SupervisorLevelOfAuthority
			FROM 
			(
			SELECT IdDocumentTemplate, IdEmployee, IdCompany, IdPassport, IsRoboticsUser, PermissionOverArea, PermissionOverLOPDLevel, SupervisorLevelOfAuthority
			FROM (
			SELECT 
				DocumentTemplates.Id AS IdDocumentTemplate,
				POE.IdEmployee AS IdEmployee,
				0 AS IdCompany,
				POE.IdPassport,
				POE.IsRoboticsUser,
				POA.Permission as PermissionOverArea,
				POL.Permission as PermissionOverLOPDLevel,
				ISNULL(PassportCategory.LevelOfAuthority,15) AS SupervisorLevelOfAuthority
			FROM DocumentTemplates 
			INNER JOIN sysroDocumentAreas ON sysroDocumentAreas.Id = DocumentTemplates.Area AND DocumentTemplates.Scope IN (0,1,3,4,5) AND  (DocumentTemplates.Id = @iddocumenttemplatepar OR @iddocumenttemplatepar = 0)
			INNER JOIN sysroDocumentLOPDLevels ON sysroDocumentLOPDLevels.Id = DocumentTemplates.LOPDAccessLevel
			INNER JOIN sysrovwSecurity_PermissionOverEmployees POE ON (POE.IdEmployee = @idemployeepar OR @idemployeepar = 0) AND CONVERT(date,getdate()) BETWEEN POE.BeginDate AND POE.EndDate AND (POE.IdPassport = @idpassportpar OR @idpassportpar = 0)
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POA WITH (nolock) ON POA.IdPassport = POE.IdPassport AND POA.FeatureAlias = sysroDocumentAreas.FeatureAlias AND POA.FeatureType = 'U'
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POL WITH (nolock) ON POL.IdPassport = POE.IdPassport AND POL.FeatureAlias = sysroDocumentLOPDLevels.FeatureAlias AND POL.FeatureType = 'U'
			INNER JOIN sysroPassports WITH (nolock) ON POE.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 
			LEFT JOIN sysroPassports_Categories AS PassportCategory ON PassportCategory.IDPassport = POE.IdPassport AND PassportCategory.IDCategory = DocumentTemplates.Area
			) AUX1 WHERE AUX1.PermissionOverArea > 0  AND AUX1.PermissionOverLOPDLevel > 0 AND AUX1.SupervisorLevelOfAuthority < 15
			) AUX 
			UNION ALL
			--DOCUMENTOS DE COMPANY
			SELECT  IdDocumentTemplate, IdEmployee, IdCompany, IdPassport, IsRoboticsUser, PermissionOverArea, PermissionOverLOPDLevel, SupervisorLevelOfAuthority
			FROM 
			(
			SELECT IdDocumentTemplate, IdEmployee, IdCompany, IdPassport, IsRoboticsUser, PermissionOverArea, PermissionOverLOPDLevel, SupervisorLevelOfAuthority 
			FROM (
			SELECT 
				DocumentTemplates.Id AS IdDocumentTemplate,
				0 AS IdEmployee,
				POG.IdGroup AS IdCompany,
				POG.IdPassport,
				POA.IsRoboticsUser,
				POA.Permission as PermissionOverArea,
				POL.Permission as PermissionOverLOPDLevel,
				ISNULL(PassportCategory.LevelOfAuthority,15) AS SupervisorLevelOfAuthority
			FROM DocumentTemplates 
			INNER JOIN sysroDocumentAreas ON sysroDocumentAreas.Id = DocumentTemplates.Area AND DocumentTemplates.Scope IN (2,6) AND (DocumentTemplates.Id = @iddocumenttemplatepar OR @iddocumenttemplatepar = 0) 
			INNER JOIN sysroDocumentLOPDLevels ON sysroDocumentLOPDLevels.Id = DocumentTemplates.LOPDAccessLevel
			INNER JOIN sysrovwSecurity_PermissionOverGroups POG ON (POG.IdGroup = @idcompanypar OR @idcompanypar = 0) AND (POG.IdPassport = @idpassport OR @idpassport = 0) AND POG.IsCompany = 1
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POA WITH (nolock) ON POA.IdPassport = POG.IdPassport AND POA.FeatureAlias = sysroDocumentAreas.FeatureAlias AND POA.FeatureType = 'U'
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POL WITH (nolock) ON POL.IdPassport = POG.IdPassport AND POL.FeatureAlias = sysroDocumentLOPDLevels.FeatureAlias AND POL.FeatureType = 'U'
			INNER JOIN sysroPassports WITH (nolock) ON POG.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 
			LEFT JOIN sysroPassports_Categories AS PassportCategory ON PassportCategory.IDPassport = POG.IdPassport AND PassportCategory.IDCategory = DocumentTemplates.Area
			) AUX1 WHERE AUX1.PermissionOverArea > 0  AND AUX1.PermissionOverLOPDLevel > 0 AND AUX1.SupervisorLevelOfAuthority < 15
			) AUX 
 	RETURN
   END
GO

CREATE OR ALTER FUNCTION [dbo].[sysrofnSecurity_PermissionsOverDocumentTemplatesWithDependencies]
   (			
     @iddocumenttemplate INT,
 	 @idemployee INT,
	 @idcompany INT,
	 @idpassport INT
   )

   RETURNS @ValueTable table(IdDocumentTemplate int, IdEmployee INT, IdCompany INT, IdPassport int, IsRoboticsUser INT, PermissionOverArea INT, PermissionOverLOPDLevel INT, SupervisorLevelOfAuthority INT, LevelOfAuthorityToNotify INT, DirectDependence BIT) 
   AS
   BEGIN
			DECLARE @iddocumenttemplatepar INT = @iddocumenttemplate
			DECLARE @idemployeepar INT = @idemployee
			DECLARE @idcompanypar INT = @idcompany
			DECLARE @idpassportpar INT = @idpassport
			

      		INSERT INTO @ValueTable
			SELECT  IdDocumentTemplate, IdEmployee, IdCompany, IdPassport, IsRoboticsUser, PermissionOverArea, PermissionOverLOPDLevel, SupervisorLevelOfAuthority, LevelOfAuthorityToNotify, CASE WHEN (SupervisorLevelOfAuthority = LevelOfAuthorityToNotify) THEN 1 ELSE 0 END  AS DirectDependence
			FROM 
			(
			SELECT IdDocumentTemplate, IdEmployee, IdCompany, IdPassport, IsRoboticsUser, PermissionOverArea, PermissionOverLOPDLevel, SupervisorLevelOfAuthority,     
				   MAX(CASE WHEN SupervisorLevelOfAuthority < 15 AND PermissionOverArea > 3 AND PermissionOverLOPDLevel> 3 THEN SupervisorLevelOfAuthority ELSE 0 END)  OVER (PARTITION BY IdDocumentTemplate, IdEmployee Order By IdDocumentTemplate ASC, IdEmployee ASC) AS LevelOfAuthorityToNotify 
			FROM (
			SELECT 
				DocumentTemplates.Id AS IdDocumentTemplate,
				POE.IdEmployee AS IdEmployee,
				0 AS IdCompany,
				POE.IdPassport,
				POE.IsRoboticsUser,
				POA.Permission as PermissionOverArea,
				POL.Permission as PermissionOverLOPDLevel,
				ISNULL(PassportCategory.LevelOfAuthority,15) AS SupervisorLevelOfAuthority
			FROM DocumentTemplates 
			INNER JOIN sysroDocumentAreas ON sysroDocumentAreas.Id = DocumentTemplates.Area AND DocumentTemplates.Scope IN (0,1,3,4,5) AND  (DocumentTemplates.Id = @iddocumenttemplatepar OR @iddocumenttemplatepar = 0)
			INNER JOIN sysroDocumentLOPDLevels ON sysroDocumentLOPDLevels.Id = DocumentTemplates.LOPDAccessLevel
			INNER JOIN sysrovwSecurity_PermissionOverEmployees POE ON (POE.IdEmployee = @idemployeepar OR @idemployeepar = 0) AND CONVERT(date,getdate()) BETWEEN POE.BeginDate AND POE.EndDate AND (POE.IdPassport = @idpassportpar OR @idpassportpar = 0)
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POA WITH (nolock) ON POA.IdPassport = POE.IdPassport AND POA.FeatureAlias = sysroDocumentAreas.FeatureAlias AND POA.FeatureType = 'U'
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POL WITH (nolock) ON POL.IdPassport = POE.IdPassport AND POL.FeatureAlias = sysroDocumentLOPDLevels.FeatureAlias AND POL.FeatureType = 'U'
			INNER JOIN sysroPassports WITH (nolock) ON POE.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 
			LEFT JOIN sysroPassports_Categories AS PassportCategory ON PassportCategory.IDPassport = POE.IdPassport AND PassportCategory.IDCategory = DocumentTemplates.Area
			) AUX1 WHERE AUX1.PermissionOverArea > 0  AND AUX1.PermissionOverLOPDLevel > 0 AND AUX1.SupervisorLevelOfAuthority < 15
			) AUX 
			UNION ALL
			--DOCUMENTOS DE COMPANY
			SELECT  IdDocumentTemplate, IdEmployee, IdCompany, IdPassport, IsRoboticsUser, PermissionOverArea, PermissionOverLOPDLevel, SupervisorLevelOfAuthority, LevelOfAuthorityToNotify, CASE WHEN (SupervisorLevelOfAuthority = LevelOfAuthorityToNotify) THEN 1 ELSE 0 END  AS DirectDependence
			FROM 
			(
			SELECT IdDocumentTemplate, IdEmployee, IdCompany, IdPassport, IsRoboticsUser, PermissionOverArea, PermissionOverLOPDLevel, SupervisorLevelOfAuthority,     
				   MAX(CASE WHEN SupervisorLevelOfAuthority < 15 AND PermissionOverArea > 3 AND PermissionOverLOPDLevel> 3 THEN SupervisorLevelOfAuthority ELSE 0 END)  OVER (PARTITION BY IdDocumentTemplate, IdCompany Order By IdDocumentTemplate ASC, IdCompany ASC) AS LevelOfAuthorityToNotify 
			FROM (
			SELECT 
				DocumentTemplates.Id AS IdDocumentTemplate,
				0 AS IdEmployee,
				POG.IdGroup AS IdCompany,
				POG.IdPassport,
				POA.IsRoboticsUser,
				POA.Permission as PermissionOverArea,
				POL.Permission as PermissionOverLOPDLevel,
				ISNULL(PassportCategory.LevelOfAuthority,15) AS SupervisorLevelOfAuthority
			FROM DocumentTemplates 
			INNER JOIN sysroDocumentAreas ON sysroDocumentAreas.Id = DocumentTemplates.Area AND DocumentTemplates.Scope IN (2,6) AND (DocumentTemplates.Id = @iddocumenttemplatepar OR @iddocumenttemplatepar = 0) 
			INNER JOIN sysroDocumentLOPDLevels ON sysroDocumentLOPDLevels.Id = DocumentTemplates.LOPDAccessLevel
			INNER JOIN sysrovwSecurity_PermissionOverGroups POG ON (POG.IdGroup = @idcompanypar OR @idcompanypar = 0) AND (POG.IdPassport = @idpassport OR @idpassport = 0) AND POG.IsCompany = 1
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POA WITH (nolock) ON POA.IdPassport = POG.IdPassport AND POA.FeatureAlias = sysroDocumentAreas.FeatureAlias AND POA.FeatureType = 'U'
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POL WITH (nolock) ON POL.IdPassport = POG.IdPassport AND POL.FeatureAlias = sysroDocumentLOPDLevels.FeatureAlias AND POL.FeatureType = 'U'
			INNER JOIN sysroPassports WITH (nolock) ON POG.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 
			LEFT JOIN sysroPassports_Categories AS PassportCategory ON PassportCategory.IDPassport = POG.IdPassport AND PassportCategory.IDCategory = DocumentTemplates.Area
			) AUX1 WHERE AUX1.PermissionOverArea > 0  AND AUX1.PermissionOverLOPDLevel > 0 AND AUX1.SupervisorLevelOfAuthority < 15
			) AUX 
 	RETURN
   END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTPortal.RefreshConfiguration')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTPortal.RefreshConfiguration','{"causes":{"refresh":1,"interval":1440,"off":""},"zones":{"refresh":1,"interval":1440,"off":""},"status":{"refresh":1,"interval":1,"off":""},"punches":{"refresh":1,"interval":1,"off":""},"accruals":{"refresh":1,"interval":-1,"off":""},"notifications":{"refresh":1,"interval":30,"off":""},"dailyrecord":{"refresh":1,"interval":-1,"off":""},"communiques":{"refresh":1,"interval":60,"off":""},"channels":{"refresh":1,"interval":60,"off":""},"telecommute":{"refresh":1,"interval":-1,"off":""},"tcinfo":{"refresh":1,"interval":-1,"off":""},"surveys":{"refresh":1,"interval":60,"off":""}}')
GO


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

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='840' WHERE ID='DBVersion'

GO
