UPDATE [dbo].[sysroFeatures]
SET [Alias] = 'Calendar.Punches.Requests.Justify'
WHERE [Alias] = 'Calendar.Punches.Punches.Requests.Justify'
GO

DELETE FROM sysroFeatures
WHERE Alias = 'Causes.EmployeesAndRulesAssign'
GO

DELETE FROM sysroFeatures
WHERE Alias = 'Concepts.EmployeeAssign'
GO

DELETE FROM sysroFeatures
WHERE Alias = 'Shifts.EmployeesAssign'
GO

UPDATE sysroFeatures
SET PermissionTypes = 'W'
WHERE Alias = 'LabAgree.EmployeeAssign'
GO

/* Eliminamos temporalmente esta opción  para que no se pueda configurar ya que todabía no está implementada */
DELETE FROM sysroPassports_PermissionsOverFeatures
WHERE IDFeature = (SELECT ID FROM sysroFeatures WHERE Alias = 'Planification.Requests.ShiftExchange')
GO
DELETE FROM sysroFeatures
WHERE Alias = 'Planification.Requests.ShiftExchange'
GO

DELETE FROM sysroPassports_PermissionsOverFeatures
WHERE IDFeature = (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar.Requests.ShiftExchange')
GO
DELETE FROM sysroFeatures
WHERE Alias = 'Calendar.Requests.ShiftExchange'
GO

UPDATE Shifts
SET ShiftType = 2
WHERE ShiftType = 1
GO

/* Función [GetRequestLevelsBelow] **********************************************************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEmployeeGroupParent]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetRequestLevelsBelow]
GO

 CREATE FUNCTION [dbo].[GetRequestLevelsBelow]
 	(
 		@idPassport int,
		@idRequest int
 	)
 RETURNS int
 AS
 BEGIN

 	DECLARE @LevelsBelow int
 	DECLARE @LevelOfAuthority int

	SELECT @LevelOfAuthority = CASE GroupType WHEN 'U' THEN sysroPassports.LevelOfAuthority ELSE (SELECT Parent.LevelOfAuthority FROM sysroPassports Parent WHERE Parent.ID = sysroPassports.IDParentPassport) END
	FROM sysroPassports 
	WHERE sysroPassports.ID = @idPassport

	DECLARE @featureAlias nvarchar(100),
			@EmployeefeatureID int,
 			@idEmployee int,
			@RequestLevel int
 	
	SELECT @featureAlias = CASE Requests.RequestType 
								WHEN 1 THEN 'Employees.UserFields.Requests' 
								WHEN 2 THEN 'Calendar.Punches.Requests.Forgotten'
								WHEN 3 THEN 'Calendar.Punches.Requests.Justify'
								WHEN 4 THEN 'Calendar.Punches.Requests.ExternalParts'
								WHEN 5 THEN 'Calendar.Requests.ShiftChange'
								WHEN 6 THEN 'Calendar.Requests.Vacations'
								WHEN 7 THEN 'Calendar.Requests.PlannedAbsence'
								ELSE 'Calendar.Requests.ShiftExchange' END,
		   @EmployeefeatureID = CASE Requests.RequestType
								WHEN 1 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Employees') 
								WHEN 2 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 3 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 4 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 5 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 6 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 7 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								ELSE (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar') END,
		   @idEmployee = Requests.IDEmployee,
		   @RequestLevel = ISNULL(Requests.StatusLevel, 11)
	FROM Requests
	WHERE Requests.ID = @idRequest

	/* Obtiene el número de niveles que hay entre el nivel del validador (@idPassport) y el nivel actual de la solicitud (@RequestLevel) */
	SELECT @LevelsBelow = 
	(SELECT COUNT( DISTINCT Parents.LevelOfAuthority)
	 FROM sysroPassports INNER JOIN sysroPassports Parents
			ON sysroPassports.IDParentPassport = Parents.ID
	 WHERE sysroPassports.GroupType = '' AND
	 	  Parents.LevelOfAuthority IS NOT NULL AND
	 	  Parents.LevelOfAuthority > @LevelOfAuthority AND
		  Parents.LevelOfAuthority <= @RequestLevel AND
		  (select dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID,@FeatureAlias, 'U', 2)) > 0 AND
		  (select dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@EmployeefeatureID,2,1,getdate())) > 0 
	 )

	IF @RequestLevel = 11  SET @LevelsBelow = @LevelsBelow + 1
 	
 RETURN @LevelsBelow
 END
