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

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='767' WHERE ID='DBVersion'
GO
