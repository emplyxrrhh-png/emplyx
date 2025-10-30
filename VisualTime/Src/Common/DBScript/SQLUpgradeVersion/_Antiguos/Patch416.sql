DELETE FROM [dbo].[sysroGUI_Actions] WHERE IDPath ='Remarks' AND IDGUIPath='Portal\ShiftControl\Calendar\Review'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI_Actions] WHERE IDPath ='ReviewConfig' AND IDGUIPath='Portal\ShiftControl\Calendar\Review')
BEGIN
INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('ReviewConfig','Portal\ShiftControl\Calendar\Review','CalendarConfig','Forms\Calendar','U:Calendar.Highlight=Read','ShowCalendarRemarksConfig()','btnTbCalendarConfig2',0,6)
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI_Actions] WHERE IDPath ='PlanificationConfig' AND IDGUIPath='Portal\ShiftControl\Calendar\Planification')
BEGIN
INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('PlanificationConfig','Portal\ShiftControl\Calendar\Planification','CalendarConfig','Forms\Calendar','U:Calendar.Highlight=Read','ShowCalendarRemarksConfig()','btnTbCalendarConfig2',0,5)
END
GO

ALTER TABLE [dbo].[EmployeeTerminalMessages]  DROP CONSTRAINT [PK_EmployeeTerminalMessages]
GO

ALTER TABLE [dbo].[EmployeeTerminalMessages]  ADD CONSTRAINT [PK_EmployeeTerminalMessages] PRIMARY KEY CLUSTERED (
	[ID] ASC
	)
GO

ALTER TABLE dbo.sysroPassports ADD
	LicenseAccepted bit NULL
GO

INSERT INTO [dbo].[sysroGUI]
           ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES
           ('Portal\Security\SDK','GUI.SDK','SDK/ManagePunches.aspx','SDK.png',NULL,'admin','Feature\SDK',NULL,108,NULL,'U:Administration.Security=Read')
GO

UPDATE dbo.sysroPassports SET LicenseAccepted=0 WHERE LicenseAccepted is NULL
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'Causes.AllowDefaultValues')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('Causes.AllowDefaultValues','0')
GO

ALTER TABLE dbo.Causes
	DROP COLUMN AbsenceNotRequiresPermission
GO

