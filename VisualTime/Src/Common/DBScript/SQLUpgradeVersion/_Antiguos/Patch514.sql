INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1645,1620,'Employees.DataLink.Exports.AdvRequests','Exportación de Solicitudes','','U','RWA',NULL,NULL,1)
GO
INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1650,1620,'Employees.DataLink.Exports.AdvCauses','Exportación de Justificaciones','','U','RWA',NULL,NULL,1)
GO
INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1655,1620,'Employees.DataLink.Exports.AdvAbsences','Exportación de Ausencias','','U','RWA',NULL,NULL,1)
GO

DELETE [dbo].[ExportGuidesTemplates]
GO

DELETE [dbo].[ExportGuides] WHERE ID IN (20005, 20006, 20007)
GO

INSERT [dbo].[ExportGuides] ([ID], [Name], [ProfileMask], [ProfileType], [Mode], [ProfileName], [Destination], [ExportFileName], [ExportFileType], [Separator], [StartCalculDay], [StartExecutionHour], [LastLog], [NextExecution], [Field_1], [Field_2], [Field_3], [Field_4], [Field_5], [Field_6], [IntervalMinutes], [DisplayParameters], [Scheduler], [EmployeeFilter], [RequieredFunctionalities], [FeatureAliasID], [ExportFileNameTimeStampFormat], [WSParameters], [AutomaticDatePeriod], [Version], [Concept], [Active], [IDDefaultTemplate], [PostProcessScript], [Enabled], [ApplyLockDate], [DatasourceFile]) 
VALUES (20005, N'Justificaciones', N'', 9, 1, N'', N'', N'', 1, N'', 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, N'1,1,0@1@1', N'', NULL, N'Employees.DataLink.Exports.AdvCauses', N'Employees', NULL, NULL, NULL, 2, N'Causes', 0, 6, NULL, NULL, 0, N'')
GO

INSERT [dbo].[ExportGuides] ([ID], [Name], [ProfileMask], [ProfileType], [Mode], [ProfileName], [Destination], [ExportFileName], [ExportFileType], [Separator], [StartCalculDay], [StartExecutionHour], [LastLog], [NextExecution], [Field_1], [Field_2], [Field_3], [Field_4], [Field_5], [Field_6], [IntervalMinutes], [DisplayParameters], [Scheduler], [EmployeeFilter], [RequieredFunctionalities], [FeatureAliasID], [ExportFileNameTimeStampFormat], [WSParameters], [AutomaticDatePeriod], [Version], [Concept], [Active], [IDDefaultTemplate], [PostProcessScript], [Enabled], [ApplyLockDate], [DatasourceFile]) 
VALUES (20006, N'Solicitudes', N'', 13, 1, N'', N'', N'', 1, N'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, N'1,1,0@1@1', N'', NULL, N'Employees.DataLink.Exports.AdvRequests', N'Employees', NULL, NULL, NULL, 2, N'Requests', 0, 6, NULL, NULL, 0, N'')
GO

INSERT [dbo].[ExportGuides] ([ID], [Name], [ProfileMask], [ProfileType], [Mode], [ProfileName], [Destination], [ExportFileName], [ExportFileType], [Separator], [StartCalculDay], [StartExecutionHour], [LastLog], [NextExecution], [Field_1], [Field_2], [Field_3], [Field_4], [Field_5], [Field_6], [IntervalMinutes], [DisplayParameters], [Scheduler], [EmployeeFilter], [RequieredFunctionalities], [FeatureAliasID], [ExportFileNameTimeStampFormat], [WSParameters], [AutomaticDatePeriod], [Version], [Concept], [Active], [IDDefaultTemplate], [PostProcessScript], [Enabled], [ApplyLockDate], [DatasourceFile]) 
VALUES (20007, N'Ausencias', N'', 14, 1, N'', N'', N'', 1, N'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, N'1,1,0@1@1', N'', NULL, N'Employees.DataLink.Exports.AdvAbsences', N'Employees', NULL, NULL, NULL, 2, N'Absences', 0, 6, NULL, NULL, 0, N'')
GO

INSERT INTO [dbo].[ExportGuidesTemplates] VALUES
(1,20001,N'employees_link',N'tmplExportEmployeesLink.xlsx',NULL,NULL,NULL),
(2,20002,N'daily_accruals',N'tmplExportAccrualsDaily.xlsx',NULL,NULL,NULL),
(3,20003,N'calendar_link',N'CalendarLinkCellLayout.xlsx',NULL,NULL,NULL),
(4,20004,N'punches',N'tmplExportPunches.xlsx',NULL,NULL,NULL),
(5,20002,N'accruals_at_date',N'tmplExportAccrualsAtDate.xlsx',NULL,NULL,NULL),
(6,20005,N'daily_causes', N'tmplExportCauses.xlsx', NULL, NULL, NULL),
(7,20006,N'requests', N'', NULL, NULL, NULL),
(8,20007,N'absences', N'', NULL, NULL, NULL),
(9,20001,N'employees_full', N'tmplExportEmployeesAdvanced.xlsx', NULL, NULL, NULL),
(10,20003,N'schedule',N'',NULL,NULL,NULL);
GO


-- Firma digital
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroParameters] WHERE [ID] = 'VID_URL')
	insert into [dbo].sysroParameters (ID,Data) values ('VID_URL','https://pre-vidsignercloud.validatedid.com/api/')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroParameters] WHERE [ID] = 'VID_SName')
	insert into [dbo].sysroParameters (ID,Data) values ('VID_SName','RoboticsSubscriptionDemo')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroParameters] WHERE [ID] = 'VID_SPwd')
	insert into [dbo].sysroParameters (ID,Data) values ('VID_SPwd','qKgq2SHUpSwUZs6VgBKX')
