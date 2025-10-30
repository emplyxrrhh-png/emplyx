 ALTER view [dbo].[sysrovwIncompletedDays] 
 AS
 SELECT punchesInfo.pInCount, punchesInfo.pOutCount, punchesInfo.Name, punchesInfo.IDEmployee,punchesInfo.Date from(
 	 SELECT ISNULL(pIn.pInCount,0) as pInCount, ISNULL(pOut.pOutCount,0) as pOutCount, ISNULL(pIn.Name,pOut.Name) as Name, Isnull(pIn.IDEmployee,pout.IDEmployee) as IDEmployee ,isnull(pIn.Date,pout.date) as Date FROM
 		 (SELECT count(*) AS pInCount, Employees.Name, Punches.IDEmployee, Punches.ShiftDate AS Date
 		 FROM Employees,  Punches
 		 WHERE Employees.ID = Punches.IDEmployee 
 		 AND (ActualType=1) 
 		 GROUP BY Punches.ShiftDate, Employees.Name, Punches.IDEmployee) pIn
 		FULL OUTER JOIN
 		 (SELECT count(*)  AS pOutCount, Employees.Name, Punches.IDEmployee, Punches.ShiftDate AS Date
 		 FROM Employees,  Punches
 		 WHERE Employees.ID = Punches.IDEmployee 
 		 AND (ActualType=2) 
 		 GROUP BY Punches.ShiftDate, Employees.Name, Punches.IDEmployee) pOut
 	 ON pIn.IDEmployee = pOut.IDEmployee AND pIn.Date = pOut.Date	 
 	 ) punchesInfo
	  INNER JOIN EmployeeContracts ec ON punchesInfo.IDEmployee = ec.IDEmployee and punchesInfo.Date between ec.BeginDate and ec.EndDate
 WHERE punchesInfo.pInCount <> punchesInfo.pOutCount
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
    						LEFT OUTER JOIN dbo.BusinessCenters with (nolock) ON dbo.DailyCauses.IDCenter = dbo.BusinessCenters.ID 
       where     (dbo.businesscenters.id in (select distinct idcenter from sysropassports_centers with (nolock) where idpassport=(SELECT isnull(IdparentPassport,0) from sysroPassports with (nolock) where ID = @idpassport)) )
    			 and dbo.businesscenters.id in (select idBusinessCenter from @businessCenterIDs) or (0 in (select idBusinessCenter from @businessCenterIDs) AND dbo.businesscenters.id is null)
 			 and dbo.dailycauses.date between @initialdate and @enddate
GO

-- Prevision de vacaciones por horas
CREATE TABLE [dbo].[ProgrammedHolidays](
	[ID] [numeric](16, 0) NOT NULL,
	[IDEmployee] [int] NOT NULL,
	[Date] [smalldatetime] NOT NULL,
	[IDCause] [smallint] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[AllDay] [bit] NOT NULL,
	[BeginTime] [datetime]  NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[Duration] [numeric](8, 6) NOT NULL,
 CONSTRAINT [PK_ProgrammedHolidays] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY] 

GO

ALTER TABLE [dbo].[ProgrammedHolidays] ADD  DEFAULT ((0)) FOR [AllDay]
GO

ALTER TABLE [dbo].[ProgrammedHolidays] ADD  CONSTRAINT [DF_ProgrammedHolidays_Duration]  DEFAULT ((0)) FOR [Duration]
GO

ALTER TABLE [dbo].[ProgrammedHolidays]  WITH CHECK ADD  CONSTRAINT [FK_ProgrammedHolidays_Causes] FOREIGN KEY([IDCause])
REFERENCES [dbo].[Causes] ([ID])
GO

ALTER TABLE [dbo].[ProgrammedHolidays] CHECK CONSTRAINT [FK_ProgrammedHolidays_Causes]
GO

ALTER TABLE [dbo].[ProgrammedHolidays]  WITH CHECK ADD  CONSTRAINT [FK_ProgrammedHolidays_Employees] FOREIGN KEY([IDEmployee])
REFERENCES [dbo].[Employees] ([ID])
GO

