
-- Modificaciones de Visits 
ALTER TABLE dbo.Visit_Fields ADD
	[values] nvarchar(MAX) NULL
GO
ALTER TABLE dbo.Visitor_Fields ADD
	[values] nvarchar(MAX) NULL
GO
ALTER TABLE dbo.Visit ADD
	[CloneEvery] nvarchar(100) NULL
GO

UPDATE [dbo].[sysroFeatures]
SET [EmployeeFeatureID]=31,
[AppHasPermissionsOverEmployees]=1
WHERE [ID]=31
GO
INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(31010,31,'Visits.Create','Crear visitas','','U','WA',0,null,31)
GO
INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(31020,31,'Visits.Manage','Gestionar visitas','','U','WA',0,null,31)
GO
INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(31030,31,'Visits.UserFields','Crear campos personalizados','','U','WA',0,null,31)
GO

INSERT INTO [dbo].[sysroLiveAdvancedParameters](ParameterName,Value)
VALUES ('vis_notifications','2')
GO
INSERT INTO [dbo].[sysroLiveAdvancedParameters](ParameterName,Value)
VALUES ('vis_cardidfield','')
GO

alter table dbo.EmployeeJobMoves ADD checked_in bit not null default 0 
GO
alter table dbo.EmployeeJobMoves ADD checked_out bit not null default 0 
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES
           (2800,2,'Calendar.Accruals','Consulta de saldos de presencia','','U','R',NULL,NULL,2)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2800, 3  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2
GO

INSERT INTO [dbo].sysroFeatures([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
VALUES (28002,28,'TaskPunches.Complete','Completar','','E','W',Null,null,null)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 28002, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 28
GO

ALTER TABLE dbo.BusinessCenters ADD
	Field5 nvarchar(MAX) NULL
GO

INSERT INTO [dbo].[sysroFieldsBusinessCenters]([ID],[Name]) VALUES(5,'Campo 5')
GO

 ALTER PROCEDURE [dbo].[Analytics_CostCenters]
 	@initialDate smalldatetime,
 	@endDate smalldatetime,
 	@idpassport int,
 	@userFieldsFilter nvarchar(max)
  AS
   SELECT     CONVERT(varchar(10), dbo.DailyCauses.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
                         + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailyCauses.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup, 
                         dbo.EmployeeContracts.IDContract, dbo.DailyCauses.IDEmployee, dbo.DailyCauses.Date,
                         dbo.Causes.Name AS CauseName, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor, isnull(dbo.BusinessCenters.ID, 0) as IDCenter, 
 						isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,  
 						isnull(dbo.BusinessCenters.Field4, '') AS Field4, isnull(dbo.BusinessCenters.Field5, '') AS Field5, dbo.DailyCauses.Value AS Value, YEAR(dbo.DailyCauses.Date) AS Año, 
                         (DATEPART(dw, dbo.DailyCauses.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailyCauses.Date) AS WeekOfYear, DATEPART(dy, 
                         dbo.DailyCauses.Date) AS DayOfYear, dbo.DailyCauses.DefaultCenter, dbo.DailyCauses.ManualCenter , 
                         dbo.Employees.Name AS EmployeeName, dbo.Groups.Name AS GroupName, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName, dbo.Groups.Path, 
                         dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee, 
 						dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroCostHour'),DailyCauses.date ) as Cost, 
 						dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPriceHour'), DailyCauses.date) as PVP,
 					    dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,
 						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
 						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
 						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
 						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
 						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),@endDate) As UserField1,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),@endDate) As UserField2,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),@endDate) As UserField3,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),@endDate) As UserField4,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),@endDate) As UserField5,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),@endDate) As UserField6,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),@endDate) As UserField7,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),@endDate) As UserField8,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),@endDate) As UserField9,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),@endDate) As UserField10
   FROM         dbo.sysroEmployeeGroups 
                         INNER JOIN dbo.Causes 
                         INNER JOIN dbo.DailyCauses ON dbo.Causes.ID = dbo.DailyCauses.IDCause  ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.DailyCauses.Date between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate 
 						INNER JOIN dbo.EmployeeContracts ON dbo.DailyCauses.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyCauses.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyCauses.Date <= dbo.EmployeeContracts.EndDate 
 						INNER JOIN dbo.Groups ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID 
 						LEFT OUTER JOIN dbo.Employees ON dbo.DailyCauses.IDEmployee = dbo.Employees.ID  
 						LEFT OUTER JOIN dbo.BusinessCenters ON dbo.DailyCauses.IDCenter = dbo.BusinessCenters.ID 
    where     (dbo.businesscenters.id is null or dbo.businesscenters.id in (select distinct idcenter from sysropassports_centers where idpassport=@idpassport) )
 			 and dbo.dailycauses.date between @initialdate and @enddate
