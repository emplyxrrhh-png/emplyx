--------------- NUEVAS VISTAS PERMISOS V3----
CREATE VIEW [dbo].[sysrovwGetPermissionOverEmployeev3] AS
SELECT groups.ID as IDGroup, x.IDPassport ,eg.IDEmployee AS EmployeeID,eg.BeginDate, eg.EndDate
FROM Groups WITH (NOLOCK)
INNER JOIN  (SELECT IDGroup, IDPassport, Path from sysroPassports_Groups WITH (NOLOCK) inner join groups on groups.id = sysroPassports_Groups.IDGroup ) x 
on groups.Path = x.Path or groups.Path like x.path + '\%' 
INNER JOIN dbo.sysrovwGetEmployeeGroup AS eg WITH (NOLOCK)  ON groups.ID = eg.IDGroup
INNER JOIN dbo.sysroPassports AS sp WITH (NOLOCK)  ON sp.ID = x.IDPassport
-- WHERE NOT EXISTS (SELECT 1 as Tot FROM sysroPermissionsOverEmployeesExceptions poe WITH (NOLOCK) where poe.PassportID = sp.IDParentPassport and EmployeeFeatureID = 1 and poe.EmployeeID=eg.IDEmployee)
WHERE NOT EXISTS ( SELECT 1 as Tot FROM sysroPassports poe WITH (NOLOCK) where poe.ID = sp.ID and  case isnull(sp.CanApproveOwnRequests,0)  when 0 then poe.IDEmployee else -1 end = eg.IDEmployee )
GO

CREATE VIEW [dbo].[sysrovwGetPermissionOverFeaturev3] AS
select  sysfp.IDGroupFeature, sysfp.IDFeature,fe.Alias , sysfp.Permision from sysroGroupFeatures_PermissionsOverFeatures sysfp WITH (NOLOCK) 
INNER JOIN dbo.sysroFeatures AS fe WITH (NOLOCK)  ON fe.ID = sysfp.IDFeature
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='575' WHERE ID='DBVersion'
GO
