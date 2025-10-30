-- No borréis esta línea
UPDATE sysroNotificationTypes SET OnlySystem = 0 WHERE Id= 83
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1018' WHERE ID='DBVersion'
GO
