-- Grupo sistema
if NOT exists (select * from dbo.sysroPassports where Description = '@@ROBOTICS@@GSystem')
BEGIN
INSERT INTO [sysroPassports] ([IDParentPassport],[GroupType],[Name],[Description],[Email],[IDUser],[IDEmployee],[IDLanguage],[LevelOfAuthority])
VALUES(NULL,'U','Grupo sistema','@@ROBOTICS@@GSystem','',NULL,NULL,0,NULL)
DECLARE @IDGROUP int
SET @IDGROUP = @@IDENTITY

INSERT INTO [dbo].[sysroUsers] ([Login],[Password],[IDSecurityGroup],[UserConfData])VALUES('´WebLogin´ : ' + CAST(@IDGROUP AS varchar(10)),NULL,'',NULL)
DECLARE @IDUser int
SET @IDUser = @@IDENTITY

UPDATE sysroPassports SET IDUser = @IDUser WHERE ID = @IDGROUP

-- Usuario sistema
INSERT INTO [dbo].[sysroPassports] ([IDParentPassport],[GroupType],[Name],[Description],[Email],[IDUser],[IDEmployee],[IDLanguage],[LevelOfAuthority])
VALUES(@IDGROUP,'','System','@@ROBOTICS@@System','',NULL,NULL,0,NULL)
DECLARE @IDPASSPORT int
SET @IDPASSPORT = @@IDENTITY

INSERT INTO [dbo].[sysroUsers] ([Login],[Password],[IDSecurityGroup],[UserConfData])VALUES('´WebLogin´ : ' + CAST(@IDPASSPORT AS varchar(10)),NULL,'',NULL)
DECLARE @IDUserPassport int
SET @IDUserPassport = @@IDENTITY

UPDATE sysroPassports SET IDUser = @IDUserPassport, IsSupervisor = 1, LicenseAccepted=1,EnabledVTVisits = 0,EnabledVTVisitsApp = 0,EnabledVTDesktop=0,EnabledVTPortal=0,EnabledVTPortalApp=0,EnabledVTSupervisor=0,EnabledVTSupervisorApp=0 WHERE ID = @IDPASSPORT

-- Permisos del grupo system
DELETE From [dbo].sysroPassports_PermissionsOverFeatures Where IDPassport = @IDGROUP

INSERT INTO [dbo].sysroPassports_PermissionsOverFeatures([IDPassport], [IDFeature], [Permission])  select @IDGROUP, sysroFeatures.ID, 
	(CASE 
        WHEN CHARINDEX('A',PermissionTypes) > 0 THEN 9 
        WHEN CHARINDEX('W',PermissionTypes) > 0 THEN 6 
        WHEN CHARINDEX('R',PermissionTypes) > 0 THEN 3 
    End ) from sysroFeatures where Type = 'U' and IDParent is NULL

DELETE From [dbo].sysroPassports_PermissionsOverGroups Where IDPassport = @IDGROUP
INSERT INTO [dbo].sysroPassports_PermissionsOverGroups([IDPassport], [IDGroup] ,[IDApplication], [Permission])  select @IDGROUP, Groups.ID, 1,6 from Groups where CHARINDEX('\', Path) = 0
INSERT INTO [dbo].sysroPassports_PermissionsOverGroups([IDPassport], [IDGroup] ,[IDApplication], [Permission])  select @IDGROUP, Groups.ID, 2,6 from Groups where CHARINDEX('\', Path) = 0
INSERT INTO [dbo].sysroPassports_PermissionsOverGroups([IDPassport], [IDGroup] ,[IDApplication], [Permission])  select @IDGROUP, Groups.ID, 9,6 from Groups where CHARINDEX('\', Path) = 0
INSERT INTO [dbo].sysroPassports_PermissionsOverGroups([IDPassport], [IDGroup] ,[IDApplication], [Permission])  select @IDGROUP, Groups.ID, 11,6 from Groups where CHARINDEX('\', Path) = 0
INSERT INTO [dbo].sysroPassports_PermissionsOverGroups([IDPassport], [IDGroup] ,[IDApplication], [Permission])  select @IDGROUP, Groups.ID, 25,6 from Groups where CHARINDEX('\', Path) = 0
INSERT INTO [dbo].sysroPassports_PermissionsOverGroups([IDPassport], [IDGroup] ,[IDApplication], [Permission])  select @IDGROUP, Groups.ID, 31,6 from Groups where CHARINDEX('\', Path) = 0
INSERT INTO [dbo].sysroPassports_PermissionsOverGroups([IDPassport], [IDGroup] ,[IDApplication], [Permission])  select @IDGROUP, Groups.ID, 32,6 from Groups where CHARINDEX('\', Path) = 0

-- Rol sistema
DECLARE @IDRol int
SELECT @IDRol = (max(ID) + 1) from dbo.sysroGroupFeatures

INSERT INTO [dbo].sysroGroupFeatures (ID,Name) Values (@IDRol,'@@ROBOTICS@@System')

DELETE From [dbo].sysroGroupFeatures_PermissionsOverFeatures Where IDGroupFeature = @IDRol

INSERT INTO [dbo].sysroGroupFeatures_PermissionsOverFeatures (IDGroupFeature, IDFeature, Permision)
	SELECT @IDRol, sysroFeatures.ID, (CASE 
			WHEN CHARINDEX('A',PermissionTypes) > 0 THEN 9 
			WHEN CHARINDEX('W',PermissionTypes) > 0 THEN 6 
			WHEN CHARINDEX('R',PermissionTypes) > 0 THEN 3 
		End )
FROM sysroFeatures WHERE Type = 'U'

INSERT INTO sysroSecurityNode_Passports (IDSecurityNode,IDPassport,IDGroupFeature,LevelOfAuthority) 
SELECT 1,id,@IDRol,LevelOfAuthority  FROM sysroPassports
WHERE IDParentPassport IN (SELECT ID FROM sysroPassports where IDParentPassport  is null and Description like '%@@ROBOTICS@@GSystem%') 

END
GO

-- Reglas de solicitudes
INSERT INTO [dbo].[sysroRuleType] (ID, TYPE) VALUES(4,'AutomaticValidation')
GO

INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(7,4)
GO

ALTER TABLE [dbo].[Requests] ADD 
	[AutomaticValidation] [bit] NULL DEFAULT(0), [ValidationDate] [smalldatetime] NULL
GO

UPDATE Requests SET AutomaticValidation=0 WHERE AutomaticValidation IS NULL
GO

ALTER TABLE dbo.RequestsRules ADD [RuleCriteria] nvarchar(max) NULL
GO

INSERT INTO [dbo].[sysroRuleType] (ID, TYPE) VALUES(5,'LimitDateRequested')
GO

INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(7,5)
GO

INSERT INTO [dbo].[sysroRuleType] (ID, TYPE) VALUES(6,'MinimumDaysBeforeRequestedDate')
GO

INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(7,6)
GO

INSERT INTO [dbo].[sysroRuleType] (ID, TYPE) VALUES(7,'MaxNotScheduledDays')
GO

INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(7,7)
GO

INSERT INTO [dbo].[sysroRuleType] (ID, TYPE) VALUES(8,'MinCoverageRequiered')
GO

INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(7,8)
GO

INSERT INTO [dbo].[sysroScheduleRulesTypes]([ID],[Name],[System])
     VALUES (11,'MaxNotScheduled',0)
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='450' WHERE ID='DBVersion'
GO