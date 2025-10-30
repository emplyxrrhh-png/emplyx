MERGE INTO sysroLiveAdvancedParameters
USING (VALUES ('MinimumCalendarDataWarning')) AS Source (ParameterName)
ON sysroLiveAdvancedParameters.ParameterName = Source.ParameterName
WHEN MATCHED THEN
    UPDATE SET Value = 10000
WHEN NOT MATCHED THEN
    INSERT (ParameterName, Value)
    VALUES ('MinimumCalendarDataWarning', 10000);
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='799' WHERE ID='DBVersion'
GO
