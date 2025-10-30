ALTER PROCEDURE [dbo].[Report_calendario_anual_usuarios_resumen]
@monthStart nvarchar(2) = '1',
@yearStart nvarchar(4) = '2021',
@monthEnd nvarchar(2) = '04',
@yearEnd nvarchar(4) = '2021',
@IDEmployee nvarchar(max) = '777',
@IDGroup nvarchar(9) = '364'
AS
SET @monthStart = FORMAT(CAST(@monthStart as int), '00');
SET @yearStart = FORMAT(CAST(@yearStart as int), '0000');
SET @monthEnd = FORMAT(CAST(@monthEnd as int), '00');
SET @yearEnd = FORMAT(CAST(@yearEnd as int), '0000');
DECLARE @fechaStart date = DATEFROMPARTS(@yearStart,@monthStart,'01');
declare @fechaEnd datetime = EOMONTH(DATEFROMPARTS(@yearEnd,@monthEnd,'01'));
select ISNULL(ds.ShiftName1,s.Name) AS Name, s.ShortName, ISNULL(ds.ShiftColor1,s.Color) AS Color, COUNT(ISNULL(ds.IDShiftUsed,ds.IDShift1)) as Days, SUM(ISNULL(ds.ExpectedWorkingHours,s.ExpectedWorkingHours)) as ExpectedWorkingHours, IIF(ISNULL(pa.AbsenceID,0)>0,1,0) as IsAbsence from DailySchedule ds
inner join shifts s on (IDShiftUsed is not null and IDShiftUsed = s.ID) or (IDShift1 = s.ID)
inner join EmployeeGroups as eg on ds.IDEmployee = eg.IDEmployee and ds.Date between eg.BeginDate and eg.EndDate
and ds.Date between eg.BeginDate and eg.EndDate
left join ProgrammedAbsences as pa
on ds.IDEmployee = pa.IDEmployee
and ds.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
where ds.IDEmployee IN (select * from split(@IDEmployee,','))
and Date between @fechaStart and EOMONTH(@fechaEnd)
and eg.IDGroup IN (select * from split(@IDGroup,','))
GROUP BY ISNULL(ds.ShiftName1,s.Name), s.ShortName, ISNULL(ds.ShiftColor1,s.Color), ISNULL(pa.AbsenceID,0);
RETURN NULL

EXECUTE [dbo].[Report_calendario_anual_usuarios_resumen]
GO


-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='651' WHERE ID='DBVersion'
GO