-- No borréis esta línea
CREATE OR ALTER FUNCTION dbo.EncodeExternalId (@id INT)
RETURNS VARCHAR(50)
AS
BEGIN
	DECLARE @shiftedid INT = @id + 100000
    DECLARE @hex VARCHAR(20) = CONVERT(VARCHAR(20), FORMAT(@shiftedid, 'X')) -- convierte a hex
    DECLARE @i INT = 1
    DECLARE @result VARCHAR(50) = ''

    WHILE @i <= LEN(@hex)
    BEGIN
        DECLARE @char CHAR(1) = SUBSTRING(@hex, @i, 1)
        DECLARE @dec INT =
            CASE 
                WHEN @char BETWEEN '0' AND '9' THEN ASCII(@char) - ASCII('0')
                WHEN @char BETWEEN 'A' AND 'F' THEN ASCII(@char) - ASCII('A') + 10
            END
        SET @result += RIGHT('00' + CAST(@dec AS VARCHAR), 2)
        SET @i += 1
    END

    RETURN @result
END
GO

DROP TABLE TMPDummyRulesGroups
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1059' WHERE ID='DBVersion'
GO
