UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

DROP PROCEDURE [dbo].[Report_calendario_anual_usuarios]
GO
CREATE PROCEDURE [dbo].[Report_calendario_anual_usuarios]
@month nvarchar(2) = '06',
@year nvarchar(4) = '2021',
@employee nvarchar(max) = '16'
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
WITH Dates AS(
SELECT @StartDate as Fecha, @employee as IDEmployee, datepart(ww, @StartDate) as Week, datepart(dw, @StartDate) as DayWeek, datepart(dd, @StartDate) as Day, datepart(mm, @StartDate) as Month, NULL as NameShift, 16777215 as Color, NULL as Hours, NULL as Absence, NULL as Pending
UNION ALL
SELECT DATEADD(DAY, 1,Fecha), @employee as IDEmployee, datepart(ww, DATEADD(DAY, 1,Fecha)) as Week, datepart(dw, DATEADD(DAY, 1,Fecha)) as DayWeek, datepart(dd, DATEADD(DAY, 1,Fecha)) as Day, datepart(mm, DATEADD(DAY, 1,Fecha)) as Month,NULL as NameShift, 16777215 as Color, NULL as Hours, NULL as Absence, NULL as Pending
FROM Dates
WHERE DATEADD(DAY, 1,Fecha) <= @EndDate)
select Date as Fecha, d.IDEmployee, datepart(ww, Date) as Week, datepart(dw, Date) as DayWeek, datepart(dd, Date) as Day, datepart(mm, Date) as Month, shifts.Name as NameShift, ISNULL(shifts.Color,16777215) as Color, shifts.ExpectedWorkingHours as Hours, pa.AbsenceID as Absence, r.Status as Pending
from DailySchedule as d
left join Shifts on (d.IDShift1 = Shifts.ID)
left join ProgrammedAbsences as pa
on d.IDEmployee = pa.IDEmployee
and d.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
left join Requests as r on d.IDEmployee = r.IDEmployee
and r.Status = 0 and r.RequestType = 6 and r.ID IN (select IDRequest from sysroRequestDays where Date = d.Date)
where d.IDEmployee = @employee and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year
UNION ALL
select dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.Month, CAST(dat.NameShift as nvarchar(max)) as NameShift, dat.Color, dat.Hours, dat.Absence, dat.Pending
from Dates dat
join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and datepart(MM, dsc.Date) = @month and datepart(YYYY, dsc.Date) = @year)
AND (ec.BeginDate <= dat.Fecha and ec.EndDate >= dat.Fecha)
UNION ALL
select dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.Month, CAST(dat.NameShift as nvarchar(max)) as NameShift, NULL as Color, dat.Hours, dat.Absence, dat.Pending
from Dates dat
join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and datepart(MM, dsc.Date) = @month and datepart(YYYY, dsc.Date) = @year)
AND ec.EndDate < dat.Fecha
group by fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.Month, dat.NameShift, dat.Hours, dat.Absence, dat.Pending
order by IDEmployee, Fecha, Month, Day, DayWeek
return null

EXECUTE [dbo].[Report_calendario_anual_usuarios]
GO


DROP PROCEDURE [dbo].[Report_calendario_mensual_usuarios]
GO
CREATE PROCEDURE [dbo].[Report_calendario_mensual_usuarios]
@month nvarchar(2) = '04',
@year nvarchar(4) = '2021',
@employee nvarchar(max) = '2'
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
DECLARE @StartDate date = DATEFROMPARTS(@year,@month,'01');
DECLARE @EndDate date = EOMONTH(@StartDate);

