-- Tabla y registros para fichajes multizona horaria
ALTER TABLE dbo.Punches ADD TimeZone nvarchar(100) NULL
GO
ALTER TABLE dbo.Zones ADD TimeZone nvarchar(100) NULL
GO
insert into dbo.sysroNotificationTypes values(42,'Punch with Timezone not reliable',NULL,5,'Calendar.Punches.Punches','U',1)
GO
insert into dbo.Notifications values(2101,42,'Fichajes con zonas no fiables','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>',NULL,1,'SYSTEM',0,0,0,1,NULL)
GO
-- Fin fichajes multizona horaria

-- Tabla de relacion Empleado-Autorizaciones de Acceso

CREATE TABLE [dbo].[EmployeeAccessAuthorization](
	[IDEmployee] [int] NOT NULL,
	[IDAuthorization] [smallint] NOT NULL,
 CONSTRAINT [PK_EmployeeAccessAuthorization] PRIMARY KEY CLUSTERED 
(
	[IDEmployee] ASC,
	[IDAuthorization] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[EmployeeAccessAuthorization]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeAccessAuthorization_Authorization] FOREIGN KEY([IDAuthorization])
REFERENCES [dbo].[AccessGroups] ([ID])
GO

ALTER TABLE [dbo].[EmployeeAccessAuthorization] CHECK CONSTRAINT [FK_EmployeeAccessAuthorization_Authorization]
GO

ALTER TABLE [dbo].[EmployeeAccessAuthorization]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeAccessAuthorization_Employees] FOREIGN KEY([IDEmployee])
REFERENCES [dbo].[Employees] ([ID])
GO

ALTER TABLE [dbo].[EmployeeAccessAuthorization] CHECK CONSTRAINT [FK_EmployeeAccessAuthorization_Employees]
GO

CREATE VIEW [dbo].[sysrovwAllEmployeeAccessGroups]
AS
SELECT        dbo.Employees.ID AS EmployeeID, dbo.Employees.Name AS EmployeeName, dbo.AccessGroups.ID AS AccessGroupID, 
                         dbo.AccessGroups.Name AS AccessGroupName
FROM            dbo.Employees INNER JOIN
                         dbo.AccessGroups ON dbo.AccessGroups.ID IN
                             (SELECT        dbo.Employees.IDAccessGroup
                               UNION
                               SELECT        IDAuthorization
                               FROM            dbo.EmployeeAccessAuthorization
                               WHERE        (IDEmployee = dbo.Employees.ID))
GO

CREATE VIEW [dbo].[sysroAccessAuthorizationsCube]
AS
SELECT dbo.sysroEmployeeGroups.IDEmployee AS IDEmployee, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.IDGroup AS IDGroup, dbo.sysroEmployeeGroups.FullGroupName As FullGroupName, dbo.sysroEmployeeGroups.Path AS GroupPath, 
       dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee, dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract AS IDContract, dbo.AccessGroups.Name AS AuthorizationName, 
       Zones.Name AS ZoneName, zones.IsWorkingZone AS IsWorkingZone, AccessPeriods.Name As AccessPeriodName, 1 AS BelongsToGroup
    FROM dbo.sysroEmployeeGroups 
     INNER JOIN dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
     INNER JOIN dbo.EmployeeContracts ON dbo.Employees.ID = dbo.EmployeeContracts.IDEmployee and getdate() between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
     INNER JOIN dbo.AccessGroups ON dbo.AccessGroups.ID IN
                    (SELECT        dbo.Employees.IDAccessGroup
                    UNION
                    SELECT        IDAuthorization
                    FROM            dbo.EmployeeAccessAuthorization
                    WHERE        (IDEmployee = dbo.Employees.ID))
     INNER JOIN dbo.AccessGroupsPermissions ON AccessGroupsPermissions.IDAccessGroup = AccessGroups.ID
     INNER JOIN dbo.AccessPeriods ON AccessPeriods.ID = AccessGroupsPermissions.IDAccessPeriod
     INNER JOIN dbo.Zones on Zones.ID = AccessGroupsPermissions.IDZone
GO
-- Fin  Empleado-Autorizaciones de Acceso

-- Inicio cubo fichajes para analitica de calendario

CREATE VIEW [dbo].[sysroScheduleCube4]
 AS
