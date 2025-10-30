-- Servicios 2.0 
ALTER TABLE dbo.Notifications ADD [CreatorID] [nvarchar](50) NULL
GO

-- Marcamos las tareas que crea el servidor de Visualtime para poder filtrar
ALTER TABLE SYSROTASKS ADD SystemTask bit null default(0)
GO
UPDATE SYSROTASKS set SystemTask=0 where SystemTask IS NULL
GO



-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='253' WHERE ID='DBVersion'
GO
