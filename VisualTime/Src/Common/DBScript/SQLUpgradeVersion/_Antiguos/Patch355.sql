ALTER FUNCTION [dbo].[GetFeatureNextLevelPassportsByEmployee]
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

	SET @RetNames = (SELECT  Name  + ','  AS [text()] from @tmpTable tt where  tt.LevelOfAuthority = (SELECT max(LevelOfAuthority) from @tmpTable) For XML PATH (''))
	
	IF @RetNames <> ''
    	BEGIN
    		SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
    	END
		
	RETURN @RetNames
   END
GO

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

	SET @RetNames = (SELECT  PassportID  + ','  AS [text()] from @tmpTable tt where  tt.LevelOfAuthority = (SELECT max(LevelOfAuthority) from @tmpTable) For XML PATH (''))

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


    	SET @RetNames = (SELECT  PassportID  + ','  AS [text()] from @tmpTable tt where  tt.LevelOfAuthority = (SELECT max(LevelOfAuthority) from @tmpTable) For XML PATH (''))
	
		IF @RetNames <> ''
    		BEGIN
    			SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
    		END
		
		RETURN @RetNames
  END
GO

 ALTER FUNCTION [dbo].[GetRequestNextLevelPassports]
  (	
    	@idRequest int	
  )
  RETURNS nvarchar(1000)
  AS
  BEGIN
    	DECLARE @RetNames nvarchar(1000)
    	SET @RetNames = ''

		SET @RetNames = (SELECT sysroPassports.Name + ','  AS [text()]
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

ALTER FUNCTION [dbo].[GetRequestNextLevelPassportsIDs]
  (	
    	@idRequest int	
  )
  RETURNS nvarchar(1000)
  AS
  BEGIN
		DECLARE @RetNames nvarchar(1000)
    	SET @RetNames = ''

		SET @RetNames = (SELECT sysroPassports.ID  + ','  AS [text()]
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

UPDATE dbo.sysroParameters SET Data='355' WHERE ID='DBVersion'
GO


