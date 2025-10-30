-- Actualizamos el flag de seguridad de la pantalla de Analitica de datos
UPDATE sysroGui SET SecurityFlags = SecurityFlags + '111111111111111111111111111111' WHERE LEN(SecurityFlags) = 10 AND LanguageReference='Analytics'
GO
-- Actualizamos la tabla de auditoria con nuevos elementos
INSERT INTO sysroAuditElements VALUES (44,'ShiftsGroups',NULL)
GO
INSERT INTO sysroAuditElements VALUES (45,'AnnualLimits',NULL)
GO
INSERT INTO sysroAuditElements VALUES (46,'PairedMoves','Emparejador de fichajes')
GO
INSERT INTO sysroAuditElements VALUES (47,'PunchIn','Fichajes de entrada de presencia')
GO
INSERT INTO sysroAuditElements VALUES (48,'PunchOut','Fichajes de salida de presencia') 
GO
/* ***************************************************************************************************************************** */
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='229' WHERE ID='DBVersion'
GO
