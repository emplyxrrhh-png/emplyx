-- Permisos
if exists (select 1 from sysroLiveAdvancedParameters where ParameterName = 'SecurityMode' and Value = '3')
BEGIN
	if not exists (select 1 from sysroGroupFeatures_PermissionsOverFeatures where IDFeature = 35 and IDGroupFeature = 0)
		insert into sysroGroupFeatures_PermissionsOverFeatures (IDGroupFeature,IDFeature,Permision) values (0,35,9)

	if not exists (select 1 from sysroGroupFeatures_PermissionsOverFeatures where IDFeature = 1645 and IDGroupFeature = 0)
		insert into sysroGroupFeatures_PermissionsOverFeatures (IDGroupFeature,IDFeature,Permision) values (0,1645,9)

	if not exists (select 1 from sysroGroupFeatures_PermissionsOverFeatures where IDFeature = 1650 and IDGroupFeature = 0)
		insert into sysroGroupFeatures_PermissionsOverFeatures (IDGroupFeature,IDFeature,Permision) values (0,1650,9)

	if not exists (select 1 from sysroGroupFeatures_PermissionsOverFeatures where IDFeature = 1655 and IDGroupFeature = 0)
		insert into sysroGroupFeatures_PermissionsOverFeatures (IDGroupFeature,IDFeature,Permision) values (0,1655,9)

	update sysroGroupFeatures_PermissionsOverFeatures set Permision = 9 where IDFeature = 35 and IDGroupFeature = 0
	update sysroGroupFeatures_PermissionsOverFeatures set Permision = 9 where IDFeature = 1645 and IDGroupFeature = 0
	update sysroGroupFeatures_PermissionsOverFeatures set Permision = 9 where IDFeature = 1650 and IDGroupFeature = 0
	update sysroGroupFeatures_PermissionsOverFeatures set Permision = 9 where IDFeature = 1655 and IDGroupFeature = 0

	if not exists (select 1 from sysroPassports_PermissionsOverFeatures where IDFeature = 35 and IDPassport in( select IDParentPassport from sysroPassports where IDGroupFeature = 0 and IsSupervisor = 1))
		insert into sysroPassports_PermissionsOverFeatures (IDPassport,IDFeature,Permission) select IDParentPassport,35,9 from sysroPassports where IDGroupFeature = 0 and IsSupervisor = 1
	
	if not exists (select 1 from sysroPassports_PermissionsOverFeatures where IDFeature = 1645 and IDPassport in( select IDParentPassport from sysroPassports where IDGroupFeature = 0 and IsSupervisor = 1))
		insert into sysroPassports_PermissionsOverFeatures (IDPassport,IDFeature,Permission) select IDParentPassport,1645,9 from sysroPassports where IDGroupFeature = 0 and IsSupervisor = 1
	
	if not exists (select 1 from sysroPassports_PermissionsOverFeatures where IDFeature = 1650 and IDPassport in( select IDParentPassport from sysroPassports where IDGroupFeature = 0 and IsSupervisor = 1))
		insert into sysroPassports_PermissionsOverFeatures (IDPassport,IDFeature,Permission) select IDParentPassport,1650,9 from sysroPassports where IDGroupFeature = 0 and IsSupervisor = 1
	
	if not exists (select 1 from sysroPassports_PermissionsOverFeatures where IDFeature = 1655 and IDPassport in( select IDParentPassport from sysroPassports where IDGroupFeature = 0 and IsSupervisor = 1))
		insert into sysroPassports_PermissionsOverFeatures (IDPassport,IDFeature,Permission) select IDParentPassport,1655,9 from sysroPassports where IDGroupFeature = 0 and IsSupervisor = 1


	update sysroPassports_PermissionsOverFeatures set Permission = 9 where IDFeature = 35 and IDPassport in( select IDParentPassport from sysroPassports where IDGroupFeature = 0 and IsSupervisor = 1)
	update sysroPassports_PermissionsOverFeatures set Permission = 9 where IDFeature = 1645 and IDPassport in( select IDParentPassport from sysroPassports where IDGroupFeature = 0 and IsSupervisor = 1)
	update sysroPassports_PermissionsOverFeatures set Permission = 9 where IDFeature = 1650 and IDPassport in( select IDParentPassport from sysroPassports where IDGroupFeature = 0 and IsSupervisor = 1)
	update sysroPassports_PermissionsOverFeatures set Permission = 9 where IDFeature = 1655 and IDPassport in( select IDParentPassport from sysroPassports where IDGroupFeature = 0 and IsSupervisor = 1)

	exec dbo.sysro_GenerateAllPermissionsOverFeatures
END
GO

-- Teletrabajo V2
DECLARE @sql nvarchar(max)
SELECT @sql = 'ALTER TABLE [dbo].[Employees] DROP CONSTRAINT ' + df.NAME
FROM sys.default_constraints df
INNER JOIN sys.tables t ON df.parent_object_id = t.object_id
INNER JOIN sys.columns c ON df.parent_object_id = c.object_id AND df.parent_column_id = c.column_id
where t.name = 'Employees' and c.name = 'Telecommuting'
EXEC sp_executeSql @sql
GO

ALTER TABLE [dbo].[Employees] DROP COLUMN Telecommuting
GO

