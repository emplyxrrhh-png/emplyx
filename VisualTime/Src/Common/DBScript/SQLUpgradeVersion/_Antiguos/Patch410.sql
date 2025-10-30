IF EXISTS (SELECT 1 FROM dbo.sysroLiveAdvancedParameters WHERE ParameterName = 'ShouldNotifyExitBeforeTime')
	UPDATE dbo.sysroLiveAdvancedParameters SET Value = 0 WHERE ParameterName = 'ShouldNotifyExitBeforeTime'
ELSE
	INSERT INTO dbo.sysroLiveAdvancedParameters VALUES ('ShouldNotifyExitBeforeTime',0)
GO


UPDATE dbo.sysroParameters SET Data='410' WHERE ID='DBVersion'
GO
