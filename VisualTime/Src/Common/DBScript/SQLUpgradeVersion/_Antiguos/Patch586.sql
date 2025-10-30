INSERT [dbo].[sysroGUI] ([IDPath], [LanguageReference], [URL], [IconURL], [Type], [Parameters], [RequiredFeatures], [SecurityFlags], [Priority], [AllowedSecurity], [RequiredFunctionalities]) VALUES (N'Portal\ZonesStatus', N'Gui.ZonesStatus', N'/ZonesStatus', N'ZonesStatus.png', NULL, N'SaaSEnabled', NULL, NULL, 1499, NULL, N'U:Employees=Read')

GO

DROP VIEW IF EXISTS [dbo].[sysrovwEmployeesLastPunchByDate] 
GO
CREATE VIEW [dbo].[sysrovwEmployeesLastPunchByDate]
  AS
 SELECT IDEmployee, Shiftdate, Datetime, DATEDIFF(minute,Datetime,getdate())/60.0 AS Offset, ActualType, InTelecommute, IDZone, WorkCenter, IdTerminal, CASE WHEN Terminals.Type = 'LivePortal' THEN 1 ELSE 0 END IsPortal  FROM (
 SELECT row_number() OVER (PARTITION BY IdEmployee, Shiftdate ORDER BY DateTime DESC) AS 'RowNumber', IDEmployee, Shiftdate, Datetime, InTelecommute, IDZone, WorkCenter, ActualType, IdTerminal FROM Punches WITH(NOLOCK) WHERE IDEmployee > 0 
 ) AUX
 LEFT JOIN Terminals ON Terminals.ID = AUX.IdTerminal
 WHERE AUX.RowNumber = 1
GO

DROP VIEW IF EXISTS [dbo].[sysrovwLastMonthZoneMoves] 
GO
CREATE VIEW [dbo].[sysrovwLastMonthZoneMoves]  
     AS  
 SELECT PunchesIn.IDEmployee, PunchesIn.ShiftDate as ShiftDateIn, PunchesIn.DateTime as DateTimeIn, PunchesIn.IDTerminal as IDTerminalIn, PunchesIn.IDZone as IDZoneIn, PunchesIn.ActualType as ActualTypeIn, PunchesOut.ShiftDate as ShiftDateOut, PunchesOut.DateTime as DateTimeOut, PunchesOut.IDTerminal as IDTerminalOut, PunchesOut.IDZone as IDZoneOut, PunchesOut.ActualType as ActualTypeOut  FROM 
 (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY datetime asc, IdEmployee ASC) AS 'RowNumber', IDEmployee, Shiftdate, Datetime, InTelecommute, IDZone, ActualType, IdTerminal FROM Punches WITH(NOLOCK) WHERE IDEmployee > 0 AND DATEDIFF(day,ShiftDate, GETDATE()) <= 30) PunchesIn
 LEFT JOIN 
 (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY datetime asc, IdEmployee ASC) AS 'RowNumber', IDEmployee, Shiftdate, Datetime, InTelecommute, IDZone, ActualType, IdTerminal FROM Punches WITH(NOLOCK) WHERE IDEmployee > 0 AND DATEDIFF(day,ShiftDate, GETDATE()) <= 30) PunchesOut
 ON PunchesIn.RowNumber + 1 = PunchesOut.RowNumber  AND PunchesIn.IDEmployee = PunchesOut.IDEmployee
GO 

