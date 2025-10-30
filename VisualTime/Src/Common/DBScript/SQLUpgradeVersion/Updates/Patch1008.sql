-- No borréis esta línea
CREATE OR ALTER VIEW sysrovwEmployeeUserFieldTypifiedValues AS
SELECT 
    e.IDEmployee,
    e.FieldName,
    e.Date AS StartDate,
    ISNULL(LEAD(e.Date) OVER (PARTITION BY e.IDEmployee, e.FieldName ORDER BY e.Date) - 1, CONVERT(SMALLDATETIME, '2079-01-01',120)) AS EndDate,
    e.Value AS RawValue,
    s.FieldType,
    CASE WHEN s.FieldType = 0 THEN e.Value END AS ValueText,
    CASE WHEN s.FieldType = 1 THEN TRY_CAST(CAST(e.Value AS VARCHAR(MAX)) AS INT) END AS ValueInt,
    CASE WHEN s.FieldType = 2 THEN TRY_CAST(CAST(e.Value AS VARCHAR(MAX)) AS DATE) END AS ValueDate,
    CASE WHEN s.FieldType = 3 THEN TRY_CAST(CAST(e.Value AS VARCHAR(MAX)) AS DECIMAL(18,4)) END AS ValueDecimal,
    CASE WHEN s.FieldType = 5 THEN e.Value END AS ValueList
FROM EmployeeUserFieldValues e
LEFT JOIN sysroUserfields s ON e.FieldName = s.FieldName
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1008' WHERE ID='DBVersion'
GO
