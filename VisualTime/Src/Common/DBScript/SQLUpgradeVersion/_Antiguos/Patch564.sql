IF EXISTS (SELECT * FROM sysroGUI WHERE Edition  = 'Starter')
UPDATE sysroGUI SET Edition = 'Starter' WHERE IDPath = 'Portal\Configuration\Terminal'
GO
IF EXISTS (SELECT * FROM sysroGUI WHERE Edition  = 'Starter')
UPDATE sysroGUI SET Edition = 'Starter' WHERE IDPath = 'Portal\Reports\Genius'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='564' WHERE ID='DBVersion'
GO