ALTER TABLE [dbo].[ProgrammedHolidays] CHECK CONSTRAINT [FK_ProgrammedHolidays_Employees]
GO

-- Dias solicitados de una solicitud de vacaciones por horas/dias
CREATE TABLE [dbo].[sysroRequestDays](
	[IDRequest] [int] NOT NULL,
	[Date] [smalldatetime] NOT NULL,
	[AllDay] [bit] NULL,
	[BeginTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[Duration] [numeric](8, 6) NULL,
 CONSTRAINT [PK_sysroRequestDays] PRIMARY KEY CLUSTERED 
(
	[IDRequest] ASC,
	[Date] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[sysroRequestDays]  WITH CHECK ADD  CONSTRAINT [FK_sysroRequestDays_Requests] FOREIGN KEY([IDRequest])
REFERENCES [dbo].[Requests] ([ID])
GO

ALTER TABLE [dbo].[sysroRequestDays] CHECK CONSTRAINT [FK_sysroRequestDays_Requests]
GO


-- definicion de devengos automaticos
ALTER  TABLE [dbo].[Concepts] ADD 
	[AutomaticAccrualType] [smallint] NULL DEFAULT (0),
	[AutomaticAccrualCriteria] nvarchar(MAX) NULL,
	[AutomaticAccrualIDCause] [smallint] NULL DEFAULT (0)
GO

-- Creamos nuevo campo en las justificaciones diarias para indicar si se ha generado a partir de un devengo automatico
ALTER TABLE dbo.DailyCauses DROP CONSTRAINT [PK_DailyCauses]
GO

ALTER TABLE [dbo].[DailyCauses] ADD [AccruedRule] [smallint] NOT  NULL DEFAULT(0)
GO

UPDATE [dbo].[DailyCauses] Set AccruedRule= 0 WHERE AccruedRule IS NULL
GO


ALTER TABLE [dbo].[DailyCauses] ADD 
	CONSTRAINT [PK_DailyCauses] PRIMARY KEY  NONCLUSTERED 
	(
	  [IDEmployee],
	  [Date],
	  [IDRelatedIncidence],
	  [IDCause],
	  [IDCenter],
	  [AccrualsRules],
	  [DailyRule],
	  [AccruedRule]
	)  WITH FILLFACTOR = 90 ON [PRIMARY] 
GO




-- Permisos para solicitudes portal y desktop

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =2560)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2560,2500,'Calendar.Requests.PlannedHour','Previsión vacaciones','','U','RWA',NULL,13,2)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2560, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2500
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =21160)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(21160,21100,'Planification.Requests.PlannedHour','Previsión vacaciones/permisos por horas','','E','RW',NULL,NULL,NULL)
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =2570)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2570,2500,'Calendar.Requests.PlannedOvertime','Previsión exceso de horas','','U','RWA',NULL,14,2)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2570, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2500
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =21170)
BEGIN
	INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(21170,21100,'Planification.Requests.PlannedOvertime','Previsión exceso de horas','','E','RW',NULL,NULL,NULL)
END
GO

EXEC dbo.sysro_GenerateAllPermissionsOverFeatures
GO

CREATE PROCEDURE [dbo].[sysro_GenerateAllPermissionsOverGroupsByEmployeeFeatureID]
 (
     		@EmployeeFeatureID int
     	)
     AS
     BEGIN
   	DECLARE @featureEmployeeID int
 	DELETE FROM sysroPermissionsOverGroups where employeefeatureid= @EmployeeFeatureID

  	DECLARE EmployeeFeatureCursor CURSOR
  	FOR 
  		SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid = @EmployeeFeatureID
  	OPEN EmployeeFeatureCursor
  	FETCH NEXT FROM EmployeeFeatureCursor INTO @featureEmployeeID
  	WHILE @@FETCH_STATUS = 0
  	BEGIN	
  		
  		INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission) 
  				select supPassports.ID, Groups.ID, @featureEmployeeID, dbo.GetPassportLevelOfAuthority(supPassports.ID),
  						dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,@featureEmployeeID,0)
  				from sysroPassports supPassports, Groups
  				where supPassports.GroupType = 'U'
  		
  		FETCH NEXT FROM EmployeeFeatureCursor 
  		INTO @featureEmployeeID
  	END 
  	CLOSE EmployeeFeatureCursor
  	DEALLOCATE EmployeeFeatureCursor
 	    	
     END
