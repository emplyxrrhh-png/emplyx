CREATE TABLE [dbo].[ProjectTemplates](
	[ID] [int] NOT NULL,
	[Project] [nvarchar](75) NOT NULL,
	[IDPassport] [int] NOT NULL,
	[IDCenter] [smallint] NOT NULL,
 CONSTRAINT [PK_ProjectTemplates] PRIMARY KEY NONCLUSTERED 
(

	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[TaskTemplates](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](75) NOT NULL,
	[ShortName] [nvarchar](4) NULL,
	[Description] [ntext] NULL,
	[Color] [int] NOT NULL,
	[IDProject] [int] NOT NULL,
	[Tag] [nvarchar](75) NULL,
	[Priority] [int] NOT NULL,
	[IDPassport] [int] NOT NULL,
	[ExpectedStartDate] [smalldatetime] NULL,
	[ExpectedEndDate] [smalldatetime] NULL,
	[InitialTime] [numeric](18, 6) NOT NULL,
	[TypeCollaboration] [tinyint] NULL,
	[ModeCollaboration] [tinyint] NULL,
	[TypeActivation] [tinyint] NULL,
	[ActivationTask] [int] NULL,
	[ActivationDate] [smalldatetime] NULL,
	[TypeClosing] [tinyint] NULL,
	[ClosingDate] [smalldatetime] NULL,
	[TypeAuthorization] [tinyint] NULL
 CONSTRAINT [PK_TaskTemplates] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[TaskTemplates] ADD  DEFAULT ((0)) FOR [Color]
GO

ALTER TABLE [dbo].[TaskTemplates] ADD  DEFAULT ((0)) FOR [Priority]
GO

ALTER TABLE [dbo].[TaskTemplates] ADD  DEFAULT ((0)) FOR [IDPassport]
GO

ALTER TABLE [dbo].[TaskTemplates] ADD  DEFAULT ((0)) FOR [InitialTime]
GO

ALTER TABLE [dbo].[TaskTemplates] ADD  DEFAULT ((0)) FOR [TypeCollaboration]
GO

ALTER TABLE [dbo].[TaskTemplates] ADD  DEFAULT ((0)) FOR [ModeCollaboration]
GO

ALTER TABLE [dbo].[TaskTemplates] ADD  DEFAULT ((0)) FOR [TypeActivation]
GO

ALTER TABLE [dbo].[TaskTemplates] ADD  DEFAULT ((0)) FOR [ActivationTask]
GO

ALTER TABLE [dbo].[TaskTemplates] ADD  DEFAULT ((0)) FOR [TypeClosing]
GO

ALTER TABLE [dbo].[TaskTemplates] ADD  DEFAULT ((0)) FOR [TypeAuthorization]
GO

CREATE TABLE [dbo].[EmployeeTaskTemplates](
	[IDEmployee] [int] NOT NULL,
	[IDTask] [int]  NOT NULL,
 CONSTRAINT [PK_EmployeeTaskTemplates] PRIMARY KEY CLUSTERED 
(
	[IDEmployee] ASC,
	[IDTask] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[GroupTaskTemplates](
	[IDGroup] [int] NOT NULL,
	[IDTask] [int]  NOT NULL,
 CONSTRAINT [PK_GroupTaskTemplates] PRIMARY KEY CLUSTERED 
(
	[IDGroup] ASC,
	[IDTask] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO




CREATE TABLE [dbo].[sysroFieldsTask](
	[ID] [tinyint] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Type] [smallint] NOT NULL,
	[Action] [tinyint] NOT NULL,
 CONSTRAINT [PK_sysroFieldsTask] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY] 

GO

INSERT INTO [dbo].[sysroFieldsTask] (ID, Name, TYPE, Action ) VALUES(1,'Campo 1', 0, 0)
GO
INSERT INTO [dbo].[sysroFieldsTask] (ID, Name, TYPE, Action ) VALUES(2,'Campo 2', 0, 0)
GO
INSERT INTO [dbo].[sysroFieldsTask] (ID, Name, TYPE, Action ) VALUES(3,'Campo 3', 0, 0)
GO
INSERT INTO [dbo].[sysroFieldsTask] (ID, Name, TYPE, Action ) VALUES(4,'Campo 4', 1, 0)
GO
INSERT INTO [dbo].[sysroFieldsTask] (ID, Name, TYPE, Action ) VALUES(5,'Campo 5', 1, 0)
GO
INSERT INTO [dbo].[sysroFieldsTask] (ID, Name, TYPE, Action ) VALUES(6,'Campo 6', 1, 0)
GO


ALTER TABLE [dbo].[Punches] ADD Field1 [nvarchar](100) NULL , Field2 [nvarchar](100) NULL , Field3 [nvarchar](100) NULL , Field4 [numeric](18,6) NULL , Field5 [numeric](18,6) NULL , Field6 [numeric](18,6) NULL 
GO


EXEC sp_rename 'DailyTaskAccruals', 'TMP_DailyTaskAccruals_OLD'
GO

CREATE TABLE [dbo].[DailyTaskAccruals](
	[IDEmployee] [int] NOT NULL,
	[IDTask] [int] NOT NULL,
	[Date] [smalldatetime] NOT NULL,
	[IDPart] [int] NOT NULL,
	[Value] [numeric](9, 6) NOT NULL,
	[Field1] [nvarchar](100) NULL , 
	[Field2] [nvarchar](100) NULL , 
	[Field3] [nvarchar](100) NULL , 
	[Field4] [numeric](18,6) NULL , 
	[Field5] [numeric](18,6) NULL , 
	[Field6] [numeric](18,6) NULL, 
 CONSTRAINT [PK_ZDailyTaskAccruals] PRIMARY KEY NONCLUSTERED 
(
	[IDEmployee] ASC,
	[Date] ASC,
	[IDPart] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[DailyTaskAccruals] SELECT IDEmployee, IDTask, DATE, IDTask, value, NULL, NULL, NULL, NULL, NULL, NULL FROM [dbo].[TMP_DailyTaskAccruals_OLD]
GO


INSERT INTO dbo.sysroTasks (ProcessID,CreatorID,Context,DateCreated ,ID ,SystemTask ,ExternalTask)  VALUES
('PROC:\\TASKACCRUALS','SESSION:\\SYSTEM','<?xml version="1.0"?><roCollection version="2.0"><Item key="Status" type="8">PENDING</Item><Item key="Command" type="8">ON_UPDATED_EMPLOYEETASKPUNCHES</Item></roCollection>', getdate(),'TASK:\\119999', 0, 0)
GO


INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
     VALUES (28,NULL,'TaskPunches','Fichajes de tareas','','E','RW',NULL)

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
     VALUES (29,NULL,'TaskTotals','Saldos de tareas','','E','R',NULL)

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
     VALUES (28000,28,'TaskPunches.Query','Consulta','','E','R',NULL)

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
     VALUES (28001,28,'TaskPunches.Punches','Fichar','','E','W',NULL)

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
     VALUES (28100,28,'TaskPunches.Requests','Solicitudes','','E','RW',NULL)

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
     VALUES (28110,28100,'TaskPunches.Requests.Forgotten','Fichajes Olvidados','','E','RW',NULL)

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
     VALUES (29001,29,'Tasktotals.Query','Consulta de saldos','','E','R',NULL)
GO

INSERT INTO [dbo].[sysroGUI] ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\Task\TaskTemplates','TaskTemplates','TaskTemplates/TaskTemplates.aspx','TaskTemplates.png',NULL,NULL,'Feature\Productiv',NULL,700,NULL,'U:Tasks.TemplatesDefinition=Read')
GO
 
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
     VALUES (25600,25,'Tasks.TemplatesDefinition','Definicion de plantillas','','U','RWA',NULL)
GO
 
INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures]	([IDPassport],[IDFeature],[Permission])
	VALUES	(3,25600,9)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] ([IDPassport], [IDFeature], [Permission])
SELECT sysroPassports.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM sysroFeatures, sysroPassports		
WHERE sysroFeatures.ID IN (25500, 25510) AND
	  sysroPassports.IDParentPassport IS NULL AND sysroPassports.GroupType = 'U'
GO

DELETE FROM [dbo].[sysroPassports_PermissionsOverGroups] WHERE [IDApplication] IN (25)
GO

INSERT INTO sysroPassports_PermissionsOverGroups (IDPassport, IDGroup, IDApplication, Permission)
SELECT sysroPassports.ID, Groups.ID, sysroFeatures.ID, 6
FROM sysroFeatures, Groups, sysroPassports
WHERE sysroFeatures.ID IN (25) AND sysroFeatures.AppHasPermissionsOverEmployees = 1 AND Groups.Path = CONVERT(varchar, Groups.ID) AND
	  sysroPassports.GroupType = 'U'
GO


INSERT INTO [dbo].[sysroGUI] ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('LivePortal\Tasks','LivePortal.Tasks','','Task.png',NULL,NULL,NULL,NULL,810,NULL,NULL)

INSERT INTO [dbo].[sysroGUI] ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('LivePortal\Tasks\MyTask','LivePortal.MyTask','Tasks/CurrentTask.aspx','CurrentTask.png',NULL,'ProductivEmployee','Feature\Productiv',NULL,910,NULL,NULL)

INSERT INTO [dbo].[sysroGUI] ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES	('LivePortal\Tasks\Accruals','LivePortal.TaskAccruals','Tasks/TaskAccrualsQuery.aspx','Querys.png',NULL,'ProductivEmployee','Feature\Productiv',NULL,1010,NULL,'E:Tasktotals.Query=Read')

INSERT INTO [dbo].[sysroGUI] ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES('LivePortal\Tasks\Punches','LivePortal.TaskPunches','Tasks/TaskPunches.aspx','Punches.png',NULL,'ProductivEmployee','Feature\Productiv',NULL,1110,NULL,'E:TaskPunches.Query=Read')

GO

ALTER TABLE dbo.Requests ADD
	IDTask smallint NULL
GO
ALTER TABLE dbo.Requests SET (LOCK_ESCALATION = TABLE)
GO


CREATE FUNCTION dbo.GetPassportLevelOfAuthority
(
	@idPassport int
)
RETURNS int
AS
BEGIN
	declare @levelOfAuthority int

	;with cte (id,idparentpassport,levelofauthority)
	as
	(
	select ID,IDParentPassport, LevelOfAuthority from sysroPassports where id = @idPassport 
	 union all 
	select t.id,t.IDParentPassport, t.LevelOfAuthority
	from sysroPassports t join cte c on t.id = c.idparentpassport
	)
	select @levelOfAuthority =  (select top 1 levelofauthority from cte where not levelofauthority is null)

	IF @levelOfAuthority is null set @levelOfAuthority = 1

	RETURN @levelOfAuthority
END
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
			@RequestLevel int
	SELECT @LevelOfAuthority = dbo.GetPassportLevelOfAuthority(@idPassport)
	SELECT @featureAlias = CASE Requests.RequestType 
								WHEN 1 THEN 'Employees.UserFields.Requests' 
								WHEN 2 THEN 'Calendar.Punches.Requests.Forgotten'
								WHEN 3 THEN 'Calendar.Punches.Requests.Justify'
								WHEN 4 THEN 'Calendar.Punches.Requests.ExternalParts'
								WHEN 5 THEN 'Calendar.Requests.ShiftChange'
								WHEN 6 THEN 'Calendar.Requests.Vacations'
								WHEN 7 THEN 'Calendar.Requests.PlannedAbsence'
								WHEN 9 THEN 'Calendar.Requests.PlannedCause'
 								WHEN 10 THEN 'Tasks.Requests.Forgotten'
								ELSE 'Calendar.Requests.ShiftExchange' END,
		   @EmployeefeatureID = CASE Requests.RequestType
								WHEN 1 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Employees') 
								WHEN 2 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 3 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 4 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 5 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 6 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 7 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 9 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
 								WHEN 10 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Tasks')
								ELSE (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar') END,
		   @idEmployee = Requests.IDEmployee,
		   @RequestLevel = ISNULL(Requests.StatusLevel, 11)
   	FROM Requests
	WHERE Requests.ID = @idRequest
	
	/* Obtiene el número de niveles que hay entre el nivel del validador (@idPassport) y el nivel actual de la solicitud (@RequestLevel) */
	SELECT @LevelsBelow = 
		(SELECT COUNT( DISTINCT dbo.GetPassportLevelOfAuthority(Parents.ID))
		 FROM sysroPassports INNER JOIN sysroPassports Parents
		 ON sysroPassports.IDParentPassport = Parents.ID INNER JOIN 
		 (SELECT Id, dbo.GetPassportLevelOfAuthority(ID) AS value FROM sysroPassports) gpla
		 ON gpla.ID = Parents.id 
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
 			@idEmployee int
 	SELECT @FeatureAlias = CASE Requests.RequestType 
 								WHEN 1 THEN 'Employees.UserFields.Requests' 
 								WHEN 2 THEN 'Calendar.Punches.Requests.Forgotten'
 								WHEN 3 THEN 'Calendar.Punches.Requests.Justify'
 								WHEN 4 THEN 'Calendar.Punches.Requests.ExternalParts'
 								WHEN 5 THEN 'Calendar.Requests.ShiftChange'
 								WHEN 6 THEN 'Calendar.Requests.Vacations'
 								WHEN 7 THEN 'Calendar.Requests.PlannedAbsence'
 								WHEN 9 THEN 'Calendar.Requests.PlannedCause'
 								WHEN 10 THEN 'Tasks.Requests.Forgotten'
 								ELSE 'Calendar.Requests.ShiftExchange' END,
 		   @EmployeefeatureID = CASE Requests.RequestType 
 								WHEN 1 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Employees') 
 								WHEN 2 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
 								WHEN 3 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
 								WHEN 4 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
 								WHEN 5 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
 								WHEN 6 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
 								WHEN 7 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
 								WHEN 9 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
 								WHEN 10 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Tasks')
 								ELSE (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar') END,
 		   @idEmployee = Requests.IDEmployee
 	FROM Requests
 	WHERE Requests.ID = @idRequest
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
 	 WHERE sysroPassports.GroupType <> 'U' AND
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
  	WHERE sysroPassports.GroupType <> 'U' AND 
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
  			 WHERE sysroPassports.GroupType <> 'U' 
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
 
ALTER PROCEDURE [dbo].[WebLogin_Passports_Select] 
(
	@idPassport int
)
AS
   	SELECT ID,
   		IDParentPassport,
   		GroupType,
   		Name,
   		Description,
   		Email,
   		IDUser,
   		IDEmployee,
   		IDLanguage,
   		dbo.GetPassportLevelOfAuthority(@idPassport) AS LevelOfAuthority,
   		ConfData,
  		AuthenticationMerge,
  		StartDate,
  		ExpirationDate,
  		[State]
   	FROM sysroPassports
   	WHERE ID = @idPassport
   	
RETURN
GO
 
ALTER TABLE [dbo].[Tasks] ADD IDEmployeeUpdateStatus [int]  NULL
GO

ALTER TABLE [dbo].[Tasks] ADD Field1 [nvarchar](100) NULL , Field2 [nvarchar](100) NULL , Field3 [nvarchar](100) NULL , Field4 [numeric](18,6) NULL , Field5 [numeric](18,6) NULL , Field6 [numeric](18,6) NULL 
GO


CREATE TABLE [dbo].[FieldsTask](
	[IDTask] [int] NOT NULL,
	[IDField] [tinyint] NOT NULL,
 CONSTRAINT [PK_FieldsTask] PRIMARY KEY NONCLUSTERED 
(
	[IDTask] ASC,
	[IDField] ASC
) ON [PRIMARY]
) ON [PRIMARY] 
GO

CREATE TABLE [dbo].[FieldsTaskTemplate](
	[IDTaskTemplate] [int] NOT NULL,
	[IDField] [tinyint] NOT NULL,
 CONSTRAINT [PK_FieldsTaskTemplate] PRIMARY KEY NONCLUSTERED 
(
	[IDTaskTemplate] ASC,
	[IDField] ASC
) ON [PRIMARY]
) ON [PRIMARY] 
GO

CREATE TABLE [dbo].[FieldsProjectTemplate](
	[IDProjectTemplate] [int] NOT NULL,
	[IDField] [tinyint] NOT NULL,
 CONSTRAINT [PK_FieldsProjectTemplate] PRIMARY KEY NONCLUSTERED 
(
	[IDProjectTemplate] ASC,
	[IDField] ASC
) ON [PRIMARY]
) ON [PRIMARY] 
GO



ALTER VIEW [dbo].[sysroTasksCube]
 AS
SELECT     dbo.sysroEmployeeGroups.IDGroup, dbo.sysroEmployeeGroups.FullGroupName AS GroupName, dbo.Employees.ID AS IDEmployee, 
                      dbo.Employees.Name AS EmployeeName, DATEPART(year, dbo.DailyTaskAccruals.Date) AS Date_Year, DATEPART(month, dbo.DailyTaskAccruals.Date) 
                      AS Date_Month, DATEPART(day, dbo.DailyTaskAccruals.Date) AS Date_Day, dbo.DailyTaskAccruals.Date, dbo.BusinessCenters.ID AS IDCenter, 
                      dbo.BusinessCenters.Name AS CenterName, dbo.Tasks.ID AS IDTask, dbo.Tasks.Name AS TaskName, 
                      ISNULL(dbo.Tasks.Field1,'') as Field1_Task,
                      ISNULL(dbo.Tasks.Field2,'') as Field2_Task,
                      ISNULL(dbo.Tasks.Field3,'') as Field3_Task,
                      ISNULL(CONVERT(numeric(25, 6),dbo.Tasks.Field4),0) as Field4_Task,
                      ISNULL(CONVERT(numeric(25, 6),dbo.Tasks.Field5),0) as Field5_Task,
                      ISNULL(CONVERT(numeric(25, 6),dbo.Tasks.Field6),0) as Field6_Task,
                      ISNULL(dbo.DailyTaskAccruals.Field1,'') as Field1_Total,
                      ISNULL(dbo.DailyTaskAccruals.Field2,'') as Field2_Total,
                      ISNULL(dbo.DailyTaskAccruals.Field3,'') as Field3_Total,
                      ISNULL(CONVERT(numeric(25, 6),dbo.DailyTaskAccruals.Field4),0) as Field4_Total,
                      ISNULL(CONVERT(numeric(25, 6),dbo.DailyTaskAccruals.Field5),0) as Field5_Total,
                      ISNULL(CONVERT(numeric(25, 6),dbo.DailyTaskAccruals.Field6),0) as Field6_Total,
                      ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Value), 0) AS Value, 
                      ISNULL(dbo.Tasks.Project, '') AS Project, ISNULL(dbo.Tasks.Tag, '') AS Tag, dbo.Tasks.IDPassport, 
		      case when Tasks.Status = 0 then 'Activa' ELSE CASE WHEN Tasks.Status = 1 THEN 'Finalizada' ELSE CASE WHEN Tasks.Status = 2 then 'Cancelada' ELSE 'Pendiente' END END END as Estado
FROM         dbo.Employees INNER JOIN
                      dbo.DailyTaskAccruals ON dbo.DailyTaskAccruals.IDEmployee = dbo.Employees.ID INNER JOIN
                      dbo.Tasks ON dbo.Tasks.ID = dbo.DailyTaskAccruals.IDTask INNER JOIN
                      dbo.sysroEmployeeGroups ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyTaskAccruals.IDEmployee INNER JOIN
                      dbo.BusinessCenters ON dbo.Tasks.IDCenter = dbo.BusinessCenters.ID
WHERE     (dbo.DailyTaskAccruals.Date BETWEEN dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate)
GO

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (25700 ,25 ,'Tasks.FieldsDefinition' ,'Definicion de la ficha' ,'' ,'U' ,'RWA' ,NULL)
GO


INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] (IDPassport, IDFeature, Permission)
SELECT sysroPassports.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM sysroFeatures, sysroPassports		
WHERE sysroFeatures.ID IN (25700) AND
	  sysroPassports.IDParentPassport IS NULL AND sysroPassports.GroupType = 'U'
GO

ALTER TABLE [dbo].[Tasks] ADD [UpdateStatusDate] [smalldatetime] NULL
GO

UPDATE [dbo].[sysroGUI] Set LanguageReference='ProductiV' WHERE IDPath = 'Portal\Task'
GO

UPDATE [dbo].[sysroGUI] Set LanguageReference='Task.Status' WHERE IDPath = 'Portal\Task\Tasks'
GO

DROP VIEW [dbo].[sysroCurrentTasksStatus]
GO

CREATE VIEW [dbo].[sysroCurrentTasksStatus]
AS
SELECT     dbo.Tasks.ID, dbo.BusinessCenters.Name AS Center, dbo.BusinessCenters.ID AS IDCenter, dbo.Tasks.Name, dbo.Tasks.Status, dbo.Tasks.Color, dbo.Tasks.Project, 
                      dbo.Tasks.Tag, dbo.Tasks.Priority, dbo.Tasks.ActivationTask, Tasks_1.Color AS ColorTaskPrev, CONVERT(numeric(25, 4), ISNULL(dbo.Tasks.InitialTime, 0) 
                      + ISNULL(dbo.Tasks.TimeChangedRequirements, 0) + ISNULL(dbo.Tasks.ForecastErrorTime, 0) + ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) 
                      + ISNULL(dbo.Tasks.EmployeeTime, 0) + ISNULL(dbo.Tasks.TeamTime, 0) + ISNULL(dbo.Tasks.MaterialTime, 0) + ISNULL(dbo.Tasks.OtherTime, 0)) AS Duration,
                          (SELECT     ISNULL(CONVERT(numeric(25, 4), SUM(Value)), 0) AS Expr1
                            FROM          dbo.DailyTaskAccruals
                            WHERE      (IDTask = dbo.Tasks.ID)) AS Worked, CASE WHEN CONVERT(numeric(25, 4), isnull(dbo.Tasks.InitialTime, 0) 
                      + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0) + isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) 
                      + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0) + isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) 
                      > 0 THEN CASE WHEN
                          (SELECT     isnull(CONVERT(numeric(25, 4), SUM(Value)), 0)
                            FROM          DailyTaskAccruals
                            WHERE      DailyTaskAccruals.IDTask = dbo.Tasks.ID) * 100 / CONVERT(numeric(25, 4), isnull(dbo.Tasks.InitialTime, 0) 
                      + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0) + isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) 
                      + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0) + isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) 
                      > 100.00 THEN 100 ELSE
                          (SELECT     isnull(CONVERT(numeric(25, 4), SUM(Value)), 0)
                            FROM          DailyTaskAccruals
                            WHERE      DailyTaskAccruals.IDTask = dbo.Tasks.ID) * 100 / CONVERT(numeric(25, 4), isnull(dbo.Tasks.InitialTime, 0) 
                      + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0) + isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) 
                      + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0) + isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) 
                      END ELSE 0 END AS Progress, CASE WHEN CONVERT(numeric(25, 4), isnull(dbo.Tasks.InitialTime, 0) + isnull(dbo.Tasks.TimeChangedRequirements, 0) 
                      + isnull(dbo.Tasks.ForecastErrorTime, 0) + isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0) 
                      + isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) > 0 THEN CASE WHEN
                          (SELECT     isnull(CONVERT(numeric(25, 4), SUM(Value)), 0)
                            FROM          DailyTaskAccruals
                            WHERE      DailyTaskAccruals.IDTask = dbo.Tasks.ID) * 100 / CONVERT(numeric(25, 4), isnull(dbo.Tasks.InitialTime, 0) 
                      + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0) + isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) 
                      + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0) + isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) 
                      > 100.00 THEN 1 ELSE 0 END ELSE 0 END AS Exceeded, dbo.Tasks.IDPassport, dbo.Tasks.ExpectedStartDate, dbo.Tasks.ExpectedEndDate, 
                      Tasks_1.Name AS NameTaskPrev, dbo.Tasks.ShortName, CONVERT(nvarchar(200),
                          (SELECT     COUNT(DISTINCT LastPunch.IDEmployee) AS Expr1
                            FROM          (SELECT     ID AS IDEmployee,
                                                                               (SELECT     TOP (1) TypeData
                                                                                 FROM          dbo.Punches
                                                                                 WHERE      (ActualType = 4) AND (IDEmployee = dbo.Employees.ID)
                                                                                 ORDER BY DateTime DESC) AS IDTask
                                                    FROM          dbo.Employees) AS LastPunch INNER JOIN
                                                   dbo.EmployeeStatus ON LastPunch.IDEmployee = dbo.EmployeeStatus.IDEmployee
                            WHERE      (LastPunch.IDTask = dbo.Tasks.ID) AND (dbo.EmployeeStatus.LastPunch >= DATEADD(day, - 4, GETDATE())))) + '@' + CONVERT(nvarchar(200),
                          (SELECT     COUNT(DISTINCT LastPunch_2.IDEmployee) AS Expr1
                            FROM          (SELECT     ID AS IDEmployee,
                                                                               (SELECT     TOP (1) DateTime
                                                                                 FROM          dbo.Punches AS Punches_2
                                                                                 WHERE      (ActualType = 4) AND (IDEmployee = Employees_2.ID) AND (TypeData = dbo.Tasks.ID)
                                                                                 ORDER BY DateTime DESC) AS PunchDate
                                                    FROM          dbo.Employees AS Employees_2) AS LastPunch_2 INNER JOIN
                                                   dbo.EmployeeStatus AS EmployeeStatus_2 ON LastPunch_2.IDEmployee = EmployeeStatus_2.IDEmployee
                            WHERE      (LastPunch_2.PunchDate > DATEADD(day, - 7, GETDATE())) AND (LastPunch_2.PunchDate IS NOT NULL) AND (LastPunch_2.IDEmployee NOT IN
                                                       (SELECT DISTINCT LastPunch_1.IDEmployee
                                                         FROM          (SELECT     ID AS IDEmployee,
                                                                                                            (SELECT     TOP (1) TypeData
                                                                                                              FROM          dbo.Punches AS Punches_1
                                                                                                              WHERE      (ActualType = 4) AND (IDEmployee = Employees_1.ID)
                                                                                                              ORDER BY DateTime DESC) AS IDTask
                                                                                 FROM          dbo.Employees AS Employees_1) AS LastPunch_1 INNER JOIN
                                                                                dbo.EmployeeStatus AS EmployeeStatus_1 ON LastPunch_1.IDEmployee = EmployeeStatus_1.IDEmployee
                                                         WHERE      (LastPunch_1.IDTask = dbo.Tasks.ID) AND (EmployeeStatus_1.LastPunch >= DATEADD(day, - 4, GETDATE())))))) AS Employees
