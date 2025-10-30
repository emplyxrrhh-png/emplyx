ALTER PROCEDURE [dbo].[Report_accesos_por_zona]
	@idPassport nvarchar(100) = '1',
	@startDate datetime2 = '20210301',
	@endDate datetime2 = '20210301',
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

ALTER PROCEDURE [dbo].[Report_auditoria_fichajes]
	@idPassport nvarchar(100) = '1',
	@startDate datetime2 = '20210301',
	@endDate datetime2 = '20210301',
	@IDEmployee nvarchar(max) = '114,242'
AS
select p.action, p.DateTime, p.TimeStamp as AuditPunchDate, p.IDPassport, sp.Name as UserName, p.actualType, p.IDTerminal, t.Description as terminalDescription, t.Type as terminalType, eg.EmployeeName, p.IDEmployee, eg.FullGroupName, pc.IDPunch as punchCapture, p.isNotReliable, p.inTelecommute, (SELECT TOP 1 Value FROM EmployeeUserFieldValues  
WHERE EmployeeUserFieldValues.IDEmployee = 
p.IDEmployee and FieldName = 'NIF' ORDER BY EmployeeUserFieldValues.Date DESC) AS Dni, (select count(*) from requests r where r.RequestType in (2,15) and r.idemployee = p.idemployee and r.date1 = p.datetime) as Request 
from punches p 
inner join (select * from split(@IDEmployee,',')) tmp on p.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe On poe.IDPassport = @idPassport
And poe.IDEmployee = p.IDEmployee
and CAST(p.DateTime as date) between poe.BeginDate and poe.EndDate
and poe.IdFeature = 1 and poe.FeaturePermission > 1
left join Terminals t on p.IDTerminal = t.ID
left join PunchesCaptures pc on p.id = pc.IDPunch
join sysrovwAllEmployeeGroups eg on p.IDEmployee = eg.IDEmployee 
left join sysroPassports sp on p.IDPassport = sp.ID
where p.type between 1 and 3 
and CAST(p.DateTime as date) between CAST(@startDate as date) and CAST(@endDate as date) 
order by p.DateTime
RETURN NULL
GO

ALTER PROCEDURE [dbo].[Report_listado_diario_comedor]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@startDate datetime2 = '20210301',
	@endDate datetime2 = '20210301'
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

ALTER PROCEDURE [dbo].[Report_listado_diario_comedor_total]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@startDate datetime2 = '20210301',
	@endDate datetime2 = '20210301'
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


ALTER PROCEDURE [dbo].[Report_listado_total_comedor]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@startDate datetime2 = '20210301',
	@endDate datetime2 = '20210301'
AS
select  p.IDEmployee, p.Type, COUNT(p.Type) as Total,
(
select COUNT(sp.Type) from Punches sp 
inner join (select * from split(@IDEmployee,',')) tmp on sp.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = sp.IDEmployee and sp.DateTime between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
where sp.IDEmployee = p.IDEmployee 
and (sp.InvalidType IS NULL OR sp.InvalidType = 0) 
and sp.Type = 10
and CAST(sp.DateTime as date) between cast(@startDate as date) and cast(@endDate as date) 
group by sp.IDEmployee
) as TotalEmp, 
(
select COUNT(sp.Type) from Punches sp 
inner join (select * from split(@IDEmployee,',')) tmp on sp.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = sp.IDEmployee and sp.DateTime between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
join sysrovwAllEmployeeGroups se ON sp.IDEmployee = se.IDEmployee
where se.IDGroup = e.IDGroup 
and (sp.InvalidType IS NULL OR sp.InvalidType = 0) 
and sp.Type = 10
and CAST(sp.DateTime as date) between cast(@startDate as date) and cast(@endDate as date)  
group by se.IDGroup
) as TotalGroup,
(
select COUNT(sp.Type) from Punches sp 
inner join (select * from split(@IDEmployee,',')) tmp on sp.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = sp.IDEmployee and sp.DateTime between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
where (sp.InvalidType IS NULL OR sp.InvalidType = 0) 
and sp.Type = 10
and CAST(sp.DateTime as date) between cast(@startDate as date) and cast(@endDate as date)  
) as TotalSelection ,
d.Name as NameDinningRoom,
e.FullGroupName, e.EmployeeName, e.GroupName, e.Path, e.IDGroup 
from Punches p 
inner join (select * from split(@IDEmployee,',')) tmp on p.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = p.IDEmployee and p.DateTime between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
join sysrovwAllEmployeeGroups e ON p.IDEmployee = e.IDEmployee
left join DiningRoomTurns d ON p.TypeData = d.ID
where (p.InvalidType IS NULL OR p.InvalidType = 0) 
and p.Type = 10
and CAST(p.DateTime as date) between cast(@startDate as date) and cast(@endDate as date) 
group by p.IDEmployee, p.Type, d.Name, e.FullGroupName, e.EmployeeName, e.GroupName, e.Path, e.IDGroup
order by FullGroupName
RETURN NULL 
GO

