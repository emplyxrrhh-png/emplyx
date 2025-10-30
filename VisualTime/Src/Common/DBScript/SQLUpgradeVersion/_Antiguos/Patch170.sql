-- Elimina campos obsoletos
ALTER TABLE [dbo].[EmployeeJobMoves] DROP CONSTRAINT DF_EmployeeJobMoves_ISExported
GO

ALTER TABLE [dbo].[TeamJobMoves] DROP CONSTRAINT DF_TeamJobMoves_ISExported
GO


ALTER TABLE [dbo].[EmployeeJobMoves] DROP COLUMN [IsExported]
GO


ALTER TABLE [dbo].[TeamJobMoves] DROP COLUMN [IsExported]
GO

-- Elimina tablas obsoletas
DROP TABLE AccessGroupZones
GO

DROP TABLE AccessGroupsTimeZones
GO

--
-- Actualiza tablas de piezas de produccion para marcar los tipos que utilizamos
--

ALTER TABLE [dbo].[sysroPieceTypes] ADD [IsUsed] [bit] NULL 
GO

UPDATE sysroPieceTypes SET IsUsed=1 where ID=1
GO

UPDATE sysroPieceTypes SET IsUsed=0 where ID<>1
GO


--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='170' WHERE ID='DBVersion'
GO
