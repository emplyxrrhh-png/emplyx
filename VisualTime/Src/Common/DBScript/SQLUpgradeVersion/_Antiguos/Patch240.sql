/* Introducimos los campos de la SysroDailyIncidencesDescription necesarios para lanzar el informe de los que más */
INSERT INTO SysroDailyIncidencesDescription values (0,'')	
GO
INSERT INTO SysroDailyIncidencesDescription values (1001,'Horas trabajadas')
GO
INSERT INTO SysroDailyIncidencesDescription values (1010,'Horas extras')
GO
INSERT INTO SysroDailyIncidencesDescription values (1011,'Ausencia')
GO
INSERT INTO SysroDailyIncidencesDescription values (1020,'Retraso')
GO
INSERT INTO SysroDailyIncidencesDescription values (1021,'Interrupción')
GO
INSERT INTO SysroDailyIncidencesDescription values (1022,'Salida anticipada')
GO
INSERT INTO SysroDailyIncidencesDescription values (1030,'Horas extras en flexibles')
GO
INSERT INTO SysroDailyIncidencesDescription values (1031,'Ausencia en horas flexibles')
GO
INSERT INTO SysroDailyIncidencesDescription values (1041,'Descanso demasiado largo')
GO
INSERT INTO SysroDailyIncidencesDescription values (1042,'Descanso demasiado corto')
GO
INSERT INTO SysroDailyIncidencesDescription values (1050,'Diferencia positiva')
GO
INSERT INTO SysroDailyIncidencesDescription values (1051,'Diferencia negativa')
GO

/* ***************************************************************************************************************************** */
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='240' WHERE ID='DBVersion'
GO
