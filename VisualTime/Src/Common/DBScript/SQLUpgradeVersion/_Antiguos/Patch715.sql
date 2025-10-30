
DROP TABLE [dbo].[ChannelGroups]
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

ALTER TABLE [dbo].[ChannelGroups]  WITH CHECK ADD FOREIGN KEY([IdGroup])
REFERENCES [dbo].[Groups] ([ID])
GO
-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='715' WHERE ID='DBVersion'
GO
