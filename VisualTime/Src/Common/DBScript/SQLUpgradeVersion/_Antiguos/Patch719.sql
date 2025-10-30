ALTER TABLE [dbo].[ChannelConversationMessages] DROP COLUMN IsAnonymous
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='719' WHERE ID='DBVersion'
GO
