ALTER DATABASE CURRENT SET COMPATIBILITY_LEVEL = 120
GO

insert into sysroRequestRuleTypes values (14, 1)
GO

UPDATE [dbo].[sysroGui] SET Parameters='SaaSEnabled' WHERE URL LIKE '%Surveys%'
GO

update [dbo].[sysroGui] set Parameters = 'SaaSEnabled' where  IDPath= 'Portal\Reports\AdvReport'
GO
update sysrogui set RequiredFunctionalities='U:Access.Zones=Read' where idpath='Portal\Configuration\Zones'
GO

IF EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'RoboticsSupportUrl' and Value= 'http://www.robotics.es/RoboticsQS.exe')
	update sysroLiveAdvancedParameters set Value='http://www.visualtime.net/RoboticsQS.exe' where ParameterName = 'RoboticsSupportUrl'
GO

--Importar Tareas / Centros de Coste + templates

IF NOT EXISTS (SELECT * FROM [dbo].[ImportGuides] WHERE ID = 25)
	INSERT [dbo].[ImportGuides] ([ID], [Name], [Template], [Mode], [Type], [FormatFilePath], [SourceFilePath], [Separator], [CopySource], [LastLog], [RequieredFunctionalities], [FeatureAliasID], [Destination], [Parameters], [Enabled], [Version], [Concept], [Active], [IDDefaultTemplate], [PreProcessScript], [Edition]) 
	VALUES (25, N'Tareas', 0, 1, 1, N'', N'', N'', 1, N'', N'Tasks.DataLink.Imports.Tasks', N'Tasks', 0, NULL, 1, 2, N'Tasks', NULL, 5, NULL, NULL)
GO

IF NOT EXISTS (SELECT * FROM [dbo].[ImportGuides] WHERE ID = 26)
	INSERT [dbo].[ImportGuides] ([ID], [Name], [Template], [Mode], [Type], [FormatFilePath], [SourceFilePath], [Separator], [CopySource], [LastLog], [RequieredFunctionalities], [FeatureAliasID], [Destination], [Parameters], [Enabled], [Version], [Concept], [Active], [IDDefaultTemplate], [PreProcessScript], [Edition]) 
	VALUES (26, N'Centros de coste', 0, 1, 1, N'', N'', N'', 1, N'', N'BusinessCenters.DataLink.Imports.BusinessCenter', N'Employees', 0, NULL, 1, 2, N'Costs', NULL, 6, NULL, NULL)
GO

IF NOT EXISTS (SELECT * FROM [dbo].[ImportGuidesTemplates] WHERE ID = 5)
	INSERT [dbo].[ImportGuidesTemplates] ([ID], [IDParentGuide], [Name], [Profile], [Parameters], [PostProcessScript], [PreProcessScript])
	VALUES (5, 25,  N'Básica', N'tmplImportTasks.xlsx',  NULL, NULL, NULL)
GO

IF NOT EXISTS (SELECT * FROM [dbo].[ImportGuidesTemplates] WHERE ID = 6)
	INSERT [dbo].[ImportGuidesTemplates] ([ID], [IDParentGuide], [Name], [Profile], [Parameters], [PostProcessScript], [PreProcessScript])
	VALUES (6, 26,  N'Básica', N'tmplImportBusinessCenters.xlsx',  NULL, NULL, NULL)
GO

--Exportar Tareas / Centros de Coste + templates

