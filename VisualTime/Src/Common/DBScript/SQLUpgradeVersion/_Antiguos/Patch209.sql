-- Nuevo campos para ordenar los campos de la ficha
alter table sysroUserFields add
	[Pos] [smallint] NULL
GO
--Nuevas vistas para el informe de Emergencia
create view sysrovwEmployeesInOutMoves
as
SELECT dbo.Moves.IDEmployee, MAX(dbo.Moves.InDateTime) AS DateTime, dbo.Moves.InIDReader AS IDReader,'IN' AS Status, 'Att' AS MoveType
FROM dbo.Moves 
WHERE (dbo.Moves.InDateTime IS NOT NULL) 
GROUP BY dbo.Moves.IDEmployee, dbo.Moves.InIDReader
UNION
SELECT dbo.Moves.IDEmployee, MAX(dbo.Moves.OutDateTime) AS DateTime, dbo.Moves.OutIDReader AS IDReader,'OUT' AS Status, 'Att' AS MoveType
FROM dbo.Moves 
WHERE (dbo.Moves.OutDateTime IS NOT NULL)
GROUP BY dbo.Moves.IDEmployee, dbo.Moves.OutIDReader, dbo.Moves.OutIDReader
UNION
SELECT am.IDEmployee, MAX(am.DateTime) AS DateTime, am.IDReader,CASE zo.IsWorkingZone WHEN 1 THEN 'IN' ELSE 'OUT' END AS Status, 'Acc' AS MoveType
FROM dbo.AccessMoves AS am 
	INNER JOIN dbo.Zones AS zo ON am.IDZone = zo.ID 
WHERE (DateTime IS NOT NULL)
GROUP BY am.IDEmployee, am.IDReader, zo.IsWorkingZone, am.DateTime
GO

create view sysrovwCurrentEmployeesPresenceStatus
as
SELECT DISTINCT IDEmployee, EmployeeName,
 (SELECT TOP 1 DateTime FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee ORDER BY DateTime DESC) AS DateTime,
 (SELECT TOP 1 IDReader FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee ORDER BY DateTime DESC) AS IDReader,
 (SELECT TOP 1 Status FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee ORDER BY DateTime DESC) AS Status
FROM dbo.sysrovwCurrentEmployeeGroups sc
GO
create view sysrovwVisitsInOutMoves 
AS
SELECT vp.EmpVisitedId AS IDEmployee, MAX(vm.BeginTime) AS DateTime,
 (SELECT top 1 sc.IDReader FROM dbo.sysrovwEmployeesInOutMoves sc WHERE vp.EmpVisitedId = sc.IDEmployee ORDER BY sc.DateTime DESC) AS IDReader,
'IN' AS Status, vs.Name AS Visitor
FROM dbo.VisitPlan AS vp
		INNER JOIN dbo.Visitors AS vs ON vs.ID = vp.VisitorId 
		INNER JOIN dbo.VisitMoves AS vm ON vm.VisitPlanId = vp.ID
WHERE (vm.BeginTime IS NOT NULL)
GROUP BY vp.EmpVisitedId, vs.Name, vp.PlannedById
UNION
SELECT vp.EmpVisitedId AS IDEmployee, MAX(vm.EndTime) AS DateTime,
 (SELECT top 1 sc.IDReader FROM dbo.sysrovwEmployeesInOutMoves sc WHERE vp.EmpVisitedId = sc.IDEmployee ORDER BY sc.DateTime DESC) AS IDReader,
'OUT' AS Status, vs.Name AS Visitor
FROM dbo.VisitPlan AS vp 
		INNER JOIN dbo.Visitors AS vs ON vs.ID = vp.VisitorId 
		INNER JOIN dbo.VisitMoves AS vm ON vm.VisitPlanId = vp.ID
WHERE (vm.EndTime IS NOT NULL)
GROUP BY vp.EmpVisitedId, vs.Name, vp.PlannedById
GO
create view sysrovwCurrentVisitsPresenceStatus
as
SELECT DISTINCT IDEmployee, EmployeeName,
 (SELECT TOP 1 DateTime FROM dbo.sysrovwVisitsInOutMoves WHERE (IDEmployee = dbo.sysrovwCurrentEmployeeGroups.IDEmployee) ORDER BY DateTime DESC) AS DateTime,
 (SELECT TOP 1 IDReader FROM dbo.sysrovwVisitsInOutMoves WHERE (IDEmployee = dbo.sysrovwCurrentEmployeeGroups.IDEmployee) ORDER BY DateTime DESC) AS IDReader,
 (SELECT TOP 1 Status FROM dbo.sysrovwVisitsInOutMoves WHERE (IDEmployee = dbo.sysrovwCurrentEmployeeGroups.IDEmployee) ORDER BY DateTime DESC) AS Status,
 (SELECT TOP 1 Visitor FROM dbo.sysrovwVisitsInOutMoves WHERE (IDEmployee = dbo.sysrovwCurrentEmployeeGroups.IDEmployee) ORDER BY DateTime DESC) AS Visitor
FROM dbo.sysrovwCurrentEmployeeGroups
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='209' WHERE ID='DBVersion'
GO





