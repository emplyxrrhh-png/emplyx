update dbo.ImportGuides set version = 2 where id>=20
GO
update dbo.ExportGuides set version = 2 where id>=20000
GO

ALTER TABLE dbo.ImportGuides ADD [PreProcessScript] [nvarchar](50) NULL
GO
ALTER TABLE dbo.ExportGuides ADD [PostProcessScript] [nvarchar](50) NULL
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='464' WHERE ID='DBVersion'
GO
