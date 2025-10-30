--Update informesDX
-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='668' WHERE ID='DBVersion'
GO