GO

 ALTER PROCEDURE [dbo].[Analytics_Tasks]
 	@initialDate smalldatetime,
 	@endDate smalldatetime,
 	@idpassport int,
 	@userFieldsFilter nvarchar(max)
  AS

   SELECT     dbo.sysroEmployeeGroups.IDGroup, dbo.sysroEmployeeGroups.FullGroupName AS GroupName, dbo.Employees.ID AS IDEmployee, 
                         dbo.Employees.Name AS EmployeeName, DATEPART(year, dbo.DailyTaskAccruals.Date) AS Date_Year, DATEPART(month, dbo.DailyTaskAccruals.Date) 
                         AS Date_Month, DATEPART(day, dbo.DailyTaskAccruals.Date) AS Date_Day, dbo.DailyTaskAccruals.Date, dbo.BusinessCenters.ID AS IDCenter, 
                         dbo.BusinessCenters.Name AS CenterName, dbo.Tasks.ID AS IDTask, dbo.Tasks.Name AS TaskName, dbo.Tasks.InitialTime, ISNULL(dbo.Tasks.Field1, '') 
                         AS Field1_Task, ISNULL(dbo.Tasks.Field2, '') AS Field2_Task, ISNULL(dbo.Tasks.Field3, '') AS Field3_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field4), 0) 
                         AS Field4_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field5), 0) AS Field5_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field6), 0) AS Field6_Task, 
                         ISNULL(dbo.DailyTaskAccruals.Field1, '') AS Field1_Total, ISNULL(dbo.DailyTaskAccruals.Field2, '') AS Field2_Total, ISNULL(dbo.DailyTaskAccruals.Field3, '') 
                         AS Field3_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field4), 0) AS Field4_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field5), 
                         0) AS Field5_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field6), 0) AS Field6_Total, ISNULL(CONVERT(numeric(25, 6), 
                         dbo.DailyTaskAccruals.Value), 0) AS Value, ISNULL(dbo.Tasks.Project, '') AS Project, ISNULL(dbo.Tasks.Tag, '') AS Tag, dbo.Tasks.IDPassport, 
                         ISNULL(dbo.Tasks.TimeChangedRequirements, 0) AS TimeChangedRequirements, ISNULL(dbo.Tasks.ForecastErrorTime, 0) AS ForecastErrorTime, 
                         ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) AS NonProductiveTimeIncidence, ISNULL(dbo.Tasks.EmployeeTime, 0) AS EmployeeTime, 
                         ISNULL(dbo.Tasks.TeamTime, 0) AS TeamTime, ISNULL(dbo.Tasks.MaterialTime, 0) AS MaterialTime, ISNULL(dbo.Tasks.OtherTime, 0) AS OtherTime, 
                         ISNULL(dbo.Tasks.InitialTime, 0) + ISNULL(dbo.Tasks.TimeChangedRequirements, 0) + ISNULL(dbo.Tasks.ForecastErrorTime, 0) 
                         + ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) + ISNULL(dbo.Tasks.EmployeeTime, 0) + ISNULL(dbo.Tasks.TeamTime, 0) + ISNULL(dbo.Tasks.MaterialTime, 0) 
                         + ISNULL(dbo.Tasks.OtherTime, 0) AS Duration, 
                         CASE WHEN Tasks.Status = 0 THEN 'Activa' ELSE CASE WHEN Tasks.Status = 1 THEN 'Finalizada' ELSE CASE WHEN Tasks.Status = 2 THEN 'Cancelada' ELSE 'Pendiente'
                          END END END AS Estado, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
 					    dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,
 						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
 						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
 						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
 						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
 						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),@endDate) As UserField1,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),@endDate) As UserField2,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),@endDate) As UserField3,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),@endDate) As UserField4,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),@endDate) As UserField5,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),@endDate) As UserField6,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),@endDate) As UserField7,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),@endDate) As UserField8,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),@endDate) As UserField9,
 						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),@endDate) As UserField10
   FROM         dbo.sysroEmployeeGroups 
                         INNER JOIN dbo.DailyTaskAccruals ON dbo.DailyTaskAccruals.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.DailyTaskAccruals.Date BETWEEN dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate 
                         INNER JOIN dbo.Tasks ON dbo.Tasks.ID = dbo.DailyTaskAccruals.IDTask 
                         INNER JOIN dbo.BusinessCenters ON dbo.Tasks.IDCenter = dbo.BusinessCenters.ID 
                         INNER JOIN dbo.EmployeeContracts ON dbo.DailyTaskAccruals.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyTaskAccruals.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyTaskAccruals.Date <= dbo.EmployeeContracts.EndDate
 						INNER JOIN dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
   WHERE     (dbo.businesscenters.id is null OR dbo.BusinessCenters.ID IN (SELECT DISTINCT IDCenter FROM sysroPassports_Centers WHERE IDPassport=(SELECT isnull(IdparentPassport,0) from sysroPassports where ID = @idpassport)) )
 			AND dbo.DailyTaskAccruals.Date between @initialDate and @endDate