GO

ALTER TABLE dbo.Documents ADD SignStatus int null default(0)
GO
UPDATE Documents SET SignStatus=0 where SignStatus is null
GO

ALTER TABLE dbo.DocumentTemplates ADD RequiresSigning bit null default(0)
GO
UPDATE DocumentTemplates SET RequiresSigning=0 where RequiresSigning is null
GO

ALTER TABLE dbo.Documents ADD [SignReport] [image] NULL
GO

UPDATE sysroLiveAdvancedParameters SET Value = 14 WHERE ParameterName='VTPortalApiVersion'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroParameters] WHERE [ID] = 'VID_SDocs')
	insert into [dbo].sysroParameters (ID,Data) values ('VID_SDocs','0')
GO

 ALTER PROCEDURE [dbo].[Analytics_Requests_Schedule]  
        @initialDate smalldatetime,        
        @endDate smalldatetime,        
        @idpassport int,        
        @employeeFilter nvarchar(max),        
        @userFieldsFilter nvarchar(max),      
 	   @requestTypesFilter nvarchar(max)        
   AS        
        DECLARE @employeeIDs Table(idEmployee int)        
     DECLARE @requestTypeFilter Table(idRequest int)        
 	  DECLARE @SecurityMode int
 	  SELECT @SecurityMode = convert(numeric(3),isnull(value,'1')) from sysroLiveAdvancedParameters where ParameterName = 'SecurityMode'
        insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter,@initialDate,@endDate;        
     insert into @requestTypeFilter select * from dbo.SplitToInt(@requestTypesFilter,',');         
        SELECT     
 	   
 	   dbo.requests.ID AS KeyView, isnull(case when aux.NumeroDias > 0 then aux.NumeroDias else DATEDIFF(day,dbo.Requests.Date1, dbo.Requests.Date2) + 1 end,0) as NumeroDias, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee,         
                              dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Requests.RequestDate as 'Date',         
                              dbo.Requests.Status AS RequestsStatus,         
             Case         
          WHEN Requests.Status IN(2,3) THEN (SELECT top 1 isnull(sysropassports.Name,'') from RequestsApprovals inner join sysroPassports on RequestsApprovals.IDPassport = sysroPassports.ID  where IDRequest = Requests.ID AND Status in(2,3) )        
          ELSE '---'        
          END as 'ApproveRefusePassport',        
          Case         
           WHEN Requests.Status IN(0,1) Then (select dbo.GetRequestNextLevelPassports(Requests.ID,@SecurityMode))        
           ELSE '---'        
          END as 'PendingPassport',        
          dbo.Requests.RequestType,   
          isnull(CONVERT(NVARCHAR(4000), dbo.Requests.Date1, 120),'') as 'BeginDateRequest',        
          isnull(CONVERT(NVARCHAR(4000), dbo.Requests.FromTime, 8),'') as 'BeginHourRequest',        
          isnull(CONVERT(NVARCHAR(4000), isnull(dbo.Requests.Date2,dbo.Requests.Date1), 120),'') as 'EndDateRequest',        
          isnull(CONVERT(NVARCHAR(4000), dbo.Requests.ToTime, 8),'') as 'EndHourRequest',        
          ISNULL((SELECT Name from dbo.Causes WHERE ID = dbo.Requests.IDCause),'') as 'CauseNameRequest',        
          ISNULL((SELECT Name from dbo.Shifts WHERE ID = dbo.Requests.IDShift),'') as 'ShiftNameRequest',        
          CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')) as CommentsRequest,        
		  CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldName, '')) as FieldNameRequest, 
		  CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldValue, '')) as FieldValueRequest, 
                              CONVERT(nvarchar(8), dbo.Requests.RequestDate, 108) AS Time, DAY(dbo.Requests.RequestDate) AS Day,         
                              MONTH(dbo.Requests.RequestDate) AS Month, YEAR(dbo.Requests.RequestDate) AS Año, (DATEPART(dw, dbo.Requests.RequestDate) + @@DATEFIRST - 1 - 1)         
                              % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate,         
                              dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,         
               dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,        
             dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,      
       dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,        
             dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,      
       dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,        
             dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,      
       dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,        
             dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,      
       dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,        
             dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,      
       dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,        
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),dbo.Requests.RequestDate) As UserField1,        
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),dbo.Requests.RequestDate) As UserField2,        
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),dbo.Requests.RequestDate) As UserField3,        
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),dbo.Requests.RequestDate) As UserField4,        
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),dbo.Requests.RequestDate) As UserField5,        
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),dbo.Requests.RequestDate) As UserField6,        
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),dbo.Requests.RequestDate) As UserField7,        
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),dbo.Requests.RequestDate) As UserField8,        
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),dbo.Requests.RequestDate) As UserField9,        
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),dbo.Requests.RequestDate) As UserField10        
        FROM         dbo.sysroEmployeeGroups with (nolock) INNER JOIN        
                              dbo.requests with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.requests.IDEmployee AND dbo.requests.RequestDate >= dbo.sysroEmployeeGroups.BeginDate AND         
                              dbo.requests.RequestDate <= dbo.sysroEmployeeGroups.EndDate INNER JOIN        
                              dbo.EmployeeContracts with (nolock) ON dbo.requests.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.requests.RequestDate >= dbo.EmployeeContracts.BeginDate AND         
                              dbo.requests.RequestDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN        
                              dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
 							 left join (SELECT IDRequest, COUNT(*) NumeroDias FROM sysroRequestDays GROUP BY IDRequest) aux on aux.IDRequest = requests.id
        WHERE dbo.GetRequestPassportPermission(@idpassport, Requests.ID) > 1        
          AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.requests.RequestDate between @initialDate and @endDate        
          and dbo.Requests.RequestType in (select idRequest from @requestTypeFilter)      
        GROUP BY  dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.requests.RequestDate), YEAR(dbo.requests.RequestDate), dbo.requests.ID,         
                              dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.requests.RequestDate), dbo.requests.RequestDate, CONVERT(nvarchar(8), dbo.requests.RequestDate, 108),         
                              dbo.requests.RequestDate, dbo.EmployeeContracts.IDContract,
							  dbo.Employees.Name,         
                              (DATEPART(dw, dbo.requests.RequestDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.requests.RequestType, dbo.requests.Status, dbo.requests.Date1, dbo.requests.FromTime, dbo.requests.Date2, dbo.requests.ToTime, dbo.requests.IDCause,CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldName, '')),CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldValue, '')), dbo.requests.IDShift,CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')), dbo.sysroEmployeeGroups.Path,         
                              dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,         
          dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate        , aux.NumeroDias
        HAVING      (dbo.Requests.RequestType IN(1,2,3,4,5,6,7,8,9,11,14,15))
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI_Actions] WHERE IDPath = 'Share' and IDGUIPath = 'Portal\General\Genius')
	insert into [dbo].sysroGUI_Actions(IDPath,IDGUIPath,LanguageTag,RequieredFeatures,RequieredFunctionalities,AfterFunction,CssClass,Section,ElementIndex,AppearsOnPopup)
	values ('Share','Portal\General\Genius','tbShare',NULL,NULL,'shareCurrentGenius()','btnShare',0,4,0)
