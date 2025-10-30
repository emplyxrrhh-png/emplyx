ALTER VIEW [dbo].[sysrovwDaysAbsences]
     AS
 		SELECT IdCause, IDEmployee, Date1 as BeginDate, Date2 as FinishDate, 0 As MaxLastingDays, -1 as AbsenceID, Id as RequestId,  1 as IsRequest 
 		FROM Requests 
 		WHERE RequestType = 7 AND (Status = 0 OR Status = 1)
     UNION
 		SELECT IdCause, IDEmployee, BeginDate, FinishDate, MaxLastingDays, AbsenceID, -1 as RequestId, 0 as IsRequest
 		FROM ProgrammedAbsences 
GO

 ALTER VIEW [dbo].[sysrovwHoursAbsences]  
      AS  
    SELECT IDCause, IDEmployee, Date1 as [Date], Date2 as FinishDate, FromTime as BeginTime, ToTime as EndTime, Hours as Duration, -1 as AbsenceID, Id as RequestId, 1 as IsRequest  
    FROM Requests   
    WHERE RequestType = 9 AND (Status = 0 OR Status = 1)  
      UNION  
    SELECT IdCause, IDEmployee, Date, FinishDate, BeginTime, EndTime, Duration, AbsenceID, -1 as RequestId, 0 as IsRequest  
    FROM ProgrammedCauses 
