--Añadimos grupo
INSERT INTO ConceptCategories values('Centro de Coste')
GO

--
--Actualizamos campos de produccion
--

-- Añade campo JobTemplates
ALTER TABLE [dbo].[JobTemplates] ADD [IDMachine] [tinyint] NULL
GO
ALTER TABLE [dbo].[JobTemplates] ADD [AskMachine] [tinyint] NULL 
GO

-- Añade campo Jobs
ALTER TABLE [dbo].[Jobs] ADD [AskMachine] [tinyint] NULL 
GO


--Actualizamos sin equipo
UPDATE TEAMS SET NAME='(Sin equipo)' WHERE ID=0
GO

--Actualizamos maquinas
ALTER TABLE [dbo].[Machines] ADD [ReaderInputCode] [int] NULL
GO
UPDATE MACHINES SET ReaderInputCode=0 
GO


-- Añade acceso a la pantalla de informes en el tab de produccion
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity,RequiredFeatures) 
	VALUES ('NavBar\Job\Reports','Reports','roFormReports.vbd','reports.ico','3111111111',1010,'NR','Forms\JobStatus')
GO

---Actualizamso GUI
UPDATE sysroGUI SET Priority=700  WHERE IDPath='NavBar\Job\JobStatus'
GO
UPDATE sysroGUI SET Priority=710  WHERE IDPath='NavBar\Job\Teams'
GO
UPDATE sysroGUI SET Priority=720  WHERE IDPath='NavBar\Job\JobTemplates'
GO
UPDATE sysroGUI SET Priority=730  WHERE IDPath='NavBar\Job\Machines'
GO
UPDATE sysroGUI SET Priority=740  WHERE IDPath='NavBar\Job\JobIncidences'
GO
UPDATE sysroGUI SET Priority=750  WHERE IDPath='NavBar\Job\JobIncidences'
GO
UPDATE sysroGUI SET Priority=760  WHERE IDPath='NavBar\Job\Reports'
GO

DELETE FROM sysroGUI WHERE IDPath='NavBar\Job\MachineStatus'
GO


--Actualzamos maquinas
UPDATE MachineGroups SET Name='(Sin grupo)' WHERE ID= 0
GO


-- Creamos tabla de contadores
CREATE TABLE [dbo].[CustomCounters] (
	[JobCounter] [decimal](18, 0) NOT NULL 
) ON [PRIMARY]
GO
INSERT INTO CustomCounters VALUES (0)
GO

--Modificar categorias de produccion
UPDATE JobIncidenceCategories set Name='(ninguna)' WHERE ID = 0
GO

--Actualizamos sysrogui
UPDATE sysroGUI SET RequiredFeatures='Forms\Terminals' WHERE IDPath='NavBar\Config\Terminals'
GO

UPDATE sysroGUI SET RequiredFeatures='Forms\Options' WHERE IDPath='NavBar\Config\Options'
GO


--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='168' WHERE ID='DBVersion'
GO

