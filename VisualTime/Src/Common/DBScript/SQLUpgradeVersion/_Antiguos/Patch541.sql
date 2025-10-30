IF EXISTS (SELECT * FROM sysroGUI WHERE Edition  = 'Starter')
UPDATE sysroGUI SET Edition = 'Starter' WHERE IDPath = 'Portal\Configuration\Terminal'
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='541' WHERE ID='DBVersion'
GO