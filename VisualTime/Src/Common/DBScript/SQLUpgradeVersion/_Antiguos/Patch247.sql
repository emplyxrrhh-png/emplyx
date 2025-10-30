UPDATE [dbo].[sysroFeatures]
SET [Alias] = 'Calendar.Punches.Requests'
WHERE [Alias] = 'Calendar.Punches.Punches.Requests'
GO

UPDATE [dbo].[sysroFeatures]
SET [Alias] = 'Calendar.Punches.Requests.Forgotten'
WHERE [Alias] = 'Calendar.Punches.Punches.Requests.Forgotten'
GO

UPDATE [dbo].[sysroFeatures]
SET [Alias] = 'Calendar.Punches.Requests.Justify'
WHERE [Alias] = 'Calendar.Punches.Punches.Requests.Justify'
GO

UPDATE [dbo].[sysroFeatures]
SET [Alias] = 'Calendar.Punches.Requests.ExternalParts'
WHERE [Alias] = 'Calendar.Punches.Punches.Requests.ExternalParts'
GO

/* ***************************************************************************************************************************** */
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='247' WHERE ID='DBVersion'
GO