ALTER PROCEDURE [dbo].[Report_pagos_por_contrato]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@startDate datetime2 = '20210301',
	@endDate datetime2 = '20210301',
	@groupConcepts nvarchar(max) = '0'
AS
SELECT (select name from sysroReportGroups where id = EmployeeConcepts.IDReportGroup) as NombreGrupo, EmployeeConcepts.FullGroupName, EmployeeConcepts.IdGroup, EmployeeConcepts.EmployeeName, EmployeeConcepts.IDEmployee, EmployeeConcepts.Position, EmployeeConcepts.IDConcept, EmployeeConcepts.ConceptName, EmployeeConcepts.ConceptType,
CASE WHEN ISNULL(AccrualsPrice.Price,'') = '' THEN 0 ELSE CONVERT(Numeric(18,3),AccrualsPrice.Price) END AS Price,
MAX(AccrualsPrice.Date) AS MaxDate, SUM(ISNULL(AccrualsPrice.Value,0)) AS Value, EmployeeConcepts.IDReportGroup,
AccrualsPrice.IDContract, AccrualsPrice.BeginDate, AccrualsPrice.EndDate
FROM
(
SELECT sysrovwAllEmployeeGroups.FullGroupName, sysrovwAllEmployeeGroups.IdGroup, sysrovwAllEmployeeGroups.EmployeeName, sysrovwAllEmployeeGroups.IDEmployee, Concepts.ID AS IDConcept, sysroReportGroupConcepts.Position, Concepts.Name AS ConceptName, Concepts.IDType AS ConceptType, sysroReportGroupConcepts.IDReportGroup
FROM Concepts INNER JOIN sysroReportGroupConcepts ON Concepts.ID = sysroReportGroupConcepts.IDConcept CROSS JOIN sysrovwAllEmployeeGroups
WHERE 
(sysroReportGroupConcepts.IDReportGroup IN (select * from split(@groupConcepts, ','))) AND
(sysrovwAllEmployeeGroups.IDEmployee IN (select * from split(@IDEmployee, ',')))
) AS EmployeeConcepts
LEFT OUTER JOIN 
(
SELECT DailyAccruals.IDEmployee, DailyAccruals.IDConcept, DailyAccruals.Date,
CASE WHEN ISNULL(Concepts.FixedPay,0) <> 0 THEN
    convert(nvarchar(100),Concepts.PayValue)
ELSE
    dbo.GetValueFromEmployeeUserFieldValues(DailyAccruals.IDEmployee, SUBSTRING(Concepts.UsedField,5,100), DailyAccruals.Date)
END AS Price,
DailyAccruals.Value, EmployeeContracts.IDContract, EmployeeContracts.BeginDate, EmployeeContracts.EndDate
FROM DailyAccruals INNER JOIN Concepts ON DailyAccruals.IDConcept = Concepts.ID
inner join (select * from split(@IDEmployee,',')) tmp on DailyAccruals.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = DailyAccruals.IDEmployee and DailyAccruals.Date between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
INNER JOIN EmployeeContracts ON 
    DailyAccruals.IDEmployee = EmployeeContracts.IDEmployee AND 
    DailyAccruals.Date >= EmployeeContracts.BeginDate AND 
    DailyAccruals.Date <= EmployeeContracts.EndDate 
WHERE
cast(DailyAccruals.Date as date) BETWEEN cast(@startDate as date) AND cast(@endDate as date)
) AS AccrualsPrice
ON 
EmployeeConcepts.IDEmployee = AccrualsPrice.IDEmployee AND EmployeeConcepts.IDConcept = AccrualsPrice.IDConcept
WHERE ISNULL(AccrualsPrice.Value,0) <> 0
GROUP BY EmployeeConcepts.FullGroupName, EmployeeConcepts.IdGroup, EmployeeConcepts.EmployeeName, EmployeeConcepts.IDEmployee, EmployeeConcepts.Position, EmployeeConcepts.IDConcept, EmployeeConcepts.ConceptName, EmployeeConcepts.ConceptType, Price, EmployeeConcepts.IDReportGroup,
AccrualsPrice.IDContract, AccrualsPrice.BeginDate, AccrualsPrice.EndDate
ORDER BY EmployeeConcepts.FullGroupName, EmployeeConcepts.EmployeeName, EmployeeConcepts.Position
RETURN NULL 
GO