GO

  ALTER VIEW [dbo].[sysrovwForecastDocumentsFaults]
      AS
      select case WHEN docsabs.daysafterabsencebegin IS NULL  THEN DATEADD(day, docsabs.daysafterabsenceend,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate))) ELSE DATEADD(DAY,docsabs.daysafterabsencebegin ,pa.BeginDate) END DueDate,
      isnull(doc.id,docpending.id) docid, 
      docsabs.documenttemplateid templateid,  
      pa.IDEmployee idemp,  
      Null idcomp,  
      CASE pa.IsRequest WHEN 0 THEN pa.absenceid ELSE pa.requestid END as forecastId,  
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
      Left  join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate, IdDaysAbsence, IdRequest order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = 0 or Status = 1) docpending on docpending.IdDocumentTemplate = docsabs.documenttemplateid and (docpending.IdDaysAbsence = pa.AbsenceID  OR docpending.IdRequest = pa.RequestID ) and ((docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsabs.ApprovalLevelRequired)) and docsabs.ApprovalLevelRequired > 0 )
      Left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid
              and doc.IdEmployee = pa.IDEmployee          
      		and (doc.IdDaysAbsence = pa.absenceid OR doc.IdRequest = pa.requestid)
     			and (doc.Status = 1 and ((doc.StatusLevel <= docsabs.ApprovalLevelRequired and docsabs.ApprovalLevelRequired<>0) or docsabs.ApprovalLevelRequired=0))
      		and ((doc.EndDate >= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) or doc.EndDate >= isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate)))         
      		and (doc.BeginDate <= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)))  
      where doc.RowNumber is NULL 
      and ((docpending.RowNumber1 is NULL and docsabs.Compulsory = 1) or docpending.RowNumber1 =1 ) 
      and (((docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsabs.ApprovalLevelRequired)) and docsabs.ApprovalLevelRequired > 0 ) OR ((docsabs.daysafterabsencebegin Is Not null And (DATEDIFF(day,pa.BeginDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsencebegin) And (DATEDIFF(day,pa.BeginDate,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate))) >= docsabs.daysafterabsencebegin OR docsabs.every IS NULL)) Or  (docsabs.daysafterabsenceend Is Not null And (DATEDIFF(day,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate)), DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsenceend))))
      and (docsabs.BeginValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsabs.BeginValidity <= pa.BeginDate))    
      And LOPDAccessLevel In (-1,0,1,2) And (Area In (-1,0,1,2,3,4))
      UNION
      select case WHEN docsabs.daysafterabsencebegin IS NULL  THEN DATEADD(day, docsabs.daysafterabsenceend,pc.FinishDate) ELSE DATEADD(DAY,docsabs.daysafterabsencebegin ,pc.Date) END DueDate,
      	isnull(doc.id,docpending.id) docid, 
      	docsabs.documenttemplateid templateid,  
      	pc.IDEmployee idemp,  
      	Null idcomp,  
      	CASE pc.IsRequest WHEN 0 THEN pc.absenceid ELSE pc.requestid END as forecastId,  
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
      left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate, IdHoursAbsence, IdRequest order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = 0 or Status = 1) docpending on docpending.IdDocumentTemplate = docsabs.documenttemplateid and (docpending.IdHoursAbsence = pc.AbsenceID  OR docpending.IdRequest = pc.RequestID ) and ((docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsabs.ApprovalLevelRequired)) and docsabs.ApprovalLevelRequired > 0 )
      Left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid
              and doc.IdEmployee = pc.IDEmployee
      		and (doc.IdHoursAbsence = pc.absenceid  OR doc.IdRequest = pc.requestid )
     		and (doc.Status = 1 and ((doc.StatusLevel <= docsabs.ApprovalLevelRequired and docsabs.ApprovalLevelRequired<>0) or docsabs.ApprovalLevelRequired=0))
      		and ((doc.EndDate >= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) or doc.EndDate >= pc.FinishDate)
      		and (doc.BeginDate <= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) 
      where doc.RowNumber is NULL 
      and ((docpending.RowNumber1 is NULL and docsabs.Compulsory = 1) or docpending.RowNumber1 =1 ) 
      and (((docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsabs.ApprovalLevelRequired)) and docsabs.ApprovalLevelRequired > 0 ) OR ((docsabs.daysafterabsencebegin Is Not null And (DATEDIFF(day,pc.Date, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsencebegin) And (DATEDIFF(day,pc.Date,pc.FinishDate) >= docsabs.daysafterabsencebegin OR docsabs.every IS NULL)) Or  (docsabs.daysafterabsenceend Is Not null And (DATEDIFF(day,pc.FinishDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsenceend))))
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
      and ((docpending.RowNumber1 is NULL and docsabs.Compulsory = 1) or docpending.RowNumber1 =1 ) 
      and (((docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsabs.ApprovalLevelRequired)) and docsabs.ApprovalLevelRequired > 0 ) OR ((docsabs.daysafterabsencebegin Is Not null And (DATEDIFF(day,po.BeginDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsencebegin) And (DATEDIFF(day,po.BeginDate,po.EndDate) >= docsabs.daysafterabsencebegin OR docsabs.every IS NULL)) Or  (docsabs.daysafterabsenceend Is Not null And (DATEDIFF(day,po.EndDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsenceend))))
      and (docsabs.BeginValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsabs.BeginValidity <= po.BeginDate))    
      And LOPDAccessLevel In (-1,0,1,2) And (Area In (-1,0,1,2,3,4))
GO

CREATE VIEW [dbo].[sysrovwEmployeeStatus]
 AS
	SELECT svEmployees.IdEmployee, 
	tmpAttendance.ShiftDate AttShiftDate, 
	tmpAttendance.DateTime as AttDatetime, 
	CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN 'Out-Calculated' ELSE CASE tmpAttendance.ActualType WHEN 1 THEN 'In' ELSE 'Out' END END as AttStatus, 
	tmpTasks.DateTime as TskDatetime, 
    Tasks.Project + ' : ' +  Tasks.Name as TskName,  
	tmpcosts.DateTime as CostDatetime, 
	BusinessCenters.Name as CostCenterName 
	FROM sysrovwcurrentemployeegroups svEmployees
	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberAtt', * FROM Punches WHERE ActualType = 1 or ActualType = 2) tmpAttendance ON svEmployees.IDEmployee = tmpAttendance.IdEmployee
	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberTsk', * FROM Punches WHERE ActualType = 4) tmpTasks ON svEmployees.IDEmployee = tmpTasks.IdEmployee
	left join Tasks ON Tasks.Id = tmpTasks.TypeData
	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberCost', * FROM Punches WHERE ActualType = 13) tmpCosts ON svEmployees.IDEmployee = tmpCosts.IdEmployee
	left join BusinessCenters ON tmpCosts.TypeData = BusinessCenters.ID
	WHERE CurrentEmployee = 1
	and (RowNumberAtt = 1 or RowNumberAtt is null)
	and (RowNumberTsk = 1 or RowNumberTsk is null)
	and (RowNumberCost = 1 or RowNumberCost is null)
	and (tmpAttendance.DateTime IS NOT NULL OR tmpTasks.DateTime IS NOT NULL OR tmpCosts.DateTime IS NOT NULL)
GO

delete from auditlive where ClientLocation like '%VTPortal%' and ElementID=22 and ActionID=4
GO
delete from auditlive where ClientLocation like '%VTPortal%' and ElementID=1 and ActionID=1
GO

UPDATE sysroParameters SET Data='505' WHERE ID='DBVersion'
GO

