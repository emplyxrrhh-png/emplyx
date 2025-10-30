-- Nuevo campo de idiomas a nivel de usuario
-- alter table sysrousers add language nvarchar(3)
-- GO

-- Nueva opción en el menú para modificar el idioma
-- insert into sysrogui (IDPath, LanguageReference, URL, IconURL,Type,Parameters,RequiredFeatures,SecurityFlags,Priority,AllowedSecurity)
-- values('Menu\Tools\ChangeLanguage','ChangeLanguage','fn:\\ShowLanguage',NULL,NULL,NULL,NULL,'3111111111111111111111111111111111111111
-- ',586,'NR')
-- GO

-- Adaptar el tamaño de los permisos en las tabla Groups y sysrogui
update groups set securityflags = left(securityflags + '1111111111111111111111111111111111111111',40)
GO
update sysrogui set securityflags = left(securityflags + '1111111111111111111111111111111111111111',40)
GO

-- Añadimos la columna StartupValue de la DailyAccruals a la clave principal para que el mismo día y para el mismo empleado y acumulado
-- podamos tener un resultado de una regla de acumulados (CarryOver=1) y un valor incial (CarryOver=1 y StartupValue=1)
BEGIN TRANSACTION
GO
ALTER TABLE dbo.DailyAccruals
            DROP CONSTRAINT PK_DailyTotals
GO
ALTER TABLE dbo.DailyAccruals ADD CONSTRAINT
            PK_DailyTotals PRIMARY KEY NONCLUSTERED 
            (
            IDEmployee,
            Date,
            IDConcept,
            CarryOver,
            StartupValue
            ) ON [PRIMARY]
GO
COMMIT
GO


-- Añadimos a la tabla EmployeeAccrualsRules la columna Position, para poder ordenar las reglas de acumulados de un empleado
ALTER TABLE dbo.EmployeeAccrualsRules ADD
            Position int NULL
GO


-- Añadimos la columna EXECUTEFROMLASTEXECDAY a la tabla ACCRUALSRULES para poder distinguir si la regla de acumulado
-- se comporta al modo antiguo (acumulando desde la última ejecución de la regla) o al nuevo (se acumula desde el inicio de período.
IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ACCRUALSRULES' AND COLUMN_NAME = 'ExecuteFromLastExecDay')
BEGIN 
     ALTER TABLE ACCRUALSRULES ADD ExecuteFromLastExecDay bit NULL 
END
GO
UPDATE ACCRUALSRULES SET ExecuteFromLastExecDay = 1 WHERE ExecuteFromLastExecDay IS NULL
GO

--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='189' WHERE ID='DBVersion'
GO
