ALTER PROCEDURE [dbo].[Report_calendario_mensual_grupos]
@idPassport nvarchar(100) = '1',
@month nvarchar(2) = '01',
@year nvarchar(4) = '2021',
@IDEmployee nvarchar(max) = '114,242'
AS

DECLARE @beginDate DATE = CONCAT(@year, '-', @month, '-01')
DECLARE @endDate DATE = DATEADD(MONTH, 1, @beginDate)
;WITH EmployeeCTE AS (
    SELECT value AS EmployeeID
    FROM split(@IDEmployee, ',')
)

select * from(
select DailySchedule.IDEmployee, datepart(dd, Date) as Date, shifts.Name as NameShift
from DailySchedule 
inner join EmployeeCTE on DailySchedule.IDEmployee = EmployeeCTE.EmployeeID
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = DailySchedule.IDEmployee and cast(DailySchedule.Date as date) between poe.BeginDate and poe.EndDate and poe.IdFeature = 2 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
inner join Shifts on
(DailySchedule.IDShiftUsed is not null and DailySchedule.IDShiftUsed = Shifts.ID)
or (DailySchedule.IDShift1 = Shifts.ID) 
where cast(date as date) between @beginDate and @endDate ) t
PIVOT(Max(NameShift) FOR Date IN("1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20","21","22","23","24","25","26","27","28","29","30","31")
) AS pivot_table

inner join ( select emp.ID AS EmployeeID, emp.Name As EmployeeName, ec.IDContract as Contract, ec.BeginDate as BeginContract, ec.EndDate as EndContract, g.Id as IDGroup, g.Name as GroupName, g.FullGroupName as Ruta 
from Employees emp
inner join EmployeeCTE on emp.ID = EmployeeCTE.EmployeeID
inner join EmployeeContracts ec on ec.IDEmployee = emp.ID join EmployeeGroups eg on eg.IDEmployee = emp.ID
inner join Groups g on eg.IDGroup = g.ID 
where cast(ec.BeginDate as date) < @EndDate
    AND (cast(ec.EndDate as date) > @BeginDate OR ec.EndDate IS NULL)
    AND cast(eg.BeginDate as date) < @EndDate
    AND (cast(eg.EndDate as date) > @BeginDate OR eg.EndDate IS NULL) 
) as emps on pivot_table.IDEmployee = emps.EmployeeID

inner join ( 
select * from( select DailySchedule.IDEmployee,
CONCAT('c',datepart(dd, Date)) as Date, ISNULL(DailySchedule.ShiftColor1,s.color) as Color from DailySchedule
inner join EmployeeCTE on DailySchedule.IDEmployee = EmployeeCTE.EmployeeID
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = DailySchedule.IDEmployee and cast(DailySchedule.Date as date) between poe.BeginDate and poe.EndDate and poe.IdFeature = 2 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
join shifts s on (IDShiftUsed is not null and IDShiftUsed = s.ID) or (IDShift1 = s.ID)
where cast(date as date) between @beginDate and @endDate ) t PIVOT(Max(Color) FOR Date
IN("c1","c2","c3","c4","c5","c6","c7","c8","c9","c10","c11","c12","c13","c14","c15","c16","c17","c18","c19","c20","c21","c22","c23","c24","c25","c26","c27","c28","c29","c30","c31")) AS pivot_table 
) as p2 on pivot_table.IDEmployee = p2.IDEmployee

inner join ( 
select * from( select DailySchedule.IDEmployee, CONCAT('h',datepart(dd, Date)) as Date, 
CASE WHEN s.ShiftType = 2 THEN 0 ELSE ISNULL(DailySchedule.ExpectedWorkingHours, s.ExpectedWorkingHours) END AS Horas
from DailySchedule
inner join EmployeeCTE on DailySchedule.IDEmployee = EmployeeCTE.EmployeeID
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = DailySchedule.IDEmployee and cast(DailySchedule.Date as date) between poe.BeginDate and poe.EndDate and poe.IdFeature = 2 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
join shifts s on (IDShiftUsed is not null and IDShiftUsed = s.ID) or (IDShift1 = s.ID) 
where cast(date as date) between @beginDate and @endDate ) t
PIVOT(Max(Horas) FOR Date IN("h1","h2","h3","h4","h5","h6","h7","h8","h9","h10","h11","h12","h13","h14","h15","h16","h17","h18","h19","h20","h21","h22","h23","h24","h25","h26","h27","h28","h29","h30","h31")) AS pivot_table 
) as p3 on pivot_table.IDEmployee = p3.IDEmployee 

