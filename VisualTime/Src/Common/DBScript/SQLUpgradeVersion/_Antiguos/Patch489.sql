UPDATE [dbo].[sysrogui] SET RequiredFeatures = 'Feature\SaaS' WHERE IDPath = 'Portal\GeneralManagement\Communiques'
GO

CREATE FUNCTION dbo.SplitString (@stringToSplit VARCHAR(MAX), @separator VARCHAR(1))
RETURNS @returnList TABLE ([Name] [nvarchar] (500))
AS
	BEGIN
		DECLARE @name NVARCHAR(255)
		DECLARE @pos INT

		WHILE CHARINDEX(@separator, @stringToSplit) > 0
		BEGIN
		  SELECT @pos  = CHARINDEX(@separator, @stringToSplit)  
		  SELECT @name = SUBSTRING(@stringToSplit, 1, @pos-1)

		  INSERT INTO @returnList 
		  SELECT @name

		  SELECT @stringToSplit = SUBSTRING(@stringToSplit, @pos+1, LEN(@stringToSplit)-@pos)
		END

		INSERT INTO @returnList
		SELECT @stringToSplit

		RETURN 
	END
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='489' WHERE ID='DBVersion'
GO

