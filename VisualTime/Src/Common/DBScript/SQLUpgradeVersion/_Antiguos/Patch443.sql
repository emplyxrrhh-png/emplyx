ALTER TABLE dbo.ExportGuides ADD
	AutomaticDatePeriod nvarchar(MAX) NULL
GO

ALTER PROCEDURE [dbo].[Analytics_Punches]  
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
					dbo.Punches.TypeData AS PunchIDTypeData, CASE WHEN dbo.Punches.TypeData <> 0 THEN dbo.Causes.Name ELSE '' END AS PunchTypeData,   
					dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day,   
					MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Año, (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1)   
					% 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName,   
					dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END AS IsInvalid, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails)   
					AS Details, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate,   
					dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, dbo.Punches.IsNotReliable, dbo.Punches.Location AS PunchLocation,   
					dbo.Punches.LocationZone AS PunchLocationZone, dbo.Punches.FullAddress , dbo.Punches.TimeZone AS PunchTimeZone, dbo.sysroPassports.Name AS EditorName,  
					dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,  
					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),@pendDate) As UserField1,  
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),@pendDate) As UserField2,  
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),@pendDate) As UserField3,  
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),@pendDate) As UserField4,  
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),@pendDate) As UserField5,  
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),@pendDate) As UserField6,  
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),@pendDate) As UserField7,  
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),@pendDate) As UserField8,  
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),@pendDate) As UserField9,  
					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@pendDate) As UserField10  
      FROM         dbo.sysroEmployeeGroups with (nolock) INNER JOIN  
					dbo.Punches with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee AND dbo.Punches.ShiftDate >= dbo.sysroEmployeeGroups.BeginDate AND   
					dbo.Punches.ShiftDate <= dbo.sysroEmployeeGroups.EndDate INNER JOIN  
					dbo.EmployeeContracts with (nolock) ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND   
					dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN  
					dbo.Zones with (nolock) ON dbo.Punches.IDZone = dbo.Zones.ID LEFT OUTER JOIN  
					dbo.Terminals with (nolock) ON dbo.Punches.IDTerminal = dbo.Terminals.ID LEFT OUTER JOIN  
					dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID LEFT OUTER JOIN  
					dbo.sysroPassports with (nolock) ON dbo.Punches.IDPassport = dbo.sysroPassports.ID LEFT OUTER JOIN  
					dbo.Causes with (nolock) ON dbo.Punches.TypeData = dbo.Causes.ID  
      WHERE dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeeGroups.IDEmployee,2,0,0,dbo.Punches.ShiftDate) > 1  
					AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @pinitialDate and @pendDate  
      GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID,   
					dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108),   
					dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name,   
					(DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name,   
					CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path,   
					dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,   
					dbo.Punches.IsNotReliable, dbo.Punches.Location, dbo.Punches.LocationZone, dbo.Punches.FullAddress, dbo.Punches.TimeZone, dbo.sysroPassports.Name, dbo.Punches.ActualType,   
					dbo.Causes.Name, dbo.Punches.TypeData, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  
      HAVING (dbo.Punches.Type = 1) OR  
					(dbo.Punches.Type = 2) OR  
					(dbo.Punches.Type = 3) OR  
					(dbo.Punches.Type = 7 AND (dbo.Punches.ActualType = 1 OR dbo.Punches.ActualType = 2))
GO

 ALTER VIEW [dbo].[sysrovwAccessAuthorizations]
 AS
	 select IDEmployee, IDAuthorization, '0' as GroupPath  from EmployeeAccessAuthorization
	 UNION 
	 select id as IDEmployee, idaccessgroup as IDAuthorization, '0' as GroupPath from employees where IDAccessGroup is not null
	 UNION 
	 SELECT IDEmployee, IDAuthorization, gr.Path as GroupPath  FROM GroupsAccessAuthorization ga
	 join Groups gr on ga.idgroup = gr.id
	 join sysrovwCurrentEmployeeGroups ce2 on (ce2.Path = gr.Path or ce2.Path like gr.Path + '\%' )
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroReaderTemplates] WHERE [Type] = 'wx1' and ID=1)
	INSERT INTO  [dbo].[sysroReaderTemplates]
           ([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
	VALUES ('wx1',1,1,'TA',1,'Fast','X','Server','1,0','1,0',0,'1,0',0,'Remote',1,0,'')	
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroReaderTemplates] WHERE [Type] = 'wx1' and ID=2)
	INSERT INTO  [dbo].[sysroReaderTemplates]
           ([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
	VALUES ('wx1',1,2,'TSK',1,'Interactive','X','Server','1,0','1,0',0,'1,0',0,'Remote',1,0,'')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroReaderTemplates] WHERE [Type] = 'wx1' and ID=3)
	INSERT INTO  [dbo].[sysroReaderTemplates]
           ([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
	VALUES ('wx1',1,3,'TATSK',1,'Fast','X','Server','1,0','1,0',0,'1,0',0,'Remote',1,0,'')
GO


IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'VTLive.SMS.ConnectionString')
insert into sysroLiveAdvancedParameters values ('VTLive.SMS.ConnectionString', 'DefaultEndpointsProtocol=https;AccountName=rosp;AccountKey=cntFPCNRe+RQifgT3TrohnaSHVf6O420aBV2H2SiVZYt9anMMLtf0WETkTN6mLUqL+CEPkSadJ2r39mmAEEmew==;EndpointSuffix=core.windows.net')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'VTLive.SMS.QueueName')
insert into sysroLiveAdvancedParameters values ('VTLive.SMS.QueueName', 'saasautenticacion')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'VTLive.SMS.Enabled')
insert into sysroLiveAdvancedParameters values ('VTLive.SMS.Enabled', 'false')
GO

UPDATE dbo.sysroGUI_Actions SET AfterFunction = 'ShowRemoveEmployeeData(''#ID#'')' WHERE IDGUIPath = 'Portal\General\Employees\Employees' and IDPath = 'EmployeeDelWzd'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.Consents.Enabled')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.Consents.Enabled','false')
GO

ALTER TABLE [dbo].[Terminals] ADD SyncStartDate smalldatetime NULL
GO

INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode])
VALUES('mx8',1,105,'')
GO

update dbo.sysroLiveAdvancedParameters SET Value = 'CODED' where ParameterName = 'VTLive.Debug.LogLevel'
GO

UPDATE dbo.sysroParameters SET Data='443' WHERE ID='DBVersion'
GO