ALTER PROCEDURE [dbo].[Report_pagos_por_contrato_total_grupo]
	@idPassport nvarchar(100) = '1',
	@Employees nvarchar(max) = '0',
	@startDate datetime2 = '20210301',
	@endDate datetime2 = '20210301',
	@IDReportGroup smallint = 0,
	@IDGroup smallint = 0
AS
SELECT EmployeeConcepts.FullGroupName, EmployeeConcepts.Position, EmployeeConcepts.IDConcept, EmployeeConcepts.ConceptName, 
SUM(
CASE WHEN ISNULL(AccrualsPrice.Price,'') = '' THEN 0 ELSE CONVERT(Numeric(18,3),AccrualsPrice.Price) END * ISNULL(AccrualsPrice.Value,0)
) AS ImporteGrupo
FROM
(
SELECT dbo.GetFullGroupPathName(sysrovwAllEmployeeGroups.IDGroup) AS FullGroupName, sysrovwAllEmployeeGroups.EmployeeName, sysrovwAllEmployeeGroups.IDEmployee, Concepts.ID AS IDConcept, sysroReportGroupConcepts.Position, Concepts.Name AS ConceptName
FROM Concepts INNER JOIN sysroReportGroupConcepts ON Concepts.ID = sysroReportGroupConcepts.IDConcept CROSS JOIN sysrovwAllEmployeeGroups
WHERE (sysroReportGroupConcepts.IDReportGroup = @IDReportGroup) AND
sysrovwAllEmployeeGroups.IDEmployee IN (select * from split(@Employees, ',')) AND
(sysrovwAllEmployeeGroups.IDGroup=@IDGroup)
) AS EmployeeConcepts
LEFT OUTER JOIN 
(
SELECT sysrovwAllEmployeeGroups.IDGroup, DailyAccruals.IDEmployee, DailyAccruals.IDConcept, 
CASE WHEN ISNULL(Concepts.FixedPay,0) <> 0 THEN convert(nvarchar(100),Concepts.PayValue)
ELSE dbo.GetValueFromEmployeeUserFieldValues(DailyAccruals.IDEmployee, SUBSTRING(Concepts.UsedField,5,100), DailyAccruals.Date) END AS Price,
DailyAccruals.Value
FROM DailyAccruals INNER JOIN Concepts ON DailyAccruals.IDConcept = Concepts.ID
inner join (select * from split(@Employees,',')) tmp on DailyAccruals.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = DailyAccruals.IDEmployee and DailyAccruals.Date between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
                   INNER JOIN sysrovwAllEmployeeGroups ON DailyAccruals.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee
WHERE
cast(DailyAccruals.Date as date) BETWEEN cast(@StartDate as date) AND cast(@EndDate as date)
) AS AccrualsPrice
ON 
EmployeeConcepts.IDEmployee = AccrualsPrice.IDEmployee AND EmployeeConcepts.IDConcept = AccrualsPrice.IDConcept
WHERE ISNULL(AccrualsPrice.Value,0) <> 0
GROUP BY EmployeeConcepts.FullGroupName, EmployeeConcepts.Position, EmployeeConcepts.IDConcept, EmployeeConcepts.ConceptName
ORDER BY EmployeeConcepts.FullGroupName, EmployeeConcepts.Position
RETURN NULL 
GO


