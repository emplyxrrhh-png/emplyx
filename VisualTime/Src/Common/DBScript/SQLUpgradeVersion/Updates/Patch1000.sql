ALTER TABLE PUNCHES
ADD Remarks NVARCHAR(MAX) NULL

GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1000' WHERE ID='DBVersion'
GO
