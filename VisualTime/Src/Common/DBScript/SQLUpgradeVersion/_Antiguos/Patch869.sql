-- No borréis esta línea
ALTER TABLE sysroDetectionQueries ADD Active BIT NOT NULL DEFAULT 1
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='869' WHERE ID='DBVersion'

GO
