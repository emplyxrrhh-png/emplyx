-- No borréis esta línea
ALTER TABLE [dbo].[sysroNotificationTasks] ADD DestinationsNotified INT NOT NULL DEFAULT 0
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='794' WHERE ID='DBVersion'
GO
