ALTER VIEW [dbo].[sysrovwGetPermissionOverFeature] AS
 select pp.idPassport, pof.FeatureID, rf.Alias, Permission
 from  dbo.sysrovwParentPassports AS pp WITH (NOLOCK)
 inner join sysroPermissionsOverFeatures pof WITH (NOLOCK)
 on pof.PassportID= pp.idParentPassport 
 left outer join sysroFeatures rf WITH (NOLOCK)
 on rf.id=pof.FeatureID
 where rf.Type = 'U'
GO

ALTER VIEW [dbo].[sysrovwParentPassports]
AS
SELECT        ID as idPassport
			, CASE WHEN isnull(GroupType, '') = 'U' THEN id ELSE IDParentPassport END AS idParentPassport
FROM            dbo.sysroPassports WITH (NOLOCK)
WHERE        (ISNULL(GroupType, '') = 'U') OR
                         (IDParentPassport IS not NULL and isnull(IsSupervisor,0)=1)
GO

ALTER VIEW [dbo].[sysrovwGetPermissionOverEmployee] AS
 SELECT        pp.idPassport AS PassportID, pp.idParentPassport AS ParentPassportID, pog.EmployeeFeatureID, pog.EmployeeGroupID, eg.IDEmployee AS EmployeeID, pog.Permission, pog.LevelOfAuthority, ISNULL(poe.Permission, 9) 
                          AS EmployeesExceptionsPermission, CASE WHEN pog.Permission > isnull(poe.Permission, 9) THEN isnull(poe.Permission, 9) ELSE pog.Permission END AS CalculatedPermission, eg.BeginDate, eg.EndDate
 FROM            dbo.sysrovwParentPassports AS pp WITH (NOLOCK)
 INNER JOIN dbo.sysroPermissionsOverGroups AS pog  WITH (NOLOCK)
 ON pp.idParentPassport = pog.PassportID 
 INNER JOIN dbo.sysrovwGetEmployeeGroup AS eg WITH (NOLOCK)
 ON pog.EmployeeGroupID = eg.IDGroup
 LEFT OUTER JOIN dbo.sysroPermissionsOverEmployeesExceptions AS poe WITH (NOLOCK)
 ON eg.IDEmployee = poe.EmployeeID AND poe.EmployeeFeatureID = pog.EmployeeFeatureID AND poe.PassportID = pog.PassportID
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='574' WHERE ID='DBVersion'
GO