GO

update sysroGUI_Actions set ElementIndex = 5 where IDPath = 'DeleteGenius' and IDGUIPath = 'Portal\General\Genius'
GO

--Movil
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [Alias] = 'sysroMobile') AND NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [FieldName] = 'Móvil' )
INSERT INTO [dbo].[sysroUserFields] ([FieldName],[FieldType],[Used],[AccessLevel] ,[Pos],[Category],[ListValues],[Type],[Description],[AccessValidation],[History],[RequestPermissions],[RequestCriteria],[isSystem],[Alias],[DocumentTemplateId]) 
VALUES('Móvil',0,1,0,null,'VisualTime','',0,'',0,0,1,'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>',1,'sysroMobile',null)
ELSE
UPDATE [dbo].[sysroUserFields] set Fieldtype=0, Alias='sysroMobile', Category='VisualTime', isSystem=1 where Fieldname = 'Móvil'
GO


--Genero
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [Alias] = 'sysroGender') AND NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [FieldName] = 'Género' )
INSERT INTO [dbo].[sysroUserFields] ([FieldName],[FieldType],[Used],[AccessLevel] ,[Pos],[Category],[ListValues],[Type],[Description],[AccessValidation],[History],[RequestPermissions],[RequestCriteria],[isSystem],[Alias],[DocumentTemplateId]) 
VALUES('Género',5,1,2,null,'Igualdad','Hombre~Mujer',0,'',0,0,0,'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>',1,'sysroGender',null)
ELSE
UPDATE [dbo].[sysroUserFields] set  Alias='sysroGender',Category='Igualdad', isSystem=1 where Fieldname = 'Género'
GO