ALTER PROCEDURE [dbo].[Report_pagos_por_contrato_total_report]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@startDate datetime2 = '20210301',
	@endDate datetime2 = '20210301',
	@grupoSaldos nvarchar(max) = '0'
AS
SELECT EmployeeConcepts.Position, EmployeeConcepts.IDConcept, EmployeeConcepts.ConceptName, 
SUM(
CASE WHEN ISNULL(AccrualsPrice.Price,'') = '' THEN 0 ELSE CONVERT(Numeric(18,3),AccrualsPrice.Price) END * ISNULL(AccrualsPrice.Value,0)
) AS ImporteGrupo
FROM
(
SELECT sysrovwAllEmployeeGroups.IDEmployee, Concepts.ID AS IDConcept, sysroReportGroupConcepts.Position, Concepts.Name AS ConceptName
FROM Concepts INNER JOIN sysroReportGroupConcepts ON Concepts.ID = sysroReportGroupConcepts.IDConcept CROSS JOIN sysrovwAllEmployeeGroups
WHERE (sysroReportGroupConcepts.IDReportGroup IN (select * from split(@grupoSaldos, ','))) AND
sysrovwAllEmployeeGroups.IDEmployee IN (select * from split(@IDEmployee, ','))
) AS EmployeeConcepts
LEFT OUTER JOIN 
(
SELECT sysrovwAllEmployeeGroups.IDGroup, DailyAccruals.IDEmployee, DailyAccruals.IDConcept, 
CASE WHEN ISNULL(Concepts.FixedPay,0) <> 0 THEN convert(nvarchar(100),Concepts.PayValue)
ELSE dbo.GetValueFromEmployeeUserFieldValues(DailyAccruals.IDEmployee, SUBSTRING(Concepts.UsedField,5,100), DailyAccruals.Date) END AS Price,
DailyAccruals.Value
FROM DailyAccruals INNER JOIN Concepts ON DailyAccruals.IDConcept = Concepts.ID
inner join (select * from split(@IDEmployee,',')) tmp on DailyAccruals.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = DailyAccruals.IDEmployee and DailyAccruals.Date between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
                   INNER JOIN sysrovwAllEmployeeGroups ON DailyAccruals.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee
WHERE
DailyAccruals.Date BETWEEN @startdate AND @enddate
) AS AccrualsPrice
ON EmployeeConcepts.IDEmployee = AccrualsPrice.IDEmployee AND EmployeeConcepts.IDConcept = AccrualsPrice.IDConcept
WHERE ISNULL(AccrualsPrice.Value,0) <> 0
GROUP BY EmployeeConcepts.Position, EmployeeConcepts.IDConcept, EmployeeConcepts.ConceptName
ORDER BY  EmployeeConcepts.Position
RETURN NULL 
GO

ALTER PROCEDURE [dbo].[Report_porcentaje_absentismo]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@FechaInicio datetime2 = '20210301',
	@FechaFin datetime2 = '20210301'