DECLARE @sql nvarchar(max)
SELECT @sql = 'ALTER TABLE [dbo].[Employees] DROP CONSTRAINT ' + df.NAME
FROM sys.default_constraints df
INNER JOIN sys.tables t ON df.parent_object_id = t.object_id
INNER JOIN sys.columns c ON df.parent_object_id = c.object_id AND df.parent_column_id = c.column_id
where t.name = 'Employees' and c.name = 'TelecommutingDays'
EXEC sp_executeSql @sql
GO

ALTER TABLE [dbo].[Employees] DROP COLUMN TelecommutingDays
GO


ALTER TABLE [dbo].[EmployeeContracts] ADD Telecommuting BIT NOT NULL DEFAULT 0;
GO
ALTER TABLE [dbo].[EmployeeContracts] ADD TelecommutingMandatoryDays nvarchar(100) NOT NULL DEFAULT '';
GO
ALTER TABLE [dbo].[EmployeeContracts] ADD TelecommutingOptionalDays nvarchar(100) NOT NULL DEFAULT '';
GO
ALTER TABLE [dbo].[EmployeeContracts] ADD TelecommutingMaxOptionalDays Integer DEFAULT 0;
GO
ALTER TABLE [dbo].[EmployeeContracts] ADD TelecommutingAgreementStart DATE
GO
ALTER TABLE [dbo].[EmployeeContracts] ADD TelecommutingAgreementEnd DATE
GO


ALTER TABLE [dbo].[LabAgree] ADD Telecommuting BIT NOT NULL DEFAULT 0;
GO
ALTER TABLE [dbo].[LabAgree] ADD TelecommutingMandatoryDays nvarchar(100) NOT NULL DEFAULT '';
GO
ALTER TABLE [dbo].[LabAgree] ADD TelecommutingOptionalDays nvarchar(100) NOT NULL DEFAULT '';
GO
ALTER TABLE [dbo].[LabAgree] ADD TelecommutingMaxOptionalDays Integer DEFAULT 0;
GO
ALTER TABLE [dbo].[LabAgree] ADD TelecommutingAgreementStart DATE
GO
ALTER TABLE [dbo].[LabAgree] ADD TelecommutingAgreementEnd DATE
GO

DROP VIEW IF EXISTS [dbo].[sysrovwTelecommutingAgreement]
GO
CREATE VIEW [dbo].[sysrovwTelecommutingAgreement]
    AS
		SELECT * FROM (
		SELECT  
		EC.IDEmployee, 
		EC.IDContract, 
		EC.BeginDate AS ContractStart, 
		EC.EndDate AS ContractEnd,
		LA.ID AS LabAgreeId,
		CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN (CASE WHEN LA.TelecommutingAgreementStart IS NULL THEN '' ELSE 'LabAgree' END) ELSE 'Contract' END AS TelecommutingAgreementSource,
		ISNULL(CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN LA.Telecommuting ELSE EC.Telecommuting END,0) AS Telecommuting,
		CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN LA.TelecommutingMandatoryDays ELSE EC.TelecommutingMandatoryDays END AS TelecommutingMandatoryDays,
		CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN LA.TelecommutingOptionalDays ELSE EC.TelecommutingOptionalDays END AS TelecommutingOptionalDays,
		CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN LA.TelecommutingMaxOptionalDays ELSE EC.TelecommutingMaxOptionalDays END AS TelecommutingMaxOptionalDays,
		CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN LA.TelecommutingAgreementStart ELSE EC.TelecommutingAgreementStart END AS TelecommutingAgreementStart,
		CASE WHEN EC.TelecommutingAgreementStart IS NULL THEN LA.TelecommutingAgreementEnd ELSE EC.TelecommutingAgreementEnd END AS TelecommutingAgreementEnd
		FROM EmployeeContracts EC
		LEFT JOIN LabAgree LA ON EC.IDLabAgree = LA.ID
		) AUX
		WHERE ContractEnd >= TelecommutingAgreementStart AND TelecommutingAgreementEnd >= ContractStart
GO

DROP FUNCTION IF EXISTS [dbo].[EmployeeZonesBetweenDates]
 GO
