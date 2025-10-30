ALTER TABLE [dbo].[ChannelConversationMessages] ALTER COLUMN [CreatedOn] Datetime
GO

ALTER TABLE [dbo].[ChannelConversations] ALTER COLUMN [CreatedOn] Datetime
GO

ALTER TABLE [dbo].[ChannelConversations] ALTER COLUMN [LastStatusChangeOn] Datetime
GO

ALTER TABLE [dbo].[Channels] ALTER COLUMN [Title] [nvarchar](250) NULL
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='721' WHERE ID='DBVersion'
GO
