--
-- Crear tabla de definicion de horarios para informes
--
DROP TABLE [dbo].[TMPSHIFTDEFINITIONS]
GO

CREATE TABLE [dbo].[TMPSHIFTDEFINITIONS] (
	[IDShift] [smallint] NOT NULL ,
	[Layers] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL, 
	[Zones] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL, 
	[Rules] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
)
GO

--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='195' WHERE ID='DBVersion'
GO

--
-- Exportaciones y ExportacionesAndorra cambian de Canfiguración a Presencia 
--

UPDATE sysroGUI SET IDPath='NavBar\FirstTab\Exports', PRIORITY=700 WHERE URL='roFormExports.vbd'
GO

UPDATE sysroGUI SET IDPath='NavBar\FirstTab\ExportAndorra', PRIORITY=710 WHERE URL='roFormExportAndorra.vbd'
GO
