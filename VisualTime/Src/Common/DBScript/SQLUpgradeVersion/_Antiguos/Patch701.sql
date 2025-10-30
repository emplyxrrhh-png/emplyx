ALTER TABLE ReportExecutions ALTER COLUMN ExecutionDate datetime;
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='701' WHERE ID='DBVersion'
GO
