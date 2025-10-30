
update sysroGUI set RequiredFeatures = null where IDPath = 'Portal\Reports\DataLinkBusiness'
GO

update sysroGUI set RequiredFeatures = null where IDPath = 'Portal\ShiftManagement\LabAgree'
GO


update sysroGUI set RequiredFeatures = null where IDPath = 'Portal\Configuration\Notifications'
GO

update sysroGUI set RequiredFeatures = null where IDPath = 'Portal\Security\Audit'
GO
update sysroGUI set RequiredFeatures = null where IDPath = 'Portal\Configuration\UserFields'
GO
update sysroGUI set RequiredFeatures = null where IDPath = 'Portal\Configuration\Options'
GO

update sysroGUI set RequiredFeatures = null where IDPath = 'Portal\Company\SecurityFunctions'
GO
update sysroGUI set RequiredFeatures = null where IDPath = 'Portal\Company\AdvSupervisors'
GO
update sysroGUI set RequiredFeatures = null where IDPath = 'Portal\Security\AdvancedSecurity'
GO
update sysroGUI set RequiredFeatures = null where IDPath = 'Portal\Security\Aministration'
GO
update sysroGUI set RequiredFeatures = null where IDPath = 'Portal\Security\Diagnostics'
GO
update sysroGUI set RequiredFeatures = null where IDPath = 'Portal\Security\License'
GO
update sysroGUI set RequiredFeatures = null where IDPath = 'Portal\Security\LockDB'
GO
update sysroGUI set RequiredFeatures = null where IDPath = 'Portal\Configuration\SecurityOptions'
GO

update sysroGUI_Actions set RequieredFeatures = null  where RequieredFeatures = 'forms\passports' 
GO

update sysroGUI_Actions set RequieredFeatures = null  where RequieredFeatures = 'Process\ReportServer' 
GO

update sysroGUI_Actions set RequieredFeatures = null  where RequieredFeatures = 'Process\Notifier' 
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='962' WHERE ID='DBVersion'
GO
