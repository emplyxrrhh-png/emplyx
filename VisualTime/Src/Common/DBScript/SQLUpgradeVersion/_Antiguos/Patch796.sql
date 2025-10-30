DROP PROCEDURE IF EXISTS [dbo].[Report_tareas_diarias_usuario]
GO
CREATE PROCEDURE [dbo].[Report_tareas_diarias_usuario]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@fechaStart date = '20210301',
	@fechaEnd date = '20210301'
AS
select da.Date, sum(da.Value) as TValue, da.IDEmployee, ec.IDContract, e.FullGroupName, e.EmployeeName, e.GroupName, e.Path, e.IDGroup, cast(efv.Value as nvarchar) as NIF from dailyAccruals da 
inner join (select * from splitMAX(@IDEmployee,',')) tmp on da.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport =  @idPassport And poe.IDEmployee = da.IDEmployee And cast(da.Date as date) between poe.BeginDate And poe.EndDate 
LEFT JOIN EmployeeUserFieldValues efv ON da.IDEmployee = efv.IDEmployee AND cast(efv.FieldName as nvarchar) LIKE 'NIF' 
join sysrovwAllEmployeeGroups e ON da.IDEmployee = e.IDEmployee
join EmployeeContracts ec ON da.IDEmployee = ec.IDEmployee
where cast(da.Date as date) between cast(@fechaStart as date) and cast(@fechaEnd as date) 
and ec.begindate<=cast(@fechaEnd as date) and ec.EndDate>=cast(@fechaStart as date) 
AND ec.IDContract = (select top 1 sec.IDContract from employeeContracts sec where da.Date between sec.BeginDate and sec.EndDate  and IDEmployee = da.IDEmployee)
AND da.IDConcept IN (select ID from Concepts where Description LIKE '%THT%') 
group by da.Date, ec.IDContract, da.IDEmployee, e.FullGroupName, e.EmployeeName, e.GroupName, e.Path, e.IDGroup, cast(efv.Value as nvarchar)
RETURN NULL
GO

DROP PROCEDURE IF EXISTS [dbo].[Report_tareas_diarias_usuario_sub]
GO
CREATE PROCEDURE [dbo].[Report_tareas_diarias_usuario_sub]
	@IDEmployee nvarchar(max) = '0',
	@Fecha datetime2 = '20210301'
AS
select t.project, t.name, dt.Value, dt.field4, dt.Date from dailyTaskAccruals dt JOIN tasks t ON dt.IDTask = t.ID 
where dt.IDEmployee = @IDEmployee and cast(dt.Date as date) = cast(@Fecha as date)
RETURN NULL
GO

-----------------------------

DROP PROCEDURE IF EXISTS [dbo].[Report_seguimiento_plan_horario_usuarios]
GO
CREATE PROCEDURE [dbo].[Report_seguimiento_plan_horario_usuarios]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@FechaInicio date = '20210301',
	@FechaFin date = '20210301'
AS
select es.IDEmployee, es.Name, es.IDContract, es.IDShift, es.ShiftName, es.ExpectedWorkingHours, es.CurrentDate, es.FullGroupName, cast(efv.Value as nvarchar) as NIF, (select SUM(sdi.Value) from dailyIncidences sdi 
where sdi.Date = di.Date and sdi.IDEmployee = di.IDEmployee and sdi.idType IN (1001, 1010, 1030, 1042, 1050)) as IncidenceValue from sysroEmployeesShifts es 
inner join (select * from splitMAX(@IDEmployee,',')) tmp on es.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport =  @idPassport And poe.IDEmployee = es.IDEmployee And cast(es.CurrentDate as date) between poe.BeginDate And poe.EndDate
LEFT JOIN EmployeeUserFieldValues efv ON es.IDEmployee = efv.IDEmployee AND efv.FieldName = 'NIF' 
left join dailyIncidences di ON es.IDEmployee = di.IDEmployee AND es.CurrentDate = di.Date
where cast(es.CurrentDate as date) between cast(@FechaInicio as date) and cast(@FechaFin as date) and es.IDShift is not null 
group by es.IDEmployee, es.Name, es.IDContract, es.IDShift, es.ShiftName, es.ExpectedWorkingHours, es.CurrentDate, es.FullGroupName, cast(efv.Value as nvarchar), di.Date, di.IDEmployee
order by FullGroupName, Name, es.CurrentDate
RETURN NULL
GO

DROP PROCEDURE IF EXISTS [dbo].[Report_seguimiento_plan_horario_usuarios_subExcess]
GO
CREATE PROCEDURE [dbo].[Report_seguimiento_plan_horario_usuarios_subExcess]
	@IDEmployee nvarchar(max) = '0',
	@Date date = '20210301'
AS
select SUM(di.Value) as Value from DailyIncidences di
where di.IDEmployee = @IDEmployee and cast(di.Date as date) = @Date
and di.IDType IN (1010, 1030, 1050)
RETURN NULL
GO

DROP PROCEDURE IF EXISTS [dbo].[Report_seguimiento_plan_horario_usuarios_subPunches]
GO
CREATE PROCEDURE [dbo].[Report_seguimiento_plan_horario_usuarios_subPunches]
	@IDEmployee nvarchar(max) = '0',
	@Date date = '20210301'
AS

select * from Punches p  
where p.IDEmployee = @IDEmployee and cast(p.ShiftDate as Date) = @Date
and p.ActualType IN (1,2)
RETURN NULL
GO

