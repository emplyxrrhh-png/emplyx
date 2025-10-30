-- Nuevo tipo de solicitud para webTerminal
if not exists(SELECT * FROM sysroRequestType where IdType = 8)
Insert Into sysroRequestType (IdType, Type) VALUES (8,'Just.Fichaje')
GO
alter table dbo.Employees add AllowRequestMoveReason bit DEFAULT(0)
GO
update dbo.Employees set AllowRequestMoveReason=0
GO
ALTER TABLE dbo.Terminals add AllowMoveReason bit default(1)
GO
update dbo.Terminals set AllowMoveReason=1
GO
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='207' WHERE ID='DBVersion'
GO





