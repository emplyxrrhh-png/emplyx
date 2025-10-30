update dbo.sysroGUI set Parameters = 'SaaSEnabled', RequiredFeatures = '' where IDPath='Portal\GeneralManagement\Communiques'
GO

IF not exists (select * from [dbo].[sysroLiveAdvancedParameters] where ParameterName = 'VTLive.SaaSEnabled')
	insert into [dbo].[sysroLiveAdvancedParameters] values ('VTLive.SaaSEnabled', 'false')
GO

INSERT INTO sysroLiveAdvancedParameters VALUES ('CTaimaApiVersion', '1'), 
											   ('CTaimaSyncPunchesEnabled', 'True'),
											   ('CTaimaTenantId', ''),
											   ('CTaimaOcmApimSubscriptionKey', ''),
											   ('BroadcasterUseWindowsRegistry', 'True'),
                                               ('BroadcasterUseDatabaseMode', 'False')

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
   left join (select doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, adt.every, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope, doct.compulsory, adt.IDLabAgree, doct.ApprovalLevelRequired from DocumentTemplates doct inner join CausesDocumentsTracking adt on adt.iddocument  = doct.id where ((doct.Scope = 3 or doct.scope = 4))) docsabs on docsabs.idcause = pa.IDCause and (docsabs.IDLabAgree = 0 or docsabs.IDLabAgree = ec.IDLabAgree) 
   Left  join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate, IdDaysAbsence order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = 0 or Status = 1) docpending on docpending.IdDocumentTemplate = docsabs.documenttemplateid and docpending.IdDaysAbsence = pa.AbsenceID and (docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsabs.ApprovalLevelRequired))
   Left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid
           and doc.IdEmployee = pa.IDEmployee          
   		and doc.IdDaysAbsence = pa.absenceid         
  		and (doc.Status = 1 and ((doc.StatusLevel <= docsabs.ApprovalLevelRequired and docsabs.ApprovalLevelRequired<>0) or docsabs.ApprovalLevelRequired=0))
   		and ((doc.EndDate >= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) or doc.EndDate >= isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate)))         
   		and (doc.BeginDate <= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)))  
   where doc.RowNumber is NULL 
   --and (docpending.RowNumber1 is NULL or docpending.RowNumber1 =1)
   and ((docpending.RowNumber1 is NULL and docsabs.Compulsory = 1) or docpending.RowNumber1 =1 ) 
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
   left join (select doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, adt.every, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope, doct.compulsory, adt.IDLabAgree, doct.ApprovalLevelRequired from DocumentTemplates doct inner join CausesDocumentsTracking adt on adt.iddocument  = doct.id where ((doct.Scope = 3 or doct.scope = 4))) docsabs on docsabs.idcause = pc.IDCause and (docsabs.IDLabAgree = 0 or docsabs.IDLabAgree = ec.IDLabAgree) 
   left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate, IdHoursAbsence order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = 0 or Status = 1) docpending on docpending.IdDocumentTemplate = docsabs.documenttemplateid and docpending.IdHoursAbsence = pc.AbsenceID and (docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsabs.ApprovalLevelRequired))
   Left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid
           and doc.IdEmployee = pc.IDEmployee
   		and doc.IdHoursAbsence = pc.absenceid
  		and (doc.Status = 1 and ((doc.StatusLevel <= docsabs.ApprovalLevelRequired and docsabs.ApprovalLevelRequired<>0) or docsabs.ApprovalLevelRequired=0))
   		and ((doc.EndDate >= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) or doc.EndDate >= pc.FinishDate)
   		and (doc.BeginDate <= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) 
   where doc.RowNumber is NULL 
   --and (docpending.RowNumber1 is NULL or docpending.RowNumber1 =1)
   and ((docpending.RowNumber1 is NULL and docsabs.Compulsory = 1) or docpending.RowNumber1 =1 ) 
   and ((docsabs.daysafterabsencebegin Is Not null And (DATEDIFF(day,pc.Date, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsencebegin) And (DATEDIFF(day,pc.Date,pc.FinishDate) >= docsabs.daysafterabsencebegin OR docsabs.every IS NULL)) Or  (docsabs.daysafterabsenceend Is Not null And (DATEDIFF(day,pc.FinishDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsenceend))) 
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
   Right join (select doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, adt.every, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope, doct.compulsory, adt.IDLabAgree, doct.ApprovalLevelRequired from DocumentTemplates doct inner join CausesDocumentsTracking adt on adt.iddocument  = doct.id where (doct.scope = 4)) docsabs on docsabs.idcause = po.IDCause and (docsabs.IDLabAgree = 0 or docsabs.IDLabAgree = ec.IDLabAgree) 
   Left  join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate, IdOverTimeForecast order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = 0 or Status = 1) docpending on docpending.IdDocumentTemplate = docsabs.documenttemplateid and docpending.IdOverTimeForecast = po.ID and (docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsabs.ApprovalLevelRequired))
   Left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid
           and doc.IdEmployee = po.IDEmployee          
   		and doc.IdOvertimeForecast = po.id         
  		and (doc.Status = 1 and ((doc.StatusLevel <= docsabs.ApprovalLevelRequired and docsabs.ApprovalLevelRequired<>0) or docsabs.ApprovalLevelRequired=0))
   		and ((doc.EndDate >= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) or doc.EndDate >= po.EndDate)         
   		and (doc.BeginDate <= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)))  
   where doc.RowNumber is NULL 
   -- and (docpending.RowNumber1 is NULL or docpending.RowNumber1 =1)
   and ((docpending.RowNumber1 is NULL and docsabs.Compulsory = 1) or docpending.RowNumber1 =1 ) 
   and ((docsabs.daysafterabsencebegin Is Not null And (DATEDIFF(day,po.BeginDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsencebegin) And (DATEDIFF(day,po.BeginDate,po.EndDate) >= docsabs.daysafterabsencebegin OR docsabs.every IS NULL)) Or  (docsabs.daysafterabsenceend Is Not null And (DATEDIFF(day,po.EndDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsenceend)))  
   and (docsabs.BeginValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsabs.BeginValidity <= po.BeginDate))    
   And LOPDAccessLevel In (-1,0,1,2) And (Area In (-1,0,1,2,3,4))
GO

-- Actualiza versión de la base de datos
Declare @spnames CURSOR
Declare @spname nvarchar(max)
Declare @moddef nvarchar(max)
Set @spnames = CURSOR FOR
select distinct object_name(c.id) 
from syscomments c, sysobjects o 
where c.text like '%CONVERT(NVARCHAR(4000)%'
and c.id = o.id
and o.type = 'P'   
OPEN @spnames
FETCH NEXT
FROM @spnames into @spname
WHILE @@FETCH_STATUS = 0
BEGIN   
    Set @moddef =
    (SELECT
    Replace ((REPLACE(definition,'CONVERT(NVARCHAR(4000)','convert(nvarchar(4000)')),'ALTER','create')
    FROM sys.sql_modules a
    JOIN 
    (   select type, name,object_id
    from sys.objects b
    where type in (
    'p' -- procedures
    )
    and is_ms_shipped = 0
    )b
    ON a.object_id=b.object_id where b.name = @spname)              
    exec('drop procedure dbo.' + @spname)
    execute sp_executesql @moddef
    FETCH NEXT FROM @spnames into @spname
END
GO

ALTER TABLE [dbo].[Communiques] ADD [Archived] [bit] NOT NULL DEFAULT 0
GO

ALTER TABLE dbo.TerminalReaders ADD NFC nvarchar(50) NULL
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'NFC', 1, N'TA', N'1', N'Blind', N'S', N'Local', N'1,0', N'0', N'0', N'0', N'0', 'Remote', 0, 0, N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'NFC', 1, N'TA', N'1', N'Blind', N'E', N'Local', N'1,0', N'0', N'0', N'0', N'0', 'Remote', 0, 0, N'')
GO

UPDATE dbo.ImportGuidesTemplates SET Profile = 'CalendarLinkCellLayout.xlsx' WHERE ID = 2
GO

UPDATE dbo.sysroLiveAdvancedParameters SET Value = 11 WHERE ParameterName='VTPortalApiVersion'
GO

IF not exists (select * from [dbo].DocumentTemplates where Scope = 7 AND IsSystem =1)
    INSERT INTO dbo.DocumentTemplates(Id,Name,ShortName,Description,Scope,Area,AccessValidation,BeginValidity,EndValidity,ApprovalLevelRequired,EmployeeDeliverAllowed,SupervisorDeliverAllowed,IsSystem,DefaultExpiration,ExpirePrevious,Compulsory,LOPDAccessLevel,DaysBeforeDelete,Notifications,LeaveDocType)
    VALUES (ISNULL((SELECT (MAX(Id) + 1) FROM DocumentTemplates),1), 'Comunicado','r_c','',7,1,0,'20200101','20790101',0,0,0,1,0,0,0,0,0,'',null)
GO

UPDATE sysroParameters SET Data='490' WHERE ID='DBVersion'
GO

