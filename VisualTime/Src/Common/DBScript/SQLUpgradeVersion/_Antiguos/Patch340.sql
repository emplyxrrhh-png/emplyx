DELETE FROM [dbo].[sysroPassports_Sessions]
GO

ALTER TABLE [dbo].[sysroPassports_Sessions] DROP CONSTRAINT [FK_sysroPassport_Sessions_sysroPassport_Sessions]
GO

ALTER TABLE [dbo].[sysroPassports_Sessions] DROP CONSTRAINT [PK_sysroPassport_Sessions]
GO

ALTER TABLE dbo.sysroPassports_Sessions ADD CONSTRAINT
	PK_sysroPassports_Sessions PRIMARY KEY CLUSTERED 
	(
	ID,
	InvalidationCode
	)  ON [PRIMARY]

GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='340' WHERE ID='DBVersion'
GO


