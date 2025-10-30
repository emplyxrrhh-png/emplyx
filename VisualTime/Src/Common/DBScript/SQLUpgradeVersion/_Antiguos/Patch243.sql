/* Tabla para guardar los campos de la ficha para luego poder realizar agrupaciones en el informe Lista de Empleados agrupados por ficha personal*/
CREATE TABLE [dbo].[TMPUSERFIELDS]
(
	[IDEmployee] [int] NOT NULL,
	[UserField] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_TMPUSERFIELDS] PRIMARY KEY CLUSTERED 
(
	[IDEmployee] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

/* ACTUALIZACIÓN PARA VISUALTIME VISITAS */
--TABLA DE UBICACIONES
CREATE TABLE [dbo].[VisitLocations](
	[ID] [int] IDENTITY(0,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_VisitLocations_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

--AÑADIMOS LA "SIN UBICACIÓN"
INSERT INTO VisitLocations VALUES ('')
GO

--AÑADIMOS LA UBICACIÓN A LAS VISITAS Y LAS RETRICCIONES
ALTER TABLE VisitPlan ADD IDLocation int NULL
GO
ALTER TABLE [dbo].[VisitPlan] WITH CHECK ADD CONSTRAINT [FK_VisitPlan_VisitLocations] FOREIGN KEY([IDLocation]) REFERENCES [dbo].[VisitLocations] ([ID])
GO
ALTER TABLE [dbo].[VisitPlan] CHECK CONSTRAINT [FK_VisitPlan_VisitLocations]
GO

--OBTENER UBICACIONES
CREATE PROCEDURE [dbo].[Visits_VisitLocations_GetAll]
AS
	SELECT ID, Name FROM VisitLocations WHERE ID>0
GO

--OBTENER UNA UBICACIÓN
CREATE PROCEDURE [dbo].[Visits_VisitLocations_GetById]
(
 	@visitLocationId as int
)
AS
 	SELECT * FROM VisitLocations WHERE [ID] = @visitLocationId;
GO

--INSERT LOCATIONS
CREATE PROCEDURE [dbo].[Visits_VisitLocations_Insert]
(
 	@Name nvarchar(50)
)
AS
	INSERT INTO VisitLocations(Name) VALUES (@Name);
 	SELECT ID, Name FROM VisitLocations WHERE (ID = @@IDENTITY);
 	RETURN @@ROWCOUNT
GO

--UPDATE LOCATIONS
CREATE PROCEDURE [dbo].[Visits_VisitLocations_Update]
(
 	@Name nvarchar(50),
 	@ID int
)
AS
	UPDATE VisitLocations SET Name = @Name WHERE (ID = @ID);
 	SELECT ID, Name FROM VisitLocations WHERE (ID = @ID)
GO

--DELETE LOCATIONS
CREATE PROCEDURE [dbo].[Visits_VisitLocations_Delete]
(
 	@ID int
)
AS
	SET NOCOUNT OFF;
	DELETE FROM VisitLocations WHERE (ID = @ID)
	RETURN @@ROWCOUNT
GO

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
  
ALTER PROCEDURE [dbo].[Visits_VisitPlan_Update]
(
  	@VisitorAlias nvarchar(50),
  	@VisitorId int,
  	@EmpVisitedId int,
  	@Date smalldatetime,
  	@Comments text,
  	@Status int,
  	@Type int,
  	@ID int,
 	@Ticket int,
	@IDLocation int
)
AS
  	SET NOCOUNT OFF;
	UPDATE VisitPlan SET VisitorAlias = @VisitorAlias, VisitorId = @VisitorId, EmpVisitedId = @EmpVisitedId, Date = @Date, Comments = @Comments, Status = @Status, Type = @Type, Ticket = @Ticket, IDLocation = @IDLocation WHERE (ID = @ID) ;
  	SELECT ID, VisitorAlias, VisitorId, EmpVisitedId, Date, Comments, Status, PlannedById, Type, IDLocation FROM VisitPlan WHERE (ID = @ID)
GO

ALTER PROCEDURE [dbo].[Visits_VisitPlan_Insert]
(
  	@VisitorAlias nvarchar(50),
  	@VisitorId int,
  	@EmpVisitedId int,
  	@Date smalldatetime,
  	@Comments text,
  	@Status int,
  	@PlannedById int,
  	@Type int,
 	@PeriodicID int,
 	@PeriodicBegin smalldatetime,
 	@PeriodicEnd smalldatetime,
	@Location int
)
AS
  	SET NOCOUNT OFF;
 	INSERT INTO VisitPlan(VisitorAlias, VisitorId, EmpVisitedId, Date, Comments, Status, PlannedById, Type, PlannedDate,PeriodicID, PeriodicBegin, PeriodicEnd, IDLocation) VALUES (@VisitorAlias, @VisitorId, @EmpVisitedId, @Date, @Comments, @Status, @PlannedById, @Type, getdate(), @PeriodicID, @PeriodicBegin, @PeriodicEnd, @Location);
	RETURN @@IDENTITY
GO

ALTER VIEW [dbo].[sysroVisitPlan]
AS
	SELECT dbo.Visitors.Name AS VisitorName, dbo.Visitors.NIF AS VisitorNIF, dbo.Employees.Name AS EmployeeVisitedName, dbo.VisitPlan.ID, 
		dbo.VisitPlan.VisitorAlias, dbo.VisitPlan.VisitorId, dbo.VisitPlan.EmpVisitedId, dbo.VisitPlan.Date, dbo.VisitPlan.Comments, dbo.VisitPlan.Status, 
		dbo.VisitPlan.PlannedById, dbo.VisitPlan.PlannedDate, dbo.VisitPlan.Type, dbo.VisitPlan.Ticket, dbo.VisitPlan.PeriodicID, dbo.VisitPlan.PeriodicBegin, 
		dbo.VisitPlan.PeriodicEnd, dbo.VisitPlan.IDLocation, dbo.VisitMoves.BeginTime, dbo.VisitMoves.EndTime
	FROM dbo.Employees 
		INNER JOIN dbo.VisitPlan ON dbo.Employees.ID = dbo.VisitPlan.EmpVisitedId 
		LEFT OUTER JOIN dbo.VisitMoves ON dbo.VisitPlan.ID = dbo.VisitMoves.VisitPlanId 
		LEFT OUTER JOIN dbo.Visitors ON dbo.VisitPlan.VisitorId = dbo.Visitors.ID
GO

/* Auditoria: Se añade la auditoria de introducción masiva de fichajes y justificaciones */
INSERT INTO sysroauditelements values (49,'MassiveMoves','Introducción masiva de fichajes')
GO
INSERT INTO sysroauditelements values (50,'MassiveCauses','Introducción masiva de justificaciones')
GO

/* Actualizamos la tabla de órdenes (campo que controla si se cierra o se abre manualmente la orden) */
ALTER TABLE Orders ADD ManualEndDate BIT NULL DEFAULT 0
GO
UPDATE Orders SET ManualEndDate=0
GO

/* ***************************************************************************************************************************** */
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='243' WHERE ID='DBVersion'
GO
