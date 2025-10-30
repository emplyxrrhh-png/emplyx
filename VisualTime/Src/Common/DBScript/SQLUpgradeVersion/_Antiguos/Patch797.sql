CREATE OR ALTER PROCEDURE [dbo].[Report_bradford]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '114,242'
AS
select e.FullGroupName, e.EmployeeName, e.IDEmployee, e.GroupName, e.Path, e.IDGroup from sysrovwAllEmployeeGroups e 
inner join (select * from splitMAX(@IDEmployee,',')) tmp on e.IDEmployee = tmp.Value 
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = e.IDEmployee and DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) between poe.BeginDate and poe.EndDate and poe.IdFeature = 1 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
RETURN NULL 
GO


CREATE OR ALTER PROCEDURE [dbo].[Report_justificaciones_detalle]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@causes nvarchar(max) = '0',
	@FechaInicio smalldatetime = '20210301',
	@FechaFin smalldatetime = '20210301'
AS
select emp.ID AS EmployeeID, emp.Name As EmployeeName, ec.IDContract as Contract, g.Id as IDGroup, g.Name as GroupName, g.FullGroupName as Ruta, ds.Date, cs.Name as CauseName, ds.Value 
from DailyCauses ds inner join Employees emp on emp.ID = ds.IDEmployee 
inner join (select * from split(@IDEmployee,',')) tmp on ds.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = ds.IDEmployee and ds.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 3 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
inner join EmployeeContracts ec on ec.IDEmployee = ds.IDEmployee and ds.Date between ec.BeginDate and ec.EndDate 
inner join EmployeeGroups eg on eg.IDEmployee = ds.IDEmployee and ds.Date between eg.BeginDate and eg.EndDate 
inner join Groups g on eg.IDGroup = g.ID left join Causes cs on ds.IDCause = cs.ID 
where ds.IDCause in (select * from split(@causes,',')) and CAST(ds.Date as date) between cast(@FechaInicio as date) and cast(@FechaFin as date)
RETURN NULL
GO


CREATE OR ALTER PROCEDURE [dbo].[Report_justificaciones_detalle_totalizador]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@causes nvarchar(max) = '0',
	@ContractID nvarchar(max) = '0',
	@GroupId nvarchar(max) = '0',
	@FechaInicio smalldatetime = '20210301',
	@FechaFin smalldatetime = '20210301'
AS
select cs.Name AS CauseName, sum(ds.Value) AS CauseValue
from DailyCauses ds
inner join (select * from split(@IDEmployee,',')) tmp on ds.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = ds.IDEmployee and ds.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 3 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
inner join Employees emp on emp.ID = ds.IDEmployee
inner join EmployeeContracts ec on ec.IDEmployee = ds.IDEmployee and ds.Date between ec.BeginDate and ec.EndDate
inner join EmployeeGroups eg on eg.IDEmployee = ds.IDEmployee and ds.Date between eg.BeginDate and eg.EndDate
inner join Groups g on eg.IDGroup = g.ID
left join Causes cs on ds.IDCause = cs.ID
where cast(ds.Date as date) between cast(@FechaInicio as date) and cast(@FechaFin as date) and ds.IDCause in (select * from split(@causes,',')) and ec.IDContract = @ContractID and eg.IDGroup = @GroupId
group by cs.name
RETURN NULL 
GO

CREATE OR ALTER PROCEDURE [dbo].[Report_prevision_absentismo_subAbsences]
	@idPassport nvarchar(100) = '1',
	@Employees nvarchar(max) = '0',
	@StartDate smalldatetime = '20210301',
	@EndDate smalldatetime = '20210301',
	@idGroup nvarchar(max) = '0'
AS
select sysrovwAllEmployeeGroups.EmployeeName, CAST(ProgrammedAbsences.BeginDate AS DATE) as Date, CAST(ISNULL(ProgrammedAbsences.FinishDate,ProgrammedAbsences.BeginDate) AS DATE) as xFinish, ProgrammedAbsences.FinishDate, ProgrammedAbsences.MaxLastingDays, Causes.Name as Cause from sysrovwAllEmployeeGroups 
INNER JOIN ProgrammedAbsences ON sysrovwAllEmployeeGroups.IDEmployee=ProgrammedAbsences.IDEmployee
INNER JOIN Causes ON ProgrammedAbsences.IDCause=Causes.ID
inner join (select * from splitMAX(@Employees,',')) tmp on ProgrammedAbsences.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = ProgrammedAbsences.IDEmployee and cast(ProgrammedAbsences.BeginDate as date) between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
WHERE sysrovwAllEmployeeGroups.IDGroup = @idGroup
AND (cast(ProgrammedAbsences.BeginDate as date) BETWEEN cast(@StartDate as date) and cast(@EndDate as date)
 OR cast(ProgrammedAbsences.FinishDate as date) BETWEEN cast(@StartDate as date) and cast(@EndDate as date)
 OR cast(ProgrammedAbsences.BeginDate as date) >= cast(@StartDate as date) 
 AND cast(ProgrammedAbsences.FinishDate as date) <= ISNULL(cast(@EndDate as date),DATEADD(d,MaxLastingDays-1,ProgrammedAbsences.BeginDate))
 OR cast(@StartDate as date) BETWEEN cast(ProgrammedAbsences.BeginDate as date) AND ISNULL(ProgrammedAbsences.FinishDate,DATEADD(d,MaxLastingDays-1,ProgrammedAbsences.BeginDate))
 OR cast(@EndDate as date) BETWEEN cast(ProgrammedAbsences.BeginDate as date) AND ISNULL(ProgrammedAbsences.FinishDate,DATEADD(d,MaxLastingDays-1,ProgrammedAbsences.BeginDate))
 OR cast(ProgrammedAbsences.BeginDate as date) <= cast(@StartDate as date) AND ProgrammedAbsences.FinishDate >= cast(@EndDate as date))
RETURN NULL 
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='797' WHERE ID='DBVersion'
GO
