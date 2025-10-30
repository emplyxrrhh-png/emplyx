--
-- Actualiza sysroEntryTypes 
--
INSERT INTO dbo.sysroEntryTypes (Type, Description, Context) VALUES ('L','Accesos y Presencia integrado','PRESENCIA')
GO
INSERT INTO dbo.sysroEntryTypes (Type, Description, Context) VALUES ('I','Accesos no valido','ACCESOS')
GO
-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='149' WHERE ID='DBVersion'
GO
