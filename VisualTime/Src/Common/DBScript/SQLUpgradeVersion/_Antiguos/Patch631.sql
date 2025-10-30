ALTER TABLE [dbo].[Punches] ADD HasPhoto bit
GO
ALTER TABLE [dbo].[Punches] ADD PhotoOnAzure bit
GO
UPDATE [dbo].[Punches] SET HasPhoto = 1 WHERE ID IN (SELECT IDPunch From PunchesCaptures)
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='631' WHERE ID='DBVersion'
GO
