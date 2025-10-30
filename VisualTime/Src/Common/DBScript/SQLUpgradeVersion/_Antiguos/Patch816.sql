-- No borréis esta línea
CREATE OR ALTER   PROCEDURE [dbo].[Report_justificaciones_detalle]
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '0',
@causes nvarchar(max) = '0',
@FechaInicio datetime2 = '20210301',
@FechaFin datetime2 = '20210301'
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

CREATE OR ALTER   PROCEDURE [dbo].[Report_justificaciones_detalle_totalizador]
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '0',
@causes nvarchar(max) = '0',
@ContractID nvarchar(max) = '0',
@GroupId nvarchar(max) = '0',
@FechaInicio datetime2 = '20210301',
@FechaFin datetime2 = '20210301'
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

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='816' WHERE ID='DBVersion'
GO