ALTER PROCEDURE [dbo].[Analytics_CostCenters]
    	@initialDate smalldatetime,
    	@endDate smalldatetime,
    	@idpassport int,
    	@userFieldsFilter nvarchar(max),
 	@businessCenterFilter nvarchar(max)
     AS
 	DECLARE @businessCenterIDs Table(idBusinessCenter int)
 	insert into @businessCenterIDs select * from dbo.SplitToInt(@businessCenterFilter,',');
      SELECT     CONVERT(varchar(10), dbo.DailyCauses.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
                            + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailyCauses.Date, 120) AS KeyView, dbo.DailyCauses.Date,
                            dbo.Causes.Name AS CauseName, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor,  isnull(dbo.BusinessCenters.ID, 0) as IDCenter, 
    						isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,  
    						isnull(dbo.BusinessCenters.Field4, '') AS Field4, isnull(dbo.BusinessCenters.Field5, '') AS Field5, dbo.DailyCauses.Value AS Value, YEAR(dbo.DailyCauses.Date) AS Año, 
                            (DATEPART(dw, dbo.DailyCauses.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailyCauses.Date) AS WeekOfYear, DATEPART(dy, 
                            dbo.DailyCauses.Date) AS DayOfYear, dbo.DailyCauses.DefaultCenter, dbo.DailyCauses.ManualCenter ,                            
    						dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroCostHour'),DailyCauses.date ) as Cost, 
    						dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPriceHour'), DailyCauses.date) as PVP
    					    
      FROM         dbo.sysroEmployeeGroups with (nolock) 
                            INNER JOIN dbo.Causes with (nolock) 
                            INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause  ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.DailyCauses.Date between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate 
    						INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyCauses.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyCauses.Date <= dbo.EmployeeContracts.EndDate 
    						INNER JOIN dbo.Groups with (nolock) ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID 
    						LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.Employees.ID  
    						LEFT OUTER JOIN dbo.BusinessCenters with (nolock) ON isnull(dbo.DailyCauses.IDCenter,0) = dbo.BusinessCenters.ID 
       where     isnull(dbo.DailyCauses.IDCenter,0) in (select idBusinessCenter from @businessCenterIDs) 
			 and dbo.dailycauses.date between @initialdate and @enddate

GO

ALTER PROCEDURE [dbo].[Analytics_CostCenters_Detail]
   	@initialDate smalldatetime,
   	@endDate smalldatetime,
   	@idpassport int,
	@employeeFilter nvarchar(max),
   	@userFieldsFilter nvarchar(max),
	@businessCenterFilter nvarchar(max)
    AS
	DECLARE @employeeIDs Table(idEmployee int)
	insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter,@initialDate,@endDate;

	DECLARE @businessCenterIDs Table(idBusinessCenter int)
	insert into @businessCenterIDs select * from dbo.SplitToInt(@businessCenterFilter,',');

     SELECT     CONVERT(varchar(10), dbo.DailyCauses.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
                           + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailyCauses.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup, 
                           dbo.EmployeeContracts.IDContract, dbo.DailyCauses.IDEmployee, dbo.DailyCauses.Date,
                           dbo.Causes.Name AS CauseName, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor, isnull(dbo.BusinessCenters.ID, 0) as IDCenter, 
   						isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,  
   						isnull(dbo.BusinessCenters.Field4, '') AS Field4, isnull(dbo.BusinessCenters.Field5, '') AS Field5, dbo.DailyCauses.Value AS Value, YEAR(dbo.DailyCauses.Date) AS Año, 
                           (DATEPART(dw, dbo.DailyCauses.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailyCauses.Date) AS WeekOfYear, DATEPART(dy, 
                           dbo.DailyCauses.Date) AS DayOfYear, dbo.DailyCauses.DefaultCenter, dbo.DailyCauses.ManualCenter , 
                           dbo.Employees.Name AS EmployeeName, dbo.Groups.Name AS GroupName, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName, dbo.Groups.Path, 
                           dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee, 
   						dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroCostHour'),DailyCauses.date ) as Cost, 
   						dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPriceHour'), DailyCauses.date) as PVP,
   					    dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,
   						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
   						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
   						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
   						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
   						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),@endDate) As UserField1,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),@endDate) As UserField2,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),@endDate) As UserField3,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),@endDate) As UserField4,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),@endDate) As UserField5,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),@endDate) As UserField6,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),@endDate) As UserField7,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),@endDate) As UserField8,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),@endDate) As UserField9,
   						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),@endDate) As UserField10
     FROM         dbo.sysroEmployeeGroups with (nolock) 
                           INNER JOIN dbo.Causes with (nolock) 
                           INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause  ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.DailyCauses.Date between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate 
   						INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyCauses.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyCauses.Date <= dbo.EmployeeContracts.EndDate 
   						INNER JOIN dbo.Groups with (nolock) ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID 
   						LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.Employees.ID  
   						LEFT OUTER JOIN dbo.BusinessCenters with (nolock) ON ISNULL(dbo.DailyCauses.IDCenter,0) = dbo.BusinessCenters.ID 
      where     
   			 dbo.dailycauses.date between @initialdate and @enddate
			 and  isnull(dbo.DailyCauses.IDCenter,0) in (select idBusinessCenter from @businessCenterIDs) 
			 AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs)
GO

-- Nuevos tipos de justificaciones (dia, personalizada)
ALTER TABLE dbo.Causes ADD 
DayType [bit] NULL default(0),
CustomType [bit] NULL default(0)
GO

UPDATE dbo.Causes SET DayType=0 WHERE DayType is NULL
GO

UPDATE dbo.Causes SET CustomType=0 WHERE CustomType is NULL
GO


-- Equivalencia automatica en caso de justificaciones de tipo dia
ALTER TABLE dbo.Causes ADD 
AutomaticEquivalenceType [smallint] NULL DEFAULT(0),
AutomaticEquivalenceCriteria nvarchar(max) NULL,
AutomaticEquivalenceIDCause [smallint] NULL DEFAULT(0)
GO

UPDATE dbo.Causes SET AutomaticEquivalenceType=0 WHERE AutomaticEquivalenceType is NULL
GO

UPDATE dbo.Causes SET AutomaticEquivalenceIDCause=0 WHERE AutomaticEquivalenceIDCause is NULL
GO


-- Nuevo tipo de saldo personalizado
ALTER TABLE dbo.Concepts ADD 
CustomType [bit] NULL default(0)
GO

