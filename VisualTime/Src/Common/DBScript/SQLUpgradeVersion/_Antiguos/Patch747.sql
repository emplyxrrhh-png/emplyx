-- No borréis esta línea
IF EXISTS (SELECT * FROM sysroParameters WHERE ID ='CCode' AND CONVERT(VARCHAR,Data) = 'cura2728')
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
     (20011,N'Exportación Productividad Monchos',N'',19,1,N'',N'',N'',1,N'',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'0,1,0@0@0',NULL,NULL,N'Employees.DataLink.Exports.Punches',N'Employees',NULL,NULL,NULL,2,N'ZCustom',0)
    DECLARE @templateIndex AS INT
    SET @templateIndex = (SELECT MAX(ID) FROM ExportGuidesTemplates)
    INSERT INTO ExportGuidesTemplates values (@templateIndex+1,20011,'attendance_productivity','',NULL,NULL,NULL)
END 
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='747' WHERE ID='DBVersion'
GO
