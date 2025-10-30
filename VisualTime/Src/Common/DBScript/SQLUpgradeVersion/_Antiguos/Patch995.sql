--PATCH SQL PARA INFORME FICHA DEL USUARIO
CREATE OR ALTER PROCEDURE [dbo].[Report_ficha_empleado]
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
GROUP BY e.ID, eg.EmployeeName, eg.FullGroupName, ac.Name 
OPTION (USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'))
RETURN NULL
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_ficha_empleado_contracts]
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
inner join LabAgree la ON la.ID = ec.IDLabAgree
GROUP BY e.ID, ec.IDContract,  ec.BeginDate, ec.EndDate, la.Name 
OPTION (USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'))
RETURN NULL
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_ficha_empleado_movility]
@IDEmployee nvarchar(max) = '1'
AS
SELECT eg.IDEmployee, eg.BeginDate, eg.EndDate, eg.GroupName FROM sysroEmployeeGroups eg
where eg.IDEmployee = @IDEmployee
ORDER BY eg.BeginDate desc
RETURN NULL
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_ficha_empleado_fields]
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '2' 
AS
select ef.*, uf.Category, uf.AccessLevel,
(select poe.featurePermission from sysrovwSecurity_PermissionOverEmployeeAndFeature poe where IdPassport = @idPassport and IdEmployee = ef.IDEmployee and FeatureAlias like 'Employees.UserFields.Information.Low' and GETDATE() between poe.BeginDate and poe.EndDate) as Low,
(select poe.featurePermission from sysrovwSecurity_PermissionOverEmployeeAndFeature poe where IdPassport = @idPassport and IdEmployee = ef.IDEmployee and FeatureAlias like 'Employees.UserFields.Information.Medium' and GETDATE() between poe.BeginDate and poe.EndDate) as Medium,
(select poe.featurePermission from sysrovwSecurity_PermissionOverEmployeeAndFeature poe where IdPassport = @idPassport and IdEmployee = ef.IDEmployee and FeatureAlias like 'Employees.UserFields.Information.High' and GETDATE() between poe.BeginDate and poe.EndDate) as High 
from EmployeeUserFieldValues ef inner join sysrouserfields uf on uf.FieldName = ef.FieldName
where ef.IDEmployee = @IDEmployee and ef.Date <= cast(getDate() as date)
RETURN NULL
GO


CREATE OR ALTER PROCEDURE [dbo].[Report_ficha_empleado_authentication]
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '2'
AS
select sp.IDEmployee, aut.Method, aut.Credential 
from Employees e 
inner join sysroPassports sp ON e.ID = sp.IDEmployee
inner join sysroPassports_AuthenticationMethods aut ON sp.ID = aut.IDPassport
where e.ID = @IDEmployee AND aut.Enabled = 1
OPTION (USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'))
RETURN NULL
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_ficha_empleado_plans]
@startDate datetime2 = '20210301',
@endDate datetime2 = '20210301',
@IDEmployee nvarchar(max) = '114,242'
AS
select s.Name, count(ds.Date) as Recuento, s.IsFloating, ds.StartShift1 
from DailySchedule ds 
inner join shifts s ON s.id = ds.IDShift1
where ds.IDEmployee = @IDEmployee and CAST(ds.Date as date) between CAST(@startDate as date) and CAST(@endDate as date)
--sysroEmployeesShifts ?
group by s.Name, s.IsFloating, ds.StartShift1 
order by s.Name 
RETURN NULL
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_ficha_empleado_absences]
@startDate datetime2 = '20210301',
@endDate datetime2 = '20210301',
@IDEmployee nvarchar(max) = '114,242'
AS
select c.Name as Cause, pa.BeginDate, pa.FinishDate, pa.MaxLastingDays
from ProgrammedAbsences pa
INNER JOIN Causes c ON pa.IDCause=c.ID
where pa.IDEmployee = @IDEmployee 
and (cast(pa.BeginDate as date) BETWEEN cast(@StartDate as date) and cast(@EndDate as date)
OR cast(pa.FinishDate as date) BETWEEN cast(@StartDate as date) and cast(@EndDate as date)
OR cast(pa.BeginDate as date) >= cast(@StartDate as date)
AND cast(pa.FinishDate as date) <= ISNULL(cast(@EndDate as date),DATEADD(d,MaxLastingDays-1,pa.BeginDate))
OR cast(@StartDate as date) BETWEEN cast(pa.BeginDate as date) AND ISNULL(pa.FinishDate,DATEADD(d,MaxLastingDays-1,pa.BeginDate))
OR cast(@EndDate as date) BETWEEN cast(pa.BeginDate as date) AND ISNULL(pa.FinishDate,DATEADD(d,MaxLastingDays-1,pa.BeginDate))
OR cast(pa.BeginDate as date) <= cast(@StartDate as date) AND pa.FinishDate >= cast(@EndDate as date))
RETURN NULL
GO


