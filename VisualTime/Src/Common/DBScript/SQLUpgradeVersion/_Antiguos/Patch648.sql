ALTER TABLE GROUPS
ADD Export VARCHAR(100) NULL

GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='648' WHERE ID='DBVersion'
GO
