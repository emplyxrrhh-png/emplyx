alter VIEW [dbo].[sysrovwCommuniquesEmployees]    
  AS    
  SELECT IdCommunique, IdEmployee FROM [dbo].[CommuniqueEmployees]    
      LEFT JOIN [dbo].[Communiques] ON Communiques.Id = CommuniqueEmployees.IdCommunique    
       UNION    

       SELECT IdCommunique, sysrovwCurrentEmployeeGroups.IdEmployee FROM [dbo].[CommuniqueGroups]    
      LEFT JOIN [dbo].[Communiques] ON Communiques.Id = CommuniqueGroups.IdCommunique    
       INNER JOIN [dbo].[Groups] on CommuniqueGroups.idgroup = Groups.id    
       INNER JOIN [dbo].[sysrovwCurrentEmployeeGroups] on sysrovwCurrentEmployeeGroups.IDGroup = CommuniqueGroups.idgroup
       and sysrovwCurrentEmployeeGroups.CurrentEmployee = 1    
      INNER JOIN [dbo].[sysroPassports] ON Communiques.IdCreatedBy = sysroPassports.Id    
    and (sysrovwCurrentEmployeeGroups.IDGroup IN( SELECT Id FROM Groups WHERE ID = CommuniqueGroups.IdGroup   OR Path LIKE '' + (SELECT Path from Groups where id=CommuniqueGroups.IdGroup  ) +'\%'  AND dbo.WebLogin_GetPermissionOverGroup(sysroPassports.Id    , CommuniqueGroups.IdGroup  , 1, 0) >=  3) ) and dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.Id    ,sysrovwCurrentEmployeeGroups.IdEmployee,1,0,1,GetDate())>= 3 
    go

ALTER FUNCTION [dbo].[GetEmployeeBiometricsIDLive]
    (				
    	
    )
    RETURNS @ValueTable table(idPassport integer, RXA100_0 datetime,RXA100_1 datetime, RXFFNG_0 datetime, RXFFNG_1 datetime, RXFFAC_0 datetime, RXFPAL_0 datetime)
    AS
    BEGIN
  	declare @idPassport integer
  	declare @idPassportAnt integer=0
  	declare @Version as nvarchar(max)
  	declare @BiometricID as integer 
  	declare @LenBiometricData integer 
  	declare @TimeStamp as datetime
 	declare @fnumber integer
  	declare @RXA100_0 datetime = null
  	declare @RXA100_1 datetime = null
  	declare @RXFFNG_0 datetime = null
  	declare @RXFFNG_1 datetime = null
 	declare @RXFFNG_2 datetime = null
 	declare @RXFFNG_3 datetime = null
 	declare @RXFFNG_4 datetime = null
 	declare @RXFFNG_5 datetime = null
 	declare @RXFFNG_6 datetime = null
 	declare @RXFFNG_7 datetime = null
 	declare @RXFFNG_8 datetime = null
 	declare @RXFFNG_9 datetime = null
  	declare @RXFFAC_0 datetime = null
	declare @RXFPAL_0 datetime = null
  	
  	declare TableCursor cursor fast_forward for
 		select Numero, IDPassport, Version, BiometricID, TimeStamp, LenBiometricData from 
 		(
 		select ROW_NUMBER() OVER (PARTITION BY IDPassport ORDER BY IdPassport, BiometricID ASC) Numero,  PAM.IDPassport, PAM.Version, PAM.BiometricID, TimeStamp, (len(dbo.f_BinaryToBase64(PAM.biometricdata))) as LenBiometricData from 
 		sysroPassports_AuthenticationMethods PAM				
 		where PAM.Method =4 and PAM.Enabled=1 And PAM.Version IN ('RXA100','RXFFNG','RXFFAC','ZKUNIFAC','ZKUNIPAL') And len(dbo.f_BinaryToBase64(PAM.biometricdata))>4
 		) aux 
 		where Numero <=2 or (Version = 'ZKUNIPAL' and BiometricID = 0)
  	open TableCursor
    	
    	fetch next from TableCursor into @fnumber, @idPassport, @Version, @BiometricID,@TimeStamp, @LenBiometricData		
  	while (@@FETCH_STATUS <> -1)
  	begin
  		if @idPassportAnt = 0 set @idPassportAnt=@idPassport
  				
  		if @idPassportAnt<>@idPassport
  			begin
  				insert into @ValueTable values (@idPassportAnt, @RXA100_0, @RXA100_1, @RXFFNG_0, @RXFFNG_1, @RXFFAC_0, @RXFPAL_0)
  				set @RXA100_0= null
  				set @RXA100_1= null
  				set @RXFFNG_0= null
  				set @RXFFNG_1= null
  				set @RXFFAC_0= null
				set @RXFPAL_0= null
  				set @idPassportAnt = @idPassport
  			end
  		if @Version='RXA100' and @BiometricID=0 set @RXA100_0=@TimeStamp
  		if @Version='RXA100' and @BiometricID=1 set @RXA100_1=@TimeStamp
  		if @Version='RXFFNG' and @fnumber=1 set @RXFFNG_0=@TimeStamp
  		if @Version='RXFFNG' and @fnumber=2 set @RXFFNG_1=@TimeStamp
  		if (@Version='RXFFAC' or @Version='ZKUNIFAC') and @BiometricID=0 AND @LenBiometricData>4 set @RXFFAC_0=@TimeStamp
		if @Version='ZKUNIPAL' and @BiometricID=0 set @RXFPAL_0=@TimeStamp
  							
  		fetch next from TableCursor into @fnumber, @idPassport, @Version, @BiometricID, @TimeStamp, @LenBiometricData
  	end
  	if @idPassportAnt<>0  Insert into @ValueTable values (@idPassportAnt, @RXA100_0, @RXA100_1, @RXFFNG_0, @RXFFNG_1, @RXFFAC_0, @RXFPAL_0)
  	close TableCursor
  	deallocate TableCursor
    	RETURN
    END
