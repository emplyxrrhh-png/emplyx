ALTER TABLE dbo.ExportGuides ADD
	EmployeeFilter nvarchar(MAX) NULL
GO

INSERT INTO [dbo].[sysroLiveAdvancedParameters]([ParameterName],[Value])
     VALUES ('ADFSEnabled','0')
GO

INSERT INTO [dbo].[sysroLiveAdvancedParameters]([ParameterName],[Value])
     VALUES ('ADFSStandardLoginEnabled','1')
GO

INSERT INTO [dbo].[sysroLiveAdvancedParameters]([ParameterName],[Value])
     VALUES ('CollectorPrepareFiles_Period','3')
GO

INSERT INTO [dbo].[sysroLiveAdvancedParameters]([ParameterName],[Value])
     VALUES ('ConnectorExporterFTP_Period','6')
GO


UPDATE sysroParameters SET Data='352' WHERE ID='DBVersion'
GO


