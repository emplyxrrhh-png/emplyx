--
-- Cambiar tipos de campos IDTemplate a String
-- 05/2004

-- Campos para conocer de que orden teorica viene una real (se cumplimenta al lanzar la orden)
ALTER TABLE [dbo].[Jobs] ALTER COLUMN [IDTemplate] [nvarchar] (50)
GO
ALTER TABLE [dbo].[Orders] ALTER COLUMN [IDTemplate] [nvarchar] (50)
GO


--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='175' WHERE ID='DBVersion'
GO
