-- No borréis esta línea
ALTER TABLE dbo.Terminals ALTER column PunchStamp NVARCHAR(50)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='936' WHERE ID='DBVersion'
GO
