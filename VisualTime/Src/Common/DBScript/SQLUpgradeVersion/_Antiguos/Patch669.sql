-- No borréis esta línea
UPDATE dbo.sysropassports_authenticationMethods SET LastUpdatePassword = null where Credential like '\%'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='669' WHERE ID='DBVersion'
GO
