-- ************* STORED PROCEDURES PARA LA APLICACION VISUALTIME VISITAS **********************

-- *************************Visits_USER_GETUSERBYUSERNAME**************************************
DROP PROCEDURE Visits_User_GetUserByUsername
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE  PROCEDURE Visits_User_GetUserByUsername
	@Username NVarchar(255)
AS
SELECT 
 	ID, WebLogin, AllowVisits, AllowVisitsPlan, AllowVisitsAdmin, Name
FROM 
	Employees
WHERE 
	WebLogin = @Username
GO

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- *************************Visits_USER_AUTHENTICATE**************************************
DROP PROCEDURE Visits_User_Authenticate
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO



CREATE PROCEDURE Visits_User_Authenticate
  @Username NVarChar(255),
  @Password NVarChar(255)
AS
IF EXISTS(select * from employees where webLogin = @Username AND WebPassword = @Password AND (AllowVisits=1 OR AllowVisitsPlan=1 OR AllowVisitsAdmin=1))
  RETURN 0
ELSE
  RETURN -1

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- *************************Visits_User_ChangePassword**************************************
DROP PROCEDURE Visits_User_ChangePassword
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE Visits_User_ChangePassword
@Username NVarChar(255),
@Password NVarChar(255),
@NewPassword NVarChar(255)
AS
UPDATE employees SET WebPassword = @NewPassword  Where webLogin = @Username AND WebPassword = @Password
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- *************************Visits_EMPLOYEE_GETEMPLOYEEBYID**************************************
DROP PROCEDURE Visits_Employee_GetEmployeeByID
GO


SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

CREATE PROCEDURE Visits_Employee_GetEmployeeByID
	@employeeId as int
AS

	DECLARE @isIn bit;
	EXEC @isIn = [Visits_Employee_IsIn] @employeeId

	SELECT *, @isIn as IsIn FROM Employees WHERE [ID] = @employeeId;
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



-- *************************Visits_VISITOR_GETVISITORBYID**************************************
DROP PROCEDURE Visits_Visitor_GetVisitorByID
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE Visits_Visitor_GetVisitorByID
	@visitorId as int
AS
	SELECT * FROM Visitors WHERE [ID] = @visitorId;

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- *************************Visits_VISITOR_INSERT**************************************
DROP PROCEDURE Visits_Visitor_Insert
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO




CREATE   PROCEDURE Visits_Visitor_Insert
@Name NVARCHAR(50),
@NIF NVARCHAR(20),
@Company NVARCHAR(50),
@Position NVARCHAR(50),
@Comments Text,
@Type Int
AS


BEGIN TRAN
	INSERT Visitors
		(Name, NIF, Company, Position, Comments, Type, 	Created)
		VALUES
		(@Name, @NIF, @Company, @Position, @Comments, @Type, getdate() )

COMMIT TRAN
RETURN @@IDENTITY

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- *************************Visits_VISITOR_SEARCH**************************************
DROP PROCEDURE Visits_Visitor_Search
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE  PROCEDURE  Visits_Visitor_Search
	@VisitorNIF NVarchar(255),
	@VisitorName NVarchar(255),
	@VisitorCompany NVarchar(255)
AS
-- Definición de las variables que se utilizarán para crear la setencia SQL
DECLARE @sSQL nvarchar(2000)
DECLARE @sWHERE nvarchar(2000)

SET @VisitorName = '%' + @VisitorName +'%'
set @VisitorCompany= '%' + @VisitorCompany + '%'

