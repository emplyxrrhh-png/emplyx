-- Visits_Visitor_Search
set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO

 ALTER  PROCEDURE  [dbo].[Visits_Visitor_Search]
 	@VisitorNIF NVarchar(255),
 	@VisitorName NVarchar(255),
 	@VisitorCompany NVarchar(255)
 AS
 DECLARE @sSQL nvarchar(2000)
 DECLARE @sWHERE nvarchar(2000)
 --SET @VisitorName = '%' + @VisitorName +'%'
 --set @VisitorCompany= '%' + @VisitorCompany + '%'
 SET @sSQL ='SELECT ID, Name, NIF, Company, Position, Comments,Type, Created, LastVisitDate FROM Visitors '
 SET @sSQL = @SSQL + ' WHERE Company like ''' + @VisitorCompany + ''' AND Name like ''' + @VisitorName +''''
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
 
-- Visits_VisitPlan_GetFiltered
set ANSI_NULLS OFF
set QUOTED_IDENTIFIER OFF
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
 			 @visitType as int = -1 -- Tipo de visita
 AS
 DECLARE @sSQL nvarchar(2000)
 DECLARE @sWHERE nvarchar(2000)
              SET @sWHERE = ''
              --IF @visitorName <> ''  SET @visitorName = '%' + @visitorName +'%'
              --IF @visitorCompany <> ''  SET @visitorCompany= '%' + @visitorCompany + '%'
              --IF @employeeVisitedName <> ''  SET @employeeVisitedName = '%' + @employeeVisitedName +'%'
                          -- Sentencia básica, filtrando por fecha
                          SET @sSQL ='SELECT * FROM sysroVisitPlan '
                          SET @sSQL = @SSQL + ' WHERE year([Date]) =  ' + str(year(@selectedDate)) + ' AND month([Date]) =  ' + str(month(@selectedDate)) + '  AND day([Date]) =  ' + str(day(@selectedDate))
                          --Restricciones
                          IF @visitStatus <> -1 SET @sWHERE = @sWHERE + ' AND Status = ' + str(@visitStatus)
                          IF @visitorName <>'' SET @sWHERE = @sWHERE + ' AND (VisitorName LIKE ''' + @visitorName + ''' OR VisitorAlias LIKE ''' + @visitorName + ''')'
                          IF @visitorNIF <> '' SET @sWHERE = @sWHERE + ' AND VisitorNIF = ''' + @visitorNIF +''''
                          IF @employeeVisitedName <> '' SET @sWHERE = @sWHERE + ' AND EmployeeVisitedName LIKE ''' + @employeeVisitedName + ''''
                          IF @visitPlanTicket <> -1 SET @sWHERE = @sWHERE + ' AND Ticket = ' + str(@visitPlanTicket)
 						 IF @visitType <> -1 SET @sWHERE = @sWHERE + ' AND Type = ' + str(@visitType)
                          IF @sWHERE<>'' SET @sSQL = @sSQL + @sWHERE 
                          -- Finalmente ordenamos por hora ascendente
                          SET @sSQL = @sSQL + ' ORDER BY Date ASC '
 EXEC sp_executesql @statement = @sSQL
 GO

 
 -- Visits_Employee_GetAllowVisits
set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO

 /****** Object:  Stored Procedure dbo.Visits_Employee_GetAllowVisits    Script Date: 05/02/2006 21:17:45 ******/
 ALTER   PROCEDURE  [dbo].[Visits_Employee_GetAllowVisits]
 	@Name nvarchar(255)
 AS
 IF @Name =''
 	SELECT *, 1 as IsIn FROM Employees, dbo.sysrosubvwCurrentEmployeePeriod  WHERE [AllowVisits] = 1  and IDEmployee = ID ORDER BY Name;
 ELSE
 BEGIN
 	--SET @Name = '%' + @Name +'%'
 	SELECT *, 1 as IsIn FROM Employees, dbo.sysrosubvwCurrentEmployeePeriod  WHERE [AllowVisits] = 1 AND [NAME] like @Name and IDEmployee = ID ORDER BY Name;
 END
GO
 
/* ***************************************************************************************************************************** */
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='228' WHERE ID='DBVersion'
GO
