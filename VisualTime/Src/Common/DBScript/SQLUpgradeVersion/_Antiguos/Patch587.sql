IF EXISTS (SELECT * FROM sysroGUI WHERE Edition  = 'Starter')
UPDATE Causes SET VisibilityPermissions = 0  WHERE Name = 'Horas extraordinarias' OR Name = 'Tiempo recuperado'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO
UPDATE sysroParameters SET Data='587' WHERE ID='DBVersion'
GO
