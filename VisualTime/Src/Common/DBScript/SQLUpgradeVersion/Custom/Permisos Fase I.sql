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
							 WHERE SPE.Permission =1 
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
	 
		IF EXISTS (SELECT * FROM sysrovwSecurity_PermissionOverEmployees WHERE IdPassport = @pidPassport AND IdEmployee = @pidEmployee AND CONVERT(DATE,@pdate) BETWEEN BeginDate AND EndDate)
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
WHERE 1 = CASE WHEN Requests.RequestType = 12 THEN (SELECT COUNT(IDCenter) FROM sysroPassports_Centers c WITH (nolock) WHERE c.IDCenter = Requests.IDCenter AND c.IDPassport = sysroPassports.IDParentPassport) ELSE 1 END
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


-----------------------------------------------------

DROP VIEW IF EXISTS [dbo].[sysrovwSecurity_PermissionOverGroups]
GO
CREATE VIEW [dbo].[sysrovwSecurity_PermissionOverGroups]
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
							 INNER JOIN sysroPassports SP ON SP.Id = SPG.IDPassport AND SP.IsSupervisor = 1
							) POG ON groups.path = POG.path OR groups.path LIKE POG.path + '\%'
GO 

DROP VIEW IF EXISTS [dbo].[sysrovwSecurity_PermissionOverCostCenters]
GO
CREATE VIEW [dbo].[sysrovwSecurity_PermissionOverCostCenters]
AS
		SELECT sp.ID AS IDPassport, 
		       spc.IDCenter AS IDCostCenter
		FROM   sysroSecurityGroupFeature_Centers SPC WITH (nolock)
		INNER JOIN sysroPassports SP ON SP.IDGroupFeature = SPC.IDGroupFeature AND SP.IsSupervisor = 1
GO

DELETE [dbo].[sysroPassports_Employees]
GO

INSERT INTO [dbo].[sysroPassports_Employees] (IDPassport, IDEmployee, Permission)
SELECT Id AS IdPassport, IdEmployee, CanApproveOwnRequests AS Permission 
FROM [dbo].[sysroPassports] 
WHERE IsSupervisor = 1 AND GroupType = 'E'
GO

DROP VIEW IF EXISTS [dbo].[sysrovwSecurity_PermissionOverFeatures]
GO
CREATE VIEW [dbo].[sysrovwSecurity_PermissionOverFeatures] 
AS  
	 --As Supervisors
	 SELECT SP.Id IdPassport, SP.IdEmployee, SF.Id IdFeature, SF.Alias FeatureAlias, SF.Type FeatureType, SGFPOF.Permision Permission
	 FROM sysroPassports SP
	 INNER JOIN sysroGroupFeatures SGF ON SGF.ID = SP.IDGroupFeature AND SP.IsSupervisor = 1 AND (GroupType = '' OR GroupType = 'E')
	 INNER JOIN sysroGroupFeatures_PermissionsOverFeatures SGFPOF ON SGFPOF.IDGroupFeature = SGF.ID
	 INNER JOIN sysroFeatures SF ON SF.Type = 'U' AND SF.ID = SGFPOF.IDFeature
	 UNION ALL
	 --As Employee
	 SELECT SP.Id IdPassport, SP.IdEmployee, SF.Id IdFeature, SF.Alias FeatureAlias, SF.Type FeatureType, SPPOF.Permission
	 FROM sysroPassports SP 
	 INNER JOIN sysroPassports_PermissionsOverFeatures SPPOF ON SPPOF.IDPassport = SP.ID AND GroupType = 'E' 
	 INNER JOIN sysroFeatures SF ON SF.Type = 'E' AND SF.ID = SPPOF.IDFeature
GO

