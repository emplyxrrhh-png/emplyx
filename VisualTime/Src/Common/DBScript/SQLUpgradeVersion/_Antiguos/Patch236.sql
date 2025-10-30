-- Actualizamos las solicitudes de fichajes olvidados que sean sin justificación a la justificacion de NO JUSTIFICADO
UPDATE WTRequest SET Cause=0 WHERE Cause IS NULL AND RequestType = 1
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='236' WHERE ID='DBVersion'
GO
