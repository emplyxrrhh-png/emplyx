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
     (20021,N'Plus de Presencia - Madrid',N'adv_Plus',29,1,N'',N'',N'',1,N';',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'0,1,0@0@0',NULL,NULL,N'Employees.DataLink.Custom.PlusPreM',N'Employees',NULL,NULL,NULL,2,N'IvecoPlusPresenciaMadrid',0)

	 	DECLARE @templateIndex AS INT
    SET @templateIndex = (SELECT MAX(ID) FROM ExportGuidesTemplates)
    INSERT INTO ExportGuidesTemplates values (@templateIndex+1,20021,'Plus de Presencia - Madrid*','adv_Plus01.xlsx',NULL,NULL,NULL)
	-------
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
     (20022,N'Plus de Presencia - Valladolid',N'adv_Plus',30,1,N'',N'',N'',1,N';',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'0,1,0@0@0',NULL,NULL,N'Employees.DataLink.Custom.PlusPreV',N'Employees',NULL,NULL,NULL,2,N'IvecoPlusPresenciaValladolid',0)

	 SET @templateIndex = (SELECT MAX(ID) FROM ExportGuidesTemplates)
    INSERT INTO ExportGuidesTemplates values (@templateIndex+1,20022,'Plus de Presencia - Valladolid*','adv_Plus02.xlsx',NULL,NULL,NULL)
 
END
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='880' WHERE ID='DBVersion'
GO
