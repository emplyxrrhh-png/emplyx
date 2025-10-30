-- No borréis esta línea
DELETE sysroQueries WHERE Description LIKE '%V2%'
GO
DELETE sysroQueries WHERE Description LIKE '%GPA%'
GO
DELETE sysroQueries WHERE Description LIKE '%informe planificado%'
GO

DELETE sysroQueries WHERE Name = 'Permisos sobre solicitudes pendientes'
GO

INSERT INTO dbo.sysroQueries (Name, Description, Value, Parameters) VALUES ('Permisos sobre solicitudes pendientes', 'Muestra los permisos de los distintos supervisores sobre las solicitudes pendientes, incluyendo si el supervisor es o no directo para cada solicitud', '
SELECT	EMP.Name Solicitante,
    CASE 
        WHEN REQ.RequestType = 1 THEN ''Ficha''
        WHEN REQ.RequestType = 2 THEN ''Fichaje Olvidado''
        WHEN REQ.RequestType = 3 THEN ''Justificación fichaje''
        WHEN REQ.RequestType = 4 THEN ''Parte de trabajo externo''
        WHEN REQ.RequestType = 5 THEN ''Cambio de horario''
        WHEN REQ.RequestType = 6 THEN ''Vacaciones o permiso''
        WHEN REQ.RequestType = 7 THEN ''Previsión ausencia por días''
        WHEN REQ.RequestType = 8 THEN ''Intercambio de horario''
        WHEN REQ.RequestType = 9 THEN ''Previsión ausencia por horas''
        WHEN REQ.RequestType = 10 THEN ''Fichaje de tareas olvidado''
        WHEN REQ.RequestType = 11 THEN ''Cancelación de vacaciones''
        WHEN REQ.RequestType = 12 THEN ''Fichaje de centros de coste olvidado''
        WHEN REQ.RequestType = 13 THEN ''Previsión de vacaciones''
        WHEN REQ.RequestType = 14 THEN ''Previsión de horas de exceso''
        WHEN REQ.RequestType = 15 THEN ''Resumen de trabajo externo''
        WHEN REQ.RequestType = 16 THEN ''Teletrabajo''
        WHEN REQ.RequestType = 17 THEN ''Declaración de jornada''
        ELSE ''?'' END AS [Tipo Solicitud],
		REQ.ID AS [Identificador de solicitud],
		ISNULL(CAU.Name, '''') AS [Justificación],
		ISNULL(SHI.Name, '''') AS [Horario],
		REQ.RequestDate AS [Fecha de solicitud],
		REQ.Date1 AS [Fecha Inicio],
		REQ.Date2 AS [Fecha Fin],
		SP.Name Supervisor,
		CASE DirectDependence WHEN 1 THEN ''SI'' ELSE ''NO'' END AS [Es supervisor directo], 
		SupervisorLevelOfAuthority [Nivel de Autorización Supervisor] , 
		CASE Permission WHEN 9 THEN ''Administración'' WHEN 6 THEN ''Escritura'' WHEN 3 THEN ''Lectura'' WHEN 0 THEN ''Sin permiso'' END AS [Permiso sobre solicitud], 
		CASE RequestCurrentStatus WHEN 0 THEN ''Pendiente'' WHEN 1 THEN ''En curso'' WHEN 2 THEN ''Aprobada'' WHEN 3 THEN ''Denegada'' END AS [Estado solicitud],  
		RequestCurrentApprovalLevel [Nivel de Autorización Actual], 
		NextLevelOfAuthorityRequired [Siguiente Nivel de Autorización]
FROM sysrovwSecurity_PendingRequestsDependencies SPRD
INNER JOIN sysroPassports SP ON SP.ID = SPRD.IdPassport AND SPRD.IsRoboticsUser = 0
INNER JOIN Employees EMP ON EMP.id = SPRD.IdEmployee
INNER JOIN Requests REQ ON REQ.id = SPRD.IdRequest
LEFT JOIN Causes CAU ON CAU.ID = REQ.IDCause
LEFT JOIN Shifts SHI ON SHI.ID = REQ.IDShift
OPTION (USE HINT(''QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150''));
', NULL)
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='849' WHERE ID='DBVersion'

GO
