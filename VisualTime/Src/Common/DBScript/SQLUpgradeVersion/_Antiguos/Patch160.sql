
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity) 
	VALUES ('Menu\StartUP','VTStartUp','roFormStartup.vbd','','3111111111',400,'NR')
GO

--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='160' WHERE ID='DBVersion'
GO

