IF (select count(*) from sysroGUI where Edition = 'Starter') > 0 
BEGIN

update dbo.sysroGUI set Edition = 'Starter' where IDPath = 'Portal\Communications'

END

GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='731' WHERE ID='DBVersion'
GO
