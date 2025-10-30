-- No borréis esta línea
ALTER TABLE [dbo].[ChannelConversations] ALTER COLUMN Password NVARCHAR(64)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='738' WHERE ID='DBVersion'
GO
