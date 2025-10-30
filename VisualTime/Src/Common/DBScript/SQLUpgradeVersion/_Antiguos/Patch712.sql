CREATE VIEW [dbo].[sysrovwChannelEmployees]
AS
SELECT IdChannel, IdEmployee FROM [dbo].[ChannelEmployees]
LEFT JOIN [dbo].[Channels] ON Channels.Id = ChannelEmployees.IdChannel
UNION
SELECT IdChannel, ceg.IDEmployee FROM Channels
INNER JOIN [dbo].[ChannelGroups] cg ON Channels.Id = cg.IdChannel
INNER JOIN [dbo].[Groups] ON Groups.ID =  cg.idgroup
INNER JOIN [dbo].[sysrovwCurrentEmployeeGroups] ceg ON (ceg.Path = Groups.Path OR ceg.Path LIKE Groups.Path + '\%') AND ceg.CurrentEmployee = 1
GO

ALTER TABLE [dbo].[Channels] ADD [IdDeletedBy] [int] NULL
GO
ALTER TABLE [dbo].[Channels] ADD [DeletedOn] [smalldatetime] NULL
GO
ALTER TABLE [dbo].[Channels] ALTER COLUMN [IdModifiedBy] [int] NULL
GO
ALTER TABLE [dbo].[Channels] ALTER COLUMN [ModifiedOn] [smalldatetime] NULL
GO
ALTER TABLE [dbo].[Channels] ALTER COLUMN [PublishedOn] [smalldatetime] NULL
GO



CREATE TABLE [dbo].[ChannelConversations](
	[Id] [int] NOT NULL,
	[IdChannel] [int] NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedOn] [date] NOT NULL,
	[Title] [varchar](255) NOT NULL,
	[ReferenceNumber] [varchar](10) NOT NULL,
	[IsAnonymous] [bit] NOT NULL,
	[Status] [int] NOT NULL,
	[LastStatusChangeBy] [int] NULL,
	[LastStatusChangeOn] [smalldatetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ChannelConversations]  WITH CHECK ADD FOREIGN KEY([IdChannel])
REFERENCES [dbo].[Channels] ([Id])
GO


CREATE TABLE [dbo].[ChannelConversationMessages](
	[Id] [int] NOT NULL,
	[IdConversation] [int] NOT NULL,
	[Body] [varchar](max) NOT NULL,
	[IsAnonymous] [bit] NOT NULL,
	[IdEmployee] [int] NOT NULL,
	[IdSupervisor] [int] NOT NULL,
	[CreatedOn] [smalldatetime] NOT NULL,
	[Status] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ChannelConversationMessages]  WITH CHECK ADD FOREIGN KEY([IdConversation])
REFERENCES [dbo].[ChannelConversations] ([Id])
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='712' WHERE ID='DBVersion'
GO
