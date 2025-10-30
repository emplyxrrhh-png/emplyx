delete sysroGUI where IDPath like '%Analytics%'
GO
delete sysroGUI where IDPath like '%Portal\Configuration\Documents%'
GO
delete sysroGUI where URL like '%ReportScheduler%'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='616' WHERE ID='DBVersion'
GO
