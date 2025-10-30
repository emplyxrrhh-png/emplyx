-- Actualizamos campos de produccion para poder utilizar piezas o no

ALTER TABLE [dbo].[Jobs] ADD [AllowPieces] [bit] NULL
GO
ALTER TABLE [dbo].[JobTemplates] ADD [AllowPieces] [bit] NULL
GO
update jobs set allowpieces=1 where allowpieces is null
go
update jobtemplates set allowpieces=1 where allowpieces is null
go

--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='161' WHERE ID='DBVersion'
GO

