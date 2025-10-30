
CREATE PROCEDURE [dbo].[WebLogin_PlateExists] 
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
			SELECT IDPassport
			FROM   sysroPassports_AuthenticationMethods
			WHERE  ((Method = @method) AND (Credential = @credential) AND (IDPassport <> @idPassport)) OR
			       ((Method = @method) AND (Credential = @credential) AND (Version <> @version) AND (IDPassport = @idPassport))
		)
		SELECT 1
	ELSE
		SELECT 0
RETURN
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='293' WHERE ID='DBVersion'
GO
