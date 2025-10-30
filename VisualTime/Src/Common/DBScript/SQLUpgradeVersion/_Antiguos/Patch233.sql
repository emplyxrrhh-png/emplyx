--ACTUALIZAMOS LA LICENCIA DE LA ANALITICA DE DATOS
UPDATE sysrogui SET RequiredFeatures='Forms\Calendar' WHERE URL LIKE '%Analytics%'
GO

/* ***************************************************************************************************************************** */

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='233' WHERE ID='DBVersion'
GO
