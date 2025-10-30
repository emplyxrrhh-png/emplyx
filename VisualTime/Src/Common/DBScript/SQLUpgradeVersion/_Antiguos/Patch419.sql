alter table dbo.sysroUserFields add [ID] [int] IDENTITY(1,1) NOT NULL
GO

UPDATE dbo.sysroParameters SET Data='419' WHERE ID='DBVersion'
GO