WITH Dates AS(
SELECT @StartDate as Fecha, @employee as IDEmployee, datepart(ww, @StartDate) as Week, datepart(dw, @StartDate) as DayWeek, datepart(dd, @StartDate) as Day, NULL as NameShift, 16777215 as Color, NULL as Hours, NULL as Absence, NULL as Pending
UNION ALL
SELECT DATEADD(DAY, 1,Fecha), @employee as IDEmployee, datepart(ww, DATEADD(DAY, 1,Fecha)) as Week, datepart(dw, DATEADD(DAY, 1,Fecha)) as DayWeek, datepart(dd, DATEADD(DAY, 1,Fecha)) as Day, NULL as NameShift, 16777215 as Color, NULL as Hours, NULL as Absence, NULL as Pending
FROM Dates
WHERE DATEADD(DAY, 1,Fecha) <= @EndDate
)
Select NULL as Fecha, @employee as IDEmployee, NULL as Week, DayWeek, NULL as Day, NULL as NameShift, NULL as Color, NULL as Hours, NULL as Absence, NULL as Pending from (
SELECT top 7
ROW_NUMBER() OVER(ORDER BY Date ASC) AS DayWeek,
date, @employee as idemployee
FROM DailySchedule ) x where DayWeek < (select TOP(1) datepart(dw, Date) as DayWeek from DailySchedule where DailySchedule.IDEmployee = @employee and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year order by Date)
UNION
select Date as Fecha, d.IDEmployee, datepart(ww, Date) as Week, datepart(dw, Date) as DayWeek, datepart(dd, Date) as Day, CAST(shifts.Name as nvarchar(max)) as NameShift, shifts.Color as Color, shifts.ExpectedWorkingHours as Hours, pa.AbsenceID as Absence, r.Status as Pending
from DailySchedule as d
inner join Shifts on (d.IDShift1 = Shifts.ID)
left join ProgrammedAbsences as pa
on d.IDEmployee = pa.IDEmployee
and d.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
left join Requests as r on d.IDEmployee = r.IDEmployee
and r.Status = 0 and r.RequestType = 6 and r.ID IN (select IDRequest from sysroRequestDays where Date = d.Date)
where d.IDEmployee = @employee and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year
UNION
select dat.Fecha, dat.IDEmployee, dat.Week, Dat.DayWeek, dat.Day, CAST(dat.NameShift as nvarchar(max)) as NameShift, dat.Color, dat.Hours, dat.Absence, dat.Pending
from Dates dat
join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and datepart(MM, dsc.Date) = @month and datepart(YYYY, dsc.Date) = @year)
AND (ec.BeginDate <= dat.Fecha and ec.EndDate >= dat.Fecha)
UNION
select dat.Fecha, dat.IDEmployee, dat.Week, Dat.DayWeek, dat.Day, CAST(dat.NameShift as nvarchar(max)) as NameShift, NULL as Color, dat.Hours, dat.Absence, dat.Pending
from Dates dat
join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and datepart(MM, dsc.Date) = @month and datepart(YYYY, dsc.Date) = @year)
AND ec.EndDate < dat.Fecha
group by dat.fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.NameShift, dat.color, dat.Hours, dat.Absence, dat.Pending
order by IDEmployee, Week, DayWeek, Day
return null

EXECUTE [dbo].[Report_calendario_mensual_usuarios]
GO


DROP PROCEDURE [dbo].[Report_calendario_semanal_grupos_list]
GO
CREATE PROCEDURE [dbo].[Report_calendario_semanal_grupos_list]
@idPassport nvarchar(max) = '5',
@StartDate smalldatetime = '20220626',
@employees nvarchar(max) = '244,245,371,542,566'
AS
DECLARE @EndDate smalldatetime = DATEADD(DAY, 6, @StartDate);

WITH Dates AS(
SELECT @StartDate as Fecha, NULL as IDEmployee,
NULL As EmployeeName, NULL as Contract,
NULL as BeginContract, NULL as EndContract,
NULL as IDGroup, NULL as GroupName,NULL as Ruta
UNION ALL
SELECT DATEADD(DAY, 1,Fecha), NULL as IDEmployee,
NULL As EmployeeName, NULL as Contract,
NULL as BeginContract, NULL as EndContract,
NULL as IDGroup, NULL as GroupName,NULL as Ruta
FROM Dates
WHERE DATEADD(DAY, 1,Fecha) <= @EndDate)
select distinct d.IDEmployee,
emp.Name As EmployeeName, ec.IDContract as Contract,
ec.BeginDate as BeginContract, ec.EndDate as EndContract,
g.Id as IDGroup, g.Name as GroupName, g.FullGroupName as Ruta
from Dates
left join DailySchedule as d on Dates.Fecha = d.Date and d.IDEmployee IN (select * from split(@employees,',')) and d.Date between @StartDate and @EndDate
left join Shifts on (d.IDShiftUsed is not null and d.IDShiftUsed = Shifts.ID) or (d.IDShift1 = Shifts.ID)
left join Employees emp on emp.ID = d.IDEmployee
left join EmployeeContracts ec on ec.IDEmployee = emp.ID AND (ec.BeginDate <= @EndDate and ec.EndDate >= @EndDate)
left join EmployeeGroups eg on eg.IDEmployee = emp.ID AND (eg.BeginDate <= @EndDate and eg.EndDate >= @EndDate)
left join Groups g on eg.IDGroup = g.ID
left join ProgrammedAbsences as pa on d.IDEmployee = pa.IDEmployee AND pa.AbsenceID is null
and d.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
where dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,d.IDEmployee,2,0,0,GetDate()) > 0
group by d.IDEmployee, datepart(mm, Date), datepart(YYYY, Date), datepart(dd, EOMONTH(Date)), emp.Name, ec.IDContract,
ec.BeginDate, ec.EndDate,
g.Id, g.Name, g.FullGroupName
order by d.IDEmployee
option (maxrecursion 0)
RETURN NULL

