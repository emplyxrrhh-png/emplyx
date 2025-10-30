update sysroGUI set RequiredFeatures = 'Feature\Collectives' where IDPath = 'Portal\Company\Collectives' AND URL = '/Collectives'
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1014' WHERE ID='DBVersion'
GO
