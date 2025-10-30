
INSERT INTO [sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (9220,9200,'Access.Zones.Supervision','Supervisión','','U','R',0)
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT IDPassport AS IDPassport, 9220 AS IDFeature, 3 AS Permission
FROM sysroPassports_PermissionsOverFeatures
WHERE sysroPassports_PermissionsOverFeatures.IDFeature = 9200
GO


--Eliminar relacion
ALTER TABLE RequestsApprovals DROP CONSTRAINT FK_RequestsApprovals_sysroPassports
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='274' WHERE ID='DBVersion'
GO