DROP PROCEDURE IF EXISTS [dbo].[Report_seguimiento_plan_horario_usuarios_subIncidencesCauses]
GO
CREATE PROCEDURE [dbo].[Report_seguimiento_plan_horario_usuarios_subIncidencesCauses]
	@IDEmployee nvarchar(max) = '0',
	@Date date = '20210301'
AS
select di.BeginTime, di.EndTime, c.Name as Cause, did.Description as Incidence, dc.Value, dc.IDCause from DailyCauses dc
JOIN Causes c ON c.ID = dc.IDCause
LEFT JOIN DailyIncidences di ON dc.IDRelatedIncidence = di.ID AND dc.IDEmployee = di.IDEmployee AND dc.Date = di.Date
LEFT JOIN sysroDailyIncidencesDescription did ON di.IDType = did.IDIncidence
where cast(dc.Date as Date) = @Date
and dc.IDEmployee = @IDEmployee
and (di.IDType IS NOT NULL OR di.IDType != 0)
RETURN NULL
GO


------------

ALTER PROCEDURE [dbo].[Report_justificaciones_detalle]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@causes nvarchar(max) = '0',
	@FechaInicio smalldatetime = '20210301',
	@FechaFin smalldatetime = '20210301'
AS
select emp.ID AS EmployeeID, emp.Name As EmployeeName, ec.IDContract as Contract, g.Id as IDGroup, g.Name as GroupName, g.FullGroupName as Ruta, ds.Date, cs.Name as CauseName, ds.Value 
from DailyCauses ds inner join Employees emp on emp.ID = ds.IDEmployee 
inner join (select * from split(@IDEmployee,',')) tmp on ds.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = ds.IDEmployee and ds.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 1 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
inner join EmployeeContracts ec on ec.IDEmployee = ds.IDEmployee and ds.Date between ec.BeginDate and ec.EndDate 
inner join EmployeeGroups eg on eg.IDEmployee = ds.IDEmployee and ds.Date between eg.BeginDate and eg.EndDate 
inner join Groups g on eg.IDGroup = g.ID left join Causes cs on ds.IDCause = cs.ID 
where ds.IDCause in (select * from split(@causes,',')) and CAST(ds.Date as date) between cast(@FechaInicio as date) and cast(@FechaFin as date)
RETURN NULL 
GO

ALTER PROCEDURE [dbo].[Report_justificaciones_detalle_totalizador]
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
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = ds.IDEmployee and ds.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 1 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
inner join Employees emp on emp.ID = ds.IDEmployee
inner join EmployeeContracts ec on ec.IDEmployee = ds.IDEmployee and ds.Date between ec.BeginDate and ec.EndDate
inner join EmployeeGroups eg on eg.IDEmployee = ds.IDEmployee and ds.Date between eg.BeginDate and eg.EndDate
inner join Groups g on eg.IDGroup = g.ID
left join Causes cs on ds.IDCause = cs.ID
where cast(ds.Date as date) between cast(@FechaInicio as date) and cast(@FechaFin as date) and ds.IDCause in (select * from split(@causes,',')) and ec.IDContract = @ContractID and eg.IDGroup = @GroupId
group by cs.name
RETURN NULL 
GO


ALTER PROCEDURE [dbo].[Report_lista_de_tareas]
	@idPassport nvarchar(100) = '1',
	@IDTask nvarchar(max) = '0'
AS
select isnull(Project,'') as Project,  Name, Description, barcode from Tasks 
inner join sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) 
on pof.IDPassport = @idPassport And pof.IdFeature = 1 AND pof.Permission > 1 
where id > 0 and id in (select * from split(@IDTask,','))
RETURN NULL 
GO


ALTER PROCEDURE [dbo].[Report_listado_diario_comedor]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@startDate smalldatetime = '20210301',
	@endDate smalldatetime = '20210301'
AS
select p.*, e.FullGroupName, e.EmployeeName, e.GroupName, e.Path, e.IDGroup from Punches p 
inner join (select * from split(@IDEmployee,',')) tmp on p.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = p.IDEmployee and p.DateTime between poe.BeginDate and poe.EndDate and poe.IdFeature = 1 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
join sysrovwAllEmployeeGroups e ON p.IDEmployee = e.IDEmployee
where (p.InvalidType IS NULL OR p.InvalidType IN (0, 7)) 
and p.Type = 10
and CAST(p.DateTime as date) between cast(@startDate as date) and cast(@endDate as date)
order by FullGroupName
RETURN NULL 
GO


ALTER PROCEDURE [dbo].[Report_listado_diario_comedor_total]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@startDate smalldatetime = '20210301',
	@endDate smalldatetime = '20210301'
AS
select COUNT(p.ID) as Total from Punches p 
inner join (select * from split(@IDEmployee,',')) tmp on p.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = p.IDEmployee and p.DateTime between poe.BeginDate and poe.EndDate and poe.IdFeature = 1 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
join sysrovwAllEmployeeGroups e ON p.IDEmployee = e.IDEmployee
where (p.InvalidType IS NULL OR p.InvalidType IN (0, 7)) 
and p.Type = 10
and CAST(p.DateTime as date) between cast(@startDate as date) and cast(@endDate as date)
RETURN NULL 
GO




-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='796' WHERE ID='DBVersion'
GO
