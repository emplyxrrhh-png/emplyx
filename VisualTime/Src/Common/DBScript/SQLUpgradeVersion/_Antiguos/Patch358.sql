
ALTER VIEW [dbo].[sysroAccessCube]
AS
SELECT        dbo.Punches.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee, 
                         dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, CONVERT(nvarchar(8), 
                         dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day, MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Year, 
                         (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, 
                         dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName, dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL 
                         THEN 0 ELSE 1 END AS IsInvalid, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails) AS Details, dbo.sysroEmployeeGroups.Path, 
                         dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, 
                         dbo.sysroEmployeeGroups.IDGroup
FROM            dbo.sysroEmployeeGroups INNER JOIN
                         dbo.Punches ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee AND dbo.Punches.ShiftDate BETWEEN 
                         dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate INNER JOIN
                         dbo.EmployeeContracts ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND 
                         dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                         dbo.Zones ON dbo.Punches.IDZone = dbo.Zones.ID LEFT OUTER JOIN
                         dbo.Terminals ON dbo.Punches.IDTerminal = dbo.Terminals.ID LEFT OUTER JOIN
                         dbo.Employees ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID, 
                         dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108), 
                         dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name, 
                         (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name, 
                         CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(NVARCHAR(4000), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path, 
                         dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, 
                         dbo.sysroEmployeeGroups.IDGroup
HAVING        (dbo.Punches.Type = 5) OR
                         (dbo.Punches.Type = 6) OR
                         (dbo.Punches.Type = 7)

GO
 
ALTER TABLE dbo.EmployeeGroups ADD
	IsTransfer tinyint NOT NULL CONSTRAINT DF_EmployeeGroups_IsTransfer DEFAULT 0
GO

insert into dbo.sysroLiveAdvancedParameters(ParameterName,Value) values('TransferAvailable','False')
GO

INSERT INTO [dbo].[sysroNotificationTypes]([ID],[Name],[Description],[Scheduler],[Feature],[FeatureType],[OnlySystem]) VALUES (43,'Mobility Update',NULL,10,'Employees.GroupMobility','U',0)
GO
INSERT INTO [dbo].[sysroNotificationTypes]([ID],[Name],[Description],[Scheduler],[Feature],[FeatureType],[OnlySystem]) VALUES (44,'Mobility Execution',NULL,360,'Employees.GroupMobility','U',0)
GO


CREATE TABLE [dbo].[Visit](
	[IDVisit] [nvarchar](40) NOT NULL,
	[BeginDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[Repeat] [bit] NOT NULL,
	[IDEmployee] [int] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[Status] [smallint] NOT NULL,
	[Modified] [datetime] NOT NULL,
 	[Deleted] [bit] NOT NULL,
CONSTRAINT [PK_Visits] PRIMARY KEY CLUSTERED 
(
	[IDVisit] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Visit] ADD  CONSTRAINT [DF_Visit_Repeat]  DEFAULT ((0)) FOR [Repeat]
GO

ALTER TABLE [dbo].[Visit] ADD  CONSTRAINT [DF_Visit_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO

ALTER TABLE [dbo].[Visit] ADD  CONSTRAINT [DF_Visit_Modified]  DEFAULT (getdate()) FOR [Modified]
GO
--==============================================================================================================================

CREATE TABLE [dbo].[Visit_Fields](
	[IDField] [nvarchar](40) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Required] [smallint] NOT NULL,
	[position] [smallint] NOT NULL,
	[deleted] [smallint] NOT NULL,
 CONSTRAINT [PK_Visits_Fields] PRIMARY KEY CLUSTERED 
(
	[IDField] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Visit_Fields] ADD  CONSTRAINT [DF_Visit_Fields_position]  DEFAULT ((1)) FOR [position]
GO

ALTER TABLE [dbo].[Visit_Fields] ADD  CONSTRAINT [DF_Visit_Fields_deleted]  DEFAULT ((0)) FOR [deleted]
GO

--==============================================================================================================================

CREATE TABLE [dbo].[Visit_Fields_Value](
	[IDField] [nvarchar](40) NOT NULL,
	[IDVisit] [nvarchar](40) NOT NULL,
	[Value] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_Visits_Fields_Values] PRIMARY KEY CLUSTERED 
(
	[IDField] ASC,
	[IDVisit] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Visit_Fields_Value]  WITH CHECK ADD  CONSTRAINT [FK_Visit_Fields_Value_Visit] FOREIGN KEY([IDVisit])
REFERENCES [dbo].[Visit] ([IDVisit])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Visit_Fields_Value] CHECK CONSTRAINT [FK_Visit_Fields_Value_Visit]
GO

ALTER TABLE [dbo].[Visit_Fields_Value]  WITH CHECK ADD  CONSTRAINT [FK_Visit_Fields_Value_Visit_Fields] FOREIGN KEY([IDField])
REFERENCES [dbo].[Visit_Fields] ([IDField])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Visit_Fields_Value] CHECK CONSTRAINT [FK_Visit_Fields_Value_Visit_Fields]
GO

--==============================================================================================================================

CREATE TABLE [dbo].[Visitor](
	[IDVisitor] [nvarchar](40) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_Visitor] PRIMARY KEY CLUSTERED 
(
	[IDVisitor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Visitor] ADD  CONSTRAINT [DF_Visitor_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO
--==============================================================================================================================

CREATE TABLE [dbo].[Visitor_Fields](
	[IDField] [nvarchar](40) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[required] [smallint] NOT NULL,
	[AskEvery] [int] NOT NULL,
	[position] [smallint] NOT NULL,
	[deleted] [bit] NOT NULL,
 CONSTRAINT [PK_Visitor_Fields] PRIMARY KEY CLUSTERED 
(
	[IDField] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Visitor_Fields] ADD  CONSTRAINT [DF_Visitor_Fields_deleted]  DEFAULT ((0)) FOR [deleted]
GO

--==============================================================================================================================
CREATE TABLE [dbo].[Visitor_Fields_Value](
	[IDField] [nvarchar](40) NOT NULL,
	[IDVisitor] [nvarchar](40) NOT NULL,
	[Value] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_Visits_Fields_Value] PRIMARY KEY CLUSTERED 
(
	[IDField] ASC,
	[IDVisitor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Visitor_Fields_Value]  WITH CHECK ADD  CONSTRAINT [FK_Visitor_Fields_Value_Visitor] FOREIGN KEY([IDVisitor])
REFERENCES [dbo].[Visitor] ([IDVisitor])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Visitor_Fields_Value] CHECK CONSTRAINT [FK_Visitor_Fields_Value_Visitor]
GO

ALTER TABLE [dbo].[Visitor_Fields_Value]  WITH CHECK ADD  CONSTRAINT [FK_Visitor_Fields_Value_Visitor_Fields] FOREIGN KEY([IDField])
REFERENCES [dbo].[Visitor_Fields] ([IDField])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Visitor_Fields_Value] CHECK CONSTRAINT [FK_Visitor_Fields_Value_Visitor_Fields]
GO

--==============================================================================================================================

CREATE TABLE [dbo].[Visit_Visitor](
	[IDVisit] [nvarchar](40) NOT NULL,
	[IDVisitor] [nvarchar](40) NOT NULL,
 CONSTRAINT [PK_Visit_Visitor] PRIMARY KEY CLUSTERED 
(
	[IDVisit] ASC,
	[IDVisitor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Visit_Visitor]  WITH CHECK ADD  CONSTRAINT [FK_Visit_Visitor_Visit_Visitor] FOREIGN KEY([IDVisitor])
REFERENCES [dbo].[Visitor] ([IDVisitor])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Visit_Visitor] CHECK CONSTRAINT [FK_Visit_Visitor_Visit_Visitor]
GO

ALTER TABLE [dbo].[Visit_Visitor]  WITH CHECK ADD  CONSTRAINT [FK_Visit_Visitor_Visits] FOREIGN KEY([IDVisit])
REFERENCES [dbo].[Visit] ([IDVisit])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Visit_Visitor] CHECK CONSTRAINT [FK_Visit_Visitor_Visits]
GO
--==============================================================================================================================

CREATE TABLE [dbo].[Visit_Visitor_Punch](
	[IDVisit] [nvarchar](40) NOT NULL,
	[IDVisitor] [nvarchar](40) NOT NULL,
	[DatePunch] [datetime] NOT NULL,
	[Action] [nvarchar](3) NOT NULL,
 CONSTRAINT [PK_Visit_Visitor_Punch] PRIMARY KEY CLUSTERED 
(
	[IDVisit] ASC,
	[IDVisitor] ASC,
	[DatePunch] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Visit_Visitor_Punch]  WITH CHECK ADD  CONSTRAINT [FK_Visit_Visitor_Punch_Visitor] FOREIGN KEY([IDVisitor])
REFERENCES [dbo].[Visitor] ([IDVisitor])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Visit_Visitor_Punch] CHECK CONSTRAINT [FK_Visit_Visitor_Punch_Visitor]
GO

ALTER TABLE [dbo].[Visit_Visitor_Punch]  WITH CHECK ADD  CONSTRAINT [FK_Visit_Visitor_Punch_Visits] FOREIGN KEY([IDVisit])
REFERENCES [dbo].[Visit] ([IDVisit])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Visit_Visitor_Punch] CHECK CONSTRAINT [FK_Visit_Visitor_Punch_Visits]
GO


CREATE VIEW [dbo].[sysrovwVisitDateList]
AS
SELECT        IDVisit, CONVERT(datetime, CONVERT(varchar(10), BeginDate, 120), 120) AS cBeginDate, CASE WHEN year(EndDate) = 1970 THEN dateadd(s, - 1, dateadd(d, 1, 
                         CONVERT(datetime, CONVERT(varchar(10), BeginDate, 120), 120))) ELSE dateadd(s, - 1, dateadd(d, 1, CONVERT(datetime, CONVERT(varchar(10), EndDate, 120), 120))) 
                         END AS cEndDate
FROM            dbo.Visit

GO


ALTER TABLE dbo.sysroSecurityParameters ADD
	BlockAccessVTVisits bit NULL
GO

ALTER TABLE dbo.sysroPassports ADD
	EnabledVTVisits bit NULL,
	EnabledVTVisitsApp bit NULL
GO

ALTER PROCEDURE [dbo].[WebLogin_Passports_SelectByIDUser]
(
   	@idUser int
)
AS
SELECT ID,
   	IDParentPassport,
   	GroupType,
   	Name,
   	Description,
   	Email,
   	IDUser,
   	IDEmployee,
   	IDLanguage,
   	LevelOfAuthority,
  	ConfData,
  	AuthenticationMerge,
  	StartDate,
  	ExpirationDate,
  	[State],
 	EnabledVTDesktop,
 	EnabledVTPortal,
 	EnabledVTSupervisor,
 	EnabledVTPortalApp ,
 	EnabledVTSupervisorApp ,
 	NeedValidationCode ,
 	TimeStampValidationCode ,
 	ValidationCode,
	EnabledVTVisits,
	EnabledVTVisitsApp
  		
FROM sysroPassports
WHERE IDUser = @idUser
   	
RETURN
GO

ALTER PROCEDURE [dbo].[WebLogin_Passports_SelectByIDEmployee] 
(
   	@idEmployee int
)
AS
SELECT ID,
   	IDParentPassport,
   	GroupType,
   	Name,
   	Description,
   	Email,
   	IDUser,
   	IDEmployee,
   	IDLanguage,
   	LevelOfAuthority,
  	ConfData,
  	AuthenticationMerge,
  	StartDate,
  	ExpirationDate,
  	[State],
 	EnabledVTDesktop,
 	EnabledVTPortal,
 	EnabledVTSupervisor,
 	EnabledVTPortalApp ,
 	EnabledVTSupervisorApp ,
 	NeedValidationCode ,
 	TimeStampValidationCode,
 	ValidationCode,
	EnabledVTVisits,
	EnabledVTVisitsApp
  		
FROM sysroPassports
WHERE IDEmployee = @idEmployee
   	
RETURN
GO

ALTER PROCEDURE [dbo].[WebLogin_Passports_SelectAllUser] 
   	
AS
SELECT ID,
   	IDParentPassport,
   	GroupType,
   	Name,
   	Description,
   	Email,
   	IDUser,
   	IDEmployee,
   	IDLanguage,
   	LevelOfAuthority,
  	ConfData,
  	AuthenticationMerge,
  	StartDate,
  	ExpirationDate,
  	[State],
 	EnabledVTDesktop,
 	EnabledVTPortal,
 	EnabledVTSupervisor,
 	EnabledVTPortalApp ,
 	EnabledVTSupervisorApp ,
 	NeedValidationCode ,
 	TimeStampValidationCode ,
 	ValidationCode,
	EnabledVTVisits,
	EnabledVTVisitsApp

FROM sysroPassports
WHERE IDUser IS NOT NULL
   	
RETURN
GO

ALTER PROCEDURE [dbo].[WebLogin_Passports_SelectAllEmployee] 
   	
AS
SELECT ID,
   	IDParentPassport,
   	GroupType,
   	Name,
   	Description,
   	Email,
   	IDUser,
   	IDEmployee,
   	IDLanguage,
   	LevelOfAuthority,
  	ConfData,
  	AuthenticationMerge,
  	StartDate,
  	ExpirationDate,
  	[State],
 	EnabledVTDesktop,
 	EnabledVTPortal,
 	EnabledVTSupervisor,
 	EnabledVTPortalApp ,
 	EnabledVTSupervisorApp ,
 	NeedValidationCode ,
 	TimeStampValidationCode ,
 	ValidationCode,
	EnabledVTVisits,
	EnabledVTVisitsApp
FROM sysroPassports
WHERE IDEmployee IS NOT NULL
   	
RETURN
GO


ALTER PROCEDURE [dbo].[WebLogin_Passports_SelectAll] 
   	
AS
SELECT ID,
   	IDParentPassport,
   	GroupType,
   	Name,
   	Description,
   	Email,
   	IDUser,
   	IDEmployee,
   	IDLanguage,
   	LevelOfAuthority,
  	ConfData,
  	AuthenticationMerge,
  	StartDate,
  	ExpirationDate,
  	[State],
 	EnabledVTDesktop,
 	EnabledVTPortal,
 	EnabledVTSupervisor,
 	EnabledVTPortalApp ,
 	EnabledVTSupervisorApp ,
 	NeedValidationCode ,
 	TimeStampValidationCode ,
 	ValidationCode,
	EnabledVTVisits,
	EnabledVTVisitsApp
FROM sysroPassports
   	
RETURN
GO

 ALTER PROCEDURE [dbo].[WebLogin_Passports_Select] 
 (
 	@idPassport int
 )
 AS
    SELECT ID,
    	IDParentPassport,
    	GroupType,
    	Name,
    	Description,
    	Email,
    	IDUser,
    	IDEmployee,
    	IDLanguage,
    	dbo.GetPassportLevelOfAuthority(@idPassport) AS LevelOfAuthority,
    	ConfData,
   	AuthenticationMerge,
   	StartDate,
   	ExpirationDate,
   	[State],
 	EnabledVTDesktop,
 	EnabledVTPortal,
 	EnabledVTSupervisor,
 	EnabledVTPortalApp ,
 	EnabledVTSupervisorApp ,
 	NeedValidationCode ,
 	TimeStampValidationCode,
 	ValidationCode,
	EnabledVTVisits,
	EnabledVTVisitsApp
    FROM sysroPassports
    WHERE ID = @idPassport
    	
RETURN
GO

 ALTER PROCEDURE [dbo].[WebLogin_Passports_Insert] 
    	(
    		@id int OUTPUT, @idParentPassport int, @groupType varchar(1), @name nvarchar(50), @description nvarchar(100), @email nvarchar(100), @idUser int, @idEmployee int, @idLanguage int,
    		@levelOfAuthority tinyint, @ConfData text, @AuthenticationMerge nvarchar(50), @StartDate smalldatetime, @ExpirationDate smalldatetime, @State smallint, @EnabledVTDesktop bit,
  		@EnabledVTPortal bit, @EnabledVTSupervisor bit, @EnabledVTPortalApp bit , @EnabledVTSupervisorApp bit , @NeedValidationCode bit , @TimeStampValidationCode smalldatetime ,
  		@ValidationCode nvarchar(100), @EnabledVTVisits bit, @EnabledVTVisitsApp bit
    	)
    AS
    	INSERT INTO sysroPassports (
    		IDParentPassport, GroupType, Name, Description, Email, IDUser, IDEmployee, IDLanguage, LevelOfAuthority, ConfData, AuthenticationMerge, StartDate, ExpirationDate, [State],
   		EnabledVTDesktop, EnabledVTPortal, EnabledVTSupervisor, EnabledVTPortalApp, EnabledVTSupervisorApp, NeedValidationCode, TimeStampValidationCode, ValidationCode, EnabledVTVisits, EnabledVTVisitsApp
    	)
    	VALUES (
    		@idParentPassport, @groupType, @name, @description, @email, @idUser, @idEmployee, @idLanguage, @levelOfAuthority, @ConfData, @AuthenticationMerge, @StartDate, @ExpirationDate,
   		@State, @EnabledVTDesktop, @EnabledVTPortal, @EnabledVTSupervisor, @EnabledVTPortalApp, @EnabledVTSupervisorApp, @NeedValidationCode, @TimeStampValidationCode, @ValidationCode, @EnabledVTVisits, @EnabledVTVisitsApp 
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
  (@id int,
  @idParentPassport int,
  @groupType varchar(1),
  @name nvarchar(50),
  @description nvarchar(4000),
  @email nvarchar(100),
  @idUser int,
  @idEmployee int,
  @idLanguage int,
  @levelOfAuthority tinyint,
  @ConfData text,
  @AuthenticationMerge nvarchar(50),
  @StartDate smalldatetime,
  @ExpirationDate smalldatetime,
  @State smallint,
  @EnabledVTDesktop bit,
  @EnabledVTPortal bit,
  @EnabledVTSupervisor bit,
  @EnabledVTPortalApp bit,
  @EnabledVTSupervisorApp bit,
  @NeedValidationCode bit,
  @TimeStampValidationCode smalldatetime,
  @ValidationCode  nvarchar(100),
  @EnabledVTVisits bit,
  @EnabledVTVisitsApp bit)
 AS
  IF @groupType <> 'U' 
  	BEGIN
  		SET @levelOfAuthority = NULL
  	END
  UPDATE sysroPassports SET
  	IDParentPassport = @idParentPassport,
  	GroupType = @groupType,
  	Name = @name,
  	Description = @description,
  	Email = @email,
  	IDUser = @idUser,
  	IDEmployee = @idEmployee,
  	IDLanguage = @idLanguage,
  	LevelOfAuthority = @levelOfAuthority,
  	ConfData = @ConfData,
  	AuthenticationMerge = @AuthenticationMerge,
  	StartDate = @StartDate,
  	ExpirationDate = @ExpirationDate,
  	[State] = @State, 
 	EnabledVTDesktop = @EnabledVTDesktop,
 	EnabledVTPortal = @EnabledVTPortal,
 	EnabledVTSupervisor = @EnabledVTSupervisor,
 	EnabledVTPortalApp = @EnabledVTPortalApp,
 	EnabledVTSupervisorApp = @EnabledVTSupervisorApp,
 	NeedValidationCode = @NeedValidationCode,
 	TimeStampValidationCode = @TimeStampValidationCode,
 	ValidationCode  = @ValidationCode, 
 	EnabledVTVisits = @EnabledVTVisits,
 	EnabledVTVisitsApp = @EnabledVTVisitsApp
  	
  WHERE ID = @id
 IF @groupType = 'U' 
   	BEGIN
 		-- 1 / Actualizar los permisos del grupo y sus hijos
 		IF NOT EXISTS ( SELECT ID FROM RequestPassportPermissionsPending WHERE IDAction = 1 AND IDObject = @id)
 			insert into RequestPassportPermissionsPending Values(1, @id)
   	END
   	 
  RETURN
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(31,null,'Visits','Visitas','','U','WA',0,null,null)
GO
 
UPDATE dbo.sysroParameters SET Data='358' WHERE ID='DBVersion'
GO