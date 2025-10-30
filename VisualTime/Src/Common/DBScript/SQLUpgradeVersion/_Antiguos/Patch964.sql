ALTER TABLE sysrolivetasks ADD TraceGroup nvarchar(50) null
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='964' WHERE ID='DBVersion'
GO
