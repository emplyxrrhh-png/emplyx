IF EXISTS (SELECT * FROM sysroLiveAdvancedParameters where ParameterName = 'Customization' AND Value = 'TAIF')
BEGIN
    UPDATE [dbo].[ExportGuides] SET [Name] = N'Historial Cambio de Categoría*', IsCustom = 1 WHERE [ID] = 20028
    UPDATE [dbo].[ExportGuidesTemplates] SET [Name] = N'Historial Cambio de Categoría*' WHERE [IDParentGuide] = 20028

    UPDATE [dbo].[ExportGuides] SET [Name] = N'Historial de Puestos*', IsCustom = 1 WHERE [ID] = 20025
    UPDATE [dbo].[ExportGuidesTemplates] SET [Name] = N'Historial de Puestos*' WHERE [IDParentGuide] = 20025

END
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='901' WHERE ID='DBVersion'
GO
