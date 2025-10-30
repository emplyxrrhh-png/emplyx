ALTER TABLE [dbo].[ReportExecutions]
ADD Format INT NULL
DEFAULT (0)
GO

/*******************************************************************/

CREATE TABLE [dbo].[ReportPlannedExecutions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReportId] [int] NOT NULL,
	[PassportId] [int] NOT NULL,
	[Language] [nvarchar](3) NULL,
	[Scheduler] [nvarchar](max) NULL,
	[Destination] [nvarchar](max) NULL,
	[Format] [int] NULL,
	[Parameters] [nvarchar](max) NULL,
	[NextExecutionDate] [datetime] NOT NULL,
	[LastExecutionDate] [datetime] NULL,
	[RegisteredPlannedExecutionDate] [datetime] NOT NULL,
	[ViewFields] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ReportPlannedExecutions]  WITH CHECK ADD  CONSTRAINT [FK_ReportPlannedExecutions_ReportLayouts] FOREIGN KEY([ReportId])
REFERENCES [dbo].[ReportLayouts] ([ID])
GO

ALTER TABLE [dbo].[ReportPlannedExecutions] CHECK CONSTRAINT [FK_ReportPlannedExecutions_ReportLayouts]
GO

ALTER TABLE [dbo].[ReportPlannedExecutions]  WITH CHECK ADD  CONSTRAINT [FK_ReportPlannedExecutions_sysroPassports] FOREIGN KEY([PassportId])
REFERENCES [dbo].[sysroPassports] ([ID])
GO

ALTER TABLE [dbo].[ReportPlannedExecutions] CHECK CONSTRAINT [FK_ReportPlannedExecutions_sysroPassports]
GO

/*******************************************************************/

CREATE NONCLUSTERED INDEX [PX_TASKS_TYPEAUTH]
ON [dbo].[Tasks] ([Status],[TypeAuthorization],[ID])
GO

/*******************************************************************/

CREATE TABLE [dbo].[ReportExecutionsLastParameters](
	[PassportId] [int] NOT NULL,
	[ReportId] [int] NOT NULL,
	[Parameters] [nvarchar](max) NULL,
 CONSTRAINT [IX_ReportExecutionsLastParameters] UNIQUE NONCLUSTERED 
(
	[PassportId] ASC,
	[ReportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ReportExecutionsLastParameters]  WITH CHECK ADD  CONSTRAINT [FK_ReportExecutionsLastParameters_ReportLayouts] FOREIGN KEY([ReportId])
REFERENCES [dbo].[ReportLayouts] ([ID])
GO

ALTER TABLE [dbo].[ReportExecutionsLastParameters] CHECK CONSTRAINT [FK_ReportExecutionsLastParameters_ReportLayouts]
GO

ALTER TABLE [dbo].[ReportExecutionsLastParameters]  WITH CHECK ADD  CONSTRAINT [FK_ReportExecutionsLastParameters_sysroPassports] FOREIGN KEY([PassportId])
REFERENCES [dbo].[sysroPassports] ([ID])
GO

ALTER TABLE [dbo].[ReportExecutionsLastParameters] CHECK CONSTRAINT [FK_ReportExecutionsLastParameters_sysroPassports]
GO

/*******************************************************************/

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='482' WHERE ID='DBVersion'
GO

