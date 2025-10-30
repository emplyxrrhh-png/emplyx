ALTER TABLE Documents
ADD DocumentExternalId NVARCHAR(256);
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='968' WHERE ID='DBVersion'
GO
