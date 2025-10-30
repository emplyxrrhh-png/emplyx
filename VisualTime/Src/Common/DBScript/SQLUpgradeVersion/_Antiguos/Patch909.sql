-- No borréis esta línea
CREATE FUNCTION dbo.GenerateKeyFromText (@Texto NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
BEGIN
    -- Primero, eliminamos los acentos reemplazando las vocales acentuadas
    SET @Texto = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@Texto, 'á', 'a'), 'é', 'e'), 'í', 'i'), 'ó', 'o'), 'ú', 'u')
    SET @Texto = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@Texto, 'Á', 'A'), 'É', 'E'), 'Í', 'I'), 'Ó', 'O'), 'Ú', 'U')
	SET @Texto = REPLACE(@Texto, '+', ' ')

    DECLARE @Resultado NVARCHAR(MAX) = ''
    DECLARE @Palabra NVARCHAR(MAX) = ''
    DECLARE @Longitud INT = LEN(@Texto)
    DECLARE @Indice INT = 1
    DECLARE @Char NCHAR(1)

    -- Recorremos cada carácter del texto
    WHILE @Indice <= @Longitud
    BEGIN
        SET @Char = SUBSTRING(@Texto, @Indice, 1)
        
        -- Verificamos si el carácter es alfabético o numérico
        IF @Char LIKE '[a-zA-Z0-9]'
        BEGIN
            SET @Palabra = @Palabra + @Char
        END
        ELSE IF @Char = ' ' AND LEN(@Palabra) > 0
        BEGIN
            -- Si encontramos un espacio y ya tenemos una palabra, la añadimos al resultado en camel case
            SET @Resultado = @Resultado + UPPER(LEFT(@Palabra, 1)) + LOWER(SUBSTRING(@Palabra, 2, LEN(@Palabra) - 1))
            SET @Palabra = ''
        END

        SET @Indice = @Indice + 1
    END

    -- Añadir la última palabra al resultado si existe
    IF LEN(@Palabra) > 0
    BEGIN
        SET @Resultado = @Resultado + UPPER(LEFT(@Palabra, 1)) + LOWER(SUBSTRING(@Palabra, 2, LEN(@Palabra) - 1))
    END

    -- Devolver el resultado
    RETURN @Resultado
END
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='909' WHERE ID='DBVersion'
GO
