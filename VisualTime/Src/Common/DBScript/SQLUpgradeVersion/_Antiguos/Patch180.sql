-- PANTALLA DE EMPLEADOS LOGIN AUTENTIFICACION DE WINDOWS
ALTER TABLE dbo.Employees ADD BiometricID int NULL
GO

update dbo.Employees set biometricID = id 
GO

--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='180' WHERE ID='DBVersion'
GO
