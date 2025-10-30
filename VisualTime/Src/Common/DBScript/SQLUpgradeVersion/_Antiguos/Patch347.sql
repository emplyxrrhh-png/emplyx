alter table dbo.punches add TimeStamp smalldatetime
GO

ALTER FUNCTION [dbo].[GetEmployeeGroupChilds] 
 	(
 		@idEmployeeGroup int
 	)
 RETURNS @ValueTable table(ID int)
 AS
 	BEGIN
 		DECLARE @Path nvarchar(100)
 		SELECT @Path = Path
 		FROM Groups
 		WHERE ID = @idEmployeeGroup
 	
 		INSERT INTO @ValueTable
 			SELECT ID
 			FROM Groups
 			WHERE Path LIKE @Path + '\%'
 			ORDER BY ID
 		
 	RETURN
 	END
GO

ALTER TABLE dbo.sysroUserFields ADD
	isSystem bit NOT NULL CONSTRAINT DF_sysroUserFields_isSystem DEFAULT 0,
	Alias nvarchar(50) NULL
GO

-- Nueva funcionalidad de centros de coste
ALTER TABLE dbo.TerminalReaders ADD
	IDCostCenter int NULL
GO

INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,69,'CO',1,'Fast','X','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO
INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,70,'TACO',1,'Fast','X','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO

INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('rxC',1,4,'CO',1,'Blind','E,S,X','Local','1,0','0','0','1,0','0','Remote',1,'1','')
GO
INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('rxC',1,5,'TACO',1,'Blind','E,S,X','Local','1,0','0','0','1,0','0','Remote',1,'1','')
GO

INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('rxF',1,4,'CO',1,'Blind','E,S,X','Local','1,0','0','0','1,0','0','Remote',1,'1','')
GO
INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('rxF',1,5,'TACO',1,'Blind','E,S,X','Local','1,0','0','0','1,0','0','Remote',1,'1','')
GO

ALTER TABLE [dbo].[Groups] ADD [IDCenter] [smallint] NULL 
GO

ALTER TABLE [dbo].[DailyIncidences] ADD [IDCenter] [smallint] NULL DEFAULT(0)
GO
UPDATE [dbo].[DailyIncidences] Set IDCenter = 0 WHERE IDCenter IS NULL
GO


ALTER TABLE dbo.DailyCauses DROP CONSTRAINT [PK_DailyCauses]
GO

ALTER TABLE [dbo].[DailyCauses] ADD [IDCenter] [smallint] NOT  NULL DEFAULT(0)
GO

UPDATE [dbo].[DailyCauses] Set IDCenter = 0 WHERE IDCenter IS NULL
GO


ALTER TABLE [dbo].[DailyCauses] ADD 
	CONSTRAINT [PK_DailyCauses] PRIMARY KEY  NONCLUSTERED 
	(
	  [IDEmployee],
	  [Date],
	  [IDRelatedIncidence],
	  [IDCause],
	  [IDCenter],
	  [AccrualsRules]
	)  WITH FILLFACTOR = 90 ON [PRIMARY] 
GO

ALTER TABLE [dbo].[DailyIncidences] ADD [DefaultCenter] [bit] NULL DEFAULT(1)
GO
UPDATE [dbo].[DailyIncidences] Set DefaultCenter = 1 WHERE DefaultCenter IS NULL
GO


ALTER TABLE [dbo].[DailyCauses] ADD  [DefaultCenter] [bit] NULL DEFAULT(1)
GO
UPDATE [dbo].[DailyCauses] Set  DefaultCenter = 1 WHERE DefaultCenter IS NULL
GO
ALTER TABLE [dbo].[DailyCauses] ADD
	[ManualCenter] [bit] NULL DEFAULT(0)
GO
UPDATE [dbo].[DailyCauses] SET ManualCenter=0 WHERE ManualCenter IS NULL
GO


ALTER TABLE [dbo].[Shifts] ADD [IDCenter] [smallint] NULL DEFAULT(0)
GO
UPDATE [dbo].[Shifts] Set IDCenter = 0 WHERE IDCenter IS NULL
GO

ALTER TABLE [dbo].[Shifts] ADD [ApplyCenterOnAbsence] [bit] NULL DEFAULT(0)
GO
UPDATE [dbo].[Shifts] Set ApplyCenterOnAbsence = 0 WHERE ApplyCenterOnAbsence IS NULL
GO


-- ELIMINAMOS LA FEATURE DE CENTROS DE COSTE
DELETE FROM sysroFeatures WHERE ID=25400
GO

