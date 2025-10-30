UPDATE sysroReaderTemplates SET type='rxa' WHERE type like 'rxA'
GO

UPDATE sysroReaderTemplates SET type='rxB' WHERE type like 'rxb'
GO

UPDATE sysroReaderTemplates SET type='SAIWALL' WHERE type like 'SAIWALL%'
GO

UPDATE sysroReaderTemplates SET type='rxA' WHERE type like 'rxa'
GO

UPDATE sysroReaderTemplates SET type='rxA100FP' WHERE type like 'rxa100fp'
GO

UPDATE sysroReaderTemplates SET type='rxA100F' WHERE type like 'rxA100f'
GO

UPDATE sysroReaderTemplates SET type='rxA100P' WHERE type like 'rxa100p'
GO

UPDATE sysroReaderTemplates SET type='rxA200' WHERE type like 'rxa200'
GO

-- Terminal de control de accesos de vehículos de MasterASP
INSERT INTO sysroReaderTemplates VALUES ('masterASP',1,47,'ACC',1,'Blind','X','Local',0,0,0,1,0,NULL,1)
GO

-- Tabla de soporte para la herramienta VTUpgradePrepare para actualización a Live
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DF_TMPsysroGUI_Priority]') and OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [dbo].[TMPsysroGUI] DROP CONSTRAINT [DF_TMPsysroGUI_Priority]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[TMPsysroGUI]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[TMPsysroGUI]
GO

CREATE TABLE [dbo].[TMPsysroGUI](
	[IDPath] [nvarchar](200) NOT NULL,
	[LanguageReference] [nvarchar](64) NULL,
	[URL] [nvarchar](200) NULL,
	[IconURL] [nvarchar](200) NULL,
	[Type] [nvarchar](64) NULL,
	[Parameters] [nvarchar](200) NULL,
	[RequiredFeatures] [nvarchar](200) NULL,
	[SecurityFlags] [nvarchar](99) NULL,
	[Priority] [int] NOT NULL,
	[AllowedSecurity] [nvarchar](25) NULL,
	[RequiredFunctionalities] [nvarchar](200) NULL,
 CONSTRAINT [PK_TMPsysroGUI] PRIMARY KEY NONCLUSTERED 
(
	[IDPath] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TMPsysroGUI] ADD  CONSTRAINT [DF_TMPsysroGUI_Priority]  DEFAULT ((999999)) FOR [Priority]
GO

-- Nuevo campo para tabla de terminales (añadimos esquema dbo por si no es el por defecto)
alter table [dbo].[terminals] add [RigidMode] [tinyint] default(0)
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='294' WHERE ID='DBVersion'
GO
