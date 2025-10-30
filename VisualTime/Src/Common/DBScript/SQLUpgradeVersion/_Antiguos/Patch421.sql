--Documentos de seguimiento
alter table dbo.CausesDocumentsTracking add Every int  null
GO
update dbo.CausesDocumentsTracking set Every = 1 where Parameters like '%"TypeRequest" type="2">6%'
GO

--Documentos de tipo justificante
ALTER TABLE [dbo].[Documents] ADD IdOvertimeForecast int null
GO

-- Eliminamos vistas obsoletas
DROP VIEW [dbo].[sysrovwAbsencesDocumentsFaults]
GO
DROP VIEW [dbo].[sysrovwAbsenteeismDocumentsFaults]
GO

-- Vista para estado de documentación de previsiones (absentismo y horas de exceso)
IF EXISTS(SELECT 1 FROM SYS.VIEWS WHERE NAME='sysrovwForecastDocumentsFaults' AND TYPE='V')
DROP VIEW [dbo].[sysrovwForecastDocumentsFaults];
GO
CREATE VIEW [dbo].[sysrovwForecastDocumentsFaults]
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
Right join (select doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, adt.every, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope, adt.IDLabAgree from DocumentTemplates doct inner join CausesDocumentsTracking adt on adt.iddocument  = doct.id where ((doct.Scope = 3 or doct.scope = 4) and doct.compulsory = 1)) docsabs on docsabs.idcause = pa.IDCause and (docsabs.IDLabAgree = 0 or docsabs.IDLabAgree = ec.IDLabAgree) 
Left  join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate, IdDaysAbsence order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = 0) docpending on docpending.IdDocumentTemplate = docsabs.documenttemplateid and docpending.IdDaysAbsence = pa.AbsenceID 
Left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid
        and doc.IdEmployee = pa.IDEmployee          
		and doc.IdDaysAbsence = pa.absenceid         
		and doc.Status not in (0,3,4)         
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
Right join (select doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, adt.every, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope, adt.IDLabAgree from DocumentTemplates doct inner join CausesDocumentsTracking adt on adt.iddocument  = doct.id where ((doct.Scope = 3 or doct.scope = 4) and doct.compulsory = 1)) docsabs on docsabs.idcause = pc.IDCause and (docsabs.IDLabAgree = 0 or docsabs.IDLabAgree = ec.IDLabAgree) 
left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate, IdHoursAbsence order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = 0) docpending on docpending.IdDocumentTemplate = docsabs.documenttemplateid and docpending.IdHoursAbsence = pc.AbsenceID 
Left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid
        and doc.IdEmployee = pc.IDEmployee
		and doc.IdHoursAbsence = pc.absenceid
		and doc.Status not in (0,3,4)
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
Right join (select doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, adt.every, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope, adt.IDLabAgree from DocumentTemplates doct inner join CausesDocumentsTracking adt on adt.iddocument  = doct.id where (doct.scope = 4 and doct.compulsory = 1)) docsabs on docsabs.idcause = po.IDCause and (docsabs.IDLabAgree = 0 or docsabs.IDLabAgree = ec.IDLabAgree) 
Left  join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate, IdOverTimeForecast order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = 0) docpending on docpending.IdDocumentTemplate = docsabs.documenttemplateid and docpending.IdOverTimeForecast = po.ID 
Left join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid
        and doc.IdEmployee = po.IDEmployee          
		and doc.IdOvertimeForecast = po.id         
		and doc.Status not in (0,3,4)         
		and ((doc.EndDate >= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) or doc.EndDate >= po.EndDate)         
		and (doc.BeginDate <= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)))  
where doc.RowNumber is NULL 
and (docpending.RowNumber1 is NULL or docpending.RowNumber1 =1)
and ((docsabs.daysafterabsencebegin Is Not null And (DATEDIFF(day,po.BeginDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsencebegin) And (DATEDIFF(day,po.BeginDate,po.EndDate) >= docsabs.daysafterabsencebegin OR docsabs.every IS NULL)) Or  (docsabs.daysafterabsenceend Is Not null And (DATEDIFF(day,po.EndDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsenceend)))  
and (docsabs.BeginValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsabs.BeginValidity <= po.BeginDate))    
And LOPDAccessLevel In (-1,0,1,2) And (Area In (-1,0,1,2,3,4))
GO

-- Vista para pantalla de estado de abesntismo
ALTER VIEW [dbo].[sysrovwAbsencesEx]
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
 			(SELECT CASE WHEN (select COUNT(*) from sysrovwForecastDocumentsFaults adf where adf.forecastID = pa.AbsenceID and adf.forecasttype = 'days') > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered, 
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
 			(SELECT CASE WHEN (select COUNT(*) from sysrovwForecastDocumentsFaults adf where adf.forecastID = pc.AbsenceID and adf.forecasttype = 'hours') > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered, 
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

-- Limpiamos alertas de PGA
UPDATE dbo.sysroNotificationTasks SET Executed = 1 WHERE IDNotification IN (1800,1801)
GO
UPDATE dbo.sysroNotificationTasks SET FiredDate = CONVERT(smalldatetime,'2017-01-01 00:00',120) WHERE IDNotification = 2002
GO

UPDATE dbo.sysroLiveAdvancedParameters SET Value = 4 WHERE ParameterName='VTPortalApiVersion'
GO

ALTER TABLE dbo.ExportGuides ADD ExportFileNameTimeStampFormat nvarchar(20)
GO

UPDATE dbo.sysroParameters SET Data='421' WHERE ID='DBVersion'
GO
