DROP VIEW [dbo].[sysroEmployeesShifts]
GO

CREATE VIEW [dbo].[sysroEmployeesShifts]
AS
SELECT        dbo.Employees.ID AS IDEmployee, dbo.Employees.Name, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.Path, 
                         dbo.Shifts.ID AS IDShift, dbo.Shifts.Name AS ShiftName, (CASE WHEN isnull(dbo.DailySchedule.IsHolidays, 0) = 1 THEN 0 ELSE isnull(dbo.DailySchedule.ExpectedWorkingHours, 
                         dbo.Shifts.ExpectedWorkingHours) END) AS ExpectedWorkingHours, dbo.DailySchedule.Date AS CurrentDate, dbo.sysroEmployeeGroups.SecurityFlags, 
                         dbo.GetFullGroupPathName(dbo.sysroEmployeeGroups.IDGroup) AS FullGroupName, dbo.Shifts.IsFloating, dbo.Shifts.StartFloating, CASE WHEN dbo.DailySchedule.Date <= GETDATE() 
                         THEN StartShiftUsed ELSE StartShift1 END AS StartFloatingOnDay
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

DELETE [dbo].[sysroGUI_Actions] WHERE [IDPath] = 'WeekPlan'
GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('Template','Portal\ShiftControl\Calendar\Planification','TemplateAssign','Forms\Calendar','U:Calendar.Scheduler=Write','ShowTemplateAssignWizard()','btnTbPlanS2',1,1)

GO

DROP VIEW [dbo].[sysroDailyScheduleByContract]
GO

CREATE VIEW [dbo].[sysroDailyScheduleByContract]
AS
SELECT        IDEmployee, Date, IDShift1, IDShift2, IDShift3, IDShift4, IDShiftUsed, Remarks, Status, JobStatus, LockedDay, StartShiftUsed, StartShift1, StartShift2, StartShift3, StartShift4, IDAssignment, IsCovered, 
                         OldIDAssignment, OldIDShift, IDEmployeeCovered, TaskStatus, IDShiftBase, StartShiftBase, IDAssignmentBase, IsHolidays,
                             (SELECT        TOP (1) IDContract
                               FROM            dbo.EmployeeContracts
                               WHERE        (dbo.DailySchedule.IDEmployee = IDEmployee) AND (BeginDate <= dbo.DailySchedule.Date) AND (EndDate >= dbo.DailySchedule.Date)) AS NumContrato, ExpectedWorkingHours
FROM            dbo.DailySchedule

GO

UPDATE dbo.sysroParameters SET Data='395' WHERE ID='DBVersion'
GO
