--
-- Actualizamos la tabla sysroGUI nueva opción de menú Ver / Ir a página inicio
-- 05/2004

INSERT INTO [dbo].[sysroGUI] (IDPath, LanguageReference, URL, SecurityFlags, Priority, AllowedSecurity) VALUES ('Menu\View\GoHomePage', 'GoHomePage', 'fn://ShowStartupForm', '3111111111', '545', 'NR')
GO

-- Añadir campo para conocer desde que terminal se ha dado de alta cada huella de empleado
ALTER TABLE [dbo].[Employees] ADD [BiometricIDTerminal] [int] 
GO

-- Añadir campo para conocer de que orden teorica viene una real (se cumplimenta al lanzar la orden)
ALTER TABLE [dbo].[Jobs] ADD [IDTemplate] [numeric] (18)
GO
ALTER TABLE [dbo].[Orders] ADD [IDTemplate] [numeric] (18)
GO


--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='174' WHERE ID='DBVersion'
GO
