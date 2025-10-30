DROP FUNCTION IF EXISTS [dbo].[sysrofnSecurity_RequestsNotificationsDependencies]
GO

DROP FUNCTION IF EXISTS [dbo].[sysrofnSecurity_DocumentsNotificationsDependencies]
 GO

 CREATE FUNCTION [dbo].[sysrofnSecurity_DocumentsNotificationsDependencies]
   (			
     @iddocumenttemplate INT,
 	 @idemployee INT,
	 @idcompany INT
   )

   RETURNS @ValueTable table(IdDocumentTemplate int, IdEmployee INT, IdCompany INT, IdPassport int, PermissionOverArea INT, PermissionOverLOPDLevel INT, SupervisorLevelOfAuthority INT, LevelOfAuthorityToNotify INT, ShouldBeNotified BIT) 
   AS
   BEGIN
			DECLARE @iddocumenttemplatepar INT = @iddocumenttemplate
			DECLARE @idemployeepar INT = @idemployee
			DECLARE @idcompanypar INT = @idcompany
			

      		INSERT INTO @ValueTable
			SELECT  IdDocumentTemplate, IdEmployee, IdCompany, IdPassport, PermissionOverArea, PermissionOverLOPDLevel, SupervisorLevelOfAuthority, LevelOfAuthorityToNotify, CASE WHEN (SupervisorLevelOfAuthority = LevelOfAuthorityToNotify) THEN 1 ELSE 0 END  AS ShouldBeNotified
			FROM 
			(
			SELECT IdDocumentTemplate, IdEmployee, IdCompany, IdPassport, PermissionOverArea, PermissionOverLOPDLevel, SupervisorLevelOfAuthority,     
				   MAX(CASE WHEN SupervisorLevelOfAuthority < 15 AND PermissionOverArea > 3 AND PermissionOverLOPDLevel> 3 THEN SupervisorLevelOfAuthority ELSE 0 END)  OVER (PARTITION BY IdDocumentTemplate, IdEmployee Order By IdDocumentTemplate ASC, IdEmployee ASC) AS LevelOfAuthorityToNotify 
			FROM (
			SELECT 
				DocumentTemplates.Id AS IdDocumentTemplate,
				POE.IdEmployee AS IdEmployee,
				0 AS IdCompany,
				POE.IdPassport,
				POA.Permission as PermissionOverArea,
				POL.Permission as PermissionOverLOPDLevel,
				ISNULL(PassportCategory.LevelOfAuthority,15) AS SupervisorLevelOfAuthority
			FROM DocumentTemplates 
			INNER JOIN sysroDocumentAreas ON sysroDocumentAreas.Id = DocumentTemplates.Area AND DocumentTemplates.Scope IN (0,1,3,4,5) AND  DocumentTemplates.Id = @iddocumenttemplatepar
			INNER JOIN sysroDocumentLOPDLevels ON sysroDocumentLOPDLevels.Id = DocumentTemplates.LOPDAccessLevel
			INNER JOIN sysrovwSecurity_PermissionOverEmployees POE ON POE.IdEmployee = @idemployeepar AND CONVERT(date,getdate()) BETWEEN POE.BeginDate AND POE.EndDate
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POA WITH (nolock) ON POA.IdPassport = POE.IdPassport AND POA.IdFeature = sysroDocumentAreas.IdFeature AND POA.Permission > 3
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POL WITH (nolock) ON POL.IdPassport = POE.IdPassport AND POL.IdFeature = sysroDocumentLOPDLevels.IdFeature AND POL.Permission > 3
			INNER JOIN sysroPassports WITH (nolock) ON POE.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 AND sysroPassports.Description NOT LIKE '%@@ROBOTICS@@%'
			LEFT JOIN sysroPassports_Categories AS PassportCategory ON PassportCategory.IDPassport = POE.IdPassport AND PassportCategory.IDCategory = DocumentTemplates.Area
			) AUX1 WHERE AUX1.PermissionOverArea > 3  AND AUX1.PermissionOverLOPDLevel > 3  
			) AUX 
			UNION ALL
			--DOCUMENTOS DE COMPANY
			SELECT  IdDocumentTemplate, IdEmployee, IdCompany, IdPassport, PermissionOverArea, PermissionOverLOPDLevel, SupervisorLevelOfAuthority, LevelOfAuthorityToNotify, CASE WHEN (SupervisorLevelOfAuthority = LevelOfAuthorityToNotify) THEN 1 ELSE 0 END  AS ShouldBeNotified
			FROM 
			(
			SELECT IdDocumentTemplate, IdEmployee, IdCompany, IdPassport, PermissionOverArea, PermissionOverLOPDLevel, SupervisorLevelOfAuthority,     
				   MAX(CASE WHEN SupervisorLevelOfAuthority < 15 AND PermissionOverArea > 3 AND PermissionOverLOPDLevel> 3 THEN SupervisorLevelOfAuthority ELSE 0 END)  OVER (PARTITION BY IdDocumentTemplate, IdCompany Order By IdDocumentTemplate ASC, IdCompany ASC) AS LevelOfAuthorityToNotify 
			FROM (
			SELECT 
				DocumentTemplates.Id AS IdDocumentTemplate,
				0 AS IdEmployee,
				POG.IdGroup AS IdCompany,
				POG.IdPassport,
				POA.Permission as PermissionOverArea,
				POL.Permission as PermissionOverLOPDLevel,
				ISNULL(PassportCategory.LevelOfAuthority,15) AS SupervisorLevelOfAuthority
			FROM DocumentTemplates 
			INNER JOIN sysroDocumentAreas ON sysroDocumentAreas.Id = DocumentTemplates.Area AND DocumentTemplates.Scope IN (2,6) AND DocumentTemplates.Id = @iddocumenttemplatepar 
			INNER JOIN sysroDocumentLOPDLevels ON sysroDocumentLOPDLevels.Id = DocumentTemplates.LOPDAccessLevel
			INNER JOIN sysrovwSecurity_PermissionOverGroups POG ON POG.IdPassport = @idcompanypar
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POA WITH (nolock) ON POA.IdPassport = POG.IdPassport AND POA.IdFeature = sysroDocumentAreas.IdFeature AND POA.Permission > 3
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POL WITH (nolock) ON POL.IdPassport = POG.IdPassport AND POL.IdFeature = sysroDocumentLOPDLevels.IdFeature AND POL.Permission > 3
			INNER JOIN sysroPassports WITH (nolock) ON POG.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 AND sysroPassports.Description NOT LIKE '%@@ROBOTICS@@%'
			LEFT JOIN sysroPassports_Categories AS PassportCategory ON PassportCategory.IDPassport = POG.IdPassport AND PassportCategory.IDCategory = DocumentTemplates.Area
			) AUX1 WHERE AUX1.PermissionOverArea > 3  AND AUX1.PermissionOverLOPDLevel > 3  
			) AUX 
 	RETURN
   END
