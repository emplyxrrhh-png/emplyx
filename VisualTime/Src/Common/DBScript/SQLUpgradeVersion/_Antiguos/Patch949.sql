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
 			l.Name AS LabAgree,
			CA.REALVALUE AS RealValue,
			CASE WHEN (SELECT VALUE FROM sysroLiveAdvancedParameters WHERE ParameterName = ''Timegate.Identification.CustomUserFieldId'') IS NULL OR (SELECT VALUE FROM sysroLiveAdvancedParameters WHERE ParameterName = ''Timegate.Identification.CustomUserFieldId'') = ''''
OR (SELECT JSON_VALUE(VALUE, ''$.UserFieldId'') FROM sysroLiveAdvancedParameters WHERE ParameterName = ''Timegate.Identification.CustomUserFieldId'') = -1
THEN cast(emp.id as nvarchar(max))
ELSE 
(SELECT value
FROM  GetEmployeeUserFieldValue(emp.id,(SELECT FieldName FROM sysroUserFields AS UF WHERE UF.ID = (SELECT JSON_VALUE(VALUE, ''$.UserFieldId'') AS UserFieldId
FROM sysroLiveAdvancedParameters
WHERE ParameterName = ''Timegate.Identification.CustomUserFieldId''
)),GETDATE())
)
END AS PINId
             FROM employees emp    
     			 inner join sysroPassports sp on sp.IDEmployee = emp.id    
     			 inner join sysroPassports_AuthenticationMethods spam on spam.IDPassport = sp.ID    
     			 inner join EmployeeContracts ec on emp.id = ec.IDEmployee    
     			 inner join sysroEmployeeGroups eg on eg.IDEmployee = emp.id    
     			 inner join Groups g on g.id = eg.IDGroup    
     			 left join Terminals t on t.id = spam.BiometricTerminalId    
 				 left join LabAgree l on l.id = ec.idlabagree
    			 LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID
				 LEFT JOIN CardAliases AS CA ON CAST(CA.IDCard AS NVARCHAR(100)) = spam.Credential				 
     		WHERE 
			(select count(*) from sysrovwSecurity_PermissionOverEmployeeAndFeature as pef where pef.IdFeature = 2 and cast(getdate() as date) between BeginDate and EndDate and idpassport = ' + CAST(@pidpassport as nvarchar(100)) + ' and idemployee = emp.Id and FeaturePermission > 0) > 0
     			AND emp.Id in(' + @employeeIDFilter + ') and convert(smalldatetime,''' + convert(nvarchar(max),@pinitialDate,112) + ''',120) >= ec.BeginDate and convert(smalldatetime,''' + convert(nvarchar(max),@pendDate,112) + ''',120) <= ec.EndDate  
 				AND (((VERSION <> ''RXA200''  AND VERSION <> '''')
 			 OR ((Version = ''RXA200'' OR VERSION = '''') AND ENABLED = 1 AND (SELECT COUNT(*) FROM sysroPassports_AuthenticationMethods WHERE IDPassport = spam.IDPassport AND BiometricData IS NOT NULL) = 0) )			 
 			 OR Method <> 4)) as query';
    	EXEC(@SqlStatement)


-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='949' WHERE ID='DBVersion'
GO