IF NOT EXISTS (SELECT * FROM [dbo].[ExportGuides] WHERE ID = 20008)
	INSERT [dbo].[ExportGuides] ([ID], [Name], [ProfileMask], [ProfileType], [Mode], [ProfileName], [Destination], [ExportFileName], [ExportFileType], [Separator], [StartCalculDay], [StartExecutionHour], [LastLog], [NextExecution], [Field_1], [Field_2], [Field_3], [Field_4], [Field_5], [Field_6], [IntervalMinutes], [DisplayParameters], [Scheduler], [EmployeeFilter], [RequieredFunctionalities], [FeatureAliasID], [ExportFileNameTimeStampFormat], [WSParameters], [AutomaticDatePeriod], [Version], [Concept], [Active], [IDDefaultTemplate], [PostProcessScript], [Enabled], [ApplyLockDate], [DatasourceFile], [Edition])
	VALUES (20008, N'Tareas', N'', 15, 1, N'', N'', N'', 1, N'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, N'1,1,0@1@1', N'', NULL, N'Tasks.DataLink.Exports.AdvTasks', N'Tasks', NULL, NULL, NULL, 2, N'Tasks', 0, 11, NULL, NULL, 0, N'', NULL)
GO

IF NOT EXISTS (SELECT * FROM [dbo].[ExportGuides] WHERE ID = 20009)
	INSERT [dbo].[ExportGuides] ([ID], [Name], [ProfileMask], [ProfileType], [Mode], [ProfileName], [Destination], [ExportFileName], [ExportFileType], [Separator], [StartCalculDay], [StartExecutionHour], [LastLog], [NextExecution], [Field_1], [Field_2], [Field_3], [Field_4], [Field_5], [Field_6], [IntervalMinutes], [DisplayParameters], [Scheduler], [EmployeeFilter], [RequieredFunctionalities], [FeatureAliasID], [ExportFileNameTimeStampFormat], [WSParameters], [AutomaticDatePeriod], [Version], [Concept], [Active], [IDDefaultTemplate], [PostProcessScript], [Enabled], [ApplyLockDate], [DatasourceFile], [Edition]) 
	VALUES (20009, N'Centros de coste', N'', 16, 1, N'', N'', N'', 1, N'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, N'1,1,0@1@1', N'', NULL, N'BusinessCenters.DataLink.Exports.AdvBusinessCenter', N'Employees', NULL, NULL, NULL, 2, N'Costs', 0, 12, NULL, NULL, 0, N'', NULL)
GO

IF NOT EXISTS (SELECT * FROM [dbo].[ExportGuidesTemplates] WHERE ID = 11)
	INSERT [dbo].[ExportGuidesTemplates] ([ID], [IDParentGuide], [Name], [Profile], [Parameters], [PostProcessScript], [PreProcessScript])
	VALUES (11, 20008, 'tasks', 'tmplExportTasks.xlsx' , NULL, NULL, NULL)
GO

IF NOT EXISTS (SELECT * FROM [dbo].[ExportGuidesTemplates] WHERE ID = 12)
	INSERT [dbo].[ExportGuidesTemplates] ([ID], [IDParentGuide], [Name], [Profile], [Parameters], [PostProcessScript], [PreProcessScript])
	VALUES (12, 20009, 'costs', 'tmplExportBusinessCenters.xlsx' , NULL, NULL, NULL)
GO

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
  			emp.Id as IDEmployee, emp.Name as EmployeeName, spam.method as Method, spam.credential as Credential, spam.version as Version,     
  			CASE WHEN len(CONVERT(VARCHAR(MAX), CONVERT(VARBINARY(MAX),spam.BiometricData))) > 0 THEN ''X'' ELSE '''' END AS BiometricData,    
  			spam.Enabled as Enabled, spam.TimeStamp as TimeStamp, spam.TimeStamp AS TimeStamp_ToDateString, t.Description as Terminal, spam.BiometricAlgorithm as BiometricAlgorithm,     
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
 			dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
          FROM employees emp    
  			 inner join sysroPassports sp on sp.IDEmployee = emp.id    
  			 inner join sysroPassports_AuthenticationMethods spam on spam.IDPassport = sp.ID    
  			 inner join EmployeeContracts ec on emp.id = ec.IDEmployee    
  			 inner join sysroEmployeeGroups eg on eg.IDEmployee = emp.id    
  			 inner join Groups g on g.id = eg.IDGroup    
  			 left join Terminals t on t.id = spam.BiometricTerminalId    
 			 LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID
  		WHERE dbo.WebLogin_GetPermissionOverEmployee(' + CAST(@pidpassport as nvarchar(100)) + ',emp.Id,2,0,0,GetDate()) > 1      
  			AND emp.Id in(' + @employeeIDFilter + ') and convert(smalldatetime,''' + convert(nvarchar(max),@pinitialDate,112) + ''',120) >= ec.BeginDate and convert(smalldatetime,''' + convert(nvarchar(max),@pendDate,112) + ''',120) <= ec.EndDate  ) as query';
 	EXEC(@SqlStatement)

GO

ALTER VIEW [dbo].[sysrovwEmployeeStatus] AS 
	SELECT svEmployees.IdEmployee, Employees.Image as EmployeeImage, Employees.Name as EmployeeName, 
		tmpAttendance.ShiftDate AttShiftDate, tmpAttendance.DateTime as AttDatetime, tmpAttendance.InTelecommute, 
		CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN 'Out-Calculated' 
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
	FROM sysrovwcurrentemployeegroups svEmployees 
	inner join Employees ON Employees.ID = svEmployees.IDEmployee 
	LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY Id ORDER BY LEN(Path) DESC, LEN(PathGroup) DESC) AS 'RowNumberG', * FROM (SELECT Groups.Id IdGroup, Groups.Path PathGroup, Groups.IDCenter IdCenterGroup, GAux.Id, GAux.Path FROM Groups 
				INNER JOIN Groups as GAux ON GAux.Path = Groups.Path OR (GAux.Path LIKE CONCAT(Groups.Path ,'\%')) 
				WHERE Groups.IDCenter IS NOT NULL) GrouCentersAux) GrouCenters ON GrouCenters.ID = svEmployees.IDGroup 
	LEFT JOIN DailySchedule WITH (NOLOCK) ON DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) = DailySchedule.Date AND svEmployees.IdEmployee = DailySchedule.IDEmployee 	
	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberTsk', * FROM Punches WHERE ActualType = 4 AND DATEDIFF(day,ShiftDate, GETDATE()) < 120) tmpTasks ON svEmployees.IDEmployee = tmpTasks.IdEmployee 
	left join Tasks ON Tasks.Id = tmpTasks.TypeData LEFT JOIN sysroLiveAdvancedParameters ON (ParameterName = 'LastPunchCenterEnabled' and convert(nvarchar,Value) = 1) 
	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberCost', * FROM Punches WHERE ActualType = 13) tmpCosts ON svEmployees.IDEmployee = tmpCosts.IdEmployee AND (sysroLiveAdvancedParameters.Value = 1 OR tmpCosts.ShiftDate = DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)) 
	left join BusinessCenters ON tmpCosts.TypeData = BusinessCenters.ID 
	left join BusinessCenters BusinessCentersGroups WITH (NOLOCK) ON GrouCenters.IdCenterGroup = BusinessCentersGroups.ID 
	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberAtt', * FROM Punches WHERE (ActualType = 1 OR ActualType = 2) AND DATEDIFF(day,ShiftDate, GETDATE()) < 120) tmpAttendance ON svEmployees.IDEmployee = tmpAttendance.IdEmployee 
	LEFT JOIN Shifts WITH (NOLOCK) ON Shifts.Id = DailySchedule.IDShift1 
	LEFT JOIN BusinessCenters BusinessCentersShifts WITH (NOLOCK) ON Shifts.IDCenter = BusinessCentersShifts.Id 
	WHERE CurrentEmployee = 1 and (RowNumberAtt = 1 or RowNumberAtt is null) and (RowNumberTsk = 1 or RowNumberTsk is null) and (RowNumberCost = 1 or RowNumberCost is null) AND (RowNumberG = 1 OR RowNumberG IS NULL) 
GO

UPDATE sysroParameters SET Data='529' WHERE ID='DBVersion'
GO



-- Informes nueva era. Calendario mensual grupos + Calendario mensual usuarios + Calendario anual usuarios
DROP PROCEDURE [dbo].[Report_calendario_mensual_grupos]
GO
CREATE PROCEDURE [dbo].[Report_calendario_mensual_grupos]
	@idPassport nvarchar(100) = '1',
	@month nvarchar(2) = '01',
	@year nvarchar(4) = '2021',
	@IDEmployee nvarchar(max) = '114,242'
AS
  select * from(
  select IDEmployee, datepart(dd, Date) as Date, shifts.Name as NameShift from DailySchedule
  inner join Shifts on (DailySchedule.IDShiftUsed is not null and DailySchedule.IDShiftUsed = Shifts.ID) or (DailySchedule.IDShift1 = Shifts.ID)
  where IDEmployee IN (select * from split(@IDEmployee,','))
  and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,IDEmployee,2,0,0,GetDate()) > 0 
  and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year
  ) t PIVOT(Max(NameShift) FOR Date IN("1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20","21","22","23","24","25","26","27","28","29","30","31")) AS pivot_table
  join (
	  select emp.ID AS EmployeeID, emp.Name As EmployeeName, ec.IDContract as Contract, 
	  ec.BeginDate as BeginContract, ec.EndDate as EndContract,
	  g.Id as IDGroup, g.Name as GroupName, g.FullGroupName as Ruta
	  from Employees emp
	  join EmployeeContracts ec on ec.IDEmployee = emp.ID
	  join EmployeeGroups eg on eg.IDEmployee = emp.ID
	  join Groups g on eg.IDGroup = g.ID
	    where emp.ID IN (select * from split(@IDEmployee,',')) 
		and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,emp.ID,2,0,0,GetDate()) > 0 
		AND (datepart(YYYY, ec.BeginDate) < @year or (datepart(MM, ec.BeginDate) <= @month and datepart(YYYY, ec.BeginDate) <= @year)) and (datepart(YYYY, ec.EndDate) > @year or (datepart(MM, ec.EndDate) >= @month and datepart(YYYY, ec.EndDate) = @year))
		AND (datepart(YYYY, eg.BeginDate) < @year or (datepart(MM, eg.BeginDate) <= @month and datepart(YYYY, eg.BeginDate) <= @year)) and (datepart(YYYY, eg.EndDate) > @year or (datepart(MM, eg.EndDate) >= @month and datepart(YYYY, eg.EndDate) = @year))
  ) as emps on pivot_table.IDEmployee = emps.EmployeeID
  inner join (
    select * from(
  select IDEmployee, CONCAT('c',datepart(dd, Date)) as Date, s.color as Color from DailySchedule 
  join shifts s on (IDShiftUsed is not null and IDShiftUsed = s.ID) or (IDShift1 = s.ID)
  where IDEmployee IN (select * from split(@IDEmployee,','))
  and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,IDEmployee,2,0,0,GetDate()) > 0 
  and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year
  ) t PIVOT(Max(Color) FOR Date IN("c1","c2","c3","c4","c5","c6","c7","c8","c9","c10","c11","c12","c13","c14","c15","c16","c17","c18","c19","c20","c21","c22","c23","c24","c25","c26","c27","c28","c29","c30","c31")) AS pivot_table
  ) as p2 on pivot_table.IDEmployee = p2.IDEmployee
  inner join (
  select * from(
  select IDEmployee, CONCAT('h',datepart(dd, Date)) as Date, s.ExpectedWorkingHours as Horas from DailySchedule 
  join shifts s on (IDShiftUsed is not null and IDShiftUsed = s.ID) or (IDShift1 = s.ID)
  where IDEmployee IN (select * from split(@IDEmployee,','))
  and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,IDEmployee,2,0,0,GetDate()) > 0 
  and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year
  ) t PIVOT(Max(Horas) FOR Date IN("h1","h2","h3","h4","h5","h6","h7","h8","h9","h10","h11","h12","h13","h14","h15","h16","h17","h18","h19","h20","h21","h22","h23","h24","h25","h26","h27","h28","h29","h30","h31")) AS pivot_table
  ) as p3 on pivot_table.IDEmployee = p3.IDEmployee
  inner join (
  select * from(
  select d.IDEmployee, CONCAT('a',datepart(dd, Date)) as Date, pa.AbsenceID as Absence  from DailySchedule as d
  left join ProgrammedAbsences as pa 
  on d.IDEmployee = pa.IDEmployee
  and d.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
  where d.IDEmployee IN (select * from split(@IDEmployee,','))
  and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,d.IDEmployee,2,0,0,GetDate()) > 0 
  and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year
  ) t PIVOT(Max(Absence) FOR Date IN("a1","a2","a3","a4","a5","a6","a7","a8","a9","a10","a11","a12","a13","a14","a15","a16","a17","a18","a19","a20","a21","a22","a23","a24","a25","a26","a27","a28","a29","a30","a31")) AS pivot_table
) as p4 on pivot_table.IDEmployee = p4.IDEmployee
  inner join (
    select * from(
  select d.IDEmployee, CONCAT('v',datepart(dd, Date)) as Date, r.Status as Status from DailySchedule as d
  left join Requests as r on d.IDEmployee = r.IDEmployee
  and d.Date between r.Date1 and r.Date2
  and r.Status = 0 and r.RequestType = 6
  where d.IDEmployee IN (select * from split(@IDEmployee,','))
  and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,d.IDEmployee,2,0,0,GetDate()) > 0 
  and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year
  ) t PIVOT(Max(Status) FOR Date IN("v1","v2","v3","v4","v5","v6","v7","v8","v9","v10","v11","v12","v13","v14","v15","v16","v17","v18","v19","v20","v21","v22","v23","v24","v25","v26","v27","v28","v29","v30","v31")) AS pivot_table
  ) as p5 on pivot_table.IDEmployee = p5.IDEmployee
GO

DROP PROCEDURE [dbo].[Report_calendario_mensual_grupos_resumen]
GO
CREATE PROCEDURE [dbo].[Report_calendario_mensual_grupos_resumen]
	@month nvarchar(2) = '01',
	@year nvarchar(4) = '2021',
	@IDEmployee nvarchar(max) = '114,242',
	@IDGroup nvarchar(9) = '123'

AS
	select s.Name, s.ShortName, s.Color, COUNT(ISNULL(ds.IDShiftUsed,ds.IDShift1)) as Days, SUM(s.ExpectedWorkingHours) as ExpectedWorkingHours from DailySchedule ds
  inner join shifts s on (IDShiftUsed is not null and IDShiftUsed = s.ID) or (IDShift1 = s.ID)
  inner join EmployeeGroups as eg on ds.IDEmployee = eg.IDEmployee
  and ds.Date between eg.BeginDate and eg.EndDate
  where ds.IDEmployee IN (select * from split(@IDEmployee,','))
  and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year
  and eg.IDGroup = @IDGroup
  GROUP BY s.Name, s.ShortName, s.Color;
  RETURN
EXEC Report_calendario_mensual_grupos_resumen;
GO

DROP PROCEDURE [dbo].[Report_calendario_mensual_usuarios]
GO
CREATE PROCEDURE [dbo].[Report_calendario_mensual_usuarios]
	@month nvarchar(2) = '04',
 	@year nvarchar(4) = '2021',
 	@employee nvarchar(max) = '2'
 AS
	SET @month = FORMAT(CAST(@month as int), '00');
    SET @year = FORMAT(CAST(@year as int), '0000');
	IF @year < 1990
	BEGIN
		SET @year = 1990;
	END
	IF @month < 1
	BEGIN
		SET @month = 1;
	END
	DECLARE @StartDate date = DATEFROMPARTS(@year,@month,'01');
	DECLARE @EndDate date = EOMONTH(@StartDate);

 WITH Dates AS(
    SELECT @StartDate as Fecha, @employee as IDEmployee, datepart(ww, @StartDate) as Week, datepart(dw, @StartDate) as DayWeek, datepart(dd, @StartDate) as Day, NULL as NameShift, 16777215 as Color, NULL as Hours, NULL as Absence, NULL as Pending 
    UNION ALL
    SELECT DATEADD(DAY, 1,Fecha), @employee as IDEmployee, datepart(ww, DATEADD(DAY, 1,Fecha)) as Week, datepart(dw, DATEADD(DAY, 1,Fecha)) as DayWeek, datepart(dd, DATEADD(DAY, 1,Fecha)) as Day, NULL as NameShift, 16777215 as Color, NULL as Hours, NULL as Absence, NULL as Pending 
    FROM Dates
    WHERE DATEADD(DAY, 1,Fecha) <= @EndDate
	) 
 Select NULL as Fecha, @employee as IDEmployee, NULL as Week, DayWeek, NULL as Day, NULL as NameShift, NULL as Color, NULL as Hours, NULL as Absence, NULL as Pending from (
 SELECT top 7
 ROW_NUMBER() OVER(ORDER BY Date ASC) AS DayWeek,
 date, @employee as idemployee
 FROM DailySchedule ) x where DayWeek < (select TOP(1) datepart(dw, Date) as DayWeek from DailySchedule where DailySchedule.IDEmployee = @employee and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year order by Date)
 UNION
  select Date as Fecha, d.IDEmployee, datepart(ww, Date) as Week, datepart(dw, Date) as DayWeek, datepart(dd, Date) as Day, CAST(shifts.Name as nvarchar(max)) as NameShift, shifts.Color as Color, shifts.ExpectedWorkingHours as Hours, pa.AbsenceID as Absence, r.Status as Pending
   from DailySchedule as d
   inner join Shifts on (d.IDShift1 = Shifts.ID)
   left join ProgrammedAbsences as pa 
   on d.IDEmployee = pa.IDEmployee
   and d.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
   left join Requests as r on d.IDEmployee = r.IDEmployee
   and d.Date between r.Date1 and r.Date2
   and r.Status = 0 and r.RequestType = 6
   where d.IDEmployee = @employee and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year
	UNION
   select dat.Fecha, dat.IDEmployee, dat.Week, Dat.DayWeek, dat.Day, CAST(dat.NameShift as nvarchar(max)) as NameShift, dat.Color, dat.Hours, dat.Absence, dat.Pending 
   from Dates dat 
   join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
   where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and datepart(MM, dsc.Date) = @month and datepart(YYYY, dsc.Date) = @year)
   AND (ec.BeginDate <= dat.Fecha and ec.EndDate >= dat.Fecha)
   UNION
   select dat.Fecha, dat.IDEmployee, dat.Week, Dat.DayWeek, dat.Day, CAST(dat.NameShift as nvarchar(max)) as NameShift, NULL as Color, dat.Hours, dat.Absence, dat.Pending 
   from Dates dat 
   join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
   where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and datepart(MM, dsc.Date) = @month and datepart(YYYY, dsc.Date) = @year)
   AND ec.EndDate < dat.Fecha
order by IDEmployee, Week, DayWeek, Day
	return null
	EXEC Report_calendario_mensual_usuarios;
GO

DROP PROCEDURE [dbo].[Report_calendario_anual_usuarios_list]
GO
CREATE PROCEDURE [dbo].[Report_calendario_anual_usuarios_list]
	@idPassport nvarchar(max) = '1024',
	@monthStart nvarchar(2) = '01',
	@yearStart nvarchar(4) = '2021',
	@monthEnd nvarchar(2) = '04',
	@yearEnd nvarchar(4) = '2021',
	@employees nvarchar(max) = '1,2'
AS
	SET @monthStart = FORMAT(CAST(@monthStart as int), '00');
    SET @yearStart = FORMAT(CAST(@yearStart as int), '0000');
	SET @monthEnd = FORMAT(CAST(@monthEnd as int), '00');
	SET @yearEnd = FORMAT(CAST(@yearEnd as int), '0000');
	DECLARE @StartDate date = DATEFROMPARTS(@yearStart,@monthStart,'01');
	DECLARE @EndDate date = EOMONTH(DATEFROMPARTS(@yearEnd,@monthEnd,'01'));
 WITH Dates AS(
    SELECT @StartDate as Fecha, NULL as IDEmployee, 
 0 as CountDays, 
 datepart(mm, @StartDate) as Month, 
 datepart(YYYY, @StartDate) as Year,
 datepart(dd, EOMONTH(@StartDate)) as LastDayMonth,
 0 as Hours,
 NULL As EmployeeName, NULL as Contract, 
	  NULL as BeginContract, NULL as EndContract,
	  NULL as IDGroup, NULL as GroupName,NULL as Ruta 
    UNION ALL
    SELECT DATEADD(DAY, 1,Fecha), NULL as IDEmployee, 
 0 as CountDays, 
 datepart(mm, DATEADD(DAY, 1,Fecha)) as Month, 
 datepart(YYYY, DATEADD(DAY, 1,Fecha)) as Year,
 datepart(dd, EOMONTH(DATEADD(DAY, 1,Fecha))) as LastDayMonth,
 0 as Hours,
 NULL As EmployeeName, NULL as Contract, 
	  NULL as BeginContract, NULL as EndContract,
	  NULL as IDGroup, NULL as GroupName,NULL as Ruta 
	  FROM Dates
    WHERE DATEADD(DAY, 1,Fecha) <= @EndDate)
 select d.IDEmployee, 
 (select count(IDEmployee) from DailySchedule where IDEmployee = d.IDEmployee and (datepart(YYYY, d.Date) LIKE datepart(YYYY, Date) AND datepart(mm, d.Date) LIKE datepart(mm, Date)) AND ISNULL(IDShiftUsed,IDShift1) IS NOT NULL) as CountDays, 
 datepart(mm, Date) as Month, 
 datepart(YYYY, Date) as Year,
 datepart(dd, EOMONTH(Date)) as LastDayMonth,
 SUM(shifts.ExpectedWorkingHours) as Hours,
 emp.Name As EmployeeName, ec.IDContract as Contract, 
	  ec.BeginDate as BeginContract, ec.EndDate as EndContract,
	  g.Id as IDGroup, g.Name as GroupName, g.FullGroupName as Ruta
	  from Dates 
	  left join DailySchedule as d on Dates.Fecha = d.Date 
  left join Shifts on (d.IDShiftUsed is not null and d.IDShiftUsed = Shifts.ID) or (d.IDShift1 = Shifts.ID)
  left join Employees emp on emp.ID = d.IDEmployee
  left join EmployeeContracts ec on ec.IDEmployee = emp.ID
	 left join EmployeeGroups eg on eg.IDEmployee = emp.ID
	  left join Groups g on eg.IDGroup = g.ID
  left join ProgrammedAbsences as pa 
  on d.IDEmployee = pa.IDEmployee
  and d.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
  where d.IDEmployee IN (select * from split(@employees,',')) and Date between CONCAT(@yearStart,@monthStart,'01') and EOMONTH(convert(smalldatetime,CONCAT(@yearEnd,@monthEnd,'01'),120)) 
  AND (datepart(YYYY, ec.BeginDate) < @yearStart or (datepart(MM, ec.BeginDate) <= @monthStart and datepart(YYYY, ec.BeginDate) <= @yearStart)) and (datepart(YYYY, ec.EndDate) > @yearEnd or (datepart(MM, ec.EndDate) >= @monthEnd and datepart(YYYY, ec.EndDate) = @yearEnd))
		AND (datepart(YYYY, eg.BeginDate) < @yearStart or (datepart(MM, eg.BeginDate) <= @monthStart and datepart(YYYY, eg.BeginDate) <= @yearStart)) and (datepart(YYYY, eg.EndDate) > @yearEnd or (datepart(MM, eg.EndDate) >= @monthEnd and datepart(YYYY, eg.EndDate) = @yearEnd))
  AND pa.AbsenceID is null
  and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,d.IDEmployee,2,0,0,GetDate()) > 0
  group by d.IDEmployee, datepart(mm, Date), datepart(YYYY, Date), datepart(dd, EOMONTH(Date)), emp.Name, ec.IDContract, 
	  ec.BeginDate, ec.EndDate,
	  g.Id, g.Name, g.FullGroupName
  order by d.IDEmployee, Month, Year
  option (maxrecursion 0)
  RETURN NULL

  EXECUTE [dbo].[Report_calendario_anual_usuarios_list]
GO

DROP PROCEDURE [dbo].[Report_calendario_anual_usuarios]
GO
CREATE PROCEDURE [dbo].[Report_calendario_anual_usuarios]
	@month nvarchar(2) = '06',
 	@year nvarchar(4) = '2021',
 	@employee nvarchar(max) = '16'
 AS
	SET @month = FORMAT(CAST(@month as int), '00');
    SET @year = FORMAT(CAST(@year as int), '0000');
	IF @year < 1990
	BEGIN
		SET @year = 1990;
	END
	IF @month < 1
	BEGIN
		SET @month = 1;
	END
	DECLARE @StartDate date = DATEFROMPARTS(@year,@month,1);
	DECLARE @EndDate date = EOMONTH(@StartDate);
 WITH Dates AS(
    SELECT @StartDate as Fecha, @employee as IDEmployee, datepart(ww, @StartDate) as Week, datepart(dw, @StartDate) as DayWeek, datepart(dd, @StartDate) as Day, datepart(mm, @StartDate) as Month, NULL as NameShift, 16777215 as Color, NULL as Hours, NULL as Absence, NULL as Pending 
    UNION ALL
    SELECT DATEADD(DAY, 1,Fecha), @employee as IDEmployee, datepart(ww, DATEADD(DAY, 1,Fecha)) as Week, datepart(dw, DATEADD(DAY, 1,Fecha)) as DayWeek, datepart(dd, DATEADD(DAY, 1,Fecha)) as Day, datepart(mm, DATEADD(DAY, 1,Fecha)) as Month,NULL as NameShift, 16777215 as Color, NULL as Hours, NULL as Absence, NULL as Pending 
    FROM Dates
    WHERE DATEADD(DAY, 1,Fecha) <= @EndDate)
  select Date as Fecha, d.IDEmployee, datepart(ww, Date) as Week, datepart(dw, Date) as DayWeek, datepart(dd, Date) as Day, datepart(mm, Date) as Month, shifts.Name as NameShift, ISNULL(shifts.Color,16777215) as Color, shifts.ExpectedWorkingHours as Hours, pa.AbsenceID as Absence, r.Status as Pending 
   from DailySchedule as d
   left join Shifts on (d.IDShift1 = Shifts.ID)
   left join ProgrammedAbsences as pa 
   on d.IDEmployee = pa.IDEmployee
   and d.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
   left join Requests as r on d.IDEmployee = r.IDEmployee
   and d.Date between r.Date1 and r.Date2
   and r.Status = 0 and r.RequestType = 6
   where d.IDEmployee = @employee and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year
   UNION ALL
   select dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.Month, CAST(dat.NameShift as nvarchar(max)) as NameShift, dat.Color, dat.Hours, dat.Absence, dat.Pending 
   from Dates dat 
   join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
   where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and datepart(MM, dsc.Date) = @month and datepart(YYYY, dsc.Date) = @year)
   AND (ec.BeginDate <= dat.Fecha and ec.EndDate >= dat.Fecha)
   UNION ALL
   select dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.Month, CAST(dat.NameShift as nvarchar(max)) as NameShift, NULL as Color, dat.Hours, dat.Absence, dat.Pending 
   from Dates dat 
   join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
   where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and datepart(MM, dsc.Date) = @month and datepart(YYYY, dsc.Date) = @year)
   AND ec.EndDate < dat.Fecha
   order by IDEmployee, Fecha, Month, Day, DayWeek
   return null

  EXECUTE [dbo].[Report_calendario_anual_usuarios]
GO

DROP PROCEDURE [dbo].[Report_calendario_anual_usuarios_resumen]
GO
CREATE PROCEDURE [dbo].[Report_calendario_anual_usuarios_resumen]
	@monthStart nvarchar(2) = '1',
	@yearStart nvarchar(4) = '2021',
	@monthEnd nvarchar(2) = '04',
	@yearEnd nvarchar(4) = '2021',
	@IDEmployee nvarchar(max) = '777',
	@IDGroup nvarchar(9) = '364'

AS
	SET @monthStart = FORMAT(CAST(@monthStart as int), '00');
    SET @yearStart = FORMAT(CAST(@yearStart as int), '0000');
	SET @monthEnd = FORMAT(CAST(@monthEnd as int), '00');
	SET @yearEnd = FORMAT(CAST(@yearEnd as int), '0000');
	DECLARE @fechaStart date = DATEFROMPARTS(@yearStart,@monthStart,'01');
	declare @fechaEnd datetime = EOMONTH(DATEFROMPARTS(@yearEnd,@monthEnd,'01'));

	select s.Name, s.ShortName, s.Color, COUNT(ISNULL(ds.IDShiftUsed,ds.IDShift1)) as Days, SUM(s.ExpectedWorkingHours) as ExpectedWorkingHours from DailySchedule ds
  inner join shifts s on (IDShiftUsed is not null and IDShiftUsed = s.ID) or (IDShift1 = s.ID)
  inner join EmployeeGroups as eg on ds.IDEmployee = eg.IDEmployee
  and ds.Date between eg.BeginDate and eg.EndDate
  left join ProgrammedAbsences as pa 
  on ds.IDEmployee = pa.IDEmployee
  and ds.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
  where ds.IDEmployee IN (select * from split(@IDEmployee,','))
  and Date between @fechaStart and EOMONTH(@fechaEnd)
  and eg.IDGroup IN (select * from split(@IDGroup,','))
  and pa.AbsenceID is null
  GROUP BY s.Name, s.ShortName, s.Color;
  RETURN NULL
EXEC Report_calendario_anual_usuarios_resumen;

GO

DROP PROCEDURE [dbo].[Genius_IncidencesByTime]
GO

CREATE PROCEDURE [dbo].[Genius_IncidencesByTime]
           @initialDate smalldatetime,    
           @endDate smalldatetime,    
           @idpassport int,    
           @employeeFilter nvarchar(max),    
           @userFieldsFilter nvarchar(max),
		   @initialTime smalldatetime,
		   @endTime smalldatetime
           AS    
		   
          DECLARE @employeeIDs Table(idEmployee int)        
        DECLARE @pinitialDate smalldatetime = (select convert(date, @initialDate)),        
           @pendDate smalldatetime = (select convert(date, @endDate)), 		   
           @pidpassport int = @idpassport,                        
           @puserFieldsFilter nvarchar(max) = @userFieldsFilter,
		   @pemployeeFilter nvarchar(max) = @employeeFilter
     DECLARE @intdatefirst int  
     SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'  
     SET DateFirst @intdatefirst; 
	 
	 insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;       
	 
	 IF (@endTime > @initialTime)
	 BEGIN
		 SELECT *, 
		 CASE WHEN BeginJustificationTime < EndJustificationTime THEN 
		 DATEDIFF(SECOND, BeginJustificationTime, EndJustificationTime)*1.0/3600 	 
		 ELSE (DATEDIFF(SECOND, BeginJustificationTime, CAST('23:59:59' AS TIME))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndJustificationTime)*1.0/3600 
		 END AS TotalJustificationHHMM_ToHours, 
		 CASE WHEN BeginTimeInFilter <= EndTimeInFilter THEN DATEDIFF(SECOND, BeginTimeInFilter, EndTimeInFilter)*1.0/3600 
		 ELSE (DATEDIFF(SECOND, BeginTimeInFilter, (CAST('23:59:59' AS TIME)))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndTimeInFilter)*1.0/3600 
		 END AS TotalTimeInFilterHHMM_ToHours,
		 CASE WHEN BeginJustificationTime < EndJustificationTime THEN 
		 DATEDIFF(SECOND, BeginJustificationTime, EndJustificationTime)*1.0/3600 	 
		 ELSE (DATEDIFF(SECOND, BeginJustificationTime, CAST('23:59:59' AS TIME))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndJustificationTime)*1.0/3600 
		 END AS TotalJustification, 
		 CASE WHEN BeginTimeInFilter <= EndTimeInFilter THEN DATEDIFF(SECOND, BeginTimeInFilter, EndTimeInFilter)*1.0/3600 
		 ELSE (DATEDIFF(SECOND, BeginTimeInFilter, (CAST('23:59:59' AS TIME)))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndTimeInFilter)*1.0/3600 
		 END AS TotalTimeInFilter
		 FROM
			   (SELECT     
			   CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)     
									 + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup,     
									 dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.DailySchedule.Date AS Date_ToDateString, dbo.sysroDailyIncidencesDescription.Description AS IncidenceName,     
									 dbo.TimeZones.Name AS ZoneTime, dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS Value, SUM(dbo.DailyCauses.Value) AS ValueHHMM_ToHours, MONTH(dbo.DailySchedule.Date) AS Mes, YEAR(dbo.DailySchedule.Date) AS Año,     
									 (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, dbo.DailySchedule.Date) AS WeekOfYear, DATEPART(dy, dbo.DailySchedule.Date) AS DayOfYear, 
									 DATEPART(QUARTER, dbo.DailySchedule.Date) as Quarter, CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName,     
									 dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.FullGroupName AS FullGroupName, dbo.sysroEmployeeGroups.Path,     
									 dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,    
					dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString, dbo.DailySchedule.IDAssignment,    
				 case when dbo.DailySchedule.IDAssignment > 0 then (select Name FROM dbo.Assignments WHERE ID=dbo.DailySchedule.IDAssignment ) else '' END As AssignmentName,    
									(select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition)) as IDProductiveUnit,    
									(SELECT Name from ProductiveUnits where id in (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition))) as ProductiveUnitName,    
					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,    
					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,    
					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,    
					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,    
					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,    
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.DailySchedule.Date) As UserField1,    
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.DailySchedule.Date) As UserField2,    
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.DailySchedule.Date) As UserField3,    
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.DailySchedule.Date) As UserField4,    
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.DailySchedule.Date) As UserField5,    
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.DailySchedule.Date) As UserField6,    
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.DailySchedule.Date) As UserField7,    
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.DailySchedule.Date) As UserField8,    
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.DailySchedule.Date) As UserField9,    
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.DailySchedule.Date) As UserField10,
 				   dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age, convert(nvarchar(max),dbo.sysroRemarks.Text) as Remark,
				   dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,
				   dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date) as Punches,
				   CAST (dbo.dailyincidences.begintime AS TIME) AS BeginJustificationTime, 
			   CAST(dbo.dailyincidences.endtime AS TIME) AS EndJustificationTime, 	
			   CASE WHEN CAST(DBO.DailyIncidences.Date AS DATE) BETWEEN CAST(@INITIALDATE AS DATE) AND CAST(@ENDDATE AS DATE) AND CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME)
					THEN CAST(dbo.DailyIncidences.BeginTime AS TIME)
					WHEN CAST(DBO.DailyIncidences.Date AS DATE) BETWEEN CAST(@INITIALDATE AS DATE) AND CAST(@ENDDATE AS DATE) AND CAST(dbo.DailyIncidences.EndTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME) AND CAST(dbo.DailyIncidences.BeginTime AS TIME) NOT BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME)
					THEN CAST(@initialTime AS TIME)
					WHEN CAST(DBO.DailyIncidences.Date AS DATE) BETWEEN CAST(@INITIALDATE AS DATE) AND CAST(@ENDDATE AS DATE) AND CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) NOT BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME)
					THEN CAST(DBO.DailyIncidences.BeginTime AS TIME)
					WHEN ((CAST(DBO.DailyIncidences.Date AS DATE) BETWEEN CAST(@INITIALDATE AS DATE) AND CAST(@ENDDATE AS DATE) AND CAST(@initialTime AS TIME) BETWEEN CAST(DBO.DailyIncidences.BeginTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) AND CAST(@endTime AS TIME) BETWEEN CAST(DBO.DailyIncidences.BeginTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME))
					OR (CAST(DBO.DailyIncidences.BeginTime AS TIME) > CAST(DBO.DailyIncidences.EndTime AS TIME) AND CAST(@initialTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) AND CAST(@endTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME)))
					THEN CAST(@initialTime AS TIME)				
			   END AS BeginTimeInFilter,
			   CASE WHEN CAST(DBO.DailyIncidences.Date AS DATE) BETWEEN CAST(@INITIALDATE AS DATE) AND CAST(@ENDDATE AS DATE) AND CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME)
					THEN CAST(dbo.DailyIncidences.EndTime AS TIME)
					WHEN CAST(DBO.DailyIncidences.Date AS DATE) BETWEEN CAST(@INITIALDATE AS DATE) AND CAST(@ENDDATE AS DATE) AND CAST(dbo.DailyIncidences.EndTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME) AND CAST(dbo.DailyIncidences.BeginTime AS TIME) NOT BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME)
					THEN CAST(DBO.DailyIncidences.EndTime AS TIME)
					WHEN CAST(DBO.DailyIncidences.Date AS DATE) BETWEEN CAST(@INITIALDATE AS DATE) AND CAST(@ENDDATE AS DATE) AND CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) NOT BETWEEN CAST(@initialTime AS TIME) AND CAST(@endTime AS TIME)
					THEN CAST(@endTime AS TIME)
					WHEN ((CAST(DBO.DailyIncidences.Date AS DATE) BETWEEN CAST(@INITIALDATE AS DATE) AND CAST(@ENDDATE AS DATE) AND CAST(@initialTime AS TIME) BETWEEN CAST(DBO.DailyIncidences.BeginTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) AND CAST(@endTime AS TIME) BETWEEN CAST(DBO.DailyIncidences.BeginTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME))
					OR (CAST(DBO.DailyIncidences.BeginTime AS TIME) > CAST(DBO.DailyIncidences.EndTime AS TIME) AND CAST(@initialTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) AND CAST(@endTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME)))
					THEN CAST(@endTime AS TIME)
			   END AS EndTimeInFilter
			   FROM         dbo.sysroEmployeeGroups with (nolock)    
				   INNER JOIN dbo.Causes with (nolock)    
				   INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause     
				   INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailySchedule.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate    
				   INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailySchedule.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailySchedule.Date between dbo.EmployeeContracts.BeginDate AND dbo.EmployeeContracts.EndDate     
				   INNER JOIN dbo.sysroDailyIncidencesDescription with (nolock)    
				   INNER JOIN dbo.DailyIncidences with (nolock)    
				   INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID ON dbo.sysroDailyIncidencesDescription.IDIncidence = dbo.DailyIncidences.IDType ON dbo.DailyCauses.IDEmployee = dbo.DailyIncidences.IDEmployee AND dbo.DailyCauses.Date = dbo.DailyIncidences.Date			  
				   AND dbo.DailyCauses.IDRelatedIncidence = dbo.DailyIncidences.ID AND dbo.DailyIncidences.Date  between DATEADD(DAY, -1,dbo.sysroEmployeeGroups.BeginDate) AND dbo.sysroEmployeeGroups.EndDate    
				   LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.DailySchedule.IDShiftUsed = dbo.Shifts.ID     
				   LEFT OUTER JOIN dbo.Shifts AS Shifts_1 with (nolock) ON dbo.DailySchedule.IDShift1 = Shifts_1.ID    
				   LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID    
				LEFT OUTER JOIN dbo.sysroRemarks on dbo.sysroRemarks.ID = dbo.DailySchedule.Remarks    
				LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
			  WHERE dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.DailyCauses.IDEmployee,2,0,0,dbo.DailyCauses.date) > 1                 		  
				AND dbo.sysroEmployeeGroups.IDEmployee in(select idEmployee from @employeeIDs)     
				and dbo.dailyCauses.Date between DATEADD(DAY,-1,@pinitialDate) and @pendDate    
			   GROUP BY dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name,     
									 YEAR(dbo.DailySchedule.Date), CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)     
						 + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120), CASE WHEN DailySchedule.Date <= GETDATE()     
									 THEN Shifts.Name ELSE Shifts_1.Name END, dbo.Employees.Name, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, DATEPART(iso_week,     
									 dbo.DailySchedule.Date), DATEPART(dy, dbo.DailySchedule.Date), (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1,     
									 dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,     
									 dbo.sysroEmployeeGroups.CurrentEmployee, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate, dbo.DailySchedule.IDAssignment, dbo.DailySchedule.IDDailyBudgetPosition, convert(nvarchar(max),dbo.sysroRemarks.Text),
									 dbo.sysrovwEmployeeLockDate.LockDate,
									 dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date),
									 dbo.dailyincidences.begintime, dbo.dailyincidences.endtime,DBO.DailyIncidences.Date) AS SUB	
									 WHERE BeginTimeInFilter IS NOT NULL AND (DATEDIFF(SECOND, BeginTimeInFilter, EndTimeInFilter)*1.0/3600 > 0)								 									 									 
		END
		ELSE
		BEGIN
			SELECT *, 
			 CASE WHEN BeginJustificationTime < EndJustificationTime THEN 
			 DATEDIFF(SECOND, BeginJustificationTime, EndJustificationTime)*1.0/3600 	 
			 ELSE (DATEDIFF(SECOND, BeginJustificationTime, CAST('23:59:59' AS TIME))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndJustificationTime)*1.0/3600 
			 END AS TotalJustificationHHMM_ToHours, 
			 CASE WHEN BeginTimeInFilter <= EndTimeInFilter THEN DATEDIFF(SECOND, BeginTimeInFilter, EndTimeInFilter)*1.0/3600 
			 ELSE (DATEDIFF(SECOND, BeginTimeInFilter, (CAST('23:59:59' AS TIME)))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndTimeInFilter)*1.0/3600 
			 END AS TotalTimeInFilterHHMM_ToHours,
			 CASE WHEN BeginJustificationTime < EndJustificationTime THEN 
			 DATEDIFF(SECOND, BeginJustificationTime, EndJustificationTime)*1.0/3600 	 
			 ELSE (DATEDIFF(SECOND, BeginJustificationTime, CAST('23:59:59' AS TIME))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndJustificationTime)*1.0/3600 
			 END AS TotalJustification, 
			 CASE WHEN BeginTimeInFilter <= EndTimeInFilter THEN DATEDIFF(SECOND, BeginTimeInFilter, EndTimeInFilter)*1.0/3600 
			 ELSE (DATEDIFF(SECOND, BeginTimeInFilter, (CAST('23:59:59' AS TIME)))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndTimeInFilter)*1.0/3600 
			 END AS TotalTimeInFilter
			 FROM
				   (SELECT     
				   CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)     
										 + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup,     
										 dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.DailySchedule.Date AS Date_ToDateString, dbo.sysroDailyIncidencesDescription.Description AS IncidenceName,     
										 dbo.TimeZones.Name AS ZoneTime, dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS Value, SUM(dbo.DailyCauses.Value) AS ValueHHMM_ToHours, MONTH(dbo.DailySchedule.Date) AS Mes, YEAR(dbo.DailySchedule.Date) AS Año,     
										 (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, dbo.DailySchedule.Date) AS WeekOfYear, DATEPART(dy, dbo.DailySchedule.Date) AS DayOfYear, 
										 DATEPART(QUARTER, dbo.DailySchedule.Date) as Quarter, CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName,     
										 dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.FullGroupName AS FullGroupName, dbo.sysroEmployeeGroups.Path,     
										 dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,    
						dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString, dbo.DailySchedule.IDAssignment,    
					 case when dbo.DailySchedule.IDAssignment > 0 then (select Name FROM dbo.Assignments WHERE ID=dbo.DailySchedule.IDAssignment ) else '' END As AssignmentName,    
										(select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition)) as IDProductiveUnit,    
										(SELECT Name from ProductiveUnits where id in (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition))) as ProductiveUnitName,    
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,    
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,    
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,    
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,    
						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,    
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.DailySchedule.Date) As UserField1,    
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.DailySchedule.Date) As UserField2,    
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.DailySchedule.Date) As UserField3,    
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.DailySchedule.Date) As UserField4,    
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.DailySchedule.Date) As UserField5,    
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.DailySchedule.Date) As UserField6,    
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.DailySchedule.Date) As UserField7,    
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.DailySchedule.Date) As UserField8,    
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.DailySchedule.Date) As UserField9,    
						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.DailySchedule.Date) As UserField10,
 					   dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age, convert(nvarchar(max),dbo.sysroRemarks.Text) as Remark,
					   dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,
					   dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date) as Punches,
					   CAST (dbo.dailyincidences.begintime AS TIME) AS BeginJustificationTime, 
				   CAST(dbo.dailyincidences.endtime AS TIME) AS EndJustificationTime, 	
				   CASE 
						WHEN ((CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST('23:59:59' AS TIME) AND (CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST('23:59:59' AS TIME))))
						OR(CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@endTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@endTime AS TIME))
						OR (CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST('23:59:59' AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@endTime AS TIME))
						THEN CAST(DBO.DAILYINCIDENCES.BEGINTIME AS TIME)
						WHEN (((CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST('23:59:59' AS TIME))
						OR (CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@endTime AS TIME))) AND CAST(DBO.DailyIncidences.BeginTimE AS TIME) < CAST(@initialTIME AS TIME))
						THEN CAST(@INITIALTIME AS TIME)
						WHEN (CAST(DBO.DailyIncidences.BEGINTIME AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@ENDTIME AS TIME))
						OR (CAST(DBO.DailyIncidences.BEGINTIME AS TIME) BETWEEN CAST(@INITIALTIME AS TIME) AND CAST('23:59:59' AS TIME))
						THEN CAST(DBO.DAILYINCIDENCES.BEGINTIME AS TIME)
					    WHEN CAST(dbo.DailyIncidences.EndTime AS TIME) < CAST(dbo.DailyIncidences.BeginTime AS TIME) AND CAST(@INITIALTIME AS TIME) BETWEEN CAST(DBO.DailyIncidences.BEGINTIME AS TIME) AND CAST('23:59:59' AS TIME) 
						AND CAST(@ENDTIME AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME)
						THEN CAST(@INITIALTIME AS TIME)
				   END AS BeginTimeInFilter,
				   CASE 
						WHEN ((CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST('23:59:59' AS TIME) AND (CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST('23:59:59' AS TIME))))
						OR(CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@endTime AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@endTime AS TIME))
						OR (CAST(DBO.DailyIncidences.BeginTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST('23:59:59' AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@endTime AS TIME))
						THEN CAST(DBO.DAILYINCIDENCES.EndTime AS TIME)
						WHEN (((CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST(@initialTime AS TIME) AND CAST('23:59:59' AS TIME))
						OR (CAST(DBO.DailyIncidences.EndTime AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@endTime AS TIME))) AND CAST(DBO.DailyIncidences.BeginTimE AS TIME) < CAST(@initialTIME AS TIME))
						THEN CAST(DBO.DailyIncidences.EndTime AS TIME)
						WHEN (CAST(DBO.DailyIncidences.BEGINTIME AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(@ENDTIME AS TIME))
						OR (CAST(DBO.DailyIncidences.BEGINTIME AS TIME) BETWEEN CAST(@INITIALTIME AS TIME) AND CAST('23:59:59' AS TIME))
						THEN CAST(@endTime AS TIME)
					    WHEN CAST(@INITIALTIME AS TIME) BETWEEN CAST(DBO.DailyIncidences.BEGINTIME AS TIME) AND CAST('23:59:59' AS TIME) 
						AND CAST(@ENDTIME AS TIME) BETWEEN CAST('00:00:00' AS TIME) AND CAST(DBO.DailyIncidences.EndTime AS TIME)
						THEN CAST(@endTime AS TIME)				   
				   END AS EndTimeInFilter
				   FROM         dbo.sysroEmployeeGroups with (nolock)    
					   INNER JOIN dbo.Causes with (nolock)    
					   INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause     
					   INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailySchedule.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate    
					   INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailySchedule.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailySchedule.Date between dbo.EmployeeContracts.BeginDate AND dbo.EmployeeContracts.EndDate     
					   INNER JOIN dbo.sysroDailyIncidencesDescription with (nolock)    
					   INNER JOIN dbo.DailyIncidences with (nolock)    
					   INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID ON dbo.sysroDailyIncidencesDescription.IDIncidence = dbo.DailyIncidences.IDType ON dbo.DailyCauses.IDEmployee = dbo.DailyIncidences.IDEmployee AND dbo.DailyCauses.Date = dbo.DailyIncidences.Date			  
					   AND dbo.DailyCauses.IDRelatedIncidence = dbo.DailyIncidences.ID AND CAST(dbo.DailyIncidences.Date AS DATE)  between CAST(@initialDate AS DATE) AND CAST(DATEADD(DAY, 1,@endDate) AS DATE)
					   LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.DailySchedule.IDShiftUsed = dbo.Shifts.ID     
					   LEFT OUTER JOIN dbo.Shifts AS Shifts_1 with (nolock) ON dbo.DailySchedule.IDShift1 = Shifts_1.ID    
					   LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID    
					LEFT OUTER JOIN dbo.sysroRemarks on dbo.sysroRemarks.ID = dbo.DailySchedule.Remarks    
					LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
				  WHERE dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.DailyCauses.IDEmployee,2,0,0,dbo.DailyCauses.date) > 1                 		  
					AND dbo.sysroEmployeeGroups.IDEmployee in(select idEmployee from @employeeIDs)     
					and dbo.dailyCauses.Date between DATEADD(DAY,-1,@pinitialDate) and @pendDate    
				   GROUP BY dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name,     
										 YEAR(dbo.DailySchedule.Date), CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)     
							 + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120), CASE WHEN DailySchedule.Date <= GETDATE()     
										 THEN Shifts.Name ELSE Shifts_1.Name END, dbo.Employees.Name, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, DATEPART(iso_week,     
										 dbo.DailySchedule.Date), DATEPART(dy, dbo.DailySchedule.Date), (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1,     
										 dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,     
										 dbo.sysroEmployeeGroups.CurrentEmployee, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate, dbo.DailySchedule.IDAssignment, dbo.DailySchedule.IDDailyBudgetPosition, convert(nvarchar(max),dbo.sysroRemarks.Text),
										 dbo.sysrovwEmployeeLockDate.LockDate,
										 dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date),
										 dbo.dailyincidences.begintime, dbo.dailyincidences.endtime,DBO.DailyIncidences.Date) AS SUB	
										 WHERE BeginTimeInFilter IS NOT NULL 										 
										 AND ((BeginTimeInFilter <= EndTimeInFilter AND DATEDIFF(SECOND, BeginTimeInFilter, EndTimeInFilter)*1.0/3600 >0)
										  OR (BEGINTIMEINFILTER > ENDTIMEINFILTER AND (DATEDIFF(SECOND, BeginTimeInFilter, (CAST('23:59:59' AS TIME)))+1)*1.0/3600 + DATEDIFF(SECOND, CAST('00:00:00' AS TIME), EndTimeInFilter)*1.0/3600 > 0))
										 
		END


GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusViews] where IdPassport = 0 and Name = 'justificationsByUserDateTime')
INSERT INTO [dbo].[GeniusViews]
           ([IdPassport]
           ,[Name]
           ,[Description]
           ,[DS]
           ,[TypeView]
           ,[CreatedOn]
           ,[Employees]
           ,[DateFilterType]
           ,[DateInf]
           ,[DateSup]
           ,[CubeLayout]
           ,[Concepts]
           ,[UserFields]
           ,[BusinessCenters]
           ,[CustomFields]
           ,[DSFunction]
           ,[Feature]
           ,[RequieredFeature])
     VALUES
           (0,	'justificationsByUserDateTime',	'',	'C',	1,	'20220125',	'',	0,	'20220125',	'20220125',	'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"}],"rows":[{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"},{"uniqueName":"IncidenceName"},{"uniqueName":"CauseName"},{"uniqueName":"BeginTimeInFilter"},{"uniqueName":"EndTimeInFilter"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Tiempo Total Justificación_ToHours","formula":"sum(\"TotalJustification\")"},{"uniqueName":"Tiempo Total Periodo_ToHours","formula":"sum(\"TotalTimeInFilter\")"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"columns"}},"creationDate":"2022-01-28T17:59:45.632Z"}','','','','{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}',	'Genius_IncidencesByTime(@initialDate,@endDate,@idpassport,@employeeFilter,@userFieldsFilter,@initialTime,@endTime)',	'Calendar',	'Calendar.Analytics')
GO

CREATE PROCEDURE [dbo].[RemoveHistoryFromUserField]
  	(
      @UserFieldName nvarchar(max)
  	)
  	AS
	BEGIN
		DECLARE @pUserFieldName nvarchar(max) = @UserFieldName

		update dbo.EmployeeUserFieldValues set date = '19000102' where FieldName = (select FieldName from sysroUserFields where isnull(Alias,FieldName) = @pUserFieldName ) and Date = '19000101'

		update dbo.sysroUserFields set History = 0 where isnull(Alias,FieldName) = @pUserFieldName

		MERGE dbo.EmployeeUserFieldValues AS euf 
		USING(select *
		from (
		  select s.IDEmployee, 
				 s.FieldName,
				 s.Date,
				 max(s.Date) over (partition by s.IDEmployee) as max_date
		  from dbo.EmployeeUserFieldValues s
		  where  FieldName = (select FieldName from sysroUserFields where isnull(Alias,FieldName) = @pUserFieldName )
		) t
		where Date = max_date) AS uf   
		ON euf.IDEmployee = uf.IDEmployee and euf.FieldName = uf.FieldName and euf.Date = uf.Date
		WHEN MATCHED THEN
		UPDATE SET euf.Date = '19000101';

		delete from dbo.EmployeeUserFieldValues where FieldName = (select FieldName from sysroUserFields where isnull(Alias,FieldName) = @pUserFieldName ) and Date <> '19000101'
	END
GO

exec RemoveHistoryFromUserField 'NIF'
GO

exec RemoveHistoryFromUserField 'sysroImportKey'
GO

ALTER FUNCTION [dbo].[EmployeeZonesBetweenDates]
   (				
 	@datebeginpar smalldatetime,
 	@dateendpar smalldatetime,
	@employeeidlist nvarchar(max)
   )
   RETURNS @ValueTable table(IDEmployee int, IDContract NVARCHAR(50), TelecommutingExpected BIT, TelecommutePlanned BIT, InTelecommute BIT, RefDate smalldatetime, ContractWorkCenter NVARCHAR(50),
							 DailyWorkCenter NVARCHAR(50), CalculatedWorkCenter NVARCHAR(50), InAbsence BIT, NoWork BIT, ZoneOnDate NVARCHAR(50), ExpectedZone NVARCHAR(50)) 
   AS
   BEGIN
    declare @iniperiod smalldatetime 
 	declare @endperiod smalldatetime
	DECLARE @employees nvarchar(max)
	DECLARE @employeeIDs Table(idEmployee int)
 	SET @iniperiod = @datebeginpar
 	SET @endperiod = @dateendpar
	SET @employees = @employeeidlist

	IF LEN(@employees) = 0 
		INSERT INTO @ValueTable
		SELECT Employees.Id AS IDEmployee,
			   EmployeeContracts.IDContract,
			   CASE WHEN (Agreement.Telecommuting = 1 AND ISNULL(Agreement.TelecommutingMandatoryDays,'') = '') THEN 0 ELSE (CASE WHEN CHARINDEX(CONVERT(VARCHAR,(DATEPART(dw, dt) + @@DATEFIRST - 1 ) % 7), Agreement.TelecommutingMandatoryDays) > 0 THEN 1 ELSE 0 END) END AS TelecommutingExpected,
			   DailySchedule.Telecommuting AS TelecommutePlanned,
			   ISNULL(PunchesTC.ID,0) InTelecommute,
			   dt AS [RefDate],
			   ISNULL(EmployeeContracts.Enterprise,'') as ContractWorkCenter,
			   ISNULL(DailySchedule.WorkCenter,'') as DailyWorkCenter,
			   CASE WHEN DailySchedule.WorkCenter IS NOT NULL THEN ISNULL(DailySchedule.WorkCenter,'') ELSE ISNULL(EmployeeContracts.Enterprise,'') END AS CalculatedWorkCenter,
			   CASE WHEN ProgrammedAbsences.IDEmployee IS NULL THEN 0 ELSE 1 END AS InAbsence,
			   CASE WHEN Shifts.ID IS NULL OR Shifts.ShiftType = 2 OR Shifts.ExpectedWorkingHours = 0 THEN 1 ELSE 0 END AS NoWork,
			   CASE WHEN Punches_LastOnDate.ActualType IS NOT NULL AND Punches_LastOnDate.ActualType <> 2 THEN  ISNULL(ZoneOnPastDate.Name,'?')  END AS ZoneOnDate,
			   CASE WHEN Punches_LastOnPastDates.ActualType IS NOT NULL AND Punches_LastOnPastDates.ActualType <> 2 AND dt >= DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) THEN ISNULL(ZoneExpectedOnDate.Name,'?') END  AS ExpectedZone
		FROM   [dbo].[Alldays] (@iniperiod, @endperiod)
			   LEFT JOIN EmployeeContracts WITH(NOLOCK) ON alldays.dt BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate
			   LEFT JOIN sysrovwTelecommutingAgreement Agreement ON Agreement.IDContract = EmployeeContracts.IDContract 
			   INNER JOIN Employees WITH(NOLOCK) ON Employees.ID = EmployeeContracts.IDEmployee
			   LEFT JOIN DailySchedule WITH(NOLOCK) ON DailySchedule.Date = alldays.dt AND DailySchedule.IDEmployee = EmployeeContracts.IDemployee
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Shiftdate ORDER BY DateTime DESC) AS 'RowNumberTC', * FROM Punches WITH(NOLOCK) WHERE InTelecommute = 1 AND Shiftdate >= @iniperiod) PunchesTC ON PunchesTC.IDEmployee = EmployeeContracts.IDemployee AND PunchesTC.ShiftDate = dt AND (RowNumberTC = 1 OR RowNumberTC IS NULL)
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Shiftdate ORDER BY DateTime DESC) AS 'RowNumberLastOnDate', * FROM Punches WITH(NOLOCK) WHERE IDZone > 0 AND ActualType <> 2 AND Shiftdate >= @iniperiod) Punches_LastOnDate ON Punches_LastOnDate.IDEmployee = EmployeeContracts.IDemployee AND Punches_LastOnDate.ShiftDate = dt AND (RowNumberLastOnDate = 1 OR RowNumberLastOnDate IS NULL)
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY DateTime DESC) AS 'RowNumberLastOnPastDates', * FROM Punches WITH(NOLOCK) WHERE IDZone > 0 AND ActualType <> 2 AND Shiftdate >= DATEADD(day,-30,@iniperiod) AND ShiftDate <= DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)) Punches_LastOnPastDates ON Punches_LastOnPastDates.IDEmployee = EmployeeContracts.IDemployee AND (RowNumberLastOnPastDates = 1 OR RowNumberLastOnPastDates IS NULL)
			   LEFT JOIN Zones AS ZoneOnPastDate ON ZoneOnPastDate.ID = Punches_LastOnDate.IDZone
			   LEFT JOIN Zones AS ZoneExpectedOnDate ON ZoneExpectedOnDate.ID = Punches_LastOnPastDates.IDZone
			   LEFT JOIN ProgrammedAbsences WITH(NOLOCK) ON ProgrammedAbsences.IDEmployee = EmployeeContracts.IDEmployee AND dt BETWEEN ProgrammedAbsences.BeginDate AND isnull(ProgrammedAbsences.FinishDate,DATEADD(day,ProgrammedAbsences.maxlastingdays,ProgrammedAbsences.BeginDate))
			   LEFT JOIN Shifts WITH(NOLOCK) ON Shifts.ID = DailySchedule.IDShift1
	ELSE
		INSERT INTO @ValueTable
		SELECT Employees.Id AS IDEmployee,
			   EmployeeContracts.IDContract,
			   CASE WHEN (Agreement.Telecommuting = 1 AND ISNULL(Agreement.TelecommutingMandatoryDays,'') = '') THEN 0 ELSE (CASE WHEN CHARINDEX(CONVERT(VARCHAR,(DATEPART(dw, dt) + @@DATEFIRST - 1 ) % 7), Agreement.TelecommutingMandatoryDays) > 0 THEN 1 ELSE 0 END) END AS TelecommutingExpected,
			   DailySchedule.Telecommuting AS TelecommutePlanned,
			   ISNULL(PunchesTC.ID,0) InTelecommute,
			   dt AS [RefDate],
			   ISNULL(EmployeeContracts.Enterprise,'') as ContractWorkCenter,
			   ISNULL(DailySchedule.WorkCenter,'') as DailyWorkCenter,
			   CASE WHEN DailySchedule.WorkCenter IS NOT NULL THEN ISNULL(DailySchedule.WorkCenter,'') ELSE ISNULL(EmployeeContracts.Enterprise,'') END AS CalculatedWorkCenter,
			   CASE WHEN ProgrammedAbsences.IDEmployee IS NULL THEN 0 ELSE 1 END AS InAbsence,
			   CASE WHEN Shifts.ID IS NULL OR Shifts.ShiftType = 2 OR Shifts.ExpectedWorkingHours = 0 THEN 1 ELSE 0 END AS NoWork,
			   CASE WHEN Punches_LastOnDate.ActualType IS NOT NULL AND Punches_LastOnDate.ActualType <> 2 THEN  ISNULL(ZoneOnPastDate.Name,'?')  END AS ZoneOnDate,
			   CASE WHEN Punches_LastOnPastDates.ActualType IS NOT NULL AND Punches_LastOnPastDates.ActualType <> 2 AND dt >= DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) THEN ISNULL(ZoneExpectedOnDate.Name,'?') END  AS ExpectedZone
		FROM   [dbo].[Alldays] (@iniperiod, @endperiod)
			   LEFT JOIN EmployeeContracts WITH(NOLOCK) ON alldays.dt BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate
			   LEFT JOIN sysrovwTelecommutingAgreement Agreement ON Agreement.IDContract = EmployeeContracts.IDContract 
			   INNER JOIN Employees WITH(NOLOCK) ON Employees.ID = EmployeeContracts.IDEmployee
			   LEFT JOIN DailySchedule WITH(NOLOCK) ON DailySchedule.Date = alldays.dt AND DailySchedule.IDEmployee = EmployeeContracts.IDemployee
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Shiftdate ORDER BY DateTime DESC) AS 'RowNumberTC', * FROM Punches WITH(NOLOCK) WHERE InTelecommute = 1 AND Shiftdate >= @iniperiod) PunchesTC ON PunchesTC.IDEmployee = EmployeeContracts.IDemployee AND PunchesTC.ShiftDate = dt AND (RowNumberTC = 1 OR RowNumberTC IS NULL)
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Shiftdate ORDER BY DateTime DESC) AS 'RowNumberLastOnDate', * FROM Punches WITH(NOLOCK) WHERE IDZone > 0 AND ActualType <> 2 AND Shiftdate >= @iniperiod) Punches_LastOnDate ON Punches_LastOnDate.IDEmployee = EmployeeContracts.IDemployee AND Punches_LastOnDate.ShiftDate = dt AND (RowNumberLastOnDate = 1 OR RowNumberLastOnDate IS NULL)
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY DateTime DESC) AS 'RowNumberLastOnPastDates', * FROM Punches WITH(NOLOCK) WHERE IDZone > 0 AND ActualType <> 2 AND Shiftdate >= DATEADD(day,-30,@iniperiod) AND ShiftDate <= DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)) Punches_LastOnPastDates ON Punches_LastOnPastDates.IDEmployee = EmployeeContracts.IDemployee AND (RowNumberLastOnPastDates = 1 OR RowNumberLastOnPastDates IS NULL)
			   LEFT JOIN Zones AS ZoneOnPastDate ON ZoneOnPastDate.ID = Punches_LastOnDate.IDZone
			   LEFT JOIN Zones AS ZoneExpectedOnDate ON ZoneExpectedOnDate.ID = Punches_LastOnPastDates.IDZone
			   LEFT JOIN ProgrammedAbsences WITH(NOLOCK) ON ProgrammedAbsences.IDEmployee = EmployeeContracts.IDEmployee AND dt BETWEEN ProgrammedAbsences.BeginDate AND isnull(ProgrammedAbsences.FinishDate,DATEADD(day,ProgrammedAbsences.maxlastingdays,ProgrammedAbsences.BeginDate))
			   LEFT JOIN Shifts WITH(NOLOCK) ON Shifts.ID = DailySchedule.IDShift1
        WHERE EmployeeContracts.IDEmployee IN (SELECT Value AS IDEmployye FROM SplitToInt(@employees,','))
	RETURN
   END
GO

UPDATE sysroliveAdvancedParameters SET Value = '1' where ParameterName = 'VisualTime.Procees.NotifierVersion'
GO

UPDATE sysroParameters SET Data='529' WHERE ID='DBVersion'
GO


