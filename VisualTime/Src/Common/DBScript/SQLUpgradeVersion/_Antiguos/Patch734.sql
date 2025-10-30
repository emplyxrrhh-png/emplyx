-- No borréis esta línea
UPDATE [dbo].[Channels] SET Deleted = 0, Status = 0 WHERE IsComplaintChannel = 1
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='734' WHERE ID='DBVersion'
GO
