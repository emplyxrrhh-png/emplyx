
-- Añadido soporte para sirenas	
--======================================
ALTER TABLE dbo.sysroReaderTemplates ADD
	SupportedSirens nvarchar(20) NULL
GO
ALTER TABLE dbo.sysroReaderTemplates SET (LOCK_ESCALATION = TABLE)
GO

UPDATE [dbo].[sysroReaderTemplates] SET [SupportedSirens] = '1'
 WHERE Type not in ('LivePortal','masterASP','SAIWALL','Virtual')
GO
--======================================

-- AGREGAR IdConcept EN LA VISTA DE ANALITICA DE CALENDARIO
 ALTER VIEW [dbo].[sysroScheduleCube1] AS
 SELECT RIGHT('0000000' + CAST(dbo.DailyAccruals.IDEmployee AS varchar), 7) + '-' + RIGHT('00000' + CAST(dbo.DailyAccruals.IDConcept AS varchar), 5) 
        + '-' + + LEFT(CONVERT(VARCHAR, dbo.DailyAccruals.Date, 120), 10) AS KeyView, 
		dbo.DailyAccruals.IDEmployee, dbo.DailyAccruals.IDConcept, dbo.sysrovwAllEmployeeGroups.GroupName, dbo.sysrovwAllEmployeeGroups.EmployeeName, dbo.Concepts.Name AS ConceptName, 
		dbo.DailyAccruals.Date, SUM(dbo.DailyAccruals.Value) AS Value, COUNT(*) AS Count, MONTH(dbo.DailyAccruals.Date) AS Mes, YEAR(dbo.DailyAccruals.Date) AS Año,
		DATEPART(dw, dbo.DailyAccruals.Date) AS DayOfWeek, DATEPART(wk, dbo.DailyAccruals.Date) AS WeekOfYear,  DATEPART(dy, dbo.DailyAccruals.Date) AS DayOfYear
 FROM         dbo.sysrovwAllEmployeeGroups INNER JOIN
                        dbo.DailyAccruals ON dbo.sysrovwAllEmployeeGroups.IDEmployee = dbo.DailyAccruals.IDEmployee INNER JOIN
                        dbo.Concepts ON dbo.DailyAccruals.IDConcept = dbo.Concepts.ID
 GROUP BY dbo.sysrovwAllEmployeeGroups.GroupName, dbo.sysrovwAllEmployeeGroups.EmployeeName, dbo.Concepts.Name, dbo.DailyAccruals.Date, 
                        RIGHT('0000000' + CAST(dbo.DailyAccruals.IDEmployee AS varchar), 7) + '-' + RIGHT('00000' + CAST(dbo.DailyAccruals.IDConcept AS varchar), 5) 
                        + '-' + + LEFT(CONVERT(VARCHAR, dbo.DailyAccruals.Date, 120), 10), dbo.DailyAccruals.IDEmployee, dbo.DailyAccruals.IDConcept, MONTH(dbo.DailyAccruals.Date), YEAR(dbo.DailyAccruals.Date)
GO


-- AGREGAR ConceptS EN LA TABLA DE VISTAS DE ANALITICA DE DATOS
--===============================================================
ALTER TABLE [dbo].[sysroSchedulerViews]
ADD [Concepts] nvarchar(4000) NULL DEFAULT('')
GO

UPDATE sysroSchedulerViews SET Concepts = ''
GO


-- VISTA OPTIMIZADA DE ANALITICA DE CALENDARIO
--==============================================
ALTER VIEW [dbo].[sysroScheduleCube3] AS

SELECT     CONVERT(varchar(10), DailySchedule.IDEmployee, 120) + CONVERT(varchar(10), DailySchedule.Date, 120) AS KeyView, DailySchedule.IDEmployee, 
                      DailySchedule.Date, sysroDailyIncidencesDescription.Description AS IncidenceName, TimeZones.Name AS ZoneTime, Causes.Name AS CauseName, 
                      SUM(DailyCauses.Value) AS Value, YEAR(DailySchedule.Date) AS Año, 
                      DATEPART(dw, DailySchedule.Date) AS DayOfWeek, DATEPART(wk, DailySchedule.Date) AS WeekOfYear, DATEPART(dy, DailySchedule.Date) AS DayOfYear, 
                      CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName, Employees.Name AS EmployeeName, Groups.Name AS GroupName
FROM         EmployeeGroups INNER JOIN
                      Causes INNER JOIN
                      DailyCauses ON Causes.ID = DailyCauses.IDCause INNER JOIN
                      DailySchedule ON DailyCauses.IDEmployee = DailySchedule.IDEmployee AND DailyCauses.Date = DailySchedule.Date ON 
                      EmployeeGroups.IDEmployee = DailySchedule.IDEmployee AND EmployeeGroups.BeginDate <= DailySchedule.Date AND 
                      EmployeeGroups.EndDate >= DailySchedule.Date INNER JOIN
                      EmployeeContracts ON DailySchedule.IDEmployee = EmployeeContracts.IDEmployee AND DailySchedule.Date >= EmployeeContracts.BeginDate AND 
                      DailySchedule.Date <= EmployeeContracts.EndDate LEFT OUTER JOIN
                      Groups ON EmployeeGroups.IDGroup = Groups.ID LEFT OUTER JOIN
                      Employees ON DailySchedule.IDEmployee = Employees.ID LEFT OUTER JOIN
                      sysroDailyIncidencesDescription INNER JOIN
                      DailyIncidences INNER JOIN
                      TimeZones ON DailyIncidences.IDZone = TimeZones.ID ON sysroDailyIncidencesDescription.IDIncidence = DailyIncidences.IDType ON 
                      DailyCauses.IDEmployee = DailyIncidences.IDEmployee AND DailyCauses.Date = DailyIncidences.Date AND 
                      DailyCauses.IDRelatedIncidence = DailyIncidences.ID LEFT OUTER JOIN
                      Shifts ON DailySchedule.IDShiftUsed = Shifts.ID LEFT OUTER JOIN
                      Shifts AS Shifts_1 ON DailySchedule.IDShift1 = Shifts_1.ID
GROUP BY DailySchedule.IDEmployee, DailySchedule.Date, sysroDailyIncidencesDescription.Description, TimeZones.Name, Causes.Name, YEAR(DailySchedule.Date), 
                      CONVERT(varchar(10), DailySchedule.IDEmployee, 120) + CONVERT(varchar(10), DailySchedule.Date, 120), CASE WHEN DailySchedule.Date <= GETDATE() 
                      THEN Shifts.Name ELSE Shifts_1.Name END, Employees.Name, Groups.Name
GO

-- Creamos tabla para biometria de terminales ZK   (rxC, rxCe)
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmployeeBiometricDataZK](
	[IDEmployee] [int] NOT NULL,
	[IDFinger] [smallint] NOT NULL,
	[Data] [image] NULL,
	[TimeStamp] [smalldatetime] NULL,
	[IDTerminal] [int] NULL,
 CONSTRAINT [PK_EmployeeBiometricDataZK] PRIMARY KEY CLUSTERED 
(
	[IDEmployee] ASC,
	[IDFinger] ASC
) ON [PRIMARY]
) ON [PRIMARY] 
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='299' WHERE ID='DBVersion'
GO
