CREATE TABLE [dbo].[sysroPassports_Employees](
	[IDPassport] [int] NOT NULL,
	[IDEmployee] [int] NOT NULL,
	[Permission] [bit] NOT NULL,
 CONSTRAINT [PK_sysroPassports_Employees] PRIMARY KEY CLUSTERED 
(
	[IDPassport] ASC,
	[IDEmployee] ASC,
	[Permission] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[sysroPassports_Employees]  WITH CHECK ADD  CONSTRAINT [FK_sysroPassports_Employees_Employees] FOREIGN KEY([IDEmployee])
REFERENCES [dbo].[Employees] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[sysroPassports_Employees] CHECK CONSTRAINT [FK_sysroPassports_Employees_Employees]
GO

ALTER TABLE [dbo].[sysroPassports_Employees]  WITH CHECK ADD  CONSTRAINT [FK_sysroPassports_Employees_sysroPassports] FOREIGN KEY([IDPassport])
REFERENCES [dbo].[sysroPassports] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[sysroPassports_Employees] CHECK CONSTRAINT [FK_sysroPassports_Employees_sysroPassports]
GO

ALTER TABLE [dbo].[sysroRequestType] ADD EmployeeFeatureId [INT], SupervisorFeatureId [INT]
GO

UPDATE [dbo].[sysroRequestType] SET EmployeeFeatureId = 22100, SupervisorFeatureId = 1560 WHERE IdType = 1
GO
UPDATE [dbo].[sysroRequestType] SET EmployeeFeatureId = 20110 , SupervisorFeatureId = 2321 WHERE IdType = 2
GO
UPDATE [dbo].[sysroRequestType] SET EmployeeFeatureId = 20120 , SupervisorFeatureId = 2322 WHERE IdType = 3
GO
UPDATE [dbo].[sysroRequestType] SET EmployeeFeatureId = 20130 , SupervisorFeatureId = 2323 WHERE IdType = 4
GO
UPDATE [dbo].[sysroRequestType] SET EmployeeFeatureId = 21110 , SupervisorFeatureId = 2510 WHERE IdType = 5
GO
UPDATE [dbo].[sysroRequestType] SET EmployeeFeatureId = 21120 , SupervisorFeatureId = 2520 WHERE IdType = 6
GO
UPDATE [dbo].[sysroRequestType] SET EmployeeFeatureId = 21140 , SupervisorFeatureId = 2540 WHERE IdType = 7
GO
UPDATE [dbo].[sysroRequestType] SET EmployeeFeatureId = 21150 , SupervisorFeatureId = 2550 WHERE IdType = 9
GO
UPDATE [dbo].[sysroRequestType] SET EmployeeFeatureId = 21160 , SupervisorFeatureId = 2560 WHERE IdType = 13
GO
UPDATE [dbo].[sysroRequestType] SET EmployeeFeatureId = 21170 , SupervisorFeatureId = 2570 WHERE IdType = 14
GO
UPDATE [dbo].[sysroRequestType] SET EmployeeFeatureId = 21130 , SupervisorFeatureId = 2580  WHERE IdType = 8
GO
UPDATE [dbo].[sysroRequestType] SET EmployeeFeatureId = 28110 , SupervisorFeatureId = 25800  WHERE IdType = 10
GO
UPDATE [dbo].[sysroRequestType] SET EmployeeFeatureId = 21120 , SupervisorFeatureId = 2530 WHERE IdType = 11
GO
UPDATE [dbo].[sysroRequestType] SET EmployeeFeatureId = 20140 , SupervisorFeatureId = 2324  WHERE IdType = 12
GO
UPDATE [dbo].[sysroRequestType] SET EmployeeFeatureId = 20150 , SupervisorFeatureId = 2325 WHERE IdType = 15
GO
UPDATE [dbo].[sysroRequestType] SET EmployeeFeatureId = 0 , SupervisorFeatureId = 2590 WHERE IdType = 16
GO
UPDATE [dbo].[sysroRequestType] SET EmployeeFeatureId = 20002 , SupervisorFeatureId = 2595 WHERE IdType = 17
GO

DROP VIEW IF EXISTS [dbo].[sysrovwSecurity_PermissionOverEmployees]
GO
CREATE VIEW [dbo].[sysrovwSecurity_PermissionOverEmployees]
AS
	  SELECT POG.IdPassport,
			 EG.IdEmployee,
			 EG.BeginDate,
			 EG.EndDate
	  FROM   Groups WITH (nolock)
			 INNER JOIN (SELECT idgroup, idpassport, path
						 FROM   sysropassports_groups SPG WITH (nolock)
						 INNER JOIN groups ON groups.id = SPG.idgroup
						 INNER JOIN sysroPassports SP ON SP.Id = SPG.IDPassport AND SP.IsSupervisor = 1
						) POG ON groups.path = POG.path OR groups.path LIKE POG.path + '\%'
			 INNER JOIN dbo.sysrovwgetemployeegroup AS EG WITH (nolock) ON groups.id = EG.idgroup
			 LEFT OUTER JOIN sysroPassports_Employees SPE WITH (nolock) ON SPE.IDPassport = POG.idpassport AND SPE.IDEmployee = EG.idemployee
	  WHERE SPE.Permission IS NULL OR SPE.Permission = 1 
	  UNION ALL
	  SELECT SPE.IdPassport,
			 SPE.IdEmployee,
			 CONVERT(smalldatetime,'1900-01-01',120) AS BeginDate,
			 CONVERT(smalldatetime,'2079-01-01',120) AS EndDate
	  FROM   sysroPassports_Employees SPE 
	  WHERE  SPE.Permission = 1 
GO

DROP VIEW IF EXISTS [dbo].[sysrovwSecurity_PermissionOverGroups]
GO
CREATE VIEW [dbo].[sysrovwSecurity_PermissionOverGroups]
AS
		  -- Direct permission
		  SELECT POG.IdPassport,
				 Groups.Id AS IdGroup,
				 CONVERT(smalldatetime,'1900-01-01',120) AS BeginDate,
				 CONVERT(smalldatetime,'2079-01-01',120) AS EndDate
		  FROM   groups WITH (nolock)
				 INNER JOIN (SELECT idgroup, idpassport, path
							 FROM   sysropassports_groups SPG WITH (nolock)
							 INNER JOIN groups ON groups.id = SPG.idgroup
							 INNER JOIN sysroPassports SP ON SP.Id = SPG.IDPassport AND SP.IsSupervisor = 1
							) POG ON groups.path = POG.path OR groups.path LIKE POG.path + '\%'
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
							 WHERE SPE.Permission =1 AND SPE.IDPassport = 5003
							 ) POG1 ON Groups.Path = POG1.Path OR POG1.Path LIKE Groups.Path + '\%'
GO

DROP VIEW IF EXISTS [dbo].[sysrovwSecurity_PermissionOverFeatures]
GO
CREATE VIEW [dbo].[sysrovwSecurity_PermissionOverFeatures] 
AS  
	 --Only Supervisors
	 SELECT SP.Id IdPassport, SF.Id IdFeature, SF.Alias FeatureAlias, SF.Type FeatureType, SGFPOF.Permision Permission
	 FROM sysroPassports SP
	 INNER JOIN sysroGroupFeatures SGF ON SGF.ID = SP.IDGroupFeature
	 INNER JOIN sysroGroupFeatures_PermissionsOverFeatures SGFPOF ON SGFPOF.IDGroupFeature = SGF.ID
	 INNER JOIN sysroFeatures SF ON SF.Type = 'U' AND SF.ID = SGFPOF.IDFeature
	 WHERE IsSupervisor = 1 AND GroupType = ''
	 UNION ALL
	 --Empleados And Supervisor, as Supervisor
	 SELECT SP.Id IdPassport, SF.Id IdFeature, SF.Alias FeatureAlias, SF.Type FeatureType, SGFPOF.Permision Permission 
	 FROM sysroPassports SP 
	 INNER JOIN sysroGroupFeatures SGF ON SGF.ID = SP.IDGroupFeature
	 INNER JOIN sysroGroupFeatures_PermissionsOverFeatures SGFPOF ON SGFPOF.IDGroupFeature = SGF.ID
	 INNER JOIN sysroFeatures SF ON SF.Type = 'U' AND SF.ID = SGFPOF.IDFeature
	 WHERE IsSupervisor = 1 AND GroupType = 'E'
	 UNION ALL
	 --Empleados And Supervisor, as Employee
	 SELECT SP.Id IdPassport, SF.Id IdFeature, SF.Alias FeatureAlias, SF.Type FeatureType, SPPOF.Permission 
	 FROM sysroPassports SP 
	 INNER JOIN sysroPassports_PermissionsOverFeatures SPPOF ON SPPOF.IDPassport = SP.ID
	 INNER JOIN sysroFeatures SF ON SF.Type = 'E' AND SF.ID = SPPOF.IDFeature
	 WHERE IsSupervisor = 1 AND GroupType = 'E'
	 UNION ALL
	 --Only Employee
	 SELECT SP.Id IdPassport, SF.Id IdFeature, SF.Alias FeatureAlias, SF.Type FeatureType, SPPOF.Permission 
	 FROM sysroPassports SP 
	 INNER JOIN sysroPassports_PermissionsOverFeatures SPPOF ON SPPOF.IDPassport = SP.ID
	 INNER JOIN sysroFeatures SF ON SF.Type = 'E' AND SF.ID = SPPOF.IDFeature
	 WHERE IsSupervisor = 0 AND GroupType = 'E'
GO

DROP FUNCTION IF EXISTS [dbo].[sysrofnSecurity_PermissionOverEmployees]
GO
CREATE FUNCTION [dbo].[sysrofnSecurity_PermissionOverEmployees](@idPassport int, @idEmployee int, @date datetime)  RETURNS INT AS      
    BEGIN      
		DECLARE @Result INT   
		DECLARE @pidEmployee INT = @idEmployee
		DECLARE @pidPassport INT = @idPassport
		DECLARE @pdate DateTime = @date
	 
		IF EXISTS (SELECT * FROM sysrovwSecurity_PermissionOverEmployees WHERE IdPassport = @pidPassport AND IdEmployee = @pidEmployee AND @pdate BETWEEN BeginDate AND EndDate)
			SET @Result = 1
		ELSE
			SET @Result = 0
	    RETURN @Result
    END
GO

DROP FUNCTION IF EXISTS [dbo].[sysrofnSecurity_PermissionOverFeatures]
GO
CREATE FUNCTION [dbo].[sysrofnSecurity_PermissionOverFeatures](@idPassport int, @featureAlias nvarchar(MAX), @featureType varchar(1))  RETURNS INT AS      
    BEGIN      
		DECLARE @Result INT   
		DECLARE @pidPassport INT = @idPassport
		DECLARE @pfeatureAlias nvarchar(MAX) = @featureAlias
		DECLARE @pfeatureType nvarchar(1) = @featureType
	 
		SELECT @Result = ISNULL(Permission,0) FROM sysrovwSecurity_PermissionOverFeatures WHERE IdPassport = @pidPassport AND FeatureAlias = @pfeatureAlias AND FeatureType = @pfeatureType
	    
		RETURN ISNULL(@Result,0)
    END
GO 

DROP FUNCTION IF EXISTS [dbo].[sysrofnSecurity_PermissionOverGroups]
GO
CREATE FUNCTION [dbo].[sysrofnSecurity_PermissionOverGroups](@idPassport int, @idGroup int)  RETURNS INT AS      
    BEGIN      
		DECLARE @Result INT   
		DECLARE @pidPassport INT = @idPassport
		DECLARE @pidGroup INT = @idGroup
	 
    	IF EXISTS (SELECT * FROM sysrovwSecurity_PermissionOverGroups WHERE IdPassport = @pidPassport AND IdGroup = @pidGroup)
			SET @Result = 1
		ELSE
			SET @Result = 0
	    RETURN @Result
	    
		RETURN @Result
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
INNER JOIN (SELECT Id AS IdCause, ISNULL(BusinessGroup,'') AS BusinessGroup, IDCategory, MinLevelOfAuthority AS LevelOfAuthority FROM Causes WITH (nolock)) AS CausesBusinessGroups ON ISNULL(Requests.IDCause, 0) = CausesBusinessGroups.IdCause
INNER JOIN (SELECT Shifts.Id AS IdShift, ISNULL(ShiftGroups.BusinessGroup, '') AS BusinessGroup FROM Shifts WITH (nolock) LEFT OUTER JOIN ShiftGroups WITH (nolock) ON Shifts.IDGroup = ShiftGroups.ID) AS ShiftsBusinessGroups ON ISNULL(Requests.IDShift,0) = ShiftsBusinessGroups.IdShift
LEFT JOIN sysroPassports_Categories AS PassportCategory ON PassportCategory.IDPassport = sysroPassports.Id AND PassportCategory.IDCategory = sysroRequestType.IDCategory
LEFT JOIN sysroPassports_Categories AS PassportCategoryByCause ON PassportCategoryByCause.IDPassport = sysroPassports.Id AND PassportCategoryByCause.IDCategory = CausesBusinessGroups.IDCategory
WHERE 1 = CASE WHEN Requests.RequestType = 12 THEN (SELECT COUNT(sysroSecurityGroupFeature_Centers.IDCenter) FROM sysroSecurityGroupFeature_Centers INNER JOIN sysroGroupFeatures ON sysroGroupFeatures.Id = sysroSecurityGroupFeature_Centers.IDGroupFeature AND sysroSecurityGroupFeature_Centers.IDCenter = Requests.IDCenter WHERE sysroGroupFeatures.Id = sysroPassports.IDGroupFeature) ELSE 1 END
  AND 1 = CASE WHEN LEN(sysroGroupFeatures.BusinessGroupList) > 0 AND LEN(CausesBusinessGroups.BusinessGroup) > 0 AND Requests.IDCause > 0 AND CHARINDEX(CausesBusinessGroups.BusinessGroup, sysroGroupFeatures.BusinessGroupList) = 0 THEN 0 ELSE 1 END
  AND 1 = CASE WHEN LEN(sysroGroupFeatures.businessgrouplist) > 0 AND LEN(ShiftsBusinessGroups.BusinessGroup) > 0 AND Requests.IDShift > 0 AND CHARINDEX(ShiftsBusinessGroups.BusinessGroup, sysroGroupFeatures.BusinessGroupList) = 0 THEN 0 ELSE 1 END	
GO

DROP VIEW IF EXISTS [dbo].[sysrovwSecurity_PendingRequestsDependencies]
GO
CREATE VIEW [dbo].[sysrovwSecurity_PendingRequestsDependencies]
AS
SELECT  IdRequest, IdEmployee, IdPassport, Permission, SupervisorLevelOfAuthority, ShowFromLevelOfAuthority, RequestCurrentStatus, RequestCurrentApprovalLevel, NextLevelOfAuthorityRequired, CASE WHEN (SupervisorLevelOfAuthority = NextLevelOfAuthorityRequired) THEN 1 ELSE 0 END  AS DirectDependence
FROM 
(
SELECT IdRequest, IdEmployee, IdPassport, Permission, SupervisorLevelOfAuthority, ShowFromLevelOfAuthority, RequestCurrentStatus, RequestCurrentApprovalLevel,     
	   MAX(CASE WHEN SupervisorLevelOfAuthority < RequestCurrentApprovalLevel AND Permission >= 3 THEN SupervisorLevelOfAuthority ELSE 0 END)  OVER (PARTITION BY IdRequest Order By IdRequest ASC) AS NextLevelOfAuthorityRequired 
FROM sysrovwSecurity_PermissionOverRequests WHERE  Permission >= 3 AND RequestCurrentStatus IN (0,1) AND AutomaticValidation = 0 
) AUX 
GO

DROP INDEX IF EXISTS [dbo].[sysroPassports_PermissionsOverFeatures].[IX_sysroPassports_PermissionsOverFeatures_Permission]
GO
CREATE NONCLUSTERED INDEX [IX_sysroPassports_PermissionsOverFeatures_Permission]
ON [dbo].[sysroPassports_PermissionsOverFeatures] ([IDFeature])
INCLUDE ([Permission])
GO

DROP INDEX IF EXISTS [dbo].[Requests].[IX_RequestsStatus_Permissions]
GO
CREATE NONCLUSTERED INDEX [IX_RequestsStatus_Permissions]
ON [dbo].[Requests] ([Status])
INCLUDE ([IDEmployee],[RequestType],[StatusLevel],[IDCause],[IDShift],[IDCenter])


-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='751' WHERE ID='DBVersion'
GO
