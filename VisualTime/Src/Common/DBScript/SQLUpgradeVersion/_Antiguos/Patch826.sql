-- No borréis esta línea
ALTER TABLE [dbo].[Zones] ADD IpsRestriction nvarchar(max) NULL
GO	

ALTER TABLE [dbo].[DailySchedule] ALTER COLUMN TimestampEngine datetime
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='826' WHERE ID='DBVersion'
GO
