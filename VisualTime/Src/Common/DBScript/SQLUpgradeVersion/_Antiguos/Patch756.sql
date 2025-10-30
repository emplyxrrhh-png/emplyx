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
	  UNION ALL
	  SELECT SPE.IdPassport,
			 0 AS IsRoboticsUser,
			 SPE.IdEmployee,
			 CONVERT(smalldatetime,'1900-01-01',120) AS BeginDate,
			 CONVERT(smalldatetime,'2079-01-01',120) AS EndDate
	  FROM   sysroPassports_Employees SPE
	  WHERE  SPE.Permission = 1 
GO

DROP VIEW IF EXISTS [dbo].[sysrovwSecurity_PermissionOverFeatures]
GO
CREATE VIEW [dbo].[sysrovwSecurity_PermissionOverFeatures] 
AS  
	 --As Supervisors
	 SELECT SP.Id IdPassport, 
	        CASE CHARINDEX('@@ROBOTICS@@',SP.Description) WHEN 0 THEN 0 ELSE 1 END AS IsRoboticsUser,
	        SP.IdEmployee, 
			SF.Id IdFeature, 
			SF.Alias FeatureAlias, 
			SF.Type FeatureType, 
			SGFPOF.Permision Permission
	 FROM sysroPassports SP
	 INNER JOIN sysroGroupFeatures SGF ON SGF.ID = SP.IDGroupFeature AND SP.IsSupervisor = 1 AND (GroupType = '' OR GroupType = 'E')
	 INNER JOIN sysroGroupFeatures_PermissionsOverFeatures SGFPOF ON SGFPOF.IDGroupFeature = SGF.ID
	 INNER JOIN sysroFeatures SF ON SF.Type = 'U' AND SF.ID = SGFPOF.IDFeature
	 UNION ALL
	 --As Employee
	 SELECT SP.Id IdPassport, 
			CASE CHARINDEX('@@ROBOTICS@@',SP.Description) WHEN 0 THEN 0 ELSE 1 END AS IsRoboticsUser,
	        SP.IdEmployee, 
			SF.Id IdFeature, 
			SF.Alias FeatureAlias, 
			SF.Type FeatureType, 
			SPPOF.Permission
	 FROM sysroPassports SP 
	 INNER JOIN sysroPassports_PermissionsOverFeatures SPPOF ON SPPOF.IDPassport = SP.ID AND GroupType = 'E' 
	 INNER JOIN sysroFeatures SF ON SF.Type = 'E' AND SF.ID = SPPOF.IDFeature
GO

DROP VIEW IF EXISTS [dbo].[sysrovwSecurity_PermissionOverExceptionEmployeesGroups]
GO
CREATE VIEW [dbo].[sysrovwSecurity_PermissionOverExceptionEmployeesGroups]
AS
		  -- Indirect permission due to employee exception
		  SELECT POG1.IdPassport,
				 Groups.Id AS IDGroup,
				 POG1.BeginDate,
				 POG1.EndDate
		  FROM   Groups WITH (nolock)
				 INNER JOIN (SELECT SPE.IdPassport, GR.Id IdGroup, GR.Path, SPE.Permission, EG.begindate, EG.enddate
							 FROM sysroPassports_Employees SPE WITH (nolock)
							 INNER JOIN dbo.sysrovwGetEmployeeGroup AS EG WITH (nolock) ON SPE.IDEmployee = EG.idemployee
							 INNER JOIN Groups GR ON GR.Id = EG.idgroup
							 WHERE SPE.Permission =1 
							 ) POG1 ON Groups.Path = POG1.Path OR POG1.Path LIKE Groups.Path + '\%'
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='756' WHERE ID='DBVersion'
GO
