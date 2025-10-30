--Nuevo campo para tareas, poder asignar una tarea a un terminal
ALTER TABLE JOBS ADD  [IDTerminal] [tinyint] NULL DEFAULT (0)
GO
UPDATE JOBS SET IDTerminal = 0 WHERE IDTerminal is null
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='215' WHERE ID='DBVersion'
GO
