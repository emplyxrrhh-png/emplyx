
ALTER PROCEDURE [dbo].[Genius_Supervisors]
@initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max) AS
DECLARE @employeeIDs Table(idEmployee int)
DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter

insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter, @pinitialDate, @pendDate;

SELECT DBO.EMPLOYEES.Name as Supervisor, DBO.sysroGroupFeatures.Name as Rol,
QL.LevelOfAuthority as 'QualityLevel', TM.LevelOfAuthority as 'TimeManagementLevel', L.LevelOfAuthority as 'LaborLevel',
P.LevelOfAuthority as 'PlanningLevel', PR.LevelOfAuthority as 'PreventionLevel', S.LevelOfAuthority as 'SecurityLevel',
GR.FullGroupName, SUPEEMPINFO.Name AS EmployeeName, 1 AS Enabled,
dbo.GetEmployeeUserFieldValueMin(SUPEMP.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),@endDate) As UserField1,
dbo.GetEmployeeUserFieldValueMin(SUPEMP.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),@endDate) As UserField2,
dbo.GetEmployeeUserFieldValueMin(SUPEMP.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),@endDate) As UserField3,
dbo.GetEmployeeUserFieldValueMin(SUPEMP.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),@endDate) As UserField4,
dbo.GetEmployeeUserFieldValueMin(SUPEMP.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),@endDate) As UserField5,
dbo.GetEmployeeUserFieldValueMin(SUPEMP.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),@endDate) As UserField6,
dbo.GetEmployeeUserFieldValueMin(SUPEMP.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),@endDate) As UserField7,
dbo.GetEmployeeUserFieldValueMin(SUPEMP.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),@endDate) As UserField8,
dbo.GetEmployeeUserFieldValueMin(SUPEMP.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),@endDate) As UserField9,
dbo.GetEmployeeUserFieldValueMin(SUPEMP.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@endDate) As UserField10
FROM DBO.sysroPassports
INNER JOIN DBO.Employees ON DBO.sysroPassports.IDEmployee = DBO.EMPLOYEES.ID
INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature as pef ON pef.IdFeature = 1 and @pendDate between PEF.BeginDate and PEF.EndDate and PEF.idpassport = @pidpassport and PEF.idemployee = DBO.EMPLOYEES.ID and PEF.FeaturePermission > 0
inner join sysroGroupFeatures on DBO.sysroPassports.IDGroupFeature = DBO.sysroGroupFeatures.ID
INNER JOIN DBO.EmployeeContracts ON DBO.EmployeeContracts.IDEmployee = DBO.Employees.ID
LEFT OUTER JOIN (SELECT * FROM DBO.sysroPassports_Categories
LEFT OUTER JOIN DBO.sysroCategoryTypes ON DBO.sysroCategoryTypes.ID = DBO.sysroPassports_Categories.IDCategory
where dbo.sysroCategoryTypes.Description = 'Quality') as QL on ql.IDPassport = DBO.sysroPassports.ID
LEFT OUTER JOIN (SELECT * FROM DBO.sysroPassports_Categories
LEFT OUTER JOIN DBO.sysroCategoryTypes ON DBO.sysroCategoryTypes.ID = DBO.sysroPassports_Categories.IDCategory
where dbo.sysroCategoryTypes.Description = 'Attendance Control') as TM on TM.IDPassport = DBO.sysroPassports.ID
LEFT OUTER JOIN (SELECT * FROM DBO.sysroPassports_Categories
LEFT OUTER JOIN DBO.sysroCategoryTypes ON DBO.sysroCategoryTypes.ID = DBO.sysroPassports_Categories.IDCategory
where dbo.sysroCategoryTypes.Description = 'Labor') as L on L.IDPassport = DBO.sysroPassports.ID
LEFT OUTER JOIN (SELECT * FROM DBO.sysroPassports_Categories
LEFT OUTER JOIN DBO.sysroCategoryTypes ON DBO.sysroCategoryTypes.ID = DBO.sysroPassports_Categories.IDCategory
where dbo.sysroCategoryTypes.Description = 'Planning') as P on P.IDPassport = DBO.sysroPassports.ID
LEFT OUTER JOIN (SELECT * FROM DBO.sysroPassports_Categories
LEFT OUTER JOIN DBO.sysroCategoryTypes ON DBO.sysroCategoryTypes.ID = DBO.sysroPassports_Categories.IDCategory
where dbo.sysroCategoryTypes.Description = 'Prevention') as PR on PR.IDPassport = DBO.sysroPassports.ID
LEFT OUTER JOIN (SELECT * FROM DBO.sysroPassports_Categories
LEFT OUTER JOIN DBO.sysroCategoryTypes ON DBO.sysroCategoryTypes.ID = DBO.sysroPassports_Categories.IDCategory
where dbo.sysroCategoryTypes.Description = 'Security') as S on S.IDPassport = DBO.sysroPassports.ID
LEFT JOIN DBO.sysrovwSecurity_PermissionOverEmployeeAndFeature AS SUPEMP ON SUPEMP.IdFeature = 1 AND SUPEMP.IdPassport = DBO.sysroPassports.ID AND @endDate BETWEEN SUPEMP.BeginDate AND SUPEMP.EndDate
LEFT OUTER JOIN DBO.EmployeeGroups AS SUPEEMPGR ON SUPEEMPGR.IDEmployee = SUPEMP.IdEmployee
LEFT OUTER JOIN DBO.EmployeeContracts AS SUPEMPCT ON SUPEMPCT.IDEmployee = SUPEMP.IDEmployee
LEFT OUTER JOIN Groups AS GR ON GR.ID = SUPEEMPGR.IDGroup
LEFT OUTER JOIN DBO.EMPLOYEES AS SUPEEMPINFO ON SUPEEMPINFO.ID = SUPEMP.IdEmployee
WHERE IsSupervisor = 1 AND FullGroupName IS NOT NULL
AND DBO.EMPLOYEES.Id in(SELECT idEmployee FROM @employeeIDs) and @pinitialDate >= DBO.EmployeeContracts.BeginDate and @pendDate <= DBO.EmployeeContracts.EndDate
and @pinitialDate >= SUPEMPCT.BeginDate and @pendDate <= SUPEMPCT.EndDate
and @pinitialDate >= DBO.EmployeeContracts.BeginDate and @pendDate <= DBO.EmployeeContracts.EndDate
and @pinitialDate >= SUPEEMPGR.BeginDate and @pendDate <= SUPEEMPGR.EndDate
group by DBO.EMPLOYEES.Name, DBO.sysroGroupFeatures.Name, QL.LevelOfAuthority, TM.LevelOfAuthority, L.LevelOfAuthority,
P.LevelOfAuthority, PR.LevelOfAuthority, S.LevelOfAuthority, GR.FullGroupName, SUPEEMPINFO.Name,
DBO.sysroPassports.IDEmployee, SUPEMP.IdEmployee


