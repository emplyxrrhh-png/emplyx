CREATE OR ALTER VIEW sysrovwAccruals_DailyAccrualsGrouped AS
WITH ConsecutiveDates AS (
    SELECT
        IDEmployee,
		IDConcept,
        Date,
        Value,
        CarryOver,
        StartupValue,
		ExpiredDate,
		StartEnjoymentDate,
		BeginPeriod,
		EndPeriod,
        DATEADD(DAY, -ROW_NUMBER() OVER (PARTITION BY IDEmployee, IDConcept ORDER BY IDemployee, IDConcept, Date), Date) AS GroupIdentifier
    FROM DailyAccruals
),
DailyValuesToGroup AS (
    SELECT
        IDEmployee,
		IDConcept,
        MIN(Date) AS Date,
        MAX(Date) AS EndDate,
        SUM(Value) AS TotalValue,
        GroupIdentifier,
		CarryOver,
		StartupValue,
		ExpiredDate,
		StartEnjoymentDate,
		BeginPeriod,
		EndPeriod
    FROM ConsecutiveDates
    WHERE CarryOver = 0 AND StartupValue = 0
    GROUP BY IDEmployee, IDConcept, GroupIdentifier, CarryOver, StartupValue, ExpiredDate, StartEnjoymentDate, BeginPeriod, EndPeriod
),
StartupOrCarryOverValues AS (
    SELECT
        IDEmployee,
		IDConcept,
        Date,
        NULL AS EndDate,
        Value AS TotalValue,
        NULL AS GroupIdentifier,
		CarryOver,
		StartupValue,
		ExpiredDate,
		StartEnjoymentDate,
		BeginPeriod,
		EndPeriod
    FROM DailyAccruals
	WHERE (CarryOver <> 0 OR StartupValue <> 0)
)
SELECT
    IDEmployee,
	IDConcept,
	CASE WHEN StartupValue = 1 AND CarryOver = 1 THEN 'StartupValue'
                                               ELSE CASE WHEN StartupValue = 0 AND CarryOver = 1 THEN 'ExpiredOrCarryOver'
	                                           ELSE 'DailyValue' END END [ValueType],
    Date,
	EndDate,
	ExpiredDate,
	StartEnjoymentDate,
	TotalValue AS Value,
	BeginPeriod,
	EndPeriod
FROM (
    SELECT * FROM DailyValuesToGroup
    UNION ALL
    SELECT * FROM StartupOrCarryOverValues
) AS DailyAccrualsGrouped
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='867' WHERE ID='DBVersion'

GO
