DELETE FROM sysroPassports_AuthorizedAdress
GO

UPDATE sysroLiveAdvancedParameters SET Value = '21' WHERE ParameterName='VTPortalApiVersion'
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='944' WHERE ID='DBVersion'
GO
