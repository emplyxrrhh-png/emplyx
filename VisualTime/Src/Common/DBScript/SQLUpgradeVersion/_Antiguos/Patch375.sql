CREATE FUNCTION [dbo].[IsBlockDateRestrictionActive]
  (	
 	@idPassport int,
	@idGroup int,
	@date datetime
  )
  RETURNS int
  AS
  BEGIN
    	
		DECLARE @GroupCloseDate datetime
		DECLARE @RestrictionDisabled int
		DECLARE @BlockDateActive int

		Set @BlockDateActive = 0

		-- Si @RestrictionDisabled = 0, no debemos aplicar la restricción de fecha
    	SELECT @RestrictionDisabled = COUNT(*) FROM sysroSecurityParameters WHERE (IDPassport IN (SELECT ID FROM GetPassportParents(@idPassport)) OR IDPassport IN (0,@idPassport)) AND CalendarLock = 0
    	   		   
		IF @RestrictionDisabled = 0 
		BEGIN
			select @GroupCloseDate = CloseDate from Groups WHERE ID = dbo.GetCompanyGroup(@idGroup)
            
			IF @GroupCloseDate IS NOT NULL 
			BEGIN
				IF @GroupCloseDate >= @date SET @BlockDateActive = 1
			END
			
		END

		return @BlockDateActive
  END
GO

ALTER TABLE dbo.Visit ADD
            Name nvarchar(150) NOT NULL CONSTRAINT DF_Visit_Name DEFAULT ''
GO


UPDATE dbo.sysroParameters SET Data='375' WHERE ID='DBVersion'
GO
