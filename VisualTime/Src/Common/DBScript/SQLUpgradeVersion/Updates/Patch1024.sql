-- No borréis esta línea
update sysroNotificationTypes set OnlySystem = 0 where id in (72,84)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1024' WHERE ID='DBVersion'
GO
