delete from ReportLayouts where LayoutXMLBinary is null
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='573' WHERE ID='DBVersion'
GO
