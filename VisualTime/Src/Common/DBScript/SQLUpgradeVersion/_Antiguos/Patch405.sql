 ALTER PROCEDURE [dbo].[WebLogin_CredentialExists] 
   	(
   		@credential nvarchar(255),
  		@method int,
  		@version nvarchar(50),
   		@idPassport int = NULL		
   	)
   AS
   IF @method = 1
   BEGIN
		IF EXISTS (
   			SELECT ID
   			FROM sysroPassports p
   				 LEFT JOIN sysroPassports_AuthenticationMethods a ON p.ID = a.IDPassport			 			 
   			WHERE a.Method = @method AND a.version = @version AND a.Credential = @credential
 				  AND (@idPassport IS NULL OR IDPassport <> @idPassport) AND a.Enabled = 1 AND
  				  (p.IDEmployee IS NULL OR
  				  ISNULL((SELECT COUNT(*) FROM EmployeeContracts 
  						  WHERE EmployeeContracts.IDEmployee = p.IDEmployee AND
  								EmployeeContracts.BeginDate <= GETDATE() AND EmployeeContracts.EndDate >= GETDATE()), 0) > 0) 
   		)
   			SELECT 1
   		ELSE
  			SELECT 0
   	END
	ELSE
	BEGIN
		IF EXISTS (
   			SELECT ID
   			FROM sysroPassports p
   				 LEFT JOIN sysroPassports_AuthenticationMethods a ON p.ID = a.IDPassport			 			 
   			WHERE a.Method = @method AND
  				  a.version = @version AND
   				  (a.Credential = @credential OR (a.credential IN (select convert(nvarchar,IDCard) from cardaliases where convert(numeric(20,0),RealValue) = convert(numeric(20,0),@credential))))			  
 				  AND
   				  (@idPassport IS NULL OR IDPassport <> @idPassport) AND
  				  a.Enabled = 1 AND
  				  (p.IDEmployee IS NULL OR
  				  ISNULL((SELECT COUNT(*) FROM EmployeeContracts 
  						  WHERE EmployeeContracts.IDEmployee = p.IDEmployee AND
  								EmployeeContracts.BeginDate <= GETDATE() AND EmployeeContracts.EndDate >= GETDATE()), 0) > 0) 
   		)
   			SELECT 1
   		ELSE
  			SELECT 0
	END

   	RETURN
GO


ALTER TABLE [dbo].[Causes] ADD [ExternalWork] [bit] NULL DEFAULT(0)
GO
 
UPDATE [dbo].[Causes] Set ExternalWork= 0 WHERE ExternalWork IS NULL
GO 

ALTER TABLE [dbo].[Entries] ADD [UsesIdEmployee] [bit] NULL 
GO

insert into dbo.sysronotificationtypes values(48,'Requests Result',NULL,360,'','',1)
GO
insert into dbo.notifications values(1910,48,'Avido de cambio de estado en solicitud','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>',NULL,1,'SYSTEM',0,0,0,0,NULL)
GO

CREATE TABLE dbo.sysroPassports_Notifications
	(
	IDPassport int NOT NULL,
	LastRequestNotification datetime NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.sysroPassports_Notifications ADD CONSTRAINT
	PK_sysroPassports_Notifications PRIMARY KEY CLUSTERED 
	(
	IDPassport
	) ON [PRIMARY]

GO

ALTER TABLE dbo.sysroPassports_Notifications ADD CONSTRAINT
	FK_sysroPassports_Notifications_sysroPassports FOREIGN KEY
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

	DELETE FROM sysroPassports_Notifications
    	WHERE IDPassport = @id
    	
  	DELETE FROM sysroPassports
    	WHERE ID = @id
    	
    	SET NOCOUNT OFF
    	
    	RETURN
GO

-- Añadimos parámetro para poder indicar que el formato de los tiempos en productiv es en cestesimal
INSERT INTO dbo.sysroLiveAdvancedParameters ([ParameterName],[Value]) VALUES ('OnlyFirstNotificationBetweenSupervisors','0')
GO

INSERT INTO dbo.sysroLiveAdvancedParameters ([ParameterName],[Value]) VALUES ('ConfirmHolidayOverFlow','0')
GO

-- Backup de transacciones de fichajes
CREATE TABLE [dbo].[sysroPunchesTransactions](
	[CommandText] [text] NOT NULL,
	[PunchDate] [smalldatetime] NOT NULL,
	[IDEmployee] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[Exported] [bit] NOT NULL default(0)
	)
GO

UPDATE dbo.sysroParameters SET Data='405' WHERE ID='DBVersion'
GO
