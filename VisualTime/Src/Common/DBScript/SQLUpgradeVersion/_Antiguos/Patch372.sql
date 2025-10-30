-- centros de coste
ALTER TABLE [dbo].[BusinessCenters] ADD
	[Field1] [nvarchar](MAX) NULL,
	[Field2] [nvarchar](MAX) NULL,
	[Field3] [nvarchar](MAX) NULL,
	[Field4] [nvarchar](MAX) NULL,
	Status [tinyint] NULL DEFAULT(1)
GO

UPDATE [dbo].[BusinessCenters] SET Status=1 WHERE STATUS IS NULL
GO



CREATE TABLE [dbo].[sysroFieldsBusinessCenters](
	[ID] [tinyint] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_sysroFieldsBusinessCenters] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY] 
GO

INSERT INTO [dbo].[sysroFieldsBusinessCenters] (ID, Name) VALUES(1,'Campo 1')
GO
INSERT INTO [dbo].[sysroFieldsBusinessCenters] (ID, Name) VALUES(2,'Campo 2')
GO
INSERT INTO [dbo].[sysroFieldsBusinessCenters] (ID, Name) VALUES(3,'Campo 3')
GO
INSERT INTO [dbo].[sysroFieldsBusinessCenters] (ID, Name) VALUES(4,'Campo 4')
GO

ALTER VIEW [dbo].[sysroCostControlCube]
 AS
 SELECT     CONVERT(varchar(10), dbo.DailyCauses.IDEmployee, 120) + '-' + CAST(dbo.EmployeeGroups.IDGroup AS varchar) 
                       + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailyCauses.Date, 120) AS KeyView, dbo.EmployeeGroups.IDGroup, 
                       dbo.EmployeeContracts.IDContract, dbo.DailyCauses.IDEmployee, dbo.DailyCauses.Date,
                       dbo.Causes.Name AS CauseName, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor, isnull(dbo.BusinessCenters.ID, 0) as IDCenter, isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,  isnull(dbo.BusinessCenters.Field4, '') AS Field4, dbo.DailyCauses.Value AS Value, YEAR(dbo.DailyCauses.Date) AS Año, 
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

ALTER TABLE [dbo].[EmployeeCenters] ADD 
	[BeginDate] [smalldatetime]  NULL,
	[EndDate] [smalldatetime] NULL
GO

UPDATE [dbo].[EmployeeCenters] Set BeginDate = CONVERT(SMALLDATETIME, '2015/01/01 00:00', 120) , EndDate = CONVERT(SMALLDATETIME, '2079/01/01 00:00', 120) WHERE BeginDate IS NULL
GO

CREATE TABLE [dbo].[BusinessCenterZones](
	[IDCenter] [smallint] NOT NULL,
	[IDZone] [tinyint] NOT NULL,
 CONSTRAINT [PK_BusinessCenterZones] PRIMARY KEY NONCLUSTERED 
(
	[IDCenter] ASC,
	[IDZone] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BusinessCenters] ADD 
	[AuthorizationMode] [smallint] NULL DEFAULT(0)
GO

UPDATE [dbo].[BusinessCenters] SET [AuthorizationMode] = 0 WHERE [AuthorizationMode] IS NULL
GO





-- Exportación avanzada de centros de coste
CREATE VIEW [dbo].[sysroDailyCausesByContract]
AS
SELECT        IDEmployee, Date, IDRelatedIncidence, IDCause, Value, Manual, CauseUser, CauseUserType, AccrualsRules, IsNotReliable, IDCenter, DefaultCenter, ManualCenter,
                             (SELECT        TOP (1) IDContract
                               FROM            dbo.EmployeeContracts
                               WHERE        (dbo.DailyCauses.IDEmployee = IDEmployee) AND (BeginDate <= dbo.DailyCauses.Date) AND (EndDate >= dbo.DailyCauses.Date)) AS NumContrato,
                             (SELECT        TOP (1) PERCENT CASE WHEN fixedpay = 0 THEN
                                                             (SELECT        TOP (1) (CAST(CAST(Value AS varchar(10)) AS decimal(10, 6)))
                                                               FROM            EmployeeUserFieldValues
                                                               WHERE        idEmployee = DailyCauses.idemployee AND FieldName = RIGHT(Concepts.UsedField, LEN(Concepts.usedfield) - 4) AND 
                                                                                         EmployeeUserFieldValues.DATE <= DailyCauses.date
                                                               ORDER BY date DESC) WHEN FixedPay = 1 THEN payvalue END AS Expr1
                               FROM            dbo.Concepts
                               WHERE        (ViewInPays <> 0) AND (dbo.DailyCauses.IDCause = ID)) AS CosteHora
FROM            dbo.DailyCauses

GO

INSERT INTO [dbo].[ExportGuides]
           ([ID],[Name],[Mode],[ProfileType],[ProfileMask],[ExportFileType],[DisplayParameters])
     VALUES
           (9010,'Exportación avanzada de centros de coste', 1,7,'adv_Centers',1,'1,1,0@1@1')
GO

-- Permisos para exportaciones e importaciones
ALTER TABLE dbo.ExportGuides ADD
	FeatureAliasID nvarchar(200) NULL
GO

ALTER TABLE dbo.ImportGuides ADD
	FeatureAliasID nvarchar(200) NULL
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1600,1,'Employees.DataLink','Enlace de datos','','U','RWA',NULL,NULL,1)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1600, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1610,1600,'Employees.DataLink.Imports','Importaciones','','U','RWA',NULL,NULL,1)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1610, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1600
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1611,1610,'Employees.DataLink.Imports.Employees','Importación empleados','','U','RWA',NULL,NULL,1)
GO

