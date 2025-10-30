-- No borréis esta línea
CREATE OR ALTER PROCEDURE [dbo].[Report_fichajes_al_detalle_conaccesos_totales]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@fechaStart datetime2 = '20210301',
	@fechaEnd datetime2 = '20210301'
AS
select es.*, efv.Value as NIF 
, (select top 1 DateTime from Punches with (nolock) where  ActualType= 5 and IDZone in(select ID from Zones where isnull(IsWorkingZone, 0) = 1) and Punches.IDEmployee = es.IDEmployee and Punches.ShiftDate = es.CurrentDate order by DateTime asc) as FirstAcc
, (select top 1 DateTime from Punches with (nolock) where  ActualType= 5 and IDZone in(select ID from Zones where isnull(IsWorkingZone, 0) = 0) and Punches.IDEmployee = es.IDEmployee and Punches.ShiftDate = es.CurrentDate order by DateTime desc) as LastAcc
, (select top 1 DateTime from Punches with (nolock) where  ActualType= 1 and Punches.IDEmployee = es.IDEmployee and Punches.ShiftDate = es.CurrentDate order by DateTime asc) as FirstPre
, (select top 1 DateTime from Punches with (nolock) where  ActualType= 2 and Punches.IDEmployee = es.IDEmployee and Punches.ShiftDate = es.CurrentDate order by DateTime desc) as LastPre
from sysroEmployeesShifts es 
inner join (select * from splitMAX(@IDEmployee,',')) tmp on es.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport =  @idPassport And poe.IDEmployee = es.IDEmployee And cast(es.CurrentDate as date) between poe.BeginDate And poe.EndDate 
LEFT JOIN EmployeeUserFieldValues efv ON es.IDEmployee = efv.IDEmployee AND efv.FieldName LIKE 'NIF' 
where cast(es.CurrentDate as date) between cast(@fechaStart as date) and cast(@fechaEnd as date) 
and es.IDShift is not null
RETURN NULL 
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='947' WHERE ID='DBVersion'
GO
