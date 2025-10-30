-- No borréis esta línea
ALTER TABLE StartupValues ADD EnjoymentUnit TINYINT
GO
ALTER TABLE StartupValues ADD StartEnjoymentValue INT
GO
ALTER TABLE StartupValues ADD ExpirationUnit TINYINT
GO
ALTER TABLE StartupValues ADD ExpirationValue INT
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='861' WHERE ID='DBVersion'

GO