GO

-- Soporte centralita mxC 4 puertas
INSERT INTO dbo.sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mxC',3,95,'ACC','1','Blind','X','Local','0','0','0','1,0','0')
GO
INSERT INTO dbo.sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mxC',3,96,'ACCTA','1','Blind','X','Local','0','0','0','1,0','0')
GO
INSERT INTO dbo.sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mxC',4,97,'ACC','1','Blind','X','Local','0','0','0','1,0','0')
GO
INSERT INTO dbo.sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mxC',4,98,'ACCTA','1','Blind','X','Local','0','0','0','1,0','0')
GO

CREATE FUNCTION dbo.UFN_SEPARATES_COLUMNS(
 @TEXT      varchar(8000)
,@COLUMN    tinyint
,@SEPARATOR char(1)
)RETURNS varchar(8000)
AS
  BEGIN
       DECLARE @POS_START  int 
       DECLARE @POS_END    int 
	   SET @POS_START = 1
	   SET @POS_END = CHARINDEX(@SEPARATOR, @TEXT, @POS_START)

       WHILE (@COLUMN >1 AND @POS_END> 0)
         BEGIN
             SET @POS_START = @POS_END + 1
             SET @POS_END = CHARINDEX(@SEPARATOR, @TEXT, @POS_START)
             SET @COLUMN = @COLUMN - 1
         END 

       IF @COLUMN > 1  SET @POS_START = LEN(@TEXT) + 1
       IF @POS_END = 0 SET @POS_END = LEN(@TEXT) + 1 

       RETURN SUBSTRING (@TEXT, @POS_START, @POS_END - @POS_START)
  END
GO

