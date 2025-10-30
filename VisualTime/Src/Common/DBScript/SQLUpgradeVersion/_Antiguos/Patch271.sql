INSERT INTO [sysroNotificationTypes] ([ID],[Name],[Scheduler])
VALUES (16,'End Period Employee',120)
GO

INSERT INTO [sysroNotificationTypes] ([ID],[Name],[Scheduler])
VALUES (17,'End Period Enterprise',120)
GO

ALTER PROCEDURE [dbo].[Visits_VisitPlan_GetFiltered]
     @selectedDate as smalldatetime,   -- Fecha de la visita solicitada   
     @onlyOwnVisits as bit = 0,  -- Sólo mostrar las visitas propias
     @idEmployee as int = -1,  -- Id del empleado que está realizando la consulta (para el caso de que @onlyOwnVisits sea verdadero
     @visitStatus as int = -1,  -- Estado de la visita
     @timeFrom as smalldatetime = NULL,  -- Hora del día a partir de la que quiero ver las visitas
     @timeTo as smalldatetime = NULL,  -- Hora del día hasta la que quiero ver las visitas
     @visitorName as nvarchar(50) = '',  -- Nombre del visitante (permite búsquedas parciales)
     @visitorCompany as nvarchar(50) = '',   -- Empresa del visitante (permite búsquedas parciales) 
     @visitorNIF as nvarchar(20) = '',  -- NIF del visitante
     @employeeVisitedName as nvarchar(50) = '',  --Id del empleado visitado
     @visitPlanTicket as int = -1,  -- Identificador numérico de la visita
   	@visitType as int = -1, -- Tipo de visita
 	@visitLocation as int = -1 -- Ubicación de la visita	
 AS
 	DECLARE @sSQL nvarchar(2000)
 	DECLARE @sWHERE nvarchar(2000)
         SET @sWHERE = ''
         SET @sSQL ='SELECT * FROM sysroVisitPlan '
         SET @sSQL = @SSQL + ' WHERE year([Date]) =  ' + str(year(@selectedDate)) + ' AND month([Date]) =  ' + str(month(@selectedDate)) + '  AND day([Date]) =  ' + str(day(@selectedDate))
 		--Restricciones
         IF @visitStatus <> -1 SET @sWHERE = @sWHERE + ' AND Status = ' + str(@visitStatus)
         IF @visitorName <>'' SET @sWHERE = @sWHERE + ' AND (VisitorName LIKE ''' + @visitorName + ''' OR VisitorAlias LIKE ''' + @visitorName + ''')'
         IF @visitorNIF <> '' SET @sWHERE = @sWHERE + ' AND VisitorNIF = ''' + @visitorNIF +''''
         IF @employeeVisitedName <> '' SET @sWHERE = @sWHERE + ' AND EmployeeVisitedName LIKE ''' + @employeeVisitedName + ''''
         IF @visitPlanTicket <> -1 SET @sWHERE = @sWHERE + ' AND Ticket = ' + str(@visitPlanTicket)
   		IF @visitType <> -1 SET @sWHERE = @sWHERE + ' AND Type = ' + str(@visitType)
 		IF @visitLocation <> -1 SET @sWHERE = @sWHERE + ' AND IDLocation = ' + str(@visitLocation)
         IF @sWHERE<>'' SET @sSQL = @sSQL + @sWHERE 
 		-- Finalmente ordenamos por hora ascendente
 		SET @sSQL = @sSQL + ' ORDER BY Date DESC, BeginTime DESC '
 EXEC sp_executesql @statement = @sSQL
 GO
 
CREATE PROCEDURE [dbo].[Visits_VisitPlan_GetAllByPeriodAndStatus]
  	@BeginDate smalldatetime,
  	@EndDate smalldatetime,
  	@Status int,
  	@Location int
  AS
 	IF @Status = 1 Or @Status=4 
 		IF @Location = -1
 			SELECT * FROM sysroVisitPlan WHERE [Date] >= @BeginDate AND [Date] <= @EndDate AND [Status] = @Status AND (IDLocation IS NULL OR IDLocation >= 0) ORDER BY [Date] DESC		
 		ELSE
 			SELECT * FROM sysroVisitPlan WHERE [Date] >= @BeginDate AND [Date] <= @EndDate AND [Status] = @Status AND IDLocation = @Location ORDER BY [Date] DESC
 	ELSE IF @Status = 2 
 		IF @Location = -1
 			SELECT * FROM sysroVisitPlan WHERE BeginTime >= @BeginDate AND BeginTime<=@EndDate AND EndTime IS NULL AND [Status] = @Status AND (IDLocation IS NULL OR IDLocation >= 0) ORDER BY [Date] DESC, BeginTime DESC
 		ELSE
 			SELECT * FROM sysroVisitPlan WHERE BeginTime >= @BeginDate AND BeginTime<=@EndDate AND EndTime IS NULL AND [Status] = @Status AND IDLocation = @Location ORDER BY [Date] DESC, BeginTime DESC
 	ELSE
 		IF @Location = -1
 			SELECT * FROM sysroVisitPlan WHERE BeginTime >= @BeginDate AND EndTime <= @EndDate AND [Status] = @Status AND (IDLocation IS NULL OR IDLocation >= 0) ORDER BY [Date] DESC, BeginTime DESC
 		ELSE
 			SELECT * FROM sysroVisitPlan WHERE BeginTime >= @BeginDate AND EndTime <= @EndDate AND [Status] = @Status AND IDLocation = @Location ORDER BY [Date] DESC, BeginTime DESC
 			
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='271' WHERE ID='DBVersion'
GO
