DROP TABLE [dbo].[sysroPassports_Notifications]
GO

CREATE TABLE dbo.sysroPassports_Data
	(
	IDPassport int NOT NULL,
	LastRequestNotification datetime NULL,
	SecurityToken nvarchar(50) NULL,
	AuthToken nvarchar(50) NULL,
	SessionContext nvarchar(max)
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.sysroPassports_Data ADD CONSTRAINT
	PK_sysroPassports_Data PRIMARY KEY CLUSTERED 
	(
	IDPassport
	) ON [PRIMARY]

GO

ALTER TABLE dbo.sysroPassports_Data ADD CONSTRAINT
	FK_sysroPassports_Data_sysroPassports FOREIGN KEY
	(
	IDPassport
	) REFERENCES dbo.sysroPassports
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

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

	DELETE FROM sysroPassports_Data
    	WHERE IDPassport = @id
    	
  	DELETE FROM sysroPassports
    	WHERE ID = @id
    	
    	SET NOCOUNT OFF
    	
    	RETURN
GO

UPDATE dbo.sysroParameters SET Data='406' WHERE ID='DBVersion'
GO
