DROP PROCEDURE IF EXISTS [dbo].[Report_saldos_con_detalle_total]
GO
CREATE PROCEDURE [dbo].[Report_saldos_con_detalle_total]
    @idPassport nvarchar(100) = '1',
    @IDEmployee nvarchar(max) = '0',
    @IDConcept nvarchar(max) = '0',
    @ContractID nvarchar(max) = '0',
    @GroupId smallint = '0',
    @FechaInicio date = '20210301',
    @FechaFin date = '20210301'
AS
BEGIN
    -- Crear una tabla temporal para almacenar los IDs de los conceptos con su orden
    CREATE TABLE #ConceptOrder (ConceptID INT, RowNum INT);

    INSERT INTO #ConceptOrder (ConceptID, RowNum)
    SELECT Value, ROW_NUMBER() OVER (ORDER BY (SELECT NULL))
    FROM split(@IDConcept, ',');
    
    SELECT 
        cs.Name AS ConceptName,
        cs.ID AS ConceptID,
        SUM(ds.Value) AS ConceptValue,
        co.RowNum
    FROM DailyAccruals ds
    INNER JOIN (SELECT * FROM splitMAX(@IDEmployee, ',')) tmp ON ds.IDEmployee = tmp.Value
    INNER JOIN sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) 
        ON poe.IDPassport = @idPassport 
        AND poe.IDEmployee = ds.IDEmployee 
        AND CAST(ds.Date AS date) BETWEEN poe.BeginDate AND poe.EndDate 
    INNER JOIN Employees emp ON emp.ID = ds.IDEmployee
    INNER JOIN EmployeeContracts ec ON ec.IDEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN ec.BeginDate AND ec.EndDate
    INNER JOIN EmployeeGroups eg ON eg.IDEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN eg.BeginDate AND eg.EndDate
    INNER JOIN Groups g ON eg.IDGroup = g.ID
    LEFT JOIN Concepts cs ON ds.IDConcept = cs.ID
    LEFT JOIN #ConceptOrder co ON ds.IDConcept = co.ConceptID
    WHERE ds.Date BETWEEN @FechaInicio AND @FechaFin 
        AND ec.IDContract = @ContractID 
        AND eg.IDGroup = @GroupId  
        AND ds.IDConcept IN (SELECT Value FROM split(@IDConcept, ','))
    GROUP BY cs.Name, cs.ID, co.RowNum
    ORDER BY co.RowNum;
    
    DROP TABLE #ConceptOrder;
END
GO

DROP PROCEDURE IF EXISTS [dbo].[Report_saldos_con_detalle]
GO
CREATE PROCEDURE [dbo].[Report_saldos_con_detalle]
    @idPassport nvarchar(100) = '1',
    @IDEmployee nvarchar(max) = '0',
    @IDConcept nvarchar(max) = '0',
    @FechaInicio date = '20210301',
    @FechaFin date = '20210301'
AS
BEGIN
  
    CREATE TABLE #ConceptOrder (ConceptID INT, RowNum INT);

  
    INSERT INTO #ConceptOrder (ConceptID, RowNum)
    SELECT Value, ROW_NUMBER() OVER (ORDER BY (SELECT NULL))
    FROM split(@IDConcept, ',');

  
    SELECT 
        emp.ID AS EmployeeID, 
        emp.Name AS EmployeeName, 
        ec.IDContract as Contract, 
        g.Id as IDGroup, 
        g.Name as GroupName, 
        g.FullGroupName as Ruta, 
        ds.Date, 
        cs.Name as ConceptName, 
        cs.ID as ConceptID, 
        ds.Value 
    FROM DailyAccruals ds 
    INNER JOIN (SELECT * FROM splitMAX(@IDEmployee, ',')) tmp ON ds.IDEmployee = tmp.Value
    INNER JOIN sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) 
        ON poe.IDPassport = @idPassport 
        AND poe.IDEmployee = ds.IDEmployee 
        AND CAST(ds.Date AS date) BETWEEN poe.BeginDate AND poe.EndDate 
    INNER JOIN Employees emp ON emp.ID = ds.IDEmployee 
    INNER JOIN EmployeeContracts ec ON ec.IDEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN ec.BeginDate AND ec.EndDate 
    INNER JOIN EmployeeGroups eg ON eg.IDEmployee = ds.IDEmployee 
        AND ds.Date BETWEEN eg.BeginDate AND eg.EndDate 
    INNER JOIN Groups g ON eg.IDGroup = g.ID
    LEFT JOIN Concepts cs ON ds.IDConcept = cs.ID 
    LEFT JOIN #ConceptOrder co ON ds.IDConcept = co.ConceptID
    WHERE CAST(ds.Date AS date) BETWEEN CAST(@FechaInicio AS date) AND CAST(@FechaFin AS date) 
        AND ds.IDConcept IN (SELECT Value FROM split(@IDConcept, ','))
    ORDER BY co.RowNum;

  
    DROP TABLE #ConceptOrder;
END
GO


-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='907' WHERE ID='DBVersion'
GO