CREATE FUNCTION [dbo].[EmployeeZonesBetweenDates]
   (				
 	@datebeginpar smalldatetime,
 	@dateendpar smalldatetime,
	@employeeidlist nvarchar(max)
   )
   RETURNS @ValueTable table(IDEmployee int, IDContract NVARCHAR(50), TelecommutingExpected BIT, TelecommutePlanned BIT, InTelecommute BIT, RefDate smalldatetime, ContractWorkCenter NVARCHAR(50),
							 DailyWorkCenter NVARCHAR(50), CalculatedWorkCenter NVARCHAR(50), InAbsence BIT, NoWork BIT, ZoneOnDate NVARCHAR(50), ExpectedZone NVARCHAR(50)) 
   AS
   BEGIN
    declare @iniperiod smalldatetime 
 	declare @endperiod smalldatetime
	DECLARE @employees nvarchar(max)
	DECLARE @employeeIDs Table(idEmployee int)
 	SET @iniperiod = @datebeginpar
 	SET @endperiod = @dateendpar
	SET @employees = @employeeidlist

	IF LEN(@employees) = 0 
		INSERT INTO @ValueTable
		SELECT Employees.Id AS IDEmployee,
			   EmployeeContracts.IDContract,
			   CASE WHEN (Agreement.Telecommuting = 1 AND ISNULL(Agreement.TelecommutingMandatoryDays,'') = '') THEN 1 ELSE (CASE WHEN CHARINDEX(CONVERT(VARCHAR,(DATEPART(dw, dt) + @@DATEFIRST - 1 ) % 7), Agreement.TelecommutingMandatoryDays) > 0 THEN 1 ELSE 0 END) END AS TelecommutingExpected,
			   DailySchedule.Telecommuting AS TelecommutePlanned,
			   ISNULL(PunchesTC.ID,0) InTelecommute,
			   dt AS [RefDate],
			   ISNULL(EmployeeContracts.Enterprise,'') as ContractWorkCenter,
			   ISNULL(DailySchedule.WorkCenter,'') as DailyWorkCenter,
			   CASE WHEN DailySchedule.WorkCenter IS NOT NULL THEN ISNULL(DailySchedule.WorkCenter,'') ELSE ISNULL(EmployeeContracts.Enterprise,'') END AS CalculatedWorkCenter,
			   CASE WHEN ProgrammedAbsences.IDEmployee IS NULL THEN 0 ELSE 1 END AS InAbsence,
			   CASE WHEN Shifts.ID IS NULL OR Shifts.ShiftType = 2 OR Shifts.ExpectedWorkingHours = 0 THEN 1 ELSE 0 END AS NoWork,
			   CASE WHEN Punches_LastOnDate.ActualType IS NOT NULL AND Punches_LastOnDate.ActualType <> 2 THEN  ISNULL(ZoneOnPastDate.Name,'?')  END AS ZoneOnDate,
			   CASE WHEN Punches_LastOnPastDates.ActualType IS NOT NULL AND Punches_LastOnPastDates.ActualType <> 2 AND dt >= DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) THEN ISNULL(ZoneExpectedOnDate.Name,'?') END  AS ExpectedZone
		FROM   [dbo].[Alldays] (@iniperiod, @endperiod)
			   LEFT JOIN EmployeeContracts WITH(NOLOCK) ON alldays.dt BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate
			   LEFT JOIN sysrovwTelecommutingAgreement Agreement ON Agreement.IDContract = EmployeeContracts.IDContract 
			   INNER JOIN Employees WITH(NOLOCK) ON Employees.ID = EmployeeContracts.IDEmployee
			   LEFT JOIN DailySchedule WITH(NOLOCK) ON DailySchedule.Date = alldays.dt AND DailySchedule.IDEmployee = EmployeeContracts.IDemployee
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Shiftdate ORDER BY DateTime DESC) AS 'RowNumberTC', * FROM Punches WITH(NOLOCK) WHERE InTelecommute = 1 AND Shiftdate >= @iniperiod) PunchesTC ON PunchesTC.IDEmployee = EmployeeContracts.IDemployee AND PunchesTC.ShiftDate = dt AND (RowNumberTC = 1 OR RowNumberTC IS NULL)
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Shiftdate ORDER BY DateTime DESC) AS 'RowNumberLastOnDate', * FROM Punches WITH(NOLOCK) WHERE IDZone > 0 AND ActualType <> 2 AND Shiftdate >= @iniperiod) Punches_LastOnDate ON Punches_LastOnDate.IDEmployee = EmployeeContracts.IDemployee AND Punches_LastOnDate.ShiftDate = dt AND (RowNumberLastOnDate = 1 OR RowNumberLastOnDate IS NULL)
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY DateTime DESC) AS 'RowNumberLastOnPastDates', * FROM Punches WITH(NOLOCK) WHERE IDZone > 0 AND ActualType <> 2 AND Shiftdate >= DATEADD(day,-30,@iniperiod) AND ShiftDate <= DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)) Punches_LastOnPastDates ON Punches_LastOnPastDates.IDEmployee = EmployeeContracts.IDemployee AND (RowNumberLastOnPastDates = 1 OR RowNumberLastOnPastDates IS NULL)
			   LEFT JOIN Zones AS ZoneOnPastDate ON ZoneOnPastDate.ID = Punches_LastOnDate.IDZone
			   LEFT JOIN Zones AS ZoneExpectedOnDate ON ZoneExpectedOnDate.ID = Punches_LastOnPastDates.IDZone
			   LEFT JOIN ProgrammedAbsences WITH(NOLOCK) ON ProgrammedAbsences.IDEmployee = EmployeeContracts.IDEmployee AND dt BETWEEN ProgrammedAbsences.BeginDate AND isnull(ProgrammedAbsences.FinishDate,DATEADD(day,ProgrammedAbsences.maxlastingdays,ProgrammedAbsences.BeginDate))
			   LEFT JOIN Shifts WITH(NOLOCK) ON Shifts.ID = DailySchedule.IDShift1
	ELSE
		INSERT INTO @ValueTable
		SELECT Employees.Id AS IDEmployee,
			   EmployeeContracts.IDContract,
			   CASE WHEN (Agreement.Telecommuting = 1 AND ISNULL(Agreement.TelecommutingMandatoryDays,'') = '') THEN 1 ELSE (CASE WHEN CHARINDEX(CONVERT(VARCHAR,(DATEPART(dw, dt) + @@DATEFIRST - 1 ) % 7), Agreement.TelecommutingMandatoryDays) > 0 THEN 1 ELSE 0 END) END AS TelecommutingExpected,
			   DailySchedule.Telecommuting AS TelecommutePlanned,
			   ISNULL(PunchesTC.ID,0) InTelecommute,
			   dt AS [RefDate],
			   ISNULL(EmployeeContracts.Enterprise,'') as ContractWorkCenter,
			   ISNULL(DailySchedule.WorkCenter,'') as DailyWorkCenter,
			   CASE WHEN DailySchedule.WorkCenter IS NOT NULL THEN ISNULL(DailySchedule.WorkCenter,'') ELSE ISNULL(EmployeeContracts.Enterprise,'') END AS CalculatedWorkCenter,
			   CASE WHEN ProgrammedAbsences.IDEmployee IS NULL THEN 0 ELSE 1 END AS InAbsence,
			   CASE WHEN Shifts.ID IS NULL OR Shifts.ShiftType = 2 OR Shifts.ExpectedWorkingHours = 0 THEN 1 ELSE 0 END AS NoWork,
			   CASE WHEN Punches_LastOnDate.ActualType IS NOT NULL AND Punches_LastOnDate.ActualType <> 2 THEN  ISNULL(ZoneOnPastDate.Name,'?')  END AS ZoneOnDate,
			   CASE WHEN Punches_LastOnPastDates.ActualType IS NOT NULL AND Punches_LastOnPastDates.ActualType <> 2 AND dt >= DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) THEN ISNULL(ZoneExpectedOnDate.Name,'?') END  AS ExpectedZone
		FROM   [dbo].[Alldays] (@iniperiod, @endperiod)
			   LEFT JOIN EmployeeContracts WITH(NOLOCK) ON alldays.dt BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate
			   LEFT JOIN sysrovwTelecommutingAgreement Agreement ON Agreement.IDContract = EmployeeContracts.IDContract 
			   INNER JOIN Employees WITH(NOLOCK) ON Employees.ID = EmployeeContracts.IDEmployee
			   LEFT JOIN DailySchedule WITH(NOLOCK) ON DailySchedule.Date = alldays.dt AND DailySchedule.IDEmployee = EmployeeContracts.IDemployee
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Shiftdate ORDER BY DateTime DESC) AS 'RowNumberTC', * FROM Punches WITH(NOLOCK) WHERE InTelecommute = 1 AND Shiftdate >= @iniperiod) PunchesTC ON PunchesTC.IDEmployee = EmployeeContracts.IDemployee AND PunchesTC.ShiftDate = dt AND (RowNumberTC = 1 OR RowNumberTC IS NULL)
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Shiftdate ORDER BY DateTime DESC) AS 'RowNumberLastOnDate', * FROM Punches WITH(NOLOCK) WHERE IDZone > 0 AND ActualType <> 2 AND Shiftdate >= @iniperiod) Punches_LastOnDate ON Punches_LastOnDate.IDEmployee = EmployeeContracts.IDemployee AND Punches_LastOnDate.ShiftDate = dt AND (RowNumberLastOnDate = 1 OR RowNumberLastOnDate IS NULL)
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY DateTime DESC) AS 'RowNumberLastOnPastDates', * FROM Punches WITH(NOLOCK) WHERE IDZone > 0 AND ActualType <> 2 AND Shiftdate >= DATEADD(day,-30,@iniperiod) AND ShiftDate <= DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)) Punches_LastOnPastDates ON Punches_LastOnPastDates.IDEmployee = EmployeeContracts.IDemployee AND (RowNumberLastOnPastDates = 1 OR RowNumberLastOnPastDates IS NULL)
			   LEFT JOIN Zones AS ZoneOnPastDate ON ZoneOnPastDate.ID = Punches_LastOnDate.IDZone
			   LEFT JOIN Zones AS ZoneExpectedOnDate ON ZoneExpectedOnDate.ID = Punches_LastOnPastDates.IDZone
			   LEFT JOIN ProgrammedAbsences WITH(NOLOCK) ON ProgrammedAbsences.IDEmployee = EmployeeContracts.IDEmployee AND dt BETWEEN ProgrammedAbsences.BeginDate AND isnull(ProgrammedAbsences.FinishDate,DATEADD(day,ProgrammedAbsences.maxlastingdays,ProgrammedAbsences.BeginDate))
			   LEFT JOIN Shifts WITH(NOLOCK) ON Shifts.ID = DailySchedule.IDShift1
        WHERE EmployeeContracts.IDEmployee IN (SELECT Value AS IDEmployye FROM SplitToInt(@employees,','))
	RETURN
   END
