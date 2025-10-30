--Añadimos las nuevas columnas a la tabla VisitPlan para controlar la perioricidad
ALTER TABLE VisitPlan ADD PeriodicID int
GO
ALTER TABLE VisitPlan ADD PeriodicBegin smalldatetime
GO
ALTER TABLE VisitPlan ADD PeriodicEnd smalldatetime
GO

-- VTVisitas: Tablas para campos de ficha de Visitantes y Visitas
CREATE TABLE [dbo].[sysroVisitorUserFields](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) COLLATE Modern_Spanish_CI_AS NOT NULL,
	[Required] [bit] NOT NULL,
	[DataType] [int] NOT NULL,
	[Position] [int] NOT NULL,
	[Used] [bit] NOT NULL,
 CONSTRAINT [PK_sysroVisitorUserFields] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[sysroVisitUserFields](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) COLLATE Modern_Spanish_CI_AS NOT NULL,
	[Required] [bit] NOT NULL,
	[DataType] [int] NOT NULL,
	[Position] [int] NOT NULL,
	[Used] [bit] NOT NULL,
 CONSTRAINT [PK_sysroVisitUserFields] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[VisitUserFieldsValues](
	[VisitplanId] [int] NOT NULL,
	[CustomFieldId] [int] NOT NULL,
	[Value] [nvarchar](255) COLLATE Modern_Spanish_CI_AS NOT NULL,
 CONSTRAINT [PK_VisitUserFieldsValues] PRIMARY KEY CLUSTERED 
(
	[VisitplanId] ASC,
	[CustomFieldId] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[VisitUserFieldsValues] WITH CHECK ADD CONSTRAINT [FK_VisitUserFieldsValues_sysroVisitUserFields] FOREIGN KEY([CustomFieldId]) REFERENCES [dbo].[sysroVisitUserFields] ([Id])
GO
ALTER TABLE [dbo].[VisitUserFieldsValues] CHECK CONSTRAINT [FK_VisitUserFieldsValues_sysroVisitUserFields]
GO
ALTER TABLE [dbo].[VisitUserFieldsValues] WITH CHECK ADD CONSTRAINT [FK_VisitUserFieldsValues_VisitPlan] FOREIGN KEY([VisitplanId]) REFERENCES [dbo].[VisitPlan] ([ID])
GO
ALTER TABLE [dbo].[VisitUserFieldsValues] CHECK CONSTRAINT [FK_VisitUserFieldsValues_VisitPlan]
GO

CREATE TABLE [dbo].[VisitorUserFieldsValues](
	[VisitorId] [int] NOT NULL,
	[CustomFieldId] [int] NOT NULL,
	[Value] [nvarchar](255) COLLATE Modern_Spanish_CI_AS NOT NULL,
 CONSTRAINT [PK_VisitorUserFieldsValues] PRIMARY KEY CLUSTERED 
(
	[VisitorId] ASC,
	[CustomFieldId] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[VisitorUserFieldsValues] WITH CHECK ADD CONSTRAINT [FK_VisitorUserFieldsValues_sysroVisitorUserFields1] FOREIGN KEY([CustomFieldId]) REFERENCES [dbo].[sysroVisitorUserFields] ([Id])
GO
ALTER TABLE [dbo].[VisitorUserFieldsValues] CHECK CONSTRAINT [FK_VisitorUserFieldsValues_sysroVisitorUserFields1]
GO
ALTER TABLE [dbo].[VisitorUserFieldsValues] WITH CHECK ADD CONSTRAINT [FK_VisitorUserFieldsValues_Visitors] FOREIGN KEY([VisitorId]) REFERENCES [dbo].[Visitors] ([ID])
GO
ALTER TABLE [dbo].[VisitorUserFieldsValues] CHECK CONSTRAINT [FK_VisitorUserFieldsValues_Visitors]
GO


-- Modificación Stored Visitas
/****** Objeto:  StoredProcedure [dbo].[Visits_VisitPlan_GetFiltered]    Fecha de la secuencia de comandos: 01/06/2009 21:36:37 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
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
						 IF @visitType <> -1 SET @sWHERE = @sWHERE + ' AND Type = ' + str(@visitType)
                         IF @sWHERE<>'' SET @sSQL = @sSQL + @sWHERE 
                         -- Finalmente ordenamos por hora ascendente
                         SET @sSQL = @sSQL + ' ORDER BY Date ASC '
EXEC sp_executesql @statement = @sSQL
GO

--Stored procedure para la inserción de Visitas
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
	@PeriodicEnd smalldatetime
)
AS
 	SET NOCOUNT OFF;

	INSERT INTO VisitPlan(VisitorAlias, VisitorId, EmpVisitedId, Date, Comments, Status, PlannedById, Type, PlannedDate,PeriodicID, PeriodicBegin, PeriodicEnd) VALUES (@VisitorAlias, @VisitorId, @EmpVisitedId, @Date, @Comments, @Status, @PlannedById, @Type, getdate(), @PeriodicID, @PeriodicBegin, @PeriodicEnd);

RETURN @@IDENTITY
GO

--stored procedure para obtener los datos de la visita a partir del ID
ALTER PROCEDURE [dbo].[Visits_VisitPlan_GetVisitPlanByID]
 	@ID integer
AS
SELECT *
FROM sysroVisitPlan
WHERE [ID] = @ID
GO

CREATE PROCEDURE [dbo].[Visits_UserFields_Delete]
 (
 	@Original_ID int,
	@userfieldtype Int
 )
 AS
 	SET NOCOUNT OFF;
IF @userfieldtype = 2
		-- VISTA
		BEGIN
			 DELETE FROM VisitUserFieldsValues WHERE (CustomFieldId = @Original_ID)
			 DELETE FROM sysroVisitUserFields WHERE (ID = @Original_ID)
		END
ELSE
		-- VISTANTE
		BEGIN
			 DELETE FROM VisitorUserFieldsValues WHERE (CustomFieldId = @Original_ID)
			 DELETE FROM sysroVisitorUserFields WHERE (ID = @Original_ID)
		END
GO

CREATE PROCEDURE [dbo].[Visits_UserFields_GetAll]
	@userfieldtype Int
 AS
 	SET NOCOUNT ON;
 IF @userfieldtype = 2
		-- VISTA
		BEGIN
			SELECT ID, Name, Required, DataType, Position, Used, '' as Value FROM sysroVisitUserFields
		END
 ELSE
		-- VISTANTE
		BEGIN
			SELECT ID, Name, Required, DataType, Position, Used, '' as Value FROM sysroVisitorUserFields
		END
GO

CREATE PROCEDURE [dbo].[Visits_UserFields_GetAllByForeignId]
	@foreignid Int,
	@userfieldtype Int
 AS

 IF @userfieldtype = 2
		-- VISTA
		BEGIN
			 SELECT ID, [Name], Required, DataType, Position, Used, ISNULL([Value],'') [Value]
				FROM sysroVisitUserFields 
					LEFT OUTER JOIN VisitUserFieldsValues ON (sysroVisitUserFields.Id = VisitUserFieldsValues.CustomFieldId AND VisitUserFieldsValues.VisitPlanId = @foreignid)
		END
  ELSE
		-- VISTOR
		BEGIN
			 SELECT ID, [Name], Required, DataType, Position, Used, ISNULL([Value],'') [Value]
				FROM sysroVisitorUserFields 
					LEFT OUTER JOIN VisitorUserFieldsValues ON (sysroVisitorUserFields.Id = VisitorUserFieldsValues.CustomFieldId AND VisitorUserFieldsValues.VisitorId = @foreignid)
		END
GO

CREATE PROCEDURE [dbo].[Visits_UserFields_Insert]
 (
 	@Name nvarchar(50),
 	@Required bit,
	@DataType int,
	@Position int,
	@Used bit,
	@userfieldtype Int
 )
 AS
 	SET NOCOUNT OFF;
 IF 	@userfieldtype = 2
	BEGIN
		 INSERT INTO sysroVisitUserFields(Name, Required, DataType, Position, Used) 
								  VALUES (@Name, @Required, @DataType, @Position, @Used);
 			SELECT ID, Name, Required, DataType, Position, Used FROM sysroVisitUserFields WHERE (ID = @@IDENTITY);
 			RETURN @@ROWCOUNT
	END
 ELSE
	BEGIN
		 INSERT INTO sysroVisitorUserFields(Name, Required, DataType, Position, Used) 
								  VALUES (@Name, @Required, @DataType, @Position, @Used);
 			SELECT ID, Name, Required, DataType, Position, Used FROM sysroVisitorUserFields WHERE (ID = @@IDENTITY);
 			RETURN @@ROWCOUNT
	END
GO

CREATE PROCEDURE [dbo].[Visits_UserFields_InsertValue]
 (
	@foreignId int,
	@CustomFieldId int,
	@userfieldtype Int,
 	@Value nvarchar(50)
 )
 AS
 	SET NOCOUNT OFF;
 IF 	@userfieldtype = 2
	BEGIN
	    INSERT INTO VisitUserFieldsValues (VisitPlanId,CustomFieldId,Value) VALUES (@foreignId,@CustomFieldId,@Value);
	END
 ELSE
	BEGIN
	    INSERT INTO VisitorUserFieldsValues (VisitorId,CustomFieldId,Value) VALUES (@foreignId,@CustomFieldId,@Value);
	END
GO

CREATE PROCEDURE [dbo].[Visits_UserFields_UpdateValue]
 (
	@foreignId int,
	@CustomFieldId int,
	@userfieldtype Int,
 	@Value nvarchar(50)
 )
 AS
 	SET NOCOUNT OFF;
 IF 	@userfieldtype = 2
	BEGIN
		 UPDATE VisitUserFieldsValues SET Value = @Value
				WHERE VisitplanId = @foreignId AND CustomFieldId = @CustomFieldid;
	END
 ELSE
	BEGIN
		 UPDATE VisitorUserFieldsValues SET Value = @Value
				WHERE VisitorId = @foreignId AND CustomFieldId = @CustomFieldid;
	END
GO

CREATE PROCEDURE [dbo].[Visits_UserFields_ValueExists]
 (
	@foreignId int,
	@userfieldtype Int,
	@CustomFieldId int
 )
 AS
 	SET NOCOUNT OFF;
 DECLARE @Exist int
 IF 	@userfieldtype = 2
	BEGIN
		 SELECT @Exist = Count(*) FROM VisitUserFieldsValues WHERE VisitPlanId = @foreignId AND CustomFieldId = @CustomFieldId;
		 RETURN @Exist
	END 
 ELSE
	BEGIN
		 SELECT @Exist = Count(*) FROM VisitorUserFieldsValues WHERE VisitorId = @foreignId AND CustomFieldId = @CustomFieldId;
		 RETURN @Exist
	END 
GO

CREATE PROCEDURE [dbo].[Visits_GetFieldValueByColumnName]
 	@ColumnName NVarchar(50),
	@foreignID Int,
	@userfieldtype Int
AS
	declare @ExistColumn int

IF @userfieldtype = 2
	BEGIN
		SELECT @ExistColumn = (SELECT COUNT(Name) Exist FROM sysroVisitUserFields WHERE Name LIKE @ColumnName)
		IF @ExistColumn <> 0 
			SELECT Value FROM VisitUserFieldsValues INNER JOIN sysroVisitUserFields ON VisitUserFieldsValues.CustomFieldID = sysroVisitUserFields.ID WHERE Name LIKE @ColumnName AND VisitPlanID = @foreignID		
		ELSE
			RETURN '-99999'
	END
ELSE
	BEGIN
		SELECT @ExistColumn = (SELECT COUNT(Name) Exist FROM sysroVisitorUserFields WHERE Name LIKE @ColumnName)
		IF @ExistColumn <> 0 
			SELECT Value FROM VisitorUserFieldsValues INNER JOIN sysroVisitorUserFields ON VisitorUserFieldsValues.CustomFieldID = sysroVisitorUserFields.ID WHERE Name LIKE @ColumnName AND VisitorID = @foreignID
		ELSE
			RETURN '-99999'
	END
GO

CREATE PROCEDURE [dbo].[Visits_UserFields_NameExists]
 (
      @Name nvarchar(50),
      @userfieldtype Int
 )
 AS
      SET NOCOUNT OFF;
 DECLARE @Exist int
 IF   @userfieldtype = 2
      BEGIN
             SELECT @Exist = Count(*) FROM sysroVisitUserFields WHERE Name = @Name;
             RETURN @Exist
      END 
 ELSE
      BEGIN
             SELECT @Exist = Count(*) FROM sysroVisitorUserFields WHERE Name = @Name;
             RETURN @Exist
      END 
GO

ALTER PROCEDURE [dbo].[Visits_Visitor_DeleteFromDate]
 	@Duration as int
 AS
 UPDATE VisitPlan SET VisitorID = NULL, VisitorAlias = 'Eliminado LOPD ' + Convert(Varchar,Getdate(),103)
 WHERE VisitorID IN (Select ID FROM Visitors WHERE LastVisitdate is not null and datediff(day,lastvisitdate,getdate()) >@Duration);
 DELETE FROM Visitors WHERE (LastVisitdate is not null and datediff(day,lastvisitdate,getdate()) >@Duration) OR (LastVisitdate is null and datediff(day,Created,getdate()) >@Duration);
 
 --Borramos los campos de la ficha de los visitantes eliminados
 DELETE VisitorUserFieldsValues FROM Visitors
	INNER JOIN VisitorUserFieldsValues ON Visitors.ID = VisitorUserFieldsValues.VisitorID
 WHERE (LastVisitdate is not null and datediff(day,lastvisitdate,getdate()) >@Duration) OR (LastVisitdate is null and datediff(day,Created,getdate()) >@Duration);
GO 

DROP View sysroVisitPlan
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object:  View dbo.sysroVisitPlan    Script Date: 05/02/2006 21:17:44 ******/
CREATE VIEW [dbo].[sysroVisitPlan]
AS
SELECT     dbo.Visitors.Name AS VisitorName, dbo.Visitors.NIF AS VisitorNIF, dbo.Employees.Name AS EmployeeVisitedName, dbo.VisitPlan.ID, 
                      dbo.VisitPlan.VisitorAlias, dbo.VisitPlan.VisitorId, dbo.VisitPlan.EmpVisitedId, dbo.VisitPlan.Date, dbo.VisitPlan.Comments, dbo.VisitPlan.Status, 
                      dbo.VisitPlan.PlannedById, dbo.VisitPlan.PlannedDate, dbo.VisitPlan.Type, dbo.VisitPlan.Ticket, dbo.VisitPlan.PeriodicID, dbo.VisitPlan.PeriodicBegin, 
                      dbo.VisitPlan.PeriodicEnd, dbo.VisitMoves.BeginTime, dbo.VisitMoves.EndTime
FROM         dbo.Employees INNER JOIN
                      dbo.VisitPlan ON dbo.Employees.ID = dbo.VisitPlan.EmpVisitedId LEFT OUTER JOIN
                      dbo.VisitMoves ON dbo.VisitPlan.ID = dbo.VisitMoves.VisitPlanId LEFT OUTER JOIN
                      dbo.Visitors ON dbo.VisitPlan.VisitorId = dbo.Visitors.ID
GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[Visits_VisitPlan_Delete]
(
 	@Original_ID int
)
 AS
 	SET NOCOUNT OFF;
DELETE VisitUserFieldsValues WHERE VisitPlanID = @Original_ID
DELETE FROM VisitPlan WHERE (ID = @Original_ID)
GO
 
CREATE PROCEDURE [dbo].[Visits_UserFields_Update]
(
  	@UserFieldName nvarchar(50),
  	@UserFieldId int,
	@userfieldtype Int
)
AS
  	SET NOCOUNT OFF;
IF @userfieldtype = 2
		-- VISITA
		BEGIN
			 UPDATE sysroVisitUserFields SET Name = @UserFieldName WHERE Id = @UserFieldId
		END
ELSE
		-- VISTANTE
		BEGIN
			 UPDATE sysroVisitorUserFields SET Name = @UserFieldName WHERE Id = @UserFieldId
		END
GO



-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='217' WHERE ID='DBVersion'
GO
