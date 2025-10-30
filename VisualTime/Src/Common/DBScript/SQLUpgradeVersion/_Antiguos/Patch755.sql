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

   RETURNS @ValueTable table(IdRequest int, IdEmployee INT, IdPassport int, Permission INT, SupervisorLevelOfAuthority INT, ShowFromLevelOfAuthority INT, RequestCurrentStatus INT, RequestCurrentApprovalLevel INT, NextLevelOfAuthorityRequired INT, DirectDependence BIT) 
   AS
   BEGIN
			DECLARE @iderequestpar INT = @iderequest

      		INSERT INTO @ValueTable
			SELECT IdRequest, IdEmployee, IdPassport, Permission, SupervisorLevelOfAuthority, ShowFromLevelOfAuthority, RequestCurrentStatus, RequestCurrentApprovalLevel, NextLevelOfAuthorityRequired, DirectDependence 
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





-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='755' WHERE ID='DBVersion'
GO