SET @sSQL ='SELECT ID, Name, NIF, Company, Position, Comments,Type, Created, LastVisitDate FROM Visitors '
SET @sSQL = @SSQL + ' WHERE Company like ''' + @VisitorCompany + ''' AND Name like ''' + @VisitorName +''''

--Restricciones
if @VisitorNIF <>''
begin
	SET @sWHERE = ' AND NIF =''' + @VisitorNIF + ''''
end

IF @sWHERE<>''
BEGIN
	SET @sSQL = @sSQL + @sWHERE 
END

SET @sSQL = @sSQL + ' ORDER BY Name'

EXEC sp_executesql @statement = @sSQL

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- *************************Visits_VISITPLAN_DELETE**************************************
DROP PROCEDURE Visits_VisitPlan_Delete
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE PROCEDURE dbo.Visits_VisitPlan_Delete
(
	@Original_ID int
)
AS
	SET NOCOUNT OFF;
DELETE FROM VisitPlan WHERE (ID = @Original_ID)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- *************************Visits_VISITPLAN_GETALLBYDATE**************************************
DROP PROCEDURE Visits_VisitPlan_GetAllByDate
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO



CREATE  PROCEDURE Visits_VisitPlan_GetAllByDate
	@date smalldatetime
AS
SELECT 
	*
FROM 
	VisitPlan
WHERE
	year([Date]) =  year(@date) 
             AND month([Date]) =  month(@date) 
             AND day([Date]) =  day(@date) 
ORDER BY
	[Date] Asc

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



-- *************************Visits_VISITPLAN_INSERT**************************************
DROP PROCEDURE Visits_VisitPlan_Insert
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE PROCEDURE dbo.Visits_VisitPlan_Insert
(
	@VisitorAlias nvarchar(50),
	@VisitorId int,
	@EmpVisitedId int,
	@Date smalldatetime,
	@Comments text,
	@Status int,
	@PlannedById int,
	@Type int
)
AS
	SET NOCOUNT OFF;
INSERT INTO VisitPlan(VisitorAlias, VisitorId, EmpVisitedId, Date, Comments, Status, PlannedById, Type, PlannedDate) VALUES (@VisitorAlias, @VisitorId, @EmpVisitedId, @Date, @Comments, @Status, @PlannedById, @Type, getdate());
--	SELECT ID, VisitorAlias, VisitorId, EmpVisitedId, Date, Comments, Status, PlannedById, Type FROM VisitPlan WHERE (ID = @@IDENTITY)
RETURN @@IDENTITY

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- *************************Visits_VISITPLAN_SELECT**************************************
DROP PROCEDURE Visits_VisitPlan_Select
GO


SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE PROCEDURE dbo.Visits_VisitPlan_Select
AS
	SET NOCOUNT ON;
SELECT ID, VisitorAlias, VisitorId, EmpVisitedId, Date, Comments, Status, PlannedById, Type FROM VisitPlan

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- *************************Visits_VISITPLAN_UPDATE**************************************
DROP PROCEDURE Visits_VisitPlan_Update
GO


SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE PROCEDURE dbo.Visits_VisitPlan_Update
(
	@VisitorAlias nvarchar(50),
	@VisitorId int,
	@EmpVisitedId int,
	@Date smalldatetime,
	@Comments text,
	@Status int,
	@Type int,
	@ID int
)
AS
	SET NOCOUNT OFF;
UPDATE VisitPlan SET VisitorAlias = @VisitorAlias, VisitorId = @VisitorId, EmpVisitedId = @EmpVisitedId, Date = @Date, Comments = @Comments, Status = @Status, Type = @Type WHERE (ID = @ID) ;
	SELECT ID, VisitorAlias, VisitorId, EmpVisitedId, Date, Comments, Status, PlannedById, Type FROM VisitPlan WHERE (ID = @ID)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- *************************Visits_VISITTYPES_DELETE**************************************
DROP PROCEDURE Visits_VisitTypes_Delete
GO


SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE PROCEDURE dbo.Visits_VisitTypes_Delete
(
	@ID int
)
AS
	SET NOCOUNT OFF;
DELETE FROM VisitTypes WHERE (ID = @ID)
RETURN @@ROWCOUNT

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- *************************Visits_VISITTYPES_GETALL**************************************
DROP PROCEDURE Visits_VisitTypes_GetAll
GO


SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE PROCEDURE dbo.Visits_VisitTypes_GetAll
AS
	SET NOCOUNT ON;
SELECT ID, Name, ImageURL FROM VisitTypes

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO




-- *************************Visits_VISITTYPES_INSERT**************************************
DROP PROCEDURE Visits_VisitTypes_Insert
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE PROCEDURE dbo.Visits_VisitTypes_Insert
(
	@Name nvarchar(50),
	@ImageURL nvarchar(50)
)
AS
	SET NOCOUNT OFF;
INSERT INTO VisitTypes(Name, ImageURL) VALUES (@Name, @ImageURL);
	SELECT ID, Name, ImageURL FROM VisitTypes WHERE (ID = @@IDENTITY);
	RETURN @@ROWCOUNT

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- *************************Visits_VISITTYPES_UPDATE**************************************

DROP PROCEDURE Visits_VisitTypes_Update
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE PROCEDURE dbo.Visits_VisitTypes_Update
(
	@Name nvarchar(50),
	@ImageURL nvarchar(50),
	@ID int
)
AS
	SET NOCOUNT OFF;
UPDATE VisitTypes SET Name = @Name, ImageURL = @ImageURL WHERE (ID = @ID);
	SELECT ID, Name, ImageURL FROM VisitTypes WHERE (ID = @ID)

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- *************************Visits_EMPLOYEE_GETALLOWVISITS**************************************
DROP PROCEDURE Visits_Employee_GetAllowVisits
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

/****** Object:  Stored Procedure dbo.Visits_Employee_GetAllowVisits    Script Date: 05/02/2006 21:17:45 ******/

CREATE   PROCEDURE  Visits_Employee_GetAllowVisits
	@Name nvarchar(255)
AS


IF @Name =''
	SELECT *, 1 as IsIn FROM Employees, dbo.sysrosubvwCurrentEmployeePeriod  WHERE [AllowVisits] = 1  and IDEmployee = ID ORDER BY Name;
ELSE
BEGIN
	SET @Name = '%' + @Name +'%'
	SELECT *, 1 as IsIn FROM Employees, dbo.sysrosubvwCurrentEmployeePeriod  WHERE [AllowVisits] = 1 AND [NAME] like @Name and IDEmployee = ID ORDER BY Name;
END
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- *************************Visits_EMPLOYEE_ISIN**************************************

DROP PROCEDURE Visits_Employee_IsIn
GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/****** Object:  Stored Procedure dbo.Visits_Employee_IsIn    Script Date: 05/02/2006 21:17:45 ******/
CREATE PROCEDURE Visits_Employee_IsIn 
	@employeeId int
AS

	DECLARE @lastAccessDate smalldatetime;
	DECLARE @lastPunchIn smalldatetime;
	DECLARE @lastPunchOut smalldatetime;


   -- ULTIMO FICHAJE DE ACCESOS
      Select Top 1 @lastAccessDate = DateTime  From AccessMoves,Zones, sysrosubvwCurrentEmployeePeriod Where AccessMoves.IDZone = Zones.ID And AccessMoves.IDEmployee = str(@employeeId) And AccessMoves.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee  Order by DateTime desc

   -- ULTIMO FICHAJE DE PRESENCIA
   	--  ENTRADA
	Select Top 1 @lastPunchIn = InDateTime From Moves, sysrosubvwCurrentEmployeePeriod Where Moves.IDEmployee = str(@employeeId)  AND Moves.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee Order by InDateTime desc


	-- SALIDA 
	Select Top 1 @lastPunchOut = OutDateTime From Moves, sysrosubvwCurrentEmployeePeriod Where Moves.IdEmployee = str(@employeeId)  AND Moves.IDEmployee = sysrosubvwCurrentEmployeePeriod.IDEmployee Order by OutDateTime desc



     -- CALCULOS
	-- Si Ultima Entrada > Ultima Salida => Está Presente
	IF @lastPunchIn > @lastPunchOut RETURN 1
	ELSE
		IF @lastAccessDate > @lastPunchOut RETURN 1
		ELSE
			RETURN 0;
		-- Si Ultimo Acceso > Ultima salida = > Está Presente
			-- Está Ausente
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO





-- *************************Visits_VISITPLAN_GETVISITPLANBYID**************************************
DROP PROCEDURE Visits_VisitPlan_GetVisitPlanByID
GO

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO




CREATE   PROCEDURE dbo.Visits_VisitPlan_GetVisitPlanByID
	@ID integer
AS
SELECT ID, VisitorAlias, VisitorId, EmpVisitedId, Date, Comments, Status, PlannedById, Type, Ticket, BeginTime, EndTime, PlannedDate
FROM sysroVisitPlan
WHERE [ID] = @ID




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- *************************Visits_VISITPLAN_GETFILTERED**************************************
DROP PROCEDURE Visits_VisitPlan_GetFiltered
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE  PROCEDURE  Visits_VisitPlan_GetFiltered
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
            @visitPlanTicket as int = -1  -- Identificador numérico de la visita
AS

-- Definición de las variables que se utilizarán para crear la setencia SQL
DECLARE @sSQL nvarchar(2000)
DECLARE @sWHERE nvarchar(2000)

            SET @sWHERE = ''
            IF @visitorName <> ''  SET @visitorName = '%' + @visitorName +'%'
            IF @visitorCompany <> ''  SET @visitorCompany= '%' + @visitorCompany + '%'
            IF @employeeVisitedName <> ''  SET @employeeVisitedName = '%' + @employeeVisitedName +'%'

                        -- Sentencia básica, filtrando por fecha
                        SET @sSQL ='SELECT * FROM sysroVisitPlan '
                        SET @sSQL = @SSQL + ' WHERE year([Date]) =  ' + str(year(@selectedDate)) + ' AND month([Date]) =  ' + str(month(@selectedDate)) + '  AND day([Date]) =  ' + str(day(@selectedDate))

                        --Restricciones
                        IF @visitStatus <> -1 SET @sWHERE = @sWHERE + ' AND Status = ' + str(@visitStatus)
                        IF @visitorName <>'' SET @sWHERE = @sWHERE + ' AND (VisitorName LIKE ''' + @visitorName + ''' OR VisitorAlias LIKE ''' + @visitorName + ''')'
                        IF @visitorNIF <> '' SET @sWHERE = @sWHERE + ' AND VisitorNIF = ''' + @visitorNIF +''''
                        IF @employeeVisitedName <> '' SET @sWHERE = @sWHERE + ' AND EmployeeVisitedName LIKE ''' + @employeeVisitedName + ''''
                        IF @visitPlanTicket <> -1 SET @sWHERE = @sWHERE + ' AND Ticket = ' + str(@visitPlanTicket)
                        IF @sWHERE<>'' SET @sSQL = @sSQL + @sWHERE 

                        -- Finalmente ordenamos por hora ascendente
                        SET @sSQL = @sSQL + ' ORDER BY Date ASC '

EXEC sp_executesql @statement = @sSQL

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- *************************Visits_VISITTYPES_GETBYID**************************************
DROP PROCEDURE Visits_VisitTypes_GetById
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO



CREATE PROCEDURE Visits_VisitTypes_GetById
	@visitTypeId as int
AS
	SELECT * FROM VisitTypes WHERE [ID] = @visitTypeId;


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- *************************Visits_VISITOR_UPDATE**************************************
DROP PROCEDURE Visits_Visitor_Update
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


CREATE      PROCEDURE Visits_Visitor_Update
@Id Int,
@Name NVARCHAR(50),
@NIF NVARCHAR(20),
@Company NVARCHAR(50),
@Position NVARCHAR(50),
@Comments Text,
@Type Int
AS


BEGIN TRAN
	UPDATE Visitors
		SET [Name]=@Name, [NIF]=@NIF, [Company]=@Company, [Position]=@Position, 
			[Comments]=@Comments, [Type]=@Type
		WHERE [ID]=@Id		

COMMIT TRAN



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- ************************  Visits_VisitMoves_Delete  ************************

DROP PROCEDURE Visits_VisitMoves_Delete
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE    PROCEDURE dbo.Visits_VisitMoves_Delete
	@ID int
AS
	SET NOCOUNT OFF;
DELETE FROM VisitMoves WHERE ID = @ID


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- ************************  Visits_VisitMoves_Select  ************************
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

DROP   PROCEDURE dbo.Visits_VisitMoves_Select
GO

CREATE   PROCEDURE dbo.Visits_VisitMoves_Select
AS
	SET NOCOUNT ON;
SELECT ID, VisitPlanId, BeginTime, EndTime FROM VisitMoves

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- ************************  Visits_VisitMoves_Update  ************************
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


DROP     PROCEDURE dbo.Visits_VisitMoves_Update
GO

CREATE     PROCEDURE dbo.Visits_VisitMoves_Update
	@ID int
AS
	SET NOCOUNT OFF;
UPDATE VisitMoves SET EndTime = getdate() WHERE (VisitPlanID = @ID);
	SELECT ID, VisitPlanId, BeginTime, EndTime FROM VisitMoves WHERE (ID = @ID)



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- ************************  Visits_Visitor_DeleteFromDate  ************************
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

DROP PROCEDURE dbo.Visits_Visitor_DeleteFromDate
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO



CREATE         PROCEDURE dbo.Visits_Visitor_DeleteFromDate
	@Duration as int
AS
-- Borramos los visitantes que lleven más de @Duration dias sin realizar una vista (LOPD) y actualizamos sus visitas (ALIAS)
UPDATE VisitPlan SET VisitorID = NULL, VisitorAlias = 'Eliminado LOPD ' + Convert(Varchar,Getdate(),103)
WHERE VisitorID IN (Select ID FROM Visitors WHERE LastVisitdate is not null and datediff(day,lastvisitdate,getdate()) >@Duration);
DELETE FROM Visitors WHERE (LastVisitdate is not null and datediff(day,lastvisitdate,getdate()) >@Duration) OR (LastVisitdate is null and datediff(day,Created,getdate()) >@Duration);


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- ************************  Visits_VisitMoves_Insert  ************************

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

DROP              PROCEDURE dbo.Visits_VisitMoves_Insert
GO

CREATE             PROCEDURE dbo.Visits_VisitMoves_Insert
(
	@ID int
)
AS
	SET NOCOUNT OFF;
INSERT INTO VisitMoves(VisitPlanID,BeginTime, EndTime) VALUES (@ID, getdate(), NULL);

--Insertar el número de TICKET en la planificación
UPDATE VisitPlan SET Ticket=(SELECT count(*) FROM visitmoves
WHERE year([BeginTime]) =  year(getdate()) 
      AND month([BeginTime]) =  month(getdate()) 
      AND day([BeginTime]) =  day(getdate())) 
WHERE ID=@ID;
--Insertar en el visitante la fecha de la visita
UPDATE Visitors SET LastVisitDate=getdate() WHERE ID = (SELECT VisitorID FROM VisitPlan WHERE ID = @ID);

Declare @Ticket int
SELECT @Ticket = Ticket FROM VisitPlan WHERE ID=@ID
RETURN @Ticket


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='194' WHERE ID='DBVersion'
GO

