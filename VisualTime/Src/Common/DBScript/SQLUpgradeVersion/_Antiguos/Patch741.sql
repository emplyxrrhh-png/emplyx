-- No borréis esta línea
ALTER TABLE [dbo].[Channels] ADD [PrivacyPolicy] TEXT NOT NULL DEFAULT('')
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='741' WHERE ID='DBVersion'
GO