GO

 ALTER PROCEDURE  [dbo].[Genius_Users]
             @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @requestTypesFilter nvarchar(max) AS      
     		DECLARE @employeeIDs Table(idEmployee int)      
			DECLARE @requestTypeFilter Table(idRequest int)
     		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter       
    		DECLARE @employeeIDFilter NVARCHAR(MAX)
    		DECLARE @featureNames Table(fieldName nvarchar(max), requestTypeId int)
    		DECLARE @featureFields NVARCHAR(max)
    		DECLARE @securityVersion int = 1
    		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate
			insert into @requestTypeFilter select * from dbo.SplitToInt(@requestTypesFilter,',');
    		select @employeeIDFilter = substring(tmp.empIds,2, len(tmp.empIds) -1) from
    			(select convert(nvarchar(max), (select ',' + convert(nvarchar(max),idemployee) as [data()] from @employeeIDs FOR XML PATH(''))) as empIds) tmp
    		IF @requestTypesFilter = ''
    			SET @requestTypesFilter = '-1'
    		IF EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'SecurityMode')
    			select @securityVersion = convert(int,value) from [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'SecurityMode'
    			
    		
    		
    		insert into @featureNames select type, IdType from sysroRequestType where IdType in (select idRequest from @requestTypeFilter)

			
    		
    			select @featureFields = isnull(tmp.empIds,'') from
    			(select convert(nvarchar(max), (select ', (select STRING_AGG(CONVERT(nvarchar(4000), sp.name), '','') AS ConcatenatedIds from dbo.sysrofnSecurity_PermissionOverRequestTypes(emp.ID,' + cast(requestTypeId as nvarchar(max)) + ') as pr inner join sysroPassports as sp on sp.ID = pr.IdPassport) AS [sup' + fieldName + ']' as [data()] from @featureNames FOR XML PATH(''))) as empIds) tmp
    		
    		
    		print @featureFields
    	DECLARE @SqlStatement NVARCHAR(MAX)
    	SET @SqlStatement = N'
     		select * from (  
     			select CAST(emp.ID AS varchar) + ''-'' + CAST(eg.IDGroup AS varchar) + ''-'' + ec.IDContract AS KeyView,
     			emp.Id as IDEmployee, emp.Name as EmployeeName, spam.method as Method, spam.credential as Credential, 
 				CASE WHEN VERSION <> ''RXA200'' THEN spam.version 
 				ELSE '''' END as Version,     
     			CASE WHEN len(CONVERT(VARCHAR(MAX), CONVERT(VARBINARY(MAX),spam.BiometricData))) > 0 THEN ''X'' ELSE '''' END AS BiometricData,    
     			spam.Enabled as Enabled, 
 				CASE WHEN (VERSION = '''' OR VERSION = ''RXA200'') AND METHOD = 4 THEN NULL
 				ELSE spam.TimeStamp END as TimeStamp, 
 				CASE WHEN (VERSION = '''' OR VERSION = ''RXA200'') AND METHOD = 4 THEN NULL
 				ELSE spam.TimeStamp END AS TimeStamp_ToDateString, t.Description as Terminal, spam.BiometricAlgorithm as BiometricAlgorithm,     
     			g.name as GroupName, g.FullGroupName, ec.IDContract, ec.BeginDate as BeginContract, ec.BeginDate as BeginContract_ToDateString, ec.EndDate as EndContract, ec.EndDate as EndContract_ToDateString, eg.BeginDate as BeginDate, eg.BeginDate as BeginDate_ToDateString, eg.EndDate as EndDate, eg.EndDate as EndDate_ToDateString,
     			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,1,''\'') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,2,''\'') As Nivel2,      
     			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,3,''\'') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,4,''\'') As Nivel4,      
     			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,5,''\'') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,6,''\'') As Nivel6,      
     			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,7,''\'') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,8,''\'') As Nivel8,      
     			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,9,''\'') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,10,''\'') As Nivel10,      
     			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',1,'',''),GetDate()) As UserField1,      
     			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',2,'',''),GetDate()) As UserField2,      
     			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',3,'',''),GetDate()) As UserField3,      
     			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',4,'',''),GetDate()) As UserField4,      
     			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',5,'',''),GetDate()) As UserField5,      
     			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',6,'',''),GetDate()) As UserField6,      
     			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',7,'',''),GetDate()) As UserField7,      
     			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',8,'',''),GetDate()) As UserField8,      
     			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',9,'',''),GetDate()) As UserField9,      
     			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',10,'',''),GetDate()) As UserField10,
    			(select top 1 datetime from punches where IdEmployee = emp.Id order by datetime desc) AS [LastPunch_ToDateString],
    			dbo.GetEmployeeAge(emp.Id) As Age' + @featureFields + ',    
    			dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,
  			CASE WHEN sp.LoginWithoutContract = 1 THEN ''X'' ELSE NULL END AS LoginWithoutContract,
 			l.Name AS LabAgree
             FROM employees emp    
     			 inner join sysroPassports sp on sp.IDEmployee = emp.id    
     			 inner join sysroPassports_AuthenticationMethods spam on spam.IDPassport = sp.ID    
     			 inner join EmployeeContracts ec on emp.id = ec.IDEmployee    
     			 inner join sysroEmployeeGroups eg on eg.IDEmployee = emp.id    
     			 inner join Groups g on g.id = eg.IDGroup    
     			 left join Terminals t on t.id = spam.BiometricTerminalId    
 				 left join LabAgree l on l.id = ec.idlabagree
    			 LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID
     		WHERE 
			(select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and cast(getdate() as date) between BeginDate and EndDate and idpassport = ' + CAST(@pidpassport as nvarchar(100)) + ' and idemployee = emp.Id and FeaturePermission > 0) > 0
     			AND emp.Id in(' + @employeeIDFilter + ') and convert(smalldatetime,''' + convert(nvarchar(max),@pinitialDate,112) + ''',120) >= ec.BeginDate and convert(smalldatetime,''' + convert(nvarchar(max),@pendDate,112) + ''',120) <= ec.EndDate  
 				AND (((VERSION <> ''RXA200''  AND VERSION <> '''')
 			 OR ((Version = ''RXA200'' OR VERSION = '''') AND ENABLED = 1 AND (SELECT COUNT(*) FROM sysroPassports_AuthenticationMethods WHERE IDPassport = spam.IDPassport AND BiometricData IS NOT NULL) = 0) )			 
 			 OR Method <> 4)) as query';
    	EXEC(@SqlStatement)
GO

 ALTER PROCEDURE [dbo].[Genius_SalaryGap]    
           @initialDate smalldatetime,@endDate smalldatetime,@idpassport int,@employeeFilter nvarchar(max),@userFieldsFilter nvarchar(max) AS    
       DECLARE @employeeIDs Table(idEmployee int)    
       DECLARE @pinitialDate smalldatetime = @initialDate  
       DECLARE @pendDate smalldatetime = @endDate  
          DECLARE @pidpassport int = @idpassport  
          DECLARE @pemployeeFilter nvarchar(max) = @employeeFilter  
          DECLARE @puserFieldsFilter nvarchar(max) = @userFieldsFilter  
          insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate    
   	 select salaryGap.*,
   		CASE WHEN salaryGap.sysroGender = 'Hombre' THEN salaryGap.sysroBaseSalary ELSE NULL END as sysroBaseSalary_man,
   		CASE WHEN salaryGap.sysroGender = 'Mujer' THEN salaryGap.sysroBaseSalary ELSE NULL END as sysroBaseSalary_woman,
  		CASE WHEN salaryGap.sysroGender = 'Hombre' THEN salaryGap.sysroEarningsOverTime ELSE NULL END as sysroEarningsOverTime_man,
   		CASE WHEN salaryGap.sysroGender = 'Mujer' THEN salaryGap.sysroEarningsOverTime ELSE NULL END as sysroEarningsOverTime_woman,
  		CASE WHEN salaryGap.sysroGender = 'Hombre' THEN salaryGap.sysroExtraSalary ELSE NULL END as sysroExtraSalary_man,
   		CASE WHEN salaryGap.sysroGender = 'Mujer' THEN salaryGap.sysroExtraSalary ELSE NULL END as sysroExtraSalary_woman,
  		CASE WHEN salaryGap.sysroGender = 'Hombre' THEN salaryGap.sysroTotalSalary ELSE NULL END as sysroTotalSalary_man,
   		CASE WHEN salaryGap.sysroGender = 'Mujer' THEN salaryGap.sysroTotalSalary ELSE NULL END as sysroTotalSalary_woman,
  		CASE WHEN salaryGap.sysroGender = 'Hombre' THEN salaryGap.sysroSalarySupp ELSE NULL END as sysroSalarySupp_man,
   		CASE WHEN salaryGap.sysroGender = 'Mujer' THEN salaryGap.sysroSalarySupp ELSE NULL END as sysroSalarySupp_woman
   	 from(
          SELECT emp.ID As IDEmployee, sreg.IDGroup, ec.IDContract,  
            sreg.GroupName, sreg.FullGroupName, emp.Name AS EmployeeName,     
            sreg.Path, sreg.CurrentEmployee, sreg.BeginDate, sreg.BeginDate AS BeginDate_ToDateString, sreg.EndDate, sreg.EndDate AS EndDate_ToDateString,
            ec.BeginDate AS BeginContract, ec.BeginDate AS BeginContract_ToDateString, ec.EndDate AS EndContract, ec.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,    
            convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroBaseSalary'),@pendDate)) As sysroBaseSalary,
  		  convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroEarningsOverTime'),@pendDate)) As sysroEarningsOverTime,
  		  convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroExtraSalary'),@pendDate)) As sysroExtraSalary,
  		  convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroTotalSalary'),@pendDate)) As sysroTotalSalary,
  		  convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroSalarySupp'),@pendDate)) As sysroSalarySupp,
            convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroProfessionalCategory'),@pendDate)) As sysroProfessionalCategory,    
            convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPosition'),@pendDate)) As sysroPosition,    
            convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroGender'),@pendDate)) As sysroGender,
  		  dbo.GetEmployeeAge(sreg.idEmployee) As Age,  
            dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,2,'\') As Nivel2,    
            dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,4,'\') As Nivel4,    
            dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,6,'\') As Nivel6,    
            dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,8,'\') As Nivel8,    
            dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,10,'\') As Nivel10,    
            dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),@pendDate) As UserField1,    
            dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),@pendDate) As UserField2,    
            dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),@pendDate) As UserField3,    
            dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),@pendDate) As UserField4,    
            dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),@pendDate) As UserField5,    
            dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),@pendDate) As UserField6,    
            dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),@pendDate) As UserField7,    
            dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),@pendDate) As UserField8,    
            dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),@pendDate) As UserField9,    
            dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@pendDate) As UserField10    ,
 		   dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
          FROM   
     dbo.sysroEmployeeGroups as sreg with (nolock)  
      INNER JOIN dbo.Employees emp  with (nolock) on sreg.IDEmployee = emp.ID  
	  INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature as pef ON pef.IdFeature = 1 and @pendDate between PEF.BeginDate and PEF.EndDate and PEF.idpassport = @pidpassport and PEF.idemployee = emp.ID and PEF.IsRoboticsUser = 0 and PEF.FeaturePermission > 0
            INNER JOIN dbo.EmployeeContracts ec with (nolock) ON ec.IDEmployee = emp.ID AND @pendDate between ec.BeginDate and ec.EndDate    
 		   LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID
          WHERE  		  
		  sreg.IDEmployee in( select idEmployee from @employeeIDs) and CurrentEmployee = 1 and @pendDate between sreg.BeginDate and sreg.EndDate) salaryGap
