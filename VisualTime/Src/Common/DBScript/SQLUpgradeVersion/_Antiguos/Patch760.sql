update ReportLayouts set RequieredFeature = NULL where IdPassport = NULL
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='760' WHERE ID='DBVersion'
GO
