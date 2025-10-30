----------------ALERTAS----------------------------------------------------------------------
-- Modificaciones en tabla de definición de Notificaciones
ALTER TABLE [dbo].[Notifications]
ADD 
	[ShowOnDesktop] [bit] NULL DEFAULT(0)
GO

UPDATE dbo.Notifications SET ShowOnDesktop=0 WHERE ShowOnDesktop Is null
GO

-- Fechas en que se produjo la alarma
ALTER TABLE [dbo].sysroNotificationTasks ADD [FiredDate] DATETIME NULL
GO

-- Marcamos las notificaciones que son de sistema
ALTER TABLE dbo.sysroNotificationTypes ADD [OnlySystem] [bit] NULL DEFAULT(0)
GO
UPDATE dbo.sysroNotificationTypes SET OnlySystem = 0
GO
UPDATE dbo.sysroNotificationTypes SET OnlySystem = 1 WHERE ID IN(13,14,27,28,32,33,35, 36,37,38,39,30)
GO
UPDATE dbo.sysroNotificationTypes SET FeatureType = 'U' WHERE FeatureType = 'E'
GO

-- Correción de definición de tipo de features
update dbo.sysroNotificationTypes set FeatureType = 'U' where feature = 'Calendar.Punches.Punches'
GO

-- Nueva alerta de solicitudes pendientes
INSERT INTO [dbo].sysroNotificationTypes VALUES(40, 'Requests Pending',NULL, 120, '*Requests*','U',1)
GO
INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal, ShowOnDesktop) VALUES(2000, 40, 'Aviso de solicitudes pendientes','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',0,1)
GO

-- Nueva alerta de Empleados presentes con documentación caducada
INSERT INTO [dbo].sysroNotificationTypes VALUES(41, 'Employee Present With Expired Docs',NULL,120, 'Employees.UserFields.Information','U',1)
GO
INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal, ShowOnDesktop) VALUES(2001, 41, 'Aviso de empleado presente con documentación caducada','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',0,1)
GO

-- Nueva alerta de sistema de documentos no estregados
INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal, ShowOnDesktop) VALUES(2002, 34, 'Alerta de documentación de absentismo no recibida','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',0,1)
GO

-- Configuramos visibilidad de alertas en VisualTime DeskTop
UPDATE [dbo].Notifications SET ShowOnDesktop = 1 WHERE IDType in (40,41,19,21,20,15,34) AND CreatorID = 'SYSTEM'
GO

-- Columna para control de estado de PRL
ALTER TABLE [dbo].EmployeeStatus ADD [PRLStatus] [nvarchar](2) NULL
GO

INSERT INTO [dbo].sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
   VALUES ('Portal\General\Alerts','Alerts','Alerts/Alerts.aspx','Alerts.png',NULL,NULL,NULL,NULL,104,NULL,NULL)
GO

-- Columna para control de última tarea borrada de un tipo 
ALTER TABLE [dbo].Notifications ADD [LastTaskDeleted] DATETIME NULL
GO

-- Informamos FiredDate para notificaciones ya existentes
update sysroNotificationTasks set FiredDate = NextDateTimeExecuted where FiredDate is null and IDNotification in (1001, 1002, 1003, 1010, 2000, 2001, 2002)
GO

-- Limpieza de alertas
-- 1.- Fichajes impares
-- Todas las que ya no aplican
delete sysroNotificationTasks where ID in ( 
select noti.id from (select * from sysroNotificationTasks where IDNotification = 1001) noti 
full outer join 
(select * from sysrovwIncompletedDays) as incom 
on incom.IDEmployee = noti.Key1Numeric and incom.date = noti.Key3DateTime 
where incom.IDEmployee Is Null)
-- Y en cualquier caso las más antiguas de 30 días
delete sysroNotificationTasks where IDNotification = 1001 and  Key3DateTime < DATEADD(d,-30,getdate())
GO


