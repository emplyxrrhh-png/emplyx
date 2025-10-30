-- Borramos las tablas antiguas temporales de permisos ya no se utilizan
DROP TABLE dbo.TmpRequestPassportPermissions
GO

DROP TABLE dbo.TMPPermissions
GO

-- Fin Borramos las tablas antiguas temporales de permisos ya no se utilizan

 ALTER PROCEDURE [dbo].[WebLogin_Passports_Delete] 
   	(
   		@id int
   	)
   AS
   	SET NOCOUNT ON
   	
 	DELETE FROM sysroPermissionsOverEmployeesExceptions
 	WHERE PassportID = @id

	DELETE FROM sysroPermissionsOverFeatures
 	WHERE PassportID = @id

	DELETE FROM sysroPermissionsOverGroups
 	WHERE PassportID = @id

	DELETE FROM sysroPermissionsOverRequests
 	WHERE IDParentPassport = @id

   	DELETE FROM sysroSecurityParameters
   	WHERE IDPassport = @id
   	 	
   	DELETE FROM sysroPassports_PasswordHistory
   	WHERE IDPassport = @id
   	
  	DELETE FROM sysroPassports_AuthorizedAdress
   	WHERE IDPassport = @id
   	
 	DELETE FROM sysroPassports
   	WHERE ID = @id
   	
   	SET NOCOUNT OFF
   	
   	RETURN
GO

ALTER TABLE dbo.sysroPassports_Sessions ADD
	InvalidationCode int NOT NULL CONSTRAINT DF_sysroPassports_Sessions_InvalidationCode DEFAULT 0
GO


CREATE PROCEDURE [dbo].[sysro_SetInvalidationCodeSession]
	(
		@idPassport int,
		@invalidationCode int
	)
    AS
    BEGIN
  	
	DECLARE @insertPassportIDs table(IDPassport int)
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

	;WITH cteInsert AS 
	(
		SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType
		FROM sysroPassports a
		WHERE Id = @parentPassport AND GroupType = 'U'
		UNION ALL
		SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType
		FROM sysropassports a JOIN cteInsert c ON a.IDParentPassport = c.id
		where a.GroupType <> 'U'
	)
	INSERT INTO @insertPassportIDs SELECT Id FROM cteInsert

	UPDATE sysroPassports_Sessions SET InvalidationCode = @invalidationCode WHERE IDPassport IN (SELECT IDPassport from @insertPassportIDs)
	   
END
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='334' WHERE ID='DBVersion'
GO


