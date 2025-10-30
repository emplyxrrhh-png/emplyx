
ALTER TABLE dbo.sysroGUI_Actions ADD
	AppearsOnPopup bit NULL
GO

update dbo.sysroGUI_Actions set AppearsOnPopup = 0
GO

-- Nueva notificacion para enviar mail de aviso de fichaje durante ausencia por dias
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 65)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,OnlySystem, Feature,FeatureType) VALUES(65,'Advice for holiday during programmed absence',null, 120, 0,'Employees','U')
GO


INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (65,1,'Body',1,'EmployeeName','HolidayOnProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (65,1,'Body',2,'BeginDate','HolidayOnProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (65,1,'Body',3,'Cause','HolidayOnProgrammedAbsence')
GO
INSERT INTO dbo.NotificationMessageParameters (IdNotificationType,Scenario, Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey) VALUES (65,1,'Subject',0,'','HolidayOnProgrammedAbsence')
GO

-- Se añade filtro de grupo de negocio a la hora de verificar los permisos sobre las solicitudes
ALTER FUNCTION [dbo].[GetRequestPassportPermission]
     (
     	@idPassport int,
      	@idRequest int	
     )
     RETURNS int
     AS
     BEGIN
      	DECLARE @FeatureAlias nvarchar(100),
      			@EmployeefeatureID int,
      			@idEmployee int,
     			@RequestType int,
  				@BusinessCenter int,
				@IDCause int,
				@IDShift int

     			
      	SELECT @RequestType = Requests.RequestType,
     				@idEmployee = Requests.IDEmployee,
  				@BusinessCenter = Requests.IDCenter,
				@IDCause = isnull(Requests.IDCause,0),
				@IDShift = isnull(Requests.IDShift,0)

  		FROM Requests
     	WHERE Requests.ID = @idRequest
     	
     	SELECT @featureAlias = Alias, 
     		   @EmployeefeatureID = EmployeeFeatureId 
     	FROM dbo.sysroFeatures 
     	WHERE sysroFeatures.AliasId = @RequestType
      	
      	
      	DECLARE @Permission int
 		DECLARE @EmployeePermission int
      	SET @Permission = (select dbo.WebLogin_GetPermissionOverFeature(@idPassport,@FeatureAlias, 'U', 2)) 
      	
      	IF @Permission > 0 
      	BEGIN
      		SET @EmployeePermission = (select dbo.WebLogin_GetPermissionOverEmployee(@idPassport,@idEmployee,@EmployeefeatureID,2,1,getdate()))
      	END
  		IF @Permission > 0 AND @EmployeePermission > 0 AND @RequestType = 12
  		BEGIN
  			DECLARE @Centers int
  			set @Centers = (select isnull(count(*),0) as total from sysroPassports_Centers where idcenter = @BusinessCenter AND IDPassport IN(select IDParentPassport  from sysroPassports where id = @idPassport ))
  			IF @Centers = 0
  			BEGIN
  				SET @Permission = 0 
  			END
  			
  		END

		-- Permisos sobre Grupos de negocio
	    IF @Permission > 0 AND @EmployeePermission > 0 
  		BEGIN
			declare @IDParentPassport int,
				    @BusinessGroupList nvarchar(max)

		  	SELECT @IDParentPassport = isnull(IDParentPassport,0)
	     	FROM dbo.sysroPassports 
		 	WHERE sysroPassports.ID = @idPassport

			SELECT @BusinessGroupList = isnull(BusinessGroupList,'')
	     	FROM dbo.sysroPassports 
		 	WHERE sysroPassports.ID = @IDParentPassport

			
			if len(@BusinessGroupList) > 0 
			begin
				if @IDCause > 0
				BEGIN
					declare @BusinessGroupListCause nvarchar(max)

					set @BusinessGroupListCause = (SELECT ISNULL(BusinessGroup, '') AS BusinessGroup FROM Causes WHERE (Causes.ID = @IDCause) )
					if (len(@BusinessGroupListCause) > 0 )
					begin
						if charindex(@BusinessGroupListCause, @BusinessGroupList) = 0 
						begin
							SET @Permission = 0 
						end
					end

				END

				if @IDShift > 0 and @Permission > 0
				BEGIN
					declare @BusinessGroupListShift nvarchar(max)

					set @BusinessGroupListShift = (SELECT ISNULL(ShiftGroups.BusinessGroup, '') AS BusinessGroup FROM Shifts LEFT OUTER JOIN ShiftGroups ON Shifts.IDGroup = ShiftGroups.ID WHERE (Shifts.ID = @IDShift) )
					if (len(@BusinessGroupListShift) > 0 )
					begin
						if charindex(@BusinessGroupListShift, @BusinessGroupList) = 0 
						begin
							SET @Permission = 0 
						end
					end
				END
			end
  		END
      		
 		IF @EmployeePermission > @Permission
 			RETURN @Permission
 		RETURN @EmployeePermission
      	
   END
GO
   

update sysroGUI_Actions set AppearsOnPopup = 1 where ID in (34,35,39,186,206,208)
GO

update sysroGUI_Actions set AppearsOnPopup=1 where id in(176, 177, 178, 179, 185, 193, 194)
GO

delete from sysroGUI_Actions where id in(33, 42, 45, 47, 187, 194)
GO

update sysroGUI_Actions set AppearsOnPopup=0 where IDPath='MaxMinimize'
GO

update sysroGUI_Actions set AppearsOnPopup = 1 where IDPath='Reports'
GO

-- Dominio por defecto para login con Active Directory
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.AD.DefaultDomain')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.AD.DefaultDomain','')
GO

-- Correción vista de alertas de documentación de absentismo
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

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='460' WHERE ID='DBVersion'
GO
