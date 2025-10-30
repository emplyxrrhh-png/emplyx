-- No borréis esta línea
CREATE OR ALTER PROCEDURE dbo.GetCollectiveEmployees
(
    @ReferenceDate DATETIME,
    @IdCollective INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @HavingCondition NVARCHAR(MAX);

	SELECT @HavingCondition = REPLACE(Filter, 'GETDATE()', '@RefDate') FROM (
	SELECT Filter,
		   BeginDate,
		   ISNULL(LEAD(CollectivesDefinitions.BeginDate) OVER (PARTITION BY CollectivesDefinitions.IDCollective ORDER BY CollectivesDefinitions.BeginDate) - 1, CONVERT(SMALLDATETIME, '2079-01-01',120)) AS EndDate
	FROM CollectivesDefinitions
	WHERE IDCollective = @IdCollective 
	) AUX
	WHERE CAST(@ReferenceDate AS DATE) BETWEEN AUX.BeginDate AND EndDate
	
    -- Construcción de la consulta dinámica
    DECLARE @Sql NVARCHAR(MAX) = '
    SELECT sysrovwEmployeeUserFieldTypifiedValues.IDEmployee
    FROM sysrovwEmployeeUserFieldTypifiedValues
    INNER JOIN Employees ON Employees.ID = sysrovwEmployeeUserFieldTypifiedValues.IDEmployee
    WHERE @RefDate BETWEEN StartDate AND EndDate
    GROUP BY sysrovwEmployeeUserFieldTypifiedValues.IDEmployee
    HAVING ' + @HavingCondition + '
    ORDER BY sysrovwEmployeeUserFieldTypifiedValues.IDEmployee;';

    EXEC sp_executesql @Sql, N'@RefDate DATETIME', @ReferenceDate;
END;

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1017' WHERE ID='DBVersion'
GO