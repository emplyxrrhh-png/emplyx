ALTER FUNCTION [dbo].[GetRequestPassportPermission]
   (
   	@idPassport int,
    	@idRequest int	
   )
   RETURNS int
   AS
   BEGIN
    	DECLARE @FeatureAlias nvarchar(100),
    			@EmployeefeatureID int,
    			@idEmployee int,
   				@RequestType int,
				@BusinessCenter int
   			
    	SELECT @RequestType = Requests.RequestType,
   				@idEmployee = Requests.IDEmployee,
				@BusinessCenter = Requests.IDCenter
		FROM Requests
   	WHERE Requests.ID = @idRequest
   	
   	SELECT @featureAlias = Alias, 
   		   @EmployeefeatureID = EmployeeFeatureId 
   	FROM dbo.sysroFeatures 
   	WHERE sysroFeatures.AliasId = @RequestType
    	
    	
    	DECLARE @Permission int
    	SET @Permission = (select dbo.WebLogin_GetPermissionOverFeature(@idPassport,@FeatureAlias, 'U', 2)) 
    	
    	IF @Permission > 0 
    	BEGIN
    		SET @Permission = (select dbo.WebLogin_GetPermissionOverEmployee(@idPassport,@idEmployee,@EmployeefeatureID,2,1,getdate()))
    	END

		IF @Permission > 0 AND @RequestType = 12
		BEGIN
			DECLARE @Centers int
			set @Centers = (select isnull(count(*),0) as total from sysroPassports_Centers where idcenter = @BusinessCenter AND IDPassport IN(select IDParentPassport  from sysroPassports where id = @idPassport ))
			IF @Centers = 0
			BEGIN
				SET @Permission = 0 
			END
			
		END
    		    
    	RETURN @Permission
 END
GO

UPDATE dbo.sysroParameters SET Data='373' WHERE ID='DBVersion'
GO