ALTER PROCEDURE [dbo].[Report_calendario_anual_usuarios]
@month nvarchar(2) = '06',
@year nvarchar(4) = '2021',
@employee nvarchar(max) = '16',
@IDPassport nvarchar(100) = '0'
AS
SET @month = FORMAT(CAST(@month as int), '00');
SET @year = FORMAT(CAST(@year as int), '0000');
IF @year < 1990
BEGIN
SET @year = 1990;
END
IF @month < 1
BEGIN
SET @month = 1;
END
DECLARE @StartDate date = DATEFROMPARTS(@year,@month,1);
DECLARE @EndDate date = EOMONTH(@StartDate);
SET DATEFIRST 1;
WITH Dates AS(
SELECT @StartDate as Fecha, @employee as IDEmployee, datepart(ww, @StartDate) as Week, datepart(dw, @StartDate) as DayWeek, datepart(dd, @StartDate) as Day, datepart(mm, @StartDate) as Month, NULL as NameShift, null as Color, NULL as Hours, NULL as Absence, NULL as Pending
UNION ALL
SELECT DATEADD(DAY, 1,Fecha), @employee as IDEmployee, datepart(ww, DATEADD(DAY, 1,Fecha)) as Week, datepart(dw, DATEADD(DAY, 1,Fecha)) as DayWeek, datepart(dd, DATEADD(DAY, 1,Fecha)) as Day, datepart(mm, DATEADD(DAY, 1,Fecha)) as Month,NULL as NameShift, null as Color, NULL as Hours, NULL as Absence, NULL as Pending
FROM Dates
WHERE DATEADD(DAY, 1,Fecha) <= @EndDate)
select * from (
select Date as Fecha, d.IDEmployee, datepart(ww, Date) as Week, datepart(dw, Date) as DayWeek, datepart(dd, Date) as Day, datepart(mm, Date) as Month, shifts.Name as NameShift, ISNULL(d.ShiftColor1,ISNULL(shifts.Color,16777215)) as Color, ISNULL(d.ExpectedWorkingHours, shifts.ExpectedWorkingHours) as Hours, pa.AbsenceID as Absence, r.Status as Pending
from DailySchedule as d
inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) 
On poe.IDPassport = @IDPassport 
And poe.IDEmployee = d.IDEmployee
And d.Date between poe.BeginDate And poe.EndDate 
join EmployeeContracts ec on ec.IDEmployee = d.IDEmployee
left join Shifts on (d.IDShift1 = Shifts.ID)
left join ProgrammedAbsences as pa
on d.IDEmployee = pa.IDEmployee
and d.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
left join Requests as r on d.IDEmployee = r.IDEmployee
and r.Status IN (0,1) and r.RequestType = 6 and r.ID IN (select IDRequest from sysroRequestDays where Date = d.Date)
where d.IDEmployee = @employee and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year
UNION ALL
select dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.Month, CAST(dat.NameShift as nvarchar(max)) as NameShift, 16777215 as Color, dat.Hours, dat.Absence, dat.Pending
from Dates dat
inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) 
On poe.IDPassport = @IDPassport 
And poe.IDEmployee = dat.IDEmployee
And dat.Fecha between poe.BeginDate And poe.EndDate 
join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and datepart(MM, dsc.Date) = @month and datepart(YYYY, dsc.Date) = @year)
and dat.fecha between (select top 1 begindate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc) and (select top 1 EndDate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc)
UNION ALL
select dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.Month, CAST(dat.NameShift as nvarchar(max)) as NameShift, null as Color, dat.Hours, dat.Absence, dat.Pending
from Dates dat
inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) 
On poe.IDPassport = @IDPassport 
And poe.IDEmployee = dat.IDEmployee
And dat.Fecha between poe.BeginDate And poe.EndDate 
join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and datepart(MM, dsc.Date) = @month and datepart(YYYY, dsc.Date) = @year)
and dat.fecha not between (select top 1 begindate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc) and (select top 1 EndDate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc)
) t
group by fecha, t.IDEmployee, Week, DayWeek, Day, Month, NameShift, Color, Hours, Absence, Pending
order by IDEmployee, Fecha, Month, Day, DayWeek
return null
GO


