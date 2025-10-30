-- se modifica el tipo del campo para que sea decimal
update sysroUserFields set FieldType=3 where Alias like 'sysroHRCost'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 52)
	INSERT INTO [dbo].[sysroNotificationTypes] ([ID],[Name],[Description],[Scheduler],[Feature],[FeatureType],[OnlySystem])
     VALUES (52,'Document validation action',NULL,360,'Documents','U',0)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notifications] WHERE ID = 751)
	INSERT INTO [dbo].[Notifications] ([ID],[IDType],[Name],[Condition],[Destination],[Schedule],[Activated],[CreatorID],[AllowPortal],[IDPassportDestination],[AllowMail],[ShowOnDesktop],[LastTaskDeleted])
     VALUES (751,52,'Aviso de cambio de estado de documento','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>',NULL,1,'SYSTEM',0,0,0,0,NULL)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 53)
	INSERT INTO [dbo].[sysroNotificationTypes] ([ID],[Name],[Description],[Scheduler],[Feature],[FeatureType],[OnlySystem])
     VALUES (53,'Document Pendig For Employee',NULL,360,'Documents','U',1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notifications] WHERE ID = 702)
	INSERT INTO [dbo].[Notifications] ([ID],[IDType],[Name],[Condition],[Destination],[Schedule],[Activated],[CreatorID],[AllowPortal],[IDPassportDestination],[AllowMail],[ShowOnDesktop],[LastTaskDeleted])
     VALUES (702,53,'Aviso a supervisor de documento próximo a ser requerido','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>',NULL,1,'SYSTEM',0,0,0,0,NULL)
GO


ALTER TABLE dbo.Documents ADD
	SupervisorRemarks nvarchar(MAX) NULL
GO

CREATE TABLE [dbo].[AccessAuthorizationDocumentsTracking](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[IDAuthorization] [int] NOT NULL,
	[IDDocument] [int] NOT NULL
 CONSTRAINT [PK_AccessAuthorizationDocumentsTracking] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Documents] DROP CONSTRAINT [FK_Documnets_Employees] 
GO

CREATE VIEW [dbo].[sysrovwAccessAuthorizationsByCompany]
AS
select distinct dbo.GetCompanyGroup(IDGroup) IDCompany, IDAuthorization from dbo.sysrovwAccessAuthorizations svaa
inner join dbo.EmployeeGroups eg on svaa.IDEmployee = eg.IDEmployee 
GO

CREATE VIEW [dbo].[sysrovwEmployeePRLDocumentaionFaults]
   AS
	select docsauth.BeginValidity DueDate,
		isnull(doc.id,docpending.id) docid, 
		docsauth.documenttemplateid templateid,  
		svaa.IDEmployee idemployee,  
		Null idcompany,  
		svaa.IDAuthorization as authorizationId, 
		docsauth.scope,
		docsauth.AccessValidation,
		agp.IDZone
		from sysrovwAccessAuthorizations svaa
		left join employeecontracts ec on ec.idemployee = svaa.idemployee and DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)) between ec.BeginDate and ec.EndDate                                                                                                                                   
		inner join (select doct.Id documenttemplateid, doct.BeginValidity, doct.EndValidity, aadt.IDAuthorization, doct.LOPDAccessLevel, doct.Area, doct.scope, doct.AccessValidation, doct.ApprovalLevelRequired from DocumentTemplates doct inner join AccessAuthorizationDocumentsTracking aadt on aadt.IDDocument  = doct.id where doct.Scope =5 and doct.compulsory = 1) docsauth on docsauth.IDAuthorization = svaa.IDAuthorization
		Left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = 0 or Status = 1) docpending on docpending.IdDocumentTemplate = docsauth.documenttemplateid and docpending.IdEmployee = svaa.IDEmployee and (docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsauth.ApprovalLevelRequired))
		Left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsauth.documenttemplateid        
			and doc.IdEmployee = svaa.IDEmployee          
			and (doc.Status = 1 and ((docsauth.ApprovalLevelRequired <> 0 and doc.StatusLevel <= docsauth.ApprovalLevelRequired) or docsauth.ApprovalLevelRequired = 0))
			and DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)) between doc.BeginDate and doc.EndDate 
		inner join AccessGroupsPermissions agp on agp.IDAccessGroup = svaa.IDAuthorization
		where doc.RowNumber is NULL 
		and (docpending.RowNumber1 is NULL or docpending.RowNumber1 =1) 
		And (docsauth.BeginValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsauth.BeginValidity <= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))))    
		And (docsauth.EndValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsauth.EndValidity >= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))))    
	UNION
	select docsauth.BeginValidity DueDate,
		isnull(doc.id,docpending.id) docid, 
		docsauth.documenttemplateid templateid,  
		svce.IDEmployee idemployee,  
		svac.IDCompany idcompany,  
		svac.IDAuthorization as authorizationId, 
		docsauth.scope,
		docsauth.AccessValidation,
		agp.IDZone
		from sysrovwAccessAuthorizationsbyCompany svac
		inner join sysrovwCurrentEmployeeGroups svce on dbo.GetCompanyGroup(svce.idgroup) = svac.IDCompany
		inner join sysrovwAccessAuthorizations svaa on svaa.IDEmployee = svce.IDEmployee and svaa.IDAuthorization = svac.IDAuthorization
		inner join (select doct.Id documenttemplateid, doct.BeginValidity, doct.EndValidity, aadt.IDAuthorization, doct.LOPDAccessLevel, doct.Area, doct.scope, doct.AccessValidation, doct.ApprovalLevelRequired from DocumentTemplates doct inner join AccessAuthorizationDocumentsTracking aadt on aadt.IDDocument  = doct.id where doct.Scope =6 and doct.compulsory = 1) docsauth on docsauth.IDAuthorization = svac.IDAuthorization
		Left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = 0 or Status = 1) docpending on docpending.IdDocumentTemplate = docsauth.documenttemplateid and docpending.IdCompany = svac.IDCompany and (docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsauth.ApprovalLevelRequired))
		Left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsauth.documenttemplateid        
			and doc.IdCompany = svac.IDCompany
			and (doc.Status = 1 and ((docsauth.ApprovalLevelRequired <> 0 and doc.StatusLevel <= docsauth.ApprovalLevelRequired) or docsauth.ApprovalLevelRequired = 0))
			and DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)) between doc.BeginDate and doc.EndDate 
		inner join AccessGroupsPermissions agp on agp.IDAccessGroup = svac.IDAuthorization
		where doc.RowNumber is NULL 
		and (docpending.RowNumber1 is NULL or docpending.RowNumber1 =1) 
		And (docsauth.BeginValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsauth.BeginValidity <= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))))    
		And (docsauth.EndValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsauth.EndValidity >= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))))    
