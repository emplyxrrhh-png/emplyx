CREATE PROCEDURE [dbo].[Genius_Dining_Punches]
           @initialDate smalldatetime,  
           @endDate smalldatetime,  
           @idpassport int,  
           @employeeFilter nvarchar(max),  
           @userFieldsFilter nvarchar(max)  
           AS  
           DECLARE @employeeIDs Table(idEmployee int) 
      	 DECLARE @pinitialDate smalldatetime = @initialDate,  
           @pendDate smalldatetime = @endDate,  
           @pidpassport int = @idpassport,  
           @pemployeeFilter nvarchar(max) = @employeeFilter,  
           @puserFieldsFilter nvarchar(max) = @userFieldsFilter  
           insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
           SELECT     dbo.Punches.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee,   
     					dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Punches.ActualType AS PunchDirection,   
     					dbo.Punches.TypeData AS PunchIDTypeData, CASE WHEN ISNULL(dbo.Punches.InTelecommute,0) = 1 THEN 'X' ELSE '' END AS Telecommute,
     					dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, dbo.Punches.ShiftDate AS Date_ToDateString, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day,   
     					MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Año, (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1)   
     					% 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName,   
     					dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL THEN '' ELSE 'Offline' END AS Type, CONVERT(nvarchar(MAX), dbo.Punches.TypeDetails)   
     					AS Details, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString,
     					dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.IDGroup, dbo.Punches.IsNotReliable, dbo.Punches.Location AS PunchLocation,   
     					dbo.Punches.LocationZone AS PunchLocationZone, dbo.Punches.FullAddress , dbo.Punches.TimeZone AS PunchTimeZone, dbo.sysroPassports.Name AS EditorName,  
     					dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString, 
     					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
     					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
     					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
     					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
     					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
     					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.Punches.ShiftDate) As UserField1,  
     					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.Punches.ShiftDate) As UserField2,  
     					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.Punches.ShiftDate) As UserField3,  
     					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.Punches.ShiftDate) As UserField4,  
     					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.Punches.ShiftDate) As UserField5,  
     					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.Punches.ShiftDate) As UserField6,  
     					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.Punches.ShiftDate) As UserField7,  
     					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.Punches.ShiftDate) As UserField8,  
     					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.Punches.ShiftDate) As UserField9,  
     					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.Punches.ShiftDate) As UserField10,
  					dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age,
 					dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,
					DiningRoomTurns.Name AS DiningTurn
           FROM         dbo.sysroEmployeeGroups with (nolock) INNER JOIN  
     					dbo.Punches with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee AND dbo.Punches.ShiftDate >= dbo.sysroEmployeeGroups.BeginDate AND   
     					dbo.Punches.ShiftDate <= dbo.sysroEmployeeGroups.EndDate 
						INNER JOIN DiningRoomTurns ON Punches.TypeData=DiningRoomTurns.ID
						INNER JOIN  
     					dbo.EmployeeContracts with (nolock) ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND   
     					dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN  
     					dbo.Zones with (nolock) ON dbo.Punches.IDZone = dbo.Zones.ID LEFT OUTER JOIN  
     					dbo.Terminals with (nolock) ON dbo.Punches.IDTerminal = dbo.Terminals.ID LEFT OUTER JOIN  
     					dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID LEFT OUTER JOIN  
     					dbo.sysroPassports with (nolock) ON dbo.Punches.IDPassport = dbo.sysroPassports.ID --LEFT OUTER JOIN  
     					--dbo.Causes with (nolock) ON dbo.Punches.TypeData = dbo.Causes.ID  
 						LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
           WHERE dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeeGroups.IDEmployee,2,0,0,dbo.Punches.ShiftDate) > 1  
     					AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @pinitialDate and @pendDate  
						AND Punches.Type=10
           GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID,   
     					dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108),   
     					dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name,   
     					(DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name,   
     					CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(nvarchar(MAX), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path,   
     					dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,   
     					dbo.Punches.IsNotReliable, dbo.Punches.Location, dbo.Punches.LocationZone, dbo.Punches.FullAddress, dbo.Punches.TimeZone, dbo.sysroPassports.Name, dbo.Punches.ActualType,   
     					dbo.Punches.TypeData, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate, dbo.Punches.InTelecommute, DiningRoomTurns.Name
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO


UPDATE sysroParameters SET Data='570' WHERE ID='DBVersion'
GO