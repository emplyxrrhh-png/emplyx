-- Añade opción de cambio de contraseña
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,SecurityFlags,Priority,AllowedSecurity)
  VALUES ('Menu\Tools\ChangePassword','ChangePassword','fn:\\ShowPassword','3333333333',585,'NW')
GO
INSERT INTO sysroGUI(IDPath,LanguageReference,SecurityFlags,Priority,AllowedSecurity)
  VALUES ('Menu\Tools\S5','^SEPARATOR','3111111111',587,'NR')
GO

-- Modifica la tabla de Shifts, para añadir el campo ShortName
ALTER TABLE [dbo].[Shifts] ADD ShortName NVARCHAR(3)
GO


-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='141' WHERE ID='DBVersion'
GO

