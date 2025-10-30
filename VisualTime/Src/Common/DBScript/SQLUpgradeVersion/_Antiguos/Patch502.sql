 ALTER VIEW [dbo].[sysrovwForecastDocumentsFaults]
     AS
     select case WHEN docsabs.daysafterabsencebegin IS NULL  THEN DATEADD(day, docsabs.daysafterabsenceend,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate))) ELSE DATEADD(DAY,docsabs.daysafterabsencebegin ,pa.BeginDate) END DueDate,
     isnull(doc.id,docpending.id) docid, 
     docsabs.documenttemplateid templateid,  
     pa.IDEmployee idemp,  
     Null idcomp,  
     pa.absenceid as forecastId,  
     docsabs.scope,  
     CASE pa.IsRequest WHEN 1 THEN 'requestdays' ELSE 'days' END as forecasttype, 
     pa.begindate begindate, 
     isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate)) enddate,  
     NULL starthour,  
     null endhour,  
     pa.idcause idcause 
     from sysrovwDaysAbsences pa  
     left join employeecontracts ec on ec.idemployee = pa.idemployee and pa.BeginDate between ec.BeginDate and ec.EndDate 
     left join (select doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, adt.every, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope, doct.compulsory, adt.IDLabAgree, doct.ApprovalLevelRequired from DocumentTemplates doct inner join CausesDocumentsTracking adt on adt.iddocument  = doct.id where ((doct.Scope = 3 or doct.scope = 4))) docsabs on docsabs.idcause = pa.IDCause and (docsabs.IDLabAgree = 0 or docsabs.IDLabAgree = ec.IDLabAgree) 
     Left  join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate, IdDaysAbsence, IdRequest order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = 0 or Status = 1) docpending on docpending.IdDocumentTemplate = docsabs.documenttemplateid and (docpending.IdDaysAbsence = pa.AbsenceID OR docpending.IdRequest = pa.AbsenceID) and ((docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsabs.ApprovalLevelRequired)) and docsabs.ApprovalLevelRequired > 0 )
     Left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid
             and doc.IdEmployee = pa.IDEmployee          
     		and (doc.IdDaysAbsence = pa.absenceid OR doc.IdRequest = pa.absenceid)
    			and (doc.Status = 1 and ((doc.StatusLevel <= docsabs.ApprovalLevelRequired and docsabs.ApprovalLevelRequired<>0) or docsabs.ApprovalLevelRequired=0))
     		and ((doc.EndDate >= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) or doc.EndDate >= isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate)))         
     		and (doc.BeginDate <= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)))  
     where doc.RowNumber is NULL 
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
     	pc.absenceid as forecastId,  
     	docsabs.scope,  
     	CASE pc.IsRequest WHEN 1 THEN 'requesthours' ELSE 'hours' END forecasttype, 
     	pc.date date, 
     	pc.FinishDate enddate,  
     	pc.begintime starthour,  
     	pc.endtime endhour,  
     	pc.idcause idcause 
     from sysrovwHoursAbsences pc  
     left join employeecontracts ec on ec.idemployee = pc.idemployee and pc.Date between ec.BeginDate and ec.EndDate 
     left join (select doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, adt.every, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope, doct.compulsory, adt.IDLabAgree, doct.ApprovalLevelRequired from DocumentTemplates doct inner join CausesDocumentsTracking adt on adt.iddocument  = doct.id where ((doct.Scope = 3 or doct.scope = 4))) docsabs on docsabs.idcause = pc.IDCause and (docsabs.IDLabAgree = 0 or docsabs.IDLabAgree = ec.IDLabAgree) 
     left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate, IdHoursAbsence, IdRequest order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = 0 or Status = 1) docpending on docpending.IdDocumentTemplate = docsabs.documenttemplateid and (docpending.IdHoursAbsence = pc.AbsenceID OR docpending.IdRequest = pc.AbsenceID) and ((docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsabs.ApprovalLevelRequired)) and docsabs.ApprovalLevelRequired > 0 )
     Left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid
             and doc.IdEmployee = pc.IDEmployee
     		and (doc.IdHoursAbsence = pc.absenceid OR doc.IdRequest = pc.absenceid)
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
     Left  join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate, IdOverTimeForecast order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = 0 or Status = 1) docpending on docpending.IdDocumentTemplate = docsabs.documenttemplateid and docpending.IdOverTimeForecast = po.ID and ((docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsabs.ApprovalLevelRequired))  and docsabs.ApprovalLevelRequired > 0 )
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




ALTER TABLE dbo.Shifts ADD DailyFactor numeric(6,3) NULL DEFAULT(1)
GO
UPDATE dbo.Shifts SET DailyFactor=1 where DailyFactor is NULL
go

UPDATE sysroParameters SET Data='502' WHERE ID='DBVersion'
GO

