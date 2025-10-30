delete sysroLiveAdvancedParameters where ParameterName like '%VTLive.AdvancedDatalink%' 
GO

delete sysroGUI where IDPath = 'Portal\Reports\DataLink' 
GO

update sysroGUI set RequiredFeatures = null where IDPath = 'Portal\Reports\DataLinkBusiness'
GO

update sysroGUI set RequiredFeatures = null where IDPath = 'Portal\ShiftManagement\LabAgree'
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='960' WHERE ID='DBVersion'
GO
