IF EXISTS (SELECT 1 FROM sysroliveAdvancedParameters where ParameterName = 'VTLive.GeniusAnalytics' and (Value = 'false' or Value = '0'))
BEGIN
	DELETE from Genius_Views_Scheduler
	DELETE from sysroLiveTasks where action like 'ANALYTICSTASK'

	MERGE INTO sysroLiveAdvancedParameters AS target
		USING (VALUES ('VTLive.GeniusAnalytics','true')) AS source (ParameterName,Value)
		ON target.ParameterName = source.ParameterName 
		WHEN MATCHED THEN
			UPDATE SET Value = source.Value
		WHEN NOT MATCHED THEN
			INSERT (ParameterName, Value)
			VALUES (source.ParameterName, source.Value);

END
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1003' WHERE ID='DBVersion'
GO
