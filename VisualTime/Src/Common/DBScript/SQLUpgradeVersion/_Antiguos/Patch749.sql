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
SUM(ISNULL(d.ExpectedWorkingHours,shifts.ExpectedWorkingHours)) as Hours,
emp.Name As EmployeeName, ec.IDContract as Contract,
ec.BeginDate as BeginContract, ec.EndDate as EndContract,
g.Id as IDGroup, g.Name as GroupName, g.FullGroupName as Ruta
from Dates
left join DailySchedule as d on Dates.Fecha = d.Date
left join Shifts on (d.IDShiftUsed is not null and d.IDShiftUsed = Shifts.ID) or (d.IDShift1 = Shifts.ID)
left join Employees emp on emp.ID = d.IDEmployee
left join EmployeeContracts ec on ec.IDEmployee = emp.ID
left join EmployeeGroups eg on eg.IDEmployee = emp.ID and d.Date between eg.BeginDate and eg.EndDate
left join Groups g on eg.IDGroup = g.ID
where d.IDEmployee IN (select * from split(@employees,',')) and Date between CONCAT(@yearStart,@monthStart,'01') and EOMONTH(convert(smalldatetime,CONCAT(@yearEnd,@monthEnd,'01'),120))
AND ec.IDContract = (select top 1 sec.IDContract from employeeContracts sec where Dates.Fecha between sec.BeginDate and sec.EndDate  and IDEmployee = emp.ID)
AND g.ID = (select top 1 seg.IDGroup from employeeGroups seg where Dates.Fecha between seg.BeginDate and seg.EndDate and IDEmployee = emp.ID)
and dbo.WebLogin_GetPermissionOverEmployee(CAST(@idPassport as nvarchar(100)) ,d.IDEmployee,2,0,0,GetDate()) > 0
group by d.IDEmployee, datepart(mm, Date), datepart(YYYY, Date), datepart(dd, EOMONTH(Date)), emp.Name, ec.IDContract,
ec.BeginDate, ec.EndDate,
g.Id, g.Name, g.FullGroupName
order by d.IDEmployee, Month, Year
option (maxrecursion 0)
RETURN NULL

EXECUTE [dbo].[Report_calendario_anual_usuarios_list]
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='749' WHERE ID='DBVersion'
GO
