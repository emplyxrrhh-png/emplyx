-- No borréis esta línea
UPDATE sysroDetectionQueries SET QueryText = 
    'WITH MaxPunches AS (
				@SELECT# 
					IDEmployee, 
					ShiftDate,
					MAX(CreatedOn) AS MaxPunchTimeStamp
				FROM punches
				WHERE ShiftDate > DATEADD(DAY, -15, CAST(GETDATE() AS DATE))
				  AND DATEDIFF(MINUTE, CreatedOn, GETDATE()) > 5
				  AND ActualType IN (1,2,13,7)
				GROUP BY IDEmployee, ShiftDate
			)
			@SELECT# 
				ds.IDEmployee,
				ds.TimestampEngine, 
				mp.MaxPunchTimeStamp,
				ds.Date
			FROM DailySchedule ds
			INNER JOIN MaxPunches mp ON mp.IDEmployee = ds.IDEmployee 
										AND mp.ShiftDate = ds.date
			WHERE ds.TimestampEngine <> CONVERT(SMALLDATETIME, ''1900-01-01'', 120)
				AND ds.Status = 70
				AND DATEDIFF(SECOND, ds.TimestampEngine, mp.MaxPunchTimeStamp) > 30 '
    WHERE Id = 1
GO

UPDATE ReportPlannedExecutions 
SET SCHEDULER = REPLACE(SCHEDULER, '+00:00:00', '') 
where Scheduler like '%+00:00:00'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='874' WHERE ID='DBVersion'

GO