ALTER PROCEDURE [dbo].[sysro_UpdatePassportPermissionsOverEmployeesExceptions]
 	(
 		@idPassport int
 	)
     AS
     BEGIN
   	
 	DECLARE @updatePassportIDs table(IDPassport int)
 	;WITH cteUpdate AS 
 	(
 	SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType
 	FROM sysroPassports a
 	WHERE Id = @idPassport and GroupType = 'U'
 	UNION ALL
 	SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType
 	FROM sysropassports a JOIN cteUpdate c ON c.IDParentPassport = a.id
 	where a.GroupType = 'U'
 	)
 	INSERT INTO @updatePassportIDs SELECT Id FROM cteUpdate
 	DELETE FROM sysroPermissionsOverEmployeesExceptions where PassportID in (select IDPassport from @updatePassportIDs)
  	DECLARE @parentPassportID int
 	DECLARE @employeeID int
 	DECLARE @applicationID int
 	DECLARE @permission int
 	
 	DECLARE @insertPassportIDs table(IDPassport int)
 	DECLARE @passportID int
 	DECLARE @oldPermission int
  	DECLARE ExceptionsCursor CURSOR FOR 
  		SELECT IDPassport,IDEmployee,IDApplication,Permission FROM sysroPassports_PermissionsOverEmployees WHERE IDPassport in (select IDPassport from @updatePassportIDs)
  	OPEN ExceptionsCursor
  	FETCH NEXT FROM ExceptionsCursor INTO @parentPassportID,@employeeID,@applicationID,@permission
  	WHILE @@FETCH_STATUS = 0
  	BEGIN	
  		
 		;WITH cteInsert AS 
 		(
 		SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType
 		FROM sysroPassports a
 		WHERE Id = @parentPassportID and GroupType = 'U'
 		UNION ALL
 		SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType
 		FROM sysropassports a JOIN cteInsert c ON a.IDParentPassport = c.id
 		where a.GroupType = 'U'
 		)
 		INSERT INTO @insertPassportIDs SELECT Id FROM cteInsert
 		DECLARE InsertPassportsCursor CURSOR FOR
 			SELECT IDPassport from @insertPassportIDs
 		OPEN InsertPassportsCursor
 		FETCH NEXT FROM InsertPassportsCursor INTO @passportID
 		WHILE @@FETCH_STATUS = 0
 		BEGIN
 			
 			
 			IF EXISTS (SELECT Permission FROM sysroPermissionsOverEmployeesExceptions WHERE PassportID = @passportID AND EmployeeFeatureID = @applicationID AND EmployeeID = @employeeID)
 			BEGIN
 				SELECT @oldPermission = isnull(Permission,-1) FROM sysroPermissionsOverEmployeesExceptions WHERE PassportID = @passportID AND EmployeeFeatureID = @applicationID AND EmployeeID = @employeeID
 				IF @oldPermission > -1
 					IF @oldPermission > @permission
   						UPDATE [dbo].[sysroPermissionsOverEmployeesExceptions] SET Permission = @permission WHERE PassportID = @passportID AND EmployeeFeatureID = @applicationID AND EmployeeID = @employeeID
 			END
   			ELSE
 			BEGIN
   				INSERT INTO [dbo].[sysroPermissionsOverEmployeesExceptions] (PassportID, EmployeeID, EmployeeFeatureID, Permission) VALUES (@passportID,@employeeID,@applicationID,@permission)
 			END
 			
 			FETCH NEXT FROM InsertPassportsCursor INTO @passportID
 		END
 		CLOSE InsertPassportsCursor
  		DEALLOCATE InsertPassportsCursor
  		
 		DELETE FROM @insertPassportIDs
  		FETCH NEXT FROM ExceptionsCursor INTO @parentPassportID,@employeeID,@applicationID,@permission
  	END 
  	CLOSE ExceptionsCursor
  	DEALLOCATE ExceptionsCursor
 	   
 END

GO


-- Hay que añadir un campo identity para identificar de forma única cada valor inicial
alter table [dbo].[StartupValues] drop CONSTRAINT [PK_StartupValues]
GO
alter table [dbo].[StartupValues] drop CONSTRAINT [FK_StartupValues_Concepts]
GO
alter table [dbo].[StartupValues] ADD [IDStartupValue] [int] IDENTITY(1,1) NOT NULL
GO
alter table [dbo].[StartupValues] ADD CONSTRAINT [PK_StartupValue] PRIMARY KEY([IDStartupValue])
GO

-- Actualizamos la tabla de relacion entre Convenio y Valores iniciales para añadir el identificador del valor inicial
alter table [dbo].[LabAgreeStartupValues] drop CONSTRAINT [PK_LabAgreeStartupLimits]
GO
alter table [dbo].[LabAgreeStartupValues] ADD [IDStartupValue] [int]  NULL
GO
update LabAgreeStartupValues set IDStartupValue = (select IDStartupValue from StartupValues where idconcept = LabAgreeStartupValues.IDConcept ) WHERE [IDStartupValue] IS NULL
GO
alter table [dbo].[LabAgreeStartupValues] ALTER COLUMN [IDStartupValue] [int] NOT NULL
GO
alter table [dbo].[LabAgreeStartupValues] ADD CONSTRAINT [PK_LabAgreeStartupValues] PRIMARY KEY([IDLabAgree],[IDStartupValue])
GO

-- procedure para replicar las reglas de convenio asignadas a cada uno de ellos
ALTER TABLE [dbo].[LabAgreeAccrualsRules] ADD [IdNewAccrualsRules] [smallint]  NULL
GO

