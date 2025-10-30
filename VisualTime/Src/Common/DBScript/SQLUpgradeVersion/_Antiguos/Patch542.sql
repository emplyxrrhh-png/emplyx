alter table dbo.startupvalues alter column scalingvalues nvarchar(500)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='542' WHERE ID='DBVersion'
GO