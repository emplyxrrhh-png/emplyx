
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

ALTER TABLE communiques ALTER COLUMN allowedresponses nvarchar(500);
GO

UPDATE sysroParameters SET Data='539' WHERE ID='DBVersion'
GO