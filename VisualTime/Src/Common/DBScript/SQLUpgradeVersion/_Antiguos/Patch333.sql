ALTER FUNCTION [dbo].[WebLogin_GetPermissionOverFeature] 
  	(
  		@idPassport int,
  		@featureAlias nvarchar(100),
  		@featureType varchar(1),
  		@mode int
  	)
  RETURNS int
  AS
  BEGIN
  	DECLARE @Result int
  	DECLARE @IDFeature int
 	DECLARE @parentPassport int
   	DECLARE @GroupType nvarchar(50)
  	
 	SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @idPassport
   		
   	
 	IF @featureType = 'U'
 	BEGIN
 		IF @GroupType = 'U'
   		BEGIN
   			SET @parentPassport = @idPassport
   		END
   		ELSE
   		BEGIN
   			SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @idPassport
   		END
  		SELECT @IDFeature = ID FROM sysroFeatures WHERE Alias = @featureAlias AND Type = @featureType
  		SELECT @Result = Permission FROM sysroPermissionsOverFeatures WHERE PassportID = @parentPassport AND FeatureID = @IDFeature
  	END
 	ELSE
 	BEGIN
 		SELECT @Result = dbo.sysro_GetPermissionOverFeature(@idPassport,@featureAlias,@featureType,@mode)
 	END
  	RETURN @Result
  END
GO
 
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='333' WHERE ID='DBVersion'
GO


