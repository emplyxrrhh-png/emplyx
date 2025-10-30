IF EXISTS (SELECT * FROM sysroLiveAdvancedParameters where ParameterName = 'Customization' AND Value = 'TAIF')
BEGIN

UPDATE ExportGuides SET Concept = 'IvecoCategoryChange' WHERE ID = 20028

END

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='896' WHERE ID='DBVersion'
GO
