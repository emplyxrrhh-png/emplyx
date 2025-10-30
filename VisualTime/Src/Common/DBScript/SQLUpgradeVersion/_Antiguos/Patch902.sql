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
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept =@idconcept1 ) as totalvaluegroup1, 
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept2 ) as totalvaluegroup2, 
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept3 ) as totalvaluegroup3, 
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept4 ) as totalvaluegroup4, 
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept5 ) as totalvaluegroup5, 
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept6 ) as totalvaluegroup6, 
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept7 ) as totalvaluegroup7, 
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept8 ) as totalvaluegroup8, 
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept9 ) as totalvaluegroup9, 
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept10 ) as totalvaluegroup10, 
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept11 ) as totalvaluegroup11, 
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept12 ) as totalvaluegroup12, 
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept13 ) as totalvaluegroup13, 
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept14 ) as totalvaluegroup14, 
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startDate and @endDate and idemployee in (select id from sysroEmployeeGroups as vw WITH (NOLOCK) inner join employees WITH (NOLOCK) on vw.idemployee = employees.id  where vw.idgroup = sysroEmployeeGroups.idgroup and vw.IDEmployee in (select * from splitMAX(@IDEmployee,',')) AND Date BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate) 
and idconcept = @idconcept15 ) as totalvaluegroup15, 
Employees.Name, tmp.idemployee,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idconcept1 ) as totalvalueemployee1,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idconcept2 ) as totalvalueemployee2,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idconcept3 ) as totalvalueemployee3,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idConcept4 ) as totalvalueemployee4,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idconcept5 ) as totalvalueemployee5,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idConcept6 ) as totalvalueemployee6,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idconcept7 ) as totalvalueemployee7,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idConcept8 ) as totalvalueemployee8,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idconcept9 ) as totalvalueemployee9,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idconcept10 ) as totalvalueemployee10,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idconcept11 ) as totalvalueemployee11,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idConcept12 ) as totalvalueemployee12,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idConcept13 ) as totalvalueemployee13,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idConcept14 ) as totalvalueemployee14,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract WITH (NOLOCK) where date between @startdate and @enddate
and idemployee = tmp.idemployee and idconcept = @idConcept15 ) as totalvalueemployee15,
convert(smalldatetime, convert(char(4),datepart (year,tmp.days)) + '/' + replace(convert(char(2),datepart(month,tmp.days)),' ','')  +  '/01'  ,120) as monthdate,
isnull(sum(shifts.expectedworkinghours),0) as expectedworkinghours,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept =  @idConcept1
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'))) as value1,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept2
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'))) as value2,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept3
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'))) as value3,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept4
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'))) as value4,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept5
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'))) as value5,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept6
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'))) as value6,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept7
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'))) as value7,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept8
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'))) as value8,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept9
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'))) as value9,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept10
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'))) as value10,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept11
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'))) as value11,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept12
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'))) as value12,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept13
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'))) as value13,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept14
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'))) as value14,
(select isnull(sum(value),0) from sysroDailyAccrualsByContract where sysroDailyAccrualsByContract.idconcept = @idConcept15
and tmp.idemployee = sysroDailyAccrualsByContract.idemployee and date between DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01') and EOMONTH(DATEFROMPARTS(datepart (year,tmp.days),datepart (month,tmp.days),'01'))) as value15,
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

UPDATE sysroParameters SET Data='902' WHERE ID='DBVersion'
GO
