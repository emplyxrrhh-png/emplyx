ALTER TABLE SHIFTS DROP COLUMN VisibilityCollectives

GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1022' WHERE ID='DBVersion'
GO
