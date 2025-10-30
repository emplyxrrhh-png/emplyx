-- No borréis esta línea

ALTER PROCEDURE [dbo].[ObtainEmployeeIDsFromFilter]    
     @employeeFilter nvarchar(max),    
     @initialDate smalldatetime,    
     @endDate smalldatetime    
    AS     
    BEGIN  
      declare @pemployeeFilter nvarchar(max) = @employeeFilter    
      declare @pinitialDate smalldatetime   = @initialDate   
      declare @pendDate smalldatetime  = @endDate   
     
	  DECLARE @SQLString nvarchar(MAX);    
      SET @SQLString = 'SELECT DISTINCT IDEmployee FROM sysroEmployeeGroups '
	  SET @SQLString = @SQLString + ' WHERE (' + @pemployeeFilter + ') AND '
      SET @SQLString = @SQLString + '((''' + CONVERT(VARCHAR(10), @pinitialDate, 112) + ''' <= dbo.sysroEmployeeGroups.EndDate) AND '
	  SET @SQLString = @SQLString + ' (''' + CONVERT(VARCHAR(10), @pendDate, 112) + ''' >= dbo.sysroEmployeeGroups.BeginDate))'    
      SET @SQLString = @SQLString + ' ORDER BY IDEmployee'    
      
	  exec sp_executesql @SQLString
    end
GO

DROP VIEW IF EXISTS [dbo].[sysrovwSecurity_PermissionOverEmployeeAndFeature]
GO
CREATE VIEW [dbo].[sysrovwSecurity_PermissionOverEmployeeAndFeature]
AS
  SELECT POE.IdPassport,
         POE.IsRoboticsUser,
		 POE.IdEmployee,
		 POE.BeginDate,
		 POE.EndDate,
		 POF.IdFeature, 
		 POF.FeatureAlias, 
		 POF.FeatureType, 
		 POF.Permission FeaturePermission
  FROM sysrovwSecurity_PermissionOverEmployees POE
  INNER JOIN sysrovwSecurity_PermissionOverFeatures POF ON POE.IdPassport = POF.IdPassport AND POF.FeatureType = 'U'
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
WHERE 1 = CASE WHEN Requests.RequestType = 12 THEN (SELECT COUNT(IDCenter) FROM sysroPassports_Centers c WITH (nolock) WHERE c.IDCenter = Requests.IDCenter AND c.IDPassport = sysroPassports.IDParentPassport) ELSE 1 END
  AND 1 = CASE WHEN LEN(sysroGroupFeatures.BusinessGroupList) > 0 AND LEN(CausesBusinessGroups.BusinessGroup) > 0 AND Requests.IDCause > 0 AND CHARINDEX(CausesBusinessGroups.BusinessGroup, sysroGroupFeatures.BusinessGroupList) = 0 THEN 0 ELSE 1 END
  AND 1 = CASE WHEN LEN(sysroGroupFeatures.businessgrouplist) > 0 AND LEN(ShiftsBusinessGroups.BusinessGroup) > 0 AND Requests.IDShift > 0 AND CHARINDEX(ShiftsBusinessGroups.BusinessGroup, sysroGroupFeatures.BusinessGroupList) = 0 THEN 0 ELSE 1 END	
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='758' WHERE ID='DBVersion'
GO
