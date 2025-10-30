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
left join EmployeeGroups eg on eg.IDEmployee = emp.ID and d.Date between eg.BeginDate and eg.EndDate
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

DROP PROCEDURE [dbo].[Report_calendario_anual_usuarios_resumen]
GO
CREATE PROCEDURE [dbo].[Report_calendario_anual_usuarios_resumen]
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
select s.Name, s.ShortName, s.Color, COUNT(ISNULL(ds.IDShiftUsed,ds.IDShift1)) as Days, SUM(s.ExpectedWorkingHours) as ExpectedWorkingHours from DailySchedule ds
inner join shifts s on (IDShiftUsed is not null and IDShiftUsed = s.ID) or (IDShift1 = s.ID)
inner join EmployeeGroups as eg on ds.IDEmployee = eg.IDEmployee and ds.Date between eg.BeginDate and eg.EndDate
and ds.Date between eg.BeginDate and eg.EndDate
left join ProgrammedAbsences as pa
on ds.IDEmployee = pa.IDEmployee
and ds.Date between pa.BeginDate and ISNULL(FinishDate,DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate))
where ds.IDEmployee IN (select * from split(@IDEmployee,','))
and Date between @fechaStart and EOMONTH(@fechaEnd)
and eg.IDGroup IN (select * from split(@IDGroup,','))
and pa.AbsenceID is null
GROUP BY s.Name, s.ShortName, s.Color;
RETURN NULL

EXECUTE [dbo].[Report_calendario_anual_usuarios_resumen]
GO

-- Personalización permisos iniciales al crear empleado para CR
 ALTER PROCEDURE [dbo].[WebLogin_Passports_Insert] 
       	(
       		@id int OUTPUT, @idParentPassport int, @groupType varchar(1), @name nvarchar(50), @description nvarchar(100), @email nvarchar(100), @idUser int, @idEmployee int, @idLanguage int,
       		@levelOfAuthority tinyint, @ConfData text, @AuthenticationMerge nvarchar(50), @StartDate smalldatetime, @ExpirationDate smalldatetime, @State smallint, @EnabledVTDesktop bit,
     		@EnabledVTPortal bit, @EnabledVTSupervisor bit, @EnabledVTPortalApp bit , @EnabledVTSupervisorApp bit , @NeedValidationCode bit , @TimeStampValidationCode smalldatetime ,
     		@ValidationCode nvarchar(100), @EnabledVTVisits bit, @EnabledVTVisitsApp bit, @PhotoRequiered bit, @LocationRequiered bit, @LicenseAccepted bit, @IsSupervisor bit
       	)
       AS
 	  
 	DECLARE @Customization nvarchar(max)
 	DECLARE @SecurityMode nvarchar(max)
 	select @Customization = value FROM sysroLiveAdvancedParameters WHERE ParameterName = 'Customization'
 	select @SecurityMode = value FROM sysroLiveAdvancedParameters WHERE ParameterName = 'SecurityMode'
 	if @Customization = 'CRUZROJA'
 	begin
       	SET @PhotoRequiered = 0
 		SET @LocationRequiered = 0
 	END
 	INSERT INTO sysroPassports (
       	IDParentPassport, GroupType, Name, Description, Email, IDUser, IDEmployee, IDLanguage, LevelOfAuthority, ConfData, AuthenticationMerge, StartDate, ExpirationDate, [State],
      	EnabledVTDesktop, EnabledVTPortal, EnabledVTSupervisor, EnabledVTPortalApp, EnabledVTSupervisorApp, NeedValidationCode, TimeStampValidationCode, ValidationCode, 
  		EnabledVTVisits, EnabledVTVisitsApp, PhotoRequiered, LocationRequiered, LicenseAccepted, IsSupervisor
     )
     VALUES (
       	@idParentPassport, @groupType, @name, @description, @email, @idUser, @idEmployee, @idLanguage, @levelOfAuthority, @ConfData, @AuthenticationMerge, @StartDate, @ExpirationDate, @State, 
  		@EnabledVTDesktop, @EnabledVTPortal, @EnabledVTSupervisor, @EnabledVTPortalApp, @EnabledVTSupervisorApp, @NeedValidationCode, @TimeStampValidationCode, @ValidationCode, 
  		@EnabledVTVisits, @EnabledVTVisitsApp, @PhotoRequiered, @LocationRequiered, @LicenseAccepted, @IsSupervisor
     )
 	SELECT @id = @@IDENTITY
    	IF @groupType = 'U' 
      	BEGIN
    		-- 0 / Actualizar los permisos del grupo y sus hijos
      		insert into RequestPassportPermissionsPending Values(0, @id)
      	END
 		IF @idEmployee > 0 and @SecurityMode <> '1'
      	BEGIN
 			INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,20,6) 
 			INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,20000,3)
 			INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,20001,6)
 			INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,20100,6)
 			INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,20110,6)

			if @Customization = 'CRUZROJA'
 			begin
 				INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,20120,6)
				INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,20140,0)
				INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,20130,0)
				INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,20150,0)
				INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21,6)
				INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21200,6)
				INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21000,3)
				INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21100,6)
				INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21140,6)
				INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21150,6)
				INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21160,6)
				INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21170,0)
				INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21110,6)
				INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21130,0)
				INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21120,6)
 			END
 		END
   RETURN



UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO
UPDATE sysroParameters SET Data='598' WHERE ID='DBVersion'
GO

