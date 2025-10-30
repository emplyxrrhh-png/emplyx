-- UNIDADES PRODUCTIVAS
CREATE TABLE [dbo].[ProductiveUnits](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[ShortName] [nvarchar](4) NULL,
	[Description] [nvarchar](max) NULL,
	[Color] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_ProductiveUnits] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY] 
GO

-- MODOS DE CADA UNIDAD PRODUCTIVA
CREATE TABLE [dbo].[ProductiveUnit_Modes](
	[ID] [int] NOT NULL,
	[IDProductiveUnit] [int] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[ShortName] [nvarchar](4) NULL,
	[Description] [nvarchar](max) NULL,
	[Color] [int] NOT NULL DEFAULT (0),
	[CostValue] [numeric](18, 3) NULL DEFAULT(0),
 CONSTRAINT [PK_ProductiveUnit_Modes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC

) ON [PRIMARY]
) ON [PRIMARY] 
GO

-- POSICIONES DE CADA MODO
CREATE TABLE [dbo].[ProductiveUnit_Mode_Positions](
	[ID] [int] NOT NULL,
	[IDMode] [int] NOT NULL,
	[Quantity] [smallint] NOT NULL DEFAULT(0),
	[IsExpandable] [bit] NOT NULL DEFAULT(0),
	[IDShift] [smallint] NULL DEFAULT (0),
	[StartShift] [datetime] NULL,
	[LayersDefinition] [nvarchar](max) NULL,
	[ExpectedWorkingHours] [numeric](9, 6) NULL,
	[IDAssignment] [smallint] NULL,
 CONSTRAINT [PK_ProductiveUnit_Mode_Positions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC

) ON [PRIMARY]
) ON [PRIMARY] 
GO

-- PRESUPUESTOS
CREATE TABLE [dbo].[DailyBudgets](
	[ID] numeric(16,0) NOT NULL,
	[IDNode] [int] NOT NULL,
	[IDProductiveUnit] [int] NOT NULL,
	[Date] [smalldatetime] NOT NULL,
	[IDMode] [int] NOT NULL,
	[CostValue] [numeric](18, 3) NULL DEFAULT(0),
 CONSTRAINT [PK_DailyBudgets] PRIMARY KEY CLUSTERED 
(
	[ID] ASC

) ON [PRIMARY]
) ON [PRIMARY] 
GO

-- POSICIONES DE CADA PRESUPUESTO
CREATE TABLE [dbo].[DailyBudget_Positions](
	[ID] numeric(16,0) NOT NULL,
	[IDDailyBudget] numeric(16,0) NOT NULL,
	[Quantity] [smallint] NOT NULL DEFAULT(0),
	[IsExpandable] [bit] NOT NULL DEFAULT(0),
	[IDShift] [smallint] NULL DEFAULT (0),
	[StartShift] [datetime] NULL,
	[LayersDefinition] [nvarchar](max) NULL,
	[ExpectedWorkingHours] [numeric](9, 6) NULL,
	[IDAssignment] [smallint] NULL,
 CONSTRAINT [PK_DailyBudget_Positions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC

) ON [PRIMARY]
) ON [PRIMARY] 
GO

-- ID de la posicion del presupuesto al que esta asignada la planificacion del empleado
ALTER TABLE dbo.DailySchedule ADD IDDailyBudgetPosition numeric(16,0) NULL DEFAULT(0)
GO

UPDATE dbo.DailySchedule Set IDDailyBudgetPosition = 0 WHERE IDDailyBudgetPosition IS NULL
GO


-- CAMPOS ORGANIGRAMA
ALTER TABLE dbo.sysroSecurityNodes ADD IsProductiveCenter Bit NULL DEFAULT(0)
GO

UPDATE dbo.sysroSecurityNodes Set IsProductiveCenter = 0 where IsProductiveCenter is null
GO


ALTER TABLE dbo.sysroSecurityNodes ADD IsGeographicLocation Bit NULL DEFAULT(0)
GO

