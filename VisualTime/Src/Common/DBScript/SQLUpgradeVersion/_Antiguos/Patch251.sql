/* ***************************************************************************************************************************** */
/* Actualizamos el nivel de mando a 1 para los grupos de passports que tengan el campo LevelOfAuthority a NULL. */
UPDATE sysroPassports
SET LevelOfAuthority = 1
WHERE GroupType = 'U' AND LevelOfAuthority IS NULL

GO

/* ***************************************************************************************************************************** */
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='251' WHERE ID='DBVersion'
GO