CREATE PROCEDURE [dbo].[LabAgreeAccrualsRulesNewIDs]
AS
 	BEGIN

		DECLARE @IdAccrualsRules int,@IdIDLabAgree int

		DECLARE LabAgreeAccrualsRulesCursor CURSOR
			FOR SELECT LabAgreeAccrualsRules.IdAccrualsRules, LabAgreeAccrualsRules.IDLabAgree
				FROM LabAgreeAccrualsRules WHERE IdNewAccrualsRules IS NULL ORDER BY IDLabAgree,IdAccrualsRules  


		OPEN LabAgreeAccrualsRulesCursor

		FETCH NEXT FROM LabAgreeAccrualsRulesCursor
		INTO @IdAccrualsRules,@IdIDLabAgree

		WHILE @@FETCH_STATUS = 0
		BEGIN

		    DECLARE @IdNewAccrualRule int
		    SELECT @IdNewAccrualRule = MAX(isnull(IdAccrualsRule,0)) FROM AccrualsRules;
		    SET @IdNewAccrualRule= @IdNewAccrualRule + 1;
		    
			-- Para cada regla del convenio , creamos una nueva 
			-- y el id generado lo añadimos en el campo  IdNewAccrualsRules
			INSERT INTO dbo.AccrualsRules  SELECT @IdNewAccrualRule ,  Name, Description, Definition, Schedule,BeginDate,EndDate, Priority, ExecuteFromLastExecDay   FROM AccrualsRules WHERE IdAccrualsRule=@IdAccrualsRules
			
			UPDATE dbo.LabAgreeAccrualsRules Set IdNewAccrualsRules =  @IdNewAccrualRule WHERE IDLabAgree=@IdIDLabAgree AND IdAccrualsRules=@IdAccrualsRules
			

			FETCH NEXT FROM LabAgreeAccrualsRulesCursor 
			INTO @IdAccrualsRules,@IdIDLabAgree

		END 
		CLOSE LabAgreeAccrualsRulesCursor
		DEALLOCATE LabAgreeAccrualsRulesCursor
		
		-- Actualizamos todas las relaciones de convenios con reglas para asignar el nuevo id de la regla
		UPDATE dbo.LabAgreeAccrualsRules set IdAccrualsRules =IdNewAccrualsRules  WHERE IdNewAccrualsRules IS NOT NULL

 	END
GO

EXEC [dbo].[LabAgreeAccrualsRulesNewIDs]
GO

ALTER TABLE [dbo].[LabAgreeAccrualsRules] DROP COLUMN [IdNewAccrualsRules] 
GO

DROP PROCEDURE [dbo].[LabAgreeAccrualsRulesNewIDs]
GO


-- procedure para replicar los valores iniciales de convenio asignadas a cada uno de ellos
ALTER TABLE [dbo].[LabAgreeStartupValues] ADD [IdNewStartupValue] [int]  NULL
GO

CREATE PROCEDURE [dbo].[LabAgreeStartupValuesNewIDs]
AS
 	BEGIN

		DECLARE @IDStartupValue int,@IdIDLabAgree int

		DECLARE LabAgreeStartupValuesCursor CURSOR
			FOR SELECT LabAgreeStartupValues.IDStartupValue, LabAgreeStartupValues.IDLabAgree
				FROM LabAgreeStartupValues WHERE IdNewStartupValue IS NULL ORDER BY IDLabAgree,IDStartupValue  


		OPEN LabAgreeStartupValuesCursor

		FETCH NEXT FROM LabAgreeStartupValuesCursor
		INTO @IDStartupValue ,@IdIDLabAgree

		WHILE @@FETCH_STATUS = 0
		BEGIN

			-- Para cada valor inicial del convenio , creamos uno nuevo
			INSERT INTO dbo.StartupValues (IDConcept,Name,StartValueType, StartValue,MaximumValueType, MaximumValue, MinimumValueType, MinimumValue  ) SELECT IDConcept,Name,StartValueType, StartValue,MaximumValueType, MaximumValue, MinimumValueType, MinimumValue   FROM StartupValues WHERE IDStartupValue=@IDStartupValue
			
			DECLARE @IdNewStartupValue int
			select @IdNewStartupValue=@@IDENTITY;
			
			UPDATE dbo.LabAgreeStartupValues Set IdNewStartupValue =  @IdNewStartupValue WHERE IDLabAgree=@IdIDLabAgree AND IDStartupValue=@IDStartupValue

			FETCH NEXT FROM LabAgreeStartupValuesCursor 
			INTO @IDStartupValue ,@IdIDLabAgree

		END 
		CLOSE LabAgreeStartupValuesCursor
		DEALLOCATE LabAgreeStartupValuesCursor
		
		-- Actualizamos todas las relaciones de convenios con valores iniciales para asignar el nuevo id del valor inicial
		UPDATE dbo.LabAgreeStartupValues set IDStartupValue =IdNewStartupValue  WHERE IdNewStartupValue IS NOT NULL

 	END
