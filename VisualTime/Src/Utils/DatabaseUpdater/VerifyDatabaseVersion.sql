-- =============================================
-- Script para verificar la versión de la base de datos
-- VisualTime Database Version Checker
-- =============================================

USE [VisualTime]
GO

PRINT '========================================='
PRINT 'Verificación de Base de Datos VisualTime'
PRINT '========================================='
PRINT ''

-- Verificar versión actual de la base de datos
PRINT 'VERSIÓN ACTUAL DE LA BASE DE DATOS:'
PRINT '-----------------------------------------'
SELECT 
    ID as 'Parámetro',
    Data as 'Valor'
FROM sysroParameters 
WHERE ID = 'DBVERSION'
PRINT ''

-- Verificar información general de la base de datos
PRINT 'INFORMACIÓN GENERAL:'
PRINT '-----------------------------------------'
SELECT 
    ID as 'Parámetro',
    CASE 
        WHEN LEN(Data) > 100 THEN LEFT(Data, 100) + '...'
        ELSE Data
    END as 'Valor'
FROM sysroParameters 
WHERE ID IN ('DBUniqueID', 'DBVERSION', 'ACTIVE')
PRINT ''

-- Verificar última ejecución de triggers
PRINT 'ÚLTIMA EJECUCIÓN DE TRIGGERS:'
PRINT '-----------------------------------------'
DECLARE @xmlData XML
SELECT @xmlData = CAST(Data AS XML)
FROM sysroParameters
WHERE ID = 'SYSFLAGS'

SELECT 
    item.value('(@key)[1]', 'VARCHAR(50)') AS 'Trigger',
    item.value('(text())[1]', 'VARCHAR(50)') AS 'Última Ejecución'
FROM @xmlData.nodes('//Item') AS T(item)
WHERE item.value('(@key)[1]', 'VARCHAR(50)') LIKE 'LastTriggered%'
PRINT ''

-- Verificar opciones de base de datos
PRINT 'OPCIONES DE LA BASE DE DATOS:'
PRINT '-----------------------------------------'
SELECT
    name as 'Propiedad',
    CASE 
        WHEN collation_name IS NOT NULL THEN collation_name
        WHEN is_auto_close_on = 1 THEN 'Sí'
        WHEN is_auto_shrink_on = 1 THEN 'Sí'
        WHEN is_auto_create_stats_on = 1 THEN 'Sí'
        WHEN is_auto_update_stats_on = 1 THEN 'Sí'
        ELSE 'No'
    END as 'Valor'
FROM sys.databases
WHERE name = 'VisualTime'
PRINT ''

PRINT '========================================='
PRINT 'Verificación completada'
PRINT '========================================='
