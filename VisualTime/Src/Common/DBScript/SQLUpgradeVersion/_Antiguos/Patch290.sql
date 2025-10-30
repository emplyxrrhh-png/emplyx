INSERT INTO sysroNotificationTypes ([ID],[Name],[Scheduler])
VALUES (18, 'Concept Not Reached', 60)
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='290' WHERE ID='DBVersion'
GO
