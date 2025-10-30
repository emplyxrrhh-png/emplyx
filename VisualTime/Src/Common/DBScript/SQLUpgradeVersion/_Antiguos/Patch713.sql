DROP TABLE [dbo].[ChannelConversationMessages]
GO
DROP TABLE [dbo].[ChannelConversations]
GO

CREATE TABLE [dbo].[ChannelConversations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdChannel] [int] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedOn] [smalldatetime] NOT NULL,
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
	[Id] [int]  IDENTITY(1,1)  NOT NULL,
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

UPDATE sysroParameters SET Data='713' WHERE ID='DBVersion'
GO
