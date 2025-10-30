-- No borréis esta línea
ALTER TABLE [dbo].[ChannelConversations] ADD [Complexity] SMALLINT NOT NULL DEFAULT(0)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='740' WHERE ID='DBVersion'
GO
