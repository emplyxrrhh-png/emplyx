ALTER TABLE sysroReaderTemplates
ADD Partners bit

GO

UPDATE sysroReaderTemplates SET Partners = 1 WHERE Type= 'masterASP'

GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='801' WHERE ID='DBVersion'
GO
