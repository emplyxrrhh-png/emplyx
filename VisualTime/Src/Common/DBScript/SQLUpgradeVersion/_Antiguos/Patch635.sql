ALTER TABLE [dbo].[Concepts]  ADD [AutoApproveRequestsDR] BIT NULL DEFAULT(0)
GO

UPDATE Concepts Set AutoApproveRequestsDR = 0 where  AutoApproveRequestsDR IS NULL
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='635' WHERE ID='DBVersion'
GO
