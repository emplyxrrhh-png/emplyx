--Remember to add the file to the Updates folder 
CREATE PROCEDURE DropNonPrimaryNonUniqueIndexes
    @tableName NVARCHAR(128)
AS
BEGIN
    DECLARE @indexName NVARCHAR(128)
    DECLARE @sql NVARCHAR(MAX)

    DECLARE cursorIndexes CURSOR FOR
    SELECT name
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(@tableName)
    AND is_primary_key = 0 -- Ignora los índices de clave primaria
    AND is_unique_constraint = 0 -- Ignora los índices de restricciones únicas

    OPEN cursorIndexes
    FETCH NEXT FROM cursorIndexes INTO @indexName

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @sql = 'DROP INDEX ' + QUOTENAME(@indexName) + ' ON ' + QUOTENAME(@tableName)
        EXEC sp_executesql @sql

        FETCH NEXT FROM cursorIndexes INTO @indexName
    END

    CLOSE cursorIndexes
    DEALLOCATE cursorIndexes
END
GO

EXEC DropNonPrimaryNonUniqueIndexes @tableName = 'TerminalsSyncTasks';
GO

DROP PROCEDURE IF EXISTS DropNonPrimaryNonUniqueIndexes;
GO


CREATE NONCLUSTERED INDEX [IX_TerminalsSyncTasks_LoadNext_Delete]
ON [dbo].[TerminalsSyncTasks] ([IDTerminal],[DeleteOnConfirm],[TaskDate])
INCLUDE ([Task],[IDEmployee],[IDFinger],[ID],[Parameter1],[Parameter2],[TaskData],[TaskSent],[TaskRetries])
GO