UPDATE dbo.sysroSecurityNodes Set IsGeographicLocation = 0 where IsGeographicLocation is null
GO


ALTER TABLE dbo.sysroSecurityNodes ADD IDScheduleTemplate int NULL DEFAULT(0)
GO

UPDATE dbo.sysroSecurityNodes Set IDScheduleTemplate = 0 where IDScheduleTemplate is null
GO


-- Analiticas
DROP VIEW [dbo].[sysroEmployeesShifts]
GO

 CREATE VIEW [dbo].[sysroEmployeesShifts]
 AS
 SELECT        dbo.Employees.ID AS IDEmployee, dbo.Employees.Name, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.Path, 
                          dbo.Shifts.ID AS IDShift, dbo.Shifts.Name AS ShiftName, (CASE WHEN isnull(dbo.DailySchedule.IsHolidays, 0) = 1 THEN 0 ELSE isnull(dbo.DailySchedule.ExpectedWorkingHours, 
                          dbo.Shifts.ExpectedWorkingHours) END) AS ExpectedWorkingHours, dbo.DailySchedule.Date AS CurrentDate, dbo.sysroEmployeeGroups.SecurityFlags, 
                          dbo.GetFullGroupPathName(dbo.sysroEmployeeGroups.IDGroup) AS FullGroupName, dbo.Shifts.IsFloating, dbo.Shifts.StartFloating, CASE WHEN dbo.DailySchedule.Date <= GETDATE() 
                          THEN StartShiftUsed ELSE StartShift1 END AS StartFloatingOnDay, dbo.DailySchedule.IDAssignment, case when dbo.DailySchedule.IDAssignment > 0 then (select Name FROM dbo.Assignments WHERE ID=dbo.DailySchedule.IDAssignment ) else '' END As AssignmentName,
                          (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition)) as IDProductiveUnit,
                          (SELECT Name from ProductiveUnits where id in (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition))) as ProductiveUnitName


 FROM            dbo.DailySchedule INNER JOIN
                          dbo.Employees ON dbo.DailySchedule.IDEmployee = dbo.Employees.ID INNER JOIN
                          dbo.EmployeeContracts ON dbo.Employees.ID = dbo.EmployeeContracts.IDEmployee AND dbo.DailySchedule.Date >= dbo.EmployeeContracts.BeginDate AND 
                          dbo.DailySchedule.Date <= dbo.EmployeeContracts.EndDate INNER JOIN
                          dbo.sysroEmployeeGroups ON dbo.Employees.ID = dbo.sysroEmployeeGroups.IDEmployee AND dbo.DailySchedule.Date >= dbo.sysroEmployeeGroups.BeginDate AND 
                          dbo.DailySchedule.Date <= dbo.sysroEmployeeGroups.EndDate LEFT OUTER JOIN
                          dbo.Shifts ON dbo.Shifts.ID =
                              (SELECT        CASE WHEN Date <= GETDATE() THEN IDShiftUsed ELSE IDShift1 END AS Expr1
                                FROM            dbo.DailySchedule AS DS
                                WHERE        (Date = dbo.DailySchedule.Date) AND (IDEmployee = dbo.DailySchedule.IDEmployee))

GO

-- Notificacion cobertura presupuesto
IF NOT EXISTS (SELECT 1 FROM [dbo].[Notifications] WHERE ID = 903)
	INSERT INTO [dbo].[Notifications] ([ID],[IDType],[Name],[Condition],[Destination],[Schedule],[Activated],[CreatorID],[AllowPortal],[IDPassportDestination],[AllowMail],[ShowOnDesktop],[LastTaskDeleted])
     VALUES (903,54,'Cobertura insuficiente en una unidad productiva','<?xml version="1.0"?><roCollection version="2.0"><Item key="MaxDays" type="8">3</Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>',NULL,1,'SYSTEM',0,0,0,1,NULL)
GO

-- Asignacion centro de coste a unidad productiva
ALTER TABLE [dbo].[ProductiveUnits] ADD [IDCenter] [smallint] NULL DEFAULT(0)
GO
UPDATE [dbo].[ProductiveUnits] Set IDCenter = 0 WHERE IDCenter IS NULL
GO

