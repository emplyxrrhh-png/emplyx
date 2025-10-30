--
-- Actualizamos la tabla de AccessGroupsHollyday, cambiamos los campos clave
--

ALTER TABLE [dbo].[AccessGroupsHollyday] DROP CONSTRAINT [PK_AccessGroupsHollyday] 
GO
ALTER TABLE [dbo].[AccessGroupsHollyday] WITH NOCHECK ADD CONSTRAINT [PK_AccessGroupsHollyday] PRIMARY KEY NONCLUSTERED (IDAccessGroup, DayOfYear,BeginTime) 
GO

-- Actualizamos la tabla de Jobs
ALTER TABLE [dbo].[Jobs] ADD CONSTRAINT [DF_IDMachineGroup] DEFAULT (0) FOR [IDMachineGroup]
GO
ALTER TABLE [dbo].[Jobs] ADD CONSTRAINT [DF_IDMachine] DEFAULT (0) FOR [IDMachine]
GO

ALTER TABLE [dbo].[Orders] ALTER COLUMN  [Amount] [numeric](18, 2) NULL
GO

ALTER TABLE [dbo].[OrderTemplates] DROP CONSTRAINT DF_OrderTemplates_PiecesPerUnit
GO

ALTER TABLE [dbo].[OrderTemplates] ALTER COLUMN  [UnitPieces] [numeric](18, 2) NULL
GO


--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='155' WHERE ID='DBVersion'
GO