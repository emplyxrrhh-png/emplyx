ALTER FUNCTION [dbo].[TryConvertDate]
 (
   @value nvarchar(4000)
 )
RETURNS date
AS
BEGIN
DECLARE @resDate date
DECLARE @DateYear nvarchar(4)
DECLARE @DateMonth nvarchar(2)
DECLARE @DateDay nvarchar(2)

IF LEN(@value) = 10
BEGIN
 	SET @DateYear = LEFT(@value, 4)				-- YYYY
	SET @DateMonth = SUBSTRING(@value, 6, 2)	-- MM
	SET @DateDay = RIGHT(@value, 2)				-- DD
 
	IF ISDATE(@DateYear+'/'+@DateMonth+'/'+@DateDay) = 1
	BEGIN
 		SET @resDate = (SELECT CONVERT(date, @value))
	END
	ELSE
	BEGIN
 		IF ISDATE(@DateYear+'/'+@DateDay+'/'+@DateMonth) = 1
		BEGIN
 			SET @resDate = (SELECT CONVERT(date, @value))
		END
 		ELSE
		BEGIN
 			SET @resDate = (SELECT CONVERT(date, ''))
		END
	END
END
RETURN @resDate
END
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO


UPDATE sysroParameters SET Data='568' WHERE ID='DBVersion'
GO