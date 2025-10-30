-- No borréis esta línea
ALTER VIEW [dbo].[sysrovwEmployeeStatus]  AS  
  
WITH params AS (  
select convert(numeric(18,6),value)*-1 as maxMovementHours  
from sysroLiveAdvancedParameters  
where parametername='OptionsParameters.MaxMovementHours')  
  
SELECT svEmployees.IdEmployee, Employees.Image as EmployeeImage, Employees.Name as EmployeeName,  
tmpAttendance.ShiftDate AttShiftDate, tmpAttendance.DateTime as AttDatetime, tmpAttendance.InTelecommute,  
CASE WHEN tmpAttendance.DateTime < dateadd(hour, params.maxMovementHours, getdate()) THEN 'Out-Calculated'  
ELSE CASE tmpAttendance.ActualType WHEN 1 THEN 'In'  
ELSE 'Out' END END as AttStatus,  
tmpTasks.DateTime as TskDatetime, Tasks.Project + ' : ' +  Tasks.Name as TskName,  
CASE WHEN BusinessCenters.Name IS NOT NULL THEN tmpcosts.DateTime  
ELSE (CASE WHEN BusinessCentersShifts.Name IS NOT NULL THEN DailySchedule.Date  
ELSE CASE WHEN GrouCenters.IdCenterGroup IS NOT NULL THEN svEmployees.BeginDate END END)  
END AS CostDatetime ,  
CASE WHEN BusinessCenters.Name IS NOT NULL THEN BusinessCenters.Name  
ELSE (CASE WHEN BusinessCentersShifts.Name IS NOT NULL THEN BusinessCentersShifts.Name  
ELSE BusinessCentersGroups.Name END) END AS CostCenterName ,  
CASE WHEN BusinessCenters.ID IS NOT NULL THEN BusinessCenters.ID  
ELSE (CASE WHEN BusinessCentersShifts.ID IS NOT NULL THEN BusinessCentersShifts.ID  
ELSE GrouCenters.IdCenterGroup END) END AS CostIDCenter  
FROM params, sysrovwcurrentemployeegroups svEmployees  
inner join Employees ON Employees.ID = svEmployees.IDEmployee  
LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY Id ORDER BY LEN(Path) DESC, LEN(PathGroup) DESC) AS 'RowNumberG', * FROM (SELECT Groups.Id IdGroup, Groups.Path PathGroup, Groups.IDCenter IdCenterGroup, GAux.Id, GAux.Path FROM Groups  
INNER JOIN Groups as GAux ON GAux.Path = Groups.Path OR (GAux.Path LIKE CONCAT(Groups.Path ,'\%'))  
WHERE ISNULL(Groups.IDCenter,0) > 0) GrouCentersAux) GrouCenters ON GrouCenters.ID = svEmployees.IDGroup  
LEFT JOIN DailySchedule WITH (NOLOCK) ON DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) = DailySchedule.Date AND svEmployees.IdEmployee = DailySchedule.IDEmployee  
left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberTsk', * FROM Punches WHERE ActualType = 4 AND DATEDIFF(day,ShiftDate, GETDATE()) < 120) tmpTasks ON svEmployees.IDEmployee = tmpTasks.IdEmployee  
left join Tasks ON Tasks.Id = tmpTasks.TypeData LEFT JOIN sysroLiveAdvancedParameters ON (ParameterName = 'LastPunchCenterEnabled' and convert(nvarchar,Value) = 1)  
left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberCost', * FROM Punches WHERE ActualType = 13) tmpCosts ON svEmployees.IDEmployee = tmpCosts.IdEmployee AND (sysroLiveAdvancedParameters.Value 
= 1 OR tmpCosts.ShiftDate = DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))  
left join BusinessCenters ON tmpCosts.TypeData = BusinessCenters.ID  
left join BusinessCenters BusinessCentersGroups WITH (NOLOCK) ON GrouCenters.IdCenterGroup = BusinessCentersGroups.ID  
left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberAtt', * FROM Punches WHERE (ActualType = 1 OR ActualType = 2) AND DATEDIFF(day,ShiftDate, GETDATE()) < 120) tmpAttendance ON svEmployees.IDEmployee = tmpAttendance.IdEmployee  
LEFT JOIN Shifts WITH (NOLOCK) ON Shifts.Id = DailySchedule.IDShift1  
LEFT JOIN BusinessCenters BusinessCentersShifts WITH (NOLOCK) ON Shifts.IDCenter = BusinessCentersShifts.Id  
WHERE CurrentEmployee = 1 and (RowNumberAtt = 1 or RowNumberAtt is null) and (RowNumberTsk = 1 or RowNumberTsk is null) and (RowNumberCost = 1 or RowNumberCost is null) AND (RowNumberG = 1 OR RowNumberG IS NULL)  
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='798' WHERE ID='DBVersion'
GO
