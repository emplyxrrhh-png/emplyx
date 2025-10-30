ALTER TABLE [dbo].[EmployeeStatus] ADD [LocalBeginMandatory] [smalldatetime] NULL
GO


-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='650' WHERE ID='DBVersion'
GO
