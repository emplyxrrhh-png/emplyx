CREATE PROCEDURE AllocateUserNotificationsToAllowedInterval
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @OldID INT, @NewID INT;

    -- Cursor per recórrer els registres amb CreatorID NULL
    DECLARE cur CURSOR FOR
    SELECT ID FROM Notifications WHERE CreatorID IS NULL and ID < 5000;

    OPEN cur;
    FETCH NEXT FROM cur INTO @OldID;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Trobar el primer enter lliure superior a 5000
        SELECT @NewID = ISNULL(Max(ID) + 1,5000)
        FROM Notifications where CreatorID is null and ID >= 5000; 
        
        -- Actualitzar la taula principal
        UPDATE Notifications
        SET ID = @NewID
        WHERE ID = @OldID;

        -- Actualitzar la taula relacionada
        UPDATE sysroNotificationTasks
        SET IDNotification = @NewID
        WHERE IDNotification = @OldID;

        FETCH NEXT FROM cur INTO @OldID;
    END

    CLOSE cur;
    DEALLOCATE cur;
END;
GO

BEGIN TRANSACTION;
BEGIN TRY
    EXEC AllocateUserNotificationsToAllowedInterval;
    
    IF NOT EXISTS (SELECT 1 FROM Notifications WHERE ID = 4991 AND IDType = 92)
	    INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowMail) 
	    VALUES(4991, 92, 'Aviso nuevo comunicado planificado','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',0)

    IF NOT EXISTS (SELECT 1 FROM Notifications WHERE ID = 4990 AND IDType = 90)
	    INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowMail) 
	    VALUES(4990, 90, 'Aviso nueva conversación creada','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',1)

    IF NOT EXISTS (SELECT 1 FROM Notifications WHERE ID = 4980 AND IDType = 89)
	    INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowMail) 
	    VALUES(4980, 89, 'Nuevo mensaje del empleado en el canal','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',1)

    IF NOT EXISTS (SELECT 1 FROM Notifications WHERE ID = 4920 AND IDType = 83)
	    INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) 
	    VALUES(4920, 83, 'Fichajes antes inicio jornada','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')

    IF NOT EXISTS (SELECT 1 FROM Notifications WHERE ID = 4900 AND IDType = 71)
	    INSERT INTO [dbo].[Notifications] ([ID],[IDType],[Name],[Condition],[Destination],[Schedule],[Activated],[CreatorID],[AllowPortal],[IDPassportDestination],[AllowMail],[ShowOnDesktop],[LastTaskDeleted])
        VALUES (4900,71,'Analitica planificada ejecutada','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>',NULL,1,'SYSTEM',0,0,1,0,NULL)

    IF NOT EXISTS (SELECT 1 FROM Notifications WHERE ID = 4601 AND IDType = 47)
        INSERT INTO [dbo].[Notifications]([ID],[IDType],[Name],[Condition],[Destination],[Schedule],[Activated],[CreatorID],[AllowPortal],[IDPassportDestination],[AllowMail],[ShowOnDesktop],[LastTaskDeleted])
        VALUES(4601,47,'Aviso de valor del saldo por debajo del mínimo','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>',null,0,'SYSTEM',0,0,0,0,null)

    IF NOT EXISTS (SELECT 1 FROM Notifications WHERE ID = 4600 AND IDType = 46) 
        INSERT INTO [dbo].[Notifications]([ID],[IDType],[Name],[Condition],[Destination],[Schedule],[Activated],[CreatorID],[AllowPortal],[IDPassportDestination],[AllowMail],[ShowOnDesktop],[LastTaskDeleted])
        VALUES(4600,46,'Aviso de valor del saldo por encima del máximo','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>',null,0,'SYSTEM',0,0,0,0,null)
    
    IF NOT EXISTS (SELECT 1 FROM Notifications WHERE ID = 4501 AND IDType = 45) 
        INSERT INTO [dbo].[Notifications]([ID],[IDType],[Name],[Condition],[Destination],[Schedule],[Activated],[CreatorID],[AllowPortal],[IDPassportDestination],[AllowMail],[ShowOnDesktop],[LastTaskDeleted])
        VALUES(4501,45,'Modificacion de visitas','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>',null,1,'SYSTEM',0,0,0,0,null)
    
    COMMIT;
END TRY
BEGIN CATCH
    ROLLBACK;
END CATCH;
GO

DROP PROCEDURE AllocateUserNotificationsToAllowedInterval
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1016' WHERE ID='DBVersion'
GO