CREATE OR ALTER PROCEDURE [dbo].[Report_ficha_empleado_programmedCauses]
@startDate datetime2 = '20210301',
@endDate datetime2 = '20210301',
@IDEmployee nvarchar(max) = '114,242'
AS
select c.Name as Cause, pc.Date, pc.Duration 
from ProgrammedCauses pc
INNER JOIN Causes c ON pc.IDCause=c.ID
where pc.IDEmployee = @IDEmployee 
and ((
cast(pc.Date as date) >= cast(@StartDate as date) and cast(pc.Date as date) <= cast(@EndDate as date)
) or (
CAST(ISNULL(pc.FinishDate,pc.Date) AS DATE) >= cast(@StartDate as date) and CAST(ISNULL(pc.FinishDate,pc.Date) AS DATE) <= cast(@EndDate as date)
) or (
cast(pc.Date as date) < cast(@StartDate as date) AND CAST(ISNULL(pc.FinishDate,pc.Date) AS DATE) > cast(@EndDate as date)
))
RETURN NULL
GO


CREATE OR ALTER PROCEDURE [dbo].[Report_ficha_empleado_conditions]
@IDEmployee nvarchar(max) = '1'
AS
select ar.Name, ar.BeginDate, ar.EndDate 
from employeeAccrualsRules ea
INNER JOIN AccrualsRules ar ON ea.IDAccrualsRules=ar.IdAccrualsRule
where ea.IDEmployee = @IDEmployee 
RETURN NULL
GO


CREATE OR ALTER PROCEDURE [dbo].[Report_ficha_empleado_concepts]
@idPassport nvarchar(100) = '1',
@startDate datetime2 = '20210301',
@endDate datetime2 = '20210301',
@IDEmployee nvarchar(max) = '114,242'
AS
select c.ID, c.Name as Saldo, ec.IDContract, ec.BeginDate, ec.EndDate, 
(select sum(sda.value) from DailyAccruals sda where sda.IDEmployee = @IDEmployee and sda.IDConcept = da.IDConcept and cast(sda.Date as date) between cast((CASE WHEN ec.BeginDate > @startDate THEN ec.beginDate ELSE @startDate END) as date) and cast((CASE WHEN ec.EndDate < @endDate THEN ec.EndDate ELSE @endDate END) as date)) as SumaPeriodo,
(select sum(sda.value) from DailyAccruals sda where sda.IDEmployee = @IDEmployee and sda.IDConcept = da.IDConcept and cast(sda.Date as date) between cast((CASE WHEN ec.BeginDate > DATEFROMPARTS(YEAR(GETDATE()), 1, 1) THEN ec.beginDate ELSE DATEFROMPARTS(YEAR(GETDATE()), 1, 1) END) as date) and cast((CASE WHEN ec.EndDate < DATEFROMPARTS(YEAR(GETDATE()), 12, 31) THEN ec.EndDate ELSE DATEFROMPARTS(YEAR(GETDATE()), 12, 31) END) as date)) as SumaYear 
from Concepts c 
inner join DailyAccruals da ON da.IDConcept = c.ID
inner join EmployeeContracts ec ON da.IDEmployee = ec.IDEmployee
where da.IDEmployee = @IDEmployee AND cast(ec.BeginDate as date) <= cast(@endDate as date) and cast(ec.EndDate as date) >= cast(@startDate as date)
group by c.ID, c.Name, ec.IDContract, ec.BeginDate, ec.EndDate, da.IDConcept
order by c.Name 
OPTION (USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'))
RETURN NULL
GO


CREATE OR ALTER PROCEDURE [dbo].[Report_ficha_empleado_arras]
@IDEmployee nvarchar(max) = '1'
AS
select c.Name, al.StartupValue, al.MaxValue 
from employeeConceptAnnualLimits al
INNER JOIN Concepts c ON c.ID=al.IDConcept
where al.IDEmployee = @IDEmployee 
RETURN NULL
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='995' WHERE ID='DBVersion'
GO