DROP TABLE IF EXISTS [dbo].[sysroDocumentAreas]
GO
CREATE TABLE [dbo].[sysroDocumentAreas](
	[Id] [int] NOT NULL,
	[IdCategory] [int] NOT NULL,
	[FeatureId] [int] NOT NULL,
 CONSTRAINT [PK_sysroDocumentAreas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [dbo].[sysroDocumentAreas]  WITH NOCHECK ADD  CONSTRAINT [FK_sysroDocumentAreas_sysroCategoryTypes] FOREIGN KEY([IdCategory])
REFERENCES [dbo].[sysroCategoryTypes] ([ID])
GO

ALTER TABLE [dbo].[sysroDocumentAreas] CHECK CONSTRAINT [FK_sysroDocumentAreas_sysroCategoryTypes]
GO



INSERT INTO [dbo].[sysroDocumentAreas] (Id, IdCategory, FeatureId) VALUES (0, 0, 32210)
GO
INSERT INTO [dbo].[sysroDocumentAreas] (Id, IdCategory, FeatureId) VALUES (1, 1, 32220)
GO
INSERT INTO [dbo].[sysroDocumentAreas] (Id, IdCategory, FeatureId) VALUES (2, 2, 32230)
GO
INSERT INTO [dbo].[sysroDocumentAreas] (Id, IdCategory, FeatureId) VALUES (3, 3, 32240)
GO
INSERT INTO [dbo].[sysroDocumentAreas] (Id, IdCategory, FeatureId) VALUES (4, 4, 32250)
GO

DROP TABLE IF EXISTS [dbo].[sysroDocumentLOPDLevels]
GO
CREATE TABLE [dbo].[sysroDocumentLOPDLevels](
	[Id] [INT] NOT NULL,
	[FeatureId] [INT] NOT NULL,
 CONSTRAINT [PK_sysroDocumentLOPDLevels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[FeatureId] ASC
))
GO

INSERT INTO [dbo].[sysroDocumentLOPDLevels] (Id, FeatureId) VALUES (0, 32310)
GO
INSERT INTO [dbo].[sysroDocumentLOPDLevels] (Id, FeatureId) VALUES (1, 32320)
GO
INSERT INTO [dbo].[sysroDocumentLOPDLevels] (Id, FeatureId) VALUES (2, 32330)
GO

---->>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
ALTER TABLE sysroNotificationTypes ADD RequiresPermissionOverEmployees BIT
GO

DROP VIEW IF EXISTS [dbo].[sysrovwSecurity_PendingRequestsDependencies]
GO
CREATE VIEW [dbo].[sysrovwSecurity_PendingRequestsDependencies]  
AS  
SELECT  IdRequest, IdEmployee, IdPassport, Permission, SupervisorLevelOfAuthority, ShowFromLevelOfAuthority, RequestCurrentStatus, RequestCurrentApprovalLevel, NextLevelOfAuthorityRequired, AutomaticValidation, CASE WHEN (SupervisorLevelOfAuthority = NextLevelOfAuthorityRequired) THEN 1 ELSE 0 END  AS DirectDependence  
FROM   
(  
SELECT IdRequest, IdEmployee, IdPassport, Permission, SupervisorLevelOfAuthority, ShowFromLevelOfAuthority, RequestCurrentStatus, RequestCurrentApprovalLevel, AutomaticValidation,       
    MAX(CASE WHEN SupervisorLevelOfAuthority < RequestCurrentApprovalLevel AND Permission >= 3 THEN SupervisorLevelOfAuthority ELSE 0 END)  OVER (PARTITION BY IdRequest Order By IdRequest ASC) AS NextLevelOfAuthorityRequired   
FROM sysrovwSecurity_PermissionOverRequests WHERE  Permission >= 3 AND RequestCurrentStatus IN (0,1)  
) AUX   
GO

DROP FUNCTION IF EXISTS [dbo].[sysrofnSecurity_GeneralNotificationsDependencies]
 GO

 CREATE FUNCTION [dbo].[sysrofnSecurity_GeneralNotificationsDependencies]
   (			
     @idnotificationtype INT,
 	 @idemployee INT
   )

   RETURNS @ValueTable table(IdNotificationType int, IdPassport int, IdEmployee INT, Permission INT, SupervisorLevelOfAuthority INT, LevelOfAuthorityToNotify INT, ShouldBeNotified BIT) 
   AS
   BEGIN
			DECLARE @idnotificationtypepar INT = @idnotificationtype
			DECLARE @idemployeepar INT = @idemployee

      		INSERT INTO @ValueTable
			SELECT  IdNotificationType, IdPassport, IdEmployee, Permission, SupervisorLevelOfAuthority, LevelOfAuthorityToNotify, CASE WHEN (SupervisorLevelOfAuthority = LevelOfAuthorityToNotify) THEN 1 ELSE 0 END  AS ShouldBeNotified
			FROM 
			(
			SELECT IdNotificationType, IdPassport, IdEmployee, Permission, SupervisorLevelOfAuthority,     
				   MAX(CASE WHEN SupervisorLevelOfAuthority < 15 AND Permission > 3 THEN SupervisorLevelOfAuthority ELSE 0 END)  OVER (PARTITION BY IdNotificationType, IdEmployee Order By IdNotificationType ASC, IdEmployee ASC) AS LevelOfAuthorityToNotify 
			FROM 
			(
			-- Notificaciones que no requieren permisos sobre empleados
					SELECT 
						SNT.Id AS IdNotificationType,
						POF.IdPassport, 
						0 AS IdEmployee,
						POF.Permission,
						ISNULL(SPC.LevelOfAuthority, 15) AS SupervisorLevelOfAuthority
					FROM sysroNotificationTypes SNT
					LEFT JOIN sysrovwSecurity_PermissionOverFeatures POF ON SNT.Feature = POF.FeatureAlias AND POF.FeatureType = 'U' AND SNT.RequiresPermissionOverEmployees = 0
					INNER JOIN sysroPassports WITH (nolock) ON POF.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 AND sysroPassports.Description NOT LIKE '%@@ROBOTICS@@%'
					LEFT JOIN sysroPassports_Categories SPC ON SPC.IDPassport = POF.IdPassport AND SPC.IDCategory = SNT.IDCategory
					WHERE @idemployeepar = 0 AND SNT.Id = @idnotificationtypepar
					UNION ALL
					-- Notificaciones que SI requieren permisos sobre empleados 
					SELECT 
						SNT.Id AS IdNotificationType,
						POF.IdPassport, 
						POE.IdEmployee,
						POF.Permission,
						ISNULL(SPC.LevelOfAuthority, 15) AS SupervisorLevelOfAuthority
					FROM sysroNotificationTypes SNT
					LEFT JOIN sysrovwSecurity_PermissionOverFeatures POF ON SNT.Feature = POF.FeatureAlias AND POF.FeatureType = 'U'  AND SNT.RequiresPermissionOverEmployees = 1 
					INNER JOIN sysroPassports WITH (nolock) ON POF.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 AND sysroPassports.Description NOT LIKE '%@@ROBOTICS@@%'
					LEFT JOIN sysrovwSecurity_PermissionOverEmployees POE ON POE.IdPassport = POF.IdPassport AND CONVERT(date,getdate()) BETWEEN POE.BeginDate AND POE.EndDate
					LEFT JOIN sysroPassports_Categories SPC ON SPC.IDPassport = POF.IdPassport AND SPC.IDCategory = SNT.IDCategory 
					WHERE POE.IdEmployee = @idemployeepar AND SNT.Id = @idnotificationtypepar
			) AUX1 WHERE  AUX1.Permission > 3  
			) AUX2
 	RETURN
   END
GO

 DROP FUNCTION IF EXISTS [dbo].[sysrofnSecurity_RequestsNotificationsDependencies]
 GO

 CREATE FUNCTION [dbo].[sysrofnSecurity_RequestsNotificationsDependencies]
   (			
     @iderequest INT
   )

   RETURNS @ValueTable table(IdRequest int, IdEmployee INT, IdPassport int, Permission INT, SupervisorLevelOfAuthority INT, ShowFromLevelOfAuthority INT, RequestCurrentStatus INT, RequestCurrentApprovalLevel INT, NextLevelOfAuthorityRequired INT, ShouldBeNotified BIT) 
   AS
   BEGIN
			DECLARE @iderequestpar INT = @iderequest

      		INSERT INTO @ValueTable
			SELECT IdRequest, IdEmployee, IdPassport, Permission, SupervisorLevelOfAuthority, ShowFromLevelOfAuthority, RequestCurrentStatus, RequestCurrentApprovalLevel, NextLevelOfAuthorityRequired, DirectDependence AS ShouldBeNotified
			FROM sysrovwSecurity_PendingRequestsDependencies
			WHERE IdRequest = @iderequestpar
 	RETURN
   END
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
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POA WITH (nolock) ON POA.IdPassport = POE.IdPassport AND POA.IdFeature = sysroDocumentAreas.FeatureId AND POA.Permission > 3
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POL WITH (nolock) ON POL.IdPassport = POE.IdPassport AND POL.IdFeature = sysroDocumentLOPDLevels.FeatureId AND POL.Permission > 3
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
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POA WITH (nolock) ON POA.IdPassport = POG.IdPassport AND POA.IdFeature = sysroDocumentAreas.FeatureId AND POA.Permission > 3
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POL WITH (nolock) ON POL.IdPassport = POG.IdPassport AND POL.IdFeature = sysroDocumentLOPDLevels.FeatureId AND POL.Permission > 3

			INNER JOIN sysroPassports WITH (nolock) ON POG.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 AND sysroPassports.Description NOT LIKE '%@@ROBOTICS@@%'
			LEFT JOIN sysroPassports_Categories AS PassportCategory ON PassportCategory.IDPassport = POG.IdPassport AND PassportCategory.IDCategory = DocumentTemplates.Area
			) AUX1 WHERE AUX1.PermissionOverArea > 3  AND AUX1.PermissionOverLOPDLevel > 3  
			) AUX 
 	RETURN
   END
