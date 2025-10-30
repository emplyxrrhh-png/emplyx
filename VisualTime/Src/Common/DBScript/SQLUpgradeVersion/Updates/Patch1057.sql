UPDATE SYSROGUI SET RequiredFeatures = 'Feature\Collectives' WHERE IDPath = 'Portal\Company\Collectives'
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1057' WHERE ID='DBVersion'
GO
