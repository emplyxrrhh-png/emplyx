update sysroGUI set RequiredFeatures = '!Version\CegidHub' where IDPath IN ('Portal\Communiques');

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='684' WHERE ID='DBVersion'
GO
