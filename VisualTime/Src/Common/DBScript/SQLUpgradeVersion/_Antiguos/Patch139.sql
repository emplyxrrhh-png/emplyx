-- Añade acceso a la pantalla de terminales
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity) 
	VALUES ('NavBar\Config\Terminals','Terminals','roFormTerminals.vbd','ReaderOnly_32_256.ico','3111111111',900,'NWR')
GO

-- Modifica la opción de terminales para que tenga que estar registrada
UPDATE sysroGUI SET RequiredFeatures='Forms\Terminals' WHERE IDPath='NavBar\Config\Terminals'
GO


-- Crea la tabla de sirenas
CREATE TABLE [dbo].[TerminalSirens] (
	[ID] [tinyint] NOT NULL ,
	[IDTerminal] [tinyint] NOT NULL ,
	[Weekday] [tinyint] NOT NULL ,
	[Hour] [datetime] NOT NULL ,
	[Duration] [tinyint] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TerminalSirens] WITH NOCHECK ADD 
	CONSTRAINT [PK_TerminalSirens] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO


-- Modifica las opciones de usuario del menú
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='N' WHERE IDPath='Menu'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='N' WHERE IDPath='Menu\File'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='N' WHERE IDPath='Menu\File\Close'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Edit'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\View'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\View\Bar'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\View\Status'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\View\Tasks'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Action'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Tools'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Tools\Options'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Tools\CommsOptions'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Help'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Help\Contents'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Help\TOC'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Help\Search'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Help\HelpS1'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Help\Tutorial'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Help\HelpS2'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Help\About'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Help\HelpS3'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Help\Support'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Help\Support\Downloads'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Help\S4'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='N' WHERE IDPath='NavBar'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='NavBar\FirstTab'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NWR' WHERE IDPath='NavBar\FirstTab\Employees'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NWR' WHERE IDPath='NavBar\FirstTab\Scheduler'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NW' WHERE IDPath='NavBar\FirstTab\Reports'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NWR' WHERE IDPath='NavBar\FirstTab\Shifts'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NWR' WHERE IDPath='NavBar\FirstTab\Concepts'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NW' WHERE IDPath='NavBar\Config\Options'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='N' WHERE IDPath='NavBar\Config\Users'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='NavBar\Config'
GO
UPDATE sysroGUI Set SecurityFlags='3111111111', AllowedSecurity='NR' WHERE IDPath='Menu\Help\Agent'
GO



-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='139' WHERE ID='DBVersion'
GO

