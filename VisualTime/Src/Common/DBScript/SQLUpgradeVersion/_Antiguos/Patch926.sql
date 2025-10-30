ALTER PROCEDURE [dbo].[Report_registro_jornada_laboral]
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '0',
@FechaInicio date = '20210301',
@FechaFin date = '20210301'
AS
select distinct ds.IDEmployee, punches.ShiftDate, s.Name, ds.Date, (select top 1 IDContract FROM EmployeeContracts where EmployeeContracts.IDEmployee = e.id order by begindate desc )  as IDContract,(SELECT TOP 1 cast(Value as nvarchar) FROM EmployeeUserFieldValues  WHERE EmployeeUserFieldValues.IDEmployee = e.ID and FieldName = 'NIF' ORDER BY EmployeeUserFieldValues.Date DESC) AS Dni, e.Name as EmployeeName
from DailySchedule ds
inner join (select * from splitMAX(@IDEmployee,',')) tmp on ds.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport =  @idPassport And poe.IDEmployee = ds.IDEmployee And cast(isnull(@FechaInicio,getdate()) as date) between poe.BeginDate And poe.EndDate
inner join Shifts s with (nolock) ON s.ID = ISNULL(ds.IDShiftUsed, ds.IDShift1)
inner join employees e with (nolock) ON e.ID = ds.IDEmployee
left join punches with (nolock) ON punches.IDEmployee = ds.IDEmployee and punches.ShiftDate = ds.Date and ActualType in (1,2) and punches.IDEmployee > 0
where cast(ds.Date as date) between cast(isnull(@FechaInicio,getdate()) as date) and cast(isnull(@FechaFin,getdate()) as date)
option (USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'))
RETURN NULL
GO


-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='926' WHERE ID='DBVersion'
GO
