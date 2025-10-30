CREATE OR ALTER PROCEDURE [dbo].[Report_dias_fichajes_impares]
@idPassport nvarchar(100) = '1',
@startDate datetime2 = '20210301',
@endDate datetime2 = '20210301',
@IDEmployee nvarchar(max) = '114,242'
AS
select vwid.*, es.IsFloating, es.StartFloatingOnDay, es.ShiftName, eg.EmployeeName, eg.FullGroupName
from sysrovwIncompletedDays vwid
inner join (select * from split(@IDEmployee,',')) tmp on vwid.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe On poe.IDPassport = @idPassport
And poe.IDEmployee = vwid.IDEmployee
and CAST(vwid.Date as date) between poe.BeginDate and poe.EndDate
and poe.IdFeature = 1 and poe.FeaturePermission > 1
inner join sysrovwAllEmployeeGroups eg on vwid.IDEmployee = eg.IDEmployee
inner join sysroEmployeesShifts es on vwid.IDEmployee = es.IDEmployee and vwid.Date = es.CurrentDate 
where CAST(vwid.Date as date) between CAST(@startDate as date) and CAST(@endDate as date)
order by vwid.Date
RETURN NULL
GO


CREATE OR ALTER PROCEDURE [dbo].[Report_dias_fichajes_impares_subPunches]
@currentDate datetime2 = '20210301',
@IDEmployee nvarchar(max) = '0'
AS
select p.DateTime, p.ActualType, p.IDTerminal, t.Description as terminal 
from punches p
left join Terminals t on t.ID = p.IDTerminal 
where CAST(p.ShiftDate as date) = CAST(@currentDate as date) and p.ActualType in (1,2) and p.IDEmployee = @IDEmployee
RETURN NULL
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='897' WHERE ID='DBVersion'
GO