EXECUTE [dbo].[Report_calendario_semanal_grupos_list]
GO


DROP PROCEDURE [dbo].[Report_calendario_semanal_grupos]
GO
CREATE PROCEDURE [dbo].[Report_calendario_semanal_grupos]
@StartDate smalldatetime = '20220620',
@employee nvarchar(max) = '244',
@IDGroup int = 17
AS
DECLARE @EndDate smalldatetime = DATEADD(DAY, 6, @StartDate);

WITH Dates AS(
SELECT @StartDate as Fecha, @employee as IDEmployee, datepart(ww, @StartDate) as Week, datepart(dw, @StartDate) as DayWeek, datepart(dd, @StartDate) as Day, datepart(mm, @StartDate) as Month, NULL as NameShift, 16777215 as Color, NULL as Hours, NULL as Absence, NULL as Pending
UNION ALL
SELECT DATEADD(DAY, 1,Fecha), @employee as IDEmployee, datepart(ww, DATEADD(DAY, 1,Fecha)) as Week, datepart(dw, DATEADD(DAY, 1,Fecha)) as DayWeek, datepart(dd, DATEADD(DAY, 1,Fecha)) as Day, datepart(mm, DATEADD(DAY, 1,Fecha)) as Month,NULL as NameShift, 16777215 as Color, NULL as Hours, NULL as Absence, NULL as Pending
FROM Dates
WHERE DATEADD(DAY, 1,Fecha) <= @EndDate)
select d.Date as Fecha, d.IDEmployee, datepart(ww, d.Date) as Week, datepart(dw, d.Date) as DayWeek, datepart(dd, d.Date) as Day, datepart(mm, d.Date) as Month, shifts.Name as NameShift, ISNULL(shifts.Color,16777215) as Color, shifts.ExpectedWorkingHours as Hours, pa.AbsenceID as Absence, r.Status as Pending
from DailySchedule as d
left join Shifts on (d.IDShift1 = Shifts.ID)
left join ProgrammedAbsences as pa
on d.IDEmployee = pa.IDEmployee
left join EmployeeGroups eg ON eg.IDEmployee = d.IDEmployee and eg.IDGroup = @IDGroup 
and d.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
left join Requests as r on d.IDEmployee = r.IDEmployee
and r.Status = 0 and r.RequestType = 6 and r.ID IN (select IDRequest from sysroRequestDays where Date = d.Date)
where d.IDEmployee = @employee and d.Date between @StartDate and @EndDate
UNION ALL
select dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.Month, CAST(dat.NameShift as nvarchar(max)) as NameShift, dat.Color, dat.Hours, dat.Absence, dat.Pending
from Dates dat
left join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and dsc.Date between @StartDate and @EndDate)
AND (ec.BeginDate <= dat.Fecha and ec.EndDate >= dat.Fecha)
UNION ALL
select dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.Month, CAST(dat.NameShift as nvarchar(max)) as NameShift, NULL as Color, dat.Hours, dat.Absence, dat.Pending
from Dates dat
left join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and dsc.Date between @StartDate and @EndDate)
AND ec.EndDate < dat.Fecha
group by dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.Month, dat.NameShift, dat.hours, dat.Absence, dat.Pending
order by IDEmployee, Fecha, Month, Day, DayWeek
return null

EXECUTE [dbo].[Report_calendario_semanal_grupos]
GO


UPDATE sysroParameters SET Data='576' WHERE ID='DBVersion'
GO
