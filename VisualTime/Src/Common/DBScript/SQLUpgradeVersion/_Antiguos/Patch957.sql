UPDATE [dbo].[sysroUserFields] set [Unique] = 1 where [Alias] = 'sysroVisualtimeID'
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='957' WHERE ID='DBVersion'
GO
