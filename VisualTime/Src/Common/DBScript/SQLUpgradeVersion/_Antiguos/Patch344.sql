-- Actualizamos versión de BBDD  VALUES

-- Esta exportación de fichajes es especial para Lyreco, por lo que se quita del standard
DELETE FROM EXPORTGUIDES WHERE ID=9007 
GO           

UPDATE sysroParameters SET Data='344' WHERE ID='DBVersion'
GO


