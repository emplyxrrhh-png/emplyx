ALTER PROCEDURE [dbo].[Genius_Supervisors]
@initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max) AS
DECLARE @employeeIDs Table(idEmployee int)
DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter

insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter, @pinitialDate, @pendDate;

SELECT DBO.EMPLOYEES.Name as Supervisor, DBO.sysroGroupFeatures.Name as Rol,
QL.LevelOfAuthority as 'QualityLevel', TM.LevelOfAuthority as 'TimeManagementLevel', L.LevelOfAuthority as 'LaborLevel',
P.LevelOfAuthority as 'PlanningLevel', PR.LevelOfAuthority as 'PreventionLevel', S.LevelOfAuthority as 'SecurityLevel',
GR.FullGroupName, case when SUPEEMPINFO.Name is not null then SUPEEMPINFO.Name else 'ZZ Usuario restringido' end AS EmployeeName, 1 AS Enabled,
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
LEFT OUTER JOIN DBO.EMPLOYEES AS SUPEEMPINFO ON SUPEEMPINFO.ID = SUPEMP.IdEmployee AND (SELECT COUNT(*) FROM sysrovwSecurity_PermissionOverEmployeeAndFeature as pef2 WHERE pef2.IdFeature = 1 and @pendDate between PEF2.BeginDate and PEF2.EndDate and PEF2.idpassport = @pidpassport and PEF2.idemployee = SUPEEMPINFO.ID and PEF.FeaturePermission > 0) > 0
WHERE IsSupervisor = 1 AND FullGroupName IS NOT NULL
AND DBO.EMPLOYEES.Id in(SELECT idEmployee FROM @employeeIDs) and @pinitialDate >= DBO.EmployeeContracts.BeginDate and @pendDate <= DBO.EmployeeContracts.EndDate
and @pinitialDate >= SUPEMPCT.BeginDate and @pendDate <= SUPEMPCT.EndDate
and @pinitialDate >= DBO.EmployeeContracts.BeginDate and @pendDate <= DBO.EmployeeContracts.EndDate
and @pinitialDate >= SUPEEMPGR.BeginDate and @pendDate <= SUPEEMPGR.EndDate
group by DBO.EMPLOYEES.Name, DBO.sysroGroupFeatures.Name, QL.LevelOfAuthority, TM.LevelOfAuthority, L.LevelOfAuthority,
P.LevelOfAuthority, PR.LevelOfAuthority, S.LevelOfAuthority, GR.FullGroupName, SUPEEMPINFO.Name,
DBO.sysroPassports.IDEmployee, SUPEMP.IdEmployee


GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='802' WHERE ID='DBVersion'
GO
