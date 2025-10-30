-- No borréis esta línea
ALTER TABLE Causes
ALTER COLUMN Name NVARCHAR(100);
GO

ALTER TABLE sysroQueries
ALTER COLUMN Name NVARCHAR(200);
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='910' WHERE ID='DBVersion'
GO
