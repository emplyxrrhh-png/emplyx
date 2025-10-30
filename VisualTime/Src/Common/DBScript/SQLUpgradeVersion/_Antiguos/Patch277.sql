Update sysroNotificationTypes
SET Name = 'Before Begin Programmed Absence' WHERE ID=3

Update sysroNotificationTypes
SET Name = 'Finish Programmed Absence' WHERE ID=6

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='277' WHERE ID='DBVersion'
GO