UPDATE dbo.Concepts SET CustomType=0 WHERE CustomType is NULL
GO

-- utilizar el saldo en la gestion de vacaciones y permisos
ALTER TABLE dbo.concepts ADD
	ApplyOnHolidaysRequest bit NULL DEFAULT(0)
GO

UPDATE Concepts SET ApplyOnHolidaysRequest = 0 WHERE ApplyOnHolidaysRequest is null
GO

-- Limite maximo de tiempo a justificar
ALTER TABLE dbo.Causes ADD
	MaxTimeToForecast [numeric](19, 6) NULL default(0)
GO


UPDATE dbo.Causes SET MaxTimeToForecast=0 WHERE MaxTimeToForecast IS NULL
GO


-- Caducidades
ALTER TABLE dbo.Concepts ADD ApplyExpiredHours Bit NULL default(0)
GO

UPDATE dbo.Concepts Set ApplyExpiredHours=0 WHERE ApplyExpiredHours IS NULL
GO

ALTER TABLE dbo.Concepts ADD ExpiredHoursCriteria nvarchar(max) NULL 
GO

ALTER TABLE dbo.Concepts ADD ExpiredIDCause [smallint] NULL default(0)
GO


ALTER TABLE dbo.DailyAccruals ADD PositiveValue [numeric] (19, 6) NULL 
GO

ALTER TABLE dbo.DailyAccruals ADD NegativeValue [numeric] (19, 6) NULL 
GO

ALTER TABLE dbo.DailyAccruals ADD ExpiredDate [smalldatetime] NULL
GO

--Desaparecen los documentos de absentismo
delete dbo.sysroGUI where IDPath = 'Portal\ShiftManagement\AbsencesDocuments'
GO

--Eliminamos datos de GPA
--Si hay documentos los guardo en tabla de backup ...
IF EXISTS (SELECT * FROM AbsenceTracking)
BEGIN
	CREATE TABLE [dbo].[BCK_AbsenceTracking](
		[ID] [numeric](16, 0) NOT NULL,
		[TypeAbsence] [tinyint] NOT NULL,
		[IDEmployee] [int] NOT NULL,
		[IDCause] [smallint] NOT NULL,
		[Date] [smalldatetime] NOT NULL,
		[IDAbsence] [int] NULL,
		[TrackDay] [smalldatetime] NOT NULL,
		[IDDocument] [int] NOT NULL,
		[DeliveryDate] [smalldatetime] NULL,
		[IDPassport] [int] NULL,
		[NotificationDate] [smalldatetime] NULL,
		[NotificationHistory] [nvarchar](max) NULL,
		[AttachmentFile] [image] NULL,
		[Comments] [nvarchar](max) NULL,
		[AttachmentFileName] [nvarchar](50) NULL,
	 CONSTRAINT [PK_BCKAbsenceTracking] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	ALTER TABLE [dbo].[BCK_AbsenceTracking] ADD  DEFAULT ((0)) FOR [IDAbsence]

	INSERT INTO [dbo].[BCK_AbsenceTracking] SELECT * FROM [dbo].[AbsenceTracking]
	DELETE [dbo].[AbsenceTracking]
END
GO
--Borro asignación de documentos a justificaciones
delete dbo.CausesDocuments
GO
--Borro definición de documentos de absentismo
delete dbo.DocumentsAbsences
GO

--En versión posterior a 3.5 entró documentación de absentismo deshabilitado. En la siguiente, se debe actualizar incondicionalmente (se decidió que con Doc de GPA no se podía actualizar de versión de VT) 
UPDATE dbo.sysroLiveAdvancedParameters SET Value = '0' WHERE ParameterName = 'GPADocumentation.LegacyMode'
GO

IF OBJECT_ID('dbo.AbsenceDocumentsTracking', 'U') IS NOT NULL 
	EXEC sp_rename 'dbo.AbsenceDocumentsTracking', 'CausesDocumentsTracking';
GO

if exists(select 1 from sys.views where name='sysrovwLeaveOrPermissionDocumentsFaults' and type='v')
DROP VIEW [dbo].[sysrovwLeaveOrPermissionDocumentsFaults];
GO

 CREATE VIEW [dbo].[sysrovwLeaveOrPermissionDocumentsFaults]
 AS
select case WHEN docsabs.daysafterabsencebegin IS NULL  THEN DATEADD(day, docsabs.daysafterabsenceend,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate))) ELSE DATEADD(DAY,docsabs.daysafterabsencebegin ,pa.BeginDate) END DueDate,
       doc.id docid, 
	   docsabs.documenttemplateid templateid, 
	   pa.IDEmployee idemp, 
	   Null idcomp, 
	   pa.absenceid, 
	   docsabs.scope, 
	   'days' absencetype, 
	   pa.begindate begindate, 
	   isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate)) enddate, 
	   NULL starthour, 
	   null endhour, 
	   pa.idcause idcause 
