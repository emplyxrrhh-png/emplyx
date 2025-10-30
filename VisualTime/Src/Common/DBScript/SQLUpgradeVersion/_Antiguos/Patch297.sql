ALTER TABLE [dbo].[Notifications]
ADD 
	[AllowPortal] [bit] NULL DEFAULT(0),
	[IDPassportDestination] [int] NULL DEFAULT(0)
GO


UPDATE dbo.Notifications SET IDPassportDestination=0 WHERE IDPassportDestination Is null
GO

INSERT INTO dbo.sysroNotificationTypes VALUES(19,'Day with unmatched time record',null, 120)
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(1001, 19, 'Dias con fichajes impares','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO


INSERT INTO dbo.sysroNotificationTypes VALUES(20,'Day with unreliable time record',null, 120)
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(1002, 20, 'Dias con fichajes no fiables','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO


INSERT INTO dbo.sysroNotificationTypes VALUES(21,'Non justified incident',null, 120)
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(1003, 21, 'Dias con incidencias no justificadas','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO


INSERT INTO dbo.sysroNotificationTypes VALUES(22,'IDCard not assigned to employee',null, 1)
GO


INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(1004, 22, 'Tarjeta no asignada a ningún empleado','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO


INSERT INTO dbo.sysroNotificationTypes VALUES(23,'Task close to finish',null, 120)
GO

INSERT INTO dbo.sysroNotificationTypes VALUES(24,'Task close to start',null, 120)
GO

INSERT INTO dbo.sysroNotificationTypes VALUES(25,'Task exceeding planned time',null, 120)
GO

INSERT INTO dbo.sysroNotificationTypes VALUES(26,'Task exceeding finished date',null, 120)
GO

INSERT INTO dbo.sysroNotificationTypes VALUES(27,'Request Holidays',null, 120)
GO

INSERT INTO dbo.sysroNotificationTypes VALUES(28,'Request Shift Change',null, 120)
GO


INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(1005, 1, 'Inicio de contrato','<?xml version="1.0"?><roCollection version="2.0"><Item key="DaysBefore" type="2">2</Item><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(1006, 2, 'Finalización de contrato','<?xml version="1.0"?><roCollection version="2.0"><Item key="DaysBefore" type="2">2</Item><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(1007, 6, 'Ausencia de varios dias','<?xml version="1.0"?><roCollection version="2.0"><Item key="DaysNoWorking" type="2">2</Item><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(1008, 9, 'Accesos inválidos','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(1009, 11, 'Terminal desconectado','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(1010, 15, 'Empleados que deberian estar presentes','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(1011, 23, 'Tareas que finalizan hoy','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO


INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(1012, 24, 'Tareas que empiezan hoy','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO


INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(1013, 25, 'Tareas donde se ha excedido el tiempo inicial','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(1014, 26, 'Tareas donde se ha sobrepasado la fecha de finalización prevista','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(1015, 27, 'Solicitud de vacaciones o permisos','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID) VALUES(1016, 28, 'Solicitud de cambio de horario','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM')
GO



UPDATE dbo.Notifications SET AllowPortal=0 WHERE AllowPortal Is null
GO

UPDATE dbo.Notifications SET AllowPortal=1 WHERE CreatorID = 'SYSTEM'
GO


ALTER TABLE dbo.sysroNotificationTasks ADD 
IsReaded [Bit] NULL DEFAULT(0),
IDPassportReaded [int] NULL DEFAULT(0),
IsReadedAt [datetime] NULL
GO

UPDATE dbo.sysroNotificationTasks SET IsReaded=1 WHERE IsReaded Is null
GO

UPDATE dbo.sysroNotificationTasks SET IDPassportReaded=0 WHERE IDPassportReaded Is null
GO

ALTER TABLE dbo.sysroPassports ADD Image [image] NULL
GO

ALTER TABLE dbo.Employees ADD
	HighlightColor int NOT NULL CONSTRAINT DF_Employees_HighlightColor DEFAULT 0
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='297' WHERE ID='DBVersion'
GO




