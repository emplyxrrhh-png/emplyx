ALTER TABLE [dbo].[DailySchedule] ADD TimestampHolidays smalldatetime NULL
GO

ALTER TABLE [dbo].[ProgrammedHolidays] ADD Timestamp smalldatetime NULL
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='689' WHERE ID='DBVersion'
GO
