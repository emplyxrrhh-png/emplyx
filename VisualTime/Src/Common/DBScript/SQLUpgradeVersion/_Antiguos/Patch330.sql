-- Creación de alertas de solicitud pendiente para los últimos 30 días
INSERT INTO sysroNotificationtasks (IDNotification, Key1Numeric,  FiredDate) 
SELECT 2000, ID, RequestDate 
FROM requests 
WHERE status not in (2,3)
AND RequestDate >= dateadd(d,-30,getdate())
AND NOT EXISTS (SELECT *  From sysroNotificationTasks  
Where sysroNotificationTasks.Key1Numeric = requests.ID  AND IDNotification=2000) 
GO

-- Corrección de WebLogin_GetPermissionOverGroup
ALTER FUNCTION [dbo].[WebLogin_GetPermissionOverGroup] 
 	(
 		@idPassport int,
 		@idGroup int,
 		@idApplication int,
 		@mode int
 	)
 RETURNS int
 AS
 BEGIN
 	/* In addition to the 3 normal modes 
 		0=Normal
 		1=Direct Only
 		2=Inherited Only, including group inheritance
 		  within current passport
 	there is a 4th mode:
 		3=Inherited Only, excluding group inheritance 
 		  within current passport (usefull for MaxConfigurable) */
 	
 	DECLARE @Result int
 	DECLARE @NewResult int
 	DECLARE @ParentPassport int
  	DECLARE @GroupType nvarchar(50)
 	
 	/* Return the most restricted passport permission by looking at parent
 	passport's permissions one by one */
 	
 	IF @mode <> 3
 		SELECT @Result = dbo.WebLogin_GetPassportPermissionOverGroup(@idPassport, @idGroup, @idApplication, @mode)
 	
 	IF @mode <> 1
 	BEGIN
 		
		SELECT @GroupType = isnull(GroupType, '')
  		FROM sysroPassports
  		WHERE ID = @idPassport
  		
  		if @GroupType = 'U'
  		begin
  			SET @ParentPassport = @idPassport
  		end
  		else
  		begin
  			SELECT @ParentPassport = IDParentPassport
  			FROM sysroPassports
  			WHERE ID = @idPassport
  		end
		 		
 		/* If looking for inherited only, the constraint is for first check only.
 		Other queries should be threated as normal */
 		WHILE NOT @ParentPassport IS NULL
 		BEGIN
 			SELECT @NewResult = dbo.WebLogin_GetPassportPermissionOverGroup(@ParentPassport, @idGroup, @idApplication, 0)
 			IF NOT @NewResult IS NULL AND @NewResult < @Result OR @Result IS NULL
 				SET @Result = @NewResult
 			SELECT @ParentPassport = IDParentPassport
 			FROM sysroPassports
 			WHERE ID = @ParentPassport
 		END
 	END
 	
 	/* Return result */
 	IF @Result IS NULL
 		SET @Result = 0
 	
 	RETURN @Result
 END
 GO
  
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='330' WHERE ID='DBVersion'
GO


