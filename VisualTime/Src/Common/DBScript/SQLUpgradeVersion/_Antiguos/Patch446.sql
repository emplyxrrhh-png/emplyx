
insert into sysroGUI_Actions values('CurrentLogged','Portal\Security\SecurityChart\Supervisors','tbCurrentLoggedUsers','Forms\Passports','U:Administration.Security=Admin','ShowCurrentLoggedUsers()','btnTbCurrent3',0,5)
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='446' WHERE ID='DBVersion'
GO