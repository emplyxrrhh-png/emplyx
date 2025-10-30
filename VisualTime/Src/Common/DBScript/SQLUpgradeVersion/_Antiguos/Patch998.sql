ALTER PROCEDURE [dbo].[Report_ficha_empleado_contracts]
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '114,242'
AS
SELECT e.ID,
ec.IDContract, ec.BeginDate, ec.EndDate, la.Name as Convenio,
(select top 1 Value from EmployeeUserFieldValues where IDEmployee = e.ID AND FieldName = 'Jornada' and EmployeeUserFieldValues.date <= cast(getDate() as date) order by EmployeeUserFieldValues.date desc) As Jornada,
(SELECT CONVERT(decimal(20,0),Credential) FROM sysroPassports_AuthenticationMethods INNER JOIN sysroPassports ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport WHERE Credential != '' AND Method = 3 AND Version = '' AND BiometricID = 0 AND Enabled = 1 AND sysroPassports.IDEmployee = e.ID) as CardLive,
(select CONVERT(decimal(20,0),EmployeeContracts.IDCard) from EmployeeContracts Where EmployeeContracts.EndDate>=GETDATE() and EmployeeContracts.BeginDate<=GETDATE() AND EmployeeContracts.IDEmployee = e.ID) as CardNum
FROM employees e
inner join (select * from split(@IDEmployee,',')) tmp on e.ID = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe On poe.IDPassport = @idPassport
and (GETDATE() between poe.BeginDate and poe.EndDate
or GETDATE() between poe.BeginDate and poe.EndDate)
and poe.IdFeature = 1 and poe.FeaturePermission > 1
inner join EmployeeContracts ec ON ec.IDEmployee = e.ID
left join LabAgree la ON la.ID = ec.IDLabAgree
GROUP BY e.ID, ec.IDContract,  ec.BeginDate, ec.EndDate, la.Name
OPTION (USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'))
RETURN NULL
GO

ALTER   PROCEDURE [dbo].[Report_ficha_empleado]
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '114,242'
AS
SELECT e.ID,
eg.EmployeeName, eg.FullGroupName, AC.Name as GrupoAcceso
FROM employees e
inner join (select * from split(@IDEmployee,',')) tmp on e.ID = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe On poe.IDPassport = @idPassport
and (GETDATE() between poe.BeginDate and poe.EndDate
or GETDATE() between poe.BeginDate and poe.EndDate)
and poe.IdFeature = 1 and poe.FeaturePermission > 1
inner join sysrovwAllEmployeeGroups eg on e.ID = eg.IDEmployee
left join AccessGroups AC on e.IDAccessGroup = AC.ID
WHERE eg.EndDate>=GETDATE() and eg.BeginDate<=GETDATE()
GROUP BY e.ID, eg.EmployeeName, eg.FullGroupName, ac.Name
OPTION (USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'))
RETURN NULL
GO

ALTER PROCEDURE [dbo].[Report_ficha_empleado_authentication]
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '2'
AS
select distinct sp.IDEmployee, aut.Method 
from Employees e
inner join sysroPassports sp ON e.ID = sp.IDEmployee
inner join sysroPassports_AuthenticationMethods aut ON sp.ID = aut.IDPassport
where e.ID = @IDEmployee AND aut.Enabled = 1
OPTION (USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'))
RETURN NULL
GO

DROP PROCEDURE [dbo].[Report_ficha_empleado_conditions]
GO

DROP PROCEDURE [dbo].[Report_ficha_empleado_arras]
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='998' WHERE ID='DBVersion'
GO
