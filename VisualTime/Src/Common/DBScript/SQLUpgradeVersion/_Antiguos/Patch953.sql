-- No borréis esta línea
CREATE TABLE TMPDetailedCalendarEmployee_DailyIncidences (
	[IDReportTask] [numeric](16, 0) NOT NULL,
    Fecha DATE NOT NULL,                 
    Tipo NVARCHAR(50) NOT NULL,        
    Nombre NVARCHAR(255),               
    NombreCorto NVARCHAR(100),          
    FechaDuplicada DATE NOT NULL,       
    Valor  [numeric](19, 6),                
    HoraInicio TIME,                    
    HoraFin TIME,                       
    IDEmpleado INT NOT NULL            
)
GO

CREATE OR ALTER PROCEDURE Report_Informe_Detallado_Usuario
	@p_begin DATETIME,
    @p_end DATETIME,
    @IDsEmp NVARCHAR(MAX),
	@IdReportTask [numeric](16, 0)
AS
BEGIN
    SET NOCOUNT ON;
	INSERT INTO TMPDetailedCalendarEmployee_DailyIncidences (
        Fecha, 
        Tipo, 
        Nombre, 
        NombreCorto, 
        FechaDuplicada, 
        Valor, 
        HoraInicio, 
        HoraFin, 
        IDEmpleado,
		IDReportTask
    )
    SELECT dc.Date AS Fecha, 'FICHAJES' AS Tipo, Causes.Name, Causes.ShortName, dc.date, dc.value, di.BeginTime, di.EndTime, dc.IDEmployee, @IdReportTask
    FROM DailyCauses dc
    INNER JOIN dbo.SplitMAX(@IDsEmp, ',') split ON split.Value = dc.IDEmployee
    INNER JOIN Punches p 
        ON p.ShiftDate = dc.Date 
        AND p.TypeData = dc.IDCause 
        AND p.TypeData <> 0 
        AND p.TypeData IS NOT NULL
    INNER JOIN DailyIncidences di 
        ON di.IDEmployee = dc.IDEmployee 
        AND di.ID = dc.IDRelatedIncidence 
        AND di.Date = dc.Date
    INNER JOIN Causes 
        ON Causes.ID = dc.IDCause
    WHERE dc.Manual = 0
		AND dc.Date BETWEEN @p_begin AND @p_end

    UNION
    SELECT dc.Date AS Fecha, 'HORAS' AS Tipo, Causes.Name, Causes.ShortName, dc.date, dc.value, di.BeginTime, di.EndTime, dc.IDEmployee, @IdReportTask
    FROM DailyCauses dc
    INNER JOIN dbo.SplitMAX(@IDsEmp, ',') split ON split.Value = dc.IDEmployee
    INNER JOIN ProgrammedCauses pc 
        ON pc.IDEmployee = dc.IDEmployee 
        AND pc.Date = dc.Date 
        AND pc.IDCause = dc.IDCause
    INNER JOIN DailyIncidences di 
        ON di.IDEmployee = dc.IDEmployee 
        AND di.ID = dc.IDRelatedIncidence 
        AND di.Date = dc.Date
    INNER JOIN Causes 
        ON Causes.ID = dc.IDCause
    WHERE dc.Manual = 0 
	    AND dc.Date BETWEEN @p_begin AND @p_end
    UNION
    SELECT dc.Date AS Fecha, 'DIAS' AS Tipo, Causes.Name, Causes.ShortName, dc.date, dc.value, di.BeginTime, di.EndTime, dc.IDEmployee, @IdReportTask
    FROM DailyCauses dc
    INNER JOIN dbo.SplitMAX(@IDsEmp, ',') split ON split.Value = dc.IDEmployee
    INNER JOIN (
        SELECT *, 
               CASE 
                   WHEN pa.MaxLastingDays = 0 THEN pa.FinishDate 
                   ELSE DATEADD(DAY, pa.MaxLastingDays, pa.BeginDate) 
               END AS EndDate
        FROM ProgrammedAbsences pa
    ) PaAux 
        ON PaAux.IDEmployee = dc.IDEmployee 
        AND dc.Date BETWEEN PaAux.BeginDate AND PaAux.EndDate 
        AND PaAux.IDCause = dc.IDCause
    INNER JOIN DailyIncidences di 
        ON di.IDEmployee = dc.IDEmployee 
        AND di.ID = dc.IDRelatedIncidence 
        AND di.Date = dc.Date
    INNER JOIN Causes 
        ON Causes.ID = dc.IDCause
    WHERE dc.Manual = 0
		AND dc.Date BETWEEN @p_begin AND @p_end
    UNION
    SELECT dc.Date AS Fecha, 'MANUAL' AS Tipo, Causes.Name, Causes.ShortName, dc.date, dc.value, di.BeginTime, di.EndTime, dc.IDEmployee, @IdReportTask
    FROM DailyCauses dc
    INNER JOIN dbo.SplitMAX(@IDsEmp, ',') split ON split.Value = dc.IDEmployee
    INNER JOIN Causes 
        ON Causes.ID = dc.IDCause
    LEFT JOIN DailyIncidences di 
        ON di.IDEmployee = dc.IDEmployee 
        AND di.ID = dc.IDRelatedIncidence 
        AND di.Date = dc.Date
    WHERE dc.Manual = 1 
      AND dc.IDCause <> 0 
	  AND dc.Date BETWEEN @p_begin AND @p_end
    UNION
    SELECT dc.Date AS Fecha, 'VACACIONES' AS Tipo, Causes.Name, Causes.ShortName, dc.date, dc.value, NULL AS BeginTime, NULL AS EndTime, dc.IDEmployee, @IdReportTask
    FROM DailyCauses dc
    INNER JOIN dbo.SplitMAX(@IDsEmp, ',') split ON split.Value = dc.IDEmployee
    INNER JOIN Shifts s 
        ON s.IDCauseHolidays = dc.IDCause 
        AND dc.IDCause <> 0
    INNER JOIN DailySchedule ds 
        ON ds.IDShiftUsed = s.ID 
        AND ds.IDEmployee = dc.IDEmployee 
        AND ds.Date = dc.Date
    INNER JOIN Causes 
        ON Causes.ID = dc.IDCause
    WHERE dc.Manual = 0
		AND dc.Date BETWEEN @p_begin AND @p_end
    UNION
    SELECT dc.Date AS Fecha, 'VACACIONESHORAS' AS Tipo, Causes.Name, Causes.ShortName, dc.date, dc.value, di.BeginTime, di.EndTime, dc.IDEmployee, @IdReportTask
    FROM DailyCauses dc
    INNER JOIN dbo.SplitMAX(@IDsEmp, ',') split ON split.Value = dc.IDEmployee
    INNER JOIN ProgrammedHolidays pc 
        ON pc.IDEmployee = dc.IDEmployee 
        AND pc.Date = dc.Date 
        AND pc.IDCause = dc.IDCause
    INNER JOIN DailyIncidences di 
        ON di.IDEmployee = dc.IDEmployee 
        AND di.ID = dc.IDRelatedIncidence 
        AND di.Date = dc.Date
    INNER JOIN Causes 
        ON Causes.ID = dc.IDCause
    WHERE dc.Manual = 0 
		AND dc.Date BETWEEN @p_begin AND @p_end
    UNION
    SELECT dc.Date AS Fecha, 'OVERTIMES' AS Tipo, Causes.Name, Causes.ShortName, dc.date, dc.value, di.BeginTime, di.EndTime, dc.IDEmployee, @IdReportTask
    FROM DailyCauses dc
    INNER JOIN dbo.SplitMAX(@IDsEmp, ',') split ON split.Value = dc.IDEmployee
    INNER JOIN ProgrammedOvertimes pc 
        ON pc.IDEmployee = dc.IDEmployee 
        AND pc.BeginDate = dc.Date 
        AND pc.IDCause = dc.IDCause
    INNER JOIN DailyIncidences di 
        ON di.IDEmployee = dc.IDEmployee 
        AND di.ID = dc.IDRelatedIncidence 
        AND di.Date = dc.Date
    INNER JOIN Causes 
        ON Causes.ID = dc.IDCause
    WHERE dc.Manual = 0 
		AND dc.Date BETWEEN @p_begin AND @p_end
option (maxrecursion 0, USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'))
END
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='953' WHERE ID='DBVersion'
GO