GO

DROP FUNCTION IF EXISTS [dbo].[sysrofnSecurity_DocumentsNotificationsDependencies]
 GO

DROP VIEW IF EXISTS [dbo].[sysrovwSecurity_PermissionOverEmployees]
GO
CREATE VIEW [dbo].[sysrovwSecurity_PermissionOverEmployees]
AS
	  SELECT POG.IdPassport,
			 POG.IsRoboticsUser,
			 EG.IdEmployee,
			 EG.BeginDate,
			 EG.EndDate
	  FROM   Groups WITH (nolock)
			 INNER JOIN (SELECT idgroup, idpassport, path, CASE CHARINDEX('@@ROBOTICS@@',SP.Description) WHEN 0 THEN 0 ELSE 1 END AS IsRoboticsUser
						 FROM   sysropassports_groups SPG WITH (nolock)
						 INNER JOIN groups ON groups.id = SPG.idgroup
						 INNER JOIN sysroPassports SP ON SP.Id = SPG.IDPassport AND SP.IsSupervisor = 1
						) POG ON groups.path = POG.path OR groups.path LIKE POG.path + '\%'
			 INNER JOIN dbo.sysrovwgetemployeegroup AS EG WITH (nolock) ON groups.id = EG.idgroup
			 LEFT OUTER JOIN sysroPassports_Employees SPE WITH (nolock) ON SPE.IDPassport = POG.idpassport AND SPE.IDEmployee = EG.idemployee
	  WHERE SPE.Permission IS NULL OR SPE.Permission = 1 
	  UNION
	  SELECT SPE.IdPassport,
			 0 AS IsRoboticsUser,
			 SPE.IdEmployee,
			 CONVERT(smalldatetime,'1900-01-01',120) AS BeginDate,
			 CONVERT(smalldatetime,'2079-01-01',120) AS EndDate
	  FROM   sysroPassports_Employees SPE
	  WHERE  SPE.Permission = 1 
