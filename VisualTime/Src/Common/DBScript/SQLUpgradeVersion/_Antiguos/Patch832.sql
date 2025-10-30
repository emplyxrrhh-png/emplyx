-- No borréis esta línea
DROP TRIGGER IF EXISTS [dbo].[trg_UpdateStatusLevel]
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='832' WHERE ID='DBVersion'
GO
