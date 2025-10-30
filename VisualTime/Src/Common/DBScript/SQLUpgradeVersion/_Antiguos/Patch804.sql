IF NOT EXISTS (SELECT 1 FROM sysroParameters WHERE ID = 'RUNENGINE')
BEGIN
	INSERT INTO dbo.sysroParameters(ID,Data) VALUES('RUNENGINE','0')
END 
-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='804' WHERE ID='DBVersion'
GO
