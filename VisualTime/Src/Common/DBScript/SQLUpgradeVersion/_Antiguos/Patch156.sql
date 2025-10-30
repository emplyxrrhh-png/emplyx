--
-- Actualizamos laS tablaS de Produccion
--

ALTER TABLE [dbo].[JobTemplates] DROP CONSTRAINT [DF_JobTemplates_PiecesPerUnit] 
GO


ALTER TABLE [dbo].[JobTemplates] ALTER COLUMN  [PieceTime] [numeric](18, 6) not null 
GO

ALTER TABLE [dbo].[JobTemplates] ALTER COLUMN  [PreparationTime] [numeric](18, 6) not null 
GO

ALTER TABLE [dbo].[JobTemplates] ALTER COLUMN  [UnitPieces] [numeric](18, 6) not null 
GO

ALTER TABLE [dbo].[Jobs] ALTER COLUMN  [PieceTime] [numeric](18, 6) not null 
GO

ALTER TABLE [dbo].[Jobs] ALTER COLUMN  [PreparationTime] [numeric](18, 6) not null 
GO

ALTER TABLE [dbo].[Jobs] drop CONSTRAINT [DP_Jobs]
GO

ALTER TABLE [dbo].[Jobs] ALTER COLUMN  [UnitPieces] [numeric](18, 6)  null 
GO

ALTER TABLE [dbo].[Jobs] ADD CONSTRAINT [DP_Jobs] DEFAULT (0) FOR [UnitPieces]
GO


ALTER TABLE [dbo].[JobTemplates] ADD CONSTRAINT [DF_JobTemplates_PiecesPerUnit] DEFAULT (0) FOR [UnitPieces]
GO


--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='156' WHERE ID='DBVersion'
GO