ALTER TABLE [dbo].[Employees] ADD Timestamp smalldatetime NULL
GO

ALTER TABLE [dbo].[DailySchedule] ADD Timestamp smalldatetime NULL
GO
-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='688' WHERE ID='DBVersion'
GO
