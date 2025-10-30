INSERT INTO [dbo].[sysroScheduleRulesTypes]([ID],[Name],[System])
     VALUES (13,'MinMaxExpectedHoursInPeriod',0)
GO

INSERT INTO [dbo].[sysroScheduleRulesTypes]([ID],[Name],[System])
     VALUES (14,'MinMaxShiftsSequence',0)
GO

INSERT INTO [dbo].[sysroScheduleRulesTypes]([ID],[Name],[System])
     VALUES (15,'Custom',0)
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='478' WHERE ID='DBVersion'
GO