--Permisos de unidades productivas

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =33)
BEGIN
	INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
		VALUES (33,NULL,'ProductiveUnit','Unidades Productivas','','U','RWA',0,NULL,NULL)	

	declare @usrId int
	SELECT @usrId = ID FROM sysroPassports where name = 'Administradores'

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] VALUES (@usrId,33,9)	
END
GO
	
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =33100)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(33100,33,'ProductiveUnit.Definition','Definición de unidades productivas','','U','RWA',NULL,NULL,33)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 33100, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 33	
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =34)
BEGIN
	INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
		VALUES (34,NULL,'Budgets','Presupuestos','','U','RWA',0,NULL,NULL)	

	declare @usrId int
	SELECT @usrId = ID FROM sysroPassports where name = 'Administradores'

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] VALUES (@usrId,34,9)	
END
GO
	
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =34100)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(34100,34,'Budgets.Definition','Definición de presupuestos','','U','RWA',NULL,NULL,34)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 34100, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 34	
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =34200)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(34200,34,'Budgets.Schedule','Planificación de presupuestos','','U','RWA',NULL,NULL,34)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 34200, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 34	
END
GO

--Entradas en el menu principal de presupuestos y unidades productivas

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI] WHERE [IDPath] ='Portal\ShiftControl\Budget')
BEGIN
INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES
           ('Portal\ShiftControl\Budget','Gui.Budget','AIScheduler/Budget.aspx','Budget.png',NULL,'AdvancedSecurity','Feature\HRScheduling',NULL,'104',NULL,'U:Budgets.Schedule=Read OR U:Budgets.Definition=Read')
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI] WHERE [IDPath] ='Portal\ShiftManagement\ProductiveUnit')
BEGIN
INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES
           ('Portal\ShiftManagement\ProductiveUnit','Gui.ProductiveUnit','AIScheduler/ProductiveUnit.aspx','ProductiveUnit.png',NULL,'AdvancedSecurity','Feature\HRScheduling',NULL,'104',NULL,'U:ProductiveUnit.Definition=Read')
END
GO


--Iconos barra central de unidades productivas

