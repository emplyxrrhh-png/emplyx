--Grupos
update [dbo].[sysroGroupFeatures_PermissionsOverFeatures] 
	set Permision = 9 
where IDGroupFeature = 0 and IDFeature = 36
GO

update [dbo].[sysroGroupFeatures_PermissionsOverFeatures] 
	set Permision = 9 
where IDGroupFeature = 0 and IDFeature = 36000
GO

--Calculos
update [dbo].sysroPermissionsOverFeatures 
	set Permission = 9 
where PassportID in (SELECT IDParentPassport from sysroPassports where IDGroupFeature = 0)
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='727' WHERE ID='DBVersion'
GO