GO

INSERT INTO [dbo].[sysroRequestType] VALUES (16, 'Telecommute', '5')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =2590)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2590,2500,'Calendar.Requests.Telecommute','Teletrabajo','','U','RWA',NULL,16,2)

	INSERT INTO [dbo].[sysroGroupFeatures_PermissionsOverFeatures] (IDGroupFeature,IDFeature,Permision) select IDGroupFeature,2590,Permision from [dbo].[sysroGroupFeatures_PermissionsOverFeatures] WHERE [IDFeature] = 2500

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2590, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2500

	exec dbo.sysro_GenerateAllPermissionsOverFeatures
END
GO

ALTER TABLE dbo.Notifications ADD InProgress bit not null DEFAULT 0
GO

UPDATE dbo.Notifications SET InProgress = 0
GO

ALTER TABLE dbo.Notifications ADD GUID nvarchar(max) NULL
GO

ALTER TABLE dbo.Notifications ADD LastCheck datetime NULL
GO

update dbo.sysroNotificationTypes set Scheduler = 2 where Scheduler = 1
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 82)
BEGIN
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,Feature,FeatureType,OnlySystem) VALUES(82,'Telecommute agreement finish',null, 1,'Calendar.Scheduler','U', 360)
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (82,1,'Subject',0,'','TelecommuteAgreement')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (82,1,'Body',1,'EmployeeName','TelecommuteAgreement')
	INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (82,1,'Body',2,'AgreementFinish','TelecommuteAgreement')
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notifications] WHERE ID = 1603)
	INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal) VALUES(1603, 82, 'Aviso de fin de acuerdo de telebrajo','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'VisualTime.Procees.NotifierVersion')
	insert into sysroLiveAdvancedParameters values ('VisualTime.Procees.NotifierVersion', '2')
GO

--Estudio Genius Tiempo efectivo
DELETE [dbo].[GeniusViews] WHERE [Name] = 'effectiveHours' and [IdPassport] = 0
GO


DROP PROCEDURE [dbo].[Genius_EfectiveWork]
GO
CREATE PROCEDURE [dbo].[Genius_EfectiveWork] @initialDate smalldatetime, @endDate smalldatetime, @idpassport int,  @userFieldsFilter nvarchar(max), @employeeFilter nvarchar(max) AS  
  		--DECLARE @employeeIDs Table(idEmployee int)
		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @puserFieldsFilter nvarchar(max) = @userFieldsFilter
		DECLARE @pemployeeFilter nvarchar(max) = @employeeFilter
		DECLARE @intdatefirst int  
  		SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'  
  		SET DateFirst @intdatefirst;  
  		
		CREATE TABLE #TMP_EffectiveHours (idemployee INT)
		--INSERT INTO @employeeIDs EXEC dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
		INSERT INTO #TMP_EffectiveHours EXEC dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
		
		SELECT	IDEmployee,
				EmployeeName,
				GroupName, 
				FullGroupName,
				WorkCenter,
				IDContract,
				BeginContract_ToDateString,
				EndContract_ToDateString,
				Age, 
				RegDate AS Date_ToDateString,
				Mes,
				Año,
				DayOfWeek,
				WeekOfYear,
				DayOfYear,
				Quarter,
				Nivel1, 
				Nivel2, 
				Nivel3, 
				Nivel4, 
				Nivel5, 
				Nivel6,
				Nivel7, 
				Nivel8, 
				Nivel9, 
				Nivel10, 
				UserField1, 
				UserField2, 
				UserField3, 
				UserField4, 
				UserField5, 
				UserField6, 
				UserField7, 
				UserField8, 
				UserField9, 
				UserField10, 
				Office as Office_ToHours, 
				ISNULL(Telecommute,0) AS Telecommute_ToHours 
		FROM
		(
			SELECT Employees.Id AS IdEmployee,
				dt AS RegDate, 
				MONTH(dt) AS Mes,   
  				YEAR(dt) AS Año, 
				(DATEPART(dw, dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 
				DATEPART(iso_week, dt) AS WeekOfYear, 
				DATEPART(dy, dt) AS DayOfYear, 
				DATEPART(QUARTER, dt) AS Quarter, 
				Employees.Name as EmployeeName, 
				dbo.sysroEmployeeGroups.GroupName, 
				dbo.sysroEmployeeGroups.FullGroupName, 
				EmployeeContracts.Enterprise As WorkCenter,
				EmployeeContracts.IDContract AS IDContract,
				EmployeeContracts.BeginDate AS BeginContract_ToDateString,
				EmployeeContracts.EndDate AS EndContract_ToDateString,
				ISNULL(sysrovwDailyEfectiveWorkingHours.Value,0.0) As Value, 
				CASE WHEN sysrovwDailyEfectiveWorkingHours.InTelecommute = 1 THEN 'Telecommute' ELSE 'Office' END AS TimeType,
			  	dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,
				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,
				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,
				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,
				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,
				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dt) As UserField1,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dt) As UserField2,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dt) As UserField3,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dt) As UserField4,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dt) As UserField5,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dt) As UserField6,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dt) As UserField7,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dt) As UserField8,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dt) As UserField9,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dt) As UserField10,
 				dbo.GetEmployeeAge(Employees.Id) As Age    
			FROM [dbo].[Alldays] (@pinitialDate, @pendDate)
			LEFT JOIN EmployeeContracts with (nolock) ON dt BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate
			LEFT JOIN sysroEmployeeGroups with (nolock) ON dt BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate AND EmployeeContracts.IDEmployee = sysroEmployeeGroups.IDEmployee
			LEFT JOIN Employees with (nolock) ON Employees.ID = sysroEmployeeGroups.IDEmployee
			LEFT JOIN DailySchedule with (nolock) ON DailySchedule.IDEmployee =  Employees.ID AND DailySchedule.Date = dt
			LEFT JOIN sysrovwDailyEfectiveWorkingHours with (nolock) ON sysrovwDailyEfectiveWorkingHours.Date = dt AND sysrovwDailyEfectiveWorkingHours.IdEmployee = Employees.ID
			--WHERE sysroEmployeeGroups.IdEmployee IN (SELECT idemployee FROM @employeeIDs)
			WHERE sysroEmployeeGroups.IdEmployee IN (SELECT idemployee FROM #TMP_EffectiveHours)
			AND dbo.WebLogin_GetPermissionOverEmployee(@idpassport,Employees.Id,2,0,0,dt) > 1 
		) TMP
		PIVOT (AVG(Value) for TimeType in (Office,Telecommute)) AS Accrual
		ORDER BY IdEmployee ASC, RegDate ASC
