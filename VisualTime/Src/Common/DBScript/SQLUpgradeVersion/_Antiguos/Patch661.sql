ALTER TABLE GeniusExecutions ADD SASLink nvarchar(max);
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='661' WHERE ID='DBVersion'
GO
