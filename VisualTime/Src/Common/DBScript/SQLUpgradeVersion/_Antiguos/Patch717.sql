-- No borréis esta línea

DELETE FROM NotificationMessageParameters WHERE IDNotificationType = 39 and Scenario = 1 and ParameterLanguageKey = 'PassportName'

UPDATE NotificationMessageParameters
	SET Parameter = 1
 WHERE IDNotificationType = 39 and Scenario = 1 and ParameterLanguageKey = 'Password'

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='717' WHERE ID='DBVersion'
GO
