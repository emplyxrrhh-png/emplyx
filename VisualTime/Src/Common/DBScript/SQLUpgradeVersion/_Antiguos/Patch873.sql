-- No borréis esta línea
-- Actualiza el campo de equivalencia
ALTER TABLE [dbo].[DiningRoomTurns] ALTER COLUMN [Export] [nvarchar](15) NULL
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='873' WHERE ID='DBVersion'

GO
