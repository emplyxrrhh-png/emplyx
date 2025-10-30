-- No borréis esta línea
DROP FUNCTION IF EXISTS [dbo].[sysrofnSecurity_PermissionsOverDocumentTemplates]
 GO

 CREATE FUNCTION [dbo].[sysrofnSecurity_PermissionsOverDocumentTemplates]
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
			INNER JOIN sysrovwSecurity_PermissionOverGroups POG ON (POG.IdGroup = @idcompanypar OR @idcompanypar = 0) AND (POG.IdPassport = @idpassport OR @idpassport = 0)
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POA WITH (nolock) ON POA.IdPassport = POG.IdPassport AND POA.FeatureAlias = sysroDocumentAreas.FeatureAlias AND POA.FeatureType = 'U'
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POL WITH (nolock) ON POL.IdPassport = POG.IdPassport AND POL.FeatureAlias = sysroDocumentLOPDLevels.FeatureAlias AND POL.FeatureType = 'U'
			INNER JOIN sysroPassports WITH (nolock) ON POG.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 
			LEFT JOIN sysroPassports_Categories AS PassportCategory ON PassportCategory.IDPassport = POG.IdPassport AND PassportCategory.IDCategory = DocumentTemplates.Area
			) AUX1 WHERE AUX1.PermissionOverArea > 0  AND AUX1.PermissionOverLOPDLevel > 0 AND AUX1.SupervisorLevelOfAuthority < 15
			) AUX 
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
		   pove.IsRoboticsUser,
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
INNER JOIN sysroPassports WITH (nolock) ON pove.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 
INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (nolock) ON pof.IdPassport = pove.IdPassport AND pof.FeatureAlias = sysroFeatures.Alias
INNER JOIN sysroGroupFeatures ON sysroGroupFeatures.Id = sysroPassports.IDGroupFeature
INNER JOIN (SELECT Id AS IdCause, ISNULL(BusinessGroup,'') AS BusinessGroup, IDCategory, MinLevelOfAuthority AS LevelOfAuthority, ISNULL(ApprovedAtLevel,1) AS ApprovedAtLevel FROM Causes WITH (nolock)) AS CausesBusinessGroups ON ISNULL(Requests.IDCause, 0) = CausesBusinessGroups.IdCause
INNER JOIN (SELECT Shifts.Id AS IdShift, ISNULL(ShiftGroups.BusinessGroup, '') AS BusinessGroup FROM Shifts WITH (nolock) LEFT OUTER JOIN ShiftGroups WITH (nolock) ON Shifts.IDGroup = ShiftGroups.ID) AS ShiftsBusinessGroups ON ISNULL(Requests.IDShift,0) = ShiftsBusinessGroups.IdShift
LEFT JOIN sysroPassports_Categories AS PassportCategory ON PassportCategory.IDPassport = sysroPassports.Id AND PassportCategory.IDCategory = sysroRequestType.IDCategory
LEFT JOIN sysroPassports_Categories AS PassportCategoryByCause ON PassportCategoryByCause.IDPassport = sysroPassports.Id AND PassportCategoryByCause.IDCategory = CausesBusinessGroups.IDCategory
WHERE 1 = CASE WHEN Requests.RequestType = 12 THEN (SELECT COUNT(IDCostCenter) FROM sysrovwSecurity_PermissionOverCostCenters c WITH (nolock) WHERE c.IDCostCenter = Requests.IDCenter AND c.IDPassport = sysroPassports.Id) ELSE 1 END
  AND 1 = CASE WHEN LEN(sysroGroupFeatures.BusinessGroupList) > 0 AND LEN(CausesBusinessGroups.BusinessGroup) > 0 AND Requests.IDCause > 0 AND CHARINDEX(CausesBusinessGroups.BusinessGroup, sysroGroupFeatures.BusinessGroupList) = 0 THEN 0 ELSE 1 END
  AND 1 = CASE WHEN LEN(sysroGroupFeatures.businessgrouplist) > 0 AND LEN(ShiftsBusinessGroups.BusinessGroup) > 0 AND Requests.IDShift > 0 AND CHARINDEX(ShiftsBusinessGroups.BusinessGroup, sysroGroupFeatures.BusinessGroupList) = 0 THEN 0 ELSE 1 END	
  AND CASE WHEN CausesBusinessGroups.Idcause = 0 THEN ISNULL(PassportCategory.LevelOfAuthority,15) ELSE ISNULL(PassportCategoryByCause.LevelOfAuthority,15) END < 15
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='782' WHERE ID='DBVersion'
GO
