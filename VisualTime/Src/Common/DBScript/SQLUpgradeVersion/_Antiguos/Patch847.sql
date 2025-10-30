ALTER PROCEDURE [dbo].[Report_accesos_por_zona]
	@idPassport nvarchar(100) = '1',
	@startDate datetime2 = '20210301',
	@endDate datetime2 = '20210301',
	@IDEmployee nvarchar(max) = '114,242',
	@Zonas nvarchar(max) = '114,242',
	@accessType nvarchar(3) = '1,0'
AS
DECLARE @showValids BIT;
DECLARE @showInvalids BIT;
SET @showValids = CASE WHEN LEFT(@accessType, 1) = '1' THEN 1 ELSE 0 END;
SET @showInvalids = CASE WHEN RIGHT(@accessType, 1) = '1' THEN 1 ELSE 0 END;

select p.*, z.Name as Zona, t.Description as Terminal, e.FullGroupName, e.EmployeeName, e.GroupName, e.Path, e.IDGroup 
from Punches p 
inner join (select * from split(@IDEmployee,',')) tmp on p.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = p.IDEmployee and p.Datetime between poe.BeginDate and poe.EndDate and poe.IdFeature = 1 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
join sysrovwAllEmployeeGroups e ON p.IDEmployee = e.IDEmployee
join zones z ON p.IDZone = z.ID 
join terminals t ON p.IDTerminal = t.ID 
where 
(
    (@showValids = 1 AND p.Type IN (5, 7)) OR
    (@showInvalids = 1 AND p.Type = 6) OR
    (@showValids = 1 AND @showInvalids = 1 AND p.Type IN (5, 6, 7))
)
and CAST(p.DateTime as date) between CAST(@startDate as date) and CAST(@endDate as date)
and p.IDZone IN (select * from split(@Zonas,',')) 
order by z.Name, FullGroupName
RETURN NULL
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='847' WHERE ID='DBVersion'

GO
