CREATE OR ALTER FUNCTION dbo.GenerateShortName_SQL (
    @fullName NVARCHAR(MAX),
    @existingShortNamesString NVARCHAR(MAX) = NULL, -- Comma-separated list of existing short names
    @length INT = 3,
    @timeFormat BIT = 1,                             -- Whether to handle time format (XX:XX - XX:XX) specially
    @singleWordMode VARCHAR(10) = 'prefix',          -- 'prefix' or 'initials' for single word
    @fillMode VARCHAR(10) = 'letters'                -- 'letters' or 'numbers' to fill if short
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    -- Variable declarations
    DECLARE @shortName NVARCHAR(MAX) = '';
    DECLARE @words TABLE (id INT IDENTITY(1,1) PRIMARY KEY, word NVARCHAR(MAX) NOT NULL);
    DECLARE @wordCount INT = 0;

    -- Validate and default options
    IF @length IS NULL OR @length <= 0 SET @length = 3;
    IF @singleWordMode IS NULL OR @singleWordMode NOT IN ('prefix', 'initials') SET @singleWordMode = 'prefix';
    IF @fillMode IS NULL OR @fillMode NOT IN ('letters', 'numbers') SET @fillMode = 'letters';
    IF @timeFormat IS NULL SET @timeFormat = 1;

    -- Handle empty or null fullName
    IF @fullName IS NULL OR LTRIM(RTRIM(@fullName)) = ''
    BEGIN
        RETURN REPLICATE('1', @length);
    END

    SET @fullName = LTRIM(RTRIM(@fullName));

    SET @fullName = REPLACE(@fullName, 'á', 'a'); SET @fullName = REPLACE(@fullName, 'Á', 'A');
    SET @fullName = REPLACE(@fullName, 'é', 'e'); SET @fullName = REPLACE(@fullName, 'É', 'E');
    SET @fullName = REPLACE(@fullName, 'í', 'i'); SET @fullName = REPLACE(@fullName, 'Í', 'I');
    SET @fullName = REPLACE(@fullName, 'ó', 'o'); SET @fullName = REPLACE(@fullName, 'Ó', 'O');
    SET @fullName = REPLACE(@fullName, 'ú', 'u'); SET @fullName = REPLACE(@fullName, 'Ú', 'U');
    SET @fullName = REPLACE(@fullName, 'à', 'a'); SET @fullName = REPLACE(@fullName, 'À', 'A');
    SET @fullName = REPLACE(@fullName, 'è', 'e'); SET @fullName = REPLACE(@fullName, 'È', 'E');
    SET @fullName = REPLACE(@fullName, 'ì', 'i'); SET @fullName = REPLACE(@fullName, 'Ì', 'I');
    SET @fullName = REPLACE(@fullName, 'ò', 'o'); SET @fullName = REPLACE(@fullName, 'Ò', 'O');
    SET @fullName = REPLACE(@fullName, 'ù', 'u'); SET @fullName = REPLACE(@fullName, 'Ù', 'U');
    SET @fullName = REPLACE(@fullName, 'â', 'a'); SET @fullName = REPLACE(@fullName, 'Â', 'A');
    SET @fullName = REPLACE(@fullName, 'ê', 'e'); SET @fullName = REPLACE(@fullName, 'Ê', 'E');
    SET @fullName = REPLACE(@fullName, 'î', 'i'); SET @fullName = REPLACE(@fullName, 'Î', 'I');
    SET @fullName = REPLACE(@fullName, 'ô', 'o'); SET @fullName = REPLACE(@fullName, 'Ô', 'O');
    SET @fullName = REPLACE(@fullName, 'û', 'u'); SET @fullName = REPLACE(@fullName, 'Û', 'U');
    SET @fullName = REPLACE(@fullName, 'ä', 'a'); SET @fullName = REPLACE(@fullName, 'Ä', 'A');
    SET @fullName = REPLACE(@fullName, 'ë', 'e'); SET @fullName = REPLACE(@fullName, 'Ë', 'E');
    SET @fullName = REPLACE(@fullName, 'ï', 'i'); SET @fullName = REPLACE(@fullName, 'Ï', 'I');
    SET @fullName = REPLACE(@fullName, 'ö', 'o'); SET @fullName = REPLACE(@fullName, 'Ö', 'O');
    SET @fullName = REPLACE(@fullName, 'ü', 'u'); SET @fullName = REPLACE(@fullName, 'Ü', 'U');
    SET @fullName = REPLACE(@fullName, 'ã', 'a'); SET @fullName = REPLACE(@fullName, 'Ã', 'A');
    SET @fullName = REPLACE(@fullName, 'õ', 'o'); SET @fullName = REPLACE(@fullName, 'Õ', 'O');
    SET @fullName = REPLACE(@fullName, 'ñ', 'n'); SET @fullName = REPLACE(@fullName, 'Ñ', 'N');
    SET @fullName = REPLACE(@fullName, 'ç', 'c'); SET @fullName = REPLACE(@fullName, 'Ç', 'C');

    -- Split fullName into words (requires SQL Server 2016+ for STRING_SPLIT)
    -- Filter out empty strings that might result from multiple spaces
    IF LEN(@fullName) > 0
    BEGIN
        INSERT INTO @words (word) SELECT LTRIM(RTRIM(value)) FROM Split(@fullName, ' ') WHERE LTRIM(RTRIM(value)) <> '';
        SELECT @wordCount = COUNT(*) FROM @words;
    END

    -- If no words found (e.g., fullName was only spaces or became empty after normalization)
    IF @wordCount = 0
    BEGIN
         RETURN REPLICATE('1', @length);
    END

    -- Time Format Logic
    DECLARE @hasTimeFormatHandled BIT = 0;
    IF @timeFormat = 1
    BEGIN
        DECLARE @firstWord_tf NVARCHAR(MAX) = (SELECT word FROM @words WHERE id = 1);
        
        IF @wordCount >= 3
        BEGIN
            DECLARE @secondWord_tf NVARCHAR(MAX) = (SELECT word FROM @words WHERE id = 2);
            DECLARE @thirdWord_tf NVARCHAR(MAX) = (SELECT word FROM @words WHERE id = 3);

            IF @firstWord_tf LIKE '[0-9][0-9]:[0-9][0-9]' AND @secondWord_tf = '-' AND @thirdWord_tf LIKE '[0-9][0-9]:[0-9][0-9]'
            BEGIN
                SET @hasTimeFormatHandled = 1;
                DECLARE @time_word_idx INT = 4; -- Start from the 4th word
                WHILE @time_word_idx <= @wordCount
                BEGIN
                    IF LEN(@shortName) >= @length BREAK;
                    DECLARE @current_time_word NVARCHAR(MAX);
                    SELECT @current_time_word = word FROM @words WHERE id = @time_word_idx;
                    IF LEN(@current_time_word) > 0
                    BEGIN
                        SET @shortName = @shortName + UPPER(LEFT(@current_time_word, 1));
                    END
                    SET @time_word_idx = @time_word_idx + 1;
                END
            END
        END

        IF @hasTimeFormatHandled = 0 AND @wordCount = 1 AND @firstWord_tf LIKE '[0-9][0-9]:[0-9][0-9]'
        BEGIN
            SET @hasTimeFormatHandled = 1;
            SET @shortName = REPLACE(@firstWord_tf, ':', '');
        END
    END

    -- Main ShortName Generation (if not handled by time format logic)
    IF @hasTimeFormatHandled = 0
    BEGIN
        IF @wordCount > 1
        BEGIN
            SET @shortName = @shortName + UPPER(LEFT((SELECT word FROM @words WHERE id = 1), 1));
            IF LEN(@shortName) < @length 
                SET @shortName = @shortName + UPPER(LEFT((SELECT word FROM @words WHERE id = 2), 1));
            
            IF @wordCount > 2 AND LEN(@shortName) < @length
            BEGIN
                 SET @shortName = @shortName + UPPER(LEFT((SELECT TOP 1 word FROM @words ORDER BY id DESC), 1)); -- Last word's initial
            END
        END
        ELSE IF @wordCount = 1 -- Single word
        BEGIN
            DECLARE @singleWordValue NVARCHAR(MAX) = (SELECT word FROM @words WHERE id = 1);
            IF @singleWordMode = 'prefix'
            BEGIN
                SET @shortName = UPPER(LEFT(@singleWordValue, @length));
            END
            ELSE -- 'initials'
            BEGIN
                SET @shortName = UPPER(LEFT(@singleWordValue, 1));
                IF LEN(@singleWordValue) >= 3
                BEGIN
                    IF LEN(@shortName) < @length SET @shortName = @shortName + UPPER(SUBSTRING(@singleWordValue, (LEN(@singleWordValue) / 2) + 1, 1));
                    IF LEN(@shortName) < @length SET @shortName = @shortName + UPPER(RIGHT(@singleWordValue, 1));
                END
                ELSE IF LEN(@singleWordValue) = 2
                BEGIN
                    IF LEN(@shortName) < @length SET @shortName = @shortName + UPPER(RIGHT(@singleWordValue, 1));
                END
            END
        END
    END

    -- Truncate if longer than requested length (intermediate step)
    IF LEN(@shortName) > @length SET @shortName = LEFT(@shortName, @length);

    -- Add more characters if shorter than requested length
    IF LEN(@shortName) < @length
    BEGIN
        IF @fillMode = 'letters'
        BEGIN
            DECLARE @fill_word_id INT = 1;
            WHILE LEN(@shortName) < @length AND @fill_word_id <= @wordCount
            BEGIN
                DECLARE @word_to_fill_from NVARCHAR(MAX);
                SELECT @word_to_fill_from = word FROM @words WHERE id = @fill_word_id;

                DECLARE @char_idx_in_word INT = 2; -- Start from the second character for filling (JS: j=1)
                WHILE @char_idx_in_word <= LEN(@word_to_fill_from) AND LEN(@shortName) < @length
                BEGIN
                    SET @shortName = @shortName + UPPER(SUBSTRING(@word_to_fill_from, @char_idx_in_word, 1));
                    SET @char_idx_in_word = @char_idx_in_word + 1;
                END
                SET @fill_word_id = @fill_word_id + 1;
            END
        END

        -- If still too short, add numbers
        DECLARE @numIndex INT = 1;
        WHILE LEN(@shortName) < @length
        BEGIN
            SET @shortName = @shortName + CAST(@numIndex AS VARCHAR(10)); 
            SET @numIndex = @numIndex + 1;
        END
    END

    SET @shortName = LEFT(@shortName, @length);

    -- Handle duplicates if @existingShortNamesString is provided
    IF @existingShortNamesString IS NOT NULL AND LTRIM(RTRIM(@existingShortNamesString)) <> ''
    BEGIN
        DECLARE @ExistingNamesTable TABLE (name NVARCHAR(MAX) COLLATE Latin1_General_CS_AS); 
        INSERT INTO @ExistingNamesTable (name) SELECT LTRIM(RTRIM(value)) FROM Split(@existingShortNamesString, ',');

        IF EXISTS (SELECT 1 FROM @ExistingNamesTable WHERE name = @shortName)
        BEGIN
            DECLARE @originalGeneratedShortName NVARCHAR(MAX) = @shortName; 
            DECLARE @dupIndex INT = 1;
            DECLARE @tempShortName NVARCHAR(MAX) = @shortName;

            WHILE (1=1) 
            BEGIN
                DECLARE @indexStrForDup NVARCHAR(10) = CAST(@dupIndex AS VARCHAR(10));
                DECLARE @keepCharsForDup INT = @length - LEN(@indexStrForDup);

                IF @length > 0 AND @keepCharsForDup < 1 SET @keepCharsForDup = 1;
                ELSE IF @keepCharsForDup < 0 SET @keepCharsForDup = 0;

                DECLARE @candidateName NVARCHAR(MAX);
                IF @keepCharsForDup > 0
                    SET @candidateName = LEFT(@originalGeneratedShortName, @keepCharsForDup) + @indexStrForDup;
                ELSE 
                    SET @candidateName = @indexStrForDup;
                
                SET @tempShortName = LEFT(@candidateName, @length);

                IF NOT EXISTS (SELECT 1 FROM @ExistingNamesTable WHERE name = @tempShortName)
                BEGIN
                    SET @shortName = @tempShortName;
                    BREAK; 
                END
                
                SET @dupIndex = @dupIndex + 1;
                IF @dupIndex > 10000 
                BEGIN
                    SET @shortName = @tempShortName; 
                    BREAK;
                END
            END
        END
    END

    RETURN LEFT(@shortName, @length);
END
GO

ALTER TABLE sysroGroupFeatures
ADD ExternalId VARCHAR(15) NULL;
GO

BEGIN TRANSACTION;
-- Actualizamos el ExternalId actual de los roles con un nombre generado
DECLARE @FeaturesToUpdate_Initials TABLE (
    RowId INT IDENTITY(1,1) PRIMARY KEY,
    ActualID INT UNIQUE,
    Name NVARCHAR(MAX)
);

INSERT INTO @FeaturesToUpdate_Initials (ActualID, Name)
SELECT ID, Name
FROM sysroGroupFeatures
WHERE Name NOT LIKE '%@@ROBOTICS@@%'
ORDER BY ID;

DECLARE @TotalRows_Initials INT = @@ROWCOUNT;
DECLARE @CurrentRow_Initials INT = 1;
DECLARE @CurrentActualID_Initials INT;
DECLARE @CurrentName_Initials NVARCHAR(MAX);
DECLARE @existingNames_Initials NVARCHAR(MAX);
DECLARE @generatedShortName_Initials NVARCHAR(MAX);

WHILE @CurrentRow_Initials <= @TotalRows_Initials
BEGIN
    SELECT
        @CurrentActualID_Initials = ActualID,
        @CurrentName_Initials = Name
    FROM @FeaturesToUpdate_Initials
    WHERE RowId = @CurrentRow_Initials;

    SELECT @existingNames_Initials = STRING_AGG(ISNULL(ExternalId, ''), ',') WITHIN GROUP (ORDER BY ExternalId)
    FROM sysroGroupFeatures
    WHERE ExternalId IS NOT NULL AND ExternalId <> '';

    SET @generatedShortName_Initials = dbo.GenerateShortName_SQL(@CurrentName_Initials, @existingNames_Initials, 5, 1, 'initials', 'letters');

    UPDATE sysroGroupFeatures
    SET ExternalId = @generatedShortName_Initials
    WHERE ID = @CurrentActualID_Initials;

    SET @CurrentRow_Initials = @CurrentRow_Initials + 1;
END;
COMMIT TRANSACTION;
GO


-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1060' WHERE ID='DBVersion'
GO
