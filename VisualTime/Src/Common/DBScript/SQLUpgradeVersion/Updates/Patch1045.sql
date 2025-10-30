-- No borréis esta línea
ALTER TABLE [dbo].[CollectivesDefinitions] ADD [Description] NVARCHAR(MAX) NULL
GO
ALTER TABLE [dbo].[CollectivesDefinitions] ADD [ModifiedBy] NVARCHAR(100) NOT NULL DEFAULT ''
GO
ALTER TABLE [dbo].[CollectivesDefinitions] ADD [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE()
GO
UPDATE [dbo].[CollectivesDefinitions] SET [ModifiedDate] = BeginDate
GO
ALTER TABLE [dbo].[Collectives] ADD [CreatedBy] NVARCHAR(100) NOT NULL DEFAULT ''
GO
ALTER TABLE [dbo].[Collectives] ADD [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE()
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1045' WHERE ID='DBVersion'
GO
