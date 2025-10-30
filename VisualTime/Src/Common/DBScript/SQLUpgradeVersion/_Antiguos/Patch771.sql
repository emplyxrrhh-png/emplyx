DROP PROCEDURE IF EXISTS [dbo].[Report_accesos_por_zona]
GO
CREATE PROCEDURE [dbo].[Report_accesos_por_zona]
	@idPassport nvarchar(100) = '1',
	@startDate smalldatetime = '20210301',
	@endDate smalldatetime = '20210301',
	@IDEmployee nvarchar(max) = '114,242',
	@Zonas nvarchar(max) = '114,242'
AS
select p.*, z.Name as Zona, t.Description as Terminal, e.FullGroupName, e.EmployeeName, e.GroupName, e.Path, e.IDGroup 
from Punches p 
inner join (select * from split(@IDEmployee,',')) tmp on p.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = p.IDEmployee and p.Datetime between poe.BeginDate and poe.EndDate and poe.IdFeature = 1 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
join sysrovwAllEmployeeGroups e ON p.IDEmployee = e.IDEmployee
join zones z ON p.IDZone = z.ID 
join terminals t ON p.IDTerminal = t.ID 
where (p.Type = 5 OR p.Type = 7)
and CAST(p.DateTime as date) between CAST(@startDate as date) and CAST(@endDate as date)
and p.IDZone IN (select * from split(@Zonas,',')) 
order by z.Name, FullGroupName
RETURN NULL
GO

DROP PROCEDURE IF EXISTS [dbo].[Report_bradford]
GO
CREATE PROCEDURE [dbo].[Report_bradford]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '114,242'
AS
select e.FullGroupName, e.EmployeeName, e.IDEmployee, e.GroupName, e.Path, e.IDGroup from sysrovwAllEmployeeGroups e 
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = e.IDEmployee and DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) between poe.BeginDate and poe.EndDate and poe.IdFeature = 1 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
RETURN NULL 
GO


-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='771' WHERE ID='DBVersion'
GO
