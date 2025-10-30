ALTER TABLE Terminals
ADD Deleted BIT DEFAULT 0;
GO

UPDATE Terminals SET Deleted = 0 where Deleted IS NULL;
GO
-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1004' WHERE ID='DBVersion'
GO
