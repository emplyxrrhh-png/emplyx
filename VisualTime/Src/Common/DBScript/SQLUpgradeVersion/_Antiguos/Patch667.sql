--Actualización informes DX
-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='667' WHERE ID='DBVersion'
GO
