-- Añadimos parámetro para poder habilitar o no el sumario de empleado
INSERT INTO dbo.sysroLiveAdvancedParameters ([ParameterName],[Value]) VALUES ('EmployeeSummaryEnabled','0')
GO


UPDATE dbo.sysroParameters SET Data='407' WHERE ID='DBVersion'
GO
