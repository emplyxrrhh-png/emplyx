
ALTER TABLE Causes ADD RoundingByDailyScope BIT NOT NULL DEFAULT 0
GO

ALTER TABLE ProgrammedAbsences ADD Description NVARCHAR(255)
GO

UPDATE sysroParameters SET Data='124' WHERE ID='DBVersion'
GO




