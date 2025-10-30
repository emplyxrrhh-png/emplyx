-- No borréis esta línea
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
select Date as Fecha, d.IDEmployee, datepart(ww, Date) as Week, datepart(dw, Date) as DayWeek, datepart(dd, Date) as Day, datepart(mm, Date) as Month, shifts.Name as NameShift, ISNULL(d.ShiftColor1,ISNULL(shifts.Color,16777215)) as Color, CASE WHEN shifts.ShiftType = 2 THEN 0 ELSE ISNULL(d.ExpectedWorkingHours, shifts.ExpectedWorkingHours) END AS Hours, pa.AbsenceID as Absence, r.Status as Pending
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

ALTER PROCEDURE [dbo].[Report_calendario_anual_usuarios_list]
@idPassport nvarchar(max) = '1024',
@monthStart nvarchar(2) = '01',
@yearStart nvarchar(4) = '2021',
@monthEnd nvarchar(2) = '04',
@yearEnd nvarchar(4) = '2021',
@employees nvarchar(max) = '1,2'
AS
SET @monthStart = FORMAT(CAST(@monthStart as int), '00');
SET @yearStart = FORMAT(CAST(@yearStart as int), '0000');
SET @monthEnd = FORMAT(CAST(@monthEnd as int), '00');
SET @yearEnd = FORMAT(CAST(@yearEnd as int), '0000');
DECLARE @StartDate date = DATEFROMPARTS(@yearStart,@monthStart,'01');
DECLARE @EndDate date = EOMONTH(DATEFROMPARTS(@yearEnd,@monthEnd,'01'));
WITH Dates AS(
SELECT @StartDate as Fecha, NULL as IDEmployee,
0 as CountDays,
datepart(mm, @StartDate) as Month,
datepart(YYYY, @StartDate) as Year,
datepart(dd, EOMONTH(@StartDate)) as LastDayMonth,
0 as Hours,
NULL As EmployeeName, NULL as Contract,
NULL as BeginContract, NULL as EndContract,
NULL as IDGroup, NULL as GroupName,NULL as Ruta
UNION ALL
SELECT DATEADD(DAY, 1,Fecha), NULL as IDEmployee,
0 as CountDays,
datepart(mm, DATEADD(DAY, 1,Fecha)) as Month,
datepart(YYYY, DATEADD(DAY, 1,Fecha)) as Year,
datepart(dd, EOMONTH(DATEADD(DAY, 1,Fecha))) as LastDayMonth,
0 as Hours,
NULL As EmployeeName, NULL as Contract,
NULL as BeginContract, NULL as EndContract,
NULL as IDGroup, NULL as GroupName,NULL as Ruta
FROM Dates
WHERE DATEADD(DAY, 1,Fecha) <= @EndDate)
select d.IDEmployee,
(select count(IDEmployee) from DailySchedule where IDEmployee = d.IDEmployee and (datepart(YYYY, d.Date) LIKE datepart(YYYY, Date) AND datepart(mm, d.Date) LIKE datepart(mm, Date)) AND ISNULL(IDShiftUsed,IDShift1) IS NOT NULL) as CountDays,
datepart(mm, Date) as Month,
datepart(YYYY, Date) as Year,
datepart(dd, EOMONTH(Date)) as LastDayMonth,
SUM(CASE WHEN shifts.ShiftType = 2 THEN 0 ELSE ISNULL(d.ExpectedWorkingHours,shifts.ExpectedWorkingHours) end) AS Hours,
emp.Name As EmployeeName, ec.IDContract as Contract,
ec.BeginDate as BeginContract, ec.EndDate as EndContract,
g.Id as IDGroup, g.Name as GroupName, g.FullGroupName as Ruta
from Dates
left join 
(select d.* from DailySchedule d 
INNER JOIN (select * from split(@employees,',')) tmp on d.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = d.IDEmployee and d.Date between poe.BeginDate and poe.EndDate and poe.IdFeature = 2 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
)
as d on Dates.Fecha = d.Date
left join Shifts on (d.IDShiftUsed is not null and d.IDShiftUsed = Shifts.ID) or (d.IDShift1 = Shifts.ID)
left join Employees emp on emp.ID = d.IDEmployee
left join EmployeeContracts ec on ec.IDEmployee = emp.ID
left join EmployeeGroups eg on eg.IDEmployee = emp.ID and d.Date between eg.BeginDate and eg.EndDate
left join Groups g on eg.IDGroup = g.ID
where Date between CONCAT(@yearStart,@monthStart,'01') and EOMONTH(convert(smalldatetime,CONCAT(@yearEnd,@monthEnd,'01'),120))
AND ec.IDContract = (select top 1 sec.IDContract from employeeContracts sec where Dates.Fecha between sec.BeginDate and sec.EndDate  and IDEmployee = emp.ID)
AND g.ID = (select top 1 seg.IDGroup from employeeGroups seg where Dates.Fecha between seg.BeginDate and seg.EndDate and IDEmployee = emp.ID)
group by d.IDEmployee, datepart(mm, Date), datepart(YYYY, Date), datepart(dd, EOMONTH(Date)), emp.Name, ec.IDContract,
ec.BeginDate, ec.EndDate,
g.Id, g.Name, g.FullGroupName
order by d.IDEmployee, Month, Year
option (maxrecursion 0)
RETURN NULL
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
select ISNULL(ds.ShiftName1,s.Name) AS Name, s.ShortName, ISNULL(ds.ShiftColor1,s.Color) AS Color, COUNT(ISNULL(ds.IDShiftUsed,ds.IDShift1)) as Days, SUM(CASE WHEN s.ShiftType = 2 THEN 0 ELSE ISNULL(ds.ExpectedWorkingHours,s.ExpectedWorkingHours) end) as ExpectedWorkingHours from DailySchedule ds
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


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='821' WHERE ID='DBVersion'
GO