FROM         dbo.Tasks LEFT OUTER JOIN
                      dbo.Tasks AS Tasks_1 ON dbo.Tasks.ActivationTask = Tasks_1.ID INNER JOIN
                      dbo.BusinessCenters ON dbo.Tasks.IDCenter = dbo.BusinessCenters.ID
GO


  ALTER PROCEDURE [dbo].[EmployeesAuthorized]
  (
    	@employeesWhere nvarchar(4000),
   		@date nvarchar(15) /*yyyy-MM-dd*/
  )
  AS
  BEGIN
  	CREATE TABLE #EmployeesTable
    	( 			
  		IDActivity nvarchar(10) NOT NULL,
  		IDGroup nvarchar(10) NOT NULL,
  		[Path] nvarchar(50) NOT NULL,
  		IDEmployee int NOT NULL
  	)
  	CREATE TABLE #DocumentsTable
  	(			
  		[Name] nvarchar(50) NOT NULL,
  		[Description] nvarchar(4000) NULL, 
  		ValidityFrom smalldatetime NULL, 
  		ValidityTo smalldatetime NULL, 
  		AccessValidation smallint NOT NULL,
  		Expired bit NOT NULL
  	)
  	CREATE TABLE #EmployeesAuthorized
  	(
  		IDCompany nvarchar(500) NOT NULL,
  		CompanyName nvarchar(500),
  		IDEmployee int NOT NULL,
  		EmployeeName nvarchar(500)
  	)
  	DECLARE @sql nvarchar(4000)
  	DECLARE @newDate smalldatetime
    		if (charindex('{',@employeesWhere) > 0)
    		begin
    			SELECT @employeesWhere = replace(@employeesWhere,'{','')
    		end
    		
    		if (charindex('}',@employeesWhere) > 0)
    		begin
    			SELECT @employeesWhere = replace(@employeesWhere,'}','')
    		end
    		if (charindex('[',@employeesWhere) > 0)
    		begin
    			SELECT @employeesWhere = replace(@employeesWhere,'[','(')
    		end
    		if (charindex(']',@employeesWhere) > 0)
    		begin
    			SELECT @employeesWhere = replace(@employeesWhere,']',')')
    		end
    		if (charindex('startswith',@employeesWhere) > 0)
    		begin
    			SELECT @employeesWhere = replace(@employeesWhere,'startswith','Like')
    		end
    		if (charindex('("',@employeesWhere) > 0) 
    		begin
    			SELECT @employeesWhere = replace(@employeesWhere,'("','(''')
    		end
    		if (charindex('")',@employeesWhere) > 0)
    		begin
    			SELECT @employeesWhere = replace(@employeesWhere,'")','%'')')
    		end
    		if (charindex('","',@employeesWhere) > 0)
    		begin
    			SELECT @employeesWhere = replace(@employeesWhere,'","','%'') or sysrovwAllEmployeeGroups.Path	like (''')
    		end
  		if (charindex('EndDate < DateTime (2079, 01, 01, 00, 00, 00)',@employeesWhere) > 0)
    		begin
    			SELECT @employeesWhere = replace(@employeesWhere,'EndDate < DateTime (2079, 01, 01, 00, 00, 00)','EndDate < convert(smalldatetime,''2079-01-01 00:00:00'',120)')
    		end
    		if (charindex('BeginDate > DateTime',@employeesWhere) > 0)
    		begin
    			SELECT @employeesWhere = replace(@employeesWhere,'BeginDate > DateTime','BeginDate > getdate())))--')
    		end
  	
  	--Seteamos las variables
  	DELETE FROM #EmployeesTable
  	DELETE FROM #EmployeesAuthorized
  	SELECT @newDate =  CONVERT(smalldatetime, @Date ,120)
  	SET @sql = 'INSERT INTO #EmployeesTable SELECT dbo.sysroActivityCompanies.IDActivity, dbo.sysroActivityCompanies.CompanyID, dbo.sysroActivityCompanies.Path, dbo.sysrovwAllEmployeeGroups.IDEmployee
  				FROM dbo.sysroActivityCompanies 
  					INNER JOIN dbo.sysrovwAllEmployeeGroups ON dbo.sysroActivityCompanies.CompanyID = dbo.sysrovwAllEmployeeGroups.IDCompany
  					INNER JOIN dbo.Employees ON Employees.ID = dbo.sysrovwAllEmployeeGroups.IDEmployee
  					INNER JOIN dbo.EmployeeContracts ON dbo.EmployeeContracts.IDEmployee = dbo.Employees.ID
  				WHERE Employees.RiskControlled = 1 
  					AND dbo.EmployeeContracts.BeginDate <= CONVERT(smalldatetime,''' + @Date + ''',120)
  					AND dbo.EmployeeContracts.EndDate >= CONVERT(smalldatetime,''' + @Date + ''',120)
  					AND dbo.sysrovwAllEmployeeGroups.BeginDate <= CONVERT(smalldatetime,''' + @Date + ''',120)
  					AND dbo.sysrovwAllEmployeeGroups.EndDate >= CONVERT(smalldatetime,''' + @Date + ''',120)'
  	
  	--Añadimos el filtro de empleados a la consulta
  	SELECT @sql = @sql + ' AND ' + @employeesWhere 
  	EXECUTE (@sql)
  	
  	--Para cada empleado comprobamos si tiene todos los documentos en regla
  	DECLARE @idCompany nvarchar(10), @companyName nvarchar(500), @idEmployee int, @employeeName nvarchar(500), @expired bit
  	DECLARE @idActivity nvarchar(10), @companyPath nvarchar(1000), @count int, @expiredDocuments int
  	DECLARE Employees CURSOR FOR SELECT * FROM #EmployeesTable				
  	OPEN Employees
  		SET @expired = 0
  		FETCH NEXT FROM Employees
    		INTO @idActivity, @idCompany, @companyPath, @idEmployee
    		WHILE @@FETCH_STATUS = 0 
    			BEGIN
  				-- Obtenemos los documentos del empleado
  				DELETE FROM #DocumentsTable
    				INSERT INTO #DocumentsTable
    				EXEC dbo.EmployeeDocuments @idEmployee, @newDate
  				SELECT @expired = COUNT(*) FROM #DocumentsTable WHERE #DocumentsTable.Expired = 1 AND #DocumentsTable.AccessValidation = 1	
  				
  				--Insertamos al empleado que lo tiene todo en regla
  				IF @expired = 0
  				BEGIN
  					--Miramos si tiene algun documento de empresa caducado
  					DECLARE @idSubCompany int
    					DECLARE CompaniesCursor CURSOR FOR SELECT [Value] FROM dbo.SplitInt(@companyPath, '\')
  					SELECT @expiredDocuments = 0
    					OPEN CompaniesCursor
  						FETCH NEXT FROM CompaniesCursor
    						INTO @idSubCompany
    						WHILE @@FETCH_STATUS = 0 
    						BEGIN
  							DELETE FROM #DocumentsTable
    							INSERT INTO #DocumentsTable
  							EXEC dbo.CompanyDocuments @idSubCompany, @newDate
    							SELECT @expired = COUNT(*) FROM #DocumentsTable WHERE #DocumentsTable.Expired = 1 AND #DocumentsTable.AccessValidation = 1	
  							
  							--Insertamos al empleado que lo tiene todo en regla
  							IF @expired > 0
  							BEGIN
  								SELECT @expiredDocuments = 1
    							END
  											
    							FETCH NEXT FROM CompaniesCursor 
    							INTO @idSubCompany
    						END
  					--SELECT  @count = count(*) from #EmployeesAuthorized where IDEmployee = @idEmployee
  					IF (@expiredDocuments = 0)
  					BEGIN
  						INSERT INTO #EmployeesAuthorized
  						SELECT @idCompany, dbo.sysroActivityCompanies.GroupName, @idEmployee, Name
  						FROM dbo.sysrovwAllEmployeeGroups
  							INNER JOIN dbo.sysroActivityCompanies ON dbo.sysroActivityCompanies.CompanyID = dbo.sysrovwAllEmployeeGroups.IDCompany
  							INNER JOIN dbo.Employees ON dbo.Employees.ID = dbo.sysrovwAllEmployeeGroups.IDEmployee
  						WHERE Employees.ID = @idEmployee
  					END
    					CLOSE CompaniesCursor
    					DEALLOCATE CompaniesCursor
  				END
  				FETCH NEXT FROM Employees
  				INTO @idActivity, @idCompany, @companyPath, @idEmployee
  			END
  	CLOSE Employees
    	DEALLOCATE Employees
  	SELECT * FROM #EmployeesAuthorized
  END
  GO
  

--CAMBIOS HORARIOS DE EXPRESS A PRO
UPDATE Shifts
SET              ShiftType = 1, IsFloating = 0, TypeShift = NULL
WHERE     (ISNULL(TypeShift, N'') <> N'') AND (ShiftType IS NULL) AND (ID > 0) AND (IsObsolete = 0) AND (IsTemplate = 0)
GO

--Indice para la consulta de estado de tareas
CREATE NONCLUSTERED INDEX [Punches_28_1908917872__K5_K3_K7_K10] ON [dbo].[Punches]
(
      [ActualType] ASC,
      [IDEmployee] ASC,
      [TypeData] ASC,
      [DateTime] ASC
) ON [PRIMARY]
GO



  -- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='303' WHERE ID='DBVersion'
GO