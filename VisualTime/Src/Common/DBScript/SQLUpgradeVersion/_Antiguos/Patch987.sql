IF EXISTS (SELECT ID FROM Notifications where ID = 4991 AND (CreatorID IS NULL OR CreatorID <>'SYSTEM'))
BEGIN 
	DECLARE @NewId INT
	SELECT  @NewId = MAX(ID)+1 FROM Notifications
	UPDATE Notifications SET ID = @NewId WHERE ID = 4991 AND (CreatorID IS NULL OR CreatorID <>'SYSTEM')
	-- Las notificaciones que realmente eran del tipo correcto, y no de Comunicado planificado, las asigno al nuevo ID para que no se vuelvan a generar.
	UPDATE sysroNotificationTasks SET IDNotification = @NewId WHERE IDNotification = 4991 AND CHARINDEX('@', ISNULL(Parameters,'')) = 0

	IF NOT EXISTS (SELECT 1 FROM Notifications WHERE ID = 4991 AND IDType = 92)
	INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowMail) 
	VALUES(4991, 92, 'Aviso nuevo comunicado planificado','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',0)
END
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='987' WHERE ID='DBVersion'
GO
