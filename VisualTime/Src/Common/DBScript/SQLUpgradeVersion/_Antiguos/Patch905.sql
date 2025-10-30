ALTER PROCEDURE [dbo].[Report_monthly_accruals_by_contract]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@IDConcept nvarchar(max) = '0',
	@monthStart nvarchar(2) = '01',
@yearStart nvarchar(4) = '2021',
@monthEnd nvarchar(2) = '04',
@yearEnd nvarchar(4) = '2021',
@idConcept1 nvarchar(max) = '0',
@idConcept2 nvarchar(max) = '0',
@idConcept3 nvarchar(max) = '0',
@idConcept4 nvarchar(max) = '0',
@idConcept5 nvarchar(max) = '0',
@idConcept6 nvarchar(max) = '0',
@idConcept7 nvarchar(max) = '0',
@idConcept8 nvarchar(max) = '0',
@idConcept9 nvarchar(max) = '0',
@idConcept10 nvarchar(max) = '0',
@idConcept11 nvarchar(max) = '0',
@idConcept12 nvarchar(max) = '0',
@idConcept13 nvarchar(max) = '0',
@idConcept14 nvarchar(max) = '0',
@idConcept15 nvarchar(max) = '0',
@IDReportTask nvarchar(max) = '0'
AS

SET @monthStart = FORMAT(CAST(@monthStart as int), '00');
SET @yearStart = FORMAT(CAST(@yearStart as int), '0000');
SET @monthEnd = FORMAT(CAST(@monthEnd as int), '00');
SET @yearEnd = FORMAT(CAST(@yearEnd as int), '0000');
DECLARE @StartDate date = DATEFROMPARTS(@yearStart,@monthStart,'01');
DECLARE @EndDate date = EOMONTH(DATEFROMPARTS(@yearEnd,@monthEnd,'01'));

