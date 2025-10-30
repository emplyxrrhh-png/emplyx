-- No borréis esta línea
ALTER TABLE [dbo].[sysroUserFields] ADD [Unique] [bit] NOT NULL DEFAULT (0)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='945' WHERE ID='DBVersion'
GO
