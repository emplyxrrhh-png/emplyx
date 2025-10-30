ALTER FUNCTION [dbo].[GetDirectSupervisorByRequest]
 (	
     @idEmployee int,
     @featureAlias nvarchar(100),
     @employeeFeatureId int,
 	@RequestType int
 )
 RETURNS nvarchar(1000)
 AS
 BEGIN
 DECLARE @RetNames nvarchar(1000);
 SET @RetNames = ''
 DECLARE @tmpTable Table(PassportID int,LevelOfAuthority int ,Name nvarchar(50))
 	-- si es seguridad v3
 	begin
 	insert into @tmpTable 
 	SELECT sysroPassports.id AS PassportID, dbo.GetPassportLevelOfAuthority(sysroPassports.ID,@RequestType,0,3) AS LevelOfAuthority, sysroPassports.Name
      		FROM sysroPassports  
      		WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null AND 
      				dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@employeeFeatureID,2,1,getdate()) > 3 AND 
   				dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID, @featureAlias,'U',2) > 3 AND Description not like '@@ROBOTICS@@%'
 	SET @RetNames = (SELECT  CONVERT(nvarchar(4000),PassportID ) + ','  AS [text()] from @tmpTable tt where  tt.LevelOfAuthority = (SELECT max(LevelOfAuthority) from @tmpTable where LevelOfAuthority <> 15) For XML PATH (''))
 	IF @RetNames <> ''
 		BEGIN
      		SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
 		END
 	end
 RETURN @RetNames
 END
