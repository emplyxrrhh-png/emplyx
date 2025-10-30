DROP TRIGGER IF EXISTS hotfixBiometry
GO
-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='864' WHERE ID='DBVersion'

GO