GO
DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rx1', 1, N'TACO', N'1', N'Blind', N'E,S,X',  N'Local', N'1,0', N'0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

ALTER VIEW [dbo].[sysrovwForecastDocumentsFaults]
      AS
      select case WHEN docsabs.daysafterabsencebegin IS NULL  THEN DATEADD(day, docsabs.daysafterabsenceend,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays-1,pa.begindate))) ELSE DATEADD(DAY,docsabs.daysafterabsencebegin ,pa.BeginDate) END DueDate,
      isnull(doc.id,docpending.id) docid, 
      docsabs.documenttemplateid templateid,  
      pa.IDEmployee idemp,  
      Null idcomp,  
      pa.absenceid as forecastId,  
      docsabs.scope,  
      CASE pa.IsRequest WHEN 1 THEN 'requestdays' ELSE 'days' END as forecasttype, 
      pa.begindate begindate, 
      isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays-1,pa.begindate)) enddate,  
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
     			and ((doc.Status = 1 and ((doc.StatusLevel <= docsabs.ApprovalLevelRequired and docsabs.ApprovalLevelRequired<>0) or docsabs.ApprovalLevelRequired=0)) OR doc.Status = 2)
      		and ((doc.EndDate >= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) or doc.EndDate >= isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays-1,pa.begindate)))         
      		and (doc.BeginDate <= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)))  
      where doc.RowNumber is NULL 
      and ((docpending.RowNumber1 is NULL and docsabs.Compulsory = 1) or docpending.RowNumber1 =1 ) 
      and ((docsabs.daysafterabsencebegin Is Not null And (DATEDIFF(day,pa.BeginDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsencebegin) And (DATEDIFF(day,pa.BeginDate,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays-1,pa.begindate))) >= docsabs.daysafterabsencebegin OR docsabs.every IS NULL)) Or  (docsabs.daysafterabsenceend Is Not null And (DATEDIFF(day,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays-1,pa.begindate)), DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsenceend)))  
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
     		and ((doc.Status = 1 and ((doc.StatusLevel <= docsabs.ApprovalLevelRequired and docsabs.ApprovalLevelRequired<>0) or docsabs.ApprovalLevelRequired=0)) OR doc.Status = 2)
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
     		and ((doc.Status = 1 and ((doc.StatusLevel <= docsabs.ApprovalLevelRequired and docsabs.ApprovalLevelRequired<>0) or docsabs.ApprovalLevelRequired=0)) OR doc.Status = 2)
      		and ((doc.EndDate >= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) or doc.EndDate >= po.EndDate)         
      		and (doc.BeginDate <= DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)))  
      where doc.RowNumber is NULL 
      -- and (docpending.RowNumber1 is NULL or docpending.RowNumber1 =1)
      and ((docpending.RowNumber1 is NULL and docsabs.Compulsory = 1) or docpending.RowNumber1 =1 ) 
      and ((docsabs.daysafterabsencebegin Is Not null And (DATEDIFF(day,po.BeginDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsencebegin) And (DATEDIFF(day,po.BeginDate,po.EndDate) >= docsabs.daysafterabsencebegin OR docsabs.every IS NULL)) Or  (docsabs.daysafterabsenceend Is Not null And (DATEDIFF(day,po.EndDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsenceend)))  
      and (docsabs.BeginValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsabs.BeginValidity <= po.BeginDate))    
      And LOPDAccessLevel In (-1,0,1,2) And (Area In (-1,0,1,2,3,4))
GO
--Fichajes HOLA y ADIOS en terminales faciales y palma

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFPTD', 1, N'TA', N'1', N'Fast', N'X', N'Local', N'1,0', N'0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFP', 1, N'TA', N'1', N'Fast', N'X', N'Local', N'1,0', N'0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

DECLARE @MaxId int;  
SET @MaxId = (SELECT  maX(id)+1  from dbo.sysroreadertemplates);  
INSERT [dbo].[sysroReaderTemplates] ([ID], [Type], [IDReader], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) 
VALUES (@MaxId, N'rxFL', 1, N'TA', N'1', N'Fast', N'X', N'Local', N'1,0', N'0', N'0', N'1,0', N'0', NULL, 1, N'1', N'')
GO

ALTER TABLE TMPEmergencyBasic ADD EmergencyPoint nvarchar(100)
ALTER TABLE TMPEmergencyBasic ADD EmergencyNumber nvarchar(100)
ALTER TABLE TMPEmergencyBasic ADD UserFieldValue2 nvarchar(100)
ALTER TABLE TMPEmergencyBasic ADD UserFieldValue3 nvarchar(100)
ALTER TABLE TMPEmergencyBasic ADD UserFieldValue4 nvarchar(100)
ALTER TABLE TMPEmergencyBasic ALTER COLUMN  UserFieldValue nvarchar(100)




UPDATE sysroParameters SET Data='504' WHERE ID='DBVersion'
GO

