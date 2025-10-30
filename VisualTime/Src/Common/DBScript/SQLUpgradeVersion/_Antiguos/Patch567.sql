UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

update sysropassports set showhelp=0 
GO

alter VIEW [dbo].[sysrovwDashboardEmployeeStatus]  
     AS  
                SELECT svEmployees.IdEmployee,   
                svEmployees.EmployeeName,  
                ISNULL(DailySchedule.ShiftName1,Shifts.Name) ShiftName,  
				svEmployees.IDGroup,
                tmpLastPunch.Type as LastPunchType,  
                tmpAttendance.InTelecommute as InTelecommute,  
                tmpLastPunch.ShiftDate as LastPunchDate,  
                tmpLastPunch.DateTime as LastPunchDateTime,  
                tmpLastPunch.TypeData as LastPunchTypeData,  
                Employees.Image EmployeeImage,  
                tmpAttendance.ShiftDate LastAttendanceDate,   
                tmpAttendance.DateTime as LastAttendancePunchDatetime,  
                ISNULL(Causes.Name, '') as LastAttendanceCause,  
                CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN 'Out-Calculated' ELSE CASE tmpAttendance.ActualType WHEN 1 THEN 'In' ELSE 'Out' END END as AttendanceStatus,   
                tmpTasks.DateTime as LastTaskPunchDatetime,   
          ISNULL(Tasks.Project + ' : ' +  Tasks.Name,'') as LastTaskName,    
                tmpcosts.DateTime as LastCostPunchDatetime,   
                CASE WHEN BusinessCentersPunch.Name IS NOT NULL THEN BusinessCentersPunch.Name ELSE (CASE WHEN BusinessCentersShifts.Name IS NOT NULL THEN BusinessCentersShifts.Name ELSE BusinessCentersGroups.Name END) END AS CostCenterName,  
                CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN '' ELSE tmpLastPunch.LocationZone END As LocationName,  
                CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN '' ELSE Zones.Name END As ZoneName,  
                CASE Shifts.ShiftType WHEN 2 THEN 1 ELSE 0 END AS InHolidays,  
                CASE WHEN ProgrammedAbsences.IDCause > 0 THEN 1 ELSE 0 END AS InAbsence,  
                CASE WHEN ProgrammedAbsences.IDCause > 0 THEN Causes1.Name ELSE '' END AS InAbsenceCause,  
                CASE WHEN tmpHourAbsences.IDCause > 0 THEN 1 ELSE 0 END AS InHourAbsence,  
                CASE WHEN tmpHourAbsences.IDCause > 0 THEN Causes3.Name ELSE '' END AS InHourAbsenceCause,  
                CASE WHEN tmpHoursHolidays.IDEmployee IS NOT NULL THEN 1 ELSE 0 END AS InHoursHolidays,  
                CASE WHEN Requests.IDCause > 0 THEN 1 ELSE 0 END AS AbsenceRequested,  
                CASE WHEN Requests.IDCause > 0 THEN Causes2.Name ELSE '' END AS AbsenceRequestCause,  
                CASE WHEN tmpHourAbsenceRequest.IDCause > 0 THEN 1 ELSE 0 END AS HoursAbsenceRequested,  
                CASE WHEN tmpHourAbsenceRequest.IDCause > 0 THEN Causes2.Name ELSE '' END AS HoursAbsenceRequestCause  
                FROM sysrovwcurrentemployeegroups svEmployees WITH (NOLOCK)  
                INNER JOIN Employees WITH (NOLOCK) ON Employees.Id = svEmployees.IDEmployee  
       LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY Id ORDER BY LEN(Path) DESC, LEN(PathGroup) DESC) AS 'RowNumberG', * FROM (SELECT Groups.Id IdGroup, Groups.Path PathGroup, Groups.IDCenter IdCenterGroup, GAux.Id, GAux.Path FROM Groups   
                                 INNER JOIN Groups as GAux ON GAux.Path = Groups.Path OR (GAux.Path LIKE CONCAT(Groups.Path ,'\%'))   
                                 WHERE Groups.IDCenter IS NOT NULL) GrouCentersAux) GrouCenters ON GrouCenters.ID = svEmployees.IDGroup  
                LEFT JOIN DailySchedule WITH (NOLOCK) ON DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) = DailySchedule.Date AND svEmployees.IdEmployee = DailySchedule.IDEmployee  
                LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberPunch', * FROM Punches WITH (NOLOCK) WHERE DATEDIFF(day,ShiftDate, GETDATE()) < 30  ) tmpLastPunch ON svEmployees.IDEmployee = tmpLastPunch.IdEmployee  
                LEFT JOIN Zones ON tmpLastPunch.IDZone = ZOnes.ID  
                LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberAtt', * FROM Punches WITH (NOLOCK) WHERE (ActualType = 1 or ActualType = 2) AND DATEDIFF(day,ShiftDate, GETDATE()) < 30 ) tmpAttendance ON svEmployees.IDEmployee = tmpAttendance.IdEmployee  
                LEFT JOIN Causes ON tmpAttendance.TypeData = Causes.ID AND Causes.ID >0  
                LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberTsk', * FROM Punches WITH (NOLOCK) WHERE ActualType = 4 AND DATEDIFF(day,ShiftDate, GETDATE()) < 30) tmpTasks ON svEmployees.IDEmployee = tmpTasks.IdEmployee  
                LEFT JOIN Tasks ON Tasks.Id = tmpTasks.TypeData  
                LEFT JOIN sysroLiveAdvancedParameters ON (ParameterName = 'LastPunchCenterEnabled' and convert(nvarchar,Value) = 1)   
                LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberCost', * FROM Punches WITH (NOLOCK) WHERE ActualType = 13 AND DATEDIFF(day,ShiftDate, GETDATE()) < 30) tmpCosts ON svEmployees.IDEmployee = tmpCosts.IdEmployee AND (sysroLiveAdvancedParameters.Value = 1 OR tmpCosts.ShiftDate = DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))  
                LEFT JOIN BusinessCenters BusinessCentersPunch WITH (NOLOCK) ON tmpCosts.TypeData = BusinessCentersPunch.ID              
                LEFT JOIN Shifts WITH (NOLOCK) ON Shifts.Id = DailySchedule.IDShift1  
                LEFT JOIN BusinessCenters BusinessCentersShifts WITH (NOLOCK) ON Shifts.IDCenter = BusinessCentersShifts.Id  
       LEFT JOIN BusinessCenters BusinessCentersGroups WITH (NOLOCK) ON GrouCenters.IdCenterGroup = BusinessCentersGroups.ID  
                LEFT JOIN ProgrammedAbsences WITH (NOLOCK) ON ProgrammedAbsences.IDEmployee = svEmployees.IdEmployee AND  DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) BETWEEN ProgrammedAbsences.BeginDate AND ISNULL(ProgrammedAbsences.FinishDate,DATEADD(day,ProgrammedAbsences.MaxLastingDays-1,ProgrammedAbsences.BeginDate))  
                LEFT JOIN Causes as Causes1 WITH (NOLOCK) ON Causes1.ID = ProgrammedAbsences.IDCause  
                LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Date ORDER BY idemployee ASC, Date DESC) AS 'RowNumberHourHol', * FROM ProgrammedHolidays WITH (NOLOCK)) tmpHoursHolidays ON tmpHoursHolidays.Date = DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) and tmpHoursHolidays.IDEmployee = svEmployees.IdEmployee  
                LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Date ORDER BY idemployee ASC, Date DESC) AS 'RowNumberHourAbsence', * FROM ProgrammedCauses WITH (NOLOCK)) tmpHourAbsences ON  tmpHourAbsences.IDEmployee = svEmployees.IdEmployee AND DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) BETWEEN tmpHourAbsences.Date AND tmpHourAbsences.FinishDate  
                LEFT JOIN Causes as Causes3 WITH (NOLOCK) ON Causes3.ID = tmpHourAbsences.IDCause  
                LEFT JOIN Requests WITH (NOLOCK) ON Requests.IDEmployee = svEmployees.IdEmployee AND Requests.RequestType = 7 AND DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) BETWEEN Requests.Date1 AND Requests.Date2 AND Requests.Status IN (0,1)  
                LEFT JOIN Causes as Causes2 WITH (NOLOCK) ON Causes2.ID = Requests.IDCause  
                LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Date1, Date2 ORDER BY idemployee ASC, RequestDate DESC) AS 'RowHourAbsenceRequest', * FROM Requests WITH (NOLOCK) WHERE RequestType = 9 AND Status IN (0,1)) tmpHourAbsenceRequest ON tmpHourAbsenceRequest.IDEmployee = svEmployees.IdEmployee  AND DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) BETWEEN tmpHourAbsenceRequest.Date1 AND tmpHourAbsenceRequest.Date2   
                LEFT JOIN Causes as Causes4 WITH (NOLOCK) ON Causes4.ID = tmpHourAbsenceRequest.IDCause  
                WHERE CurrentEmployee = 1  
                AND (RowNumberAtt = 1 OR RowNumberAtt IS NULL)  
                AND (RowNumberPunch = 1 OR RowNumberPunch IS NULL)  
                AND (RowNumberTsk = 1 OR RowNumberTsk IS NULL)  
                AND (RowNumberCost = 1 OR RowNumberCost IS NULL)  
                AND (RowNumberHourHol = 1 OR RowNumberHourHol IS NULL)  
       AND (RowNumberHourAbsence = 1 OR RowNumberHourAbsence IS NULL)  
                AND (RowHourAbsenceRequest = 1 OR RowHourAbsenceRequest IS NULL)  
       AND (RowNumberG = 1 OR RowNumberG IS NULL)
GO

UPDATE sysroParameters SET Data='567' WHERE ID='DBVersion'
GO