update sysroGUI_Actions set AppearsOnPopup=1 where id in(110, 188, 107, 111, 112, 109)
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='465' WHERE ID='DBVersion'
GO
