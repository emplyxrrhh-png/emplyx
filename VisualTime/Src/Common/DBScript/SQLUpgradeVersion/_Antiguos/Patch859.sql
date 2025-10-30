-- No borréis esta línea
CREATE OR ALTER FUNCTION dbo.sysrofnPastConsecutiveNonWorkingDaysFromDate (
    @IDemployee INT,
	@fromdate DATE,
	@todate DATE
)
RETURNS INT
AS
BEGIN
    DECLARE @quantity INT;

	WITH SortedDates AS (
		SELECT TOP 1 (Jump - 1) Jumped FROM (
		SELECT TOP 2 DailySchedule.IdEmployee, Date, LAG(DailySchedule.Date) OVER (ORDER BY DailySchedule.Date DESC) AS LastDate, DATEDIFF(DAY, Date, ISNULL(LAG(DailySchedule.Date) OVER (ORDER BY DailySchedule.Date DESC), @todate)) AS Jump
		FROM DailySchedule 
		INNER JOIN Shifts ON Shifts.ID = ISNULL(DailySchedule.IDShiftUsed, IDShift1)
		LEFT OUTER JOIN ProgrammedAbsences PA ON  PA.Idemployee = DailySchedule.IDEmployee AND PA.BeginDate <= DailySchedule.Date AND isnull(FinishDate, dateadd("d", MaxLastingDays, Begindate)) >= DailySchedule.Date 
		WHERE Shifts.ExpectedWorkingHours > 0
			AND Date BETWEEN @fromdate AND @todate 
			AND Dailyschedule.IDEmployee = @IDemployee
			AND PA.AbsenceID IS NULL
		) AUX WHERE Jump > 0	
    )
    SELECT @quantity = Jumped
    FROM SortedDates

    RETURN @quantity;
END;
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='859' WHERE ID='DBVersion'

GO
