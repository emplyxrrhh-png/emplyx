--
-- Cambiar tipos de campos IDTemplate a String
-- 06/2004

-- Tabla para tareas FTP terminales MX4fRT

CREATE TABLE [TerminalsTasksFTP] (
	[IDTask] [bigint] IDENTITY (1, 1) NOT NULL ,
	[IDTerminal] [tinyint] NOT NULL ,
	[IDsEmployee] [nvarchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[ActionAD] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[DateTask] [datetime] NOT NULL ,
	[DeleteConfirm] [bit] NOT NULL CONSTRAINT [DF_TerminalsTasksFTP_DeleteConfirm] DEFAULT (0)
) ON [PRIMARY]
GO


--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='176' WHERE ID='DBVersion'
GO
