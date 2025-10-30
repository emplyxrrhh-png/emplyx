-- No borréis esta línea
ALTER PROCEDURE [dbo].[Report_listado_diario_comedor]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@startDate smalldatetime = '20210301',
	@endDate smalldatetime = '20210301'
AS
select p.*, e.FullGroupName, e.EmployeeName, e.GroupName, e.Path, e.IDGroup,(select idcontract from employeecontracts with (nolock) where employeecontracts.idemployee = e.idemployee and employeecontracts.BeginDate <= p.ShiftDate and employeecontracts.EndDate >= p.ShiftDate) as IDContract, (select isnull(name, '')  from DiningRoomTurns with (nolock) where DiningRoomTurns.id = p.typeData) as DiningName from Punches p 
inner join (select * from split(@IDEmployee,',')) tmp on p.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = p.IDEmployee and p.DateTime between poe.BeginDate and poe.EndDate and poe.IdFeature = 1 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
join sysrovwAllEmployeeGroups e ON p.IDEmployee = e.IDEmployee
where (p.InvalidType IS NULL OR p.InvalidType IN (0, 7)) 
and p.Type = 10
and CAST(p.DateTime as date) between cast(@startDate as date) and cast(@endDate as date)
order by FullGroupName, EmployeeName, DateTime
RETURN NULL 
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='809' WHERE ID='DBVersion'
GO
