CREATE TABLE [dbo].[Collectives](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Collectives] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Collectives] ADD  DEFAULT ('') FOR [CreatedBy]
GO

ALTER TABLE [dbo].[Collectives] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO

CREATE TABLE [dbo].[CollectivesDefinitions](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[IDCollective] [int] NOT NULL,
	[Definition] [nvarchar](max) NOT NULL,
	[Filter] [nvarchar](max) NOT NULL,
	[BeginDate] [smalldatetime] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[ModifiedBy] [nvarchar](100) NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_CollectivesDefinitions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[CollectivesDefinitions] ADD  DEFAULT ('') FOR [ModifiedBy]
GO

ALTER TABLE [dbo].[CollectivesDefinitions] ADD  DEFAULT (getdate()) FOR [ModifiedDate]
GO

ALTER TABLE [dbo].[CollectivesDefinitions]  WITH CHECK ADD  CONSTRAINT [FK_CollectivesDefinitions_Collectives] FOREIGN KEY([IDCollective])
REFERENCES [dbo].[Collectives] ([ID])
GO

ALTER TABLE [dbo].[CollectivesDefinitions] CHECK CONSTRAINT [FK_CollectivesDefinitions_Collectives]
GO

--------------------------------------------------------------------------------------------------------
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
GO

CREATE OR ALTER PROCEDURE dbo.CheckCollectivesEmployees
(
    @StartDate DATETIME,
    @EndDate DATETIME = NULL,
    @CollectiveIDs NVARCHAR(MAX),
    @EmployeeID INT = NULL,  -- 0 = no filtrar por empleado, NULL/valor = filtrar por empleado específico
    @ErrorMessage NVARCHAR(MAX) OUTPUT,
    @HasErrors BIT OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
	SET @HasErrors = 0;
    SET @ErrorMessage = '';
    
    -- Si no se proporciona fecha final, usar la fecha inicial
    IF @EndDate IS NULL
        SET @EndDate = @StartDate;
    
    -- Considerar @EmployeeID = 0 como indicador para no filtrar por empleado
    IF @EmployeeID = 0
        SET @EmployeeID = NULL;
    
    -- Tabla para resultados
    CREATE TABLE #ResultEmployees (
        IDEmployee INT, 
        IDCollective INT,
        CollectiveDate DATETIME,
        IsInCollective BIT
    );
    
    -- Tabla para colectivos
    DECLARE @CollectivesTable TABLE (IDCollective INT);
    
    -- Separar IDs de colectivos
    INSERT INTO @CollectivesTable
    SELECT CAST(value AS INT) FROM SplitInt(@CollectiveIDs, ',');
    
    -- Procesar cada día del rango
    DECLARE @CurrentDate DATETIME = @StartDate;
    
    WHILE @CurrentDate <= @EndDate
    BEGIN
        -- Procesar cada colectivo para la fecha actual
        DECLARE @CurrentCollective INT;
        DECLARE CollectiveCursor CURSOR FOR SELECT IDCollective FROM @CollectivesTable;
        
        OPEN CollectiveCursor;
        FETCH NEXT FROM CollectiveCursor INTO @CurrentCollective;
        
        WHILE @@FETCH_STATUS = 0
        BEGIN
            DECLARE @HavingCondition NVARCHAR(MAX);
            
            -- Obtener condición específica para esta fecha y colectivo
            SELECT @HavingCondition = REPLACE(Filter, 'GETDATE()', '@RefDate') FROM (
                SELECT Filter,
                       BeginDate,
                       ISNULL(LEAD(BeginDate) OVER (PARTITION BY IDCollective ORDER BY BeginDate) - 1, 
                              CONVERT(SMALLDATETIME, '2079-01-01', 120)) AS EndDate
                FROM CollectivesDefinitions
                WHERE IDCollective = @CurrentCollective 
            ) AUX
            WHERE CAST(@CurrentDate AS DATE) BETWEEN AUX.BeginDate AND EndDate;
            
            -- Si existe una condición válida para esta fecha y colectivo
            IF @HavingCondition IS NOT NULL AND @HavingCondition <> ''
            BEGIN
                DECLARE @Sql NVARCHAR(MAX);
                
                -- Construir consulta según si se busca un empleado específico o todos
                IF @EmployeeID IS NOT NULL
                BEGIN
                    -- Verificar un empleado específico
                    SET @Sql = '
                    INSERT INTO #ResultEmployees (IDEmployee, IDCollective, CollectiveDate, IsInCollective)
                    SELECT @EmpID, @CollectiveID, @CurrDate,
                           CASE WHEN EXISTS (
                               SELECT 1 
                               FROM sysrovwEmployeeUserFieldTypifiedValues
                               INNER JOIN Employees ON Employees.ID = sysrovwEmployeeUserFieldTypifiedValues.IDEmployee
                               WHERE sysrovwEmployeeUserFieldTypifiedValues.IDEmployee = @EmpID
                               AND @RefDate BETWEEN StartDate AND EndDate
                               GROUP BY sysrovwEmployeeUserFieldTypifiedValues.IDEmployee
                               HAVING ' + @HavingCondition + '
                           ) THEN 1 ELSE 0 END';
                    BEGIN TRY
                        EXEC sp_executesql @Sql, 
                            N'@EmpID INT, @RefDate DATETIME, @CollectiveID INT, @CurrDate DATETIME', 
                            @EmployeeID, @CurrentDate, @CurrentCollective, @CurrentDate;
                    END TRY
                    BEGIN CATCH
                        -- Registrar error y continuar con el siguiente colectivo
						SET @HasErrors = 1;
						SET @ErrorMessage = @ErrorMessage + CHAR(13) + 'Error en fecha ' + CONVERT(VARCHAR, @CurrentDate, 120) + 
                              ' colectivo ' + CAST(@CurrentCollective AS VARCHAR) + 
                              ' empleado ' + CAST(@EmployeeID AS VARCHAR) +
                              ': ' + ERROR_MESSAGE();
                    END CATCH                    
                    EXEC sp_executesql @Sql, 
                        N'@EmpID INT, @RefDate DATETIME, @CollectiveID INT, @CurrDate DATETIME', 
                        @EmployeeID, @CurrentDate, @CurrentCollective, @CurrentDate;
                END
                ELSE
                BEGIN
                    -- Obtener todos los empleados del colectivo
                    SET @Sql = '
                    INSERT INTO #ResultEmployees (IDEmployee, IDCollective, CollectiveDate, IsInCollective)
                    SELECT sysrovwEmployeeUserFieldTypifiedValues.IDEmployee, @CollectiveID, @CurrDate, 1
                    FROM sysrovwEmployeeUserFieldTypifiedValues
                    INNER JOIN Employees ON Employees.ID = sysrovwEmployeeUserFieldTypifiedValues.IDEmployee
                    WHERE @RefDate BETWEEN StartDate AND EndDate
                    GROUP BY sysrovwEmployeeUserFieldTypifiedValues.IDEmployee
                    HAVING ' + @HavingCondition;
                    
                    BEGIN TRY
                        EXEC sp_executesql @Sql, 
                            N'@RefDate DATETIME, @CollectiveID INT, @CurrDate DATETIME', 
                            @CurrentDate, @CurrentCollective, @CurrentDate;
                    END TRY
                    BEGIN CATCH
                        -- Registrar error y continuar
						SET @HasErrors = 1;
						SET @ErrorMessage = @ErrorMessage + CHAR(13) + 'Error on date ' + CONVERT(VARCHAR, @CurrentDate, 120) + ': ' + ERROR_MESSAGE();
                    END CATCH
                END
            END
            ELSE IF @EmployeeID IS NOT NULL
            BEGIN
                -- Si no hay definición para esta fecha/colectivo y hay un empleado específico,
                -- registrar que no pertenece al colectivo
                INSERT INTO #ResultEmployees (IDEmployee, IDCollective, CollectiveDate, IsInCollective)
                VALUES (@EmployeeID, @CurrentCollective, @CurrentDate, 0);
            END
            
            FETCH NEXT FROM CollectiveCursor INTO @CurrentCollective;
        END
        
        CLOSE CollectiveCursor;
        DEALLOCATE CollectiveCursor;

        -- Si las fechas son iguales, salimos del bucle después de la primera iteración
        IF CAST(@StartDate AS DATE) = CAST(@EndDate AS DATE)
            BREAK;
        
        -- Avanzar al siguiente día
        SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
    END
    
	-- Devolver todos los datos detallados siempre
	SELECT 
		IDEmployee, 
		IDCollective, 
		CollectiveDate, 
		IsInCollective 
	FROM #ResultEmployees 
	ORDER BY IDEmployee, IDCollective, CollectiveDate;

	-- Limpiar
	DROP TABLE #ResultEmployees;
END;
GO

CREATE   PROCEDURE dbo.CheckCollectivesEmployees  
(  
    @StartDate DATETIME,  
    @EndDate DATETIME = NULL,  
    @CollectiveIDs NVARCHAR(MAX),  
    @EmployeeID INT = NULL,  -- 0 = no filtrar por empleado, NULL/valor = filtrar por empleado específico  
    @ErrorMessage NVARCHAR(MAX) OUTPUT,  
    @HasErrors BIT OUTPUT  
)  
AS  
BEGIN  
    SET NOCOUNT ON;  
    SET @HasErrors = 0;  
    SET @ErrorMessage = '';  
      
    -- Si no se proporciona fecha final, usar la fecha inicial  
    IF @EndDate IS NULL  
        SET @EndDate = @StartDate;  
      
    -- Considerar @EmployeeID = 0 como indicador para no filtrar por empleado  
    IF @EmployeeID = 0  
        SET @EmployeeID = NULL;  
      
    -- Tabla para resultados  
    CREATE TABLE #ResultEmployees (  
        IDEmployee INT,   
        IDCollective INT,  
        CollectiveDate DATETIME,  
        IsInCollective BIT  
    );  
      
    -- Tabla para colectivos  
    DECLARE @CollectivesTable TABLE (IDCollective INT);  
      
    -- Separar IDs de colectivos  
    INSERT INTO @CollectivesTable  
    SELECT CAST(value AS INT) FROM SplitInt(@CollectiveIDs, ',');  
      
    -- Procesar cada día del rango  
    DECLARE @CurrentDate DATETIME = @StartDate;  
    DECLARE @SameDates BIT = CASE WHEN CAST(@StartDate AS DATE) = CAST(@EndDate AS DATE) THEN 1 ELSE 0 END;  
      
    WHILE @CurrentDate <= @EndDate  
    BEGIN  
        -- Procesar cada colectivo para la fecha actual  
        DECLARE @CurrentCollective INT;  
        DECLARE CollectiveCursor CURSOR FOR SELECT IDCollective FROM @CollectivesTable;  
          
        OPEN CollectiveCursor;  
        FETCH NEXT FROM CollectiveCursor INTO @CurrentCollective;  
          
        WHILE @@FETCH_STATUS = 0  
        BEGIN  
            DECLARE @HavingCondition NVARCHAR(MAX);  
              
            -- Obtener condición específica para esta fecha y colectivo  
            SELECT @HavingCondition = REPLACE(Filter, 'GETDATE()', '@RefDate') FROM (  
                SELECT Filter,  
                       BeginDate,  
                       ISNULL(LEAD(BeginDate) OVER (PARTITION BY IDCollective ORDER BY BeginDate) - 1,   
                              CONVERT(SMALLDATETIME, '2079-01-01', 120)) AS EndDate  
                FROM CollectivesDefinitions  
                WHERE IDCollective = @CurrentCollective   
            ) AUX  
            WHERE CAST(@CurrentDate AS DATE) BETWEEN AUX.BeginDate AND EndDate;  
              
            -- Si existe una condición válida para esta fecha y colectivo  
            IF @HavingCondition IS NOT NULL AND @HavingCondition <> ''  
            BEGIN  
                DECLARE @Sql NVARCHAR(MAX);  
                  
                -- Construir consulta según si se busca un empleado específico o todos  
                IF @EmployeeID IS NOT NULL  
                BEGIN  
                    -- Verificar un empleado específico  
                    SET @Sql = '  
                    INSERT INTO #ResultEmployees (IDEmployee, IDCollective, CollectiveDate, IsInCollective)  
                    SELECT @EmpID, @CollectiveID, @CurrDate,  
                           CASE WHEN EXISTS (  
                               SELECT 1   
                               FROM sysrovwEmployeeUserFieldTypifiedValues  
                               INNER JOIN Employees ON Employees.ID = sysrovwEmployeeUserFieldTypifiedValues.IDEmployee  
                               WHERE sysrovwEmployeeUserFieldTypifiedValues.IDEmployee = @EmpID  
                               AND @RefDate BETWEEN StartDate AND EndDate  
                               GROUP BY sysrovwEmployeeUserFieldTypifiedValues.IDEmployee  
                               HAVING ' + @HavingCondition + '  
                           ) THEN 1 ELSE 0 END';  
                    BEGIN TRY  
                        EXEC sp_executesql @Sql,   
                            N'@EmpID INT, @RefDate DATETIME, @CollectiveID INT, @CurrDate DATETIME',   
                            @EmployeeID, @CurrentDate, @CurrentCollective, @CurrentDate;  
                    END TRY  
                    BEGIN CATCH  
                        -- Registrar error y continuar con el siguiente colectivo  
                        SET @HasErrors = 1;  
                        SET @ErrorMessage = @ErrorMessage + CHAR(13) + 'Error en fecha ' + CONVERT(VARCHAR, @CurrentDate, 120) +   
                              ' colectivo ' + CAST(@CurrentCollective AS VARCHAR) +   
                              ' empleado ' + CAST(@EmployeeID AS VARCHAR) +  
                              ': ' + ERROR_MESSAGE();  
                    END CATCH  
                END  
                ELSE  
                BEGIN  
                    -- Obtener todos los empleados del colectivo  
                    SET @Sql = '  
                    INSERT INTO #ResultEmployees (IDEmployee, IDCollective, CollectiveDate, IsInCollective)  
                    SELECT sysrovwEmployeeUserFieldTypifiedValues.IDEmployee, @CollectiveID, @CurrDate, 1  
                    FROM sysrovwEmployeeUserFieldTypifiedValues  
                    INNER JOIN Employees ON Employees.ID = sysrovwEmployeeUserFieldTypifiedValues.IDEmployee  
                    WHERE @RefDate BETWEEN StartDate AND EndDate  
                    GROUP BY sysrovwEmployeeUserFieldTypifiedValues.IDEmployee  
                    HAVING ' + @HavingCondition;  
                      
                    BEGIN TRY  
                        EXEC sp_executesql @Sql,   
                            N'@RefDate DATETIME, @CollectiveID INT, @CurrDate DATETIME',   
                            @CurrentDate, @CurrentCollective, @CurrentDate;  
                    END TRY  
                    BEGIN CATCH  
                        -- Registrar error y continuar  
                        SET @HasErrors = 1;  
                        SET @ErrorMessage = @ErrorMessage + CHAR(13) + 'Error on date ' + CONVERT(VARCHAR, @CurrentDate, 120) + ': ' + ERROR_MESSAGE();  
                    END CATCH  
                END  
            END  
            ELSE IF @EmployeeID IS NOT NULL  
            BEGIN  
                -- Si no hay definición para esta fecha/colectivo y hay un empleado específico,  
                -- registrar que no pertenece al colectivo  
                INSERT INTO #ResultEmployees (IDEmployee, IDCollective, CollectiveDate, IsInCollective)  
                VALUES (@EmployeeID, @CurrentCollective, @CurrentDate, 0);  
            END  
              
            FETCH NEXT FROM CollectiveCursor INTO @CurrentCollective;  
        END  
          
        CLOSE CollectiveCursor;  
        DEALLOCATE CollectiveCursor;  
  
        -- Si las fechas son iguales, salimos del bucle después de la primera iteración  
        IF @SameDates = 1  
            BREAK;  
          
        -- Avanzar al siguiente día  
        SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);  
    END  
      
    -- Devolver todos los datos detallados siempre  
    SELECT   
        IDEmployee,   
        IDCollective,   
        CollectiveDate,   
        IsInCollective   
    FROM #ResultEmployees   
    ORDER BY IDEmployee, IDCollective, CollectiveDate;  
  
    -- Limpiar  
    DROP TABLE #ResultEmployees;  
END;  