GO

/* Función [GetRequestMinStatusLevel] **********************************************************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEmployeeGroupParent]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetRequestMinStatusLevel]
GO

 CREATE FUNCTION [dbo].[GetRequestMinStatusLevel]
 	(
 		@idPassport int,
		@featureAlias nvarchar(100),
		@EmployeefeatureID int,
 		@idEmployee int
 	)
 RETURNS int
 AS
 BEGIN
 	/* While looking only at permissions defined directly on passport,
 	returns the first permission found in the employees groups hierarchy */
 	DECLARE @MinStatusLevel int
 	DECLARE @LevelOfAuthority int

	SELECT @LevelOfAuthority = CASE GroupType WHEN 'U' THEN sysroPassports.LevelOfAuthority ELSE (SELECT Parent.LevelOfAuthority FROM sysroPassports Parent WHERE Parent.ID = sysroPassports.IDParentPassport) END
	FROM sysroPassports 
	WHERE sysroPassports.ID = @idPassport
 	

	SELECT @MinStatusLevel = 
	(SELECT TOP 1 Parents.LevelOfAuthority
	 FROM sysroPassports INNER JOIN sysroPassports Parents
			ON sysroPassports.IDParentPassport = Parents.ID
	 WHERE sysroPassports.GroupType = '' AND
	 	  Parents.LevelOfAuthority IS NOT NULL AND
	 	  Parents.LevelOfAuthority > @LevelOfAuthority AND
		  (select dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID,@FeatureAlias, 'U', 2)) > 0 AND
		  (select dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@EmployeefeatureID,2,1,getdate())) > 0 
	 ORDER BY Parents.LevelOfAuthority ASC)


 	
 RETURN @MinStatusLevel
 END