GO

ALTER   PROCEDURE [dbo].[Genius_Tasks]
@initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @userFieldsFilter nvarchar(max), @employeeFilter nvarchar(max) AS

DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pemployeeFilter nvarchar(max) = @employeeFilter
DECLARE @employeeIDs Table(idEmployee int)


insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter, @pinitialDate, @pendDate;


SELECT  dbo.sysroEmployeeGroups.IDGroup, dbo.sysroEmployeeGroups.FullGroupName AS GroupName, dbo.Employees.ID AS IDEmployee,
dbo.Employees.Name AS EmployeeName, DATEPART(year, dbo.DailyTaskAccruals.Date) AS Date_Year, DATEPART(month, dbo.DailyTaskAccruals.Date) AS Date_Month,
DATEPART(day, dbo.DailyTaskAccruals.Date) AS Date_Day, dbo.DailyTaskAccruals.Date, dbo.DailyTaskAccruals.Date AS Date_ToDateString, dbo.BusinessCenters.ID AS IDCenter, dbo.BusinessCenters.Name AS CenterName,
dbo.Tasks.ID AS IDTask, dbo.Tasks.Name AS TaskName, dbo.Tasks.InitialTime, ISNULL(dbo.Tasks.Field1, '') AS Field1_Task, ISNULL(dbo.Tasks.Field2, '') AS Field2_Task, ISNULL(dbo.Tasks.Field3, '') AS Field3_Task,
ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field4), 0) AS Field4_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field5), 0) AS Field5_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field6), 0) AS Field6_Task,
ISNULL(dbo.DailyTaskAccruals.Field1, '') AS Field1_Total, ISNULL(dbo.DailyTaskAccruals.Field2, '') AS Field2_Total, ISNULL(dbo.DailyTaskAccruals.Field3, '') AS Field3_Total,
ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field4), 0) AS Field4_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field5), 0) AS Field5_Total,
ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field6), 0) AS Field6_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Value), 0) AS Value,
ISNULL(dbo.Tasks.Project, '') AS Project, ISNULL(dbo.Tasks.Tag, '') AS Tag, dbo.Tasks.IDPassport, ISNULL(dbo.Tasks.TimeChangedRequirements, 0) AS TimeChangedRequirements,
ISNULL(dbo.Tasks.ForecastErrorTime, 0) AS ForecastErrorTime, ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) AS NonProductiveTimeIncidence, ISNULL(dbo.Tasks.EmployeeTime, 0) AS EmployeeTime,
ISNULL(dbo.Tasks.TeamTime, 0) AS TeamTime, ISNULL(dbo.Tasks.MaterialTime, 0) AS MaterialTime, ISNULL(dbo.Tasks.OtherTime, 0) AS OtherTime,
ISNULL(dbo.Tasks.InitialTime, 0) + ISNULL(dbo.Tasks.TimeChangedRequirements, 0) + ISNULL(dbo.Tasks.ForecastErrorTime, 0) + ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) + ISNULL(dbo.Tasks.EmployeeTime, 0) + ISNULL(dbo.Tasks.TeamTime, 0) + ISNULL(dbo.Tasks.MaterialTime, 0) + ISNULL(dbo.Tasks.OtherTime, 0) AS Duration,
CASE WHEN Tasks.Status = 0 THEN 'Activa' ELSE CASE WHEN Tasks.Status = 1 THEN 'Finalizada' ELSE CASE WHEN Tasks.Status = 2 THEN 'Cancelada' ELSE 'Pendiente' END END END AS Estado,
dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.DailyTaskAccruals.Date) As UserField1,
dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.DailyTaskAccruals.Date) As UserField2,
dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.DailyTaskAccruals.Date) As UserField3,
dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.DailyTaskAccruals.Date) As UserField4,
dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.DailyTaskAccruals.Date) As UserField5,
dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.DailyTaskAccruals.Date) As UserField6,
dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.DailyTaskAccruals.Date) As UserField7,
dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.DailyTaskAccruals.Date) As UserField8,
dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.DailyTaskAccruals.Date) As UserField9,
dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.DailyTaskAccruals.Date) As UserField10,
dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age  ,
dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
FROM    dbo.sysroEmployeeGroups with (nolock)
INNER JOIN dbo.DailyTaskAccruals with (nolock) ON dbo.DailyTaskAccruals.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.DailyTaskAccruals.Date BETWEEN dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate
INNER JOIN dbo.Tasks with (nolock) ON dbo.Tasks.ID = dbo.DailyTaskAccruals.IDTask
INNER JOIN dbo.BusinessCenters with (nolock) ON dbo.Tasks.IDCenter = dbo.BusinessCenters.ID
INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyTaskAccruals.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyTaskAccruals.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyTaskAccruals.Date <= dbo.EmployeeContracts.EndDate
INNER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
WHERE    (dbo.businesscenters.id is null OR dbo.BusinessCenters.ID IN (SELECT DISTINCT c.IDCostCenter FROM sysrovwSecurity_PermissionOverCostCenters c with (nolock) WHERE IDPassport=@pidpassport) )
AND dbo.DailyTaskAccruals.Date between @pinitialDate and @pendDate
AND (select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and dbo.DailyTaskAccruals.Date between pef.BeginDate and pef.EndDate and pef.idpassport = @pidpassport and pef.idemployee = dbo.DailyTaskAccruals.IDEmployee and pef.FeaturePermission > 0) > 0  
AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs)

GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='792' WHERE ID='DBVersion'
GO
