ALTER PROCEDURE [dbo].[Report_calendario_mensual_grupos]
@idPassport nvarchar(100) = '1',
@month nvarchar(2) = '01',
@year nvarchar(4) = '2021',
@IDEmployee nvarchar(max) = '114,242'
AS
select * from(
select IDEmployee, datepart(dd, Date) as Date, shifts.Name as NameShift
from DailySchedule inner join Shifts on
(DailySchedule.IDShiftUsed is not null and DailySchedule.IDShiftUsed = Shifts.ID)
or (DailySchedule.IDShift1 = Shifts.ID)
where IDEmployee IN
(select * from split(@IDEmployee,','))
and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,IDEmployee,2,0,0,GetDate()) > 0
and datepart(MM, Date) = @month
and datepart(YYYY, Date) = @year ) t
PIVOT(Max(NameShift) FOR Date IN("1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20","21","22","23","24","25","26","27","28","29","30","31")) AS pivot_table
join ( select emp.ID AS EmployeeID, emp.Name As EmployeeName, ec.IDContract as Contract, ec.BeginDate as BeginContract, ec.EndDate as EndContract, g.Id as IDGroup, g.Name as GroupName, g.FullGroupName as Ruta from Employees emp
join EmployeeContracts ec on ec.IDEmployee = emp.ID join EmployeeGroups eg on eg.IDEmployee = emp.ID
join Groups g on eg.IDGroup = g.ID where emp.ID IN (select * from split(@IDEmployee,','))
and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,emp.ID,2,0,0,GetDate()) > 0
AND (datepart(YYYY, ec.BeginDate) < @year
or (datepart(MM, ec.BeginDate) <= @month and datepart(YYYY, ec.BeginDate) <= @year))
and (datepart(YYYY, ec.EndDate) > @year or (datepart(MM, ec.EndDate) >= @month and datepart(YYYY, ec.EndDate) = @year))
AND (datepart(YYYY, eg.BeginDate) < @year
or (datepart(MM, eg.BeginDate) <= @month and datepart(YYYY, eg.BeginDate) <= @year))
and (datepart(YYYY, eg.EndDate) > @year or (datepart(MM, eg.EndDate) >= @month
and datepart(YYYY, eg.EndDate) = @year)) ) as emps on pivot_table.IDEmployee = emps.EmployeeID
inner join ( select * from( select IDEmployee,
CONCAT('c',datepart(dd, Date)) as Date, s.color as Color from DailySchedule
join shifts s on (IDShiftUsed is not null and IDShiftUsed = s.ID) or (IDShift1 = s.ID)
where IDEmployee IN (select * from split(@IDEmployee,',')) and
dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,IDEmployee,2,0,0,GetDate()) > 0 and
datepart(MM, Date) = @month and datepart(YYYY, Date) = @year ) t PIVOT(Max(Color) FOR Date
IN("c1","c2","c3","c4","c5","c6","c7","c8","c9","c10","c11","c12","c13","c14","c15","c16","c17","c18","c19","c20","c21","c22","c23","c24","c25","c26","c27","c28","c29","c30","c31")) AS pivot_table ) as p2
on pivot_table.IDEmployee = p2.IDEmployee
inner join ( select * from( select IDEmployee, CONCAT('h',datepart(dd, Date)) as Date, ISNULL(DailySchedule.ExpectedWorkingHours,s.ExpectedWorkingHours) as Horas from DailySchedule
join shifts s on (IDShiftUsed is not null and IDShiftUsed = s.ID) or (IDShift1 = s.ID) where IDEmployee IN (select * from split(@IDEmployee,','))
and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,IDEmployee,2,0,0,GetDate()) > 0 and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year ) t
PIVOT(Max(Horas) FOR Date IN("h1","h2","h3","h4","h5","h6","h7","h8","h9","h10","h11","h12","h13","h14","h15","h16","h17","h18","h19","h20","h21","h22","h23","h24","h25","h26","h27","h28","h29","h30","h31")) AS pivot_table ) as p3
on pivot_table.IDEmployee = p3.IDEmployee inner join ( select * from( select d.IDEmployee, CONCAT('a',datepart(dd, Date)) as Date, pa.AbsenceID as Absence
from DailySchedule as d left join ProgrammedAbsences as pa on d.IDEmployee = pa.IDEmployee and d.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
where d.IDEmployee IN (select * from split(@IDEmployee,',')) and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,d.IDEmployee,2,0,0,GetDate()) > 0
and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year ) t PIVOT(Max(Absence) FOR
Date IN("a1","a2","a3","a4","a5","a6","a7","a8","a9","a10","a11","a12","a13","a14","a15","a16","a17","a18","a19","a20","a21","a22","a23","a24","a25","a26","a27","a28","a29","a30","a31")) AS pivot_table ) as p4 on pivot_table.IDEmployee = p4.IDEmployee
left join ( select * from( select d.IDEmployee, CONCAT('v',datepart(dd, d.Date)) as Date, r.Status as Status from DailySchedule as d left join Requests as r on d.IDEmployee = r.IDEmployee and
r.Status IN (0,1) and r.RequestType = 6
inner join sysroRequestDays srd ON r.id = srd.IDRequest and d.Date = srd.Date
where d.IDEmployee IN (select * from split(@IDEmployee,','))
and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,d.IDEmployee,2,0,0,GetDate()) > 0 and datepart(MM, d.Date) = @month
and datepart(YYYY, d.Date) = @year ) t PIVOT(Max(Status) FOR Date IN("v1","v2","v3","v4","v5","v6","v7","v8","v9","v10","v11","v12","v13","v14","v15","v16","v17","v18","v19","v20","v21","v22","v23","v24","v25","v26","v27","v28","v29","v30","v31")) AS pivot_table )
as p5 on pivot_table.IDEmployee = p5.IDEmployee
LEFT JOIN Employees AS emp on emp.ID = pivot_table.IDEmployee
order by emp.Name

EXECUTE [dbo].[Report_calendario_mensual_grupos]
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='633' WHERE ID='DBVersion'
GO