UPDATE [dbo].[ImportGuides] SET [RequieredFunctionalities] = 'Employees.DataLink.Imports.Employees', [FeatureAliasID] = 'Employees' WHERE ID = 1
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1611, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1610
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1612,1610,'Employees.DataLink.Imports.Company','Importación empresas','','U','RWA',NULL,NULL,1)
GO

UPDATE [dbo].[ImportGuides] SET [RequieredFunctionalities] = 'Employees.DataLink.Imports.Company', [FeatureAliasID] = 'Employees' WHERE ID = 4
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1612, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1610
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1613,1610,'Employees.DataLink.Imports.Absences','Importación de ausencias prolongadas','','U','RWA',NULL,NULL,1)
GO

UPDATE [dbo].[ImportGuides] SET [RequieredFunctionalities] = 'Employees.DataLink.Imports.Absences', [FeatureAliasID] = 'Employees' WHERE ID = 10
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1613, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1610
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1620,1600,'Employees.DataLink.Exports','Exportaciones','','U','RWA',NULL,NULL,1)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1620, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1600
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1621,1620,'Employees.DataLink.Exports.Iberper','Exportación Iberper','','U','RWA',NULL,NULL,1)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Employees.DataLink.Exports.Iberper', [FeatureAliasID] = 'Employees' WHERE ID = 8981
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1621, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1620
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1622,1620,'Employees.DataLink.Exports.LoginClass','Exportación Login class','','U','RWA',NULL,NULL,1)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Employees.DataLink.Exports.LoginClass', [FeatureAliasID] = 'Employees' WHERE ID = 8982
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1622, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1620
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1623,1620,'Employees.DataLink.Exports.Labor','Exportación Labor','','U','RWA',NULL,NULL,1)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Employees.DataLink.Exports.Labor', [FeatureAliasID] = 'Employees' WHERE ID = 8983
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1623, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1620
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1624,1620,'Employees.DataLink.Exports.AccrualsDay','Exportación de saldos diarios','','U','RWA',NULL,NULL,1)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Employees.DataLink.Exports.AccrualsDay', [FeatureAliasID] = 'Employees' WHERE ID = 9000
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1624, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1620
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1625,1620,'Employees.DataLink.Exports.AccrualsPeriod','Exportación de saldos periódicos','','U','RWA',NULL,NULL,1)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Employees.DataLink.Exports.AccrualsPeriod', [FeatureAliasID] = 'Employees' WHERE ID = 9001
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1625, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1620
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1626,1620,'Employees.DataLink.Exports.AdvAccruals','Exportación avanzada de saldos','','U','RWA',NULL,NULL,1)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Employees.DataLink.Exports.AdvAccruals', [FeatureAliasID] = 'Employees' WHERE ID = 9004
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1626, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1620
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1627,1620,'Employees.DataLink.Exports.AdvAbsence','Exportación avanzada de ausencias prolongadas','','U','RWA',NULL,NULL,1)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Employees.DataLink.Exports.AdvAbsence', [FeatureAliasID] = 'Employees' WHERE ID = 9005
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1627, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1620
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1628,1620,'Employees.DataLink.Exports.AdvDinning','Exportación avanzada de comedor','','U','RWA',NULL,NULL,1)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Employees.DataLink.Exports.AdvDinning', [FeatureAliasID] = 'Employees' WHERE ID = 9006
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1628, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1620
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1629,1620,'Employees.DataLink.Exports.AdvEmployees','Exportación avanzada de empleados','','U','RWA',NULL,NULL,1)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Employees.DataLink.Exports.AdvEmployees', [FeatureAliasID] = 'Employees' WHERE ID = 9009
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1629, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1620
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(16210,1620,'Employees.DataLink.Exports.StdEmployees','Exportación estándar','','U','RWA',NULL,NULL,1)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 16210, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1620
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(16211,1620,'Employees.DataLink.Exports.Sage','Exportación Sage','','U','RWA',NULL,NULL,1)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 16211, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1620
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2700,2,'Calendar.DataLink','Enlace de datos','','U','RWA',NULL,NULL,2)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2700, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2710,2700,'Calendar.DataLink.Imports','Importaciones','','U','RWA',NULL,NULL,2)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2710, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2700
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2711,2710,'Calendar.DataLink.Imports.Schedule','Importación de planificación','','U','RWA',NULL,NULL,2)
GO

