-- No borréis esta línea
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



UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='753' WHERE ID='DBVersion'
GO