from ProgrammedAbsences pa 
left outer join employeecontracts ec on ec.idemployee = pa.idemployee and pa.BeginDate between ec.BeginDate and ec.EndDate
Right join (select doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope, adt.IDLabAgree from DocumentTemplates doct inner join CausesDocumentsTracking adt on adt.iddocument  = doct.id where (doct.Scope = 3 and doct.compulsory = 1)) docsabs on docsabs.idcause = pa.IDCause and (docsabs.IDLabAgree = 0 or docsabs.IDLabAgree = ec.IDLabAgree)
Left outer join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid 
     And doc.IdEmployee = pa.IDEmployee  
	 and doc.IdDaysAbsence = pa.absenceid 
	 and doc.Status not in (0,3,4) -- Estados incorrectos 
	 and ((doc.EndDate >= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) or doc.EndDate >= isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate))) --Estado correcto o expirado, pero caduca, siempre que sea dentro de la ausencia
	 and (doc.BeginDate <= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) -- Estado correcto pero todavía no es vigente
where  ((docsabs.daysafterabsencebegin is not null and (DATEDIFF(day,pa.BeginDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsencebegin) and (DATEDIFF(day,pa.BeginDate,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate))) >= docsabs.daysafterabsencebegin)) or  (docsabs.daysafterabsenceend is not null and (DATEDIFF(day,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate)), DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsenceend))) 
and (docsabs.BeginValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsabs.BeginValidity <= pa.BeginDate))  
and doc.RowNumber is NULL
And LOPDAccessLevel in (-1,0,1,2) And (Area in (-1,0,1,2,3,4))
GO

if exists(select 1 from sys.views where name='sysrovwAbsenteeismDocumentsFaults' and type='v')
DROP VIEW [dbo].[sysrovwAbsenteeismDocumentsFaults];
GO

CREATE VIEW [dbo].[sysrovwAbsenteeismDocumentsFaults]
 AS
 SELECT * FROM sysrovwLeaveOrPermissionDocumentsFaults
GO

if exists(select 1 from sys.views where name='sysrovwAbsencesEx' and type='v')
DROP VIEW [dbo].[sysrovwAbsencesEx];
GO
 
 CREATE VIEW [dbo].[sysrovwAbsencesEx]
 AS
 SELECT row_number() OVER(order by x.BeginDate) AS ID, x.* ,emp.Name as EmployeeName, cause.Name as CauseName, grp.IDGroup, grp.FullGroupName,
         grp.Path, grp.CurrentEmployee, grp.BeginDate AS BeginDateMobility, grp.EndDate AS EndDateMobility
  FROM (
  (SELECT 	AbsenceID, NULL AS IDRelatedObject,
  			IDCause AS IDCause, 
  			IDEmployee AS IDEmployee, 
  			BeginDate AS BeginDate, 
  			ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) AS FinishDate, 
  			NULL AS BeginTime, 
  			NULL AS EndTime, 
  			CONVERT(NVARCHAR(4000),Description) AS Description ,
			(SELECT CASE WHEN (select COUNT(*) from sysrovwAbsenteeismDocumentsFaults adf where adf.AbsenceID = pa.AbsenceID) > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered, 
  			(SELECT CASE WHEN GETDATE() between BeginDate and DATEADD(DAY,1,(ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )))) THEN '1' WHEN ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) < GETDATE()  THEN '2'  ELSE '0' END) AS Status,
  			'ProgrammedAbsence' AS AbsenceType 
   FROM dbo.ProgrammedAbsences pa)
  union
  (SELECT   AbsenceID, ID AS IDRelatedObject,
  			IDCause AS IDCause, 
  			IDEmployee AS IDEmployee, 
  			Date AS BeginDate, 
  			ISNULL(FinishDate,Date) AS FinishDate, 
  			BeginTime AS BeginTime, 
  			EndTime AS EndTime, 
  			CONVERT(NVARCHAR(4000),Description) AS Description, 
			1  AS DocumentsDelivered, 
  			(SELECT CASE WHEN GETDATE() between Date and DATEADD(DAY,1,(ISNULL(FinishDate, Date))) THEN '1' WHEN  DATEADD(DAY,1,ISNULL(FinishDate,Date)) < GETDATE()  THEN '2'  ELSE '0' END) AS Status,
  			'ProgrammedCause' AS AbsenceType 
   FROM dbo.ProgrammedCauses pc)
  union
  (SELECT   NULL as AbsenceID, ID AS IDRelatedObject,
  			IDCause AS IDCause, 
  			IDEmployee AS IDEmployee, 
  			Date1 AS BeginDate, 
  			ISNULL(Date2,Date1) AS FinishDate, 
  			FromTime AS BeginTime, 
  			ToTime AS EndTime, 
  			CONVERT(NVARCHAR(4000),Comments) AS Description, 
  			1 AS DocumentsDelivered,
  			(SELECT CASE WHEN Status = 0 THEN '-1' ELSE '-2' END) AS Status,
  			(SELECT CASE WHEN RequestType = 7 THEN 'RequestAbsence' ELSE 'RequestCause' END) AS AbsenceType 
   FROM dbo.Requests where RequestType in(7,9) and Status in(0,1))
   ) x
   inner join dbo.Employees as emp on x.IDEmployee = emp.ID
   inner join dbo.Causes as cause on x.IDCause = cause.ID
   inner join dbo.sysrovwCurrentEmployeeGroups as grp on x.IDEmployee = grp.IDEmployee

GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI] WHERE IDPath = 'Portal\GeneralManagement\Documents')
	INSERT INTO [dbo].[sysroGUI] ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
		VALUES ('Portal\GeneralManagement\Documents','Documents','Documents/DocumentTemplate.aspx','Documents.png',NULL,NULL,'Feature\Documents',NULL,106,NULL,'U:Documents.DocumentsDefinition=Read')
	ELSE UPDATE [dbo].[sysroGUI] SET [RequiredFeatures] = 'Feature\Absences' WHERE [IDPath] = 'Portal\GeneralManagement\Documents'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].sysroGUI_Actions WHERE IDPath = 'NewDocument' AND IDGUIPath = 'Portal\GeneralManagement\Documents\Template')
	INSERT INTO sysroGUI_Actions ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
		VALUES ('NewDocument', 'Portal\GeneralManagement\Documents\Template','tbAddNewDocument','Feature\Documents',NULL,'newDocumentTemplate()','btnTbAddTaskTemplate',0,1)
	ELSE UPDATE [dbo].[sysroGUI_Actions] SET [RequieredFeatures] = 'Feature\Absences' WHERE [IDGUIPath] = 'Portal\GeneralManagement\Documents\Template' AND [IDPath] = 'NewDocument'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].sysroGUI_Actions WHERE IDPath = 'Del' AND IDGUIPath = 'Portal\GeneralManagement\Documents\Template')
	INSERT INTO [dbo].sysroGUI_Actions ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
		VALUES ('Del', 'Portal\GeneralManagement\Documents\Template','tbDelDocument','Feature\Documents',NULL,'ShowRemoveDocumentTemplate(''#ID#'')','btnTbDel2',0,2)
	ELSE UPDATE [dbo].[sysroGUI_Actions] SET [RequieredFeatures] = 'Feature\Absences' WHERE [IDGUIPath] = 'Portal\GeneralManagement\Documents\Template' AND [IDPath] = 'Del'
GO


ALTER TABLE dbo.DocumentTemplates ADD LeaveDocType tinyint null
GO

update dbo.sysroGUI set IDPath ='Portal\General\SecurityChart', Priority=101 where IDPath='Portal\Security\SecurityChart'
GO
update dbo.sysroGUI set Priority=102 where IDPath='Portal\General\Employees'
GO
update dbo.sysroGUI set Priority=103 where IDPath='Portal\General\DataLink'
GO
update dbo.sysroGUI set Priority=104 where IDPath='Portal\General\Requests'
GO
update dbo.sysroGUI set Priority=105 where IDPath='Portal\General\Alerts'
GO


UPDATE dbo.sysroParameters SET Data='416' WHERE ID='DBVersion'
GO
