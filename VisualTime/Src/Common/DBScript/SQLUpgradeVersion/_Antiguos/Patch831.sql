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

UPDATE sysroParameters SET Data='831' WHERE ID='DBVersion'
GO
