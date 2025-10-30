DELETE sysroDatabaseTexts

-- Justificaciones
-- SELECT Name, Description, * FROM Causes 
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','causes','.',LOWER(LanguageTag),'.','name','.','rotext','=',Name) FROM Causes WHERE Name <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','causes','.',LOWER(LanguageTag),'.','description','.','rotext','=', Description) FROM Causes WHERE CONVERT(NVARCHAR, Description) <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO

-- Saldos
-- select Name, Description, * from Concepts
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','concepts','.',LOWER(LanguageTag),'.','name','.','rotext','=',Name) FROM Concepts  WHERE Name <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','concepts','.',LOWER(LanguageTag),'.','description','.','rotext','=',Description) FROM Concepts WHERE CONVERT(NVARCHAR, Description) <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO

-- Grupos de saldos
-- select Name, * from sysroReportGroups
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','sysroreportgroups','.',LOWER(LanguageTag),'.','name','.','rotext','=',Name) FROM sysroReportGroups  WHERE Name <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO

-- Tipos de acumulados
-- select Name, * FROM sysroConceptTypes
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','sysroconcepttypes','.',LOWER(LanguageTag),'.','name','.','rotext','=',Name) FROM sysroConceptTypes  WHERE Name <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO

-- Horarios
-- select Name, Description, * from Shifts
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','shifts','.',LOWER(LanguageTag),'.','name','.','rotext','=',Name) FROM Shifts WHERE Name <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','shifts','.',LOWER(LanguageTag),'.','description','.','rotext','=',Description) FROM Shifts WHERE CONVERT(NVARCHAR, Description) <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO

-- Zonas horarias
-- select Name, * FROM TimeZones
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','timezones','.',LOWER(LanguageTag),'.','name','.','rotext','=',Name) FROM TimeZones WHERE Name <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO

-- Departamentos
-- select Name, * FROM Groups
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','groups','.',LOWER(LanguageTag),'.','name','.','rotext','=',Name) FROM Groups WHERE Name <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO

-- Consultas
-- select Name, Description, * from sysroQueries
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','sysroqueries','.',LOWER(LanguageTag),'.','name','.','rotext','=',Name) FROM sysroQueries WHERE Name <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','sysroqueries','.',LOWER(LanguageTag),'.','description','.','rotext','=',Description) FROM sysroQueries WHERE CONVERT(NVARCHAR, Description) <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO

-- Campos de la ficha
-- select FieldName, ListValues, Category, * from sysroUserFields
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','sysrouserfields','.',LOWER(LanguageTag),'.','fieldname','.','rotext','=',FieldName) FROM sysroUserFields WHERE FieldName <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','sysrouserfields','.',LOWER(LanguageTag),'.','listvalues','.','rotext','=',ListValues) FROM sysroUserFields WHERE CONVERT(NVARCHAR, ListValues) <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','sysrouserfields','.',LOWER(LanguageTag),'.','category','.','rotext','=',Category) FROM sysroUserFields WHERE CONVERT(NVARCHAR, Category) <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO

-- Convenios
-- select Name, * from LabAgree
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','labagree','.',LOWER(LanguageTag),'.','name','.','rotext','=',Name) FROM LabAgree WHERE Name <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO

-- Grupos de Accesos
-- select Name, * from AccessGroups
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','accessgroups','.',LOWER(LanguageTag),'.','name','.','rotext','=',Name) FROM AccessGroups WHERE Name <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO

-- Periodos
-- select Name, * from AccessPeriods
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','accessperiods','.',LOWER(LanguageTag),'.','name','.','rotext','=',Name) FROM AccessPeriods WHERE Name <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO

-- Zonas
-- SELECT Name, Description, * FROM Zones
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','zones','.',LOWER(LanguageTag),'.','name','.','rotext','=',Name) FROM Zones WHERE Name <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','zones','.',LOWER(LanguageTag),'.','description','.','rotext','=',Description) FROM Zones WHERE CONVERT(NVARCHAR, Description) <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO

-- Valores iniciales
-- select Name, StartValueBase, * from StartupValues
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','startupvalues','.',LOWER(LanguageTag),'.','name','.','rotext','=',Name) FROM StartupValues WHERE Name <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','startupvalues','.',LOWER(LanguageTag),'.','startvaluebase','.','rotext','=',StartValueBase) FROM StartupValues WHERE CONVERT(NVARCHAR, StartValueBase) <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO

-- Límites de justificaciones
-- select Name, * from CauseLimitValues
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','causelimitvalues','.',LOWER(LanguageTag),'.','name','.','rotext','=',Name) FROM CauseLimitValues WHERE Name <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO


-- Reglas de Arrastre
-- select Name, Description, * From AccrualsRules
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','accrualsrules','.',LOWER(LanguageTag),'.','name','.','rotext','=',Name) FROM AccrualsRules WHERE Name <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','accrualsrules','.',LOWER(LanguageTag),'.','description','.','rotext','=',Description) FROM AccrualsRules WHERE CONVERT(NVARCHAR, Description) <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO

--Vacaciones
-- select Name, * From RequestsRules
INSERT INTO sysroDatabaseTexts SELECT CONCAT('RES_database_one','.','requestsrules','.',LOWER(LanguageTag),'.','name','.','rotext','=',Name) FROM RequestsRules WHERE Name <> '' AND LanguageTag <> '' AND LanguageTag IS NOT NULL
GO

SELECT * FROM sysroDatabaseTexts ORDER BY [Key] ASC

---- SCRIPT LIMPIEZA BBD PARA BBD INICIAL
--update sysroParameters set Data = '' where ID = 'CCode'
--GO
--UPDATE sysroParameters set Data = '0' where ID = 'DXVERSION'
--GO
--DELETE AuditLive
--GO
--DELETE sysroRequestLevel
--GO
--UPDATE sysroPassports_AuthenticationMethods SET Timestamp = NULL, LastDateInvalidAccessAttempted = NULL, InvalidAccessAttemps = 0, Password = '' WHERE Credential = 'Admin'
--GO
--DELETE GeniusViews where IdPassport NOT IN (-1, 0)
--GO
--DELETE sysroUserTasks
--GO
--DELETE AccrualsRules WHERE IdAccrualsRule NOT IN (SELECT IdAccrualsRules FROM LabAgreeAccrualsRules)
--GO
--DELETE sysroDatabaseTexts
--GO