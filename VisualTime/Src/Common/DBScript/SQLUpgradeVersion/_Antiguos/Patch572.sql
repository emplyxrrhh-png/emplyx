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
    				@BusinessCenter int,
  				@IDCause int,
  				@IDShift int
       			
        	SELECT @RequestType = Requests.RequestType,
       				@idEmployee = Requests.IDEmployee,
    				@BusinessCenter = Requests.IDCenter,
  				@IDCause = isnull(Requests.IDCause,0),
  				@IDShift = isnull(Requests.IDShift,0)
    		FROM Requests
       	WHERE Requests.ID = @idRequest
       	
       	SELECT @featureAlias = Alias, 
       		   @EmployeefeatureID = EmployeeFeatureId 
       	FROM dbo.sysroFeatures 
       	WHERE sysroFeatures.AliasId = @RequestType
        

        	
        	DECLARE @Permission int
   		DECLARE @EmployeePermission int,@GroupType nvarchar(50),@IDParentPassport int, @EmployeeGroup int
		 		-- NUEVO
 		SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @idPassport   
    	 
 		IF @GroupType = 'U'    
 		BEGIN    
 			SET @IDParentPassport =@idPassport  
 		END    
 		ELSE    
 		BEGIN    
 			SELECT @IDParentPassport = IDParentPassport FROM sysroPassports WHERE ID = @idPassport
 		END    

			SET @Permission = (select Permission from sysroPermissionsOverFeatures where PassportID = @IDParentPassport  and featureID =@EmployeefeatureID)
        	
        	IF @Permission > 0 
        	BEGIN
				SET @EmployeePermission = (select Permission from sysroPermissionsOverGroups where EmployeeFeatureID=@EmployeefeatureID and PassportID=@IDParentPassport  and EmployeeGroupID in(select idgroup from employeegroups where idemployee=@idEmployee and convert(date,getdate()) between BeginDate and EndDate) 
				and (select count(PassportId) from sysroPermissionsOverEmployeesExceptions poe where poe.PassportID = @IDParentPassport and EmployeeFeatureID = @EmployeefeatureID and poe.EmployeeID=@idEmployee ) = 0)
				
        	END
    		IF @Permission > 0 AND @EmployeePermission > 0 AND @RequestType = 12
    		BEGIN
    			DECLARE @Centers int
 			-- MODIFICADO
    			set @Centers = (select isnull(count(*),0) as total from sysroPassports_Centers where idcenter = @BusinessCenter AND IDPassport = @IDParentPassport)
    			IF @Centers = 0
    			BEGIN
    				SET @Permission = 0 
    			END
    			
    		END
  		-- Permisos sobre Grupos de negocio
  	    IF @Permission > 0 AND @EmployeePermission > 0 
    		BEGIN
  			declare @BusinessGroupList nvarchar(max)
 			 
  		  	--SELECT @IDParentPassport = isnull(IDParentPassport,0)
  	     	--FROM dbo.sysroPassports 
  		 	--WHERE sysroPassports.ID = @idPassport
  			-- MODIFICADO
 			SELECT @BusinessGroupList = isnull(BusinessGroupList,'')
  	     	FROM dbo.sysroPassports 
  		 	WHERE sysroPassports.ID = @IDParentPassport
  			
  			if len(@BusinessGroupList) > 0 
  			begin
  				if @IDCause > 0
  				BEGIN
  					declare @BusinessGroupListCause nvarchar(max)
  					set @BusinessGroupListCause = (SELECT ISNULL(BusinessGroup, '') AS BusinessGroup FROM Causes WHERE (Causes.ID = @IDCause) )
  					if (len(@BusinessGroupListCause) > 0 )
  					begin
  						if charindex(@BusinessGroupListCause, @BusinessGroupList) = 0 
  						begin
  							SET @Permission = 0 
  						end
  					end
  				END
  				if @IDShift > 0 and @Permission > 0
  				BEGIN
  					declare @BusinessGroupListShift nvarchar(max)
  					set @BusinessGroupListShift = (SELECT ISNULL(ShiftGroups.BusinessGroup, '') AS BusinessGroup FROM Shifts LEFT OUTER JOIN ShiftGroups ON Shifts.IDGroup = ShiftGroups.ID WHERE (Shifts.ID = @IDShift) )
  					if (len(@BusinessGroupListShift) > 0 )
  					begin
  						if charindex(@BusinessGroupListShift, @BusinessGroupList) = 0 
  						begin
  							SET @Permission = 0 
  						end
  					end
  				END
  			end
    		END
        		
   		IF @EmployeePermission > @Permission
   			RETURN @Permission
   		RETURN @EmployeePermission
        	
     END
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='572' WHERE ID='DBVersion'
GO
