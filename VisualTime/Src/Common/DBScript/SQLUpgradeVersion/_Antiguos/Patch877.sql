IF NOT EXISTS (SELECT 1 FROM sysroNotificationTypes WHERE ID = 92)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem) VALUES(92,'Advice new scheduled communique',null, -1,'','', 1)	
GO


IF NOT EXISTS (SELECT 1 FROM Notifications WHERE ID = 4991 AND IDType = 92)
	INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowMail) 
	VALUES(4991, 92, 'Aviso nuevo comunicado planificado','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',0)
GO


-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='877' WHERE ID='DBVersion'

GO
