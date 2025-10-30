INSERT INTO sysroGUI
([IDPath],[LanguageReference],[URL],[IconURL],[RequiredFeatures],[Priority],[RequiredFunctionalities])
VALUES ('Portal\ShiftControl\Analytics', 'GUI.Analytics', 'Scheduler/AnalyticsScheduler.aspx', 'TaskAnalytics.png', 'Forms\Calendar', 675, 'U:Calendar=Read')
GO


INSERT INTO sysroFeatures
([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes])
VALUES (2600,2,'Calendar.Analytics','Analítica de Calendario','','U','R')
GO


INSERT INTO sysroPassports_PermissionsOverFeatures
([IDPassport],[IDFeature],[Permission])
VALUES (3,2600,3)
GO


CREATE VIEW sysroScheduleCube1
AS
SELECT     RIGHT('0000000' + CAST(dbo.DailyAccruals.IDEmployee AS varchar), 7) + '-' + RIGHT('00000' + CAST(dbo.DailyAccruals.IDConcept AS varchar), 5) 
                      + '-' + + LEFT(CONVERT(VARCHAR, dbo.DailyAccruals.Date, 120), 10) AS KeyView, dbo.DailyAccruals.IDEmployee, dbo.sysrovwAllEmployeeGroups.GroupName, 
                      dbo.sysrovwAllEmployeeGroups.EmployeeName, dbo.Concepts.Name AS ConceptName, dbo.DailyAccruals.Date, SUM(dbo.DailyAccruals.Value) AS Value, COUNT(*) 
                      AS Count, MONTH(dbo.DailyAccruals.Date) AS Mes, YEAR(dbo.DailyAccruals.Date) AS Año
FROM         dbo.sysrovwAllEmployeeGroups INNER JOIN
                      dbo.DailyAccruals ON dbo.sysrovwAllEmployeeGroups.IDEmployee = dbo.DailyAccruals.IDEmployee INNER JOIN
                      dbo.Concepts ON dbo.DailyAccruals.IDConcept = dbo.Concepts.ID
GROUP BY dbo.sysrovwAllEmployeeGroups.GroupName, dbo.sysrovwAllEmployeeGroups.EmployeeName, dbo.Concepts.Name, dbo.DailyAccruals.Date, 
                      RIGHT('0000000' + CAST(dbo.DailyAccruals.IDEmployee AS varchar), 7) + '-' + RIGHT('00000' + CAST(dbo.DailyAccruals.IDConcept AS varchar), 5) 
                      + '-' + + LEFT(CONVERT(VARCHAR, dbo.DailyAccruals.Date, 120), 10), dbo.DailyAccruals.IDEmployee, MONTH(dbo.DailyAccruals.Date), YEAR(dbo.DailyAccruals.Date)
GO


CREATE VIEW sysroScheduleCube2
AS
SELECT     RIGHT('0000000' + CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar), 7) + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10) 
                      AS KeyView, dbo.sysroEmployeesShifts.IDEmployee, dbo.sysrovwAllEmployeeGroups.GroupName, dbo.sysrovwAllEmployeeGroups.EmployeeName, 
                      dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate AS Date, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours, COUNT(*) 
                      AS Count, MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año
FROM         dbo.sysrovwAllEmployeeGroups INNER JOIN
                      dbo.sysroEmployeesShifts ON dbo.sysrovwAllEmployeeGroups.IDEmployee = dbo.sysroEmployeesShifts.IDEmployee
GROUP BY dbo.sysrovwAllEmployeeGroups.GroupName, dbo.sysrovwAllEmployeeGroups.EmployeeName, dbo.sysroEmployeesShifts.ShiftName, 
                      dbo.sysroEmployeesShifts.CurrentDate, dbo.sysroEmployeesShifts.IDEmployee, MONTH(dbo.sysroEmployeesShifts.CurrentDate), 
                      YEAR(dbo.sysroEmployeesShifts.CurrentDate)
GO


CREATE VIEW sysroScheduleCube3
AS
SELECT     
rank() OVER (ORDER BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeesShifts.GroupName, dbo.sysroEmployeesShifts.Name, dbo.sysroEmployeesShifts.CurrentDate, dbo.sysroEmployeesShifts.ShiftName, 
                      dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name) as KeyView,
sysroEmployeesShifts.IDEmployee, sysroEmployeesShifts.GroupName, sysroEmployeesShifts.Name AS EmployeeName, 
                      sysroEmployeesShifts.CurrentDate AS Date, sysroEmployeesShifts.ShiftName, sysroDailyIncidencesDescription.Description AS IncidenceName, 
                      TimeZones.Name AS ZoneTime, Causes.Name AS CauseName, SUM(DailyCauses.Value) AS Value, COUNT(*) AS Count, MONTH(sysroEmployeesShifts.CurrentDate) 
                      AS Mes, YEAR(sysroEmployeesShifts.CurrentDate) AS Año
FROM         sysroEmployeesShifts INNER JOIN
                      DailyCauses ON sysroEmployeesShifts.IDEmployee = DailyCauses.IDEmployee AND sysroEmployeesShifts.CurrentDate = DailyCauses.Date INNER JOIN
                      Causes ON DailyCauses.IDCause = Causes.ID LEFT OUTER JOIN
                      DailyIncidences INNER JOIN
                      TimeZones ON DailyIncidences.IDZone = TimeZones.ID ON DailyCauses.IDEmployee = DailyIncidences.IDEmployee AND 
                      DailyCauses.Date = DailyIncidences.Date AND DailyCauses.IDRelatedIncidence = DailyIncidences.ID INNER JOIN
                      sysroDailyIncidencesDescription ON DailyIncidences.IDType = sysroDailyIncidencesDescription.IDIncidence
GROUP BY sysroEmployeesShifts.IDEmployee, sysroEmployeesShifts.GroupName, sysroEmployeesShifts.Name, sysroEmployeesShifts.CurrentDate, 
                      sysroEmployeesShifts.ShiftName, sysroDailyIncidencesDescription.Description, TimeZones.Name, Causes.Name, MONTH(sysroEmployeesShifts.CurrentDate), 
                      YEAR(sysroEmployeesShifts.CurrentDate)
GO


-- Terminales Virtuales
INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods])
 VALUES ('Virtual',1,40,'TA',NULL,'Blind','X','Local','0',0,0,'0','0','Remote',0)
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='287' WHERE ID='DBVersion'
GO