GO

DROP TABLE IF EXISTS [dbo].[sysroDocumentAreas]
GO

CREATE TABLE [dbo].[sysroDocumentAreas](
	[Id] [int] NOT NULL,
	[IdCategory] [int] NOT NULL,
	[IdFeature] [int] NOT NULL,
	[FeatureAlias] [NVARCHAR](50) NOT NULL
 CONSTRAINT [PK_sysroDocumentAreas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

INSERT INTO [dbo].[sysroDocumentAreas] (Id, IdCategory, IdFeature, FeatureAlias) VALUES (0, 0, 32210, 'Documents.Permision.Prevention')
GO
INSERT INTO [dbo].[sysroDocumentAreas] (Id, IdCategory, IdFeature, FeatureAlias) VALUES (1, 1, 32220, 'Documents.Permision.Labor')
GO
INSERT INTO [dbo].[sysroDocumentAreas] (Id, IdCategory, IdFeature, FeatureAlias) VALUES (2, 2, 32230, 'Documents.Permision.Legal')
GO
INSERT INTO [dbo].[sysroDocumentAreas] (Id, IdCategory, IdFeature, FeatureAlias) VALUES (3, 3, 32240, 'Documents.Permision.Security')
GO
INSERT INTO [dbo].[sysroDocumentAreas] (Id, IdCategory, IdFeature, FeatureAlias) VALUES (4, 4, 32250, 'Documents.Permision.Quality')
GO

DROP TABLE IF EXISTS [dbo].[sysroDocumentLOPDLevels]
GO
CREATE TABLE [dbo].[sysroDocumentLOPDLevels](
	[Id] [INT] NOT NULL,
	[IdFeature] [INT] NOT NULL,
	[FeatureAlias] [NVARCHAR](50) NOT NULL
 CONSTRAINT [PK_sysroDocumentLOPDLevels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[IdFeature] ASC
))
GO

INSERT INTO [dbo].[sysroDocumentLOPDLevels] (Id, IdFeature, FeatureAlias) VALUES (0, 32310, 'Documents.AccessLevel.Low')
GO
INSERT INTO [dbo].[sysroDocumentLOPDLevels] (Id, IdFeature, FeatureAlias) VALUES (1, 32320, 'Documents.AccessLevel.Medium')
GO
INSERT INTO [dbo].[sysroDocumentLOPDLevels] (Id, IdFeature, FeatureAlias) VALUES (2, 32330, 'Documents.AccessLevel.High')
GO

DROP FUNCTION IF EXISTS [dbo].[sysrofnSecurity_PermissionsOverDocumentTemplates]
 GO

 CREATE FUNCTION [dbo].[sysrofnSecurity_PermissionsOverDocumentTemplates]
   (			
     @iddocumenttemplate INT,
 	 @idemployee INT,
	 @idcompany INT,
	 @idpassport INT
   )

   RETURNS @ValueTable table(IdDocumentTemplate int, IdEmployee INT, IdCompany INT, IdPassport int, PermissionOverArea INT, PermissionOverLOPDLevel INT, SupervisorLevelOfAuthority INT, LevelOfAuthorityToNotify INT, DirectDependence BIT) 
   AS
   BEGIN
			DECLARE @iddocumenttemplatepar INT = @iddocumenttemplate
			DECLARE @idemployeepar INT = @idemployee
			DECLARE @idcompanypar INT = @idcompany
			DECLARE @idpassportpar INT = @idpassport
			

      		INSERT INTO @ValueTable
			SELECT  IdDocumentTemplate, IdEmployee, IdCompany, IdPassport, PermissionOverArea, PermissionOverLOPDLevel, SupervisorLevelOfAuthority, LevelOfAuthorityToNotify, CASE WHEN (SupervisorLevelOfAuthority = LevelOfAuthorityToNotify) THEN 1 ELSE 0 END  AS DirectDependence
			FROM 
			(
			SELECT IdDocumentTemplate, IdEmployee, IdCompany, IdPassport, PermissionOverArea, PermissionOverLOPDLevel, SupervisorLevelOfAuthority,     
				   MAX(CASE WHEN SupervisorLevelOfAuthority < 15 AND PermissionOverArea > 3 AND PermissionOverLOPDLevel> 3 THEN SupervisorLevelOfAuthority ELSE 0 END)  OVER (PARTITION BY IdDocumentTemplate, IdEmployee Order By IdDocumentTemplate ASC, IdEmployee ASC) AS LevelOfAuthorityToNotify 
			FROM (
			SELECT 
				DocumentTemplates.Id AS IdDocumentTemplate,
				POE.IdEmployee AS IdEmployee,
				0 AS IdCompany,
				POE.IdPassport,
				POA.Permission as PermissionOverArea,
				POL.Permission as PermissionOverLOPDLevel,
				ISNULL(PassportCategory.LevelOfAuthority,15) AS SupervisorLevelOfAuthority
			FROM DocumentTemplates 
			INNER JOIN sysroDocumentAreas ON sysroDocumentAreas.Id = DocumentTemplates.Area AND DocumentTemplates.Scope IN (0,1,3,4,5) AND  (DocumentTemplates.Id = @iddocumenttemplatepar OR @iddocumenttemplatepar = 0)
			INNER JOIN sysroDocumentLOPDLevels ON sysroDocumentLOPDLevels.Id = DocumentTemplates.LOPDAccessLevel
			INNER JOIN sysrovwSecurity_PermissionOverEmployees POE ON (POE.IdEmployee = @idemployeepar OR @idemployeepar = 0) AND CONVERT(date,getdate()) BETWEEN POE.BeginDate AND POE.EndDate AND (POE.IdPassport = @idpassportpar OR @idpassportpar = 0)
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POA WITH (nolock) ON POA.IdPassport = POE.IdPassport AND POA.FeatureAlias = sysroDocumentAreas.FeatureAlias AND POA.FeatureType = 'U'
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POL WITH (nolock) ON POL.IdPassport = POE.IdPassport AND POL.FeatureAlias = sysroDocumentLOPDLevels.FeatureAlias AND POL.FeatureType = 'U'
			INNER JOIN sysroPassports WITH (nolock) ON POE.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 AND sysroPassports.Description NOT LIKE '%@@ROBOTICS@@%'
			LEFT JOIN sysroPassports_Categories AS PassportCategory ON PassportCategory.IDPassport = POE.IdPassport AND PassportCategory.IDCategory = DocumentTemplates.Area
			) AUX1 WHERE AUX1.PermissionOverArea > 0  AND AUX1.PermissionOverLOPDLevel > 0
			) AUX 
			UNION ALL
			--DOCUMENTOS DE COMPANY
			SELECT  IdDocumentTemplate, IdEmployee, IdCompany, IdPassport, PermissionOverArea, PermissionOverLOPDLevel, SupervisorLevelOfAuthority, LevelOfAuthorityToNotify, CASE WHEN (SupervisorLevelOfAuthority = LevelOfAuthorityToNotify) THEN 1 ELSE 0 END  AS DirectDependence
			FROM 
			(
			SELECT IdDocumentTemplate, IdEmployee, IdCompany, IdPassport, PermissionOverArea, PermissionOverLOPDLevel, SupervisorLevelOfAuthority,     
				   MAX(CASE WHEN SupervisorLevelOfAuthority < 15 AND PermissionOverArea > 3 AND PermissionOverLOPDLevel> 3 THEN SupervisorLevelOfAuthority ELSE 0 END)  OVER (PARTITION BY IdDocumentTemplate, IdCompany Order By IdDocumentTemplate ASC, IdCompany ASC) AS LevelOfAuthorityToNotify 
			FROM (
			SELECT 
				DocumentTemplates.Id AS IdDocumentTemplate,
				0 AS IdEmployee,
				POG.IdGroup AS IdCompany,
				POG.IdPassport,
				POA.Permission as PermissionOverArea,
				POL.Permission as PermissionOverLOPDLevel,
				ISNULL(PassportCategory.LevelOfAuthority,15) AS SupervisorLevelOfAuthority
			FROM DocumentTemplates 
			INNER JOIN sysroDocumentAreas ON sysroDocumentAreas.Id = DocumentTemplates.Area AND DocumentTemplates.Scope IN (2,6) AND (DocumentTemplates.Id = @iddocumenttemplatepar OR @iddocumenttemplatepar = 0) 
			INNER JOIN sysroDocumentLOPDLevels ON sysroDocumentLOPDLevels.Id = DocumentTemplates.LOPDAccessLevel
			INNER JOIN sysrovwSecurity_PermissionOverGroups POG ON (POG.IdGroup = @idcompanypar OR @idcompanypar = 0) AND (POG.IdPassport = @idpassport OR @idpassport = 0)
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POA WITH (nolock) ON POA.IdPassport = POG.IdPassport AND POA.FeatureAlias = sysroDocumentAreas.FeatureAlias AND POA.FeatureType = 'U'
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POL WITH (nolock) ON POL.IdPassport = POG.IdPassport AND POL.FeatureAlias = sysroDocumentLOPDLevels.FeatureAlias AND POL.FeatureType = 'U'
			INNER JOIN sysroPassports WITH (nolock) ON POG.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 AND sysroPassports.Description NOT LIKE '%@@ROBOTICS@@%'
			LEFT JOIN sysroPassports_Categories AS PassportCategory ON PassportCategory.IDPassport = POG.IdPassport AND PassportCategory.IDCategory = DocumentTemplates.Area
			) AUX1 WHERE AUX1.PermissionOverArea > 0  AND AUX1.PermissionOverLOPDLevel > 0
			) AUX 
 	RETURN
   END
GO

DROP FUNCTION IF EXISTS [dbo].[sysrofnSecurity_PermissionOverRequestTypes]
GO

CREATE FUNCTION [dbo].[sysrofnSecurity_PermissionOverRequestTypes]
   (			
     @idrequesttype INT,
 	 @idemployee INT
   )

   RETURNS @ValueTable table(IdRequestType int, IdPassport int, IdEmployee INT, Permission INT, SupervisorLevelOfAuthority INT) 
   AS
   BEGIN
			DECLARE @idrequesttypepar INT = @idrequesttype
			DECLARE @idemployeepar INT = @idemployee

      		INSERT INTO @ValueTable
			SELECT  RequestType as IdRequestType, IdPassport, IdEmployee, Permission, SupervisorLevelOfAuthority
			FROM 
			(
				SELECT rt.IdType as RequestType, pove.IdEmployee, pove.IdPassport, Permission, ISNULL(PassportCategory.LevelOfAuthority,15) as SupervisorLevelOfAuthority,      
					MAX(ISNULL(PassportCategory.LevelOfAuthority,15))  OVER (PARTITION BY rt.IdType,pove.Idemployee Order By rt.IdType ASC) AS NextLevelOfAuthorityRequired   

				FROM sysroRequestType rt
					INNER JOIN sysroFeatures WITH (NOLOCK) on sysroFeatures.Id = rt.SupervisorFeatureId
					INNER JOIN sysrovwSecurity_PermissionOverEmployees pove WITH (nolock) ON CONVERT(date,getdate()) BETWEEN pove.BeginDate AND pove.EndDate
					INNER JOIN sysroPassports WITH (nolock) ON pove.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 AND sysroPassports.Description NOT LIKE '%@@ROBOTICS@@%'
					INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (nolock) ON pof.IdPassport = pove.IdPassport AND pof.FeatureAlias = sysroFeatures.Alias
					INNER JOIN sysroGroupFeatures ON sysroGroupFeatures.Id = sysroPassports.IDGroupFeature
					LEFT JOIN sysroPassports_Categories AS PassportCategory ON PassportCategory.IDPassport = sysroPassports.Id AND PassportCategory.IDCategory = rt.IDCategory
				WHERE  Permission >= 3 and ISNULL(PassportCategory.LevelOfAuthority,15) < 15 and pove.IdEmployee = @idemployeepar and rt.IdType = @idrequesttypepar
			) tmp where tmp.SupervisorLevelOfAuthority = tmp.NextLevelOfAuthorityRequired
 	RETURN
   END
GO

DROP VIEW IF EXISTS [dbo].[sysrovwSecurity_PermissionOverRequests]
GO
CREATE VIEW [dbo].[sysrovwSecurity_PermissionOverRequests]
AS
SELECT     Requests.Id AS IdRequest ,
		   Requests.IdEmployee,
           pove.IdPassport,
           pof.Permission as Permission,
		   CASE WHEN CausesBusinessGroups.Idcause = 0 THEN ISNULL(PassportCategory.LevelOfAuthority,15) ELSE ISNULL(PassportCategoryByCause.LevelOfAuthority,15) END AS SupervisorLevelOfAuthority,
		   CASE WHEN CausesBusinessGroups.Idcause = 0 THEN ISNULL(PassportCategory.ShowFromLevel,15) ELSE ISNULL(PassportCategoryByCause.ShowFromLevel,15) END AS ShowFromLevelOfAuthority,
		   CASE WHEN CausesBusinessGroups.Idcause = 0 THEN 1 ELSE ISNULL(CausesBusinessGroups.ApprovedAtLevel,1) END AS ApprovedAtLevel,
		   Requests.AutomaticValidation,
		   Requests.Status as RequestCurrentStatus,
		   Requests.StatusLevel as RequestCurrentApprovalLevel
FROM       Requests WITH (nolock)
INNER JOIN sysroRequestType ON sysroRequestType.IdType = Requests.RequestType
INNER JOIN sysroFeatures WITH (NOLOCK) on sysroFeatures.Id = sysroRequestType.SupervisorFeatureId
-- INNER JOIN sysroFeatures WITH (NOLOCK) on sysroFeatures.AliasId = Requests.RequestType -- Tiene ligeramente mejor rendimiento, pero AliasId debería desaparecer ...
INNER JOIN sysrovwSecurity_PermissionOverEmployees pove WITH (nolock) ON pove.IdEmployee = Requests.IDEmployee AND CONVERT(date,getdate()) BETWEEN pove.BeginDate AND pove.EndDate
INNER JOIN sysroPassports WITH (nolock) ON pove.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 AND sysroPassports.Description NOT LIKE '%@@ROBOTICS@@%'
INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (nolock) ON pof.IdPassport = pove.IdPassport AND pof.FeatureAlias = sysroFeatures.Alias
INNER JOIN sysroGroupFeatures ON sysroGroupFeatures.Id = sysroPassports.IDGroupFeature
INNER JOIN (SELECT Id AS IdCause, ISNULL(BusinessGroup,'') AS BusinessGroup, IDCategory, MinLevelOfAuthority AS LevelOfAuthority, ISNULL(ApprovedAtLevel,1) AS ApprovedAtLevel FROM Causes WITH (nolock)) AS CausesBusinessGroups ON ISNULL(Requests.IDCause, 0) = CausesBusinessGroups.IdCause
INNER JOIN (SELECT Shifts.Id AS IdShift, ISNULL(ShiftGroups.BusinessGroup, '') AS BusinessGroup FROM Shifts WITH (nolock) LEFT OUTER JOIN ShiftGroups WITH (nolock) ON Shifts.IDGroup = ShiftGroups.ID) AS ShiftsBusinessGroups ON ISNULL(Requests.IDShift,0) = ShiftsBusinessGroups.IdShift
LEFT JOIN sysroPassports_Categories AS PassportCategory ON PassportCategory.IDPassport = sysroPassports.Id AND PassportCategory.IDCategory = sysroRequestType.IDCategory
LEFT JOIN sysroPassports_Categories AS PassportCategoryByCause ON PassportCategoryByCause.IDPassport = sysroPassports.Id AND PassportCategoryByCause.IDCategory = CausesBusinessGroups.IDCategory
WHERE 1 = CASE WHEN Requests.RequestType = 12 THEN (SELECT COUNT(IDCostCenter) FROM sysrovwSecurity_PermissionOverCostCenters c WITH (nolock) WHERE c.IDCostCenter = Requests.IDCenter AND c.IDPassport = sysroPassports.Id) ELSE 1 END
  AND 1 = CASE WHEN LEN(sysroGroupFeatures.BusinessGroupList) > 0 AND LEN(CausesBusinessGroups.BusinessGroup) > 0 AND Requests.IDCause > 0 AND CHARINDEX(CausesBusinessGroups.BusinessGroup, sysroGroupFeatures.BusinessGroupList) = 0 THEN 0 ELSE 1 END
  AND 1 = CASE WHEN LEN(sysroGroupFeatures.businessgrouplist) > 0 AND LEN(ShiftsBusinessGroups.BusinessGroup) > 0 AND Requests.IDShift > 0 AND CHARINDEX(ShiftsBusinessGroups.BusinessGroup, sysroGroupFeatures.BusinessGroupList) = 0 THEN 0 ELSE 1 END	
GO

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

UPDATE sysroParameters SET Data='773' WHERE ID='DBVersion'
GO
