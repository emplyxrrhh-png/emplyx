update sysroGUI_Actions set RequieredFeatures = null  where RequieredFeatures = 'Feature\ConcertRules' 
GO
update sysroGUI set RequiredFeatures = '!Feature\ONE' where IDPath = 'Portal\Configuration\EmergencyReport'
GO
update sysroGUI set RequiredFeatures = '!Feature\ONE' where IDPath = 'Portal\Configuration\Routes'
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='967' WHERE ID='DBVersion'
GO
