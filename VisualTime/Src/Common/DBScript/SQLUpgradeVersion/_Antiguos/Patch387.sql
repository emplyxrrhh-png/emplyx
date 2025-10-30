 ALTER PROCEDURE [dbo].[WebLogin_PlateExists] 
 (
 	@credential nvarchar(255),
 	@method int,
 	@version int,
 	@idPassport int
 )
 AS
 IF @credential = '' SELECT 0 
 ELSE
 	IF EXISTS (
 			SELECT ID
  				FROM sysroPassports p
  					LEFT JOIN sysroPassports_AuthenticationMethods a ON p.ID = a.IDPassport			 			 
 			WHERE  ((a.Method = @method) AND (a.Credential = @credential) AND (a.IDPassport <> @idPassport) AND a.Enabled = 1) OR
 			       ((a.Method = @method) AND (a.Credential = @credential) AND (a.Version <> @version) AND (a.IDPassport = @idPassport) AND a.Enabled = 1) AND
 				   (p.IDEmployee IS NULL OR ISNULL((SELECT COUNT(*) FROM EmployeeContracts WHERE EmployeeContracts.IDEmployee = p.IDEmployee AND EmployeeContracts.BeginDate <= GETDATE() AND EmployeeContracts.EndDate >= GETDATE()), 0) > 0)
 		)
 		SELECT 1
 	ELSE
 		SELECT 0
 RETURN
GO

 
UPDATE dbo.sysroParameters SET Data='387' WHERE ID='DBVersion'
GO
