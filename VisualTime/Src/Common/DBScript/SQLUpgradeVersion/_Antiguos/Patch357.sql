 ALTER TABLE [dbo].[Punches] ADD CRC NVARCHAR(13)
 GO
 
 CREATE PROCEDURE [dbo].[WebLogin_Passports_SelectDetailByParent]
 	(
 		@idParentPassport int = NULL,
 		@groupType varchar(1) = NULL
 	)
 AS
 	SELECT ID,Name, Description, GroupType
 	FROM sysroPassports
 	WHERE ((IDParentPassport IS NULL AND @idParentPassport IS NULL) OR
 		IDParentPassport = @idParentPassport) AND
 		(@groupType IS NULL OR GroupType = @groupType)
 	ORDER BY GroupType, Name
 	
 	RETURN

GO
 
UPDATE dbo.sysroParameters SET Data='357' WHERE ID='DBVersion'
GO


