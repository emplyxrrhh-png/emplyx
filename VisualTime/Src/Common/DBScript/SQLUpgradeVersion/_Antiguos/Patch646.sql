update sysrogui set IconURL = 'SecurityFunctions.png' where URL like 'Security/AdvancedSecurity.aspx'
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='646' WHERE ID='DBVersion'
GO