INSERT INTO [dbo].[sysroGUI_Actions]
			([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) 
		VALUES 
			('MaxMinimize','Portal\AIScheduling\ProductiveUnit\management','tbMaximize','Feature\HRScheduling',NULL,'MaxMinimize','btnMaximize2',0,1)
GO

INSERT INTO [dbo].[sysroGUI_Actions]
			([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) 
		VALUES 
			('New','Portal\AIScheduling\ProductiveUnit\management','tbAddNewProductiveUnit','Feature\HRScheduling','U:ProductiveUnit.Definition=Admin','newProductiveUnit()','btnTbAdd2',0,2)
GO

INSERT INTO [dbo].[sysroGUI_Actions]
			([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) 
		VALUES 
			('Delete','Portal\AIScheduling\ProductiveUnit\management','tbDelProductiveUnit','Feature\HRScheduling','U:ProductiveUnit.Definition=Admin','ShowRemoveProductiveUnit(''#ID#'')','btnTbDel2',0,3)
GO

INSERT INTO [dbo].[sysroGUI_Actions]
			([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) 
		VALUES 
			('Reports','Portal\AIScheduling\ProductiveUnit\management','tbShowReports','Feature\HRScheduling',NULL,'ShowReports','btnTbPrint2',0,4)
GO


--Iconos barra central de presupuestos / definición

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('Add','Portal\AISchedule\Budget\Definition','AddProductiveUnit','Feature\HRScheduling','U:Budgets.Definition=Write','AddProductiveUnit()','btnTbAddProductiveUnit',0,1)

GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('Remove','Portal\AISchedule\Budget\Definition','RemoveProductiveUnit','Feature\HRScheduling','U:Budgets.Definition=Write','RemoveProductiveUnit()','btnTbDelProductiveUnit',0,2)

GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('Reports','Portal\AISchedule\Budget\Definition','ShowReports','Feature\HRScheduling',NULL,'ShowReports','btnTbPrint2',0,3)

GO

--Iconos barra central de presupuestos / planificación

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('Reports','Portal\AISchedule\Budget\Schedule','ShowReports','Feature\HRScheduling',NULL,'ShowReports','btnTbPrint2',0,1)

GO

CREATE TABLE [dbo].[sysroScheduleRulesTypes](
	[ID] [smallint] NOT NULL,
	[Name] [nvarchar](32) NOT NULL,
	[System] [bit] NOT NULL,
 CONSTRAINT [PK_ScheduleRulesTypes] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)
)
GO

INSERT INTO [dbo].[sysroScheduleRulesTypes] VALUES
 (0,N'OneShiftOneDay',0),
 (1,N'RestBetweenShifts',0),
 (2,N'FreeDaysInPeriod',0),
 (3,N'MinMaxShiftsInPeriod',0),
 (4,N'MinWeekendsInPeriod',0),
 (5,N'MinMaxExpectedHours',1),
 (6,N'TwoShiftSequence',0),
 (7,N'MinMax2ShiftSequenceOnEmployee',0),
 (8,N'MaxHolidays',1),
 (9,N'WorkOnHolidays',1),
 (10,N'WorkOnWeekend',1);
 GO
 
 CREATE TABLE [dbo].[ScheduleRules](
 	[ID] [int] IDENTITY(1,1),
	[IDRule] [smallint] NOT NULL,
	[IDLabAgree] [int] NULL,
	[IDContract] [nvarchar](50) NULL,
	[Enabled] [bit] NOT NULL,
	[Weight] [smallint] NOT NULL,
	[Definition] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ScheduleRules] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
)
GO

ALTER PROCEDURE  [dbo].[Analytics_Schedule]
   	@initialDate smalldatetime,
   	@endDate smalldatetime,
   	@idpassport int,
   	@employeeFilter nvarchar(max),
   	@userFieldsFilter nvarchar(max)
    AS
    DECLARE @employeeIDs Table(idEmployee int)
    insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter,@initialDate,@endDate;
    SELECT     CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
      + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10) AS KeyView, 
      dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, 
      dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeesShifts.ShiftName, 
      dbo.sysroEmployeesShifts.CurrentDate AS Date, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours, COUNT(*) AS Count, 
      MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año, (DATEPART(dw, 
      dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear, 
      DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, 
      dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
	dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,
	dbo.sysroEmployeesShifts.IDAssignment,
	dbo.sysroEmployeesShifts.AssignmentName,
	dbo.sysroEmployeesShifts.IDProductiveUnit,
	dbo.sysroEmployeesShifts.ProductiveUnitName,
	dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
	dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
	dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
	dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
	dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
	dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),@endDate) As UserField1,
	dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),@endDate) As UserField2,
	dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),@endDate) As UserField3,
	dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),@endDate) As UserField4,
	dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),@endDate) As UserField5,
	dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),@endDate) As UserField6,
	dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),@endDate) As UserField7,
	dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),@endDate) As UserField8,
	dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),@endDate) As UserField9,
	dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),@endDate) As UserField10
		FROM         dbo.sysroEmployeesShifts with (nolock) 
		  INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.sysroEmployeeGroups.EndDate 
		 INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.EmployeeContracts.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.EmployeeContracts.EndDate 
		 LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
		WHERE  dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.sysroEmployeesShifts.IDEmployee,2,0,0,dbo.sysroEmployeesShifts.CurrentDate) > 1
	   AND dbo.sysroEmployeesShifts.IDEmployee in( select idEmployee from @employeeIDs) and dbo.sysroEmployeesShifts.CurrentDate between @initialDate and @endDate
	  GROUP BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name, 
	  dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate, MONTH(dbo.sysroEmployeesShifts.CurrentDate), 
	  YEAR(dbo.sysroEmployeesShifts.CurrentDate), (DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, 
	  dbo.sysroEmployeesShifts.CurrentDate), DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate), dbo.sysroEmployeeGroups.IDGroup, 
	  dbo.EmployeeContracts.IDContract, CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
	  + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10), dbo.sysroEmployeeGroups.Path, 
	   dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
	dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate,dbo.sysroEmployeesShifts.IDAssignment,dbo.sysroEmployeesShifts.AssignmentName,dbo.sysroEmployeesShifts.IDProductiveUnit,dbo.sysroEmployeesShifts.ProductiveUnitName
