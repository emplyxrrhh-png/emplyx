ALTER TABLE [dbo].[sysroNotificationTasks] ADD [Key6DateTime] [datetime] NULL
GO

UPDATE sysroParameters SET Data='519' WHERE ID='DBVersion'
GO


 