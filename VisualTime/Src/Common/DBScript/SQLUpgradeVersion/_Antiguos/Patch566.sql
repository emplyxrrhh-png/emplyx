delete from dbo.sysroGUI_Actions where IDPath = 'AccessMonitorV2'

GO

ALTER TABLE Zones
ADD IDZoneType SMALLINT

GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='566' WHERE ID='DBVersion'
GO