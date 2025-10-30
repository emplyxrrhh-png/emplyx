INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxb',1,1,'TA',1,'Blind','E,S,X','Local','1,0',0,0,'1,0','0','Remote')
GO

INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxb',1,2,'ACC',1,'Blind','E,S,X','Local','1,0',0,0,'1','0','Remote')
GO

INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxb',1,3,'ACCTA',1,'Blind','E,S,X','Local','1,0',0,0,'1','0','Remote')
GO

--Funcionalidad Grupos de Negocio

ALTER TABLE [ShiftGroups] ADD [BusinessGroup] [nvarchar](50) default ''
GO

UPDATE ShiftGroups SET BusinessGroup = ''
GO

ALTER TABLE [sysroPassports] ADD [BusinessGroupList] [nvarchar](300) default ''
GO

UPDATE sysroPassports SET BusinessGroupList = ''
GO

DELETE FROM sysroFeatures WHERE ID = 2011
GO
DELETE FROM sysroFeatures WHERE ID = 2012
GO
DELETE FROM sysroFeatures WHERE ID = 2013
GO
DELETE FROM sysroFeatures WHERE ID = 2014
GO
DELETE FROM sysroFeatures WHERE ID = 2015
GO
DELETE FROM sysroFeatures WHERE ID = 2016
GO
DELETE FROM sysroFeatures WHERE ID = 2017
GO
DELETE FROM sysroFeatures WHERE ID = 2018
GO
DELETE FROM sysroFeatures WHERE ID = 2019
GO
DELETE FROM sysroFeatures WHERE ID = 2020
GO
DELETE FROM sysroFeatures WHERE ID = 2021
GO
DELETE FROM sysroFeatures WHERE ID = 2022
GO

DELETE FROM sysroPassports_PermissionsOverFeatures WHERE IDFeature = 2011
GO
DELETE FROM sysroPassports_PermissionsOverFeatures WHERE IDFeature = 2012
GO
DELETE FROM sysroPassports_PermissionsOverFeatures WHERE IDFeature = 2013
GO
DELETE FROM sysroPassports_PermissionsOverFeatures WHERE IDFeature = 2014
GO
DELETE FROM sysroPassports_PermissionsOverFeatures WHERE IDFeature = 2015
GO
DELETE FROM sysroPassports_PermissionsOverFeatures WHERE IDFeature = 2016
GO
DELETE FROM sysroPassports_PermissionsOverFeatures WHERE IDFeature = 2017
GO
DELETE FROM sysroPassports_PermissionsOverFeatures WHERE IDFeature = 2018
GO
DELETE FROM sysroPassports_PermissionsOverFeatures WHERE IDFeature = 2019
GO
DELETE FROM sysroPassports_PermissionsOverFeatures WHERE IDFeature = 2020
GO
DELETE FROM sysroPassports_PermissionsOverFeatures WHERE IDFeature = 2021
GO
DELETE FROM sysroPassports_PermissionsOverFeatures WHERE IDFeature = 2022
GO





-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='273' WHERE ID='DBVersion'
GO
