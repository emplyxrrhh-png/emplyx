
alter table sysroPassports_Data alter column SecurityToken nvarchar(256) null
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='626' WHERE ID='DBVersion'
GO
