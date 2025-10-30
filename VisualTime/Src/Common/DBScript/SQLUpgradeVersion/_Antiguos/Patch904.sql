ALTER PROCEDURE [dbo].[Report_porcentaje_absentismo]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@FechaInicio datetime2 = '20210301',
	@FechaFin datetime2 = '20210301'
AS
select 	--expectedworkinghours
	(select isnull(SUM(COALESCE(ds.ExpectedWorkingHours, shifts.ExpectedWorkingHours)),0) 	
	from shifts inner join sysroDailyScheduleByContract on 
		sysroDailyScheduleByContract.idshift1 = shifts.id
		inner join (select * from split(@IDEmployee,',')) tmp on sysroDailyScheduleByContract.IDEmployee = tmp.Value
		inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = sysroDailyScheduleByContract.IDEmployee and sysroDailyScheduleByContract.Date between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
		inner join DailySchedule ds on ds.IDEmployee = sysroDailyScheduleByContract.IDEmployee and ds.Date = sysroDailyScheduleByContract.Date
	where sysroDailyScheduleByContract.idemployee = sysrovwallemployeegroups.idemployee
		and cast(sysroDailyScheduleByContract.date as date) between cast(@FechaInicio as date) and cast(@FechaFin as date) and sysroDailyScheduleByContract.NumContrato=EmployeeContracts.idcontract
	) as expectedworkinghours,
	--abstotal
	(select isnull(sum(sysroDailyAccrualsByContract.value),0)
    	from sysroDailyAccrualsByContract INNER JOIN Concepts
		ON sysroDailyAccrualsByContract.IDConcept=Concepts.ID
		inner join (select * from split(@IDEmployee,',')) tmp on sysroDailyAccrualsByContract.IDEmployee = tmp.Value
				inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = sysroDailyAccrualsByContract.IDEmployee and sysroDailyAccrualsByContract.Date between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
    	where sysroDailyAccrualsByContract.IDEmployee = sysrovwallemployeegroups.idemployee
		and cast(date as date) between cast(@FechaInicio as date) and cast(@FechaFin as date) and sysroDailyAccrualsByContract.NumContrato=EmployeeContracts.idcontract
		and IsAbsentiism = 1
	) as abstotal,
	--abstotalrewarded
	(select isnull(sum(sysroDailyAccrualsByContract.value),0) 
	from sysroDailyAccrualsByContract INNER JOIN Concepts
		ON sysroDailyAccrualsByContract.IDConcept=Concepts.ID
		inner join (select * from split(@IDEmployee,',')) tmp on sysroDailyAccrualsByContract.IDEmployee = tmp.Value
						inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = sysroDailyAccrualsByContract.IDEmployee and sysroDailyAccrualsByContract.Date between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
	where sysroDailyAccrualsByContract.IDEmployee = sysrovwallemployeegroups.idemployee
		and cast(date as date) between cast(@FechaInicio as date) and cast(@FechaFin as date) and sysroDailyAccrualsByContract.NumContrato=EmployeeContracts.idcontract
		and IsAbsentiism   = 1
		and AbsentiismRewarded = 1
	) as abstotalrewarded,
	--abstotalnotrewarded
	(select isnull(sum(sysroDailyAccrualsByContract.value),0)
	from sysroDailyAccrualsByContract INNER JOIN Concepts
		ON sysroDailyAccrualsByContract.IDConcept=Concepts.ID
		inner join (select * from split(@IDEmployee,',')) tmp on sysroDailyAccrualsByContract.IDEmployee = tmp.Value
				inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = sysroDailyAccrualsByContract.IDEmployee and sysroDailyAccrualsByContract.Date between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
	where sysroDailyAccrualsByContract.IDEmployee = sysrovwallemployeegroups.idemployee
		and cast(date as date) between cast(@FechaInicio as date) and cast(@FechaFin as date) and sysroDailyAccrualsByContract.NumContrato=EmployeeContracts.idcontract
		and IsAbsentiism   = 1
		and AbsentiismRewarded = 0
	) as abstotalnotrewarded,
	--abstotalrewardedBYGROUP
	 isnull((select isnull(sum(sysroDailyAccrualsByContract.value),0) 
	from sysroDailyAccrualsByContract INNER JOIN Concepts
		ON sysroDailyAccrualsByContract.IDConcept=Concepts.ID
		inner join (select * from split(@IDEmployee,',')) tmp on sysroDailyAccrualsByContract.IDEmployee = tmp.Value
				inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = sysroDailyAccrualsByContract.IDEmployee and sysroDailyAccrualsByContract.Date between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
		INNER JOIN sysrovwAllEmployeeGroups eg ON eg.IDEmployee = sysroDailyAccrualsByContract.IDEmployee
	where sysroDailyAccrualsByContract.IDEmployee = eg.idemployee
		and cast(date as date) between cast(@FechaInicio as date) and cast(@FechaFin as date) 
		and eg.IDGroup = sysrovwallemployeegroups.IDGroup
		and IsAbsentiism   = 1
		and AbsentiismRewarded = 1
		group by eg.IDGroup
	),0) as abstotalrewardedBYGROUP,
	--abstotalnotrewardedBYGROUP
	 isnull((select isnull(sum(sysroDailyAccrualsByContract.value),0)
	from sysroDailyAccrualsByContract INNER JOIN Concepts
		ON sysroDailyAccrualsByContract.IDConcept=Concepts.ID
		inner join (select * from split(@IDEmployee,',')) tmp on sysroDailyAccrualsByContract.IDEmployee = tmp.Value
				inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = sysroDailyAccrualsByContract.IDEmployee and sysroDailyAccrualsByContract.Date between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
		INNER JOIN sysrovwAllEmployeeGroups eg ON eg.IDEmployee = sysroDailyAccrualsByContract.IDEmployee
	where sysroDailyAccrualsByContract.IDEmployee = eg.idemployee
		and cast(date as date) between cast(@FechaInicio as date) and cast(@FechaFin as date) 
		and eg.IDGroup = sysrovwallemployeegroups.IDGroup
		and IsAbsentiism   = 1
		and AbsentiismRewarded = 0
	),0) as abstotalnotrewardedBYGROUP,
	sysrovwallemployeegroups.*, Employees.Name, EmployeeContracts.*
from sysrovwallemployeegroups , Employees, EmployeeContracts
inner join (select * from split(@IDEmployee,',')) tmp on EmployeeContracts.IDEmployee = tmp.Value
				inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = EmployeeContracts.IDEmployee and @FechaInicio between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
where sysrovwallemployeegroups.IDEmployee = Employees.ID and sysrovwallemployeegroups.IDEmployee = EmployeeContracts.idEmployee 
and EmployeeContracts.begindate<=@FechaFin and EmployeeContracts.EndDate>=@FechaInicio
order by FullGroupName
RETURN NULL 
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='904' WHERE ID='DBVersion'
GO