UPDATE [dbo].[ImportGuides] SET [RequieredFunctionalities] = 'Calendar.DataLink.Imports.Schedule', [FeatureAliasID] = 'Calendar' WHERE ID = 2
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2711, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2710
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2712,2710,'Calendar.DataLink.Imports.HRSchedule','Importación de dotación teórica','','U','RWA',NULL,NULL,2)
GO

UPDATE [dbo].[ImportGuides] SET [RequieredFunctionalities] = 'Calendar.DataLink.Imports.HRSchedule', [FeatureAliasID] = 'Calendar' WHERE ID = 3
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2712, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2710
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2713,2710,'Calendar.DataLink.Imports.Assignments','Importación de puestos asignados','','U','RWA',NULL,NULL,2)
GO

UPDATE [dbo].[ImportGuides] SET [RequieredFunctionalities] = 'Calendar.DataLink.Imports.Assignments', [FeatureAliasID] = 'Calendar' WHERE ID = 6
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2713, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2710
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2714,2710,'Calendar.DataLink.Imports.Causes','Importación de justificaciones','','U','RWA',NULL,NULL,2)
GO

UPDATE [dbo].[ImportGuides] SET [RequieredFunctionalities] = 'Calendar.DataLink.Imports.Causes', [FeatureAliasID] = 'Calendar' WHERE ID = 11
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2714, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2710
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2720,2700,'Calendar.DataLink.Exports','Exportaciones','','U','RWA',NULL,NULL,2)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2720, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2700
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2721,2720,'Calendar.DataLink.Exports.Shifts','Exportación de horarios','','U','RWA',NULL,NULL,2)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Calendar.DataLink.Exports.Shifts', [FeatureAliasID] = 'Calendar' WHERE ID = 8980
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2721, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2720
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2722,2720,'Calendar.DataLink.Exports.HolidaysShifts','Exportación de vacaciones y horarios','','U','RWA',NULL,NULL,2)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Calendar.DataLink.Exports.HolidaysShifts', [FeatureAliasID] = 'Calendar' WHERE ID = 8984
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2722, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2720
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2723,2720,'Calendar.DataLink.Exports.Assignments','Exportación de puestos','','U','RWA',NULL,NULL,2)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Calendar.DataLink.Exports.Assignments', [FeatureAliasID] = 'Calendar' WHERE ID = 8986
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2723, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2720
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2724,2720,'Calendar.DataLink.Exports.CalendarNameAssignments','Exportación calendario y puestos por nombre','','U','RWA',NULL,NULL,2)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Calendar.DataLink.Exports.CalendarNameAssignments', [FeatureAliasID] = 'Calendar' WHERE ID = 8987
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2724, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2720
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2725,2720,'Calendar.DataLink.Exports.CalendarAssignments','Exportación calendario y puestos','','U','RWA',NULL,NULL,2)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Calendar.DataLink.Exports.CalendarAssignments', [FeatureAliasID] = 'Calendar' WHERE ID = 8988
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2725, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2720
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2726,2720,'Calendar.DataLink.Exports.Calendar','Exportación de calendario','','U','RWA',NULL,NULL,2)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Calendar.DataLink.Exports.Calendar', [FeatureAliasID] = 'Calendar' WHERE ID = 9002
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2726, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2720
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2727,2720,'Calendar.DataLink.Exports.Holidays','Exportación de vacaciones','','U','RWA',NULL,NULL,2)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Calendar.DataLink.Exports.Holidays', [FeatureAliasID] = 'Calendar' WHERE ID = 9003
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2727, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2720
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(25900,25,'Tasks.DataLink','Enlace de datos','','U','RWA',NULL,NULL,25)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 25900, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 25
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(25910,25900,'Tasks.DataLink.Imports','Importaciones','','U','RWA',NULL,NULL,25)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 25910, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 25900
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(25911,25910,'Tasks.DataLink.Imports.Tasks','Importación de Tareas','','U','RWA',NULL,NULL,25)
GO

