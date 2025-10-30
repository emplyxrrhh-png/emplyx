-- No borréis esta línea
DELETE [dbo].[sysroReaderTemplates] WHERE [Type] = N'rxFe' AND [InteractionMode] = N'Fast'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='824' WHERE ID='DBVersion'
GO