ALTER PROCEDURE [dbo].[Report_calendario_anual_usuarios_resumen]
@monthStart nvarchar(2) = '1',
@yearStart nvarchar(4) = '2021',
@monthEnd nvarchar(2) = '04',
@yearEnd nvarchar(4) = '2021',
@IDEmployee nvarchar(max) = '777',
@IDGroup nvarchar(9) = '364',
@IDPassport nvarchar(100) = '0'
AS
SET @monthStart = FORMAT(CAST(@monthStart as int), '00');
SET @yearStart = FORMAT(CAST(@yearStart as int), '0000');
SET @monthEnd = FORMAT(CAST(@monthEnd as int), '00');
SET @yearEnd = FORMAT(CAST(@yearEnd as int), '0000');
DECLARE @fechaStart date = DATEFROMPARTS(@yearStart,@monthStart,'01');
declare @fechaEnd datetime = EOMONTH(DATEFROMPARTS(@yearEnd,@monthEnd,'01'));
select ISNULL(ds.ShiftName1,s.Name) AS Name, s.ShortName, ISNULL(ds.ShiftColor1,s.Color) AS Color, COUNT(ISNULL(ds.IDShiftUsed,ds.IDShift1)) as Days, SUM(ISNULL(ds.ExpectedWorkingHours,s.ExpectedWorkingHours)) as ExpectedWorkingHours from DailySchedule ds
inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) 
On poe.IDPassport = @IDPassport 
And poe.IDEmployee = ds.IDEmployee 
And ds.Date between poe.BeginDate And poe.EndDate 
inner join shifts s on (IDShiftUsed is not null and IDShiftUsed = s.ID) or (IDShift1 = s.ID)
inner join EmployeeGroups as eg on ds.IDEmployee = eg.IDEmployee and ds.Date between eg.BeginDate and eg.EndDate
and ds.Date between eg.BeginDate and eg.EndDate
left join ProgrammedAbsences as pa
on ds.IDEmployee = pa.IDEmployee
and ds.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
where ds.IDEmployee IN (select * from split(@IDEmployee,','))
and Date between @fechaStart and EOMONTH(@fechaEnd)
and eg.IDGroup IN (select * from split(@IDGroup,','))
GROUP BY ISNULL(ds.ShiftName1,s.Name), s.ShortName, ISNULL(ds.ShiftColor1,s.Color);
RETURN NULL
GO


DROP PROCEDURE IF EXISTS [dbo].[Report_listado_total_comedor]
GO
CREATE PROCEDURE [dbo].[Report_listado_total_comedor]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@startDate smalldatetime = '20210301',
	@endDate smalldatetime = '20210301'
AS
select  p.IDEmployee, p.Type, COUNT(p.Type) as Total,
(
select COUNT(sp.Type) from Punches sp 
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = sp.IDEmployee and sp.DateTime between poe.BeginDate and poe.EndDate and poe.IdFeature = 24200 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
where sp.IDEmployee = p.IDEmployee 
and (sp.InvalidType IS NULL OR sp.InvalidType = 0) 
and sp.Type = 10
and CAST(sp.DateTime as date) between cast(@startDate as date) and cast(@endDate as date) 
group by sp.IDEmployee
) as TotalEmp, 
(
select COUNT(sp.Type) from Punches sp 
inner join (select * from split(@IDEmployee,',')) tmp on p.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = sp.IDEmployee and sp.DateTime between poe.BeginDate and poe.EndDate and poe.IdFeature = 24200 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
join sysrovwAllEmployeeGroups se ON sp.IDEmployee = se.IDEmployee
where se.IDGroup = e.IDGroup 
and (sp.InvalidType IS NULL OR sp.InvalidType = 0) 
and sp.Type = 10
and CAST(sp.DateTime as date) between cast(@startDate as date) and cast(@endDate as date)  
group by se.IDGroup
) as TotalGroup,
(
select COUNT(sp.Type) from Punches sp 
inner join (select * from split(@IDEmployee,',')) tmp on p.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = sp.IDEmployee and sp.DateTime between poe.BeginDate and poe.EndDate and poe.IdFeature = 24200 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
where (sp.InvalidType IS NULL OR sp.InvalidType = 0) 
and sp.Type = 10
and CAST(sp.DateTime as date) between cast(@startDate as date) and cast(@endDate as date)  
) as TotalSelection ,
d.Name as NameDinningRoom,
e.FullGroupName, e.EmployeeName, e.GroupName, e.Path, e.IDGroup 
from Punches p 
inner join (select * from split(@IDEmployee,',')) tmp on p.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = p.IDEmployee and p.DateTime between poe.BeginDate and poe.EndDate and poe.IdFeature = 24200 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
join sysrovwAllEmployeeGroups e ON p.IDEmployee = e.IDEmployee
left join DiningRoomTurns d ON p.TypeData = d.ID
where (p.InvalidType IS NULL OR p.InvalidType = 0) 
and p.Type = 10
and CAST(p.DateTime as date) between cast(@startDate as date) and cast(@endDate as date) 
group by p.IDEmployee, p.Type, d.Name, e.FullGroupName, e.EmployeeName, e.GroupName, e.Path, e.IDGroup
order by FullGroupName
RETURN NULL 
GO


