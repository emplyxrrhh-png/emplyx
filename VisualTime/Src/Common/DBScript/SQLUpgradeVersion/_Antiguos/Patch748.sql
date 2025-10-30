-- No borréis esta línea
DELETE FROM sysroPassports_Groups where IDGroup not in(select id from groups)
GO
DELETE FROM sysroPermissionsOverGroups  where employeegroupid not in(select id from groups)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='748' WHERE ID='DBVersion'
GO
