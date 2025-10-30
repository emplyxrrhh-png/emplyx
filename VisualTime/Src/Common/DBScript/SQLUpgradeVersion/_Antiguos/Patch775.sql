update ReportLayouts set RequieredFeature = NULL where IdPassport is NULL
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='775' WHERE ID='DBVersion'
GO
