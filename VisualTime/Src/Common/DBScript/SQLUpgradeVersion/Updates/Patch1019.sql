-- No borréis esta línea
alter table sysroPassports_CacheData add Type int not null default 0
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1019' WHERE ID='DBVersion'
GO