DROP PROCEDURE IF EXISTS [dbo].[Report_pagos_por_contrato]
GO
CREATE PROCEDURE [dbo].[Report_pagos_por_contrato]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@startDate smalldatetime = '20210301',
	@endDate smalldatetime = '20210301',
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
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = DailyAccruals.IDEmployee and DailyAccruals.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 5 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
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


DROP PROCEDURE IF EXISTS [dbo].[Report_pagos_por_contrato]
GO
CREATE PROCEDURE [dbo].[Report_pagos_por_contrato]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@startDate smalldatetime = '20210301',
	@endDate smalldatetime = '20210301',
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
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = DailyAccruals.IDEmployee and DailyAccruals.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 1626 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
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



DROP PROCEDURE IF EXISTS [dbo].[Report_pagos_por_contrato_total_grupo]
GO
CREATE PROCEDURE [dbo].[Report_pagos_por_contrato_total_grupo]
	@idPassport nvarchar(100) = '1',
	@Employees nvarchar(max) = '0',
	@startDate smalldatetime = '20210301',
	@endDate smalldatetime = '20210301',
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
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = DailyAccruals.IDEmployee and DailyAccruals.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 1626 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
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



DROP PROCEDURE IF EXISTS [dbo].[Report_pagos_por_contrato_total_report]
GO
CREATE PROCEDURE [dbo].[Report_pagos_por_contrato_total_report]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@startDate smalldatetime = '20210301',
	@endDate smalldatetime = '20210301',
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
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = DailyAccruals.IDEmployee and DailyAccruals.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 1626 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
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


DROP PROCEDURE IF EXISTS [dbo].[Report_lista_de_tareas]
GO
CREATE PROCEDURE [dbo].[Report_lista_de_tareas]
	@idPassport nvarchar(100) = '1',
	@IDTask nvarchar(max) = '0'
AS
select isnull(Project,'') as Project,  Name, Description, barcode from Tasks 
inner join sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) 
on pof.IDPassport = @idPassport And pof.IdFeature = 25 AND pof.Permission > 1 
where id > 0 and id in (select * from split(@IDTask,','))
RETURN NULL 
GO


DROP PROCEDURE IF EXISTS [dbo].[Report_porcentaje_absentismo]
GO
CREATE PROCEDURE [dbo].[Report_porcentaje_absentismo]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@FechaInicio smalldatetime = '20210301',
	@FechaFin smalldatetime = '20210301'
