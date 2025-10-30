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

CREATE TABLE sysroDatabaseTexts(
[Key] NVARCHAR(MAX)
)

ALTER TABLE Causes ADD [LanguageTag] NVARCHAR(MAX)
GO
UPDATE Causes SET [LanguageTag] = dbo.GenerateKeyFromText(Name) WHERE (LanguageTag IS NULL OR LanguageTag = '')
GO

ALTER TABLE Concepts ADD [LanguageTag] NVARCHAR(MAX)
GO
UPDATE Concepts SET [LanguageTag] = dbo.GenerateKeyFromText(Name) WHERE (LanguageTag IS NULL OR LanguageTag = '')
GO

ALTER TABLE sysroReportGroups ADD [LanguageTag] NVARCHAR(MAX) 
GO
UPDATE sysroReportGroups SET [LanguageTag] = dbo.GenerateKeyFromText(Name) WHERE (LanguageTag IS NULL OR LanguageTag = '')
GO

ALTER TABLE sysroConceptTypes ADD [LanguageTag] NVARCHAR(MAX)
GO
UPDATE sysroConceptTypes SET [LanguageTag] = dbo.GenerateKeyFromText(Name) WHERE (LanguageTag IS NULL OR LanguageTag = '') 
GO

ALTER TABLE Shifts ADD [LanguageTag] NVARCHAR(MAX)
GO
UPDATE Shifts SET [LanguageTag] = dbo.GenerateKeyFromText(Name) WHERE (LanguageTag IS NULL OR LanguageTag = '') 
GO

ALTER TABLE TimeZones ADD [LanguageTag] NVARCHAR(MAX)
GO
UPDATE TimeZones SET [LanguageTag] = dbo.GenerateKeyFromText(Name) WHERE (LanguageTag IS NULL OR LanguageTag = '') 
GO

ALTER TABLE Groups ADD [LanguageTag] NVARCHAR(MAX)
GO
UPDATE Groups SET [LanguageTag] = dbo.GenerateKeyFromText(Name) WHERE (LanguageTag IS NULL OR LanguageTag = '') 
GO

ALTER TABLE sysroQueries ADD [LanguageTag] NVARCHAR(MAX)
GO
UPDATE sysroQueries SET [LanguageTag] = dbo.GenerateKeyFromText(Name) WHERE (LanguageTag IS NULL OR LanguageTag = '')
GO

ALTER TABLE sysroUserFields ADD [LanguageTag] NVARCHAR(MAX) 
GO
UPDATE sysroUserFields SET [LanguageTag] = dbo.GenerateKeyFromText(FieldName) WHERE (LanguageTag IS NULL OR LanguageTag = '')
GO

ALTER TABLE LabAgree ADD [LanguageTag] NVARCHAR(MAX) 
GO
UPDATE LabAgree SET [LanguageTag] = dbo.GenerateKeyFromText(Name) WHERE (LanguageTag IS NULL OR LanguageTag = '')
GO

ALTER TABLE AccessGroups ADD [LanguageTag] NVARCHAR(MAX) 
GO
UPDATE AccessGroups SET [LanguageTag] = dbo.GenerateKeyFromText(Name) WHERE (LanguageTag IS NULL OR LanguageTag = '')
GO

ALTER TABLE AccessPeriods ADD [LanguageTag] NVARCHAR(MAX) 
GO
UPDATE AccessPeriods SET [LanguageTag] = dbo.GenerateKeyFromText(Name) WHERE (LanguageTag IS NULL OR LanguageTag = '')
GO

ALTER TABLE Zones ADD [LanguageTag] NVARCHAR(MAX) 
GO
UPDATE Zones SET [LanguageTag] = ''
GO
UPDATE Zones SET [LanguageTag] = dbo.GenerateKeyFromText(Name) WHERE (LanguageTag IS NULL OR LanguageTag = '') AND ID NOT IN (1,255)
GO

ALTER TABLE StartupValues ADD [LanguageTag] NVARCHAR(MAX) 
GO
UPDATE StartupValues SET [LanguageTag] = ''
GO
UPDATE StartupValues SET [LanguageTag] = dbo.GenerateKeyFromText(Name) WHERE (LanguageTag IS NULL OR LanguageTag = '')
GO

ALTER TABLE CauseLimitValues ADD [LanguageTag] NVARCHAR(MAX) 
GO
UPDATE CauseLimitValues SET [LanguageTag] = ''
GO
UPDATE CauseLimitValues SET [LanguageTag] = dbo.GenerateKeyFromText(Name) WHERE (LanguageTag IS NULL OR LanguageTag = '')
GO

ALTER TABLE AccrualsRules ADD [LanguageTag] NVARCHAR(MAX) 
GO
UPDATE AccrualsRules SET [LanguageTag] = ''
GO
UPDATE AccrualsRules SET [LanguageTag] = dbo.GenerateKeyFromText(Name) WHERE (LanguageTag IS NULL OR LanguageTag = '')
GO

ALTER TABLE RequestsRules ADD [LanguageTag] NVARCHAR(MAX) 
GO
UPDATE RequestsRules SET [LanguageTag] = ''
GO
UPDATE RequestsRules SET [LanguageTag] = dbo.GenerateKeyFromText(Name) WHERE (LanguageTag IS NULL OR LanguageTag = '')
GO
