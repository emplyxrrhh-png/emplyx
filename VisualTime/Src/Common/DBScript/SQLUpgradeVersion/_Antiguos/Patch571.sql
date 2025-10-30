UPDATE sysroGUI_Actions SET AppearsOnPopup = 0 WHERE IDPath = 'CurrentLogged' AND IDGUIPath = 'Portal\Security\Passports\Supervisors'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='571' WHERE ID='DBVersion'
GO