--Grupo cotización
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [Alias] = 'sysroQuoteGroup') AND NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [FieldName] = 'Grupo de cotización' )
INSERT INTO [dbo].[sysroUserFields] ([FieldName],[FieldType],[Used],[AccessLevel] ,[Pos],[Category],[ListValues],[Type],[Description],[AccessValidation],[History],[RequestPermissions],[RequestCriteria],[isSystem],[Alias],[DocumentTemplateId]) 
VALUES('Grupo de cotización',5,1,2,null,'Igualdad','1~2~3~4~5~6~7~8~9~19~11',0,'',0,1,0,'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>',1,'sysroQuoteGroup',null)
ELSE
UPDATE [dbo].[sysroUserFields] set  Alias='sysroQuoteGroup',Category='Igualdad', isSystem=1 where Fieldname = 'Grupo de cotización'
GO

--Categoría profesional
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [Alias] = 'sysroProfessionalCategory') AND NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [FieldName] = 'Categoría profesional' )
INSERT INTO [dbo].[sysroUserFields] ([FieldName],[FieldType],[Used],[AccessLevel] ,[Pos],[Category],[ListValues],[Type],[Description],[AccessValidation],[History],[RequestPermissions],[RequestCriteria],[isSystem],[Alias],[DocumentTemplateId]) 
VALUES('Categoría profesional',5,1,2,null,'Igualdad','Ingenieros y Licenciados~Ingenieros Técnicos, Peritos y Ayudantes Titulados~Jefes Administrativos y de Taller~Ayudantes no Titulados~Oficiales Administrativos~Subalternos~Auxiliares Administrativos~Oficiales de primera y segunda~Oficiales de tercera y Especialistas~Peones~Trabajadores menores de 18 años',0,'',0,1,0,'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>',1,'sysroProfessionalCategory',null)
ELSE
UPDATE [dbo].[sysroUserFields] set  Alias='sysroProfessionalCategory',Category='Igualdad', isSystem=1 where Fieldname = 'Categoría profesional'
GO

