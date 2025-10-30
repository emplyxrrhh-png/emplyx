ALTER TABLE [dbo].[Concepts] ADD [DailyRecordMargin] [int] NULL
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='628' WHERE ID='DBVersion'
GO