GO


EXEC sysro_GenerateAllPermissionsOverGroupsByEmployeeFeatureID 2
GO



-- Saldo actual de la justificacion 
ALTER TABLE dbo.Causes ADD [IDConceptBalance] [smallint] NULL DEFAULT (0)
GO

-- Nuevas propiedades de los horarios de vacaciones
ALTER TABLE dbo.Shifts ADD TypeHolidayValue Smallint NULL Default(0)
GO

UPDATE dbo.Shifts SET TypeHolidayValue=0 WHERE TypeHolidayValue is NULL
GO

ALTER TABLE dbo.Shifts ADD HolidayValue NUMERIC(10,3) NULL Default(0)
GO

UPDATE dbo.Shifts SET HolidayValue=0 WHERE HolidayValue is NULL
GO


-- creamos el stored para generar registros diarios de solicitudes de vacaciones por dias o cancelacion de vacaciones por dias v1 para que sean compatibles con la v2
CREATE PROCEDURE [dbo].[GenerateSysroRequestDays]
  AS
  BEGIN

 -- PARA CADA DIA DEL PERIODO DE LA SOLICITUD,
 -- HAY QUE AÑADIR UN REGISTRO EN SYSROREQUESTDAY 
 -- PARA CADA DIA QUE SEA NECESARIO
 
 		DECLARE @IDRequest int,@Date1 datetime, @Date2 datetime
		DECLARE @ActualDate as datetime, @WorkingDays bit, @ApplyWorkingDays bit, @EmployeeID int,@RequestType tinyint

		DECLARE RequestsCursor CURSOR
			FOR SELECT ID, Date1, Date2, (SELECT isnull(AreWorkingDays,0) FROM Shifts WHERE ID=Requests.IDShift) AS WorkingDays, IDEmployee, RequestType  FROM Requests 
				WHERE Status <> 3 and Status <> 2 AND (RequestType = 6 or RequestType = 11)  AND not exists (SELECT 1 FROM sysroRequestDays WHERE IDRequest = Requests.id)
		OPEN RequestsCursor

		FETCH NEXT FROM RequestsCursor
		INTO @IDRequest ,@Date1, @Date2,@WorkingDays,@EmployeeID, @RequestType

		WHILE @@FETCH_STATUS = 0
		BEGIN

			DELETE FROM dbo.sysroRequestDays WHERE IDRequest = @IDRequest

			SET @ApplyWorkingDays = 0
			SET @ActualDate = @Date1

			WHILE @ActualDate <= @Date2
			BEGIN
				SET @ApplyWorkingDays=0
				IF @RequestType = 6
				-- VACACIONES POR DIAS
				BEGIN

					IF @WorkingDays = 0
					BEGIN
						-- GENERAMOS EL REGISTRO PARA TODOS LOS DIAS DEL PERIODO
						SET @ApplyWorkingDays=1
					END

					IF @WorkingDays =1
					BEGIN

						-- SOLO TENEMOS QUE GENERAR EL REGISTRO EN EL CASO QUE EL DIA TENGA HORAS TEORICAS
						DECLARE @ExpectedWorkingHours numeric(9,6)
						SET @ExpectedWorkingHours = 0
	  					SELECT @ExpectedWorkingHours = isnull(DailySchedule.ExpectedWorkingHours, isnull(Shifts.ExpectedWorkingHours,0))  FROM DailySchedule, Shifts	
							WHERE DailySchedule.IDShift1 = Shifts.ID AND  DailySchedule.IDEmployee = @EmployeeID AND DailySchedule.Date =  @ActualDate

					
						if @ExpectedWorkingHours > 0 
						BEGIN
							SET @ApplyWorkingDays=1
						END
					END

					if @ApplyWorkingDays = 1
					BEGIN
						-- Para cada dia, creamos un registro en la tabla sysroRequestDays
						INSERT INTO dbo.sysroRequestDays (IDRequest,Date) VALUES (@IDRequest, @ActualDate)
					END
				END

				IF @RequestType = 11
				BEGIN
				-- CANCELACION DE VACACIONES
				-- SOLO TENEMOS QUE GENERAR EL REGISTRO EN EL CASO QUE EL DIA TENGA PLANIFICADO DIA DE VACACIONES
					DECLARE @IsHolidays bit
					SET @IsHolidays = 0
	  				SELECT @IsHolidays = isnull(DailySchedule.IsHolidays,0)  FROM DailySchedule
						WHERE DailySchedule.IDEmployee = @EmployeeID AND DailySchedule.Date =  @ActualDate
					if @IsHolidays = 1
					BEGIN
						INSERT INTO dbo.sysroRequestDays (IDRequest,Date) VALUES (@IDRequest, @ActualDate)
					END
				END

				SET @ActualDate = DATEADD(DAY,1,@ActualDate)

			END
			FETCH NEXT FROM RequestsCursor 
			INTO @IDRequest ,@Date1, @Date2,@WorkingDays,@EmployeeID,@RequestType

		END 
		CLOSE RequestsCursor
		DEALLOCATE RequestsCursor