SELECT     dbo.Punches.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee, 
                       dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Punches.ActualType AS PunchDirection, dbo.Punches.TypeData  AS PunchIDTypeData, 
                       CASE WHEN dbo.Punches.TypeData <> 0 THEN dbo.Causes.Name ELSE '' END AS PunchTypeData, dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, 
                       CONVERT(nvarchar(8), dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day, MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Year, 
                       (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, 
                       dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName, dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL 
                       THEN 0 ELSE 1 END AS IsInvalid, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails) AS Details, dbo.sysroEmployeeGroups.Path, 
                       dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,
                       dbo.Punches.IsNotReliable AS IsNotReliable, dbo.Punches.Location AS PunchLocation, dbo.Punches.LocationZone AS PunchLocationZone, dbo.Punches.TimeZone AS PunchTimeZone, dbo.sysroPassports.Name AS EditorName
 FROM         dbo.sysroEmployeeGroups INNER JOIN
                       dbo.Punches ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee INNER JOIN
                       dbo.EmployeeContracts ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND 
                       dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                       dbo.Zones ON dbo.Punches.IDZone = dbo.Zones.ID LEFT OUTER JOIN
                       dbo.Terminals ON dbo.Punches.IDTerminal = dbo.Terminals.ID LEFT OUTER JOIN
                       dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID LEFT OUTER JOIN
                       dbo.sysroPassports ON dbo.Punches.IDPassport = dbo.sysroPassports.ID LEFT OUTER JOIN
                       dbo.Causes ON dbo.Punches.TypeData = dbo.Causes.ID
 GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID, 
                       dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108), 
                       dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name, 
                       (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name, 
                       CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path, 
                       dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, 
                       dbo.sysroEmployeeGroups.IDGroup,dbo.Punches.IsNotReliable,dbo.Punches.Location,dbo.Punches.LocationZone,
                       dbo.Punches.TimeZone,dbo.sysroPassports.Name, dbo.Punches.ActualType, dbo.Causes.Name, dbo.Punches.TypeData
 HAVING (dbo.Punches.Type = 1) OR (dbo.Punches.Type = 2) OR (dbo.Punches.Type = 3) OR (dbo.Punches.Type = 7)
GO
-- Fin cubo fichajes para analitica de calendario

-- Inicio composicion avanzada de conceptos
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TimeBagCauses_Causes]') AND parent_object_id = OBJECT_ID(N'[dbo].[ConceptCauses]'))
ALTER TABLE [dbo].[ConceptCauses] DROP CONSTRAINT [FK_TimeBagCauses_Causes]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TimeBagCauses_TimeBag]') AND parent_object_id = OBJECT_ID(N'[dbo].[ConceptCauses]'))
ALTER TABLE [dbo].[ConceptCauses] DROP CONSTRAINT [FK_TimeBagCauses_TimeBag]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_TimeBagCauses_Factor]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ConceptCauses] DROP CONSTRAINT [DF_TimeBagCauses_Factor]
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ConceptCauses_OccurrencesFactor]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ConceptCauses] DROP CONSTRAINT [DF_ConceptCauses_OccurrencesFactor]
END
GO

ALTER TABLE [dbo].[ConceptCauses] DROP CONSTRAINT [PK_TimeBagCauses]
GO

ALTER TABLE  [dbo].[ConceptCauses] ADD ID numeric(16,0) IDENTITY(1,1) NOT NULL
go
ALTER TABLE [dbo].[ConceptCauses] ADD CONSTRAINT [PK_idconceptcauses] PRIMARY KEY NONCLUSTERED ([ID]) 
GO


ALTER TABLE [dbo].[ConceptCauses]  WITH NOCHECK ADD  CONSTRAINT [FK_TimeBagCauses_TimeBag] FOREIGN KEY([IDConcept])
REFERENCES [dbo].[Concepts] ([ID])
GO

ALTER TABLE [dbo].[ConceptCauses] CHECK CONSTRAINT [FK_TimeBagCauses_TimeBag]
GO

ALTER TABLE [dbo].[ConceptCauses] ADD  CONSTRAINT [DF_TimeBagCauses_Factor]  DEFAULT (0) FOR [HoursFactor]
GO

ALTER TABLE [dbo].[ConceptCauses] ADD  CONSTRAINT [DF_ConceptCauses_OccurrencesFactor]  DEFAULT (0) FOR [OccurrencesFactor]
GO


ALTER TABLE [dbo].[ConceptCauses] ADD IDShift smallint default(0)
GO

ALTER TABLE [dbo].[ConceptCauses] ADD IDType smallint default(0)
GO

UPDATE [dbo].[ConceptCauses] SET IDShift = 0 WHERE IDShift is null
GO

UPDATE [dbo].[ConceptCauses] SET IDType = 0 WHERE IDType is null
GO

ALTER TABLE [dbo].[ConceptCauses] ADD FieldFactor nvarchar(MAX) default('')
GO

UPDATE [dbo].[ConceptCauses] SET FieldFactor = '' WHERE FieldFactor is null
GO

ALTER TABLE [dbo].[ConceptCauses] ADD TypeDayPlanned smallint default(0)
GO

UPDATE [dbo].[ConceptCauses] SET TypeDayPlanned = 0 WHERE TypeDayPlanned is null
GO
-- Fin composicion avanzada de conceptos

--Creación de indice para sysroNotificationTasks (ralentizaba proceso de Detector)
CREATE NONCLUSTERED INDEX [PX_sysroNotificationTasks_X]
ON [dbo].[sysroNotificationTasks] ([IDNotification])
INCLUDE ([ID],[Key1Numeric],[Key3DateTime])
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='337' WHERE ID='DBVersion'
GO


