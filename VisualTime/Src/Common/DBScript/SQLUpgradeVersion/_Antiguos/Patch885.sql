IF EXISTS (SELECT * FROM sysroLiveAdvancedParameters where ParameterName = 'Customization' AND Value = 'TAIF')
BEGIN
UPDATE ExportGuides SET IsCustom = 1 WHERE ID = 20025
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
     (20026,N'Workanalysis - Horas trabajadas diarias por sección*',N'',32,1,N'',N'',N'',1,N'',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'0,1,0@0@0',NULL,NULL,N'Employees.DataLink.Custom.HTDaySec',N'Employees',NULL,NULL,NULL,2,N'IvecoWorkanalysisHoursSection',0,1)     
    DECLARE @templateIndex AS INT
    SET @templateIndex = (SELECT MAX(ID) FROM ExportGuidesTemplates)
    INSERT INTO ExportGuidesTemplates values (@templateIndex+1,20026,'Workanalysis - Horas trabajadas diarias por sección*','',NULL,NULL,NULL)

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
     (20027,N'MDO Conguallo*',N'',34,1,N'',N'',N'',1,N'',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'0,1,0@0@0',NULL,NULL,N'Employees.DataLink.Custom.Conguallo',N'Employees',NULL,NULL,NULL,2,N'IvecoConguallo',0,1)     
    SET @templateIndex = (SELECT MAX(ID) FROM ExportGuidesTemplates)
    INSERT INTO ExportGuidesTemplates values (@templateIndex+1,20027,'MDO Conguallo*','',NULL,NULL,NULL)
END
GO

IF EXISTS (SELECT * FROM sysroLiveAdvancedParameters WHERE ParameterName = 'Customization' AND Value = 'TAIF')
BEGIN
    EXEC('
    CREATE FUNCTION [dbo].[GetUOMax] (@FullGroupName nvarchar(max))
    RETURNS nvarchar(max)
    AS
    BEGIN
        DECLARE @UO nvarchar(max)
        SET @UO = @FullGroupName
        
        DECLARE @Pos int, @CurrentSep int
        SET @CurrentSep = 0
        SELECT @Pos = CHARINDEX(''\'', @UO)
        
        WHILE (@Pos > 0) AND @CurrentSep <= 3 
        BEGIN
            SET @UO = SUBSTRING(@UO, @Pos + 1, LEN(@UO) - @Pos)
            SET @Pos = CHARINDEX(''\'', @UO)
            SET @CurrentSep = @CurrentSep + 1
        END
        
        IF @CurrentSep <> 4  
            SET @UO = ''''
        ELSE
        BEGIN
            SET @Pos = CHARINDEX(''\'', @UO)
            IF @Pos > 0
                SET @UO = SUBSTRING(@UO, 1, @Pos - 1)
            ELSE
                SET @UO = @UO 
        END
        
        RETURN @UO
    END
    ')
END
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='885' WHERE ID='DBVersion'
GO