GO

ALTER PROCEDURE  [dbo].[Genius_Schedule]
         	@initialDate smalldatetime,
         	@endDate smalldatetime,
         	@idpassport int,
         	@employeeFilter nvarchar(max),
         	@userFieldsFilter nvarchar(max)
          AS
          DECLARE @employeeIDs Table(idEmployee int)
      	 DECLARE @pinitialDate smalldatetime = @initialDate,  
           @pendDate smalldatetime = @endDate,  
           @pidpassport int = @idpassport,  
           @pemployeeFilter nvarchar(max) = @employeeFilter,  
           @puserFieldsFilter nvarchar(max) = @userFieldsFilter 
   		
   		DECLARE @intdatefirst int
   		SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'
   		SET DateFirst @intdatefirst;
           insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;
           SELECT     CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
      		+ '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10) AS KeyView, 
      		dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, 
      		dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeesShifts.ShiftName,
			CASE WHEN (CASE WHEN ProgrammedAbsences.IDEmployee IS NULL THEN 0 ELSE 1 END) = 1 OR (CASE WHEN Shifts.ID IS NULL OR Shifts.ShiftType = 2 OR Shifts.ExpectedWorkingHours = 0 THEN 1 ELSE 0 END) = 1 THEN '' ELSE CASE WHEN (ISNULL(sysroEmployeesShifts.Telecommuting, CASE WHEN (sysrovwTelecommutingAgreement.Telecommuting = 1 AND CHARINDEX(CONVERT(VARCHAR, (DATEPART(dw, CurrentDate) + @@DATEFIRST - 1 ) % 7), sysrovwTelecommutingAgreement.TelecommutingMandatoryDays) > 0) THEN 1 ELSE 0 END)) = 1 THEN 'X' ELSE '' END END AS TelecommutingExpected,
			dbo.sysroEmployeesShifts.CurrentDate AS Date, dbo.sysroEmployeesShifts.CurrentDate AS Date_ToDateString, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours,SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHoursAbs) as HoursAbs, COUNT(*) AS Count, 
      		MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año, (DATEPART(dw, 
      		dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear, 
      		DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear, DATEPART(QUARTER, dbo.sysroEmployeesShifts.CurrentDate) as Quarter, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, 
      		dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.endDate, dbo.sysroEmployeeGroups.endDate AS endDate_ToDateString,
      		dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.endDate AS EndContract, dbo.EmployeeContracts.endDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
      		dbo.sysroEmployeesShifts.IDAssignment,
      		convert(nvarchar(max),dbo.sysroEmployeesShifts.Remark) as Remark,
      		dbo.sysroEmployeesShifts.AssignmentName,
      		dbo.sysroEmployeesShifts.IDProductiveUnit,
      		dbo.sysroEmployeesShifts.ProductiveUnitName, 
    		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
      		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
      		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
      		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
      		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField1,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField2,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField3,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField4,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField5,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField6,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField7,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField8,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField9,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField10,
 			dbo.GetEmployeeAge(sysroEmployeesShifts.idEmployee) As Age, (SELECT LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = sysroEmployeesShifts.IDEmployee) as LockDate, dbo.GetEmployeeAge(sysroEmployeesShifts.idEmployee) As Age, (SELECT LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = sysroEmployeesShifts.IDEmployee) as LockDate_ToDateString
        FROM dbo.sysroEmployeesShifts with (nolock) 
      		INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.sysroEmployeeGroups.endDate 
      		INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.EmployeeContracts.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.EmployeeContracts.endDate 
			LEFT JOIN dbo.sysrovwTelecommutingAgreement ON sysrovwTelecommutingAgreement.IDContract = EmployeeContracts.IDContract
			LEFT JOIN Shifts WITH(NOLOCK) ON Shifts.ID = sysroEmployeesShifts.IDShift
      		LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
     		LEFT JOIN ProgrammedAbsences WITH(NOLOCK) ON ProgrammedAbsences.IDEmployee = EmployeeContracts.IDEmployee AND CurrentDate BETWEEN ProgrammedAbsences.BeginDate AND isnull(ProgrammedAbsences.FinishDate,DATEADD(day,ProgrammedAbsences.maxlastingdays,ProgrammedAbsences.BeginDate))
		WHERE  dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeesShifts.IDEmployee,2,0,0,dbo.sysroEmployeesShifts.CurrentDate) > 1
      		AND dbo.sysroEmployeesShifts.IDEmployee in( select idEmployee from @employeeIDs) and dbo.sysroEmployeesShifts.CurrentDate between @pinitialDate and @pendDate
      	GROUP BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name, 
      		dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate, MONTH(dbo.sysroEmployeesShifts.CurrentDate), 
			YEAR(dbo.sysroEmployeesShifts.CurrentDate), (DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, 
      		dbo.sysroEmployeesShifts.CurrentDate), DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate), dbo.sysroEmployeeGroups.IDGroup, 
      		dbo.EmployeeContracts.IDContract, CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
      		+ '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10), dbo.sysroEmployeeGroups.Path, 
      		dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.endDate,
      		dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.endDate,dbo.sysroEmployeesShifts.IDAssignment,dbo.sysroEmployeesShifts.AssignmentName,dbo.sysroEmployeesShifts.IDProductiveUnit,dbo.sysroEmployeesShifts.ProductiveUnitName, convert(nvarchar(max),dbo.sysroEmployeesShifts.Remark),
			CASE WHEN (CASE WHEN ProgrammedAbsences.IDEmployee IS NULL THEN 0 ELSE 1 END) = 1 OR (CASE WHEN Shifts.ID IS NULL OR Shifts.ShiftType = 2 OR Shifts.ExpectedWorkingHours = 0 THEN 1 ELSE 0 END) = 1 THEN '' ELSE CASE WHEN (ISNULL(sysroEmployeesShifts.Telecommuting, CASE WHEN (sysrovwTelecommutingAgreement.Telecommuting = 1 AND CHARINDEX(CONVERT(VARCHAR, (DATEPART(dw, CurrentDate) + @@DATEFIRST - 1 ) % 7), sysrovwTelecommutingAgreement.TelecommutingMandatoryDays) > 0) THEN 1 ELSE 0 END)) = 1 THEN 'X' ELSE '' END END 
