--Comportamiento de Accesos para terminales de terceros
INSERT INTO sysroReaderTemplates VALUES  (N'Virtual',1,114,N'ACC',N'1',N'Blind',N'X',N'Local',N'0',N'0',N'0',N'0',N'0',N'Remote',0,NULL,N'');
GO

INSERT INTO sysrogui values ('Portal\GeneralManagement\AdvReport', 'AdvReport', '/Report', 'ReportScheduler.png', null, null, 'Feature\AdvReport', null, 105, 'NWR', 'U:Administration.ReportScheduler.Definition=Read')
GO

/**Elimino las tablas de la anterior versión y creo las nuevas**/

ALTER TABLE [dbo].[ReportLayoutPermission] DROP CONSTRAINT [FK_ReportLayoutPermission_sysroPassports]
GO
ALTER TABLE [dbo].[ReportLayoutPermission] DROP CONSTRAINT [FK_ReportLayoutPermission_ReportLayouts]
GO

DROP TABLE [dbo].[ReportLayouts]
GO
DROP TABLE [dbo].[ReportLayoutPermission]
GO
DROP TABLE [dbo].[ReportFiles]
GO

CREATE TABLE [dbo].[ReportCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](350) NULL,
	[MotherBranchId] [int] NULL,
 CONSTRAINT [PK_ReportCategoriesTree] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) ON [PRIMARY],
 CONSTRAINT [UK_reportCategoriesHierarchy] UNIQUE NONCLUSTERED 
(
	[CategoryName] ASC,
	[MotherBranchId] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ReportExecutions](
	[LayoutID] [int] NOT NULL,
	[FileLink] [nvarchar](500) NULL,
	[PassportID] [int] NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[Status] [int] NULL,
	[ExecutionDate] [smalldatetime] NULL,
 CONSTRAINT [PK_ReportExecutions] PRIMARY KEY CLUSTERED 
(
	[Guid] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ReportLayoutCategories](
	[ReportLayoutId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ReportLayoutPermission](
	[ReportId] [int] NOT NULL,
	[PassportId] [int] NOT NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ReportLayouts](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LayoutName] [varchar](500) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[LayoutXMLBinary] [varbinary](max) NULL,
	[IdPassport] [int] NOT NULL,
	[CreationDate] [smalldatetime] NOT NULL,
	[LayoutPreviewXMLBinary] [varbinary](max) NULL,
 CONSTRAINT [PK_ReportLayouts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY],
 CONSTRAINT [Uniques] UNIQUE NONCLUSTERED 
(
	[LayoutName] ASC
) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[ReportLayouts] ADD  CONSTRAINT [DF_ReportLayouts_Description]  DEFAULT ('') FOR [Description]
GO
ALTER TABLE [dbo].[ReportLayouts] ADD  CONSTRAINT [DF_ReportLayouts_CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [dbo].[ReportCategories]  WITH CHECK ADD  CONSTRAINT [FK_ReportCategoriesTree_ReportCategoriesTree] FOREIGN KEY([MotherBranchId])
REFERENCES [dbo].[ReportCategories] ([Id])
GO
ALTER TABLE [dbo].[ReportCategories] CHECK CONSTRAINT [FK_ReportCategoriesTree_ReportCategoriesTree]
GO
ALTER TABLE [dbo].[ReportExecutions]  WITH CHECK ADD  CONSTRAINT [FK_ReportFiles_ReportLayouts] FOREIGN KEY([LayoutID])
REFERENCES [dbo].[ReportLayouts] ([ID])
GO
ALTER TABLE [dbo].[ReportExecutions] CHECK CONSTRAINT [FK_ReportFiles_ReportLayouts]
GO
ALTER TABLE [dbo].[ReportExecutions]  WITH CHECK ADD  CONSTRAINT [FK_ReportFiles_sysroPassports] FOREIGN KEY([PassportID])
REFERENCES [dbo].[sysroPassports] ([ID])
GO
ALTER TABLE [dbo].[ReportExecutions] CHECK CONSTRAINT [FK_ReportFiles_sysroPassports]
GO
ALTER TABLE [dbo].[ReportLayoutCategories]  WITH CHECK ADD  CONSTRAINT [FK_ReportLayoutsCategories_ReportCategories] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[ReportCategories] ([Id])
GO
ALTER TABLE [dbo].[ReportLayoutCategories] CHECK CONSTRAINT [FK_ReportLayoutsCategories_ReportCategories]
GO
ALTER TABLE [dbo].[ReportLayoutCategories]  WITH CHECK ADD  CONSTRAINT [FK_ReportLayoutsCategories_ReportLayouts] FOREIGN KEY([ReportLayoutId])
REFERENCES [dbo].[ReportLayouts] ([ID])
GO
ALTER TABLE [dbo].[ReportLayoutCategories] CHECK CONSTRAINT [FK_ReportLayoutsCategories_ReportLayouts]
GO
ALTER TABLE [dbo].[ReportLayoutPermission]  WITH CHECK ADD  CONSTRAINT [FK_ReportLayoutPermission_ReportLayouts] FOREIGN KEY([ReportId])
REFERENCES [dbo].[ReportLayouts] ([ID])
GO
ALTER TABLE [dbo].[ReportLayoutPermission] CHECK CONSTRAINT [FK_ReportLayoutPermission_ReportLayouts]
GO
ALTER TABLE [dbo].[ReportLayoutPermission]  WITH CHECK ADD  CONSTRAINT [FK_ReportLayoutPermission_sysroPassports] FOREIGN KEY([PassportId])
REFERENCES [dbo].[sysroPassports] ([ID])
GO
ALTER TABLE [dbo].[ReportLayoutPermission] CHECK CONSTRAINT [FK_ReportLayoutPermission_sysroPassports]
GO
ALTER TABLE [dbo].[ReportLayouts]  WITH CHECK ADD  CONSTRAINT [FK_ReportLayouts_sysroPassports] FOREIGN KEY([IdPassport])
REFERENCES [dbo].[sysroPassports] ([ID])
GO
ALTER TABLE [dbo].[ReportLayouts] CHECK CONSTRAINT [FK_ReportLayouts_sysroPassports]
GO
delete from  sysroShiftTimeZones where IDShift in (select id from shifts where IsTemplate=1)
GO
delete from  sysroShiftsLayers where IDShift in (select id from shifts where IsTemplate=1)
GO
delete from shifts where IsTemplate=1
GO
-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='476' WHERE ID='DBVersion'
GO

