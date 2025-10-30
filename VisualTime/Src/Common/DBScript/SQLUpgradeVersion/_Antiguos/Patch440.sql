ALTER TABLE dbo.tmpEmergencyBasic DROP CONSTRAINT PK_TMPEmergencyBasic
GO

UPDATE dbo.sysroParameters SET Data='440' WHERE ID='DBVersion'
GO

