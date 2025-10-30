-- No borréis esta línea
ALTER VIEW [dbo].[sysrovwSecurity_PermissionOverEmployees]  
AS  
   SELECT POG.IdPassport,  
    POG.IsRoboticsUser,  
    EG.IdEmployee,  
    EG.BeginDate,  
    EG.EndDate  
   FROM   Groups WITH (nolock)  
    INNER JOIN (SELECT idgroup, idpassport, path, CASE CHARINDEX('@@ROBOTICS@@',SP.Description) WHEN 0 THEN 0 ELSE 1 END AS IsRoboticsUser  
       FROM   sysroPassports_Groups SPG WITH (nolock)  
       INNER JOIN groups ON groups.id = SPG.idgroup  
       INNER JOIN sysroPassports SP ON SP.Id = SPG.IDPassport AND SP.IsSupervisor = 1 AND SP.State = 1 AND CONVERT(Date,GETDATE()) BETWEEN ISNULL(SP.StartDate, CONVERT(Date, GETDATE())) AND ISNULL(SP.ExpirationDate, CONVERT(Date, GETDATE()))  
      ) POG ON groups.path = POG.path OR groups.path LIKE POG.path + '\%'  
    INNER JOIN dbo.sysrovwgetemployeegroup AS EG WITH (nolock) ON groups.id = EG.idgroup  
    LEFT OUTER JOIN sysroPassports_Employees SPE WITH (nolock) ON SPE.IDPassport = POG.idpassport AND SPE.IDEmployee = EG.idemployee  
   WHERE SPE.Permission IS NULL
   UNION  
   SELECT SPE.IdPassport,  
    0 AS IsRoboticsUser,  
    SPE.IdEmployee,  
    CONVERT(smalldatetime,'1900-01-01',120) AS BeginDate,  
    CONVERT(smalldatetime,'2079-01-01',120) AS EndDate  
   FROM   sysroPassports_Employees SPE  
   INNER JOIN sysroPassports SP ON SP.Id = SPE.Idpassport AND SP.IsSupervisor = 1 AND SP.State = 1 AND CONVERT(Date,GETDATE()) BETWEEN ISNULL(SP.StartDate, CONVERT(Date, GETDATE())) AND ISNULL(SP.ExpirationDate, CONVERT(Date, GETDATE()))
   WHERE  SPE.Permission = 1   
GO

ALTER VIEW [dbo].[sysrovwSecurity_PermissionOverGroups]
AS
		  SELECT POG.IdPassport,
				 Groups.Id AS IdGroup,
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

ALTER VIEW [dbo].[sysrovwSecurity_PermissionOverCostCenters]
AS
		SELECT sp.ID AS IDPassport, 
		       spc.IDCenter AS IDCostCenter
		FROM   sysroSecurityGroupFeature_Centers SPC WITH (nolock)
		INNER JOIN sysroPassports SP ON SP.IDGroupFeature = SPC.IDGroupFeature AND SP.IsSupervisor = 1 AND SP.State = 1 AND CONVERT(Date,GETDATE()) BETWEEN ISNULL(SP.StartDate, CONVERT(Date, GETDATE())) AND ISNULL(SP.ExpirationDate, CONVERT(Date, GETDATE()))
GO

ALTER VIEW [dbo].[sysrovwSecurity_PermissionOverFeatures] 
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
	 INNER JOIN sysroGroupFeatures SGF ON SGF.ID = SP.IDGroupFeature AND SP.IsSupervisor = 1 AND (GroupType = '' OR GroupType = 'E')  AND SP.State = 1 AND CONVERT(Date,GETDATE()) BETWEEN ISNULL(SP.StartDate, CONVERT(Date, GETDATE())) AND ISNULL(SP.ExpirationDate, CONVERT(Date, GETDATE()))
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

ALTER VIEW [dbo].[sysrovwSecurity_PermissionOverExceptionEmployeesGroups]
AS
		  -- Indirect permission due to employee exception
		  SELECT POG1.IdPassport,
				 Groups.Id AS IDGroup,
				 POG1.BeginDate,
				 POG1.EndDate
		  FROM   Groups WITH (nolock)
				 INNER JOIN (SELECT SPE.IdPassport, GR.Id IdGroup, GR.Path, SPE.Permission, EG.begindate, EG.enddate
							 FROM sysroPassports_Employees SPE WITH (nolock)
							 INNER JOIN sysroPassports SP ON SP.Id = SPE.Idpassport AND SP.IsSupervisor = 1 AND SP.State = 1 AND CONVERT(Date,GETDATE()) BETWEEN ISNULL(SP.StartDate, CONVERT(Date, GETDATE())) AND ISNULL(SP.ExpirationDate, CONVERT(Date, GETDATE()))
							 INNER JOIN dbo.sysrovwGetEmployeeGroup AS EG WITH (nolock) ON SPE.IDEmployee = EG.idemployee
							 INNER JOIN Groups GR ON GR.Id = EG.idgroup
							 WHERE SPE.Permission =1 
							 ) POG1 ON Groups.Path = POG1.Path OR POG1.Path LIKE Groups.Path + '\%'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='810' WHERE ID='DBVersion'
GO
