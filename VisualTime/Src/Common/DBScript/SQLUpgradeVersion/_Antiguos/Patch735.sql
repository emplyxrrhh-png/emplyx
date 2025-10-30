-- No borréis esta línea
ALTER TABLE [dbo].[ChannelConversations] ALTER COLUMN ReferenceNumber NVARCHAR(36)
GO
ALTER TABLE [dbo].[ChannelConversations] ADD [Password] [nvarchar](50) NULL
GO
ALTER TABLE [dbo].[ChannelConversations] ADD [ExtraData] [nvarchar](MAX) NULL
GO

UPDATE sysroParameters SET Data='735' WHERE ID='DBVersion'
GO
