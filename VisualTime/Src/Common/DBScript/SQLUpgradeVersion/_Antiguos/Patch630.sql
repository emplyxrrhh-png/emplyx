ALTER PROCEDURE [dbo].[Genius_Requests_Schedule]
          @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @requestTypesFilter nvarchar(max) AS        
  		
  		DECLARE @employeeIDs Table(idEmployee int)        
  		DECLARE @requestTypeFilter Table(idRequest int)        
   		DECLARE @SecurityMode int
   		
  		SELECT @SecurityMode = convert(numeric(3),isnull(value,'1')) from sysroLiveAdvancedParameters where ParameterName = 'SecurityMode'
          insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter,@initialDate,@endDate;        
  		insert into @requestTypeFilter select * from dbo.SplitToInt(@requestTypesFilter,',');         
  		SELECT dbo.requests.ID AS KeyView, isnull(case when aux.NumeroDias > 0 then aux.NumeroDias else DATEDIFF(day,dbo.Requests.Date1, dbo.Requests.Date2) + 1 end,0) as NumeroDias, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee,         
  				dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Requests.RequestDate as 'Date', dbo.Requests.RequestDate as 'Date_ToDateString',
  				dbo.Requests.Status AS RequestsStatus,         
  				Case WHEN Requests.Status IN(2,3) THEN (SELECT top 1 isnull(sysropassports.Name,'') from RequestsApprovals inner join sysroPassports on RequestsApprovals.IDPassport = sysroPassports.ID  where IDRequest = Requests.ID AND Status in(2,3) ) ELSE '---' END as 'ApproveRefusePassport',        
  				Case WHEN Requests.Status IN(0,1) Then (select dbo.GetRequestNextLevelPassports(Requests.ID,@SecurityMode)) ELSE '---' END as 'PendingPassport',        
  				dbo.Requests.RequestType, isnull(CONVERT(NVARCHAR(4000), dbo.Requests.Date1, 120),'') as 'BeginDateRequest', isnull(CONVERT(NVARCHAR(4000), dbo.Requests.FromTime, 8),'') as 'BeginHourRequest', 
  				isnull(CONVERT(NVARCHAR(4000), isnull(dbo.Requests.Date2,dbo.Requests.Date1), 120),'') as 'EndDateRequest',        
  				isnull(CONVERT(NVARCHAR(4000), dbo.Requests.ToTime, 8),'') as 'EndHourRequest',        
  				ISNULL((SELECT Name from dbo.Causes WHERE ID = dbo.Requests.IDCause),'') as 'CauseNameRequest',        
  				ISNULL((SELECT Name from dbo.Shifts WHERE ID = dbo.Requests.IDShift),'') as 'ShiftNameRequest',        
  				CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')) as CommentsRequest,        
  				CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldName, '')) as FieldNameRequest, 
  				CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldValue, '')) as FieldValueRequest, 
                  CONVERT(nvarchar(8), dbo.Requests.RequestDate, 108) AS Time, DAY(dbo.Requests.RequestDate) AS Day,         
                  MONTH(dbo.Requests.RequestDate) AS Month, YEAR(dbo.Requests.RequestDate) AS Año, (DATEPART(dw, dbo.Requests.RequestDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 
  				1 AS Value, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString,      
                  dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.IDGroup,         
  				dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @endDate AS EndPeriod, @endDate AS EndPeriod_ToDateString,
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,      
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,        
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,      
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,        
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,      
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,        
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,      
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,        
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,      
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),dbo.Requests.RequestDate) As UserField1,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),dbo.Requests.RequestDate) As UserField2,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),dbo.Requests.RequestDate) As UserField3,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),dbo.Requests.RequestDate) As UserField4,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),dbo.Requests.RequestDate) As UserField5,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),dbo.Requests.RequestDate) As UserField6,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),dbo.Requests.RequestDate) As UserField7,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),dbo.Requests.RequestDate) As UserField8,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),dbo.Requests.RequestDate) As UserField9,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),dbo.Requests.RequestDate) As UserField10,
 				dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age,        
				dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
          FROM dbo.sysroEmployeeGroups with (nolock) 
  				INNER JOIN dbo.requests with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.requests.IDEmployee AND dbo.requests.RequestDate >= dbo.sysroEmployeeGroups.BeginDate AND dbo.requests.RequestDate <= dbo.sysroEmployeeGroups.EndDate 
  				INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.requests.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.requests.RequestDate >= dbo.EmployeeContracts.BeginDate AND dbo.requests.RequestDate <= dbo.EmployeeContracts.EndDate 
  				LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
   				left join (SELECT IDRequest, COUNT(*) NumeroDias FROM sysroRequestDays GROUP BY IDRequest) aux on aux.IDRequest = requests.id
				LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
          WHERE dbo.GetRequestPassportPermission(@idpassport, Requests.ID) > 1        
  				AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.requests.RequestDate between @initialDate and @endDate        
  				AND dbo.Requests.RequestType in (select idRequest from @requestTypeFilter)      
  		GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.requests.RequestDate), YEAR(dbo.requests.RequestDate), dbo.requests.ID,         
  					dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.requests.RequestDate), dbo.requests.RequestDate, CONVERT(nvarchar(8), dbo.requests.RequestDate, 108),         
  					dbo.requests.RequestDate, dbo.EmployeeContracts.IDContract,
  					dbo.Employees.Name, (DATEPART(dw, dbo.requests.RequestDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.requests.RequestType, dbo.requests.Status, dbo.requests.Date1, dbo.requests.FromTime, dbo.requests.Date2, dbo.requests.ToTime, dbo.requests.IDCause,CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldName, '')),CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldValue, '')), dbo.requests.IDShift,CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')), 
  					dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate , aux.NumeroDias, dbo.sysrovwEmployeeLockDate.LockDate
          HAVING  (dbo.Requests.RequestType IN(1,2,3,4,5,6,7,8,9,11,14,15,17))

GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='630' WHERE ID='DBVersion'
GO
