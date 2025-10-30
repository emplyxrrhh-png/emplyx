--Añadimos hora de entrada de los visitantes presentes
 ALTER view [dbo].[sysrovwCurrentVisistsStatus]
    as
    select v.IDVisitor, v.Name VisitorName, e.ID,e.Name EmployeeName, euf.Value, LastMoves.DatePunch
    from [dbo].[Visit_Visitor] vv
    join [dbo].[Visitor] v on vv.IDVisitor = v.IDVisitor
    join [dbo].[Visit] vi on vv.IDVisit = vi.IDVisit
    join [dbo].[Employees] e on vi.IDEmployee = e.ID
    join [dbo].[EmployeeUserFieldValues] euf on e.ID = euf.IDEmployee
	join (SELECT ROW_NUMBER() OVER(PARTITION BY IDVisit, idvisitor ORDER BY idvisit, idvisitor, DatePunch DESC) AS "Row Number", * FROM Visit_Visitor_Punch) AS LastMoves on LastMoves.IDVisit = vi.IDVisit and LastMoves.IDVisitor = vv.IDVisitor and [Row Number] = 1
    where FieldName like (SELECT FieldName FROM [dbo].[sysroUserFields]
                          WHERE [Description] like 'Info.EmergencyPointInfo') 
    and vi.Status = 1 and LastMoves.DatePunch >= DateAdd(n, -2880, GETDATE()) and LastMoves.Action = 'IN'
GO


-- SDK Fichajes
INSERT [dbo].[ExportGuides] ([ID], [Name], [ProfileMask], [ProfileType], [Mode], [ProfileName], [Destination], [ExportFileName], 
                                               [ExportFileType], [Separator], [StartCalculDay], [StartExecutionHour], [LastLog], [NextExecution], [Field_1], 
                                               [Field_2], [Field_3], [Field_4], [Field_5], [Field_6], [IntervalMinutes], [DisplayParameters], [Scheduler], [EmployeeFilter], 
                                               [RequieredFunctionalities], [FeatureAliasID]) 