GO

 ALTER VIEW [dbo].[sysrovwDashboardEmployeeStatus]
   AS
  	SELECT svEmployees.IdEmployee, 
 	svEmployees.EmployeeName,
 	Shifts.Name ShiftName,
 	tmpLastPunch.Type as LastPunchType,
 	tmpLastPunch.ShiftDate as LastPunchDate,
 	tmpLastPunch.DateTime as LastPunchDateTime,
 	tmpLastPunch.TypeData as LastPunchTypeData,
 	Employees.Image EmployeeImage,
  	tmpAttendance.ShiftDate LastAttendanceDate, 
  	tmpAttendance.DateTime as LastAttendancePunchDatetime,
 	ISNULL(Causes.Name, '') as LastAttendanceCause,
  	CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN 'Out-Calculated' ELSE CASE tmpAttendance.ActualType WHEN 1 THEN 'In' ELSE 'Out' END END as AttendanceStatus, 
  	tmpTasks.DateTime as LastTaskPunchDatetime, 
     ISNULL(Tasks.Project + ' : ' +  Tasks.Name,'') as LastTaskName,  
  	tmpcosts.DateTime as LastCostPunchDatetime, 
 	CASE WHEN BusinessCentersPunch.Name IS NOT NULL THEN BusinessCentersPunch.Name ELSE (CASE WHEN BusinessCentersShifts.Name IS NOT NULL THEN BusinessCentersShifts.Name ELSE [dbo].[GetEmployeeBusinessCenterNameOnDate](svEmployees.IDEmployee,GETDATE()) END) END AS CostCenterName,
 	CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN '' ELSE tmpLastPunch.LocationZone END As LocationName,
 	CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN '' ELSE Zones.Name END As ZoneName,
 	CASE Shifts.ShiftType WHEN 2 THEN 1 ELSE 0 END AS InHolidays,
 	CASE WHEN ProgrammedAbsences.IDCause > 0 THEN 1 ELSE 0 END AS InAbsence,
 	CASE WHEN ProgrammedAbsences.IDCause > 0 THEN Causes1.Name ELSE '' END AS InAbsenceCause,
 	CASE WHEN tmpHourAbsences.IDCause > 0 THEN 1 ELSE 0 END AS InHourAbsence,
 	CASE WHEN tmpHourAbsences.IDCause > 0 THEN Causes3.Name ELSE '' END AS InHourAbsenceCause,
 	CASE WHEN tmpHoursHolidays.IDEmployee IS NOT NULL THEN 1 ELSE 0 END AS InHoursHolidays,
 	CASE WHEN Requests.IDCause > 0 THEN 1 ELSE 0 END AS AbsenceRequested,
 	CASE WHEN Requests.IDCause > 0 THEN Causes2.Name ELSE '' END AS AbsenceRequestCause,
 	CASE WHEN tmpHourAbsenceRequest.IDCause > 0 THEN 1 ELSE 0 END AS HoursAbsenceRequested,
 	CASE WHEN tmpHourAbsenceRequest.IDCause > 0 THEN Causes2.Name ELSE '' END AS HoursAbsenceRequestCause
  	FROM sysrovwcurrentemployeegroups svEmployees WITH (NOLOCK)
 	INNER JOIN Employees WITH (NOLOCK) ON Employees.Id = svEmployees.IDEmployee
	LEFT JOIN DailySchedule WITH (NOLOCK) ON DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) = DailySchedule.Date AND svEmployees.IdEmployee = DailySchedule.IDEmployee
 	LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberPunch', * FROM Punches WITH (NOLOCK) WHERE DATEDIFF(day,ShiftDate, GETDATE()) < 30  ) tmpLastPunch ON svEmployees.IDEmployee = tmpLastPunch.IdEmployee
 	LEFT JOIN Zones ON tmpLastPunch.IDZone = ZOnes.ID
  	LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberAtt', * FROM Punches WITH (NOLOCK) WHERE (ActualType = 1 or ActualType = 2) AND DATEDIFF(day,ShiftDate, GETDATE()) < 30 ) tmpAttendance ON svEmployees.IDEmployee = tmpAttendance.IdEmployee
 	LEFT JOIN Causes ON tmpAttendance.TypeData = Causes.ID AND Causes.ID >0
  	LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberTsk', * FROM Punches WITH (NOLOCK) WHERE ActualType = 4 AND DATEDIFF(day,ShiftDate, GETDATE()) < 30) tmpTasks ON svEmployees.IDEmployee = tmpTasks.IdEmployee
  	LEFT JOIN Tasks ON Tasks.Id = tmpTasks.TypeData
 	LEFT JOIN sysroLiveAdvancedParameters ON (ParameterName = 'LastPunchCenterEnabled' and convert(nvarchar,Value) = 1) 
  	LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberCost', * FROM Punches WITH (NOLOCK) WHERE ActualType = 13 AND DATEDIFF(day,ShiftDate, GETDATE()) < 30) tmpCosts ON svEmployees.IDEmployee = tmpCosts.IdEmployee AND (sysroLiveAdvancedParameters.Value = 1 OR tmpCosts.ShiftDate = DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))
  	LEFT JOIN BusinessCenters BusinessCentersPunch WITH (NOLOCK) ON tmpCosts.TypeData = BusinessCentersPunch.ID 	
 	LEFT JOIN Shifts WITH (NOLOCK) ON Shifts.Id = DailySchedule.IDShift1
 	LEFT JOIN BusinessCenters BusinessCentersShifts WITH (NOLOCK) ON Shifts.IDCenter = BusinessCentersShifts.Id
 	LEFT JOIN ProgrammedAbsences WITH (NOLOCK) ON ProgrammedAbsences.IDEmployee = svEmployees.IdEmployee AND  DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) BETWEEN ProgrammedAbsences.BeginDate AND ISNULL(ProgrammedAbsences.FinishDate,DATEADD(day,ProgrammedAbsences.MaxLastingDays-1,ProgrammedAbsences.BeginDate))
 	LEFT JOIN Causes as Causes1 WITH (NOLOCK) ON Causes1.ID = ProgrammedAbsences.IDCause
 	LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Date ORDER BY idemployee ASC, Date DESC) AS 'RowNumberHourHol', * FROM ProgrammedHolidays WITH (NOLOCK)) tmpHoursHolidays ON tmpHoursHolidays.Date = DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) and tmpHoursHolidays.IDEmployee = svEmployees.IdEmployee
 	LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Date ORDER BY idemployee ASC, Date DESC) AS 'RowNumberHourAbsence', * FROM ProgrammedCauses WITH (NOLOCK)) tmpHourAbsences ON  tmpHourAbsences.IDEmployee = svEmployees.IdEmployee AND DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) BETWEEN tmpHourAbsences.Date AND tmpHourAbsences.FinishDate
 	LEFT JOIN Causes as Causes3 WITH (NOLOCK) ON Causes3.ID = tmpHourAbsences.IDCause
 	LEFT JOIN Requests WITH (NOLOCK) ON Requests.IDEmployee = svEmployees.IdEmployee AND Requests.RequestType = 7 AND DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) BETWEEN Requests.Date1 AND Requests.Date2 AND Requests.Status IN (0,1)
 	LEFT JOIN Causes as Causes2 WITH (NOLOCK) ON Causes2.ID = Requests.IDCause
 	LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Date1, Date2 ORDER BY idemployee ASC, RequestDate DESC) AS 'RowHourAbsenceRequest', * FROM Requests WITH (NOLOCK) WHERE RequestType = 9 AND Status IN (0,1)) tmpHourAbsenceRequest ON tmpHourAbsenceRequest.IDEmployee = svEmployees.IdEmployee  AND DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) BETWEEN tmpHourAbsenceRequest.Date1 AND tmpHourAbsenceRequest.Date2 
 	LEFT JOIN Causes as Causes4 WITH (NOLOCK) ON Causes4.ID = tmpHourAbsenceRequest.IDCause
 	WHERE CurrentEmployee = 1
  	AND (RowNumberAtt = 1 OR RowNumberAtt IS NULL)
 	AND (RowNumberPunch = 1 OR RowNumberPunch IS NULL)
  	AND (RowNumberTsk = 1 OR RowNumberTsk IS NULL)
  	AND (RowNumberCost = 1 OR RowNumberCost IS NULL)
 	AND (RowNumberHourHol = 1 OR RowNumberHourHol IS NULL)
     AND (RowNumberHourAbsence = 1 OR RowNumberHourAbsence IS NULL)
 	AND (RowHourAbsenceRequest = 1 OR RowHourAbsenceRequest IS NULL)