GO

ALTER VIEW [dbo].[sysrovwForecastDocumentsFaults]
 AS
 select case WHEN docsabs.daysafterabsencebegin IS NULL  THEN DATEADD(day, docsabs.daysafterabsenceend,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate))) ELSE DATEADD(DAY,docsabs.daysafterabsencebegin ,pa.BeginDate) END DueDate,
 isnull(doc.id,docpending.id) docid, 
 docsabs.documenttemplateid templateid,  
 pa.IDEmployee idemp,  
 Null idcomp,  
 pa.absenceid  as forecastId,  
 docsabs.scope,  
 'days' forecasttype, 
 pa.begindate begindate, 
 isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate)) enddate,  
 NULL starthour,  
 null endhour,  
 pa.idcause idcause 
 from ProgrammedAbsences pa  
 left join employeecontracts ec on ec.idemployee = pa.idemployee and pa.BeginDate between ec.BeginDate and ec.EndDate 
 left join (select doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, adt.every, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope, adt.IDLabAgree, doct.ApprovalLevelRequired from DocumentTemplates doct inner join CausesDocumentsTracking adt on adt.iddocument  = doct.id where ((doct.Scope = 3 or doct.scope = 4) and doct.compulsory = 1)) docsabs on docsabs.idcause = pa.IDCause and (docsabs.IDLabAgree = 0 or docsabs.IDLabAgree = ec.IDLabAgree) 
 Left  join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate, IdDaysAbsence order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = 0 or Status = 1) docpending on docpending.IdDocumentTemplate = docsabs.documenttemplateid and docpending.IdDaysAbsence = pa.AbsenceID and (docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsabs.ApprovalLevelRequired))
 Left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid
         and doc.IdEmployee = pa.IDEmployee          
 		and doc.IdDaysAbsence = pa.absenceid         
		and (doc.Status = 1 and ((doc.StatusLevel <= docsabs.ApprovalLevelRequired and docsabs.ApprovalLevelRequired<>0) or docsabs.ApprovalLevelRequired=0))
 		and ((doc.EndDate >= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) or doc.EndDate >= isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate)))         
 		and (doc.BeginDate <= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)))  
 where doc.RowNumber is NULL 
 and (docpending.RowNumber1 is NULL or docpending.RowNumber1 =1)
 and ((docsabs.daysafterabsencebegin Is Not null And (DATEDIFF(day,pa.BeginDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsencebegin) And (DATEDIFF(day,pa.BeginDate,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate))) >= docsabs.daysafterabsencebegin OR docsabs.every IS NULL)) Or  (docsabs.daysafterabsenceend Is Not null And (DATEDIFF(day,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate)), DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsenceend)))  
 and (docsabs.BeginValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsabs.BeginValidity <= pa.BeginDate))    
 And LOPDAccessLevel In (-1,0,1,2) And (Area In (-1,0,1,2,3,4))
 UNION
 select case WHEN docsabs.daysafterabsencebegin IS NULL  THEN DATEADD(day, docsabs.daysafterabsenceend,pc.FinishDate) ELSE DATEADD(DAY,docsabs.daysafterabsencebegin ,pc.Date) END DueDate,
 	isnull(doc.id,docpending.id) docid, 
 	docsabs.documenttemplateid templateid,  
 	pc.IDEmployee idemp,  
 	Null idcomp,  
 	pc.absenceid  as forecastId,  
 	docsabs.scope,  
 	'hours' forecasttype, 
 	pc.date date, 
 	pc.FinishDate enddate,  
 	pc.begintime starthour,  
 	pc.endtime endhour,  
 	pc.idcause idcause 
 from ProgrammedCauses pc  
 left join employeecontracts ec on ec.idemployee = pc.idemployee and pc.Date between ec.BeginDate and ec.EndDate 
 left join (select doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, adt.every, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope, adt.IDLabAgree, doct.ApprovalLevelRequired from DocumentTemplates doct inner join CausesDocumentsTracking adt on adt.iddocument  = doct.id where ((doct.Scope = 3 or doct.scope = 4) and doct.compulsory = 1)) docsabs on docsabs.idcause = pc.IDCause and (docsabs.IDLabAgree = 0 or docsabs.IDLabAgree = ec.IDLabAgree) 
 left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate, IdHoursAbsence order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = 0 or Status = 1) docpending on docpending.IdDocumentTemplate = docsabs.documenttemplateid and docpending.IdHoursAbsence = pc.AbsenceID and (docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsabs.ApprovalLevelRequired))
 Left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid
         and doc.IdEmployee = pc.IDEmployee
 		and doc.IdHoursAbsence = pc.absenceid
		and (doc.Status = 1 and ((doc.StatusLevel <= docsabs.ApprovalLevelRequired and docsabs.ApprovalLevelRequired<>0) or docsabs.ApprovalLevelRequired=0))
 		and ((doc.EndDate >= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) or doc.EndDate >= pc.FinishDate)
 		and (doc.BeginDate <= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) 
 where doc.RowNumber is NULL 
 and (docpending.RowNumber1 is NULL or docpending.RowNumber1 =1)
 and ((docsabs.daysafterabsencebegin Is Not null And (DATEDIFF(day,pc.Date, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsencebegin OR docsabs.every IS NULL) And (DATEDIFF(day,pc.Date,pc.FinishDate) >= docsabs.daysafterabsencebegin)) Or  (docsabs.daysafterabsenceend Is Not null And (DATEDIFF(day,pc.FinishDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsenceend)))  
 and (docsabs.BeginValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsabs.BeginValidity <= pc.Date))    
 And LOPDAccessLevel In (-1,0,1,2) And (Area In (-1,0,1,2,3,4))
 UNION
 select case WHEN docsabs.daysafterabsencebegin IS NULL  THEN DATEADD(day, docsabs.daysafterabsenceend,po.EndDate) ELSE DATEADD(DAY,docsabs.daysafterabsencebegin ,po.BeginDate) END DueDate,
 	isnull(doc.id,docpending.id) docid, 
 	docsabs.documenttemplateid templateid,  
 	po.IDEmployee idemp,  
 	Null idcomp,  
 	po.id as forecastId,  
 	docsabs.scope,  
 	'overtime' forecasttype, 
 	po.begindate begindate, 
 	po.EndDate enddate,  
 	po.begintime starthour,  
 	po.endtime endhour,  
 	po.idcause idcause 
 from ProgrammedOvertimes po  
 left join employeecontracts ec on ec.idemployee = po.idemployee and po.BeginDate between ec.BeginDate and ec.EndDate 
 Right join (select doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, adt.every, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope, adt.IDLabAgree, doct.ApprovalLevelRequired from DocumentTemplates doct inner join CausesDocumentsTracking adt on adt.iddocument  = doct.id where (doct.scope = 4 and doct.compulsory = 1)) docsabs on docsabs.idcause = po.IDCause and (docsabs.IDLabAgree = 0 or docsabs.IDLabAgree = ec.IDLabAgree) 
 Left  join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate, IdOverTimeForecast order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = 0 or Status = 1) docpending on docpending.IdDocumentTemplate = docsabs.documenttemplateid and docpending.IdOverTimeForecast = po.ID and (docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsabs.ApprovalLevelRequired))
 Left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid
         and doc.IdEmployee = po.IDEmployee          
 		and doc.IdOvertimeForecast = po.id         
		and (doc.Status = 1 and ((doc.StatusLevel <= docsabs.ApprovalLevelRequired and docsabs.ApprovalLevelRequired<>0) or docsabs.ApprovalLevelRequired=0))
 		and ((doc.EndDate >= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) or doc.EndDate >= po.EndDate)         
 		and (doc.BeginDate <= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)))  
 where doc.RowNumber is NULL 
 and (docpending.RowNumber1 is NULL or docpending.RowNumber1 =1)
 and ((docsabs.daysafterabsencebegin Is Not null And (DATEDIFF(day,po.BeginDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsencebegin) And (DATEDIFF(day,po.BeginDate,po.EndDate) >= docsabs.daysafterabsencebegin OR docsabs.every IS NULL)) Or  (docsabs.daysafterabsenceend Is Not null And (DATEDIFF(day,po.EndDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsenceend)))  
 and (docsabs.BeginValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsabs.BeginValidity <= po.BeginDate))    
 And LOPDAccessLevel In (-1,0,1,2) And (Area In (-1,0,1,2,3,4))
GO




UPDATE dbo.sysroParameters SET Data='425' WHERE ID='DBVersion'
GO
