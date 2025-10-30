-- Ampliamos el SecurityFlags de algunos elementos de la sysroGUI para permitir más grupos
--

update sysrogui set SecurityFlags = SecurityFlags+'11111111111111111111' where len(SecurityFlags) < 15
GO


-- Modificación de Stored de Visitas para saber si el empleado está presente o no.
--
set ANSI_NULLS OFF
set QUOTED_IDENTIFIER OFF
GO

 ALTER PROCEDURE [dbo].[Visits_Employee_IsIn] 
 	@employeeId int
 AS
 	DECLARE @lastAccessDate smalldatetime;
 	DECLARE @lastPunchIn smalldatetime;
 	DECLARE @lastPunchOut smalldatetime;
	DECLARE @isWorkingZone bit;
    -- ULTIMO FICHAJE DE ACCESOS
       Select Top 1 @lastAccessDate = DateTime, @isWorkingZone = IsWorkingZone  From AccessMoves,Zones, sysrosubvwCurrentEmployeePeriod Where AccessMoves.IDZone = Zones.ID And AccessMoves.IDEmployee = str(@employeeId) And AccessMoves.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee  Order by DateTime desc
    -- ULTIMO FICHAJE DE ENTRADA DE PRESENCIA
 	Select Top 1 @lastPunchIn = InDateTime From Moves, sysrosubvwCurrentEmployeePeriod Where Moves.IDEmployee = str(@employeeId)  AND Moves.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee Order by InDateTime desc
    -- ULTIMO FICHAJE DE SALIDA DE PRESENCIA
 	Select Top 1 @lastPunchOut = OutDateTime From Moves, sysrosubvwCurrentEmployeePeriod Where Moves.IdEmployee = str(@employeeId)  AND Moves.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee Order by OutDateTime desc

	IF @lastAccessDate = NULL and @lastPunchIn = NULL and @lastPunchOut = NULL
		-- Si no hay ningún fichaje de ningún tipo, estoy ausente
		BEGIN
			RETURN 0
		END
	ELSE
		IF @lastAccessDate = NULL set @lastAccessDate = convert(smalldatetime,'1900-01-01 00:00',120)
		IF @lastPunchIn = NULL  set @lastPunchIn = convert(smalldatetime,'1900-01-01 00:00',120)
		IF @lastPunchOut = NULL  set @lastPunchOut = convert(smalldatetime,'1900-01-01 00:00',120)
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
UPDATE sysroParameters SET Data='205' WHERE ID='DBVersion'
GO





