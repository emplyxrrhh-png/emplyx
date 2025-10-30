CREATE TABLE [dbo].[Channels](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NULL,
	[IdCreatedBy] [int] NOT NULL,
	[Status] [smallint] NOT NULL,
	[CreatedOn] [smalldatetime] NOT NULL,
	[IdModifiedBy] [int] NOT NULL,
	[ModifiedOn] [smalldatetime] NOT NULL,
	[PublishedOn] [smalldatetime] NOT NULL,
	[ReceiptAcknowledgment] [bit] NOT NULL,
	[AllowAnonymous] [bit] NOT NULL,
	[IsComplaintChannel] [bit] NOT NULL, 
	[Deleted] [bit] NOT NULL
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Channels] ADD  DEFAULT ((1)) FOR [AllowAnonymous]
GO

ALTER TABLE [dbo].[Channels] ADD  DEFAULT ((0)) FOR [Deleted]
GO

ALTER TABLE [dbo].[Channels] ADD  DEFAULT ((0)) FOR [ReceiptAcknowledgment]
GO

ALTER TABLE [dbo].[Channels] ADD  DEFAULT ((0)) FOR [IsComplaintChannel]
GO

ALTER TABLE [dbo].[Channels]  WITH CHECK ADD FOREIGN KEY([IdCreatedBy])
REFERENCES [dbo].[sysroPassports] ([ID])
GO


CREATE TABLE [dbo].[ChannelEmployees](
	[IdChannel] [int] NOT NULL,
	[IdEmployee] [int] NOT NULL,
 CONSTRAINT [PK_ChannelEmployees] PRIMARY KEY CLUSTERED 
(
	[IdChannel] ASC,
	[IdEmployee] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ChannelEmployees]  WITH CHECK ADD FOREIGN KEY([IdEmployee])
REFERENCES [dbo].[Employees] ([ID])
GO

ALTER TABLE [dbo].[ChannelEmployees]  WITH CHECK ADD FOREIGN KEY([IdChannel])
REFERENCES [dbo].[Channels] ([Id])
GO



CREATE TABLE [dbo].[ChannelGroups](
	[IdChannel] [int] NOT NULL,
	[IdGroup] [int] NOT NULL,
 CONSTRAINT [PK_ChannelGroups] PRIMARY KEY CLUSTERED 
(
	[IdChannel] ASC,
	[IdGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ChannelGroups]  WITH CHECK ADD FOREIGN KEY([IdChannel])
REFERENCES [dbo].[Channels] ([Id])
GO

ALTER TABLE [dbo].[ChannelGroups]  WITH CHECK ADD FOREIGN KEY([IdChannel])
REFERENCES [dbo].[Groups] ([ID])
GO

CREATE TABLE [dbo].[ChannelSupervisors](
	[IdChannel] [int] NOT NULL,
	[IdSupervisor] [int] NOT NULL,
 CONSTRAINT [PK_ChannelSupervisors] PRIMARY KEY CLUSTERED 
(
	[IdChannel] ASC,
	[IdSupervisor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ChannelSupervisors]  WITH CHECK ADD FOREIGN KEY([IdSupervisor])
REFERENCES [dbo].[sysroPassports] ([ID])
GO

ALTER TABLE [dbo].[ChannelSupervisors]  WITH CHECK ADD FOREIGN KEY([IdChannel])
REFERENCES [dbo].[Channels] ([Id])
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='710' WHERE ID='DBVersion'
GO