GO
/* Función [GetRequestNextLevel] **********************************************************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEmployeeGroupParent]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetRequestNextLevel]
GO

CREATE FUNCTION [dbo].[GetRequestNextLevel]
(	
	@idRequest int	
)
RETURNS int
AS
BEGIN

	DECLARE @NextLevel int

	SELECT @NextLevel = 
	(SELECT MAX(Parents.LevelOfAuthority)
	FROM sysroPassports INNER JOIN sysroPassports Parents ON sysroPassports.IDParentPassport = Parents.ID 
	WHERE sysroPassports.GroupType = '' AND 
	      Parents.LevelOfAuthority IS NOT NULL AND 
	      Parents.LevelOfAuthority < ISNULL(Requests.StatusLevel, 11) AND 
	      dbo.GetRequestPassportPermission(sysroPassports.ID, Requests.ID) > 0)
	FROM Requests
	WHERE Requests.ID = @idRequest
		   
	RETURN @NextLevel

END
GO
/* Función [GetRequestNextLevelPassports] **********************************************************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEmployeeGroupParent]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetRequestNextLevelPassports]
GO

CREATE FUNCTION [dbo].[GetRequestNextLevelPassports]
(	
	@idRequest int	
)
RETURNS nvarchar(1000)
AS
BEGIN

	DECLARE @RetNames nvarchar(1000), 
			@PassportName nvarchar(50)

	SET @RetNames = ''

	DECLARE PassportsCursor CURSOR
		FOR SELECT sysroPassports.Name 
			FROM sysroPassports INNER JOIN sysroPassports Parents ON sysroPassports.IDParentPassport = Parents.ID 
			WHERE sysroPassports.GroupType = '' AND 
				  Parents.LevelOfAuthority IS NOT NULL AND 
				  Parents.LevelOfAuthority = dbo.GetRequestNextLevel(@idRequest) AND 
				  dbo.GetRequestPassportPermission(sysroPassports.ID, @idRequest) > 0
			ORDER BY sysroPassports.Name
	OPEN PassportsCursor
	FETCH NEXT FROM PassportsCursor
	INTO @PassportName
	WHILE @@FETCH_STATUS = 0
	BEGIN	
	
		IF @RetNames = ''
			BEGIN
				SET @RetNames = @PassportName
			END
		ELSE
			BEGIN
				SET @RetNames = @RetNames + ', '+ @PassportName
			END

		FETCH NEXT FROM PassportsCursor 
		INTO @PassportName
	END 
	CLOSE PassportsCursor
	DEALLOCATE PassportsCursor

	RETURN @RetNames

END
GO
/* Función [GetRequestPassportPermission] **********************************************************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEmployeeGroupParent]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetRequestPassportPermission]
GO

CREATE FUNCTION [dbo].[GetRequestPassportPermission]
(
	@idPassport int,
	@idRequest int	
)
RETURNS int
AS
BEGIN

	DECLARE @FeatureAlias nvarchar(100),
			@EmployeefeatureID int,
			@idEmployee int

	SELECT @FeatureAlias = CASE Requests.RequestType 
								WHEN 1 THEN 'Employees.UserFields.Requests' 
								WHEN 2 THEN 'Calendar.Punches.Requests.Forgotten'
								WHEN 3 THEN 'Calendar.Punches.Requests.Justify'
								WHEN 4 THEN 'Calendar.Punches.Requests.ExternalParts'
								WHEN 5 THEN 'Calendar.Requests.ShiftChange'
								WHEN 6 THEN 'Calendar.Requests.Vacations'
								WHEN 7 THEN 'Calendar.Requests.PlannedAbsence'
								ELSE 'Calendar.Requests.ShiftExchange' END,
		   @EmployeefeatureID = CASE Requests.RequestType 
								WHEN 1 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Employees') 
								WHEN 2 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 3 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 4 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 5 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 6 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 7 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								ELSE (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar') END,
		   @idEmployee = Requests.IDEmployee
	FROM Requests
	WHERE Requests.ID = @idRequest


	DECLARE @Permission int

	SET @Permission = (select dbo.WebLogin_GetPermissionOverFeature(@idPassport,@FeatureAlias, 'U', 2)) 
	
	IF @Permission > 0 
		BEGIN

			SET @Permission = (select dbo.WebLogin_GetPermissionOverEmployee(@idPassport,@idEmployee,@EmployeefeatureID,2,1,getdate()))

		END
		   
	RETURN @Permission

END
GO

/* Índices para optimizar las consultas de solicitudes ****************************************************************/
CREATE NONCLUSTERED INDEX [_dta_index_Employees_38_708197573__K1_2_6960] ON [dbo].[Employees] 
(
	[ID] ASC
)
INCLUDE ( [Name]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [_dta_index_Requests_38_410484541__K6_K2_K1_K4_3_7_8_9_10_12_13_14_15_16] ON [dbo].[Requests] 
(
	[Status] ASC,
	[IDEmployee] ASC,
	[ID] ASC,
	[RequestDate] ASC
)
INCLUDE ( [RequestType],
[StatusLevel],
[Date1],
[Date2],
[FieldName],
[IDCause],
[Hours],
[IDShift],
[IDEmployeeExchange],
[NotReaded]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [_dta_index_RequestsApprovals_38_506484883__K1_K3] ON [dbo].[RequestsApprovals] 
(
	[IDRequest] ASC,
	[DateTime] ASC
)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

CREATE STATISTICS [_dta_stat_1483868353_10_1_3_2] ON [dbo].[sysroPassports]([LevelOfAuthority], [ID], [GroupType], [IDParentPassport])
GO

CREATE NONCLUSTERED INDEX [_dta_index_sysroPassports_38_1483868353__K2_K3_K1_K10] ON [dbo].[sysroPassports] 
(
	[IDParentPassport] ASC,
	[GroupType] ASC,
	[ID] ASC,
	[LevelOfAuthority] ASC
)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [_dta_index_sysroPassports_38_1483868353__K10_K3_1_2] ON [dbo].[sysroPassports] 
(
	[LevelOfAuthority] ASC,
	[GroupType] ASC
)
INCLUDE ( [ID],
[IDParentPassport]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO




/* ***************************************************************************************************************************** */
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='248' WHERE ID='DBVersion'
GO
