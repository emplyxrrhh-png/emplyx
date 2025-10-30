-- No borréis esta línea
ALTER TABLE DiningRoomTurns ADD Export nvarchar(5) NULL
GO
UPDATE DiningRoomTurns Set Export = '0' where Export is null
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='808' WHERE ID='DBVersion'
GO
