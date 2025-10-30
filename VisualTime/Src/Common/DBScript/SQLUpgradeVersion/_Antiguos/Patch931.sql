CREATE OR ALTER FUNCTION [dbo].[GetHolidayPeriods]    
    (      
     @startDate smalldatetime,    
     @endDate smalldatetime,
	 @idEmployee int,
	 @holidayIds nvarchar(max) 
    )    
    RETURNS nvarchar(max)    
    AS    
    BEGIN    

	
	Declare @pstartDate as smalldatetime = @startDate,
			@pendDate as smalldatetime = @endDate,
			@pidEmployee as int = @idEmployee,
			@pholidayIds as nvarchar(max) = @holidayIds


	Declare @retValue as nvarchar(max);
	DECLARE @shiftIDs Table(id int);
	INSERT INTO @shiftIDs(id) select * from dbo.SplitToInt(@pholidayIds,',');  


	WITH CTE AS (
		SELECT
        Date,
        isHolidays,
        DATEADD(DAY, -ROW_NUMBER() OVER (ORDER BY Date), Date) AS Grupo
		FROM DailySchedule
		WHERE isHolidays = 1 and IDEmployee = @pidEmployee
		  AND Date between @pstartDate and @pendDate and IDShift1 in (select id from @shiftIDs)
	)
	, Periodos AS (
		SELECT 
			MIN(Date) AS FechaInicio, 
			MAX(Date) AS FechaFin
		FROM CTE
		GROUP BY Grupo
	)
	-- Concatenar los periodos en una sola cadena
	SELECT @retValue = STRING_AGG(
		CASE 
			WHEN FechaInicio = FechaFin THEN CONVERT(VARCHAR, FORMAT(FechaInicio, 'dd/MM')) 
			ELSE CONVERT(VARCHAR, FORMAT(FechaInicio, 'dd/MM')) + ' a ' + CONVERT(VARCHAR, FORMAT(FechaFin, 'dd/MM'))
		END, '; ')
	FROM Periodos;


	RETURN @retValue    
    END
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='931' WHERE ID='DBVersion'
GO
