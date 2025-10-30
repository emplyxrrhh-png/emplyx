IF EXISTS (SELECT * FROM sysroLiveAdvancedParameters where ParameterName = 'Customization' AND Value = 'TAIF')
BEGIN
 
    INSERT INTO [dbo].[ExportGuides]
               ([ID]
               ,[Name]
               ,[ProfileMask]
               ,[ProfileType]
               ,[Mode]
               ,[ProfileName]
               ,[Destination]
               ,[ExportFileName]
               ,[ExportFileType]
               ,[Separator]
               ,[StartCalculDay]
               ,[StartExecutionHour]
               ,[LastLog]
               ,[NextExecution]
               ,[Field_1]
               ,[Field_2]
               ,[Field_3]
               ,[Field_4]
               ,[Field_5]
               ,[Field_6]
               ,[IntervalMinutes]
               ,[DisplayParameters]
               ,[Scheduler]
               ,[EmployeeFilter]
               ,[RequieredFunctionalities]
               ,[FeatureAliasID]
               ,[ExportFileNameTimeStampFormat]
               ,[WSParameters]
               ,[AutomaticDatePeriod]
               ,[Version]
               ,[Concept]
               ,[Active])
    VALUES
     (20028,N'Exportación Historial Cambio de Categoría',N'',36,1,N'',N'',N'',1,N';',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'0,1,0@0@0',NULL,NULL,N'Employees.DataLink.Custom.CatChangeHist',N'Employees',NULL,NULL,NULL,2,N'IvecoChangeJobHistory',0)

	DECLARE @templateIndex AS INT
    SET @templateIndex = (SELECT MAX(ID) FROM ExportGuidesTemplates)
    INSERT INTO ExportGuidesTemplates values (@templateIndex+1,20028,'Exportación Historial Cambio de Categoría*','',NULL,NULL,NULL)
END
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='888' WHERE ID='DBVersion'
GO
