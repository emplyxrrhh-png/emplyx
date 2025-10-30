ALTER PROCEDURE  [dbo].[Genius_Users]
            @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @requestTypesFilter nvarchar(max) AS      
    		DECLARE @employeeIDs Table(idEmployee int)      
    		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter       
   		DECLARE @employeeIDFilter NVARCHAR(MAX)
   		DECLARE @featureNames Table(feature nvarchar(MAX), featureid int, fieldName nvarchar(max), requestTypeId int)
   		DECLARE @featureFields NVARCHAR(max)
   		DECLARE @securityVersion int = 1
   		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate
   		select @employeeIDFilter = substring(tmp.empIds,2, len(tmp.empIds) -1) from
   			(select convert(nvarchar(max), (select ',' + convert(nvarchar(max),idemployee) as [data()] from @employeeIDs FOR XML PATH(''))) as empIds) tmp
   		IF @requestTypesFilter = ''
   			SET @requestTypesFilter = '-1'
   		IF EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'SecurityMode')
   			select @securityVersion = convert(int,value) from [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'SecurityMode'
   			
   		
   		
   		insert into @featureNames exec dbo.ObtainFeaturesFromFilter @requestTypesFilter
   		if @securityVersion < 3
   		begin
   			select @featureFields = isnull(tmp.empIds,'') from
   				(select convert(nvarchar(max), (select ',dbo.GetFeatureNextLevelPassportsByEmployee(emp.id,''' + feature  +''',' + convert(nvarchar(max),featureid) + ') AS [sup' + fieldName + ']' as [data()] from @featureNames FOR XML PATH(''))) as empIds) tmp
   		end
   		else
   		begin
   			select @featureFields = isnull(tmp.empIds,'') from
   			(select convert(nvarchar(max), (select ',dbo.GetDirectSupervisorNameByRequest(emp.id,''' + feature  +''',' + convert(nvarchar(max),featureid) + ',' + convert(nvarchar(max),requestTypeId) + ') AS [sup' + fieldName + ']' as [data()] from @featureNames FOR XML PATH(''))) as empIds) tmp
   		end
   		
   		
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
    		WHERE dbo.WebLogin_GetPermissionOverEmployee(' + CAST(@pidpassport as nvarchar(100)) + ',emp.Id,2,0,0,GetDate()) > 1      
    			AND emp.Id in(' + @employeeIDFilter + ') and convert(smalldatetime,''' + convert(nvarchar(max),@pinitialDate,112) + ''',120) >= ec.BeginDate and convert(smalldatetime,''' + convert(nvarchar(max),@pendDate,112) + ''',120) <= ec.EndDate  
				AND (((VERSION <> ''RXA200''  AND VERSION <> '''')
			 OR ((Version = ''RXA200'' OR VERSION = '''') AND ENABLED = 1 AND (SELECT COUNT(*) FROM sysroPassports_AuthenticationMethods WHERE IDPassport = spam.IDPassport AND BiometricData IS NOT NULL) = 0) )			 
			 OR Method <> 4)) as query';
   	EXEC(@SqlStatement)

GO

UPDATE [dbo].[GeniusViews]
   SET CubeLayout = '{"slice":{"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"UserField7"},{"uniqueName":"UserField4"},{"uniqueName":"UserField8"},{"uniqueName":"UserField1"},{"uniqueName":"UserField5"},{"uniqueName":"LabAgree"},{"uniqueName":"Age"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Enabled","aggregation":"count"}],"expands":{"expandAll":true},"flatOrder":["GroupName","EmployeeName","UserField7","UserField6","UserField4","UserField8","UserField1","UserField5","Age"]},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2022-01-20T11:40:24.414Z"}'					    
 WHERE Name = 'usersWorkCenter' and IdPassport = 0
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO


UPDATE sysroParameters SET Data='552' WHERE ID='DBVersion'
GO