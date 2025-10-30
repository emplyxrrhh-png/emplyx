-- Reordenacion elementos productiv
update sysroGUI set Priority = 680 where IDPath = 'Portal\Task\Analytics'
GO
update sysroGUI set Priority = 690 where IDPath = 'Portal\Task\TaskTemplates'
GO
update sysroGUI set Priority = 700 where IDPath = 'Portal\Task\BusinessCenters'
GO
-- Fin reordenacion elementos productiv

-- Solicitud de fichajes olvidados de tareas
ALTER TABLE dbo.Requests ADD
	IDTask1 int NULL, IDTask2 int NULL, 
	CompletedTask bit NULL Default(0),
	[Field1] [nvarchar](100) NULL,
	[Field2] [nvarchar](100) NULL,
	[Field3] [nvarchar](100) NULL,
	[Field4] [numeric](18, 6) NULL,
	[Field5] [numeric](18, 6) NULL,
	[Field6] [numeric](18, 6) NULL
GO

ALTER TABLE dbo.Requests DROP COLUMN
	IDTask
GO


INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
     VALUES (25800,25,'Tasks.Requests.Forgotten','Fichajes olvidado tareas','','U','RWA',NULL)
GO


INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures]	([IDPassport],[IDFeature],[Permission])
	VALUES	(3,25800,9)
GO

DELETE FROM [dbo].[sysroPassports_PermissionsOverGroups] WHERE [IDApplication] IN (25)
GO

INSERT INTO sysroPassports_PermissionsOverGroups (IDPassport, IDGroup, IDApplication, Permission)
SELECT sysroPassports.ID, Groups.ID, sysroFeatures.ID, 6
FROM sysroFeatures, Groups, sysroPassports
WHERE sysroFeatures.ID IN (25) AND sysroFeatures.AppHasPermissionsOverEmployees = 1 AND Groups.Path = CONVERT(varchar, Groups.ID) AND
	  sysroPassports.GroupType = 'U'
GO

-- Lista de valores de los atributos
ALTER TABLE [dbo].[sysroFieldsTask] ADD
	[TypeValue] [tinyint] NULL DEFAULT(0),
	[ListValues] [text] NULL
GO

UPDATE [dbo].[sysroFieldsTask] SET TypeValue=0 WHERE TypeValue IS NULL
GO

INSERT INTO sysroGUI
([IDPath],[LanguageReference],[URL],[IconURL],[RequiredFeatures],[Priority],[RequiredFunctionalities])
VALUES ('Portal\Access\Analytics', 'Analytics', 'Access/Analytics.aspx', 'TaskAnalytics.png', 'Forms\Access', 715, 'U:Access.Analytics=Read')
GO

