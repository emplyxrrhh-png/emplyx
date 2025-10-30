ALTER TABLE [dbo].[Requests] ADD CRC NVARCHAR(32) NULL
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='620' WHERE ID='DBVersion'
GO
