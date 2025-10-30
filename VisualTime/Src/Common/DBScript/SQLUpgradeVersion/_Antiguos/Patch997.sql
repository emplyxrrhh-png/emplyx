alter table sysroPassports_Data add KeyValidated int
GO
update sysroPassports_Data set KeyValidated = 1
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='997' WHERE ID='DBVersion'
GO