select sysroEmployeeGroups.FullGroupName,  sysroEmployeeGroups.idgroup, sysroEmployeeGroups.groupname,IDReportTask,
CASE WHEN @idConcept1 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept =@idconcept1 )
ELSE NULL END as totalvaluegroup1, 
CASE WHEN @idConcept2 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept2 )
ELSE NULL END as totalvaluegroup2, 
CASE WHEN @idConcept3 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept3 )
ELSE NULL END as totalvaluegroup3, 
CASE WHEN @idConcept4 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept4 )
ELSE NULL END as totalvaluegroup4, 
CASE WHEN @idConcept5 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept5 )
ELSE NULL END as totalvaluegroup5, 
CASE WHEN @idConcept6 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept6 )
ELSE NULL END as totalvaluegroup6, 
CASE WHEN @idConcept7 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept7 ) 
ELSE NULL END as totalvaluegroup7, 
CASE WHEN @idConcept8 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept8 )
ELSE NULL END as totalvaluegroup8, 
CASE WHEN @idConcept9 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept9 )
ELSE NULL END as totalvaluegroup9, 
CASE WHEN @idConcept10 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept10 ) 
ELSE NULL END as totalvaluegroup10, 
CASE WHEN @idConcept11 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept11 )
ELSE NULL END as totalvaluegroup11, 
CASE WHEN @idConcept12 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept12 )
ELSE NULL END as totalvaluegroup12, 
CASE WHEN @idConcept13 <> '0' THEN(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept13 )
ELSE NULL END as totalvaluegroup13, 
CASE WHEN @idConcept14 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept14 ) 
ELSE NULL END as totalvaluegroup14, 
CASE WHEN @idConcept15 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept15 ) 
ELSE NULL END as totalvaluegroup15, 
Employees.Name, tmp.idemployee,
CASE WHEN @idConcept1 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idconcept1 ) 
ELSE NULL END as totalvalueemployee1,
CASE WHEN @idConcept2 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idconcept2 ) 
ELSE NULL END as totalvalueemployee2,
CASE WHEN @idConcept3 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idconcept3 ) 
ELSE NULL END as totalvalueemployee3,
CASE WHEN @idConcept4 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idConcept4 ) 
ELSE NULL END as totalvalueemployee4,
CASE WHEN @idConcept5 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idconcept5 ) 
ELSE NULL END as totalvalueemployee5,
CASE WHEN @idConcept6 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idConcept6 ) 
ELSE NULL END as totalvalueemployee6,
CASE WHEN @idConcept7 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idconcept7 )
ELSE NULL END as totalvalueemployee7,
CASE WHEN @idConcept8 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idConcept8 )
ELSE NULL END as totalvalueemployee8,
CASE WHEN @idConcept9 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idconcept9 )
ELSE NULL END as totalvalueemployee9,
CASE WHEN @idConcept10 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idconcept10 ) 
ELSE NULL END as totalvalueemployee10,
CASE WHEN @idConcept11 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idconcept11 ) 
ELSE NULL END as totalvalueemployee11,
CASE WHEN @idConcept12 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idConcept12 )
ELSE NULL END as totalvalueemployee12,
CASE WHEN @idConcept13 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idConcept13 ) 
ELSE NULL END as totalvalueemployee13,
CASE WHEN @idConcept14 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idConcept14 )
ELSE NULL END as totalvalueemployee14,
CASE WHEN @idConcept15 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idConcept15 ) 
ELSE NULL END as totalvalueemployee15,
convert(smalldatetime, convert(char(4),datepart (year,tmp.days)) + '/' + replace(convert(char(2),datepart(month,tmp.days)),' ','')  +  '/01'  ,120) as monthdate,
isnull(sum(shifts.expectedworkinghours),0) as expectedworkinghours,
CASE WHEN @idConcept1 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept =  @idConcept1
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01')))
ELSE NULL END as value1,
CASE WHEN @idConcept2 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept2
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01')))
ELSE NULL END as value2,
CASE WHEN @idConcept3 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept3
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01')))
ELSE NULL END as value3,
CASE WHEN @idConcept4 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept4
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01')))
ELSE NULL END as value4,
CASE WHEN @idConcept5 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept5
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'))) 
ELSE NULL END as value5,
CASE WHEN @idConcept6 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept6
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01')))
ELSE NULL END as value6,
CASE WHEN @idConcept7 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept7
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01')))
ELSE NULL END as value7,
CASE WHEN @idConcept8 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept8
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01')))
ELSE NULL END as value8,
CASE WHEN @idConcept9 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept9
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01')))
ELSE NULL END as value9,
CASE WHEN @idConcept10 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept10
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01')))
ELSE NULL END as value10,
CASE WHEN @idConcept11 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept11
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01')))
ELSE NULL END as value11,
CASE WHEN @idConcept12 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept12
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01')))
ELSE NULL END as value12,
CASE WHEN @idConcept13 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept13
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01')))
ELSE NULL END as value13,
CASE WHEN @idConcept14 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept14
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01')))
ELSE NULL END as value14,
CASE WHEN @idConcept15 <> '0' THEN (select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept15
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01')))
ELSE NULL END as value15,
EmployeeContracts.IDContract, EmployeeContracts.BeginDate, EmployeeContracts.EndDate
from (select idemployee, idconcept, days, Position, IDReportGroup,IDReportTask
from tmpMonthlyEmployeeCalendar WITH (NOLOCK) , sysroreportgroupconcepts WITH (NOLOCK), sysroEmployeeGroups WITH (NOLOCK)
where days between @startdate and @enddate and idconcept in (@idconcept1,@idconcept2,@idconcept3,@idconcept4,@idconcept5,@idconcept6,@idconcept7,@idconcept8,@idconcept9,@idconcept10,@idconcept11,@idconcept12)
    group by idemployee , idconcept, days, Position, IDReportGroup,IDReportTask)tmp
inner join employees WITH (NOLOCK) on tmp.idemployee=employees.id
INNER JOIN sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK)
ON poe.IDPassport = @idPassport
AND poe.IDEmployee = Employees.ID
AND tmp.Days BETWEEN poe.BeginDate AND poe.EndDate
inner join concepts WITH (NOLOCK) on tmp.idconcept = concepts.id
inner join sysroEmployeeGroups WITH (NOLOCK) on sysroEmployeeGroups.IDemployee=tmp.idemployee
AND tmp.Days BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate
left outer join sysroDailyScheduleByContract WITH (NOLOCK) on tmp.days = sysroDailyScheduleByContract.date
and tmp.idemployee = sysroDailyScheduleByContract.idemployee
left join shifts WITH (NOLOCK) on sysroDailyScheduleByContract.idshift1 = shifts.id
               INNER JOIN EmployeeContracts ON sysroDailyScheduleByContract.NumContrato= EmployeeContracts.idContract
where Position > 0 and IDReportTask = @IDREPORTTASK and tmp.IDEmployee in (select * from splitMAX(@IDEmployee,','))
group by  sysroEmployeeGroups.FullGroupName, sysroEmployeeGroups.idgroup, sysroEmployeeGroups.groupname, employees.name,
tmp.idemployee, convert(smalldatetime, convert(char(4),datepart (year,tmp.days)) + '/' + replace(convert(char(2),datepart(month,tmp.days)),' ','')  +  '/01'  ,120), 
IDReportTask, sysroEmployeeGroups.BeginDate, sysroEmployeeGroups.EndDate , EmployeeContracts.IDContract, EmployeeContracts.BeginDate, EmployeeContracts.EndDate,
DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'), EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'))

GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='905' WHERE ID='DBVersion'
GO
