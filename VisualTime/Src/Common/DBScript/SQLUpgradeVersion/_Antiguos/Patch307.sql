
CREATE PROCEDURE dbo.WebLogin_PermissionsOverEmployees_Get_Extended
	@idPassport int,
	@idEmployee int,
	@idApplication int,
	@mode int,
	@includeGroups bit,
	@date datetime
AS
BEGIN
	SELECT dbo.WebLogin_GetPermissionOverEmployee(@idPassport, @idEmployee, @idApplication, @mode, @includeGroups, @date) AS Permission
 	
 	RETURN
END
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='307' WHERE ID='DBVersion'
GO
