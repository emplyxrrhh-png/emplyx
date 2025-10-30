--Actualización de la tabla sysrogui (debido a que el botón de saldos se mostraba siempre al no tener una licencia asociada)
UPDATE sysrogui SET RequiredFeatures = 'Forms\Concepts' WHERE IDPath = 'Portal\ShiftControl\Accruals'
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='255' WHERE ID='DBVersion'
GO