-- 2.- Fichajes no fiables
-- Todas las que ya no aplican
delete sysroNotificationTasks where ID in (
select noti.id from
(select * from sysroNotificationTasks where IDNotification = 1002) noti
full outer join
(select * from punches where IsNotReliable = 0) as punch
on punch.IDEmployee = noti.Key1Numeric and punch.shiftdate = noti.Key3DateTime
Where punch.IDEmployee Is Null
)
GO
-- Y en cualquier caso las más antiguas de 30 días
delete sysroNotificationTasks where IDNotification = 1002 and  Key3DateTime < DATEADD(d,-30,getdate())
GO


-- 3.- Fichajes no justificados (1003)
-- Todas las que ya no aplican
delete sysroNotificationTasks where ID in ( 
select noti.id from ( 
select * from sysroNotificationTasks where IDNotification = 1003) noti 
full outer join (
select * from DailyCauses where IDCause = 0) as caus 
on caus.IDEmployee = noti.Key1Numeric and caus.Date = noti.Key3DateTime 
Where caus.IDEmployee Is Null)
GO

-- Y en cualquier caso las más antiguas de 30 días
delete sysroNotificationTasks where IDNotification = 1003 and  Key3DateTime < DATEADD(d,-30,getdate())
GO

-- 4.- Empleados que deberían estar presentes. Borramos todas las alarmas más antiguas de 1 día
delete sysroNotificationTasks where IDNotification = 1010 and  Key3DateTime < DATEADD(d,-1,getdate())
GO


----------------ALERTAS----------------------------------------------------------------------

-- Pantalla de Solicitudes disponible sin licencia para poder usarse con terminales mx8P
update [dbo].sysrogui set RequiredFeatures = null where RequiredFeatures = 'Feature\LivePortal'
GO

----------------LICENCIA AUDITORIA------------------------------------------------------------
update sysroGUI set RequiredFeatures='Forms\Audit' where idPath='NavBar\Config\Audit' OR idPath='Portal\Security\Audit'
GO


-- Se crea un nuevo indice y se modifica uno existente para mejorar rendimiento al modificar un saldo y recalcular
CREATE NONCLUSTERED INDEX [PX_DailyAccrtuals_20140606]
ON [dbo].[DailyAccruals] ([IDConcept],[Date])
INCLUDE ([IDEmployee],[CarryOver],[StartupValue])
GO


DROP index [_dta_index_DailySchedule_c_6_503672842__K9_K2] ON [dbo].[DailySchedule] 
GO

CREATE NONCLUSTERED INDEX [_dta_index_DailySchedule_c_6_503672842__K9_K2] ON [dbo].[DailySchedule] 
(
	[Status] ASC,
	[Date] ASC
)ON [PRIMARY]
GO

-- Correción de vista de fichajes impares
 ALTER view [dbo].[sysrovwIncompletedDays] 
 AS
 SELECT punchesInfo.pInCount, punchesInfo.pOutCount, punchesInfo.Name, punchesInfo.IDEmployee,punchesInfo.Date from(
	 SELECT pIn.pInCount, ISNULL(pOut.pOutCount,0) as pOutCount, pIn.Name, pIn.IDEmployee,pIn.Date FROM
		 (SELECT count(*) AS pInCount, Employees.Name, Punches.IDEmployee, Punches.ShiftDate AS Date
		 FROM Employees,  Punches
		 WHERE Employees.ID = Punches.IDEmployee 
		 AND (ActualType=1) 
		 GROUP BY Punches.ShiftDate, Employees.Name, Punches.IDEmployee) pIn
		FULL OUTER JOIN
		 (SELECT count(*)  AS pOutCount, Employees.Name, Punches.IDEmployee, Punches.ShiftDate AS Date
		 FROM Employees,  Punches
		 WHERE Employees.ID = Punches.IDEmployee 
		 AND (ActualType=2) 
		 GROUP BY Punches.ShiftDate, Employees.Name, Punches.IDEmployee) pOut
	 ON pIn.IDEmployee = pOut.IDEmployee AND pIn.Date = pOut.Date	 
	 ) punchesInfo
 WHERE punchesInfo.pInCount <> punchesInfo.pOutCount

GO

-- correccion permisos sobre la pantalla de documentos de absentismo
UPDATE sysrogui SET RequiredFunctionalities='U:DocumentsAbsences=Read' WHERE IDPath LIKE 'Portal\ShiftManagement\AbsencesDocuments'
GO




-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='326' WHERE ID='DBVersion'
GO

