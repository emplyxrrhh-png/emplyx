update sysroGUI set RequiredFeatures = '!Feature\CegidHub' where IDPath IN ('Portal\Users\Surveys', 'Portal\Users\OnBoarding', 'Portal\Communiques');
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='686' WHERE ID='DBVersion'
GO
