
ALTER TABLE [dbo].[Concepts] ADD Color [int]  NULL
GO

ALTER TABLE [dbo].[Concepts] ADD  DEFAULT (0) FOR [Color]
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='310' WHERE ID='DBVersion'
GO

