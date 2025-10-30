-- Informe de emergencia
CREATE TABLE [dbo].[TMPEmergencyTotals](
	[MeetingPoint] [nvarchar](100) NOT NULL,
	[PresentEmployees] [int] NULL,
	[PresentVisits] [int] NULL,
	[LastEntry24] [int] NULL,
	[CouldBePresents] [int] NULL,
	[Absents] [int] NULL,
	[InOtherZones] [int] NULL
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[TMPEmergencyVisits](
	[VisitPlanId] [int] NULL,
	[BeginTime] [datetime] NULL,
	[VisitorId] [int] NULL,
	[EmpVisitedId] [int] NULL,
	[MeettingPoint] [nvarchar](100) NULL
) ON [PRIMARY]
GO

ALTER VIEW [dbo].[sysrovwVisitsPresenceStatusPunches]
AS
SELECT        vp.EmpVisitedId AS IDEmployee, MAX(vm.BeginTime) AS DateTime,
                             (SELECT        TOP 1 sc.IDReader
                               FROM            dbo.sysrovwCurrentEmployeesPresenceStatusPunches sc
                               WHERE        vp.EmpVisitedId = sc.IDEmployee
                               ORDER BY sc.DateTime DESC) AS IDReader,
                             (SELECT        TOP 1 sc2.IDZone
                               FROM            dbo.sysrovwCurrentEmployeesPresenceStatusPunches sc2
                               WHERE        vp.EmpVisitedId = sc2.IDEmployee
                               ORDER BY sc2.DateTime DESC) AS IDZone, 'IN' AS Status, vs.ID AS VisitorID, vs.Name AS Visitor
FROM            dbo.VisitPlan AS vp INNER JOIN
                         dbo.Visitors AS vs ON vs.ID = vp.VisitorId INNER JOIN
                         dbo.VisitMoves AS vm ON vm.VisitPlanId = vp.ID
WHERE        (vm.BeginTime IS NOT NULL)
GROUP BY vp.EmpVisitedId, vs.Name, vp.PlannedById, vs.ID
UNION
SELECT        vp.EmpVisitedId AS IDEmployee, MAX(vm.EndTime) AS DateTime,
                             (SELECT        TOP 1 sc.IDReader
                               FROM            dbo.sysrovwCurrentEmployeesPresenceStatusPunches sc
                               WHERE        vp.EmpVisitedId = sc.IDEmployee
                               ORDER BY sc.DateTime DESC) AS IDReader,
                             (SELECT        TOP 1 sc2.IDZone
                               FROM            dbo.sysrovwCurrentEmployeesPresenceStatusPunches sc2
                               WHERE        vp.EmpVisitedId = sc2.IDEmployee
                               ORDER BY sc2.DateTime DESC) AS IDZone, 'OUT' AS Status, vs.ID AS VisitorID, vs.Name AS Visitor
FROM            dbo.VisitPlan AS vp INNER JOIN
                         dbo.Visitors AS vs ON vs.ID = vp.VisitorId INNER JOIN
                         dbo.VisitMoves AS vm ON vm.VisitPlanId = vp.ID
WHERE        (vm.EndTime IS NOT NULL)
GROUP BY vp.EmpVisitedId, vs.Name, vp.PlannedById, vs.ID

GO

UPDATE dbo.sysroParameters SET Data='367' WHERE ID='DBVersion'
GO