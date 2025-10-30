-- Indices para el asistente de justificacion masiva de incidencias
CREATE NONCLUSTERED INDEX [DailyCauses_Incidences]
ON [dbo].[DailyCauses] ([Date])
INCLUDE ([IDEmployee],[IDRelatedIncidence],[IDCause],[Value],[Manual],[CauseUser],[CauseUserType],[AccrualsRules],[IsNotReliable])
GO

CREATE NONCLUSTERED INDEX [DailyCauses_Incidences_2]
ON [dbo].[DailyIncidences] ([IDEmployee],[Date],[IDType])
INCLUDE ([ID],[IDZone],[Value],[BeginTime],[EndTime])
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='342' WHERE ID='DBVersion'
GO


