DROP PROCEDURE IF EXISTS [dbo].[Report_lista_de_tareas]
GO
CREATE PROCEDURE [dbo].[Report_lista_de_tareas]
	@idPassport nvarchar(100) = '1',
	@IDTask nvarchar(max) = '0'
AS
select isnull(Project,'') as Project,  Name, Description, barcode from Tasks 
inner join sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) 
on pof.IDPassport = @idPassport And pof.IdFeature = 25 AND pof.Permission > 3 
where id > 0 and id in (select * from split(@IDTask,','))
RETURN NULL 
GO


DROP PROCEDURE IF EXISTS [dbo].[Report_justificaciones_detalle]
GO
CREATE PROCEDURE [dbo].[Report_justificaciones_detalle]
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


DROP PROCEDURE IF EXISTS [dbo].[Report_justificaciones_detalle_totalizador]
GO
CREATE PROCEDURE [dbo].[Report_justificaciones_detalle_totalizador]
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


DROP PROCEDURE IF EXISTS [dbo].[Report_listado_diario_comedor]
GO
CREATE PROCEDURE [dbo].[Report_listado_diario_comedor]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@startDate smalldatetime = '20210301',
	@endDate smalldatetime = '20210301'
AS
select p.*, e.FullGroupName, e.EmployeeName, e.GroupName, e.Path, e.IDGroup from Punches p 
inner join (select * from split(@IDEmployee,',')) tmp on p.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = p.IDEmployee and p.DateTime between poe.BeginDate and poe.EndDate and poe.IdFeature = 24200 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
join sysrovwAllEmployeeGroups e ON p.IDEmployee = e.IDEmployee
where (p.InvalidType IS NULL OR p.InvalidType IN (0, 7)) 
and p.Type = 10
and CAST(p.DateTime as date) between cast(@startDate as date) and cast(@endDate as date)
order by FullGroupName
RETURN NULL 
GO


DROP PROCEDURE IF EXISTS [dbo].[Report_listado_diario_comedor_total]
GO
CREATE PROCEDURE [dbo].[Report_listado_diario_comedor_total]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@startDate smalldatetime = '20210301',
	@endDate smalldatetime = '20210301'
AS
select COUNT(p.ID) as Total from Punches p 
inner join (select * from split(@IDEmployee,',')) tmp on p.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = p.IDEmployee and p.DateTime between poe.BeginDate and poe.EndDate and poe.IdFeature = 24200 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
join sysrovwAllEmployeeGroups e ON p.IDEmployee = e.IDEmployee
where (p.InvalidType IS NULL OR p.InvalidType IN (0, 7)) 
and p.Type = 10
and CAST(p.DateTime as date) between cast(@startDate as date) and cast(@endDate as date)
RETURN NULL 
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='785' WHERE ID='DBVersion'
GO
