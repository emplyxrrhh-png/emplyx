-- Nueva tabla para los terminales 'MXC'
CREATE TABLE [TerminalReadersMZA] (
	[IDTerminal] [tinyint] NOT NULL ,
	[ID] [tinyint] NOT NULL ,
	[IDZone] [tinyint] NOT NULL ,
	[Output] [tinyint] NOT NULL ,
	CONSTRAINT [PK_TerminalReadersMZA] PRIMARY KEY  CLUSTERED 
	(
		[IDTerminal],
		[ID],
		[IDZone]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO


--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='188' WHERE ID='DBVersion'
GO
