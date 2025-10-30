
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
    				  (a.Credential = @credential OR (a.credential IN (select convert(nvarchar,IDCard) from cardaliases where convert(numeric(20,0),RealValue) = convert(numeric(20,0),REPLACE(@credential,'.','')))))			  
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

    GO

delete from sysroFeatures where (alias = 'Visits.Definition' or Alias = 'Visits.Program') and Type = 'E'
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='624' WHERE ID='DBVersion'
GO