GO

-- Índice propuesto por Merck
CREATE INDEX [IX_Groups_CloseDate] ON [dbo].[Groups] ([CloseDate])
GO

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
		END
  RETURN
GO

MERGE dbo.EmployeeUserFieldValues AS euf 
USING(SELECT * FROM dbo.EmployeeUserFieldValues where FieldName IN(select FieldName from sysroUserFields where FieldType=2) and SUBSTRING(Value,3,1) = '/') AS uf   
ON euf.IDEmployee = uf.IDEmployee and euf.FieldName = uf.FieldName and euf.Date = uf.Date
WHEN MATCHED THEN
UPDATE SET euf.Value = CONCAT(SUBSTRING(uf.value, 7, 4), '/',SUBSTRING(uf.value, 4, 2), '/',SUBSTRING(uf.value, 1, 2));
GO

update dbo.EmployeeUserFieldValues set date = '19000102' where FieldName = (select FieldName from sysroUserFields where Alias = 'sysroImportKey' ) and Date = '19000101'
GO

update dbo.sysroUserFields set History = 0 where Alias = 'sysroImportKey'
GO

MERGE dbo.EmployeeUserFieldValues AS euf 
USING(select *
from (
  select s.IDEmployee, 
		 s.FieldName,
		 s.Date,
         max(s.Date) over (partition by s.IDEmployee) as max_date
  from dbo.EmployeeUserFieldValues s
  where  FieldName = (select FieldName from sysroUserFields where Alias = 'sysroImportKey' )
) t
where Date = max_date) AS uf   
ON euf.IDEmployee = uf.IDEmployee and euf.FieldName = uf.FieldName and euf.Date = uf.Date
WHEN MATCHED THEN
UPDATE SET euf.Date = '19000101';
GO

delete from dbo.EmployeeUserFieldValues where FieldName = (select FieldName from sysroUserFields where Alias = 'sysroImportKey' ) and Date <> '19000101'
GO

ALTER TABLE [dbo].[Communiques]
ADD  PlanificationDate smalldatetime NULL
GO

--Gestión del menú para Gestión Documental
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI] WHERE [URL] = 'Diagnostics/Status.aspx')
    insert into [dbo].sysroGUI(IDPath,LanguageReference,URL,IconURL,Type,Parameters,RequiredFeatures,SecurityFlags,Priority,AllowedSecurity,RequiredFunctionalities)
         values ('Portal\Security\Diagnostics','GUI.Diagnostics','Diagnostics/Status.aspx','Options.png',NULL,'RoboticsEmployee','Forms\Passports',NULL,108,NULL,'U:Administration.Security=Read')
GO

CREATE FUNCTION dbo.TryConvertDate
(
  @value nvarchar(4000)
)
RETURNS date
AS
BEGIN
  RETURN (SELECT CONVERT(date, CASE 
    WHEN ISDATE(@value) = 1 THEN @value END)
  );
END
GO


ALTER FUNCTION [dbo].[GetEmployeeAge](
     @idEmployee int    
 )    
 returns int    
 as    
 begin    
 declare @iMonthDayDob int    
 declare @iMonthDayPassedDate int    
 declare @PassedDate datetime = GETDATE()    
 declare @empDate datetime    
 select @empDate = dbo.TryConvertDate(convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(@idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroBirthdate'),@PassedDate)))    
 select @iMonthDayDob = CAST(datepart (mm,@empDate) * 100 + datepart  (dd,@empDate) AS int)     
 select @iMonthDayPassedDate = CAST(datepart (mm,@PassedDate) * 100 + datepart  (dd,@PassedDate) AS int)     
 return DateDiff(yy,@empDate, @PassedDate)     
 - CASE WHEN @iMonthDayDob <= @iMonthDayPassedDate    
   THEN 0     
   ELSE 1    
   END  
    
 End
GO

UPDATE sysroParameters SET Data='520' WHERE ID='DBVersion'
GO


 