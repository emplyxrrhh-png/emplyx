-- Modificamos la tabla de movimientos para indicar si el fichaje es sospechosos
ALTER TABLE dbo.MOVES
ADD IsNotReliableIN bit default(0),
IsNotReliableOUT bit default(0)
GO

-- ORDENAMOS LOS ACUMULADOS PARA LAS CONSULTAS POR EL TERMINAL
ALTER TABLE dbo.Concepts 
ADD Pos tinyint null default(0)
GO

update dbo.Concepts Set Pos=0 where pos is null
GO

CREATE PROCEDURE [dbo].[ActualizaPos] AS
BEGIN
	DECLARE @Pos int;
	DECLARE @Cont int;
	DECLARE @ID int;
	--- OBTENEMOS LOS ACUMULADOS 
	DECLARE crs CURSOR FOR

    SELECT Pos, id from concepts Where ViewInTerminals=1 order by name asc
	OPEN crs;
	FETCH NEXT FROM crs INTO @Pos,@ID;
	SET @Cont = 0
	WHILE @@FETCH_STATUS = 0 BEGIN
		SET @Cont = @Cont  + 1 ;
		SET @Pos = @Cont;
		update concepts set Pos=@Pos where id=@ID
	-- VAMOS AL SIGUIENTE REGISTRO
	FETCH NEXT FROM crs INTO @Pos,@ID;

	END;
	CLOSE crs;
	DEALLOCATE crs;
END
GO

-- Lo ejecutamos
EXEC [ActualizaPos]
GO

-- Lo eliminamos
DROP PROCEDURE [dbo].[ActualizaPos]
GO

-- Creamos nuevos campos para calcular el saldo de vacaciones
ALTER TABLE dbo.Shifts 
ADD IDConceptBalance smallint null default(0),
AreWorkingDays bit null default(0)
GO

-- Modificamos la tabla de justificaciones para determinar si la justificacion es sospechosa
ALTER TABLE dbo.DailyCauses
ADD IsNotReliable bit default(0)
GO


-- Creamos nuevos campos de permisos en solicitudes
ALTER TABLE dbo.Employees add
AllowRequestsOnTerminal bit null default(0),
AllowRequestLeave bit null default(0),
AllowRequestShift bit null default(0),
HolidaysShift smallint null default(0),
AllowRequestProgrammedAbs  bit null default (0),
AllowRequestAnticipatedInc bit null default(0),
AllowRequestMove bit null default(0),
AllowRequestExternalJob bit null default(0),
AllowRequestShiftChange bit null default(0)
GO


-- Actualizamos los permisos de solicitud
-- Si había solicitudes de webTeminal, las habilitamos por defecto por compatibilidad. Si no, no.
IF NOT EXISTS (select * from wtrequest)
UPDATE  dbo.Employees SET AllowRequestsOnTerminal =0 WHERE  AllowRequestsOnTerminal IS NULL
ELSE
UPDATE  dbo.Employees SET AllowRequestsOnTerminal =1 WHERE  AllowRequestsOnTerminal IS NULL
GO

IF NOT EXISTS (select * from wtrequest)
UPDATE  dbo.Employees SET AllowRequestShiftChange =0 WHERE  AllowRequestShiftChange IS NULL
ELSE
UPDATE  dbo.Employees SET AllowRequestShiftChange =1 WHERE  AllowRequestShiftChange IS NULL
GO

IF NOT EXISTS (select * from wtrequest)
UPDATE  dbo.Employees SET AllowRequestLeave =0 WHERE  AllowRequestLeave IS NULL
ELSE
UPDATE  dbo.Employees SET AllowRequestLeave =1 WHERE  AllowRequestLeave IS NULL
GO

IF NOT EXISTS (select * from wtrequest)
UPDATE  dbo.Employees SET AllowRequestShift =0 WHERE  AllowRequestShift IS NULL
ELSE
UPDATE  dbo.Employees SET AllowRequestShift =1 WHERE  AllowRequestShift IS NULL
GO

IF NOT EXISTS (select * from wtrequest)
UPDATE  dbo.Employees SET AllowRequestProgrammedAbs =0 WHERE  AllowRequestProgrammedAbs IS NULL
ELSE
UPDATE  dbo.Employees SET AllowRequestProgrammedAbs =1 WHERE  AllowRequestProgrammedAbs IS NULL
GO

IF NOT EXISTS (select * from wtrequest)
UPDATE  dbo.Employees SET AllowRequestAnticipatedInc =0 WHERE  AllowRequestAnticipatedInc IS NULL
ELSE
UPDATE  dbo.Employees SET AllowRequestAnticipatedInc =1 WHERE  AllowRequestAnticipatedInc IS NULL
GO

IF NOT EXISTS (select * from wtrequest)
UPDATE  dbo.Employees SET AllowRequestMove =0 WHERE  AllowRequestMove IS NULL
ELSE
UPDATE  dbo.Employees SET AllowRequestMove =1 WHERE  AllowRequestMove IS NULL
GO

IF NOT EXISTS (select * from wtrequest)
UPDATE  dbo.Employees SET AllowRequestExternalJob =0 WHERE  AllowRequestExternalJob IS NULL
ELSE
UPDATE  dbo.Employees SET AllowRequestExternalJob =1 WHERE  AllowRequestExternalJob IS NULL
GO


UPDATE sysroParameters SET Data='202' WHERE ID='DBVersion'
GO

