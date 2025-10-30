--
-- Actualizamos la informacion de acumulados de ocurrencias
--

if exists(SELECT * FROM shifts where name like 'Pack 1')
	Insert Into sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission) VALUES (3,8100,9)
GO
if exists(SELECT * FROM shifts where name like 'Pack 1')
	Insert Into sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission) VALUES (3,2300,9)
GO
if exists(SELECT * FROM shifts where name like 'Pack 1')
	Insert Into sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission) VALUES (3,2310,9)
GO
if exists(SELECT * FROM shifts where name like 'Pack 1')
	Insert Into sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission) VALUES (3,2320,9)
GO
if exists(SELECT * FROM shifts where name like 'Pack 1')
	Insert Into sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission) VALUES (3,2321,9)
GO
if exists(SELECT * FROM shifts where name like 'Pack 1')
	Insert Into sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission) VALUES (3,2322,9)
GO
if exists(SELECT * FROM shifts where name like 'Pack 1')
	Insert Into sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission) VALUES (3,2323,9)
GO

if exists(SELECT * FROM shifts where name like 'Pack 1')
	update sysroPassports_PermissionsOverFeatures Set Permission = 3 where IDPassport=3 and IDFeature= 8000
GO


if exists(SELECT * FROM shifts where name like 'Pack 1')
	Update conceptcauses set Composition = '<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item><Item key="FactorType" type="2">0</Item></roCollection>' where idconcept in(27,50,56) and composition is null
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='263' WHERE ID='DBVersion'
GO
