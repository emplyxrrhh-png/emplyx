ALTER TABLE EmployeeContracts ALTER COLUMN EndContractReason NVARCHAR(MAX) NULL
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='696' WHERE ID='DBVersion'
GO
