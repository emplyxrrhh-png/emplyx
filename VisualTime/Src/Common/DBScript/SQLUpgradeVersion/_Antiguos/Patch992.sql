ALTER TABLE [dbo].[DailySchedule] ADD [IDPreviousShift] [smallint] NULL
GO

ALTER TABLE [dbo].[DailySchedule]  WITH NOCHECK ADD  CONSTRAINT [FK_DailySchedulePreviousShift_Shifts] FOREIGN KEY([IDPreviousShift])
REFERENCES [dbo].[Shifts] ([ID])
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='992' WHERE ID='DBVersion'
GO