GO

EXEC [dbo].[LabAgreeStartupValuesNewIDs]
GO

ALTER TABLE [dbo].LabAgreeStartupValues DROP COLUMN [IdNewStartupValue] 
GO

DROP PROCEDURE [dbo].[LabAgreeStartupValuesNewIDs]
GO


-- ELIMINAMOS LAS ENTRADAS DE PANTALLA DE VALORES INICIALES Y REGLAS DE CONVENIO
DELETE FROM dbo.sysroGUI WHERE IDPath like 'Portal\LabAgree\LabAgreeRules'
GO
DELETE FROM dbo.sysroGUI WHERE IDPath like 'Portal\LabAgree\StartupValues'
GO
DELETE FROM dbo.sysroGUI_Actions WHERE IDGUIPath like 'Portal\LabAgree\StartupValues%'
GO
DELETE FROM  dbo.sysroGUI_Actions WHERE IDGUIPath like 'Portal\LabAgree\LabAgreeRules%'
GO
UPDATE dbo.sysroGUI SET IDPath = 'Portal\ShiftManagement\LabAgree' WHERE IDPath = 'Portal\LabAgree\LabAgree'
GO
DELETE FROM dbo.sysroGUI WHERE IDPath = 'Portal\LabAgree'
GO
UPDATE dbo.sysrogui SET Priority = 102 WHERE IDPath = 'Portal\ShiftManagement\Accruals'
GO
UPDATE dbo.sysrogui SET Priority = 103 WHERE IDPath = 'Portal\ShiftManagement\Shifts'
GO
UPDATE dbo.sysrogui SET Priority = 103 WHERE IDPath = 'Portal\ShiftManagement\ShiftsPro'
GO
UPDATE dbo.sysrogui SET Priority = 104 WHERE IDPath = 'Portal\ShiftManagement\Causes'
GO
UPDATE dbo.sysrogui SET Priority = 105 WHERE IDPath = 'Portal\ShiftManagement\Assignments'
GO


-- ELIMINAMOS LAS FEATURES DE REGLAS DE CONVENIO Y VALORES INICIALES
DELETE FROM sysroFeatures WHERE ID IN(11300,11310,11400,11410)
GO

EXEC dbo.sysro_GenerateAllPermissionsOverFeatures
GO

-- Nueva tabla de limites por justificacion
CREATE TABLE [dbo].[CauseLimitValues](
	[IDCauseLimitValue] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[IDCause] [smallint] NOT NULL,
	[MaximumAnnualValueType] [int] NULL CONSTRAINT [DF_CauseLimitValues_MaximumAnnualValueType]  DEFAULT ((0)),
	[MaximumAnnualValue] [nvarchar](max) NULL,
	[MaximumMonthlyType] [int] NULL CONSTRAINT [DF_CauseLimitValues_MaximumMonthlyValueType]  DEFAULT ((0)),
	[MaximumMonthlyValue] [nvarchar](max) NULL,
	[IDExcessCause] [smallint] NULL CONSTRAINT [DF_CauseLimitValues_IDExcessCause]  DEFAULT ((0)),
 CONSTRAINT [PK_CauseLimitValues] PRIMARY KEY CLUSTERED 
(
	[IDCauseLimitValue] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[LabAgreeCauseLimitValues](
	[IDLabAgree] [int] NOT NULL,
	[IDCauseLimitValue] [smallint] NOT NULL,
	[BeginDate] [smalldatetime] NOT NULL,
	[EndDate] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_LabAgreeCauseLimitValues] PRIMARY KEY CLUSTERED 
(
	[IDLabAgree] ASC,
	[IDCauseLimitValue] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO


UPDATE dbo.sysroParameters SET Data='380' WHERE ID='DBVersion'
GO
