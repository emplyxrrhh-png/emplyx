
CREATE TABLE dbo.sysroDeletedPunchesSync
	(
	ID numeric(16, 0) NOT NULL IDENTITY (1, 1),
	PunchID numeric(16, 0) NOT NULL,
	CONSTRAINT PK_sysroPunchesSync PRIMARY KEY CLUSTERED 
	(
	ID
	) ON [PRIMARY]
	) ON [PRIMARY]
GO

-- Nueva exportación avanzada de fichajes
alter table [dbo].[ExportGuides] add [IntervalMinutes] SMALLINT
GO
ALTER TABLE [dbo].[ExportGuides] ADD  CONSTRAINT [DF_ExportGuides_IntervalMinutes]  DEFAULT (('0')) FOR [IntervalMinutes]
GO

INSERT INTO [dbo].[ExportGuides]
           ([ID],[Name],[Mode],[ExportFileType],[ProfileMask],[ProfileType])
     VALUES
           (9007,'Exportación avanzada de fichajes',1,2,'adv_Punches',4)
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='341' WHERE ID='DBVersion'
GO


