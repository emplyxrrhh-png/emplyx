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
SET DATEFIRST 1;
WITH Dates AS(
SELECT @StartDate as Fecha, @employee as IDEmployee, datepart(ww, @StartDate) as Week, datepart(dw, @StartDate) as DayWeek, datepart(dd, @StartDate) as Day, datepart(mm, @StartDate) as Month, NULL as NameShift, null as Color, NULL as Hours, NULL as Absence, NULL as Pending
UNION ALL
SELECT DATEADD(DAY, 1,Fecha), @employee as IDEmployee, datepart(ww, DATEADD(DAY, 1,Fecha)) as Week, datepart(dw, DATEADD(DAY, 1,Fecha)) as DayWeek, datepart(dd, DATEADD(DAY, 1,Fecha)) as Day, datepart(mm, DATEADD(DAY, 1,Fecha)) as Month,NULL as NameShift, null as Color, NULL as Hours, NULL as Absence, NULL as Pending
FROM Dates
WHERE DATEADD(DAY, 1,Fecha) <= @EndDate)
select * from (
select Date as Fecha, d.IDEmployee, datepart(ww, Date) as Week, datepart(dw, Date) as DayWeek, datepart(dd, Date) as Day, datepart(mm, Date) as Month, shifts.Name as NameShift, ISNULL(shifts.Color,16777215) as Color, shifts.ExpectedWorkingHours as Hours, pa.AbsenceID as Absence, r.Status as Pending
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
select dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.Month, CAST(dat.NameShift as nvarchar(max)) as NameShift, 16777215 as Color, dat.Hours, dat.Absence, dat.Pending
from Dates dat
join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and datepart(MM, dsc.Date) = @month and datepart(YYYY, dsc.Date) = @year)
and dat.fecha between (select top 1 begindate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc) and (select top 1 EndDate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc)
UNION ALL
select dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.Month, CAST(dat.NameShift as nvarchar(max)) as NameShift, null as Color, dat.Hours, dat.Absence, dat.Pending
from Dates dat
join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and datepart(MM, dsc.Date) = @month and datepart(YYYY, dsc.Date) = @year)
and dat.fecha not between (select top 1 begindate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc) and (select top 1 EndDate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc)
) t
group by fecha, t.IDEmployee, Week, DayWeek, Day, Month, NameShift, Color, Hours, Absence, Pending
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
SET DATEFIRST 1;
WITH Dates AS(
SELECT @StartDate as Fecha, @employee as IDEmployee, datepart(ww, @StartDate) as Week, datepart(dw, @StartDate) as DayWeek, datepart(dd, @StartDate) as Day, NULL as NameShift, null as Color, NULL as Hours, NULL as Absence, NULL as Pending
UNION ALL
SELECT DATEADD(DAY, 1,Fecha), @employee as IDEmployee, datepart(ww, DATEADD(DAY, 1,Fecha)) as Week, datepart(dw, DATEADD(DAY, 1,Fecha)) as DayWeek, datepart(dd, DATEADD(DAY, 1,Fecha)) as Day, NULL as NameShift, null as Color, NULL as Hours, NULL as Absence, NULL as Pending
FROM Dates
WHERE DATEADD(DAY, 1,Fecha) <= @EndDate
)
Select NULL as Fecha, @employee as IDEmployee, NULL as Week, DayWeek, NULL as Day, NULL as NameShift, NULL as Color, NULL as Hours, NULL as Absence, NULL as Pending from (
SELECT top 7
ROW_NUMBER() OVER(ORDER BY Fecha ASC) AS DayWeek,
fecha , 129 as idemployee
FROM dates) x where DayWeek < case when (select TOP(1) DayWeek from dates) = 1 then 7 else (select TOP(1) DayWeek from dates)-1 end
UNION ALL
select * from (
select Date as Fecha, d.IDEmployee, datepart(ww, Date) as Week, datepart(dw, Date) as DayWeek, datepart(dd, Date) as Day, shifts.Name as NameShift, ISNULL(shifts.Color,16777215) as Color, shifts.ExpectedWorkingHours as Hours, pa.AbsenceID as Absence, r.Status as Pending
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
select dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, CAST(dat.NameShift as nvarchar(max)) as NameShift, 16777215 as Color, dat.Hours, dat.Absence, dat.Pending
from Dates dat
join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and datepart(MM, dsc.Date) = @month and datepart(YYYY, dsc.Date) = @year)
and dat.fecha between (select top 1 begindate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc) and (select top 1 EndDate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc)
UNION ALL
select dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, CAST(dat.NameShift as nvarchar(max)) as NameShift, null as Color, dat.Hours, dat.Absence, dat.Pending
from Dates dat
join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and datepart(MM, dsc.Date) = @month and datepart(YYYY, dsc.Date) = @year)
and dat.fecha not between (select top 1 begindate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc) and (select top 1 EndDate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc)
) t
group by t.fecha, t.IDEmployee, t.Week, t.DayWeek, t.Day, t.NameShift, t.color, t.Hours, t.Absence, t.Pending
order by IDEmployee, Week, DayWeek, Day
return null

