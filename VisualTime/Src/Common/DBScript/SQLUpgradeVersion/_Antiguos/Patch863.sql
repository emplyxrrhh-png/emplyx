-- No borréis esta línea
ALTER TABLE DailyAccruals ADD StartEnjoymentDate SMALLDATETIME
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='863' WHERE ID='DBVersion'

GO
