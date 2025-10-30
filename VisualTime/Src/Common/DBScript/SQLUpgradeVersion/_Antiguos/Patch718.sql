DELETE FROM NotificationMessageParameters WHERE IDNotificationType = 55 and Scenario = 1 and ParameterLanguageKey = 'PassportName'

UPDATE NotificationMessageParameters
	SET Parameter = 1
 WHERE IDNotificationType = 55 and Scenario = 1 and ParameterLanguageKey = 'RecoverCode'

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='718' WHERE ID='DBVersion'
GO