--Puesto
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [Alias] = 'sysroPosition') AND NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [FieldName] = 'Puesto' )
INSERT INTO [dbo].[sysroUserFields] ([FieldName],[FieldType],[Used],[AccessLevel] ,[Pos],[Category],[ListValues],[Type],[Description],[AccessValidation],[History],[RequestPermissions],[RequestCriteria],[isSystem],[Alias],[DocumentTemplateId]) 
VALUES('Puesto',0,1,1,null,'Igualdad','',0,'',0,1,0,'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>',1,'sysroPosition',null)
ELSE
UPDATE [dbo].[sysroUserFields] set  Alias='sysroPosition',Category='Igualdad', isSystem=1 where Fieldname = 'Puesto'
GO

--Salario total anual
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [Alias] = 'sysroTotalSalary') AND NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [FieldName] = 'Salario total anual' )
INSERT INTO [dbo].[sysroUserFields] ([FieldName],[FieldType],[Used],[AccessLevel] ,[Pos],[Category],[ListValues],[Type],[Description],[AccessValidation],[History],[RequestPermissions],[RequestCriteria],[isSystem],[Alias],[DocumentTemplateId]) 
VALUES('Salario total anual',3,1,2,null,'Igualdad','',0,'',0,1,0,'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>',1,'sysroTotalSalary',null)
ELSE
UPDATE [dbo].[sysroUserFields] set  Alias='sysroTotalSalary',Category='Igualdad', isSystem=1 where Fieldname = 'Salario total anual'
GO

--Salario base anual
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [Alias] = 'sysroBaseSalary') AND NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [FieldName] = 'Salario base anual' )
INSERT INTO [dbo].[sysroUserFields] ([FieldName],[FieldType],[Used],[AccessLevel] ,[Pos],[Category],[ListValues],[Type],[Description],[AccessValidation],[History],[RequestPermissions],[RequestCriteria],[isSystem],[Alias],[DocumentTemplateId]) 
VALUES('Salario base anual',3,1,2,null,'Igualdad','',0,'',0,1,0,'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>',1,'sysroBaseSalary',null)
ELSE
UPDATE [dbo].[sysroUserFields] set  Alias='sysroBaseSalary',Category='Igualdad', isSystem=1 where Fieldname = 'Salario base anual'
GO

--Complementos salariales anuales
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [Alias] = 'sysroSalarySupp') AND NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [FieldName] = 'Complementos salariales anuales' )
INSERT INTO [dbo].[sysroUserFields] ([FieldName],[FieldType],[Used],[AccessLevel] ,[Pos],[Category],[ListValues],[Type],[Description],[AccessValidation],[History],[RequestPermissions],[RequestCriteria],[isSystem],[Alias],[DocumentTemplateId]) 
VALUES('Complementos salariales anuales',3,1,2,null,'Igualdad','',0,'',0,1,0,'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>',1,'sysroSalarySupp',null)
ELSE
UPDATE [dbo].[sysroUserFields] set  Alias='sysroSalarySupp',Category='Igualdad', isSystem=1 where Fieldname = 'Complementos salariales anuales'
GO

--Percepciones extrasalariales
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [Alias] = 'sysroExtraSalary') AND NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [FieldName] = 'Percepciones extrasalariales' )
INSERT INTO [dbo].[sysroUserFields] ([FieldName],[FieldType],[Used],[AccessLevel] ,[Pos],[Category],[ListValues],[Type],[Description],[AccessValidation],[History],[RequestPermissions],[RequestCriteria],[isSystem],[Alias],[DocumentTemplateId]) 
VALUES('Percepciones extrasalariales',3,1,2,null,'Igualdad','',0,'',0,1,0,'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>',1,'sysroExtraSalary',null)
ELSE
UPDATE [dbo].[sysroUserFields] set  Alias='sysroExtraSalary', Category='Igualdad', isSystem=1 where Fieldname = 'Percepciones extrasalariales'
GO

