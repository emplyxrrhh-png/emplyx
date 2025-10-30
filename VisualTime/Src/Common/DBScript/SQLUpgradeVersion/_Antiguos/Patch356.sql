ALTER FUNCTION [dbo].[GetFeatureNextLevelPassportsIDsByEmployee]
  (	
    	@idEmployee int,
    	@featureAlias nvarchar(100),
    	@employeeFeatureId int
  )
  RETURNS nvarchar(1000)
  AS
  BEGIN
	DECLARE @RetNames nvarchar(1000)
	SET @RetNames = ''

	DECLARE @tmpTable Table(PassportID int,LevelOfAuthority int ,Name nvarchar(50))

	insert into @tmpTable 
	SELECT sysroPassports.id AS PassportID, dbo.GetPassportLevelOfAuthority(sysroPassports.ID) AS LevelOfAuthority, sysroPassports.Name
    		FROM sysroPassports  
    		WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null AND 
    			  dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@employeeFeatureID,2,1,getdate()) > 3 AND 
 			  dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID, @featureAlias,'U',2) > 3 AND Description not like '@@ROBOTICS@@%'

	SET @RetNames = (SELECT  CONVERT(NVARCHAR(4000),PassportID ) + ','  AS [text()] from @tmpTable tt where  tt.LevelOfAuthority = (SELECT max(LevelOfAuthority) from @tmpTable) For XML PATH (''))

	IF @RetNames <> ''
    	BEGIN
    		SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
    	END
    
	RETURN @RetNames
  END
GO

ALTER FUNCTION [dbo].[GetFeatureNextLevelPassportsIDs]
  (	
    	@featureAlias nvarchar(100)   	
  )
  RETURNS nvarchar(1000)
  AS
  BEGIN
    	DECLARE @RetNames nvarchar(1000)
    	SET @RetNames = ''
    	

		DECLARE @tmpTable Table(PassportID int,LevelOfAuthority int ,Name nvarchar(50))

		insert into @tmpTable 
		SELECT sysroPassports.id AS PassportID, dbo.GetPassportLevelOfAuthority(sysroPassports.ID) AS LevelOfAuthority, sysroPassports.Name
    			FROM sysroPassports  
    			WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null and
    			dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID, @featureAlias,'U',2) > 3 AND Description not like '@@ROBOTICS@@%'


    	SET @RetNames = (SELECT  CONVERT(NVARCHAR(4000),PassportID )  + ','  AS [text()] from @tmpTable tt where  tt.LevelOfAuthority = (SELECT max(LevelOfAuthority) from @tmpTable) For XML PATH (''))
	
		IF @RetNames <> ''
    		BEGIN
    			SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
    		END
		
		RETURN @RetNames
  END
GO

ALTER FUNCTION [dbo].[GetRequestNextLevelPassportsIDs]
  (	
    	@idRequest int	
  )
  RETURNS nvarchar(1000)
  AS
  BEGIN
		DECLARE @RetNames nvarchar(1000)
    	SET @RetNames = ''

		SET @RetNames = (SELECT CONVERT(NVARCHAR(4000),sysroPassports.ID)  + ','  AS [text()]
    			 FROM sysroPassports 
    			 INNER JOIN (SELECT id, dbo.GetPassportLevelOfAuthority(ID) AS Level FROM sysroPassports) gpl
    			 on sysroPassports.ID=gpl.ID 
    			 WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null 
				 AND sysroPassports.Description not like '%@@ROBOTICS@@%'
    			 AND gpl.level=dbo.GetRequestNextLevel(@idRequest)
  				 AND dbo.GetRequestPassportPermission(sysroPassports.ID, @idRequest) > 3
    			 ORDER BY sysroPassports.Name For XML PATH (''))

		IF @RetNames <> ''
    		BEGIN
    			SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
    		END

    	RETURN @RetNames
  END
GO

ALTER FUNCTION [dbo].[GetSupervisedEmployeesByPassport]
  (
  	@idPassport int,
  	@featureAlias nvarchar(50)
  )
  RETURNS @result table (ID int PRIMARY KEY)
  AS
  	BEGIN
  		DECLARE @EmployeeID int
  		DECLARE @SupervisorLevel int
  		DECLARE @featureEmployeeID int
  		DECLARE @featurePermission int
  		DECLARE @parentPassport int
   		
 		DECLARE @GroupType nvarchar(50)
  	
 		SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @idPassport
   		
   		if @GroupType = 'U'
   		begin
   			SET @parentPassport = @idPassport
   		end
   		else
   		begin
   			SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @idPassport
   		end
  		
  		SELECT @SupervisorLevel = (SELECT dbo.GetPassportLevelOfAuthority(@idPassport))
  		SELECT @featureEmployeeID = (SELECT EmployeeFeatureID from sysroFeatures where Alias = @featureAlias)
  		SELECT @featurePermission = (dbo.WebLogin_GetPermissionOverFeature(@idPassport, @featureAlias,'U',2))
  		
  		IF @featurePermission > 3
  			BEGIN
  				INSERT INTO @result
 					select IDEmployee from (
 						select IDEmployee from sysrovwCurrentEmployeeGroups where IDGroup in (
 							SELECT EmployeeGroupID from (select EmployeeGroupID, MAX(LevelOfAuthority) as MaxLevel, PassportID from [dbo].[sysroPermissionsOverGroups]
  												where Permission > 3 and  EmployeeFeatureID  = @featureEmployeeID  group by EmployeeGroupID,PassportID) allPerm
 							WHERE allPerm.PassportID = @parentPassport and allPerm.MaxLevel = @SupervisorLevel)) allEmployees
 						WHERE IDEmployee not in (select EmployeeID from sysroPermissionsOverEmployeesExceptions where PassportID = @parentPassport and EmployeeFeatureID = @featureEmployeeID and Permission <= 3)
  			END
  		RETURN
  	END
GO

UPDATE dbo.sysroParameters SET Data='356' WHERE ID='DBVersion'
GO


