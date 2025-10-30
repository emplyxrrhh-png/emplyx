-- No borréis esta línea
ALTER TABLE dbo.TMPHOLIDAYSCONTROLByContract ALTER column StartupValue numeric(16,4) NOT NULL 
GO

ALTER TABLE dbo.TMPHOLIDAYSCONTROLByContract ALTER column CarryOver numeric(16,4) NOT NULL 
GO

ALTER TABLE dbo.TMPHOLIDAYSCONTROLByContract ALTER column EnjoyedDays numeric(16,4) NOT NULL 
GO

ALTER TABLE dbo.TMPHOLIDAYSCONTROLByContract ALTER column CurrentBalance numeric(16,4) NOT NULL 
GO

ALTER TABLE dbo.TMPHOLIDAYSCONTROLByContract ALTER column DaysProvided numeric(16,4) NOT NULL 
GO

ALTER TABLE dbo.TMPHOLIDAYSCONTROLByContract ALTER column EndingBalance numeric(16,4) NOT NULL 
GO

ALTER TABLE dbo.TMPHOLIDAYSCONTROLByContract ALTER column DaysPendingApproval numeric(16,4) NULL 
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='978' WHERE ID='DBVersion'
GO
