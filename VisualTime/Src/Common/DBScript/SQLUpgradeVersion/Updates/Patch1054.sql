ALTER TABLE sysroPassports
ADD ShowUpdatePopup BIT NOT NULL DEFAULT 0;
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1054' WHERE ID='DBVersion'
GO