--Percepciones por horas extraordinarias y horas complementarias
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [Alias] = 'sysroEarningsOverTime') AND NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [FieldName] = 'Percepciones por extraordinarias y complementarias' )
INSERT INTO [dbo].[sysroUserFields] ([FieldName],[FieldType],[Used],[AccessLevel] ,[Pos],[Category],[ListValues],[Type],[Description],[AccessValidation],[History],[RequestPermissions],[RequestCriteria],[isSystem],[Alias],[DocumentTemplateId]) 
VALUES('Percepciones por extraordinarias y complementarias',3,1,2,null,'Igualdad','',0,'',0,1,0,'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>',1,'sysroEarningsOverTime',null)
ELSE
UPDATE [dbo].[sysroUserFields] set  Alias='sysroEarningsOverTime', Category='Igualdad', isSystem=1 where Fieldname = 'Percepciones por extraordinarias y complementarias'
GO


-- Vistas CC
IF NOT EXISTS (SELECT 1 FROM [dbo].[GeniusViews] WHERE [Name] = 'TotalsByCostcentersAndCause' and [IdPassport] = 0)
	INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature]) 
		VALUES (0, N'TotalsByCostcentersAndCause', N'', N'B', 1, CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'', N'0', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"ManualCenter","caption":"Manual"},{"uniqueName":"DefaultCenter","caption":"Centro por defecto"},{"uniqueName":"Field1","caption":"Campo1"},{"uniqueName":"Field2","caption":"Campo2"},{"uniqueName":"Field3","caption":"Campo3"},{"uniqueName":"Field4","caption":"Campo4"},{"uniqueName":"Field5","caption":"Campo5"}],"rows":[{"uniqueName":"CenterName","caption":"Centro de coste"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"CauseName","caption":"Justificación"}],"measures":[{"uniqueName":"Value_ToHours","formula":"sum(\"Value\")","caption":"Value_ToHours"},{"uniqueName":"TotalCost","aggregation":"sum","caption":"Coste total","format":"-1bnvddxa7n2r00"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"columns"}},"formats":[{"name":"-1bnvddxa7n2r00","currencySymbol":"€"}],"creationDate":"2021-05-04T07:35:28.270Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":true,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}', N'', N'')
ELSE
    UPDATE [dbo].[GeniusViews] SET [CubeLayout] =N'{"slice":{"reportFilters":[{"uniqueName":"ManualCenter","caption":"Manual"},{"uniqueName":"DefaultCenter","caption":"Centro por defecto"},{"uniqueName":"Field1","caption":"Campo1"},{"uniqueName":"Field2","caption":"Campo2"},{"uniqueName":"Field3","caption":"Campo3"},{"uniqueName":"Field4","caption":"Campo4"},{"uniqueName":"Field5","caption":"Campo5"}],"rows":[{"uniqueName":"CenterName","caption":"Centro de coste"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"CauseName","caption":"Justificación"}],"measures":[{"uniqueName":"Value_ToHours","formula":"sum(\"Value\")","caption":"Value_ToHours"},{"uniqueName":"TotalCost","aggregation":"sum","caption":"Coste total","format":"-1bnvddxa7n2r00"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"columns"}},"formats":[{"name":"-1bnvddxa7n2r00","currencySymbol":"€"}],"creationDate":"2021-05-04T07:35:28.270Z"}' WHERE [Name] = 'TotalsByCostcentersAndCause' and [IdPassport] = 0
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[GeniusViews] WHERE [Name] = 'CostCenterAndCauseByEmployee' and [IdPassport] = 0)
	INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature]) 
		VALUES (0, N'CostCenterAndCauseByEmployee', N'', N'B', 2, CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'', N'0', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"CenterName"},{"uniqueName":"EmployeeName"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"CauseName"}],"measures":[{"uniqueName":"Value_ToHours","formula":"sum(\"Value\")","caption":"Value_ToHours"},{"uniqueName":"TotalCost","aggregation":"sum","format":"-12v8uv10sh6r0"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"columns"}},"formats":[{"name":"-12v8uv10sh6r0","currencySymbol":"€"}],"creationDate":"2021-04-23T07:26:57.970Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":true,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}', N'', N'')
