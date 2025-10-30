--MODIFICACIONES DE LOS STOREDS DE PLANIFICACIÓN DE VISITAS
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
		SET @sSQL = @sSQL + ' ORDER BY Date ASC '
EXEC sp_executesql @statement = @sSQL
GO

ALTER PROCEDURE [dbo].[Visits_Employee_IsIn]
   	@employeeId int
   AS
   	DECLARE @lastAccessDate smalldatetime;
   	DECLARE @lastPunchIn smalldatetime;
   	DECLARE @lastPunchOut smalldatetime;
  	DECLARE @isWorkingZone bit;
 	DECLARE @isLivePunches bit;
 	if exists (select * from Punches)
 		BEGIN
 			set @isLivePunches = 1
 		END
 	ELSE
 		BEGIN
 			set @isLivePunches = 0
 		END
     if (@isLivePunches = 0)
 		begin
 			 -- ULTIMO FICHAJE DE ACCESOS
 			Select Top 1 @lastAccessDate = DateTime, @isWorkingZone = IsWorkingZone  From AccessMoves,Zones, sysrosubvwCurrentEmployeePeriod Where AccessMoves.IDZone = Zones.ID And AccessMoves.IDEmployee = str(@employeeId) And AccessMoves.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee  Order by DateTime desc
 			 -- ULTIMO FICHAJE DE ENTRADA DE PRESENCIA
 			Select Top 1 @lastPunchIn = InDateTime From Moves, sysrosubvwCurrentEmployeePeriod Where Moves.IDEmployee = str(@employeeId)  AND Moves.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee Order by InDateTime desc
 			 -- ULTIMO FICHAJE DE SALIDA DE PRESENCIA
 			Select Top 1 @lastPunchOut = OutDateTime From Moves, sysrosubvwCurrentEmployeePeriod Where Moves.IdEmployee = str(@employeeId)  AND Moves.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee Order by OutDateTime desc
 		end
 	else
 		BEGIN
 			 -- ULTIMO FICHAJE DE ACCESOS
 			Select Top 1 @lastAccessDate = DateTime, @isWorkingZone = IsWorkingZone  From Punches,Zones, sysrosubvwCurrentEmployeePeriod Where Punches.IDZone = Zones.ID And Punches.IDEmployee = str(@employeeId) And Punches.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee AND Type = 5 Order by DateTime desc
 			 -- ULTIMO FICHAJE DE ENTRADA DE PRESENCIA
 			Select Top 1 @lastPunchIn = DateTime From Punches, sysrosubvwCurrentEmployeePeriod Where Punches.IDEmployee = str(@employeeId)  AND Punches.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee AND Type = 1 Order by DateTime desc
 			 -- ULTIMO FICHAJE DE SALIDA DE PRESENCIA
 			Select Top 1 @lastPunchOut = DateTime From Punches, sysrosubvwCurrentEmployeePeriod Where Punches.IdEmployee = str(@employeeId)  AND Punches.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee AND Type = 2 Order by DateTime desc
 		END
 			
  	
	IF ISDATE(@lastAccessDate) <> 1 and ISDATE(@lastPunchIn) <> 1 and ISDATE(@lastPunchIn) <> 1
  		-- Si no hay ningún fichaje de ningún tipo, estoy ausente
  		BEGIN
  			RETURN 0
  		END
  	ELSE
  		IF ISDATE(@lastAccessDate) <> 1 set @lastAccessDate = convert(smalldatetime,'1900-01-01 00:00',120)
  		IF ISDATE(@lastPunchIn) <> 1 set @lastPunchIn = convert(smalldatetime,'1900-01-01 00:00',120)
  		IF ISDATE(@lastPunchOut) <> 1 set @lastPunchOut = convert(smalldatetime,'1900-01-01 00:00',120)
  		-- CALCULOS
   		IF @lastPunchIn >= @lastPunchOut and @lastPunchIn >= @lastAccessDate
   			-- Si lo último es una entrada de presencia ... estoy presente
  			BEGIN
  				RETURN 1
  			END
   		ELSE
  			BEGIN
  				IF @lastPunchOut >= @lastPunchIn and @lastPunchOut >= @lastAccessDate 
  					-- Si lo último es una salida de presencia ... estoy ausente
  					BEGIN
  						RETURN 0
  					END
  				ELSE
  					BEGIN
   						IF @lastAccessDate >= @lastPunchOut and @lastAccessDate >= @lastPunchIn and @isWorkingZone = 1 
  							-- Si lo último es un fichaje de accesos y es a una zona de trabajo ... estoy presente
  							BEGIN
  								RETURN 1
  							END
   						ELSE 
  							BEGIN
  								-- Si lo último es un fichaje de accesos y es a una zona de NO trabajo ... estoy ausente
  								RETURN 0
  							END
  					END
  			END

GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='288' WHERE ID='DBVersion'
GO
