-- Añade campos en la tabla de terminales 
ALTER TABLE [dbo].[Terminals] ADD [Type] NVARCHAR(50)
GO
ALTER TABLE [dbo].[Terminals] ADD [Driver] NVARCHAR(50)
GO
ALTER TABLE [dbo].[Terminals] ADD [Behavior] NVARCHAR(50)
GO
ALTER TABLE [dbo].[Terminals] ADD [Location] NVARCHAR(50)
GO
ALTER TABLE [dbo].[Terminals] ADD [LastUpdate] SMALLDATETIME
GO
ALTER TABLE [dbo].[Terminals] ADD [LastStatus] NVARCHAR(50)
GO
ALTER TABLE [dbo].[Terminals] ADD [IsTimeReferee] BIT DEFAULT (0)
GO


-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='142' WHERE ID='DBVersion'
GO

