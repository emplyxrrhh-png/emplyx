-- Aumentamos el tamaño del campo Path para poder tener mayor profuncidad en el árbol de departamentos
ALTER TABLE Terminals ADD skin tinyint
GO

--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='197' WHERE ID='DBVersion'
GO


