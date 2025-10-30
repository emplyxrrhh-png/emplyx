-- No borréis esta línea
CREATE OR ALTER FUNCTION [dbo].[sysrfnDailyEfectiveWorkingHours] (
    @IdEmployee INT,
    @ShiftDate DATE
)
RETURNS TABLE
AS
RETURN
(
    WITH PunchesWithAverages AS (
        SELECT
            P.IdEmployee,
            P.ShiftDate,
            P.DateTime,
            P.ActualType,
            P.TimeSpan,
            P.InTelecommute,
            Avg(CAST(P.ActualType AS DECIMAL(10,2))) OVER (
                PARTITION BY P.IdEmployee, P.ShiftDate
                ORDER BY P.DateTime ASC
                ROWS BETWEEN CURRENT ROW AND 1 FOLLOWING
            ) AS MediaSiguienteDelDia,
            Avg(CAST(P.ActualType AS DECIMAL(10,2))) OVER (
                PARTITION BY P.IdEmployee, P.ShiftDate
                ORDER BY P.DateTime ASC
                ROWS 1 PRECEDING
            ) AS MediaAnteriorDelDia,
            SUM(P.TimeSpan) OVER (
                PARTITION BY P.IdEmployee, P.ShiftDate
                ORDER BY P.DateTime ASC
                ROWS BETWEEN CURRENT ROW AND 1 FOLLOWING
            ) AS Value
        FROM PUNCHES P
        INNER JOIN Employees E ON E.Id = P.IdEmployee
        WHERE
            P.ActualType IN (1,2)
            AND P.IdEmployee = @IdEmployee
            AND P.ShiftDate = @ShiftDate
    )
    SELECT
        IdEmployee,
        ShiftDate AS Date,
        SUM(Value)/3600 AS Value,
        InTelecommute
    FROM PunchesWithAverages
    WHERE
        ActualType = 1
        AND MediaAnteriorDelDia <> 2
        AND MediaSiguienteDelDia = 1.5
    GROUP BY
        IdEmployee,
        ShiftDate,
        InTelecommute
)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1048' WHERE ID='DBVersion'
GO