END
GO

EXEC  [dbo].[GenerateSysroRequestDays]
GO

UPDATE dbo.sysroLiveAdvancedParameters SET Value = 2 WHERE ParameterName='VTPortalApiVersion'
GO

-- Indica si la justificación es de tipo vacaciones
ALTER TABLE dbo.Causes ADD [IsHoliday] [bit] NOT NULL DEFAULT (0)
GO

-- Ampliamos los decimales del factor de la composicion
ALTER TABLE [dbo].[ConceptCauses] ALTER COLUMN 
	[HoursFactor] [decimal](10, 6) NOT NULL
GO

ALTER TABLE [dbo].[ConceptCauses] ALTER COLUMN 
	[OccurrencesFactor] [decimal](10, 6) NOT NULL
GO

-- Tipos de solicitudes
DELETE FROM sysroRequestType
GO

INSERT INTO sysroRequestType (IdType,Type) VALUES(1,'UserFieldsChange')
GO

INSERT INTO sysroRequestType (IdType,Type) VALUES(2,'ForbiddenPunch')
GO

INSERT INTO sysroRequestType (IdType,Type) VALUES(3,'JustifyPunch')
GO

INSERT INTO sysroRequestType (IdType,Type) VALUES(4,'ExternalWorkResumePart')
GO

INSERT INTO sysroRequestType (IdType,Type) VALUES(5,'ChangeShift')
GO

INSERT INTO sysroRequestType (IdType,Type) VALUES(6,'VacationsOrPermissions')
GO

INSERT INTO sysroRequestType (IdType,Type) VALUES(7,'PlannedAbsences')
GO

INSERT INTO sysroRequestType (IdType,Type) VALUES(8,'ExchangeShiftBetweenEmployees')
GO

INSERT INTO sysroRequestType (IdType,Type) VALUES(9,'PlannedCauses')
GO

INSERT INTO sysroRequestType (IdType,Type) VALUES(10,'ForbiddenTaskPunch')
GO

INSERT INTO sysroRequestType (IdType,Type) VALUES(11,'CancelHolidays')
GO

INSERT INTO sysroRequestType (IdType,Type) VALUES(12,'ForgottenCostCenterPunch')
GO

INSERT INTO sysroRequestType (IdType,Type) VALUES(13,'PlannedHolidays')
GO

INSERT INTO sysroRequestType (IdType,Type) VALUES(14,'PlannedOvertimes')
GO

