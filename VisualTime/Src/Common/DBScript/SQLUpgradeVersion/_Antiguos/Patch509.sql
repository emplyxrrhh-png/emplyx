ALTER VIEW [dbo].[sysrovwCommuniquesEmployees]    
AS    
   SELECT IdCommunique, IdEmployee FROM [dbo].[CommuniqueEmployees]      
	LEFT JOIN [dbo].[Communiques] ON Communiques.Id = CommuniqueEmployees.IdCommunique      
   UNION    
   SELECT IdCommunique, sysrovwCurrentEmployeeGroups.IdEmployee FROM [dbo].[CommuniqueGroups]      
   LEFT JOIN [dbo].[Communiques] ON Communiques.Id = CommuniqueGroups.IdCommunique      
   INNER JOIN [dbo].[Groups] ON CommuniqueGroups.idgroup = Groups.id
   INNER JOIN [dbo].[sysrovwCurrentEmployeeGroups] ON (sysrovwCurrentEmployeeGroups.Path = Groups.Path OR sysrovwCurrentEmployeeGroups.Path LIKE Groups.Path + '\%')
   INNER JOIN [dbo].[sysroPassports] ON Communiques.IdCreatedBy = sysroPassports.Id 
          and (dbo.WebLogin_GetPermissionOverGroup(sysroPassports.Id, sysrovwCurrentEmployeeGroups.IdGroup, 1, 0) >=  3)  
		  and dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.Id, sysrovwCurrentEmployeeGroups.IdEmployee,1,0,1,GetDate())>= 3
   WHERE sysrovwCurrentEmployeeGroups.CurrentEmployee = 1  
GO

ALTER VIEW [dbo].[sysrovwEmployeeStatus]
   AS
 	SELECT svEmployees.IdEmployee, 
 	Employees.Image as EmployeeImage,
 	Employees.Name as EmployeeName,
  	tmpAttendance.ShiftDate AttShiftDate, 
  	tmpAttendance.DateTime as AttDatetime, 
  	CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN 'Out-Calculated' ELSE CASE tmpAttendance.ActualType WHEN 1 THEN 'In' ELSE 'Out' END END as AttStatus, 
  	tmpTasks.DateTime as TskDatetime, 
     Tasks.Project + ' : ' +  Tasks.Name as TskName,  
 	CASE WHEN BusinessCenters.Name IS NOT NULL THEN tmpcosts.DateTime ELSE (CASE WHEN BusinessCentersShifts.Name IS NOT NULL THEN DailySchedule.Date ELSE CASE WHEN [dbo].[GetEmployeeBusinessCenterNameOnDate](svEmployees.IDEmployee,GETDATE()) IS NOT NULL THEN svEmployees.BeginDate END END) END AS CostDatetime ,
  	CASE WHEN BusinessCenters.Name IS NOT NULL THEN BusinessCenters.Name ELSE (CASE WHEN BusinessCentersShifts.Name IS NOT NULL THEN BusinessCentersShifts.Name ELSE [dbo].[GetEmployeeBusinessCenterNameOnDate](svEmployees.IDEmployee,GETDATE()) END) END AS CostCenterName ,
 	CASE WHEN BusinessCenters.ID IS NOT NULL THEN BusinessCenters.ID ELSE (CASE WHEN BusinessCentersShifts.ID IS NOT NULL THEN BusinessCentersShifts.ID ELSE [dbo].[GetEmployeeBusinessCenterOnDate](svEmployees.IDEmployee,GETDATE()) END) END AS CostIDCenter
  	FROM sysrovwcurrentemployeegroups svEmployees
 	inner join Employees ON Employees.ID = svEmployees.IDEmployee
  	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberAtt', * FROM Punches WHERE (ActualType = 1 OR ActualType = 2) AND DATEDIFF(day,ShiftDate, GETDATE()) < 120) tmpAttendance ON svEmployees.IDEmployee = tmpAttendance.IdEmployee
  	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberTsk', * FROM Punches WHERE ActualType = 4 AND DATEDIFF(day,ShiftDate, GETDATE()) < 120) tmpTasks ON svEmployees.IDEmployee = tmpTasks.IdEmployee
  	left join Tasks ON Tasks.Id = tmpTasks.TypeData
 	LEFT JOIN sysroLiveAdvancedParameters ON (ParameterName = 'LastPunchCenterEnabled' and convert(nvarchar,Value) = 1) 
  	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberCost', * FROM Punches WHERE ActualType = 13) tmpCosts ON svEmployees.IDEmployee = tmpCosts.IdEmployee AND (sysroLiveAdvancedParameters.Value = 1 OR tmpCosts.ShiftDate = DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))
  	left join BusinessCenters ON tmpCosts.TypeData = BusinessCenters.ID
 	LEFT JOIN DailySchedule WITH (NOLOCK) ON DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) = DailySchedule.Date AND svEmployees.IdEmployee = DailySchedule.IDEmployee
 	LEFT JOIN Shifts WITH (NOLOCK) ON Shifts.Id = DailySchedule.IDShift1
 	LEFT JOIN BusinessCenters BusinessCentersShifts WITH (NOLOCK) ON Shifts.IDCenter = BusinessCentersShifts.Id
  	WHERE CurrentEmployee = 1
  	and (RowNumberAtt = 1 or RowNumberAtt is null)
  	and (RowNumberTsk = 1 or RowNumberTsk is null)
  	and (RowNumberCost = 1 or RowNumberCost is null)
GO
update punches set MaskAlert=null, TemperatureAlert=null where IDTerminal in (select id from terminals where type not like '%td') and (MaskAlert=0 or TemperatureAlert=0) and ShiftDate > '20200101'
GO

UPDATE sysroParameters SET Data='509' WHERE ID='DBVersion'
GO

