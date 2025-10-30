CREATE TABLE sysroDetectionQueries (
    Id SMALLINT PRIMARY KEY NOT NULL,
    QueryText NVARCHAR(MAX) NOT NULL,
	RunEvery SMALLINT NOT NULL,
	LastExecution SMALLDATETIME
);
GO

CREATE TABLE sysroCorrectionQueries (
    Id SMALLINT PRIMARY KEY,
    DetectionQueryId SMALLINT,
    QueryText NVARCHAR(MAX) NOT NULL,
    Parameters NVARCHAR(MAX) NOT NULL,
    EngineTask NVARCHAR(50),
	LastExecution SMALLDATETIME,
    FOREIGN KEY (DetectionQueryId) REFERENCES sysroDetectionQueries(Id)
);
GO

INSERT INTO sysroDetectionQueries (Id, QueryText, RunEvery, LastExecution)
VALUES (
    1,
    'WITH MaxPunches AS (
				@SELECT# 
					IDEmployee, 
					ShiftDate,
					MAX(CreatedOn) AS MaxPunchTimeStamp
				FROM punches
				WHERE ShiftDate > DATEADD(DAY, -15, CAST(GETDATE() AS DATE))
				  AND DATEDIFF(MINUTE, CreatedOn, GETDATE()) > 5
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
				AND ds.TimestampEngine < mp.MaxPunchTimeStamp ',
    1,  
    NULL
);
GO

INSERT INTO sysroCorrectionQueries (Id, DetectionQueryId, QueryText, Parameters, EngineTask)
VALUES (
    1,  
    1,  
    '@UPDATE# DailySchedule SET Status = 0 WHERE Date = @date AND IDEmployee = @idemployee',
    '@date = Date(Date ), @idemployee = IDEmployee ( int)','dailyschedule'
);
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='868' WHERE ID='DBVersion'

GO