VALUES (9011, N'Fichajes con plantilla', N'adv_Punches_SDK', 8, 1, N'', N'0', NULL, 1, NULL, NULL, NULL, N'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, N'1,1,0@1@1', NULL, NULL, N'Employees.DataLink.Exports.Punches', N'Employees')
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(1640,1620,'Employees.DataLink.Exports.Punches','Exportación avanzada de fichajes','','U','RWA',NULL,NULL,1)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 1640, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 1620
GO

--EXPORTACION AVANZADA DE PLANIFICACIÓN V2
INSERT INTO [dbo].[ExportGuides]  VALUES
 (9012,N'Exportación avanzada de planificación',N'CalendarLinkCellLayout',12,1,NULL,NULL,NULL,1,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'0,1,0@0@0',NULL,NULL,N'Calendar.DataLink.Exports.Calendar',N'Calendar')
GO

--IMPORTACION  AVANZADA DE PLANIFICACIÓN V2
INSERT INTO [dbo].[ImportGuides] VALUES
 (14,N'Carga de Planificación Avanzada',0,0,0,'','',N'',0,'',N'Calendar.DataLink.Imports.Schedule',N'Calendar',0)
GO


EXEC dbo.sysro_GenerateAllPermissionsOverFeatures
GO

--  NUEVOS CAMPOS PARA INDICAR APLICACIONES PERMITIDAS DEL PASSPORT
ALTER TABLE [dbo].[sysroPassports] ADD  
	PhotoRequiered bit NULL DEFAULT(1),
	LocationRequiered bit NULL DEFAULT(1)
GO

UPDATE [dbo].[sysroPassports] SET PhotoRequiered = 1 WHERE PhotoRequiered is null
GO

UPDATE [dbo].[sysroPassports] SET LocationRequiered = 1 WHERE LocationRequiered is null
GO

ALTER PROCEDURE [dbo].[WebLogin_Passports_Select] 
  (
  	@idPassport int
  )
  AS
     SELECT ID,
     	IDParentPassport, GroupType, Name, Description,	Email, IDUser, IDEmployee, IDLanguage, dbo.GetPassportLevelOfAuthority(@idPassport) AS LevelOfAuthority,
     	ConfData, AuthenticationMerge, StartDate, ExpirationDate, [State], EnabledVTDesktop, EnabledVTPortal, EnabledVTSupervisor, EnabledVTPortalApp ,
  		EnabledVTSupervisorApp, NeedValidationCode, TimeStampValidationCode, ValidationCode, EnabledVTVisits, EnabledVTVisitsApp, 
		PhotoRequiered, LocationRequiered, LicenseAccepted, IsSupervisor
     FROM sysroPassports
     WHERE ID = @idPassport
     	
 RETURN
GO

 ALTER PROCEDURE [dbo].[WebLogin_Passports_SelectByIDEmployee] 
 (
    	@idEmployee int
 )
 AS
 SELECT ID, IDParentPassport, GroupType, Name, Description, Email, IDUser, IDEmployee, IDLanguage, LevelOfAuthority, ConfData, AuthenticationMerge, StartDate, ExpirationDate,
   		[State], EnabledVTDesktop, EnabledVTPortal,	EnabledVTSupervisor, EnabledVTPortalApp, EnabledVTSupervisorApp, NeedValidationCode, TimeStampValidationCode, ValidationCode,
 		EnabledVTVisits, EnabledVTVisitsApp, PhotoRequiered, LocationRequiered, LicenseAccepted, IsSupervisor
   		
 FROM sysroPassports
 WHERE IDEmployee = @idEmployee
    	
 RETURN
GO

 ALTER PROCEDURE [dbo].[WebLogin_Passports_SelectByIDUser]
 (
    	@idUser int
 )
 AS
 SELECT ID, IDParentPassport, GroupType, Name, Description, Email, IDUser, IDEmployee, IDLanguage, LevelOfAuthority, ConfData, AuthenticationMerge, StartDate, ExpirationDate,
   		[State], EnabledVTDesktop, EnabledVTPortal, EnabledVTSupervisor, EnabledVTPortalApp, EnabledVTSupervisorApp, NeedValidationCode, TimeStampValidationCode, ValidationCode,
 		EnabledVTVisits, EnabledVTVisitsApp, PhotoRequiered, LocationRequiered, LicenseAccepted, IsSupervisor
   		
 FROM sysroPassports
 WHERE IDUser = @idUser
    	
 RETURN
GO

 ALTER PROCEDURE [dbo].[WebLogin_Passports_SelectAll] 
    	
 AS
 SELECT ID, IDParentPassport, GroupType, Name, Description, Email, IDUser, IDEmployee, IDLanguage, LevelOfAuthority, ConfData, AuthenticationMerge, StartDate, ExpirationDate,
		[State], EnabledVTDesktop, EnabledVTPortal, EnabledVTSupervisor, EnabledVTPortalApp, EnabledVTSupervisorApp, NeedValidationCode, TimeStampValidationCode, ValidationCode,
		EnabledVTVisits, EnabledVTVisitsApp, PhotoRequiered, LocationRequiered, LicenseAccepted, IsSupervisor
		
 FROM sysroPassports
    	
 RETURN
GO

 ALTER PROCEDURE [dbo].[WebLogin_Passports_SelectAllEmployee] 
    	
 AS
 SELECT ID, IDParentPassport, GroupType, Name, Description, Email, IDUser, IDEmployee, IDLanguage, LevelOfAuthority, ConfData, AuthenticationMerge, StartDate, ExpirationDate,
   		[State], EnabledVTDesktop, EnabledVTPortal, EnabledVTSupervisor, EnabledVTPortalApp, EnabledVTSupervisorApp, NeedValidationCode, TimeStampValidationCode, ValidationCode,
 		EnabledVTVisits, EnabledVTVisitsApp, PhotoRequiered, LocationRequiered, LicenseAccepted, IsSupervisor
 FROM sysroPassports
 WHERE IDEmployee IS NOT NULL
    	
 RETURN
GO

 ALTER PROCEDURE [dbo].[WebLogin_Passports_SelectAllUser] 
    	
 AS
 SELECT ID, IDParentPassport, GroupType, Name, Description, Email, IDUser, IDEmployee, IDLanguage, LevelOfAuthority, ConfData, AuthenticationMerge, StartDate, ExpirationDate,
   		[State], EnabledVTDesktop, EnabledVTPortal, EnabledVTSupervisor, EnabledVTPortalApp, EnabledVTSupervisorApp, NeedValidationCode, TimeStampValidationCode, ValidationCode,
 		EnabledVTVisits, EnabledVTVisitsApp, PhotoRequiered, LocationRequiered, LicenseAccepted, IsSupervisor
 FROM sysroPassports
 WHERE IDUser IS NOT NULL
    	
 RETURN
GO

  ALTER PROCEDURE [dbo].[WebLogin_Passports_Insert] 
     	(
     		@id int OUTPUT, @idParentPassport int, @groupType varchar(1), @name nvarchar(50), @description nvarchar(100), @email nvarchar(100), @idUser int, @idEmployee int, @idLanguage int,
     		@levelOfAuthority tinyint, @ConfData text, @AuthenticationMerge nvarchar(50), @StartDate smalldatetime, @ExpirationDate smalldatetime, @State smallint, @EnabledVTDesktop bit,
   		@EnabledVTPortal bit, @EnabledVTSupervisor bit, @EnabledVTPortalApp bit , @EnabledVTSupervisorApp bit , @NeedValidationCode bit , @TimeStampValidationCode smalldatetime ,
   		@ValidationCode nvarchar(100), @EnabledVTVisits bit, @EnabledVTVisitsApp bit, @PhotoRequiered bit, @LocationRequiered bit, @LicenseAccepted bit, @IsSupervisor bit
     	)
     AS
     	INSERT INTO sysroPassports (
     		IDParentPassport, GroupType, Name, Description, Email, IDUser, IDEmployee, IDLanguage, LevelOfAuthority, ConfData, AuthenticationMerge, StartDate, ExpirationDate, [State],
    		EnabledVTDesktop, EnabledVTPortal, EnabledVTSupervisor, EnabledVTPortalApp, EnabledVTSupervisorApp, NeedValidationCode, TimeStampValidationCode, ValidationCode, 
			EnabledVTVisits, EnabledVTVisitsApp, PhotoRequiered, LocationRequiered, LicenseAccepted, IsSupervisor
     	)
     	VALUES (
     		@idParentPassport, @groupType, @name, @description, @email, @idUser, @idEmployee, @idLanguage, @levelOfAuthority, @ConfData, @AuthenticationMerge, @StartDate, @ExpirationDate, @State, 
			@EnabledVTDesktop, @EnabledVTPortal, @EnabledVTSupervisor, @EnabledVTPortalApp, @EnabledVTSupervisorApp, @NeedValidationCode, @TimeStampValidationCode, @ValidationCode, 
			@EnabledVTVisits, @EnabledVTVisitsApp, @PhotoRequiered, @LocationRequiered, @LicenseAccepted, @IsSupervisor
     	)
     	
     	SELECT @id = @@IDENTITY
     	
  	IF @groupType = 'U' 
    	BEGIN
  		-- 0 / Actualizar los permisos del grupo y sus hijos
    		insert into RequestPassportPermissionsPending Values(0, @id)
    	END
 RETURN
GO

ALTER PROCEDURE [dbo].[WebLogin_Passports_Update] 
   (@id int, @idParentPassport int, @groupType varchar(1), @name nvarchar(50), @description nvarchar(4000), @email nvarchar(100), @idUser int, @idEmployee int, @idLanguage int,
   @levelOfAuthority tinyint, @ConfData text, @AuthenticationMerge nvarchar(50), @StartDate smalldatetime, @ExpirationDate smalldatetime, @State smallint, @EnabledVTDesktop bit,
   @EnabledVTPortal bit, @EnabledVTSupervisor bit, @EnabledVTPortalApp bit, @EnabledVTSupervisorApp bit, @NeedValidationCode bit, @TimeStampValidationCode smalldatetime, @ValidationCode  nvarchar(100),
   @EnabledVTVisits bit, @EnabledVTVisitsApp bit, @PhotoRequiered bit, @LocationRequiered bit, @LicenseAccepted bit, @IsSupervisor bit)
  AS
   IF @groupType <> 'U' 
   	BEGIN
   		SET @levelOfAuthority = NULL
   	END
   UPDATE sysroPassports SET
   	IDParentPassport = @idParentPassport, GroupType = @groupType, Name = @name, Description = @description, Email = @email, IDUser = @idUser, IDEmployee = @idEmployee, IDLanguage = @idLanguage,
   	LevelOfAuthority = @levelOfAuthority, ConfData = @ConfData, AuthenticationMerge = @AuthenticationMerge, StartDate = @StartDate, ExpirationDate = @ExpirationDate, [State] = @State, 
  	EnabledVTDesktop = @EnabledVTDesktop, EnabledVTPortal = @EnabledVTPortal, EnabledVTSupervisor = @EnabledVTSupervisor, EnabledVTPortalApp = @EnabledVTPortalApp,	EnabledVTSupervisorApp = @EnabledVTSupervisorApp,
  	NeedValidationCode = @NeedValidationCode, TimeStampValidationCode = @TimeStampValidationCode, ValidationCode  = @ValidationCode, EnabledVTVisits = @EnabledVTVisits,
  	EnabledVTVisitsApp = @EnabledVTVisitsApp, PhotoRequiered = @PhotoRequiered, LocationRequiered = @LocationRequiered, LicenseAccepted = @LicenseAccepted, IsSupervisor = @IsSupervisor
   	
   WHERE ID = @id
  IF @groupType = 'U' 
    	BEGIN
  		-- 1 / Actualizar los permisos del grupo y sus hijos
  		IF NOT EXISTS ( SELECT ID FROM RequestPassportPermissionsPending WHERE IDAction = 1 AND IDObject = @id)
  			insert into RequestPassportPermissionsPending Values(1, @id)
    	END
    	 
   RETURN
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTPortal.RequiereFineLocation')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTPortal.RequiereFineLocation','1')
GO



UPDATE dbo.sysroParameters SET Data='418' WHERE ID='DBVersion'
GO
