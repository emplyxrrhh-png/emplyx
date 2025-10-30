-- Añade valores por defecto en tabla Entries y JobEntries
ALTER TABLE [dbo].[Entries] WITH NOCHECK ADD
	CONSTRAINT [DF_Entries_IDCause] DEFAULT (0) FOR [IDCause]
GO
ALTER TABLE [dbo].[JobEntries] WITH NOCHECK ADD
	CONSTRAINT [DF_JobEntries_IDMachine] DEFAULT (0) FOR [IDMachine]
GO
ALTER TABLE [dbo].[JobEntries] WITH NOCHECK ADD
	CONSTRAINT [DF_JobEntries_IDIncidence] DEFAULT (0) FOR [IDIncidence]
GO
UPDATE sysRoGuI SET RequiredFeatures='' WHERE IDPath='NavBar\Config\Terminals'
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='143' WHERE ID='DBVersion'
GO

