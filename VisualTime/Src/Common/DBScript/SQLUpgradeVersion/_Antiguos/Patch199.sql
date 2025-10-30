-- Se añade nuevo estado de las peticiones.
INSERT INTO sysroRequestStatus VALUES (3, 'Anulado')
GO

--- Nueva extensión de enlace de datos
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity, RequiredFeatures) 
	VALUES ('NavBar\FirstTab\DataLink','DataLink','roFormGuidesExtraction.vbd','exports.ico','3111111111',1000,'NWR','Forms\Datalink')
GO

CREATE TABLE [dbo].[GuideExtractionFields](
	[IDGuide] [smallint] NOT NULL,
	[ID] [smallint] NOT NULL,
	[Pos] [smallint] NOT NULL,
	[Definition] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_GuideExtractionFields] PRIMARY KEY NONCLUSTERED 
(
	[IDGuide] ,
	[ID] 
) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[GuidesExtraction](
	[ID] [smallint] NOT NULL,
	[Name] [nvarchar](128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Type] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Definition] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[FilePath] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_GuidesExtraction] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO


UPDATE sysroParameters SET Data='199' WHERE ID='DBVersion'
GO


