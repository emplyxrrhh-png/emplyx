-- Añadimos campo IsBiometric a la tabla TerminalReaders para indicar si el lector es Biométrico o no

-- Nuevo campo para tabla de terminales
alter table terminalreaders add Type nvarchar(3) default('RDR')
GO
update terminalreaders set Type='RDR' where Type is null or len(Type)<2
GO
update terminalreaders set Mode='TA' where Mode is null or len(Mode)<2
GO


-- Inactividades por Zonas 
CREATE TABLE [ZonesException] (
	[IDZone] [tinyint] NOT NULL ,
	[ExceptionDate] [smalldatetime] NOT NULL ,
	CONSTRAINT [PK_ZonesException] PRIMARY KEY  CLUSTERED 
	(
		[IDZone],
		[ExceptionDate]
	)  ON [PRIMARY] ,
	CONSTRAINT [FK_ZonesException_Zones] FOREIGN KEY 
	(
		[IDZone]
	) REFERENCES [Zones] (
		[ID]
	)
) ON [PRIMARY]
GO


CREATE TABLE [ZonesInactivity] (
	[IDZone] [tinyint] NOT NULL ,
	[WeekDay] [tinyint] NOT NULL ,
	[Begin] [smalldatetime] NOT NULL ,
	[End] [smalldatetime] NOT NULL ,
	CONSTRAINT [PK_ZonesInactivity] PRIMARY KEY  CLUSTERED 
	(
		[IDZone],
		[WeekDay],
		[Begin]
	)  ON [PRIMARY] ,
	CONSTRAINT [FK_ZonesInactivity_Zones] FOREIGN KEY 
	(
		[IDZone]
	) REFERENCES [Zones] (
		[ID]
	)
) ON [PRIMARY]
GO


--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='186' WHERE ID='DBVersion'
GO