UPDATE [dbo].[ImportGuides] SET [RequieredFunctionalities] = 'Tasks.DataLink.Imports.Tasks', [FeatureAliasID] = 'Tasks' WHERE ID = 5
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 25911, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 25910
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(25920,25900,'Tasks.DataLink.Exports','Exportaciones','','U','RWA',NULL,NULL,25)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 25920, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 25900
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(25921,25920,'Tasks.DataLink.Exports.Tasks','Exportación de tareas','','U','RWA',NULL,NULL,25)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Tasks.DataLink.Exports.Tasks', [FeatureAliasID] = 'Tasks' WHERE ID = 8985
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 25921, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 25920
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(25922,25920,'Tasks.DataLink.Exports.AdvTasks','Exportación avanzada de tareas','','U','RWA',NULL,NULL,25)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'Tasks.DataLink.Exports.AdvTasks', [FeatureAliasID] = 'Tasks' WHERE ID = 9008
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 25922, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 25920
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(12400,12000,'BusinessCenters.DataLink','Enlace de datos','','U','RWA',NULL,NULL,NULL)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 12400, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 12000
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(12410,12400,'BusinessCenters.DataLink.Exports','Exportaciones','','U','RWA',NULL,NULL,NULL)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 12410, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 12400
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(12411,12410,'BusinessCenters.DataLink.Exports.AdvBusinessCenter','Exportación avanzada de centros de coste','','U','RWA',NULL,NULL,NULL)
GO

UPDATE [dbo].[ExportGuides] SET [RequieredFunctionalities] = 'BusinessCenters.DataLink.Exports.AdvBusinessCenter', [FeatureAliasID] = 'Employees' WHERE ID = 9010
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 12411, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 12410
GO

UPDATE [dbo].[sysroGUI] SET [RequiredFunctionalities] = 'U:Employees.DataLink=Read OR U:Calendar.DataLink=Read OR U:Tasks.DataLink=Read OR U:BusinessCenters.DataLink=Read' WHERE IDPath = 'Portal\General\DataLink'
GO

EXEC dbo.sysro_GenerateAllPermissionsOverFeatures
GO
-- Fin permisos exportaciones e importaciones estandard.

-- Fecha de bloqueo a nivel de grupo
ALTER TABLE dbo.Groups ADD
	CloseDate smalldatetime NULL
GO

ALTER TABLE dbo.sysroSecurityParameters ADD
	CalendarLock bit NOT NULL CONSTRAINT DF_sysroSecurityParameters_CalendarLock DEFAULT 1
GO

UPDATE [dbo].[sysroSecurityParameters] SET [CalendarLock] = 1 
GO