GO

ALTER PROCEDURE [dbo].[Analytics_Incidences]
    	@initialDate smalldatetime,
    	@endDate smalldatetime,
    	@idpassport int,
    	@employeeFilter nvarchar(max),
    	@userFieldsFilter nvarchar(max)
     AS
     DECLARE @employeeIDs Table(idEmployee int)
     insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter,@initialDate,@endDate;
     SELECT     CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
                           + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup, 
                           dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description AS IncidenceName, 
                           dbo.TimeZones.Name AS ZoneTime, dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS Value, YEAR(dbo.DailySchedule.Date) AS Año, 
                           (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailySchedule.Date) AS WeekOfYear, DATEPART(dy, 
                           dbo.DailySchedule.Date) AS DayOfYear, CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName, 
                           dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.FullGroupName AS FullGroupName, dbo.sysroEmployeeGroups.Path, 
                           dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,
    						dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod, dbo.DailySchedule.IDAssignment,
							case when dbo.DailySchedule.IDAssignment > 0 then (select Name FROM dbo.Assignments WHERE ID=dbo.DailySchedule.IDAssignment ) else '' END As AssignmentName,
                          (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition)) as IDProductiveUnit,
                          (SELECT Name from ProductiveUnits where id in (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition))) as ProductiveUnitName,
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
     FROM         dbo.sysroEmployeeGroups with (nolock)
    					INNER JOIN dbo.Causes with (nolock)
    					INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause 
    					INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailySchedule.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate
    					INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailySchedule.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailySchedule.Date between dbo.EmployeeContracts.BeginDate AND dbo.EmployeeContracts.EndDate 
    					LEFT OUTER JOIN dbo.sysroDailyIncidencesDescription with (nolock)
    					INNER JOIN dbo.DailyIncidences with (nolock)
    					INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID ON dbo.sysroDailyIncidencesDescription.IDIncidence = dbo.DailyIncidences.IDType ON dbo.DailyCauses.IDEmployee = dbo.DailyIncidences.IDEmployee AND dbo.DailyCauses.Date = dbo.DailyIncidences.Date AND dbo.DailyCauses.IDRelatedIncidence = dbo.DailyIncidences.ID AND dbo.DailyIncidences.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate
    					LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.DailySchedule.IDShiftUsed = dbo.Shifts.ID 
    					LEFT OUTER JOIN dbo.Shifts AS Shifts_1 with (nolock) ON dbo.DailySchedule.IDShift1 = Shifts_1.ID
    					LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
    WHERE	dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.DailyCauses.IDEmployee,2,0,0,dbo.DailyCauses.date) > 1
    			AND dbo.sysroEmployeeGroups.IDEmployee in(select idEmployee from @employeeIDs) 
   			and dbo.dailyCauses.Date between @initialDate and @endDate
     GROUP BY dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name, 
                           YEAR(dbo.DailySchedule.Date), CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
                           + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120), CASE WHEN DailySchedule.Date <= GETDATE() 
                           THEN Shifts.Name ELSE Shifts_1.Name END, dbo.Employees.Name, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, DATEPART(wk, 
                           dbo.DailySchedule.Date), DATEPART(dy, dbo.DailySchedule.Date), (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1, 
                           dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, 
                           dbo.sysroEmployeeGroups.CurrentEmployee, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate, dbo.DailySchedule.IDAssignment, dbo.DailySchedule.IDDailyBudgetPosition


GO
 

update dbo.sysroparameters set data='431' where id='dbversion'
go
