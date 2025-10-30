CREATE TABLE [dbo].[Exports] (
	[IdExport] [int] NOT NULL ,
	[Description] [nvarchar] (50) NOT NULL ,
	[Path] [nvarchar] (50)  NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ExportsFormats] (
	[IdExport] [int] NOT NULL ,
	[IdOrder] [int] NOT NULL ,
	[TypeField] [nvarchar] (10) NOT NULL ,
	[Value] [nvarchar] (10) NOT NULL ,
	[PropertyField] [nvarchar] (10) NOT NULL ,
	[Length] [nvarchar] (10) NOT NULL ,
	[Padding] [nvarchar] (10) NULL ,
	[SeparatorDec] [nvarchar] (10) NULL ,
	[NumDec] [nvarchar] (10) NULL ,
	[DataFormat] [nvarchar] (10) NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ExportsPeriods] (
	[IdExport] [int] NOT NULL ,
	[PeriodType] [nvarchar] (10) NOT NULL ,
	[DateInf] [nvarchar] (10) NULL ,
	[DateSup] [nvarchar] (10) NOT NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Exports] WITH NOCHECK ADD 
	CONSTRAINT [PK_Exports] PRIMARY KEY  NONCLUSTERED 
	(
		[IdExport]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ExportsFormats] WITH NOCHECK ADD 
	CONSTRAINT [PK_ExportsFormats] PRIMARY KEY  NONCLUSTERED 
	(
		[IdExport],
		[IdOrder]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ExportsPeriods] WITH NOCHECK ADD 
	CONSTRAINT [PK_ExportsPeriods] PRIMARY KEY  NONCLUSTERED 
	(
		[IdExport]
	)  ON [PRIMARY] 
GO


INSERT INTO sysroGUI (IDPath, LanguageReference, URL, IconURL, RequiredFeatures, SecurityFlags, Priority, AllowedSecurity) VALUES ('NavBar\Config\Exports','Exports','roFormExports.vbd','Log.ico','Feature\AccrualExports','3111111111',900,'NWR')
GO

--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='158' WHERE ID='DBVersion'
GO