GO

ALTER PROCEDURE [dbo].[Genius_Punches]
          @initialDate smalldatetime,  
          @endDate smalldatetime,  
          @idpassport int,  
          @employeeFilter nvarchar(max),  
          @userFieldsFilter nvarchar(max)  
          AS  
          DECLARE @employeeIDs Table(idEmployee int) 
     	 DECLARE @pinitialDate smalldatetime = @initialDate,  
          @pendDate smalldatetime = @endDate,  
          @pidpassport int = @idpassport,  
          @pemployeeFilter nvarchar(max) = @employeeFilter,  
          @puserFieldsFilter nvarchar(max) = @userFieldsFilter  
          insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
          SELECT     dbo.Punches.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee,   
    					dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Punches.ActualType AS PunchDirection,   
    					dbo.Punches.TypeData AS PunchIDTypeData, CASE WHEN dbo.Punches.TypeData <> 0 THEN dbo.Causes.Name ELSE '' END AS PunchTypeData,   CASE WHEN ISNULL(dbo.Punches.InTelecommute,0) = 1 THEN 'X' ELSE '' END AS Telecommute,
    					dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, dbo.Punches.ShiftDate AS Date_ToDateString, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day,   
    					MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Año, (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1)   
    					% 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName,   
    					dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END AS IsInvalid, CONVERT(nvarchar(MAX), dbo.Punches.TypeDetails)   
    					AS Details, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString,
    					dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.IDGroup, dbo.Punches.IsNotReliable, dbo.Punches.Location AS PunchLocation,   
    					dbo.Punches.LocationZone AS PunchLocationZone, dbo.Punches.FullAddress , dbo.Punches.TimeZone AS PunchTimeZone, dbo.sysroPassports.Name AS EditorName,  
    					dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString, 
    					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
    					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
    					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
    					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
    					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.Punches.ShiftDate) As UserField1,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.Punches.ShiftDate) As UserField2,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.Punches.ShiftDate) As UserField3,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.Punches.ShiftDate) As UserField4,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.Punches.ShiftDate) As UserField5,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.Punches.ShiftDate) As UserField6,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.Punches.ShiftDate) As UserField7,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.Punches.ShiftDate) As UserField8,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.Punches.ShiftDate) As UserField9,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.Punches.ShiftDate) As UserField10,
 					dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age,
					dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
          FROM         dbo.sysroEmployeeGroups with (nolock) INNER JOIN  
    					dbo.Punches with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee AND dbo.Punches.ShiftDate >= dbo.sysroEmployeeGroups.BeginDate AND   
    					dbo.Punches.ShiftDate <= dbo.sysroEmployeeGroups.EndDate INNER JOIN  
    					dbo.EmployeeContracts with (nolock) ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND   
    					dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN  
    					dbo.Zones with (nolock) ON dbo.Punches.IDZone = dbo.Zones.ID LEFT OUTER JOIN  
    					dbo.Terminals with (nolock) ON dbo.Punches.IDTerminal = dbo.Terminals.ID LEFT OUTER JOIN  
    					dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID LEFT OUTER JOIN  
    					dbo.sysroPassports with (nolock) ON dbo.Punches.IDPassport = dbo.sysroPassports.ID LEFT OUTER JOIN  
    					dbo.Causes with (nolock) ON dbo.Punches.TypeData = dbo.Causes.ID  
						LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
          WHERE dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeeGroups.IDEmployee,2,0,0,dbo.Punches.ShiftDate) > 1  
    					AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @pinitialDate and @pendDate  
          GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID,   
    					dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108),   
    					dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name,   
    					(DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name,   
    					CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(nvarchar(MAX), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path,   
    					dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,   
    					dbo.Punches.IsNotReliable, dbo.Punches.Location, dbo.Punches.LocationZone, dbo.Punches.FullAddress, dbo.Punches.TimeZone, dbo.sysroPassports.Name, dbo.Punches.ActualType,   
    					dbo.Causes.Name, dbo.Punches.TypeData, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate, dbo.Punches.InTelecommute
          HAVING (dbo.Punches.Type = 1) OR  
    					(dbo.Punches.Type = 2) OR  
    					(dbo.Punches.Type = 3) OR  
    					(dbo.Punches.Type = 7 AND (dbo.Punches.ActualType = 1 OR dbo.Punches.ActualType = 2))

GO

-- TimeSpan a fichajes ya existentes para timepo efectivo en oficina
UPDATE [dbo].[Punches] SET TimeSpan = CASE WHEN ActualType = 1 THEN datediff(second, datetime, convert(smalldatetime, '2021-01-01 00:00')) WHEN ActualType = 2 THEN datediff(second, convert(smalldatetime, '2021-01-01 00:00'), datetime) END
WHERE ShiftDate >= '20210101' 
GO

-- Nombre de fórmula en vista de tiempo efectivo
UPDATE [dbo].[GeniusViews] set [CubeLayout] =  N'{"slice":{"rows":[{"uniqueName":"WorkCenter"},{"uniqueName":"Mes"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Office_ToHours","aggregation":"sum"},{"uniqueName":"Telecommute_ToHours","aggregation":"sum"},{"uniqueName":"%","formula":"sum(\"Telecommute_ToHours\") / (sum(\"Office_ToHours\") + sum(\"Telecommute_ToHours\") )","format":"-5c5v3gxgfjf00"}],"expands":{"expandAll":true},"flatOrder":["WorkCenter","Date_ToDateString","Office_ToHours","Telecommute_ToHours"]},"options":{"grid":{"type":"classic","showTotals":"off"},"chart":{"type":"stacked_column","activeMeasure":{"uniqueName":"Fórmula #1","aggregation":"none"}}},"formats":[{"name":"-5c5v3gxgfjf00","decimalPlaces":2,"isPercent":true}],"creationDate":"2021-10-06T11:12:36.972Z"}' WHERE [DSFunction] LIKE 'Genius_EfectiveWork%'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'VisualTime.Procees.Notifier.TerminalDisconnectedTime')
	insert into sysroLiveAdvancedParameters values ('VisualTime.Procees.Notifier.TerminalDisconnectedTime', '0')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'VisualTime.Procees.Notifier.LateArrivalTime')
	insert into sysroLiveAdvancedParameters values ('VisualTime.Procees.Notifier.LateArrivalTime', '0')
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TMPReports_CriterioCausesIncidences]') AND type in (N'U'))
	CREATE TABLE [dbo].[TMPReports_CriterioCausesIncidences](
		[EmployeeID] [int] NULL,
		[EmployeeName] [nvarchar](50) NULL,
		[Contract] [nvarchar](50) NULL,
		[IDGroup] [int] NULL,
		[GroupName] [nvarchar](50) NULL,
		[Ruta] [nvarchar](max) NULL,
		[Date] [smalldatetime] NULL,
		[Incidence] [nvarchar](64) NULL,
		[Cause] [nvarchar](100) NULL,
		[StartDate] [nvarchar](8) NULL,
		[EndDate] [nvarchar](8) NULL,
		[Value] [numeric](19, 6) NULL,
		[IDReportTask] [int] NULL,
		[Remarks] [text] NULL
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TMPReports_CriterioSaldos]') AND type in (N'U'))
	CREATE TABLE [dbo].[TMPReports_CriterioSaldos](
		[EmployeeID] [int] NULL,
		[EmployeeName] [nvarchar](50) NULL,
		[Contract] [nvarchar](50) NULL,
		[IDGroup] [int] NULL,
		[GroupName] [nvarchar](50) NULL,
		[Ruta] [nvarchar](max) NULL,
		[Date] [smalldatetime] NULL,
		[Concept1] [numeric](19, 6) NULL,
		[Concept2] [numeric](19, 6) NULL,
		[Concept3] [numeric](19, 6) NULL,
		[IDReportTask] [int] NULL,
		[Remarks] [text] NULL
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
-- Nueva notificación de cambio en estado de teletrabajo
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 75)
INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,OnlySystem) VALUES(75,'Advice for change in telecommuting',null, 1, 1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notifications] WHERE ID = 1300)
INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal) VALUES(1300, 75, 'Aviso de cambio en teletrabajo','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',1)
GO

UPDATE sysroParameters SET Data='527' WHERE ID='DBVersion'
GO


 