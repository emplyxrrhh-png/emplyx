delete from ReportLayouts where LayoutXMLBinary is null;
GO

DELETE FROM ReportExecutionsLastParameters WHERE ReportId = (select id from ReportLayouts where LayoutName LIKE 'Saldos día a día')
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='956' WHERE ID='DBVersion'
GO
