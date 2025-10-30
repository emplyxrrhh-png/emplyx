-- No borréis esta línea
UPDATE sysroFeatures SET PermissionTypes = 'W' WHERE ID = 1800
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='679' WHERE ID='DBVersion'
GO
