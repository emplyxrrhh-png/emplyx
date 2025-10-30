-- Campo para controlar si se debe aplicar lógica Win32 o Live a las ausencias previstas por horas (clientes provenientes de actualización R3 a Live)
ALTER TABLE dbo.ProgrammedCauses ADD Win32 [BIT] NOT NULL DEFAULT(0)
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='284' WHERE ID='DBVersion'
GO