ELSE
    UPDATE [dbo].[GeniusViews] SET [CubeLayout] =N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"CenterName"},{"uniqueName":"EmployeeName"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"CauseName"}],"measures":[{"uniqueName":"Value_ToHours","formula":"sum(\"Value\")","caption":"Value_ToHours"},{"uniqueName":"TotalCost","aggregation":"sum","format":"-12v8uv10sh6r0"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"columns"}},"formats":[{"name":"-12v8uv10sh6r0","currencySymbol":"€"}],"creationDate":"2021-04-23T07:26:57.970Z"}' WHERE [Name] = 'CostCenterAndCauseByEmployee' and [IdPassport] = 0
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[GeniusViews] WHERE [Name] = 'CostCenterRequests' and [IdPassport] = 0)
	INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature]) 
		VALUES (0, N'CostCenterRequests', N'', N'B', 3, CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'', N'0', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"CurrentEmployee"},{"uniqueName":"IDContract"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"PunchDate"},{"uniqueName":"NameCenter"},{"uniqueName":"CommentsRequest"},{"uniqueName":"ApproveRefusePassport"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Value","aggregation":"sum"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"rows"}},"creationDate":"2021-04-22T14:07:23.838Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}', N'', N'')
ELSE
    UPDATE [dbo].[GeniusViews] SET [CubeLayout] =N'{"slice":{"reportFilters":[{"uniqueName":"CurrentEmployee"},{"uniqueName":"IDContract"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"PunchDate"},{"uniqueName":"NameCenter"},{"uniqueName":"CommentsRequest"},{"uniqueName":"ApproveRefusePassport"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Value","aggregation":"sum"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"rows"}},"creationDate":"2021-04-22T14:07:23.838Z"}' WHERE [Name] = 'CostCenterRequests' and [IdPassport] = 0
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[GeniusViews] WHERE [Name] = 'ActualCostCenter' and [IdPassport] = 0)
	INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature]) 
		VALUES (0, N'ActualCostCenter', N'', N'B', 4, CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'', N'0', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"CenterName"},{"uniqueName":"CostDateTime.Year","caption":"CostDateTime.Año"},{"uniqueName":"CostDateTime.Month","caption":"CostDateTime.Mes"},{"uniqueName":"CostDateTime.Day","caption":"CostDateTime.Día"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"KeyView","aggregation":"count"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"rows"}},"creationDate":"2021-04-22T14:12:27.914Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":true,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}', N'', N'')
ELSE
    UPDATE [dbo].[GeniusViews] SET [CubeLayout] =N'{"slice":{"reportFilters":[{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"CenterName"},{"uniqueName":"CostDateTime.Year","caption":"CostDateTime.Año"},{"uniqueName":"CostDateTime.Month","caption":"CostDateTime.Mes"},{"uniqueName":"CostDateTime.Day","caption":"CostDateTime.Día"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"KeyView","aggregation":"count"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"rows"}},"creationDate":"2021-04-22T14:12:27.914Z"}' WHERE [Name] = 'ActualCostCenter' and [IdPassport] = 0
GO


-- Vistas Acc
IF NOT EXISTS (SELECT 1 FROM [dbo].[GeniusViews] WHERE [Name] = 'accessByEmployee' and [IdPassport] = 0)
	INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature]) 
		VALUES (0, N'accessByEmployee', N'', N'A', 1, CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'', N'0', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"CurrentEmployee"},{"uniqueName":"IDContract"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date.Year","caption":"Date.Año"},{"uniqueName":"Date.Month","caption":"Date.Mes"},{"uniqueName":"Date.Day","caption":"Date.Día"},{"uniqueName":"Time"},{"uniqueName":"TerminalName"},{"uniqueName":"InvalidDesc"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Value","aggregation":"sum"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"rows"}},"creationDate":"2021-04-22T11:13:12.165Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}', N'', N'')
