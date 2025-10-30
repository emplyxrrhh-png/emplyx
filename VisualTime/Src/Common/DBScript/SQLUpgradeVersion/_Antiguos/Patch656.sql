--Patch para actualizar informes NE

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='656' WHERE ID='DBVersion'
GO
