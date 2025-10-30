-- No borréis esta línea
UPDATE sysronotificationtypes SET Feature = '*Documents*' WHERE Feature = 'Documents'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='778' WHERE ID='DBVersion'
GO
