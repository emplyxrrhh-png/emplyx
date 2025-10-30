-- ***************** TABLA para las grabaciones de las aplicaciones externas (p.ej: webTerminal)  ***************** --

CREATE TABLE [sysroExternalTasks] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Task] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[ExternalApp] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[DateCreated] [smalldatetime] NULL ,
	CONSTRAINT [PK_sysroExternalTasks] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO


--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='184' WHERE ID='DBVersion'
GO
