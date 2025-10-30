
update sysroGUI set RequiredFeatures = '!Version\CegidHub' where IDPath IN ('Portal\Users\Surveys', 'Portal\Users\OnBoarding'); 
GO


-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='683' WHERE ID='DBVersion'
GO
