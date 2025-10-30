ALTER PROCEDURE [dbo].[Genius_Supervisors]
          @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max) AS  
  		DECLARE @employeeIDs Table(idEmployee int)    		   		
  		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter 
		
  		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter, @pinitialDate, @pendDate;    		
  		
  		SELECT DBO.EMPLOYEES.Name as Supervisor, DBO.sysroGroupFeatures.Name as Rol,
		QL.LevelOfAuthority as 'QualityLevel', TM.LevelOfAuthority as 'TimeManagementLevel', L.LevelOfAuthority as 'LaborLevel',
		P.LevelOfAuthority as 'PlanningLevel', PR.LevelOfAuthority as 'PreventionLevel', S.LevelOfAuthority as 'SecurityLevel',
		DBO.sysrovwCurrentEmployeeGroups.FullGroupName, DBO.sysrovwCurrentEmployeeGroups.EmployeeName, 1 AS Enabled,
		dbo.GetEmployeeUserFieldValueMin(sysrovwCurrentEmployeeGroups.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),@endDate) As UserField1,  
  					dbo.GetEmployeeUserFieldValueMin(sysrovwCurrentEmployeeGroups.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),@endDate) As UserField2,  
  					dbo.GetEmployeeUserFieldValueMin(sysrovwCurrentEmployeeGroups.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),@endDate) As UserField3,  
  					dbo.GetEmployeeUserFieldValueMin(sysrovwCurrentEmployeeGroups.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),@endDate) As UserField4,  
  					dbo.GetEmployeeUserFieldValueMin(sysrovwCurrentEmployeeGroups.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),@endDate) As UserField5,  
  					dbo.GetEmployeeUserFieldValueMin(sysrovwCurrentEmployeeGroups.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),@endDate) As UserField6,  
  					dbo.GetEmployeeUserFieldValueMin(sysrovwCurrentEmployeeGroups.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),@endDate) As UserField7,  
  					dbo.GetEmployeeUserFieldValueMin(sysrovwCurrentEmployeeGroups.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),@endDate) As UserField8,  
  					dbo.GetEmployeeUserFieldValueMin(sysrovwCurrentEmployeeGroups.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),@endDate) As UserField9,  
  					dbo.GetEmployeeUserFieldValueMin(sysrovwCurrentEmployeeGroups.IDEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@endDate) As UserField10
		FROM DBO.sysroPassports
		INNER JOIN DBO.Employees ON DBO.sysroPassports.IDEmployee = DBO.EMPLOYEES.ID
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
		LEFT OUTER JOIN DBO.sysroPermissionsOverGroups ON DBO.sysroPermissionsOverGroups.PassportID = DBO.sysroPassports.IDParentPassport AND EmployeeFeatureID = 1 AND Permission > 0
		LEFT OUTER JOIN DBO.sysrovwCurrentEmployeeGroups ON DBO.sysrovwCurrentEmployeeGroups.IDGroup = DBO.sysroPermissionsOverGroups.EmployeeGroupID
		LEFT OUTER JOIN DBO.EmployeeContracts AS UC ON UC.IDEmployee = DBO.sysrovwCurrentEmployeeGroups.IDEmployee
		WHERE IsSupervisor = 1 AND FullGroupName IS NOT NULL
		AND dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,DBO.Employees.Id,2,0,0,GetDate()) > 1      
		AND DBO.EMPLOYEES.Id in(SELECT idEmployee FROM @employeeIDs) and @pinitialDate >= DBO.EmployeeContracts.BeginDate and @pendDate <= DBO.EmployeeContracts.EndDate
		and @pinitialDate >= UC.BeginDate and @pendDate <= UC.EndDate
		and @pinitialDate >= DBO.EmployeeContracts.BeginDate and @pendDate <= DBO.EmployeeContracts.EndDate
		and @pinitialDate >= DBO.sysrovwCurrentEmployeeGroups.BeginDate and @pendDate <= DBO.sysrovwCurrentEmployeeGroups.EndDate
		group by DBO.EMPLOYEES.Name, DBO.sysroGroupFeatures.Name, QL.LevelOfAuthority, TM.LevelOfAuthority, L.LevelOfAuthority,
		P.LevelOfAuthority, PR.LevelOfAuthority, S.LevelOfAuthority, DBO.sysrovwCurrentEmployeeGroups.FullGroupName, DBO.sysrovwCurrentEmployeeGroups.EmployeeName,
		DBO.sysroPassports.IDEmployee, sysrovwCurrentEmployeeGroups.IDEmployee


GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='704' WHERE ID='DBVersion'
GO