ELSE
    UPDATE [dbo].[GeniusViews] SET [CubeLayout] =N'{"slice":{"reportFilters":[{"uniqueName":"CurrentEmployee"},{"uniqueName":"IDContract"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date.Year","caption":"Date.Año"},{"uniqueName":"Date.Month","caption":"Date.Mes"},{"uniqueName":"Date.Day","caption":"Date.Día"},{"uniqueName":"Time"},{"uniqueName":"TerminalName"},{"uniqueName":"InvalidDesc"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Value","aggregation":"sum"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"rows"}},"creationDate":"2021-04-22T11:13:12.165Z"}' WHERE [Name] = 'accessByEmployee' and [IdPassport] = 0
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[GeniusViews] WHERE [Name] = 'authorizationsAccessByEmployee' and [IdPassport] = 0)
	INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature]) 
		VALUES (0, N'authorizationsAccessByEmployee', N'', N'A', 2, CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'', N'0', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"CurrentEmployee"},{"uniqueName":"IDContract"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"AuthorizationName"},{"uniqueName":"AccessPeriodName"},{"uniqueName":"ZoneName"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"IDEmployee","aggregation":"count"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"rows"}},"creationDate":"2021-04-22T13:09:37.009Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}', N'', N'')
ELSE
    UPDATE [dbo].[GeniusViews] SET [CubeLayout] =N'{"slice":{"reportFilters":[{"uniqueName":"CurrentEmployee"},{"uniqueName":"IDContract"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"AuthorizationName"},{"uniqueName":"AccessPeriodName"},{"uniqueName":"ZoneName"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"IDEmployee","aggregation":"count"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"rows"}},"creationDate":"2021-04-22T13:09:37.009Z"}' WHERE [Name] = 'authorizationsAccessByEmployee' and [IdPassport] = 0
GO


-- Vistas Tareas
IF NOT EXISTS (SELECT 1 FROM [dbo].[GeniusViews] WHERE [Name] = 'tasksByEmployee' and [IdPassport] = 0)
	INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature]) 
		VALUES (0, N'tasksByEmployee', N'', N'T', 1, CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'', N'0', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"CenterName"},{"uniqueName":"Project"},{"uniqueName":"TaskName"},{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"Date_Year"},{"uniqueName":"Date_Month"},{"uniqueName":"Date_Day"}],"measures":[{"uniqueName":"Value_ToHours","formula":"sum(\"Value\")","caption":"Value_ToHours"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"rows"}},"creationDate":"2021-04-22T14:19:18.308Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}', N'', N'')
ELSE
    UPDATE [dbo].[GeniusViews] SET [CubeLayout] =N'{"slice":{"reportFilters":[{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"CenterName"},{"uniqueName":"Project"},{"uniqueName":"TaskName"},{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"Date_Year"},{"uniqueName":"Date_Month"},{"uniqueName":"Date_Day"}],"measures":[{"uniqueName":"Value_ToHours","formula":"sum(\"Value\")","caption":"Value_ToHours"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"rows"}},"creationDate":"2021-04-22T14:19:18.308Z"}' WHERE [Name] = 'tasksByEmployee' and [IdPassport] = 0
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[GeniusViews] WHERE [Name] = 'taskRequests' and [IdPassport] = 0)
	INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature]) 
		VALUES (0, N'taskRequests', N'', N'T', 2, CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'', N'0', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'{"slice":{"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date.Year","caption":"Date.Año"},{"uniqueName":"Date.Month","caption":"Date.Mes"},{"uniqueName":"Date.Day","caption":"Date.Día"},{"uniqueName":"TypeDesc"},{"uniqueName":"NameTask1"},{"uniqueName":"NameTask2"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"KeyView","aggregation":"count"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"columns"}},"creationDate":"2021-04-22T14:23:17.823Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}', N'', N'')
ELSE
    UPDATE [dbo].[GeniusViews] SET [CubeLayout] =N'{"slice":{"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date.Year","caption":"Date.Año"},{"uniqueName":"Date.Month","caption":"Date.Mes"},{"uniqueName":"Date.Day","caption":"Date.Día"},{"uniqueName":"TypeDesc"},{"uniqueName":"NameTask1"},{"uniqueName":"NameTask2"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"KeyView","aggregation":"count"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"columns"}},"creationDate":"2021-04-22T14:23:17.823Z"}' WHERE [Name] = 'taskRequests' and [IdPassport] = 0
GO



 UPDATE sysroParameters SET Data='514' WHERE ID='DBVersion'
 GO

 