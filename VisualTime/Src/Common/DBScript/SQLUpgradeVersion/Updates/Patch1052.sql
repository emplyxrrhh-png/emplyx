UPDATE SYSROGUI SET PARAMETERS = NULL WHERE IDPath = 'Portal\Company\Collectives'
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1052' WHERE ID='DBVersion'
GO
