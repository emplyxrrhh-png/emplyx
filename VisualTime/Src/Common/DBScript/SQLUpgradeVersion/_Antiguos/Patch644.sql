-- No borréis esta línea
insert into sysroGUI values ('Portal\Security\AdvancedSecurity', 'AdvancedSecurity', 'Security/AdvancedSecurity.aspx','Security.png',null,null,'Forms\Passports',null,2107,null,'U:Administration.Security=Read',null)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='644' WHERE ID='DBVersion'
GO