EXECUTE [dbo].[Report_calendario_mensual_usuarios]
GO


DROP PROCEDURE [dbo].[Report_calendario_anual_usuarios_list]
GO
CREATE PROCEDURE [dbo].[Report_calendario_anual_usuarios_list]
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
 SUM(shifts.ExpectedWorkingHours) as Hours,
 emp.Name As EmployeeName, ec.IDContract as Contract, 
	  ec.BeginDate as BeginContract, ec.EndDate as EndContract,
	  g.Id as IDGroup, g.Name as GroupName, g.FullGroupName as Ruta
	  from Dates 
	  left join DailySchedule as d on Dates.Fecha = d.Date 
  left join Shifts on (d.IDShiftUsed is not null and d.IDShiftUsed = Shifts.ID) or (d.IDShift1 = Shifts.ID)
  left join Employees emp on emp.ID = d.IDEmployee
  left join EmployeeContracts ec on ec.IDEmployee = emp.ID
	 left join EmployeeGroups eg on eg.IDEmployee = emp.ID
	  left join Groups g on eg.IDGroup = g.ID
  left join ProgrammedAbsences as pa 
  on d.IDEmployee = pa.IDEmployee
  and d.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
  where d.IDEmployee IN (select * from split(@employees,',')) and Date between CONCAT(@yearStart,@monthStart,'01') and EOMONTH(convert(smalldatetime,CONCAT(@yearEnd,@monthEnd,'01'),120)) 
  AND ec.IDContract = (select top 1 sec.IDContract from employeeContracts sec where Dates.Fecha between sec.BeginDate and sec.EndDate  and IDEmployee = emp.ID)
  AND g.ID = (select top 1 seg.IDGroup from employeeGroups seg where Dates.Fecha between seg.BeginDate and seg.EndDate and IDEmployee = emp.ID)
  AND pa.AbsenceID is null
  and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,d.IDEmployee,2,0,0,GetDate()) > 0
  group by d.IDEmployee, datepart(mm, Date), datepart(YYYY, Date), datepart(dd, EOMONTH(Date)), emp.Name, ec.IDContract, 
	  ec.BeginDate, ec.EndDate,
	  g.Id, g.Name, g.FullGroupName
  order by d.IDEmployee, Month, Year
  option (maxrecursion 0)
  RETURN NULL

  EXECUTE [dbo].[Report_calendario_anual_usuarios_list]
  GO




DROP PROCEDURE [dbo].[Report_calendario_semanal_grupos]
GO
CREATE PROCEDURE [dbo].[Report_calendario_semanal_grupos]
@StartDate smalldatetime = '20220620',
@employee nvarchar(max) = '244',
@IDGroup int = 17
AS
DECLARE @EndDate smalldatetime = DATEADD(DAY, 6, @StartDate);
SET DATEFIRST 1;
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
and dat.fecha not between (select top 1 begindate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc) and (select top 1 EndDate from employeeContracts sec where sec.IDEmployee = dat.IDEmployee order by BeginDate desc)
group by dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.Month, dat.NameShift, dat.hours, dat.Absence, dat.Pending
order by IDEmployee, Fecha, Month, Day, DayWeek
return null

EXECUTE [dbo].[Report_calendario_semanal_grupos]
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='578' WHERE ID='DBVersion'
GO
