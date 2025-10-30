-- Se añade un nuevo campo en la tabla de usuarios para bloquear la cuenta
ALTER TABLE SysroUsers add Locked bit null default(0)
GO
UPDATE SysroUsers set Locked = 0 where Locked is null
GO


UPDATE sysroParameters SET Data='200' WHERE ID='DBVersion'
GO