-- CREAMOS LA NUEVAS FEATURES
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (12000 ,Null ,'BusinessCenters' ,'Centros de Coste' ,'' ,'U' ,'RWA' ,NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (12100 ,12000 ,'BusinessCenters.Definition' ,'Definicion Centros de coste' ,'' ,'U' ,'RWA' ,NULL)
GO

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
  VALUES (1590 ,1 ,'Employees.BusinessCenters' ,'Asignar centros' ,'' ,'U' ,'RWA' ,NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (12200 ,12000 ,'BusinessCenters.Punches' ,'Edición de fichajes' ,'' ,'U' ,'RW' ,NULL)
GO

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (12300 ,12000 ,'BusinessCenters.Analytics' ,'Analítica de costes' ,'' ,'U' ,'R' ,NULL)
GO


-- ACTUALIZAMOS PERMISOS
INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT sysroPassports.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM sysroFeatures, sysroPassports		
WHERE sysroFeatures.ID IN (12000, 12100,12200,12300, 1590) AND
	  sysroPassports.IDParentPassport IS NULL AND sysroPassports.GroupType = 'U'
GO

EXEC dbo.sysro_GenerateAllPermissionsOverFeatures
GO


-- ELIMINAMOS EL REGISTRO ACTUAL DE CENTROS DE COSTE
DELETE FROM sysroGUI WHERE IDPath='Portal\Task\BusinessCenters'
GO


-- CREAMOS EL NUEVO REGISTRO DENTRO DE GUI
update dbo.sysrogui Set Priority = Priority + 1 where IDPath like 'Portal\OHP'
GO
update dbo.sysrogui Set Priority = Priority + 1 where IDPath like 'Portal\DiningRoom'
GO

update dbo.sysrogui Set Priority = Priority + 1 where IDPath like 'Portal\Security'
GO

update dbo.sysrogui Set Priority = Priority + 1 where IDPath like 'Portal\Configuration'
GO

INSERT INTO dbo.sysroGUI (IDPath,LanguageReference,URL,IconURL,[Type], [Parameters], RequiredFeatures, SecurityFlags, Priority,AllowedSecurity,RequiredFunctionalities)
VALUES('Portal\CostControl', 'CostControl', null ,	'BusinessCenters.png'	,NULL	,NULL	,NULL,NULL	,1007	,NULL	,'U:Tasks.Definition=Read OR U:BusinessCenters.Definition=Read')
GO

INSERT INTO dbo.sysroGUI VALUES('Portal\CostControl\Analytics','CostControlAnalytics','Scheduler/AnalyticsCostControl.aspx','CostAnalytics.png',NULL,NULL,'Feature\CostControl',NULL,102,NULL,'U:BusinessCenters.Definition=Read') 
GO


INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,[Type], [Parameters], RequiredFeatures, SecurityFlags, Priority,AllowedSecurity,RequiredFunctionalities)
VALUES('Portal\CostControl\BusinessCenters', 'BusinessCenters', 'Tasks/BusinessCenters.aspx',	'BusinessCenters.png'	,NULL	,NULL	,NULL,NULL	,101	,NULL	,'U:Tasks.Definition=Read OR U:BusinessCenters.Definition=Read')
GO

-- RELACION EMPLEADOS Y CENTROS DE COSTE
CREATE TABLE [dbo].[EmployeeCenters](
	[IDEmployee] [int] NOT NULL,
	[IDCenter] [smallint]NOT NULL,
 CONSTRAINT [PK_EmployeeCenters] PRIMARY KEY NONCLUSTERED 
(
	[IDEmployee] ASC,
	[IDCenter] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Causes] ADD
	[CostFactor] [numeric](8, 6) NULL DEFAULT(1)
GO

UPDATE [dbo].[Causes] SET CostFactor= 1 WHERE CostFactor IS NULL
GO

CREATE VIEW [dbo].[sysroCostControlCube]
AS
SELECT     CONVERT(varchar(10), dbo.DailyCauses.IDEmployee, 120) + '-' + CAST(dbo.EmployeeGroups.IDGroup AS varchar) 
                      + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailyCauses.Date, 120) AS KeyView, dbo.EmployeeGroups.IDGroup, 
                      dbo.EmployeeContracts.IDContract, dbo.DailyCauses.IDEmployee, dbo.DailyCauses.Date,
                      dbo.Causes.Name AS CauseName, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor, isnull(dbo.BusinessCenters.ID, 0) as IDCenter, isnull(dbo.BusinessCenters.Name, '') AS CenterName , dbo.DailyCauses.Value AS Value, YEAR(dbo.DailyCauses.Date) AS Año, 
                      (DATEPART(dw, dbo.DailyCauses.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailyCauses.Date) AS WeekOfYear, DATEPART(dy, 
                      dbo.DailyCauses.Date) AS DayOfYear, dbo.DailyCauses.DefaultCenter, dbo.DailyCauses.ManualCenter , 
                      dbo.Employees.Name AS EmployeeName, dbo.Groups.Name AS GroupName, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName, dbo.Groups.Path, 
                      dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee AS CurrentEmployee, dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroCostHour'),DailyCauses.date ) as Cost, dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPriceHour'), DailyCauses.date) as PVP
FROM         dbo.EmployeeGroups INNER JOIN
                      dbo.Causes INNER JOIN
                      dbo.DailyCauses ON dbo.Causes.ID = dbo.DailyCauses.IDCause 
                      ON dbo.EmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.EmployeeGroups.BeginDate <= dbo.DailyCauses.Date AND 
                      dbo.EmployeeGroups.EndDate >= dbo.DailyCauses.Date INNER JOIN
                      dbo.EmployeeContracts ON dbo.DailyCauses.IDEmployee = dbo.EmployeeContracts.IDEmployee AND 
                      dbo.DailyCauses.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyCauses.Date <= dbo.EmployeeContracts.EndDate INNER JOIN
                      dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                      dbo.sysrosubvwCurrentEmployeePeriod ON dbo.DailyCauses.IDEmployee = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee LEFT OUTER JOIN
                      dbo.Employees ON dbo.DailyCauses.IDEmployee = dbo.Employees.ID  LEFT OUTER JOIN
                      dbo.BusinessCenters ON dbo.DailyCauses.IDCenter = dbo.BusinessCenters.ID 
GO

ALTER TABLE dbo.EmployeeTerminalMessages ADD [ID] int IDENTITY(1,1) NOT NULL
GO

UPDATE sysroParameters SET Data='347' WHERE ID='DBVersion'
GO