CREATE FUNCTION [dbo].[GetCompanyGroup] 
  	(
  		@IDGroup int
  	)
  RETURNS int
  AS
  	BEGIN
  		
  		/* Get group path */
  		DECLARE @UO nvarchar(max)
  		SELECT  @UO = ISNULL(Path,'0') From Groups WHERE ID = @IDGroup
  		
  		DECLARE @Pos int, @CurrentSep int
  		SET @CurrentSep = 0
  		SELECT @Pos = CHARINDEX('\', @UO)
  		IF @Pos > 0  
		BEGIN
			SET @Pos = CHARINDEX('\', @UO)
  			SET @UO = SUBSTRING(@UO,0, @Pos)
		END
  		
  	RETURN CONVERT(int,@UO)
END
GO

CREATE FUNCTION [dbo].[IsBlockDateRestrictionActive]
  (	
 	@idPassport int,
	@idGroup int,
	@date datetime
  )
  RETURNS int
  AS
  BEGIN
    	
		DECLARE @GroupCloseDate date
		DECLARE @RestrictionDisabled int
		DECLARE @BlockDateActive int

		Set @BlockDateActive = 0

		-- Si @RestrictionDisabled = 0, no debemos aplicar la restricción de fecha
    	SELECT @RestrictionDisabled = COUNT(*) FROM sysroSecurityParameters WHERE (IDPassport IN (SELECT ID FROM GetPassportParents(@idPassport)) OR IDPassport IN (0,@idPassport)) AND CalendarLock = 0
    	   		   
		IF @RestrictionDisabled = 0 
		BEGIN
			select @GroupCloseDate = CloseDate from Groups WHERE ID = dbo.GetCompanyGroup(@idGroup)
            
			IF @GroupCloseDate IS NOT NULL 
			BEGIN
				IF @GroupCloseDate >= @date SET @BlockDateActive = 1
			END
			
		END

		return @BlockDateActive
  END
GO

ALTER FUNCTION [dbo].[WebLogin_GetPermissionOverGroup] 
   	(
   		@idPassport int,
   		@idGroup int,
   		@idApplication int,
   		@mode int --Esto no se utiliza pero se mantiene para que la definición de la función sea la misma
   	)
   RETURNS int
   AS
   BEGIN
   	DECLARE @Result int
 	DECLARE @parentPassport int
   	DECLARE @GroupType nvarchar(50)
  	
 	SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @idPassport
   		
   	if @GroupType = 'U'
   	begin
   		SET @parentPassport = @idPassport
   	end
   	else
   	begin
   		SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @idPassport
   	end
 	SELECT @Result = Permission FROM sysroPermissionsOverGroups WHERE PassportID = @parentPassport AND EmployeeGroupID = @idGroup AND EmployeeFeatureID = @idApplication
  	
	IF @Result > 3 AND @idApplication = 2 AND dbo.IsBlockDateRestrictionActive(@idPassport,@idGroup,GETDATE()) = 1 SET @Result = 3

 	RETURN @Result
   END
GO

 ALTER FUNCTION [dbo].[WebLogin_GetPassportPermissionOverGroup]
  	(
  		@idPassport int,
  		@idGroup int,
  		@idApplication int,
 		@mode int --Esto no se utiliza pero se mantiene para que la definición de la función sea la misma
  	)
  RETURNS int
  AS
  BEGIN
  	DECLARE @Result int
 	DECLARE @parentPassport int
   	DECLARE @GroupType nvarchar(50)
  	
 	SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @idPassport
   		
   	if @GroupType = 'U'
   	begin
   		SET @parentPassport = @idPassport
   	end
   	else
   	begin
   		SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @idPassport
   	end
 	SELECT @Result = Permission FROM sysroPermissionsOverGroups WHERE PassportID = @parentPassport AND EmployeeGroupID = @idGroup AND EmployeeFeatureID = @idApplication
  	
	IF @Result > 3 AND @idApplication = 2 AND dbo.IsBlockDateRestrictionActive(@idPassport,@idGroup,GETDATE()) = 1 SET @Result = 3

 	RETURN @Result
  END
GO

ALTER FUNCTION [dbo].[WebLogin_GetPermissionOverEmployee] 
    	(
    		@idPassport int,
    		@idEmployee int,
    		@idApplication int,
    		@mode int, --Esto no se utiliza pero se mantiene para que la definición de la función sea la misma
    		@includeGroups bit, --Esto no se utiliza pero se mantiene para que la definición de la función sea la misma
    		@date datetime
    	)
    RETURNS int
    AS
    BEGIN
    	DECLARE @Result int
  		DECLARE @parentPassport int
    	DECLARE @GroupType nvarchar(50)
   	
  		SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @idPassport
    		
		IF @GroupType = 'U'
		BEGIN
    		SET @parentPassport = @idPassport
		END
		ELSE
		BEGIN
    		SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @idPassport
		END

 		 SELECT @Result = isnull(sysroPermissionsOverEmployeesExceptions.Permission,sysroPermissionsOverGroups.Permission) FROM sysroPermissionsOverGroups
  			inner join (select sysrovwAllEmployeeMobilities.*, eg.StartupDate from sysrovwAllEmployeeMobilities 
 				inner join(select IDEmployee, min(beginDate) as StartupDate from sysrovwAllEmployeeMobilities where BeginDate > @date or @date between BeginDate and EndDate group by IDEmployee)eg
 				on eg.IDEmployee = sysrovwAllEmployeeMobilities.IDEmployee) em
 				on sysroPermissionsOverGroups.EmployeeGroupID=em.IDGroup and ((@date between em.BeginDate and em.EndDate) or em.StartupDate >@date) and em.IDEmployee = @idEmployee
 			left outer join sysroPermissionsOverEmployeesExceptions on em.IDEmployee=sysroPermissionsOverEmployeesExceptions.EmployeeID 
  			and sysroPermissionsOverEmployeesExceptions.PassportID = @parentPassport and sysroPermissionsOverEmployeesExceptions.EmployeeFeatureID =  @idApplication
  			where sysroPermissionsOverGroups.PassportID = @parentPassport and sysroPermissionsOverGroups.EmployeeFeatureID =  @idApplication
	
		IF @Result > 3 AND @idApplication = 2 AND dbo.IsBlockDateRestrictionActive(@idPassport,dbo.GetEmployeeGroup(@idEmployee,@date),@date) = 1 SET @Result = 3
	 		 			
  		RETURN @Result
    END
GO


-- Añadimos la dirección en fichajes de anywhere
ALTER TABLE dbo.Punches ADD
	FullAddress nvarchar(MAX) NULL
GO

--Aumentamos la longitud del campo empresa
Alter table [dbo].[EmployeeContracts]  ALTER Column [Enterprise] nvarchar(50) NULL
GO

-- Informe de identificadores biométricos
 CREATE FUNCTION [dbo].[GetEmployeeBiometricsIDLive]
  (				
  	
  )
  RETURNS @ValueTable table(idPassport integer, RXA100_0 datetime,RXA100_1 datetime, RXFFNG_0 datetime, RXFFNG_1 datetime, RXFFAC_0 datetime)
  
  AS
  BEGIN
	declare @idPassport integer
	declare @idPassportAnt integer=0
	declare @Version as nvarchar(max)
	declare @BiometricID as integer 
	declare @LenBiometricData integer 
	declare @TimeStamp as datetime
	declare @RXA100_0 datetime = null
	declare @RXA100_1 datetime = null
	declare @RXFFNG_0 datetime = null
	declare @RXFFNG_1 datetime = null
	declare @RXFFAC_0 datetime = null
	

	declare TableCursor cursor fast_forward for
		select PAM.IDPassport, PAM.Version, PAM.BiometricID, TimeStamp, (len(dbo.f_BinaryToBase64(PAM.biometricdata))) as LenBiometricData
		from sysroPassports_AuthenticationMethods PAM				
		where PAM.Method =4 and PAM.Enabled=1 And PAM.Version IN ('RXA100','RXFFNG','RXFFAC')
		order by PAM.IDPassport 

	open TableCursor
  	
  	fetch next from TableCursor into @idPassport, @Version, @BiometricID,@TimeStamp, @LenBiometricData		

	while (@@FETCH_STATUS <> -1)
	begin
		if @idPassportAnt = 0 set @idPassportAnt=@idPassport
				
		if @idPassportAnt<>@idPassport
			begin
				insert into @ValueTable values (@idPassportAnt, @RXA100_0, @RXA100_1, @RXFFNG_0, @RXFFNG_1, @RXFFAC_0)
				set @RXA100_0= null
				set @RXA100_1= null
				set @RXFFNG_0= null
				set @RXFFNG_1= null
				set @RXFFAC_0= null

				set @idPassportAnt = @idPassport
			end

		if @Version='RXA100' and @BiometricID=0 set @RXA100_0=@TimeStamp
		if @Version='RXA100' and @BiometricID=1 set @RXA100_1=@TimeStamp
		if @Version='RXFFNG' and @BiometricID=0 AND @LenBiometricData>4 set @RXFFNG_0=@TimeStamp
		if @Version='RXFFNG' and @BiometricID=1 AND @LenBiometricData>4 set @RXFFNG_1=@TimeStamp
		if @Version='RXFFAC' and @BiometricID=0 AND @LenBiometricData>4 set @RXFFAC_0=@TimeStamp
							
		fetch next from TableCursor into @idPassport, @Version, @BiometricID, @TimeStamp, @LenBiometricData
	end

	if @idPassportAnt<>0  Insert into @ValueTable values (@idPassportAnt, @RXA100_0, @RXA100_1, @RXFFNG_0, @RXFFNG_1, @RXFFAC_0)

	close TableCursor
	deallocate TableCursor
  	RETURN
  END
GO

-- Comportamiento Accesos en 1 sentido, presencia en 2
INSERT INTO dbo.sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx8',1,76,'ACCTA','1','Blind','E,S','LocalServer,ServerLocal,Server,Local','0','1,0','0','1,0','1,0')
GO

UPDATE dbo.sysroParameters SET Data='372' WHERE ID='DBVersion'
GO