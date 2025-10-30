--
-- Nueva tabla para guardar la configuracion de las importaciones en el enlace de datos
--
CREATE TABLE [dbo].[ImportGuides](
	[ID] [smallint] NOT NULL,
	[Name] [nvarchar](128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Template] [smallint] NULL,
	[Mode] [smallint] NULL,
	[Type] [smallint] NULL,
	[FormatFilePath] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SourceFilePath] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Separator] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[CopySource] Bit NULL ,
	[LastLog] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
CONSTRAINT [PK_ImportGuides] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[ImportGuides] (ID,Name, Template, Mode, Type, FormatFilePath, SourceFilePath, Separator, CopySource)
 Values(1, 'Carga de Empleados', 0, 1, 1, '','','',0)
GO

INSERT INTO [dbo].[ImportGuides] (ID,Name, Template, Mode, Type, FormatFilePath, SourceFilePath, Separator, CopySource)
 Values(2, 'Carga de Planificación', 0, 0, 0, '','','',0)
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='262' WHERE ID='DBVersion'
GO