left join ( 
select * from( select d.IDEmployee, CONCAT('a',datepart(dd, Date)) as Date, pa.AbsenceID as Absence from DailySchedule as d 
inner join EmployeeCTE on d.IDEmployee = EmployeeCTE.EmployeeID
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = d.IDEmployee and cast(d.Date as date) between poe.BeginDate and poe.EndDate and poe.IdFeature = 2 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
left join ProgrammedAbsences as pa on d.IDEmployee = pa.IDEmployee and cast(d.date as date) between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
where cast(date as date) between @beginDate and @endDate ) t PIVOT(Max(Absence) FOR
Date IN("a1","a2","a3","a4","a5","a6","a7","a8","a9","a10","a11","a12","a13","a14","a15","a16","a17","a18","a19","a20","a21","a22","a23","a24","a25","a26","a27","a28","a29","a30","a31")) AS pivot_table 
) as p4 on pivot_table.IDEmployee = p4.IDEmployee

left join ( 
select * from( select d.IDEmployee, CONCAT('v',datepart(dd, d.Date)) as Date, r.Status as Status from DailySchedule as d 
inner join EmployeeCTE on d.IDEmployee = EmployeeCTE.EmployeeID
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = d.IDEmployee and cast(d.Date as date) between poe.BeginDate and poe.EndDate and poe.IdFeature = 2 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
left join Requests as r on d.IDEmployee = r.IDEmployee and
r.Status IN (0,1) and r.RequestType = 6
inner join sysroRequestDays srd ON r.id = srd.IDRequest and cast(d.Date as date) = cast(srd.Date as date)
where cast(d.date as date) between @beginDate and @endDate ) t PIVOT(Max(Status) FOR Date IN("v1","v2","v3","v4","v5","v6","v7","v8","v9","v10","v11","v12","v13","v14","v15","v16","v17","v18","v19","v20","v21","v22","v23","v24","v25","v26","v27","v28","v29","v30","v31")) AS pivot_table 
) as p5 on pivot_table.IDEmployee = p5.IDEmployee

LEFT JOIN Employees AS emp on emp.ID = pivot_table.IDEmployee
order by emp.Name
GO


ALTER PROCEDURE [dbo].[Report_calendario_mensual_grupos_resumen]
@month nvarchar(2) = '01',
@year nvarchar(4) = '2021',
@IDEmployee nvarchar(max) = '114,242',
@IDGroup nvarchar(9) = '123'
AS
select ISNULL(ds.ShiftName1,s.Name) as Name, s.ShortName, ISNULL(ds.ShiftColor1,s.Color) as Color, COUNT(ISNULL(ds.IDShiftUsed,ds.IDShift1)) as Days, 
SUM(CASE WHEN S.ShiftType = 2 THEN 0 ELSE ISNULL(ds.ExpectedWorkingHours, s.ExpectedWorkingHours) END) AS ExpectedWorkingHours
from DailySchedule ds inner join shifts s on (IDShiftUsed is not null and IDShiftUsed = s.ID) or (IDShift1 = s.ID) inner join EmployeeGroups as eg on ds.IDEmployee = eg.IDEmployee and ds.Date between eg.BeginDate and eg.EndDate where ds.IDEmployee IN (select * from split(@IDEmployee,',')) and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year and eg.IDGroup = @IDGroup GROUP BY ISNULL(ds.ShiftName1,s.Name), s.ShortName, ISNULL(ds.ShiftColor1,s.Color);
RETURN NULL
GO


