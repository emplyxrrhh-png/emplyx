IF NOT EXISTS (SELECT ID FROM sysroParameters where ID = 'CACHEINIT')
BEGIN
	insert into sysroParameters(id,Data) values('CACHEINIT','1900/01/01 00:00:00')
END
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='993' WHERE ID='DBVersion'
GO