GO

DROP FUNCTION IF EXISTS [dbo].[sysrofnSecurity_PermissionOverEmployees]
GO
CREATE FUNCTION [dbo].[sysrofnSecurity_PermissionOverEmployees](@idPassport int, @idEmployee int, @date datetime)  RETURNS INT AS      
    BEGIN      
		DECLARE @Result INT   
		DECLARE @pidEmployee INT = @idEmployee
		DECLARE @pidPassport INT = @idPassport
		DECLARE @pdate DateTime = @date
	 
		IF EXISTS (SELECT * FROM sysrovwSecurity_PermissionOverEmployees WHERE IdPassport = @pidPassport AND IdEmployee = @pidEmployee AND CONVERT(DATE,@pdate) BETWEEN BeginDate AND EndDate)
			SET @Result = 1
		ELSE
			SET @Result = 0
	    RETURN @Result
    END
GO

UPDATE sysroNotificationTypes SET RequiresPermissionOverEmployees = 0
GO

UPDATE sysroNotificationTypes SET RequiresPermissionOverEmployees = 1 WHERE ID NOT IN(89) AND FeatureType = 'U'
GO

UPDATE sysroNotificationTypes SET Feature = '*Channels*' WHERE Id = 89
GO

UPDATE sysroNotificationTypes SET Feature = '*Documents*' WHERE Id IN (49, 53)
GO

