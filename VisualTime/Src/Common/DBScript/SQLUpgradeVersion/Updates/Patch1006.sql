ALTER TABLE PUNCHES
ADD NotReliableCause NVARCHAR(100)

GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1006' WHERE ID='DBVersion'
GO
