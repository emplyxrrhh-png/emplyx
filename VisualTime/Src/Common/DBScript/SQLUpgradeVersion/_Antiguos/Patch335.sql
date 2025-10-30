------ EXPORTACIONES 
CREATE TABLE [dbo].[ExportGuides](
	[ID] [smallint] NOT NULL,
	[Name] [nvarchar](128) NOT NULL,		
	[ProfileMask] [nvarchar](128) NOT NULL,
	[ProfileType] [smallint] NOT NULL,	
	[Mode] [smallint] NULL,			
	[ProfileName] [nvarchar](128) NULL,
	[Destination][nvarchar](128) NULL,
	[ExportFileName] [nvarchar](128) NULL,	
	[ExportFileType] [smallint] NULL,	
	[Separator] [nvarchar](1) NULL,
	[StartCalculDay] [smallint] NULL,
	[StartExecutionHour] [nvarchar](5) NULL,	
	[LastLog] [text] NULL,
	[NextExecution] [DateTime] NULL,
	[Field_1] [numeric] (18,6) NULL,
	[Field_2] [numeric] (18,6) NULL,
	[Field_3] [nvarchar](128) NULL,	 
	[Field_4] [nvarchar](128) NULL,	 
	[Field_5] [DateTime] NULL,	 
	[Field_6] [DateTime] NULL,	 


 CONSTRAINT [PK_ExportGuides] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

----- EXPORTACIONES DIRECTAS
INSERT INTO [dbo].[ExportGuides]
           ([ID],[Name],[Mode],[ExportFileType],[ProfileMask],[ProfileType])
     VALUES
           (9000,'Exportación de saldos diarios', 1,1,'',0)
GO

INSERT INTO [dbo].[ExportGuides]
           ([ID],[Name],[Mode],[ExportFileType],[ProfileMask],[ProfileType])
     VALUES
           (9001,'Exportación de saldos periódicos', 1,1,'',0)
GO

INSERT INTO [dbo].[ExportGuides]
           ([ID],[Name],[Mode],[ExportFileType],[ProfileMask],[ProfileType])
     VALUES
           (9002,'Exportación de calendario', 1,1,'',0)
GO

INSERT INTO [dbo].[ExportGuides]
           ([ID],[Name],[Mode],[ExportFileType],[ProfileMask],[ProfileType])
     VALUES
           (9003,'Exportación de vacaciones',1,1,'',0)
GO

INSERT INTO [dbo].[ExportGuides]
           ([ID],[Name],[Mode],[ExportFileType],[ProfileMask],[ProfileType])
     VALUES
           (9004,'Exportación avanzada de saldos', 1,2,'adv_Accruals',1)
GO
INSERT INTO [dbo].[ExportGuides]
           ([ID],[Name],[Mode],[ExportFileType],[ProfileMask],[ProfileType])
     VALUES
           (9005,'Exportación avanzada de ausencias prolongadas', 1,2,'adv_ProAbs',2)
GO
INSERT INTO [dbo].[ExportGuides]
           ([ID],[Name],[Mode],[ExportFileType],[ProfileMask],[ProfileType])
     VALUES
           (9006,'Exportación avanzada de comedor',1,2,'adv_Dinner',3)
GO

-- Factor de exportación a alfanumérico en Concepts
ALTER TABLE [dbo].[Concepts] DROP  CONSTRAINT [DF_Concepts_Export] 
GO

ALTER TABLE  [dbo].[Concepts] ALTER COLUMN [Export]  NVARCHAR(15)
GO

ALTER TABLE [dbo].[Concepts] ADD  CONSTRAINT [DF_Concepts_Export]  DEFAULT (('0')) FOR [Export]
GO

UPDATE CONCEPTS SET EXPORT=REPLACE(EXPORT,'.00','');
GO

-- Factor de exportación en Causes
ALTER TABLE [dbo].[Causes] add [Export]  NVARCHAR(15)
GO

ALTER TABLE [dbo].[Causes] ADD  CONSTRAINT [DF_Causes_Export]  DEFAULT (('0')) FOR [Export]
GO

UPDATE [dbo].[Causes] SET [Export]='0' 
GO

-- Fecha de recaída
ALTER TABLE [dbo].ProgrammedAbsences add  [RelapsedDate] smalldatetime
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='335' WHERE ID='DBVersion'
GO