DROP VIEW IF EXISTS [dbo].[sysrovwZoneEmployeeStatus]  
GO
CREATE VIEW [dbo].[sysrovwZoneEmployeeStatus]  
     AS  
       SELECT svEmployees.IdEmployee,   
                svEmployees.EmployeeName,                  
				svEmployees.IDGroup,
                tmpLastPunch.Type as LastPunchType,  
                tmpLastPunch.ActualType as LastPunchActualType, 
                tmpAttendance.InTelecommute as InTelecommute,  
                tmpLastPunch.ShiftDate as LastPunchDate,  
                tmpLastPunch.DateTime as LastPunchDateTime,  
                tmpLastPunch.TypeData as LastPunchTypeData,  
                tmpLastPunch.IDTerminal as LastPunchIDTerminal,
                Employees.Image EmployeeImage,  
                tmpAttendance.ShiftDate LastAttendanceDate,   
                tmpAttendance.DateTime as LastAttendancePunchDatetime,  
                CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN 'Out-Calculated' ELSE CASE tmpAttendance.ActualType WHEN 1 THEN 'In' ELSE 'Out' END END as AttendanceStatus,   
                tmpTasks.DateTime as LastTaskPunchDatetime,   
                ISNULL(Tasks.Project + ' : ' +  Tasks.Name,'') as LastTaskName,    
                tmpcosts.DateTime as LastCostPunchDatetime,   
                CASE WHEN BusinessCentersPunch.Name IS NOT NULL THEN BusinessCentersPunch.Name ELSE (CASE WHEN BusinessCentersShifts.Name IS NOT NULL THEN BusinessCentersShifts.Name ELSE BusinessCentersGroups.Name END) END AS CostCenterName,                 
				CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN '' ELSE Zones.ID END As IdZone                              
       FROM (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberPunch', * FROM Punches WITH (NOLOCK) WHERE DATEDIFF(day,ShiftDate, GETDATE()) < 30  ) tmpLastPunch
	            LEFT JOIN sysrovwcurrentemployeegroups svEmployees WITH (NOLOCK)  ON svEmployees.IDEmployee = tmpLastPunch.IdEmployee
                INNER JOIN Employees WITH (NOLOCK) ON Employees.Id = svEmployees.IDEmployee  
                LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY Id ORDER BY LEN(Path) DESC, LEN(PathGroup) DESC) AS 'RowNumberG', * FROM (SELECT Groups.Id IdGroup, Groups.Path PathGroup, Groups.IDCenter IdCenterGroup, GAux.Id, GAux.Path FROM Groups   
                                 INNER JOIN Groups as GAux ON GAux.Path = Groups.Path OR (GAux.Path LIKE CONCAT(Groups.Path ,'\%'))   
                                 WHERE Groups.IDCenter IS NOT NULL) GrouCentersAux) GrouCenters ON GrouCenters.ID = svEmployees.IDGroup  
                LEFT JOIN DailySchedule WITH (NOLOCK) ON DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) = DailySchedule.Date AND svEmployees.IdEmployee = DailySchedule.IDEmployee  
                LEFT JOIN Zones ON tmpLastPunch.IDZone = ZOnes.ID  
                LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberAtt', * FROM Punches WITH (NOLOCK) WHERE (ActualType = 1 or ActualType = 2) AND DATEDIFF(day,ShiftDate, GETDATE()) < 30 ) tmpAttendance ON svEmployees.IDEmployee = tmpAttendance.IdEmployee  
                LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberTsk', * FROM Punches WITH (NOLOCK) WHERE ActualType = 4 AND DATEDIFF(day,ShiftDate, GETDATE()) < 30) tmpTasks ON svEmployees.IDEmployee = tmpTasks.IdEmployee  
                LEFT JOIN Tasks ON Tasks.Id = tmpTasks.TypeData  
                LEFT JOIN sysroLiveAdvancedParameters ON (ParameterName = 'LastPunchCenterEnabled' and convert(nvarchar,Value) = 1)   
                LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberCost', * FROM Punches WITH (NOLOCK) WHERE ActualType = 13 AND DATEDIFF(day,ShiftDate, GETDATE()) < 30) tmpCosts ON svEmployees.IDEmployee = tmpCosts.IdEmployee AND (sysroLiveAdvancedParameters.Value = 1 OR tmpCosts.ShiftDate = DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))  
                LEFT JOIN BusinessCenters BusinessCentersPunch WITH (NOLOCK) ON tmpCosts.TypeData = BusinessCentersPunch.ID              
                LEFT JOIN Shifts WITH (NOLOCK) ON Shifts.Id = DailySchedule.IDShift1  
                LEFT JOIN BusinessCenters BusinessCentersShifts WITH (NOLOCK) ON Shifts.IDCenter = BusinessCentersShifts.Id  
                LEFT JOIN BusinessCenters BusinessCentersGroups WITH (NOLOCK) ON GrouCenters.IdCenterGroup = BusinessCentersGroups.ID                  
       WHERE CurrentEmployee = 1  
                AND (RowNumberAtt = 1 OR RowNumberAtt IS NULL)  
                AND (RowNumberPunch = 1 OR RowNumberPunch IS NULL)  
                AND (RowNumberTsk = 1 OR RowNumberTsk IS NULL)  
                AND (RowNumberCost = 1 OR RowNumberCost IS NULL)                  
                AND (RowNumberG = 1 OR RowNumberG IS NULL)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO
UPDATE sysroParameters SET Data='586' WHERE ID='DBVersion'
GO
