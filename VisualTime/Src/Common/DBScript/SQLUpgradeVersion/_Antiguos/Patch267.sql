ALTER TABLE dbo.Entries ADD
	Reliability int NULL
GO
ALTER TABLE dbo.Entries ADD CONSTRAINT
	DF_Entries_Reliability DEFAULT 100 FOR Reliability
GO

/* Nueva table para guardar los fichajes inválidos (de momento sólo de los terminales mx7 en modo TAACC) */
CREATE TABLE dbo.InvalidMoves
	(
	ID numeric(16, 0) NOT NULL,
	DateTime smalldatetime NOT NULL,
	IDCard numeric(28, 0) NOT NULL,
	IDReader tinyint NOT NULL,
	Type char(1) NOT NULL,
	IDCause smallint NOT NULL,
	Rdr tinyint NULL,
	IDCapture int NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.InvalidMoves ADD CONSTRAINT
	DF_InvalidMoves_IDCause DEFAULT (0) FOR IDCause
GO
ALTER TABLE dbo.InvalidMoves ADD CONSTRAINT
	DF_InvalidMoves_Rdr DEFAULT (0) FOR Rdr
GO
ALTER TABLE dbo.InvalidMoves ADD CONSTRAINT
	PK_InvalidMoves PRIMARY KEY CLUSTERED 
	(
	ID
	) ON [PRIMARY]

GO
ALTER TABLE dbo.InvalidMoves ADD CONSTRAINT
	FK_InvalidMoves_Captures FOREIGN KEY
	(
	IDCapture
	) REFERENCES dbo.Captures
	(
	ID
	)
	
GO


-- Marcamos las tareas que crea el servidor de Visualtime para poder filtrar
ALTER TABLE SYSROTASKS ADD SystemTask bit null default(0)
GO
UPDATE SYSROTASKS set SystemTask=0 where SystemTask IS NULL
GO


ALTER TABLE [dbo].[sysroTasks] ADD ExternalTask bit default(0)
GO
UPDATE [dbo].[sysroTasks] SET ExternalTask = 0 WHERE ExternalTask is null
GO


ALTER TABLE [dbo].[sysroUsers] ADD [EmployeeAssigned] [int] NULL DEFAULT (0)
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='267' WHERE ID='DBVersion'
GO
