ALTER TABLE Collectives 
ALTER COLUMN [Description] [nvarchar](4000)
GO

-- No borréis esta línea


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1020' WHERE ID='DBVersion'
GO

