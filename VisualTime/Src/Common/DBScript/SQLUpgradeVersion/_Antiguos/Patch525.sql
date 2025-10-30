CREATE TABLE [dbo].[DeletedProgrammedHolidays](
	[IDEmployee] [int] NOT NULL,
	[IDHoursHoliday] [int]  NULL,
	[PlannedDate] [datetime] NULL,
	[TimeStamp] [smalldatetime] NULL
) ON [PRIMARY]
GO

-- ROLLBACK DE TELETRRABAJO
DELETE FROM [dbo].[sysroNotificationTypes] WHERE ID = 75
GO

DELETE FROM [dbo].[Notifications] WHERE ID = 1300
GO

UPDATE sysroParameters SET Data='525' WHERE ID='DBVersion'
GO


 