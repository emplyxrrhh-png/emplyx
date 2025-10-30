ALTER TABLE EXPORTGUIDES
ALTER COLUMN CONCEPT NVARCHAR(30)

GO

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
     (20015,N'MDO Madrid Diaria',N'',21,1,N'',N'',N'',1,N';',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'0,1,0@0@0',NULL,NULL,N'Employees.DataLink.Custom.MDOMDay',N'Employees',NULL,NULL,NULL,2,N'IvecoMDOMadridD',0)
    DECLARE @templateIndex AS INT
    SET @templateIndex = (SELECT MAX(ID) FROM ExportGuidesTemplates)
    INSERT INTO ExportGuidesTemplates values (@templateIndex+1,20015,'MDO Madrid Diaria*','',NULL,NULL,NULL)


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
     (20013,N'Exportación IVECO (MDO Valladolid diaria)',N'',28,1,N'',N'',N'',1,N'',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'0,1,0@0@0',NULL,NULL,N'Employees.DataLink.Custom.MDOVDay',N'Employees',NULL,NULL,NULL,2,N'IvecoMDOValladolidD',0)     
    SET @templateIndex = (SELECT MAX(ID) FROM ExportGuidesTemplates)
    INSERT INTO ExportGuidesTemplates values (@templateIndex+1,20013,'MDO Valladolid diaria*','',NULL,NULL,NULL)
 
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
     (20014,N'Exportación IVECO (MDO Valladolid mensual)',N'',27,1,N'',N'',N'',1,N'',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'0,1,0@0@0',NULL,NULL,N'Employees.DataLink.Custom.MDOVMonth',N'Employees',NULL,NULL,NULL,2,N'IvecoMDOValladolidM',0)     
    SET @templateIndex = (SELECT MAX(ID) FROM ExportGuidesTemplates)
    INSERT INTO ExportGuidesTemplates values (@templateIndex+1,20014,'MDO Valladolid mensual*','',NULL,NULL,NULL)
 
    INSERT INTO [dbo].[ExportGuidesTemplates]
           ([ID]
           ,[IDParentGuide]
           ,[Name]
           ,[Profile])
	 SELECT MAX(ID) + 1,
			20002,
			'Saldos a SAP IF3*',
			'adv_SAPIF3_Daily.xlsx'
	 FROM ExportGuidesTemplates

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
     (20016,N'MDO Madrid Mensual',N'',22,1,N'',N'',N'',1,N';',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'0,1,0@0@0',NULL,NULL,N'Employees.DataLink.Custom.MDOMMonth',N'Employees',NULL,NULL,NULL,2,N'IvecoMDOMadridM',0)
    
    SET @templateIndex = (SELECT MAX(ID) FROM ExportGuidesTemplates)
    INSERT INTO ExportGuidesTemplates values (@templateIndex+1,20016,'MDO Madrid Mensual*','',NULL,NULL,NULL)

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
     (20017,N'MDO Logistic Madrid Diaria',N'',23,1,N'',N'',N'',1,N';',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'0,1,0@0@0',NULL,NULL,N'Employees.DataLink.Custom.LogisticDayM',N'Employees',NULL,NULL,NULL,2,N'IvecoMDOLogisticMadridD',0)
    
    SET @templateIndex = (SELECT MAX(ID) FROM ExportGuidesTemplates)
    INSERT INTO ExportGuidesTemplates values (@templateIndex+1,20017,'MDO Logistic Madrid Diaria*','',NULL,NULL,NULL)

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
     (20018,N'MDO Logistic Madrid Mensual',N'',24,1,N'',N'',N'',1,N';',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'0,1,0@0@0',NULL,NULL,N'Employees.DataLink.Custom.LogisticMonthMW',N'Employees',NULL,NULL,NULL,2,N'IvecoMDOLogisticMadridM',0)

	 SET @templateIndex = (SELECT MAX(ID) FROM ExportGuidesTemplates)
    INSERT INTO ExportGuidesTemplates values (@templateIndex+1,20018,'MDO Logistic Madrid Mensual*','',NULL,NULL,NULL)

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
     (20019,N'MDO Madrid Mensual - Workanalysis',N'',25,1,N'',N'',N'',1,N';',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'0,1,0@0@0',NULL,NULL,N'Employees.DataLink.Custom.MDOMMonthW',N'Employees',NULL,NULL,NULL,2,N'IvecoMDOMadridMWorkanalysis',0)

	 SET @templateIndex = (SELECT MAX(ID) FROM ExportGuidesTemplates)
    INSERT INTO ExportGuidesTemplates values (@templateIndex+1,20019,'MDO Madrid Mensual - Workanalysis*','',NULL,NULL,NULL)

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
     (20020,N'MDO Logistic Mensual - Workanalysis',N'',26,1,N'',N'',N'',1,N';',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'0,1,0@0@0',NULL,NULL,N'Employees.DataLink.Custom.LogisticMonthMW',N'Employees',NULL,NULL,NULL,2,N'IvecoMDOLogisticMWorkanalysis',0)
    
    SET @templateIndex = (SELECT MAX(ID) FROM ExportGuidesTemplates)
    INSERT INTO ExportGuidesTemplates values (@templateIndex+1,20020,'MDO Logistic Mensual - Workanalysis*','',NULL,NULL,NULL)

END
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='879' WHERE ID='DBVersion'
GO
