-- Correciones permisos en nivel de autoridad

CREATE FUNCTION dbo.GetPassportLevelOfAuthority
(
	@idPassport int
)
RETURNS int
AS
BEGIN
	declare @levelOfAuthority int

	;with cte (id,idparentpassport,levelofauthority)
	as
	(
	select ID,IDParentPassport, LevelOfAuthority from sysroPassports where id = @idPassport 
	 union all 
	select t.id,t.IDParentPassport, t.LevelOfAuthority
	from sysroPassports t join cte c on t.id = c.idparentpassport
	)
	select @levelOfAuthority =  (select top 1 levelofauthority from cte where not levelofauthority is null)

	IF @levelOfAuthority is null set @levelOfAuthority = 1

	RETURN @levelOfAuthority
END
GO

ALTER FUNCTION [dbo].[GetRequestLevelsBelow]
   	(
   		@idPassport int,
  		@idRequest int
   	)
   RETURNS int
   AS
   BEGIN
   	DECLARE @LevelsBelow int,
   			@LevelOfAuthority int,
  			@featureAlias nvarchar(100),
  			@EmployeefeatureID int,
   			@idEmployee int,
  			@RequestLevel int
   	SELECT @LevelOfAuthority = dbo.GetPassportLevelOfAuthority(@idPassport)
  	SELECT @featureAlias = CASE Requests.RequestType 
  								WHEN 1 THEN 'Employees.UserFields.Requests' 
  								WHEN 2 THEN 'Calendar.Punches.Requests.Forgotten'
  								WHEN 3 THEN 'Calendar.Punches.Requests.Justify'
  								WHEN 4 THEN 'Calendar.Punches.Requests.ExternalParts'
  								WHEN 5 THEN 'Calendar.Requests.ShiftChange'
  								WHEN 6 THEN 'Calendar.Requests.Vacations'
  								WHEN 7 THEN 'Calendar.Requests.PlannedAbsence'
  								WHEN 9 THEN 'Calendar.Requests.PlannedCause'
  								ELSE 'Calendar.Requests.ShiftExchange' END,
  		   @EmployeefeatureID = CASE Requests.RequestType
  								WHEN 1 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Employees') 
  								WHEN 2 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
  								WHEN 3 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
  								WHEN 4 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
  								WHEN 5 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
  								WHEN 6 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
  								WHEN 7 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
  								WHEN 9 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
  								ELSE (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar') END,
  		   @idEmployee = Requests.IDEmployee,
  		   @RequestLevel = ISNULL(Requests.StatusLevel, 11)
  	FROM Requests
  	WHERE Requests.ID = @idRequest
  	/* Obtiene el número de niveles que hay entre el nivel del validador (@idPassport) y el nivel actual de la solicitud (@RequestLevel) */
  	SELECT @LevelsBelow = 
  	(SELECT COUNT( DISTINCT dbo.GetPassportLevelOfAuthority(Parents.ID))
  	 FROM sysroPassports INNER JOIN sysroPassports Parents
  			ON sysroPassports.IDParentPassport = Parents.ID
  	 WHERE sysroPassports.GroupType <> 'U' AND
  	 	  dbo.GetPassportLevelOfAuthority(Parents.ID) > @LevelOfAuthority AND
  		  dbo.GetPassportLevelOfAuthority(Parents.ID) <= @RequestLevel AND
  		  (select dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID,@FeatureAlias, 'U', 2)) > 0 AND
  		  (select dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@EmployeefeatureID,2,1,getdate())) > 0 
  	 )
  	IF @RequestLevel = 11  SET @LevelsBelow = @LevelsBelow + 1
   	
   RETURN @LevelsBelow
END
GO
  
ALTER FUNCTION [dbo].[GetRequestMinStatusLevel]
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
 	
 	SELECT @LevelOfAuthority = dbo.GetPassportLevelOfAuthority(@idPassport)
  	
 	SELECT @MinStatusLevel = 
 	(SELECT TOP 1 Parents.LevelOfAuthority
 	 FROM sysroPassports INNER JOIN sysroPassports Parents
 			ON sysroPassports.IDParentPassport = Parents.ID
 	 WHERE sysroPassports.GroupType <> 'U' AND
 	 	  dbo.GetPassportLevelOfAuthority(Parents.ID) > @LevelOfAuthority AND
 		  (select dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID,@FeatureAlias, 'U', 2)) > 0 AND
 		  (select dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@EmployeefeatureID,2,1,getdate())) > 0 
 	 ORDER BY Parents.LevelOfAuthority ASC)
  	
  RETURN @MinStatusLevel
  END
GO
  
ALTER FUNCTION [dbo].[GetRequestNextLevel]
  (	
  	@idRequest int	
  )
  RETURNS int
  AS
  BEGIN
  	DECLARE @NextLevel int
  	SELECT @NextLevel = 
  	(SELECT MAX(dbo.GetPassportLevelOfAuthority(Parents.ID))
  	FROM sysroPassports  INNER JOIN sysroPassports Parents
		ON sysroPassports.IDParentPassport = Parents.ID
  	WHERE sysroPassports.GroupType <> 'U' AND 
  	      dbo.GetPassportLevelOfAuthority(Parents.ID) < ISNULL(Requests.StatusLevel, 11) AND
  	      dbo.GetRequestPassportPermission(sysroPassports.ID, Requests.ID) > 0)
  	FROM Requests
  	WHERE Requests.ID = @idRequest
  		   
  	RETURN @NextLevel
END
GO

 
ALTER FUNCTION [dbo].[GetRequestNextLevelPassports]
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
		FOR  SELECT sysroPassports.Name 
  			 FROM sysroPassports 
  			 INNER JOIN (SELECT id, dbo.GetPassportLevelOfAuthority(ID) AS Level FROM sysroPassports) gpl
  			 on sysroPassports.ID=gpl.ID 
  			 WHERE sysroPassports.GroupType <> 'U' 
  			 AND gpl.level=dbo.GetRequestNextLevel(@idRequest)
			 AND dbo.GetRequestPassportPermission(sysroPassports.ID, @idRequest) > 0
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
 
ALTER PROCEDURE [dbo].[WebLogin_Passports_Select] 
   	(
   		@idPassport int
   	)
   AS
   	SELECT ID,
   		IDParentPassport,
   		GroupType,
   		Name,
   		Description,
   		Email,
   		IDUser,
   		IDEmployee,
   		IDLanguage,
   		dbo.GetPassportLevelOfAuthority(@idPassport) AS LevelOfAuthority,
   		ConfData,
  		AuthenticationMerge,
  		StartDate,
  		ExpirationDate,
  		[State]
   	FROM sysroPassports
   	WHERE ID = @idPassport
   	
   	RETURN
GO
   	

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='301' WHERE ID='DBVersion'
GO