-- tipos de reglas de validacion
CREATE TABLE [dbo].[sysroRuleType](
	[ID] [tinyint] NOT NULL,
	[Type] [varchar](max) NOT NULL,
 CONSTRAINT [PK_sysroRuleType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[sysroRuleType] (ID, TYPE) VALUES(1,'NegativeAccrual')
GO

INSERT INTO [dbo].[sysroRuleType] (ID, TYPE) VALUES(2,'MaxNumberDays')
GO

INSERT INTO [dbo].[sysroRuleType] (ID, TYPE) VALUES(3,'PeriodEnjoyment')
GO

-- Relacion entre tipos de solicitudes y tipos de reglas
CREATE TABLE [dbo].[sysroRequestRuleTypes] (
	[IDRequestType] [tinyint] NOT NULL,
	[IDRuleType] [tinyint] NOT NULL,
 CONSTRAINT [PK_sysroRequestRuleTypes] PRIMARY KEY CLUSTERED 
(
	[IDRequestType], [IDRuleType] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO


INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(6,1)
GO

INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(13,1)
GO

INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(7,2)
GO

INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(6,3)
GO

INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(13,3)
GO

-- reglas de validacion de solicitudes
CREATE TABLE [dbo].[RequestsRules](
	[IDRule] [int] NOT NULL,
	[IDLabAgree] [int] NOT NULL,
	[IDRequestType] [tinyint] NOT NULL,
	[IDRuleType] [smallint] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Definition] [nvarchar](max) NULL,
	[Activated] [bit] NULL DEFAULT(1),
 CONSTRAINT [PK_RequestsRules] PRIMARY KEY CLUSTERED 
(
	[IDRule] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[RequestsRules]  WITH CHECK ADD  CONSTRAINT [FK_RequestsRules_LabAgrees] FOREIGN KEY([IDLabAgree])
REFERENCES [dbo].[LabAgree] ([ID])
GO

ALTER TABLE [dbo].[RequestsRules] CHECK CONSTRAINT [FK_RequestsRules_LabAgrees]
GO

-- PREVISIONES DE HORAS DE EXCESO
CREATE TABLE [dbo].[ProgrammedOvertimes](
	[ID] [numeric](16, 0) NOT NULL,
	[IDEmployee] [int] NOT NULL,
	[BeginDate] [smalldatetime] NOT NULL,
	[EndDate] [smalldatetime] NOT NULL,
	[IDCause] [smallint] NOT NULL,
	[Duration] [numeric](8, 6) NOT NULL,
	[Description] [nvarchar](MAX) NULL,
	[BeginTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[MinDuration] [numeric](8, 6) NULL DEFAULT ((0)),
 CONSTRAINT [PK_ProgrammedOvertimes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY] 

GO

ALTER TABLE [dbo].[ProgrammedOvertimes]  WITH CHECK ADD  CONSTRAINT [FK_ProgrammedOvertimes_Causes] FOREIGN KEY([IDCause])
REFERENCES [dbo].[Causes] ([ID])
GO

ALTER TABLE [dbo].[ProgrammedOvertimes] CHECK CONSTRAINT [FK_ProgrammedOvertimes_Causes]
GO

ALTER TABLE [dbo].[ProgrammedOvertimes]  WITH CHECK ADD  CONSTRAINT [FK_ProgrammedOvertimes_Employees] FOREIGN KEY([IDEmployee])
REFERENCES [dbo].[Employees] ([ID])
GO

ALTER TABLE [dbo].[ProgrammedOvertimes] CHECK CONSTRAINT [FK_ProgrammedOvertimes_Employees]
GO

--Documentación
alter table [dbo].[documents] add IdRequest int
GO

ALTER TABLE [dbo].[Documents]  WITH CHECK ADD  CONSTRAINT [FK_Documnets_Requests] FOREIGN KEY([IdRequest])
REFERENCES [dbo].[Requests] ([ID])
GO
ALTER TABLE [dbo].[Documents]  WITH CHECK ADD  CONSTRAINT [FK_Documnets_Employees] FOREIGN KEY([IdEmployee])
REFERENCES [dbo].[Employees] ([ID])
GO

alter table dbo.ProgrammedAbsences add AbsenceID int identity(1,1)
GO
alter table dbo.ProgrammedCauses add AbsenceID int identity(1,1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'GPADocumentation.LegacyMode')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('GPADocumentation.LegacyMode','1')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 50)
	INSERT INTO [dbo].[sysroNotificationTypes] ([ID],[Name],[Description],[Scheduler],[Feature],[FeatureType],[OnlySystem])
     VALUES (50,'Document pendig',NULL,360,'Documents','U',1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notifications] WHERE ID = 701)
	INSERT INTO [dbo].[Notifications] ([ID],[IDType],[Name],[Condition],[Destination],[Schedule],[Activated],[CreatorID],[AllowPortal],[IDPassportDestination],[AllowMail],[ShowOnDesktop],[LastTaskDeleted])
     VALUES (701,50,'Aviso de documento próximo a ser requerido','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>',NULL,1,'SYSTEM',0,0,0,0,NULL)
GO

CREATE TABLE [dbo].[AbsenceDocumentsTracking](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[IDCause] [int] NOT NULL,
	[IDLabAgree] [int] NOT NULL,
	[IDDocument] [int] NOT NULL,
	[DaysAfterAbsenceBegin] [int],
	[DaysAfterAbsenceEnd] [int],
	[Parameters] [nvarchar](max) NULL,
 CONSTRAINT [PK_AbsenceDocumentsTracking] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

alter table [dbo].[causes] add AbsenceNotRequiresPermission bit 
GO 

alter table [dbo].[sysroNotificationTasks] add Key5Numeric int
GO 

 CREATE PROCEDURE [dbo].[GetAbsenteeismDocumentaionFaults]
  	@idemployee int,
	@idpassport int,
	@idabsence int,
	@absencetype int,
	@LOPDPermissions nvarchar(30),
	@AreaPermissions nvarchar(30)
    AS 
   begin
    DECLARE @SQLString nvarchar(MAX);
   	SET @SQLString = ''
	IF @absencetype = 1 OR @absencetype = 0
	BEGIN
		SET @SQLString = 'select docsabs.documenttemplateid templateid, pa.IDEmployee idemp, Null idcomp, pa.absenceid, docsabs.scope, ''days'' absencetype, pa.begindate begindate, isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate)) enddate, NULL starthour, null endhour, pa.idcause idcause '
		SET @SQLString = @SQLString + 'from ProgrammedAbsences pa '
		SET @SQLString = @SQLString + 'Right join (select doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope from DocumentTemplates doct '
		SET @SQLString = @SQLString + 'inner join AbsenceDocumentsTracking adt on adt.iddocument  = doct.id where (doct.Scope = 3 or doct.scope = 4)) docsabs on docsabs.idcause = pa.IDCause '
		SET @SQLString = @SQLString + 'Left outer join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As ''RowNumber'', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid And doc.IdEmployee = pa.IDEmployee  and doc.IdDaysAbsence = pa.absenceid and (doc.Status = 1 and doc.CurrentlyValid=1) '
		SET @SQLString = @SQLString + 'where  (doc.RowNumber Is null or ( doc.RowNumber = 1 and (doc.Status <> 1 or doc.CurrentlyValid<>1))) '
		SET @SQLString = @SQLString + 'and ((docsabs.daysafterabsencebegin is not null and (DATEDIFF(day,pa.BeginDate, GETDATE()) >= docsabs.daysafterabsencebegin) and (DATEDIFF(day,pa.BeginDate,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate))) >= docsabs.daysafterabsencebegin)) or  (docsabs.daysafterabsenceend is not null and (DATEDIFF(day,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate)), GETDATE()) >= docsabs.daysafterabsenceend)))  '
		SET @SQLString = @SQLString + 'and (docsabs.BeginValidity = CONVERT(smalldatetime,''1900-01-01 00:00:00.000'',120) or (docsabs.BeginValidity <= pa.BeginDate)) '
		SET @SQLString = @SQLString + 'And [dbo].[WebLogin_GetPermissionOverEmployee](' + str(@idpassport) + ',pa.idemployee,32,0,1,GETDATE()) >= 3 '
		IF len(@LOPDPermissions) = 0 SET @SQLString = @SQLString + 'And LOPDAccessLevel in (-1,0,1,2) ' ELSE SET @SQLString = @SQLString + 'And LOPDAccessLevel in (-1,' + @LOPDPermissions +  ') '
		IF len(@AreaPermissions) = 0 SET @SQLString = @SQLString + 'And (Area in (-1,0,1,2,3,4)) ' ELSE SET @SQLString = @SQLString + 'And (Area in (-1,'+ @AreaPermissions +')) '
		SET @SQLString = @SQLString + 'And (Area in (-1,0,1,2,3,4)) '
		IF @idabsence > 0 SET @SQLString = @SQLString + 'And pa.absenceid = ' + STR(@idabsence)
		IF @idemployee > 0 SET @SQLString = @SQLString + 'And pa.IDEmployee = ' +STR(@idemployee)
	END
	IF @absencetype = 0
	BEGIN
		SET @SQLString = @SQLString + 'UNION '
	END
	IF @absencetype = 2 OR @absencetype = 0
	BEGIN
		SET @SQLString = @SQLString + 'select docsabs.documenttemplateid templateid, pc.IDEmployee idemp, Null idcomp, pc.absenceid, docsabs.scope, ''hours'' absencetype, pc.date begindate, NULL enddate, pc.begintime starthour, pc.endtime endhour, pc.idcause idcause  '
		SET @SQLString = @SQLString + 'from ProgrammedCauses pc '
		SET @SQLString = @SQLString + 'Right join (select doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope from DocumentTemplates doct '
		SET @SQLString = @SQLString + 'inner join AbsenceDocumentsTracking adt on adt.iddocument  = doct.id where (doct.Scope = 3 or doct.scope = 4)) docsabs on docsabs.idcause = pc.IDCause '
		SET @SQLString = @SQLString + 'Left outer join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As ''RowNumber'', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid And doc.IdEmployee = pc.IDEmployee  and doc.IdDaysAbsence = pc.absenceid and (doc.Status = 1 and doc.CurrentlyValid=1) '
		SET @SQLString = @SQLString + 'where  (doc.RowNumber Is null or ( doc.RowNumber = 1 and (doc.Status <> 1 or doc.CurrentlyValid<>1))) '
		SET @SQLString = @SQLString + 'and ((docsabs.daysafterabsencebegin is not null and (DATEDIFF(day,pc.Date, GETDATE()) >= docsabs.daysafterabsencebegin)and (DATEDIFF(day,pc.Date,pc.Date) >= docsabs.daysafterabsencebegin)) or  (docsabs.daysafterabsenceend is not null and (DATEDIFF(day,pc.Date, GETDATE()) >= docsabs.daysafterabsenceend))) '
		SET @SQLString = @SQLString + 'and (docsabs.BeginValidity = CONVERT(smalldatetime,''1900-01-01 00:00:00.000'',120) or (docsabs.BeginValidity <= pc.Date)) '
		SET @SQLString = @SQLString + 'And [dbo].[WebLogin_GetPermissionOverEmployee](' + str(@idpassport) + ',pc.idemployee,32,0,1,GETDATE()) >= 3 '
		IF len(@LOPDPermissions) = 0 SET @SQLString = @SQLString + 'And LOPDAccessLevel in (-1,0,1,2) ' ELSE SET @SQLString = @SQLString + 'And LOPDAccessLevel in (-1,' + @LOPDPermissions +  ') '
		IF len(@AreaPermissions) = 0 SET @SQLString = @SQLString + 'And (Area in (-1,0,1,2,3,4)) ' ELSE SET @SQLString = @SQLString + 'And (Area in (-1,'+ @AreaPermissions +')) '
		IF @idabsence > 0 SET @SQLString = @SQLString + 'And pc.absenceid = ' + STR(@idabsence)
		IF @idemployee > 0 SET @SQLString = @SQLString + 'And pc.IDEmployee = ' +STR(@idemployee)
	END
  	exec sp_executesql @SQLString
   end
GO

--Vista para para pantalla de seguimiento de absentismo
 CREATE VIEW [dbo].[sysrovwAbsenteeismDocumentsFaults]
 AS
select case WHEN docsabs.daysafterabsencebegin IS NULL  THEN DATEADD(day, docsabs.daysafterabsenceend,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate))) ELSE DATEADD(DAY,docsabs.daysafterabsencebegin ,pa.BeginDate) END DueDate, doc.id docid, docsabs.documenttemplateid templateid, pa.IDEmployee idemp, Null idcomp, pa.absenceid, docsabs.scope, 'days' absencetype, pa.begindate begindate, isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate)) enddate, NULL starthour, null endhour, pa.idcause idcause 
from ProgrammedAbsences pa 
Right join (select doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope from DocumentTemplates doct inner join AbsenceDocumentsTracking adt on adt.iddocument  = doct.id where (doct.Scope = 3 or doct.scope = 4)) docsabs on docsabs.idcause = pa.IDCause 
Left outer join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid And doc.IdEmployee = pa.IDEmployee  and doc.IdDaysAbsence = pa.absenceid 
where  (doc.RowNumber Is null or ( doc.RowNumber = 1 and (doc.Status <> 1 or doc.CurrentlyValid<>1 or (doc.EndDate < DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)))))) 
and ((docsabs.daysafterabsencebegin is not null and (DATEDIFF(day,pa.BeginDate, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsencebegin) and (DATEDIFF(day,pa.BeginDate,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate))) >= docsabs.daysafterabsencebegin)) or  (docsabs.daysafterabsenceend is not null and (DATEDIFF(day,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays,pa.begindate)), DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsenceend))) 
and (docsabs.BeginValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsabs.BeginValidity <= pa.BeginDate)) 
and (doc.BeginDate is null or DATEDIFF(day, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)),doc.BeginDate)>0)  
And [dbo].[WebLogin_GetPermissionOverEmployee](1,pa.idemployee,32,0,1,GETDATE()) >= 3 
And LOPDAccessLevel in (-1,0,1,2) 
And (Area in (-1,0,1,2,3,4)) 
UNION 
select case WHEN docsabs.daysafterabsencebegin IS NULL  THEN DATEADD(day, docsabs.daysafterabsenceend,pc.Date) ELSE DATEADD(DAY,docsabs.daysafterabsencebegin ,pc.Date) END DueDate, doc.id docid, docsabs.documenttemplateid templateid, pc.IDEmployee idemp, Null idcomp, pc.absenceid, docsabs.scope, 'hours' absencetype, pc.date begindate, NULL enddate, pc.begintime starthour, pc.endtime endhour, pc.idcause idcause  
from ProgrammedCauses pc 
Right join (select doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope from DocumentTemplates doct inner join AbsenceDocumentsTracking adt on adt.iddocument  = doct.id where (doct.Scope = 3 or doct.scope = 4)) docsabs on docsabs.idcause = pc.IDCause 
Left outer join (Select row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid And doc.IdEmployee = pc.IDEmployee  and doc.IdDaysAbsence = pc.absenceid 
where  (doc.RowNumber Is null or ( doc.RowNumber = 1 and (doc.Status <> 1 or doc.CurrentlyValid<>1 or (doc.EndDate < DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)))))) 
and ((docsabs.daysafterabsencebegin is not null and (DATEDIFF(day,pc.Date, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsencebegin) and (DATEDIFF(day,pc.Date,pc.Date) >= docsabs.daysafterabsencebegin)) or  (docsabs.daysafterabsenceend is not null and (DATEDIFF(day,pc.Date, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsenceend)))  
and (docsabs.BeginValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsabs.BeginValidity <= pc.Date)) 
and (doc.BeginDate is null or DATEDIFF(day, DATEADD(DAY,0,DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)),doc.BeginDate)>0) 
And [dbo].[WebLogin_GetPermissionOverEmployee](1,pc.idemployee,32,0,1,GETDATE()) >= 3 
And LOPDAccessLevel in (-1,0,1,2) 
And (Area in (-1,0,1,2,3,4)) 
GO

--Esta es la que hay que usar en pantalla de seguimiento de absentismo si se usa Gestor de Documentos
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
			(SELECT CASE WHEN (select COUNT(*) from sysrovwAbsenteeismDocumentsFaults adf where adf.AbsenceID = pc.AbsenceID) > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered, 
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

UPDATE Shifts SET ShiftType = 1 WHERE ShiftType = 0
GO


UPDATE dbo.sysroParameters SET Data='414' WHERE ID='DBVersion'
GO
