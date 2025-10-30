ALTER TABLE Zones
ADD  IdZoneSupervisor INT NULL

GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO
UPDATE sysroParameters SET Data='589' WHERE ID='DBVersion'
GO
