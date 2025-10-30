
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
SET DATEFIRST 1;
WITH Dates AS(
SELECT @StartDate as Fecha, @employee as IDEmployee, datepart(ww, @StartDate) as Week, datepart(dw, @StartDate) as DayWeek, datepart(dd, @StartDate) as Day, NULL as IDShift, NULL as NameShift, null as Color, NULL as Hours, NULL as Absence, NULL as Pending
UNION ALL
SELECT DATEADD(DAY, 1,Fecha), @employee as IDEmployee, datepart(ww, DATEADD(DAY, 1,Fecha)) as Week, datepart(dw, DATEADD(DAY, 1,Fecha)) as DayWeek, datepart(dd, DATEADD(DAY, 1,Fecha)) as Day, NULL as IDShift, NULL as NameShift, null as Color, NULL as Hours, NULL as Absence, NULL as Pending
FROM Dates
WHERE DATEADD(DAY, 1,Fecha) <= @EndDate
)
Select NULL as Fecha, @employee as IDEmployee, NULL as Week, DayWeek, NULL as Day, NULL as IDShift, NULL as NameShift, NULL as Color, NULL as Hours, NULL as Absence, NULL as Pending from (
SELECT top 7
ROW_NUMBER() OVER(ORDER BY Fecha ASC) AS DayWeek,
fecha , 129 as idemployee
FROM dates) x where DayWeek < case when (select TOP(1) DayWeek from dates) = 1 then 7 else (select TOP(1) DayWeek from dates)-1 end
UNION ALL
select * from (
select Date as Fecha, d.IDEmployee, datepart(ww, Date) as Week, datepart(dw, Date) as DayWeek, datepart(dd, Date) as Day, shifts.ID as IDShift, shifts.Name as NameShift, ISNULL(shifts.Color,16777215) as Color, shifts.ExpectedWorkingHours as Hours, pa.AbsenceID as Absence, r.Status as Pending
from DailySchedule as d
join EmployeeContracts ec on ec.IDEmployee = d.IDEmployee
left join Shifts on (d.IDShift1 = Shifts.ID)
left join ProgrammedAbsences as pa
on d.IDEmployee = pa.IDEmployee
and d.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
left join Requests as r on d.IDEmployee = r.IDEmployee
and r.Status = 0 and r.RequestType = 6 and r.ID IN (select IDRequest from sysroRequestDays where Date = d.Date)
where d.IDEmployee = @employee and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year
UNION ALL
select dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.IDShift, CAST(dat.NameShift as nvarchar(max)) as NameShift, 16777215 as Color, dat.Hours, dat.Absence, dat.Pending
from Dates dat
join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and datepart(MM, dsc.Date) = @month and datepart(YYYY, dsc.Date) = @year)
and dat.fecha between (select top 1 begindate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc) and (select top 1 EndDate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc)
UNION ALL
select dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.IDShift, CAST(dat.NameShift as nvarchar(max)) as NameShift, null as Color, dat.Hours, dat.Absence, dat.Pending
from Dates dat
join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and datepart(MM, dsc.Date) = @month and datepart(YYYY, dsc.Date) = @year)
and dat.fecha not between (select top 1 begindate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc) and (select top 1 EndDate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc)
) t
group by t.fecha, t.IDEmployee, t.Week, t.DayWeek, t.Day, t.IDShift, t.NameShift, t.color, t.Hours, t.Absence, t.Pending
order by IDEmployee, Week, DayWeek, Day
return null

EXECUTE [dbo].[Report_calendario_mensual_usuarios]
GO



UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO
UPDATE sysroParameters SET Data='583' WHERE ID='DBVersion'
GO