--*********************************************************************************************************************************************************************

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


--+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
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

----------<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>

DROP FUNCTION IF EXISTS [dbo].[sysrofnSecurity_RequestsNotificationsDependencies]
GO

DROP TABLE IF EXISTS [dbo].[sysroDocumentAreas]
GO
CREATE TABLE [dbo].[sysroDocumentAreas](
	[Id] [int] NOT NULL,
	[IdCategory] [int] NOT NULL,
	[IdFeature] [int] NOT NULL,
 CONSTRAINT [PK_sysroDocumentAreas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

INSERT INTO [dbo].[sysroDocumentAreas] (Id, IdCategory, IdFeature) VALUES (0, 0, 32210)
GO
INSERT INTO [dbo].[sysroDocumentAreas] (Id, IdCategory, IdFeature) VALUES (1, 1, 32220)
GO
INSERT INTO [dbo].[sysroDocumentAreas] (Id, IdCategory, IdFeature) VALUES (2, 2, 32230)
GO
INSERT INTO [dbo].[sysroDocumentAreas] (Id, IdCategory, IdFeature) VALUES (3, 3, 32240)
GO
INSERT INTO [dbo].[sysroDocumentAreas] (Id, IdCategory, IdFeature) VALUES (4, 4, 32250)
GO

DROP TABLE IF EXISTS [dbo].[sysroDocumentLOPDLevels]
GO
CREATE TABLE [dbo].[sysroDocumentLOPDLevels](
	[Id] [INT] NOT NULL,
	[IdFeature] [INT] NOT NULL,
 CONSTRAINT [PK_sysroDocumentLOPDLevels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[IdFeature] ASC
))
GO

INSERT INTO [dbo].[sysroDocumentLOPDLevels] (Id, IdFeature) VALUES (0, 32310)
GO
INSERT INTO [dbo].[sysroDocumentLOPDLevels] (Id, IdFeature) VALUES (1, 32320)
GO
INSERT INTO [dbo].[sysroDocumentLOPDLevels] (Id, IdFeature) VALUES (2, 32330)
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

DROP FUNCTION IF EXISTS [dbo].[sysrofnSecurity_PermissionsOverDocumentTemplates]
 GO

 CREATE FUNCTION [dbo].[sysrofnSecurity_PermissionsOverDocumentTemplates]
   (			
     @iddocumenttemplate INT,
 	 @idemployee INT,
	 @idcompany INT
   )

   RETURNS @ValueTable table(IdDocumentTemplate int, IdEmployee INT, IdCompany INT, IdPassport int, PermissionOverArea INT, PermissionOverLOPDLevel INT, SupervisorLevelOfAuthority INT, LevelOfAuthorityToNotify INT, DirectDependence BIT) 
   AS
   BEGIN
			DECLARE @iddocumenttemplatepar INT = @iddocumenttemplate
			DECLARE @idemployeepar INT = @idemployee
			DECLARE @idcompanypar INT = @idcompany
			

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
			INNER JOIN sysroDocumentAreas ON sysroDocumentAreas.Id = DocumentTemplates.Area AND DocumentTemplates.Scope IN (0,1,3,4,5) AND  DocumentTemplates.Id = @iddocumenttemplatepar
			INNER JOIN sysroDocumentLOPDLevels ON sysroDocumentLOPDLevels.Id = DocumentTemplates.LOPDAccessLevel
			INNER JOIN sysrovwSecurity_PermissionOverEmployees POE ON (POE.IdEmployee = @idemployeepar OR @idemployeepar = 0) AND CONVERT(date,getdate()) BETWEEN POE.BeginDate AND POE.EndDate
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POA WITH (nolock) ON POA.IdPassport = POE.IdPassport AND POA.IdFeature = sysroDocumentAreas.IdFeature 
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POL WITH (nolock) ON POL.IdPassport = POE.IdPassport AND POL.IdFeature = sysroDocumentLOPDLevels.IdFeature 
			INNER JOIN sysroPassports WITH (nolock) ON POE.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 AND sysroPassports.Description NOT LIKE '%@@ROBOTICS@@%'
			LEFT JOIN sysroPassports_Categories AS PassportCategory ON PassportCategory.IDPassport = POE.IdPassport AND PassportCategory.IDCategory = DocumentTemplates.Area
			) AUX1 
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
			INNER JOIN sysroDocumentAreas ON sysroDocumentAreas.Id = DocumentTemplates.Area AND DocumentTemplates.Scope IN (2,6) AND DocumentTemplates.Id = @iddocumenttemplatepar 
			INNER JOIN sysroDocumentLOPDLevels ON sysroDocumentLOPDLevels.Id = DocumentTemplates.LOPDAccessLevel
			INNER JOIN sysrovwSecurity_PermissionOverGroups POG ON (POG.IdGroup = @idcompanypar OR @idcompanypar = 0)
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POA WITH (nolock) ON POA.IdPassport = POG.IdPassport AND POA.IdFeature = sysroDocumentAreas.IdFeature
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POL WITH (nolock) ON POL.IdPassport = POG.IdPassport AND POL.IdFeature = sysroDocumentLOPDLevels.IdFeature 
			INNER JOIN sysroPassports WITH (nolock) ON POG.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 AND sysroPassports.Description NOT LIKE '%@@ROBOTICS@@%'
			LEFT JOIN sysroPassports_Categories AS PassportCategory ON PassportCategory.IDPassport = POG.IdPassport AND PassportCategory.IDCategory = DocumentTemplates.Area
			) AUX1 
			) AUX 
 	RETURN
   END
GO

-----#################((((((((((((((((((((((((((((())))))))))))))))))))))))))))))))))))))))))))))))))))))###############################################

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





--&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&

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






--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
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


--%$%$%$%$%$%$%$%$%%$%$%$%$%$%$%
DROP FUNCTION IF EXISTS [dbo].[sysrofnSecurity_PermissionOverEmployees]
GO
DROP FUNCTION IF EXISTS [dbo].[sysrofnSecurity_PermissionOverFeatures]
GO
DROP FUNCTION IF EXISTS [dbo].[sysrofnSecurity_PermissionOverGroups]
GO