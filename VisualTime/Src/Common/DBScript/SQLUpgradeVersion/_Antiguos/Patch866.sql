ALTER TABLE [dbo].[sysroPassports_Data] ADD [Method] int NULL 
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='866' WHERE ID='DBVersion'

GO
