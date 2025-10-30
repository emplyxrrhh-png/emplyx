
-- Nuevos tipos de identificación por Terminal
alter table employees add AllowCardPlusBio bit default(0)
GO
alter table employees add AllowBioPriority bit default(0)
GO

--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='187' WHERE ID='DBVersion'
GO
