ALTER TABLE ReportLayouts
	ADD RequiredFunctionalities nvarchar(200) NULL
GO
UPDATE ReportLayouts
	SET RequiredFunctionalities = CONCAT('U:', RequieredFeature, '=Read')
WHERE RequieredFeature IS NOT NULL AND RequieredFeature <> '';
GO
UPDATE ReportLayouts 
SET RequieredFeature = NULL
GO
UPDATE ReportLayouts 
	SET RequieredFeature = 'Forms\Concepts'
WHERE LayoutName = 'porcentaje de absentismo';
GO
-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1012' WHERE ID='DBVersion'
GO
