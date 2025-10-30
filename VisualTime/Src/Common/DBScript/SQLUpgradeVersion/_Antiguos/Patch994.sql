UPDATE sysroLiveAdvancedParameters SET Value = '3' WHERE ParameterName = 'ReportsPersistOnSystem' AND CONVERT(INT, Value) = 212
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='994' WHERE ID='DBVersion'
GO