AS
select 	--expectedworkinghours
	(select isnull(sum(shifts.expectedworkinghours),0) 
	from shifts inner join sysroDailyScheduleByContract on 
		sysroDailyScheduleByContract.idshift1 = shifts.id
		inner join (select * from split(@IDEmployee,',')) tmp on sysroDailyScheduleByContract.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = sysroDailyScheduleByContract.IDEmployee and sysroDailyScheduleByContract.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 1626 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
	where sysroDailyScheduleByContract.idemployee = sysrovwallemployeegroups.idemployee
		and cast(date as date) between cast(@FechaInicio as date) and cast(@FechaFin as date) and sysroDailyScheduleByContract.NumContrato=EmployeeContracts.idcontract
	) as expectedworkinghours,
	--abstotal
	(select isnull(sum(sysroDailyAccrualsByContract.value),0)
    	from sysroDailyAccrualsByContract INNER JOIN Concepts
		ON sysroDailyAccrualsByContract.IDConcept=Concepts.ID
		inner join (select * from split(@IDEmployee,',')) tmp on sysroDailyAccrualsByContract.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = sysroDailyAccrualsByContract.IDEmployee and sysroDailyAccrualsByContract.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 1626 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
    	where sysroDailyAccrualsByContract.IDEmployee = sysrovwallemployeegroups.idemployee
		and cast(date as date) between cast(@FechaInicio as date) and cast(@FechaFin as date) and sysroDailyAccrualsByContract.NumContrato=EmployeeContracts.idcontract
		and IsAbsentiism = 1
	) as abstotal,
	--abstotalrewarded
	(select isnull(sum(sysroDailyAccrualsByContract.value),0) 
	from sysroDailyAccrualsByContract INNER JOIN Concepts
		ON sysroDailyAccrualsByContract.IDConcept=Concepts.ID
		inner join (select * from split(@IDEmployee,',')) tmp on sysroDailyAccrualsByContract.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = sysroDailyAccrualsByContract.IDEmployee and sysroDailyAccrualsByContract.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 1626 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
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
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = sysroDailyAccrualsByContract.IDEmployee and sysroDailyAccrualsByContract.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 1626 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
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
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = sysroDailyAccrualsByContract.IDEmployee and sysroDailyAccrualsByContract.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 1626 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
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
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = sysroDailyAccrualsByContract.IDEmployee and sysroDailyAccrualsByContract.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 1626 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
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
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = EmployeeContracts.IDEmployee and @FechaInicio between poe.BeginDate and poe.EndDate and poe.IdFeature = 1626 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
where sysrovwallemployeegroups.IDEmployee = Employees.ID and sysrovwallemployeegroups.IDEmployee = EmployeeContracts.idEmployee 
and EmployeeContracts.begindate<=@FechaFin and EmployeeContracts.EndDate>=@FechaInicio
order by FullGroupName
RETURN NULL 
GO


DROP PROCEDURE IF EXISTS [dbo].[Report_prevision_absentismo]
GO
CREATE PROCEDURE [dbo].[Report_prevision_absentismo]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@StartDate smalldatetime = '20210301',
	@EndDate smalldatetime = '20210301'
AS
select distinct e.FullGroupName, e.GroupName, e.Path, e.IDGroup,
(SELECT ISNULL(COUNT(*),0) Value 
FROM   (ProgrammedCauses INNER JOIN Causes  
ON ProgrammedCauses.IDCause=Causes.ID) 
INNER JOIN (sysrovwAllEmployeeGroups INNER JOIN Employees 
ON sysrovwAllEmployeeGroups.IDEmployee=Employees.ID) 
ON ProgrammedCauses.IDEmployee=Employees.ID
inner join (select * from splitMAX(@IDEmployee,',')) tmp on ProgrammedCauses.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = ProgrammedCauses.IDEmployee and ProgrammedCauses.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 1626 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
WHERE sysrovwAllEmployeeGroups.IDGroup = e.IDGroup
AND cast(ProgrammedCauses.Date as date) BETWEEN cast(@StartDate as date) and cast(@EndDate as date)
) as CountIncidences,
(
SELECT ISNULL(COUNT(*),0) Value 
FROM sysrovwAllEmployeeGroups
INNER JOIN ProgrammedAbsences ON sysrovwAllEmployeeGroups.IDEmployee=ProgrammedAbsences.IDEmployee
INNER JOIN Causes ON ProgrammedAbsences.IDCause=Causes.ID
inner join (select * from splitMAX(@IDEmployee,',')) tmp on ProgrammedAbsences.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = ProgrammedAbsences.IDEmployee and cast(ProgrammedAbsences.BeginDate as date) between poe.BeginDate and poe.EndDate and poe.IdFeature = 1626 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
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
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = e.IDEmployee and cast(ProgrammedAbsences.BeginDate as date) between poe.BeginDate and poe.EndDate and poe.IdFeature = 1626 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
RETURN NULL 
GO


DROP PROCEDURE IF EXISTS [dbo].[Report_prevision_absentismo_subIncidences]
GO
CREATE PROCEDURE [dbo].[Report_prevision_absentismo_subIncidences]
	@idPassport nvarchar(100) = '1',
	@Employees nvarchar(max) = '0',
	@StartDate smalldatetime = '20210301',
	@EndDate smalldatetime = '20210301',
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
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = pc.IDEmployee and pc.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 1626 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
WHERE e.IDGroup = @idGroup
RETURN NULL 
GO



DROP PROCEDURE IF EXISTS [dbo].[Report_prevision_absentismo_subAbsences]
GO
CREATE PROCEDURE [dbo].[Report_prevision_absentismo_subAbsences]
	@idPassport nvarchar(100) = '1',
	@Employees nvarchar(max) = '0',
	@StartDate smalldatetime = '20210301',
	@EndDate smalldatetime = '20210301',
	@idGroup nvarchar(max) = '0'
AS
select e.EmployeeName, CAST(pa.BeginDate AS DATE) as Date, CAST(ISNULL(pa.FinishDate,pa.BeginDate) AS DATE) as xFinish, pa.FinishDate, pa.MaxLastingDays, c.Name as Cause from sysrovwAllEmployeeGroups e 
join ProgrammedAbsences pa ON e.IDEmployee = pa.IDEmployee 
and ((
cast(pa.BeginDate as date) >= cast(@StartDate as date) and cast(pa.BeginDate as date) <= cast(@EndDate as date)
) or (
CAST(ISNULL(pa.FinishDate,pa.BeginDate) AS DATE) >= cast(@StartDate as date) and CAST(ISNULL(pa.FinishDate,pa.BeginDate) AS DATE) <= cast(@EndDate as date)
) or (
cast(pa.BeginDate as date) < cast(@StartDate as date) AND CAST(ISNULL(pa.FinishDate,pa.BeginDate) AS DATE) > cast(@EndDate as date)
)) JOIN Causes c ON pa.IDCause = c.ID
inner join (select * from splitMAX(@Employees,',')) tmp on pa.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = pa.IDEmployee and cast(pa.BeginDate as date) between poe.BeginDate and poe.EndDate 
and poe.IdFeature = 1626 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
WHERE e.IDGroup = @idGroup 
RETURN NULL 
GO


-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='789' WHERE ID='DBVersion'
GO
