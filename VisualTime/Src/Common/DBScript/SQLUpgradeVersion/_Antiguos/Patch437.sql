IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.Datalink.ExternAccessIPs')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.Datalink.ExternAccessIPs','')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.Datalink.ExternAccessUserName')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.Datalink.ExternAccessUserName','')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.Datalink.ExternAccessPassword')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.Datalink.ExternAccessPassword','')
GO

ALTER TABLE dbo.ExportGuides ADD
	WSParameters nvarchar(MAX) NULL
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.Debug.LogLevel')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.Debug.LogLevel','NONE')
GO

alter FUNCTION [dbo].[WebLogin_GetPermissionOverEmployee]    
	(@idPassport int, @idEmployee int, @idApplication int, @mode int, @includeGroups bit, @date datetime)  RETURNS int    
      AS    
	BEGIN    
		DECLARE @Result int    
		DECLARE @parentPassport int    
		DECLARE @GroupType nvarchar(50),  
		@pidPassport int = @idPassport,    
        @pidEmployee int = @idEmployee,    
        @pidApplication int = @idApplication,    
        @pmode int = @mode,  
        @pincludeGroups bit = @includeGroups ,  
        @pdate datetime   = @date,
		@employeeResult int,
		@calendarlock int
      
	SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @pidPassport   
	select @calendarlock = isnull(COUNT(*),0) from Groups where CloseDate <> null
	 
    IF @GroupType = 'U'    
    BEGIN    
        SET @parentPassport = @pidPassport    
    END    
    ELSE    
    BEGIN    
        SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @pidPassport    
    END    

	SELECT @Result = Permission, 
		@employeeResult = isnull((select poe.Permission from sysroPermissionsOverEmployeesExceptions poe where poe.PassportID = @parentPassport and poe.EmployeeFeatureID = @pidApplication and poe.EmployeeID = @pidEmployee),9)
		FROM sysroPermissionsOverGroups where EmployeeGroupID = dbo.GetEmployeeGroup(@pidEmployee,@pdate) 
			and sysroPermissionsOverGroups.EmployeeFeatureID =  @pidApplication and sysroPermissionsOverGroups.PassportID = @parentPassport


	IF @Result > @employeeResult
	BEGIN
	 SET @Result = @employeeResult
	END

	IF @calendarlock > 0
	BEGIN
		IF @Result > 3 AND @pidApplication = 2 AND dbo.IsBlockDateRestrictionActive(@pidPassport,dbo.GetEmployeeGroup(@pidEmployee,@date),@date) = 1 SET @Result = 3    
	END

	
	RETURN @Result    
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =23101)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]
			   ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
		 VALUES
			   (23101,2310,'Calendar.Punches.Punches.Photo','Fotos de fichajes','','U','R',NULL,NULL,2)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 23101, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2310

	INSERT INTO [dbo].[sysroGroupFeatures_PermissionsOverFeatures] SELECT [IDGroupFeature], 23101, [Permision]  FROM [dbo].[sysroGroupFeatures_PermissionsOverFeatures] WHERE [IDFeature] = 2310

	UPDATE [dbo].[sysroPassports_PermissionsOverFeatures] set Permission = 3 where IDFeature = 23101 and Permission > 0

	UPDATE [dbo].[sysroGroupFeatures_PermissionsOverFeatures] set [Permision] = 3 where IDFeature = 23101 and [Permision] > 0
END
GO

ALTER PROCEDURE  [dbo].[Analytics_Schedule]
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
    SELECT     CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
		+ '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10) AS KeyView, 
		dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, 
		dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeesShifts.ShiftName, 
		dbo.sysroEmployeesShifts.CurrentDate AS Date, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours, COUNT(*) AS Count, 
		MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año, (DATEPART(dw, 
		dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear, 
		DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, 
		dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.endDate,
		dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.endDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,
		dbo.sysroEmployeesShifts.IDAssignment,
		CONVERT(NVARCHAR(4000),dbo.sysroEmployeesShifts.Remark) as Remark,
		dbo.sysroEmployeesShifts.AssignmentName,
		dbo.sysroEmployeesShifts.IDProductiveUnit,
		dbo.sysroEmployeesShifts.ProductiveUnitName,
		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),@pendDate) As UserField1,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),@pendDate) As UserField2,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),@pendDate) As UserField3,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),@pendDate) As UserField4,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),@pendDate) As UserField5,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),@pendDate) As UserField6,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),@pendDate) As UserField7,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),@pendDate) As UserField8,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),@pendDate) As UserField9,
		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@pendDate) As UserField10
    FROM         dbo.sysroEmployeesShifts with (nolock) 
		INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.sysroEmployeeGroups.endDate 
		INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.EmployeeContracts.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.EmployeeContracts.endDate 
		LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
	WHERE  dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeesShifts.IDEmployee,2,0,0,dbo.sysroEmployeesShifts.CurrentDate) > 1
		AND dbo.sysroEmployeesShifts.IDEmployee in( select idEmployee from @employeeIDs) and dbo.sysroEmployeesShifts.CurrentDate between @pinitialDate and @pendDate
	GROUP BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name, 
		dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate, MONTH(dbo.sysroEmployeesShifts.CurrentDate), 
		YEAR(dbo.sysroEmployeesShifts.CurrentDate), (DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, 
		dbo.sysroEmployeesShifts.CurrentDate), DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate), dbo.sysroEmployeeGroups.IDGroup, 
		dbo.EmployeeContracts.IDContract, CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
		+ '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10), dbo.sysroEmployeeGroups.Path, 
		dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.endDate,
		dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.endDate,dbo.sysroEmployeesShifts.IDAssignment,dbo.sysroEmployeesShifts.AssignmentName,dbo.sysroEmployeesShifts.IDProductiveUnit,dbo.sysroEmployeesShifts.ProductiveUnitName, CONVERT(NVARCHAR(4000),dbo.sysroEmployeesShifts.Remark)
