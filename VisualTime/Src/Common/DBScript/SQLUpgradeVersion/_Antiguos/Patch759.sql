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
			INNER JOIN sysroDocumentAreas ON sysroDocumentAreas.Id = DocumentTemplates.Area AND DocumentTemplates.Scope IN (2,6) AND DocumentTemplates.Id = @iddocumenttemplatepar 
			INNER JOIN sysroDocumentLOPDLevels ON sysroDocumentLOPDLevels.Id = DocumentTemplates.LOPDAccessLevel
			INNER JOIN sysrovwSecurity_PermissionOverGroups POG ON (POG.IdGroup = @idcompanypar OR @idcompanypar = 0)
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POA WITH (nolock) ON POA.IdPassport = POG.IdPassport AND POA.FeatureAlias = sysroDocumentAreas.FeatureAlias AND POA.FeatureType = 'U'
			INNER JOIN sysrovwSecurity_PermissionOverFeatures POL WITH (nolock) ON POL.IdPassport = POG.IdPassport AND POL.FeatureAlias = sysroDocumentLOPDLevels.FeatureAlias AND POL.FeatureType = 'U'
			INNER JOIN sysroPassports WITH (nolock) ON POG.IdPassport = sysroPassports.Id AND sysroPassports.IsSupervisor = 1 AND sysroPassports.Description NOT LIKE '%@@ROBOTICS@@%'
			LEFT JOIN sysroPassports_Categories AS PassportCategory ON PassportCategory.IDPassport = POG.IdPassport AND PassportCategory.IDCategory = DocumentTemplates.Area
			) AUX1 WHERE AUX1.PermissionOverArea > 0  AND AUX1.PermissionOverLOPDLevel > 0
			) AUX 
 	RETURN
   END
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='759' WHERE ID='DBVersion'
GO