ALTER PROCEDURE [dbo].[Report_calendario_mensual_usuarios]
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
FROM dates) x where DayWeek <= case when (select TOP(1) DayWeek from dates) = 1 then 7 else (select TOP(1) DayWeek from dates)-1 end
UNION ALL
select * from (
select Date as Fecha, d.IDEmployee, datepart(ww, Date) as Week, datepart(dw, Date) as DayWeek, datepart(dd, Date) as Day, shifts.ID as IDShift, shifts.Name as NameShift, ISNULL(ISNULL(d.ShiftColor1,shifts.Color),16777215) as Color, 
CASE WHEN shifts.ShiftType = 2 THEN 0 ELSE ISNULL(d.ExpectedWorkingHours, shifts.ExpectedWorkingHours) END AS Hours, pa.AbsenceID as Absence, r.Status as Pending
from DailySchedule as d
join EmployeeContracts ec on ec.IDEmployee = d.IDEmployee
left join Shifts on (d.IDShift1 = Shifts.ID)
left join ProgrammedAbsences as pa
on d.IDEmployee = pa.IDEmployee
and d.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
left join Requests as r on d.IDEmployee = r.IDEmployee
and r.Status in (0,1) and r.RequestType = 6 and r.ID IN (select IDRequest from sysroRequestDays where Date = d.Date)
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
GO

ALTER PROCEDURE [dbo].[Report_calendario_semanal_grupos]
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
select distinct d.Date as Fecha, d.IDEmployee, datepart(ww, d.Date) as Week, datepart(dw, d.Date) as DayWeek, datepart(dd, d.Date) as Day, datepart(mm, d.Date) as Month, shifts.Name as NameShift, ISNULL(ISNULL(d.ShiftColor1,shifts.Color),16777215) as Color, 
CASE WHEN shifts.ShiftType = 2 THEN 0 ELSE ISNULL(d.ExpectedWorkingHours, shifts.ExpectedWorkingHours) END AS Hours, pa.AbsenceID as Absence, r.Status as Pending
from DailySchedule as d
left join Shifts on (d.IDShift1 = Shifts.ID)
left join ProgrammedAbsences as pa
on d.IDEmployee = pa.IDEmployee and d.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
left join EmployeeGroups eg ON eg.IDEmployee = d.IDEmployee and eg.IDGroup = @IDGroup
left join Requests as r on d.IDEmployee = r.IDEmployee
and r.Status in (0,1) and r.RequestType = 6 and r.ID IN (select IDRequest from sysroRequestDays where Date = d.Date)
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
GO


ALTER PROCEDURE [dbo].[Report_calendario_semanal_grupos_resumen]
@idPassport nvarchar(max) = '1024',
@StartDate smalldatetime = '20210301',
@IDEmployee nvarchar(max) = '60,100,148,151,232,252,338,171,233,363,656',
@IDGroup nvarchar(9) = '107'
AS
DECLARE @EndDate smalldatetime = DATEADD(DAY, 6, @StartDate);
select ISNULL(ds.ShiftName1,s.Name) as Name, s.ShortName, ISNULL(ds.ShiftColor1,s.Color) as Color, COUNT(ISNULL(ds.IDShiftUsed,ds.IDShift1)) as Days, 
SUM(CASE WHEN s.ShiftType = 2 THEN 0 ELSE ISNULL(ds.ExpectedWorkingHours, s.ExpectedWorkingHours) END) AS ExpectedWorkingHours from DailySchedule ds inner join shifts s on (IDShiftUsed is not null and IDShiftUsed = s.ID) or (IDShift1 = s.ID) inner join EmployeeGroups as eg on ds.IDEmployee = eg.IDEmployee and ds.Date between eg.BeginDate and eg.EndDate where ds.IDEmployee IN (select * from split(@IDEmployee,',')) and Date between @StartDate and @EndDate and eg.IDGroup = @IDGroup GROUP BY ISNULL(ds.ShiftName1,s.Name), s.ShortName, ISNULL(ds.ShiftColor1,s.Color);
RETURN NULL
GO



ALTER PROCEDURE [dbo].[Report_calendario_semanal_usuarios_resumen]
@idPassport nvarchar(max) = '1024',
@StartDate smalldatetime = '20210301',
@IDEmployee nvarchar(max) = '60,100,148,151,232,252,338,171,233,363,656'
AS
DECLARE @EndDate smalldatetime = DATEADD(DAY, 6, @StartDate);
select ISNULL(ds.ShiftName1,s.Name) as Name, s.ShortName, ISNULL(ds.ShiftColor1,s.Color) as Color, COUNT(ISNULL(ds.IDShiftUsed,ds.IDShift1)) as Days, 
SUM(CASE WHEN s.ShiftType = 2 THEN 0 ELSE ISNULL(ds.ExpectedWorkingHours, s.ExpectedWorkingHours) END) AS ExpectedWorkingHours from DailySchedule ds inner join shifts s on (IDShiftUsed is not null and IDShiftUsed = s.ID) or (IDShift1 = s.ID) inner join EmployeeGroups as eg on ds.IDEmployee = eg.IDEmployee and ds.Date between eg.BeginDate and eg.EndDate where ds.IDEmployee IN (select * from split(@IDEmployee,',')) and Date between @StartDate and @EndDate GROUP BY ISNULL(ds.ShiftName1,s.Name), s.ShortName, ISNULL(ds.ShiftColor1,s.Color);
RETURN NULL
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='825' WHERE ID='DBVersion'
GO
