-- No borréis esta línea
ALTER TABLE Terminals ADD Enabled BIT NOT NULL DEFAULT 1
GO
UPDATE Terminals SET Enabled = 1
GO

delete from sysroGUI_Actions where RequieredFeatures like 'Forms\Activities'
GO
 
update sysroGUI set RequiredFeatures = null where IDPath = 'LivePortal\CDP\CDPMoves'
GO
 
delete from sysroGUI where IDPath like 'LivePortal%'
GO
 
update sysroGUI set RequiredFeatures = 'Feature\Channels;|Feature\Complaints' where IDPath = 'Portal\Communications\Channels'
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='969' WHERE ID='DBVersion'
GO
