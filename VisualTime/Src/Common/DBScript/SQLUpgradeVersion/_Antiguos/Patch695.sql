-- No borréis esta línea
ALTER TABLE [dbo].[DailyAccruals] ADD [BeginPeriod] [smalldatetime] NULL, [EndPeriod] [smalldatetime] NULL
GO

CREATE VIEW [dbo].[sysrovwEmployeesAnnualWorkPeriods]	
AS
WITH CTE_Fechas AS (
        SELECT IDEmployee, Begindate AS BeginPeriod, EndDate FROM EmployeeContracts  WITH (NOLOCK)
        UNION ALL
        SELECT IDEmployee, DATEADD(YEAR, 1, BeginPeriod), EndDate
        FROM CTE_Fechas 
        WHERE DATEADD(YEAR, 1, BeginPeriod) <= CASE WHEN EndDate = '20790101' THEN  DATEFROMPARTS(year(getdate())+2,12,31) ELSE EndDate END
    )
    SELECT IDEmployee, BeginPeriod, CASE WHEN DATEADD(DAY,-1,DATEADD(YEAR, 1, BeginPeriod)) > EndDate THEN EndDate ELSE DATEADD(DAY,-1,DATEADD(YEAR, 1, BeginPeriod)) END as EndPeriod
    FROM CTE_Fechas  WITH (NOLOCK)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='695' WHERE ID='DBVersion'
GO