AS
select 	--expectedworkinghours
	(select isnull(sum(shifts.expectedworkinghours),0) 
	from shifts inner join sysroDailyScheduleByContract on 
		sysroDailyScheduleByContract.idshift1 = shifts.id
		inner join (select * from split(@IDEmployee,',')) tmp on sysroDailyScheduleByContract.IDEmployee = tmp.Value
		inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = sysroDailyScheduleByContract.IDEmployee and sysroDailyScheduleByContract.Date between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
	where sysroDailyScheduleByContract.idemployee = sysrovwallemployeegroups.idemployee
		and cast(date as date) between cast(@FechaInicio as date) and cast(@FechaFin as date) and sysroDailyScheduleByContract.NumContrato=EmployeeContracts.idcontract
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

ALTER PROCEDURE [dbo].[Report_prevision_absentismo]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@StartDate datetime2 = '20210301',
	@EndDate datetime2 = '20210301'
AS
select distinct e.FullGroupName, e.GroupName, e.Path, e.IDGroup,
(SELECT ISNULL(COUNT(*),0) Value 
FROM   (ProgrammedCauses INNER JOIN Causes  
ON ProgrammedCauses.IDCause=Causes.ID) 
INNER JOIN (sysrovwAllEmployeeGroups INNER JOIN Employees 
ON sysrovwAllEmployeeGroups.IDEmployee=Employees.ID) 
ON ProgrammedCauses.IDEmployee=Employees.ID
inner join (select * from splitMAX(@IDEmployee,',')) tmp on ProgrammedCauses.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = ProgrammedCauses.IDEmployee and ProgrammedCauses.Date between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
WHERE sysrovwAllEmployeeGroups.IDGroup = e.IDGroup
AND cast(ProgrammedCauses.Date as date) BETWEEN cast(@StartDate as date) and cast(@EndDate as date)
) as CountIncidences,
(
SELECT ISNULL(COUNT(*),0) Value 
FROM sysrovwAllEmployeeGroups
INNER JOIN ProgrammedAbsences ON sysrovwAllEmployeeGroups.IDEmployee=ProgrammedAbsences.IDEmployee
INNER JOIN Causes ON ProgrammedAbsences.IDCause=Causes.ID
inner join (select * from splitMAX(@IDEmployee,',')) tmp on ProgrammedAbsences.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = ProgrammedAbsences.IDEmployee and cast(ProgrammedAbsences.BeginDate as date) between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
WHERE sysrovwAllEmployeeGroups.IDGroup = e.IDGroup
AND (cast(ProgrammedAbsences.BeginDate as date) BETWEEN cast(@StartDate as date) and cast(@EndDate as date)
 OR cast(ProgrammedAbsences.FinishDate as date) BETWEEN cast(@StartDate as date) and cast(@EndDate as date)
 OR cast(ProgrammedAbsences.BeginDate as date) >= cast(@StartDate as date) 
 AND cast(ProgrammedAbsences.FinishDate as date) <= ISNULL(cast(@EndDate as date),DATEADD(d,MaxLastingDays-1,ProgrammedAbsences.BeginDate))
 OR cast(@StartDate as date) BETWEEN cast(ProgrammedAbsences.BeginDate as date) AND ISNULL(ProgrammedAbsences.FinishDate,DATEADD(d,MaxLastingDays-1,ProgrammedAbsences.BeginDate))
 OR cast(@EndDate as date) BETWEEN cast(ProgrammedAbsences.BeginDate as date) AND ISNULL(ProgrammedAbsences.FinishDate,DATEADD(d,MaxLastingDays-1,ProgrammedAbsences.BeginDate))
 OR cast(ProgrammedAbsences.BeginDate as date) <= cast(@StartDate as date) AND ProgrammedAbsences.FinishDate >= cast(@EndDate as date))
) as CountAbsences
from sysrovwAllEmployeeGroups e 
inner join (select * from splitMAX(@IDEmployee,',')) tmp on e.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = e.IDEmployee and cast(@StartDate as date) between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
RETURN NULL 
GO

ALTER   PROCEDURE [dbo].[Report_prevision_absentismo_subAbsences]
	@idPassport nvarchar(100) = '1',
	@Employees nvarchar(max) = '0',
	@StartDate datetime2 = '20210301',
	@EndDate datetime2 = '20210301',
	@idGroup nvarchar(max) = '0'
AS
select sysrovwAllEmployeeGroups.EmployeeName, CAST(ProgrammedAbsences.BeginDate AS DATE) as Date, CAST(ISNULL(ProgrammedAbsences.FinishDate,ProgrammedAbsences.BeginDate) AS DATE) as xFinish, ProgrammedAbsences.FinishDate, ProgrammedAbsences.MaxLastingDays, Causes.Name as Cause from sysrovwAllEmployeeGroups 
INNER JOIN ProgrammedAbsences ON sysrovwAllEmployeeGroups.IDEmployee=ProgrammedAbsences.IDEmployee
INNER JOIN Causes ON ProgrammedAbsences.IDCause=Causes.ID
inner join (select * from splitMAX(@Employees,',')) tmp on ProgrammedAbsences.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = ProgrammedAbsences.IDEmployee and cast(ProgrammedAbsences.BeginDate as date) between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
WHERE sysrovwAllEmployeeGroups.IDGroup = @idGroup
AND (cast(ProgrammedAbsences.BeginDate as date) BETWEEN cast(@StartDate as date) and cast(@EndDate as date)
 OR cast(ProgrammedAbsences.FinishDate as date) BETWEEN cast(@StartDate as date) and cast(@EndDate as date)
 OR cast(ProgrammedAbsences.BeginDate as date) >= cast(@StartDate as date) 
 AND cast(ProgrammedAbsences.FinishDate as date) <= ISNULL(cast(@EndDate as date),DATEADD(d,MaxLastingDays-1,ProgrammedAbsences.BeginDate))
 OR cast(@StartDate as date) BETWEEN cast(ProgrammedAbsences.BeginDate as date) AND ISNULL(ProgrammedAbsences.FinishDate,DATEADD(d,MaxLastingDays-1,ProgrammedAbsences.BeginDate))
 OR cast(@EndDate as date) BETWEEN cast(ProgrammedAbsences.BeginDate as date) AND ISNULL(ProgrammedAbsences.FinishDate,DATEADD(d,MaxLastingDays-1,ProgrammedAbsences.BeginDate))
 OR cast(ProgrammedAbsences.BeginDate as date) <= cast(@StartDate as date) AND ProgrammedAbsences.FinishDate >= cast(@EndDate as date))
RETURN NULL 
GO


ALTER PROCEDURE [dbo].[Report_prevision_absentismo_subIncidences]
	@idPassport nvarchar(100) = '1',
	@Employees nvarchar(max) = '0',
	@StartDate datetime2 = '20210301',
	@EndDate datetime2 = '20210301',
	@idGroup nvarchar(max) = '0'
AS
select e.EmployeeName, CAST(pc.Date AS DATE) as Date, CAST(ISNULL(pc.FinishDate,pc.Date) AS DATE) as xFinish, pc.Duration, c.Name as Cause, pc.IDCause from sysrovwAllEmployeeGroups e join ProgrammedCauses pc ON e.IDEmployee = pc.IDEmployee 
and ((
cast(pc.Date as date) >= cast(@StartDate as date) and cast(pc.Date as date) <= cast(@EndDate as date)
) or (
CAST(ISNULL(pc.FinishDate,pc.Date) AS DATE) >= cast(@StartDate as date) and CAST(ISNULL(pc.FinishDate,pc.Date) AS DATE) <= cast(@EndDate as date)
) or (
cast(pc.Date as date) < cast(@StartDate as date) AND CAST(ISNULL(pc.FinishDate,pc.Date) AS DATE) > cast(@EndDate as date)
)) JOIN Causes c ON pc.IDCause = c.ID
inner join (select * from splitMAX(@Employees,',')) tmp on pc.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = pc.IDEmployee and pc.Date between poe.BeginDate and poe.EndDate and poe.IdPassport = @idPassport
WHERE e.IDGroup = @idGroup
RETURN NULL 
GO


ALTER PROCEDURE [dbo].[Report_puestos_horarios_empleado]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@startDate datetime2 = '20210301',
	@endDate datetime2 = '20210301'
AS
select ds.Date, e.FullGroupName, e.Name, e.GroupName, e.ShiftName, sh.Description, e.ExpectedWorkingHours, a.Name as Puesto
from DailySchedule ds
inner join (select * from splitMAX(@IDEmployee,',')) tmp on ds.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport =  @idPassport And poe.IDEmployee = ds.IDEmployee And cast(ds.Date as date) between poe.BeginDate And poe.EndDate 
INNER JOIN sysroEmployeesShifts e ON ds.IDEmployee = e.IDEmployee AND ds.Date = e.CurrentDate 
INNER JOIN shifts sh ON e.IDShift = sh.ID 
LEFT JOIN Assignments a ON ISNULL(ds.IDAssignment, ds.IDAssignmentBase) = a.ID
where e.IDEmployee in (select * from split(@IDEmployee, ',')) AND 
cast(ds.Date as date) BETWEEN cast(@startDate as date) AND cast(@endDate as date) AND e.ShiftName is not NULL 
RETURN NULL 
GO


ALTER PROCEDURE [dbo].[Report_saldos_a_fecha]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@Fecha datetime2 = '20210301'
AS
SELECT sysroEmployeeGroups.FullGroupName, sysroEmployeeGroups.GroupName, Employees.Name , sysroEmployeeGroups.IDEmployee, Employees.ID, EmployeeContracts.BeginDate, EmployeeContracts.EndDate, EmployeeContracts.IDContract, 
(SELECT TOP 1 Value FROM EmployeeUserFieldValues  WHERE EmployeeUserFieldValues.IDEmployee = Employees.ID and FieldName = 'NIF' 
ORDER BY EmployeeUserFieldValues.Date DESC) AS Dni
 FROM   EmployeeContracts with (nolock) 
 inner join (select * from splitMAX(@IDEmployee,',')) tmp on EmployeeContracts.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport =  @idPassport And poe.IDEmployee = EmployeeContracts.IDEmployee And cast(isnull(@Fecha, getdate()) as date) between poe.BeginDate And poe.EndDate 
 INNER JOIN Employees with (nolock) ON EmployeeContracts.IDEmployee=Employees.ID INNER JOIN sysroEmployeeGroups with (nolock) ON Employees.ID=sysroEmployeeGroups.IDEmployee INNER JOIN Groups with (nolock) ON sysroEmployeeGroups.IDGroup=Groups.ID
 WHERE sysroEmployeeGroups.EndDate>= isnull(@Fecha, getdate()) AND sysroEmployeeGroups.BeginDate< isnull(@Fecha, getdate()) AND EmployeeContracts.BeginDate< isnull(@Fecha, getdate()) AND EmployeeContracts.EndDate>= isnull(@Fecha, getdate())
 ORDER BY sysroEmployeeGroups.FullGroupName, sysroEmployeeGroups.GroupName, Employees.Name, EmployeeContracts.BeginDate
RETURN NULL 
GO


ALTER PROCEDURE [dbo].[Report_seguimiento_irregularidades]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@fechaStart datetime2 = '20210301',
	@fechaEnd datetime2 = '20210301'
AS
select es.*, efv.Value as NIF from sysroEmployeesShifts es 
inner join (select * from splitMAX(@IDEmployee,',')) tmp on es.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport =  @idPassport And poe.IDEmployee = es.IDEmployee And cast(es.CurrentDate as date) between poe.BeginDate And poe.EndDate 
LEFT JOIN EmployeeUserFieldValues efv ON es.IDEmployee = efv.IDEmployee AND efv.FieldName LIKE 'NIF' 
where cast(es.CurrentDate as date) between cast(@fechaStart as date) and cast(@fechaEnd as date) 
and es.IDShift is not null
RETURN NULL 
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='818' WHERE ID='DBVersion'
GO
