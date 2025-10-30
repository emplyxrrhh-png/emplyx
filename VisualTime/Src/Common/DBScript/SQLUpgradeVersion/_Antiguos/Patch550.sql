
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

-- Informes nueva era. Calendario mensual grupos + Calendario mensual usuarios + Calendario semanal grupos + Calendario anual usuarios
DROP PROCEDURE [dbo].[Report_calendario_mensual_grupos]
GO
CREATE PROCEDURE [dbo].[Report_calendario_mensual_grupos]
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
	 inner join ( select * from( select IDEmployee, CONCAT('h',datepart(dd, Date)) as Date, s.ExpectedWorkingHours as Horas from DailySchedule 
	 join shifts s on (IDShiftUsed is not null and IDShiftUsed = s.ID) or (IDShift1 = s.ID) where IDEmployee IN (select * from split(@IDEmployee,',')) 
	 and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,IDEmployee,2,0,0,GetDate()) > 0 and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year ) t 
	 PIVOT(Max(Horas) FOR Date IN("h1","h2","h3","h4","h5","h6","h7","h8","h9","h10","h11","h12","h13","h14","h15","h16","h17","h18","h19","h20","h21","h22","h23","h24","h25","h26","h27","h28","h29","h30","h31")) AS pivot_table ) as p3 
	 on pivot_table.IDEmployee = p3.IDEmployee inner join ( select * from( select d.IDEmployee, CONCAT('a',datepart(dd, Date)) as Date, pa.AbsenceID as Absence  
	 from DailySchedule as d left join ProgrammedAbsences as pa on d.IDEmployee = pa.IDEmployee and d.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate)) 
	 where d.IDEmployee IN (select * from split(@IDEmployee,',')) and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,d.IDEmployee,2,0,0,GetDate()) > 0 
	 and datepart(MM, Date) = @month and datepart(YYYY, Date) = @year ) t PIVOT(Max(Absence) FOR 
	 Date IN("a1","a2","a3","a4","a5","a6","a7","a8","a9","a10","a11","a12","a13","a14","a15","a16","a17","a18","a19","a20","a21","a22","a23","a24","a25","a26","a27","a28","a29","a30","a31")) AS pivot_table ) as p4 on pivot_table.IDEmployee = p4.IDEmployee 
	 left join ( select * from( select d.IDEmployee, CONCAT('v',datepart(dd, d.Date)) as Date, r.Status as Status from DailySchedule as d left join Requests as r on d.IDEmployee = r.IDEmployee and 
	  r.Status = 0 and r.RequestType = 6 
	 inner join sysroRequestDays srd ON r.id = srd.IDRequest and d.Date = srd.Date
	 where d.IDEmployee IN (select * from split(@IDEmployee,',')) 
	 and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,d.IDEmployee,2,0,0,GetDate()) > 0 and datepart(MM, d.Date) = @month 
	 and datepart(YYYY, d.Date) = @year ) t PIVOT(Max(Status) FOR Date IN("v1","v2","v3","v4","v5","v6","v7","v8","v9","v10","v11","v12","v13","v14","v15","v16","v17","v18","v19","v20","v21","v22","v23","v24","v25","v26","v27","v28","v29","v30","v31")) AS pivot_table ) 
	 as p5 on pivot_table.IDEmployee = p5.IDEmployee 
 GO

 DROP PROCEDURE [dbo].[Report_calendario_semanal_grupos]
GO
 CREATE PROCEDURE [dbo].[Report_calendario_semanal_grupos]
	@StartDate smalldatetime = '20220101', 
 	@employee nvarchar(max) = '16'
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
   and d.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
   left join Requests as r on d.IDEmployee = r.IDEmployee
   and r.Status = 0 and r.RequestType = 6 and r.ID IN (select IDRequest from sysroRequestDays where Date = d.Date)
   where d.IDEmployee = @employee and d.Date between @StartDate and @EndDate
   UNION ALL
   select dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.Month, CAST(dat.NameShift as nvarchar(max)) as NameShift, dat.Color, dat.Hours, dat.Absence, dat.Pending 
   from Dates dat 
   join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
   where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and dsc.Date between @StartDate and @EndDate)
   AND (ec.BeginDate <= dat.Fecha and ec.EndDate >= dat.Fecha)
   UNION ALL
   select dat.Fecha, dat.IDEmployee, dat.Week, dat.DayWeek, dat.Day, dat.Month, CAST(dat.NameShift as nvarchar(max)) as NameShift, NULL as Color, dat.Hours, dat.Absence, dat.Pending 
   from Dates dat 
   join EmployeeContracts ec on ec.IDEmployee = dat.IDEmployee
   where dat.Fecha not in (select dsc.Date from DailySchedule dsc where dsc.IDEmployee = @employee and dsc.Date between @StartDate and @EndDate)
   AND ec.EndDate < dat.Fecha
   order by IDEmployee, Fecha, Month, Day, DayWeek
   return null

  EXECUTE [dbo].[Report_calendario_semanal_grupos]
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
order by IDEmployee, Week, DayWeek, Day
	return null
	EXEC Report_calendario_mensual_usuarios;
GO

DROP PROCEDURE [dbo].[Report_calendario_semanal_grupos_list]
GO
CREATE PROCEDURE [dbo].[Report_calendario_semanal_grupos_list]
	@idPassport nvarchar(max) = '1024',
	@StartDate smalldatetime = '20210301',
	@employees nvarchar(max) = '551,64,364'
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
	  left join DailySchedule as d on Dates.Fecha = d.Date 
  left join Shifts on (d.IDShiftUsed is not null and d.IDShiftUsed = Shifts.ID) or (d.IDShift1 = Shifts.ID)
  left join Employees emp on emp.ID = d.IDEmployee
  left join EmployeeContracts ec on ec.IDEmployee = emp.ID
	 left join EmployeeGroups eg on eg.IDEmployee = emp.ID
	  left join Groups g on eg.IDGroup = g.ID
  left join ProgrammedAbsences as pa 
  on d.IDEmployee = pa.IDEmployee
  and d.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
  where d.IDEmployee IN (select * from split(@employees,',')) and Date between @StartDate and @EndDate
  AND (ec.BeginDate <= @StartDate and ec.EndDate >= @EndDate)
		AND (eg.BeginDate <= @StartDate and eg.EndDate >= @EndDate)
  AND pa.AbsenceID is null
  and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,d.IDEmployee,2,0,0,GetDate()) > 0
  group by d.IDEmployee, datepart(mm, Date), datepart(YYYY, Date), datepart(dd, EOMONTH(Date)), emp.Name, ec.IDContract, 
	  ec.BeginDate, ec.EndDate,
	  g.Id, g.Name, g.FullGroupName
  order by d.IDEmployee
  option (maxrecursion 0)
  RETURN NULL

  EXECUTE [dbo].[Report_calendario_semanal_grupos_list]
GO


UPDATE sysroParameters SET Data='550' WHERE ID='DBVersion'
GO