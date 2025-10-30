-- No borréis esta línea
ALTER TABLE ExportGuides ADD IsCustom BIT Default 0
GO
ALTER TABLE ImportGuides ADD IsCustom BIT Default 0
GO

UPDATE ExportGuides SET IsCustom = 0
GO
UPDATE ExportGuides SET IsCustom = 1 WHERE Concept = 'ZCustom'
GO

UPDATE ImportGuides SET IsCustom = 0
GO
UPDATE ImportGuides SET IsCustom = 1 WHERE Concept = 'ZCustom'
GO

IF EXISTS (SELECT * FROM sysroLiveAdvancedParameters where ParameterName = 'Customization' AND Value = 'TAIF')
BEGIN
	UPDATE ExportGuides SET Name = 'MDO Valladolid Diaria*', IsCustom = 1 WHERE ID = 20013
	UPDATE ExportGuides SET Name = 'MDO Valladolid Mensual*', IsCustom = 1 WHERE ID = 20014
	UPDATE ExportGuides SET Name = 'MDO Madrid Diaria*', IsCustom = 1 WHERE ID = 20015
	UPDATE ExportGuides SET Name = 'MDO Madrid Mensual*', IsCustom = 1 WHERE ID = 20016
	UPDATE ExportGuides SET Name = 'MDO Logistic Madrid Diaria*', IsCustom = 1 WHERE ID = 20017
	UPDATE ExportGuides SET Name = 'MDO Logistic Madrid Mensual*', IsCustom = 1 WHERE ID = 20018
	UPDATE ExportGuides SET Name = 'MDO Madrid Mensual - Workanalysis*', IsCustom = 1 WHERE ID = 20019
	UPDATE ExportGuides SET Name = 'MDO Logistic Mensual - Workanalysis*', IsCustom = 1 WHERE ID = 20020
	UPDATE ExportGuides SET Name = 'Plus de Presencia - Madrid*', IsCustom = 1 WHERE ID = 20021
	UPDATE ExportGuides SET Name = 'Plus de Presencia - Valladolid*', IsCustom = 1 WHERE ID = 20022
END 
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='881' WHERE ID='DBVersion'
GO