GO

ALTER PROCEDURE [dbo].[Analytics_Incidences]
    	@initialDate smalldatetime,
    	@endDate smalldatetime,
    	@idpassport int,
    	@employeeFilter nvarchar(max),
    	@userFieldsFilter nvarchar(max)
     AS
     create table #employeeIDs (idEmployee int)    
	 DECLARE @pinitialDate smalldatetime = @initialDate,    
     @pendDate smalldatetime = @endDate,    
     @pidpassport int = @idpassport,    
     @pemployeeFilter nvarchar(max) = @employeeFilter,    
     @puserFieldsFilter nvarchar(max) = @userFieldsFilter    
         insert into #employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;   
     SELECT     CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
                           + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup, 
                           dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description AS IncidenceName, 
                           dbo.TimeZones.Name AS ZoneTime, dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS Value, YEAR(dbo.DailySchedule.Date) AS Año, 
                           (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailySchedule.Date) AS WeekOfYear, DATEPART(dy, 
                           dbo.DailySchedule.Date) AS DayOfYear, CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName, 
                           dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.FullGroupName AS FullGroupName, dbo.sysroEmployeeGroups.Path, 
                           dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,
    						dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod, dbo.DailySchedule.IDAssignment,
							case when dbo.DailySchedule.IDAssignment > 0 then (select Name FROM dbo.Assignments WHERE ID=dbo.DailySchedule.IDAssignment ) else '' END As AssignmentName,
                          (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition)) as IDProductiveUnit,
                          (SELECT Name from ProductiveUnits where id in (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition))) as ProductiveUnitName,
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
    						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@pendDate) As UserField10,
							CONVERT(NVARCHAR(4000),dbo.sysroRemarks.Text) as Remark
     FROM         dbo.sysroEmployeeGroups with (nolock)
    					INNER JOIN dbo.Causes with (nolock)
    					INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause 
    					INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailySchedule.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate
    					INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailySchedule.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailySchedule.Date between dbo.EmployeeContracts.BeginDate AND dbo.EmployeeContracts.EndDate 
    					LEFT OUTER JOIN dbo.sysroDailyIncidencesDescription with (nolock)
    					INNER JOIN dbo.DailyIncidences with (nolock)
    					INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID ON dbo.sysroDailyIncidencesDescription.IDIncidence = dbo.DailyIncidences.IDType ON dbo.DailyCauses.IDEmployee = dbo.DailyIncidences.IDEmployee AND dbo.DailyCauses.Date = dbo.DailyIncidences.Date AND dbo.DailyCauses.IDRelatedIncidence = dbo.DailyIncidences.ID AND dbo.DailyIncidences.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate
    					LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.DailySchedule.IDShiftUsed = dbo.Shifts.ID 
    					LEFT OUTER JOIN dbo.Shifts AS Shifts_1 with (nolock) ON dbo.DailySchedule.IDShift1 = Shifts_1.ID
    					LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
						LEFT OUTER JOIN dbo.sysroRemarks on dbo.sysroRemarks.ID = dbo.DailySchedule.Remarks
    WHERE	dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.DailyCauses.IDEmployee,2,0,0,dbo.DailyCauses.date) > 1
    			AND dbo.sysroEmployeeGroups.IDEmployee in(select idEmployee from #employeeIDs) 
   			and dbo.dailyCauses.Date between @pinitialDate and @pendDate
     GROUP BY dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name, 
                           YEAR(dbo.DailySchedule.Date), CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
                           + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120), CASE WHEN DailySchedule.Date <= GETDATE() 
                           THEN Shifts.Name ELSE Shifts_1.Name END, dbo.Employees.Name, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, DATEPART(wk, 
                           dbo.DailySchedule.Date), DATEPART(dy, dbo.DailySchedule.Date), (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1, 
                           dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, 
                           dbo.sysroEmployeeGroups.CurrentEmployee, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate, dbo.DailySchedule.IDAssignment, dbo.DailySchedule.IDDailyBudgetPosition, CONVERT(NVARCHAR(4000),dbo.sysroRemarks.Text)
GO

ALTER TABLE [dbo].[ExportGuides] ADD CONSTRAINT PK_UID UNIQUE (ID)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI_Actions] WHERE IDPath = 'EmployeeMultiAbsences')
insert into sysroGUI_Actions values ('EmployeeMultiAbsences', 'Portal\General\Employees\Employees', 'tbEmployeeMultiAbsences', 'Forms\Employees', 'U:Employees=Admin', 'ShowEmployeeMultiAbsences(''e'')', 'btnTbMassAbsence', 1, 4)
GO

ALTER TABLE visitor ADD CreatedBy integer null DEFAULT null
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI_Actions] WHERE IDPath = 'CopyAdvancedExport')
insert into sysroGUI_Actions values ('CopyAdvancedExport', 'Portal\General\DataLink\Export', 'tbCopyAdvancedExport', null, 'U:Employees=Admin', 'CopyCurrentExport(''#ID#'')', 'btnTbCopyExport', 1, 1)
GO

ALTER TABLE visit_fields add DEFAULT 0 FOR visittype
GO

UPDATE dbo.sysroParameters SET Data='437' WHERE ID='DBVersion'
GO


