ALTER TABLE [dbo].[sysroTasks] ADD ExternalTask bit default(0)
GO
UPDATE [dbo].[sysroTasks] SET ExternalTask = 0 WHERE ExternalTask is null
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='256' WHERE ID='DBVersion'
GO
