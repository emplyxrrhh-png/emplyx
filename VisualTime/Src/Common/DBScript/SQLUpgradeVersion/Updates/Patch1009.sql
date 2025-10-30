-- No borréis esta línea
ALTER TABLE CollectivesDefinitions DROP COLUMN EndDate
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1009' WHERE ID='DBVersion'
GO
