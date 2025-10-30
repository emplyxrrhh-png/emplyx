-- Modifica la tabla de piezas ya que la 
ALTER TABLE [dbo].[PiecesEntries] DROP CONSTRAINT FK_PiecesEntries_sysroPieceTypes
GO

if exists (select * from sysobjects where id = object_id(N'[dbo].[PiecesEntries]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[PiecesEntries]
GO

CREATE TABLE [dbo].[PiecesEntries] (
    [ID] [numeric](16, 0) IDENTITY (1, 1) NOT NULL ,
    [DateTime] [smalldatetime] NOT NULL ,
    [IDCard] [numeric](28, 0) NOT NULL ,
    [IDReader] [tinyint] NOT NULL ,
    [Type] [tinyint] NOT NULL ,
    [Value] [numeric](9, 2) NOT NULL ,
    [InvalidRead] [bit] NOT NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PiecesEntries] WITH NOCHECK ADD 
    CONSTRAINT [DF_Pieces_Type_1] DEFAULT (0) FOR [Type],
    CONSTRAINT [DF_PiecesEntries_InvalidRead] DEFAULT (0) FOR [InvalidRead],
    CONSTRAINT [PK_Pieces] PRIMARY KEY  NONCLUSTERED 
    (
        [ID]
    )  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[PiecesEntries] ADD 
    CONSTRAINT [FK_PiecesEntries_sysroPieceTypes] FOREIGN KEY 
    (
        [Type]
    ) REFERENCES [dbo].[sysroPieceTypes] (
        [ID]
    )
GO



-- Añade acceso a la pantalla de las opciones de producción
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity, RequiredFeatures) 
	VALUES ('NavBar\Job','JobTab',Null,Null,'3111111111',750,'NR','Forms\Teams')
GO
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity, RequiredFeatures) 
	VALUES ('NavBar\Job\JobIncidences','JobIncidences','roFormJobIncidences.vbd','JobIncidences.ico','3111111111',820,'NWR','Forms\JobIncidences')
GO
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity, RequiredFeatures) 
	VALUES ('NavBar\Job\JobStatus','JobStatus','roFormStatusJobs.vbd','OrderStatus.ico','3111111111',999,'NWR','Forms\JobStatus')
GO
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity, RequiredFeatures) 
	VALUES ('NavBar\Job\JobTemplates','JobTemplates','roFormJobs.vbd','Order.ico','3111111111',795,'NWR','Forms\JobTemplates')
GO
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity, RequiredFeatures) 
	VALUES ('NavBar\Job\Machines','Machines','roFormMachines.vbd','Machine.ico','3111111111',790,'NWR','Forms\Machines')
GO
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity, RequiredFeatures) 
	VALUES ('NavBar\Job\MachineStatus','MachineStatus','roFormStatusMachine.vbd','MachineStatus.ico','3111111111',1000,'NWR','Forms\MachineStatus')
GO
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity, RequiredFeatures) 
	VALUES ('NavBar\Job\Teams','Teams','roFormTeams.vbd','Team.ico','3111111111',810,'NWR','Forms\Teams')
GO



-- Inserta los valores por defecto en las tablas de produccón
INSERT INTO JobIncidenceCategories(ID, Name) VALUES (0,'Sin categoría')
GO

INSERT INTO JobIncidences(ID, Name, Description, ShortName, IDCategory) VALUES (0,'Sin incidencia','Trabajo sin incidencia','SIN',0)
GO

INSERT INTO MachineGroups(ID, Name, Description) VALUES (0,'Sin grupo máquinas','Trabajo sin grupo de máquinas')
GO

INSERT INTO Machines(ID, Name, Description, HourCost, IDGroup) VALUES (0,'Sin máquina','Trabajo sin máquina',0,0)
GO


INSERT INTO Teams(ID, Name, SecurityFlags) VALUES (0,'Sin equipo','311111111')
GO




-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='140' WHERE ID='DBVersion'
GO