INSERT INTO sysroFeatures
([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (9300 ,9 ,'Access.Analytics' ,'Analitica de Accesos' ,'' ,'U' ,'R' ,NULL)
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT sysroPassports.ID, sysroFeatures.ID, 3
FROM sysroFeatures, sysroPassports		
WHERE sysroFeatures.ID IN (9300) AND
	  sysroPassports.IDParentPassport IS NULL AND sysroPassports.GroupType = 'U'
AND (SELECT Count(*) from sysroPassports_PermissionsOverFeatures Where IDFeature = 9 And IDPassport = sysroPassports.ID) > 0
GO


CREATE VIEW [dbo].[sysroAccessCube]
AS
SELECT     dbo.Punches.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee, 
                      dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, CONVERT(nvarchar(8), 
                      dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day, MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Year, 
                      (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, 
                      dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName, dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL 
                      THEN 0 ELSE 1 END AS IsInvalid, CONVERT(NVARCHAR(4000),dbo.Punches.TypeDetails) AS Details
FROM         dbo.sysroEmployeeGroups INNER JOIN
                      dbo.Punches ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee INNER JOIN
                      dbo.EmployeeContracts ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND 
                      dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                      dbo.Zones ON dbo.Punches.IDZone = dbo.Zones.ID LEFT OUTER JOIN
                      dbo.Terminals ON dbo.Punches.IDTerminal = dbo.Terminals.ID LEFT OUTER JOIN
                      dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID, 
                      dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108), 
                      dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name, 
                      (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name, 
                      CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(NVARCHAR(4000),dbo.Punches.TypeDetails)
HAVING      (dbo.Punches.Type = 5) OR (dbo.Punches.Type = 6) OR (dbo.Punches.Type = 7)
GO

----- Gestion de vacaciones
ALTER TABLE dbo.Shifts ADD
	[IDCauseHolidays] [smallint] NULL
GO

UPDATE dbo.Shifts Set IDCauseHolidays=0 WHERE IDCauseHolidays is null AND ShiftType=2
GO

UPDATE dbo.Shifts Set AreWorkingDays=0 WHERE AreWorkingDays is null AND ShiftType=2
GO

UPDATE dbo.Shifts Set IDConceptBalance=0 WHERE IDConceptBalance is null AND ShiftType=2
GO

ALTER TABLE dbo.DailySchedule ADD
	IDShiftBase [smallint] NULL,
	StartShiftBase [datetime] NULL, 
	IDAssignmentBase [smallint] NULL, 
	IsHolidays [bit] NULL
GO

 ALTER VIEW [dbo].[sysroEmployeesShifts]
 AS
 SELECT dbo.Employees.ID AS IDEmployee, dbo.Employees.Name, dbo.sysroEmployeeGroups.IDGroup, 
 	dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.Path, 
 	dbo.Shifts.ID AS IDShift , dbo.Shifts.Name AS ShiftName, dbo.Shifts.ExpectedWorkingHours, dbo.DailySchedule.Date AS CurrentDate, 
 	dbo.sysroEmployeeGroups.SecurityFlags, dbo.GetFullGroupPathName(dbo.sysroEmployeeGroups.IDGroup) AS FullGroupName, dbo.Shifts.IsFloating, dbo.Shifts.StartFloating,  CASE WHEN dbo.DailySchedule.Date <= GETDATE() 
                       THEN StartShiftUsed ELSE StartShift1 END AS StartFloatingOnDay
 FROM dbo.DailySchedule 
 	INNER JOIN dbo.Employees ON dbo.DailySchedule.IDEmployee = dbo.Employees.ID 
 	INNER JOIN dbo.EmployeeContracts ON dbo.Employees.ID = dbo.EmployeeContracts.IDEmployee 
 		AND dbo.DailySchedule.Date >= dbo.EmployeeContracts.BeginDate 
 		AND dbo.DailySchedule.Date <= dbo.EmployeeContracts.EndDate 
 	INNER JOIN dbo.sysroEmployeeGroups ON dbo.Employees.ID = dbo.sysroEmployeeGroups.IDEmployee 
 		AND dbo.DailySchedule.Date >= dbo.sysroEmployeeGroups.BeginDate 
 		AND dbo.DailySchedule.Date <= dbo.sysroEmployeeGroups.EndDate 
 	LEFT OUTER JOIN dbo.Shifts ON dbo.Shifts.ID = (SELECT CASE WHEN Date<=GETDATE() THEN IDShiftUsed ELSE IDShift1 END FROM DailySchedule DS WHERE DS.Date = DailySchedule.Date AND DS.IDEmployee = DailySchedule.IDEmployee)
GO

CREATE TABLE [dbo].[TMPHOLIDAYSCONTROL](
	[IDEmployee] [int] NOT NULL,
	[IDConcept] [smallint] NOT NULL,
	[StartupValue] [numeric](16, 2) NOT NULL,
	[CarryOver] [numeric](16, 2) NOT NULL,
	[EnjoyedDays] [numeric](16, 2) NOT NULL,
	[CurrentBalance] [numeric](16, 2) NOT NULL,
	[DaysProvided] [numeric](16, 2) NOT NULL,
	[EndingBalance] [numeric](16, 2) NOT NULL
) ON [PRIMARY]

GO

-- Gestion de absentismo
CREATE TABLE [dbo].[CausesDocuments](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[IDCause] [int] NOT NULL,
	[IDLabAgree] [int] NOT NULL,
	[IDDocument] [int] NOT NULL,
	[Parameters] [nvarchar](max) NULL,
 CONSTRAINT [PK_CausesDocuments] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Causes] ADD DefaultValuesAbsences [nvarchar](MAX) default ''
GO

ALTER TABLE [dbo].[Causes] ADD TraceDocumentsAbsences bit DEFAULT 0
GO

UPDATE [dbo].[Causes] SET DefaultValuesAbsences = '' WHERE DefaultValuesAbsences is null
GO

ALTER TABLE [dbo].[Causes] ADD TraceDocumentsAbsences bit DEFAULT 0
GO

UPDATE [dbo].[Causes] SET TraceDocumentsAbsences = 0 WHERE TraceDocumentsAbsences is null
GO

CREATE TABLE [dbo].[DocumentsAbsences](
                [ID] [int] NOT NULL,
                [Name] [nvarchar](50) NOT NULL,
                [ShortName] [nvarchar](3) NULL,
                [Description] [nvarchar](max) NULL,
                [RememberText] [nvarchar](max) NULL,
 CONSTRAINT [PK_DocumentsAbsences] PRIMARY KEY CLUSTERED 
(
                [ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[DocumentsAbsencesAdvices](
                [ID] [int] IDENTITY(1,1) NOT NULL,
                [IDDocumentAbsence] [int] NULL,
                [Name] [nvarchar](50) NULL,
                [Advice] [nvarchar](max) NULL,
 CONSTRAINT [PK_DocumentsAbsencesAdvices] PRIMARY KEY CLUSTERED 
(
                [ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO



ALTER TABLE [dbo].[ProgrammedCauses]
DROP CONSTRAINT PK_ProgrammedCauses
GO

ALTER TABLE [dbo].[ProgrammedCauses] ADD [ID] [INT] NOT NULL default(0)
GO

ALTER TABLE [dbo].[ProgrammedCauses] ADD [FinishDate] [smalldatetime] NULL
GO

UPDATE  [dbo].[ProgrammedCauses] SET [FinishDate] = [Date] WHERE  [FinishDate] IS NULL
GO


ALTER TABLE [dbo].[ProgrammedCauses] ADD [MinDuration] [numeric](8,6)  NULL default(0)
GO

UPDATE [dbo].[ProgrammedCauses] SET [MinDuration] = 0 WHERE  [MinDuration] is NULL
GO


UPDATE [dbo].[ProgrammedCauses] SET [ID] = 0 WHERE  [ID] is NULL
GO



ALTER TABLE [dbo].[ProgrammedCauses]
ADD CONSTRAINT PK_ProgrammedCauses PRIMARY KEY (IDEmployee, Date, ID)
GO


CREATE TABLE [dbo].[AbsenceTracking](
	[ID] [numeric](16, 0) IDENTITY(1,1) NOT NULL,
	[TypeAbsence] [tinyint] NOT NULL,
	[IDEmployee] [int] NOT NULL,
	[IDCause] [smallint] NOT NULL,
	[Date] [smalldatetime] NOT NULL,
	[IDAbsence] [int] NULL DEFAULT(0),
	[TrackDay] [smalldatetime] NOT NULL,
	[IDDocument] [int] NOT NULL,
	[DeliveryDate] [smalldatetime] NULL,
	[IDPassport] [int] NULL,
	[NotificationDate] [smalldatetime] NULL,
	[NotificationHistory] [nvarchar](max) NULL,
	[AttachmentFile] [image] NULL,
	[Comments] [nvarchar](max) NULL,

 CONSTRAINT [PK_AbsenceTracking] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO sysroFeatures
([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (30 ,null ,'DocumentsAbsences' ,'DocumentsAbsences' ,'' ,'U' ,'RWA' ,NULL)
GO


INSERT INTO sysroFeatures
([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (30000 ,30 ,'DocumentsAbsences.Definition' ,'Definición Documentos' ,'' ,'U' ,'RWA' ,NULL)
GO


INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
VALUES (3 ,30 ,9)
GO


INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
VALUES (3 ,30000 ,9)
GO


INSERT INTO dbo.sysroNotificationTypes VALUES(32,'Absence document pending',null, 120)
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal) VALUES(1800, 32, 'Documentos pendientes de entrega','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',0)
GO

INSERT INTO dbo.sysroNotificationTypes VALUES(33,'Advice for Document not delivered',null, 120)
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal) VALUES(1801, 33, 'Aviso por documento no entregado','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',0)
GO

INSERT INTO dbo.sysroNotificationTypes VALUES(34,'Document not delivered on Track day',null, 120)
GO

INSERT INTO dbo.sysroNotificationTypes VALUES(35,'Advice for Day with unmatched time record',null, 120)
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal) VALUES(1802, 35, 'Aviso de Días con fichajes impares','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',0)
GO


INSERT INTO [dbo].[sysroGUI] ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\ShiftControl\AbsencesStatus','GUI.Absences','Absences/AbsencesStatus.aspx','AbsencesStatus.png',NULL,NULL,'Feature\Absences',NULL,617,'NWR','U:Employees=Read')
GO

CREATE VIEW [dbo].[sysrovwAbsences]
AS
SELECT row_number() OVER(order by x.BeginDate) AS ID, x.* ,emp.Name as EmployeeName, cause.Name as CauseName, grp.FullGroupName
FROM (
(SELECT 	NULL AS IDRelatedObject,
			IDCause AS IDCause, 
			IDEmployee AS IDEmployee, 
			BeginDate AS BeginDate, 
			ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) AS FinishDate, 
			NULL AS BeginTime, 
			NULL AS EndTime, 
			CONVERT(NVARCHAR(4000),Description) AS Description ,
			(SELECT CASE WHEN (select COUNT(*) from AbsenceTracking at where TrackDay < GETDATE() and DeliveryDate is null and at.IDCause = pa.IDCause and at.IDEmployee = pa.IDEmployee and at.Date = pa.BeginDate) > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered, 
			(SELECT CASE WHEN GETDATE() between BeginDate and DATEADD(DAY,1,(ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )))) THEN '1' WHEN ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) < GETDATE()  THEN '2'  ELSE '0' END) AS Status,
			'ProgrammedAbsence' AS AbsenceType 
 FROM dbo.ProgrammedAbsences pa)
union
(SELECT     ID AS IDRelatedObject,
			IDCause AS IDCause, 
			IDEmployee AS IDEmployee, 
			Date AS BeginDate, 
			ISNULL(FinishDate,Date) AS FinishDate, 
			BeginTime AS BeginTime, 
			EndTime AS EndTime, 
			CONVERT(NVARCHAR(4000),Description) AS Description, 
			(SELECT CASE WHEN (select COUNT(*) from AbsenceTracking at where TrackDay < GETDATE() and DeliveryDate is null and at.IDCause = pc.IDCause and at.IDEmployee = pc.IDEmployee and at.Date = pc.Date and at.IDAbsence = pc.ID) > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered, 
			(SELECT CASE WHEN GETDATE() between Date and DATEADD(DAY,1,(ISNULL(FinishDate, Date))) THEN '1' WHEN  DATEADD(DAY,1,ISNULL(FinishDate,Date)) < GETDATE()  THEN '2'  ELSE '0' END) AS Status,
			'ProgrammedCause' AS AbsenceType 
 FROM dbo.ProgrammedCauses pc)
union
(SELECT     ID AS IDRelatedObject,
			IDCause AS IDCause, 
			IDEmployee AS IDEmployee, 
			Date1 AS BeginDate, 
			ISNULL(Date2,Date1) AS FinishDate, 
			FromTime AS BeginTime, 
			ToTime AS EndTime, 
			CONVERT(NVARCHAR(4000),Comments) AS Description, 
			1 AS DocumentsDelivered,
			(SELECT CASE WHEN Status = 0 THEN '-1' ELSE '-2' END) AS Status,
			(SELECT CASE WHEN RequestType = 7 THEN 'RequestAbsence' ELSE 'RequestCause' END) AS AbsenceType 
 FROM dbo.Requests where RequestType in(7,9) and Status in(0,1))
 )x
 inner join dbo.Employees as emp on x.IDEmployee = emp.ID
 inner join dbo.Causes as cause on x.IDCause = cause.ID
 inner join dbo.sysrovwCurrentEmployeeGroups as grp on x.IDEmployee = grp.IDEmployee

GO

-- Optimizacion permisos sobre solicitudes
ALTER TABLE dbo.sysroFeatures ADD
	AliasID int NULL,
	EmployeeFeatureID int NULL
GO

CREATE FUNCTION [dbo].[GetAllPassportParentsRequestPermissions]
 	(
 		
 	)
 RETURNS @result table (IDPassport int, IDParentPassport int, RequestPermissionCount int)
 AS
 BEGIN
 	/* Returns all parents of specified passport */
 	
 	Declare @idPassport int
 	Declare @idRunningPassport int
 	
 	DECLARE db_cursor CURSOR FOR  
	SELECT ID
	FROM sysroPassports
	WHERE GroupType <> 'U' And IDUser is not null

	OPEN db_cursor  
	FETCH NEXT FROM db_cursor INTO @idPassport  

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		   set @idRunningPassport = @idPassport
		   
		   SELECT @idRunningPassport = IDParentPassport FROM sysroPassports WHERE ID = @idRunningPassport
		   
		   WHILE NOT @idRunningPassport IS NULL
			BEGIN
				INSERT INTO @result VALUES (@idPassport,@idRunningPassport, (select COUNT(*) from sysroPassports_PermissionsOverFeatures where IDPassport = @idRunningPassport and IDFeature in (1560,2321,2322,2323,2510,2520,2540,2550,25800) ))
				
				SELECT @idRunningPassport = IDParentPassport
				FROM sysroPassports
				WHERE ID = @idRunningPassport
			END
			
				   
		   FETCH NEXT FROM db_cursor INTO @idPassport  
	END  

	CLOSE db_cursor  
	DEALLOCATE db_cursor 
 	
 	RETURN 
 END
GO


update dbo.sysroFeatures set AliasID = 1, EmployeeFeatureID = 1 where ID = 1560
GO

update dbo.sysroFeatures set AliasID = 2, EmployeeFeatureID = 2 where ID = 2321
GO

update dbo.sysroFeatures set AliasID = 3, EmployeeFeatureID = 2 where ID = 2322
GO

update dbo.sysroFeatures set AliasID = 4, EmployeeFeatureID = 2 where ID = 2323
GO

update dbo.sysroFeatures set AliasID = 5, EmployeeFeatureID = 2 where ID = 2510
GO

update dbo.sysroFeatures set AliasID = 6, EmployeeFeatureID = 2 where ID = 2520
GO

update dbo.sysroFeatures set AliasID = 7, EmployeeFeatureID = 2 where ID = 2540
GO

update dbo.sysroFeatures set AliasID = 9, EmployeeFeatureID = 2 where ID = 2550
GO

update dbo.sysroFeatures set AliasID = 10, EmployeeFeatureID = 25 where ID = 25800
GO

 ALTER FUNCTION [dbo].[GetRequestLevelsBelow]
 	(
 	@idPassport int,
 	@idRequest int
 	)
 RETURNS int
 AS
 BEGIN
 	DECLARE @LevelsBelow int,
 			@LevelOfAuthority int,
    		@featureAlias nvarchar(100),
 			@EmployeefeatureID int,
 			@idEmployee int,
 			@RequestLevel int,
 			@RequestType int
 	SELECT @LevelOfAuthority = dbo.GetPassportLevelOfAuthority(@idPassport)
 	
 	SELECT @RequestType = Requests.RequestType,
 		   @idEmployee = Requests.IDEmployee,
 		   @RequestLevel = ISNULL(Requests.StatusLevel, 11)
    FROM Requests
 	WHERE Requests.ID = @idRequest
 	
 	SELECT @featureAlias = Alias, 
 		   @EmployeefeatureID = EmployeeFeatureId 
 	FROM dbo.sysroFeatures 
 	WHERE sysroFeatures.AliasId = @RequestType
 	
 	 	
 	/* Obtiene el número de niveles que hay entre el nivel del validador (@idPassport) y el nivel actual de la solicitud (@RequestLevel) */
 	SELECT @LevelsBelow = 
 		(SELECT COUNT( DISTINCT dbo.GetPassportLevelOfAuthority(sysroPassports.ID))
 		 FROM sysroPassports INNER JOIN 
 		 (SELECT Id, dbo.GetPassportLevelOfAuthority(ID) AS value FROM sysroPassports WHERE ID in(select DISTINCT(IDPassport) FROM dbo.GetAllPassportParentsRequestPermissions() where RequestPermissionCount > 0 )) gpla
 		 ON gpla.ID = sysroPassports.id 
 		 WHERE sysroPassports.GroupType <> 'U' AND gpla.value > @LevelOfAuthority AND gpla.value <= @RequestLevel AND
 		 (select dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID,@FeatureAlias, 'U', 2)) > 0 AND
 		 (select dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@EmployeefeatureID,2,1,getdate())) > 0)
    	IF @RequestLevel = 11  SET @LevelsBelow = @LevelsBelow + 1
 	
 	RETURN @LevelsBelow
 END
 GO
 
   ALTER FUNCTION [dbo].[GetRequestPassportPermission]
 (
 	@idPassport int,
  	@idRequest int	
 )
 RETURNS int
 AS
 BEGIN
  	DECLARE @FeatureAlias nvarchar(100),
  			@EmployeefeatureID int,
  			@idEmployee int,
 			@RequestType int
 			
  	SELECT @RequestType = Requests.RequestType,
 		   @idEmployee = Requests.IDEmployee
    FROM Requests
 	WHERE Requests.ID = @idRequest
 	
 	SELECT @featureAlias = Alias, 
 		   @EmployeefeatureID = EmployeeFeatureId 
 	FROM dbo.sysroFeatures 
 	WHERE sysroFeatures.AliasId = @RequestType
  	
  	
  	DECLARE @Permission int
  	SET @Permission = (select dbo.WebLogin_GetPermissionOverFeature(@idPassport,@FeatureAlias, 'U', 2)) 
  	
  	IF @Permission > 0 
  		BEGIN
  			SET @Permission = (select dbo.WebLogin_GetPermissionOverEmployee(@idPassport,@idEmployee,@EmployeefeatureID,2,1,getdate()))
  		END
  		   
  	RETURN @Permission
 END
 GO
 
 ALTER FUNCTION [dbo].[GetRequestMinStatusLevel]
(
	@idPassport int,
	@featureAlias nvarchar(100),
	@EmployeefeatureID int,
	@idEmployee int
)
RETURNS int
AS
BEGIN
	/* While looking only at permissions defined directly on passport,
  	returns the first permission found in the employees groups hierarchy */
  	DECLARE @MinStatusLevel int
  	DECLARE @LevelOfAuthority int
 	
 	SELECT @LevelOfAuthority = dbo.GetPassportLevelOfAuthority(@idPassport)
  	
 	SELECT @MinStatusLevel = 
 	(SELECT TOP 1 Parents.LevelOfAuthority
 	 FROM sysroPassports INNER JOIN sysroPassports Parents
 			ON sysroPassports.IDParentPassport = Parents.ID
 	 WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null AND
 	 	  dbo.GetPassportLevelOfAuthority(Parents.ID) > @LevelOfAuthority AND
 		  (select dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID,@FeatureAlias, 'U', 2)) > 0 AND
 		  (select dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@EmployeefeatureID,2,1,getdate())) > 0 
 	 ORDER BY Parents.LevelOfAuthority ASC)
  	
	RETURN @MinStatusLevel
END
GO

ALTER FUNCTION [dbo].[GetRequestNextLevel]
(	
  	@idRequest int	
)
RETURNS int
AS
BEGIN
  	DECLARE @NextLevel int
  	SELECT @NextLevel = 
  	(SELECT MAX(dbo.GetPassportLevelOfAuthority(Parents.ID))
  	FROM sysroPassports  INNER JOIN sysroPassports Parents
		ON sysroPassports.IDParentPassport = Parents.ID
  	WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null AND 
  	      dbo.GetPassportLevelOfAuthority(Parents.ID) < ISNULL(Requests.StatusLevel, 11) AND
  	      dbo.GetRequestPassportPermission(sysroPassports.ID, Requests.ID) > 0)
  	FROM Requests
  	WHERE Requests.ID = @idRequest
  		   
  	RETURN @NextLevel
END
GO

ALTER FUNCTION [dbo].[GetRequestNextLevelPassports]
(	
  	@idRequest int	
)
RETURNS nvarchar(1000)
AS
BEGIN
  	DECLARE @RetNames nvarchar(1000), 
  			@PassportName nvarchar(50)
  	SET @RetNames = ''
  	DECLARE PassportsCursor CURSOR
		FOR  SELECT sysroPassports.Name 
  			 FROM sysroPassports 
  			 INNER JOIN (SELECT id, dbo.GetPassportLevelOfAuthority(ID) AS Level FROM sysroPassports) gpl
  			 on sysroPassports.ID=gpl.ID 
  			 WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null
  			 AND gpl.level=dbo.GetRequestNextLevel(@idRequest)
			 AND dbo.GetRequestPassportPermission(sysroPassports.ID, @idRequest) > 0
  			 ORDER BY sysroPassports.Name
  	OPEN PassportsCursor
  	FETCH NEXT FROM PassportsCursor
  	INTO @PassportName
  	WHILE @@FETCH_STATUS = 0
  	BEGIN	
  	
  		IF @RetNames = ''
  			BEGIN
  				SET @RetNames = @PassportName
  			END
  		ELSE
  			BEGIN
  				SET @RetNames = @RetNames + ', '+ @PassportName
  			END
  		FETCH NEXT FROM PassportsCursor 
  		INTO @PassportName
  	END 
  	CLOSE PassportsCursor
  	DEALLOCATE PassportsCursor
  	RETURN @RetNames
END
GO

-- Campo en la tabla de terminales para que el notificador controle el número de desconexiones de cada terminal
ALTER TABLE [dbo].[Terminals] ADD [DisconnectionCounter] [tinyint] NULL default(0)
GO
UPDATE [dbo].[Terminals] SET [DisconnectionCounter] = 0
GO

-- Añade a ImportGuides un registro para la importación de puestos asignados
insert into ImportGuides (ID,Name,Template,Mode, [Type],FormatFilePath, SourceFilePath, Separator, CopySource) values(6,'Carga de Puestos Asignados',0,1,1,'','','',0)

--Añadimos la columna con el nombre de fichero del seguimiento
ALTER TABLE dbo.AbsenceTracking ADD
	AttachmentFileName nvarchar(50) NULL
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='309' WHERE ID='DBVersion'
GO

