IF EXISTS (SELECT * FROM sysroLiveAdvancedParameters where ParameterName = 'Customization' AND Value = 'TAIF')
BEGIN
UPDATE ExportGuides SET Name = 'Workanalysis - Horas trabajadas diarias por empleado*', IsCustom = 1 WHERE ID = 20023
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
               ,[Active]
               ,[IsCustom])
    VALUES
     (20024,N'Workanalysis - Report Mensual*',N'',31,1,N'',N'',N'',1,N'',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'0,1,0@0@0',NULL,NULL,N'Employees.DataLink.Custom.MonthReport',N'Employees',NULL,NULL,NULL,2,N'IvecoWorkanalysisReportM',0,1)     
    DECLARE @templateIndex AS INT
    SET @templateIndex = (SELECT MAX(ID) FROM ExportGuidesTemplates)
    INSERT INTO ExportGuidesTemplates values (@templateIndex+1,20024,'Workanalysis - Report Mensual*','',NULL,NULL,NULL)

update ExportGuides set Field_4 = '1960,2184,2276,12016,1038,1595,11281,12063,12998,13008,1217,1655,1972,2371,6397,6668,1121,1207,1446,2171,5632,5654,11229,12892,2532,3164,3188,12550,979,'
where id = 20016

END
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='883' WHERE ID='DBVersion'
GO
