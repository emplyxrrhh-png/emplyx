CREATE NONCLUSTERED INDEX sysropermissionsovergroups_Permission_EmpFeature
ON [dbo].[sysroPermissionsOverGroups] ([EmployeeFeatureID])
INCLUDE ([Permission])
GO

CREATE NONCLUSTERED INDEX sysropermissionsovergroups_Permission_EmpFeature_EmpGroup
ON [dbo].[sysroPermissionsOverGroups] ([EmployeeGroupID],[EmployeeFeatureID])
INCLUDE ([Permission])
GO

UPDATE sysroParameters SET Data='503' WHERE ID='DBVersion'
GO

